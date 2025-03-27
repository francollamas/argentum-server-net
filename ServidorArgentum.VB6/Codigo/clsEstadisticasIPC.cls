VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsEstadisticasIPC"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' TODO MIGRA: se elimino esto ya que no hace al juego en si y usaba librerias del sistema para manejar ventanas. Ver si vale la pena completarlo nuevamente o directamente borrar todo el sistema
Option Explicit

'*************************************************
Public Enum EstaNotificaciones
    CANTIDAD_ONLINE = 1
    RECORD_USUARIOS = 2
    UPTIME_SERVER = 3
    CANTIDAD_MAPAS = 4
    EVENTO_NUEVO_CLAN = 5
    
    HANDLE_WND_SERVER = 100
End Enum

'*************************************************

Private Function BuscaVentana(Wnd As Long, str As String) As Long

BuscaVentana = 0

End Function

Public Function Informar(ByVal QueCosa As EstaNotificaciones, ByVal Parametro As Long) As Long

Informar = 0

End Function

Public Function EstadisticasAndando() As Boolean

EstadisticasAndando = False

End Function

Public Sub Inicializa(ByVal hWnd As Long)

End Sub

Private Sub BuscaWndEstadisticas()

End Sub
