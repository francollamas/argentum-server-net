# 08 - Comercio y Economia

> Este documento describe todos los sistemas economicos del juego: comercio con NPCs
> (compra y venta), comercio seguro entre jugadores, el sistema bancario, y las reglas
> de manejo de oro. La economia de Argentum Online es cerrada: el oro entra al mundo
> por loot de NPCs y sale por compras a NPCs comerciantes.

---

## 1. Comercio con NPCs

### 1.1 Interaccion

El jugador interactua con un NPC comerciante (haciendo click sobre el) para abrir la
ventana de comercio. La ventana muestra el inventario del NPC y permite comprar y vender.

### 1.2 Compra (Jugador Compra del NPC)

**Calculo de precio:**
```
PrecioCompra = techo(ValorBase / Descuento)
```

Donde el descuento depende del skill de Comerciar del jugador:
```
Descuento = 1 + (SkillComerciar / 100)
```

**Ejemplos de descuento:**

| Skill Comerciar | Descuento | Precio Efectivo |
|-----------------|-----------|----------------|
| 0 | 1.0 | 100% del valor base |
| 50 | 1.5 | ~67% del valor base |
| 100 | 2.0 | 50% del valor base |

Con skill de Comerciar al maximo, el jugador paga la **mitad** del precio base.

**Proceso de compra:**
1. Se verifica que el jugador tenga suficiente oro
2. Se verifica que el item exista en el inventario del NPC
3. Se resta el oro del jugador
4. Se agrega el item al inventario del jugador
5. Se quita el item del inventario del NPC
6. El skill de Comerciar intenta subir

### 1.3 Venta (Jugador Vende al NPC)

**Calculo de precio:**
```
PrecioVenta = piso(ValorBase / 3)
```

El factor de reduccion es fijo (REDUCTOR_PRECIOVENTA = 3). El skill de Comerciar NO afecta
el precio de venta, solo el de compra.

**Restricciones de venta:**

| Restriccion | Detalle |
|-------------|---------|
| Items newbie | Precio de venta = 0 (sin valor) |
| Tipo de item | El NPC solo compra items de su tipo configurado (o "cualquiera" si acepta todo) |
| Oro | No se puede vender oro al NPC (no tiene sentido) |
| Armaduras de la Armada Real | Solo se venden al "Sastre Real" (NPC especifico) |
| Armaduras de la Legion Oscura | Solo se venden al "Sastre del Caos" (NPC especifico) |
| Consejeros (GMs) | No pueden vender items (restriccion administrativa) |

**Proceso de venta:**
1. Se verifica que el item sea vendible al NPC (tipo compatible)
2. Se calcula el precio de venta
3. Se agrega el oro al jugador
4. Se quita el item del inventario del jugador
5. Se agrega el item al inventario del NPC (si tiene espacio)
6. El skill de Comerciar intenta subir

### 1.4 Reposicion del Inventario del NPC

Los NPCs comerciantes tienen mecanismos de reposicion:

**Recarga completa:**
Cuando un NPC se queda sin items, recarga su inventario completo desde las definiciones
de datos del juego (el archivo de definicion de NPCs).

**Items cruciales:**
Los items marcados como "Crucial" se respawnean **individualmente** cuando se agotan.
Esto garantiza disponibilidad constante de items basicos (pociones, flechas, comida).

**Llaves:**
Las llaves vendidas se marcan como "vendidas" en los datos para que no reaparezcan
al recargar el inventario. Esto evita que se dupliquen llaves unicas.

---

## 2. Comercio Seguro entre Jugadores

### 2.1 Inicio del Comercio

1. El jugador A escribe el comando `/COMERCIAR` estando cerca del jugador B
2. El jugador B recibe una notificacion: "X desea comerciar contigo"
3. El jugador B escribe `/COMERCIAR` con el jugador A como objetivo
4. Se abre la ventana de comercio seguro para ambos jugadores

Ambos jugadores deben estar vivos, conectados, y a distancia de interaccion.

### 2.2 Mecanica de Oferta

Cada jugador puede ofrecer:
- Hasta **30 items** (MAX_OFFER_SLOTS = 30) de su inventario
- Una cantidad de **oro**

Las ofertas son modificables libremente hasta que el jugador confirme. Una vez confirmada,
la oferta queda **bloqueada** y no se puede cambiar.

### 2.3 Proceso de Aceptacion

1. Ambos jugadores revisan la oferta del otro
2. Cada jugador debe **aceptar** la oferta del otro (confirmacion explicita)
3. **Cuando ambos aceptan**, se ejecuta el intercambio simultaneamente

### 2.4 Ejecucion del Intercambio

Al ejecutarse:
1. Los items de A se transfieren a B, y los de B a A
2. El oro de A se transfiere a B, y el de B a A
3. Si el inventario del receptor esta lleno, los items sobrantes **caen al piso** en la posicion del jugador
4. Se registran en log las transacciones grandes:
   - Mas de 50,000 de oro
   - Mas de 1,000 unidades de items con flag de log

### 2.5 Validaciones Continuas

Durante todo el proceso de comercio, el servidor valida continuamente:

| Validacion | Consecuencia si falla |
|------------|----------------------|
| Ambos jugadores conectados | Se cancela |
| Ambos jugadores comerciando entre si | Se cancela |
| Los nombres coinciden (anti-spoof) | Se cancela |
| Ninguno muerto | Se cancela |

La validacion de nombres es una proteccion contra desconexion/reconexion: si un jugador se
desconecta y otro se conecta con el mismo slot, el nombre no coincidira y el comercio se cancela.

### 2.6 Cancelacion

El comercio se cancela automaticamente si:
- Uno de los jugadores se desconecta
- Uno de los jugadores muere
- Uno de los jugadores se teletransporta
- Uno de los jugadores cancela manualmente

Al cancelar, todos los items ofrecidos vuelven a sus duenos originales.

---

## 3. Sistema Bancario

### 3.1 Acceso

El banco se accede a traves de NPCs de tipo **Banquero**. Al interactuar, se abre la interfaz
del banco.

### 3.2 Estructura

| Propiedad | Valor |
|-----------|-------|
| Slots del banco | 40 (MAX_BANCOINVENTORY_SLOTS) |
| Cantidad maxima por slot | 10,000 (igual que el inventario) |
| Persistencia | Se guarda en el archivo del personaje |

### 3.3 Depositar

1. El jugador selecciona un slot de su inventario y la cantidad a depositar
2. El servidor busca en el banco:
   - Primero, un slot que ya contenga el mismo item (para apilar)
   - Si no, un slot vacio
3. Si el banco esta lleno: "No tenes mas espacio en el banco!!"
4. El item se quita del inventario y se agrega al banco

### 3.4 Retirar

1. El jugador selecciona un slot del banco y la cantidad a retirar
2. El servidor busca en el inventario:
   - Primero, un slot que ya contenga el mismo item (para apilar)
   - Si no, un slot vacio
3. Si el inventario esta lleno: "No podes tener mas objetos"
4. El item se quita del banco y se agrega al inventario

### 3.5 Oro en el Banco

El oro NO se deposita ni retira a traves del sistema de slots del banco. El oro vive
directamente en la billetera del jugador y no necesita un mecanismo de banco separado.

El personaje tiene un campo `Banco` en sus stats que registra oro bancario, pero el
flujo principal de oro es por billetera directa.

---

## 4. Manejo de Oro

### 4.1 Limite Global

El oro maximo que puede tener un personaje es **90,000,000**.

### 4.2 Oro en el Piso

El oro tiene comportamiento especial como objeto del mundo:

**Al recoger oro del piso:**
- Va **directo a la billetera** del jugador (no ocupa slot de inventario)
- Se suma al oro existente (con tope en el maximo)

**Al tirar oro al piso:**
- Se subdivide en pilas de maximo **10,000 unidades** cada una
- Si la cantidad supera 500,000, se limita la dispersion a 500,000 en el piso (el resto se
  descuenta directamente de la billetera sin crear objetos)

### 4.3 Logging de Oro

Tirar mas de **50,000 de oro** activa un registro especial en el log del servidor que incluye:
- Quien tiro el oro
- Cuanto oro tiro
- Los nombres de **todos los jugadores cercanos** en un radio de 10 tiles

Esto sirve para detectar transferencias irregulares de oro (RMT, exploits, etc.).

### 4.4 Oro Pirata

Los Piratas con un barco especifico (un indice de barco particular) pueden tirar oro en agua.
Las demas clases solo pueden tirar oro en tierra.

### 4.5 Fuentes de Oro

| Fuente | Mecanica |
|--------|----------|
| Loot de NPCs | Los NPCs con GiveGLD > 0 tiran oro al morir |
| Venta de items | Los NPCs comerciantes pagan oro por items |
| Robo | Se roba oro de la billetera de otros jugadores |
| Comercio entre jugadores | Transferencia directa de oro |

### 4.6 Sumideros de Oro

| Sumidero | Mecanica |
|----------|----------|
| Compra de items | Los NPCs comerciantes cobran oro por items |
| Muerte | El oro del personaje NO se pierde al morir (queda en la billetera) |

Nota: la economia del AO tiene pocos sumideros de oro naturales. La principal salida es la
compra a NPCs comerciantes. Esto puede generar inflacion a largo plazo.

---

## 5. Skill de Comerciar

### 5.1 Efecto

El skill de Comerciar reduce el precio de compra en NPCs:
```
Descuento = 1 + (Skill / 100)
```

No afecta el precio de venta (siempre es ValorBase / 3).

### 5.2 Progresion

El skill sube tanto al comprar como al vender a NPCs. Cada transaccion es un intento de
uso del skill, con las reglas estandar de progresion (50 EXP al acertar, 20 al fallar).

### 5.3 Impacto Economico

Con skill 100, un jugador paga el **50% del precio base** por cualquier item. Esto representa
un ahorro significativo a lo largo del juego y hace que el skill de Comerciar sea valioso
incluso para clases de combate.

---

## 6. Ciclo Economico

El flujo economico general del juego sigue este patron:

```
Recoleccion (Pesca, Mineria, Tala)
        │
        v
Refinamiento (Fundir minerales en lingotes)
        │
        v
Produccion (Herreria, Carpinteria)
        │
        v
Uso (Equipamiento, Consumibles, Municiones)
        │
        v
Deterioro/Perdida (Muerte = items caen, Espada Matadragones se destruye)
        │
        v
Reciclaje (Fundir armas = recuperar % de lingotes)
```

### 6.1 Fuentes de Items

| Fuente | Tipo de Items |
|--------|--------------|
| NPCs comerciantes | Items basicos y avanzados (compra con oro) |
| Drops de NPCs | Items aleatorios de loot tables |
| Crafting (Herreria) | Armas y armaduras de metal |
| Crafting (Carpinteria) | Armas de madera, flechas, barcos |
| Recoleccion | Materias primas (pescado, minerales, madera) |
| Recompensas de faccion | Armaduras faccionarias |

### 6.2 Salidas de Items

| Salida | Mecanica |
|--------|----------|
| Muerte del jugador | Todos los items caen al piso (pueden ser recogidos por otros) |
| Venta a NPCs | Items desaparecen del juego (el NPC los absorbe) |
| Consumo | Pociones, comida, bebidas, municiones se gastan al usar |
| Destruccion especial | Espada Matadragones al matar dragon |
| Limpieza del mundo | Objetos temporales en el piso se eliminan periodicamente |

### 6.3 Observaciones de Balance

La economia del AO tiene estas caracteristicas:
- **Inflacion de oro**: los NPCs generan oro infinitamente (loot). Los sumideros son limitados.
- **Deflacion de items**: los items se destruyen al morir pero se crean por crafting y loot.
- **El Trabajador es la clase productora**: sin Trabajadores, la produccion de items de alta
  gama es extremadamente costosa en tiempo y skill.
- **El comercio entre jugadores** es la principal forma de redistribucion de riqueza.
- **Las facciones inyectan items premium** (armaduras faccionarias) que no se pueden craftear.

---

## 7. Relacion con Otros Sistemas

### 7.1 Inventario
- El inventario es el medio de intercambio para todo el comercio
- El banco extiende la capacidad de almacenamiento
- Los slots limitados obligan a decisiones de prioridad

### 7.2 Oficios
- Los oficios producen los items que se comercian
- Las materias primas tienen valor comercial propio
- El circuito recoleccion-refinamiento-produccion es el motor de la economia

### 7.3 Combate
- La muerte genera redistribucion forzada de items (loot del muerto)
- Los NPCs son la fuente primaria de oro y items raros
- El equipamiento de combate es la demanda principal de la economia

### 7.4 Facciones
- Las armaduras faccionarias solo se venden/compran a NPCs especificos (sastres)
- Las recompensas faccionarias inyectan items al sistema
- Items de faccion no pueden robarse ni comerciarse libremente

### 7.5 Reputacion
- Robar afecta la reputacion negativamente
- Trabajar (y por extension comerciar lo producido) sube reputacion Plebe
