# Pre-CodeConverter: Limpieza previa a la conversión VB.NET → C#

> **Herramienta objetivo**: [`icsharpcode/CodeConverter`](https://github.com/icsharpcode/CodeConverter)
>
> **Principio del converter**: *"creates code as close as possible to what the old code did"* —
> no limpia deuda técnica, solo traduce sintaxis. Si entra código legacy, sale código legacy en C#.
>
> **Objetivo de este plan**: corregir únicamente los patrones que el converter **no resuelve bien**
> o que post-conversión quedan como **código obsoleto, incorrecto o con dependencias de
> `Microsoft.VisualBasic.dll`** innecesarias.
>
> **Branch activo**: `feature/fix-vb6-vbnet-migration-warnings`

---

## Metodología de trabajo: Roslyn + IA

**Regla fundamental de este plan**: los cambios masivos y estructurales se hacen con
**scripts de Roslyn**, no editando archivos uno por uno. La IA asiste en la generación
de esos scripts bajo demanda y en la revisión de casos edge — no en ir archivo por archivo.

### Por qué Roslyn y no edición manual

Este codebase tiene ~56.000 líneas distribuidas en 50 archivos `.vb`. Los patrones a
corregir son repetitivos pero semánticamente sensibles (no son simples find & replace):
necesitan analizar el AST para distinguir, por ejemplo, un `Err.Number` dentro de un
`Catch` de uno fuera de él, o un `vbNullString` en asignación de uno en comparación.

Roslyn tiene acceso al árbol sintáctico y semántico completo del proyecto. Un script
bien escrito puede hacer en segundos lo que tomaría horas de edición manual con riesgo
de error humano.

### Cómo se trabaja

1. **La IA genera el script de Roslyn** (C# usando `Microsoft.CodeAnalysis`) específico
   para el patrón a corregir.
2. **Se ejecuta el script** sobre el proyecto VB.NET directamente.
3. **Se verifica con `dotnet build`** y se commitea.
4. La IA solo edita archivos individuales para los casos puntuales (≤3 lugares) donde
   un script sería sobredimensionado.

### Estructura de un script Roslyn típico

Los scripts se crean como proyectos de consola desechables en `Script/roslyn/` y se
ejecutan con `dotnet run`:

```
Script/
  roslyn/
    fix-err-number/        ← un proyecto por acción
      Program.cs
      fix-err-number.csproj
    fix-vbnullstring/
      Program.cs
      ...
```

Cada script:
- Carga el workspace del proyecto VB.NET via `MSBuildWorkspace`
- Recorre el AST buscando el patrón exacto (no regex sobre texto)
- Aplica la transformación y escribe los archivos modificados en disco
- Imprime un resumen de cambios realizados

### Cuándo usar Roslyn vs edición directa

| Criterio | Roslyn | Edición directa |
|----------|--------|-----------------|
| Patrón aparece en >3 archivos | ✅ | — |
| Requiere análisis semántico (distinguir contextos) | ✅ | — |
| Transformación estructural (reemplazar nodo AST) | ✅ | — |
| Caso puntual conocido, ≤3 lugares exactos | — | ✅ |
| Acción 4 (2 líneas en 1 archivo) | — | ✅ |
| Acción 8 (auditoría, no edición) | — | ✅ |

> **Importante para la IA**: al ejecutar cualquier acción de este plan, **siempre evaluar
> primero si corresponde un script de Roslyn**. Si el patrón afecta más de 3 archivos o
> requiere distinción semántica de contexto, **generar el script antes de tocar código**.
> No pedir permiso para hacerlo — está implícito en este plan.

---

## Estado General

| # | Acción | Impacto | Archivos | Estado |
|---|--------|---------|----------|--------|
| 1 | Reemplazar `Err.Number`/`Err.Description` en bloques `Catch` | Crítico | 9 archivos, ~25 lugares | 🔲 |
| 2 | Reemplazar `MsgBox()` en código de servidor | Crítico | `FileIO.vb` (4 lugares) | ✅ |
| 3 | Reemplazar `vbNullString` | Alto | ~18 archivos | 🔲 |
| 4 | Reemplazar `vbObjectError` en `clsByteQueue.vb` | Alto | 1 archivo, 2 líneas | ✅ |
| 5 | Reemplazar `IsNumeric()` por `TryParse()` | Medio | 5 archivos, ~15 lugares | 🔲 |
| 6 | Reemplazar `Today` / `TimeOfDay` en concatenaciones | Medio | 4 archivos, ~8 lugares | 🔲 |
| 7 | Renombrar identificadores `_Renamed` antes de convertir | Bajo | Global, 4 tipos centrales | 🔲 |
| 8 | Auditar `ReDim Preserve` sobre arrays multidimensionales | Bajo | `FileIO.vb` (2 lugares) | 🔲 |

**Leyenda**: ✅ Completado | 🔄 En progreso | 🔲 Pendiente

---

## Qué NO necesita limpieza previa

El converter **maneja bien** estos casos según su CHANGELOG documentado — no hay que tocarlos:

| Patrón | Qué hace el converter |
|--------|-----------------------|
| `Call` keyword (~3.700 usos) | Lo elimina correctamente (v8.0.3) |
| `Module ... End Module` (38 módulos) | Convierte a `internal static class` |
| `ReDim Preserve` sobre arrays 1D | Convierte a `Array.Resize(ref arr, n)` (v8.1.3, v10.0.1) |
| `ByVal` / `ByRef` | Traduce correctamente |
| `Convert.ToXxx()` | Pasa directo, válido en C# |
| `For i = 1 To N` / loops 1-based | Convierte la sintaxis; la lógica 1-based es responsabilidad del dev |
| `With ... End With` | Expande a acceso completo (ya en el plan de Etapa 8) |

---

## Acción 1 — `Err.Number` / `Err.Description` en bloques `Catch`

> **Herramienta**: Script Roslyn — 9 archivos, patrón requiere verificar que `Err.*`
> esté dentro de un nodo `CatchBlockSyntax`. No es un simple find & replace.

### Por qué es crítico

El converter traduce `Err.Number` a `Interaction.Err().Number` y `Err.Description` a
`Interaction.Err().Description` (de `Microsoft.VisualBasic.Interaction`). **Compila**, pero
el objeto `Err` dentro de un bloque `Catch ex As Exception` **no contiene el error actual** —
`ex` lo tiene. El resultado es que los logs de error llevan años mostrando datos vacíos o
de errores anteriores.

### Patrón actual (incorrecto)

```vb
Try
    ' ... código ...
Catch ex As Exception
    Console.WriteLine("Error in X: " & ex.Message)
    Call LogError("Error en X. Error " & Err.Number & " : " & Err.Description)
End Try
```

### Reemplazo correcto

```vb
Try
    ' ... código ...
Catch ex As Exception
    Console.WriteLine("Error in X: " & ex.Message)
    Call LogError("Error en X. [" & ex.GetType().Name & "] " & ex.Message)
End Try
```

### Archivos afectados

| Archivo | Usos aprox. | Estado |
|---------|-------------|--------|
| `Trabajo.vb` | 6 | 🔲 |
| `SistemaCombate.vb` | 5 | 🔲 |
| `TCP.vb` | 3 | 🔲 |
| `FileIO.vb` | 3 | 🔲 |
| `modHechizos.vb` | 2 | 🔲 |
| `MODULO_NPCs.vb` | 2 | 🔲 |
| `Modulo_InventANDobj.vb` | 1 | 🔲 |
| `mdlCOmercioConUsuario.vb` | 1 | 🔲 |
| `modCentinela.vb` | 1 | 🔲 |

### Verificación

```bash
rg 'Err\.Number|Err\.Description|Err\.HelpContext|Err\.HelpFile|Err\.Source' Legacy/Source/ --type vb
# Resultado esperado: 0 matches
dotnet build
```

---

## Acción 2 — `MsgBox()` en código de servidor

> **Herramienta**: Edición directa — 4 lugares exactos en un solo archivo. Cada
> reemplazo es diferente (2 son `LogError`, 2 son `Throw`), más rápido a mano.

### Por qué es crítico

El converter traduce `MsgBox(...)` a `Interaction.MsgBox(...)`. En Windows compila, pero en
un servidor headless (Linux/Docker) **lanza excepción en runtime o bloquea el proceso**
esperando input de UI que nunca llega.

### Patrón actual

```vb
Catch ex As Exception
    MsgBox("Error cargando hechizos.dat " & Err.Number & ": " & Err.Description)
End Try
```

### Reemplazo correcto

```vb
Catch ex As Exception
    Throw New Exception("Error cargando hechizos.dat: " & ex.Message, ex)
End Try
```

O si el error es recuperable y se prefiere loguear sin lanzar:

```vb
Catch ex As Exception
    Call LogError("Error cargando hechizos.dat: " & ex.Message)
End Try
```

### Ubicaciones exactas en `FileIO.vb`

| Función | Línea aprox. | Reemplazo sugerido |
|---------|-------------|-------------------|
| `CargarSpawnList` | ~297 | `LogError` + mensaje |
| `LoadMotd` | ~845 | `LogError` + mensaje |
| `CargarBackUp` | ~1207 | `Throw` (fallo fatal de mapa) |
| `LoadMapData` | ~1245 | `Throw` (fallo fatal de mapa) |

> **Nota**: Los dos últimos son fallos de carga de mapa — corresponde lanzar excepción para
> que el servidor falle rápido y visible, no silenciosamente.

### Verificación

```bash
rg 'MsgBox\b' Legacy/Source/ --type vb
# Resultado esperado: 0 matches (o solo en comentarios)
dotnet build
```

---

## Acción 3 — `vbNullString`

> **Herramienta**: Script Roslyn — ~18 archivos, y el reemplazo correcto depende del
> contexto sintáctico (asignación vs. comparación vs. parámetro default). Un script
> puede visitar cada `IdentifierNameSyntax` con valor `vbNullString` y emitir la
> transformación adecuada según el nodo padre.

### Por qué es alto

El converter puede traducir `vbNullString` a `""` o a `Constants.vbNullString`
dependiendo del contexto. En VB6 `vbNullString` es técnicamente `Nothing` (puntero nulo),
no una cadena vacía — la semántica no es idéntica. El C# resultante puede tener
comparaciones incorrectas.

### Tabla de reemplazos

| Contexto de uso | Reemplazo correcto |
|----------------|-------------------|
| Asignación: `x = vbNullString` | `x = String.Empty` |
| Comparación: `If x = vbNullString` | `If String.IsNullOrEmpty(x)` |
| Parámetro por default: `Optional ByVal x As String = vbNullString` | `Optional ByVal x As String = ""` |

### Archivos afectados (>1 uso)

| Archivo | Usos | Estado |
|---------|------|--------|
| `MODULO_NPCs.vb` | 7 | 🔲 |
| `Modulo_UsUaRiOs.vb` | 5 | 🔲 |
| `GameLogic.vb` | 2 | 🔲 |
| `clsClan.vb` | 3 | 🔲 |
| `Admin.vb` | 1 | 🔲 |

> Ejecutar `rg 'vbNullString' Legacy/Source/ --type vb` para lista completa actualizada.

### Verificación

```bash
rg 'vbNullString' Legacy/Source/ --type vb
# Resultado esperado: 0 matches
dotnet build
```

---

## Acción 4 — `vbObjectError` en `clsByteQueue.vb`

> **Herramienta**: Edición directa — 2 líneas en 1 archivo. Trivial.

### Por qué es alto

`vbObjectError` es una constante COM legacy (`&H80040000 = -2147221504`). El converter
la traduce a `(int)Constants.vbObjectError + 9`, generando una dependencia de
`Microsoft.VisualBasic.Constants` por dos líneas de código.

### Ubicación exacta

`Legacy/Source/clsByteQueue.vb`, líneas 8-9:

```vb
' ANTES:
Private Const NOT_ENOUGH_DATA As Integer = vbObjectError + 9
Private Const NOT_ENOUGH_SPACE As Integer = vbObjectError + 10

' DESPUÉS (valor literal equivalente):
Private Const NOT_ENOUGH_DATA As Integer = -2147221495
Private Const NOT_ENOUGH_SPACE As Integer = -2147221494
```

> **Alternativa mejor**: convertirlas en excepciones tipadas antes de migrar, si hay tiempo.
> `NOT_ENOUGH_DATA` y `NOT_ENOUGH_SPACE` son códigos de error arrojados como `New Exception`.

### Verificación

```bash
rg 'vbObjectError' Legacy/Source/ --type vb
# Resultado esperado: 0 matches
dotnet build
```

---

## Acción 5 — `IsNumeric()` → `TryParse()`

> **Herramienta**: Script Roslyn — el reemplazo no es uniforme (el tipo del `TryParse`
> depende del tipo de la variable que recibe el resultado). Roslyn puede resolver el
> tipo del símbolo receptor via análisis semántico y emitir el `TryParse` correcto
> (`Short.TryParse`, `Integer.TryParse`, etc.) sin necesidad de revisar cada caso a mano.

### Por qué es medio

El converter traduce `IsNumeric(x)` a `Information.IsNumeric(x)` de
`Microsoft.VisualBasic.Information`. Compila y funciona, pero:
1. Mantiene dependencia de `Microsoft.VisualBasic.dll`
2. `IsNumeric()` acepta strings como `"1,5"`, fechas y booleanos como numéricos —
   semántica diferente a `TryParse()`. Si el código depende de eso, hay que saberlo.

### Patrón de reemplazo

```vb
' ANTES:
If IsNumeric(Temps) Then
    valor = Convert.ToInt16(Temps)
End If

' DESPUÉS:
Dim parsed As Short
If Short.TryParse(Temps, parsed) Then
    valor = parsed
End If
```

Para `Double`:
```vb
' ANTES:
If IsNumeric(x) Then resultado = Convert.ToDouble(x)

' DESPUÉS:
Dim d As Double
If Double.TryParse(x, System.Globalization.NumberStyles.Any,
                   System.Globalization.CultureInfo.InvariantCulture, d) Then
    resultado = d
End If
```

### Archivos afectados

| Archivo | Usos | Tipo esperado | Estado |
|---------|------|--------------|--------|
| `clsClan.vb` | 8 | `Short` / `Integer` | 🔲 |
| `modGuilds.vb` | 2 | `Short` | 🔲 |
| `FileIO.vb` | 2 | `Short` | 🔲 |
| `clsMapSoundManager.vb` | 1 | `Short` | 🔲 |
| `Queue.vb` | 1 | `Boolean` (IsEmpty) — no es `IsNumeric` real | 🔲 |

> **Nota `Queue.vb`**: La función `IsEmpty()` en ese archivo no es `IsNumeric` — es una
> función propia que retorna booleano. No confundir.

### Verificación

```bash
rg '\bIsNumeric\b' Legacy/Source/ --type vb
# Resultado esperado: 0 matches
dotnet build
```

---

## Acción 6 — `Today` / `TimeOfDay` en concatenaciones de string

> **Herramienta**: Script Roslyn — detectar `Today` y `TimeOfDay` como
> `IdentifierNameSyntax` dentro de expresiones de concatenación (`BinaryExpressionSyntax`
> con operador `&`) y reemplazar por `.ToString("...")`. Son ~8 lugares en 4 archivos;
> borderline para edición directa, pero un script evita olvidar alguno.

### Por qué es medio

El converter traduce `Today` a `DateTime.Today` y `TimeOfDay` a `DateTime.Now.TimeOfDay`
cuando tiene contexto semántico. Sin embargo, cuando están dentro de concatenaciones con `&`,
puede generar `Conversions.ToString(DateTime.Today)` — un helper de `Microsoft.VisualBasic`
— en lugar del idiomático `.ToString()` de C#.

### Patrón de reemplazo

```vb
' ANTES:
Call LogError(Today & " " & Err.Description & " " & Err.Source)
AppendLog("logs/Main.log", Today & " " & TimeOfDay & " server cerrado.")

' DESPUÉS:
Call LogError(DateTime.Today.ToShortDateString() & " " & ex.Message)
AppendLog("logs/Main.log", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " server cerrado.")
```

> **Oportunidad**: unificar el formato de fecha/hora en un helper único antes de convertir,
> para que todo el logging use el mismo formato en C#.

### Archivos afectados

| Archivo | Usos | Estado |
|---------|------|--------|
| `FileIO.vb` | 2 (en catch con `Err.*`) | 🔲 |
| `Protocol.vb` | 4 | 🔲 |
| `clsClan.vb` | 1 | 🔲 |
| `GameLoop.vb` | 1 | 🔲 |

> **Nota**: Los usos de `FileIO.vb` ya se cubren parcialmente con la Acción 1 (al reemplazar
> `Err.*` en esos catch). Revisar que no queden `Today`/`TimeOfDay` sueltos después.

### Verificación

```bash
rg '\bToday\b|\bTimeOfDay\b' Legacy/Source/ --type vb
# Resultado esperado: 0 matches
dotnet build
```

---

## Acción 7 — Renombrar identificadores `_Renamed`

> **Herramienta**: Script Roslyn con `RenameSymbol` de `Microsoft.CodeAnalysis.Rename` —
> es exactamente para esto. Un script itera cada símbolo `_Renamed`, resuelve la
> `ISymbol` correspondiente y llama `Renamer.RenameSymbolAsync()` con el nombre nuevo.
> Esto garantiza que todos los usos (declaración + referencias) se actualicen
> consistentemente, incluyendo archivos que no están en la lista visible.
> **No usar find & replace de texto** — puede romper comentarios o strings con el mismo nombre.

### Por qué es bajo (pero vale la pena hacerlo en VB)

El wizard de upgrade VB6→VB.NET renombró identificadores que colisionaban con keywords
del runtime. El converter los pasa tal cual — el C# resultante va a tener 1.241 referencias
a nombres como `Char_Renamed`, `Object_Renamed`, etc. Compila, pero es confuso y dificulta
el trabajo posterior.

**Es más seguro hacerlo en VB** donde el tooling (Roslyn en VS) tiene el contexto completo
para hacer el rename sin romper nada. Post-conversión funciona también, pero son más archivos.

### Mapa de renombres sugeridos

| Identificador actual | Propuesto | Ocurrencias |
|---------------------|-----------|-------------|
| `Char_Renamed` | `CharInfo` | **522** |
| `Object_Renamed` | `ItemSlot` | **375** |
| `ObjData_Renamed` | `ObjData` | **264** |
| `MapInfo_Renamed` | `MapInfo` | **80** |
| `ModClase_Renamed` | `ModClase` | presente |
| `LevelSkill_Renamed` | `LevelSkill` | presente |

> **Verificar primero** que el nombre propuesto no colisiona con otro tipo o variable
> existente antes de aplicar cada rename. Usar **Rename (Ctrl+R, R)** en Visual Studio
> sobre el código VB — no hacer find & replace manual.

### Verificación

```bash
rg '_Renamed\b' Legacy/Source/ --type vb
# Resultado esperado: 0 matches
dotnet build
```

---

## Acción 8 — `ReDim Preserve` sobre arrays multidimensionales

> **Herramienta**: Script Roslyn de **auditoría** (solo lectura, sin modificar) — recorre
> todos los nodos `ReDimClauseSyntax` con `Preserve`, verifica el rango de dimensiones
> del array y reporta los que tienen más de 1 dimensión. La corrección posterior es manual
> porque implica rediseño de estructura de datos.

### Por qué es bajo pero con riesgo puntual

El converter convierte `ReDim Preserve` de arrays **1D** a `Array.Resize(ref arr, n)`
correctamente (documentado en CHANGELOG v8.1.3 y v10.0.1).

Para arrays **multidimensionales**, `Array.Resize` no aplica — el converter puede generar
código que no compila o que tiene comportamiento incorrecto.

### Ubicaciones a auditar en `FileIO.vb`

```vb
' Línea ~1181 y ~1231:
ReDim MapData(NumMaps, XMaxMapSize, YMaxMapSize)   ' 3D — sin Preserve, OK
ReDim MapInfo_Renamed(NumMaps)                      ' 1D — OK

' Verificar si en alguna rama del código existe:
ReDim Preserve MapData(...)    ' ← ESTE sería problemático
```

Si `MapData` (array 3D) tiene algún `ReDim Preserve` en cualquier rama de código,
convertirlo a `List(Of T)` o reestructurar **antes** de la conversión automática.

### Verificación

```bash
rg 'ReDim Preserve' Legacy/Source/ --type vb
# Revisar manualmente cada resultado que involucre arrays de más de 1 dimensión
```

---

## Orden de Ejecución Recomendado

| Paso | Acción | Herramienta | Tiempo estimado |
|------|--------|-------------|-----------------|
| 1 | Acción 4 — `vbObjectError` | Edición directa | 5 min |
| 2 | Acción 2 — `MsgBox()` | Edición directa | 15 min |
| 3 | Acción 8 — Auditoría `ReDim Preserve` multidim | Script Roslyn (solo lectura) | 20 min |
| 4 | Acción 1 — `Err.Number`/`Err.Description` | Script Roslyn | 30 min |
| 5 | Acción 3 — `vbNullString` | Script Roslyn | 30 min |
| 6 | Acción 6 — `Today`/`TimeOfDay` | Script Roslyn | 20 min |
| 7 | Acción 5 — `IsNumeric` → `TryParse` | Script Roslyn | 30 min |
| 8 | Acción 7 — Renombrar `_Renamed` | Script Roslyn (`RenameSymbolAsync`) | 45 min |

> **Flujo por cada paso con Roslyn**:
> 1. La IA genera el script en `Script/roslyn/<nombre>/`
> 2. `dotnet run --project Script/roslyn/<nombre>`
> 3. El script imprime resumen de cambios
> 4. `dotnet build` para verificar
> 5. `git add -A && git commit -m "fix: <acción>"`

---

## Verificación Final Pre-Conversión

```bash
# Sin rastros de Microsoft.VisualBasic legacy en el código de negocio:
rg 'Err\.Number|Err\.Description|Err\.HelpContext|Err\.Source' Legacy/Source/ --type vb
rg '\bMsgBox\b' Legacy/Source/ --type vb
rg '\bvbNullString\b' Legacy/Source/ --type vb
rg '\bvbObjectError\b' Legacy/Source/ --type vb
rg '\bIsNumeric\b' Legacy/Source/ --type vb
rg '\bToday\b|\bTimeOfDay\b' Legacy/Source/ --type vb
rg '_Renamed\b' Legacy/Source/ --type vb

# Build limpio:
dotnet build
# Resultado esperado: 0 errores (warnings son aceptables)

# Servidor arranca:
dotnet run --project Server
```

Una vez que todos estos dan 0 resultados y el build es limpio, el proyecto está listo
para pasarle el CodeConverter.

---

## Referencias

- [icsharpcode/CodeConverter — Wiki](https://github.com/icsharpcode/CodeConverter/wiki)
- [CHANGELOG del converter](https://github.com/icsharpcode/CodeConverter/blob/master/CHANGELOG.md)
- [MIGRATION_PLAN.md](./MIGRATION_PLAN.md) — Etapas 0-8 (limpieza VB.NET previa)
- [MIGRATION_STAGE7_PLAN.md](./MIGRATION_STAGE7_PLAN.md) — Plan detallado de Option Strict On

---

*Creado: 2026-03-25 — Branch: `feature/fix-vb6-vbnet-migration-warnings`*
