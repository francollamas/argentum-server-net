'Codigo by Loopzer todos los derechos se le reservan a él!
'GSZone 2010
Option Explicit On
Option Strict Off

Imports System.IO
Imports System.Text.Json

Module CargaMapa
    Public Structure TXY
        Public x As Short
        Public y As Short
    End Structure

    Public Structure TXYTriger
        Public XY As TXY
        Public Numero As Short
    End Structure

    Public Structure TXYG1
        Public XY As TXY
        Public Numero As Short
    End Structure

    Public Structure TXYG2
        Public XY As TXY
        Public Numero As Short
    End Structure

    Public Structure TXYG3
        Public XY As TXY
        Public Numero As Short
    End Structure

    Public Structure TXYG4
        Public XY As TXY
        Public Numero As Short
    End Structure

    Public Structure TXYObj
        Public XY As TXY
        Public mobj As Obj
    End Structure

    Public Structure TXYNpc
        Public XY As TXY
        Public Numero As Short
    End Structure

    Public Structure TXYSalir
        Public XY As TXY
        Public Salida As WorldPos
    End Structure

    Public Sub CargarMapa(ind As Short, MAPFl As String)
        Dim CountObj As Short
        Dim CTriger As Short
        Dim CG1 As Short
        Dim CG2 As Short
        Dim CG3 As Short
        Dim CG4 As Short
        Dim CNpc As Short
        Dim CSalir As Short
        Dim CBlk As Short

        Dim Mocha_Obj() As TXYObj
        Dim Mocha_Triger() As TXYTriger
        Dim Mocha_CG1() As TXYG1
        Dim Mocha_CG2() As TXYG2
        Dim Mocha_CG3() As TXYG3
        Dim Mocha_CG4() As TXYG4
        Dim Mocha_Npc() As TXYNpc
        Dim Mocha_Salir() As TXYSalir
        Dim Mocha_BLK() As TXY

        Using _
            fileStream As _
                New IO.FileStream(IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps", ind & ".map"),
                                  IO.FileMode.Open)
            Using reader As New IO.BinaryReader(fileStream)
                CountObj = reader.ReadInt16()
                CTriger = reader.ReadInt16()
                CG1 = reader.ReadInt16()
                CG2 = reader.ReadInt16()
                CG3 = reader.ReadInt16()
                CG4 = reader.ReadInt16()
                CNpc = reader.ReadInt16()
                CSalir = reader.ReadInt16()
                CBlk = reader.ReadInt16()

                ReDim Mocha_Obj(CountObj - 1)
                ReDim Mocha_Triger(CTriger - 1)
                ReDim Mocha_CG1(CG1 - 1)
                ReDim Mocha_CG2(CG2 - 1)
                ReDim Mocha_CG3(CG3 - 1)
                ReDim Mocha_CG4(CG4 - 1)
                ReDim Mocha_Npc(CNpc - 1)
                ReDim Mocha_Salir(CSalir - 1)
                ReDim Mocha_BLK(CBlk - 1)

                For i = 0 To CountObj - 1
                    Mocha_Obj(i).XY.x = reader.ReadInt16()
                    Mocha_Obj(i).XY.y = reader.ReadInt16()
                    Mocha_Obj(i).mobj.ObjIndex = reader.ReadInt16()
                    Mocha_Obj(i).mobj.Amount = reader.ReadInt16()
                Next

                For i = 0 To CTriger - 1
                    Mocha_Triger(i).XY.x = reader.ReadInt16()
                    Mocha_Triger(i).XY.y = reader.ReadInt16()
                    Mocha_Triger(i).Numero = reader.ReadInt16()
                Next

                For i = 0 To CG1 - 1
                    Mocha_CG1(i).XY.x = reader.ReadInt16()
                    Mocha_CG1(i).XY.y = reader.ReadInt16()
                    Mocha_CG1(i).Numero = reader.ReadInt16()
                Next

                For i = 0 To CG2 - 1
                    Mocha_CG2(i).XY.x = reader.ReadInt16()
                    Mocha_CG2(i).XY.y = reader.ReadInt16()
                    Mocha_CG2(i).Numero = reader.ReadInt16()
                Next

                For i = 0 To CG3 - 1
                    Mocha_CG3(i).XY.x = reader.ReadInt16()
                    Mocha_CG3(i).XY.y = reader.ReadInt16()
                    Mocha_CG3(i).Numero = reader.ReadInt16()
                Next

                For i = 0 To CG4 - 1
                    Mocha_CG4(i).XY.x = reader.ReadInt16()
                    Mocha_CG4(i).XY.y = reader.ReadInt16()
                    Mocha_CG4(i).Numero = reader.ReadInt16()
                Next

                For i = 0 To CNpc - 1
                    Mocha_Npc(i).XY.x = reader.ReadInt16()
                    Mocha_Npc(i).XY.y = reader.ReadInt16()
                    Mocha_Npc(i).Numero = reader.ReadInt16()
                Next

                For i = 0 To CSalir - 1
                    Mocha_Salir(i).XY.x = reader.ReadInt16()
                    Mocha_Salir(i).XY.y = reader.ReadInt16()
                    Mocha_Salir(i).Salida.Map = reader.ReadInt16()
                    Mocha_Salir(i).Salida.X = reader.ReadInt16()
                    Mocha_Salir(i).Salida.Y = reader.ReadInt16()
                Next

                For i = 0 To CBlk - 1
                    Mocha_BLK(i).x = reader.ReadInt16()
                    Mocha_BLK(i).y = reader.ReadInt16()
                Next
            End Using
        End Using

        Dim t As Short
        Dim tm As TXYObj
        Dim tt As TXYTriger
        Dim tg1 As TXYG1
        Dim tg2 As TXYG2
        Dim tg3 As TXYG3
        Dim tg4 As TXYG4
        Dim tnpc As TXYNpc
        Dim tsalir As TXYSalir
        Dim tblk As TXY

        For t = 0 To CountObj - 1
            tm = Mocha_Obj(t)
            MapData(ind, tm.XY.x, tm.XY.y).OBJInfo = tm.mobj
        Next

        For t = 0 To CTriger - 1
            tt = Mocha_Triger(t)
            MapData(ind, tt.XY.x, tt.XY.y).trigger = CType(tt.Numero, eTrigger)
        Next

        For t = 0 To CG1 - 1
            tg1 = Mocha_CG1(t)
            MapData(ind, tg1.XY.x, tg1.XY.y).Graphic(1) = tg1.Numero
        Next

        For t = 0 To CG2 - 1
            tg2 = Mocha_CG2(t)
            MapData(ind, tg2.XY.x, tg2.XY.y).Graphic(2) = tg2.Numero
        Next

        For t = 0 To CG3 - 1
            tg3 = Mocha_CG3(t)
            MapData(ind, tg3.XY.x, tg3.XY.y).Graphic(3) = tg3.Numero
        Next

        For t = 0 To CG4 - 1
            tg4 = Mocha_CG4(t)
            MapData(ind, tg4.XY.x, tg4.XY.y).Graphic(4) = tg4.Numero
        Next

        Dim x As Short
        Dim y As Short
        Dim npcfile As String

        For t = 0 To CNpc - 1
            tnpc = Mocha_Npc(t)
            x = tnpc.XY.x
            y = tnpc.XY.y
            MapData(ind, x, y).NpcIndex = tnpc.Numero

            npcfile = DatPath & "NPCs.dat"

            'Si el npc debe hacer respawn en la pos
            'original la guardamos

            If CInt(LeerNPCs.GetValue("NPC" & MapData(ind, x, y).NpcIndex, "PosOrig")) = 1 Then
                'If Val(GetVar(npcfile, "NPC" & MapData(ind, x, y).NpcIndex, "PosOrig")) = 1 Then
                MapData(ind, x, y).NpcIndex = OpenNPC(MapData(ind, x, y).NpcIndex)
                Npclist(MapData(ind, x, y).NpcIndex).Orig.Map = ind
                Npclist(MapData(ind, x, y).NpcIndex).Orig.x = x
                Npclist(MapData(ind, x, y).NpcIndex).Orig.y = y
            Else
                MapData(ind, x, y).NpcIndex = OpenNPC(MapData(ind, x, y).NpcIndex)
            End If

            Npclist(MapData(ind, x, y).NpcIndex).Pos.Map = ind
            Npclist(MapData(ind, x, y).NpcIndex).Pos.x = x
            Npclist(MapData(ind, x, y).NpcIndex).Pos.y = y

            Call MakeNPCChar(True, 0, MapData(ind, x, y).NpcIndex, ind, x, y)
        Next

        For t = 0 To CSalir - 1
            tsalir = Mocha_Salir(t)
            MapData(ind, tsalir.XY.x, tsalir.XY.y).TileExit = tsalir.Salida
        Next

        For t = 0 To CBlk - 1
            tblk = Mocha_BLK(t)
            MapData(ind, tblk.x, tblk.y).Blocked = 1
        Next

        Dim jsonString As String = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps",
                                                                 ind & ".json"))
        Dim jsonDoc As JsonDocument = JsonDocument.Parse(jsonString)
        Dim root As JsonElement = jsonDoc.RootElement

        With MapInfo_Renamed(ind)
            Dim tempProperty As JsonElement
            If root.TryGetProperty("name", tempProperty) Then .name = tempProperty.GetString()
            If root.TryGetProperty("musicnum", tempProperty) Then .Music = tempProperty.GetInt32()
            If root.TryGetProperty("magiasinefecto", tempProperty) Then .MagiaSinEfecto = tempProperty.GetByte()
            If root.TryGetProperty("invisinefecto", tempProperty) Then .InviSinEfecto = tempProperty.GetByte()
            If root.TryGetProperty("resusinefecto", tempProperty) Then .ResuSinEfecto = tempProperty.GetByte()
            If root.TryGetProperty("noencriptarmp", tempProperty) Then .NoEncriptarMP = tempProperty.GetByte()
            If root.TryGetProperty("robonpc", tempProperty) Then .RoboNpcsPermitido = tempProperty.GetByte()
            If root.TryGetProperty("pk", tempProperty) Then .Pk = tempProperty.GetByte() = 1
            If root.TryGetProperty("terreno", tempProperty) Then .Terreno = tempProperty.GetString()
            If root.TryGetProperty("zona", tempProperty) Then .Zona = tempProperty.GetString()
            If root.TryGetProperty("restringir", tempProperty) Then .Restringir = tempProperty.GetString()
            If root.TryGetProperty("backup", tempProperty) Then .BackUp = tempProperty.GetByte()
        End With
    End Sub
End Module