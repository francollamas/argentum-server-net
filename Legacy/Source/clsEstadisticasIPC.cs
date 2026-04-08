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

    private int BuscaVentana(ref int Wnd, ref string str)
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