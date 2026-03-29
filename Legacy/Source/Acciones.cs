using System;

namespace Legacy;

internal static class Acciones
{
    // '
    // Modulo para manejar las acciones (doble click) de los carteles, foro, puerta, ramitas
    // 

    // '
    // Ejecuta la accion del doble click
    // 
    // @param UserIndex UserIndex
    // @param Map Numero de mapa
    // @param X X
    // @param Y Y

    public static void Accion(short UserIndex, short Map, short X, short Y)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short tempIndex;

        try
        {
            // ¿Rango Visión? (ToxicWaste)
            if ((Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.Y - Y)) > AI.RANGO_VISION_Y) |
                (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.X - X)) > AI.RANGO_VISION_X)) return;

            // ¿Posicion valida?
            if (Extra.InMapBounds(Map, X, Y))
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (Declaraciones.MapData[Map, X, Y].NpcIndex > 0) // Acciones NPCs
                {
                    tempIndex = Declaraciones.MapData[Map, X, Y].NpcIndex;

                    // Set the target NPC
                    withBlock.flags.TargetNPC = tempIndex;

                    if (Declaraciones.Npclist[tempIndex].Comercia == 1)
                    {
                        // ¿Esta el user muerto? Si es asi no puede comerciar
                        if (withBlock.flags.Muerto == 1)
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        // Is it already in commerce mode??
                        if (withBlock.flags.Comerciando) return;

                        if (Matematicas.Distancia(ref Declaraciones.Npclist[tempIndex].Pos, ref withBlock.Pos) > 3)
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        // Iniciamos la rutina pa' comerciar.
                        modSistemaComercio.IniciarComercioNPC(UserIndex);
                    }

                    else if (Declaraciones.Npclist[tempIndex].NPCtype == Declaraciones.eNPCType.Banquero)
                    {
                        // ¿Esta el user muerto? Si es asi no puede comerciar
                        if (withBlock.flags.Muerto == 1)
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        // Is it already in commerce mode??
                        if (withBlock.flags.Comerciando) return;

                        if (Matematicas.Distancia(ref Declaraciones.Npclist[tempIndex].Pos, ref withBlock.Pos) > 3)
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        // A depositar de una
                        modBanco.IniciarDeposito(UserIndex);
                    }
                    else if ((Declaraciones.Npclist[tempIndex].NPCtype == Declaraciones.eNPCType.Revividor) |
                             (Declaraciones.Npclist[tempIndex].NPCtype == Declaraciones.eNPCType.ResucitadorNewbie))
                    {
                        if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.Npclist[tempIndex].Pos) > 10)
                        {
                            Protocol.WriteConsoleMsg(UserIndex,
                                "El sacerdote no puede curarte debido a que estás demasiado lejos.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        // Revivimos si es necesario
                        if ((withBlock.flags.Muerto == 1) &
                            ((Declaraciones.Npclist[tempIndex].NPCtype == Declaraciones.eNPCType.Revividor) |
                             Extra.EsNewbie(UserIndex))) UsUaRiOs.RevivirUsuario(UserIndex);

                        if ((Declaraciones.Npclist[tempIndex].NPCtype == Declaraciones.eNPCType.Revividor) |
                            Extra.EsNewbie(UserIndex))
                        {
                            // curamos totalmente
                            withBlock.Stats.MinHp = withBlock.Stats.MaxHp;
                            Protocol.WriteUpdateUserStats(UserIndex);
                        }
                    }
                }

                // ¿Es un obj?
                else if (Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex > 0)
                {
                    tempIndex = Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex;

                    withBlock.flags.TargetObj = tempIndex;

                    switch (Declaraciones.objData[tempIndex].OBJType)
                    {
                        case Declaraciones.eOBJType.otPuertas: // Es una puerta
                        {
                            AccionParaPuerta(Map, X, Y, UserIndex);
                            break;
                        }
                        case Declaraciones.eOBJType.otCarteles: // Es un cartel
                        {
                            AccionParaCartel(Map, X, Y, UserIndex);
                            break;
                        }
                        case Declaraciones.eOBJType.otForos: // Foro
                        {
                            AccionParaForo(Map, X, Y, UserIndex);
                            break;
                        }
                        case Declaraciones.eOBJType.otLeña: // Leña
                        {
                            if ((tempIndex == Declaraciones.FOGATA_APAG) & (withBlock.flags.Muerto == 0))
                                AccionParaRamita(Map, X, Y, UserIndex);

                            break;
                        }
                    }
                }
                // >>>>>>>>>>>OBJETOS QUE OCUPAM MAS DE UN TILE<<<<<<<<<<<<<
                else if (Declaraciones.MapData[Map, X + 1, Y].ObjInfo.ObjIndex > 0)
                {
                    tempIndex = Declaraciones.MapData[Map, X + 1, Y].ObjInfo.ObjIndex;
                    withBlock.flags.TargetObj = tempIndex;

                    switch (Declaraciones.objData[tempIndex].OBJType)
                    {
                        case Declaraciones.eOBJType.otPuertas: // Es una puerta
                        {
                            AccionParaPuerta(Map, Convert.ToInt16(X + 1), Y, UserIndex);
                            break;
                        }
                    }
                }

                else if (Declaraciones.MapData[Map, X + 1, Y + 1].ObjInfo.ObjIndex > 0)
                {
                    tempIndex = Declaraciones.MapData[Map, X + 1, Y + 1].ObjInfo.ObjIndex;
                    withBlock.flags.TargetObj = tempIndex;

                    switch (Declaraciones.objData[tempIndex].OBJType)
                    {
                        case Declaraciones.eOBJType.otPuertas: // Es una puerta
                        {
                            AccionParaPuerta(Map, Convert.ToInt16(X + 1), Convert.ToInt16(Y + 1), UserIndex);
                            break;
                        }
                    }
                }

                else if (Declaraciones.MapData[Map, X, Y + 1].ObjInfo.ObjIndex > 0)
                {
                    tempIndex = Declaraciones.MapData[Map, X, Y + 1].ObjInfo.ObjIndex;
                    withBlock.flags.TargetObj = tempIndex;

                    switch (Declaraciones.objData[tempIndex].OBJType)
                    {
                        case Declaraciones.eOBJType.otPuertas: // Es una puerta
                        {
                            AccionParaPuerta(Map, X, Convert.ToInt16(Y + 1), UserIndex);
                            break;
                        }
                    }
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in Accion: " + ex.Message);
        }
    }

    public static void AccionParaForo(short Map, short X, short Y, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 02/01/2010
        // 02/01/2010: ZaMa - Agrego foros faccionarios
        // ***************************************************

        try
        {
            Declaraciones.WorldPos Pos;

            Pos.Map = Map;
            Pos.X = X;
            Pos.Y = Y;

            if (Matematicas.Distancia(ref Pos, ref Declaraciones.UserList[UserIndex].Pos) > 2)
            {
                Protocol.WriteConsoleMsg(UserIndex, "Estas demasiado lejos.", Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (modForum.SendPosts(UserIndex,
                    ref Declaraciones.objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex].ForoID))
                Protocol.WriteShowForumForm(UserIndex);
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in AccionParaForo: " + ex.Message);
        }
    }

    public static void AccionParaPuerta(short Map, short X, short Y, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            if (!(Matematicas.Distance(Declaraciones.UserList[UserIndex].Pos.X, Declaraciones.UserList[UserIndex].Pos.Y,
                    X, Y) > 2d))
            {
                if (Declaraciones.objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex].Llave == 0)
                {
                    if (Declaraciones.objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex].Cerrada == 1)
                    {
                        // Abre la puerta
                        if (Declaraciones.objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex].Llave == 0)
                        {
                            Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex = Declaraciones
                                .objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex].IndexAbierta;

                            modSendData.SendToAreaByPos(Map, X, Y,
                                Protocol.PrepareMessageObjectCreate(
                                    Declaraciones.objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex]
                                        .GrhIndex, Convert.ToByte(X), Convert.ToByte(Y)));

                            // Desbloquea
                            Declaraciones.MapData[Map, X, Y].Blocked = 0;
                            Declaraciones.MapData[Map, X - 1, Y].Blocked = 0;

                            // Bloquea todos los mapas
                            General.Bloquear(true, Map, X, Y, false);
                            General.Bloquear(true, Map, Convert.ToInt16(X - 1), Y, false);


                            // Sonido
                            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                Protocol.PrepareMessagePlayWave(Declaraciones.SND_PUERTA, Convert.ToByte(X),
                                    Convert.ToByte(Y)));
                        }

                        else
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "La puerta esta cerrada con llave.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                    else
                    {
                        // Cierra puerta
                        Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex = Declaraciones
                            .objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex].IndexCerrada;

                        modSendData.SendToAreaByPos(Map, X, Y,
                            Protocol.PrepareMessageObjectCreate(
                                Declaraciones.objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex]
                                    .GrhIndex, Convert.ToByte(X), Convert.ToByte(Y)));

                        Declaraciones.MapData[Map, X, Y].Blocked = 1;
                        Declaraciones.MapData[Map, X - 1, Y].Blocked = 1;


                        General.Bloquear(true, Map, Convert.ToInt16(X - 1), Y, true);
                        General.Bloquear(true, Map, X, Y, true);

                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                            Protocol.PrepareMessagePlayWave(Declaraciones.SND_PUERTA, Convert.ToByte(X),
                                Convert.ToByte(Y)));
                    }

                    Declaraciones.UserList[UserIndex].flags.TargetObj =
                        Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex;
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "La puerta está cerrada con llave.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in AccionParaPuerta: " + ex.Message);
        }
    }

    public static void AccionParaCartel(short Map, short X, short Y, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            if ((int)Declaraciones.objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex].OBJType == 8)
                if (Declaraciones.objData[Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex].texto.Length > 0)
                    Protocol.WriteShowSignal(UserIndex, Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex);
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in AccionParaCartel: " + ex.Message);
        }
    }

    public static void AccionParaRamita(short Map, short X, short Y, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            var Suerte = default(byte);
            byte exito;
            Declaraciones.Obj obj;

            Declaraciones.WorldPos Pos;
            Pos.Map = Map;
            Pos.X = X;
            Pos.Y = Y;

            var Fogatita = new Declaraciones.WorldPos();
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (Matematicas.Distancia(ref Pos, ref withBlock.Pos) > 2)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                if ((Declaraciones.MapData[Map, X, Y].trigger == Declaraciones.eTrigger.ZONASEGURA) |
                    !Declaraciones.mapInfo[Map].Pk)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes hacer fogatas en zona segura.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                if ((withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Supervivencia] > 1) &
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Supervivencia] < 6))
                    Suerte = 3;
                else if ((withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Supervivencia] >= 6) &
                         (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Supervivencia] <= 10))
                    Suerte = 2;
                else if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Supervivencia] >= 10 &&
                         withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Supervivencia] != 0) Suerte = 1;

                exito = Convert.ToByte(Matematicas.RandomNumber(1, Suerte));

                if (exito == 1)
                {
                    if ((Declaraciones.mapInfo[withBlock.Pos.Map].Zona ?? "") != Declaraciones.Ciudad)
                    {
                        obj.ObjIndex = Declaraciones.FOGATA;
                        obj.Amount = 1;

                        Protocol.WriteConsoleMsg(UserIndex, "Has prendido la fogata.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);

                        InvUsuario.MakeObj(ref obj, Map, X, Y);

                        // Las fogatas prendidas se deben eliminar
                        Fogatita.Map = Map;
                        Fogatita.X = X;
                        Fogatita.Y = Y;
                        Declaraciones.TrashCollector.Add(Fogatita);

                        UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Supervivencia, true);
                    }
                    else
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "La ley impide realizar fogatas en las ciudades.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    }
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No has podido hacer fuego.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Supervivencia, false);
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in AccionParaRamita: " + ex.Message);
        }
    }
}