Option Strict Off
Option Explicit On
Friend Class clsEstadisticasIPC
    ' TODO MIGRA: se elimino esto ya que no hace al juego en si y usaba librerias del sistema para manejar ventanas. Ver si vale la pena completarlo nuevamente o directamente borrar todo el sistema

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

    'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Function BuscaVentana(ByRef Wnd As Integer, ByRef str_Renamed As String) As Integer

        BuscaVentana = 0
    End Function

    Public Function Informar(QueCosa As EstaNotificaciones, Parametro As Integer) As Integer

        Informar = 0
    End Function

    Public Function EstadisticasAndando() As Boolean

        EstadisticasAndando = False
    End Function

    Public Sub Inicializa()
    End Sub

    Private Sub BuscaWndEstadisticas()
    End Sub
End Class
