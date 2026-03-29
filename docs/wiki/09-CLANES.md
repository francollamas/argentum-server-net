# 09 - Clanes

> Este documento describe el sistema de clanes (guilds) de Argentum Online: fundacion,
> gestion de miembros, sistema de alineacion y antifaccion, elecciones de lider,
> guerras entre clanes, alianzas, y funciones de administracion.

---

## 1. Vision General

Los clanes son organizaciones permanentes de jugadores. A diferencia de las parties (grupos
temporales), los clanes persisten entre sesiones, tienen nombre, descripcion, reglas internas,
y pueden tener relaciones diplomaticas con otros clanes (guerras, alianzas).

El servidor soporta hasta **1,000 clanes** simultaneos.

---

## 2. Fundacion de un Clan

### 2.1 Requisitos del Fundador

Para fundar un clan, el jugador debe cumplir **todos** estos requisitos:

| Requisito | Valor |
|-----------|-------|
| Nivel minimo | 25 |
| Skill de Liderazgo | 90 o superior |
| No pertenecer a ningun clan | Debe estar libre |
| Compatibilidad de alineacion | Su estado debe ser compatible con la alineacion elegida |

### 2.2 Alineaciones de Clan

Existen **6 tipos de alineacion**, y cada uno impone restricciones sobre quien puede fundar
y quien puede ser miembro:

| Alineacion | Requisito del Fundador | Quien Puede Ser Miembro |
|-----------|----------------------|------------------------|
| Real (Armada) | Ser miembro del Ejercito Real | Solo miembros del Ejercito Real |
| Del Mal (Legion) | Ser miembro de la Legion Oscura | Solo miembros de la Legion Oscura |
| Legal (Ciudadano) | No ser criminal | Solo ciudadanos (no-criminales) |
| Criminal | Ser criminal | Solo criminales |
| Neutral | No pertenecer a ninguna faccion | Cualquiera sin faccion |
| Game Masters | Ser Dios o Admin | Solo GMs |

### 2.3 Nombre del Clan

- Solo se permiten letras minusculas (a-z), espacios, y la letra n con tilde
- No se permiten nombres duplicados
- El nombre es **inmutable** una vez creado

### 2.4 Al Crearse

- El fundador se asigna automaticamente como lider y primer miembro
- Se registra la fecha de fundacion
- Se notifica a todos los clanes existentes de la nueva fundacion
- Se inicializan las estructuras de datos del clan (descripcion vacia, codex vacio, etc.)

---

## 3. Datos del Clan

Cada clan almacena la siguiente informacion:

| Campo | Limite | Modificable por |
|-------|--------|----------------|
| Nombre | Inmutable | Nadie (se define al crear) |
| Fundador | Inmutable | Nadie |
| Lider | Cambia por elecciones | Sistema de elecciones |
| Descripcion | Max 256 caracteres | Lider |
| Codex | 8 entradas, max 256 chars cada una | Lider |
| Noticias del clan | Max 1024 caracteres | Solo el lider |
| URL del sitio web | Max 40 caracteres | Lider |
| Fecha de fundacion | Inmutable | Nadie |
| Puntos de antifaccion | 0 a 5 | Sistema automatico |

---

## 4. Gestion de Miembros

### 4.1 Solicitud de Ingreso (Aspirantes)

Un jugador puede solicitar ingreso a un clan. El proceso es:

1. El jugador envia una solicitud al clan deseado
2. La solicitud queda registrada como "aspirante"
3. El lider decide si acepta o rechaza

**Restricciones:**
- Los **newbies** no pueden solicitar ingreso
- Solo se puede ser aspirante a **un clan a la vez**
- Si se solicita ingreso a otro clan, la solicitud anterior se cancela automaticamente
- El clan puede acumular hasta **10 aspirantes** simultaneamente

### 4.2 Aceptacion de Aspirantes

Cuando el lider acepta a un aspirante:

1. Se verifica que el aspirante **sigue cumpliendo** los requisitos de alineacion del clan
   (puede haber cambiado de estado entre la solicitud y la aceptacion)
2. Si cumple, se agrega como miembro
3. Si esta online, su estado visual se actualiza inmediatamente
4. Se notifica al chat del clan

### 4.3 Rechazo de Aspirantes

Cuando el lider rechaza a un aspirante:
- Se registra un motivo de rechazo en el archivo del personaje
- Se notifica al jugador rechazado

### 4.4 Expulsion de Miembros

El lider puede expulsar a cualquier miembro excepto a si mismo:

- El lider **NO puede ser expulsado** por ningun miembro
- El lider **NO puede abandonar** el clan voluntariamente (debe transferir liderazgo primero)
- Los GMs pueden expulsar a cualquiera (sin restriccion de rango)
- El sistema de antifaccion puede expulsar automaticamente (ver seccion 5)

### 4.5 Notificaciones de Conexion

Cuando un miembro del clan se conecta o desconecta, se envia una notificacion al chat del
clan para que los demas miembros lo sepan.

---

## 5. Sistema de Antifaccion y Degradacion de Alineacion

### 5.1 Proposito

El sistema de antifaccion asegura que los miembros de un clan sigan cumpliendo los requisitos
de alineacion. Si un miembro cambia de estado (por ejemplo, un ciudadano se vuelve criminal
en un clan Legal), el sistema actua automaticamente.

### 5.2 Cuando se Activa

Se evalua periodicamente (cada 6 segundos, en el tick de mantenimiento lento del game loop)
y tambien al nivel 25 (cuando el jugador puede elegir faccion).

### 5.3 Logica de Antifaccion

**Si el miembro infractor es el lider:**
- La alineacion del clan **baja un grado**:
  - Real (Armada) -> Legal (Ciudadano)
  - Del Mal (Legion) -> Criminal
  - Legal, Criminal, Neutral, GM -> Neutral
- Se recalcula la membresia completa (todos los miembros se re-evaluan)

**Si el miembro infractor NO es el lider:**
- El miembro es **expulsado automaticamente**
- Se suma **1 punto de antifaccion** al clan
- Cuando los puntos de antifaccion alcanzan **5** (MAXANTIFACCION):
  - Se baja el grado de alineacion del clan (misma regla que para el lider)
  - Se resetean los puntos de antifaccion a 0

### 5.4 Cascada de Degradacion

La degradacion de alineacion puede causar una cascada: al bajar la alineacion, mas miembros
pueden dejar de cumplir los requisitos, lo que puede causar mas expulsiones o incluso otra
degradacion. El sistema se estabiliza cuando todos los miembros restantes cumplen la nueva
alineacion.

---

## 6. Elecciones de Lider

### 6.1 Apertura de Elecciones

Solo el **lider actual** puede abrir elecciones. Una vez abiertas:
- Duran exactamente **24 horas**
- No se pueden cancelar una vez iniciadas

### 6.2 Votacion

- Cualquier miembro del clan puede votar escribiendo `/VOTO NICKNAME`
- Solo se computa **un voto por miembro**
- El voto **no puede cambiarse** una vez emitido
- Los votos se persisten en archivo (sobreviven al reinicio del servidor)

### 6.3 Conteo

Al expirar las 24 horas, el servidor realiza el conteo automaticamente:

| Resultado | Accion |
|-----------|--------|
| Un ganador claro | Se convierte en nuevo lider; se publica en las noticias del clan |
| Empate | Se notifica al clan pero NO se cambia el lider |
| Sin votos | La votacion se cierra desierta, sin cambios |
| El ganador abandono el clan | La votacion queda desierta |

### 6.4 Post-Eleccion

- El archivo de votaciones se elimina tras el conteo
- El nuevo lider tiene plenos poderes inmediatamente
- Las noticias del clan se actualizan automaticamente con el resultado

---

## 7. Relaciones entre Clanes

Los clanes pueden tener tres estados de relacion entre si:

| Estado | Significado |
|--------|-------------|
| Paz | Estado por defecto. Sin interaccion hostil ni cooperativa |
| Guerra | Conflicto activo. Los miembros de ambos clanes son hostiles |
| Aliados | Cooperacion activa. Los miembros de ambos clanes se consideran amigos |

### 7.1 Declaracion de Guerra

- Solo el **lider** puede declarar guerra
- No se puede declarar guerra al propio clan
- No se puede declarar guerra a un clan con quien ya se esta en guerra
- La guerra es **bidireccional e inmediata**: no requiere aceptacion del otro clan
- Ambos clanes son notificados

### 7.2 Propuestas de Paz

Estando en guerra, un lider puede enviar una propuesta de paz:

1. El lider del clan A envia propuesta de paz al clan B
2. El lider del clan B puede **aceptar** o **rechazar**
3. **Si acepta**: ambos clanes pasan a estado de Paz inmediatamente
4. **Si rechaza**: se notifica en las noticias del clan proponente

### 7.3 Alianzas

- Solo se pueden proponer alianzas a clanes con los que se esta en **Paz**
- Requiere **propuesta y aceptacion** (no es unilateral como la guerra)
- Al aceptar, ambos clanes pasan a estado de Aliados

### 7.4 Implicaciones en el Juego

Las relaciones entre clanes afectan:
- **Propiedad de NPC**: miembros del mismo clan pueden atacar NPCs con dueno aliado
- **Combate PvP**: las guerras de clan pueden tener reglas especiales de engagement
- **Chat**: los miembros del clan comparten un canal de comunicacion exclusivo

---

## 8. Comunicacion del Clan

### 8.1 Chat de Clan

Los miembros online del clan comparten un canal de chat exclusivo. Los mensajes enviados
al chat del clan llegan a todos los miembros conectados.

### 8.2 Escucha por GMs

Los GMs pueden "escuchar" el chat de cualquier clan mediante un comando administrativo.
Esto los suscribe al canal de comunicaciones del clan sin ser miembros, permitiendo moderacion.

---

## 9. Persistencia

Los datos de los clanes se persisten en una base de datos de clanes separada de los archivos
de personaje. Cada clan tiene su propio registro con:
- Datos del clan (nombre, lider, descripcion, codex, noticias, alineacion, etc.)
- Lista de miembros
- Lista de aspirantes
- Relaciones con otros clanes (guerras, alianzas)
- Datos de elecciones (si hay una activa)

Los clanes se cargan al iniciar el servidor y se guardan durante el WorldSave.

---

## 10. Evento de Nivel 25

Al alcanzar el nivel 25, si el personaje pertenece a un clan con alineacion faccionaria
(Real o del Mal), es **expulsado automaticamente** del clan. Esto obliga al jugador a
elegir faccion por si mismo y unirse a un clan apropiado conscientemente.

---

## 11. Relacion con Otros Sistemas

### 11.1 Facciones
- La alineacion del clan esta directamente vinculada al sistema de facciones
- Los clanes Real y del Mal tienen requisitos de membresia faccionarios
- El cambio de faccion de un miembro puede activar el sistema de antifaccion

### 11.2 Combate
- Miembros del mismo clan pueden atacar NPCs apropiados por un companero
- Las guerras de clan definen hostilidades entre grupos de jugadores
- Las alianzas de clan facilitan la cooperacion en combate

### 11.3 Party
- Los clanes y las parties son sistemas independientes
- Un jugador puede estar en un clan y en una party simultaneamente
- Los clanes son permanentes, las parties son temporales

### 11.4 Administracion
- Los GMs pueden escuchar chats de clan
- Los GMs pueden expulsar miembros de cualquier clan
- Los GMs tienen su propia alineacion de clan (GM)
