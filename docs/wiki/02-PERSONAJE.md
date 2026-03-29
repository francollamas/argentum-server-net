# 02 - Personaje

> Este documento describe todo lo relacionado al personaje del jugador: creacion, atributos,
> clases, razas, niveles, habilidades, muerte, resurreccion, y reputacion. El personaje es
> la entidad central del juego; casi todos los demas sistemas operan sobre el.

---

## 1. Creacion de Personaje

### 1.1 Datos Requeridos

Para crear un personaje nuevo, el jugador debe definir:

1. **Nombre**
2. **Raza**
3. **Genero**
4. **Clase**
5. **Cabeza** (apariencia facial)
6. **Ciudad natal**
7. **Atributos** (dados)

Cada uno tiene sus propias validaciones y restricciones detalladas a continuacion.

### 1.2 Nombre

- Solo se aceptan caracteres alfabeticos ASCII (a-z, A-Z) y espacios
- No puede estar vacio
- Se valida contra una lista de **nombres prohibidos** cargada al inicio del servidor
- No puede repetir el nombre de un personaje ya existente
- La verificacion de existencia se hace contra el sistema de persistencia (si ya existe un archivo de ese personaje, se rechaza)

### 1.3 Raza

Existen 5 razas jugables:

| Raza | Tamano | Nota |
|------|--------|------|
| Humano | Normal | Raza estandar, equilibrada |
| Elfo | Normal | Agil, buen caster |
| Elfo Oscuro (Drow) | Normal | Variante del elfo |
| Gnomo | Pequeno | Usa modelo de cuerpo mas bajo |
| Enano | Pequeno | Usa modelo de cuerpo mas bajo, robusto |

Cada raza aplica **modificadores flotantes** sobre los 5 atributos base del personaje al momento
de la creacion. Estos modificadores se cargan desde un archivo de balance externo, lo que permite
ajustarlos sin recompilar.

Los Enanos y Gnomos comparten un modelo visual mas pequeno, lo que tambien afecta las animaciones
de armas (tienen un set propio de animaciones).

### 1.4 Genero

Dos opciones: **Hombre** o **Mujer**.

El genero afecta:
- El set de cabezas disponibles (cada combinacion raza+genero tiene un rango propio)
- Ciertos items de equipamiento estan restringidos por genero
- El modelo visual base del personaje

### 1.5 Clase

Existen **12 clases** jugables. Cada clase define un perfil completamente diferente de progresion,
combate, y habilidades disponibles.

#### Resumen de Clases

| Clase | Perfil | Mana Inicial | Mana/Nivel | HP/Nivel | STA/Nivel | Habilidad Especial |
|-------|--------|-------------|------------|----------|-----------|-------------------|
| Mago | Caster puro | INT x 3 | 2.8 x INT | 1 | 14 | Requiere baculo para hechizos potentes |
| Clerigo | Soporte magico | 50 | 2 x INT | 2 | 15 | Puede apunalar |
| Guerrero | Melee puro | 0 | 0 | 3 (2 post-35) | 15 | +2 HP extra al subir de nivel |
| Asesino | Melee/sigilo | 50 | INT | 3 (1 post-35) | 15 | Apunalar con dano x1.4 |
| Ladron | Sigilo/robo | 0 | 0 | 2 | 18 | Paralisis manual, desarmar, robar objetos |
| Bardo | Soporte mixto | 50 | 2 x INT | 2 | 15 | Usa instrumentos magicos para resucitar |
| Druida | Naturaleza | 50 | 2 x INT | 2 | 15 | Mimetismo con NPCs, mejor domador |
| Bandido | Melee/magia oscura | 50 | INT/3 x 2 | 3 (1 post-35) | 38 | Golpe critico con Espada Vikinga, hurto de items |
| Paladin | Melee/magia | 0 | INT | 3 (1 post-35) | 15 | Puede apunalar, recibe pociones azules al crear |
| Cazador | Distancia | 0 | 0 | 3 (2 post-35) | 15 | +1 HP extra, ocultamiento permanente con skill alto |
| Trabajador | Oficios | 0 | 0 | 2 | 40 | Gasta mucho menos energia al trabajar, mejores ratios de crafting |
| Pirata | Naval/melee | 0 | 0 | 3 | 15 | Acuchillar, mejor navegacion, se oculta como galeon fantasmal |

#### Detalle por Clase

**Mago**
- Unica clase que comienza con mana igual a Inteligencia x 3
- Mayor ganancia de mana por nivel (2.8 x INT)
- Menor ganancia de HP por nivel (1 punto de golpe)
- Requiere baculos para maximizar el dano magico; sin baculo, el dano se reduce un 30%
- Algunos hechizos avanzados exigen un nivel minimo de poder de baculo

**Clerigo**
- Comienza con 50 de mana
- Buena ganancia de mana (2 x INT) y HP moderado
- Puede apunalar (ataque sorpresa con dano multiplicado)
- Clase semiguerrera con capacidad de soporte

**Guerrero**
- Sin mana, orientado 100% al combate fisico
- La mejor progresion de HP: 3 puntos de golpe por nivel (reducido a 2 despues del nivel 35)
- Recibe 2 puntos de HP adicionales al subir de nivel (bonus exclusivo)
- Resistente y constante en combate

**Asesino**
- Clase hibrida con algo de mana (50 inicial, gana INT por nivel)
- Buena progresion de golpe (3, reducido a 1 post-35)
- La mejor habilidad de apunalar: dano x1.4 contra jugadores (las demas clases que pueden apunalar hacen x1.5, que paradojicamente es MAS dano, pero el Asesino tiene MAYOR PROBABILIDAD de activarlo)
- Contra NPCs, el apunal hace siempre x2 sin importar la clase

**Ladron**
- Sin mana, especializado en sigilo y robo
- Progresion de HP moderada (2 puntos de golpe)
- Stamina por nivel: 18 (energia especial de ladron)
- Habilidades unicas en PvP:
  - Puede paralizar al golpear con guantes de hurto (50% de la duracion normal)
  - Puede desarmar al oponente (quitarle el arma equipada)
  - Es la unica clase que puede robar OBJETOS del inventario ajeno (las demas solo roban oro)

**Bardo**
- Clase de soporte con mana decente (50 inicial, 2 x INT por nivel)
- Usa instrumentos magicos (Laud Elfico, Laud Magico) para ciertos hechizos como resucitar
- +4% de dano magico al equipar un Laud Elfico
- Progresion equilibrada

**Druida**
- Similar al Bardo en progresion de stats
- Habilidad exclusiva de **mimetismo**: puede copiar la apariencia de NPCs
- Cuando esta mimetizado como NPC, las criaturas hostiles lo ignoran completamente
- El mejor domador del juego (modificador de domar = 6, vs 10 de las clases comunes)
- Con Flauta Elfica equipada: 50% menos mana en mimetismo, 30% menos en invocaciones, 10% menos en otros hechizos
- Comienza con un hechizo adicional (hechizo de inicio especial)

**Bandido**
- Hibrido melee/magia con buena stamina (38 por nivel)
- Mana limitada (50 inicial, gana INT/3 x 2 por nivel)
- Habilidad exclusiva de **golpe critico** con la Espada Vikinga: dano adicional del 75% del golpe normal
- Con Guantes de Hurto equipados: puede desequipar al oponente (escudo, arma, casco, en ese orden) y hurtar objetos de su inventario
- Se oculta por la mitad del tiempo de las demas clases

**Paladin**
- Sin mana inicial, pero gana INT por nivel (progresion tardia de mana)
- Buena progresion de golpe (3, reducido a 1 post-35)
- Puede apunalar
- Al crearse, recibe pociones azules (mana) como item inicial

**Cazador**
- Sin mana, especializado en combate a distancia
- Buena progresion de HP (3, reducido a 2 post-35)
- Recibe 1 punto de HP adicional al subir de nivel (bonus exclusivo, menos que el Guerrero)
- Comienza con arco y flechas
- Habilidad unica: con skill de Ocultarse > 90 y una armadura especifica, puede permanecer oculto **indefinidamente**

**Trabajador**
- La mayor stamina del juego: 40 puntos por nivel
- Consume dramaticamente menos energia al trabajar: 2 puntos vs 6 de las demas clases
- Tiene multiplicador x1 en todos los oficios (herreria, carpinteria, mineria), mientras las demas clases tienen x3 o x4
- Esto significa que necesita MUCHO menos skill para craftear los mismos items
- Progresion de combate pobre (2 puntos de golpe, sin mana)

**Pirata**
- La unica clase con progresion de golpe constante (3 puntos por nivel, SIN reduccion post-35)
- Habilidad exclusiva de **acuchillar**: funciona con armas arrojadizas y cuerpo a cuerpo
- Modificador de navegacion x1 (las demas clases x2), navega con la mitad del skill requerido
- Al ocultarse mientras navega, se transforma en un galeon fantasmal en lugar de hacerse invisible

### 1.6 Cabeza (Apariencia Facial)

El jugador selecciona una cabeza de un catalogo. Cada combinacion de raza + genero tiene un rango
de cabezas valido:

| Raza | Hombre | Mujer |
|------|--------|-------|
| Humano | 1 - 40 | 70 - 89 |
| Elfo | 101 - 122 | 170 - 188 |
| Elfo Oscuro | 201 - 221 | 270 - 288 |
| Enano | 301 - 319 | 370 - 384 |
| Gnomo | 401 - 416 | 470 - 484 |

El servidor valida que la cabeza seleccionada este dentro del rango correspondiente.

### 1.7 Ciudad Natal (Hogar)

Determina la posicion inicial del personaje en el mundo. Las ciudades disponibles son:

| Ciudad | Descripcion |
|--------|-------------|
| Ullathorpe | Ciudad principal y por defecto |
| Nix | Segunda ciudad |
| Banderbill | Tercera ciudad |
| Lindos | Cuarta ciudad |
| Arghal | Quinta ciudad |

Cada ciudad tiene coordenadas fijas (mapa, X, Y) cargadas desde un archivo de datos.
Si no se selecciona ciudad, se usa Ullathorpe por defecto.

### 1.8 Atributos Iniciales (Dados)

Antes de crear el personaje, el jugador debe "tirar los dados". Los 5 atributos se determinan
aleatoriamente y luego se les aplican los modificadores de la raza seleccionada.

Si la Fuerza resultante es 0, el servidor rechaza la creacion (significa que no se tiraron los dados).

Los atributos tienen un rango global de **6 a 40** puntos.

### 1.9 Items Iniciales por Clase

Al crearse, el personaje recibe items iniciales que varian segun la clase:

- **Paladin**: recibe pociones azules (mana)
- **Cazador**: recibe arco y flechas
- **Druida**: recibe un hechizo adicional (ademas de los comunes)
- **Todas las clases**: reciben items newbie basicos (ropa, arma basica, comida)

---

## 2. Atributos

### 2.1 Los 5 Atributos

| Atributo | Abreviatura | Efecto Principal |
|----------|-------------|-----------------|
| Fuerza | FUE | Dano fisico (bonus cuando supera 15) |
| Agilidad | AGI | Evasion, stamina inicial, velocidad percibida |
| Inteligencia | INT | Mana por nivel (clases magicas), resistencia a interrumpir meditacion |
| Carisma | CAR | Domar criaturas, liderazgo |
| Constitucion | CON | HP inicial, HP ganado por nivel |

### 2.2 Rangos

- **Minimo absoluto**: 6 puntos
- **Maximo absoluto**: 40 puntos
- **Maximo con pociones**: el doble del valor base (si la base es 20, el maximo temporal es 40)

### 2.3 Impacto en Stats Iniciales

- **HP inicial**: 15 + aleatorio(1, CON/3)
- **Mana inicial**: depende de la clase (0, 50, o INT x 3 para Magos)
- **Stamina inicial**: 20 x aleatorio(1, AGI/6)

### 2.4 Modificacion Temporal por Pociones

Las pociones de atributos (Agilidad o Fuerza) aumentan el atributo temporalmente.
Restricciones:
- Un atributo no puede exceder el doble de su valor base original
- No puede exceder el maximo global (40)
- Al morir, los atributos se restauran a sus valores base
- Las pociones tienen duracion limitada en ticks; al expirar, se restauran los valores base
- Se guardan los valores base en un backup para poder restaurarlos

---

## 3. Stats Vitales

### 3.1 HP (Puntos de Vida)

- **Inicial**: 15 + aleatorio(1, CON/3)
- **Maximo**: 999
- **Ganancia por nivel**: distribucion probabilistica basada en la clase y la constitucion
  - La formula usa un valor base por clase (ModVida) ajustado por constitucion:
    `ModVida[clase] - (21 - CON) * 0.5`
  - Luego se aplica una distribucion probabilistica con 5 niveles de ganancia (E1-E5 para
    enteros, S1-S4 para semi-enteros) cargados desde el archivo de balance
  - Esto introduce variabilidad: dos personajes identicos pueden tener HP ligeramente diferentes
- **Bonus de clase**: Guerrero gana +2 HP extra, Cazador gana +1 HP extra al subir de nivel
- **Regeneracion**: ocurre automaticamente con el tiempo si el personaje no tiene hambre ni sed
  (ver sistema de regeneracion en Arquitectura General)
- **Al morir**: HP queda en 0
- **Al resucitar**: HP = Constitucion (con tope en MaxHP)

### 3.2 Mana

- **Inicial**: depende de la clase (ver tabla de clases)
- **Maximo**: 9,999
- **Ganancia por nivel**: depende de la clase y la Inteligencia
  - Mago: 2.8 x INT
  - Clerigo, Bardo, Druida: 2 x INT
  - Asesino, Paladin: INT
  - Bandido: INT/3 x 2
  - Guerrero, Ladron, Cazador, Trabajador, Pirata: 0
- **Regeneracion**: solo por meditacion activa (ver seccion 5.5)
- **Al resucitar**: mana queda en 0

### 3.3 Stamina (Energia)

- **Inicial**: 20 x aleatorio(1, AGI/6)
- **Maximo**: 999
- **Ganancia por nivel**: valor fijo segun la clase
  - Trabajador: 40
  - Bandido: 38
  - Ladron: 18
  - Guerrero, Clerigo, Bardo, Druida, Asesino, Paladin, Cazador, Pirata: 15
  - Mago: 14
- **Consumo**: atacar (1-10 aleatorio), trabajar (2 o 6 segun clase), robar (15), moverse en ciertos terrenos
- **Regeneracion**: automatica con el tiempo si no tiene hambre ni sed
- **Al morir**: stamina queda en 0

### 3.4 Hambre

- **Inicial**: 100/100
- **Decremento**: cada N ticks del game loop, se reduce 10 puntos
- **Efecto de llegar a 0**: activa flag de hambriento. Mientras tenga hambre:
  - No puede entrenar habilidades
  - No regenera HP ni stamina
- **Restauracion**: comiendo alimentos (items de tipo "uso unico" que restauran hambre)

### 3.5 Sed

- **Inicial**: 100/100
- **Decremento**: cada N ticks del game loop, se reduce 10 puntos
- **Efecto de llegar a 0**: activa flag de sediento. Mismas consecuencias que el hambre
- **Restauracion**: bebiendo (items de tipo "bebida" o botellas llenas)

---

## 4. Sistema de Niveles y Experiencia

### 4.1 Nivel Maximo

El nivel maximo tecnico es 255, pero el gameplay efectivo esta disenado para ~50 niveles.

### 4.2 Experiencia para Subir de Nivel (ELU)

La experiencia requerida para subir comienza en 300 y escala con un multiplicador que decrece
gradualmente:

| Rango de Niveles | Multiplicador |
|-----------------|---------------|
| 1 - 14 | x 1.4 |
| 15 - 20 | x 1.35 |
| 21 - 32 | x 1.3 |
| 33 - 40 | x 1.225 |
| 41+ | x 1.25 |

Ejemplo de progresion:
- Nivel 1 a 2: 300 EXP
- Nivel 2 a 3: 420 EXP (300 x 1.4)
- Nivel 3 a 4: 588 EXP (420 x 1.4)
- ...y asi sucesivamente

Experiencia maxima acumulable: 99,999,999.

### 4.3 Fuentes de Experiencia

| Fuente | Experiencia |
|--------|-------------|
| Matar un NPC | Proporcional al dano infligido (ver Combate) |
| Matar otro jugador (PvP) | Nivel de la victima x 2 |
| Subir un punto de skill | 50 puntos de experiencia |

### 4.4 Al Subir de Nivel

Cuando la experiencia acumulada alcanza o supera el ELU, el personaje sube de nivel:

1. **HP**: se incrementa segun la distribucion probabilistica de la clase/constitucion
2. **Mana**: se incrementa segun la formula de la clase
3. **Stamina**: se incrementa un valor fijo segun la clase
4. **Golpe (HIT min/max)**: se incrementa segun la clase:
   - Guerrero, Asesino, Bandido, Paladin, Cazador, Pirata: +3 (reducido post-35 para algunos)
   - Clerigo, Bardo, Druida, Ladron, Trabajador: +2
   - Mago: +1
   - Los incrementos de golpe se reducen despues del nivel 35 para ciertas clases
5. **Skill Points**: +5 puntos de habilidad asignables (el primer nivel da 10)
6. **Recuperacion total de vida**: HP se restaura al maximo al subir

### 4.5 Topes de Stats

| Stat | Maximo | Nota |
|------|--------|------|
| HP | 999 | |
| Mana | 9,999 | |
| Stamina | 999 | |
| HIT (golpe) | 99 (bajo nivel 36) / 999 (nivel 36+) | El tope sube a partir del nivel 36 |
| Defensa | 99 | |

### 4.6 Evento de Nivel 25

Al alcanzar el nivel 25, si el personaje pertenece a un clan con alineacion faccionaria (Real o
del Mal), es **expulsado automaticamente** del clan. Esto fuerza al jugador a elegir faccion por
si mismo al llegar al nivel requerido para enlistarse.

---

## 5. Sistema de Habilidades (Skills)

### 5.1 Lista de Skills

El juego tiene **20 habilidades**:

| # | Skill | Categoria | Uso Principal |
|---|-------|-----------|---------------|
| 1 | Magia | Combate | Lanzar hechizos, efectividad magica |
| 2 | Robar | Social/PvP | Robar items y oro a otros jugadores |
| 3 | Tacticas | Combate | Evasion en combate, se sube al esquivar |
| 4 | Armas | Combate | Precision con armas cuerpo a cuerpo |
| 5 | Meditar | Soporte | Regenerar mana activamente |
| 6 | Apunalar | Combate | Ataque sorpresa con dano multiplicado |
| 7 | Ocultarse | Sigilo | Volverse invisible temporalmente |
| 8 | Supervivencia | Utilidad | Crear fogatas, resistencia ambiental |
| 9 | Talar | Oficio | Cortar arboles para obtener madera |
| 10 | Comerciar | Economia | Mejores precios en tiendas NPC |
| 11 | Defensa | Combate | Bloqueo con escudo |
| 12 | Pesca | Oficio | Pescar con cana o red |
| 13 | Mineria | Oficio | Extraer minerales y fundir lingotes |
| 14 | Carpinteria | Oficio | Construir objetos de madera |
| 15 | Herreria | Oficio | Forjar armas y armaduras de metal |
| 16 | Liderazgo | Social | Crear y liderar parties |
| 17 | Domar | Utilidad | Domesticar criaturas como mascotas |
| 18 | Proyectiles | Combate | Precision con arcos y armas a distancia |
| 19 | Wrestling | Combate | Combate sin armas (punos) |
| 20 | Navegacion | Transporte | Manejar embarcaciones |

### 5.2 Progresion de Skills

- **Rango**: 0 a 100 puntos por skill
- **Tope por nivel**: existe una tabla que limita el skill maximo segun el nivel del personaje.
  Un personaje de bajo nivel no puede tener skills altos, aunque tenga los puntos
- **Experiencia de skill**: cada skill tiene su propia barra de experiencia
  - Al acertar una accion de skill: +50 EXP de skill
  - Al fallar una accion de skill: +20 EXP de skill
  - Cuando la EXP de skill alcanza el umbral (ELU del skill), el skill sube 1 punto
- **Al subir un skill**: se ganan 50 puntos de experiencia general del personaje

### 5.3 Skill Points

- Al subir de nivel, el jugador recibe **5 puntos de habilidad** asignables libremente
- El primer nivel otorga **10 puntos** en lugar de 5
- Los puntos se asignan a cualquier skill, incrementandolo directamente
- Existe un sistema de proteccion que verifica que no se asignen mas puntos de los disponibles

### 5.4 Restricciones para Entrenar Skills

Para que un skill suba por uso (no por asignacion de puntos), se requiere:
- **No tener hambre** (hambre > 0)
- **No tener sed** (sed > 0)
- **Haber asignado los skill points iniciales** (los 10 puntos del primer nivel)

### 5.5 Meditacion (Skill 5)

La meditacion es el unico mecanismo de regeneracion de mana. Funcionalidad:

- El jugador activa la meditacion manualmente
- Hay un **delay de 2 segundos** antes de que empiece a surtir efecto
- Cada tick del game loop, si esta meditando, regenera mana
- La cantidad de mana regenerada depende del nivel de skill de Meditar y del nivel del personaje
- Se muestra un efecto visual de meditacion que varia segun el nivel:
  - Skill bajo: efecto pequeno
  - Skill medio: efecto mediano
  - Skill alto: efecto grande
  - Skill muy alto: efecto extra grande
  - Skill maximo: efecto maximo
- **Interrupciones**: la meditacion se cancela si:
  - El jugador se mueve
  - El jugador ataca
  - El jugador recibe dano que supera un umbral basado en: MinHP, Inteligencia, skill de Meditar, y un factor aleatorio
  - El jugador lanza un hechizo

---

## 6. Muerte y Resurreccion

### 6.1 Al Morir

Cuando un personaje muere, se ejecuta la siguiente secuencia:

1. **HP a 0, Stamina a 0**
2. **Se limpian todos los estados**: veneno, paralisis, estupidez, invisibilidad, mimetismo, meditacion, descanso
3. **Se pierde TODO el inventario**: todos los objetos caen al piso en la posicion del personaje
   - **Excepcion newbie**: los newbies solo pierden objetos no-newbie (conservan los newbie)
   - **Excepcion arena**: en zonas de pelea (trigger 6), NO se pierden objetos
4. **Se desequipa TODO**: arma, armadura, casco, escudo, anillo, municion
5. **Se eliminan todas las mascotas** (mueren inmediatamente)
6. **La apariencia cambia**: el cuerpo se convierte en un fantasma, la cabeza en cabeza de fantasma. Si estaba navegando, se transforma en una fragata fantasmal
7. **Los atributos temporales se restauran**: si habia tomado pociones, vuelve a sus valores base
8. **Se activa un "seguro de resurreccion"**: impide que te ataquen inmediatamente al resucitar (excepto en arenas)
9. **Se cancela el comercio seguro activo** (si tenia uno en curso)
10. **Penalizacion de experiencia en party**: el grupo del jugador sufre una penalizacion

### 6.2 Como Fantasma

Mientras esta muerto, el personaje:
- Puede moverse por el mundo como un fantasma
- No puede atacar, usar objetos, lanzar hechizos, ni comerciar
- No puede ser atacado
- Puede usar el comando /home para viajar a su ciudad natal (tarda 30 segundos, debe quedarse quieto)
- Puede ser resucitado por otro jugador (hechizo de resurreccion) o por un NPC sacerdote

### 6.3 Resurreccion

Al resucitar (por hechizo de otro jugador o por NPC):
- **HP**: se restaura a un valor igual a la Constitucion del personaje (con tope en MaxHP)
- **Mana, Stamina**: quedan en 0
- **Hambre y Sed**: quedan en 0 (hambriento y sediento)
- **Apariencia**: vuelve al cuerpo normal desnudo y cabeza original
- **Si estaba navegando**: recupera la apariencia de barco correspondiente
- **Se cancela el viaje a casa** si estaba en progreso

### 6.4 Resurreccion por Hechizo

La resurreccion por hechizo tiene requisitos y costos especiales para el caster:

- El caster debe tener la barra de stamina llena
- **Mago**: necesita baculo con poder suficiente
- **Bardo**: necesita Laud Elfico o Laud Magico equipado
- **Druida**: necesita Flauta Elfica o Flauta Magica equipada
- **Costo para el caster**: pierde HP proporcionalmente al nivel del resucitado:
  `HP = HP x (1 - NivelResucitado x 0.015)`
  Esto significa que resucitar a un personaje de nivel 48+ puede MATAR al caster
- Si el caster muere por el esfuerzo, el hechizo no se cuenta como exitoso
- Resucitar a un ciudadano otorga 500 puntos de Nobleza al caster
- No funciona en mapas con "resurreccion sin efecto"

### 6.5 Resurreccion por NPC

Los NPCs de tipo "Revividor" (sacerdotes) resucitan al jugador automaticamente al interactuar.
Los NPCs de tipo "Resucitador Newbie" solo resucitan jugadores por debajo del nivel 12.

---

## 7. Sistema de Reputacion

### 7.1 Los 6 Contadores

Cada personaje tiene 6 contadores de reputacion independientes:

| Contador | Tipo | Se gana por | Valor Inicial |
|----------|------|-------------|---------------|
| Noble | Positivo | Matar criminales, resucitar ciudadanos | 1,000 |
| Burgues | Positivo | Acciones de ciudadania | 0 |
| Plebe | Positivo | Trabajar (pescar, minar, construir) | 30 |
| Ladron | Negativo | Robar | 0 |
| Bandido | Negativo | Atacar ciudadanos | 0 |
| Asesino | Negativo | Matar ciudadanos inocentes | 0 |

Maximo absoluto de cualquier contador: 6,000,000.

### 7.2 Promedio y Clasificacion

El promedio se calcula como:
```
Promedio = (Noble + Burgues + Plebe - Ladron - Bandido - Asesino) / 6
```

- **Promedio >= 0**: el personaje es **Ciudadano** (nombre azul)
- **Promedio < 0**: el personaje es **Criminal** (nombre rojo)

### 7.3 Cambios de Reputacion

**Acciones que suman reputacion positiva:**
- Matar un criminal: + Noble
- Matar un NPC hostil: + Plebe/Cazador
- Resucitar a un ciudadano: + 500 Noble
- Trabajar (pescar, minar, construir): + Plebe

**Acciones que suman reputacion negativa:**
- Atacar a un ciudadano: + Bandido, Nobleza se reduce a la mitad
- Matar a un ciudadano: + Asesino (x2), se pierden TODOS los puntos de Burgues, Noble y Plebe
- Robar: + Ladron
- Matar NPCs no hostiles: + Asesino
- Matar guardias: masivamente + Asesino, se borran Noble y Plebe

**Zona de arena**: en arenas de combate (trigger 6), matar no afecta la reputacion.

### 7.4 Color del Nombre

El color del nombre sobre el personaje refleja su estado:
- **Azul**: ciudadano normal
- **Rojo**: criminal
- **Color especial**: cuando un jugador esta en estado "atacable" temporal (fue agredido y puede
  defenderse sin consecuencias)

---

## 8. Newbie

### 8.1 Definicion

Un personaje es considerado **newbie** mientras su nivel sea menor o igual a 12.

### 8.2 Protecciones del Newbie

1. **Items newbie exclusivos**: los newbies reciben items marcados como "newbie" que:
   - No pueden tirarse al piso
   - No se pierden al morir
   - Solo pueden ser usados/equipados por newbies
2. **Restriccion de equipamiento**: solo pueden usar items marcados como newbie
3. **Proteccion en dungeon newbie**: si el personaje esta en el dungeon newbie y deja de ser
   newbie (llega a nivel 13), es transportado automaticamente a su ciudad de origen
4. **Al morir**: solo pierde objetos NO newbie (los newbie los conserva)

### 8.3 Transicion a No-Newbie

Al alcanzar el nivel 13, el personaje deja de ser newbie:
- Se le quitan **automaticamente** todos los items newbie del inventario
- Se desactivan las protecciones especiales
- Si esta en el dungeon newbie, es teletransportado a su ciudad

---

## 9. Login y Logout

### 9.1 Flujo de Login

1. Validar que el personaje existe en el sistema de persistencia
2. Validar password (comparacion case-insensitive)
3. Verificar que no haya otro usuario con el mismo nombre conectado
4. Verificar que no haya otro usuario con la misma IP (si el multi-login esta deshabilitado)
5. Verificar que no se exceda el limite de usuarios simultaneos
6. Cargar todos los datos del personaje desde la persistencia
7. Validar integridad del personaje (cabeza, cuerpo y skills dentro de rangos validos)
8. Asignar privilegios segun las listas de administradores
9. Resolver posicion:
   - Si la posicion guardada esta ocupada por otro jugador, buscar una posicion adyacente libre
   - Si no hay posicion libre, desconectar al usuario que ocupa la posicion
10. Equipar barco si la posicion guardada es sobre agua
11. Enviar al cliente: datos del mapa, musica, stats completos, inventario, hechizos
12. Crear el personaje visual en el mapa (notificar a jugadores cercanos)
13. Recargar mascotas si esta en zona PK
14. Reconectar al clan
15. Activar proteccion temporal contra NPCs (5 segundos)
16. Registrar en log

### 9.2 Flujo de Logout

1. Guardar el personaje completo en la persistencia
2. Eliminar el personaje visual del mapa (notificar a jugadores cercanos)
3. Eliminar todas las mascotas
4. Desconectar del clan
5. Cancelar comercio seguro activo (si habia uno)
6. Abandonar party (si estaba en una)
7. Resetear centinela (si estaba siendo vigilado)
8. Registrar en log

---

## 10. Persistencia del Personaje

### 10.1 Que Se Guarda

El personaje se serializa en un archivo individual con las siguientes secciones:

| Seccion | Contenido |
|---------|-----------|
| Identidad | Password, nombre, clase, raza, genero, hogar, email, descripcion, nivel, posicion |
| Stats | HP, mana, stamina, hambre, sed, experiencia, ELU, golpe, oro, atributos (actuales y base) |
| Skills | Los 20 skills con su nivel, experiencia y umbral |
| Hechizos | Los 35 slots de hechizos aprendidos |
| Inventario | Los 30 slots con indice de objeto, cantidad, y si esta equipado |
| Banco | Los 40 slots del banco con indice y cantidad |
| Reputacion | Los 6 contadores de reputacion |
| Faccion | Pertenencia a Armada/Caos, kills, recompensas, fecha de ingreso, reenlistadas |
| Flags | Estado de ban, mascotas, muertes acumuladas, flags diversos |
| Contadores | Pena de carcel pendiente, skills asignados |
| Clan | Indice del clan al que pertenece |
| Muertes | Usuarios matados, NPCs muertos (totales) |

### 10.2 Cuando Se Guarda

- Al desconectarse (inmediato)
- Durante el WorldSave periodico
- Por comando administrativo
