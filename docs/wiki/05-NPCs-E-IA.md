# 05 - NPCs e Inteligencia Artificial

> Este documento describe el sistema de NPCs (Non-Player Characters): tipos funcionales,
> comportamientos de inteligencia artificial, sistema de aggro, mascotas y control de criaturas,
> respawn, sistema de loot y drops, pathfinding, y la IA cooperativa avanzada del evento pretoriano.

---

## 1. Vision General

Los NPCs son todas las entidades del mundo que no son controladas por jugadores: criaturas
hostiles, guardias de ciudad, comerciantes, sacerdotes, mascotas invocadas, entrenadores, etc.

Cada NPC tiene un **tipo funcional** que determina como interactua con el mundo, y un **tipo
de IA** que determina como se mueve y toma decisiones. Estos dos conceptos son independientes:
un NPC de tipo "comun" puede tener IA de "movimiento aleatorio" o de "persecucion activa".

El servidor soporta hasta **10,000 NPCs simultaneos** incluyendo criaturas, guardias, mascotas,
NPCs de servicio, etc.

---

## 2. Tipos Funcionales de NPC

### 2.1 Comun (Tipo 0)

Criatura generica del mundo. Puede ser hostil o pasiva segun su configuracion. Incluye:
animales, monstruos, criaturas de dungeon, bosses, etc.

### 2.2 Revividor / Sacerdote (Tipo 1)

NPC que resucita jugadores muertos. Al interactuar con un fantasma (jugador muerto), lo revive
automaticamente. Es el metodo principal de resurreccion sin depender de otro jugador.

### 2.3 Guardia Real (Tipo 2)

Guardia de la faccion Real. Protege ciudades atacando a criminales. Detalles:
- Escanea las 4 direcciones adyacentes buscando criminales
- Ataca automaticamente a cualquier criminal que encuentre
- Tambien ataca a quien lo haya agredido directamente (sin importar alineacion)
- Se respawnea periodicamente durante el WorldSave

### 2.4 Entrenador (Tipo 3)

NPC que permite al jugador domar criaturas como mascotas. Puede tener hasta 7 tipos de
criaturas disponibles para domar.

### 2.5 Banquero (Tipo 4)

NPC que gestiona el banco del jugador. Al interactuar, abre la interfaz de deposito/retiro
(ver documento 08-COMERCIO-Y-ECONOMIA.md).

### 2.6 Noble (Tipo 5)

NPC de faccion noble. Otorga reputacion y gestion faccionaria.

### 2.7 Dragon (Tipo 6)

Criatura especial de alto nivel. Tiene interaccion unica con la Espada Matadragones:
- La espada inflige dano devastador contra el dragon (MinHP + Defensa = muerte de un golpe)
- La espada se destruye al matar al dragon
- Los dragones pueden contraatacar con magia si son atacados por elementales de fuego

### 2.8 Timbero (Tipo 7)

NPC de apuestas y juegos de azar. Gestiona el sistema de apuestas in-game.

### 2.9 Guardia del Caos (Tipo 8)

Equivalente del Guardia Real pero para la faccion del Caos:
- Ataca a ciudadanos (no-criminales) en lugar de criminales
- Tambien ataca a quien lo haya agredido si es criminal
- Se respawnea periodicamente

### 2.10 Resucitador Newbie (Tipo 9)

Version limitada del sacerdote: solo resucita jugadores de nivel menor a 12 (newbies).
Los personajes de mayor nivel deben buscar un sacerdote normal u otro jugador.

### 2.11 Pretoriano (Tipo 10)

NPC del evento pretoriano con IA cooperativa avanzada. Cada subtipo (sacerdote, guerrero,
mago, cazador, rey) tiene un comportamiento unico y sofisticado. Se detalla en la seccion 9.

### 2.12 Gobernador (Tipo 11)

NPC de gobierno de ciudad. Gestiona funciones administrativas de la ciudad.

---

## 3. Tipos de Inteligencia Artificial

### 3.1 Estatico (IA 1)

- No se mueve
- No ataca
- Tipicamente usado para NPCs de servicio (comerciantes, banqueros, sacerdotes)

### 3.2 Movimiento Aleatorio (IA 2)

- Se mueve en una direccion aleatoria (norte, sur, este, oeste) con probabilidad de 1/12 por tick de IA
- Si es guardia, combina movimiento aleatorio con persecucion de objetivos detectados
- Es el comportamiento por defecto de muchas criaturas pasivas

### 3.3 Hostil Activo (IA 3)

- Busca activamente al jugador mas cercano en rango de vision
- Cuando detecta un objetivo, se mueve hacia el en linea recta (persecucion directa)
- Ataca al llegar a rango melee (tile adyacente)
- Si el objetivo se aleja, lo persigue
- Si el objetivo muere o sale del rango, busca otro

### 3.4 Defensa / Represalia (IA 4)

- Normalmente pasivo: no ataca a menos que lo ataquen primero
- Cuando recibe dano de un jugador, cambia a modo persecucion contra ese jugador especifico
- Si el agresor sale de rango o muere, vuelve a comportamiento pasivo
- Tipico de criaturas neutrales que solo se defienden

### 3.5 Guardia (IA 5)

- Combina movimiento aleatorio con persecucion de objetivos segun faccion:
  - **Guardia Real**: persigue y ataca criminales
  - **Guardia del Caos**: persigue y ataca ciudadanos
- Tambien ataca a cualquiera que lo haya agredido directamente
- Escanea las 4 direcciones adyacentes buscando objetivos

### 3.6 Objeto Magico / Trampa (IA 6)

- NPC estatico que lanza hechizos a jugadores cercanos al azar
- No siempre ataca al mismo objetivo; cambia de target aleatoriamente
- Representa trampas magicas, glifos, o fuentes de energia hostil

### 3.7 Seguir al Amo / Mascota (IA 8)

- Sigue a su dueno (el jugador que lo invoco o domo) manteniendo distancia de ~3 tiles
- Cuando no hay objetivo de ataque, se mueve aleatoriamente cerca del dueno
- Si el dueno se aleja mucho, se mueve hacia el
- Cambia automaticamente de comportamiento cuando el dueno entra en combate (ver seccion 7)

### 3.8 NPC Ataca NPC (IA 9)

- Modo de combate entre NPCs
- Principalmente usado cuando una mascota de jugador ataca a una criatura
- El NPC persigue y ataca a otro NPC especifico
- Cuando el objetivo NPC muere, vuelve a seguir al amo (IA 8)

### 3.9 Pathfinding (IA 10)

- Version avanzada de la IA hostil activa
- Usa un algoritmo de busqueda de caminos (A*) para encontrar la ruta al jugador mas cercano
- Puede rodear obstaculos en lugar de quedar bloqueado
- Radio de deteccion: 10 tiles
- Si no encuentra camino, se mueve aleatoriamente
- Recalcula el camino si se bloquea o si llega al final sin alcanzar al objetivo

### 3.10 Pretorianos (IA 20-24)

IAs cooperativas avanzadas exclusivas del evento pretoriano. Cada tipo (sacerdote, guerrero,
mago, cazador, rey) tiene un comportamiento unico. Se detallan en la seccion 9.

---

## 4. Sistema de Aggro y Deteccion

### 4.1 Rango de Vision

Los NPCs tienen el mismo rango de vision que los jugadores:
- **Horizontal**: 8 tiles
- **Vertical**: 6 tiles

### 4.2 Que No Detectan

Los NPCs **no detectan** a jugadores que esten:
- Muertos (fantasmas)
- Invisibles (por hechizo)
- Ocultos (por skill)
- En consulta con un GM
- Marcados como "Ignorado" (druidas mimetizados como NPC)
- Protegidos por timer de nuevo ingreso (5 segundos post-login)
- Admins que se hayan marcado como no perseguibles

### 4.3 Aggro de Criaturas Hostiles (Alineacion Malvada)

Las criaturas con alineacion distinta a 0 (malvadas):
- Escanean las 4 direcciones adyacentes buscando jugadores
- Atacan al **primer jugador vivo** que encuentren en rango
- Tambien atacan mascotas de otros jugadores que esten paralizadas
- NO atacan druidas mimetizados (los ignoran)

### 4.4 Aggro de Criaturas Defensivas (Alineacion Buena)

Las criaturas con alineacion 0 (buenas/neutrales):
- Solo atacan al jugador que **las ataco primero** (campo AttackedBy)
- Si el agresor sale de rango o muere, restauran su comportamiento original
- No son hostiles por defecto

### 4.5 Aggro de Guardias Reales

- Escanean las 4 direcciones adyacentes continuamente
- Atacan a cualquier **criminal** que encuentren
- Tambien atacan a **quien los haya atacado directamente** (sin importar alineacion)
- Prioridad: primero el agresor directo, luego criminales cercanos

### 4.6 Aggro de Guardias del Caos

Inverso a los guardias reales:
- Atacan a **ciudadanos no-criminales**
- Tambien atacan a quien los haya atacado si es criminal

### 4.7 Cambio de Aggro en Mascotas

Las mascotas cambian su aggro dinamicamente:
- Al ser atacadas: cambian a modo NPCDEFENSA y persiguen al agresor
- Las mascotas del Ejercito Real con seguro activado no atacan ciudadanos
- Cuando su amo ataca un NPC: las mascotas cambian a modo NpcAtacaNpc y ayudan al dueno
- Cuando el objetivo muere: vuelven a seguir al amo (modo SigueAmo)

---

## 5. Movimiento de NPCs

### 5.1 Movimiento Aleatorio

- El NPC elige una direccion aleatoria (N/S/E/O)
- Se mueve 1 tile en esa direccion si no esta bloqueado
- Probabilidad de moverse: 1/12 por tick de IA (~100ms)
- Los guardias combinan esto con persecucion cuando detectan un objetivo

### 5.2 Persecucion Directa

El NPC calcula la direccion hacia el objetivo:
- Si el objetivo esta al norte, se mueve al norte
- Si esta al noreste, alterna entre norte y este
- Es movimiento en linea recta sin evasion de obstaculos
- Si el camino esta bloqueado, el NPC queda atascado

### 5.3 Pathfinding (A*)

Solo para NPCs con IA tipo 10:
- Busca jugadores en un radio de 10 tiles
- Calcula la ruta mas corta evitando obstaculos (tiles bloqueados)
- Sigue el camino tile a tile
- Recalcula si el camino se bloquea o si llega al final sin alcanzar al objetivo
- Si no encuentra camino despues de los intentos maximos, se mueve aleatoriamente

### 5.4 NPCs Inmovilizados

Los NPCs que estan inmovilizados (por hechizo de inmovilizacion):
- No se desplazan
- Pueden actuar (atacar fisicamente o lanzar hechizos) en la direccion que miran
- Siguen detectando enemigos en su campo visual frontal

### 5.5 Interaccion con Fantasmas

Los NPCs pueden "empujar" fantasmas (jugadores muertos) al moverse a un tile ocupado por uno:
- El fantasma se desplaza a la posicion anterior del NPC
- No se permite traslado entre agua y tierra en estos empujones
- Esto evita que los fantasmas bloqueen el paso de las criaturas

### 5.6 Validacion de Terreno

Cada NPC puede tener restricciones de terreno:
- **AguaValida**: si puede moverse en tiles de agua (criaturas acuaticas)
- **TierraInvalida**: si NO puede moverse en tierra (criaturas exclusivamente acuaticas)
- Los NPCs no pueden moverse a tiles bloqueados ni a tiles con trigger de posicion invalida (trigger 3)

---

## 6. Propiedades de los NPCs

Cada NPC tiene un conjunto extenso de propiedades configurables:

### 6.1 Propiedades de Combate

| Propiedad | Descripcion |
|-----------|-------------|
| Attackable | Si puede ser atacado por jugadores |
| Hostile | Si es hostil a jugadores (activa la IA de persecucion) |
| Alineacion | 0 = bueno, 1 = malvado, 2 = noble, 4 = salvaje |
| MinHIT / MaxHIT | Rango de dano fisico |
| MaxHP / MinHP | Vida maxima y actual |
| def | Defensa fisica |
| defM | Defensa magica |
| Veneno | Si puede envenenar al golpear (30% de chance) |
| AtacaDoble | Si puede hacer doble ataque (fisico + hechizo, 50% de chance) |
| LanzaSpells | Cantidad de hechizos que puede lanzar |
| AfectaParalisis | 0 = susceptible, 1 = inmune a paralisis |

### 6.2 Propiedades de Comportamiento

| Propiedad | Descripcion |
|-----------|-------------|
| Movement (IA) | Tipo de inteligencia artificial |
| Domable | Nivel de skill necesario para domarlo (0 = no domable) |
| AguaValida | Si puede moverse en agua |
| TierraInvalida | Si no puede moverse en tierra |

### 6.3 Propiedades de Recompensa

| Propiedad | Descripcion |
|-----------|-------------|
| GiveEXP | Experiencia total que otorga al morir |
| GiveGLD | Oro que suelta al morir |
| Drop[1-5] | Hasta 5 items que suelta al morir (indice + cantidad) |
| Inventario | Items que lleva (para comerciantes; tambien se tiran al morir) |

### 6.4 Propiedades Especiales

| Propiedad | Descripcion |
|-----------|-------------|
| Owner | Jugador dueno (si es mascota) |
| MaestroUser | Indice del jugador que controla al NPC |
| Respawn | Si debe reaparecer al morir |
| RespawnOrigPos | Si reaparece en su posicion original |
| InvReSpawn | Si su inventario se regenera al reaparecer |
| IsPretoriano | Si es un NPC del evento pretoriano |
| Expresiones | Frases aleatorias que dice el NPC |
| Criaturas | Para entrenadores: lista de criaturas domables |

---

## 7. Mascotas y Control

### 7.1 Como se Obtienen las Mascotas

Hay dos formas de obtener mascotas:
- **Domar**: usando el skill de Domar sobre una criatura domable
- **Invocar**: mediante hechizos de invocacion

### 7.2 Domar Criaturas

**Requisitos:**
- Estar en zona PK (no se puede domar en zona segura)
- El NPC debe tener la propiedad Domable > 0
- El NPC no debe tener dueno actualmente
- No tener ya el maximo de mascotas (3)
- No tener ya 2 criaturas del mismo tipo

**Formula de puntos de domar:**
```
PuntosDomar = Carisma x SkillDomar
```

**Modificadores de clase:**
- Druida, Cazador: modificador 6 (mas facil)
- Clerigo: modificador 7
- Demas clases: modificador 10 (mas dificil)

**Bonificaciones por equipo:**
- Flauta Elfica: 20% de reduccion en puntos requeridos
- Flauta Magica: 11% de reduccion

**Probabilidad de exito:** incluso con puntos suficientes, hay solo un **20% de chance** de
exito (1 en 5 intentos).

**En zona segura:** las mascotas "esperan afuera" (desaparecen del mapa pero se conservan
en la data del jugador).

### 7.3 Comportamiento de Mascotas

Las mascotas siguen estas reglas:

1. **Modo pasivo**: siguen al dueno a ~3 tiles de distancia, moviendose aleatoriamente cuando estan cerca
2. **Dueno ataca NPC**: las mascotas cambian a modo "NpcAtacaNpc" y ayudan al dueno
   - Excepcion: elementales de fuego/tierra no cambian automaticamente
3. **Dueno es atacado por jugador**: las mascotas cambian a modo "NPCDEFENSA" contra el agresor
4. **Objetivo muere**: vuelven a modo pasivo (seguir al dueno)
5. **Mascotas de la Armada Real**: con seguro activado, no atacan ciudadanos

### 7.4 Mascotas Invocadas vs Domadas

| Aspecto | Invocadas | Domadas |
|---------|-----------|---------|
| Duracion | Limitada (timer de invocacion) | Permanente (mientras el jugador este conectado) |
| Origen | Hechizo de invocacion | Skill de Domar |
| Al morir el jugador | Se eliminan | Se eliminan |
| Al desconectarse | Se eliminan | Se guardan en la persistencia |
| Al reconectarse | No se restauran | Se restauran si esta en zona PK |

### 7.5 NPCs Magicos Especiales como Mascotas

- **Elemental de agua**: no ataca cuerpo a cuerpo, solo lanza hechizos
- **Elemental de fuego**: lanza hechizos a distancia, puede combatir NPC vs NPC con magia
- Estos elementales requieren intervencion explicita del dueno para atacar (no se activan automaticamente cuando el dueno ataca)

### 7.6 Limite de Mascotas

- Maximo total: 3 mascotas simultaneas
- Maximo por tipo: 2 criaturas del mismo tipo de NPC

---

## 8. Respawn y Muerte de NPCs

### 8.1 Al Morir un NPC

Cuando un NPC muere, se ejecuta la siguiente secuencia:

1. **Experiencia**: se distribuye entre los jugadores que lo danaron (proporcional al dano hecho)
2. **Reputacion**: segun el tipo de NPC:
   - NPC hostil: suma Plebe/Cazador al matador
   - NPC no hostil: suma Asesino
   - Guardia: masivamente suma Asesino, borra Noble y Plebe
3. **Loot**: se tiran al piso los items del inventario, drops, y oro
4. **Respawn**: si corresponde, se genera un nuevo NPC del mismo tipo

### 8.2 Sistema de Loot

**Inventario del NPC:**
Los NPCs pueden tener items predefinidos en su inventario (para comerciantes y criaturas con loot).
Al morir, estos items se tiran al piso en la posicion donde murio.

**Drops:**
Ademas del inventario, cada NPC puede tener hasta **5 entradas de drop** (ObjIndex + Amount).
Estos drops tambien se tiran al piso.

**Oro:**
Si el NPC tiene `GiveGLD > 0`, el oro se tira al piso en pilas de hasta 10,000 unidades.
Si la cantidad es mayor, se crean multiples pilas.

**Flag InvReSpawn:**
Controla si el inventario del NPC se regenera al reaparecer. Crucial para NPCs comerciantes.

### 8.3 Mecanica de Respawn

Cuando un NPC muere y tiene la flag de respawn habilitada:

1. Se crea un nuevo NPC del mismo tipo (misma definicion)
2. **Si tiene RespawnOrigPos**: reaparece en su posicion original (la definida en el mapa)
3. **Si no**: reaparece en una posicion aleatoria valida del mismo mapa
4. La busqueda de posicion valida tiene un maximo de intentos

**Posicion valida para respawn:**
- No debe ser un trigger de zona segura, teleport, puerta, o posicion invalida
- No debe haber jugadores cerca
- Debe ser terreno valido para el tipo de NPC (agua o tierra segun corresponda)
- No debe haber otro NPC ya ocupando el tile

**Guardias:**
Los guardias se respawnean durante el WorldSave periodico y cada 15 minutos. Esto garantiza
que las ciudades siempre tengan proteccion.

---

## 9. IA Cooperativa del Evento Pretoriano

El evento pretoriano es el sistema de IA mas avanzado del juego. Un "clan" de 8 NPCs coopera
con roles diferenciados para defender su posicion contra los jugadores.

### 9.1 Composicion del Clan Pretoriano

| Cantidad | Tipo | IA | Funcion |
|----------|------|-----|---------|
| 1 | Rey Pretoriano | 24 | Objetivo principal, soporte/tank |
| 2 | Sacerdote Pretoriano | 20 | Soporte, curacion, desparalizar |
| 3 | Guerrero Pretoriano | 21 | Combate cuerpo a cuerpo |
| 1 | Cazador Pretoriano | 23 | Ataque a distancia, anti-caster |
| 1 | Mago Pretoriano | 22 | Ataque magico AOE, deteccion de invisibles |

### 9.2 Mapa Pretoriano

El mapa pretoriano tiene **2 alcobas** (posiciones fijas). Cuando muere el rey de una alcoba,
se respawnea el clan completo en la **otra alcoba**. Esto obliga a los jugadores a adaptarse
y reposicionarse.

### 9.3 IA del Sacerdote Pretoriano

El sacerdote sigue una lista de prioridades estricta (de mayor a menor):

1. **Desparalizar aliados**: si algun pretoriano esta paralizado, lanza hechizo de remocion de paralisis (20% de efectividad)
2. **Paralizar enemigos**: lanza paralisis al jugador mas cercano
3. **Paralizar mascotas**: paraliza mascotas de jugadores que esten en rango
4. **Curar aliados heridos**: cura al pretoriano con menor HP
5. **Atacar**: si no hay nada mejor que hacer, lanza Tormenta Avanzada contra jugadores

### 9.4 IA del Guerrero Pretoriano

Comportamiento de combate cuerpo a cuerpo directo:
- Persigue al jugador mas cercano
- Ataca fisicamente al llegar a rango
- Si no encuentra objetivo, se reagrupa con el clan
- Comportamiento relativamente simple pero efectivo en grupo

### 9.5 IA del Cazador Pretoriano

El cazador tiene una IA sofisticada de combate a distancia:

**Seleccion de objetivo:**
- Prioriza **magos y clerigos** como targets (son peligrosos pero fragiles)
- Ataca con flechas magicas

**Posicionamiento:**
- Se mantiene a **media distancia** entre el rey y los atacantes
- No se acerca demasiado al combate melee
- Si queda solo (sin aliados), se reagrupa con el ejercito pretoriano

### 9.6 IA del Mago Pretoriano

La IA mas compleja del juego:

**Deteccion de invisibilidad:**
- Tiene un **35% de probabilidad** de detectar jugadores invisibles por tick

**Seleccion de objetivo:**
- Prioriza jugadores **paralizados** (faciles de eliminar)
- Prioriza jugadores **invisibles** (detectados con su habilidad especial)

**Comportamiento suicida ("Rotura de Vara"):**
- Cuando sus HP bajan de **750**, inicia una secuencia suicida
- Lanza **Apocalipsis** repetidamente (hasta 6 cargas) antes de morir
- El Apocalipsis es el hechizo mas devastador del juego

**Posicionamiento:**
- **Evita el combate cuerpo a cuerpo**: si un enemigo se acerca, retrocede hacia el centro del grupo
- Prefiere atacar a distancia con hechizos

### 9.7 IA del Rey Pretoriano

El rey tiene **dos modos de comportamiento** completamente diferentes:

**Modo Soporte (mientras tiene pretorianos vivos):**
- Desparaliza aliados (20% de efectividad)
- Cura envenenamiento de aliados
- Cura heridas de aliados
- Se mantiene al maximo de HP
- NO ataca directamente; deja el combate a su ejercito

**Modo Berserker (cuando TODOS sus pretorianos mueren):**
- Cambia completamente de comportamiento
- Sale a atacar cuerpo a cuerpo agresivamente
- Tiene "speed hack": puede atacar multiples veces por turno en las 4 direcciones
- Persigue al enemigo mas cercano con persecucion directa
- Si no puede alcanzar al enemigo, le lanza **Ceguera** y **Estupidez**

### 9.8 Sistema de Alcobas

Los pretorianos tienen conciencia de su entorno grupal:
- Cuando detectan que quedan pocos aliados en una alcoba, pueden **migrar** a la otra alcoba para reagruparse
- Al morir el rey de una alcoba, todo el clan se respawnea en la otra
- Esto crea un ciclo dinamico que obliga a los jugadores a ser moviles

---

## 10. Propiedad de NPC (Ownership en PvE)

### 10.1 Mecanica

Cuando un jugador ataca a un NPC sin dueno, se lo "apropia":

- Otros jugadores **no pueden atacar** ese NPC
- La propiedad dura **18 segundos** por defecto
- Si el dueno no ataca al NPC en ese tiempo, pierde la propiedad
- Un nuevo jugador puede entonces apropiarse

### 10.2 Excepciones a la Propiedad

| Situacion | Resultado |
|-----------|----------|
| Mismo clan que el dueno | Puede atacar sin robar propiedad |
| Misma party que el dueno | Puede atacar sin robar propiedad |
| Hechizo de paralisis sobre NPC con dueno | Se puede paralizar sin robar (pero no re-paralizar si un aliado ya lo hizo) |
| Mascota ataca NPC | No se apropia |

### 10.3 Proposito

El sistema de propiedad evita el "kill stealing" (que un jugador robe la experiencia/loot de
un NPC que otro jugador estaba peleando). Es especialmente importante en NPCs de alta experiencia
y en contenido de grupo.

---

## 11. Datos de Definicion de NPC

Cada tipo de NPC se define en un archivo de datos con los siguientes campos:

### 11.1 Identidad y Apariencia
- Nombre y descripcion
- Cuerpo y cabeza (graficos)
- Animaciones de arma, escudo, casco

### 11.2 Stats de Combate
- HP minimo y maximo
- Dano minimo y maximo
- Defensa fisica y magica
- Velocidad de ataque

### 11.3 Comportamiento
- Tipo de IA (movimiento)
- Hostilidad y atacabilidad
- Alineacion (bueno, malvado, noble, salvaje)
- Inmunidad a paralisis

### 11.4 Habilidades Especiales
- Lista de hechizos (si LanzaSpells > 0)
- Doble ataque (fisico + magico)
- Veneno
- Terreno valido (agua, tierra)

### 11.5 Recompensas
- Experiencia (GiveEXP)
- Oro (GiveGLD)
- Hasta 5 drops con indice de item y cantidad
- Inventario de items (para comerciantes y como loot adicional)

### 11.6 Miscelaneo
- Si es domable (y nivel requerido)
- Si respawnea y donde
- Frases aleatorias (expresiones)
- Criaturas enseñables (para entrenadores)

---

## 12. Relacion con Otros Sistemas

### 12.1 Combate
- Los NPCs usan el mismo sistema de formulas de dano que los jugadores
- Las mascotas participan en el combate y su experiencia va al dueno
- El aggro determina que jugadores son objetivos de los NPCs

### 12.2 Magia
- Los NPCs magicos lanzan hechizos del mismo pool que los jugadores
- Las mascotas pueden ser objetivos de hechizos (paralisis, dano)
- Las invocaciones crean NPCs nuevos

### 12.3 Inventario
- Los NPCs comerciantes gestionan inventarios de venta
- Los drops y loot alimentan la economia del juego
- El oro de los NPCs es una fuente primaria de ingreso

### 12.4 Facciones
- Los guardias protegen a la faccion correspondiente
- Los pretorianos son contenido faccionario
- La alineacion del NPC afecta la reputacion al matarlo

### 12.5 Mundo y Mapas
- Los NPCs se spawean en posiciones definidas en los mapas
- El sistema de areas determina que jugadores ven a que NPCs
- El terreno restringe el movimiento de ciertos NPCs
