# Plan de Migración: VB.NET → C# (Argentum Online Server)

> **Objetivo**: Limpiar todo el código heredado de VB6 del proyecto VB.NET (.NET 10)
> para que `icsharpcode/CodeConverter` pueda convertirlo a C# de forma limpia y confiable.
>
> **Estrategia**: Cambios iterativos. Compilar y verificar después de cada etapa/batch.
> **Branch activo**: `feature/csharp-migration`

---

## Estado General

| Etapa | Descripción | Estado |
|-------|-------------|--------|
| ✅ 0 | Error handling: On Error → Try/Catch | **COMPLETADO** |
| ✅ 1 | Reemplazar `Val()` | **COMPLETADO** (441 → 0) |
| ✅ 2 | Reemplazar funciones string VB6 (`Left`, `Right`, `Mid`, `Len`, `InStr`, etc.) | **COMPLETADO** (0 instancias restantes) |
| ✅ 3 | Reemplazar conversiones de tipo (`CStr`, `CInt`, `CDbl`, `CBool`) | **COMPLETADO** (0 instancias restantes) |
| ✅ 4 | Reemplazar `UBound()` / `LBound()` | **COMPLETADO** (58 → 0) |
| ✅ 5 | Modernizar File I/O (lectura texto/binario + logging + escritura) | **COMPLETADO** |
| ✅ 6 | Resolver `As Object` (late binding) | **COMPLETADO** (25 → 5, eliminada clase diccionario) |
| 🔲 7 | Activar `Option Strict On` archivo por archivo | Pendiente |
| 🔲 8 | Expandir `With` statements manualmente | Pendiente |

**Leyenda**: ✅ Completado | 🔄 En progreso | 🔲 Pendiente | ⏸️ Bloqueado

---

## Etapa 1: Reemplazar `Val()` — 446 instancias

### Concepto clave
`Val()` es de VB6 y **retorna 0 si el string no es numérico** (no lanza excepción).
El reemplazo debe preservar ese comportamiento. Crear una función helper en un módulo utilitario:

```vb
' En un módulo compartido (ej: General.vb o un nuevo VBCompat.vb)
Public Function ParseVal(s As String) As Double
    Dim result As Double
    If Double.TryParse(s, System.Globalization.NumberStyles.Any,
                       System.Globalization.CultureInfo.InvariantCulture, result) Then
        Return result
    End If
    Return 0
End Function
```

Luego: `Val(x)` → `ParseVal(x)` en todos los archivos.

En los casos donde el tipo esperado es Integer o Long:
- `CInt(Val(x))` → `CInt(ParseVal(x))` (temporalmente, hasta la Etapa 3)

### Progreso por archivo

| Archivo | Usos aprox. | Estado |
|---------|-------------|--------|
| modHexaStrings.vb | ~0 | ✅ (usa Convert.ToInt64 para hex) |
| clsIniReader.vb | ~0 | ✅ (sin Val) |
| clsClan.vb | ~0 | ✅ (sin Val) |
| clsParty.vb | ~0 | ✅ (sin Val) |
| modBanco.vb | ~0 | ✅ (sin Val) |
| Statistics.vb | ~0 | ✅ (sin Val) |
| Comercio.vb | ~0 | ✅ (sin Val) |
| modForum.vb | ~10 | ✅ |
| Migration.vb | ~1 | ✅ |
| CargaMapa.vb | ~1 | ✅ (comentado) |
| GameLogic.vb | ~1 | ✅ |
| modCentinela.vb | ~1 | ✅ |
| ModFacciones.vb | ~1 | ✅ |
| modGuilds.vb | ~1 | ✅ |
| Modulo_UsUaRiOs.vb | ~1 | ✅ |
| TCP.vb | ~1 | ✅ |
| ModAreas.vb | ~3 | ✅ |
| Admin.vb | ~5 | ✅ |
| clsMapSoundManager.vb | ~5 | ✅ |
| ConsultasPopulares.vb | ~5 | ✅ |
| Modulo_InventANDobj.vb | ~5 | ✅ |
| Protocol.vb | ~31 | ✅ |
| MODULO_NPCs.vb | ~44 | ✅ |
| FileIO.vb | ~334 | ✅ |

---

## Etapa 2: Funciones string VB6 — ~137 instancias

### Tabla de reemplazos

| VB6 | .NET equivalente | Notas |
|-----|-----------------|-------|
| `Left(s, n)` | `s.Substring(0, Math.Min(n, s.Length))` | Usar guarda para evitar IndexOutOfRange |
| `Right(s, n)` | `s.Substring(s.Length - Math.Min(n, s.Length))` | Ídem |
| `Mid(s, start, len)` | `s.Substring(start - 1, len)` | ⚠️ VB es 1-based |
| `Mid(s, start)` | `s.Substring(start - 1)` | ⚠️ VB es 1-based |
| `Len(s)` | `s.Length` | Directo |
| `InStr(s1, s2)` | `s1.IndexOf(s2) + 1` | ⚠️ VB retorna 1-based, .NET 0-based |
| `InStr(start, s1, s2)` | `s1.IndexOf(s2, start - 1) + 1` | Ídem, ajustar start |
| `UCase(s)` | `s.ToUpper()` | Directo |
| `LCase(s)` | `s.ToLower()` | Directo |
| `Replace(s, old, new)` | `s.Replace(old, new)` | Directo |
| `Trim(s)` | `s.Trim()` | Directo |
| `LTrim(s)` | `s.TrimStart()` | Directo |
| `RTrim(s)` | `s.TrimEnd()` | Directo |
| `Space(n)` | `New String(" "c, n)` | Directo |
| `String(n, c)` | `New String(c, n)` | Directo |

### Progreso por archivo

**COMPLETADOS (19 archivos)**:
- Migration.vb ✅ (5 inst)
- clsdicc.vb ✅ (2 inst)
- Acciones.vb ✅ (1 inst)
- InvUsuario.vb ✅ (1 inst)
- ModCola.vb ✅ (6 inst)
- modHexaStrings.vb ✅ (5 inst)
- Statistics.vb ✅ (5 inst)
- clsIniReader.vb ✅ (12 inst)
- TCP.vb ✅ (17 inst)
- Admin.vb ✅ (7 inst)
- mdParty.vb ✅ (5 inst)
- GameLogic.vb ✅ (9 inst)
- clsClan.vb ✅ (18 inst)
- modGuilds.vb ✅ (28 inst)
- FileIO.vb ✅ (27 inst)
- Protocol.vb ✅ (104 inst)

- General.vb ✅ (5 inst, incluye null guard en ReadField)

---

## Etapa 3: Conversiones de tipo — ~306 instancias

### Tabla de reemplazos

| VB6 | .NET equivalente | Notas |
|-----|-----------------|-------|
| `CStr(x)` | `x.ToString()` | Directo |
| `CInt(x)` | `Convert.ToInt32(x)` o `Integer.Parse(x)` | Usar Convert cuando x puede ser string/object; casting `CType` si ya es numérico |
| `CDbl(x)` | `Convert.ToDouble(x)` o `Double.Parse(x)` | Ídem |
| `CBool(x)` | `Convert.ToBoolean(x)` | Ídem |
| `CLng(x)` | `Convert.ToInt64(x)` | — |
| `CSng(x)` | `Convert.ToSingle(x)` | — |
| `CByte(x)` | `Convert.ToByte(x)` | — |

### Progreso por archivo

**COMPLETADO** — todos los archivos migrados. 0 instancias restantes.

Incluyó también `CShort` → `Convert.ToInt16` en todos los archivos.

---

## Etapa 4: `UBound()` / `LBound()` — 58 instancias

### Reemplazos

```vb
' Antes:
For i = 0 To UBound(arr)
For i = LBound(arr) To UBound(arr)

' Después:
For i = 0 To arr.Length - 1
For i = 0 To arr.Length - 1   ' LBound siempre es 0 en .NET
```

Para arrays de tipo List(Of T), usar `.Count - 1` en vez de `.Length - 1`.

### Progreso por archivo

| Archivo | Usos | Estado |
|---------|------|--------|
| Protocol.vb | 22 | ✅ |
| modGuilds.vb | 8 | ✅ |
| Trabajo.vb | 4 | ✅ |
| FileIO.vb | 4 | ✅ |
| clsClan.vb | 4 | ✅ |
| InvUsuario.vb | 3 | ✅ |
| General.vb | 3 | ✅ |
| Comercio.vb | 3 | ✅ |
| clsMapSoundManager.vb | 2 | ✅ |
| ConsultasPopulares.vb | 2 | ✅ |
| Admin.vb | 2 | ✅ |
| Modulo_UsUaRiOs.vb | 1 | ✅ |

---

## Etapa 5: File I/O moderno — 48 bloques

### Reglas para GetVar / WriteVar

- **GetVar** (FileIO.vb:1202-1248): **NO cambiar la lógica de parseo INI**. Solo modernizar el I/O interno: `FreeFile`/`FileOpen`/`LineInput`/`EOF`/`FileClose` → `StreamReader` con `Using`. El algoritmo de escaneo línea por línea se preserva exactamente.
- **WriteVar** (FileIO.vb:1503-1621): **Ya usa I/O moderno** (`File.ReadAllText` + `File.WriteAllText`). Nada que tocar.
- Si `GetVar` y `WriteVar` son llamados desde otros archivos, la interfaz pública **no cambia** — los callers no necesitan modificar nada.

### Sub-etapas

#### 5A: Helper de logging + 34 bloques de Append

Crear `AppendLog(relativePath, message)` en un módulo compartido. Reemplaza los 34 bloques idénticos de `FreeFile`→`FileOpen(Append)`→`PrintLine`→`FileClose` por una sola línea.

```vb
' Helper:
Public Sub AppendLog(relativePath As String, message As String)
    Dim fullPath = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath)
    IO.File.AppendAllText(fullPath, message & Environment.NewLine)
End Sub

' Uso:
' ANTES (6 líneas por bloque):
Dim nfile As Short
nfile = FreeFile()
FileOpen(nfile, path, OpenMode.Append, , OpenShare.Shared)
PrintLine(nfile, Today & " " & TimeOfDay & " " & desc)
FileClose(nfile)

' DESPUÉS (1 línea):
AppendLog("logs/Eventos.log", Today & " " & TimeOfDay & " " & desc)
```

**Archivos afectados:**

| Archivo | Bloques | Funciones |
|---------|---------|-----------|
| General.vb | 18 | LogCriticEvent, LogEjercitoReal, LogEjercitoCaos, LogIndex, LogError, LogStatic, LogTarea, LogClanes, LogIP, LogDesarrollo, LogGM, LogAsesinato, logVentaCasa, LogHackAttemp, LogCheating, LogCriticalHackAttemp, LogAntiCheat, RecargarServidor |
| FileIO.vb | 4 | CargarBackUp, LogBanFromIndex, LogBanFromName, Ban |
| Protocol.vb | 2 | HandleBugReport, HandleShutdown |
| TCP.vb | 2 | ConnectUser, CloseUser |
| ConsultasPopulares.vb | 1 | MarcarMailComoQueYaVoto |
| modCentinela.vb | 1 | LogCentinela |
| GameLoop.vb | 2 | TickAutoSave, CloseServer |

**Nota**: `logVentaCasa`, `LogHackAttemp` y `LogCriticalHackAttemp` escriben 3 líneas (separador + mensaje + separador). Se adaptan fácilmente a 3 llamadas a `AppendLog`.

#### 5B: GetVar — modernizar I/O interno (sin cambiar lógica)

Reemplazar solo las llamadas a I/O legacy dentro de GetVar, sin cambiar el algoritmo de parseo INI:

```vb
' ANTES:
fileNumber = FreeFile()
FileOpen(fileNumber, filePath, OpenMode.Input)
While Not EOF(fileNumber)
    currentLine = LineInput(fileNumber)
    ' ... parseo ...
End While
FileClose(fileNumber)

' DESPUÉS:
Using reader As New IO.StreamReader(filePath)
    While Not reader.EndOfStream
        currentLine = reader.ReadLine()
        ' ... mismo parseo ...
    End While
End Using
```

Esto elimina `FreeFile`, `FileOpen`, `EOF`, `LineInput`, `FileClose`. Los 334+ callers no cambian.

#### 5C: clsIniReader.Leer — modernizar I/O interno

Mismo patrón que GetVar. El método `Initialize` (clsIniReader.vb:134-188) usa el mismo patrón legacy:

```vb
' ANTES:
handle = FreeFile
FileOpen(handle, file, OpenMode.Input)
Do Until EOF(handle)
    Text = LineInput(handle)
    ' ... parseo ...
Loop
FileClose(handle)

' DESPUÉS:
Using reader As New IO.StreamReader(file)
    Do Until reader.EndOfStream
        Text = reader.ReadLine()
        ' ... mismo parseo ...
    Loop
End Using
```

#### 5D: Lectura de texto — 6 bloques restantes

| # | Archivo | Función | Líneas | Reemplazo |
|---|---------|---------|--------|-----------|
| 1 | FileIO.vb | `TxtDimension` | 152-171 | `IO.File.ReadAllLines(path).Length` |
| 2 | FileIO.vb | `CargarForbidenWords` | 173-191 | `IO.File.ReadAllLines(path)` |
| 3 | Admin.vb | `BanIpCargar` | 408-433 | `IO.File.ReadAllLines(path)` + loop |
| 4 | ConsultasPopulares.vb | `MailYaVoto` | 135-155 | `IO.File.ReadLines(path)` + LINQ `.Any()` |
| 5 | modForum.vb | `CargarForo` (posts) | 64-77 | `IO.StreamReader` + 3× `ReadLine()` (reemplaza `Input()`) |
| 6 | modForum.vb | `CargarForo` (anuncios) | 80-94 | `IO.StreamReader` + 3× `ReadLine()` (reemplaza `Input()`) |

**Nota sobre `Input()` en modForum.vb**: `Input(fileNum, var)` de VB6 lee campos en formato VB (delimitados por `"`). Los archivos `.for` tienen 3 líneas simples (título, autor, post). Se reemplaza con 3 `ReadLine()` directos.

#### 5E: Escritura de texto — 6 bloques

| # | Archivo | Función | Líneas | Reemplazo |
|---|---------|---------|--------|-----------|
| 1 | Admin.vb | `BanIpSave` | 386-405 | `IO.File.WriteAllLines(path, list)` |
| 2 | Statistics.vb | `DumpStatistics` (frags) | 202-502 | `IO.StreamWriter` con `Using` |
| 3 | Statistics.vb | `DumpStatistics` (huffman) | 505-526 | `IO.StreamWriter` con `Using` |
| 4 | TCP.vb | `ConnectUser` (numusers) | 1099-1102 | `IO.File.WriteAllText(path, value)` |
| 5 | modForum.vb | `SaveForum` (posts) | 194-206 | `IO.StreamWriter` con `Using` |
| 6 | modForum.vb | `SaveForum` (anuncios) | 210-223 | `IO.StreamWriter` con `Using` |

#### 5F: Escritura binaria — GrabarMapa (1 bloque)

FileIO.vb:401-530. Convertir `FilePut` → `BinaryWriter`:

```vb
' ANTES:
FreeFileMap = FreeFile
FileOpen(FreeFileMap, MAPFILE & ".Map", OpenMode.Binary)
Seek(FreeFileMap, 1)
FilePut(FreeFileMap, MapInfo_Renamed(Map).MapVersion)
' ...

' DESPUÉS:
Using fs As New IO.FileStream(MAPFILE & ".Map", IO.FileMode.Create)
    Using writer As New IO.BinaryWriter(fs)
        writer.Write(MapInfo_Renamed(Map).MapVersion)  ' Int16
        writer.Seek(264, IO.SeekOrigin.Begin)           ' skip 263 bytes + 1
        ' ...
    End Using
End Using
```

**Cuidado**: `Seek(FreeFileMap, 1)` en VB6 es **1-based** (posición byte 1 = offset 0 en .NET). `Seek(handle, Seek(handle) + 263)` salta 263 bytes desde la posición actual.

### Tabla de reemplazos completa

| VB6/Legacy | .NET moderno | Uso |
|-----------|-------------|-----|
| `FreeFile()` | *(eliminado, no necesario)* | Handle management |
| `FileOpen(n, path, OpenMode.Input)` | `Using reader As New IO.StreamReader(path)` | Lectura texto |
| `FileOpen(n, path, OpenMode.Output)` | `Using writer As New IO.StreamWriter(path)` | Escritura texto |
| `FileOpen(n, path, OpenMode.Append)` | `IO.File.AppendAllText(path, line & vbCrLf)` | Logs |
| `FileOpen(n, path, OpenMode.Binary)` | `Using fs As New IO.FileStream(...)` + `BinaryWriter` | Binario |
| `LineInput(n)` | `reader.ReadLine()` | Lectura línea |
| `Input(n, var)` | `reader.ReadLine()` (parseo manual) | Lectura VB6 format |
| `PrintLine(n, s)` | `writer.WriteLine(s)` | Escritura línea |
| `FileClose(n)` | `End Using` (dispose automático) | Cierre |
| `EOF(n)` | `reader.EndOfStream` | Fin de archivo |
| `FilePut(n, val)` | `writer.Write(val)` | Escritura binaria |
| `Seek(n, pos)` | `fs.Seek(offset, SeekOrigin)` | Posición binaria |

### Orden de ejecución

1. **5A** — Helper logging + bloques Append (más fácil, mayor impacto en líneas eliminadas)
2. **5B** — GetVar I/O interno
3. **5C** — clsIniReader I/O interno
4. **5D** — Bloques de lectura de texto restantes
5. **5E** — Bloques de escritura de texto restantes
6. **5F** — GrabarMapa binario

### Verificación

`dotnet build` después de cada sub-etapa. `dotnet run --project Server` después de 5A y 5F.

### Progreso

| Sub-etapa | Archivos | Bloques | Estado |
|-----------|----------|---------|--------|
| 5A | General.vb, FileIO.vb, Protocol.vb, TCP.vb, ConsultasPopulares.vb, modCentinela.vb, GameLoop.vb, Statistics.vb | 35 | ✅ |
| 5B | FileIO.vb (GetVar) | 1 | ✅ |
| 5C | clsIniReader.vb | 1 | ✅ |
| 5D | FileIO.vb, Admin.vb, ConsultasPopulares.vb, modForum.vb | 6 | ✅ |
| 5E | Admin.vb, Statistics.vb, TCP.vb, modForum.vb, GameLoop.vb | 6 | ✅ |
| 5F | FileIO.vb (GrabarMapa) | 1 | ✅ |

---

## Etapa 6: Resolver `As Object` — 25 instancias

Revisar cada `Dim x As Object` y determinar el tipo real.

### Análisis de instancias

**Corregidas (20):**

| Archivo | Variable/Param | Antes | Después | Razón |
|---------|---------------|-------|---------|-------|
| mdParty.vb:356 | `IsPartyMember` (return) | Object | Boolean | Retorna booleano |
| Protocol.vb:8210 | `NPCcount1` | Object | Integer | Contador con aritmética + ReDim |
| Protocol.vb:8212 | `i` | Object | Short | Loop counter (LastNPC es Short) |
| Protocol.vb:15370 | `UserIndex` (param) | Object | Short | UserList index, callers pasan Short |
| MODULO_NPCs.vb:880 | `Respawn` (param) | Object | Boolean | Default `= True` |
| praetorians.vb:891 | `azar` | Object | Short | Math.Sign(RandomNumber(...)) |
| praetorians.vb:1221 | `indice` (param) | Object | Short | NPC spell index |
| praetorians.vb:2073 | `npcind` (param) | Object | Short | NPC list index |
| praetorians.vb:2101 | `npcind` (param) | Object | Short | NPC list index |
| SecurityIp.vb:299 | `DumpTables()` (return) | Function As Object → Sub | — | No retorna nada |
| Modulo_UsUaRiOs.vb:1785 | `nextMap` | Object | Boolean | IIf retorna True/False |
| FileIO.vb:2126 | `i, X` | Object | Short | Loop variables |
| GameLogic.vb:678 | `randomi` | Object | Short | Array index de RandomNumber |
| wskapiAO.vb:113 | `Winsock_Close` (return) | Function As Object → Sub | — | Solo `Return Nothing` |
| clsClan.vb:565 | `Votante` (param) | Object | String | Callers pasan UserList().Name |
| PathFinding.vb:19 | `Limites` (return) | Object | Boolean | Retorna expresión booleana |
| clsdicc.vb | `def`, `elem`, `At()` | Object | — | Archivo eliminado, reemplazado por `Dictionary(Of String, Short)` |
| clsClan.vb:576 | `voteCount` | Object | Short | Eliminado junto con diccionario |

**Mantenidas como `As Object` (5):**
- `GameLoop.vb:37,42` + `SocketManager.vb:93` — Event handlers (`sender As Object` es el patrón .NET)
- `clsByteQueue.vb:54,72` — Serialización polimórfica con `TypeOf`/`GetType` (`ByteArrayToType` es dead code; `TypeToByteArray` usado por 4 Write methods)

### Optimización adicional: Eliminación de `diccionario` (clsdicc.vb)

La clase `diccionario` (138 líneas) era un wrapper VB6 con solo 1 caller (`clsClan.ContarVotos`). Reemplazada por `Dictionary(Of String, Short)` nativo de .NET con `StringComparer.OrdinalIgnoreCase` para preservar el comportamiento de keys case-insensitive.

### Progreso

| Archivo | Estado |
|---------|--------|
| clsdicc.vb | ✅ **Eliminado** (reemplazado por Dictionary(Of String, Short)) |
| clsClan.vb | ✅ (2 corr. + reescrito ContarVotos) |
| Protocol.vb | ✅ (3 corr.) |
| praetorians.vb | ✅ (4 corr.) |
| MODULO_NPCs.vb | ✅ (1 corr.) |
| SecurityIp.vb | ✅ (1 corr.) |
| Modulo_UsUaRiOs.vb | ✅ (1 corr.) |
| FileIO.vb | ✅ (1 corr.) |
| GameLogic.vb | ✅ (1 corr.) |
| wskapiAO.vb | ✅ (1 corr.) |
| mdParty.vb | ✅ (1 corr.) |
| PathFinding.vb | ✅ (1 corr.) |
| clsByteQueue.vb | ✅ (mantiene Object, serialización polimórfica) |
| GameLoop.vb | ✅ (mantiene Object, event handlers) |
| SocketManager.vb | ✅ (mantiene Object, event handler) |

---

## Etapa 7: Activar `Option Strict On` — 5468 errores

> **Plan detallado**: Ver [MIGRATION_STAGE7_PLAN.md](MIGRATION_STAGE7_PLAN.md) para clasificación completa de errores, ejemplos antes/después, y orden de ejecución.

`Option Strict On` ya activado globalmente. Los errores se resuelven por tipo de error (accionables), no por archivo.

### Accionables

| # | Accionable | Errores | Estado |
|---|-----------|---------|--------|
| 1 | Cambiar tipo base de Enums (As Byte/Short) | ~1334 | 🔲 |
| 2 | Reemplazar IIf() por If() | ~100+ | 🔲 |
| 3 | PlayerType bitwise → `<> 0` / `= 0` | ~352 | 🔲 |
| 4 | Truthiness numérica → `<> 0` | ~338 | 🔲 |
| 5 | Boolean → Numérico (`If(boolExpr, 1S, 0S)`) | ~84 | 🔲 |
| 6 | Casts explícitos `Convert.ToXxx()` en aritmética | ~2546 | 🔲 |
| 7 | `.ToString()` en concatenaciones string | ~30 | 🔲 |
| 8 | Residuales (ByRef narrowing, Object operators) | ~4 | 🔲 |

---

## Etapa 8: Expandir `With` statements — ~647 instancias

CodeConverter no maneja `With` nativamente; expandir manualmente genera C# más limpio.

### Estrategia
```vb
' Antes:
With UserList(UserIndex)
    .Flags.IsLogged = True
    .Stats.HP = 100
End With

' Después:
UserList(UserIndex).Flags.IsLogged = True
UserList(UserIndex).Stats.HP = 100
```

Para `With` anidados, trabajar de adentro hacia afuera.

### Archivos por cantidad de `With`

| Archivo | `With` aprox. | Estado |
|---------|--------------|--------|
| Protocol.vb | ~332 | 🔲 |
| Modulo_UsUaRiOs.vb | ~35 | 🔲 |
| modHechizos.vb | ~23 | 🔲 |
| SistemaCombate.vb | ~21 | 🔲 |
| Trabajo.vb | ~29 | 🔲 |
| *(resto ~40 archivos)* | ~207 | 🔲 |

---

## Verificación después de cada batch

⚠️ **IMPORTANTE**: Después de cada cantidad **manejable** de cambios (ej: 1-3 archivos modificados, o completar una sub-etapa), ejecutar:

```bash
cd /Users/franco/Documents/AO/argentum-server-net
dotnet build
dotnet run --project Server
```

### Checklist de verificación:

1. **Build**: Debe compilar **sin errores**. Warnings son aceptables temporalmente.
2. **Run Server**: Levantar el servidor con `dotnet run --project Server`
3. **Validar**: Verificar que:
   - ✅ El servidor arranca correctamente (sin crashes)
   - ✅ Las funcionalidades básicas responden sin problemas
   - ✅ No hay exceptions en los logs relacionadas con los cambios

**No continuar al siguiente batch hasta que la verificación sea exitosa.**

---

## Notas y trampas conocidas

| Patrón | Trampa |
|--------|--------|
| `Val(x)` | Retorna 0 si no es numérico. Usar `ParseVal()` helper. |
| `Mid(s, start)` | VB es **1-based**. En .NET usar `s.Substring(start - 1)`. |
| `InStr(s1, s2)` | VB retorna **1-based** (0 = no encontrado). .NET retorna **0-based** (-1 = no encontrado). Cada uso necesita revisión manual. |
| `Right(s, n)` | Si `n > s.Length` VB no falla, .NET sí. Usar guarda. |
| `UBound(arr)` | En C#, `arr.Length - 1`. Para `List(Of T)`, usar `.Count - 1`. |
| `With` anidados | Expandir de adentro hacia afuera. |
| `Module` → C# | CodeConverter los convierte a `static class` automáticamente. |
| `Option Strict Off` | Permite conversiones implícitas. Al activar `Strict On`, el compilador señala exactamente dónde hay problemas. |

---

## Estadísticas iniciales (baseline)

| Métrica | Valor |
|---------|-------|
| Archivos .vb totales | 51 |
| Líneas de código | ~56,700 |
| `Option Strict Off` | 48 archivos (94%) |
| `Val()` | 446 usos |
| `With` statements | 647 usos |
| `CStr()` | 256 usos |
| Funciones string VB6 (`Left/Right/Mid/Len/InStr`) | ~137 usos |
| `UBound/LBound` | 0 usos |
| `As Object` (late binding) | 31 usos |
| File I/O legacy | 48 bloques (FileOpen→FileClose): 34 append, 8 input, 6 output, 2 binary + 151 PrintLine + 6 LineInput + 6 Input + 15 Kill + 2 Dir |

---

*Última actualización: 2026-03-24 — Etapa 6 COMPLETADA (As Object: 25 → 5, 20 corregidas, clase diccionario eliminada)*
