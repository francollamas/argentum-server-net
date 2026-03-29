using System;
using System.Drawing;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy;

internal static class modCentinela
{
    private const short NPC_CENTINELA_TIERRA = 16; // Índice del NPC en el .dat
    private const short NPC_CENTINELA_AGUA = 16; // Ídem anterior, pero en mapas de agua

    private const byte TIEMPO_INICIAL = 2;

    public static short CentinelaNPCIndex; // Índice del NPC en el servidor

    public static bool centinelaActivado;

    public static tCentinela Centinela;

    public static void CallUserAttention()
    {
        // ############################################################
        // Makes noise and FX to call the user's attention.
        // ############################################################
        if (Migration.GetTickCount() - Centinela.spawnTime >= 5000L)
            if ((Centinela.RevisandoUserIndex != 0) & centinelaActivado)
                if (!Declaraciones.UserList[Centinela.RevisandoUserIndex].flags.CentinelaOK)
                {
                    Protocol.WritePlayWave(Centinela.RevisandoUserIndex, Declaraciones.SND_WARP,
                        Convert.ToByte(Declaraciones.Npclist[CentinelaNPCIndex].Pos.X),
                        Convert.ToByte(Declaraciones.Npclist[CentinelaNPCIndex].Pos.Y));
                    Protocol.WriteCreateFX(Centinela.RevisandoUserIndex,
                        Declaraciones.Npclist[CentinelaNPCIndex].character.CharIndex,
                        (short)Declaraciones.FXIDs.FXWARP, 0);

                    // Resend the key
                    CentinelaSendClave(Centinela.RevisandoUserIndex);

                    Protocol.FlushBuffer(Centinela.RevisandoUserIndex);
                }
    }

    private static void GoToNextWorkingChar()
    {
        // ############################################################
        // Va al siguiente usuario que se encuentre trabajando
        // ############################################################
        int LoopC;

        var loopTo = (int)Declaraciones.LastUser;
        for (LoopC = 1; LoopC <= loopTo; LoopC++)
            if (Declaraciones.UserList[LoopC].flags.UserLogged &&
                Declaraciones.UserList[LoopC].Counters.Trabajando > 0 &&
                (Declaraciones.UserList[LoopC].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                if (!Declaraciones.UserList[LoopC].flags.CentinelaOK)
                {
                    // Inicializamos
                    Centinela.RevisandoUserIndex = Convert.ToInt16(LoopC);
                    Centinela.TiempoRestante = TIEMPO_INICIAL;
                    Centinela.clave = Convert.ToInt16(Matematicas.RandomNumber(1, 32000));
                    Centinela.spawnTime = Convert.ToInt32(Migration.GetTickCount());

                    // Ponemos al centinela en posición
                    WarpCentinela(Convert.ToInt16(LoopC));

                    if (CentinelaNPCIndex != 0)
                    {
                        // Mandamos el mensaje (el centinela habla y aparece en consola para que no haya dudas)
                        Protocol.WriteChatOverHead(Convert.ToInt16(LoopC),
                            "Saludos " + Declaraciones.UserList[LoopC].name +
                            ", soy el Centinela de estas tierras. Me gustaría que escribas /CENTINELA " +
                            Centinela.clave + " en no más de dos minutos.",
                            Convert.ToInt16(Declaraciones.Npclist[CentinelaNPCIndex].character.CharIndex),
                            ColorTranslator.ToOle(Color.Lime));
                        Protocol.WriteConsoleMsg(Convert.ToInt16(LoopC),
                            "El centinela intenta llamar tu atención. ¡Respóndele rápido!",
                            Protocol.FontTypeNames.FONTTYPE_CENTINELA);
                        Protocol.FlushBuffer(Convert.ToInt16(LoopC));
                    }

                    return;
                }

        // No hay chars trabajando, eliminamos el NPC si todavía estaba en algún lado y esperamos otro minuto
        if (CentinelaNPCIndex != 0)
        {
            NPCs.QuitarNPC(CentinelaNPCIndex);
            CentinelaNPCIndex = 0;
        }

        // No estamos revisando a nadie
        Centinela.RevisandoUserIndex = 0;
    }

    private static void CentinelaFinalCheck()
    {
        // ############################################################
        // Al finalizar el tiempo, se retira y realiza la acción
        // pertinente dependiendo del caso
        // ############################################################
        try
        {
            string name;
            short numPenas;

            short Index;
            if (!Declaraciones.UserList[Centinela.RevisandoUserIndex].flags.CentinelaOK)
            {
                // Logueamos el evento
                LogCentinela("Centinela baneo a " + Declaraciones.UserList[Centinela.RevisandoUserIndex].name +
                             " por uso de macro inasistido.");

                // Ponemos el ban
                Declaraciones.UserList[Centinela.RevisandoUserIndex].flags.Ban = 1;

                name = Declaraciones.UserList[Centinela.RevisandoUserIndex].name;

                // Avisamos a los admins
                modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                    Protocol.PrepareMessageConsoleMsg("Servidor> El centinela ha baneado a " + name,
                        Protocol.FontTypeNames.FONTTYPE_SERVER));

                // ponemos el flag de ban a 1
                ES.WriteVar(Declaraciones.CharPath + name + ".chr", "FLAGS", "Ban", "1");
                // ponemos la pena
                var argEmptySpaces = 1024;
                numPenas = Convert.ToInt16(Migration.ParseVal(ES.GetVar(Declaraciones.CharPath + name + ".chr", "PENAS",
                    "Cant", ref argEmptySpaces)));
                ES.WriteVar(Declaraciones.CharPath + name + ".chr", "PENAS", "Cant", (numPenas + 1).ToString());
                ES.WriteVar(Declaraciones.CharPath + name + ".chr", "PENAS", "P" + (numPenas + 1),
                    "CENTINELA : BAN POR MACRO INASISTIDO " + DateTime.Today.ToString() + " " +
                    DateTime.Now.TimeOfDay.ToString());

                // Evitamos loguear el logout
                Index = Centinela.RevisandoUserIndex;
                Centinela.RevisandoUserIndex = 0;

                TCP.CloseSocket(Index);
            }

            Centinela.clave = 0;
            Centinela.TiempoRestante = 0;
            Centinela.RevisandoUserIndex = 0;

            if (CentinelaNPCIndex != 0)
            {
                NPCs.QuitarNPC(CentinelaNPCIndex);
                CentinelaNPCIndex = 0;
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in CallUserAttention: " + ex.Message);
            Centinela.clave = 0;
            Centinela.TiempoRestante = 0;
            Centinela.RevisandoUserIndex = 0;

            if (CentinelaNPCIndex != 0)
            {
                NPCs.QuitarNPC(CentinelaNPCIndex);
                CentinelaNPCIndex = 0;
            }

            var argdesc = "Error en el checkeo del centinela: " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void CentinelaCheckClave(short UserIndex, short clave)
    {
        // ############################################################
        // Corrobora la clave que le envia el usuario
        // ############################################################
        if ((clave == Centinela.clave) & (UserIndex == Centinela.RevisandoUserIndex))
        {
            Declaraciones.UserList[Centinela.RevisandoUserIndex].flags.CentinelaOK = true;
            Protocol.WriteChatOverHead(UserIndex,
                "¡Muchas gracias " + Declaraciones.UserList[Centinela.RevisandoUserIndex].name +
                "! Espero no haber sido una molestia.",
                Convert.ToInt16(Declaraciones.Npclist[CentinelaNPCIndex].character.CharIndex),
                ColorTranslator.ToOle(Color.White));
            Centinela.RevisandoUserIndex = 0;
            Protocol.FlushBuffer(UserIndex);
        }
        else
        {
            CentinelaSendClave(UserIndex);

            // Logueamos el evento
            if (UserIndex != Centinela.RevisandoUserIndex)
                LogCentinela("El usuario " + Declaraciones.UserList[UserIndex].name +
                             " respondió aunque no se le hablaba a él.");
            else
                LogCentinela("El usuario " + Declaraciones.UserList[UserIndex].name +
                             " respondió una clave incorrecta: " + clave + " - Se esperaba : " + Centinela.clave);
        }
    }

    public static void ResetCentinelaInfo()
    {
        // ############################################################
        // Cada determinada cantidad de tiempo, volvemos a revisar
        // ############################################################
        int LoopC;

        var loopTo = (int)Declaraciones.LastUser;
        for (LoopC = 1; LoopC <= loopTo; LoopC++)
            if ((Migration.migr_LenB(Declaraciones.UserList[LoopC].name) != 0) &
                (LoopC != Centinela.RevisandoUserIndex))
                Declaraciones.UserList[LoopC].flags.CentinelaOK = false;
    }

    public static void CentinelaSendClave(short UserIndex)
    {
        // ############################################################
        // Enviamos al usuario la clave vía el personaje centinela
        // ############################################################
        if (CentinelaNPCIndex == 0)
            return;

        if (UserIndex == Centinela.RevisandoUserIndex)
        {
            if (!Declaraciones.UserList[UserIndex].flags.CentinelaOK)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "¡La clave que te he dicho es /CENTINELA " + Centinela.clave + ", escríbelo rápido!",
                    Convert.ToInt16(Declaraciones.Npclist[CentinelaNPCIndex].character.CharIndex),
                    ColorTranslator.ToOle(Color.Lime));
                Protocol.WriteConsoleMsg(UserIndex, "El centinela intenta llamar tu atención. ¡Respondele rápido!",
                    Protocol.FontTypeNames.FONTTYPE_CENTINELA);
            }
            else
            {
                // Logueamos el evento
                LogCentinela("El usuario " + Declaraciones.UserList[Centinela.RevisandoUserIndex].name +
                             " respondió más de una vez la contraseña correcta.");
                Protocol.WriteChatOverHead(UserIndex, "Te agradezco, pero ya me has respondido. Me retiraré pronto.",
                    Convert.ToInt16(Declaraciones.Npclist[CentinelaNPCIndex].character.CharIndex),
                    ColorTranslator.ToOle(Color.Lime));
            }
        }
        else
        {
            Protocol.WriteChatOverHead(UserIndex, "No es a ti a quien estoy hablando, ¿No ves?",
                Convert.ToInt16(Declaraciones.Npclist[CentinelaNPCIndex].character.CharIndex),
                ColorTranslator.ToOle(Color.White));
        }
    }

    public static void PasarMinutoCentinela()
    {
        // ############################################################
        // Control del timer. Llamado cada un minuto.
        // ############################################################
        if (!centinelaActivado)
            return;

        if (Centinela.RevisandoUserIndex == 0)
        {
            GoToNextWorkingChar();
        }
        else
        {
            Centinela.TiempoRestante = Convert.ToInt16(Centinela.TiempoRestante - 1);

            if (Centinela.TiempoRestante == 0)
            {
                CentinelaFinalCheck();
                GoToNextWorkingChar();
            }
            else
            {
                // Recordamos al user que debe escribir
                if (Matematicas.Distancia(ref Declaraciones.Npclist[CentinelaNPCIndex].Pos,
                        ref Declaraciones.UserList[Centinela.RevisandoUserIndex].Pos) >
                    5) WarpCentinela(Centinela.RevisandoUserIndex);

                // El centinela habla y se manda a consola para que no quepan dudas
                Protocol.WriteChatOverHead(Centinela.RevisandoUserIndex,
                    "¡" + Declaraciones.UserList[Centinela.RevisandoUserIndex].name +
                    ", tienes un minuto más para responder! Debes escribir /CENTINELA " + Centinela.clave + ".",
                    Convert.ToInt16(Declaraciones.Npclist[CentinelaNPCIndex].character.CharIndex),
                    ColorTranslator.ToOle(Color.Red));
                Protocol.WriteConsoleMsg(Centinela.RevisandoUserIndex,
                    "¡" + Declaraciones.UserList[Centinela.RevisandoUserIndex].name +
                    ", tienes un minuto más para responder!", Protocol.FontTypeNames.FONTTYPE_CENTINELA);
                Protocol.FlushBuffer(Centinela.RevisandoUserIndex);
            }
        }
    }

    private static void WarpCentinela(short UserIndex)
    {
        // ############################################################
        // Inciamos la revisión del usuario UserIndex
        // ############################################################
        // Evitamos conflictos de índices
        if (CentinelaNPCIndex != 0)
        {
            NPCs.QuitarNPC(CentinelaNPCIndex);
            CentinelaNPCIndex = 0;
        }

        if (General.HayAgua(Declaraciones.UserList[UserIndex].Pos.Map, Declaraciones.UserList[UserIndex].Pos.X,
                Declaraciones.UserList[UserIndex].Pos.Y))
            CentinelaNPCIndex =
                NPCs.SpawnNpc(NPC_CENTINELA_AGUA, ref Declaraciones.UserList[UserIndex].Pos, true, false);
        else
            CentinelaNPCIndex =
                NPCs.SpawnNpc(NPC_CENTINELA_TIERRA, ref Declaraciones.UserList[UserIndex].Pos, true, false);

        // Si no pudimos crear el NPC, seguimos esperando a poder hacerlo
        if (CentinelaNPCIndex == 0)
            Centinela.RevisandoUserIndex = 0;
    }

    public static void CentinelaUserLogout()
    {
        // ############################################################
        // El usuario al que revisabamos se desconectó
        // ############################################################
        if (Centinela.RevisandoUserIndex != 0)
        {
            // Logueamos el evento
            LogCentinela("El usuario " + Declaraciones.UserList[Centinela.RevisandoUserIndex].name +
                         " se desolgueó al pedirsele la contraseña.");

            // Reseteamos y esperamos a otro PasarMinuto para ir al siguiente user
            Centinela.clave = 0;
            Centinela.TiempoRestante = 0;
            Centinela.RevisandoUserIndex = 0;

            if (CentinelaNPCIndex != 0)
            {
                NPCs.QuitarNPC(CentinelaNPCIndex);
                CentinelaNPCIndex = 0;
            }
        }
    }

    private static void LogCentinela(string texto)
    {
        // *************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last modified: 03/15/2006
        // Loguea un evento del centinela
        // *************************************************
        try
        {
            General.AppendLog("logs/Centinela.log",
                DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + texto);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in CentinelaCheckClave: " + ex.Message);
        }
    }
    // Tiempo inicial en minutos. No reducir sin antes revisar el timer que maneja estos datos.

    public struct tCentinela
    {
        public short RevisandoUserIndex; // ¿Qué índice revisamos?
        public short TiempoRestante; // ¿Cuántos minutos le quedan al usuario?
        public short clave; // Clave que debe escribir
        public int spawnTime;
    }
}