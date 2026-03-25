# Etapa 7: Option Strict On — Plan de Accionables

## Contexto

Si se activa `Option Strict On` en todo el proyecto, esto genera **5468 errores** de compilación (4 tipos: BC30512, BC30038, BC32013, BC32029). El objetivo es corregirlos todos sin romper la serialización binaria (archivos + protocolo TCP/IP).

**Decisiones tomadas**:
- Usar `Convert.ToXxx()` para casts (consistente con Etapa 3, no usar CInt/CShort/CByte)
- Campos Byte semánticamente booleanos (Muerto, Escondido, etc.) se mantienen como Byte → usar `<> 0`
- Los enums cambian su tipo base para coincidir con su uso en serialización

---

## Accionable 1: Cambiar tipo base de Enums (~1334 errores)

Cambiar la definición del enum para que coincida con el tipo usado en serialización. NO cambia valores numéricos, NO afecta bytes escritos.

| Enum | Archivo | Cambiar a | Entradas |
|------|---------|-----------|----------|
| `ServerPacketID` | Protocol.vb:37 | `As Byte` | ~105 |
| `ClientPacketID` | Protocol.vb:145 | `As Byte` | ~129 |
| `eGMCommands` | Declares.vb:1519 | `As Byte` | ~130 |
| `eEditOptions` | Protocol.vb:305 | `As Byte` | ~15 |
| `eHeading` | Declares.vb:327 | `As Byte` | 4 |
| `eRaza` | Declares.vb:82 | `As Byte` | 5 |
| `eGenero` | Declares.vb:90 | `As Byte` | 2 |
| `eForumMsgType` | Declares.vb:828 | `As Byte` | 6 |
| `eForumType` | Declares.vb:845 | `As Byte` | 3 |
| `eMessages` | Declares.vb:1492 | `As Short` | 24 |
| `eSkill` | Declares.vb:362 | `As Short` | 20 |
| `FXIDs` | Declares.vb:131 | `As Short` | 6 |

**NO cambiar**: `PlayerType` — es flags enum bitwise, queda como Integer.

### Ejemplo

```vb
' ANTES (Protocol.vb:37):
Private Enum ServerPacketID
    Logged
    RemoveDialogs
    ...
End Enum

' DESPUÉS:
Private Enum ServerPacketID As Byte
    Logged
    RemoveDialogs
    ...
End Enum
```

**Errores residuales esperados**: Donde se necesite conversión inversa (ej: `Byte → eHeading` al leer del protocolo), se agrega `CType(readByte, eHeading)`. Y donde eMessages se pase a WriteByte, se agrega `Convert.ToByte()`.

---

## Accionable 2: Reemplazar IIf() por If() (~57 llamadas → ~100+ errores)

`IIf()` retorna `Object`. El operador `If()` de VB.NET es tipado y short-circuit.

### Ejemplos

```vb
' ANTES (ModAreas.vb:74):
CurDay = IIf(WeekDay(Today) > 6, 1, 2)

' DESPUÉS:
CurDay = If(WeekDay(Today) > 6, Convert.ToByte(1), Convert.ToByte(2))
```

```vb
' ANTES (GameLogic.vb:1049):
FindDirection = IIf(RandomNumber(0, 1), eHeading.NORTH, eHeading.EAST)

' DESPUÉS:
FindDirection = If(RandomNumber(0, 1) <> 0, eHeading.NORTH, eHeading.EAST)
```

```vb
' ANTES (clsClan.vb:115):
CantidadEnemys = CantidadEnemys + IIf(p_Relaciones(i) = RELACIONES_GUILD.GUERRA, 1, 0)

' DESPUÉS:
CantidadEnemys = Convert.ToInt16(CantidadEnemys + If(p_Relaciones(i) = RELACIONES_GUILD.GUERRA, 1, 0))
```

```vb
' ANTES (Trabajo.vb:2155):
MiObj.ObjIndex = IIf(DarMaderaElfica, LeñaElfica, Leña)

' DESPUÉS:
MiObj.ObjIndex = If(DarMaderaElfica, LeñaElfica, Leña)
```

**Archivos**: Modulo_UsUaRiOs.vb (11), Protocol.vb (9), Trabajo.vb (7), modGuilds.vb (6), clsClan.vb (6), ModAreas.vb (4), GameLogic.vb (4), FileIO.vb (3), TCP.vb (2), AI_NPC.vb (2), otros (6)

---

## Accionable 3: PlayerType bitwise → comparación explícita (~352 errores)

El operador `And` bitwise retorna Integer, no Boolean. Con Strict On hay que comparar explícitamente.

### Ejemplos

```vb
' ANTES (InvUsuario.vb:1476):
If .flags.Privilegios And PlayerType.User Then

' DESPUÉS:
If (.flags.Privilegios And PlayerType.User) <> 0 Then
```

```vb
' ANTES (Protocol.vb:2079):
If Not .flags.Privilegios And PlayerType.RoleMaster Then

' DESPUÉS (Not bitwise invierte todos los bits, equivale a verificar que NO tiene el flag):
If (.flags.Privilegios And PlayerType.RoleMaster) = 0 Then
```

```vb
' ANTES (Protocol.vb:5156):
If .flags.Privilegios And (PlayerType.User Or PlayerType.Consejero) Then Count = Count + 1

' DESPUÉS:
If (.flags.Privilegios And (PlayerType.User Or PlayerType.Consejero)) <> 0 Then Count = Convert.ToInt16(Count + 1)
```

**Archivos principales**: Protocol.vb (~298 usos), modSendData.vb, TCP.vb, GameLogic.vb, modHechizos.vb, Admin.vb, InvUsuario.vb, SistemaCombate.vb

---

## Accionable 4: Truthiness numérica → comparación `<> 0` (~338 errores)

Campos Short/Byte/Integer usados como condición booleana implícita.

### Ejemplos

```vb
' ANTES (clsIniReader.vb:110):
If MainNodes Then        ' MainNodes es Short

' DESPUÉS:
If MainNodes <> 0 Then
```

```vb
' ANTES (Modulo_InventANDobj.vb:243):
If iCant Then            ' iCant es Short

' DESPUÉS:
If iCant <> 0 Then
```

```vb
' ANTES (MODULO_NPCs.vb:834):
If FX Then               ' FX es Short

' DESPUÉS:
If FX <> 0 Then
```

**Incluye campos semánticamente booleanos** (Muerto, Escondido, Paralizado, etc.) que se mantienen como Byte por serialización.

---

## Accionable 5: Boolean → Numérico (~84 errores)

Expresiones booleanas asignadas a variables numéricas, o booleanos en contexto aritmético.

### Ejemplos

```vb
' ANTES (SistemaCombate.vb:176):
UserImpactoNpc = (RandomNumber(1, 100) <= ProbExito)    ' Boolean → Short

' DESPUÉS:
UserImpactoNpc = If(RandomNumber(1, 100) <= ProbExito, 1S, 0S)
```

```vb
' ANTES (SistemaCombate.vb:214):
NpcImpacto = (RandomNumber(1, 100) <= ProbExito)        ' Boolean → Short

' DESPUÉS:
NpcImpacto = If(RandomNumber(1, 100) <= ProbExito, 1S, 0S)
```

**Nota**: En VB6 `True = -1`, en .NET `Convert.ToInt32(True) = 1`. Verificar caso por caso si el código depende del valor `-1`. Si es solo usado como flag 0/1, usar `If(boolExpr, 1S, 0S)`.

---

## Accionable 6: Casts explícitos en aritmética (~2546 errores)

VB.NET promueve Short+1 → Integer, división → Double. Usar `Convert.ToXxx()` consistente con Etapa 3.

### Ejemplos — Integer → Short (arithmetic promotion)

```vb
' ANTES (Modulo_InventANDobj.vb:75):
NroDrop = NroDrop + 1                        ' Short + Integer literal = Integer

' DESPUÉS:
NroDrop = Convert.ToInt16(NroDrop + 1)
```

```vb
' ANTES (MODULO_NPCs.vb:532):
Iteraciones = Iteraciones + 1

' DESPUÉS:
Iteraciones = Convert.ToInt16(Iteraciones + 1)
```

```vb
' ANTES (GameLoop.vb:251):
centinelSecs = centinelSecs + 1

' DESPUÉS:
centinelSecs = Convert.ToInt16(centinelSecs + 1)
```

### Ejemplos — RandomNumber() retorna Integer, asignado a Short

```vb
' ANTES (Modulo_InventANDobj.vb:68):
Random = RandomNumber(1, 100)                 ' Integer → Short

' DESPUÉS:
Random = Convert.ToInt16(RandomNumber(1, 100))
```

```vb
' ANTES (MODULO_NPCs.vb:500):
Pos.X = RandomNumber(MinXBorder, MaxXBorder)

' DESPUÉS:
Pos.X = Convert.ToInt16(RandomNumber(MinXBorder, MaxXBorder))
```

### Ejemplos — ParseVal() retorna Double, asignado a Short/Byte

```vb
' ANTES (MODULO_NPCs.vb:929):
.flags.AguaValida = ParseVal(Leer.GetValue("NPC" & NpcNumber, "AguaValida"))

' DESPUÉS:
.flags.AguaValida = Convert.ToByte(ParseVal(Leer.GetValue("NPC" & NpcNumber, "AguaValida")))
```

```vb
' ANTES (MODULO_NPCs.vb:951):
.flags.Domable = ParseVal(Leer.GetValue("NPC" & NpcNumber, "Domable"))

' DESPUÉS:
.flags.Domable = Convert.ToInt16(ParseVal(Leer.GetValue("NPC" & NpcNumber, "Domable")))
```

### Tabla resumen de conversiones

| Conversión | Cantidad | Causa | Fix .NET moderno |
|-----------|----------|-------|-----------------|
| Integer → Short | 1328 | `short + 1`, `RandomNumber()` | `Convert.ToInt16(expr)` |
| Double → Short | 582 | `ParseVal()`, división `/` | `Convert.ToInt16(expr)` |
| Short → Byte | 526 | Short asignado a campo Byte | `Convert.ToByte(expr)` |
| Double → Integer | 192 | División, ParseVal | `Convert.ToInt32(expr)` |
| Integer → Byte | 180 | Aritmética Integer → Byte | `Convert.ToByte(expr)` |
| Double → Byte | 168 | ParseVal, Math | `Convert.ToByte(expr)` |
| Long → Integer | 48 | `2^n` produce Double→Long | `Convert.ToInt32(expr)` |
| Double → Single | 38 | Literales Double → Single | `Convert.ToSingle(expr)` |
| Single → Byte | 10 | Float → byte | `Convert.ToByte(expr)` |

---

## Accionable 7: `.ToString()` en concatenaciones string (~30 errores)

### Ejemplos

```vb
' ANTES (Protocol.vb:3026):
") ip: " & .ip & " a la posición (" & .Pos.Map & "/" & X & "/" & Y & ")"

' DESPUÉS:
") ip: " & .ip & " a la posición (" & .Pos.Map.ToString() & "/" & X.ToString() & "/" & Y.ToString() & ")"
```

```vb
' ANTES (Trabajo.vb:997):
"¡Has obtenido el " & num & "% de los lingotes..."

' DESPUÉS:
"¡Has obtenido el " & num.ToString() & "% de los lingotes..."
```

---

## Accionable 8: Residuales (~4 errores)

### BC32029 — ByRef narrowing (TCP.vb:536)

```vb
' ANTES:
Call Winsock_Close(UserList(UserIndex).ConnID)    ' ConnID es Short, param es ByRef Integer

' DESPUÉS (wskapiAO.vb - cambiar ByRef a ByVal):
Public Sub Winsock_Close(ByVal socketID As Integer)
' La función no modifica socketID, ByVal acepta widening sin problemas
```

### BC32013 — Object `<>` operator (ModAreas.vb:100)

Se corrige automáticamente al reemplazar `IIf()` por `If()` en Accionable 2.

---

## Errores por Archivo (referencia)

| Archivo | Errores | Archivo | Errores |
|---------|---------|---------|---------|
| Protocol.vb | 1570 | modHechizos.vb | 182 |
| FileIO.vb | 720 | MODULO_NPCs.vb | 160 |
| praetorians.vb | 446 | modSendData.vb | 144 |
| SistemaCombate.vb | 258 | GameLoop.vb | 134 |
| Trabajo.vb | 238 | ModAreas.vb | 88 |
| InvUsuario.vb | 228 | clsClan.vb | 88 |
| Modulo_UsUaRiOs.vb | 218 | AI_NPC.vb | 86 |
| TCP.vb | 184 | GameLogic.vb | 82 |
| General.vb | 80 | modGuilds.vb | 66 |
| modForum.vb | 50 | Acciones.vb | 46 |
| modCentinela.vb | 42 | clsIniReader.vb | 36 |
| Admin.vb | 32 | clsMapSoundManager.vb | 30 |
| Modulo_InventANDobj.vb | 26 | ModFacciones.vb | 26 |
| PathFinding.vb | 24 | clsParty.vb | 24 |
| modNuevoTimer.vb | 22 | wskapiAO.vb | 20 |
| CargaMapa.vb | 20 | Comercio.vb | 18 |
| Statistics.vb | 14 | ModCola.vb | 14 |
| modBanco.vb | 14 | ConsultasPopulares.vb | 12 |
| Queue.vb | 8 | SecurityIp.vb | 6 |
| modHexaStrings.vb | 4 | mdlCOmercioConUsuario.vb | 4 |
| Matematicas.vb | 4 | | |

---

## Plan de Ejecución — Estrategia Archivo-por-Archivo

### Principio rector

En lugar de aplicar un accionable a TODOS los archivos (tocando cada archivo 8 veces),
se procesa **cada archivo por completo** antes de pasar al siguiente. Esto permite:
- Contexto más pequeño por prompt (mejor performance del agente AI)
- Build frecuente con feedback inmediato
- Rollback fácil si algo rompe (sabés qué archivo lo causó)

### Reglas de agrupamiento

| Criterio | Estrategia |
|----------|-----------|
| **≤20 errores** | Agrupar 3-6 archivos por prompt |
| **21-50 errores** | Agrupar 2-3 archivos por prompt |
| **51-100 errores** | 1 archivo por prompt |
| **101-250 errores** | 1 archivo por prompt, posible subdivisión interna |
| **250+ errores** | 1 archivo, aplicar accionables de menor a mayor impacto |

### Cómo aplicar accionables dentro de un archivo

Para cada archivo (o grupo), aplicar los accionables **en este orden**:
1. **Accionable 1** (si el archivo tiene enums) — Cambiar tipo base
2. **Accionable 2** — IIf → If
3. **Accionable 3** — PlayerType bitwise → <> 0
4. **Accionable 4** — Truthiness numérica → <> 0
5. **Accionable 5** — Boolean → Numérico
6. **Accionable 6** — Casts explícitos (Convert.ToXxx)
7. **Accionable 7** — .ToString() en concatenaciones
8. **Accionable 8** — Residuales (solo aplica a TCP.vb, wskapiAO.vb)

Este orden minimiza los errores intermedios: los accionables 1-5 resuelven errores de tipo que
después facilitan la corrección masiva del Accionable 6.

### Después de cada lote: Build + Smoke test

```bash
dotnet build 2>&1 | tail -5    # verificar que el error count bajó
dotnet run --project Server     # arrancar servidor, verificar que no crashea
# Si falla → revisar el último lote, hacer rollback git parcial
```

---

## Ejecución Paso a Paso

### FASE 0: Preparación

```bash
git checkout -b stage7-option-strict
dotnet build 2>&1 | grep -c "error BC"   # baseline: 5468 errores
```

### FASE 1: Enums — Cambio de tipo base (Accionable 1)

**Paso único, 2 archivos, ~1334 errores eliminados de golpe.**

Esto es lo primero que se hace. Son cambios triviales en las definiciones de enums
(agregar `As Byte` o `As Short`) + casts residuales donde se lean/escriban esos enums.
No afecta serialización, no cambia valores numéricos.

| Archivo | Cambios | Detalle |
|---------|---------|---------|
| Declares.vb | 9 enums | `eGMCommands`, `eHeading`, `eRaza`, `eGenero`, `eForumMsgType`, `eForumType`, `eMessages`, `eSkill`, `FXIDs` |
| Protocol.vb | 3 enums | `ServerPacketID`, `ClientPacketID`, `eEditOptions` |

**Después de cambiar las definiciones**, buscar y corregir los casts residuales:
- Donde se lee un Byte del protocolo y se asigna a un enum → `CType(readByte, eHeading)`
- Donde se pasa un enum a WriteByte → `Convert.ToByte(eMessages.X)`
- Donde se compara un enum con un tipo diferente → cast explícito

> **Importante**: Hacer esto en **un solo prompt**. Son cambios mecánicos en 2 archivos.
> Después de este paso quedan ~4134 errores (de 5468 originales).

```bash
dotnet build 2>&1 | grep -c "error BC"   # debería bajar a ~4134
dotnet run --project Server               # smoke test
```

---

### FASE 2: Archivos TINY (≤14 errores) — 12 archivos, ~62 errores total

**Objetivo**: Eliminar errores rápidamente, ganar confianza en el proceso.

#### Paso 2.1 — Grupo TINY-A (5 archivos, ~22 errores)
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| Matematicas.vb | 4 | 6 (casts) |
| modHexaStrings.vb | 4 | 6 (casts) |
| mdlCOmercioConUsuario.vb | 4 | 6 (casts) |
| Queue.vb | 8 | 6 (casts) |
| SecurityIp.vb | 6 | 6 (casts) |

```bash
# Prompt al agente: aplicar Accionable 6 (y los que apliquen) a estos 5 archivos
# Después:
dotnet build && dotnet run --project Server
```

#### Paso 2.2 — Grupo TINY-B (4 archivos, ~40 errores)
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| ConsultasPopulares.vb | 12 | 6 |
| modBanco.vb | 14 | 6 |
| ModCola.vb | 14 | 6 |
| Statistics.vb | 14 | 6 |

```bash
dotnet build && dotnet run --project Server
```

---

### FASE 3: Archivos SMALL (18-32 errores) — 11 archivos, ~258 errores total

> **Nota**: Los errores de estos archivos son post-enum. Ya no incluyen errores por tipo base de enum.

#### Paso 3.1 — Grupo SMALL-A (3 archivos, ~56 errores)
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| Comercio.vb | 18 | 4, 6 |
| CargaMapa.vb | 20 | 6 |
| wskapiAO.vb | 20 | 8 (ByRef fix) |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 3.2 — Grupo SMALL-B (3 archivos, ~70 errores)
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| modNuevoTimer.vb | 22 | 6 |
| clsParty.vb | 24 | 4, 6 |
| PathFinding.vb | 24 | 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 3.3 — Grupo SMALL-C (3 archivos, ~84 errores)
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| Modulo_InventANDobj.vb | 26 | 4, 6 |
| ModFacciones.vb | 26 | 3, 6 |
| clsMapSoundManager.vb | 30 | 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 3.4 — Grupo SMALL-D (2 archivos, ~78 errores)
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| Admin.vb | 32 | 3, 6 |
| clsIniReader.vb | 36 | 4, 6 |

```bash
dotnet build && dotnet run --project Server
```

---

### FASE 4: Archivos SMALL-MEDIUM (42-66 errores) — 5 archivos, ~286 errores total

#### Paso 4.1 — Grupo SMED-A (2 archivos, ~88 errores)
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| modCentinela.vb | 42 | 6 |
| Acciones.vb | 46 | 4, 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 4.2 — Archivo individual
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| modForum.vb | 50 | 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 4.3 — Archivo individual
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| modGuilds.vb | 66 | 2 (IIf), 6 |

```bash
dotnet build && dotnet run --project Server
```

---

### FASE 5: Archivos MEDIUM (80-88 errores) — 4 archivos, ~336 errores total

#### Paso 5.1 — Archivo individual
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| General.vb | 80 | 4, 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 5.2 — Archivo individual
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| GameLogic.vb | 82 | 2 (IIf), 3 (PlayerType), 4, 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 5.3 — Archivo individual
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| AI_NPC.vb | 86 | 2 (IIf), 4, 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 5.4 — Grupo MEDIUM-A (2 archivos, ~176 errores)
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| ModAreas.vb | 88 | 2 (IIf), 4, 6 |
| clsClan.vb | 88 | 2 (IIf), 6 |

**Nota**: Estos dos tienen ~88 errores cada uno pero muchos son similares (IIf, casts simples). Se agrupan porque sus patrones de error son repetitivos y fáciles de aplicar en batch.

```bash
dotnet build && dotnet run --project Server
```

---

### FASE 6: Archivos MEDIUM-LARGE (134-184 errores) — 5 archivos, ~804 errores total

Cada archivo individual, un prompt por archivo.

#### Paso 6.1
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| GameLoop.vb | 134 | 3, 4, 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 6.2
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| modSendData.vb | 144 | 3 (PlayerType), 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 6.3
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| MODULO_NPCs.vb | 160 | 4, 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 6.4
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| modHechizos.vb | 182 | 3, 4, 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 6.5
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| TCP.vb | 184 | 2 (IIf), 3, 6, 8 (ByRef) |

```bash
dotnet build && dotnet run --project Server
```

---

### FASE 7: Archivos LARGE (218-258 errores) — 3 archivos, ~684 errores total

Cada archivo individual. Aplicar accionables en orden, verificando que el archivo compila después de cada accionable principal.

#### Paso 7.1
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| Modulo_UsUaRiOs.vb | 218 | 2 (IIf ×11), 3, 4, 6 |

Estrategia: Primero reemplazar los 11 IIf (rápido), luego PlayerType, luego casts masivos.

```bash
dotnet build && dotnet run --project Server
```

#### Paso 7.2
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| InvUsuario.vb | 228 | 3 (PlayerType masivo), 4, 6 |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 7.3
| Archivo | Errores | Accionables aplicables |
|---------|---------|----------------------|
| Trabajo.vb | 238 | 2 (IIf ×7), 5, 6, 7 |

```bash
dotnet build && dotnet run --project Server
```

---

### FASE 8: Archivos VERY LARGE (446-720 errores) — 2 archivos, ~1166 errores total

#### Paso 8.1 — praetorians.vb (446 errores)
| Accionable | Errores aprox | Estrategia |
|-----------|--------------|-----------|
| 6 (Casts) | ~430 | Predominantemente Integer→Short y Short→Byte |
| 4 (Truthiness) | ~16 | Numeric checks |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 8.2 — FileIO.vb (720 errores)
| Accionable | Errores aprox | Estrategia |
|-----------|--------------|-----------|
| 2 (IIf) | ~3 | Pocos, aplicar primero |
| 4 (Truthiness) | ~30 | Numeric checks |
| 6 (Casts) | ~687 | Masivo: ParseVal→Short/Byte, Integer→Short |

**Subdivisión sugerida**: Si el agente tiene problemas con 720 errores en un prompt, dividir FileIO.vb en dos mitades (primera mitad ~línea 1-1500, segunda mitad ~1500+).

```bash
dotnet build && dotnet run --project Server
```

---

### FASE 9: Archivos MASSIVE — SistemaCombate.vb + Protocol.vb

#### Paso 9.1 — SistemaCombate.vb (258 errores)
| Accionable | Errores aprox | Estrategia |
|-----------|--------------|-----------|
| 5 (Bool→Num) | ~20 | Aplicar primero (patrones repetitivos de RandomNumber ≤ ProbExito) |
| 3 (PlayerType) | ~10 | Few occurrences, straightforward |
| 6 (Casts) | ~228 | Masivo: `Convert.ToInt16()`, `Convert.ToByte()` |

```bash
dotnet build && dotnet run --project Server
```

#### Paso 9.2-9.6 — Protocol.vb (~1570 errores restantes, post-enum)

**Este archivo se trabaja SOLO, con subdivisión obligatoria.**

Protocol.vb tiene ~6000+ líneas. Los enums ya se corrigieron en Fase 1.
Quedan ~1570 errores distribuidos en los accionables 2-8.
Se divide en **operaciones por accionable** (no por sección del archivo), ya que cada accionable
tiene patrones repetitivos que el agente puede aplicar con regex/search-replace.

##### Paso 9.2 — IIf → If (Acc. 2)
~9 llamadas IIf() en Protocol.vb.

```bash
dotnet build && dotnet run --project Server
```

##### Paso 9.3 — PlayerType bitwise (Acc. 3)
~298 usos de PlayerType bitwise. Patrón repetitivo:
```vb
If .flags.Privilegios And PlayerType.X → If (.flags.Privilegios And PlayerType.X) <> 0
```

```bash
dotnet build && dotnet run --project Server
```

##### Paso 9.4 — Truthiness (Acc. 4) + Bool→Num (Acc. 5)
Checks numéricos y boolean→numeric. ~50 errores combinados.

```bash
dotnet build && dotnet run --project Server
```

##### Paso 9.5 — Casts masivos (Acc. 6)
~1100+ errores de casts. **Subdividir en 2-3 sub-batches** si es necesario:
- Sub-batch A: WritePacket handlers (envío)
- Sub-batch B: ReadPacket handlers (recepción)
- Sub-batch C: Lógica general del archivo

```bash
dotnet build && dotnet run --project Server
```

##### Paso 9.6 — ToString (Acc. 7) + Residuales (Acc. 8)
~15 errores de .ToString() en concatenaciones de log/debug.

```bash
dotnet build && dotnet run --project Server
```

---

### FASE 10: Verificación Final

```bash
dotnet build                          # debe dar 0 errores
dotnet run --project Server           # servidor arranca sin crashes

# Smoke test manual:
# 1. Conectar un cliente
# 2. Loguear un personaje
# 3. Moverse por el mapa
# 4. Castear un hechizo
# 5. Atacar un NPC
# 6. Comerciar
# 7. Guardar y salir
```

**Checks de serialización**:
- Los valores de enum NO cambian (solo su tipo subyacente)
- Los campos Byte semánticamente booleanos siguen siendo Byte
- Los protocolos de lectura/escritura escriben los mismos bytes

---

## Resumen de Ejecución

| Fase | Tipo | Archivos | Errores | Prompts | Build+Run |
|------|------|----------|---------|---------|-----------|
| 0 | Prep | — | — | 1 | 1 |
| **1** | **ENUMS** | **2** | **~1334** | **1** | **1** |
| 2 | TINY | 12 | ~62 | 2 | 2 |
| 3 | SMALL | 11 | ~258 | 4 | 4 |
| 4 | SMALL-MED | 5 | ~286 | 3 | 3 |
| 5 | MEDIUM | 4 | ~336 | 4 | 4 |
| 6 | MED-LARGE | 5 | ~804 | 5 | 5 |
| 7 | LARGE | 3 | ~684 | 3 | 3 |
| 8 | VERY LARGE | 2 | ~1166 | 2 | 2 |
| 9 | MASSIVE | 2 | ~1828 | 6 | 6 |
| 10 | Verificación | — | — | 1 | 1 |
| **TOTAL** | | **46** | **~5468** | **~32** | **32** |

> **Por qué enums primero**: Cambiar `Enum X` → `Enum X As Byte` en Declares.vb y Protocol.vb elimina
> ~1334 errores (24% del total) con solo ~15 líneas modificadas. Estos errores se propagan a TODOS los
> archivos que usan esos enums. Resolviéndolos primero, los errores restantes por archivo bajan
> significativamente (ej: Protocol.vb pasa de 1570 a ~1100 errores residuales).

---

## Protocolo de Seguridad

### Antes de cada build
```bash
git add -A && git commit -m "wip: stage7 phase X step Y"   # checkpoint
```

### Si un build falla
```bash
git diff --stat                          # ver qué cambió
git stash                                # guardar cambios
git stash pop                            # o revertir y reintentar
```

### Si el server crashea al arrancar
1. Revisar el último archivo modificado
2. Verificar que los enums no cambiaron sus valores numéricos
3. Verificar que la serialización binaria es byte-identica
