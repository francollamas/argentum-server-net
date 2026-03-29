# Argentum Online - Documentacion Funcional del Servidor

> Este documento es el indice maestro de la documentacion funcional del servidor de Argentum Online.
> Describe **que hace** el juego, no como esta implementado.
>
> Las notas y consideraciones sobre la reimplementacion se encuentran en un documento aparte:
> [90-CONSIDERACIONES-REIMPLEMENTACION.md](90-CONSIDERACIONES-REIMPLEMENTACION.md).

---

## Que es Argentum Online

Argentum Online (AO) es un MMORPG 2D de origen argentino. El jugador controla un personaje en un mundo
medieval fantastico compuesto por cientos de mapas interconectados. El juego combina combate en tiempo
real (PvP y PvE), un sistema de oficios y artesania, comercio entre jugadores, clanes, facciones
enfrentadas, y un sistema de reputacion que clasifica a los personajes como ciudadanos o criminales.

El servidor es autoritativo: toda la logica del juego se ejecuta del lado del servidor. El cliente
es esencialmente un renderer que envia comandos y recibe actualizaciones de estado.

---

## Mapa de Features

### 1. Nucleo del Juego

| # | Sistema | Documento | Descripcion |
|---|---------|-----------|-------------|
| 01 | Arquitectura General | [01-ARQUITECTURA-GENERAL.md](01-ARQUITECTURA-GENERAL.md) | Game loop, ciclo de vida del servidor, modelo de estado, timers e intervalos |
| 02 | Personaje | [02-PERSONAJE.md](02-PERSONAJE.md) | Creacion de personaje, atributos, clases, razas, niveles, experiencia, skills, muerte y resurreccion |
| 03 | Combate | [03-COMBATE.md](03-COMBATE.md) | Formulas de impacto, evasion, dano, bloqueo, golpe critico, apunalar, PvP vs PvE, reputacion |
| 04 | Magia | [04-MAGIA.md](04-MAGIA.md) | Tipos de hechizos, efectos de estado, dano magico, invocaciones, costos y restricciones |
| 05 | NPCs e Inteligencia Artificial | [05-NPCs-E-IA.md](05-NPCs-E-IA.md) | Tipos de NPC, comportamientos de IA, aggro, mascotas, respawn, loot, pathfinding |

### 2. Sistemas Economicos

| # | Sistema | Documento | Descripcion |
|---|---------|-----------|-------------|
| 06 | Inventario y Objetos | [06-INVENTARIO-Y-OBJETOS.md](06-INVENTARIO-Y-OBJETOS.md) | Slots, peso, tipos de objeto, equipamiento, restricciones, objetos especiales |
| 07 | Oficios y Trabajo | [07-OFICIOS-Y-TRABAJO.md](07-OFICIOS-Y-TRABAJO.md) | Pesca, mineria, herreria, carpinteria, tala, navegacion, domar criaturas |
| 08 | Comercio y Economia | [08-COMERCIO-Y-ECONOMIA.md](08-COMERCIO-Y-ECONOMIA.md) | Tiendas NPC, comercio seguro entre jugadores, banco, manejo de oro |

### 3. Sistemas Sociales

| # | Sistema | Documento | Descripcion |
|---|---------|-----------|-------------|
| 09 | Clanes | [09-CLANES.md](09-CLANES.md) | Fundacion, membresía, elecciones, guerras, alianzas, sistema de antifaccion |
| 10 | Party (Grupo) | [10-PARTY.md](10-PARTY.md) | Creacion de grupo, distribucion de experiencia, liderazgo, disolucion |
| 11 | Facciones y Pretorianos | [11-FACCIONES-Y-PRETORIANOS.md](11-FACCIONES-Y-PRETORIANOS.md) | Ejercito Real vs Legion Oscura, rangos, recompensas, evento pretoriano |

### 4. Infraestructura

| # | Sistema | Documento | Descripcion |
|---|---------|-----------|-------------|
| 12 | Mundo y Mapas | [12-MUNDO-Y-MAPAS.md](12-MUNDO-Y-MAPAS.md) | Estructura de mapas, tiles, triggers, teleports, clima, sistema de areas |
| 13 | Protocolo y Red | [13-PROTOCOLO-Y-RED.md](13-PROTOCOLO-Y-RED.md) | Paquetes cliente/servidor, serializacion binaria, flujo de conexion |
| 14 | Administracion y Seguridad | [14-ADMIN-Y-SEGURIDAD.md](14-ADMIN-Y-SEGURIDAD.md) | Comandos GM, niveles de privilegio, centinela anti-bot, seguridad IP, foro, encuestas |

### 5. Reimplementacion

| # | Sistema | Documento | Descripcion |
|---|---------|-----------|-------------|
| 90 | Consideraciones de Reimplementacion | [90-CONSIDERACIONES-REIMPLEMENTACION.md](90-CONSIDERACIONES-REIMPLEMENTACION.md) | Notas arquitectonicas, decisiones de diseno y compatibilidad para el servidor nuevo |

---

## Relaciones entre Sistemas

El siguiente diagrama muestra las dependencias principales entre sistemas:

```
                    ┌──────────────┐
                    │ Arquitectura │
                    │   General    │
                    └──────┬───────┘
                           │
              ┌────────────┼────────────┐
              v            v            v
        ┌───────────┐ ┌────────┐ ┌──────────┐
        │ Personaje │ │ Mundo  │ │Protocolo │
        │           │ │ y Mapas│ │  y Red   │
        └─────┬─────┘ └───┬────┘ └──────────┘
              │            │
     ┌────────┼────────┬───┘
     v        v        v
┌─────────┐┌───────┐┌─────────────┐
│ Combate ││ Magia ││ NPCs e IA   │
└────┬────┘└───┬───┘└──────┬──────┘
     │         │           │
     v         v           v
┌──────────────────────────────────┐
│     Inventario y Objetos         │
└───────────────┬──────────────────┘
                │
     ┌──────────┼──────────┐
     v          v          v
┌─────────┐┌─────────┐┌──────────┐
│ Oficios ││Comercio ││  Banco   │
└─────────┘└─────────┘└──────────┘
                │
     ┌──────────┼──────────┐
     v          v          v
┌────────┐┌────────┐┌────────────┐
│ Clanes ││ Party  ││ Facciones  │
└────────┘└────────┘└────────────┘
                │
                v
        ┌──────────────┐
        │ Admin y      │
        │ Seguridad    │
        └──────────────┘
```

---

## Conceptos Transversales

Estos conceptos atraviesan multiples sistemas y son fundamentales para entender el juego:

### Estado Ciudadano / Criminal

Cada personaje tiene un sistema de **reputacion** compuesto por 6 contadores (Noble, Burgues, Plebe,
Ladron, Bandido, Asesino). El promedio ponderado de estos contadores determina si el personaje es
**ciudadano** (promedio >= 0) o **criminal** (promedio < 0). Este estado afecta:

- Combate: quien puede atacar a quien, consecuencias de matar
- Facciones: requisitos de ingreso al Ejercito Real o la Legion Oscura
- Clanes: alineacion del clan y compatibilidad de miembros
- Comercio: restricciones de robo entre facciones
- Visual: color del nombre sobre el personaje (azul = ciudadano, rojo = criminal)

### Sistema de Seguro (Safe Mode)

El jugador puede activar/desactivar un "seguro" que impide atacar accidentalmente a ciudadanos.
Los miembros del Ejercito Real tienen el seguro siempre activo contra ciudadanos.

### Newbie

Un personaje es considerado "newbie" hasta cierto nivel (nivel 12). Los newbies tienen:
- Items especiales que no pueden tirar ni perder al morir
- Restriccion de equipamiento (solo items newbie)
- Proteccion en zonas especiales (Dungeon Newbie)
- Al dejar de ser newbie, se les quitan automaticamente todos los items newbie

### Zonas del Mundo

El mundo se divide en distintos tipos de zona segun los triggers de los tiles:
- **Zona segura**: no se puede atacar ni ser atacado, no se puede robar
- **Zona PK**: combate libre con consecuencias de reputacion
- **Arena de pelea**: combate libre SIN consecuencias (no se pierden items ni reputacion)
- **Zona anti-piquete**: si un jugador permanece mucho tiempo, es encarcelado

### Persistencia

Los personajes se guardan en archivos individuales (uno por personaje). El servidor realiza
guardados automaticos periodicos (WorldSave) y al desconectarse cada jugador. Los datos del mundo
(objetos en el piso, NPCs spawneados) tambien se persisten en los archivos de mapa.

---

## Constantes Globales Clave

Estos son los limites fundamentales del juego que definen la escala del mundo:

| Concepto | Valor | Nota |
|----------|-------|------|
| Tamano de mapa | 100 x 100 tiles | Cada mapa es una grilla cuadrada |
| Cantidad de mapas | ~290 | Numerados secuencialmente |
| Usuarios simultaneos max | 550 (configurable) | Definido en configuracion |
| NPCs simultaneos max | 10,000 | Incluyendo mascotas y spawns |
| Nivel maximo | 50 (configurable, tope tecnico 255) | Progresion exponencial |
| Skills | 20 habilidades | Cada una de 0 a 100 |
| Hechizos por personaje | 35 | Slots de hechizo aprendibles |
| Slots de inventario | 20 base + 10 con mochila | Maximo 30 |
| Slots de banco | 40 | Almacenamiento secundario |
| Mascotas max | 3 | Invocadas o domadas |
| Miembros de party max | 5 | Un lider + 4 miembros |
| Clanes max | 1,000 | Limite global del servidor |
| Oro maximo | 90,000,000 | Por personaje |
| Experiencia maxima | 99,999,999 | Tope tecnico |

---

## Glosario

| Termino | Significado |
|---------|-------------|
| AO | Argentum Online |
| PvP | Player vs Player (combate entre jugadores) |
| PvE | Player vs Environment (combate contra NPCs) |
| NPC | Non-Player Character (personaje no jugable controlado por el servidor) |
| GM | Game Master (administrador del juego) |
| Tile | Unidad minima del mapa (casilla de 32x32 pixeles) |
| Trigger | Efecto especial asociado a un tile del mapa |
| Newbie | Jugador nuevo (nivel 1-12) con protecciones especiales |
| Ciudadano | Personaje con buena reputacion (nombre azul) |
| Criminal | Personaje con mala reputacion (nombre rojo) |
| Armada / Ejercito Real | Faccion de los ciudadanos |
| Legion / Caos | Faccion de los criminales |
| WorldSave | Guardado periodico de todo el estado del mundo |
| Pretoriano | Evento PvE cooperativo contra un clan de NPCs con IA avanzada |
| Centinela | Sistema anti-bot que verifica que el jugador esta presente |
| Mimetismo | Habilidad de copiar la apariencia de otro personaje o NPC |
| Apunalar | Ataque sorpresa con dano multiplicado (por la espalda) |
| Piquete | Bloqueo intencional de un tile; el servidor lo penaliza |

---

## Estado de la Documentacion

| # | Documento | Estado |
|---|-----------|--------|
| 00 | Indice General | Completo |
| 01 | Arquitectura General | Completo |
| 02 | Personaje | Completo |
| 03 | Combate | Completo |
| 04 | Magia | Completo |
| 05 | NPCs e IA | Completo |
| 06 | Inventario y Objetos | Completo |
| 07 | Oficios y Trabajo | Completo |
| 08 | Comercio y Economia | Completo |
| 09 | Clanes | Completo |
| 10 | Party | Completo |
| 11 | Facciones y Pretorianos | Completo |
| 12 | Mundo y Mapas | Completo |
| 13 | Protocolo y Red | Completo |
| 14 | Admin y Seguridad | Completo |
| 90 | Consideraciones de Reimplementacion | Completo |
