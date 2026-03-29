# 06 - Inventario y Objetos

> Este documento describe el sistema de inventario del personaje, los slots de equipamiento,
> todos los tipos de objetos que existen en el juego, sus efectos, y las restricciones de uso.
> El inventario es un sistema transversal que conecta combate, oficios, comercio, y progresion.

---

## 1. Estructura del Inventario

### 1.1 Slots

El inventario del personaje tiene una capacidad base y una extension:

| Concepto | Valor |
|----------|-------|
| Slots base (sin mochila) | 20 |
| Slots con mochila mediana | 25 (+5) |
| Slots con mochila grande | 30 (+10) |
| Maximo absoluto | 30 |
| Cantidad maxima por slot (stack) | 10,000 |

### 1.2 Stacking

Los items del mismo tipo se apilan automaticamente. Cuando el jugador recoge o recibe un item:

1. Primero se busca un slot que ya contenga el mismo item y cuya cantidad + la nueva no exceda 10,000
2. Si se encuentra, se apila ahi
3. Si no se encuentra (todos los slots de ese item estan llenos), se busca un slot vacio
4. Si no hay slot vacio, el item no se puede agregar ("inventario lleno")

### 1.3 Slots Extra por Mochila

Los slots extra (posiciones 21-30) estan disponibles solo mientras la mochila esta equipada.
Restricciones de los slots extra:
- **NO pueden contener objetos especiales** (los que no se caen al morir)
- Solo pueden contener items "normales"
- **Al desequipar la mochila**: todos los items en los slots extra caen al piso inmediatamente

### 1.4 Slot Virtual de Oro

El oro NO ocupa un slot de inventario. Se maneja como una "billetera" separada:
- Oro maximo: 90,000,000
- Al recoger oro del piso, va directo a la billetera
- Al tirar oro, se crea un objeto "oro" en el piso

---

## 2. Slots de Equipamiento

El personaje tiene 8 slots de equipamiento, cada uno independiente y con reglas propias:

### 2.1 Arma (Slot de Arma)

- Ocupa las manos del personaje
- Cambia la animacion visual del personaje
- Las razas Enano/Gnomo tienen animaciones de arma propias (modelo mas pequeno)
- Afecta: dano min/max, tipo de ataque (melee, proyectil, wrestling si no hay arma)
- Restricciones: clase, faccion
- Propiedades especiales posibles: envenena, apunala, acuchilla, es proyectil, requiere municion

### 2.2 Armadura (Slot de Armadura)

- Cambia el modelo visual completo del personaje (body/ropaje)
- Afecta: defensa min/max del torso
- Restricciones: clase, genero, raza, faccion
- No se puede cambiar mientras se navega

### 2.3 Casco (Slot de Casco)

- Cambia la animacion de casco sobre la cabeza
- Afecta: defensa min/max de la cabeza, puede tener defensa magica
- Restricciones: clase
- No se puede cambiar mientras se navega

### 2.4 Escudo (Slot de Escudo)

- Cambia la animacion de escudo
- Afecta: defensa (contribuye al bloqueo con escudo y a la absorcion del torso)
- Restricciones: clase, faccion
- No se puede cambiar mientras se navega

### 2.5 Anillo (Slot de Anillo)

- Sin representacion visual en el personaje
- Puede tener efectos variados: defensa magica, bonus de dano
- Restricciones: clase, faccion
- Incluye items funcionales:
  - **Flautas elficas/magicas**: permiten domar criaturas y dan bonificaciones al Druida
  - **Guantes de hurto**: permiten robar objetos y tienen efectos especiales en combate por clase

### 2.6 Municion (Slot de Municion)

- Flechas para arcos y armas a distancia
- Se consume una unidad por cada disparo
- Puede tener dano propio que se suma al del arma
- Puede tener propiedades: envenenar, paralizar

### 2.7 Barco (Slot de Barco)

- Transforma al personaje en un barco al entrar en agua
- Hay 3 tipos de barco, cada uno con variantes segun alineacion:

| Tipo | Ciudadano | Criminal | Armada Real | Legion Oscura |
|------|-----------|----------|-------------|---------------|
| Barca | Barca ciudadana | Barca criminal | - | - |
| Galera | Galera ciudadana | Galera criminal | - | - |
| Galeon | Galeon ciudadano | Galeon criminal | Fragata Real | Fragata del Caos |

- Cada barco tiene: dano min/max, defensa, y skill de navegacion minimo
- Los barcos **no son robables**
- Al navegar, el personaje pierde su cabeza y ropaje visible
- Si el personaje muere mientras navega, se transforma en una fragata fantasmal

### 2.8 Mochila (Slot de Mochila)

- Extiende la capacidad del inventario
- Dos tipos:
  - **Mochila mediana**: +5 slots (total 25)
  - **Mochila grande**: +10 slots (total 30)
- Al desequipar: los items en slots extra caen al piso

### 2.9 Proceso de Equipar

Al equipar un item:
1. Se validan todas las restricciones (clase, raza, genero, faccion, newbie)
2. Si ya hay algo equipado en ese slot, se desequipa primero
3. Se marca el item como equipado en su slot de inventario
4. Se actualizan los stats del personaje (dano, defensa, apariencia)
5. Se notifica al cliente y a los jugadores cercanos del cambio visual

### 2.10 Proceso de Desequipar

Al desequipar un item:
1. Se quita la marca de equipado del slot de inventario
2. Se actualizan los stats (se remueven los bonus)
3. Se actualiza la apariencia visual
4. El item permanece en el inventario como item normal

---

## 3. Tipos de Objetos

Cada objeto del juego tiene un tipo que determina su comportamiento fundamental.

### 3.1 Alimentos (Uso Unico)

- Se consumen al usar
- Restauran puntos de hambre
- Pueden ser comida, frutas, carne, etc.

### 3.2 Armas

Objetos equipables en el slot de arma. Propiedades:

| Propiedad | Descripcion |
|-----------|-------------|
| Dano min/max | Rango de dano base del arma |
| Animacion de arma | Grafico del arma sobre el personaje |
| Animacion de arma enana | Grafico alternativo para razas pequenas |
| Envenena | Si el arma puede envenenar al objetivo |
| Apunala | Si permite ataques de apunalar |
| Acuchilla | Si permite ataques de acuchillar (solo Pirata) |
| Proyectil | Si es un arma a distancia (arco) |
| Municion | Si requiere municion equipada |
| Refuerzo | Penetracion de armadura (solo aplica en PvP) |
| Staff Power | Poder de baculo (solo para Magos) |
| Staff Damage Bonus | Bonus de dano magico (solo para Magos) |
| Staff Affected | Si el poder del baculo afecta a los hechizos del portador |

**Armas especiales hardcodeadas:**
- **Espada Matadragones**: contra dragones, inflige dano completo igual a MinHP + Defensa del dragon (mata de un golpe efectivo). Contra cualquier otro objetivo, inflige solo 1 de dano. Se destruye al matar al dragon.
- **Espada Vikinga**: habilita el golpe critico exclusivo del Bandido.

### 3.3 Armaduras

Objetos equipables en el slot de armadura:

| Propiedad | Descripcion |
|-----------|-------------|
| Defensa min/max | Absorcion de dano al torso |
| Ropaje | Modelo visual que reemplaza al cuerpo del personaje |
| Restriccion de clase | Lista de clases que NO pueden usarla |
| Restriccion de genero | Exclusiva de hombre o mujer |
| Restriccion de raza | Para razas especificas (enanos, gnomos, drows) |
| Real / Caos | Restringida a una faccion |

### 3.4 Arboles

Objetos de mundo (no de inventario). Representan recursos talables:
- Se talan con un hacha de lenador equipada
- Producen lena (madera)
- Los **arboles elficos** producen madera elfica y requieren un hacha de lena elfica

### 3.5 Oro

Objeto especial que representa dinero en el piso:
- Al recogerlo, va directo a la billetera (no ocupa slot)
- Al tirar oro del inventario, se subdivide en pilas de maximo 10,000

**Reglas especiales de tirar oro:**
- Tirar mas de 500,000 limita la dispersion en el piso a 500,000 (el resto se descuenta directo)
- Tirar mas de 50,000 se registra en log, incluyendo los nombres de todos los jugadores cercanos (radio de 10 tiles)
- Los Piratas con un barco especifico pueden tirar oro en agua

### 3.6 Puertas

Objetos de mundo con estado abierta/cerrada:
- Pueden tener llave asociada (se necesita la llave correcta para abrir)
- Cada puerta tiene 3 graficos: abierta, cerrada, cerrada con llave
- Las llaves se consumen al usar (abren la puerta permanentemente) - depende de la implementacion
- Los GMs pueden abrir cualquier puerta

### 3.7 Contenedores

Cofres y baules que pueden contener otros objetos:
- Al interactuar, muestran su contenido
- El jugador puede tomar objetos del contenedor

### 3.8 Carteles

Objetos de mundo con texto legible:
- Al interactuar, se muestra el texto del cartel al jugador

### 3.9 Llaves

Items especiales con multiples protecciones:
- **No son robables** (otro jugador no puede robartelas)
- **No se caen al morir** (se conservan en el inventario)
- **No se pueden tirar al piso** (si el jugador es newbie)
- Cada llave tiene un codigo que la asocia a una puerta especifica

### 3.10 Foros

Objetos de mundo que permiten acceder al sistema de foros in-game:
- Al interactuar, se abre la interfaz del foro

### 3.11 Pociones

Items consumibles con efectos variados segun su tipo:

| Tipo | Color | Efecto |
|------|-------|--------|
| 1 | - | Aumenta Agilidad temporalmente (duracion en ticks) |
| 2 | - | Aumenta Fuerza temporalmente (duracion en ticks) |
| 3 | Roja | Restaura HP (aleatorio entre modificador min y max) |
| 4 | Azul | Restaura Mana: 4% del mana maximo + nivel/2 + 40/nivel |
| 5 | Violeta | Cura envenenamiento |
| 6 | Negra | **Mata instantaneamente** al jugador que la toma |

**Cooldown de pociones**: no se pueden tomar inmediatamente despues de golpear o ser golpeado.
Existe un cooldown cruzado entre ataque y uso de objetos.

**Pociones de atributos**: los atributos no pueden exceder el doble del valor base ni el maximo
global (40). Las pociones tienen duracion limitada; al expirar, los atributos vuelven a su base.

### 3.12 Bebidas

Items consumibles que restauran puntos de sed.

### 3.13 Lena / Madera

Recurso obtenido al talar arboles:
- **Lena comun**: de arboles normales, usada en carpinteria
- **Madera elfica**: de arboles elficos, recurso premium para carpinteria avanzada
- Tambien se usa para crear fogatas (con el skill de Supervivencia)

### 3.14 Fogatas

Objetos de mundo creados por el jugador con el skill de Supervivencia:
- Se crean usando una daga sobre lena tirada en el piso (minimo 3 troncos)
- Tienen dos estados: encendida y apagada
- Cantidad de fogatas creadas = cantidad de lena / 3

### 3.15 Escudos

Objetos equipables en el slot de escudo:

| Propiedad | Descripcion |
|-----------|-------------|
| Defensa min/max | Absorcion de dano adicional al torso |
| Animacion de escudo | Grafico del escudo sobre el personaje |
| Restriccion de clase | Clases prohibidas |
| Real / Caos | Restringido a faccion |

### 3.16 Cascos

Objetos equipables en el slot de casco:

| Propiedad | Descripcion |
|-----------|-------------|
| Defensa min/max | Absorcion de dano a la cabeza |
| Defensa magica min/max | Absorcion de dano magico |
| Animacion de casco | Grafico del casco sobre la cabeza |
| Restriccion de clase | Clases prohibidas |

### 3.17 Anillos

Objetos equipables en el slot de anillo:
- Pueden tener defensa magica min/max
- Pueden tener bonus de dano
- Incluyen items funcionales como flautas y guantes (ver slot de anillo)
- Restricciones: clase, faccion

**Anillo especial**: el "Super Anillo" (un item especifico) rechaza completamente los efectos
de paralisis, inmovilizacion y estupidez.

### 3.18 Teleports

Objetos de mundo que teletransportan al jugador:
- Pueden tener un radio de destino aleatorio (para teleports con area de dispersion)

### 3.19 Yacimientos Minerales

Objetos de mundo (no de inventario) que representan puntos de mineria:
- El jugador usa un piquete minero sobre ellos para extraer mineral crudo
- Hay tres tipos: hierro, plata, oro

### 3.20 Minerales en Bruto

Recursos obtenidos al minar:
- **Hierro crudo**: se funde en lingotes de hierro (14 unidades = 1 lingote)
- **Plata cruda**: se funde en lingotes de plata (20 unidades = 1 lingote)
- **Oro crudo**: se funde en lingotes de oro (35 unidades = 1 lingote)

### 3.21 Pergaminos

Items consumibles que ensenan hechizos:
- Al usar un pergamino, el jugador aprende el hechizo asociado
- El hechizo se agrega al primer slot de hechizo libre
- El pergamino se consume

### 3.22 Instrumentos Musicales

Items equipables que tienen funciones especiales:
- **Laud Elfico**: permite al Bardo resucitar, da +4% dano magico
- **Laud Magico**: permite al Bardo resucitar
- **Flauta Elfica**: da descuentos de mana al Druida, bonus al domar
- **Flauta Magica**: permite domar con bonus, equipable en slot de anillo
- Los instrumentos pueden reproducir hasta 3 sonidos diferentes
- **Cuernos de faccion**: instrumentos exclusivos de cada faccion

### 3.23 Yunques

Objetos de mundo (estaciones de trabajo) para herreria:
- El jugador debe estar cerca de un yunque para forjar
- No se recogen ni se mueven

### 3.24 Fraguas

Objetos de mundo (estaciones de trabajo) para fundir minerales:
- El jugador debe estar cerca de una fragua para fundir
- No se recogen ni se mueven

### 3.25 Barcos

Items especiales equipables en el slot de barco:
- **No son robables**
- Al equiparlos en agua, transforman al personaje visualmente
- Cada barco tiene: dano min/max, defensa, skill de navegacion minimo
- Las versiones varian segun la alineacion del personaje (ciudadano, criminal, Armada, Caos)

### 3.26 Flechas / Municion

Items consumibles equipables en el slot de municion:
- Se consumen una por cada disparo con arma de proyectil
- Tienen dano min/max propio que se suma al del arma
- Pueden tener propiedades especiales: envenenar, paralizar

### 3.27 Botellas Vacias

Items que se pueden llenar con agua:
- Al usar cerca de una fuente de agua, se convierten en botellas llenas

### 3.28 Botellas Llenas

Items consumibles que restauran sed:
- Al consumirse, vuelven a ser botellas vacias (no desaparecen)

### 3.29 Mochilas

Items equipables en el slot de mochila:
- **Mochila mediana**: agrega 5 slots extra (tipo 1)
- **Mochila grande**: agrega 10 slots extra (tipo 2)

---

## 4. Propiedades Transversales de Objetos

### 4.1 Valor Comercial

Cada objeto tiene un valor base que determina su precio de compra y venta:
- **Precio de compra** (en NPC): valor base / descuento del jugador (segun skill de Comerciar)
- **Precio de venta** (a NPC): valor base / 3

### 4.2 Restricciones de Clase

Cada objeto puede tener una lista de **clases prohibidas** (hasta 12). Si la clase del personaje
esta en la lista, no puede equipar el item. Los administradores ignoran esta restriccion.

### 4.3 Restricciones de Raza

Los objetos pueden estar restringidos por raza:
- Items exclusivos para razas enanas/gnomas
- Items exclusivos para razas humanas/elfas/drows
- Items exclusivos para Drows

### 4.4 Restricciones de Genero

Los objetos pueden ser exclusivos para Hombre o Mujer.

### 4.5 Restricciones de Faccion

Los objetos pueden estar marcados como:
- **Real**: solo para miembros del Ejercito Real
- **Caos**: solo para miembros de la Legion Oscura

Al ser expulsado de una faccion, las armaduras y escudos faccionarios se desequipan automaticamente.

### 4.6 Restriccion de Newbie

Los objetos pueden estar marcados como "newbie":
- Solo los pueden usar/equipar personajes newbie (nivel <= 12)
- No se pueden tirar al piso
- No se pierden al morir
- Se quitan automaticamente al dejar de ser newbie

### 4.7 Objetos con Log

Ciertos objetos estan marcados para registro (log):
- Cuando se tiran, recogen, comercian, o construyen, la accion se registra en el log del servidor
- Util para tracking de items valiosos o de economia

### 4.8 Objetos Cruciales

En inventarios de NPC comerciante, los items marcados como "crucial":
- Se respawnean automaticamente cuando se agotan del inventario del NPC
- Garantizan disponibilidad constante de items basicos

### 4.9 Objetos que No Se Caen

Ciertos objetos tienen la propiedad de no caerse al morir:
- Llaves: siempre se conservan
- Barcos: no son robables
- Estos items **no pueden ir en los slots extra** de la mochila

### 4.10 Upgrade de Items

Algunos objetos pueden ser mejorados a una version superior:
- El campo "Upgrade" indica el indice del item mejorado
- Se requieren materiales adicionales (un porcentaje de la diferencia entre el item original y el mejorado)
- Se requiere el skill correspondiente (herreria o carpinteria)
- Se requiere la herramienta adecuada equipada
- Al mejorar: se consume el item original y los materiales, y se obtiene el item mejorado

---

## 5. Objetos en el Piso

### 5.1 Reglas Generales

- Cada tile del mapa puede tener **un unico tipo de objeto** con una cantidad determinada
- Al tirar un item en un tile que ya tiene un item diferente, la accion falla
- Al tirar un item en un tile que ya tiene el mismo item, las cantidades se suman (hasta el maximo)
- Los objetos en el piso se persisten con el mapa durante el WorldSave

### 5.2 Recoger Objetos

- El jugador puede recoger objetos del tile donde esta parado
- Si es oro, va directo a la billetera
- Si es otro item, se intenta agregar al inventario (con stacking)
- Si el inventario esta lleno, no se puede recoger

### 5.3 Tirar Objetos

- El jugador puede tirar items de su inventario al tile donde esta
- Si el item esta equipado, se desequipa primero
- Items newbie no se pueden tirar (para newbies)
- Hay reglas especiales para tirar oro (subdivision en pilas, logs)

### 5.4 Limpieza de Objetos

Periodicamente (cada 15 minutos durante el WorldSave), el servidor limpia objetos temporales
del mundo. Los objetos marcados en el "recolector de basura" se eliminan del piso.

---

## 6. Interaccion con Otros Sistemas

### 6.1 Combate

- El arma equipada determina el tipo de ataque (melee, proyectil, wrestling) y el dano base
- La armadura, casco y escudo determinan la absorcion de dano
- La municion se consume al disparar y puede tener efectos (veneno, paralisis)
- Las propiedades del arma habilitan habilidades especiales (apunalar, acuchillar, envenenar)

### 6.2 Magia

- Los baculos (armas) afectan el poder magico del Mago
- Los instrumentos (anillos) habilitan funciones especiales (resucitar, bonus de mana)
- Los cascos y anillos pueden tener defensa magica
- Los pergaminos ensenan hechizos

### 6.3 Oficios

- Las herramientas de trabajo son armas que se equipan (hacha, piquete, cana de pesca, martillo, serrucho)
- Los recursos (minerales, madera, lingotes) se almacenan en el inventario
- Los productos crafteados se agregan al inventario

### 6.4 Comercio

- El valor del objeto determina precios de compra/venta
- El skill de Comerciar modifica el precio de compra (descuento)
- Items newbie no tienen valor de venta
- Ciertos items tienen restricciones especiales de venta (solo al sastre Real, solo al sastre del Caos)

### 6.5 Muerte

- Al morir, TODO el inventario cae al piso (excepto items newbie para newbies, y en arenas)
- Todo el equipamiento se desequipa
- Items que "no se caen" (llaves, barcos) tienen tratamiento especial
