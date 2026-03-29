using System;
using System.Drawing;

namespace Legacy;

internal static class modHechizos
{
    public const short SUPERANILLO = 700;

    public static void NpcLanzaSpellSobreUser(short NpcIndex, short UserIndex, short Spell)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 13/02/2009
        // 13/02/2009: ZaMa - Los npcs que tiren magias, no podran hacerlo en mapas donde no se permita usarla.
        // ***************************************************
        if (Declaraciones.Npclist[NpcIndex].CanAttack == 0)
            return;
        if ((Declaraciones.UserList[UserIndex].flags.invisible == 1) |
            (Declaraciones.UserList[UserIndex].flags.Oculto == 1))
            return;

        // Si no se peude usar magia en el mapa, no le deja hacerlo.
        if (Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].MagiaSinEfecto > 0)
            return;

        Declaraciones.Npclist[NpcIndex].CanAttack = 0;
        short daño;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (Declaraciones.Hechizos[Spell].SubeHP == 1)
            {
                daño = Convert.ToInt16(Matematicas.RandomNumber(Declaraciones.Hechizos[Spell].MinHp,
                    Declaraciones.Hechizos[Spell].MaxHp));
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[Spell].WAV),
                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessageCreateFX(withBlock.character.CharIndex,
                        Declaraciones.Hechizos[Spell].FXgrh, Declaraciones.Hechizos[Spell].loops));

                withBlock.Stats.MinHp = (short)(withBlock.Stats.MinHp + daño);
                if (withBlock.Stats.MinHp > withBlock.Stats.MaxHp)
                    withBlock.Stats.MinHp = withBlock.Stats.MaxHp;

                Protocol.WriteConsoleMsg(UserIndex,
                    Declaraciones.Npclist[NpcIndex].name + " te ha quitado " + daño + " puntos de vida.",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                Protocol.WriteUpdateUserStats(UserIndex);
            }

            else if (Declaraciones.Hechizos[Spell].SubeHP == 2)
            {
                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                {
                    daño = Convert.ToInt16(Matematicas.RandomNumber(Declaraciones.Hechizos[Spell].MinHp,
                        Declaraciones.Hechizos[Spell].MaxHp));

                    if (withBlock.Invent.CascoEqpObjIndex > 0)
                        daño = Convert.ToInt16(daño - Matematicas.RandomNumber(
                            Declaraciones.objData[withBlock.Invent.CascoEqpObjIndex].DefensaMagicaMin,
                            Declaraciones.objData[withBlock.Invent.CascoEqpObjIndex].DefensaMagicaMax));

                    if (withBlock.Invent.AnilloEqpObjIndex > 0)
                        daño = Convert.ToInt16(daño - Matematicas.RandomNumber(
                            Declaraciones.objData[withBlock.Invent.AnilloEqpObjIndex].DefensaMagicaMin,
                            Declaraciones.objData[withBlock.Invent.AnilloEqpObjIndex].DefensaMagicaMax));

                    if (daño < 0)
                        daño = 0;

                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[Spell].WAV),
                            Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessageCreateFX(withBlock.character.CharIndex,
                            Declaraciones.Hechizos[Spell].FXgrh, Declaraciones.Hechizos[Spell].loops));

                    withBlock.Stats.MinHp = (short)(withBlock.Stats.MinHp - daño);

                    Protocol.WriteConsoleMsg(UserIndex,
                        Declaraciones.Npclist[NpcIndex].name + " te ha quitado " + daño + " puntos de vida.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteUpdateUserStats(UserIndex);

                    // Muere
                    if (withBlock.Stats.MinHp < 1)
                    {
                        withBlock.Stats.MinHp = 0;
                        if (Declaraciones.Npclist[NpcIndex].NPCtype == Declaraciones.eNPCType.GuardiaReal)
                            SistemaCombate.RestarCriminalidad(UserIndex);
                        UsUaRiOs.UserDie(UserIndex);
                        // [Barrin 1-12-03]
                        if (Declaraciones.Npclist[NpcIndex].MaestroUser > 0)
                        {
                            // Store it!
                            Statistics.StoreFrag(Declaraciones.Npclist[NpcIndex].MaestroUser, UserIndex);

                            UsUaRiOs.ContarMuerte(UserIndex, Declaraciones.Npclist[NpcIndex].MaestroUser);
                            UsUaRiOs.ActStats(UserIndex, Declaraciones.Npclist[NpcIndex].MaestroUser);
                        }
                        // [/Barrin]
                    }
                }
            }

            if ((Declaraciones.Hechizos[Spell].Paraliza == 1) | (Declaraciones.Hechizos[Spell].Inmoviliza == 1))
                if (withBlock.flags.Paralizado == 0)
                {
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[Spell].WAV),
                            Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessageCreateFX(withBlock.character.CharIndex,
                            Declaraciones.Hechizos[Spell].FXgrh, Declaraciones.Hechizos[Spell].loops));

                    if (withBlock.Invent.AnilloEqpObjIndex == SUPERANILLO)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, " Tu anillo rechaza los efectos del hechizo.",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                        return;
                    }

                    if (Declaraciones.Hechizos[Spell].Inmoviliza == 1) withBlock.flags.Inmovilizado = 1;

                    withBlock.flags.Paralizado = 1;
                    withBlock.Counters.Paralisis = Admin.IntervaloParalizado;

                    Protocol.WriteParalizeOK(UserIndex);
                }

            if (Declaraciones.Hechizos[Spell].Estupidez == 1) // turbacion
                if (withBlock.flags.Estupidez == 0)
                {
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[Spell].WAV),
                            Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessageCreateFX(withBlock.character.CharIndex,
                            Declaraciones.Hechizos[Spell].FXgrh, Declaraciones.Hechizos[Spell].loops));

                    if (withBlock.Invent.AnilloEqpObjIndex == SUPERANILLO)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, " Tu anillo rechaza los efectos del hechizo.",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                        return;
                    }

                    withBlock.flags.Estupidez = 1;
                    withBlock.Counters.Ceguera = Admin.IntervaloInvisible;

                    Protocol.WriteDumb(UserIndex);
                }
        }
    }

    public static void NpcLanzaSpellSobreNpc(short NpcIndex, short TargetNPC, short Spell)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // solo hechizos ofensivos!

        if (Declaraciones.Npclist[NpcIndex].CanAttack == 0)
            return;
        Declaraciones.Npclist[NpcIndex].CanAttack = 0;

        short daño;

        if (Declaraciones.Hechizos[Spell].SubeHP == 2)
        {
            daño = Convert.ToInt16(Matematicas.RandomNumber(Declaraciones.Hechizos[Spell].MinHp,
                Declaraciones.Hechizos[Spell].MaxHp));
            modSendData.SendData(modSendData.SendTarget.ToNPCArea, TargetNPC,
                Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[Spell].WAV),
                    Convert.ToByte(Declaraciones.Npclist[TargetNPC].Pos.X),
                    Convert.ToByte(Declaraciones.Npclist[TargetNPC].Pos.Y)));
            modSendData.SendData(modSendData.SendTarget.ToNPCArea, TargetNPC,
                Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[TargetNPC].character.CharIndex,
                    Declaraciones.Hechizos[Spell].FXgrh, Declaraciones.Hechizos[Spell].loops));

            Declaraciones.Npclist[TargetNPC].Stats.MinHp = Declaraciones.Npclist[TargetNPC].Stats.MinHp - daño;

            // Muere
            if (Declaraciones.Npclist[TargetNPC].Stats.MinHp < 1)
            {
                Declaraciones.Npclist[TargetNPC].Stats.MinHp = 0;
                if (Declaraciones.Npclist[NpcIndex].MaestroUser > 0)
                    NPCs.MuereNpc(TargetNPC, Declaraciones.Npclist[NpcIndex].MaestroUser);
                else
                    NPCs.MuereNpc(TargetNPC, 0);
            }
        }
    }

    public static bool TieneHechizo(short i, short UserIndex)
    {
        bool TieneHechizoRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            short j;
            for (j = 1; j <= Declaraciones.MAXUSERHECHIZOS; j++)
                if (Declaraciones.UserList[UserIndex].Stats.UserHechizos[j] == i)
                {
                    TieneHechizoRet = true;
                    return TieneHechizoRet;
                }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in NpcLanzaSpellSobreUser: " + ex.Message);
        }

        return TieneHechizoRet;
    }

    public static void AgregarHechizo(short UserIndex, short Slot)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short hIndex;
        short j;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            hIndex = Declaraciones.objData[withBlock.Invent.userObj[Slot].ObjIndex].HechizoIndex;

            if (!TieneHechizo(hIndex, UserIndex))
            {
                // Buscamos un slot vacio
                for (j = 1; j <= Declaraciones.MAXUSERHECHIZOS; j++)
                    if (withBlock.Stats.UserHechizos[j] == 0)
                        break;

                if (withBlock.Stats.UserHechizos[j] != 0)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes espacio para más hechizos.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
                else
                {
                    withBlock.Stats.UserHechizos[j] = hIndex;
                    UpdateUserHechizos(false, UserIndex, Convert.ToByte(j));
                    // Quitamos del inv el item
                    InvUsuario.QuitarUserInvItem(UserIndex, Convert.ToByte(Slot), 1);
                }
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex, "Ya tienes ese hechizo.", Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    public static void DecirPalabrasMagicas(string SpellWords, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 17/11/2009
        // 25/07/2009: ZaMa - Invisible admins don't say any word when casting a spell
        // 17/11/2009: ZaMa - Now the user become visible when casting a spell, if it is hidden
        // ***************************************************
        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (withBlock.flags.AdminInvisible != 1)
                {
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessageChatOverHead(SpellWords, withBlock.character.CharIndex,
                            ColorTranslator.ToOle(Color.Cyan)));

                    // Si estaba oculto, se vuelve visible
                    if (withBlock.flags.Oculto == 1)
                    {
                        withBlock.flags.Oculto = 0;
                        withBlock.Counters.TiempoOculto = 0;

                        if (withBlock.flags.invisible == 0)
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            UsUaRiOs.SetInvisible(UserIndex, withBlock.character.CharIndex, false);
                        }
                    }
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in NpcLanzaSpellSobreUser: " + ex.Message);
        }
    }

    // '
    // Check if an user can cast a certain spell
    // 
    // @param UserIndex Specifies reference to user
    // @param HechizoIndex Specifies reference to spell
    // @return   True if the user can cast the spell, otherwise returns false
    public static bool PuedeLanzar(short UserIndex, short HechizoIndex)
    {
        bool PuedeLanzarRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 12/01/2010
        // Last Modification By: ZaMa
        // 06/11/09 - Corregida la bonificación de maná del mimetismo en el druida con flauta mágica equipada.
        // 19/11/2009: ZaMa - Validacion de mana para el Invocar Mascotas
        // 12/01/2010: ZaMa - Validacion de mana para hechizos lanzados por druida.
        // ***************************************************
        float DruidManaBonus;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.flags.Muerto != 0)
            {
                Protocol.WriteConsoleMsg(UserIndex, "No puedes lanzar hechizos estando muerto.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return PuedeLanzarRet;
            }

            if (Declaraciones.Hechizos[HechizoIndex].NeedStaff > 0)
                if (withBlock.clase == Declaraciones.eClass.Mage)
                {
                    if (withBlock.Invent.WeaponEqpObjIndex > 0)
                    {
                        if (Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].StaffPower <
                            Declaraciones.Hechizos[HechizoIndex].NeedStaff)
                        {
                            Protocol.WriteConsoleMsg(UserIndex,
                                "No posees un báculo lo suficientemente poderoso para poder lanzar el conjuro.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return PuedeLanzarRet;
                        }
                    }
                    else
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "No puedes lanzar este conjuro sin la ayuda de un báculo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return PuedeLanzarRet;
                    }
                }

            if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Magia] <
                Declaraciones.Hechizos[HechizoIndex].MinSkill)
            {
                Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes puntos de magia para lanzar este hechizo.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return PuedeLanzarRet;
            }

            if (withBlock.Stats.MinSta < Declaraciones.Hechizos[HechizoIndex].StaRequerido)
            {
                if (withBlock.Genero == Declaraciones.eGenero.Hombre)
                    Protocol.WriteConsoleMsg(UserIndex, "Estás muy cansado para lanzar este hechizo.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                else
                    Protocol.WriteConsoleMsg(UserIndex, "Estás muy cansada para lanzar este hechizo.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                return PuedeLanzarRet;
            }

            DruidManaBonus = 1f;
            if (withBlock.clase == Declaraciones.eClass.Druid)
            {
                if (withBlock.Invent.AnilloEqpObjIndex == Declaraciones.FLAUTAELFICA)
                {
                    // 50% menos de mana requerido para mimetismo
                    if (Declaraciones.Hechizos[HechizoIndex].Mimetiza == 1)
                        DruidManaBonus = 0.5f;

                    // 30% menos de mana requerido para invocaciones
                    else if (Declaraciones.Hechizos[HechizoIndex].Tipo == Declaraciones.TipoHechizo.uInvocacion)
                        DruidManaBonus = 0.7f;

                    // 10% menos de mana requerido para las demas magias, excepto apoca
                    else if (HechizoIndex != Declaraciones.APOCALIPSIS_SPELL_INDEX) DruidManaBonus = 0.9f;
                }

                // Necesita tener la barra de mana completa para invocar una mascota
                if (Declaraciones.Hechizos[HechizoIndex].Warp == 1)
                {
                    if (withBlock.Stats.MinMAN != withBlock.Stats.MaxMAN)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Debes poseer toda tu maná para poder lanzar este hechizo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return PuedeLanzarRet;
                    }
                    // Si no tiene mascotas, no tiene sentido que lo use

                    if (withBlock.NroMascotas == 0)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "Debes poseer alguna mascota para poder lanzar este hechizo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return PuedeLanzarRet;
                    }
                }
            }

            if (withBlock.Stats.MinMAN < Declaraciones.Hechizos[HechizoIndex].ManaRequerido * DruidManaBonus)
            {
                Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente maná.", Protocol.FontTypeNames.FONTTYPE_INFO);
                return PuedeLanzarRet;
            }
        }

        PuedeLanzarRet = true;
        return PuedeLanzarRet;
    }

    public static void HechizoTerrenoEstado(short UserIndex, ref bool b)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short PosCasteadaX;
        short PosCasteadaY;
        short PosCasteadaM;
        short H;
        short TempX;
        short TempY;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            PosCasteadaX = withBlock.flags.TargetX;
            PosCasteadaY = withBlock.flags.TargetY;
            PosCasteadaM = withBlock.flags.TargetMap;

            H = withBlock.flags.Hechizo;

            if (Declaraciones.Hechizos[H].RemueveInvisibilidadParcial == 1)
            {
                b = true;
                var loopTo = Convert.ToInt16(PosCasteadaX + 8);
                for (TempX = Convert.ToInt16(PosCasteadaX - 8); TempX <= loopTo; TempX++)
                {
                    var loopTo1 = Convert.ToInt16(PosCasteadaY + 8);
                    for (TempY = Convert.ToInt16(PosCasteadaY - 8); TempY <= loopTo1; TempY++)
                        if (Extra.InMapBounds(PosCasteadaM, TempX, TempY))
                            if (Declaraciones.MapData[PosCasteadaM, TempX, TempY].UserIndex > 0)
                                // hay un user
                                if ((Declaraciones.UserList[Declaraciones.MapData[PosCasteadaM, TempX, TempY].UserIndex]
                                        .flags.invisible == 1) &
                                    (Declaraciones.UserList[Declaraciones.MapData[PosCasteadaM, TempX, TempY].UserIndex]
                                        .flags.AdminInvisible == 0))

                                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                        Protocol.PrepareMessageCreateFX(
                                            Declaraciones
                                                .UserList[Declaraciones.MapData[PosCasteadaM, TempX, TempY].UserIndex]
                                                .character.CharIndex, Declaraciones.Hechizos[H].FXgrh,
                                            Declaraciones.Hechizos[H].loops));
                }

                InfoHechizo(UserIndex);
            }
        }
    }

    // '
    // Le da propiedades al nuevo npc
    // 
    // @param UserIndex  Indice del usuario que invoca.
    // @param b  Indica si se termino la operación.

    public static void HechizoInvocacion(short UserIndex, ref bool HechizoCasteado)
    {
        // ***************************************************
        // Author: Uknown
        // Last modification: 18/11/2009
        // Sale del sub si no hay una posición valida.
        // 18/11/2009: Optimizacion de codigo.
        // ***************************************************

        short NpcIndex, SpellIndex = default, NroNpcs, PetIndex;
        Declaraciones.WorldPos TargetPos;

        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // No permitimos se invoquen criaturas en zonas seguras
                if (!Declaraciones.mapInfo[withBlock.Pos.Map].Pk |
                    (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger ==
                     Declaraciones.eTrigger.ZONASEGURA))
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes invocar criaturas en zona segura.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }


                TargetPos.Map = withBlock.flags.TargetMap;
                TargetPos.X = withBlock.flags.TargetX;
                TargetPos.Y = withBlock.flags.TargetY;

                SpellIndex = withBlock.flags.Hechizo;

                // Warp de mascotas
                if (Declaraciones.Hechizos[SpellIndex].Warp == 1)
                {
                    PetIndex = UsUaRiOs.FarthestPet(UserIndex);

                    // La invoco cerca mio
                    if (PetIndex > 0) UsUaRiOs.WarpMascota(UserIndex, PetIndex);
                }

                // Invocacion normal
                else
                {
                    if (withBlock.NroMascotas >= Declaraciones.MAXMASCOTAS)
                        return;

                    var loopTo = Declaraciones.Hechizos[SpellIndex].cant;
                    for (NroNpcs = 1; NroNpcs <= loopTo; NroNpcs++)
                        if (withBlock.NroMascotas < Declaraciones.MAXMASCOTAS)
                        {
                            NpcIndex = NPCs.SpawnNpc(Declaraciones.Hechizos[SpellIndex].NumNpc, ref TargetPos, true,
                                false);
                            if (NpcIndex > 0)
                            {
                                withBlock.NroMascotas = Convert.ToInt16(withBlock.NroMascotas + 1);

                                PetIndex = Trabajo.FreeMascotaIndex(UserIndex);

                                withBlock.MascotasIndex[PetIndex] = NpcIndex;
                                withBlock.MascotasType[PetIndex] = Declaraciones.Npclist[NpcIndex].Numero;

                                {
                                    ref var withBlock1 = ref Declaraciones.Npclist[NpcIndex];
                                    withBlock1.MaestroUser = UserIndex;
                                    withBlock1.Contadores.TiempoExistencia = Admin.IntervaloInvocacion;
                                    withBlock1.GiveGLD = 0;
                                }

                                NPCs.FollowAmo(NpcIndex);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            break;
                        }
                }
            }

            InfoHechizo(UserIndex);
            HechizoCasteado = true;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in AgregarHechizo: " + ex.Message);
            {
                ref var withBlock2 = ref Declaraciones.UserList[UserIndex];
                var argdesc = "[" + ex.GetType().Name + "] " + ex.Message + " por el usuario " + withBlock2.name + "(" +
                              UserIndex + ") en (" + withBlock2.Pos.Map + ", " + withBlock2.Pos.X + ", " +
                              withBlock2.Pos.Y + "). Tratando de tirar el hechizo " +
                              Declaraciones.Hechizos[SpellIndex].Nombre + "(" + SpellIndex + ") en la posicion ( " +
                              withBlock2.flags.TargetX + ", " + withBlock2.flags.TargetY + ")";
                General.LogError(ref argdesc);
            }
        }
    }

    public static void HandleHechizoTerreno(short UserIndex, short SpellIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 18/11/2009
        // 18/11/2009: ZaMa - Optimizacion de codigo.
        // ***************************************************

        var HechizoCasteado = default(bool);
        short ManaRequerida;

        switch (Declaraciones.Hechizos[SpellIndex].Tipo)
        {
            case Declaraciones.TipoHechizo.uInvocacion:
            {
                HechizoInvocacion(UserIndex, ref HechizoCasteado);
                break;
            }

            case Declaraciones.TipoHechizo.uEstado:
            {
                HechizoTerrenoEstado(UserIndex, ref HechizoCasteado);
                break;
            }
        }

        if (HechizoCasteado)
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Magia, true);

            ManaRequerida = Declaraciones.Hechizos[SpellIndex].ManaRequerido;

            if (Declaraciones.Hechizos[SpellIndex].Warp == 1) // Invocó una mascota
                // Consume toda la mana
                ManaRequerida = withBlock.Stats.MinMAN;
            // Bonificaciones en hechizos
            else if (withBlock.clase == Declaraciones.eClass.Druid)
                // Solo con flauta equipada
                if (withBlock.Invent.AnilloEqpObjIndex == Declaraciones.FLAUTAELFICA)
                    // 30% menos de mana para invocaciones
                    ManaRequerida = Convert.ToInt16(ManaRequerida * 0.7d);

            // Quito la mana requerida
            withBlock.Stats.MinMAN = (short)(withBlock.Stats.MinMAN - ManaRequerida);
            if (withBlock.Stats.MinMAN < 0)
                withBlock.Stats.MinMAN = 0;

            // Quito la estamina requerida
            withBlock.Stats.MinSta = (short)(withBlock.Stats.MinSta - Declaraciones.Hechizos[SpellIndex].StaRequerido);
            if (withBlock.Stats.MinSta < 0)
                withBlock.Stats.MinSta = 0;

            // Update user stats
            Protocol.WriteUpdateUserStats(UserIndex);
        }
    }

    public static void HandleHechizoUsuario(short UserIndex, short SpellIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 12/01/2010
        // 18/11/2009: ZaMa - Optimizacion de codigo.
        // 12/01/2010: ZaMa - Optimizacion y agrego bonificaciones al druida.
        // ***************************************************

        var HechizoCasteado = default(bool);
        short ManaRequerida;

        switch (Declaraciones.Hechizos[SpellIndex].Tipo)
        {
            case Declaraciones.TipoHechizo.uEstado:
            {
                // Afectan estados (por ejem : Envenenamiento)
                HechizoEstadoUsuario(UserIndex, ref HechizoCasteado);
                break;
            }

            case Declaraciones.TipoHechizo.uPropiedades:
            {
                // Afectan HP,MANA,STAMINA,ETC
                HechizoCasteado = HechizoPropUsuario(UserIndex);
                break;
            }
        }

        if (HechizoCasteado)
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Magia, true);

            ManaRequerida = Declaraciones.Hechizos[SpellIndex].ManaRequerido;

            // Bonificaciones para druida
            if (withBlock.clase == Declaraciones.eClass.Druid)
                // Solo con flauta magica
                if (withBlock.Invent.AnilloEqpObjIndex == Declaraciones.FLAUTAELFICA)
                {
                    if (Declaraciones.Hechizos[SpellIndex].Mimetiza == 1)
                        // 50% menos de mana para mimetismo
                        ManaRequerida = Convert.ToInt16(ManaRequerida * 0.5d);

                    else if (SpellIndex != Declaraciones.APOCALIPSIS_SPELL_INDEX)
                        // 10% menos de mana para todo menos apoca y descarga
                        ManaRequerida = Convert.ToInt16(ManaRequerida * 0.9d);
                }

            // Quito la mana requerida
            withBlock.Stats.MinMAN = (short)(withBlock.Stats.MinMAN - ManaRequerida);
            if (withBlock.Stats.MinMAN < 0)
                withBlock.Stats.MinMAN = 0;

            // Quito la estamina requerida
            withBlock.Stats.MinSta = (short)(withBlock.Stats.MinSta - Declaraciones.Hechizos[SpellIndex].StaRequerido);
            if (withBlock.Stats.MinSta < 0)
                withBlock.Stats.MinSta = 0;

            // Update user stats
            Protocol.WriteUpdateUserStats(UserIndex);
            Protocol.WriteUpdateUserStats(withBlock.flags.TargetUser);
            withBlock.flags.TargetUser = 0;
        }
    }

    public static void HandleHechizoNPC(short UserIndex, short HechizoIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 12/01/2010
        // 13/02/2009: ZaMa - Agregada 50% bonificacion en coste de mana a mimetismo para druidas
        // 17/11/2009: ZaMa - Optimizacion de codigo.
        // 12/01/2010: ZaMa - Bonificacion para druidas de 10% para todos hechizos excepto apoca y descarga.
        // 12/01/2010: ZaMa - Los druidas mimetizados con npcs ahora son ignorados.
        // ***************************************************
        var HechizoCasteado = default(bool);
        int ManaRequerida;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            switch (Declaraciones.Hechizos[HechizoIndex].Tipo)
            {
                case Declaraciones.TipoHechizo.uEstado:
                {
                    // Afectan estados (por ejem : Envenenamiento)
                    HechizoEstadoNPC(withBlock.flags.TargetNPC, HechizoIndex, ref HechizoCasteado, UserIndex);
                    break;
                }

                case Declaraciones.TipoHechizo.uPropiedades:
                {
                    // Afectan HP,MANA,STAMINA,ETC
                    HechizoPropNPC(HechizoIndex, withBlock.flags.TargetNPC, UserIndex, ref HechizoCasteado);
                    break;
                }
            }


            if (HechizoCasteado)
            {
                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Magia, true);

                ManaRequerida = Declaraciones.Hechizos[HechizoIndex].ManaRequerido;

                // Bonificación para druidas.
                if (withBlock.clase == Declaraciones.eClass.Druid)
                {
                    // Se mostró como usuario, puede ser atacado por npcs
                    withBlock.flags.Ignorado = false;

                    // Solo con flauta equipada
                    if (withBlock.Invent.AnilloEqpObjIndex == Declaraciones.FLAUTAELFICA)
                    {
                        if (Declaraciones.Hechizos[HechizoIndex].Mimetiza == 1)
                        {
                            // 50% menos de mana para mimetismo
                            ManaRequerida = Convert.ToInt32(ManaRequerida * 0.5d);
                            // Será ignorado hasta que pierda el efecto del mimetismo o ataque un npc
                            withBlock.flags.Ignorado = true;
                        }
                        // 10% menos de mana para hechizos
                        else if (HechizoIndex != Declaraciones.APOCALIPSIS_SPELL_INDEX)
                        {
                            ManaRequerida = Convert.ToInt32(ManaRequerida * 0.9d);
                        }
                    }
                }

                // Quito la mana requerida
                withBlock.Stats.MinMAN = Convert.ToInt16(withBlock.Stats.MinMAN - ManaRequerida);
                if (withBlock.Stats.MinMAN < 0)
                    withBlock.Stats.MinMAN = 0;

                // Quito la estamina requerida
                withBlock.Stats.MinSta =
                    (short)(withBlock.Stats.MinSta - Declaraciones.Hechizos[HechizoIndex].StaRequerido);
                if (withBlock.Stats.MinSta < 0)
                    withBlock.Stats.MinSta = 0;

                // Update user stats
                Protocol.WriteUpdateUserStats(UserIndex);
                withBlock.flags.TargetNPC = 0;
            }
        }
    }


    public static void LanzarHechizo(short SpellIndex, short UserIndex)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 02/16/2010
        // 24/01/2007 ZaMa - Optimizacion de codigo.
        // 02/16/2010: Marco - Now .flags.hechizo makes reference to global spell index instead of user's spell index
        // ***************************************************
        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];

                if (withBlock.flags.EnConsulta)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes lanzar hechizos si estás en consulta.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                if (PuedeLanzar(UserIndex, SpellIndex))
                    switch (Declaraciones.Hechizos[SpellIndex].Target)
                    {
                        case Declaraciones.TargetType.uUsuarios:
                        {
                            if (withBlock.flags.TargetUser > 0)
                            {
                                if (Math.Abs((short)(Declaraciones.UserList[withBlock.flags.TargetUser].Pos.Y -
                                                     withBlock.Pos.Y)) <= AI.RANGO_VISION_Y)
                                    HandleHechizoUsuario(UserIndex, SpellIndex);
                                else
                                    Protocol.WriteConsoleMsg(UserIndex,
                                        "Estás demasiado lejos para lanzar este hechizo.",
                                        Protocol.FontTypeNames.FONTTYPE_WARNING);
                            }
                            else
                            {
                                Protocol.WriteConsoleMsg(UserIndex, "Este hechizo actúa sólo sobre usuarios.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                            }

                            break;
                        }

                        case Declaraciones.TargetType.uNPC:
                        {
                            if (withBlock.flags.TargetNPC > 0)
                            {
                                if (Math.Abs((short)(Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos.Y -
                                                     withBlock.Pos.Y)) <= AI.RANGO_VISION_Y)
                                    HandleHechizoNPC(UserIndex, SpellIndex);
                                else
                                    Protocol.WriteConsoleMsg(UserIndex,
                                        "Estás demasiado lejos para lanzar este hechizo.",
                                        Protocol.FontTypeNames.FONTTYPE_WARNING);
                            }
                            else
                            {
                                Protocol.WriteConsoleMsg(UserIndex, "Este hechizo sólo afecta a los npcs.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                            }

                            break;
                        }

                        case Declaraciones.TargetType.uUsuariosYnpc:
                        {
                            if (withBlock.flags.TargetUser > 0)
                            {
                                if (Math.Abs((short)(Declaraciones.UserList[withBlock.flags.TargetUser].Pos.Y -
                                                     withBlock.Pos.Y)) <= AI.RANGO_VISION_Y)
                                    HandleHechizoUsuario(UserIndex, SpellIndex);
                                else
                                    Protocol.WriteConsoleMsg(UserIndex,
                                        "Estás demasiado lejos para lanzar este hechizo.",
                                        Protocol.FontTypeNames.FONTTYPE_WARNING);
                            }
                            else if (withBlock.flags.TargetNPC > 0)
                            {
                                if (Math.Abs((short)(Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos.Y -
                                                     withBlock.Pos.Y)) <= AI.RANGO_VISION_Y)
                                    HandleHechizoNPC(UserIndex, SpellIndex);
                                else
                                    Protocol.WriteConsoleMsg(UserIndex,
                                        "Estás demasiado lejos para lanzar este hechizo.",
                                        Protocol.FontTypeNames.FONTTYPE_WARNING);
                            }
                            else
                            {
                                Protocol.WriteConsoleMsg(UserIndex, "Target inválido.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                            }

                            break;
                        }

                        case Declaraciones.TargetType.uTerreno:
                        {
                            HandleHechizoTerreno(UserIndex, SpellIndex);
                            break;
                        }
                    }

                if (withBlock.Counters.Trabajando != 0)
                    withBlock.Counters.Trabajando = withBlock.Counters.Trabajando - 1;

                if (withBlock.Counters.Ocultando != 0)
                    withBlock.Counters.Ocultando = withBlock.Counters.Ocultando - 1;
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleHechizoTerreno: " + ex.Message);
            var argdesc = "Error en LanzarHechizo. Error " + ex.GetType().Name + " : " + ex.Message + " Hechizo: " +
                          Declaraciones.Hechizos[SpellIndex].Nombre + "(" + SpellIndex + "). Casteado por: " +
                          Declaraciones.UserList[UserIndex].name + "(" + UserIndex + ").";
            General.LogError(ref argdesc);
        }
    }

    public static void HechizoEstadoUsuario(short UserIndex, ref bool HechizoCasteado)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 28/04/2010
        // Handles the Spells that afect the Stats of an User
        // 24/01/2007 Pablo (ToxicWaste) - Invisibilidad no permitida en Mapas con InviSinEfecto
        // 26/01/2007 Pablo (ToxicWaste) - Cambios que permiten mejor manejo de ataques en los rings.
        // 26/01/2007 Pablo (ToxicWaste) - Revivir no permitido en Mapas con ResuSinEfecto
        // 02/01/2008 Marcos (ByVal) - Curar Veneno no permitido en usuarios muertos.
        // 06/28/2008 NicoNZ - Agregué que se le de valor al flag Inmovilizado.
        // 17/11/2008: NicoNZ - Agregado para quitar la penalización de vida en el ring y cambio de ecuacion.
        // 13/02/2009: ZaMa - Arreglada ecuacion para quitar vida tras resucitar en rings.
        // 23/11/2009: ZaMa - Optimizacion de codigo.
        // 28/04/2010: ZaMa - Agrego Restricciones para ciudas respecto al estado atacable.
        // ***************************************************


        short HechizoIndex;
        short TargetIndex;

        bool EraCriminal;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            HechizoIndex = withBlock.flags.Hechizo;
            TargetIndex = withBlock.flags.TargetUser;

            // <-------- Agrega Invisibilidad ---------->
            if (Declaraciones.Hechizos[HechizoIndex].Invisibilidad == 1)
            {
                if (Declaraciones.UserList[TargetIndex].flags.Muerto == 1)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡El usuario está muerto!",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    HechizoCasteado = false;
                    return;
                }

                if (Declaraciones.UserList[TargetIndex].Counters.Saliendo)
                {
                    if (UserIndex != TargetIndex)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "¡El hechizo no tiene efecto!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        HechizoCasteado = false;
                        return;
                    }

                    Protocol.WriteConsoleMsg(UserIndex, "¡No puedes hacerte invisible mientras te encuentras saliendo!",
                        Protocol.FontTypeNames.FONTTYPE_WARNING);
                    HechizoCasteado = false;
                    return;
                }

                // No usar invi mapas InviSinEfecto
                if (Declaraciones.mapInfo[Declaraciones.UserList[TargetIndex].Pos.Map].InviSinEfecto > 0)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡La invisibilidad no funciona aquí!",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    HechizoCasteado = false;
                    return;
                }

                // Chequea si el status permite ayudar al otro usuario
                HechizoCasteado = CanSupportUser(UserIndex, TargetIndex, true);
                if (!HechizoCasteado)
                    return;

                // Si sos user, no uses este hechizo con GMS.
                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                    if ((int)(~Declaraciones.UserList[TargetIndex].flags.Privilegios & Declaraciones.PlayerType.User) !=
                        0)
                    {
                        HechizoCasteado = false;
                        return;
                    }

                Declaraciones.UserList[TargetIndex].flags.invisible = 1;
                UsUaRiOs.SetInvisible(TargetIndex, Declaraciones.UserList[TargetIndex].character.CharIndex, true);

                InfoHechizo(UserIndex);
                HechizoCasteado = true;
            }

            // <-------- Agrega Mimetismo ---------->
            if (Declaraciones.Hechizos[HechizoIndex].Mimetiza == 1)
            {
                if (Declaraciones.UserList[TargetIndex].flags.Muerto == 1) return;

                if (Declaraciones.UserList[TargetIndex].flags.Navegando == 1) return;
                if (withBlock.flags.Navegando == 1) return;

                // Si sos user, no uses este hechizo con GMS.
                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                    if ((int)(~Declaraciones.UserList[TargetIndex].flags.Privilegios & Declaraciones.PlayerType.User) !=
                        0)
                        return;

                if (withBlock.flags.Mimetizado == 1)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Ya te encuentras mimetizado. El hechizo no ha tenido efecto.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                if (withBlock.flags.AdminInvisible == 1)
                    return;

                // copio el char original al mimetizado

                withBlock.CharMimetizado.body = withBlock.character.body;
                withBlock.CharMimetizado.Head = withBlock.character.Head;
                withBlock.CharMimetizado.CascoAnim = withBlock.character.CascoAnim;
                withBlock.CharMimetizado.ShieldAnim = withBlock.character.ShieldAnim;
                withBlock.CharMimetizado.WeaponAnim = withBlock.character.WeaponAnim;

                withBlock.flags.Mimetizado = 1;

                // ahora pongo local el del enemigo
                withBlock.character.body = Declaraciones.UserList[TargetIndex].character.body;
                withBlock.character.Head = Declaraciones.UserList[TargetIndex].character.Head;
                withBlock.character.CascoAnim = Declaraciones.UserList[TargetIndex].character.CascoAnim;
                withBlock.character.ShieldAnim = Declaraciones.UserList[TargetIndex].character.ShieldAnim;
                withBlock.character.WeaponAnim = UsUaRiOs.GetWeaponAnim(UserIndex,
                    Declaraciones.UserList[TargetIndex].Invent.WeaponEqpObjIndex);

                UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                    (byte)withBlock.character.heading, withBlock.character.WeaponAnim,
                    withBlock.character.ShieldAnim, withBlock.character.CascoAnim);

                InfoHechizo(UserIndex);
                HechizoCasteado = true;
            }

            // <-------- Agrega Envenenamiento ---------->
            if (Declaraciones.Hechizos[HechizoIndex].Envenena == 1)
            {
                if (UserIndex == TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    return;
                }

                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return;
                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);
                Declaraciones.UserList[TargetIndex].flags.Envenenado = 1;
                InfoHechizo(UserIndex);
                HechizoCasteado = true;
            }

            // <-------- Cura Envenenamiento ---------->
            if (Declaraciones.Hechizos[HechizoIndex].CuraVeneno == 1)
            {
                // Verificamos que el usuario no este muerto
                if (Declaraciones.UserList[TargetIndex].flags.Muerto == 1)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡El usuario está muerto!",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    HechizoCasteado = false;
                    return;
                }

                // Chequea si el status permite ayudar al otro usuario
                HechizoCasteado = CanSupportUser(UserIndex, TargetIndex);
                if (!HechizoCasteado)
                    return;

                // Si sos user, no uses este hechizo con GMS.
                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                    if ((int)(~Declaraciones.UserList[TargetIndex].flags.Privilegios & Declaraciones.PlayerType.User) !=
                        0)
                        return;

                Declaraciones.UserList[TargetIndex].flags.Envenenado = 0;
                InfoHechizo(UserIndex);
                HechizoCasteado = true;
            }

            // <-------- Agrega Maldicion ---------->
            if (Declaraciones.Hechizos[HechizoIndex].Maldicion == 1)
            {
                if (UserIndex == TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    return;
                }

                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return;
                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);
                Declaraciones.UserList[TargetIndex].flags.Maldicion = 1;
                InfoHechizo(UserIndex);
                HechizoCasteado = true;
            }

            // <-------- Remueve Maldicion ---------->
            if (Declaraciones.Hechizos[HechizoIndex].RemoverMaldicion == 1)
            {
                Declaraciones.UserList[TargetIndex].flags.Maldicion = 0;
                InfoHechizo(UserIndex);
                HechizoCasteado = true;
            }

            // <-------- Agrega Bendicion ---------->
            if (Declaraciones.Hechizos[HechizoIndex].Bendicion == 1)
            {
                Declaraciones.UserList[TargetIndex].flags.Bendicion = 1;
                InfoHechizo(UserIndex);
                HechizoCasteado = true;
            }

            // <-------- Agrega Paralisis/Inmobilidad ---------->
            if ((Declaraciones.Hechizos[HechizoIndex].Paraliza == 1) |
                (Declaraciones.Hechizos[HechizoIndex].Inmoviliza == 1))
            {
                if (UserIndex == TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    return;
                }

                if (Declaraciones.UserList[TargetIndex].flags.Paralizado == 0)
                {
                    if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                        return;

                    if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);

                    InfoHechizo(UserIndex);
                    HechizoCasteado = true;
                    if (Declaraciones.UserList[TargetIndex].Invent.AnilloEqpObjIndex == SUPERANILLO)
                    {
                        Protocol.WriteConsoleMsg(TargetIndex, " Tu anillo rechaza los efectos del hechizo.",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                        Protocol.WriteConsoleMsg(UserIndex, " ¡El hechizo no tiene efecto!",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                        Protocol.FlushBuffer(TargetIndex);
                        return;
                    }

                    if (Declaraciones.Hechizos[HechizoIndex].Inmoviliza == 1)
                        Declaraciones.UserList[TargetIndex].flags.Inmovilizado = 1;
                    Declaraciones.UserList[TargetIndex].flags.Paralizado = 1;
                    Declaraciones.UserList[TargetIndex].Counters.Paralisis = Admin.IntervaloParalizado;

                    Protocol.WriteParalizeOK(TargetIndex);
                    Protocol.FlushBuffer(TargetIndex);
                }
            }

            // <-------- Remueve Paralisis/Inmobilidad ---------->
            if (Declaraciones.Hechizos[HechizoIndex].RemoverParalisis == 1)
                // Remueve si esta en ese estado
                if (Declaraciones.UserList[TargetIndex].flags.Paralizado == 1)
                {
                    // Chequea si el status permite ayudar al otro usuario
                    HechizoCasteado = CanSupportUser(UserIndex, TargetIndex, true);
                    if (!HechizoCasteado)
                        return;

                    Declaraciones.UserList[TargetIndex].flags.Inmovilizado = 0;
                    Declaraciones.UserList[TargetIndex].flags.Paralizado = 0;

                    // no need to crypt this
                    Protocol.WriteParalizeOK(TargetIndex);
                    InfoHechizo(UserIndex);
                }

            // <-------- Remueve Estupidez (Aturdimiento) ---------->
            if (Declaraciones.Hechizos[HechizoIndex].RemoverEstupidez == 1)
                // Remueve si esta en ese estado
                if (Declaraciones.UserList[TargetIndex].flags.Estupidez == 1)
                {
                    // Chequea si el status permite ayudar al otro usuario
                    HechizoCasteado = CanSupportUser(UserIndex, TargetIndex);
                    if (!HechizoCasteado)
                        return;

                    Declaraciones.UserList[TargetIndex].flags.Estupidez = 0;

                    // no need to crypt this
                    Protocol.WriteDumbNoMore(TargetIndex);
                    Protocol.FlushBuffer(TargetIndex);
                    InfoHechizo(UserIndex);
                }

            // <-------- Revive ---------->
            if (Declaraciones.Hechizos[HechizoIndex].Revivir == 1)
            {
                if (Declaraciones.UserList[TargetIndex].flags.Muerto == 1)
                {
                    // Seguro de resurreccion (solo afecta a los hechizos, no al sacerdote ni al comando de GM)
                    if (Declaraciones.UserList[TargetIndex].flags.SeguroResu)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡El espíritu no tiene intenciones de regresar al mundo de los vivos!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        HechizoCasteado = false;
                        return;
                    }

                    // No usar resu en mapas con ResuSinEfecto
                    if (Declaraciones.mapInfo[Declaraciones.UserList[TargetIndex].Pos.Map].ResuSinEfecto > 0)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡Revivir no está permitido aquí! Retirate de la Zona si deseas utilizar el Hechizo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        HechizoCasteado = false;
                        return;
                    }

                    // No podemos resucitar si nuestra barra de energía no está llena. (GD: 29/04/07)
                    if (withBlock.Stats.MaxSta != withBlock.Stats.MinSta)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "No puedes resucitar si no tienes tu barra de energía llena.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        HechizoCasteado = false;
                        return;
                    }


                    // revisamos si necesita vara
                    if (withBlock.clase == Declaraciones.eClass.Mage)
                    {
                        if (withBlock.Invent.WeaponEqpObjIndex > 0)
                            if (Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].StaffPower <
                                Declaraciones.Hechizos[HechizoIndex].NeedStaff)
                            {
                                Protocol.WriteConsoleMsg(UserIndex,
                                    "Necesitas un báculo mejor para lanzar este hechizo.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                HechizoCasteado = false;
                                return;
                            }
                    }
                    else if (withBlock.clase == Declaraciones.eClass.Bard)
                    {
                        if ((withBlock.Invent.AnilloEqpObjIndex != Declaraciones.LAUDELFICO) &
                            (withBlock.Invent.AnilloEqpObjIndex != Declaraciones.LAUDMAGICO))
                        {
                            Protocol.WriteConsoleMsg(UserIndex,
                                "Necesitas un instrumento mágico para devolver la vida.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            HechizoCasteado = false;
                            return;
                        }
                    }
                    else if (withBlock.clase == Declaraciones.eClass.Druid)
                    {
                        if ((withBlock.Invent.AnilloEqpObjIndex != Declaraciones.FLAUTAELFICA) &
                            (withBlock.Invent.AnilloEqpObjIndex != Declaraciones.FLAUTAMAGICA))
                        {
                            Protocol.WriteConsoleMsg(UserIndex,
                                "Necesitas un instrumento mágico para devolver la vida.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            HechizoCasteado = false;
                            return;
                        }
                    }

                    // Chequea si el status permite ayudar al otro usuario
                    HechizoCasteado = CanSupportUser(UserIndex, TargetIndex, true);
                    if (!HechizoCasteado)
                        return;

                    EraCriminal = ES.criminal(UserIndex);

                    if (!ES.criminal(TargetIndex))
                        if (TargetIndex != UserIndex)
                        {
                            withBlock.Reputacion.NobleRep = withBlock.Reputacion.NobleRep + 500;
                            if (withBlock.Reputacion.NobleRep > Declaraciones.MAXREP)
                                withBlock.Reputacion.NobleRep = Declaraciones.MAXREP;
                            Protocol.WriteConsoleMsg(UserIndex,
                                "¡Los Dioses te sonríen, has ganado 500 puntos de nobleza!",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                    if (EraCriminal & !ES.criminal(UserIndex)) UsUaRiOs.RefreshCharStatus(UserIndex);

                    {
                        ref var withBlock1 = ref Declaraciones.UserList[TargetIndex];
                        // Pablo Toxic Waste (GD: 29/04/07)
                        withBlock1.Stats.MinAGU = 0;
                        withBlock1.flags.Sed = 1;
                        withBlock1.Stats.MinHam = 0;
                        withBlock1.flags.Hambre = 1;
                        Protocol.WriteUpdateHungerAndThirst(TargetIndex);
                        InfoHechizo(UserIndex);
                        withBlock1.Stats.MinMAN = 0;
                        withBlock1.Stats.MinSta = 0;
                    }

                    // Agregado para quitar la penalización de vida en el ring y cambio de ecuacion. (NicoNZ)
                    if (SistemaCombate.TriggerZonaPelea(UserIndex, TargetIndex) !=
                        Declaraciones.eTrigger6.TRIGGER6_PERMITE)
                        // Solo saco vida si es User. no quiero que exploten GMs por ahi.
                        if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                            withBlock.Stats.MinHp = Convert.ToInt16(withBlock.Stats.MinHp *
                                                                    (1d - Declaraciones.UserList[TargetIndex].Stats
                                                                        .ELV * 0.015d));

                    if (withBlock.Stats.MinHp <= 0)
                    {
                        UsUaRiOs.UserDie(UserIndex);
                        Protocol.WriteConsoleMsg(UserIndex, "El esfuerzo de resucitar fue demasiado grande.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        HechizoCasteado = false;
                    }
                    else
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "El esfuerzo de resucitar te ha debilitado.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        HechizoCasteado = true;
                    }

                    if (Declaraciones.UserList[TargetIndex].flags.Traveling == 1)
                    {
                        Declaraciones.UserList[TargetIndex].Counters.goHome = 0;
                        Declaraciones.UserList[TargetIndex].flags.Traveling = 0;
                        // Call WriteConsoleMsg(TargetIndex, "Tu viaje ha sido cancelado.", FontTypeNames.FONTTYPE_FIGHT)
                        Protocol.WriteMultiMessage(TargetIndex, (short)Declaraciones.eMessages.CancelHome);
                    }

                    UsUaRiOs.RevivirUsuario(TargetIndex);
                }
                else
                {
                    HechizoCasteado = false;
                }
            }

            // <-------- Agrega Ceguera ---------->
            if (Declaraciones.Hechizos[HechizoIndex].Ceguera == 1)
            {
                if (UserIndex == TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    return;
                }

                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return;
                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);
                Declaraciones.UserList[TargetIndex].flags.Ceguera = 1;
                Declaraciones.UserList[TargetIndex].Counters.Ceguera = Convert.ToInt16(Admin.IntervaloParalizado / 3d);

                Protocol.WriteBlind(TargetIndex);
                Protocol.FlushBuffer(TargetIndex);
                InfoHechizo(UserIndex);
                HechizoCasteado = true;
            }

            // <-------- Agrega Estupidez (Aturdimiento) ---------->
            if (Declaraciones.Hechizos[HechizoIndex].Estupidez == 1)
            {
                if (UserIndex == TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    return;
                }

                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return;
                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);
                if (Declaraciones.UserList[TargetIndex].flags.Estupidez == 0)
                {
                    Declaraciones.UserList[TargetIndex].flags.Estupidez = 1;
                    Declaraciones.UserList[TargetIndex].Counters.Ceguera = Admin.IntervaloParalizado;
                }

                Protocol.WriteDumb(TargetIndex);
                Protocol.FlushBuffer(TargetIndex);

                InfoHechizo(UserIndex);
                HechizoCasteado = true;
            }
        }
    }

    public static void HechizoEstadoNPC(short NpcIndex, short SpellIndex, ref bool HechizoCasteado, short UserIndex)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 07/07/2008
        // Handles the Spells that afect the Stats of an NPC
        // 04/13/2008 NicoNZ - Guardias Faccionarios pueden ser
        // removidos por users de su misma faccion.
        // 07/07/2008: NicoNZ - Solo se puede mimetizar con npcs si es druida
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            if (Declaraciones.Hechizos[SpellIndex].Invisibilidad == 1)
            {
                InfoHechizo(UserIndex);
                withBlock.flags.invisible = 1;
                HechizoCasteado = true;
            }

            if (Declaraciones.Hechizos[SpellIndex].Envenena == 1)
            {
                if (!SistemaCombate.PuedeAtacarNPC(UserIndex, NpcIndex))
                {
                    HechizoCasteado = false;
                    return;
                }

                UsUaRiOs.NPCAtacado(NpcIndex, UserIndex);
                InfoHechizo(UserIndex);
                withBlock.flags.Envenenado = 1;
                HechizoCasteado = true;
            }

            if (Declaraciones.Hechizos[SpellIndex].CuraVeneno == 1)
            {
                InfoHechizo(UserIndex);
                withBlock.flags.Envenenado = 0;
                HechizoCasteado = true;
            }

            if (Declaraciones.Hechizos[SpellIndex].Maldicion == 1)
            {
                if (!SistemaCombate.PuedeAtacarNPC(UserIndex, NpcIndex))
                {
                    HechizoCasteado = false;
                    return;
                }

                UsUaRiOs.NPCAtacado(NpcIndex, UserIndex);
                InfoHechizo(UserIndex);
                withBlock.flags.Maldicion = 1;
                HechizoCasteado = true;
            }

            if (Declaraciones.Hechizos[SpellIndex].RemoverMaldicion == 1)
            {
                InfoHechizo(UserIndex);
                withBlock.flags.Maldicion = 0;
                HechizoCasteado = true;
            }

            if (Declaraciones.Hechizos[SpellIndex].Bendicion == 1)
            {
                InfoHechizo(UserIndex);
                withBlock.flags.Bendicion = 1;
                HechizoCasteado = true;
            }

            if (Declaraciones.Hechizos[SpellIndex].Paraliza == 1)
            {
                if (withBlock.flags.AfectaParalisis == 0)
                {
                    if (!SistemaCombate.PuedeAtacarNPC(UserIndex, NpcIndex, true))
                    {
                        HechizoCasteado = false;
                        return;
                    }

                    UsUaRiOs.NPCAtacado(NpcIndex, UserIndex);
                    InfoHechizo(UserIndex);
                    withBlock.flags.Paralizado = 1;
                    withBlock.flags.Inmovilizado = 0;
                    withBlock.Contadores.Paralisis = Admin.IntervaloParalizado;
                    HechizoCasteado = true;
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "El NPC es inmune a este hechizo.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    HechizoCasteado = false;
                    return;
                }
            }

            if (Declaraciones.Hechizos[SpellIndex].RemoverParalisis == 1)
            {
                if ((withBlock.flags.Paralizado == 1) | (withBlock.flags.Inmovilizado == 1))
                {
                    if (withBlock.MaestroUser == UserIndex)
                    {
                        InfoHechizo(UserIndex);
                        withBlock.flags.Paralizado = 0;
                        withBlock.Contadores.Paralisis = 0;
                        HechizoCasteado = true;
                    }
                    else if (withBlock.NPCtype == Declaraciones.eNPCType.GuardiaReal)
                    {
                        if (Extra.esArmada(UserIndex))
                        {
                            InfoHechizo(UserIndex);
                            withBlock.flags.Paralizado = 0;
                            withBlock.Contadores.Paralisis = 0;
                            HechizoCasteado = true;
                            return;
                        }

                        Protocol.WriteConsoleMsg(UserIndex,
                            "Sólo puedes remover la parálisis de los Guardias si perteneces a su facción.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        HechizoCasteado = false;
                        return;

                        Protocol.WriteConsoleMsg(UserIndex,
                            "Solo puedes remover la parálisis de los NPCs que te consideren su amo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        HechizoCasteado = false;
                        return;
                    }
                    else if (withBlock.NPCtype == Declaraciones.eNPCType.Guardiascaos)
                    {
                        if (Extra.esCaos(UserIndex))
                        {
                            InfoHechizo(UserIndex);
                            withBlock.flags.Paralizado = 0;
                            withBlock.Contadores.Paralisis = 0;
                            HechizoCasteado = true;
                            return;
                        }

                        Protocol.WriteConsoleMsg(UserIndex,
                            "Solo puedes remover la parálisis de los Guardias si perteneces a su facción.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        HechizoCasteado = false;
                        return;
                    }
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Este NPC no está paralizado",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    HechizoCasteado = false;
                    return;
                }
            }

            if (Declaraciones.Hechizos[SpellIndex].Inmoviliza == 1)
            {
                if (withBlock.flags.AfectaParalisis == 0)
                {
                    if (!SistemaCombate.PuedeAtacarNPC(UserIndex, NpcIndex, true))
                    {
                        HechizoCasteado = false;
                        return;
                    }

                    UsUaRiOs.NPCAtacado(NpcIndex, UserIndex);
                    withBlock.flags.Inmovilizado = 1;
                    withBlock.flags.Paralizado = 0;
                    withBlock.Contadores.Paralisis = Admin.IntervaloParalizado;
                    InfoHechizo(UserIndex);
                    HechizoCasteado = true;
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "El NPC es inmune al hechizo.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }
        }

        if (Declaraciones.Hechizos[SpellIndex].Mimetiza == 1)
        {
            ref var withBlock1 = ref Declaraciones.UserList[UserIndex];
            if (withBlock1.flags.Mimetizado == 1)
            {
                Protocol.WriteConsoleMsg(UserIndex, "Ya te encuentras mimetizado. El hechizo no ha tenido efecto.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (withBlock1.flags.AdminInvisible == 1)
                return;


            if (withBlock1.clase == Declaraciones.eClass.Druid)
            {
                // copio el char original al mimetizado
                withBlock1.CharMimetizado.body = withBlock1.character.body;
                withBlock1.CharMimetizado.Head = withBlock1.character.Head;
                withBlock1.CharMimetizado.CascoAnim = withBlock1.character.CascoAnim;
                withBlock1.CharMimetizado.ShieldAnim = withBlock1.character.ShieldAnim;
                withBlock1.CharMimetizado.WeaponAnim = withBlock1.character.WeaponAnim;

                withBlock1.flags.Mimetizado = 1;

                // ahora pongo lo del NPC.
                withBlock1.character.body = Declaraciones.Npclist[NpcIndex].character.body;
                withBlock1.character.Head = Declaraciones.Npclist[NpcIndex].character.Head;
                withBlock1.character.CascoAnim = Declaraciones.NingunCasco;
                withBlock1.character.ShieldAnim = Declaraciones.NingunEscudo;
                withBlock1.character.WeaponAnim = Declaraciones.NingunArma;

                UsUaRiOs.ChangeUserChar(UserIndex, withBlock1.character.body, withBlock1.character.Head,
                    (byte)withBlock1.character.heading, withBlock1.character.WeaponAnim,
                    withBlock1.character.ShieldAnim, withBlock1.character.CascoAnim);
            }

            else
            {
                Protocol.WriteConsoleMsg(UserIndex, "Sólo los druidas pueden mimetizarse con criaturas.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            InfoHechizo(UserIndex);
            HechizoCasteado = true;
        }
    }

    public static void HechizoPropNPC(short SpellIndex, short NpcIndex, short UserIndex, ref bool HechizoCasteado)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 14/08/2007
        // Handles the Spells that afect the Life NPC
        // 14/08/2007 Pablo (ToxicWaste) - Orden general.
        // ***************************************************

        int daño;

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            // Salud
            if (Declaraciones.Hechizos[SpellIndex].SubeHP == 1)
            {
                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinHp,
                    Declaraciones.Hechizos[SpellIndex].MaxHp);
                daño = daño + Matematicas.Porcentaje(daño, 3 * Declaraciones.UserList[UserIndex].Stats.ELV);

                InfoHechizo(UserIndex);
                withBlock.Stats.MinHp = withBlock.Stats.MinHp + daño;
                if (withBlock.Stats.MinHp > withBlock.Stats.MaxHp)
                    withBlock.Stats.MinHp = withBlock.Stats.MaxHp;
                Protocol.WriteConsoleMsg(UserIndex, "Has curado " + daño + " puntos de vida a la criatura.",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                HechizoCasteado = true;
            }

            else if (Declaraciones.Hechizos[SpellIndex].SubeHP == 2)
            {
                if (!SistemaCombate.PuedeAtacarNPC(UserIndex, NpcIndex))
                {
                    HechizoCasteado = false;
                    return;
                }

                UsUaRiOs.NPCAtacado(NpcIndex, UserIndex);
                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinHp,
                    Declaraciones.Hechizos[SpellIndex].MaxHp);
                daño = daño + Matematicas.Porcentaje(daño, 3 * Declaraciones.UserList[UserIndex].Stats.ELV);

                if (Declaraciones.Hechizos[SpellIndex].StaffAffected)
                    if (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Mage)
                    {
                        if (Declaraciones.UserList[UserIndex].Invent.WeaponEqpObjIndex > 0)
                            daño = Convert.ToInt32(daño *
                                (Declaraciones
                                    .objData[Declaraciones.UserList[UserIndex].Invent.WeaponEqpObjIndex]
                                    .StaffDamageBonus + 70) / 100d);
                        // Aumenta daño segun el staff-
                        // Daño = (Daño* (70 + BonifBáculo)) / 100
                        else
                            daño = Convert.ToInt32(daño * 0.7d); // Baja daño a 70% del original
                    }

                if ((Declaraciones.UserList[UserIndex].Invent.AnilloEqpObjIndex == Declaraciones.LAUDELFICO) |
                    (Declaraciones.UserList[UserIndex].Invent.AnilloEqpObjIndex == Declaraciones.FLAUTAELFICA))
                    daño = Convert.ToInt32(daño * 1.04d); // laud magico de los bardos

                InfoHechizo(UserIndex);
                HechizoCasteado = true;

                if (withBlock.flags.Snd2 > 0)
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                        Protocol.PrepareMessagePlayWave(Convert.ToByte(withBlock.flags.Snd2),
                            Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));

                // Quizas tenga defenza magica el NPC. Pablo (ToxicWaste)
                daño = daño - withBlock.Stats.defM;
                if (daño < 0)
                    daño = 0;

                withBlock.Stats.MinHp = withBlock.Stats.MinHp - daño;
                Protocol.WriteConsoleMsg(UserIndex, "¡Le has quitado " + daño + " puntos de vida a la criatura!",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                SistemaCombate.CalcularDarExp(UserIndex, NpcIndex, daño);

                if (withBlock.Stats.MinHp < 1)
                {
                    withBlock.Stats.MinHp = 0;
                    NPCs.MuereNpc(NpcIndex, UserIndex);
                }
            }
        }
    }

    public static void InfoHechizo(short UserIndex)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 25/07/2009
        // 25/07/2009: ZaMa - Code improvements.
        // 25/07/2009: ZaMa - Now invisible admins magic sounds are not sent to anyone but themselves
        // ***************************************************
        short SpellIndex;
        short tUser;
        short tNPC;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            SpellIndex = withBlock.flags.Hechizo;
            tUser = withBlock.flags.TargetUser;
            tNPC = withBlock.flags.TargetNPC;

            DecirPalabrasMagicas(Declaraciones.Hechizos[SpellIndex].PalabrasMagicas, UserIndex);

            if (tUser > 0)
            {
                // Los admins invisibles no producen sonidos ni fx's
                if ((withBlock.flags.AdminInvisible == 1) & (UserIndex == tUser))
                {
                    var argdatos = Protocol.PrepareMessageCreateFX(Declaraciones.UserList[tUser].character.CharIndex,
                        Declaraciones.Hechizos[SpellIndex].FXgrh, Declaraciones.Hechizos[SpellIndex].loops);
                    TCP.EnviarDatosASlot(UserIndex, ref argdatos);
                    var argdatos1 = Protocol.PrepareMessagePlayWave(
                        Convert.ToByte(Declaraciones.Hechizos[SpellIndex].WAV),
                        Convert.ToByte(Declaraciones.UserList[tUser].Pos.X),
                        Convert.ToByte(Declaraciones.UserList[tUser].Pos.Y));
                    TCP.EnviarDatosASlot(UserIndex, ref argdatos1);
                }
                else
                {
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, tUser,
                        Protocol.PrepareMessageCreateFX(Declaraciones.UserList[tUser].character.CharIndex,
                            Declaraciones.Hechizos[SpellIndex].FXgrh, Declaraciones.Hechizos[SpellIndex].loops));
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, tUser,
                        Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[SpellIndex].WAV),
                            Convert.ToByte(Declaraciones.UserList[tUser].Pos.X),
                            Convert.ToByte(Declaraciones.UserList[tUser].Pos.Y)));
                } // Esta linea faltaba. Pablo (ToxicWaste)
            }
            else if (tNPC > 0)
            {
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, tNPC,
                    Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[tNPC].character.CharIndex,
                        Declaraciones.Hechizos[SpellIndex].FXgrh, Declaraciones.Hechizos[SpellIndex].loops));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, tNPC,
                    Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[SpellIndex].WAV),
                        Convert.ToByte(Declaraciones.Npclist[tNPC].Pos.X),
                        Convert.ToByte(Declaraciones.Npclist[tNPC].Pos.Y)));
            }

            if (tUser > 0)
            {
                if (UserIndex != tUser)
                {
                    if (withBlock.showName)
                        Protocol.WriteConsoleMsg(UserIndex,
                            Declaraciones.Hechizos[SpellIndex].HechizeroMsg + " " + Declaraciones.UserList[tUser].name,
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    else
                        Protocol.WriteConsoleMsg(UserIndex,
                            Declaraciones.Hechizos[SpellIndex].HechizeroMsg + " alguien.",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(tUser, withBlock.name + " " + Declaraciones.Hechizos[SpellIndex].TargetMsg,
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, Declaraciones.Hechizos[SpellIndex].PropioMsg,
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
            }
            else if (tNPC > 0)
            {
                Protocol.WriteConsoleMsg(UserIndex,
                    Declaraciones.Hechizos[SpellIndex].HechizeroMsg + " " + "la criatura.",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
            }
        }
    }

    public static bool HechizoPropUsuario(short UserIndex)
    {
        bool HechizoPropUsuarioRet = default;
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 28/04/2010
        // 02/01/2008 Marcos (ByVal) - No permite tirar curar heridas a usuarios muertos.
        // 28/04/2010: ZaMa - Agrego Restricciones para ciudas respecto al estado atacable.
        // ***************************************************

        short SpellIndex;
        var daño = default(int);
        short TargetIndex;

        SpellIndex = Declaraciones.UserList[UserIndex].flags.Hechizo;
        TargetIndex = Declaraciones.UserList[UserIndex].flags.TargetUser;

        {
            ref var withBlock = ref Declaraciones.UserList[TargetIndex];
            if (withBlock.flags.Muerto != 0)
            {
                Protocol.WriteConsoleMsg(UserIndex, "No puedes lanzar este hechizo a un muerto.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return HechizoPropUsuarioRet;
            }

            // <-------- Aumenta Hambre ---------->
            if (Declaraciones.Hechizos[SpellIndex].SubeHam == 1)
            {
                InfoHechizo(UserIndex);

                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinHam,
                    Declaraciones.Hechizos[SpellIndex].MaxHam);

                withBlock.Stats.MinHam = Convert.ToInt16(withBlock.Stats.MinHam + daño);
                if (withBlock.Stats.MinHam > withBlock.Stats.MaxHam)
                    withBlock.Stats.MinHam = withBlock.Stats.MaxHam;

                if (UserIndex != TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Le has restaurado " + daño + " puntos de hambre a " + withBlock.name + ".",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(TargetIndex,
                        Declaraciones.UserList[UserIndex].name + " te ha restaurado " + daño + " puntos de hambre.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Te has restaurado " + daño + " puntos de hambre.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }

                Protocol.WriteUpdateHungerAndThirst(TargetIndex);
            }

            // <-------- Quita Hambre ---------->
            else if (Declaraciones.Hechizos[SpellIndex].SubeHam == 2)
            {
                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                if (UserIndex != TargetIndex)
                    SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);
                else
                    return HechizoPropUsuarioRet;

                InfoHechizo(UserIndex);

                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinHam,
                    Declaraciones.Hechizos[SpellIndex].MaxHam);

                withBlock.Stats.MinHam = Convert.ToInt16(withBlock.Stats.MinHam - daño);

                if (UserIndex != TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Le has quitado " + daño + " puntos de hambre a " + withBlock.name + ".",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(TargetIndex,
                        Declaraciones.UserList[UserIndex].name + " te ha quitado " + daño + " puntos de hambre.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Te has quitado " + daño + " puntos de hambre.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }

                if (withBlock.Stats.MinHam < 1)
                {
                    withBlock.Stats.MinHam = 0;
                    withBlock.flags.Hambre = 1;
                }

                Protocol.WriteUpdateHungerAndThirst(TargetIndex);
            }

            // <-------- Aumenta Sed ---------->
            if (Declaraciones.Hechizos[SpellIndex].SubeSed == 1)
            {
                InfoHechizo(UserIndex);

                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinSed,
                    Declaraciones.Hechizos[SpellIndex].MaxSed);

                withBlock.Stats.MinAGU = Convert.ToInt16(withBlock.Stats.MinAGU + daño);
                if (withBlock.Stats.MinAGU > withBlock.Stats.MaxAGU)
                    withBlock.Stats.MinAGU = withBlock.Stats.MaxAGU;

                Protocol.WriteUpdateHungerAndThirst(TargetIndex);

                if (UserIndex != TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Le has restaurado " + daño + " puntos de sed a " + withBlock.name + ".",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(TargetIndex,
                        Declaraciones.UserList[UserIndex].name + " te ha restaurado " + daño + " puntos de sed.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Te has restaurado " + daño + " puntos de sed.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
            }

            // <-------- Quita Sed ---------->
            else if (Declaraciones.Hechizos[SpellIndex].SubeSed == 2)
            {
                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);

                InfoHechizo(UserIndex);

                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinSed,
                    Declaraciones.Hechizos[SpellIndex].MaxSed);

                withBlock.Stats.MinAGU = Convert.ToInt16(withBlock.Stats.MinAGU - daño);

                if (UserIndex != TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Le has quitado " + daño + " puntos de sed a " + withBlock.name + ".",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(TargetIndex,
                        Declaraciones.UserList[UserIndex].name + " te ha quitado " + daño + " puntos de sed.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Te has quitado " + daño + " puntos de sed.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }

                if (withBlock.Stats.MinAGU < 1)
                {
                    withBlock.Stats.MinAGU = 0;
                    withBlock.flags.Sed = 1;
                }

                Protocol.WriteUpdateHungerAndThirst(TargetIndex);
            }

            // <-------- Aumenta Agilidad ---------->
            if (Declaraciones.Hechizos[SpellIndex].SubeAgilidad == 1)
            {
                // Chequea si el status permite ayudar al otro usuario
                if (!CanSupportUser(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                InfoHechizo(UserIndex);
                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinAgilidad,
                    Declaraciones.Hechizos[SpellIndex].MaxAgilidad);

                withBlock.flags.DuracionEfecto = 1200;
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] =
                    (byte)(withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] +
                           Convert.ToByte(daño));
                if (withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] > SistemaCombate.MinimoInt(
                        Declaraciones.MAXATRIBUTOS,
                        withBlock.Stats.UserAtributosBackUP[(int)Declaraciones.eAtributos.Agilidad] * 2))

                    withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] = Convert.ToByte(
                        SistemaCombate.MinimoInt(Declaraciones.MAXATRIBUTOS,
                            withBlock.Stats.UserAtributosBackUP[(int)Declaraciones.eAtributos.Agilidad] * 2));

                withBlock.flags.TomoPocion = true;
                Protocol.WriteUpdateDexterity(TargetIndex);
            }

            // <-------- Quita Agilidad ---------->
            else if (Declaraciones.Hechizos[SpellIndex].SubeAgilidad == 2)
            {
                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);

                InfoHechizo(UserIndex);

                withBlock.flags.TomoPocion = true;
                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinAgilidad,
                    Declaraciones.Hechizos[SpellIndex].MaxAgilidad);
                withBlock.flags.DuracionEfecto = 700;
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] =
                    (byte)(withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] -
                           Convert.ToByte(daño));
                if (withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] < Declaraciones.MINATRIBUTOS)
                    withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] = Declaraciones.MINATRIBUTOS;

                Protocol.WriteUpdateDexterity(TargetIndex);
            }

            // <-------- Aumenta Fuerza ---------->
            if (Declaraciones.Hechizos[SpellIndex].SubeFuerza == 1)
            {
                // Chequea si el status permite ayudar al otro usuario
                if (!CanSupportUser(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                InfoHechizo(UserIndex);
                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinFuerza,
                    Declaraciones.Hechizos[SpellIndex].MaxFuerza);

                withBlock.flags.DuracionEfecto = 1200;

                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] =
                    (byte)(withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] + Convert.ToByte(daño));
                if (withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] > SistemaCombate.MinimoInt(
                        Declaraciones.MAXATRIBUTOS,
                        withBlock.Stats.UserAtributosBackUP[(int)Declaraciones.eAtributos.Fuerza] * 2))

                    withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] = Convert.ToByte(
                        SistemaCombate.MinimoInt(Declaraciones.MAXATRIBUTOS,
                            withBlock.Stats.UserAtributosBackUP[(int)Declaraciones.eAtributos.Fuerza] * 2));

                withBlock.flags.TomoPocion = true;
                Protocol.WriteUpdateStrenght(TargetIndex);
            }

            // <-------- Quita Fuerza ---------->
            else if (Declaraciones.Hechizos[SpellIndex].SubeFuerza == 2)
            {
                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);

                InfoHechizo(UserIndex);

                withBlock.flags.TomoPocion = true;

                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinFuerza,
                    Declaraciones.Hechizos[SpellIndex].MaxFuerza);
                withBlock.flags.DuracionEfecto = 700;
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] =
                    (byte)(withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] - Convert.ToByte(daño));
                if (withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] < Declaraciones.MINATRIBUTOS)
                    withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] = Declaraciones.MINATRIBUTOS;

                Protocol.WriteUpdateStrenght(TargetIndex);
            }

            // <-------- Cura salud ---------->
            if (Declaraciones.Hechizos[SpellIndex].SubeHP == 1)
            {
                // Verifica que el usuario no este muerto
                if (withBlock.flags.Muerto == 1)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡El usuario está muerto!",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return HechizoPropUsuarioRet;
                }

                // Chequea si el status permite ayudar al otro usuario
                if (!CanSupportUser(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinHp,
                    Declaraciones.Hechizos[SpellIndex].MaxHp);
                daño = daño + Matematicas.Porcentaje(daño, 3 * Declaraciones.UserList[UserIndex].Stats.ELV);

                InfoHechizo(UserIndex);

                withBlock.Stats.MinHp = Convert.ToInt16(withBlock.Stats.MinHp + daño);
                if (withBlock.Stats.MinHp > withBlock.Stats.MaxHp)
                    withBlock.Stats.MinHp = withBlock.Stats.MaxHp;

                Protocol.WriteUpdateHP(TargetIndex);

                if (UserIndex != TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Le has restaurado " + daño + " puntos de vida a " + withBlock.name + ".",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(TargetIndex,
                        Declaraciones.UserList[UserIndex].name + " te ha restaurado " + daño + " puntos de vida.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Te has restaurado " + daño + " puntos de vida.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
            }

            // <-------- Quita salud (Daña) ---------->
            else if (Declaraciones.Hechizos[SpellIndex].SubeHP == 2)
            {
                if (UserIndex == TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    return HechizoPropUsuarioRet;
                }

                daño = Matematicas.RandomNumber(Declaraciones.Hechizos[SpellIndex].MinHp,
                    Declaraciones.Hechizos[SpellIndex].MaxHp);

                daño = daño + Matematicas.Porcentaje(daño, 3 * Declaraciones.UserList[UserIndex].Stats.ELV);

                if (Declaraciones.Hechizos[SpellIndex].StaffAffected)
                    if (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Mage)
                    {
                        if (Declaraciones.UserList[UserIndex].Invent.WeaponEqpObjIndex > 0)
                            daño = Convert.ToInt32(daño *
                                (Declaraciones
                                    .objData[Declaraciones.UserList[UserIndex].Invent.WeaponEqpObjIndex]
                                    .StaffDamageBonus + 70) / 100d);
                        else
                            daño = Convert.ToInt32(daño * 0.7d); // Baja daño a 70% del original
                    }

                if ((Declaraciones.UserList[UserIndex].Invent.AnilloEqpObjIndex == Declaraciones.LAUDELFICO) |
                    (Declaraciones.UserList[UserIndex].Invent.AnilloEqpObjIndex == Declaraciones.FLAUTAELFICA))
                    daño = Convert.ToInt32(daño * 1.04d); // laud magico de los bardos

                // cascos antimagia
                if (withBlock.Invent.CascoEqpObjIndex > 0)
                    daño = daño - Matematicas.RandomNumber(
                        Declaraciones.objData[withBlock.Invent.CascoEqpObjIndex].DefensaMagicaMin,
                        Declaraciones.objData[withBlock.Invent.CascoEqpObjIndex].DefensaMagicaMax);

                // anillos
                if (withBlock.Invent.AnilloEqpObjIndex > 0)
                    daño = daño - Matematicas.RandomNumber(
                        Declaraciones.objData[withBlock.Invent.AnilloEqpObjIndex].DefensaMagicaMin,
                        Declaraciones.objData[withBlock.Invent.AnilloEqpObjIndex].DefensaMagicaMax);

                if (daño < 0)
                    daño = 0;

                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);

                InfoHechizo(UserIndex);

                withBlock.Stats.MinHp = Convert.ToInt16(withBlock.Stats.MinHp - daño);

                Protocol.WriteUpdateHP(TargetIndex);

                Protocol.WriteConsoleMsg(UserIndex,
                    "Le has quitado " + daño + " puntos de vida a " + withBlock.name + ".",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                Protocol.WriteConsoleMsg(TargetIndex,
                    Declaraciones.UserList[UserIndex].name + " te ha quitado " + daño + " puntos de vida.",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);

                // Muere
                if (withBlock.Stats.MinHp < 1)
                {
                    if (withBlock.flags.AtacablePor != UserIndex)
                    {
                        // Store it!
                        Statistics.StoreFrag(UserIndex, TargetIndex);
                        UsUaRiOs.ContarMuerte(TargetIndex, UserIndex);
                    }

                    withBlock.Stats.MinHp = 0;
                    UsUaRiOs.ActStats(TargetIndex, UserIndex);
                    UsUaRiOs.UserDie(TargetIndex);
                }
            }

            // <-------- Aumenta Mana ---------->
            if (Declaraciones.Hechizos[SpellIndex].SubeMana == 1)
            {
                InfoHechizo(UserIndex);
                withBlock.Stats.MinMAN = Convert.ToInt16(withBlock.Stats.MinMAN + daño);
                if (withBlock.Stats.MinMAN > withBlock.Stats.MaxMAN)
                    withBlock.Stats.MinMAN = withBlock.Stats.MaxMAN;

                Protocol.WriteUpdateMana(TargetIndex);

                if (UserIndex != TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Le has restaurado " + daño + " puntos de maná a " + withBlock.name + ".",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(TargetIndex,
                        Declaraciones.UserList[UserIndex].name + " te ha restaurado " + daño + " puntos de maná.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Te has restaurado " + daño + " puntos de maná.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
            }

            // <-------- Quita Mana ---------->
            else if (Declaraciones.Hechizos[SpellIndex].SubeMana == 2)
            {
                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);

                InfoHechizo(UserIndex);

                if (UserIndex != TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Le has quitado " + daño + " puntos de maná a " + withBlock.name + ".",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(TargetIndex,
                        Declaraciones.UserList[UserIndex].name + " te ha quitado " + daño + " puntos de maná.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Te has quitado " + daño + " puntos de maná.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }

                withBlock.Stats.MinMAN = Convert.ToInt16(withBlock.Stats.MinMAN - daño);
                if (withBlock.Stats.MinMAN < 1)
                    withBlock.Stats.MinMAN = 0;

                Protocol.WriteUpdateMana(TargetIndex);
            }

            // <-------- Aumenta Stamina ---------->
            if (Declaraciones.Hechizos[SpellIndex].SubeSta == 1)
            {
                InfoHechizo(UserIndex);
                withBlock.Stats.MinSta = Convert.ToInt16(withBlock.Stats.MinSta + daño);
                if (withBlock.Stats.MinSta > withBlock.Stats.MaxSta)
                    withBlock.Stats.MinSta = withBlock.Stats.MaxSta;

                Protocol.WriteUpdateSta(TargetIndex);

                if (UserIndex != TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Le has restaurado " + daño + " puntos de energía a " + withBlock.name + ".",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(TargetIndex,
                        Declaraciones.UserList[UserIndex].name + " te ha restaurado " + daño + " puntos de energía.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Te has restaurado " + daño + " puntos de energía.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
            }

            // <-------- Quita Stamina ---------->
            else if (Declaraciones.Hechizos[SpellIndex].SubeSta == 2)
            {
                if (!SistemaCombate.PuedeAtacar(UserIndex, TargetIndex))
                    return HechizoPropUsuarioRet;

                if (UserIndex != TargetIndex) SistemaCombate.UsuarioAtacadoPorUsuario(UserIndex, TargetIndex);

                InfoHechizo(UserIndex);

                if (UserIndex != TargetIndex)
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Le has quitado " + daño + " puntos de energía a " + withBlock.name + ".",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(TargetIndex,
                        Declaraciones.UserList[UserIndex].name + " te ha quitado " + daño + " puntos de energía.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Te has quitado " + daño + " puntos de energía.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }

                withBlock.Stats.MinSta = Convert.ToInt16(withBlock.Stats.MinSta - daño);

                if (withBlock.Stats.MinSta < 1)
                    withBlock.Stats.MinSta = 0;

                Protocol.WriteUpdateSta(TargetIndex);
            }
        }

        HechizoPropUsuarioRet = true;

        Protocol.FlushBuffer(TargetIndex);
        return HechizoPropUsuarioRet;
    }

    public static bool CanSupportUser(short CasterIndex, short TargetIndex, bool DoCriminal = false)
    {
        bool CanSupportUserRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 28/04/2010
        // Checks if caster can cast support magic on target user.
        // ***************************************************

        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[CasterIndex];

                // Te podes curar a vos mismo
                if (CasterIndex == TargetIndex)
                {
                    CanSupportUserRet = true;
                    return CanSupportUserRet;
                }

                // No podes ayudar si estas en consulta
                if (withBlock.flags.EnConsulta)
                {
                    Protocol.WriteConsoleMsg(CasterIndex, "No puedes ayudar usuarios mientras estas en consulta.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return CanSupportUserRet;
                }

                // Si estas en la arena, esta todo permitido
                if (SistemaCombate.TriggerZonaPelea(CasterIndex, TargetIndex) ==
                    Declaraciones.eTrigger6.TRIGGER6_PERMITE)
                {
                    CanSupportUserRet = true;
                    return CanSupportUserRet;
                }

                // Victima criminal?
                if (ES.criminal(TargetIndex))
                {
                    // Casteador Ciuda?
                    if (!ES.criminal(CasterIndex))
                    {
                        // Armadas no pueden ayudar
                        if (Extra.esArmada(CasterIndex))
                        {
                            Protocol.WriteConsoleMsg(CasterIndex,
                                "Los miembros del ejército real no pueden ayudar a los criminales.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return CanSupportUserRet;
                        }

                        // Si el ciuda tiene el seguro puesto no puede ayudar
                        if (withBlock.flags.Seguro)
                        {
                            Protocol.WriteConsoleMsg(CasterIndex,
                                "Para ayudar criminales debes sacarte el seguro ya que te volverás criminal como ellos.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return CanSupportUserRet;
                        }
                        // Penalizacion

                        if (DoCriminal)
                        {
                            UsUaRiOs.VolverCriminal(CasterIndex);
                        }
                        else
                        {
                            var argNoblePts = Convert.ToInt32(withBlock.Reputacion.NobleRep * 0.5d);
                            var argBandidoPts = 10000;
                            DisNobAuBan(CasterIndex, ref argNoblePts, ref argBandidoPts);
                        }
                    }
                }

                // Victima ciuda o army
                // Casteador es caos? => No Pueden ayudar ciudas
                else if (Extra.esCaos(CasterIndex))
                {
                    Protocol.WriteConsoleMsg(CasterIndex,
                        "Los miembros de la legión oscura no pueden ayudar a los ciudadanos.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return CanSupportUserRet;
                }

                // Casteador ciuda/army?
                else if (!ES.criminal(CasterIndex))
                {
                    // Esta en estado atacable?
                    if (Declaraciones.UserList[TargetIndex].flags.AtacablePor > 0)
                        // No esta atacable por el casteador?
                        if (Declaraciones.UserList[TargetIndex].flags.AtacablePor != CasterIndex)
                        {
                            // Si es armada no puede ayudar
                            if (Extra.esArmada(CasterIndex))
                            {
                                Protocol.WriteConsoleMsg(CasterIndex,
                                    "Los miembros del ejército real no pueden ayudar a ciudadanos en estado atacable.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                return CanSupportUserRet;
                            }

                            // Seguro puesto?
                            if (withBlock.flags.Seguro)
                            {
                                Protocol.WriteConsoleMsg(CasterIndex,
                                    "Para ayudar ciudadanos en estado atacable debes sacarte el seguro, pero te puedes volver criminal.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                return CanSupportUserRet;
                            }

                            var argNoblePts1 = Convert.ToInt32(withBlock.Reputacion.NobleRep * 0.5d);
                            var argBandidoPts1 = 10000;
                            DisNobAuBan(CasterIndex, ref argNoblePts1, ref argBandidoPts1);
                        }
                }
            }

            CanSupportUserRet = true;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HechizoEstadoUsuario: " + ex.Message);
            var argdesc = "Error en CanSupportUser, Error: " + ex.GetType().Name + " - " + ex.Message +
                          " CasterIndex: " + CasterIndex + ", TargetIndex: " + TargetIndex;
            General.LogError(ref argdesc);
        }

        return CanSupportUserRet;
    }

    public static void UpdateUserHechizos(bool UpdateAll, short UserIndex, byte Slot)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        byte LoopC;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Actualiza un solo slot
            if (!UpdateAll)
            {
                // Actualiza el inventario
                if (withBlock.Stats.UserHechizos[Slot] > 0)
                    ChangeUserHechizo(UserIndex, Slot, withBlock.Stats.UserHechizos[Slot]);
                else
                    ChangeUserHechizo(UserIndex, Slot, 0);
            }
            else
            {
                // Actualiza todos los slots
                for (LoopC = 1; LoopC <= Declaraciones.MAXUSERHECHIZOS; LoopC++)
                    // Actualiza el inventario
                    if (withBlock.Stats.UserHechizos[LoopC] > 0)
                        ChangeUserHechizo(UserIndex, LoopC, withBlock.Stats.UserHechizos[LoopC]);
                    else
                        ChangeUserHechizo(UserIndex, LoopC, 0);
            }
        }
    }

    public static void ChangeUserHechizo(short UserIndex, byte Slot, short Hechizo)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Declaraciones.UserList[UserIndex].Stats.UserHechizos[Slot] = Hechizo;

        if ((Hechizo > 0) & (Hechizo < Declaraciones.NumeroHechizos + 1))
            Protocol.WriteChangeSpellSlot(UserIndex, Slot);
        else
            Protocol.WriteChangeSpellSlot(UserIndex, Slot);
    }


    public static void DesplazarHechizo(short UserIndex, short Dire, short HechizoDesplazado)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if ((Dire != 1) & (Dire != -1))
            return;
        if (!((HechizoDesplazado >= 1) & (HechizoDesplazado <= Declaraciones.MAXUSERHECHIZOS)))
            return;

        short TempHechizo;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (Dire == 1) // Mover arriba
            {
                if (HechizoDesplazado == 1)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes mover el hechizo en esa dirección.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
                else
                {
                    TempHechizo = withBlock.Stats.UserHechizos[HechizoDesplazado];
                    withBlock.Stats.UserHechizos[HechizoDesplazado] =
                        withBlock.Stats.UserHechizos[HechizoDesplazado - 1];
                    withBlock.Stats.UserHechizos[HechizoDesplazado - 1] = TempHechizo;
                }
            }
            else if (HechizoDesplazado == Declaraciones.MAXUSERHECHIZOS) // mover abajo
            {
                Protocol.WriteConsoleMsg(UserIndex, "No puedes mover el hechizo en esa dirección.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
            else
            {
                TempHechizo = withBlock.Stats.UserHechizos[HechizoDesplazado];
                withBlock.Stats.UserHechizos[HechizoDesplazado] = withBlock.Stats.UserHechizos[HechizoDesplazado + 1];
                withBlock.Stats.UserHechizos[HechizoDesplazado + 1] = TempHechizo;
            }
        }
    }

    public static void DisNobAuBan(short UserIndex, ref int NoblePts, ref int BandidoPts)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // disminuye la nobleza NoblePts puntos y aumenta el bandido BandidoPts puntos
        bool EraCriminal;
        EraCriminal = ES.criminal(UserIndex);

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Si estamos en la arena no hacemos nada
            if ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 6)
                return;

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) !=
                0)
            {
                // pierdo nobleza...
                withBlock.Reputacion.NobleRep = withBlock.Reputacion.NobleRep - NoblePts;
                if (withBlock.Reputacion.NobleRep < 0) withBlock.Reputacion.NobleRep = 0;

                // gano bandido...
                withBlock.Reputacion.BandidoRep = withBlock.Reputacion.BandidoRep + BandidoPts;
                if (withBlock.Reputacion.BandidoRep > Declaraciones.MAXREP)
                    withBlock.Reputacion.BandidoRep = Declaraciones.MAXREP;
                Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.NobilityLost);
                // Call WriteNobilityLost(UserIndex)
                if (ES.criminal(UserIndex))
                    if (withBlock.Faccion.ArmadaReal == 1)
                    {
                        var argExpulsado = true;
                        ModFacciones.ExpulsarFaccionReal(UserIndex, ref argExpulsado);
                    }
            }

            if (!EraCriminal & ES.criminal(UserIndex)) UsUaRiOs.RefreshCharStatus(UserIndex);
        }
    }
}