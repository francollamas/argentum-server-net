# 12 - Mundo y Mapas

> Este documento describe la estructura del mundo de Argentum Online: como se componen
> los mapas, el sistema de tiles, los tipos de trigger, los teleports y transiciones,
> el sistema de areas de interes (optimizacion de red), el clima, y la navegacion del mundo.

---

## 1. Estructura del Mundo

### 1.1 Composicion

El mundo esta compuesto por aproximadamente **290 mapas** numerados secuencialmente.
Cada mapa es una grilla independiente de **100 x 100 tiles**, donde cada tile ocupa
32 x 32 pixeles en la pantalla del cliente.

Los mapas estan interconectados por **teleports** (portales) ubicados en los bordes
o en posiciones especiales dentro de cada mapa.

### 1.2 Coordenadas

La posicion de cualquier entidad en el mundo se define con tres valores:
- **Mapa**: numero del mapa (1 a ~290)
- **X**: coordenada horizontal dentro del mapa (1 a 100)
- **Y**: coordenada vertical dentro del mapa (1 a 100)

### 1.3 Ventana Visible

El cliente muestra una ventana de **17 x 13 tiles** centrada en el jugador. Esto es lo que
el jugador puede ver en su pantalla en cualquier momento.

---

## 2. Anatomia de un Tile

Cada tile (celda) del mapa contiene la siguiente informacion:

### 2.1 Datos Estaticos (definidos al cargar el mapa)

| Dato | Descripcion |
|------|-------------|
| Bloqueado | Si el tile es transitable o no (muro, roca, agua sin barco, etc.) |
| Graficos (4 capas) | Hasta 4 capas de graficos superpuestas: suelo, decoracion baja, decoracion alta, techo |
| Trigger | Efecto especial del tile (zona segura, bajo techo, arena, etc.) |
| Salida/Teleport | Si pisar este tile teletransporta al jugador (destino: mapa, X, Y) |

### 2.2 Datos Dinamicos (cambian durante la ejecucion)

| Dato | Descripcion |
|------|-------------|
| Jugador presente | Indice del jugador parado en este tile (0 = vacio) |
| NPC presente | Indice del NPC parado en este tile (0 = vacio) |
| Objeto en el piso | Que item hay tirado (indice de objeto + cantidad, 0 = nada) |
| Estado de puerta | Para tiles con puerta: abierta o cerrada |

### 2.3 Regla de Exclusividad

- **Un tile solo puede tener un jugador a la vez**
- **Un tile solo puede tener un NPC a la vez**
- Un jugador y un NPC **pueden coexistir** en el mismo tile
- **Un tile solo puede tener un tipo de objeto** en el piso (pero con cantidad variable)

---

## 3. Tipos de Trigger

Los triggers son efectos especiales asociados a tiles que modifican las reglas del juego
en esa posicion.

### 3.1 Sin Efecto (Trigger 0)

Tile normal sin efecto especial. Es el valor por defecto.

### 3.2 Bajo Techo (Trigger 1)

El personaje esta bajo techo. Efectos:
- **Protegido de la lluvia**: no pierde stamina cuando llueve
- **Protegido del frio**: no recibe dano por estar desnudo en zona de nieve
- Afecta la visibilidad/clima percibido por el cliente

### 3.3 Trigger 2

Uso interno similar al trigger 1. Tambien protege de la intemperie (lluvia y frio).

### 3.4 Posicion Invalida (Trigger 3)

Los NPCs **no pueden pisar** estos tiles. Se usa para restringir el movimiento de criaturas
sin bloquear el paso a los jugadores. Util para evitar que los NPCs entren en zonas donde
no deberian estar (edificios, areas restringidas).

Los NPCs tampoco pueden respawnear en tiles con este trigger.

### 3.5 Zona Segura (Trigger 4)

El tile es una zona protegida. Efectos multiples:
- **No se puede atacar** a ningun jugador
- **No se puede ser atacado** por ningun jugador
- **No se puede robar**
- **No se puede domar criaturas** (las mascotas "esperan afuera")
- **No se pueden invocar criaturas**
- Protegido de la intemperie (lluvia y frio)

Las zonas seguras son tipicamente ciudades, templos, y areas de servicio.

### 3.6 Anti-piquete (Trigger 5)

Penaliza a los jugadores que permanecen demasiado tiempo en el tile:
- Se incrementa un contador cada 6 segundos
- Despues de **~2.3 minutos** continuos (23 ciclos), el jugador es **encarcelado automaticamente**
  por 10 minutos
- Se usa en pasillos estrechos y puntos de acceso criticos para evitar que los jugadores
  bloqueen intencionalmente el paso a otros

### 3.7 Zona de Pelea / Arena (Trigger 6)

Zona de combate especial sin consecuencias:
- Se puede pelear libremente (PvP permitido)
- **No se pierden items al morir** (el inventario no cae al piso)
- **No cambia la reputacion** al matar (no se vuelve criminal)
- **No se activa el seguro de resurreccion**
- Ideal para PvP competitivo, duelos, y entrenamiento

El trigger 6 puede tener sub-configuraciones:
- Permite pelea PvP
- Prohibe pelea PvP
- No aplica (ausente)

---

## 4. Teleports y Transiciones

### 4.1 Teleports de Mapa

Las salidas de mapa estan definidas tile a tile. Cada tile de salida especifica:
- **Mapa destino**: a que mapa se teletransporta
- **X destino**: coordenada X en el mapa destino
- **Y destino**: coordenada Y en el mapa destino

Cuando un jugador pisa un tile con salida, es trasladado **instantaneamente** al destino.

### 4.2 Objetos Teleport

Ademas de las salidas definidas en el mapa, existen objetos de tipo teleport que pueden
tener un **radio de dispersion**: al usarlos, el jugador aparece en una posicion aleatoria
dentro del radio, no en un punto fijo. Esto se usa para teleports que llevan a zonas abiertas.

### 4.3 Transicion de Mapa

Cuando un jugador cambia de mapa:
1. Se elimina su presencia visual del mapa anterior (notifica a jugadores cercanos)
2. Se actualiza su posicion al nuevo mapa
3. Se envia al cliente los datos del nuevo mapa (musica, terreno, tiles visibles)
4. Se crea su presencia visual en el nuevo mapa (notifica a jugadores cercanos)
5. Se recalculan las areas de interes

### 4.4 Matrix de Distancias

El servidor precalcula una **matrix de distancias** entre cada mapa y cada ciudad usando
BFS (busqueda en amplitud) sobre los teleports de los mapas. Esto permite calcular
instantaneamente cuantos "saltos" de mapa hay entre cualquier posicion y cualquier ciudad.

Se usa para el sistema de **viaje a casa** (/home): la duracion del viaje es proporcional
a la distancia en saltos de mapa.

---

## 5. Metadatos de Mapa

Cada mapa tiene metadatos globales que afectan todo lo que ocurre en el:

| Metadato | Tipo | Descripcion |
|----------|------|-------------|
| Nombre | Texto | Nombre del mapa (ej: "Ciudad de Ullathorpe") |
| Musica | Numero | Indice de la musica ambiental |
| PK | Booleano | Si es zona PK (combate entre jugadores permitido) |
| Magia sin efecto | Booleano | Si la magia no funciona en este mapa |
| Invisibilidad sin efecto | Booleano | Si la invisibilidad no funciona |
| Resurreccion sin efecto | Booleano | Si la resurreccion por hechizo no funciona |
| Terreno | Texto | Tipo de terreno (ver seccion 5.1) |
| Zona | Texto | Zona logica a la que pertenece |
| Restringir | Texto | Restricciones de acceso al mapa |
| Robo a NPC | Booleano | Si se permite robar a NPCs en este mapa |
| Backup | Booleano | Si el mapa se guarda durante el WorldSave |
| Usuarios activos | Contador | Cantidad de jugadores actualmente en el mapa (runtime) |

### 5.1 Tipos de Terreno

| Terreno | Efecto |
|---------|--------|
| BOSQUE | Terreno estandar |
| NIEVE | Dano por frio si el jugador esta desnudo a la intemperie |
| DESIERTO | Terreno estandar con clima calido |
| CIUDAD | Zona urbana, tipicamente con zonas seguras |
| CAMPO | Terreno abierto estandar |
| DUNGEON | Interior de mazmorra |

### 5.2 Mapas Especiales

| Mapa | Funcion |
|------|---------|
| Mapa 49 | Mapa exclusivo de Game Masters |
| Mapa 66 | Prision (posicion 75,47 = celda, posicion 75,65 = libertad) |
| Mapa configurable | Mapa del evento pretoriano |
| Mapas 58, 59, 60 | Ciudades de la Armada Real (combate habilitado para rangos altos) |
| Mapas 151, 156 | Ciudades de la Legion Oscura (combate habilitado para rangos altos) |

---

## 6. Sistema de Areas de Interes

### 6.1 Proposito

El sistema de areas es una optimizacion critica de ancho de banda. En lugar de enviar cada
actualizacion del mundo a todos los jugadores conectados, el servidor solo envia datos a
los jugadores que estan lo suficientemente cerca para ver la accion.

### 6.2 Division en Areas

Cada mapa de 100x100 tiles se divide en areas de **9x9 tiles**:
```
AreaX = TileX / 9
AreaY = TileY / 9
```

Esto da aproximadamente 11x11 = 121 areas por mapa.

### 6.3 Que Escucha Cada Jugador

Cada jugador "escucha" las areas adyacentes a su posicion actual. La informacion de las
areas se mantiene con bitmasks para determinar rapidamente que areas son relevantes para
cada jugador.

### 6.4 Cambio de Area

Cuando un jugador se mueve y cruza el limite de un area:

1. Se calculan las **nuevas areas visibles** (una franja de 9 tiles en la direccion del movimiento)
2. Se envian los datos de esas nuevas areas al cliente:
   - NPCs presentes en esas areas
   - Objetos en el piso
   - Otros jugadores presentes
3. Las areas que el jugador dejo de ver dejan de enviarle datos

### 6.5 Ingreso a un Mapa

Cuando un jugador entra a un mapa (por teleport, login, o resurreccion):
- Se le envia una region completa de aproximadamente **27x27 tiles** centrada en su posicion
- Esto equivale a 3x3 areas (9 areas completas)

### 6.6 Targets de Envio

El sistema define multiples targets para el envio de datos:

| Target | Destinatarios |
|--------|---------------|
| A todos (ToAll) | Todos los jugadores del servidor |
| A mapa (toMap) | Todos los jugadores en un mapa especifico |
| A area del jugador (ToPCArea) | Jugadores en areas adyacentes al jugador emisor |
| A area del NPC (ToNPCArea) | Jugadores en areas adyacentes al NPC |
| A administradores (ToAdmins/ToGM) | Solo GMs conectados |
| A miembros del clan (ToDiosesYclan) | GMs escuchando + miembros online del clan |
| A faccion Real (ToReal) | Miembros del Ejercito Real |
| A faccion Caos (ToCaos) | Miembros de la Legion Oscura |
| A muertos en area (ToDeadArea) | Solo fantasmas en el area |
| A miembros de party (ToPartyArea) | Miembros del grupo en el area |

### 6.7 Auto-optimizacion

El sistema registra estadisticas de ocupacion por mapa:
- Por **dia de la semana** (lunes a domingo)
- Por **franja horaria** (cada 3 horas = 8 franjas diarias)

Esto permite pre-dimensionar arrays de conexion y anticipar la carga de red por zona.

---

## 7. Clima

### 7.1 Estado de Lluvia

El servidor mantiene un estado global de lluvia (activa o inactiva). El clima afecta a
todos los mapas simultaneamente.

**Decision de lluvia (cada 1 minuto):**

| Condicion | Accion |
|-----------|--------|
| No llueve, pasaron < 15 min desde la ultima lluvia | No cambia |
| No llueve, pasaron >= 15 min | 2% de probabilidad por minuto de empezar |
| No llueve, pasaron >= 24 horas | Lluvia obligatoria |
| Llueve, pasaron < 5 min | Sigue lloviendo |
| Llueve, pasaron >= 5 min | Para obligatoriamente |
| Mientras llueve (antes de los 5 min) | 2% de probabilidad por minuto de parar |

**Efectos de la lluvia:**
- Los jugadores **a la intemperie** (no bajo techo, no en zona segura) pierden stamina
  periodicamente
- La lluvia impide la regeneracion de HP y stamina para jugadores a la intemperie
- Afecta visualmente al cliente (efecto de lluvia)

### 7.2 Ciclo Dia/Noche

El servidor mantiene un estado global de dia/noche que afecta la presentacion visual
del cliente. No tiene efectos mecanicos significativos en el gameplay.

---

## 8. Archivos de Mapa

### 8.1 Formato

Cada mapa se almacena en multiples archivos:

**Archivo binario principal (.map):**
Contiene los datos de tiles en formato compacto:
- Contadores de cada tipo de dato
- Arrays de objetos (posicion + indice + cantidad)
- Arrays de triggers (posicion + numero de trigger)
- Arrays de graficos de 4 capas (posicion + numero de grafico por capa)
- Arrays de NPCs iniciales (posicion + numero de NPC)
- Arrays de salidas/teleports (posicion origen + mapa/X/Y destino)
- Arrays de bloqueos (posiciones bloqueadas)

**Archivo de metadatos (.json):**
```
{
    "name": "Ciudad de Ullathorpe",
    "musicnum": 4,
    "magiasinefecto": 0,
    "terreno": "BOSQUE",
    "zona": "CIUDAD",
    "restringir": "No",
    "backup": 1,
    "pk": 1,
    "sonidos": []
}
```

### 8.2 Carga al Inicio

Todos los mapas se cargan secuencialmente al iniciar el servidor. Los datos se mantienen
en memoria durante toda la ejecucion. La estructura en memoria es una matrix tridimensional:
`MapData[mapa][x][y]` que permite acceso directo O(1) a cualquier tile.

### 8.3 Persistencia de Mapas

Los mapas marcados con `backup = 1` se guardan durante el WorldSave. Esto persiste:
- El estado de las puertas (abierta/cerrada)
- Los objetos en el piso
- Los NPCs que fueron spawneados

Los mapas sin backup no guardan estado y vuelven a su configuracion original al reiniciar
el servidor.

---

## 9. Sonidos Ambientales

Los mapas pueden tener sonidos ambientales asociados. Cada sonido tiene:
- Un numero de efecto de sonido
- Una posicion (X, Y) en el mapa donde se origina
- Se reproducen periodicamente (cada ~4 segundos) para los jugadores en el area

Los sonidos tipicos incluyen: aves, agua corriendo, viento, fuego, maquinaria, etc.

---

## 10. Movimiento en el Mundo

### 10.1 Movimiento del Jugador

El jugador se mueve tile a tile en las 4 direcciones cardinales (norte, sur, este, oeste).
Para cada intento de movimiento:

1. Se calcula el tile destino
2. Se verifica que el tile no este bloqueado
3. Se verifica que no haya otro jugador en el tile
4. Si el destino tiene teleport, se ejecuta la transicion de mapa
5. Se actualiza la posicion del jugador
6. Se notifica a los jugadores cercanos del movimiento
7. Si cambio de area, se ejecuta la logica de cambio de area

### 10.2 Movimiento de NPCs

Los NPCs se mueven con la misma logica basica pero con verificaciones adicionales:
- Validacion de terreno (agua/tierra segun el tipo de NPC)
- No pueden pisar tiles con trigger de posicion invalida (trigger 3)
- Pueden "empujar" fantasmas (jugadores muertos) al moverse a su tile

### 10.3 Agua y Tierra

El mundo tiene tiles de agua y tiles de tierra. Las reglas de movimiento dependen del tipo:

| Entidad | Tierra | Agua |
|---------|--------|------|
| Jugador a pie | Puede | No puede |
| Jugador en barco | No puede | Puede |
| NPC terrestre | Puede | No puede |
| NPC acuatico (AguaValida) | No puede (TierraInvalida) | Puede |
| NPC anfibio (AguaValida sin TierraInvalida) | Puede | Puede |

---

## 11. Relacion con Otros Sistemas

### 11.1 Combate
- Los triggers de zona segura y arena modifican las reglas de PvP
- El terreno de nieve causa dano por frio
- La lava causa dano periodico

### 11.2 NPCs
- Los NPCs se spawnean en posiciones definidas en el mapa
- El trigger de posicion invalida restringe el movimiento de NPCs
- El sistema de areas determina que jugadores ven a que NPCs

### 11.3 Protocolo
- El sistema de areas optimiza que datos se envian a cada jugador
- Las transiciones de mapa envian paquetes completos de datos
- Los cambios de area envian franjas incrementales

### 11.4 Oficios
- Los yacimientos, arboles, fraguas y yunques son objetos del mapa
- La pesca requiere tiles de agua adyacentes
- La navegacion depende de tiles de agua
