# 04 - Magia

> Este documento describe el sistema de magia de Argentum Online: tipos de hechizos, mecanica
> de casteo, calculo de dano magico, efectos de estado, invocaciones, requisitos, costos,
> bonificaciones por clase e instrumentos, y defensa magica.

---

## 1. Vision General

La magia es uno de los tres pilares del combate junto al melee y los proyectiles. A diferencia
del combate fisico que usa probabilidad de impacto, los hechizos tienen un modelo de "casteo
garantizado si se cumplen los requisitos": si el jugador tiene suficiente mana, skill, y el
objetivo es valido, el hechizo se lanza. El dano o efecto puede variar, pero no hay "miss" magico.

Cada hechizo tiene: nombre, descripcion, palabras magicas (texto visual al castear), efectos
visuales, sonidos, y un conjunto de propiedades que definen su comportamiento.

---

## 2. Clasificacion de Hechizos

### 2.1 Por Tipo de Objetivo (Target)

| Target | Descripcion | Ejemplo |
|--------|-------------|---------|
| Usuarios | Solo afectan jugadores | Curar, paralizar |
| NPCs | Solo afectan criaturas | Dano a NPC, paralizar NPC |
| Usuarios y NPCs | Pueden afectar a ambos | La mayoria de hechizos de dano |
| Terreno | Se lanzan sobre una posicion del mapa | Invocaciones, deteccion de invisibles |

### 2.2 Por Categoria Funcional

| Categoria | Identificador | Que hace |
|-----------|--------------|----------|
| Propiedades | uPropiedades | Modifica valores numericos (HP, mana, stamina, atributos) |
| Estado | uEstado | Modifica flags o condiciones (paralisis, invisibilidad, veneno) |
| Invocacion | uInvocacion | Crea criaturas en el mundo |

---

## 3. Requisitos para Lanzar un Hechizo

Los requisitos se verifican en este orden. Si alguno falla, el hechizo no se lanza:

1. **No estar muerto**
2. **Baculo requerido** (solo Magos): algunos hechizos exigen un nivel minimo de poder de baculo.
   Si el Mago no tiene baculo equipado o su poder es insuficiente, el hechizo falla
3. **Skill minimo de Magia**: cada hechizo tiene un `MinSkill` que define el nivel minimo
   requerido en el skill de Magia
4. **Stamina requerida**: algunos hechizos consumen stamina ademas de mana
5. **Mana requerida**: se verifica que el jugador tenga suficiente mana, aplicando bonificaciones
   de clase si corresponde
6. **Rango de vision**: el objetivo debe estar dentro del rango de vision vertical (6 tiles)
7. **Mapa compatible**: el mapa no debe tener la restriccion de "magia sin efecto"

---

## 4. Costo de Mana

### 4.1 Costo Base

Cada hechizo define un `ManaRequerido` como costo base.

### 4.2 Bonificaciones del Druida con Flauta Elfica

Los Druidas que tienen la Flauta Elfica equipada reciben descuentos significativos:

| Tipo de Hechizo | Descuento |
|-----------------|-----------|
| Mimetismo | 50% menos mana |
| Invocaciones | 30% menos mana |
| Resto de hechizos (excepto Apocalipsis) | 10% menos mana |
| Teletransporte de mascotas (Warp) | Consume TODA la mana (requiere barra completa) |

### 4.3 Caso Especial: Warp de Mascotas

El hechizo de teletransporte de mascotas es unico: requiere que el Druida tenga la barra de
mana **completamente llena** y consume TODA la mana al usarse. No importa cuanta mana tenga,
siempre la consume entera.

---

## 5. Hechizos de Propiedades (Dano, Curacion, Buffs)

### 5.1 Dano y Curacion de HP

Los hechizos que modifican HP tienen una flag `SubeHP`:
- **SubeHP = 1**: cura (restaura HP)
- **SubeHP = 2**: dana (quita HP)

#### Calculo de Dano Magico

**Dano base:** aleatorio entre MinHP y MaxHP del hechizo.

**Bonificacion por nivel del caster:**
```
Dano = DanoBase + DanoBase x 3% x NivelPersonaje
```
Es decir, a nivel 33 el dano magico es el doble del base. Esta bonificacion escala linealmente.

**Bonificacion por baculo (solo Magos, solo en hechizos marcados como StaffAffected):**
- Con baculo: `Dano = Dano x (70 + BonusDanoBaculo) / 100`
- Sin baculo: `Dano = Dano x 0.70` (penalizacion del 30%)

**Bonificacion por instrumentos (Bardos y Druidas):**
- Con Laud Elfico o Flauta Elfica: `Dano = Dano x 1.04` (+4% de dano)

#### Calculo de Curacion

**Curacion base:** aleatorio entre MinHP y MaxHP del hechizo.

La curacion aplica la misma bonificacion por nivel que el dano:
```
Curacion = CuracionBase + CuracionBase x 3% x NivelPersonaje
```

Las bonificaciones de baculo e instrumentos tambien aplican a la curacion.

### 5.2 Modificacion de Mana

Hechizos que restauran o drenan mana:
- **SubeMana = 1**: restaura mana (entre MinMana y MaxMana del hechizo)
- **SubeMana = 2**: drena mana

### 5.3 Modificacion de Stamina

Hechizos que restauran o drenan energia:
- **SubeSta = 1**: restaura stamina (entre MinSta y MaxSta del hechizo)
- **SubeSta = 2**: drena stamina

### 5.4 Modificacion de Hambre y Sed

Algunos hechizos pueden restaurar o quitar hambre/sed:
- **SubeHam / SubeSed**: restaura o quita los puntos correspondientes

### 5.5 Buffs de Atributos

Los hechizos pueden modificar temporalmente Agilidad, Fuerza o Carisma:

| Efecto | Duracion | Restriccion |
|--------|----------|-------------|
| Aumento de atributo | 1200 ticks del game loop | No puede superar el doble del valor base ni el maximo global (40) |
| Disminucion de atributo | 700 ticks del game loop | No puede bajar del minimo global (6) |

Los buffs son temporales y se restauran automaticamente al expirar, al morir, o al recibir
otro buff del mismo tipo.

---

## 6. Hechizos de Estado

Los hechizos de estado modifican flags del personaje, activando o desactivando condiciones.

### 6.1 Invisibilidad

**Efecto:** el personaje se vuelve invisible para otros jugadores y NPCs.

**Restricciones:**
- No funciona en mapas con "invisibilidad sin efecto"
- Se pierde al hablar palabras magicas (lanzar otro hechizo)
- Se pierde al atacar fisicamente
- Tiene duracion limitada (configurable via IntervaloInvisible)

**Diferencia con Ocultarse (skill):** la invisibilidad por hechizo y el ocultamiento por skill
son mecanismos diferentes con duraciones y reglas de cancelacion propias. Ambos hacen al personaje
invisible, pero el ocultamiento depende del skill y no de la mana.

### 6.2 Mimetismo

**Efecto:** copia la apariencia visual completa del objetivo (cuerpo, cabeza, arma, escudo, casco).

**Restricciones:**
- Solo los **Druidas** pueden mimetizarse con NPCs
- El druida mimetizado como NPC es **ignorado por criaturas hostiles** hasta que ataque
- Solo se puede estar mimetizado una vez a la vez (un nuevo mimetismo reemplaza al anterior)
- Tiene duracion limitada
- Se guarda la apariencia original para restaurarla al expirar

### 6.3 Paralisis

**Efecto:** impide todo movimiento y toda accion. El personaje queda completamente inmovilizado.

**Duracion:** configurada por IntervaloParalizado (en ticks del game loop).

**Resistencia:** el Super Anillo (item especifico) rechaza completamente la paralisis.

### 6.4 Inmovilizacion

**Efecto:** impide el movimiento pero **permite realizar acciones** (puede atacar, lanzar hechizos,
usar objetos). Solo puede actuar en la direccion que esta mirando.

**Duracion:** misma que la paralisis.

**Resistencia:** el Super Anillo tambien la rechaza.

**Diferencia con paralisis:** la paralisis bloquea todo; la inmovilizacion solo bloquea el movimiento.

### 6.5 Ceguera

**Efecto:** oscurece la pantalla del jugador objetivo, dificultando enormemente la orientacion.

**Duracion:** un tercio del tiempo de paralisis.

### 6.6 Estupidez (Aturdimiento)

**Efecto:** confunde al personaje, invirtiendo o aleatorizando sus controles de movimiento.

**Duracion:** tiempo completo de paralisis.

**Resistencia:** el Super Anillo la rechaza.

### 6.7 Envenenamiento

**Efecto:** aplica el efecto de veneno (dano periodico de 1-5 HP cada N ticks).

**Diferencia con envenenamiento de arma:** el envenenamiento por hechizo es garantizado (no hay
probabilidad, siempre se aplica si el hechizo impacta). El de arma tiene 60% de probabilidad.

### 6.8 Cura de Veneno

**Efecto:** remueve el estado de envenenamiento del objetivo.

### 6.9 Remocion de Paralisis

**Efecto:** remueve los estados de paralisis e inmovilizacion del objetivo.

### 6.10 Remocion de Estupidez

**Efecto:** remueve el estado de estupidez/aturdimiento.

### 6.11 Maldicion

**Efecto:** aplica un estado de maldicion al objetivo.

### 6.12 Remocion de Maldicion

**Efecto:** quita la maldicion del objetivo.

### 6.13 Bendicion

**Efecto:** aplica un estado de bendicion al objetivo.

### 6.14 Resurreccion

**Efecto:** revive a un personaje muerto. Es uno de los hechizos mas complejos del juego.

**Requisitos del caster:**
- Stamina llena (barra completa)
- Mago: baculo con poder suficiente
- Bardo: Laud Elfico o Laud Magico equipado
- Druida: Flauta Elfica o Flauta Magica equipada

**Requisitos del objetivo:**
- Debe estar muerto
- El mapa no debe tener "resurreccion sin efecto"

**Efectos sobre el resucitado:**
- Recupera HP igual a su Constitucion (con tope en MaxHP)
- Mana, stamina, hambre y sed quedan en 0
- Restaura su apariencia original (deja de ser fantasma)

**Costo para el caster:**
```
HP_restante = HP_actual x (1 - NivelResucitado x 0.015)
```

Tabla de ejemplo del costo:

| Nivel del resucitado | HP que pierde el caster |
|---------------------|------------------------|
| 10 | 15% de su HP |
| 20 | 30% de su HP |
| 33 | ~50% de su HP |
| 48 | 72% de su HP |
| 66+ | Probablemente muere |

**Si el caster muere por el esfuerzo**, el hechizo no se cuenta como exitoso (pero el resucitado
ya fue revivido).

**Reputacion:** resucitar a un ciudadano otorga 500 puntos de Nobleza al caster.

---

## 7. Hechizos de Invocacion

### 7.1 Invocacion de Criaturas

**Efecto:** crea uno o mas NPCs (criaturas) en posiciones cercanas al punto objetivo.

**Caracteristicas de las criaturas invocadas:**
- Siguen al jugador como mascotas
- Tienen un tiempo de existencia limitado (configurado por IntervaloInvocacion)
- No dan oro al morir
- El maximo de mascotas simultaneas es 3 (MAXMASCOTAS)

**Restricciones:**
- Solo se pueden invocar en zonas PK (no en zonas seguras)
- Si ya se tiene el maximo de mascotas, la invocacion falla
- El hechizo define que tipo de NPC se invoca y cuantos

### 7.2 Teletransporte de Mascotas (Warp)

**Efecto:** trae a la mascota mas lejana cerca del druida.

**Requisito especial:** consume TODA la mana del caster (requiere barra completa).

**Uso tipico:** cuando una mascota queda atrapada o alejada del druida.

---

## 8. Deteccion de Invisibilidad (AOE)

Existe un hechizo especial de tipo terreno que afecta un area:

**Efecto:** revela temporalmente la posicion de personajes invisibles en un area de **8 tiles**
en todas las direcciones desde el punto objetivo.

**Importante:** NO remueve la invisibilidad. Solo muestra un efecto visual sobre los personajes
invisibles en el area, revelando su posicion temporal. El personaje sigue siendo invisible
despues del efecto.

---

## 9. Defensa Magica

### 9.1 Defensa Magica de Jugadores

Los jugadores pueden reducir el dano magico recibido con:

- **Cascos**: pueden tener DefensaMagicaMin/Max que se restan del dano
- **Anillos**: pueden tener DefensaMagicaMin/Max que se restan del dano

Ambas defensas se aplican (se suman y se restan del dano total).

### 9.2 El Super Anillo

Un item especifico (el Super Anillo) rechaza completamente los siguientes efectos magicos:
- Paralisis
- Inmovilizacion
- Estupidez

No reduce dano magico; solo bloquea estos tres efectos de estado especificos.

### 9.3 Defensa Magica de NPCs

Los NPCs tienen un stat de defensa magica (`defM`) que se resta directamente del dano magico
recibido. A diferencia de los jugadores que tienen rango min/max, los NPCs tienen un valor fijo.

---

## 10. Hechizos de NPCs

### 10.1 Que NPCs Pueden Lanzar Hechizos

Los NPCs con la propiedad `LanzaSpells` tienen una lista de hechizos disponibles. Al atacar,
seleccionan un hechizo aleatorio de su lista.

### 10.2 Tipos de Hechizos de NPC

Los NPCs pueden lanzar:
- Hechizos de **dano** (SubeHP == 2)
- **Paralisis** e **inmovilizacion**
- **Estupidez** (aturdimiento)
- **Curacion** (SubeHP == 1, se curan a si mismos o a aliados)

### 10.3 Restricciones de NPCs al Castear

- No pueden lanzar magia en mapas con "magia sin efecto"
- No pueden afectar a jugadores invisibles u ocultos
- No pueden afectar a jugadores muertos
- No pueden afectar a jugadores en consulta con GM o protegidos por timer

### 10.4 Doble Ataque con Magia

Los NPCs con la propiedad "AtacaDoble" tienen un 50% de chance de lanzar un hechizo **en lugar
del segundo golpe fisico**. Es decir, primero golpean fisicamente, y luego en lugar de un
segundo golpe, lanzan un hechizo.

### 10.5 NPCs Magicos Especiales

Algunos NPCs tienen comportamiento magico unico:
- **Elemental de agua**: no ataca cuerpo a cuerpo, solo lanza hechizos
- **Elemental de fuego**: lanza hechizos a distancia, puede combatir NPC vs NPC con magia
- **Dragones**: si son atacados por un elemental de fuego, contraatacan con magia

---

## 11. Mensajes de Hechizo

Cada hechizo define tres mensajes de texto:

| Mensaje | Quien lo ve | Ejemplo |
|---------|-------------|---------|
| HechizeroMsg | El caster | "Lanzas una bola de fuego" |
| TargetMsg | El objetivo | "Una bola de fuego te golpea" |
| PropioMsg | Cuando el caster se lanza el hechizo a si mismo | "Te curas con energia magica" |

Ademas, al lanzar un hechizo se muestran las **palabras magicas** como texto sobre la cabeza
del caster. Esto revela que el jugador esta casteando (y rompe la invisibilidad).

---

## 12. Efectos Visuales y Sonoros

Cada hechizo puede definir:
- **FXgrh**: numero de grafico del efecto visual
- **loops**: cuantas veces se repite la animacion
- **WAV**: numero de efecto de sonido al lanzar

Estos efectos se envian a todos los jugadores en el area del objetivo.

---

## 13. Relacion con Otros Sistemas

### 13.1 Combate

- Los hechizos de dano comparten cooldowns cruzados con el ataque fisico
- La magia no tiene probabilidad de "miss" como el combate fisico (siempre impacta si se cumplen requisitos)
- La defensa magica es independiente de la defensa fisica

### 13.2 Inventario

- Los baculos (armas) potencian los hechizos del Mago
- Los instrumentos magicos (anillos) habilitan y potencian hechizos del Bardo y Druida
- Los pergaminos ensenan hechizos nuevos
- El Super Anillo confiere inmunidad a ciertos efectos

### 13.3 Personaje

- El mana es el recurso principal de la magia (regenerado por meditacion)
- La Inteligencia afecta la ganancia de mana por nivel
- El skill de Magia determina que hechizos se pueden lanzar
- El nivel del personaje escala directamente el dano/curacion magica

### 13.4 NPCs

- Las mascotas invocadas son NPCs con comportamiento propio
- Los NPCs magicos usan el mismo sistema de hechizos
- La defensa magica de NPCs usa un calculo diferente (valor fijo vs rango)
