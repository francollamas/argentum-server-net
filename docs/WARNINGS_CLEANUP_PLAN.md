# 📋 PLAN DE CORRECCIÓN DE 100 WARNINGS

**Proyecto:** argentum-server-net  
**Fecha:** 2026-03-29  
**Estrategia:** Híbrida (Automatización + Revisión Manual)  
**Total Warnings:** 100

---

## 📊 RESUMEN EJECUTIVO

| Categoría | Warnings | Estrategia | Estado |
|-----------|----------|------------|--------|
| CS0168 - Variables no usadas | 59 | ✅ Automatizada | ⬜ Pendiente |
| CS0219 - Variables asignadas no usadas | 5 | ✅ Automatizada | ⬜ Pendiente |
| CS0169 - Fields nunca usados | 2 | ✅ Automatizada | ⬜ Pendiente |
| CS0649 - Fields nunca asignados | 19 | 🔄 Mixta | ⬜ Pendiente |
| CS8981 - Nombre tipo lowercase | 1 | ✅ Automatizada | ⬜ Pendiente |
| CS1717 - Self-assignment | 5 | ⚠️ Manual | ⬜ Pendiente |
| CS0162 - Código inalcanzable | 6 | ⚠️ Manual | ⬜ Pendiente |
| CS0652 - Comparación fuera de rango | 2 | ⚠️ Manual | ⬜ Pendiente |
| CA1416 - API específica de plataforma | 1 | ⚠️ Manual | ⬜ Pendiente |
| **TOTAL** | **100** | | |

---

## 🎯 FASE 1: AUTOMATIZACIÓN CON ROSLYN (82 warnings)

### ⬜ Tarea 1.1: Setup del Script Roslyn

**Archivo a crear:** `Script/WarningsFixer.csx`

**Dependencias necesarias:**
```xml
<PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.8.0" />
```

**Checklist:**
- ⬜ Crear estructura de carpetas
- ⬜ Configurar Script.csproj con dependencias
- ⬜ Crear WarningsFixer.csx
- ⬜ Crear helpers en Analyzers/

---

### ⬜ Tarea 1.2: CS0168 - Variables Declaradas No Usadas (59 warnings)

**Estrategia:**
- Eliminar declaraciones de variables locales sin referencias
- **CASO ESPECIAL:** Variables en catch blocks → agregar `Console.WriteLine`

**Archivos afectados:**

#### Catch blocks con excepciones no usadas (11):
- ⬜ `SocketManager.cs:164` - catch (Exception ex) → agregar logging
- ⬜ `SocketManager.cs:217` - catch (Exception ex) → agregar logging
- ⬜ `SocketManager.cs:293` - catch (Exception ex) → agregar logging
- ⬜ `SocketManager.cs:299` - catch (Exception ex) → agregar logging
- ⬜ `SocketManager.cs:373` - catch (Exception ex) → agregar logging
- ⬜ `SocketManager.cs:428` - catch (Exception ex) → agregar logging
- ⬜ `SocketManager.cs:432` - catch (Exception ex) → agregar logging
- ⬜ `SocketManager.cs:555` - catch (Exception ex) → agregar logging
- ⬜ `SocketManager.cs:559` - catch (Exception ex) → agregar logging
- ⬜ `SocketManager.cs:563` - catch (Exception ex) → agregar logging
- ⬜ `FileIO.cs:1539` - catch (Exception ex) → agregar logging
- ⬜ `Protocol.cs:19577` - catch (Exception ex) → agregar logging

#### Variables locales no usadas (47):
- ⬜ `mdlCOmercioConUsuario.cs:131` - variable 'TerminarAhora'
- ⬜ `InvUsuario.cs:59` - variable 'flag'
- ⬜ `InvUsuario.cs:208` - variable 'i'
- ⬜ `InvUsuario.cs:541` - variable 'X'
- ⬜ `InvUsuario.cs:542` - variable 'Y'
- ⬜ `InvUsuario.cs:634` - variable 'Slot'
- ⬜ `clsIniReader.cs:78` - variable 'handle'
- ⬜ `Admin.cs:127` - variable 'Porc'
- ⬜ `modForum.cs:29` - variable 'FileIndex'
- ⬜ `modForum.cs:182` - variable 'FileIndex'
- ⬜ `FileIO.cs:437` - variable 'i'
- ⬜ `FileIO.cs:1287` - variable 'NpcIndex'
- ⬜ `FileIO.cs:1557` - variable 'TempInt'
- ⬜ `FileIO.cs:1559` - variable 'npcfile'
- ⬜ `FileIO.cs:2767` - variable 'X'
- ⬜ `FileIO.cs:2768` - variable 'Y'
- ⬜ `FileIO.cs:2874` - variable 'i'
- ⬜ `FileIO.cs:2927` - variable 'RaceIndex'
- ⬜ `modSendData.cs:66` - variable 'Map'
- ⬜ `General.cs:260` - variable 'f'
- ⬜ `Modulo_InventANDobj.cs:344` - variable 'i'
- ⬜ `praetorians.cs:931` - variable 'NPCAlInd'
- ⬜ `GameLoop.cs:609` - variable 'i'
- ⬜ `GameLoop.cs:610` - variable 'num'
- ⬜ `GameLoop.cs:706` - variable 'X'
- ⬜ `GameLoop.cs:707` - variable 'Y'
- ⬜ `GameLoop.cs:708` - variable 'UseAI'
- ⬜ `AI_NPC.cs:947` - variable 'tHeading'
- ⬜ `AI_NPC.cs:949` - variable 'SignoNS'
- ⬜ `AI_NPC.cs:950` - variable 'SignoEO'
- ⬜ `MODULO_NPCs.cs:1053` - variable 'aux'
- ⬜ `TCP.cs:864` - variable 'N'
- ⬜ `TCP.cs:1801` - variable 'N'
- ⬜ `TCP.cs:1802` - variable 'LoopC'
- ⬜ `Protocol.cs:4813` - variable 'file'
- ⬜ `Protocol.cs:4817` - variable 'postFile'
- ⬜ `Protocol.cs:6272` - variable 'isNotVisible'
- ⬜ `Protocol.cs:7973` - variable 'N'
- ⬜ `Protocol.cs:10604` - variable 'N'
- ⬜ `Protocol.cs:11623` - variable 'LoopC'
- ⬜ `Protocol.cs:13327` - variable 'LoopC'
- ⬜ `Protocol.cs:13405` - variable 'LoopC'
- ⬜ `Protocol.cs:13473` - variable 'lista'
- ⬜ `Protocol.cs:14180` - variable 'tStr'
- ⬜ `Protocol.cs:16435` - variable 'handle'
- ⬜ `Protocol.cs:17010` - variable 'error'
- ⬜ `Protocol.cs:18371` - variable 'i'

**Transformación ejemplo:**
```csharp
// ANTES
int flag;
SomeMethod();

// DESPUÉS
SomeMethod();
```

**Transformación catch block:**
```csharp
// ANTES
catch (Exception ex)
{
    // código sin usar ex
}

// DESPUÉS
catch (Exception ex)
{
    Console.WriteLine($"Exception in [MethodName]: {ex.Message}");
    // código original
}
```

---

### ⬜ Tarea 1.3: CS0219 - Variables Asignadas No Usadas (5 warnings)

**Estrategia:**
- Eliminar variable + asignación completa

**Archivos afectados:**
- ⬜ `praetorians.cs:264` - variable 'PJBestTarget'
- ⬜ `praetorians.cs:479` - variable 'PJBestTarget'
- ⬜ `FileIO.cs:2564` - variable 'argEmptySpaces2'
- ⬜ `FileIO.cs:2569` - variable 'argEmptySpaces4'
- ⬜ `FileIO.cs:2581` - variable 'argEmptySpaces8'

**Transformación:**
```csharp
// ANTES
int PJBestTarget = 0;
DoSomething();

// DESPUÉS
DoSomething();
```

---

### ⬜ Tarea 1.4: CS0169 - Fields Nunca Usados (2 warnings)

**Estrategia:**
- Eliminar field declaration completa

**Archivos afectados:**
- ⬜ `PathFinding.cs:14` - field 'TilePosY'
- ⬜ `GameLoop.cs:49` - field '_TickAutoSave_MinsPjesSave'

**Transformación:**
```csharp
// ANTES
private int TilePosY;

// DESPUÉS
// (eliminado)
```

---

### ⬜ Tarea 1.5: CS0649 - Fields Nunca Asignados (Sub-tarea: Eliminables - 15 warnings)

**Estrategia:**
- Eliminar fields que nunca se asignan NI se leen

**Archivos afectados:**
- ⬜ `Admin.cs:32` - field 'IntervaloNPCPuedeAtacar'
- ⬜ `Admin.cs:33` - field 'IntervaloNPCAI'
- ⬜ `Admin.cs:31` - field 'IntervaloLanzaHechizo'
- ⬜ `Declares.cs:1090` - field 'tPartyData.RemXP'
- ⬜ `Declares.cs:1089` - field 'tPartyData.PIndex'
- ⬜ `Declares.cs:1091` - field 'tPartyData.TargetUser'
- ⬜ `Declares.cs:879` - field 'NumFX'
- ⬜ `Declares.cs:898` - field 'EnPausa'
- ⬜ `Declares.cs:825` - field 'SERVERONLINE'
- ⬜ `Declares.cs:827` - field 'BackUp'
- ⬜ `Declares.cs:919` - field 'FX'
- ⬜ `Declares.cs:1110` - field 'FXdata.GrhIndex'
- ⬜ `Declares.cs:1109` - field 'FXdata.Nombre'
- ⬜ `Declares.cs:1111` - field 'FXdata.Delay'
- ⬜ `clsByteQueue.cs:599` - field 'ByteConverter.LongValue'
- ⬜ `clsByteQueue.cs:600` - field 'ByteConverter.SingleValue'
- ⬜ `clsByteQueue.cs:611` - field 'DoubleConverter.DoubleValue'
- ⬜ `cSolicitud.cs:5` - field 'desc'
- ⬜ `cSolicitud.cs:6` - field 'UserName'

**Nota:** El script validará que realmente no tengan referencias antes de eliminar.

---

### ⬜ Tarea 1.6: CS8981 - Nombre de Tipo Lowercase (1 warning)

**Estrategia:**
- Renombrar tipo `npc` → `Npc` usando Roslyn Renamer
- Actualizar TODAS las referencias en el proyecto

**Archivo afectado:**
- ⬜ `Declares.cs:1710` - tipo 'npc'

**Transformación:**
```csharp
// ANTES
public class npc 
{
    // ...
}

// DESPUÉS
public class Npc 
{
    // ...
}
```

**Referencias a actualizar:** ~47 referencias estimadas en todo el proyecto

---

### ⬜ Tarea 1.7: Generar Reporte de Cambios

**Archivo a generar:** `docs/WARNINGS_CLEANUP_REPORT.md`

**Contenido:**
- ✅ Total de warnings corregidas automáticamente
- 📝 Lista detallada de cada cambio (archivo:línea)
- ⚠️ Advertencias sobre cambios que podrían afectar lógica
- 📋 Diff resumido por archivo

---

## 🧐 FASE 2: REVISIÓN MANUAL ASISTIDA (18 warnings)

### ⬜ Tarea 2.1: CS1717 - Self-Assignments (5 casos)

⚠️ **REQUIERE DECISIÓN MANUAL - POSIBLES BUGS**

#### Caso 1: ⬜ `modGuilds.cs:470`
```csharp
// CÓDIGO ACTUAL (líneas 466-474)
var loopTo = codex.Length - 1;
for (i = 0; i <= loopTo; i++)
    withBlock.SetCodex(Convert.ToByte(i), ref codex[i]);

var loopTo1 = Convert.ToInt32(CANTIDADMAXIMACODEX);
for (i = i; i <= loopTo1; i++)  // ← BUG: debería inicializar desde codex.Length
{
    var argcodex = Constants.vbNullString;
    withBlock.SetCodex(Convert.ToByte(i), ref argcodex);
}
```

**Problema:** El segundo loop inicia con `i = i`, manteniendo el valor del loop anterior

**Fix propuesto:**
```csharp
for (i = codex.Length; i <= loopTo1; i++)
```

**Estado:** ⬜ Pendiente revisión  
**Riesgo:** ⚠️ ALTO - Puede causar comportamiento incorrecto en codex

---

#### Caso 2: ⬜ `ModAreas.cs:443`
```csharp
// CÓDIGO ACTUAL (líneas 438-444)
ConnGroups[Map].CountEntrys = ConnGroups[Map].CountEntrys - 1;
TempVal = ConnGroups[Map].CountEntrys;

// Move list back
var loopTo1 = TempVal;
for (LoopC = LoopC; LoopC <= loopTo1; LoopC++)  // ← BUG: falta inicializar LoopC
    ConnGroups[Map].UserEntrys[LoopC] = ConnGroups[Map].UserEntrys[LoopC + 1];
```

**Problema:** `LoopC` no se inicializa, usa valor previo (podría ser cualquier cosa)

**Fix propuesto:** Necesita revisar contexto para saber el valor inicial correcto
```csharp
// Opción A: Si LoopC viene del contexto anterior
for (LoopC = LoopC; LoopC <= loopTo1; LoopC++)

// Opción B: Si debe empezar desde índice específico (probablemente esto)
for (LoopC = 1; LoopC <= loopTo1; LoopC++)
```

**Estado:** ⬜ Pendiente revisión  
**Riesgo:** ⚠️ ALTO - Puede causar índices incorrectos en array shifting

---

#### Caso 3: ⬜ `Modulo_UsUaRiOs.cs:2131`
```csharp
// CÓDIGO ACTUAL (líneas 2128-2132)
else if (!previousMap & !nextMap)
{
    // 140 => 141 (Ninguno es superficial, el ultimo mapa es el mismo de antes)
    withBlock.flags.lastMap = withBlock.flags.lastMap;  // ← NOP inútil
}
```

**Problema:** El comentario dice que debe mantener el valor anterior, pero la asignación es redundante

**Fix propuesto:**
```csharp
else if (!previousMap & !nextMap)
{
    // 140 => 141 (Ninguno es superficial, el ultimo mapa es el mismo de antes)
    // withBlock.flags.lastMap se mantiene sin cambios
}
```

**Estado:** ⬜ Pendiente revisión  
**Riesgo:** 🟢 BAJO - Es solo una línea redundante que no hace nada

---

#### Caso 4: ⬜ `FileIO.cs:839`
```csharp
// CÓDIGO ACTUAL (líneas 837-840)
withBlock.GrhIndex = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + obj, "GrhIndex")));
if (withBlock.GrhIndex == 0) 
    withBlock.GrhIndex = withBlock.GrhIndex;  // ← NOP inútil
```

**Problema:** Si GrhIndex es 0, lo asigna a sí mismo (no hace nada)

**Fix propuesto:**
```csharp
// Opción A: Eliminar el if completo (probablemente era código de debug)
withBlock.GrhIndex = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + obj, "GrhIndex")));

// Opción B: Asignar un default si es 0
if (withBlock.GrhIndex == 0) 
    withBlock.GrhIndex = 1; // o algún valor default sensible
```

**Estado:** ⬜ Pendiente revisión  
**Riesgo:** 🟡 MEDIO - Depende si GrhIndex=0 es válido o no

---

#### Caso 5: ⬜ `FileIO.cs:2253`
```csharp
// CÓDIGO ACTUAL (líneas 2248-2254)
withBlock.LogOnTime = DateTime.Now;
withBlock.UpTime = withBlock.UpTime + Math.Abs(TempDate.Day - 30) * 24 * 3600 +
                   Thread.CurrentThread.CurrentCulture.Calendar.GetHour(TempDate) * 3600 +
                   Thread.CurrentThread.CurrentCulture.Calendar.GetMinute(TempDate) * 60 +
                   Thread.CurrentThread.CurrentCulture.Calendar.GetSecond(TempDate);
withBlock.UpTime = withBlock.UpTime;  // ← NOP inútil
WriteVar(UserFile, "INIT", "UpTime", withBlock.UpTime.ToString());
```

**Problema:** Después de calcular UpTime, lo asigna a sí mismo (redundante)

**Fix propuesto:**
```csharp
// Eliminar la línea redundante
withBlock.UpTime = withBlock.UpTime + Math.Abs(TempDate.Day - 30) * 24 * 3600 + ...;
WriteVar(UserFile, "INIT", "UpTime", withBlock.UpTime.ToString());
```

**Estado:** ⬜ Pendiente revisión  
**Riesgo:** 🟢 BAJO - Es solo una línea redundante

---

### ⬜ Tarea 2.2: CS0652 - Comparación Fuera de Rango (2 casos)

⚠️ **REQUIERE CORRECCIÓN - BUG DE TIPO**

#### Caso 1: ⬜ `Trabajo.cs:2047`
```csharp
// CÓDIGO ACTUAL
// UserSkills es byte[] (valores 0-255)
RobarSkill = withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Robar];

if ((RobarSkill <= 10) & (RobarSkill >= -1))  // ← BUG: byte nunca puede ser -1
    Suerte = 35;
else if ((RobarSkill <= 20) & (RobarSkill >= 11))
    Suerte = 30;
// ... etc
```

**Problema:** Un `byte` nunca puede ser negativo, la comparación `>= -1` siempre es `true`

**Fix propuesto:**
```csharp
if (RobarSkill <= 10)
    Suerte = 35;
else if (RobarSkill <= 20)
    Suerte = 30;
// ... etc
```

**Estado:** ⬜ Pendiente aplicación  
**Riesgo:** 🟢 BAJO - Solo elimina comparación inútil, no cambia lógica

---

#### Caso 2: ⬜ `Trabajo.cs:2702`
```csharp
// CÓDIGO ACTUAL
MeditarSkill = withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Meditar];

if ((MeditarSkill <= 10) & (MeditarSkill >= -1))  // ← BUG: byte nunca puede ser -1
    Suerte = 35;
// ... etc
```

**Problema:** Mismo que el caso anterior

**Fix propuesto:**
```csharp
if (MeditarSkill <= 10)
    Suerte = 35;
// ... etc
```

**Estado:** ⬜ Pendiente aplicación  
**Riesgo:** 🟢 BAJO - Solo elimina comparación inútil

---

### ⬜ Tarea 2.3: CS0162 - Código Inalcanzable (6 casos)

⚠️ **REQUIERE ANÁLISIS - POSIBLE LÓGICA INCORRECTA**

#### Caso 1: ⬜ `clsParty.cs:136`
```csharp
// CÓDIGO ACTUAL (líneas 130-143)
if (Distance(Declaraciones.UserList[UI].Pos.X, 
             Declaraciones.UserList[UI].Pos.Y, X, Y) <= mdParty.PARTY_MAXDISTANCIA)
{
    p_members[i].Experiencia = p_members[i].Experiencia + expThisUser;
    if (p_members[i].Experiencia < 0d) 
        p_members[i].Experiencia = 0d;
    
    if (mdParty.PARTY_EXPERIENCIAPORGOLPE)
    {
        Declaraciones.UserList[UI].Stats.Exp = 
            Declaraciones.UserList[UI].Stats.Exp + Conversion.Fix(expThisUser);
        if (Declaraciones.UserList[UI].Stats.Exp > Declaraciones.MAXEXP)
            Declaraciones.UserList[UI].Stats.Exp = Declaraciones.MAXEXP;
        UsUaRiOs.CheckUserLevel(UI);  // ← Marcado como unreachable
        Protocol.WriteUpdateUserStats(UI);
    }
}
```

**Análisis necesario:** Necesito ver si hay algo que causa early exit antes de CheckUserLevel

**Estado:** ⬜ Pendiente investigación  
**Riesgo:** 🟡 MEDIO

---

#### Caso 2: ⬜ `TCP.cs:285`
```csharp
// CÓDIGO ACTUAL (líneas 281-287)
for (LoopC = 1; LoopC <= Declaraciones.NUMSKILLS; LoopC++)
{
    if (Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] < 0)
    {
        return ValidateSkillsRet;  // ← Sale de la función
        if (Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] > 100)  // ← UNREACHABLE
            Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] = 100;
    }
}
```

**Problema:** El código después del `return` nunca se ejecuta

**Fix propuesto:**
```csharp
// Opción A: Mover la validación > 100 fuera del if
for (LoopC = 1; LoopC <= Declaraciones.NUMSKILLS; LoopC++)
{
    if (Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] < 0)
        return ValidateSkillsRet;
    
    if (Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] > 100)
        Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] = 100;
}

// Opción B: Cambiar a else if
for (LoopC = 1; LoopC <= Declaraciones.NUMSKILLS; LoopC++)
{
    if (Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] < 0)
        return ValidateSkillsRet;
    else if (Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] > 100)
        Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] = 100;
}
```

**Nota:** De todas formas, `UserSkills` es `byte[]`, nunca puede ser < 0, este if es inútil (relacionado con CS0652)

**Estado:** ⬜ Pendiente revisión  
**Riesgo:** ⚠️ ALTO - Validación de skills que nunca se ejecuta

---

#### Caso 3: ⬜ `clsClan.cs:921` (y líneas siguientes hasta 927)
```csharp
// CÓDIGO ACTUAL (líneas 918-935)
ES.WriteVar(PROPUESTASFILE, OtroGuild.ToString(), "Detalle", Constants.vbNullString);
ES.WriteVar(PROPUESTASFILE, OtroGuild.ToString(), "Pendiente", "0");

var loopTo = Convert.ToInt16(p_PropuestasDePaz.Count - 1);
for (i = 0; i <= loopTo; i++)
{
    if ((p_PropuestasDePaz[i] ?? "") == (OtroGuild.ToString() ?? ""))
        p_PropuestasDePaz.Remove(p_PropuestasDePaz[i]);
    return;  // ← Sale del método en la PRIMERA iteración siempre
}

var loopTo1 = Convert.ToInt16(p_PropuestasDeAlianza.Count - 1);  // ← UNREACHABLE
for (i = 0; i <= loopTo1; i++)  // ← UNREACHABLE
{
    if ((p_PropuestasDeAlianza[i] ?? "") == (OtroGuild.ToString() ?? ""))
        p_PropuestasDePaz.Remove(p_PropuestasDeAlianza[i]);
    return;  // ← UNREACHABLE
}
```

**Problema:** El `return` está DENTRO del loop sin condición, siempre sale en la primera iteración

**Fix propuesto:**
```csharp
// El return probablemente debería estar después del if, o usar break
for (i = 0; i <= loopTo; i++)
{
    if ((p_PropuestasDePaz[i] ?? "") == (OtroGuild.ToString() ?? ""))
    {
        p_PropuestasDePaz.Remove(p_PropuestasDePaz[i]);
        break;  // Sale del loop, no del método
    }
}

for (i = 0; i <= loopTo1; i++)
{
    if ((p_PropuestasDePaz[i] ?? "") == (OtroGuild.ToString() ?? ""))
    {
        p_PropuestasDePaz.Remove(p_PropuestasDePaz[i]);
        break;
    }
}
```

**Estado:** ⬜ Pendiente revisión  
**Riesgo:** ⚠️ ALTO - Cambia comportamiento de eliminación de propuestas

---

#### Caso 4: ⬜ `clsClan.cs:930` 
**Nota:** Ya cubierto en Caso 3 (mismo problema, segundo loop)

---

#### Caso 5: ⬜ `modHechizos.cs:1533`
**Estado:** ⬜ Pendiente investigación - necesito ver el código

---

#### Caso 6: ⬜ `praetorians.cs:2414`
**Estado:** ⬜ Pendiente investigación - necesito ver el código

---

### ⬜ Tarea 2.4: CS0649 - Fields No Asignados (Requieren Inicialización - 4 casos)

**Nota:** Estos fields SÍ se leen en alguna parte del código, pero nunca se asignan, por lo que siempre tienen el valor default.

Requieren análisis para determinar:
1. ¿Se pueden eliminar?
2. ¿Necesitan inicialización con un valor sensible?
3. ¿Son bugs de lógica (deberían asignarse en alguna parte)?

**Lista de candidatos a revisar:**
- ⬜ `Admin.cs:31-33` - Intervalos (IntervaloLanzaHechizo, IntervaloNPCPuedeAtacar, IntervaloNPCAI)
- ⬜ `Declares.cs:*` - Varios campos globales
- ⬜ Otros según análisis de referencias

**Estado:** ⬜ Pendiente análisis detallado de uso

---

### ⬜ Tarea 2.5: CA1416 - Platform-Specific API (1 caso)

#### ⬜ `Modulo_UsUaRiOs.cs:2490`

```csharp
// CÓDIGO ACTUAL (líneas 2486-2491)
if (File.Exists(Declaraciones.CharPath + ViejoNick + ".chr"))
{
    // hace un backup del char
    ViejoCharBackup = Declaraciones.CharPath + ViejoNick + ".chr.old-";
    FileSystem.Rename(Declaraciones.CharPath + ViejoNick + ".chr", ViejoCharBackup);
}
```

**Problema:** `Microsoft.VisualBasic.FileSystem.Rename` solo funciona en Windows

**Fix propuesto:**
```csharp
if (File.Exists(Declaraciones.CharPath + ViejoNick + ".chr"))
{
    // hace un backup del char
    ViejoCharBackup = Declaraciones.CharPath + ViejoNick + ".chr.old-";
    File.Move(
        Declaraciones.CharPath + ViejoNick + ".chr", 
        ViejoCharBackup, 
        overwrite: false
    );
}
```

**Estado:** ⬜ Pendiente aplicación  
**Riesgo:** 🟢 BAJO - Reemplazo directo 1:1

---

## 📁 ESTRUCTURA DE ARCHIVOS A CREAR

```
argentum-server-net/
├── docs/
│   ├── WARNINGS_CLEANUP_PLAN.md          ← Este archivo
│   └── WARNINGS_CLEANUP_REPORT.md        ← Generado por script (Fase 1)
├── Script/
│   ├── WarningsFixer.csx                 ← Script principal Roslyn
│   ├── Analyzers/
│   │   ├── UnusedVariablesFixer.cs       ← CS0168 + CS0219
│   │   ├── UnusedFieldsFixer.cs          ← CS0169 + CS0649
│   │   ├── TypeNamingFixer.cs            ← CS8981
│   │   ├── PlatformApiFixer.cs           ← CA1416
│   │   └── CodeAnalysisUtils.cs          ← Helpers
│   └── Script.csproj                     ← Actualizar con dependencias
```

---

## 🚀 ORDEN DE EJECUCIÓN

### Paso 1: ✅ Crear este documento de plan
```bash
# Ya está creado en: docs/WARNINGS_CLEANUP_PLAN.md
```

### Paso 2: ⬜ Implementar Script Roslyn (Fase 1)
```bash
cd Script
# Crear archivos del script
# Configurar dependencias
```

### Paso 3: ⬜ Ejecutar Fase 1 Automática
```bash
cd Script
dotnet script WarningsFixer.csx
```

**Output esperado:**
```
=== Roslyn Warnings Fixer ===
Cargando solución...
✓ Solución cargada: 3 proyectos

Analizando proyecto: Legacy
✓ CS0168: 59 variables no usadas eliminadas
  - 12 catch blocks actualizados con Console.WriteLine
✓ CS0219: 5 variables eliminadas
✓ CS0169: 2 fields eliminados
✓ CS0649: 15 fields eliminados (verificados sin referencias)
✓ CS8981: Tipo 'npc' renombrado a 'Npc' (47 referencias actualizadas)

Total warnings corregidas: 82/100

Guardando cambios...
✓ Cambios aplicados en Legacy/Source/

Reporte generado: docs/WARNINGS_CLEANUP_REPORT.md
```

### Paso 4: ⬜ Verificar compilación post-Fase 1
```bash
dotnet clean
dotnet build 2>&1 | tee build-after-phase1.log
```

**Warnings esperadas restantes:** 18

### Paso 5: ⬜ Revisar casos de Fase 2 (Manual)

**Orden de revisión:**

1. ⬜ **CS0652 (2 casos)** - Los más simples, solo quitar comparación inútil
2. ⬜ **CA1416 (1 caso)** - Reemplazo directo de API
3. ⬜ **CS1717 casos simples (3 casos)** - Modulo_UsUaRiOs.cs:2131, FileIO.cs:839, FileIO.cs:2253
4. ⬜ **CS0162 (6 casos)** - Requieren análisis de lógica
5. ⬜ **CS1717 casos complejos (2 casos)** - modGuilds.cs:470, ModAreas.cs:443
6. ⬜ **CS0649 para inicializar (4 casos)** - Requieren análisis de uso

### Paso 6: ⬜ Aplicar correcciones manuales
- Ir archivo por archivo
- Aplicar fixes propuestos
- Verificar compilación después de cada cambio crítico

### Paso 7: ⬜ Verificación final
```bash
dotnet clean
dotnet build

# Debe retornar: 0 Warning(s)
```

### Paso 8: ⬜ Testing manual
- Ejecutar el server
- Verificar que no hay crashes obvios
- Probar funcionalidades básicas

### Paso 9: ⬜ Commit
```bash
git add .
git commit -m "fix: resolve all 100 compiler warnings

- Automated fixes for unused variables/fields (82 warnings)
- Manual fixes for logic issues (18 warnings)
- Replaced platform-specific APIs with cross-platform alternatives
- Added exception logging in catch blocks

Closes #XX"
```

---

## 📊 PROGRESO TRACKING

| Fase | Tarea | Warnings | Estado | Fecha |
|------|-------|----------|--------|-------|
| Setup | Crear plan | - | ✅ | 2026-03-29 |
| 1 | Implementar script | - | ⬜ | |
| 1 | CS0168 | 59 | ⬜ | |
| 1 | CS0219 | 5 | ⬜ | |
| 1 | CS0169 | 2 | ⬜ | |
| 1 | CS0649 (eliminar) | 15 | ⬜ | |
| 1 | CS8981 | 1 | ⬜ | |
| 1 | **Subtotal Fase 1** | **82** | ⬜ | |
| 2 | CS0652 | 2 | ⬜ | |
| 2 | CA1416 | 1 | ⬜ | |
| 2 | CS1717 (simples) | 3 | ⬜ | |
| 2 | CS1717 (complejos) | 2 | ⬜ | |
| 2 | CS0162 | 6 | ⬜ | |
| 2 | CS0649 (inicializar) | 4 | ⬜ | |
| 2 | **Subtotal Fase 2** | **18** | ⬜ | |
| - | **TOTAL** | **100** | ⬜ | |

---

## ⚠️ CASOS DE ALTO RIESGO (Requieren extra atención)

### 🔴 Críticos (pueden romper funcionalidad)

1. **modGuilds.cs:470** - Loop de codex con inicialización incorrecta
2. **ModAreas.cs:443** - Loop de conexión de áreas con LoopC sin inicializar
3. **clsClan.cs:921** - Return dentro de loop de propuestas de paz
4. **TCP.cs:285** - Validación de skills que nunca se ejecuta

### 🟡 Moderados (pueden afectar comportamiento)

5. **FileIO.cs:839** - GrhIndex con verificación sospechosa
6. **clsParty.cs:136** - Código marcado como unreachable en party XP

### 🟢 Bajos (código redundante/inútil)

7. Resto de casos (eliminación de código muerto, comparaciones inútiles, etc.)

---

## 📝 NOTAS ADICIONALES

### Dependencias del Script Roslyn

Agregar a `Script/Script.csproj`:
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.8.0" />
  <PackageReference Include="Microsoft.Build.Locator" Version="1.6.1" />
</ItemGroup>
```

### Variables de Configuración

```csharp
// En el script
const bool DRY_RUN = false;           // true = solo reporta, no modifica
const bool VERBOSE = true;            // logs detallados
const bool BACKUP_BEFORE_CHANGE = true; // crea .bak de cada archivo
```

### Logging en Catch Blocks

Formato estándar para todos los catch:
```csharp
catch (Exception ex)
{
    Console.WriteLine($"Exception in [ClassName.MethodName]: {ex.Message}");
    // código original del catch
}
```

---

## ✅ CRITERIOS DE ÉXITO

- [ ] 100 warnings corregidas
- [ ] 0 warnings en build final
- [ ] Proyecto compila sin errores
- [ ] Server arranca sin crashes
- [ ] Todas las transformaciones documentadas en REPORT.md
- [ ] Código comprometido en git con mensaje descriptivo

---

**Última actualización:** 2026-03-29  
**Autor:** Franco (con asistencia de Claude Code)  
**Estado:** 📋 Plan creado, pendiente de ejecución
