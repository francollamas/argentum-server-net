Option Strict Off
Option Explicit On
Module Declaraciones
	'Argentum Online 0.12.2
	'Copyright (C) 2002 Márquez Pablo Ignacio
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
	'Calle 3 número 983 piso 7 dto A
	'La Plata - Pcia, Buenos Aires - Republica Argentina
	'Código Postal 1900
	'Pablo Ignacio Márquez
	
	
	''
	' Modulo de declaraciones. Aca hay de todo.
	'
	
	Public aClon As New clsAntiMassClon
	Public TrashCollector As New Collection
	
	
	Public Const MAXSPAWNATTEMPS As Short = 60
	Public Const INFINITE_LOOPS As Short = -1
	Public Const FXSANGRE As Short = 14
	
	''
	' The color of chats over head of dead characters.
	Public Const CHAT_COLOR_DEAD_CHAR As Integer = &HC0C0C0
	
	''
	' The color of yells made by any kind of game administrator.
	Public Const CHAT_COLOR_GM_YELL As Integer = &HF82FF
	
	''
	' Coordinates for normal sounds (not 3D, like rain)
	Public Const NO_3D_SOUND As Byte = 0
	
	Public Const iFragataFantasmal As Short = 87
	Public Const iFragataReal As Short = 190
	Public Const iFragataCaos As Short = 189
	Public Const iBarca As Short = 84
	Public Const iGalera As Short = 85
	Public Const iGaleon As Short = 86
	Public Const iBarcaCiuda As Short = 395
	Public Const iBarcaPk As Short = 396
	Public Const iGaleraCiuda As Short = 397
	Public Const iGaleraPk As Short = 398
	Public Const iGaleonCiuda As Short = 399
	Public Const iGaleonPk As Short = 400
	
	Public Enum iMinerales
		HierroCrudo = 192
		PlataCruda = 193
		OroCrudo = 194
		LingoteDeHierro = 386
		LingoteDePlata = 387
		LingoteDeOro = 388
	End Enum
	
	Public Enum PlayerType
		User = &H1
		Consejero = &H2
		SemiDios = &H4
		Dios = &H8
		Admin = &H10
		RoleMaster = &H20
		ChaosCouncil = &H40
		RoyalCouncil = &H80
	End Enum
	
	Public Enum eClass
		Mage = 1 'Mago
		Cleric 'Clérigo
		Warrior 'Guerrero
		Assasin 'Asesino
		Thief 'Ladrón
		Bard 'Bardo
		Druid 'Druida
		Bandit 'Bandido
		Paladin 'Paladín
		Hunter 'Cazador
		Worker 'Trabajador
		Pirat 'Pirata
	End Enum
	
	Public Enum eCiudad
		cUllathorpe = 1
		cNix
		cBanderbill
		cLindos
		cArghal
	End Enum
	
	Public Enum eRaza
		Humano = 1
		Elfo
		Drow
		Gnomo
		Enano
	End Enum
	
	Enum eGenero
		Hombre = 1
		Mujer
	End Enum
	
	Public Enum eClanType
		ct_RoyalArmy
		ct_Evil
		ct_Neutral
		ct_GM
		ct_Legal
		ct_Criminal
	End Enum
	
	Public Const LimiteNewbie As Byte = 12
	
	'Barrin 3/10/03
	'Cambiado a 2 segundos el 30/11/07
	Public Const TIEMPO_INICIOMEDITAR As Short = 2000
	
	Public Const NingunEscudo As Short = 2
	Public Const NingunCasco As Short = 2
	Public Const NingunArma As Short = 2
	
	Public Const EspadaMataDragonesIndex As Short = 402
	Public Const LAUDMAGICO As Short = 696
	Public Const FLAUTAMAGICA As Short = 208
	
	Public Const LAUDELFICO As Short = 1049
	Public Const FLAUTAELFICA As Short = 1050
	
	Public Const APOCALIPSIS_SPELL_INDEX As Short = 25
	Public Const DESCARGA_SPELL_INDEX As Short = 23
	
	Public Const SLOTS_POR_FILA As Byte = 5
	
	Public Const PROB_ACUCHILLAR As Byte = 20
	Public Const DAÑO_ACUCHILLAR As Single = 0.2
	
	Public Const MAXMASCOTASENTRENADOR As Byte = 7
	
	Public Enum FXIDs
		FXWARP = 1
		FXMEDITARCHICO = 4
		FXMEDITARMEDIANO = 5
		FXMEDITARGRANDE = 6
		FXMEDITARXGRANDE = 16
		FXMEDITARXXGRANDE = 34
	End Enum
	
	Public Const TIEMPO_CARCEL_PIQUETE As Integer = 10
	
	''
	' TRIGGERS
	'
	' @param NADA nada
	' @param BAJOTECHO bajo techo
	' @param trigger_2 ???
	' @param POSINVALIDA los npcs no pueden pisar tiles con este trigger
	' @param ZONASEGURA no se puede robar o pelear desde este trigger
	' @param ANTIPIQUETE
	' @param ZONAPELEA al pelear en este trigger no se caen las cosas y no cambia el estado de ciuda o crimi
	'
	Public Enum eTrigger
		NADA = 0
		BAJOTECHO = 1
		trigger_2 = 2
		POSINVALIDA = 3
		ZONASEGURA = 4
		ANTIPIQUETE = 5
		ZONAPELEA = 6
	End Enum
	
	''
	' constantes para el trigger 6
	'
	' @see eTrigger
	' @param TRIGGER6_PERMITE TRIGGER6_PERMITE
	' @param TRIGGER6_PROHIBE TRIGGER6_PROHIBE
	' @param TRIGGER6_AUSENTE El trigger no aparece
	'
	Public Enum eTrigger6
		TRIGGER6_PERMITE = 1
		TRIGGER6_PROHIBE = 2
		TRIGGER6_AUSENTE = 3
	End Enum
	
	'TODO : Reemplazar por un enum
	Public Const Bosque As String = "BOSQUE"
	Public Const Nieve As String = "NIEVE"
	Public Const Desierto As String = "DESIERTO"
	Public Const Ciudad As String = "CIUDAD"
	Public Const Campo As String = "CAMPO"
	Public Const Dungeon As String = "DUNGEON"
	
	' <<<<<< Targets >>>>>>
	Public Enum TargetType
		uUsuarios = 1
		uNPC = 2
		uUsuariosYnpc = 3
		uTerreno = 4
	End Enum
	
	' <<<<<< Acciona sobre >>>>>>
	Public Enum TipoHechizo
		uPropiedades = 1
		uEstado = 2
		uMaterializa = 3 'Nose usa
		uInvocacion = 4
	End Enum
	
	Public Const MAXUSERHECHIZOS As Byte = 35
	
	
	' TODO: Y ESTO ? LO CONOCE GD ?
	Public Const EsfuerzoTalarGeneral As Byte = 4
	Public Const EsfuerzoTalarLeñador As Byte = 2
	
	Public Const EsfuerzoPescarPescador As Byte = 1
	Public Const EsfuerzoPescarGeneral As Byte = 3
	
	Public Const EsfuerzoExcavarMinero As Byte = 2
	Public Const EsfuerzoExcavarGeneral As Byte = 5
	
	Public Const FX_TELEPORT_INDEX As Short = 1
	
	Public Const PORCENTAJE_MATERIALES_UPGRADE As Single = 0.85
	
	' La utilidad de esto es casi nula, sólo se revisa si fue a la cabeza...
	Public Enum PartesCuerpo
		bCabeza = 1
		bPiernaIzquierda = 2
		bPiernaDerecha = 3
		bBrazoDerecho = 4
		bBrazoIzquierdo = 5
		bTorso = 6
	End Enum
	
	Public Const Guardias As Short = 6
	
	Public Const MAX_ORO_EDIT As Integer = 5000000
	
	
	Public Const STANDARD_BOUNTY_HUNTER_MESSAGE As String = "Se te ha otorgado un premio por ayudar al proyecto reportando bugs, el mismo está disponible en tu bóveda."
	Public Const TAG_USER_INVISIBLE As String = "[INVISIBLE]"
	Public Const TAG_CONSULT_MODE As String = "[CONSULTA]"
	
	Public Const MAXREP As Integer = 6000000
	Public Const MAXORO As Integer = 90000000
	Public Const MAXEXP As Integer = 99999999
	
	Public Const MAXUSERMATADOS As Integer = 65000
	
	Public Const MAXATRIBUTOS As Byte = 40
	Public Const MINATRIBUTOS As Byte = 6
	
	Public Const LingoteHierro As Short = 386
	Public Const LingotePlata As Short = 387
	Public Const LingoteOro As Short = 388
	Public Const Leña As Short = 58
	Public Const LeñaElfica As Short = 1006
	
	Public Const MAXNPCS As Short = 10000
	Public Const MAXCHARS As Short = 10000
	
	Public Const HACHA_LEÑADOR As Short = 127
	Public Const HACHA_LEÑA_ELFICA As Short = 1005
	Public Const PIQUETE_MINERO As Short = 187
	
	Public Const DAGA As Short = 15
	Public Const FOGATA_APAG As Short = 136
	Public Const FOGATA As Short = 63
	Public Const ORO_MINA As Short = 194
	Public Const PLATA_MINA As Short = 193
	Public Const HIERRO_MINA As Short = 192
	Public Const MARTILLO_HERRERO As Short = 389
	Public Const SERRUCHO_CARPINTERO As Short = 198
	Public Const ObjArboles As Short = 4
	Public Const RED_PESCA As Short = 543
	Public Const CAÑA_PESCA As Short = 138
	
	Public Enum eNPCType
		Comun = 0
		Revividor = 1
		GuardiaReal = 2
		Entrenador = 3
		Banquero = 4
		Noble = 5
		DRAGON = 6
		Timbero = 7
		Guardiascaos = 8
		ResucitadorNewbie = 9
		Pretoriano = 10
		Gobernador = 11
	End Enum
	
	Public Const MIN_APUÑALAR As Byte = 10
	
	'********** CONSTANTANTES ***********
	
	''
	' Cantidad de skills
	Public Const NUMSKILLS As Byte = 20
	
	''
	' Cantidad de Atributos
	Public Const NUMATRIBUTOS As Byte = 5
	
	''
	' Cantidad de Clases
	Public Const NUMCLASES As Byte = 12
	
	''
	' Cantidad de Razas
	Public Const NUMRAZAS As Byte = 5
	
	
	''
	' Valor maximo de cada skill
	Public Const MAXSKILLPOINTS As Byte = 100
	
	''
	' Cantidad de Ciudades
	Public Const NUMCIUDADES As Byte = 5
	
	
	''
	'Direccion
	'
	' @param NORTH Norte
	' @param EAST Este
	' @param SOUTH Sur
	' @param WEST Oeste
	'
	Public Enum eHeading
		NORTH = 1
		EAST = 2
		SOUTH = 3
		WEST = 4
	End Enum
	
	''
	' Cantidad maxima de mascotas
	Public Const MAXMASCOTAS As Byte = 3
	
	'%%%%%%%%%% CONSTANTES DE INDICES %%%%%%%%%%%%%%%
	Public Const vlASALTO As Short = 100
	Public Const vlASESINO As Short = 1000
	Public Const vlCAZADOR As Short = 5
	Public Const vlNoble As Short = 5
	Public Const vlLadron As Short = 25
	Public Const vlProleta As Short = 2
	
	'%%%%%%%%%% CONSTANTES DE INDICES %%%%%%%%%%%%%%%
	Public Const iCuerpoMuerto As Short = 8
	Public Const iCabezaMuerto As Short = 500
	
	
	Public Const iORO As Byte = 12
	Public Const Pescado As Byte = 139
	
	Public Enum PECES_POSIBLES
		PESCADO1 = 139
		PESCADO2 = 544
		PESCADO3 = 545
		PESCADO4 = 546
	End Enum
	
	'%%%%%%%%%% CONSTANTES DE INDICES %%%%%%%%%%%%%%%
	Public Enum eSkill
		Magia = 1
		Robar = 2
		Tacticas = 3
		Armas = 4
		Meditar = 5
		Apuñalar = 6
		Ocultarse = 7
		Supervivencia = 8
		Talar = 9
		Comerciar = 10
		Defensa = 11
		Pesca = 12
		Mineria = 13
		Carpinteria = 14
		Herreria = 15
		Liderazgo = 16
		Domar = 17
		Proyectiles = 18
		Wrestling = 19
		Navegacion = 20
	End Enum
	
	Public Enum eMochilas
		Mediana = 1
		Grande = 2
	End Enum
	
	Public Const FundirMetal As Short = 88
	
	Public Enum eAtributos
		Fuerza = 1
		Agilidad = 2
		Inteligencia = 3
		Carisma = 4
		Constitucion = 5
	End Enum
	
	Public Const AdicionalHPGuerrero As Byte = 2 'HP adicionales cuando sube de nivel
	Public Const AdicionalHPCazador As Byte = 1 'HP adicionales cuando sube de nivel
	
	Public Const AumentoSTDef As Byte = 15
	Public Const AumentoStBandido As Byte = AumentoSTDef + 23
	Public Const AumentoSTLadron As Byte = AumentoSTDef + 3
	Public Const AumentoSTMago As Byte = AumentoSTDef - 1
	Public Const AumentoSTTrabajador As Byte = AumentoSTDef + 25
	
	'Tamaño del mapa
	Public Const XMaxMapSize As Byte = 100
	Public Const XMinMapSize As Byte = 1
	Public Const YMaxMapSize As Byte = 100
	Public Const YMinMapSize As Byte = 1
	
	'Tamaño del tileset
	Public Const TileSizeX As Byte = 32
	Public Const TileSizeY As Byte = 32
	
	'Tamaño en Tiles de la pantalla de visualizacion
	Public Const XWindow As Byte = 17
	Public Const YWindow As Byte = 13
	
	'Sonidos
	Public Const SND_SWING As Byte = 2
	Public Const SND_TALAR As Byte = 13
	Public Const SND_PESCAR As Byte = 14
	Public Const SND_MINERO As Byte = 15
	Public Const SND_WARP As Byte = 3
	Public Const SND_PUERTA As Byte = 5
	Public Const SND_NIVEL As Byte = 6
	
	Public Const SND_USERMUERTE As Byte = 11
	Public Const SND_IMPACTO As Byte = 10
	Public Const SND_IMPACTO2 As Byte = 12
	Public Const SND_LEÑADOR As Byte = 13
	Public Const SND_FOGATA As Byte = 14
	Public Const SND_AVE As Byte = 21
	Public Const SND_AVE2 As Byte = 22
	Public Const SND_AVE3 As Byte = 34
	Public Const SND_GRILLO As Byte = 28
	Public Const SND_GRILLO2 As Byte = 29
	Public Const SND_SACARARMA As Byte = 25
	Public Const SND_ESCUDO As Byte = 37
	Public Const MARTILLOHERRERO As Byte = 41
	Public Const LABUROCARPINTERO As Byte = 42
	Public Const SND_BEBER As Byte = 46
	
	''
	' Cantidad maxima de objetos por slot de inventario
	Public Const MAX_INVENTORY_OBJS As Short = 10000
	
	''
	' Cantidad de "slots" en el inventario con mochila
	Public Const MAX_INVENTORY_SLOTS As Byte = 30
	
	''
	' Cantidad de "slots" en el inventario sin mochila
	Public Const MAX_NORMAL_INVENTORY_SLOTS As Byte = 20
	
	''
	' Constante para indicar que se esta usando ORO
	Public Const FLAGORO As Short = MAX_INVENTORY_SLOTS + 1
	
	
	' CATEGORIAS PRINCIPALES
	Public Enum eOBJType
		otUseOnce = 1
		otWeapon = 2
		otArmadura = 3
		otArboles = 4
		otGuita = 5
		otPuertas = 6
		otContenedores = 7
		otCarteles = 8
		otLlaves = 9
		otForos = 10
		otPociones = 11
		otBebidas = 13
		otLeña = 14
		otFogata = 15
		otESCUDO = 16
		otCASCO = 17
		otAnillo = 18
		otTeleport = 19
		otYacimiento = 22
		otMinerales = 23
		otPergaminos = 24
		otInstrumentos = 26
		otYunque = 27
		otFragua = 28
		otBarcos = 31
		otFlechas = 32
		otBotellaVacia = 33
		otBotellaLlena = 34
		otManchas = 35 'No se usa
		otArbolElfico = 36
		otMochilas = 37
		otCualquiera = 1000
	End Enum
	
	'Texto
	Public Const FONTTYPE_TALK As String = "~255~255~255~0~0"
	Public Const FONTTYPE_FIGHT As String = "~255~0~0~1~0"
	Public Const FONTTYPE_WARNING As String = "~32~51~223~1~1"
	Public Const FONTTYPE_INFO As String = "~65~190~156~0~0"
	Public Const FONTTYPE_INFOBOLD As String = "~65~190~156~1~0"
	Public Const FONTTYPE_EJECUCION As String = "~130~130~130~1~0"
	Public Const FONTTYPE_PARTY As String = "~255~180~255~0~0"
	Public Const FONTTYPE_VENENO As String = "~0~255~0~0~0"
	Public Const FONTTYPE_GUILD As String = "~255~255~255~1~0"
	Public Const FONTTYPE_SERVER As String = "~0~185~0~0~0"
	Public Const FONTTYPE_GUILDMSG As String = "~228~199~27~0~0"
	Public Const FONTTYPE_CONSEJO As String = "~130~130~255~1~0"
	Public Const FONTTYPE_CONSEJOCAOS As String = "~255~60~00~1~0"
	Public Const FONTTYPE_CONSEJOVesA As String = "~0~200~255~1~0"
	Public Const FONTTYPE_CONSEJOCAOSVesA As String = "~255~50~0~1~0"
	Public Const FONTTYPE_CENTINELA As String = "~0~255~0~1~0"
	
	'Estadisticas
	Public Const STAT_MAXELV As Byte = 255
	Public Const STAT_MAXHP As Short = 999
	Public Const STAT_MAXSTA As Short = 999
	Public Const STAT_MAXMAN As Short = 9999
	Public Const STAT_MAXHIT_UNDER36 As Byte = 99
	Public Const STAT_MAXHIT_OVER36 As Short = 999
	Public Const STAT_MAXDEF As Byte = 99
	
	Public Const ELU_SKILL_INICIAL As Byte = 200
	Public Const EXP_ACIERTO_SKILL As Byte = 50
	Public Const EXP_FALLO_SKILL As Byte = 20
	
	' **************************************************************
	' **************************************************************
	' ************************ TIPOS *******************************
	' **************************************************************
	' **************************************************************
	
	Public Structure tHechizo
		Dim Nombre As String
		Dim desc As String
		Dim PalabrasMagicas As String
		Dim HechizeroMsg As String
		Dim TargetMsg As String
		Dim PropioMsg As String
		'    Resis As Byte
		Dim Tipo As TipoHechizo
		Dim WAV As Short
		Dim FXgrh As Short
		Dim loops As Byte
		Dim SubeHP As Byte
		Dim MinHp As Short
		Dim MaxHp As Short
		Dim SubeMana As Byte
		Dim MiMana As Short
		Dim MaMana As Short
		Dim SubeSta As Byte
		Dim MinSta As Short
		Dim MaxSta As Short
		Dim SubeHam As Byte
		Dim MinHam As Short
		Dim MaxHam As Short
		Dim SubeSed As Byte
		Dim MinSed As Short
		Dim MaxSed As Short
		Dim SubeAgilidad As Byte
		Dim MinAgilidad As Short
		Dim MaxAgilidad As Short
		Dim SubeFuerza As Byte
		Dim MinFuerza As Short
		Dim MaxFuerza As Short
		Dim SubeCarisma As Byte
		Dim MinCarisma As Short
		Dim MaxCarisma As Short
		Dim Invisibilidad As Byte
		Dim Paraliza As Byte
		Dim Inmoviliza As Byte
		Dim RemoverParalisis As Byte
		Dim RemoverEstupidez As Byte
		Dim CuraVeneno As Byte
		Dim Envenena As Byte
		Dim Maldicion As Byte
		Dim RemoverMaldicion As Byte
		Dim Bendicion As Byte
		Dim Estupidez As Byte
		Dim Ceguera As Byte
		Dim Revivir As Byte
		Dim Morph As Byte
		Dim Mimetiza As Byte
		Dim RemueveInvisibilidadParcial As Byte
		Dim Warp As Byte
		Dim Invoca As Byte
		Dim NumNpc As Short
		Dim cant As Short
		'    Materializa As Byte
		'    ItemIndex As Byte
		Dim MinSkill As Short
		Dim ManaRequerido As Short
		'Barrin 29/9/03
		Dim StaRequerido As Short
		Dim Target As TargetType
		Dim NeedStaff As Short
		Dim StaffAffected As Boolean
	End Structure
	
	Public Structure LevelSkill
		Dim LevelValue As Short
	End Structure
	
	Public Structure UserOBJ
		Dim ObjIndex As Short
		Dim Amount As Short
		Dim Equipped As Byte
	End Structure
	
	Public Structure Inventario
		'UPGRADE_NOTE: Object se actualizó a Object_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		<VBFixedArray(MAX_INVENTORY_SLOTS)> Dim Object_Renamed() As UserOBJ
		Dim WeaponEqpObjIndex As Short
		Dim WeaponEqpSlot As Byte
		Dim ArmourEqpObjIndex As Short
		Dim ArmourEqpSlot As Byte
		Dim EscudoEqpObjIndex As Short
		Dim EscudoEqpSlot As Byte
		Dim CascoEqpObjIndex As Short
		Dim CascoEqpSlot As Byte
		Dim MunicionEqpObjIndex As Short
		Dim MunicionEqpSlot As Byte
		Dim AnilloEqpObjIndex As Short
		Dim AnilloEqpSlot As Byte
		Dim BarcoObjIndex As Short
		Dim BarcoSlot As Byte
		Dim MochilaEqpObjIndex As Short
		Dim MochilaEqpSlot As Byte
		Dim NroItems As Short
	End Structure
	
	Public Structure tPartyData
		Dim PIndex As Short
		Dim RemXP As Double 'La exp. en el server se cuenta con Doubles
		Dim TargetUser As Short 'Para las invitaciones
	End Structure
	
	Public Structure Position
		Dim X As Short
		Dim Y As Short
	End Structure
	
	Public Structure WorldPos
		Dim Map As Short
		Dim X As Short
		Dim Y As Short
	End Structure
	
	Public Structure FXdata
		Dim Nombre As String
		Dim GrhIndex As Short
		Dim Delay As Short
	End Structure
	
	'Datos de user o npc
	'UPGRADE_NOTE: Char se actualizó a Char_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Structure Char_Renamed
		Dim CharIndex As Short
		Dim Head As Short
		Dim body As Short
		Dim WeaponAnim As Short
		Dim ShieldAnim As Short
		Dim CascoAnim As Short
		Dim FX As Short
		Dim loops As Short
		Dim heading As eHeading
	End Structure
	
	'Tipos de objetos
	Public Structure ObjData
		Dim name As String 'Nombre del obj
		Dim OBJType As eOBJType 'Tipo enum que determina cuales son las caract del obj
		Dim GrhIndex As Short ' Indice del grafico que representa el obj
		Dim GrhSecundario As Short
		'Solo contenedores
		Dim MaxItems As Short
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Conte, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim Conte As Inventario
		Dim Apuñala As Byte
		Dim Acuchilla As Byte
		Dim HechizoIndex As Short
		Dim ForoID As String
		Dim MinHp As Short ' Minimo puntos de vida
		Dim MaxHp As Short ' Maximo puntos de vida
		Dim MineralIndex As Short
		Dim LingoteInex As Short
		Dim proyectil As Short
		Dim Municion As Short
		Dim Crucial As Byte
		Dim Newbie As Short
		'Puntos de Stamina que da
		Dim MinSta As Short ' Minimo puntos de stamina
		'Pociones
		Dim TipoPocion As Byte
		Dim MaxModificador As Short
		Dim MinModificador As Short
		Dim DuracionEfecto As Integer
		Dim MinSkill As Short
		Dim LingoteIndex As Short
		Dim MinHIT As Short 'Minimo golpe
		Dim MaxHIT As Short 'Maximo golpe
		Dim MinHam As Short
		Dim MinSed As Short
		Dim def As Short
		Dim MinDef As Short ' Armaduras
		Dim MaxDef As Short ' Armaduras
		Dim Ropaje As Short 'Indice del grafico del ropaje
		Dim WeaponAnim As Short ' Apunta a una anim de armas
		Dim WeaponRazaEnanaAnim As Short
		Dim ShieldAnim As Short ' Apunta a una anim de escudo
		Dim CascoAnim As Short
		Dim Valor As Integer ' Precio
		Dim Cerrada As Short
		Dim Llave As Byte
		Dim clave As Integer 'si clave=llave la puerta se abre o cierra
		Dim Radio As Short ' Para teleps: El radio para calcular el random de la pos destino
		Dim MochilaType As Byte 'Tipo de Mochila (1 la chica, 2 la grande)
		Dim Guante As Byte ' Indica si es un guante o no.
		Dim IndexAbierta As Short
		Dim IndexCerrada As Short
		Dim IndexCerradaLlave As Short
		Dim RazaEnana As Byte
		Dim RazaDrow As Byte
		Dim RazaElfa As Byte
		Dim RazaGnoma As Byte
		Dim RazaHumana As Byte
		Dim Mujer As Byte
		Dim Hombre As Byte
		Dim Envenena As Byte
		Dim Paraliza As Byte
		Dim Agarrable As Byte
		Dim LingH As Short
		Dim LingO As Short
		Dim LingP As Short
		Dim Madera As Short
		Dim MaderaElfica As Short
		Dim SkHerreria As Short
		Dim SkCarpinteria As Short
		Dim texto As String
		'Clases que no tienen permitido usar este obj
		<VBFixedArray(NUMCLASES)> Dim ClaseProhibida() As eClass
		Dim Snd1 As Short
		Dim Snd2 As Short
		Dim Snd3 As Short
		Dim Real As Short
		Dim Caos As Short
		Dim NoSeCae As Short
		Dim StaffPower As Short
		Dim StaffDamageBonus As Short
		Dim DefensaMagicaMax As Short
		Dim DefensaMagicaMin As Short
		Dim Refuerzo As Byte
		Dim Log As Byte 'es un objeto que queremos loguear? Pablo (ToxicWaste) 07/09/07
		Dim NoLog As Byte 'es un objeto que esta prohibido loguear?
		Dim Upgrade As Short
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			'UPGRADE_WARNING: El límite inferior de la matriz ClaseProhibida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim ClaseProhibida(NUMCLASES)
		End Sub
	End Structure
	
	Public Structure Obj
		Dim ObjIndex As Short
		Dim Amount As Short
	End Structure
	
	'[Pablo ToxicWaste]
	Public Structure ModClase
		Dim Evasion As Double
		Dim AtaqueArmas As Double
		Dim AtaqueProyectiles As Double
		Dim AtaqueWrestling As Double
		Dim DañoArmas As Double
		Dim DañoProyectiles As Double
		Dim DañoWrestling As Double
		Dim Escudo As Double
	End Structure
	
	Public Structure ModRaza
		Dim Fuerza As Single
		Dim Agilidad As Single
		Dim Inteligencia As Single
		Dim Carisma As Single
		Dim Constitucion As Single
	End Structure
	'[/Pablo ToxicWaste]
	
	'[KEVIN]
	'Banco Objs
	Public Const MAX_BANCOINVENTORY_SLOTS As Byte = 40
	'[/KEVIN]
	
	'[KEVIN]
	Public Structure BancoInventario
		'UPGRADE_NOTE: Object se actualizó a Object_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		<VBFixedArray(MAX_BANCOINVENTORY_SLOTS)> Dim Object_Renamed() As UserOBJ
		Dim NroItems As Short
	End Structure
	'[/KEVIN]
	
	' Determina el color del nick
	Public Enum eNickColor
		ieCriminal = &H1
		ieCiudadano = &H2
		ieAtacable = &H4
	End Enum
	
	'*******
	'FOROS *
	'*******
	
	' Tipos de mensajes
	Public Enum eForumMsgType
		ieGeneral
		ieGENERAL_STICKY
		ieREAL
		ieREAL_STICKY
		ieCAOS
		ieCAOS_STICKY
	End Enum
	
	' Indica los privilegios para visualizar los diferentes foros
	Public Enum eForumVisibility
		ieGENERAL_MEMBER = &H1
		ieREAL_MEMBER = &H2
		ieCAOS_MEMBER = &H4
	End Enum
	
	' Indica el tipo de foro
	Public Enum eForumType
		ieGeneral
		ieREAL
		ieCAOS
	End Enum
	
	' Limite de posts
	Public Const MAX_STICKY_POST As Byte = 10
	Public Const MAX_GENERAL_POST As Byte = 35
	
	' Estructura contenedora de mensajes
	Public Structure tForo
		<VBFixedArray(MAX_STICKY_POST)> Dim StickyTitle() As String
		<VBFixedArray(MAX_STICKY_POST)> Dim StickyPost() As String
		<VBFixedArray(MAX_GENERAL_POST)> Dim GeneralTitle() As String
		<VBFixedArray(MAX_GENERAL_POST)> Dim GeneralPost() As String
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			'UPGRADE_WARNING: El límite inferior de la matriz StickyTitle ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim StickyTitle(MAX_STICKY_POST)
			'UPGRADE_WARNING: El límite inferior de la matriz StickyPost ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim StickyPost(MAX_STICKY_POST)
			'UPGRADE_WARNING: El límite inferior de la matriz GeneralTitle ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim GeneralTitle(MAX_GENERAL_POST)
			'UPGRADE_WARNING: El límite inferior de la matriz GeneralPost ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim GeneralPost(MAX_GENERAL_POST)
		End Sub
	End Structure
	
	'*********************************************************
	'*********************************************************
	'*********************************************************
	'*********************************************************
	'******* T I P O S   D E    U S U A R I O S **************
	'*********************************************************
	'*********************************************************
	'*********************************************************
	'*********************************************************
	
	Public Structure tReputacion 'Fama del usuario
		Dim NobleRep As Integer
		Dim BurguesRep As Integer
		Dim PlebeRep As Integer
		Dim LadronesRep As Integer
		Dim BandidoRep As Integer
		Dim AsesinoRep As Integer
		Dim Promedio As Integer
	End Structure
	
	'Estadisticas de los usuarios
	Public Structure UserStats
		Dim GLD As Integer 'Dinero
		Dim Banco As Integer
		Dim MaxHp As Short
		Dim MinHp As Short
		Dim MaxSta As Short
		Dim MinSta As Short
		Dim MaxMAN As Short
		Dim MinMAN As Short
		Dim MaxHIT As Short
		Dim MinHIT As Short
		Dim MaxHam As Short
		Dim MinHam As Short
		Dim MaxAGU As Short
		Dim MinAGU As Short
		Dim def As Short
		Dim Exp As Double
		Dim ELV As Byte
		Dim ELU As Integer
		<VBFixedArray(NUMSKILLS)> Dim UserSkills() As Byte
		<VBFixedArray(NUMATRIBUTOS)> Dim UserAtributos() As Byte
		<VBFixedArray(NUMATRIBUTOS)> Dim UserAtributosBackUP() As Byte
		<VBFixedArray(MAXUSERHECHIZOS)> Dim UserHechizos() As Short
		Dim UsuariosMatados As Integer
		Dim CriminalesMatados As Integer
		Dim NPCsMuertos As Short
		Dim SkillPts As Short
		<VBFixedArray(NUMSKILLS)> Dim ExpSkills() As Integer
		<VBFixedArray(NUMSKILLS)> Dim EluSkills() As Integer
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			'UPGRADE_WARNING: El límite inferior de la matriz UserSkills ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim UserSkills(NUMSKILLS)
			'UPGRADE_WARNING: El límite inferior de la matriz UserAtributos ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim UserAtributos(NUMATRIBUTOS)
			'UPGRADE_WARNING: El límite inferior de la matriz UserAtributosBackUP ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim UserAtributosBackUP(NUMATRIBUTOS)
			'UPGRADE_WARNING: El límite inferior de la matriz UserHechizos ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim UserHechizos(MAXUSERHECHIZOS)
			'UPGRADE_WARNING: El límite inferior de la matriz ExpSkills ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim ExpSkills(NUMSKILLS)
			'UPGRADE_WARNING: El límite inferior de la matriz EluSkills ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim EluSkills(NUMSKILLS)
		End Sub
	End Structure
	
	'Flags
	Public Structure UserFlags
		Dim Muerto As Byte '¿Esta muerto?
		Dim Escondido As Byte '¿Esta escondido?
		Dim Comerciando As Boolean '¿Esta comerciando?
		Dim UserLogged As Boolean '¿Esta online?
		Dim Meditando As Boolean
		Dim Descuento As String
		Dim Hambre As Byte
		Dim Sed As Byte
		Dim PuedeMoverse As Byte
		Dim TimerLanzarSpell As Integer
		Dim PuedeTrabajar As Byte
		Dim Envenenado As Byte
		Dim Paralizado As Byte
		Dim Inmovilizado As Byte
		Dim Estupidez As Byte
		Dim Ceguera As Byte
		Dim invisible As Byte
		Dim Maldicion As Byte
		Dim Bendicion As Byte
		Dim Oculto As Byte
		Dim Desnudo As Byte
		Dim Descansar As Boolean
		Dim Hechizo As Short
		Dim TomoPocion As Boolean
		Dim TipoPocion As Byte
		Dim NoPuedeSerAtacado As Boolean
		Dim AtacablePor As Short
		Dim ShareNpcWith As Short
		Dim Vuela As Byte
		Dim Navegando As Byte
		Dim Seguro As Boolean
		Dim SeguroResu As Boolean
		Dim DuracionEfecto As Integer
		Dim TargetNPC As Short ' Npc señalado por el usuario
		Dim TargetNpcTipo As eNPCType ' Tipo del npc señalado
		Dim OwnedNpc As Short ' Npc que le pertenece (no puede ser atacado)
		Dim NpcInv As Short
		Dim Ban As Byte
		Dim AdministrativeBan As Byte
		Dim TargetUser As Short ' Usuario señalado
		Dim TargetObj As Short ' Obj señalado
		Dim TargetObjMap As Short
		Dim TargetObjX As Short
		Dim TargetObjY As Short
		Dim TargetMap As Short
		Dim TargetX As Short
		Dim TargetY As Short
		Dim TargetObjInvIndex As Short
		Dim TargetObjInvSlot As Short
		Dim AtacadoPorNpc As Short
		Dim AtacadoPorUser As Short
		Dim NPCAtacado As Short
		Dim Ignorado As Boolean
		Dim EnConsulta As Boolean
		Dim StatsChanged As Byte
		Dim Privilegios As PlayerType
		Dim ValCoDe As Short
		Dim LastCrimMatado As String
		Dim LastCiudMatado As String
		Dim OldBody As Short
		Dim OldHead As Short
		Dim AdminInvisible As Byte
		Dim AdminPerseguible As Boolean
		Dim ChatColor As Integer
		'[el oso]
		Dim MD5Reportado As String
		'[/el oso]
		'[Barrin 30-11-03]
		Dim TimesWalk As Integer
		Dim StartWalk As Integer
		Dim CountSH As Integer
		'[/Barrin 30-11-03]
		'[CDT 17-02-04]
		Dim UltimoMensaje As Byte
		'[/CDT]
		Dim Silenciado As Byte
		Dim Mimetizado As Byte
		Dim CentinelaOK As Boolean 'Centinela
		Dim lastMap As Short
		Dim Traveling As Byte 'Travelin Band ¿?
	End Structure
	
	Public Structure UserCounters
		Dim IdleCount As Integer
		Dim AttackCounter As Short
		Dim HPCounter As Short
		Dim STACounter As Short
		Dim Frio As Short
		Dim Lava As Short
		Dim COMCounter As Short
		Dim AGUACounter As Short
		Dim Veneno As Short
		Dim Paralisis As Short
		Dim Ceguera As Short
		Dim Estupidez As Short
		Dim Invisibilidad As Short
		Dim TiempoOculto As Short
		Dim Mimetismo As Short
		Dim PiqueteC As Integer
		Dim Pena As Integer
		Dim SendMapCounter As WorldPos
		'[Gonzalo]
		Dim Saliendo As Boolean
		Dim salir As Short
		'[/Gonzalo]
		'Barrin 3/10/03
		Dim tInicioMeditar As Integer
		Dim bPuedeMeditar As Boolean
		'Barrin
		Dim TimerLanzarSpell As Integer
		Dim TimerPuedeAtacar As Integer
		Dim TimerPuedeUsarArco As Integer
		Dim TimerPuedeTrabajar As Integer
		Dim TimerUsar As Integer
		Dim TimerMagiaGolpe As Integer
		Dim TimerGolpeMagia As Integer
		Dim TimerGolpeUsar As Integer
		Dim TimerPuedeSerAtacado As Integer
		Dim TimerPerteneceNpc As Integer
		Dim TimerEstadoAtacable As Integer
		Dim Trabajando As Integer ' Para el centinela
		Dim Ocultando As Integer ' Unico trabajo no revisado por el centinela
		Dim failedUsageAttempts As Integer
		Dim goHome As Integer
		Dim AsignedSkills As Byte
	End Structure
	
	'Cosas faccionarias.
	Public Structure tFacciones
		Dim ArmadaReal As Byte
		Dim FuerzasCaos As Byte
		Dim CriminalesMatados As Integer
		Dim CiudadanosMatados As Integer
		Dim RecompensasReal As Integer
		Dim RecompensasCaos As Integer
		Dim RecibioExpInicialReal As Byte
		Dim RecibioExpInicialCaos As Byte
		Dim RecibioArmaduraReal As Byte
		Dim RecibioArmaduraCaos As Byte
		Dim Reenlistadas As Byte
		Dim NivelIngreso As Short
		Dim FechaIngreso As String
		Dim MatadosIngreso As Short 'Para Armadas nada mas
		Dim NextRecompensa As Short
	End Structure
	
	Public Structure tCrafting
		Dim Cantidad As Integer
		Dim PorCiclo As Short
	End Structure
	
	'Tipo de los Usuarios
	Public Structure User
		Dim name As String
		Dim ID As Integer
		Dim showName As Boolean 'Permite que los GMs oculten su nick con el comando /SHOWNAME
		'UPGRADE_NOTE: Char se actualizó a Char_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim Char_Renamed As Char_Renamed 'Define la apariencia
		Dim CharMimetizado As Char_Renamed
		Dim OrigChar As Char_Renamed
		Dim desc As String ' Descripcion
		Dim DescRM As String
		Dim clase As eClass
		Dim raza As eRaza
		Dim Genero As eGenero
		Dim email As String
		Dim Hogar As eCiudad
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Invent, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim Invent As Inventario
		Dim Pos As WorldPos
		Dim ConnIDValida As Boolean
		Dim ConnID As Short 'ID
		'[KEVIN]
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura BancoInvent, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim BancoInvent As BancoInventario
		'[/KEVIN]
		Dim Counters As UserCounters
		Dim Construir As tCrafting
		<VBFixedArray(MAXMASCOTAS)> Dim MascotasIndex() As Short
		<VBFixedArray(MAXMASCOTAS)> Dim MascotasType() As Short
		Dim NroMascotas As Short
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Stats, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim Stats As UserStats
		Dim flags As UserFlags
		Dim Reputacion As tReputacion
		Dim Faccion As tFacciones
		Dim LogOnTime As Date
		Dim UpTime As Integer
		Dim ip As String
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura ComUsu, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim ComUsu As tCOmercioUsuario
		Dim GuildIndex As Short 'puntero al array global de guilds
		Dim FundandoGuildAlineacion As modGuilds.ALINEACION_GUILD 'esto esta aca hasta que se parchee el cliente y se pongan cadenas de datos distintas para cada alineacion
		Dim EscucheClan As Short
		Dim PartyIndex As Short 'index a la party q es miembro
		Dim PartySolicitud As Short 'index a la party q solicito
		Dim KeyCrypt As Short
		Dim AreasInfo As AreaInfo
		'Outgoing and incoming messages
		Dim outgoingData As clsByteQueue
		Dim incomingData As clsByteQueue
		Dim CurrentInventorySlots As Byte
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			'UPGRADE_WARNING: El límite inferior de la matriz MascotasIndex ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim MascotasIndex(MAXMASCOTAS)
			'UPGRADE_WARNING: El límite inferior de la matriz MascotasType ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim MascotasType(MAXMASCOTAS)
			Stats.Initialize()
			ComUsu.Initialize()
		End Sub
	End Structure
	
	
	'*********************************************************
	'*********************************************************
	'*********************************************************
	'*********************************************************
	'**  T I P O S   D E    N P C S **************************
	'*********************************************************
	'*********************************************************
	'*********************************************************
	'*********************************************************
	
	Public Structure NPCStats
		Dim Alineacion As Short
		Dim MaxHp As Integer
		Dim MinHp As Integer
		Dim MaxHIT As Short
		Dim MinHIT As Short
		Dim def As Short
		Dim defM As Short
	End Structure
	
	Public Structure NpcCounters
		Dim Paralisis As Short
		Dim TiempoExistencia As Integer
	End Structure
	
	Public Structure NPCFlags
		Dim AfectaParalisis As Byte
		Dim Domable As Short
		Dim Respawn As Byte
		Dim NPCActive As Boolean '¿Esta vivo?
		Dim Follow As Boolean
		Dim Faccion As Byte
		Dim AtacaDoble As Byte
		Dim LanzaSpells As Byte
		Dim ExpCount As Integer
		Dim OldMovement As AI.TipoAI
		Dim OldHostil As Byte
		Dim AguaValida As Byte
		Dim TierraInvalida As Byte
		Dim Sound As Short
		Dim AttackedBy As String
		Dim AttackedFirstBy As String
		Dim BackUp As Byte
		Dim RespawnOrigPos As Byte
		Dim Envenenado As Byte
		Dim Paralizado As Byte
		Dim Inmovilizado As Byte
		Dim invisible As Byte
		Dim Maldicion As Byte
		Dim Bendicion As Byte
		Dim Snd1 As Short
		Dim Snd2 As Short
		Dim Snd3 As Short
	End Structure
	
	Public Structure tCriaturasEntrenador
		Dim NpcIndex As Short
		Dim NpcName As String
		Dim tmpIndex As Short
	End Structure
	
	' New type for holding the pathfinding info
	Public Structure NpcPathFindingInfo
		Dim Path() As tVertice ' This array holds the path
		Dim Target As Position ' The location where the NPC has to go
		Dim PathLenght As Short ' Number of steps *
		Dim CurPos As Short ' Current location of the npc
		Dim TargetUser As Short ' UserIndex chased
		Dim NoPath As Boolean ' If it is true there is no path to the target location
		'* By setting PathLenght to 0 we force the recalculation
		'  of the path, this is very useful. For example,
		'  if a NPC or a User moves over the npc's path, blocking
		'  its way, the function NpcLegalPos set PathLenght to 0
		'  forcing the seek of a new path.
	End Structure
	' New type for holding the pathfinding info
	
	Public Structure tDrops
		Dim ObjIndex As Short
		Dim Amount As Integer
	End Structure
	
	Public Const MAX_NPC_DROPS As Byte = 5
	
	Public Structure npc
		Dim name As String
		'UPGRADE_NOTE: Char se actualizó a Char_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim Char_Renamed As Char_Renamed 'Define como se vera
		Dim desc As String
		Dim NPCtype As eNPCType
		Dim Numero As Short
		Dim InvReSpawn As Byte
		Dim Comercia As Short
		Dim Target As Integer
		Dim TargetNPC As Integer
		Dim TipoItems As Short
		Dim Veneno As Byte
		Dim Pos As WorldPos 'Posicion
		Dim Orig As WorldPos
		Dim SkillDomar As Short
		Dim Movement As AI.TipoAI
		Dim Attackable As Byte
		Dim Hostile As Byte
		Dim PoderAtaque As Integer
		Dim PoderEvasion As Integer
		Dim Owner As Short
		Dim GiveEXP As Integer
		Dim GiveGLD As Integer
		<VBFixedArray(MAX_NPC_DROPS)> Dim Drop() As tDrops
		Dim Stats As NPCStats
		Dim flags As NPCFlags
		Dim Contadores As NpcCounters
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Invent, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim Invent As Inventario
		Dim CanAttack As Byte
		Dim NroExpresiones As Byte
		Dim Expresiones() As String ' le da vida ;)
		Dim NroSpells As Byte
		Dim Spells() As Short ' le da vida ;)
		'<<<<Entrenadores>>>>>
		Dim NroCriaturas As Short
		Dim Criaturas() As tCriaturasEntrenador
		Dim MaestroUser As Short
		Dim MaestroNpc As Short
		Dim Mascotas As Short
		' New!! Needed for pathfindig
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura PFINFO, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim PFINFO As NpcPathFindingInfo
		Dim AreasInfo As AreaInfo
		'Hogar
		Dim Ciudad As Byte
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			'UPGRADE_WARNING: El límite inferior de la matriz Drop ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim Drop(MAX_NPC_DROPS)
		End Sub
	End Structure
	
	'**********************************************************
	'**********************************************************
	'******************** Tipos del mapa **********************
	'**********************************************************
	'**********************************************************
	'Tile
	Public Structure MapBlock
		Dim Blocked As Byte
		<VBFixedArray(4)> Dim Graphic() As Short
		Dim UserIndex As Short
		Dim NpcIndex As Short
		Dim ObjInfo As Obj
		Dim TileExit As WorldPos
		Dim trigger As eTrigger
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			'UPGRADE_WARNING: El límite inferior de la matriz Graphic ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim Graphic(4)
		End Sub
	End Structure
	
	'Info del mapa
	Structure MapInfo
		Dim NumUsers As Short
		Dim Music As String
		Dim name As String
		Dim startPos As WorldPos
		Dim MapVersion As Short
		Dim Pk As Boolean
		Dim MagiaSinEfecto As Byte
		Dim NoEncriptarMP As Byte
		Dim InviSinEfecto As Byte
		Dim ResuSinEfecto As Byte
		Dim RoboNpcsPermitido As Byte
		Dim Terreno As String
		Dim Zona As String
		Dim Restringir As String
		Dim BackUp As Byte
	End Structure
	
	
	'********** V A R I A B L E S     P U B L I C A S ***********
	
	Public SERVERONLINE As Boolean
	Public ULTIMAVERSION As String
	Public BackUp As Boolean ' TODO: Se usa esta variable ?
	
	'UPGRADE_WARNING: El límite inferior de la matriz ListaRazas ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public ListaRazas(NUMRAZAS) As String
	'UPGRADE_WARNING: El límite inferior de la matriz SkillsNames ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public SkillsNames(NUMSKILLS) As String
	'UPGRADE_WARNING: El límite inferior de la matriz ListaClases ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public ListaClases(NUMCLASES) As String
	'UPGRADE_WARNING: El límite inferior de la matriz ListaAtributos ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public ListaAtributos(NUMATRIBUTOS) As String
	
	
	Public recordusuarios As Integer
	
	'
	'Directorios
	'
	
	''
	'Ruta base del server, en donde esta el "server.ini"
	Public IniPath As String
	
	''
	'Ruta base para guardar los chars
	Public CharPath As String
	
	''
	'Ruta base para los archivos de mapas
	Public MapPath As String
	
	''
	'Ruta base para los DATs
	Public DatPath As String
	
	''
	'Bordes del mapa
	Public MinXBorder As Byte
	Public MaxXBorder As Byte
	Public MinYBorder As Byte
	Public MaxYBorder As Byte
	
	''
	'Numero de usuarios actual
	Public NumUsers As Short
	Public LastUser As Short
	Public LastChar As Short
	Public NumChars As Short
	Public LastNPC As Short
	Public NumNPCs As Short
	Public NumFX As Short
	Public NumMaps As Short
	Public NumObjDatas As Short
	Public NumeroHechizos As Short
	Public AllowMultiLogins As Byte
	Public IdleLimit As Short
	Public MaxUsers As Short
	Public HideMe As Byte
	Public LastBackup As String
	Public Minutos As String
	Public haciendoBK As Boolean
	Public PuedeCrearPersonajes As Short
	Public ServerSoloGMs As Short
	
	''
	'Esta activada la verificacion MD5 ?
	Public MD5ClientesActivado As Byte
	
	
	Public EnPausa As Boolean
	Public EnTesting As Boolean
	
	
	'*****************ARRAYS PUBLICOS*************************
	Public UserList() As User 'USUARIOS
	'UPGRADE_WARNING: El límite inferior de la matriz Npclist ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	'UPGRADE_WARNING: Es posible que la matriz Npclist necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
	Public Npclist(MAXNPCS) As npc 'NPCS
	Public MapData(,,) As MapBlock
	'UPGRADE_NOTE: MapInfo se actualizó a MapInfo_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public MapInfo_Renamed() As MapInfo
	Public Hechizos() As tHechizo
	'UPGRADE_WARNING: El límite inferior de la matriz CharList ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public CharList(MAXCHARS) As Short
	'UPGRADE_NOTE: ObjData se actualizó a ObjData_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public ObjData_Renamed() As ObjData
	Public FX() As FXdata
	Public SpawnList() As tCriaturasEntrenador
	'UPGRADE_WARNING: El límite inferior de la matriz LevelSkill_Renamed ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	'UPGRADE_NOTE: LevelSkill se actualizó a LevelSkill_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public LevelSkill_Renamed(50) As LevelSkill
	Public ForbidenNames() As String
	Public ArmasHerrero() As Short
	Public ArmadurasHerrero() As Short
	Public ObjCarpintero() As Short
	Public MD5s() As String
	Public BanIps As New Collection
	'UPGRADE_WARNING: El límite inferior de la matriz Parties ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public Parties(MAX_PARTIES) As clsParty
	'UPGRADE_WARNING: El límite inferior de la matriz ModClase_Renamed ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	'UPGRADE_NOTE: ModClase se actualizó a ModClase_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public ModClase_Renamed(NUMCLASES) As ModClase
	'UPGRADE_WARNING: El límite inferior de la matriz ModRaza_Renamed ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	'UPGRADE_NOTE: ModRaza se actualizó a ModRaza_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public ModRaza_Renamed(NUMRAZAS) As ModRaza
	'UPGRADE_WARNING: El límite inferior de la matriz ModVida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public ModVida(NUMCLASES) As Double
	'UPGRADE_WARNING: El límite inferior de la matriz DistribucionEnteraVida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public DistribucionEnteraVida(5) As Short
	'UPGRADE_WARNING: El límite inferior de la matriz DistribucionSemienteraVida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public DistribucionSemienteraVida(4) As Short
	'UPGRADE_WARNING: El límite inferior de la matriz Ciudades ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Public Ciudades(NUMCIUDADES) As WorldPos
	Public distanceToCities() As HomeDistance
	'*********************************************************
	
	Structure HomeDistance
		<VBFixedArray(5)> Dim distanceToCity() As Short
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			'UPGRADE_WARNING: El límite inferior de la matriz distanceToCity ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim distanceToCity(5)
		End Sub
	End Structure
	
	Public Nix As WorldPos
	Public Ullathorpe As WorldPos
	Public Banderbill As WorldPos
	Public Lindos As WorldPos
	Public Arghal As WorldPos
	
	Public Prision As WorldPos
	Public Libertad As WorldPos
	
	Public Ayuda As New cCola
	Public ConsultaPopular As New ConsultasPopulares
	Public SonidosMapas As New SoundMapInfo
	
	Public Enum e_ObjetosCriticos
		Manzana = 1
		Manzana2 = 2
		ManzanaNewbie = 467
	End Enum
	
	Public Enum eMessages
		DontSeeAnything
		NPCSwing
		NPCKillUser
		BlockedWithShieldUser
		BlockedWithShieldother
		UserSwing
		SafeModeOn
		SafeModeOff
		ResuscitationSafeOff
		ResuscitationSafeOn
		NobilityLost
		CantUseWhileMeditating
		NPCHitUser
		UserHitNPC
		UserAttackedSwing
		UserHittedByUser
		UserHittedUser
		WorkRequestTarget
		HaveKilledUser
		UserKill
		EarnExp
		Home
		CancelHome
		FinishHome
	End Enum
	
	Public Enum eGMCommands
		GMMessage = 1 '/GMSG
		showName '/SHOWNAME
		OnlineRoyalArmy '/ONLINEREAL
		OnlineChaosLegion '/ONLINECAOS
		GoNearby '/IRCERCA
		comment '/REM
		serverTime '/HORA
		Where '/DONDE
		CreaturesInMap '/NENE
		WarpMeToTarget '/TELEPLOC
		WarpChar '/TELEP
		Silence '/SILENCIAR
		SOSShowList '/SHOW SOS
		SOSRemove 'SOSDONE
		GoToChar '/IRA
		invisible '/INVISIBLE
		GMPanel '/PANELGM
		RequestUserList 'LISTUSU
		Working '/TRABAJANDO
		Hiding '/OCULTANDO
		Jail '/CARCEL
		KillNPC '/RMATA
		WarnUser '/ADVERTENCIA
		EditChar '/MOD
		RequestCharInfo '/INFO
		RequestCharStats '/STAT
		RequestCharGold '/BAL
		RequestCharInventory '/INV
		RequestCharBank '/BOV
		RequestCharSkills '/SKILLS
		ReviveChar '/REVIVIR
		OnlineGM '/ONLINEGM
		OnlineMap '/ONLINEMAP
		Forgive '/PERDON
		Kick '/ECHAR
		Execute '/EJECUTAR
		BanChar '/BAN
		UnbanChar '/UNBAN
		NPCFollow '/SEGUIR
		SummonChar '/SUM
		SpawnListRequest '/CC
		SpawnCreature 'SPA
		ResetNPCInventory '/RESETINV
		CleanWorld '/LIMPIAR
		ServerMessage '/RMSG
		NickToIP '/NICK2IP
		IPToNick '/IP2NICK
		GuildOnlineMembers '/ONCLAN
		TeleportCreate '/CT
		TeleportDestroy '/DT
		RainToggle '/LLUVIA
		SetCharDescription '/SETDESC
		ForceMIDIToMap '/FORCEMIDIMAP
		ForceWAVEToMap '/FORCEWAVMAP
		RoyalArmyMessage '/REALMSG
		ChaosLegionMessage '/CAOSMSG
		CitizenMessage '/CIUMSG
		CriminalMessage '/CRIMSG
		TalkAsNPC '/TALKAS
		DestroyAllItemsInArea '/MASSDEST
		AcceptRoyalCouncilMember '/ACEPTCONSE
		AcceptChaosCouncilMember '/ACEPTCONSECAOS
		ItemsInTheFloor '/PISO
		MakeDumb '/ESTUPIDO
		MakeDumbNoMore '/NOESTUPIDO
		DumpIPTables '/DUMPSECURITY
		CouncilKick '/KICKCONSE
		SetTrigger '/TRIGGER
		AskTrigger '/TRIGGER with no args
		BannedIPList '/BANIPLIST
		BannedIPReload '/BANIPRELOAD
		GuildMemberList '/MIEMBROSCLAN
		GuildBan '/BANCLAN
		BanIP '/BANIP
		UnbanIP '/UNBANIP
		CreateItem '/CI
		DestroyItems '/DEST
		ChaosLegionKick '/NOCAOS
		RoyalArmyKick '/NOREAL
		ForceMIDIAll '/FORCEMIDI
		ForceWAVEAll '/FORCEWAV
		RemovePunishment '/BORRARPENA
		TileBlockedToggle '/BLOQ
		KillNPCNoRespawn '/MATA
		KillAllNearbyNPCs '/MASSKILL
		LastIP '/LASTIP
		ChangeMOTD '/MOTDCAMBIA
		SetMOTD 'ZMOTD
		SystemMessage '/SMSG
		CreateNPC '/ACC
		CreateNPCWithRespawn '/RACC
		ImperialArmour '/AI1 - 4
		ChaosArmour '/AC1 - 4
		NavigateToggle '/NAVE
		ServerOpenToUsersToggle '/HABILITAR
		TurnOffServer '/APAGAR
		TurnCriminal '/CONDEN
		ResetFactions '/RAJAR
		RemoveCharFromGuild '/RAJARCLAN
		RequestCharMail '/LASTEMAIL
		AlterPassword '/APASS
		AlterMail '/AEMAIL
		AlterName '/ANAME
		ToggleCentinelActivated '/CENTINELAACTIVADO
		DoBackUp '/DOBACKUP
		ShowGuildMessages '/SHOWCMSG
		SaveMap '/GUARDAMAPA
		ChangeMapInfoPK '/MODMAPINFO PK
		ChangeMapInfoBackup '/MODMAPINFO BACKUP
		ChangeMapInfoRestricted '/MODMAPINFO RESTRINGIR
		ChangeMapInfoNoMagic '/MODMAPINFO MAGIASINEFECTO
		ChangeMapInfoNoInvi '/MODMAPINFO INVISINEFECTO
		ChangeMapInfoNoResu '/MODMAPINFO RESUSINEFECTO
		ChangeMapInfoLand '/MODMAPINFO TERRENO
		ChangeMapInfoZone '/MODMAPINFO ZONA
		SaveChars '/GRABAR
		CleanSOS '/BORRAR SOS
		ShowServerForm '/SHOW INT
		night '/NOCHE
		KickAllChars '/ECHARTODOSPJS
		ReloadNPCs '/RELOADNPCS
		ReloadServerIni '/RELOADSINI
		ReloadSpells '/RELOADHECHIZOS
		ReloadObjects '/RELOADOBJ
		Restart '/REINICIAR
		ResetAutoUpdate '/AUTOUPDATE
		ChatColor '/CHATCOLOR
		Ignored '/IGNORADO
		CheckSlot '/SLOT
		SetIniVar '/SETINIVAR LLAVE CLAVE VALOR
	End Enum
	
	Public Const MATRIX_INITIAL_MAP As Short = 1
	
	Public Const GOHOME_PENALTY As Short = 5
	Public Const GM_MAP As Short = 49
	
	Public Const TELEP_OBJ_INDEX As Short = 1012
	
	Public Const HUMANO_H_PRIMER_CABEZA As Short = 1
	Public Const HUMANO_H_ULTIMA_CABEZA As Short = 40 'En verdad es hasta la 51, pero como son muchas estas las dejamos no seleccionables
	
	Public Const ELFO_H_PRIMER_CABEZA As Short = 101
	Public Const ELFO_H_ULTIMA_CABEZA As Short = 122
	
	Public Const DROW_H_PRIMER_CABEZA As Short = 201
	Public Const DROW_H_ULTIMA_CABEZA As Short = 221
	
	Public Const ENANO_H_PRIMER_CABEZA As Short = 301
	Public Const ENANO_H_ULTIMA_CABEZA As Short = 319
	
	Public Const GNOMO_H_PRIMER_CABEZA As Short = 401
	Public Const GNOMO_H_ULTIMA_CABEZA As Short = 416
	'**************************************************
	Public Const HUMANO_M_PRIMER_CABEZA As Short = 70
	Public Const HUMANO_M_ULTIMA_CABEZA As Short = 89
	
	Public Const ELFO_M_PRIMER_CABEZA As Short = 170
	Public Const ELFO_M_ULTIMA_CABEZA As Short = 188
	
	Public Const DROW_M_PRIMER_CABEZA As Short = 270
	Public Const DROW_M_ULTIMA_CABEZA As Short = 288
	
	Public Const ENANO_M_PRIMER_CABEZA As Short = 370
	Public Const ENANO_M_ULTIMA_CABEZA As Short = 384
	
	Public Const GNOMO_M_PRIMER_CABEZA As Short = 470
	Public Const GNOMO_M_ULTIMA_CABEZA As Short = 484
	
	' Por ahora la dejo constante.. SI se quisiera extender la propiedad de paralziar, se podria hacer
	' una nueva variable en el dat.
	Public Const GUANTE_HURTO As Short = 873
End Module