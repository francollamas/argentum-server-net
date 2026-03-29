namespace Legacy;

internal class clsEstadisticasIPC
{
    // TODO MIGRA: se elimino esto ya que no hace al juego en si y usaba librerias del sistema para manejar ventanas. Ver si vale la pena completarlo nuevamente o directamente borrar todo el sistema

    // *************************************************
    public enum EstaNotificaciones
    {
        CANTIDAD_ONLINE = 1,
        RECORD_USUARIOS = 2,
        UPTIME_SERVER = 3,
        CANTIDAD_MAPAS = 4,
        EVENTO_NUEVO_CLAN = 5,
        HANDLE_WND_SERVER = 100
    }

    // *************************************************

    // UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    private int BuscaVentana(ref int Wnd, ref string str_Renamed)
    {
        int BuscaVentanaRet = default;

        BuscaVentanaRet = 0;
        return BuscaVentanaRet;
    }

    public int Informar(EstaNotificaciones QueCosa, int Parametro)
    {
        int InformarRet = default;

        InformarRet = 0;
        return InformarRet;
    }

    public bool EstadisticasAndando()
    {
        bool EstadisticasAndandoRet = default;

        EstadisticasAndandoRet = false;
        return EstadisticasAndandoRet;
    }

    public void Inicializa()
    {
    }

    private void BuscaWndEstadisticas()
    {
    }
}