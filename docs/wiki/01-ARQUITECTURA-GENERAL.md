# 01 - Arquitectura General

> Este documento describe el ciclo de vida del servidor, el game loop, el modelo de estado,
> el sistema de timers, y la secuencia de inicializacion. Es el marco sobre el cual operan
> todos los demas sistemas del juego.

---

## 1. Vision General

El servidor de Argentum Online es una aplicacion **single-threaded** que gestiona todo el estado
del mundo del juego en un unico hilo principal. La comunicacion de red (TCP) usa operaciones
asincronas para aceptar conexiones y recibir datos, pero el procesamiento de esos datos se
serializa en el hilo principal a traves de una cola de mensajes.

El servidor es **autoritativo**: toda la logica del juego (combate, movimiento, inventario,
comercio, etc.) se ejecuta exclusivamente en el servidor. El cliente solo envia comandos
("quiero moverme al norte", "quiero atacar") y recibe actualizaciones de estado ("tu vida
ahora es 50", "el NPC se movio a la posicion X,Y").

---

## 2. Ciclo de Vida del Servidor

### 2.1 Secuencia de Arranque

El servidor sigue esta secuencia estricta al iniciar:

1. **Cargar configuracion principal** - Lee el archivo de configuracion del servidor que define:
   puerto de escucha, version del cliente requerida, maximo de usuarios, intervalos de juego,
   listas de administradores, y parametros generales.

2. **Cargar mensaje del dia (MOTD)** - Texto que se muestra a los jugadores al conectarse.

3. **Cargar IPs baneadas** - Lista de direcciones IP bloqueadas.

4. **Cargar datos de juego** - En este orden especifico:
   - Definiciones de NPCs (criaturas, comerciantes, guardias, etc.)
   - Definiciones de objetos (armas, armaduras, pociones, recursos, etc.)
   - Definiciones de hechizos (efectos, costos, requisitos)
   - Recetas de herreria (armas forjables)
   - Recetas de herreria (armaduras forjables)
   - Recetas de carpinteria
   - Datos de balance (modificadores de clase, raza, vida, party)
   - Armaduras faccionarias (items de recompensa por faccion)
   - Lista de criaturas invocables (para comandos GM)
   - Lista de nombres prohibidos
   - Coordenadas de las ciudades del mundo
   - Datos de apuestas

5. **Cargar mapas** - Se cargan todos los mapas del mundo secuencialmente. Cada mapa consiste en:
   - Datos de tiles (capas graficas, bloqueos)
   - Informacion de tiles (teleports, NPCs iniciales, objetos en el piso)
   - Metadatos (nombre, musica, tipo de zona, restricciones)
   - Sonidos ambientales

6. **Cargar base de datos de clanes** - Todos los clanes existentes con sus miembros.

7. **Inicializar sistema de areas** - Precalcula las areas de interes para optimizar el broadcast
   de red.

8. **Calcular matrix de distancias** - Genera una tabla de distancias entre cada mapa y cada
   ciudad usando BFS (Breadth-First Search) sobre los teleports de los mapas. Esto se usa para
   el sistema de "viaje a casa" (/home).

9. **Abrir el socket de escucha** - Comienza a aceptar conexiones TCP en el puerto configurado.

10. **Iniciar el game loop** - Entra en el bucle principal del juego.

### 2.2 Secuencia de Apagado

El servidor puede apagarse de forma controlada. Al hacerlo:

1. Se ejecuta un WorldSave (guardado completo del estado)
2. Se cierran todas las conexiones de clientes
3. Se cierra el socket de escucha

---

## 3. Game Loop

El game loop es el corazon del servidor. Es un bucle infinito que ejecuta distintos subsistemas
a intervalos especificos, usando un reloj de alta precision para medir el tiempo transcurrido.

### 3.1 Estructura del Loop

```
mientras el servidor este corriendo:
    tiempo_actual = reloj de alta precision

    si paso suficiente tiempo para [subsistema X]:
        ejecutar subsistema X
        registrar ultimo tiempo de ejecucion

    si paso suficiente tiempo para [subsistema Y]:
        ejecutar subsistema Y
        registrar ultimo tiempo de ejecucion

    ... (para cada subsistema)

    dormir 1 milisegundo (para no consumir 100% CPU)
```

### 3.2 Subsistemas del Game Loop

Los subsistemas se ejecutan a diferentes frecuencias segun su urgencia. De mayor a menor frecuencia:

#### Procesamiento de Red (cada 5 ms)

Lee la cola de mensajes de red y procesa cada mensaje segun su tipo:
- **Nueva conexion**: asigna un slot de usuario y prepara el buffer de comunicacion
- **Datos recibidos**: despacha los paquetes al handler de protocolo para su procesamiento
- **Desconexion**: ejecuta el flujo de logout y libera recursos

Este es el subsistema de mayor frecuencia porque la latencia de red percibida por el jugador
depende directamente de la rapidez con que se procesan sus comandos.

#### Envio de Datos (cada 10 ms)

Recorre todos los usuarios conectados y, si tienen datos pendientes en su buffer de salida,
los envia por el socket. Este subsistema existe separado del procesamiento porque permite
acumular multiples actualizaciones en un solo envio (batching), reduciendo la cantidad de
paquetes TCP y mejorando la eficiencia de red.

#### Timer Principal del Juego (cada 40 ms, ~25 ticks por segundo)

Este es el tick fundamental del juego. Define la "velocidad" a la que el mundo avanza.
Para cada jugador conectado y logueado, en cada tick se procesan:

**Efectos del tile actual:**
- Si esta sobre lava, recibe dano periodico (5% de su vida maxima)
- Si esta a la intemperie sin ropa en zona de nieve, recibe dano por frio
- Si esta a la intemperie sin ropa en otra zona, pierde stamina por frio

**Contadores de estados negativos:**
- Paralisis: se decrementa el contador. Al llegar a 0, el personaje se libera
- Ceguera: se decrementa. Al llegar a 0, se restaura la vision
- Estupidez: se decrementa. Al llegar a 0, se restaura el control

**Procesos vitales (solo si esta vivo):**
- Meditacion: si esta meditando, regenera mana progresivamente
- Veneno: si esta envenenado, recibe dano periodico (1-5 puntos). Puede morir por veneno
- Invisibilidad: se decrementa el timer. Al acabar, vuelve a ser visible
- Ocultamiento: se decrementa el timer. Al acabar, se revela
- Mimetismo: se decrementa. Al acabar, restaura su apariencia original
- Estado atacable PvP: verifica si paso el tiempo de gracia (60 segundos)
- Pociones de atributos: si tiene un buff temporal, decrementa duracion. Al acabar, restaura atributos
- Hambre: cada N ticks, reduce 10 puntos de hambre. Si llega a 0, activa flag de hambriento
- Sed: cada N ticks, reduce 10 puntos de sed. Si llega a 0, activa flag de sediento
- Regeneracion de vida y stamina: solo si no tiene hambre ni sed, y no esta lloviendo a la intemperie
  - Si esta descansando: regenera con intervalos cortos
  - Si no: regenera con intervalos largos
  - Cuando ambos estan al maximo, el descanso termina automaticamente
- Mascotas invocadas: se decrementa su tiempo de existencia. Si llega a 0, mueren

**Timeout de conexion:**
- Si hay un socket conectado pero el usuario no se logueo, se incrementa un contador de inactividad.
  Si supera el limite configurado, se cierra la conexion (proteccion contra conexiones fantasma).

**Flush de buffer:** Envia datos pendientes al cliente.

#### Inteligencia Artificial de NPCs (cada ~100 ms, configurable)

Recorre todos los NPCs activos en el mundo y ejecuta su logica de comportamiento:
- Movimiento (aleatorio, persecucion, pathfinding)
- Deteccion de jugadores en rango
- Decision de ataque (fisico o magico)
- Comportamiento especial segun tipo (guardia, mascota, pretoriano, etc.)

Los NPCs paralizados o inmovilizados no ejecutan movimiento pero pueden seguir detectando enemigos.
Los NPCs en mapas sin jugadores no ejecutan IA (optimizacion).

Ver documento [05-NPCs-E-IA.md](05-NPCs-E-IA.md) para el detalle completo.

#### Efectos de Lluvia (cada ~500 ms, configurable)

Si el clima actual es lluvioso, recorre todos los jugadores que estan "a la intemperie"
(no bajo techo, no en zona segura) y les reduce stamina. Los jugadores bajo techo o en zonas
seguras no se ven afectados.

#### Auditoria y Centinela (cada 1 segundo)

Ejecuta tres procesos:

1. **Centinela anti-bot**: cada 5 segundos, verifica jugadores que estan trabajando de forma
   repetitiva y les envia un desafio. Si no responden en 2 minutos, son baneados automaticamente.

2. **Sistema de desconexion voluntaria**: gestiona el countdown de 10 segundos cuando un jugador
   escribe /salir. Si el countdown llega a 0, desconecta al jugador.

3. **Estadisticas**: actualiza contadores de estado del servidor.

#### Ataque de NPCs (cada ~4 segundos, configurable)

Resetea el flag de "puede atacar" de todos los NPCs, habilitandolos para su proximo ciclo
de ataque. Esto controla la velocidad de ataque de las criaturas.

#### Efectos de Sonido Ambiental (cada 4 segundos)

Reproduce sonidos ambientales asociados a los mapas (aves, agua, viento, etc.) para los
jugadores presentes.

#### Ciclo de Mantenimiento Lento (cada 6 segundos)

Ejecuta procesos de mantenimiento de baja frecuencia:

1. **Anti-piquete**: si un jugador esta parado en un tile de tipo "anti-piquete" durante
   mas de ~2.3 minutos consecutivos, es encarcelado automaticamente por 10 minutos. Esto
   evita que los jugadores bloqueen pasos estrechos intencionalmente.

2. **Viaje a casa (/home)**: si un jugador muerto esta viajando a su ciudad de origen,
   decrementa el contador de viaje. Al llegar a 0 (despues de 30 segundos), lo teletransporta.
   Condicion: el jugador debe permanecer quieto durante el viaje; si se mueve, se cancela.

3. **Validacion de clanes**: verifica que los miembros de cada clan sigan cumpliendo los
   requisitos de alineacion. Si no, se ejecuta la logica de antifaccion.

4. **Flush de buffers de red**: envio final de datos pendientes.

#### Guardado Automatico y Mantenimiento Mayor (cada 1 minuto)

1. **Optimizacion de areas**: recalcula estadisticas de ocupacion por mapa.
2. **Centinela**: actualiza el ciclo del centinela anti-bot.
3. **WorldSave periodico**: cada N minutos (minimo 60, default 180), ejecuta un guardado
   completo del estado del mundo:
   - Guarda todos los personajes conectados
   - Guarda el estado de todos los foros
   - Respawnea guardias de ciudades
   - Hace backup de mapas marcados para backup
   - Hace backup de NPCs marcados para backup
4. **Cada 15 minutos**: respawn de guardias + limpieza de objetos temporales del mundo.
5. **Purga de penas**: reduce el timer de prision de los jugadores encarcelados. Al llegar a 0,
   los libera automaticamente.
6. **Verificacion de inactividad**: si un jugador supera el limite de inactividad configurado
   (en minutos), es desconectado automaticamente.
7. **Log de ocupacion**: registra cuantos usuarios hay conectados.

#### Clima (cada 1 minuto)

Decide si empieza o para de llover:
- Si no llueve y pasaron mas de 15 minutos desde la ultima lluvia: 2% de probabilidad por minuto
  de que empiece a llover
- Si no llueve y pasaron mas de 24 horas: lluvia obligatoria
- Si llueve y pasaron mas de 5 minutos: para obligatoriamente
- Mientras llueve: 2% de probabilidad por minuto de que pare antes

#### Limpieza de Logs (cada 1 minuto)

Limpia archivos temporales de log de PvP.

---

## 4. Modelo de Estado

### 4.1 Estado Global del Mundo

El servidor mantiene todo el estado del juego en memoria. Las entidades principales son:

#### Jugadores

Una tabla indexada por numero de slot (1 a MaxUsuarios). Cada entrada contiene toda la
informacion de un jugador conectado:

- **Identidad**: nombre, clase, raza, genero, ciudad de origen, descripcion
- **Apariencia**: cuerpo, cabeza, animacion de arma/escudo/casco, efectos visuales, direccion
- **Posicion**: mapa actual, coordenada X, coordenada Y
- **Stats vitales**: HP, mana, stamina, hambre, sed (actuales y maximos)
- **Atributos**: fuerza, agilidad, inteligencia, carisma, constitucion (actuales y base)
- **Combate**: dano min/max, defensa, nivel, experiencia
- **Skills**: 20 habilidades con su nivel, experiencia y umbral de subida
- **Hechizos**: hasta 35 hechizos aprendidos
- **Inventario**: hasta 30 slots + slots de equipamiento (arma, armadura, casco, escudo, anillo, municion, barco, mochila)
- **Banco**: hasta 40 slots de almacenamiento
- **Flags de estado**: mas de 50 flags booleanos que controlan el estado actual (muerto, envenenado, paralizado, invisible, comerciando, navegando, etc.)
- **Contadores temporales**: timestamps y counters para cooldowns, duraciones de efectos, timers de regeneracion
- **Reputacion**: 6 contadores (noble, burgues, plebe, ladron, bandido, asesino) + promedio
- **Faccion**: pertenencia a Ejercito Real o Legion Oscura, rangos, kills faccionarios
- **Social**: indice de clan, indice de party, mascotas activas (hasta 3)
- **Red**: buffers de entrada/salida, ID de conexion, clave de encriptacion

#### NPCs

Una tabla indexada por numero de slot (1 a 10,000). Cada entrada contiene:

- **Identidad**: nombre, numero de definicion, tipo (comun, guardia, comerciante, etc.)
- **Apariencia**: cuerpo, cabeza, animaciones
- **Posicion**: mapa, X, Y
- **Stats**: HP actual/maximo, dano min/max, defensa fisica, defensa magica, alineacion
- **Comportamiento**: tipo de IA, si es hostil, si es atacable, objetivo actual
- **Inventario**: para NPCs comerciantes, su inventario de items a la venta
- **Hechizos**: para NPCs magicos, su lista de hechizos
- **Drops**: hasta 5 items que suelta al morir, mas oro
- **Propiedades especiales**: veneno, domable, respawn, doble ataque, inmunidades
- **Pathfinding**: camino calculado, posicion objetivo
- **Dueno**: si es mascota, el indice del jugador que lo controla

#### Mapas

Una estructura tridimensional indexada por (numero_de_mapa, coordenada_x, coordenada_y).
Cada celda (tile) contiene:

- **Bloqueo**: si el tile es transitable o no
- **Graficos**: hasta 4 capas graficas (suelo, decoracion, techo, objetos ambientales)
- **Jugador presente**: indice del jugador parado en este tile (0 si vacio)
- **NPC presente**: indice del NPC parado en este tile (0 si vacio)
- **Objeto en el piso**: que item hay tirado y en que cantidad (0 si nada)
- **Salida/Teleport**: mapa y coordenadas destino (si es un portal)
- **Trigger**: efecto especial del tile (zona segura, anti-piquete, bajo techo, arena, etc.)

Ademas, cada mapa tiene metadatos globales:
- Nombre, musica ambiental
- Si es zona PK (combate entre jugadores permitido)
- Si la magia tiene efecto
- Si la invisibilidad funciona
- Si la resurreccion funciona
- Tipo de terreno (bosque, nieve, desierto, ciudad, campo, dungeon)
- Cantidad de jugadores presentes actualmente

#### Definiciones de Datos (solo lectura)

Estas estructuras se cargan al inicio y no cambian durante la ejecucion:

- **Definiciones de objetos**: para cada tipo de objeto del juego, toda su metadata
  (tipo, dano, defensa, valor, restricciones, efectos especiales, etc.)
- **Definiciones de hechizos**: para cada hechizo, sus efectos, costos, requisitos y targets
- **Definiciones de NPCs**: para cada tipo de NPC, sus stats base, comportamiento, drops e inventario
- **Recetas de crafting**: listas de items construibles con herreria y carpinteria
- **Modificadores de clase**: tablas de multiplicadores de combate por cada clase
- **Modificadores de raza**: tablas de modificadores de atributos por cada raza
- **Tabla de niveles**: experiencia requerida por nivel, skills desbloqueados por nivel
- **Coordenadas de ciudades**: posicion de spawn de cada ciudad
- **Matrix de distancias**: distancia en saltos de mapa entre cualquier mapa y cualquier ciudad

### 4.2 Exclusividad de Posicion

El mundo aplica una regla estricta: **un tile solo puede tener un jugador y/o un NPC a la vez**.
Esto significa:

- Dos jugadores no pueden ocupar el mismo tile
- Dos NPCs no pueden ocupar el mismo tile
- Un jugador y un NPC pueden coexistir en el mismo tile
- Al moverse, primero se verifica que el tile destino este libre
- Al loguearse, si la posicion guardada esta ocupada, se busca un tile adyacente libre

### 4.3 Inmutabilidad de la Grilla

La grilla del mapa (bloqueos, graficos, triggers, salidas) es **inmutable durante la ejecucion
normal**. Solo los siguientes elementos son mutables:

- Que jugador esta en cada tile
- Que NPC esta en cada tile
- Que objeto hay tirado en cada tile
- El estado de las puertas (abierta/cerrada)

Los graficos, bloqueos y triggers solo cambian por intervencion administrativa (comandos GM).

---

## 5. Sistema de Cooldowns

El servidor implementa un sistema de cooldowns (tiempos de espera entre acciones) basado en
timestamps. Cada accion tiene un timer que registra cuando se ejecuto por ultima vez. Para
ejecutar la accion de nuevo, debe haber pasado un intervalo minimo.

### 5.1 Cooldowns Principales

| Accion | Intervalo | Nota |
|--------|-----------|------|
| Atacar (melee) | Configurable | Frecuencia maxima de golpes fisicos |
| Disparar (arco) | Configurable | Frecuencia maxima de flechas |
| Lanzar hechizo | Configurable | Frecuencia maxima de casteo |
| Usar objeto | Configurable | Frecuencia maxima de uso de items |
| Trabajar | Configurable | Frecuencia maxima de acciones de oficio |

### 5.2 Cooldowns Cruzados

El sistema implementa **cooldowns cruzados** que impiden encadenar acciones de diferentes tipos
demasiado rapido. Esto es una medida anti-macro critica:

- **Magia a golpe**: despues de lanzar un hechizo, hay un tiempo de espera antes de poder
  atacar fisicamente. Al activarse, tambien resetea los timers de ataque y uso de objetos.
- **Golpe a magia**: despues de atacar fisicamente, hay un tiempo de espera antes de poder
  lanzar un hechizo. Al activarse, resetea los timers de golpe y magia.
- **Golpe a usar objeto**: despues de atacar, hay un tiempo de espera antes de poder usar
  un objeto (por ejemplo, tomar una pocion).

Estos cooldowns cruzados son fundamentales para el balance del PvP, ya que impiden que un
jugador haga "combos" de magia + golpe + pocion en secuencia instantanea.

### 5.3 Cooldowns de Estado

Algunos cooldowns no son por accion del jugador sino por estado del mundo:

- **Inmunidad post-spawn**: al loguearse o resucitar, el jugador es inmune a ataques de NPCs
  durante 5 segundos.
- **Estado atacable PvP**: cuando un ciudadano ataca a otro, queda en estado "atacable" durante
  60 segundos. La victima puede contraatacar sin consecuencias de reputacion durante ese tiempo.
- **Propiedad de NPC**: cuando un jugador ataca un NPC, se lo "apropia" durante 18 segundos.
  Otros jugadores no pueden atacar ese NPC mientras la propiedad este activa.

---

## 6. Sistema de Areas (Optimizacion de Broadcast)

### 6.1 Problema

En un servidor con cientos de jugadores distribuidos en cientos de mapas, enviar cada actualizacion
del mundo a todos los jugadores seria prohibitivamente costoso en ancho de banda. La mayoria de
las actualizaciones solo son relevantes para los jugadores que pueden ver la accion.

### 6.2 Solucion: Areas de Interes

Cada mapa de 100x100 tiles se divide en **areas** de 9x9 tiles. Esto da aproximadamente
11x11 = 121 areas por mapa.

El area de un tile se calcula como: `area_x = tile_x / 9`, `area_y = tile_y / 9`.

Cada jugador "escucha" las areas adyacentes a su posicion actual. Cuando ocurre un evento
en el mundo (un NPC se mueve, un jugador habla, se crea un efecto visual), el servidor
solo envia la notificacion a los jugadores que estan escuchando el area donde ocurrio el evento.

### 6.3 Cambio de Area

Cuando un jugador se mueve y cambia de area, se ejecuta un proceso de transicion:

1. Se calculan las nuevas areas que el jugador puede ver (una franja en la direccion del movimiento)
2. Se envian los datos de esas nuevas areas (NPCs, objetos en el piso, jugadores presentes)
3. Se deja de enviar datos de las areas que el jugador ya no puede ver

### 6.4 Ingreso a un Mapa

Cuando un jugador entra a un mapa (por teleport, login, o resurreccion), se le envia una
region completa de aproximadamente 27x27 tiles centrada en su posicion. Esto es equivalente
a enviarle 3x3 areas de golpe.

### 6.5 Targets de Envio

El sistema define multiples "targets" de envio segun quien debe recibir la informacion:

| Target | Destinatarios |
|--------|---------------|
| A todos | Todos los jugadores conectados al servidor |
| A mapa | Todos los jugadores en un mapa especifico |
| A area del jugador | Jugadores que pueden ver al jugador emisor |
| A area del NPC | Jugadores que pueden ver al NPC |
| A administradores | Solo GMs conectados |
| A miembros del clan | Miembros online de un clan especifico |
| A miembros de la party | Miembros del grupo del emisor |
| A faccion Real | Miembros del Ejercito Real |
| A faccion Caos | Miembros de la Legion Oscura |
| A area de muertos | Solo fantasmas (jugadores muertos) en el area |

### 6.6 Auto-optimizacion

El sistema registra estadisticas de cuantos jugadores hay en cada mapa, por dia de la semana
y franja horaria (cada 3 horas). Esto permite pre-dimensionar las estructuras de datos y
anticipar la carga.

---

## 7. Comunicacion de Red

### 7.1 Modelo de Conexion

El servidor escucha en un unico puerto TCP. Cada cliente establece una conexion TCP persistente
que se mantiene durante toda la sesion de juego.

Los datos se transmiten como un **stream binario** sin delimitadores de paquete a nivel de framing.
Cada paquete comienza con un byte que identifica el tipo de mensaje, seguido de los campos del
mensaje serializados secuencialmente (enteros de 16/32 bits, bytes, strings con prefijo de longitud).

### 7.2 Flujo de Datos

```
Cliente                          Servidor
  │                                 │
  │──── Conectar TCP ──────────────>│
  │                                 │
  │<─── Asignar slot ──────────────│
  │                                 │
  │──── Enviar version + hash ─────>│
  │                                 │
  │     (validar version)           │
  │                                 │
  │──── Login o Crear personaje ──>│
  │                                 │
  │<─── Datos del mundo ───────────│
  │     (mapa, personajes,          │
  │      inventario, stats)         │
  │                                 │
  │──── Comandos de juego ────────>│
  │<─── Actualizaciones ──────────│
  │     (en bucle continuo)         │
  │                                 │
  │──── Desconexion ──────────────>│
  │     (o timeout)                 │
```

### 7.3 Procesamiento de Paquetes Entrantes

Cada usuario tiene un **buffer de entrada**. Los datos recibidos por el socket se agregan a este
buffer. En cada tick de procesamiento de red (cada 5 ms), el servidor lee el buffer de cada
usuario y despacha los paquetes completos al handler correspondiente.

El handler es esencialmente un **gran despacho por tipo de paquete**: lee el primer byte del
buffer, identifica que tipo de mensaje es, lee los campos correspondientes, ejecuta la logica
asociada, y luego pasa al siguiente paquete en el buffer.

### 7.4 Procesamiento de Paquetes Salientes

El servidor escribe los datos de salida en un **buffer de salida** por usuario. Este buffer
se envia por el socket cada 10 ms (en el tick de envio de datos). Esto permite acumular
multiples actualizaciones en un solo envio TCP, reduciendo overhead.

### 7.5 Tipos de Mensajes

El protocolo define dos conjuntos de mensajes:

- **~128 tipos de mensajes del cliente**: login, movimiento, ataque, hablar, comercio,
  usar objeto, lanzar hechizo, comandos de clan, comandos de party, comandos GM, etc.

- **~108 tipos de mensajes del servidor**: crear/mover/destruir personajes, actualizar stats,
  mensajes de consola, efectos visuales, sonidos, datos de inventario, datos de comercio, etc.

El detalle completo esta en [13-PROTOCOLO-Y-RED.md](13-PROTOCOLO-Y-RED.md).

---

## 8. Persistencia

### 8.1 Archivos de Personaje

Cada personaje se guarda en un archivo individual en formato clave-valor (tipo INI).
El archivo contiene absolutamente todo el estado del personaje: stats, inventario, banco,
equipamiento, reputacion, faccion, flags, mascotas, hechizos, etc.

El guardado ocurre en tres momentos:
- Al desconectarse el jugador (inmediato)
- Durante el WorldSave periodico (para todos los conectados)
- Al ejecutar ciertos comandos administrativos

### 8.2 WorldSave

El WorldSave es un proceso periodico (cada 60-180 minutos, configurable) que guarda el estado
completo del mundo:

1. Guarda todos los personajes conectados
2. Guarda los foros in-game
3. Hace backup de los mapas marcados para backup (estado de puertas, objetos en el piso, NPCs)
4. Hace backup de NPCs con flag de backup
5. Respawnea guardias de ciudades
6. Registra en log

### 8.3 Archivos de Datos del Juego

Los datos del juego (definiciones de NPCs, objetos, hechizos, recetas, etc.) se almacenan
en archivos de formato clave-valor (tipo INI/DAT). Estos archivos se cargan una sola vez
al iniciar el servidor y no se modifican durante la ejecucion normal.

Los mapas se almacenan como archivos binarios (datos de tiles) con un complemento JSON
(metadatos). Se cargan al inicio y los datos de tiles se mantienen en memoria durante toda
la ejecucion.

---

## 9. Intervalos y Constantes de Tiempo

El servidor define docenas de intervalos configurables que controlan la velocidad de cada
aspecto del juego. Todos los intervalos se definen en el archivo de configuracion principal.

### 9.1 Intervalos de Regeneracion

| Parametro | Unidad | Descripcion |
|-----------|--------|-------------|
| Regeneracion HP (descansando) | Ticks de 40ms | Ciclos entre cada punto de HP regenerado mientras descansa |
| Regeneracion HP (normal) | Ticks de 40ms | Ciclos entre cada punto de HP regenerado sin descansar |
| Regeneracion Stamina (descansando) | Ticks de 40ms | Ciclos entre cada punto de STA regenerado mientras descansa |
| Regeneracion Stamina (normal) | Ticks de 40ms | Ciclos entre cada punto de STA regenerado sin descansar |

### 9.2 Intervalos de Necesidades

| Parametro | Unidad | Descripcion |
|-----------|--------|-------------|
| Hambre | Ticks de 40ms | Ciclos entre cada reduccion de 10 puntos de hambre |
| Sed | Ticks de 40ms | Ciclos entre cada reduccion de 10 puntos de sed |

### 9.3 Intervalos de Efectos

| Parametro | Unidad | Descripcion |
|-----------|--------|-------------|
| Veneno | Ticks de 40ms | Ciclos entre cada tick de dano por veneno |
| Paralisis | Ticks de 40ms | Duracion total de la paralisis |
| Invisibilidad | Ticks de 40ms | Duracion base de la invisibilidad por hechizo |
| Ocultamiento | Ticks de 40ms | Duracion base de la invisibilidad por skill |
| Frio | Ticks de 40ms | Ciclos entre cada tick de dano por frio/lava |
| Invocacion | Ticks | Duracion de las mascotas invocadas |

### 9.4 Intervalos de Acciones

| Parametro | Unidad | Descripcion |
|-----------|--------|-------------|
| Ataque melee | Milisegundos | Cooldown minimo entre ataques fisicos |
| Disparo de flechas | Milisegundos | Cooldown minimo entre disparos de arco |
| Lanzar hechizo | Milisegundos | Cooldown minimo entre hechizos |
| Usar objeto | Milisegundos | Cooldown minimo entre uso de items |
| Trabajar | Milisegundos | Cooldown minimo entre acciones de oficio |
| Magia a golpe | Milisegundos | Espera obligatoria despues de castear antes de golpear |
| Golpe a magia | Milisegundos | Espera obligatoria despues de golpear antes de castear |
| Golpe a usar | Milisegundos | Espera obligatoria despues de golpear antes de usar item |

### 9.5 Intervalos de Sistema

| Parametro | Unidad | Descripcion |
|-----------|--------|-------------|
| IA de NPCs | Milisegundos | Frecuencia de actualizacion de la IA |
| Ataque de NPCs | Milisegundos | Ciclo de reset de ataque de NPCs |
| Perdida de STA por lluvia | Milisegundos | Frecuencia de perdida de stamina bajo lluvia |
| WorldSave | Minutos | Intervalo entre guardados automaticos (min 60, default 180) |
| Timeout de conexion | Milisegundos | Tiempo maximo para que un socket sin login sea cerrado |
| Cierre de conexion | Milisegundos | Timeout al cerrar una conexion existente |

### 9.6 Intervalos Fijos (no configurables)

| Parametro | Valor | Descripcion |
|-----------|-------|-------------|
| Inmunidad post-spawn | 5,000 ms | Proteccion contra NPCs al loguearse/resucitar |
| Estado atacable PvP | 60,000 ms | Duracion del estado "atacable" despues de agredir |
| Propiedad de NPC | 18,000 ms | Duracion de la "propiedad" sobre un NPC atacado |

---

## 10. Consideraciones para la Reimplementacion

Estos son puntos arquitectonicos clave a tener en cuenta al disenar el nuevo servidor:

### 10.1 Single-threaded vs Multi-threaded

El servidor original es completamente single-threaded. Esto simplifica enormemente la logica
(no hay race conditions, no hay locks, no hay deadlocks) pero limita el rendimiento a un
solo nucleo de CPU. Para un servidor moderno, considerar:

- Mantener single-threaded para la logica del juego (es mas seguro y predecible)
- Usar threads separados para I/O de red y persistencia
- O usar un modelo de actores / event loop moderno (como en game servers actuales)

### 10.2 Estado en Memoria vs Base de Datos

El servidor original mantiene todo en memoria y persiste a archivos planos. Considerar:
- Mantener el estado del juego en memoria (es necesario para latencia baja)
- Usar una base de datos para persistencia de personajes (en lugar de archivos INI)
- Separar claramente el estado mutable (runtime) del estado persistido (disco)

### 10.3 Protocolo Binario

El protocolo original es un formato binario custom muy eficiente pero fragil (sin versionado,
sin checksums, sin compresion). Considerar:
- Mantener protocolo binario para eficiencia (el cliente lo requiere para compatibilidad)
- Agregar versionado de protocolo
- Considerar compresion para paquetes grandes (datos de mapa al entrar)

### 10.4 Separacion de Responsabilidades

El servidor original mezcla completamente la logica de juego con la serializacion de red,
la persistencia a disco, y el envio de mensajes. Una reimplementacion deberia separar:
- **Capa de red**: acepta conexiones, serializa/deserializa paquetes
- **Capa de logica de juego**: reglas, formulas, validaciones
- **Capa de estado**: gestion del estado del mundo
- **Capa de persistencia**: guardado/carga de datos
- **Capa de broadcast**: decision de a quien enviar cada actualizacion

### 10.5 Configuracion Externalizada

Todos los intervalos, limites, y constantes de balance deben ser configurables sin recompilar.
El servidor original ya hace esto parcialmente (via Server.ini y Balance.dat), pero muchas
constantes estan hardcodeadas. La reimplementacion deberia externalizar TODAS las constantes
de balance y gameplay.
