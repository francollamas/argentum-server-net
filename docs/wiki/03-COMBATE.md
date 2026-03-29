# 03 - Combate

> Este documento describe el sistema de combate completo de Argentum Online: formulas de impacto,
> evasion, dano, bloqueo con escudo, habilidades especiales de clase, envenenamiento, experiencia
> por combate, reglas de PvP vs PvE, reputacion en combate, y todos los cooldowns asociados.
> El combate es el sistema central del gameplay y el que mas interacciones tiene con otros sistemas.

---

## 1. Vision General del Combate

El combate en Argentum Online es en **tiempo real** con sistema de turnos implicitos (cooldowns).
No hay targeting automatico: el jugador ataca en la direccion que esta mirando, y el impacto
depende de si hay un objetivo valido en la celda adyacente en esa direccion.

El flujo de un ataque fisico es:

```
1. Verificar cooldown de ataque
2. Verificar stamina suficiente (>= 10)
3. Verificar que hay un objetivo valido en la celda adyacente
4. Verificar reglas de PvP (seguro, facciones, zonas)
5. Calcular probabilidad de impacto (ataque vs evasion)
6. Si impacta:
   a. Calcular dano base
   b. Aplicar absorcion de armadura
   c. Aplicar habilidades especiales (apunalar, golpe critico, acuchillar)
   d. Aplicar envenenamiento (si corresponde)
   e. Infligir dano
   f. Otorgar experiencia
   g. Intentar subir skills de combate
7. Si falla:
   a. Evaluar si fue bloqueo con escudo o evasion
   b. Intentar subir skills defensivos
8. Consumir stamina
9. Resetear cooldown
```

---

## 2. Probabilidad de Impacto (Hit/Miss)

### 2.1 Formula Central

La probabilidad de que un ataque impacte se calcula como:

```
Probabilidad = 50 + (PoderAtaque - PoderEvasion) x 0.4
```

Esta probabilidad se **clampea** siempre entre **10% y 90%**. Es decir:
- Sin importar cuan superior sea el atacante, siempre hay al menos 10% de chance de fallar
- Sin importar cuan superior sea el defensor, siempre hay al menos 10% de chance de ser golpeado

Se genera un numero aleatorio entre 1 y 100. Si es menor o igual a la probabilidad, hay impacto.

### 2.2 Poder de Ataque del Jugador

El poder de ataque depende del tipo de arma equipada. Existen tres variantes:

#### Ataque con Arma Cuerpo a Cuerpo

La formula escala por tramos de skill de Armas:

| Rango de Skill | Formula |
|----------------|---------|
| 0 - 30 | SkillArmas x ModClaseAtaqueArmas |
| 31 - 60 | (SkillArmas + Agilidad) x ModClaseAtaqueArmas |
| 61 - 90 | (SkillArmas + 2 x Agilidad) x ModClaseAtaqueArmas |
| 91 - 100 | (SkillArmas + 3 x Agilidad) x ModClaseAtaqueArmas |

A este resultado se le suma un **bonus por nivel** a partir del nivel 13:
```
Bonus = 2.5 x max(0, Nivel - 12)
```

#### Ataque con Proyectil (Arco)

Misma estructura escalonada pero usa:
- Skill de Proyectiles en lugar de Armas
- ModClaseAtaqueProyectiles en lugar de ModClaseAtaqueArmas
- Mismo bonus por nivel

#### Ataque con Wrestling (Punos)

Misma estructura escalonada pero usa:
- Skill de Wrestling en lugar de Armas
- ModClaseAtaqueWrestling en lugar de ModClaseAtaqueArmas
- Mismo bonus por nivel

Se activa automaticamente cuando el jugador no tiene arma equipada.

### 2.3 Poder de Evasion del Jugador

```
PoderEvasion = (SkillTacticas + SkillTacticas/33 x Agilidad) x ModClaseEvasion + 2.5 x max(0, Nivel - 12)
```

Si el jugador tiene escudo equipado, se suma el poder de evasion del escudo:
```
PoderEvasionEscudo = SkillDefensa x ModClaseEscudo / 2
```

### 2.4 Penalizacion por Meditacion

Si la victima esta meditando, su capacidad de evadir se reduce. Concretamente, la probabilidad
de que el ataque **falle** se multiplica por 0.75 (es decir, pierde un 25% de su evasion efectiva).

La probabilidad final sigue sin poder superar 90%.

### 2.5 Modificadores de Clase (ModClase)

Cada clase tiene multiplicadores propios para cada aspecto del combate. Estos valores se cargan
desde el archivo de balance y no estan hardcodeados:

| Modificador | Que afecta |
|-------------|-----------|
| ModClaseEvasion | Multiplicador de evasion |
| ModClaseAtaqueArmas | Multiplicador de precision con armas melee |
| ModClaseAtaqueProyectiles | Multiplicador de precision con arcos |
| ModClaseAtaqueWrestling | Multiplicador de precision sin armas |
| ModClaseDanoArmas | Multiplicador de dano con armas melee |
| ModClaseDanoProyectiles | Multiplicador de dano con arcos |
| ModClaseDanoWrestling | Multiplicador de dano sin armas |
| ModClaseEscudo | Multiplicador de defensa con escudo |

---

## 3. Bloqueo con Escudo

### 3.1 Cuando se Evalua

El bloqueo con escudo se evalua **solo cuando un ataque falla** (no impacta). Es decir, no es
un chequeo independiente sino una sub-evaluacion del fallo: determina si el fallo fue por
esquivar o por bloquear con el escudo.

### 3.2 Condicion

El jugador debe tener un escudo equipado.

### 3.3 Formula

```
ProbRechazo = 100 x SkillDefensa / (SkillDefensa + SkillTacticas)
```

Clampeado entre 10% y 90%.

Se genera un numero aleatorio. Si indica rechazo con escudo:
- Se reproduce un efecto de sonido de bloqueo
- Se notifica al jugador que bloqueo con el escudo

Independientemente de si el bloqueo fue o no la causa del fallo, el skill de Defensa intenta
subir experiencia.

---

## 4. Calculo de Dano Fisico

### 4.1 Dano Base del Jugador con Arma

Cuando el jugador tiene un arma equipada:

1. Se obtiene un dano aleatorio entre **MinHIT y MaxHIT del arma**
2. Si es un arma de proyectil con municion equipada, se suma un dano aleatorio de la municion
   (entre MinHIT y MaxHIT de la municion)

#### Caso Especial: Espada Matadragones

- **Contra un dragon**: el dano es igual a `MinHP del dragon + Defensa del dragon` (mata de
  un golpe efectivo). Al matar al dragon, la espada se destruye
- **Contra cualquier otro objetivo**: dano fijo de 1

### 4.2 Dano Base del Jugador sin Arma (Wrestling)

- Dano base: aleatorio entre 4 y 9
- Si tiene guantes equipados en el slot de anillo, se suma el dano del guante (MinHIT a MaxHIT)

### 4.3 Formula Final de Dano

```
Dano = (3 x DanoArma + DanoMaxArma/5 x max(0, Fuerza - 15) + DanoUsuario) x ModClaseDano
```

Donde:
- **DanoArma**: el aleatorio entre min/max del arma (paso 4.1 o 4.2)
- **DanoMaxArma**: el MaxHIT del arma
- **Fuerza**: atributo de fuerza actual. Solo contribuye si supera 15
- **DanoUsuario**: aleatorio entre MinHIT y MaxHIT propios del personaje (stats de golpe)
- **ModClaseDano**: el multiplicador de dano de la clase (segun tipo de ataque)

### 4.4 Dano Adicional por Navegacion

Si el atacante esta navegando en un barco, se suma dano adicional:
```
DanoBarco = aleatorio(MinHIT_barco, MaxHIT_barco)
```

### 4.5 Absorcion de Dano (Armadura)

El golpe impacta aleatoriamente en **cabeza** o **torso**:

**Si impacta en cabeza:**
- Si tiene casco equipado: absorbe un valor aleatorio entre MinDef y MaxDef del casco
- Si no tiene casco: absorcion = 0

**Si impacta en torso:**
- Si tiene armadura: absorbe un valor aleatorio entre MinDef y MaxDef de la armadura
- Si ademas tiene escudo: se suma la defensa del escudo (MinDef a MaxDef)
- Si no tiene armadura: absorcion = 0

**Penetracion de armadura (solo PvP):**
Si el arma del atacante tiene el atributo "Refuerzo" (penetracion), este valor se resta de la
absorcion total. Esto NO aplica contra NPCs.

**Defensa del barco:**
Si el defensor esta navegando, se suma la defensa del barco a la absorcion.

**Dano minimo:**
- **Contra jugadores (PvP)**: el dano final nunca puede ser menor a **1**
- **Contra NPCs**: el dano puede ser **0** si la defensa supera el ataque

### 4.6 Dano de NPC a Jugador

Los NPCs calculan su dano de forma mas simple:
- Dano = aleatorio(MinHIT_npc, MaxHIT_npc)
- La absorcion del jugador se calcula igual que en PvP (cabeza o torso, armadura, casco, escudo)
- El dano minimo contra un jugador es siempre **1**

---

## 5. Habilidades Especiales de Combate

### 5.1 Apunalar (Backstab)

**Que es:** un ataque sorpresa con dano multiplicado. Se evalua despues de cada golpe exitoso
si se cumple la condicion de estar atacando desde atras.

**Quien puede:** Asesino (sin minimo), Clerigo, Paladin, Pirata (con minimo de skill), Bardo.
Requiere un arma con la propiedad "Apunala" equipada.

**Probabilidad:** varia significativamente por clase, usando curvas cubicas:

| Clase | Probabilidad | Nota |
|-------|-------------|------|
| Asesino | La mas alta (~4-12% segun skill) | Formula cubica agresiva |
| Clerigo, Paladin, Pirata | Intermedia | Formula cubica conservadora |
| Bardo | Intermedia propia | Formula cubica propia |
| Demas clases | La mas baja | Formula lineal |

**Dano:**

| Situacion | Multiplicador |
|-----------|--------------|
| Asesino contra jugador | x 1.4 del dano base |
| Otra clase contra jugador | x 1.5 del dano base |
| Cualquier clase contra NPC | x 2.0 del dano base |

Nota: paradojicamente el Asesino hace MENOS dano por apunalada que otras clases contra jugadores,
pero compensa con MAYOR probabilidad de activarla.

### 5.2 Golpe Critico (Solo Bandido)

**Que es:** un golpe especialmente devastador exclusivo del Bandido.

**Requisito:** tener la **Espada Vikinga** equipada.

**Probabilidad:** formula cubica basada en el skill de Wrestling:
```
Suerte = ((0.00000003 x Skill^2 + 0.000006) x Skill + 0.000107) x Skill + 0.0893) x 100
```

Es una probabilidad baja que escala lentamente. Sirve como bonus aleatorio, no como mecanica
principal.

**Dano adicional:** 75% del dano normal del golpe.

Aplica tanto contra NPCs como contra jugadores.

### 5.3 Acuchillar (Solo Pirata)

**Que es:** una habilidad exclusiva del Pirata que funciona con armas arrojadizas y cuerpo a cuerpo.

**Requisito:** ser Pirata con un arma que tenga la propiedad "Acuchilla" equipada.

**Probabilidad:** constante configurada (20% por defecto).

**Dano adicional:** multiplicador fijo (20% extra del dano base por defecto).

### 5.4 Desequipar al Oponente (Bandido con Guantes de Hurto)

**Que es:** al golpear con los punos (sin arma equipada, pero con guantes de hurto), el Bandido
intenta desequipar un item del oponente.

**Orden de prioridad:** escudo, luego arma, luego casco (intenta en ese orden, el primero que
encuentre equipado).

**Probabilidad:**
```
Probabilidad = SkillWrestling x 0.2 + Nivel x 0.66
```

Si la victima es menor a nivel 20, recibe notificacion de que item le desequiparon.

### 5.5 Paralisis Manual (Ladron con Guantes de Hurto)

**Que es:** al golpear con los punos y guantes de hurto, el Ladron tiene chance de paralizar
a la victima.

**Duracion:** la mitad de la duracion normal de paralisis.

**Probabilidad:**
```
Probabilidad = SkillWrestling / 4
```

### 5.6 Desarmar (Ladron)

**Que es:** cada vez que el Ladron golpea exitosamente, intenta quitarle el arma equipada al
oponente.

**Probabilidad:**
```
Probabilidad = SkillWrestling x 0.2 + Nivel x 0.66
```

### 5.7 Hurto de Items en Combate (Bandido)

**Que es:** el Bandido con guantes de hurto puede robar items del inventario del oponente
durante el combate.

Este sistema esta vinculado al sistema de robo (ver documento de Oficios) pero se activa
en contexto de combate.

---

## 6. Envenenamiento por Arma

### 6.1 Armas que Envenenan

Si el arma equipada tiene la propiedad "Envenena", o en caso de proyectiles, si la municion
tiene esa propiedad, hay chance de envenenar al objetivo en cada golpe exitoso.

### 6.2 Probabilidad

- **Jugador con arma/municion venenosa:** 60% por golpe exitoso
- **NPC con propiedad de veneno:** 30% por golpe exitoso

### 6.3 Efecto del Veneno

Una vez envenenado, el personaje recibe dano periodico:
- Dano por tick de veneno: aleatorio entre 1 y 5 HP
- Frecuencia: cada N ticks del game loop (configurable via IntervaloVeneno)
- El veneno puede matar al personaje
- Se cura con: pocion violeta (antidoto) o hechizo de cura de veneno
- Se limpia automaticamente al morir

---

## 7. Interrumpir Meditacion en Combate

Cuando un jugador esta meditando y recibe dano, la meditacion puede interrumpirse.

**Formula del umbral de interrupcion:**
```
Umbral = MinHP / 100 x Inteligencia x SkillMeditar / 100 x 12 / (aleatorio(0,5) + 7)
```

Si el dano recibido supera este umbral, la meditacion se cancela. Esto significa:
- Jugadores con mucha vida, alta inteligencia y alto skill de Meditar son mas dificiles de interrumpir
- El componente aleatorio introduce variabilidad

---

## 8. Experiencia por Combate

### 8.1 Experiencia por Matar NPCs

La experiencia se otorga **proporcionalmente al dano infligido**:

```
ExpPorGolpe = Dano x (ExpTotalNPC / MaxHP_NPC)
```

Esto distribuye la experiencia total del NPC entre todos los que le hicieron dano, en proporcion
a cuanto contribuyo cada uno. La experiencia se acumula como fracciones y se entrega como enteros.

La experiencia restante (fracciones acumuladas no entregadas) se otorga al jugador que da el
**golpe final** (el que lo mata).

Si el jugador esta en **party**, la experiencia se comparte con el grupo segun las reglas de
distribucion de la party (ver documento 10-PARTY.md).

### 8.2 Experiencia por Matar Jugadores (PvP)

```
Experiencia = Nivel_victima x 2
```

### 8.3 Experiencia por Mascotas

Si una mascota del jugador participa en el combate (ataca al NPC), la experiencia de esos golpes
no se entrega inmediatamente al dueno. Se acumula y se entrega toda al jugador que da el golpe
final al NPC.

---

## 9. Consumo de Stamina en Combate

Cada ataque fisico consume entre **1 y 10** puntos de stamina aleatoriamente.

Si la stamina del personaje es menor a 10, no puede atacar. El servidor envia el mensaje:
"Estas muy cansado para luchar."

---

## 10. Reglas de PvP (Jugador contra Jugador)

### 10.1 Sistema de Seguro

El jugador puede activar o desactivar un "seguro" que impide atacar accidentalmente a ciudadanos.

- **Seguro activado**: no se puede atacar a ciudadanos (se bloquea el ataque)
- **Seguro desactivado**: se puede atacar a cualquiera, pero con consecuencias de reputacion
- Los miembros del **Ejercito Real** tienen el seguro siempre activo contra ciudadanos (no se puede desactivar para atacar ciudadanos)

### 10.2 Reglas de Quien Puede Atacar a Quien

| Atacante | Objetivo | Permitido | Consecuencia |
|----------|----------|-----------|-------------|
| Ciudadano (seguro off) | Ciudadano | Si | Se convierte en criminal. Suma Bandido, pierde Noble |
| Ciudadano | Criminal | Si | Suma Noble |
| Criminal | Ciudadano | Si | Suma Asesino (si mata) |
| Criminal | Criminal | Si | Sin consecuencia de rep |
| Armada Real | Ciudadano | **No** | Bloqueado siempre |
| Armada Real | Criminal | Si | Suma Noble faccionario |
| Legion Oscura | Legion Oscura | **No** | No pueden atacarse entre si |
| Legion Oscura | Ciudadano | Si | Suma kills faccionarios |

### 10.3 Estado "Atacable"

Cuando un ciudadano ataca a otro ciudadano, el atacante queda marcado como "atacable" durante
60 segundos. Esto permite que la victima contraataque sin consecuencias de reputacion.

### 10.4 Zonas Seguras

En tiles con trigger de **zona segura** (trigger 4):
- No se puede atacar a ningun jugador
- No se puede ser atacado
- No se puede robar

### 10.5 Arenas de Pelea

En tiles con trigger de **zona de pelea** (trigger 6):
- Se puede pelear libremente
- **No se pierden items al morir**
- **No cambia la reputacion** al matar
- Es un espacio seguro para PvP competitivo sin consecuencias

### 10.6 Restricciones de Armada de Alto Rango

Los miembros de la Armada Real de alto rango pueden atacar en las ciudades de su faccion
(mapas especificos de ciudades reales).

Los miembros del Caos de alto rango pueden atacar en las ciudades de su faccion
(mapas especificos de ciudades del caos).

### 10.7 Consecuencias de Matar

**Matar a un ciudadano:**
- El atacante gana Asesino x 2
- Pierde TODA la reputacion de Burgues, Noble y Plebe (se ponen en 0)
- Se convierte en criminal inmediatamente

**Matar a un criminal:**
- El atacante gana reputacion Noble

**Matar en arena:**
- Sin consecuencias de reputacion

**Matar guardias:**
- Masivamente suma Asesino
- Borra completamente Noble y Plebe

**Matar NPCs no hostiles (animales pacificos, etc.):**
- Suma puntos de Asesino

**Matar NPCs hostiles:**
- Suma puntos de Plebe/Cazador

### 10.8 Resucitar Ciudadanos

Resucitar a un ciudadano muerto otorga **500 puntos de Nobleza** al caster.

---

## 11. Diferencias entre PvP y PvE

| Aspecto | PvP (contra jugadores) | PvE (contra NPCs) |
|---------|----------------------|-------------------|
| Dano minimo | Siempre 1 (nunca 0) | Puede ser 0 si defensa > ataque |
| Penetracion de armadura | El "Refuerzo" del arma reduce absorcion | No aplica |
| Apunalar | Asesino x1.4, otros x1.5 | Siempre x2.0 |
| Experiencia | Nivel_victima x 2 | Proporcional al dano / HP total |
| Reputacion | Cambia segun alineacion | Cambia segun tipo de NPC |
| Propiedad de NPC | N/A | El primero en atacar "se apropia" |
| Zonas | Seguras y arenas tienen reglas especiales | NPCs pueden atacar en cualquier zona no segura |

---

## 12. Propiedad de NPC (Ownership)

### 12.1 Mecanica

Cuando un jugador ataca a un NPC que no tiene dueno, se lo "apropia". Esto significa:
- Otros jugadores no pueden atacar ese NPC
- La propiedad dura un tiempo limitado (18 segundos por defecto)
- Si el dueno no ataca al NPC en ese tiempo, pierde la propiedad

### 12.2 Excepciones

- Miembros del mismo clan o party pueden atacar el NPC sin robarlo
- Los hechizos de paralisis/inmovilizacion pueden afectar un NPC con dueno sin robarlo
- No se puede re-paralizar un NPC que ya esta paralizado y pertenece a un aliado de faccion
- Las mascotas atacando no se apropian de NPCs

---

## 13. Combate de NPCs

### 13.1 NPC vs Jugador

Los NPCs que pueden atacar (hostiles, guardias, etc.) atacan al jugador en rango:
- Tiran dano aleatorio entre su MinHIT y MaxHIT
- El jugador absorbe con armadura/casco/escudo normalmente
- El dano minimo contra jugador es siempre 1
- Los NPCs con propiedad de veneno tienen 30% de chance de envenenar
- Los NPCs con doble ataque pueden golpear fisicamente Y lanzar un hechizo (50% de chance
  de lanzar hechizo como segundo ataque)

### 13.2 NPC vs NPC

Las mascotas de jugadores pueden pelear contra NPCs hostiles:
- Se evalua impacto con la misma formula general
- El dano se calcula NPC vs NPC
- La experiencia de los golpes de la mascota se acumula para el dueno

Ciertos NPCs magicos (como elementales de fuego) pueden lanzar hechizos contra otros NPCs.

---

## 14. Intervalos y Cooldowns de Combate

### 14.1 Cooldowns de Ataque

| Tipo de Ataque | Cooldown |
|----------------|----------|
| Melee (cuerpo a cuerpo) | IntervaloUserPuedeAtacar (configurable) |
| Proyectil (arco) | IntervaloFlechasCazadores (configurable) |
| Hechizo | IntervaloLanzarHechizo (configurable) |

### 14.2 Cooldowns Cruzados

Estos son criticos para el balance del PvP:

| Transicion | Cooldown | Efecto |
|------------|----------|--------|
| Hechizo -> Golpe melee | IntervaloMagiaGolpe | Despues de castear, no puede golpear inmediatamente |
| Golpe melee -> Hechizo | IntervaloGolpeMagia | Despues de golpear, no puede castear inmediatamente |
| Golpe melee -> Usar item | IntervaloGolpeUsar | Despues de golpear, no puede tomar pocion inmediatamente |

Estos cooldowns cruzados impiden "combos" instantaneos de magia + golpe + pocion, que serian
extremadamente poderosos y desequilibrados.

### 14.3 Cooldown de Ataque de NPCs

Los NPCs tienen un ciclo de ataque global: cada ~4 segundos se resetea su flag de "puede atacar",
habilitandolos para un nuevo golpe. Esto controla la velocidad de ataque de las criaturas
independientemente de la IA.

---

## 15. Skills que Suben en Combate

| Accion | Skills que intentan subir |
|--------|--------------------------|
| Golpe melee exitoso | Armas |
| Golpe melee fallido | Armas (menor probabilidad) |
| Disparo de arco exitoso | Proyectiles |
| Disparo de arco fallido | Proyectiles (menor probabilidad) |
| Golpe de wrestling exitoso | Wrestling |
| Esquivar un ataque | Tacticas |
| Ser golpeado | Tacticas (menor probabilidad) |
| Bloquear con escudo | Defensa |
| Apunalar exitoso | Apunalar |
| Lanzar hechizo exitoso | Magia |

Recordar: al acertar una accion de skill se ganan 50 EXP de skill, al fallar se ganan 20 EXP.
Al subir un punto de skill se ganan 50 EXP generales.
