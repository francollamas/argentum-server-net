# 14 - Administracion y Seguridad

> Este documento describe los sistemas de administracion del servidor (roles de GM, comandos
> administrativos, moderacion), los sistemas de seguridad (centinela anti-bot, proteccion de IP,
> validacion de clientes), y los sistemas auxiliares (foro in-game, encuestas populares).

---

## 1. Niveles de Privilegio

### 1.1 Jerarquia de Roles

Los privilegios se manejan con **bitmasks**, permitiendo roles combinados. Un usuario puede
tener multiples roles simultaneamente.

| Rol | Nivel | Descripcion |
|-----|-------|-------------|
| User | Basico | Jugador normal. Sin privilegios administrativos |
| Consejero | 1 | Primer nivel de moderacion. Observacion y asistencia basica |
| SemiDios | 2 | Moderador intermedio. Mas poderes de gestion |
| Dios | 3 | Administrador con amplios poderes sobre jugadores y mundo |
| Admin | 4 | Maximo nivel. Control total del servidor |
| RoleMaster | Especial | Master de juego de rol. Gestiona eventos narrativos |
| ChaosCouncil | Especial | Miembro del Consejo de la faccion Caos |
| RoyalCouncil | Especial | Miembro del Consejo de la faccion Real |

### 1.2 Asignacion de Privilegios

Los privilegios se asignan mediante listas de nombres en el archivo de configuracion del servidor:
- Lista de Admins
- Lista de Dioses
- Lista de SemiDioses
- Lista de Consejeros
- Lista de RoleMasters

Al loguearse, el servidor verifica si el nombre del personaje esta en alguna de estas listas
y asigna los privilegios correspondientes.

### 1.3 Proteccion Jerarquica

Existe una proteccion contra abusos dentro de la jerarquia:
- **No se puede banear a alguien de mayor rango**
- **Si se banea a alguien del mismo rango, el baneador tambien es baneado automaticamente**
- Los Admins son el unico rango que puede actuar sobre todos los demas sin restriccion

### 1.4 Restricciones de GMs

Los GMs tienen algunas restricciones de jugador normal:
- Los Consejeros no pueden vender items (restriccion especifica)
- Ciertos GMs pueden no ser perseguibles por NPCs (flag AdminPerseguible)
- Los GMs pueden hacerse invisibles administrativamente (diferente de la invisibilidad de juego)

---

## 2. Comandos Administrativos

Los comandos administrativos se clasifican por categoria funcional. Cada comando esta
disponible segun el nivel de privilegio del GM.

### 2.1 Gestion de Jugadores

| Comando | Efecto | Nivel Minimo |
|---------|--------|-------------|
| Ban | Banear a un jugador (online u offline). Registra razon, fecha, y pena | SemiDios |
| Unban | Remover ban de un jugador. Limpia el registro | SemiDios |
| Echar (Kick) | Desconectar a un jugador forzosamente | Consejero |
| Encarcelar | Teletransportar al jugador a la prision por N minutos | SemiDios |
| Silenciar | Impedir que un jugador pueda hablar | Consejero |
| Borrar Usuario | Eliminar el archivo del personaje del disco permanentemente | Admin |
| Forzar Desconexion | Cerrar la conexion del socket directamente | SemiDios |

**Detalles del ban:**
- Se puede banear a jugadores online (se los desconecta inmediatamente) u offline
- Se registra: razon del ban, fecha, quien lo baneo
- El ban se marca en el archivo del personaje
- Al intentar loguearse baneado, se rechaza la conexion

**Detalles del encarcelamiento:**
- El jugador es teletransportado al mapa de prision (mapa 66, posicion 75,47)
- Si estaba viajando a casa (/home), se cancela el viaje
- La pena se cuenta en minutos y se purga automaticamente (1 minuto real = 1 minuto de pena)
- Al cumplir la pena, el jugador es teletransportado a la salida (mapa 66, posicion 75,65)

### 2.2 Teletransporte

| Comando | Efecto |
|---------|--------|
| Teletransportar a jugador | Mover a un jugador a una posicion especifica |
| Ir a jugador | El GM se teletransporta a la posicion de un jugador |
| Ir a posicion | El GM se teletransporta a coordenadas especificas (mapa, X, Y) |
| Traer jugador | Teletransportar a un jugador a la posicion del GM |
| Ir a mapa de GMs | El GM se teletransporta al mapa exclusivo de GMs (mapa 49) |

### 2.3 Spawn y Gestion de Mundo

| Comando | Efecto |
|---------|--------|
| Crear NPC | Spawnear un NPC en la posicion del GM |
| Crear NPC con respawn | Spawnear un NPC que reaparecer al morir |
| Crear item | Crear un item en el inventario del GM o en el piso |
| Destruir items | Eliminar items del piso en el area |
| Limpiar mapa | Eliminar todos los items del piso en el mapa actual |
| Lluvia | Activar/desactivar la lluvia manualmente |
| Noche | Activar/desactivar la noche manualmente |
| Bloquear tile | Marcar/desmarcar un tile como bloqueado |

### 2.4 Informacion y Monitoreo

| Comando | Efecto |
|---------|--------|
| Info de jugador | Ver stats completos de un jugador |
| Stats de jugador | Ver atributos y nivel de un jugador |
| Inventario de jugador | Ver el inventario completo de un jugador |
| Boveda de jugador | Ver el banco de un jugador |
| Skills de jugador | Ver las 20 habilidades de un jugador |
| Balance de jugador | Ver oro y reputacion de un jugador |
| Quien esta online | Listar jugadores conectados |
| Escuchar clan | Suscribirse al chat de un clan para monitorearlo |

### 2.5 Control del Servidor

| Comando | Efecto |
|---------|--------|
| Apagar servidor | Iniciar secuencia de apagado controlado |
| Reiniciar servidor | Reiniciar el servidor |
| Recargar configuracion | Recargar el archivo de configuracion sin reiniciar |
| Recargar objetos | Recargar definiciones de objetos |
| Recargar hechizos | Recargar definiciones de hechizos |
| WorldSave manual | Forzar un guardado completo del mundo |
| Guardar mapa | Guardar el estado del mapa actual |

### 2.6 Gestion de Facciones

| Comando | Efecto |
|---------|--------|
| Aceptar consejero | Agregar jugador como consejero de faccion |
| Quitar del Caos | Expulsar a un jugador de la Legion Oscura |
| Quitar del Real | Expulsar a un jugador del Ejercito Real |

### 2.7 Ban de IP

| Comando | Efecto |
|---------|--------|
| Banear IP | Agregar una IP a la lista de IPs baneadas |
| Desbanear IP | Remover una IP de la lista |

Las IPs baneadas se persisten en un archivo y se cargan al iniciar el servidor.

---

## 3. Sistema Centinela (Anti-Bot)

### 3.1 Proposito

El centinela es un sistema automatizado que detecta jugadores usando **macros inasistidos**
(bots que trabajan automaticamente sin supervision humana). Es especialmente relevante para
los oficios (pesca, mineria, tala) que son actividades repetitivas.

### 3.2 Funcionamiento

El ciclo del centinela opera asi:

1. **Deteccion**: el sistema busca periodicamente jugadores que esten **trabajando activamente**
   (el contador de trabajo es mayor a 0) y que sean usuarios normales (no GMs)

2. **Aparicion**: un NPC "Centinela" aparece junto al jugador sospechoso

3. **Desafio**: el centinela le dice por chat y por consola:
   ```
   "Escribe /CENTINELA [clave]"
   ```
   Donde la clave es un **numero aleatorio entre 1 y 32,000**

4. **Espera**: el jugador tiene **2 minutos** para responder correctamente

5. **Recordatorios**: cada minuto se reenvia el desafio con efectos visuales y sonoros
   (para asegurarse de que el jugador lo vea)

6. **Reposicionamiento**: si el centinela se aleja mas de 5 tiles del jugador (porque el
   jugador se movio), el centinela se reposiciona junto al jugador

### 3.3 Resultados Posibles

| Resultado | Accion |
|-----------|--------|
| Respuesta correcta | El jugador queda marcado como verificado (CentinelaOK). El centinela se retira |
| Timeout (no responde en 2 min) | El jugador es **baneado automaticamente** por "macro inasistido". Se registra pena y se cierra la conexion |
| Desconexion durante el desafio | Se registra el evento en log (sospechoso de evasion de centinela) |
| Clave incorrecta | Se le reenvia la clave correcta y se registra en log |
| Otro jugador responde | Se le dice que no hablan con el y se registra en log |

### 3.4 Ciclo de Reseteo

Periodicamente, el sistema **resetea los flags de CentinelaOK** de todos los jugadores.
Esto fuerza nuevas verificaciones incluso para jugadores que ya pasaron el desafio anteriormente.
Nadie esta exento de verificacion continua.

### 3.5 Registro

Todas las interacciones con el centinela se registran en log:
- Creacion del desafio
- Respuestas correctas e incorrectas
- Timeouts y bans automaticos
- Desconexiones durante el desafio
- Intentos de otros jugadores de responder por otro

---

## 4. Seguridad de IP

### 4.1 Intervalo entre Conexiones

Cada IP debe esperar al menos **1,000 milisegundos** entre conexiones consecutivas.

**Implementacion funcional:**
- Se mantiene una tabla ordenada de IPs con su timestamp de ultima conexion
- Al recibir una nueva conexion, se busca la IP (busqueda binaria para rendimiento)
- Si el tiempo transcurrido es menor a 1,000ms, se rechaza la conexion
- Las tablas se limpian periodicamente (cada hora) para liberar memoria

### 4.2 Limite de Conexiones Simultaneas por IP

Maximo **10 conexiones simultaneas** por IP.

**Implementacion funcional:**
- Se mantiene un contador por IP que se incrementa al conectar y se decrementa al desconectar
- Si una IP supera el limite, se rechaza la nueva conexion
- Protege contra ataques de flooding y abuso de multi-cuentas

### 4.3 Lista de IPs Baneadas

Las IPs baneadas se almacenan en un archivo que se carga al iniciar. Se puede modificar en
runtime via comandos GM.

Al recibir una conexion, se verifica la IP contra la lista. Si esta baneada, se rechaza
inmediatamente.

---

## 5. Validacion de Cliente

### 5.1 Version del Cliente

Al conectarse, el cliente envia su numero de version. El servidor compara con `ULTIMAVERSION`.
Si no coincide, se rechaza con un mensaje indicando la version correcta.

### 5.2 Hash MD5

Opcionalmente, el servidor puede validar el hash MD5 del ejecutable del cliente. Se pueden
cargar multiples hashes aceptados para soportar variantes legitimas del cliente.

Si la validacion MD5 esta habilitada y el hash no coincide, se rechaza la conexion.

---

## 6. Foro In-Game

### 6.1 Estructura

El sistema de foro es basado en archivos. Existen multiples foros identificados por un ID:

| Foro | Visibilidad |
|------|-------------|
| Foro General | Todos los jugadores |
| Foro Real | Solo miembros del Ejercito Real (y GMs) |
| Foro Caos | Solo miembros de la Legion Oscura (y GMs) |

### 6.2 Limites

| Parametro | Valor |
|-----------|-------|
| Mensajes generales por foro | Maximo 30 |
| Anuncios (sticky) por foro | Maximo 5 |

Los mensajes nuevos se insertan al principio (los mas recientes primero). Cuando se supera
el limite, el mensaje mas antiguo se pierde permanentemente (no hay paginacion ni archivo).

### 6.3 Contenido de un Post

Cada post contiene:
- **Titulo**: encabezado del mensaje
- **Autor**: nombre del jugador que lo publico
- **Contenido**: texto del mensaje

### 6.4 Tipos de Post

| Tipo | Descripcion |
|------|-------------|
| General | Post normal, visible para todos |
| General Sticky | Anuncio fijado, visible para todos |
| Real | Post solo visible para faccion Real |
| Real Sticky | Anuncio fijado, solo faccion Real |
| Caos | Post solo visible para faccion Caos |
| Caos Sticky | Anuncio fijado, solo faccion Caos |

### 6.5 Persistencia

Los foros se guardan a disco durante el **WorldSave**. Cada post se almacena en un archivo
individual con nombre basado en el ID del foro y el numero de post.

---

## 7. Consultas Populares (Encuestas)

### 7.1 Concepto

Sistema de votacion in-game para decisiones de la comunidad. Permite a los administradores
crear encuestas y a los jugadores votar.

### 7.2 Configuracion

La encuesta se define en un archivo de datos con:
- Numero de encuesta actual
- Texto de la pregunta
- Nivel minimo requerido para votar
- Opciones disponibles (texto por opcion)

### 7.3 Reglas de Votacion

| Regla | Detalle |
|-------|---------|
| Encuestas activas | Solo **una** a la vez |
| Nivel minimo | El jugador debe tener el nivel requerido |
| Votos por personaje | **Uno solo**. Se registra en el archivo del personaje |
| Votos por email | **Uno solo** por email (anti-multicuenta) |
| Cambio de voto | **No permitido** |

### 7.4 Proteccion Anti-Multicuenta

El sistema lleva dos registros para evitar votos multiples:
- **Por personaje**: se marca en el archivo del personaje que ya voto
- **Por email**: se registra el email en un archivo central de votantes

Esto dificulta (pero no elimina completamente) la manipulacion por multi-cuentas.

### 7.5 Resultados

Los jugadores pueden consultar los resultados parciales en cualquier momento. Los resultados
muestran cuantos votos tiene cada opcion.

Los resultados finales se almacenan persistentemente en el archivo de configuracion de encuestas.

---

## 8. Sistema de Penas (Carcel)

### 8.1 Mecanica

La carcel es un mecanismo de castigo donde el jugador es confinado a un mapa especifico
durante un tiempo determinado.

**Posiciones:**
- Celda de prision: Mapa 66, posicion (75, 47)
- Salida de prision: Mapa 66, posicion (75, 65)

### 8.2 Fuentes de Encarcelamiento

| Fuente | Duracion |
|--------|----------|
| Comando GM (/CARCEL) | N minutos (definido por el GM) |
| Anti-piquete automatico | 10 minutos |
| Centinela (ban por macro) | Ban permanente (no carcel temporal) |

### 8.3 Purga de Penas

Las penas se purgan automaticamente:
- Cada minuto (en el tick de guardado automatico), se decrementa el contador de pena
- Cuando llega a 0, el jugador es teletransportado a la salida de la prision
- El jugador puede jugar normalmente despues de salir

---

## 9. Sistema de Desconexion Voluntaria

### 9.1 Mecanica

Cuando un jugador escribe `/salir`:
1. Se inicia un **countdown de 10 segundos**
2. Cada segundo, el countdown se decrementa
3. Si llega a 0, el jugador es desconectado limpiamente
4. Si el jugador se mueve, ataca, o realiza cualquier accion, el countdown se cancela

### 9.2 Proposito

El delay de 10 segundos evita que los jugadores se desconecten instantaneamente para
esquivar un combate PvP. Deben sobrevivir 10 segundos quietos para desconectarse.

---

## 10. WorldSave

### 10.1 Que se Guarda

El WorldSave es el proceso periodico de guardado completo:

1. **Todos los personajes conectados**: se guardan en sus archivos individuales
2. **Foros in-game**: se guardan todos los posts a disco
3. **Mapas con backup**: se guarda el estado de tiles (puertas, objetos, NPCs) para mapas marcados
4. **NPCs con backup**: se guardan NPCs marcados para persistencia
5. **Respawn de guardias**: se recrean guardias de ciudad
6. **Limpieza del mundo**: se eliminan objetos temporales del piso (cada 15 minutos)

### 10.2 Frecuencia

- **Automatico**: cada N minutos (configurable, minimo 60, default 180)
- **Manual**: por comando GM en cualquier momento
- **Al apagar**: se ejecuta un WorldSave final antes de cerrar

### 10.3 Aviso Previo

El servidor envia un **aviso a todos los jugadores** 1 minuto antes de ejecutar el WorldSave
automatico, para que sepan que puede haber un breve lag.

---

## 11. Verificacion de Inactividad

### 11.1 Mecanica

Cada jugador tiene un contador de inactividad que se incrementa con el tiempo. Si el jugador
no realiza ninguna accion durante el limite configurado (en minutos), es desconectado
automaticamente.

### 11.2 Configuracion

El limite de inactividad (`IdleLimit`) se define en el archivo de configuracion del servidor.
Se verifica cada minuto en el tick de guardado automatico.

---

## 12. Logging

### 12.1 Que se Registra

El servidor registra en logs:
- Conexiones y desconexiones
- Bans y unbans
- Encarcelamientos
- Interacciones con el centinela
- Transacciones de oro grandes (> 50,000)
- Transacciones de items con flag de log
- Drops de items especiales
- Comandos GM ejecutados
- Errores del servidor

### 12.2 Logs de Kill

Existe un sistema de log de PvP que registra los asesinatos entre jugadores. Estos logs
se limpian periodicamente (cada 1 minuto en el tick de limpieza de logs).

---

## 13. Modo de Servidor

### 13.1 Configuraciones Especiales

| Modo | Efecto |
|------|--------|
| ServerSoloGMs | Solo los GMs pueden conectarse (mantenimiento) |
| PuedeCrearPersonajes | Si se permite crear personajes nuevos |
| AllowMultiLogins | Si se permiten multiples conexiones desde la misma IP |
| EnTesting | Modo de pruebas (puede activar comportamientos especiales) |

---

## 14. Relacion con Otros Sistemas

### 14.1 Protocolo
- Los comandos GM son paquetes del protocolo con sus propios tipos
- La validacion de version y MD5 es parte del flujo de conexion
- El centinela usa paquetes especificos para desafios y respuestas

### 14.2 Personaje
- Los bans se marcan en el archivo del personaje
- Los privilegios se asignan al loguear
- La carcel afecta la posicion del personaje

### 14.3 Game Loop
- El centinela opera en el tick de auditoria (cada 1s)
- La purga de penas opera en el tick de guardado (cada 1min)
- El WorldSave opera en el tick de guardado (configurable)
- La verificacion de inactividad opera en el tick de guardado

### 14.4 Mundo
- Los GMs pueden modificar tiles, spawnear NPCs, y crear items
- El mapa 49 es exclusivo para GMs
- El mapa 66 es la prision
