VERSION 5.00
Object = "{248DD890-BB45-11CF-9ABC-0080C7E7B78D}#1.0#0"; "MSWINSCK.OCX"
Begin VB.Form frmMain 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Argentum Online"
   ClientHeight    =   4845
   ClientLeft      =   1950
   ClientTop       =   1815
   ClientWidth     =   5190
   FillColor       =   &H00C0C0C0&
   BeginProperty Font 
      Name            =   "Arial"
      Size            =   8.25
      Charset         =   0
      Weight          =   700
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   ForeColor       =   &H80000004&
   Icon            =   "frmMain.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   PaletteMode     =   1  'UseZOrder
   ScaleHeight     =   4845
   ScaleWidth      =   5190
   StartUpPosition =   2  'CenterScreen
   Begin MSWinsockLib.Winsock Winsock1 
      Index           =   0
      Left            =   480
      Top             =   60
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin VB.TextBox txtChat 
      Height          =   2775
      Left            =   120
      MultiLine       =   -1  'True
      TabIndex        =   10
      Top             =   1920
      Width           =   4935
   End
   Begin VB.Timer tPiqueteC 
      Enabled         =   0   'False
      Interval        =   6000
      Left            =   480
      Top             =   540
   End
   Begin VB.Timer packetResend 
      Interval        =   10
      Left            =   960
      Top             =   60
   End
   Begin VB.CheckBox SUPERLOG 
      Caption         =   "log"
      Height          =   255
      Left            =   3120
      TabIndex        =   9
      Top             =   480
      Width           =   615
   End
   Begin VB.CommandButton CMDDUMP 
      Caption         =   "dump"
      Height          =   255
      Left            =   3720
      TabIndex        =   8
      Top             =   480
      Width           =   1215
   End
   Begin VB.Timer FX 
      Enabled         =   0   'False
      Interval        =   4000
      Left            =   1440
      Top             =   540
   End
   Begin VB.Timer Auditoria 
      Enabled         =   0   'False
      Interval        =   1000
      Left            =   1440
      Top             =   1020
   End
   Begin VB.Timer GameTimer 
      Enabled         =   0   'False
      Interval        =   40
      Left            =   1440
      Top             =   60
   End
   Begin VB.Timer tLluviaEvent 
      Enabled         =   0   'False
      Interval        =   60000
      Left            =   960
      Top             =   1020
   End
   Begin VB.Timer tLluvia 
      Enabled         =   0   'False
      Interval        =   500
      Left            =   960
      Top             =   540
   End
   Begin VB.Timer AutoSave 
      Enabled         =   0   'False
      Interval        =   60000
      Left            =   480
      Top             =   1020
   End
   Begin VB.Timer npcataca 
      Enabled         =   0   'False
      Interval        =   4000
      Left            =   1920
      Top             =   1020
   End
   Begin VB.Timer KillLog 
      Enabled         =   0   'False
      Interval        =   60000
      Left            =   1920
      Top             =   60
   End
   Begin VB.Timer TIMER_AI 
      Enabled         =   0   'False
      Interval        =   100
      Left            =   1935
      Top             =   540
   End
   Begin VB.Frame Frame1 
      Caption         =   "BroadCast"
      Height          =   1095
      Left            =   120
      TabIndex        =   2
      Top             =   600
      Width           =   4935
      Begin VB.CommandButton Command2 
         Caption         =   "Broadcast consola"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   2520
         TabIndex        =   6
         Top             =   720
         Width           =   2295
      End
      Begin VB.CommandButton Command1 
         Caption         =   "Broadcast clientes"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   120
         TabIndex        =   5
         Top             =   720
         Width           =   2295
      End
      Begin VB.TextBox BroadMsg 
         Height          =   315
         Left            =   1080
         TabIndex        =   4
         Top             =   240
         Width           =   3735
      End
      Begin VB.Label Label1 
         Caption         =   "Mensaje"
         Height          =   255
         Index           =   0
         Left            =   120
         TabIndex        =   3
         Top             =   240
         Width           =   855
      End
   End
   Begin VB.Label Escuch 
      Caption         =   "Label2"
      Height          =   255
      Left            =   3240
      TabIndex        =   7
      Top             =   240
      Width           =   1335
   End
   Begin VB.Label CantUsuarios 
      Appearance      =   0  'Flat
      AutoSize        =   -1  'True
      BackColor       =   &H00C0C0C0&
      BackStyle       =   0  'Transparent
      Caption         =   "Numero de usuarios:"
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   195
      Left            =   120
      TabIndex        =   1
      Top             =   240
      Width           =   1725
   End
   Begin VB.Label txStatus 
      AutoSize        =   -1  'True
      BackStyle       =   0  'Transparent
      ForeColor       =   &H000000FF&
      Height          =   210
      Left            =   120
      TabIndex        =   0
      Top             =   5520
      Width           =   45
   End
   Begin VB.Menu mnuControles 
      Caption         =   "Argentum"
      Begin VB.Menu mnuServidor 
         Caption         =   "Configuracion"
      End
      Begin VB.Menu mnuCerrar 
         Caption         =   "Cerrar Servidor"
      End
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'Argentum Online 0.12.2
'Copyright (C) 2002 M�rquez Pablo Ignacio
'
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
'
'Argentum Online is based on Baronsoft's VB6 Online RPG
'You can contact the original creator of ORE at aaron@baronsoft.com
'for more information about ORE please visit http://www.baronsoft.com/
'
'
'You can contact me at:
'morgolock@speedy.com.ar
'www.geocities.com/gmorgolock
'Calle 3 n�mero 983 piso 7 dto A
'La Plata - Pcia, Buenos Aires - Republica Argentina
'C�digo Postal 1900
'Pablo Ignacio M�rquez

Option Explicit

Public ESCUCHADAS As Long
   
Const NIM_ADD = 0
Const NIM_DELETE = 2
Const NIF_MESSAGE = 1
Const NIF_ICON = 2
Const NIF_TIP = 4

Const WM_MOUSEMOVE = &H200
Const WM_LBUTTONDBLCLK = &H203
Const WM_RBUTTONUP = &H205

Sub CheckIdleUser()
    Dim iUserIndex As Long
    
    For iUserIndex = 1 To MaxUsers
        With UserList(iUserIndex)
            'Conexion activa? y es un usuario loggeado?
            If .ConnID <> -1 And .flags.UserLogged Then
                'Actualiza el contador de inactividad
                If .flags.Traveling = 0 Then
                    .Counters.IdleCount = .Counters.IdleCount + 1
                End If
                
                If .Counters.IdleCount >= IdleLimit Then
                    Call WriteShowMessageBox(iUserIndex, "Demasiado tiempo inactivo. Has sido desconectado.")
                    'mato los comercios seguros
                    If .ComUsu.DestUsu > 0 Then
                        If UserList(.ComUsu.DestUsu).flags.UserLogged Then
                            If UserList(.ComUsu.DestUsu).ComUsu.DestUsu = iUserIndex Then
                                Call WriteConsoleMsg(.ComUsu.DestUsu, "Comercio cancelado por el otro usuario.", FontTypeNames.FONTTYPE_TALK)
                                Call FinComerciarUsu(.ComUsu.DestUsu)
                                Call FlushBuffer(.ComUsu.DestUsu) 'flush the buffer to send the message right away
                            End If
                        End If
                        Call FinComerciarUsu(iUserIndex)
                    End If
                    Call Cerrar_Usuario(iUserIndex)
                End If
            End If
        End With
    Next iUserIndex
End Sub

Private Sub Auditoria_Timer()
On Error GoTo errhand
Static centinelSecs As Byte

centinelSecs = centinelSecs + 1

If centinelSecs = 5 Then
    'Every 5 seconds, we try to call the player's attention so it will report the code.
    Call modCentinela.CallUserAttention
    
    centinelSecs = 0
End If

Call PasarSegundo 'sistema de desconexion de 10 segs

Call ActualizaEstadisticasWeb

Exit Sub

errhand:

Call LogError("Error en Timer Auditoria. Err: " & Err.Description & " - " & Err.Number)
Resume Next

End Sub

Private Sub AutoSave_Timer()

On Error GoTo Errhandler
'fired every minute
Static Minutos As Long
Static MinutosLatsClean As Long
Static MinsPjesSave As Long

Dim i As Integer
Dim num As Long

Minutos = Minutos + 1

'�?�?�?�?�?�?�?�?�?�?�
Call ModAreas.AreasOptimizacion
'�?�?�?�?�?�?�?�?�?�?�

'Actualizamos el centinela
Call modCentinela.PasarMinutoCentinela

If Minutos = MinutosWs - 1 Then
    Call SendData(SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Worldsave en 1 minuto ...", FontTypeNames.FONTTYPE_VENENO))
End If

If Minutos >= MinutosWs Then
    Call ES.DoBackUp
    Call aClon.VaciarColeccion
    Minutos = 0
End If

If MinutosLatsClean >= 15 Then
    MinutosLatsClean = 0
    Call ReSpawnOrigPosNpcs 'respawn de los guardias en las pos originales
    Call LimpiarMundo
Else
    MinutosLatsClean = MinutosLatsClean + 1
End If

Call PurgarPenas
Call CheckIdleUser

'<<<<<-------- Log the number of users online ------>>>
Dim N As Integer
N = FreeFile()
Open App.Path & "\logs\numusers.log" For Output Shared As N
Print #N, NumUsers
Close #N
'<<<<<-------- Log the number of users online ------>>>

Exit Sub
Errhandler:
    Call LogError("Error en TimerAutoSave " & Err.Number & ": " & Err.Description)
    Resume Next
End Sub

Private Sub CMDDUMP_Click()
On Error Resume Next

Dim i As Integer
For i = 1 To MaxUsers
    Call LogCriticEvent(i & ") ConnID: " & UserList(i).ConnID & ". ConnidValida: " & UserList(i).ConnIDValida & " Name: " & UserList(i).name & " UserLogged: " & UserList(i).flags.UserLogged)
Next i

Call LogCriticEvent("Lastuser: " & LastUser & " NextOpenUser: " & NextOpenUser)

End Sub

Private Sub Command1_Click()
Call SendData(SendTarget.ToAll, 0, PrepareMessageShowMessageBox(BroadMsg.Text))
''''''''''''''''SOLO PARA EL TESTEO'''''''
''''''''''SE USA PARA COMUNICARSE CON EL SERVER'''''''''''
txtChat.Text = txtChat.Text & vbNewLine & "Servidor> " & BroadMsg.Text
End Sub

Private Sub Command2_Click()
Call SendData(SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Servidor> " & BroadMsg.Text, FontTypeNames.FONTTYPE_SERVER))
''''''''''''''''SOLO PARA EL TESTEO'''''''
''''''''''SE USA PARA COMUNICARSE CON EL SERVER'''''''''''
txtChat.Text = txtChat.Text & vbNewLine & "Servidor> " & BroadMsg.Text
End Sub

Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
If Not salir Then
    Cancel = True
End If
End Sub

Private Sub Form_Unload(Cancel As Integer)
On Error Resume Next

'Save stats!!!
Call Statistics.DumpStatistics

Call LimpiaWsApi

Dim LoopC As Integer

For LoopC = 1 To MaxUsers
    If UserList(LoopC).ConnID <> -1 Then Call CloseSocket(LoopC)
Next

'Log
Dim N As Integer
N = FreeFile
Open App.Path & "\logs\Main.log" For Append Shared As #N
Print #N, Date & " " & time & " server cerrado."
Close #N

End

Set SonidosMapas = Nothing

End Sub

Private Sub FX_Timer()
On Error GoTo hayerror

Call SonidosMapas.ReproducirSonidosDeMapas

Exit Sub
hayerror:

End Sub

Private Sub GameTimer_Timer()
'********************************************************
'Author: Unknown
'Last Modify Date: -
'********************************************************
    Dim iUserIndex As Long
    Dim bEnviarStats As Boolean
    Dim bEnviarAyS As Boolean
    
On Error GoTo hayerror
    
    '<<<<<< Procesa eventos de los usuarios >>>>>>
    For iUserIndex = 1 To MaxUsers 'LastUser
        With UserList(iUserIndex)
           'Conexion activa?
           If .ConnID <> -1 Then
                '�User valido?
                
                If .ConnIDValida And .flags.UserLogged Then
                    
                    '[Alejo-18-5]
                    bEnviarStats = False
                    bEnviarAyS = False
                    
                    Call DoTileEvents(iUserIndex, .Pos.Map, .Pos.X, .Pos.Y)
                    
                    
                    If .flags.Paralizado = 1 Then Call EfectoParalisisUser(iUserIndex)
                    If .flags.Ceguera = 1 Or .flags.Estupidez Then Call EfectoCegueEstu(iUserIndex)
                    
                    
                    If .flags.Muerto = 0 Then
                        
                        '[Consejeros]
                        If (.flags.Privilegios And PlayerType.User) Then Call EfectoLava(iUserIndex)
                        
                        If .flags.Desnudo <> 0 And (.flags.Privilegios And PlayerType.User) <> 0 Then Call EfectoFrio(iUserIndex)
                        
                        If .flags.Meditando Then Call DoMeditar(iUserIndex)
                        
                        If .flags.Envenenado <> 0 And (.flags.Privilegios And PlayerType.User) <> 0 Then Call EfectoVeneno(iUserIndex)
                        
                        If .flags.AdminInvisible <> 1 Then
                            If .flags.invisible = 1 Then Call EfectoInvisibilidad(iUserIndex)
                            If .flags.Oculto = 1 Then Call DoPermanecerOculto(iUserIndex)
                        End If
                        
                        If .flags.Mimetizado = 1 Then Call EfectoMimetismo(iUserIndex)
                        
                        If .flags.AtacablePor > 0 Then Call EfectoEstadoAtacable(iUserIndex)
                        
                        Call DuracionPociones(iUserIndex)
                        
                        Call HambreYSed(iUserIndex, bEnviarAyS)
                        
                        If .flags.Hambre = 0 And .flags.Sed = 0 Then
                            If Lloviendo Then
                                If Not Intemperie(iUserIndex) Then
                                    If Not .flags.Descansar Then
                                    'No esta descansando
                                        Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloSinDescansar)
                                        If bEnviarStats Then
                                            Call WriteUpdateHP(iUserIndex)
                                            bEnviarStats = False
                                        End If
                                        Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloSinDescansar)
                                        If bEnviarStats Then
                                            Call WriteUpdateSta(iUserIndex)
                                            bEnviarStats = False
                                        End If
                                    Else
                                    'esta descansando
                                        Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloDescansar)
                                        If bEnviarStats Then
                                            Call WriteUpdateHP(iUserIndex)
                                            bEnviarStats = False
                                        End If
                                        Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloDescansar)
                                        If bEnviarStats Then
                                            Call WriteUpdateSta(iUserIndex)
                                            bEnviarStats = False
                                        End If
                                        'termina de descansar automaticamente
                                        If .Stats.MaxHp = .Stats.MinHp And .Stats.MaxSta = .Stats.MinSta Then
                                            Call WriteRestOK(iUserIndex)
                                            Call WriteConsoleMsg(iUserIndex, "Has terminado de descansar.", FontTypeNames.FONTTYPE_INFO)
                                            .flags.Descansar = False
                                        End If
                                        
                                    End If
                                End If
                            Else
                                If Not .flags.Descansar Then
                                'No esta descansando
                                    
                                    Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloSinDescansar)
                                    If bEnviarStats Then
                                        Call WriteUpdateHP(iUserIndex)
                                        bEnviarStats = False
                                    End If
                                    Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloSinDescansar)
                                    If bEnviarStats Then
                                        Call WriteUpdateSta(iUserIndex)
                                        bEnviarStats = False
                                    End If
                                    
                                Else
                                'esta descansando
                                    
                                    Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloDescansar)
                                    If bEnviarStats Then
                                        Call WriteUpdateHP(iUserIndex)
                                        bEnviarStats = False
                                    End If
                                    Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloDescansar)
                                    If bEnviarStats Then
                                        Call WriteUpdateSta(iUserIndex)
                                        bEnviarStats = False
                                    End If
                                    'termina de descansar automaticamente
                                    If .Stats.MaxHp = .Stats.MinHp And .Stats.MaxSta = .Stats.MinSta Then
                                        Call WriteRestOK(iUserIndex)
                                        Call WriteConsoleMsg(iUserIndex, "Has terminado de descansar.", FontTypeNames.FONTTYPE_INFO)
                                        .flags.Descansar = False
                                    End If
                                    
                                End If
                            End If
                        End If
                        
                        If bEnviarAyS Then Call WriteUpdateHungerAndThirst(iUserIndex)
                        
                        If .NroMascotas > 0 Then Call TiempoInvocacion(iUserIndex)
                    End If 'Muerto
                Else 'no esta logeado?
                    'Inactive players will be removed!
                    .Counters.IdleCount = .Counters.IdleCount + 1
                    If .Counters.IdleCount > IntervaloParaConexion Then
                        .Counters.IdleCount = 0
                        Call CloseSocket(iUserIndex)
                    End If
                End If 'UserLogged
                
                'If there is anything to be sent, we send it
                Call FlushBuffer(iUserIndex)
            End If
        End With
    Next iUserIndex
Exit Sub

hayerror:
    LogError ("Error en GameTimer: " & Err.Description & " UserIndex = " & iUserIndex)
End Sub

Private Sub mnuCerrar_Click()
    Unload Me
End Sub

Private Function salir() As Boolean
If MsgBox("��Atencion!! Si cierra el servidor puede provocar la perdida de datos. �Desea hacerlo de todas maneras?", vbYesNo) = vbYes Then
    Dim f
    For Each f In Forms
        Unload f
    Next
    salir = True
    Exit Function
End If

salir = False
End Function

Private Sub mnusalir_Click()
    Unload Me
End Sub

Private Sub KillLog_Timer()
On Error Resume Next
If FileExist(App.Path & "\logs\connect.log") Then Kill App.Path & "\logs\connect.log"
If FileExist(App.Path & "\logs\haciendo.log") Then Kill App.Path & "\logs\haciendo.log"
If FileExist(App.Path & "\logs\stats.log") Then Kill App.Path & "\logs\stats.log"
If FileExist(App.Path & "\logs\Asesinatos.log") Then Kill App.Path & "\logs\Asesinatos.log"
If FileExist(App.Path & "\logs\HackAttemps.log") Then Kill App.Path & "\logs\HackAttemps.log"
If Not FileExist(App.Path & "\logs\nokillwsapi.txt") Then
    If FileExist(App.Path & "\logs\wsapi.log") Then Kill App.Path & "\logs\wsapi.log"
End If

End Sub

Private Sub mnuServidor_Click()
frmServidor.Visible = True
End Sub

Private Sub npcataca_Timer()

On Error Resume Next
Dim npc As Long

For npc = 1 To LastNPC
    Npclist(npc).CanAttack = 1
Next npc

End Sub

Private Sub packetResend_Timer()
'***************************************************
'Autor: Juan Mart�n Sotuyo Dodero (Maraxus)
'Last Modification: 04/01/07
'Attempts to resend to the user all data that may be enqueued.
'***************************************************
On Error GoTo Errhandler:
    Dim i As Long
    
    For i = 1 To MaxUsers
        If UserList(i).ConnIDValida Then
            If UserList(i).outgoingData.length > 0 Then
                Call EnviarDatosASlot(i, UserList(i).outgoingData.ReadASCIIStringFixed(UserList(i).outgoingData.length))
            End If
        End If
    Next i

Exit Sub

Errhandler:
    LogError ("Error en packetResend - Error: " & Err.Number & " - Desc: " & Err.Description)
    Resume Next
End Sub

Private Sub TIMER_AI_Timer()

On Error GoTo ErrorHandler
Dim NpcIndex As Long
Dim X As Integer
Dim Y As Integer
Dim UseAI As Integer
Dim mapa As Integer
Dim e_p As Integer

'Barrin 29/9/03
If Not haciendoBK And Not EnPausa Then
    'Update NPCs
    For NpcIndex = 1 To LastNPC
        
        With Npclist(NpcIndex)
            If .flags.NPCActive Then 'Nos aseguramos que sea INTELIGENTE!
            
                ' Chequea si contiua teniendo due�o
                If .Owner > 0 Then Call ValidarPermanenciaNpc(NpcIndex)
            
                If .flags.Paralizado = 1 Then
                    Call EfectoParalisisNpc(NpcIndex)
                Else
                    e_p = esPretoriano(NpcIndex)
                    If e_p > 0 Then
                        Select Case e_p
                            Case 1  ''clerigo
                                Call PRCLER_AI(NpcIndex)
                            Case 2  ''mago
                                Call PRMAGO_AI(NpcIndex)
                            Case 3  ''cazador
                                Call PRCAZA_AI(NpcIndex)
                            Case 4  ''rey
                                Call PRREY_AI(NpcIndex)
                            Case 5  ''guerre
                                Call PRGUER_AI(NpcIndex)
                        End Select
                    Else
                        'Usamos AI si hay algun user en el mapa
                        If .flags.Inmovilizado = 1 Then
                           Call EfectoParalisisNpc(NpcIndex)
                        End If
                        
                        mapa = .Pos.Map
                        
                        If mapa > 0 Then
                            If MapInfo(mapa).NumUsers > 0 Then
                                If .Movement <> TipoAI.ESTATICO Then
                                    Call NPCAI(NpcIndex)
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End With
    Next NpcIndex
End If

Exit Sub

ErrorHandler:
    Call LogError("Error en TIMER_AI_Timer " & Npclist(NpcIndex).name & " mapa:" & Npclist(NpcIndex).Pos.Map)
    Call MuereNpc(NpcIndex, 0)
End Sub

Private Sub tLluvia_Timer()
On Error GoTo Errhandler

Dim iCount As Long
If Lloviendo Then
   For iCount = 1 To LastUser
        Call EfectoLluvia(iCount)
   Next iCount
End If

Exit Sub
Errhandler:
Call LogError("tLluvia " & Err.Number & ": " & Err.Description)
End Sub

Private Sub tLluviaEvent_Timer()

On Error GoTo ErrorHandler
Static MinutosLloviendo As Long
Static MinutosSinLluvia As Long

If Not Lloviendo Then
    MinutosSinLluvia = MinutosSinLluvia + 1
    If MinutosSinLluvia >= 15 And MinutosSinLluvia < 1440 Then
        If RandomNumber(1, 100) <= 2 Then
            Lloviendo = True
            MinutosSinLluvia = 0
            Call SendData(SendTarget.ToAll, 0, PrepareMessageRainToggle())
        End If
    ElseIf MinutosSinLluvia >= 1440 Then
        Lloviendo = True
        MinutosSinLluvia = 0
        Call SendData(SendTarget.ToAll, 0, PrepareMessageRainToggle())
    End If
Else
    MinutosLloviendo = MinutosLloviendo + 1
    If MinutosLloviendo >= 5 Then
        Lloviendo = False
        Call SendData(SendTarget.ToAll, 0, PrepareMessageRainToggle())
        MinutosLloviendo = 0
    Else
        If RandomNumber(1, 100) <= 2 Then
            Lloviendo = False
            MinutosLloviendo = 0
            Call SendData(SendTarget.ToAll, 0, PrepareMessageRainToggle())
        End If
    End If
End If

Exit Sub
ErrorHandler:
Call LogError("Error tLluviaTimer")

End Sub

Private Sub tPiqueteC_Timer()
    Dim NuevaA As Boolean
   ' Dim NuevoL As Boolean
    Dim GI As Integer
    
    Dim i As Long
    
On Error GoTo Errhandler
    For i = 1 To LastUser
        With UserList(i)
            If .flags.UserLogged Then
                If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = eTrigger.ANTIPIQUETE Then
                    .Counters.PiqueteC = .Counters.PiqueteC + 1
                    Call WriteConsoleMsg(i, "���Est�s obstruyendo la v�a p�blica, mu�vete o ser�s encarcelado!!!", FontTypeNames.FONTTYPE_INFO)
                    
                    If .Counters.PiqueteC > 23 Then
                        .Counters.PiqueteC = 0
                        Call Encarcelar(i, TIEMPO_CARCEL_PIQUETE)
                    End If
                Else
                    .Counters.PiqueteC = 0
                End If
                
                If .flags.Muerto = 1 Then
                    If .flags.Traveling = 1 Then
                        If .Counters.goHome <= 0 Then
                            Call FindLegalPos(i, Ciudades(.Hogar).Map, Ciudades(.Hogar).X, Ciudades(.Hogar).Y)
                            Call WarpUserChar(i, Ciudades(.Hogar).Map, Ciudades(.Hogar).X, Ciudades(.Hogar).Y, True)
                            Call WriteMultiMessage(i, eMessages.FinishHome)
                            .flags.Traveling = 0
                        Else
                            .Counters.goHome = .Counters.goHome - 1
                        End If
                    End If
                End If
                
                'ustedes se preguntaran que hace esto aca?
                'bueno la respuesta es simple: el codigo de AO es una mierda y encontrar
                'todos los puntos en los cuales la alineacion puede cambiar es un dolor de
                'huevos, asi que lo controlo aca, cada 6 segundos, lo cual es razonable
        
                GI = .GuildIndex
                If GI > 0 Then
                    NuevaA = False
                   ' NuevoL = False
                    If Not modGuilds.m_ValidarPermanencia(i, True, NuevaA) Then
                        Call WriteConsoleMsg(i, "Has sido expulsado del clan. �El clan ha sumado un punto de antifacci�n!", FontTypeNames.FONTTYPE_GUILD)
                    End If
                    If NuevaA Then
                        Call SendData(SendTarget.ToGuildMembers, GI, PrepareMessageConsoleMsg("�El clan ha pasado a tener alineaci�n " & GuildAlignment(GI) & "!", FontTypeNames.FONTTYPE_GUILD))
                        Call LogClanes("�El clan cambio de alineaci�n!")
                    End If
'                    If NuevoL Then
'                        Call SendData(SendTarget.ToGuildMembers, GI, PrepareMessageConsoleMsg("�El clan tiene un nuevo l�der!", FontTypeNames.FONTTYPE_GUILD))
'                        Call LogClanes("�El clan tiene nuevo lider!")
'                    End If
                End If
                
                Call FlushBuffer(i)
            End If
        End With
    Next i
Exit Sub

Errhandler:
    Call LogError("Error en tPiqueteC_Timer " & Err.Number & ": " & Err.Description)
End Sub

Private Sub Winsock1_Close(Index As Integer)
    Winsock_Close Index
End Sub

Private Sub Winsock1_ConnectionRequest(Index As Integer, ByVal requestID As Long)
    Winsock_ConnectionRequest Index, requestID
End Sub

Private Sub Winsock1_DataArrival(Index As Integer, ByVal bytesTotal As Long)
    Winsock_DataArrival Index, bytesTotal
End Sub


