# 13 - Protocolo y Red

> Este documento describe la capa de comunicacion entre cliente y servidor: la arquitectura
> de red, el protocolo binario, la serializacion de datos, el flujo de conexion, y el
> catalogo completo de tipos de paquetes en ambas direcciones. Este es el documento clave
> para garantizar compatibilidad con el cliente existente.

---

## 1. Arquitectura de Red

### 1.1 Modelo de Conexion

- **Protocolo de transporte**: TCP/IP
- **Puerto**: configurable (por defecto 7666)
- **Modelo**: un socket listener acepta conexiones entrantes; cada cliente mantiene una conexion TCP persistente durante toda la sesion
- **Codificacion de texto**: Windows-1252 (no UTF-8)
- **Buffer de recepcion**: 8,192 bytes por cliente

### 1.2 Modelo de Threading

La I/O de red es **asincrona** (patron Begin/End o equivalente moderno), pero todo el
**procesamiento de datos** se serializa en el hilo principal del game loop a traves de una
cola de mensajes thread-safe.

Los mensajes de red se encolan en la cola y se procesan cada **5 ms** (tick de procesamiento
de red del game loop). Esto garantiza que la logica del juego sea single-threaded y libre
de condiciones de carrera.

### 1.3 Identificacion de Clientes

Cada conexion recibe un **ID numerico unico** autoincrementable al conectarse. Este ID se
usa internamente para mapear la conexion al slot del usuario.

---

## 2. Serializacion Binaria

### 2.1 Formato General

Los paquetes se transmiten como un **stream binario continuo** sobre TCP. No hay delimitadores
de paquete, headers de longitud, ni framing a nivel de protocolo. La parsea es secuencial:
cada paquete comienza con un byte de tipo, y el parser sabe cuantos bytes leer segun el tipo.

### 2.2 Tipos Primitivos

| Tipo | Tamano | Byte Order | Descripcion |
|------|--------|------------|-------------|
| Byte | 1 byte | - | Valor 0-255 |
| Integer (Int16) | 2 bytes | Little-endian | Valor -32,768 a 32,767 |
| Long (Int32) | 4 bytes | Little-endian | Valor -2^31 a 2^31-1 |
| Single | 4 bytes | Little-endian | Punto flotante 32-bit |
| Double | 8 bytes | Little-endian | Punto flotante 64-bit |
| ASCIIString | 2 + N bytes | Length-prefixed | Int16 de longitud + N bytes de texto Windows-1252 |
| ASCIIStringFixed | N bytes | Raw | N bytes de texto sin prefijo de longitud |

### 2.3 Buffer de Serializacion

El sistema usa un buffer circular de **10 KB** por defecto para serializar y deserializar
paquetes. Operaciones disponibles:

**Escritura:** WriteByte, WriteInteger (Int16), WriteLong (Int32), WriteSingle, WriteDouble,
WriteASCIIString (con prefijo de longitud), WriteASCIIStringFixed (sin prefijo).

**Lectura:** ReadByte, ReadInteger, ReadLong, ReadSingle, ReadDouble, ReadASCIIString,
ReadASCIIStringFixed. Tambien existe PeekByte para leer sin consumir.

### 2.4 Estructura de un Paquete

```
[Tipo: 1 byte] [Campo1: N bytes] [Campo2: M bytes] ... [CampoN: K bytes]
```

No hay checksum, no hay longitud total, no hay versionado. El receptor debe conocer exactamente
la estructura de cada tipo de paquete para poder parsearlo correctamente.

---

## 3. Flujo de Conexion

### 3.1 Secuencia Completa

```
Cliente                                  Servidor
  |                                         |
  |---- Conexion TCP ---------------------->|
  |                                         |-- Verificar intervalo entre conexiones (>1000ms por IP)
  |                                         |-- Verificar limite de conexiones por IP (<10)
  |                                         |-- Verificar IP no baneada
  |                                         |-- Asignar socketID
  |                                         |
  |<--- (conexion aceptada) ---------------|
  |                                         |
  |---- Version del cliente + MD5 hash --->|
  |                                         |-- Validar version == ULTIMAVERSION
  |                                         |-- Validar MD5 (si esta habilitado)
  |                                         |
  |---- Login existente (nombre+pass) ---->|  (o)
  |---- Crear personaje nuevo ------------>|
  |                                         |
  |                                         |-- Validar credenciales / datos de creacion
  |                                         |-- Cargar o crear personaje
  |                                         |-- Asignar slot de usuario
  |                                         |
  |<--- Datos iniciales del mundo ---------|
  |     (mapa, musica, stats, inventario,   |
  |      hechizos, personajes cercanos)     |
  |                                         |
  |---- Comandos de juego ---------------->|
  |<--- Actualizaciones de estado ---------|
  |     (bucle continuo)                    |
  |                                         |
  |---- Desconexion ---------------------->|
  |     (o timeout por inactividad)         |
```

### 3.2 Validacion de Version

El cliente envia su numero de version y opcionalmente un hash MD5 del ejecutable. El servidor
compara:
- La version debe coincidir exactamente con `ULTIMAVERSION`
- El MD5 debe coincidir con la lista de hashes aceptados (si la validacion esta habilitada)

Si no coincide, se rechaza la conexion con un mensaje de error.

### 3.3 Login vs Creacion

El cliente puede enviar dos tipos de paquete inicial:
- **LoginExistingChar**: nombre + password para un personaje existente
- **LoginNewChar**: todos los datos de creacion (nombre, password, raza, clase, genero, cabeza, ciudad, atributos, etc.)

---

## 4. Paquetes del Cliente (Cliente -> Servidor)

Estos son todos los tipos de paquetes que el cliente puede enviar, organizados por categoria:

### 4.1 Conexion y Sesion

| Paquete | Descripcion |
|---------|-------------|
| LoginExistingChar | Login con personaje existente (nombre + password) |
| LoginNewChar | Crear personaje nuevo (todos los datos de creacion) |
| ThrowDices | Solicitar tirada de dados para atributos |
| Quit | Desconexion voluntaria (/salir) |

### 4.2 Movimiento

| Paquete | Descripcion |
|---------|-------------|
| Walk | Mover al personaje en una direccion (N/S/E/O) |

### 4.3 Combate

| Paquete | Descripcion |
|---------|-------------|
| Attack | Atacar en la direccion actual |
| CastSpell | Lanzar hechizo (indice del slot de hechizo) |
| UseSpellMacro | Macro de hechizo pre-configurado |
| LeftClick | Click izquierdo sobre un tile (targeting) |
| DoubleClick | Doble click sobre un tile (interaccion) |

### 4.4 Comunicacion

| Paquete | Descripcion |
|---------|-------------|
| Talk | Hablar (mensaje de chat normal) |
| Yell | Gritar (mensaje visible a mayor distancia) |
| Whisper | Susurrar a un jugador especifico |
| GuildChat | Mensaje al chat del clan |
| PartyChat | Mensaje al chat del party |

### 4.5 Inventario

| Paquete | Descripcion |
|---------|-------------|
| UseItem | Usar un item del inventario (slot) |
| EquipItem | Equipar un item |
| PickUp | Recoger objeto del piso |
| Drop | Tirar item al piso (slot + cantidad) |
| MoveItem | Mover item entre slots del inventario |

### 4.6 Comercio

| Paquete | Descripcion |
|---------|-------------|
| CommerceBuy | Comprar item del NPC comerciante |
| CommerceSell | Vender item al NPC comerciante |
| SafeTradeRequest | Solicitar comercio seguro con otro jugador |
| SafeTradeOffer | Ofrecer items en comercio seguro |
| SafeTradeAccept | Aceptar oferta del otro jugador |
| SafeTradeCancel | Cancelar comercio seguro |
| BankDeposit | Depositar item en el banco |
| BankWithdraw | Retirar item del banco |

### 4.7 Habilidades y Trabajo

| Paquete | Descripcion |
|---------|-------------|
| Work | Usar habilidad de trabajo (pesca, mineria, tala) |
| Meditate | Activar/desactivar meditacion |
| Rest | Activar/desactivar descanso |
| CraftBlacksmith | Construir item de herreria |
| CraftCarpenter | Construir item de carpinteria |
| SmeltItem | Fundir mineral o arma |
| AssignSkillPoint | Asignar punto de habilidad a un skill |

### 4.8 Clanes

| Paquete | Descripcion |
|---------|-------------|
| GuildCreate | Fundar un clan |
| GuildRequest | Solicitar ingreso a un clan |
| GuildAcceptMember | Aceptar aspirante |
| GuildRejectMember | Rechazar aspirante |
| GuildKickMember | Expulsar miembro |
| GuildLeave | Abandonar el clan |
| GuildUpdateNews | Actualizar noticias del clan |
| GuildDeclareWar | Declarar guerra a otro clan |
| GuildProposePeace | Proponer paz |
| GuildAcceptPeace | Aceptar propuesta de paz |
| GuildProposeAlliance | Proponer alianza |
| GuildAcceptAlliance | Aceptar alianza |
| GuildOpenElections | Abrir elecciones |
| GuildVote | Votar en elecciones |
| GuildGetInfo | Solicitar informacion del clan |
| GuildGetMembersList | Solicitar lista de miembros |
| GuildGetDetails | Solicitar detalles de un clan |

### 4.9 Party

| Paquete | Descripcion |
|---------|-------------|
| PartyCreate | Crear party |
| PartyJoin | Solicitar ingreso |
| PartyAccept | Aceptar solicitud |
| PartyKick | Expulsar miembro |
| PartyLeave | Abandonar party |
| PartyTransferLeadership | Transferir liderazgo |

### 4.10 Facciones

| Paquete | Descripcion |
|---------|-------------|
| EnlistRoyalArmy | Enlistarse en el Ejercito Real |
| EnlistChaosLegion | Enlistarse en la Legion Oscura |
| RequestFactionReward | Solicitar recompensa faccionaria |

### 4.11 Foro y Encuestas

| Paquete | Descripcion |
|---------|-------------|
| ForumPost | Publicar en el foro |
| ForumRequestPosts | Solicitar mensajes del foro |
| PollVote | Votar en una encuesta |
| PollRequest | Solicitar datos de la encuesta |

### 4.12 Anti-cheat

| Paquete | Descripcion |
|---------|-------------|
| SentinelAnswer | Responder al centinela anti-bot |

### 4.13 Comandos GM

Mas de **70 tipos de paquetes** para comandos administrativos, incluyendo:
- Teletransporte (/TELEP, /IRA, /IRPOS)
- Gestion de usuarios (/BAN, /UNBAN, /ECHAR, /CARCEL, /SILENCIAR)
- Spawn de NPCs (/ACC, /RACC)
- Creacion/destruccion de items (/CI, /DEST)
- Control del servidor (/APAGAR, /REINICIAR, /RELOADSINI)
- Control del mundo (/LLUVIA, /NOCHE, /LIMPIAR)
- Informacion (/INFO, /STAT, /INV, /SKILLS)
- Y muchos mas (ver documento 14-ADMIN-Y-SEGURIDAD.md)

---

## 5. Paquetes del Servidor (Servidor -> Cliente)

Estos son los tipos de paquetes que el servidor envia al cliente:

### 5.1 Mundo y Entidades

| Paquete | Descripcion |
|---------|-------------|
| CharacterCreate | Crear personaje visual en pantalla (body, head, arma, escudo, casco, nombre, FX) |
| CharacterRemove | Eliminar personaje visual de la pantalla |
| CharacterMove | Mover personaje en una direccion |
| CharacterChange | Cambiar apariencia de un personaje (body, head, equipamiento) |
| ObjectCreate | Crear objeto visual en el piso (grafico + posicion) |
| ObjectDelete | Eliminar objeto visual del piso |
| BlockPosition | Marcar/desmarcar un tile como bloqueado |
| ChangeMap | Cambiar de mapa (enviar datos del nuevo mapa) |
| AreaChanged | Notificar cambio de area (enviar franja de tiles nuevos) |

### 5.2 Stats y UI

| Paquete | Descripcion |
|---------|-------------|
| UpdateHP | Actualizar HP actual del jugador |
| UpdateMana | Actualizar mana actual |
| UpdateStamina | Actualizar stamina actual |
| UpdateGold | Actualizar oro en billetera |
| UpdateExp | Actualizar experiencia |
| UpdateHunger | Actualizar hambre |
| UpdateThirst | Actualizar sed |
| LevelUp | Notificar subida de nivel |
| ChangeInventorySlot | Actualizar un slot del inventario |
| ChangeBankSlot | Actualizar un slot del banco |
| ChangeSpellSlot | Actualizar un slot de hechizos |
| UpdateStrengthAndDexterity | Actualizar atributos visibles |

### 5.3 Combate y Efectos

| Paquete | Descripcion |
|---------|-------------|
| PlayMidi | Reproducir musica |
| PlayWave | Reproducir efecto de sonido |
| CreateFX | Crear efecto visual en una posicion |
| ParticleEffect | Crear efecto de particulas |
| Swing | Notificar intento de ataque (animacion) |

### 5.4 Comunicacion

| Paquete | Descripcion |
|---------|-------------|
| ChatOverHead | Mostrar texto sobre la cabeza de un personaje |
| ConsoleMessage | Mostrar mensaje en la consola del cliente |
| ErrorMessage | Mostrar mensaje de error |
| ShowMessageBox | Mostrar ventana de mensaje emergente |

Los mensajes de consola tienen tipos de fuente que determinan su color y estilo:
- TALK: chat normal
- FIGHT: mensajes de combate
- WARNING: advertencias
- INFO: informacion general
- GUILD: mensajes de clan
- PARTY: mensajes de party
- SERVER: mensajes del servidor
- CENTINELA: mensajes del sistema anti-bot
- GM: mensajes administrativos

### 5.5 Comercio

| Paquete | Descripcion |
|---------|-------------|
| NPCInventory | Enviar inventario del NPC comerciante |
| NPCOfferPrice | Enviar precio de un item |
| SafeTradeInit | Iniciar comercio seguro |
| SafeTradeUpdate | Actualizar oferta del otro jugador |
| SafeTradeComplete | Comercio seguro completado |
| SafeTradeCancel | Comercio seguro cancelado |
| BankInventory | Enviar inventario del banco |

### 5.6 Clanes y Social

| Paquete | Descripcion |
|---------|-------------|
| GuildInfo | Informacion del clan |
| GuildNews | Noticias del clan |
| GuildMembersList | Lista de miembros |
| GuildDetails | Detalles de un clan |
| GuildChatMessage | Mensaje del chat de clan |
| PartyUpdate | Actualizacion de datos de party |

### 5.7 Navegacion y Estado

| Paquete | Descripcion |
|---------|-------------|
| NavigateToggle | Activar/desactivar modo navegacion |
| MeditateToggle | Activar/desactivar meditacion |
| Disconnect | Forzar desconexion |
| PauseToggle | Pausar el juego |

---

## 6. Procesamiento de Paquetes

### 6.1 Paquetes Entrantes

Cada usuario tiene un **buffer de entrada**. Los datos recibidos por el socket se agregan
a este buffer. El procesamiento es:

1. El tick de red (cada 5ms) lee la cola de mensajes de red
2. Para cada usuario con datos en el buffer:
   a. Se lee el primer byte (tipo de paquete)
   b. Se identifica la estructura del paquete
   c. Se leen los campos correspondientes del buffer
   d. Se ejecuta la logica asociada
   e. Se repite hasta que el buffer este vacio o no haya suficientes bytes para el siguiente paquete

Si no hay suficientes bytes para completar un paquete, se deja el buffer intacto y se
procesa en el siguiente tick (los datos se acumulan hasta tener un paquete completo).

### 6.2 Paquetes Salientes

El servidor escribe los datos de salida en un **buffer de salida** por usuario. Los datos
se acumulan y se envian por el socket cada **10 ms** (tick de envio).

Esto permite **batching**: multiples actualizaciones de estado se agrupan en un solo envio
TCP, reduciendo overhead de red.

---

## 7. Relacion con Otros Sistemas

> **Nota**: Las consideraciones de compatibilidad para la reimplementacion se encuentran en
> [90-CONSIDERACIONES-REIMPLEMENTACION.md](90-CONSIDERACIONES-REIMPLEMENTACION.md).

### 7.1 Game Loop
- El procesamiento de red esta integrado en el game loop (tick de 5ms)
- El envio de datos es un subsistema separado (tick de 10ms)
- La logica de juego que generan los paquetes se ejecuta en el tick de 40ms

### 7.2 Areas de Interes
- El sistema de areas determina a quien se envian los paquetes
- Los cambios de area generan paquetes de datos incrementales
- Las transiciones de mapa generan paquetes completos

### 7.3 Seguridad
- La validacion de version y MD5 ocurre en la capa de protocolo
- Los limites de conexion por IP se verifican al nivel de socket
- El centinela usa paquetes especificos para desafios
