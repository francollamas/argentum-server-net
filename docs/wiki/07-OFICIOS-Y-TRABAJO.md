# 07 - Oficios y Trabajo

> Este documento describe todos los sistemas de oficios y habilidades no combativas:
> pesca, mineria, tala, herreria, carpinteria, navegacion, domar criaturas, ocultarse,
> robar, supervivencia, y meditar. Estos sistemas constituyen la base de la economia
> y la progresion alternativa al combate.

---

## 1. Consumo de Energia (Stamina)

Toda accion de trabajo consume stamina. El costo varia drasticamente segun la clase:

| Accion | Clase Trabajador | Otras Clases |
|--------|-----------------|-------------|
| Talar | 2 | 4 |
| Pescar | 1 | 3 |
| Minar | 2 | 5 |
| Acciones generales de oficio | 2 | 6 |

El Trabajador es, por disenio, la clase mas eficiente para trabajar, gastando entre la mitad
y un tercio de la energia que las demas clases.

Si el jugador no tiene suficiente stamina para la accion, esta falla con un mensaje de error.

---

## 2. Pesca

### 2.1 Herramientas

Existen dos herramientas de pesca, cada una con mecanica diferente:
- **Cana de pesca**: pesca basica, un pez por intento
- **Red de pesca**: pesca avanzada, puede obtener diferentes tipos de pez

Ambas deben estar **equipadas como arma** para usarse.

### 2.2 Mecanica con Cana de Pesca

**Probabilidad de exito:**
```
Suerte = -0.00125 x Skill^2 - 0.3 x Skill + 49
```
Se genera un aleatorio. Si es menor o igual a 6 (dentro del rango de suerte), hay exito.

**Resultado exitoso:**
- **Trabajador**: pesca entre 1 y `1 + max(1, (Nivel - 4) / 5)` peces
- **Otras clases**: pesca 1 pez
- El recurso obtenido es siempre "Pescado" (un unico tipo de item)

### 2.3 Mecanica con Red de Pesca

**Probabilidad de exito:** misma formula que la cana.

**Resultado exitoso:**
- Obtiene un pez aleatorio entre 4 tipos diferentes (variedad de peces)
- **Trabajador**: pesca entre 1 y 5 peces
- **Otras clases**: pesca 1 pez

### 2.4 Reputacion

Pescar siempre suma puntos de reputacion **Plebe** (independientemente del exito).

### 2.5 Skill de Pesca

El skill de Pesca sube con cada intento (exito o fallo), segun las reglas generales
de progresion de skills (50 EXP al acertar, 20 al fallar).

---

## 3. Mineria

### 3.1 Herramienta

Se requiere un **piquete minero** equipado como arma.

### 3.2 Extraccion

El jugador usa el piquete sobre un **yacimiento mineral** (objeto de tipo yacimiento en el mapa).

Al tener exito, obtiene mineral crudo del tipo correspondiente al yacimiento:
- Hierro crudo
- Plata cruda
- Oro crudo

### 3.3 Fundicion

Los minerales crudos se transforman en lingotes usando una **fragua** (estacion de trabajo
en el mapa). El jugador debe estar junto a la fragua y tener los minerales en el inventario.

**Ratios de conversion:**

| Mineral Crudo | Cantidad Necesaria | Producto |
|---------------|-------------------|----------|
| Hierro crudo | 14 unidades | 1 lingote de hierro |
| Plata cruda | 20 unidades | 1 lingote de plata |
| Oro crudo | 35 unidades | 1 lingote de oro |

**Modificador de clase para fundicion:**
- **Trabajador**: x1 (usa su skill real de mineria)
- **Todas las demas clases**: x3 (necesitan 3 veces mas skill para fundir lo mismo)

**Cantidad de lingotes por ciclo:**
```
Lingotes = max(1, (Nivel - 4) / 5)
```

Esto significa que un personaje de nivel alto produce mas lingotes por accion.

### 3.4 Reputacion

Minar suma puntos de reputacion **Plebe**.

---

## 4. Tala

### 4.1 Herramientas

- **Hacha de lenador**: para arboles normales, produce lena comun
- **Hacha de lena elfica**: para arboles elficos, produce madera elfica

La herramienta correspondiente debe estar **equipada como arma**.

### 4.2 Mecanica

El jugador usa el hacha sobre un arbol (objeto de tipo arbol o arbol elfico en el mapa).
Si tiene exito, obtiene el recurso correspondiente.

### 4.3 Consumo de Energia

| Clase | Costo por Tala |
|-------|---------------|
| Trabajador | 2 stamina |
| Otras clases | 4 stamina |

El Trabajador tiene un costo especial reducido para talar, adicional a su reduccion general.

### 4.4 Reputacion

Talar suma puntos de reputacion **Plebe**.

---

## 5. Herreria

### 5.1 Herramienta

Se requiere un **martillo de herrero** equipado como arma.

### 5.2 Estacion de Trabajo

El jugador debe estar junto a un **yunque** (objeto de tipo yunque en el mapa).

### 5.3 Materiales

Los materiales son lingotes de metal:
- **Lingotes de hierro**: el material basico
- **Lingotes de plata**: material intermedio
- **Lingotes de oro**: material premium

Cada item forjable tiene una receta que especifica cuantos lingotes de cada tipo se necesitan.

### 5.4 Modificador de Clase

- **Trabajador**: x1 (usa su skill real de herreria)
- **Todas las demas clases**: x4 (necesitan 4 veces mas skill para forjar lo mismo)

Esto significa que un Trabajador con skill 25 puede forjar lo que otra clase necesita skill 100.

### 5.5 Lista de Items Forjables

Existen dos listas separadas de recetas:
- **Armas de herrero**: espadas, hachas, mazas, etc.
- **Armaduras de herrero**: corazas, cascos, escudos, etc.

Cada receta define:
- Item resultante (indice del objeto)
- Lingotes de hierro necesarios
- Lingotes de plata necesarios
- Lingotes de oro necesarios
- Skill minimo de herreria requerido (antes del modificador de clase)

### 5.6 Sistema de Lotes

Se pueden construir multiples items por ciclo de trabajo. El sistema calcula cuantos items
se pueden hacer segun los materiales disponibles, y reduce progresivamente hasta encontrar
una cantidad viable.

### 5.7 Fundicion de Armas (Reciclaje)

Las armas existentes se pueden fundir para recuperar materiales:
- Se recupera un porcentaje aleatorio entre **10% y 25%** de los lingotes originales de la receta
- Solo se pueden fundir items que tengan receta de herreria
- Se necesita estar junto a una fragua

### 5.8 Reputacion

Forjar items suma puntos de reputacion **Plebe**.

---

## 6. Carpinteria

### 6.1 Herramienta

Se requiere un **serrucho de carpintero** equipado como arma.

### 6.2 Materiales

- **Madera (lena)**: material basico, obtenida talando arboles normales
- **Madera elfica**: material premium, obtenida talando arboles elficos

### 6.3 Modificador de Clase

- **Trabajador**: x1 (usa su skill real de carpinteria)
- **Todas las demas clases**: x3 (necesitan 3 veces mas skill)

### 6.4 Lista de Items Construibles

Una unica lista de recetas que incluye:
- Armas de madera (arcos, bastones, lanzas)
- Flechas y municiones
- Barcos
- Muebles y objetos utilitarios

Cada receta define:
- Item resultante
- Madera comun necesaria
- Madera elfica necesaria
- Skill minimo de carpinteria requerido (antes del modificador de clase)

### 6.5 Reputacion

Construir items de carpinteria suma puntos de reputacion **Plebe**.

---

## 7. Mejora de Items (Upgrade)

### 7.1 Concepto

Algunos items pueden ser mejorados a una version superior. El item tiene un campo "Upgrade"
que indica el indice del item mejorado al que puede evolucionar.

### 7.2 Requisitos

1. Tener el item original en el inventario
2. Tener los materiales de la **diferencia** entre el item superior y el original:
   ```
   MaterialesNecesarios = (MaterialesItemSuperior - MaterialesItemOriginal) x PORCENTAJE_UPGRADE
   ```
   Donde PORCENTAJE_UPGRADE es 85% por defecto (se paga el 85% de la diferencia)
3. Tener el skill necesario (herreria o carpinteria segun el tipo de item)
4. Tener la herramienta correspondiente equipada (martillo o serrucho)
5. Estar junto a la estacion de trabajo correspondiente (yunque o nada para carpinteria)

### 7.3 Resultado

- Se consume el item original
- Se consumen los materiales adicionales
- Se obtiene el item mejorado

---

## 8. Navegacion

### 8.1 Requisito

El jugador debe poseer un barco (item de tipo barco) en su inventario y tener suficiente
skill de Navegacion.

### 8.2 Modificador de Clase

| Clase | Modificador | Efecto |
|-------|-------------|--------|
| Pirata | x1 | Navega con su skill real |
| Trabajador (pescador experto) | x1.71 si Pesca = 100, sino x2 | Bonus para pescadores |
| Todas las demas clases | x2 | Necesitan el doble de skill |

Cada barco tiene un `MinSkill` de navegacion. El skill efectivo del jugador (despues del
modificador de clase) debe alcanzar ese minimo.

### 8.3 Transformacion al Navegar

Al entrar en agua con un barco equipado:
- El personaje pierde su cabeza y ropaje visible
- Se transforma visualmente en el barco correspondiente
- La apariencia del barco depende de la faccion y alineacion:
  - Ciudadano: barco ciudadano
  - Criminal: barco criminal
  - Armada Real: fragata real
  - Legion Oscura: fragata del caos

### 8.4 Restricciones al Navegar

- No se puede cambiar armadura, casco ni escudo mientras se navega
- El barco tiene sus propios stats de dano y defensa (se suman al combate)
- Si el jugador muere navegando, se transforma en fragata fantasmal

### 8.5 Skill de Navegacion

El skill sube al navegar (moverse en agua). Se evalua con cada movimiento maritimo.

---

## 9. Domar Criaturas

### 9.1 Mecanica General

Domar permite convertir una criatura del mundo en mascota propia. Es una alternativa a la
invocacion magica para obtener seguidores.

### 9.2 Formula de Puntos de Domar

```
PuntosDomar = Carisma x SkillDomar
```

El NPC tiene un nivel de "Domable" que define los puntos necesarios para domarlo.

### 9.3 Modificador de Clase

| Clase | Modificador | Nota |
|-------|-------------|------|
| Druida | 6 | El mejor domador |
| Cazador | 6 | Igual de bueno que el Druida |
| Clerigo | 7 | Ligeramente inferior |
| Demas clases | 10 | Significativamente mas dificil |

El modificador se usa para escalar el requisito del NPC. Con modificador mas bajo, se necesitan
menos puntos.

### 9.4 Bonificaciones por Equipo

| Item | Efecto |
|------|--------|
| Flauta Elfica | -20% en puntos requeridos |
| Flauta Magica | -11% en puntos requeridos |

### 9.5 Probabilidad de Exito

Incluso cumpliendo todos los requisitos de puntos, hay solo un **20% de chance de exito**
(1 en 5 intentos). Los intentos fallidos aun consumen stamina y el skill intenta subir.

### 9.6 Restricciones

- **Zona**: solo en zonas PK (no se puede domar en zona segura)
- **Maximo mascotas**: 3 simultaneas
- **Maximo por tipo**: 2 criaturas del mismo tipo de NPC
- **Sin dueno**: el NPC no puede tener ya un dueno
- **Domable**: el NPC debe tener la propiedad Domable > 0

### 9.7 Mascotas en Zona Segura

Al entrar en zona segura, las mascotas "esperan afuera": desaparecen del mapa pero se conservan
en la data del jugador. Al salir de la zona segura, reaparecen.

---

## 10. Ocultarse

### 10.1 Mecanica General

Ocultarse es un skill activo que vuelve al personaje invisible temporalmente, sin usar mana.
Es la alternativa no-magica a la invisibilidad.

### 10.2 Probabilidad de Exito

Se calcula con una curva cubica del skill de Ocultarse:
```
Probabilidad = ((0.000002 x Skill^2 - 0.0002 x Skill + 0.0064) x Skill + 0.1124) x 100
```

Con skill bajo la probabilidad es ~11%. Con skill alto llega a ~50-60%.

### 10.3 Duracion

La duracion tambien es una curva cubica inversamente proporcional al skill, multiplicada
por el intervalo base de ocultamiento. A mayor skill, mayor duracion.

### 10.4 Casos Especiales por Clase

**Cazador con skill > 90 y armadura especifica:**
- Puede permanecer oculto **indefinidamente** (sin timer de expiracion)
- Requiere una armadura especifica equipada (dos indices de armadura validos)
- Es la unica forma de ocultamiento permanente del juego

**Bandido:**
- Se oculta por la **mitad del tiempo** de las demas clases
- Compensado por sus otras habilidades de combate

**Pirata navegando:**
- Al ocultarse mientras navega, NO se hace invisible
- En su lugar, se transforma en un **galeon fantasmal** (cambio visual)
- Es detectable visualmente pero cambia su apariencia

### 10.5 Cancelacion del Ocultamiento

El ocultamiento se cancela si:
- El timer expira
- El jugador ataca
- El jugador lanza un hechizo (las palabras magicas lo revelan)
- El jugador habla

---

## 11. Robo

### 11.1 Mecanica General

El robo permite a un jugador sustraer objetos u oro de otro jugador. Es una accion PvP no
letal que tiene restricciones fuertes.

### 11.2 Restricciones de Zona

- Solo funciona en **zona PK** (no en zona segura ni en arenas)
- Debe haber un jugador objetivo en la celda adyacente en la direccion que mira el ladron

### 11.3 Restricciones de Faccion

| Situacion | Permitido |
|-----------|-----------|
| Miembro Legion Oscura roba a otro del Caos | **No** |
| Miembro Armada Real roba a ciudadano | **No** |
| Ciudadano (seguro off) roba a ciudadano | Si (pero se vuelve criminal) |
| Criminal roba a ciudadano | Si |
| Cualquiera roba a criminal | Si |

### 11.4 Consumo

Cada intento de robo consume **15 puntos de stamina**.

### 11.5 Probabilidad de Exito

Basada en escalones del skill de Robar:

| Rango de Skill | Rango del Aleatorio |
|----------------|-------------------|
| 0 - 10 | 1 a 35 |
| 11 - 20 | 1 a 30 |
| 21 - 30 | 1 a 28 |
| 31 - 40 | 1 a 24 |
| 41 - 50 | 1 a 22 |
| 51 - 60 | 1 a 20 |
| 61 - 70 | 1 a 18 |
| 71 - 80 | 1 a 15 |
| 81 - 90 | 1 a 10 |
| 91 - 100 | 1 a 5 |

El robo tiene exito si el resultado aleatorio es **menor a 3**. Con skill 100, la probabilidad
es ~40% (2 de 5). Con skill 0, es ~5.7% (2 de 35).

### 11.6 Que se Roba

**Ladron (clase):** 50% de probabilidad de robar un **objeto**, 50% de robar **oro**.

**Otras clases:** solo pueden robar **oro**.

#### Robo de Oro

| Situacion | Rango de Oro Robado |
|-----------|-------------------|
| Ladron con Guantes de Hurto | aleatorio(Nivel x 50, Nivel x 100) |
| Ladron sin Guantes | aleatorio(Nivel x 25, Nivel x 50) |
| Otras clases | aleatorio(1, 100) |

El oro se resta de la billetera de la victima (si tiene suficiente).

#### Robo de Objetos (solo Ladron)

1. Se selecciona un slot aleatorio del inventario de la victima
2. Se roba entre el **5% y el 10%** de la cantidad en ese slot, con minimo 1 unidad
3. El objeto robado se agrega al inventario del ladron

### 11.7 Items No Robables

Los siguientes items no pueden ser robados:
- Llaves
- Barcos
- Items equipados (solo se roban items sueltos del inventario)
- Items de faccion (Real o Caos)

### 11.8 Consecuencias de Robar

- Robar a un ciudadano convierte al ladron en **criminal**
- Suma puntos de reputacion **Ladron** y **Bandido**
- Se registra en log si el item robado es valioso

---

## 12. Supervivencia (Fogatas)

### 12.1 Requisito

- Tener una **daga** equipada
- Haber lena tirada en el piso (minimo 3 troncos) en una celda adyacente
- Estar a distancia menor o igual a 2 tiles de la lena

### 12.2 Probabilidad

| Rango de Skill | Probabilidad |
|----------------|-------------|
| 0 - 5 | 1 en 3 (~33%) |
| 6 - 34 | 1 en 2 (50%) |
| 35+ | Siempre exito (100%) |

### 12.3 Resultado

Si tiene exito, se crean fogatas en el mapa:
```
CantidadFogatas = CantidadLena / 3
```

La lena se consume y se reemplaza por fogatas encendidas.

---

## 13. Meditar

### 13.1 Proposito

La meditacion es el **unico mecanismo** de regeneracion de mana en el juego. Sin meditacion,
la mana solo se recupera con pociones azules.

### 13.2 Activacion

- El jugador activa la meditacion manualmente
- Hay un **delay de 2 segundos** antes de que empiece a regenerar mana
- Se muestra un efecto visual sobre el personaje que varia segun el nivel de skill

### 13.3 Efectos Visuales

| Rango de Skill | Efecto |
|----------------|--------|
| Bajo | Efecto de meditacion pequeno |
| Medio | Efecto de meditacion mediano |
| Alto | Efecto de meditacion grande |
| Muy alto | Efecto de meditacion extra grande |
| Maximo | Efecto de meditacion maximo |

### 13.4 Regeneracion

Cada tick del game loop (40ms) que el jugador esta meditando, regenera mana. La cantidad
depende del nivel de skill de Meditar y del nivel del personaje.

### 13.5 Cancelacion

La meditacion se cancela si:
- El jugador se mueve
- El jugador ataca
- El jugador lanza un hechizo
- El jugador recibe dano que supera el umbral de interrupcion (ver documento 03-COMBATE.md, seccion 7)

### 13.6 Restricciones

- Solo funciona para clases con mana > 0
- No funciona si el jugador esta muerto, paralizado, o navegando
- El jugador no debe tener hambre ni sed para que el skill pueda subir

---

## 14. Resumen de Modificadores de Clase para Oficios

Esta tabla resume como las clases interactuan con los diferentes oficios:

| Oficio | Trabajador | Demas Clases | Nota |
|--------|-----------|-------------|------|
| Mineria (fundicion) | Skill real (x1) | Skill / 3 | El Trabajador funde con 1/3 del skill |
| Herreria | Skill real (x1) | Skill / 4 | El Trabajador forja con 1/4 del skill |
| Carpinteria | Skill real (x1) | Skill / 3 | El Trabajador construye con 1/3 del skill |
| Navegacion (Pirata) | x2 | x2 | El Pirata navega con x1 (skill real) |
| Navegacion (Trabajador pescador) | x1.71 si Pesca=100 | x2 | Bonus especial para pescadores |
| Domar (Druida/Cazador) | mod 10 | mod 6 | Druida y Cazador doman con mod 6 |
| Energia por trabajo | 2 | 4-6 | El Trabajador gasta 2-3 veces menos |

### 14.1 Implicacion de Diseño

El Trabajador es la clase definitiva para oficios. Su ventaja no es marginal sino **masiva**:
puede hacer todo lo que otras clases hacen a 1/3 o 1/4 del skill requerido, gastando la mitad
de la energia. A cambio, su combate es mediocre. Esta es la tension de diseño fundamental
del sistema de clases en relacion a la economia.

---

## 15. Relacion con Otros Sistemas

### 15.1 Inventario

- Todas las herramientas de trabajo son armas que se equipan
- Los recursos (minerales, madera, lingotes, peces) ocupan slots del inventario
- Los productos crafteados se agregan al inventario
- El peso de los materiales puede limitar la capacidad de trabajo

### 15.2 Comercio

- Los productos de oficios son la fuente principal de items comerciables
- Los recursos crudos y los productos terminados tienen valor comercial
- El circuito economico es: recolectar -> refinar -> craftear -> vender

### 15.3 Combate

- Las armas y armaduras crafteadas alimentan el equipamiento de combate
- Las flechas de carpinteria son municion para arcos
- Los barcos permiten combate naval

### 15.4 Personaje

- Los skills de oficio suben con uso, generando EXP general (50 EXP por punto de skill)
- La Fuerza no afecta la recoleccion (solo el combate)
- El Carisma afecta domar criaturas
- La stamina es el recurso critico que limita la produccion
