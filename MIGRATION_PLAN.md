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
| 🔄 2 | Reemplazar funciones string VB6 (`Left`, `Right`, `Mid`, `Len`, `InStr`, etc.) | 20/20 archivos completados (131/135 inst). General.vb: 4 inst de InStr/Mid pendientes |
| 🔲 3 | Reemplazar conversiones de tipo (`CStr`, `CInt`, `CDbl`, `CBool`) | Pendiente |
| 🔲 4 | Reemplazar `UBound()` / `LBound()` | Pendiente |
| 🔲 5 | Modernizar File I/O (`FileOpen`, `Line Input`, `Print #`) | Pendiente |
| 🔲 6 | Resolver `As Object` (late binding) | Pendiente |
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

**EN REVISIÓN (1 archivo)**:
| Archivo | Estado | Notas |
|---------|--------|-------|
| General.vb | 🔄 | 1/5 inst completadas. 4 inst de InStr/Mid en función ReadField requieren conversión cuidadosa (1-based vs 0-based) |

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

| Archivo | CStr aprox. | Estado |
|---------|-------------|--------|
| Protocol.vb | ~83 | 🔲 |
| clsParty.vb | ~20 | 🔲 |
| *(resto)* | ~153 | 🔲 |

---

## Etapa 4: `UBound()` / `LBound()` — ~75 instancias

### Reemplazos

```vb
' Antes:
For i = 0 To UBound(arr)
For i = LBound(arr) To UBound(arr)

' Después:
For i = 0 To arr.Length - 1
For Each item In arr   ' cuando no se usa el índice
```

Para arrays de tipo List(Of T), usar `.Count - 1` en vez de `.Length - 1`.

### Progreso por archivo

| Archivo | Usos | Estado |
|---------|------|--------|
| Protocol.vb | ~24 | 🔲 |
| modGuilds.vb | ~7 | 🔲 |
| Trabajo.vb | ~7 | 🔲 |
| FileIO.vb | ~4 | 🔲 |
| *(resto)* | ~33 | 🔲 |

---

## Etapa 5: File I/O moderno — ~97 instancias

### Archivos afectados principales
- `FileIO.vb` — 2,374 líneas, marcado como "lento e ineficiente" en TODOs
- `TCP.vb` — 3 usos FileOpen
- `modForum.vb` — 4 usos
- `Statistics.vb` — 3 usos

### Reemplazos

| VB6/Legacy | .NET moderno |
|-----------|-------------|
| `FileOpen(n, path, OpenMode.Input)` | `Dim reader = New StreamReader(path)` |
| `FileOpen(n, path, OpenMode.Output)` | `Dim writer = New StreamWriter(path)` |
| `FileOpen(n, path, OpenMode.Append)` | `Dim writer = New StreamWriter(path, append:=True)` |
| `Line Input #n, s` | `s = reader.ReadLine()` |
| `Print #n, s` | `writer.WriteLine(s)` |
| `FileClose(n)` | `reader.Close()` / `writer.Close()` (o `Using`) |
| `EOF(n)` | `reader.EndOfStream` |

**Preferir `Using` blocks** para garantizar dispose:
```vb
Using reader As New StreamReader(path)
    While Not reader.EndOfStream
        Dim line = reader.ReadLine()
        ' ...
    End While
End Using
```

### Progreso

| Archivo | Estado |
|---------|--------|
| modForum.vb | 🔲 |
| Statistics.vb | 🔲 |
| TCP.vb | 🔲 |
| FileIO.vb | 🔲 |

---

## Etapa 6: Resolver `As Object` — ~31 instancias

Revisar cada `Dim x As Object` y determinar el tipo real.
Casos conocidos en Protocol.vb: `NPCcount1`, `i` → probablemente `Integer`.

### Progreso

| Archivo | Estado |
|---------|--------|
| Protocol.vb | 🔲 |
| *(resto)* | 🔲 |

---

## Etapa 7: Activar `Option Strict On` — 48 archivos

Cambiar archivo por archivo. Después de cada cambio: `dotnet build` y corregir errores.

**Orden recomendado** (de menor a mayor riesgo):

### Batch A — Archivos utilitarios/simples
| Archivo | Estado |
|---------|--------|
| modHexaStrings.vb | 🔲 |
| clsIniReader.vb | 🔲 |
| Migration.vb | 🔲 |
| Matematicas.vb | 🔲 |
| clsByteQueue.vb | 🔲 |
| Queue.vb | 🔲 |
| SecurityIp.vb | 🔲 |
| clsMapSoundManager.vb | 🔲 |
| clsEstadisticasIPC.vb | 🔲 |
| cSolicitud.vb | 🔲 |
| clsdicc.vb | 🔲 |
| ModCola.vb | 🔲 |
| ArrayInitializers.vb | 🔲 |
| PathFinding.vb | 🔲 |

### Batch B — Medianos
| Archivo | Estado |
|---------|--------|
| clsClan.vb | 🔲 |
| clsParty.vb | 🔲 |
| mdParty.vb | 🔲 |
| modBanco.vb | 🔲 |
| Statistics.vb | 🔲 |
| Comercio.vb | 🔲 |
| mdlCOmercioConUsuario.vb | 🔲 |
| modForum.vb | 🔲 |
| ModAreas.vb | 🔲 |
| ModFacciones.vb | 🔲 |
| praetorians.vb | 🔲 |
| wskapiAO.vb | 🔲 |
| ConsultasPopulares.vb | 🔲 |
| modCentinela.vb | 🔲 |
| modNuevoTimer.vb | 🔲 |

### Batch C — Complejos
| Archivo | Estado |
|---------|--------|
| AI_NPC.vb | 🔲 |
| SistemaCombate.vb | 🔲 |
| modGuilds.vb | 🔲 |
| modHechizos.vb | 🔲 |
| Trabajo.vb | 🔲 |
| Acciones.vb | 🔲 |
| InvUsuario.vb | 🔲 |
| Modulo_InventANDobj.vb | 🔲 |
| Characters.vb | 🔲 |
| Declares.vb | 🔲 |
| GameLogic.vb | 🔲 |
| GameLoop.vb | 🔲 |
| CargaMapa.vb | 🔲 |
| Admin.vb | 🔲 |

### Batch D — Críticos
| Archivo | Estado |
|---------|--------|
| modSendData.vb | 🔲 |
| SocketManager.vb | 🔲 |
| General.vb | 🔲 |
| Statistics.vb | 🔲 |
| MODULO_NPCs.vb | 🔲 |
| Modulo_UsUaRiOs.vb | 🔲 |
| FileIO.vb | 🔲 |
| TCP.vb | 🔲 |

### Batch E — El más grande (último)
| Archivo | Estado |
|---------|--------|
| Protocol.vb | 🔲 |

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
| `UBound/LBound` | 75 usos |
| `As Object` (late binding) | 31 usos |
| File I/O legacy | ~97 usos |

---

*Última actualización: 2026-03-23 — Plan inicial creado*
