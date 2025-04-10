Option Strict Off
Option Explicit On
Friend Class ConsultasPopulares
    '**************************************************************
    ' ConsultasPopulares.cls
    '
    '**************************************************************

    '**************************************************************************
    'This program is free software; you can redistribute it and/or modify
    'it under the terms of the Affero General Public License;
    'either version 1 of the License, or any later version.
    '
    'This program is distributed in the hope that it will be useful,
    'but WITHOUT ANY WARRANTY; without even the implied warranty of
    'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    'Affero General Public License for more details.
    '
    'You should have received a copy of the Affero General Public License
    'along with this program; if not, you can find it at http://www.affero.org/oagpl.html
    '**************************************************************************

    'Modulo de consultas popluares
    'En realidad el modulo inicial guardaba los datos de las votaciones
    'en memoria, pero no le vi el punto, las votaciones son de vez en cuando nomas
    'y el query a un .ini que registra todos los mails q ya votaron, es soportable
    'si cuando uno vota y averiguo que el mail ya voto pero el PJ no, entonces seteo
    'el flag de yavoto en el charfile ese tambien,
    'por lo que la busqueda secuencial en el .dat que tiene todos los mails q ya votaron
    'se hara .. 1 vez por PJ nomas.
    '
    'Hecha por el oso

    Private Const ARCHIVOMAILS As String = "logs/votaron.dat"
    Private Const ARCHIVOCONFIG As String = "dat/consultas.dat"

    Private pEncuestaActualNum As Short
    Private pEncuestaActualTex As String
    Private pNivelRequerido As Short
    Private pOpciones() As Short


    Public Property Numero() As Short
        Get
            Numero = pEncuestaActualNum
        End Get
        Set(ByVal Value As Short)
            pEncuestaActualNum = Value
        End Set
    End Property


    Public Property texto() As String
        Get
            texto = pEncuestaActualTex
        End Get
        Set(ByVal Value As String)
            pEncuestaActualTex = Value
        End Set
    End Property


    Public Sub LoadData()
        Dim CantOpciones As Short
        Dim i As Short

        pEncuestaActualNum = Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & ARCHIVOCONFIG, "INIT", "ConsultaActual"))
        pEncuestaActualTex = GetVar(AppDomain.CurrentDomain.BaseDirectory & ARCHIVOCONFIG, "INIT", "ConsultaActualTexto")
        pNivelRequerido = CShort(GetVar(AppDomain.CurrentDomain.BaseDirectory & ARCHIVOCONFIG, "INIT", "NivelRequerido"))

        If pEncuestaActualNum > 0 Then
            'cargo todas las opciones
            CantOpciones = Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & ARCHIVOCONFIG,
                                      "ENCUESTA" & pEncuestaActualNum, "CANTOPCIONES"))
            'UPGRADE_WARNING: El límite inferior de la matriz pOpciones ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim pOpciones(CantOpciones)
            For i = 1 To CantOpciones
                pOpciones(i) = Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & ARCHIVOCONFIG,
                                          "ENCUESTA" & pEncuestaActualNum, "OPCION" & i))
            Next i
        End If
    End Sub

    Public Function doVotar(ByVal UserIndex As Short, ByVal opcion As Short) As String
        On Error GoTo errorh
        Dim YaVoto As Boolean
        Dim CharFile As String
        Dim sufragio As Short

        'revisar q no haya votado
        'grabar en el charfile el numero de encuesta
        'actualizar resultados encuesta
        If pEncuestaActualNum = 0 Then
            doVotar = "No hay consultas populares abiertas"
            Exit Function
        End If

        CharFile = CharPath & UserList(UserIndex).name & ".chr"


        If (UserList(UserIndex).Stats.ELV >= pNivelRequerido) Then
            If (OpcionValida(opcion)) Then
                YaVoto = Val(GetVar(CharFile, "CONSULTAS", "Voto")) >= pEncuestaActualNum
                If Not YaVoto Then
                    If Not MailYaVoto(UserList(UserIndex).email) Then
                        'pj apto para votar
                        sufragio = CInt(Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & ARCHIVOCONFIG,
                                                   "RESULTADOS" & pEncuestaActualNum, "V" & opcion)))
                        sufragio = sufragio + 1
                        Call _
                            WriteVar(AppDomain.CurrentDomain.BaseDirectory & ARCHIVOCONFIG,
                                     "RESULTADOS" & pEncuestaActualNum, "V" & opcion, Str(sufragio))
                        doVotar = "Tu voto ha sido computado. Opcion: " & opcion
                        Call MarcarPjComoQueYaVoto(UserIndex)
                        Call MarcarMailComoQueYaVoto(UserList(UserIndex).email)
                    Else
                        Call MarcarPjComoQueYaVoto(UserIndex)
                        doVotar = "Este email ya voto en la consulta: " & pEncuestaActualTex
                    End If
                Else
                    doVotar = "Este personaje ya voto en la consulta: " & pEncuestaActualTex
                End If
            Else
                doVotar = "Esa no es una opcion para votar"
            End If
        Else
            doVotar = "Para votar en esta consulta debes ser nivel " & pNivelRequerido & " o superior"
        End If


        Exit Function
        errorh:
        Call LogError("Error en ConsultasPopularse.doVotar: " & Err.Description)
    End Function


    Public Function SendInfoEncuesta(ByVal UserIndex As Short) As String
        Dim i As Short
        Call _
            WriteConsoleMsg(UserIndex, "CONSULTA POPULAR NUMERO " & pEncuestaActualNum,
                            Protocol.FontTypeNames.FONTTYPE_GUILD)
        Call WriteConsoleMsg(UserIndex, pEncuestaActualTex, Protocol.FontTypeNames.FONTTYPE_GUILD)
        Call WriteConsoleMsg(UserIndex, " Opciones de voto: ", Protocol.FontTypeNames.FONTTYPE_GUILDMSG)
        For i = 1 To UBound(pOpciones)
            Call _
                WriteConsoleMsg(UserIndex,
                                "(Opcion " & i & "): " &
                                GetVar(AppDomain.CurrentDomain.BaseDirectory & ARCHIVOCONFIG,
                                       "ENCUESTA" & pEncuestaActualNum, "OPCION" & i),
                                Protocol.FontTypeNames.FONTTYPE_GUILDMSG)
        Next i
        Call _
            WriteConsoleMsg(UserIndex,
                            " Para votar una opcion, escribe /encuesta NUMERODEOPCION, por ejemplo para votar la opcion 1, escribe /encuesta 1. Tu voto no podra ser cambiado.",
                            Protocol.FontTypeNames.FONTTYPE_VENENO)
    End Function


    Private Sub MarcarPjComoQueYaVoto(ByVal UserIndex As Short)
        Call WriteVar(CharPath & UserList(UserIndex).name & ".chr", "CONSULTAS", "Voto", Str(pEncuestaActualNum))
    End Sub


    Private Function MailYaVoto(ByVal email As String) As Boolean
        'abro el archivo, while not eof levnato 1 linea y comparo. Si da true, cierro
        Dim ArchN As Short
        Dim Tmp As String

        MailYaVoto = False

        ArchN = FreeFile

        FileOpen(ArchN, AppDomain.CurrentDomain.BaseDirectory & ARCHIVOMAILS, OpenMode.Input)

        Do While Not EOF(ArchN)
            Tmp = LineInput(ArchN)
            If email = Tmp Then
                MailYaVoto = True
                FileClose(ArchN)
                Exit Function
            End If
        Loop

        FileClose(ArchN)
    End Function


    Private Sub MarcarMailComoQueYaVoto(ByVal email As String)
        Dim ArchN As Short

        ArchN = FreeFile

        FileOpen(ArchN, AppDomain.CurrentDomain.BaseDirectory & ARCHIVOMAILS, OpenMode.Append)
        PrintLine(ArchN, email)

        FileClose(ArchN)
    End Sub


    Private Function OpcionValida(ByVal opcion As Short) As Boolean
        OpcionValida = opcion > 0 And opcion <= UBound(pOpciones)
    End Function
End Class
