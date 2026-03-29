# 11 - Facciones y Pretorianos

> Este documento describe el sistema de facciones (Ejercito Real y Legion Oscura),
> incluyendo requisitos de ingreso, sistema de rangos con 15 niveles, recompensas,
> expulsion, y el evento pretoriano como contenido PvE cooperativo de faccion.

---

## 1. Vision General

Argentum Online tiene dos facciones antagonicas que representan el conflicto central del mundo:

| Faccion | Nombre Completo | Alineacion | Enemigos |
|---------|----------------|------------|----------|
| **Ejercito Real** | Armada Real | Ciudadanos | Criminales y Legion Oscura |
| **Legion Oscura** | Fuerzas del Caos | Criminales | Ciudadanos y Ejercito Real |

Las facciones son **mutuamente excluyentes**: un personaje solo puede pertenecer a una.
Ademas, un personaje que fue miembro del Ejercito Real **nunca puede unirse a la Legion Oscura**
(restriccion permanente e irreversible).

Las facciones ofrecen un sistema de progresion paralelo al de nivel, basado en **kills faccionarios**
y con **15 rangos** que otorgan equipamiento exclusivo y estatus.

---

## 2. Alistamiento en el Ejercito Real

### 2.1 Requisitos

Todos los requisitos deben cumplirse simultaneamente:

| Requisito | Valor |
|-----------|-------|
| Reputacion | No ser criminal (promedio >= 0) |
| Nivel minimo | 25 |
| Criminales matados | Al menos 30 |
| Ciudadanos matados | Exactamente 0 (nunca haber matado inocentes) |
| Puntos de Nobleza | Al menos 1,000,000 |
| Reenlistadas | No haber sido expulsado mas de 4 veces |
| Clan | No pertenecer a un clan neutral |
| Otra faccion | No ser miembro de la Legion Oscura |

### 2.2 Recompensas de Ingreso

Al enlistarse, el personaje recibe:
- **Armaduras faccionarias** de tres categorias de defensa (baja, media, alta), seleccionadas
  segun la raza y clase del personaje
- **Experiencia inicial** de recompensa

### 2.3 Datos Registrados

Al ingresar se registra:
- Nivel de ingreso
- Fecha de ingreso
- Kills al momento del ingreso (para calcular progreso posterior)
- Flag de que recibio experiencia inicial y armadura

---

## 3. Alistamiento en la Legion Oscura

### 3.1 Requisitos

| Requisito | Valor |
|-----------|-------|
| Reputacion | Ser criminal (promedio < 0) |
| Nivel minimo | 25 |
| Ciudadanos matados | Al menos 70 |
| Historial faccionario | No haber recibido experiencia inicial del Ejercito Real |
| Reenlistadas | No haber sido expulsado mas de 4 veces |
| Clan | No pertenecer a un clan neutral |

### 3.2 Recompensas de Ingreso

Identicas en estructura a la Armada:
- Armaduras faccionarias (version del Caos)
- Experiencia inicial de recompensa

---

## 4. Sistema de Rangos

Ambas facciones tienen **15 rangos** con requisitos progresivos. Cada rango exige un numero
creciente de kills faccionarios y, en rangos altos, nivel minimo y puntos de reputacion.

### 4.1 Rangos del Ejercito Real

Los rangos se desbloquean por **criminales matados** desde el ingreso:

| # | Rango | Criminales Matados | Requisitos Adicionales |
|---|-------|-------------------|----------------------|
| 1 | Aprendiz | Ingreso | - |
| 2 | Escudero | 70 | - |
| 3 | Soldado | 130 | - |
| 4 | Sargento | 210 | - |
| 5 | Teniente | 320 | - |
| 6 | Comandante | 460 | - |
| 7 | Capitan | 640 | Nivel 27 |
| 8 | Senescal | 870 | - |
| 9 | Mariscal | 1,160 | - |
| 10 | Condestable | 2,000 | Nivel 30 |
| 11 | Ejecutor Imperial | 2,500 | 2,000,000 Nobleza |
| 12 | Protector del Reino | 3,000 | 3,000,000 Nobleza |
| 13 | Avatar de la Justicia | 3,500 | 4,000,000 Nobleza + Nivel 35 |
| 14 | Guardian del Bien | 4,000 | 5,000,000 Nobleza + Nivel 36 |
| 15 | Campeon de la Luz | 5,000 | 6,000,000 Nobleza + Nivel 37 |

### 4.2 Rangos de la Legion Oscura

Los rangos se desbloquean por **ciudadanos matados** desde el ingreso:

| # | Rango | Ciudadanos Matados | Requisitos Adicionales |
|---|-------|-------------------|----------------------|
| 1 | Acolito | Ingreso | - |
| 2 | Alma Corrupta | 160 | - |
| 3 | Paria | 300 | - |
| 4 | Condenado | 490 | - |
| 5 | Esbirro | 740 | - |
| 6 | Sanguinario | 1,100 | - |
| 7 | Corruptor | 1,500 | Nivel 27 |
| 8 | Heraldo Impio | 2,010 | - |
| 9 | Caballero de la Oscuridad | 2,700 | - |
| 10 | Senor del Miedo | 4,600 | Nivel 30 |
| 11 | Ejecutor Infernal | 5,800 | Nivel 31 |
| 12 | Protector del Averno | 6,990 | Nivel 33 |
| 13 | Avatar de la Destruccion | 8,100 | Nivel 35 |
| 14 | Guardian del Mal | 9,300 | Nivel 36 |
| 15 | Campeon de la Oscuridad | 11,500 | Nivel 37 |

### 4.3 Observaciones de Balance

- La Legion Oscura requiere **significativamente mas kills** para los mismos rangos:
  rango 15 del Caos necesita 11,500 kills vs 5,000 del Real
- Esto refleja la asimetria del juego: es mas facil matar criminales (los ciudadanos son
  mayoria y los guardias ayudan) que matar ciudadanos
- Los rangos altos exigen tanto kills como nivel y reputacion, creando un embudo natural

### 4.4 Recompensas por Rango

Cada subida de rango otorga:
- **Armaduras faccionarias** en cantidades que escalan con el rango:
  - Rangos bajos: mas armaduras de defensa baja, pocas de defensa alta
  - Rangos altos: menos armaduras de defensa baja, mas de defensa alta
- **Experiencia** proporcional al rango

La cantidad y tipo de armaduras depende de la raza y clase del personaje, asi como del rango
alcanzado. Las armaduras faccionarias son items exclusivos que no se pueden craftear.

---

## 5. Restricciones de Faccion

### 5.1 Equipamiento

- Los items marcados como "Real" solo pueden ser usados por miembros del Ejercito Real
- Los items marcados como "Caos" solo pueden ser usados por miembros de la Legion Oscura
- Las armaduras de la Armada solo se venden al "Sastre Real"
- Las armaduras del Caos solo se venden al "Sastre del Caos"

### 5.2 Combate

- Los miembros del Ejercito Real **no pueden atacar ciudadanos** (seguro permanente)
- Los miembros de la Legion Oscura **no pueden atacarse entre si**
- Los miembros de la Armada de alto rango pueden atacar en ciudades de su faccion
- Los miembros del Caos de alto rango pueden atacar en ciudades del caos

### 5.3 Robo

- Los miembros de la Legion Oscura no pueden robarse entre si
- Los miembros de la Armada Real no pueden robar a ciudadanos

### 5.4 Navegacion

Al navegar, la faccion afecta la apariencia del barco:
- Armada Real: fragata real
- Legion Oscura: fragata del caos

---

## 6. Expulsion de una Faccion

### 6.1 Causas

La expulsion puede ocurrir por:
- Decision administrativa (GM)
- Cambio de estado que viola los requisitos (ciudadano se vuelve criminal en la Armada)
- Sancion del sistema

### 6.2 Efectos de la Expulsion

1. Se desequipan automaticamente las armaduras y escudos faccionarios
2. Se incrementa el contador de **reenlistadas**
3. Si esta navegando, se actualiza la apariencia visual del barco
4. El personaje pierde todos los privilegios de la faccion

### 6.3 Limite de Reenlistadas

Un personaje puede ser expulsado y reenlistarse hasta **4 veces**. Despues de la 4ta expulsion,
no puede volver a enlistarse nunca mas. Esto penaliza el comportamiento errático.

### 6.4 Restriccion Permanente

Un personaje que recibio experiencia inicial del Ejercito Real **no puede unirse a la Legion
Oscura**, incluso si es expulsado. Esta restriccion es permanente e irreversible, forzando
una eleccion significativa.

---

## 7. Evento Pretoriano

### 7.1 Concepto

El evento pretoriano es un encuentro **PvE cooperativo** de faccion donde los jugadores
enfrentan un "clan" de NPCs con inteligencia artificial avanzada y coordinada. Es el contenido
de grupo mas desafiante del juego.

### 7.2 Mapa

El evento se desarrolla en un **mapa dedicado** (configurable). Este mapa tiene dos
**alcobas** (posiciones fijas en el mapa: posicion 35,25 y posicion 67,25) que sirven como
puntos de respawn alternativos para el clan pretoriano.

### 7.3 Composicion del Clan Pretoriano

El clan se compone de **8 NPCs** con roles diferenciados:

| Cantidad | Rol | Funcion |
|----------|-----|---------|
| 1 | Rey Pretoriano | Objetivo principal. Soporte mientras tiene ejercito, berserker cuando esta solo |
| 2 | Sacerdotes | Soporte: desparalizar aliados, paralizar enemigos, curar heridos |
| 3 | Guerreros | DPS melee: persiguen y atacan al jugador mas cercano |
| 1 | Cazador | DPS a distancia: prioriza magos y clerigos, ataca con flechas magicas |
| 1 | Mago | DPS magico AOE: detecta invisibles, tiene secuencia suicida de Apocalipsis |

### 7.4 Mecanica de Alcobas

El sistema de alcobas crea un ciclo dinamico:

1. El clan pretoriano aparece en una alcoba
2. Los jugadores lo enfrentan
3. Cuando muere el rey de una alcoba, **todo el clan se respawnea en la otra alcoba**
4. Los jugadores deben reposicionarse y adaptarse
5. El ciclo se repite

Ademas, cuando los pretorianos detectan que quedan pocos aliados en una alcoba, pueden
**migrar a la otra** para reagruparse, creando movimiento tactico.

### 7.5 IA del Sacerdote

Prioridades (de mayor a menor):

1. **Desparalizar aliados pretorianos** (20% de efectividad por intento)
2. **Paralizar al jugador mas cercano**
3. **Paralizar mascotas de jugadores** en rango
4. **Curar al pretoriano con menor HP**
5. **Atacar con Tormenta Avanzada** (si no hay nada mejor que hacer)

### 7.6 IA del Guerrero

Comportamiento directo de combate cuerpo a cuerpo:
- Persigue al jugador mas cercano
- Ataca fisicamente al llegar a rango melee
- Se reagrupa con el clan si no encuentra objetivo

### 7.7 IA del Cazador

Combate a distancia especializado:

**Seleccion de objetivo:**
- **Prioriza magos y clerigos** (son amenazas de alto dano pero baja defensa)
- Ataca con flechas magicas

**Posicionamiento:**
- Se mantiene a **media distancia** entre el rey y los atacantes
- Evita el contacto melee
- Si queda solo, se reagrupa con el ejercito

### 7.8 IA del Mago

La IA mas compleja y peligrosa del evento:

**Deteccion de invisibilidad:**
- **35% de probabilidad por tick** de detectar jugadores invisibles
- Esto hace que la invisibilidad sea poco confiable contra el mago pretoriano

**Seleccion de objetivo:**
- Prioriza jugadores **paralizados** (faciles de eliminar)
- Prioriza jugadores **invisibles** detectados

**Secuencia suicida ("Rotura de Vara"):**
- Se activa cuando sus HP bajan de **750**
- Lanza **Apocalipsis** repetidamente (hasta 6 cargas)
- El Apocalipsis es el hechizo mas devastador del juego (dano masivo AOE)
- El mago muere al final de la secuencia
- Esta mecanica castiga a los jugadores que lo dejan morir lentamente

**Posicionamiento:**
- Evita el combate cuerpo a cuerpo activamente
- Si un enemigo se acerca, **retrocede hacia el centro** del grupo
- Prefiere atacar a distancia

### 7.9 IA del Rey

El rey tiene **dos modos** completamente diferentes:

#### Modo Soporte (mientras tiene pretorianos vivos)

El rey actua como lider de soporte:
- **Desparaliza** aliados pretorianos (20% de efectividad)
- **Cura envenenamiento** de aliados
- **Cura heridas** de aliados
- Se mantiene al **maximo de HP** (se cura a si mismo)
- **NO ataca directamente**; deja el combate a su ejercito

#### Modo Berserker (cuando TODOS sus pretorianos mueren)

Cambio radical de comportamiento:
- Sale a **atacar cuerpo a cuerpo** agresivamente
- Tiene **velocidad aumentada**: puede atacar multiples veces por turno en las 4 direcciones
- **Persigue** al enemigo mas cercano con determinacion
- Si no puede alcanzar a un enemigo, le lanza **Ceguera** y **Estupidez** a distancia
- Es significativamente mas peligroso solo que con su ejercito

### 7.10 Estrategia Implicita

El disenio del evento crea un dilema tactico para los jugadores:

- **Matar primero al ejercito** es mas facil pero activa el modo berserker del rey
- **Matar primero al rey** es dificil porque los sacerdotes lo curan y los guerreros protegen
- **El mago explotando** (Apocalipsis) castiga a los grupos que no lo focalizan rapido
- **Las alcobas** obligan a reposicionarse, rompiendo formaciones estables

---

## 8. Datos Faccionarios del Personaje

Cada personaje almacena los siguientes datos faccionarios en su perfil:

| Campo | Descripcion |
|-------|-------------|
| ArmadaReal | Si pertenece al Ejercito Real (0/1) |
| FuerzasCaos | Si pertenece a la Legion Oscura (0/1) |
| CriminalesMatados | Total de criminales matados (para rangos del Real) |
| CiudadanosMatados | Total de ciudadanos matados (para rangos del Caos) |
| RecompensasReal | Contador de recompensas recibidas del Real |
| RecompensasCaos | Contador de recompensas recibidas del Caos |
| RecibioExpInicialReal | Ya recibio bonus de experiencia del Real |
| RecibioExpInicialCaos | Ya recibio bonus de experiencia del Caos |
| RecibioArmaduraReal | Ya recibio armadura del Real |
| RecibioArmaduraCaos | Ya recibio armadura del Caos |
| Reenlistadas | Veces que se re-enlisto (max 4) |
| NivelIngreso | Nivel al enlistarse |
| FechaIngreso | Fecha de ingreso a la faccion |
| MatadosIngreso | Kills al momento del ingreso |
| NextRecompensa | Proximo rango de recompensa |

---

## 9. Relacion con Otros Sistemas

### 9.1 Reputacion
- El Ejercito Real requiere reputacion Noble alta
- La Legion Oscura requiere ser criminal
- Los kills faccionarios afectan la reputacion (matar criminales = +Noble, matar ciudadanos = +Asesino)

### 9.2 Combate
- Las reglas de PvP se modifican por faccion (seguro de Armada, prohibicion de ataque Caos vs Caos)
- Los guardias de cada faccion protegen sus ciudades
- Los rangos altos habilitan combate en ciudades faccionarias

### 9.3 Clanes
- Los clanes pueden tener alineacion faccionaria (Real o del Mal)
- El cambio de faccion puede activar el sistema de antifaccion del clan
- Al nivel 25, los jugadores son expulsados de clanes faccionarios para elegir por si mismos

### 9.4 Inventario
- Las armaduras faccionarias son items exclusivos con restriccion de faccion
- Al ser expulsado, se desequipan automaticamente
- Solo se pueden vender a NPCs especificos (sastres)

### 9.5 NPCs
- Los guardias reales y del caos son NPCs faccionarios
- Los pretorianos son contenido PvE exclusivo de faccion
- Los NPCs nobles gestionan el alistamiento
