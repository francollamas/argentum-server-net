using System;
using System.IO;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class modForum
{
    public const byte MAX_MENSAJES_FORO = 30;
    public const byte MAX_ANUNCIOS_FORO = 5;

    public const string FORO_REAL_ID = "REAL";
    public const string FORO_CAOS_ID = "CAOS";

    private static short NumForos;
    private static tForo[] Foros;


    public static void AddForum(string sForoID)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 22/02/2010
        // Adds a forum to the list and fills it.
        // ***************************************************
        string ForumPath;
        string PostPath;
        short PostIndex;
        short FileIndex;

        NumForos = Convert.ToInt16(NumForos + 1);
        // UPGRADE_WARNING: Es posible que la matriz Foros necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
        // UPGRADE_WARNING: El límite inferior de la matriz Foros ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        Array.Resize(ref Foros, NumForos + 1);
        ArrayInitializers.InitializeStruct(Foros);

        ForumPath = AppDomain.CurrentDomain.BaseDirectory + "foros/" + sForoID + ".for";

        {
            ref var withBlock = ref Foros[NumForos];

            withBlock.ID = sForoID;

            if (File.Exists(ForumPath))
            {
                var argEmptySpaces = 1024;
                withBlock.CantPosts =
                    Convert.ToByte(Migration.ParseVal(ES.GetVar(ForumPath, "INFO", "CantMSG", ref argEmptySpaces)));
                var argEmptySpaces1 = 1024;
                withBlock.CantAnuncios =
                    Convert.ToByte(
                        Migration.ParseVal(ES.GetVar(ForumPath, "INFO", "CantAnuncios", ref argEmptySpaces1)));

                // Cargo posts
                var loopTo = (short)withBlock.CantPosts;
                for (PostIndex = 1; PostIndex <= loopTo; PostIndex++)
                {
                    PostPath = AppDomain.CurrentDomain.BaseDirectory + "foros/" + sForoID + PostIndex + ".for";

                    using (var reader = new StreamReader(PostPath))
                    {
                        // Titulo
                        withBlock.vsPost[PostIndex].sTitulo = reader.ReadLine();
                        // Autor
                        withBlock.vsPost[PostIndex].Autor = reader.ReadLine();
                        // Mensaje
                        withBlock.vsPost[PostIndex].sPost = reader.ReadLine();
                    }
                }

                // Cargo anuncios
                var loopTo1 = (short)withBlock.CantAnuncios;
                for (PostIndex = 1; PostIndex <= loopTo1; PostIndex++)
                {
                    PostPath = AppDomain.CurrentDomain.BaseDirectory + "foros/" + sForoID + PostIndex + "a.for";

                    using (var reader = new StreamReader(PostPath))
                    {
                        // Titulo
                        withBlock.vsAnuncio[PostIndex].sTitulo = reader.ReadLine();
                        // Autor
                        withBlock.vsAnuncio[PostIndex].Autor = reader.ReadLine();
                        // Mensaje
                        withBlock.vsAnuncio[PostIndex].sPost = reader.ReadLine();
                    }
                }
            }
        }
    }

    public static short GetForumIndex(ref string sForoID)
    {
        short GetForumIndexRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 22/02/2010
        // Returns the forum index.
        // ***************************************************

        short ForumIndex;

        var loopTo = NumForos;
        for (ForumIndex = 1; ForumIndex <= loopTo; ForumIndex++)
            if ((Foros[ForumIndex].ID ?? "") == (sForoID ?? ""))
            {
                GetForumIndexRet = ForumIndex;
                return GetForumIndexRet;
            }

        return GetForumIndexRet;
    }

    public static void AddPost(short ForumIndex, ref string Post, ref string Autor, ref string Titulo, bool bAnuncio)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 22/02/2010
        // Saves a new post into the forum.
        // ***************************************************

        {
            ref var withBlock = ref Foros[ForumIndex];

            if (bAnuncio)
            {
                if (withBlock.CantAnuncios < MAX_ANUNCIOS_FORO)
                    withBlock.CantAnuncios = Convert.ToByte(withBlock.CantAnuncios + 1);

                MoveArray(ForumIndex, bAnuncio);

                // Agrego el anuncio
                {
                    ref var withBlock1 = ref withBlock.vsAnuncio[1];
                    withBlock1.sTitulo = Titulo;
                    withBlock1.Autor = Autor;
                    withBlock1.sPost = Post;
                }
            }

            else
            {
                if (withBlock.CantPosts < MAX_MENSAJES_FORO)
                    withBlock.CantPosts = Convert.ToByte(withBlock.CantPosts + 1);

                MoveArray(ForumIndex, bAnuncio);

                // Agrego el post
                {
                    ref var withBlock2 = ref withBlock.vsPost[1];
                    withBlock2.sTitulo = Titulo;
                    withBlock2.Autor = Autor;
                    withBlock2.sPost = Post;
                }
            }
        }
    }

    public static void SaveForums()
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 22/02/2010
        // Saves all forums into disk.
        // ***************************************************
        short ForumIndex;

        var loopTo = NumForos;
        for (ForumIndex = 1; ForumIndex <= loopTo; ForumIndex++)
            SaveForum(ForumIndex);
    }


    private static void SaveForum(short ForumIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 22/02/2010
        // Saves a forum into disk.
        // ***************************************************

        short PostIndex;
        short FileIndex;
        string PostPath;

        CleanForum(ForumIndex);

        {
            ref var withBlock = ref Foros[ForumIndex];

            // Guardo info del foro
            ES.WriteVar(AppDomain.CurrentDomain.BaseDirectory + "Foros/" + withBlock.ID + ".for", "INFO", "CantMSG",
                withBlock.CantPosts.ToString());
            ES.WriteVar(AppDomain.CurrentDomain.BaseDirectory + "Foros/" + withBlock.ID + ".for", "INFO",
                "CantAnuncios", withBlock.CantAnuncios.ToString());

            // Guardo posts
            var loopTo = (short)withBlock.CantPosts;
            for (PostIndex = 1; PostIndex <= loopTo; PostIndex++)
            {
                PostPath = AppDomain.CurrentDomain.BaseDirectory + "Foros/" + withBlock.ID + PostIndex + ".for";
                using (var writer = new StreamWriter(PostPath))
                {
                    writer.WriteLine(withBlock.vsPost[PostIndex].sTitulo);
                    writer.WriteLine(withBlock.vsPost[PostIndex].Autor);
                    writer.WriteLine(withBlock.vsPost[PostIndex].sPost);
                }
            }

            // Guardo Anuncios
            var loopTo1 = (short)withBlock.CantAnuncios;
            for (PostIndex = 1; PostIndex <= loopTo1; PostIndex++)
            {
                PostPath = AppDomain.CurrentDomain.BaseDirectory + "Foros/" + withBlock.ID + PostIndex + "a.for";
                using (var writer = new StreamWriter(PostPath))
                {
                    writer.WriteLine(withBlock.vsAnuncio[PostIndex].sTitulo);
                    writer.WriteLine(withBlock.vsAnuncio[PostIndex].Autor);
                    writer.WriteLine(withBlock.vsAnuncio[PostIndex].sPost);
                }
            }
        }
    }

    public static void CleanForum(short ForumIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 22/02/2010
        // Cleans a forum from disk.
        // ***************************************************
        short PostIndex;
        short NumPost;
        string ForumPath;

        {
            ref var withBlock = ref Foros[ForumIndex];

            // Elimino todo
            ForumPath = AppDomain.CurrentDomain.BaseDirectory + "Foros/" + withBlock.ID + ".for";
            if (File.Exists(ForumPath))
            {
                var argEmptySpaces = 1024;
                NumPost = Convert.ToInt16(
                    Migration.ParseVal(ES.GetVar(ForumPath, "INFO", "CantMSG", ref argEmptySpaces)));

                // Elimino los post viejos
                var loopTo = NumPost;
                for (PostIndex = 1; PostIndex <= loopTo; PostIndex++)
                    FileSystem.Kill(
                        AppDomain.CurrentDomain.BaseDirectory + "Foros/" + withBlock.ID + PostIndex + ".for");


                var argEmptySpaces1 = 1024;
                NumPost = Convert.ToInt16(Migration.ParseVal(ES.GetVar(ForumPath, "INFO", "CantAnuncios",
                    ref argEmptySpaces1)));

                // Elimino los post viejos
                var loopTo1 = NumPost;
                for (PostIndex = 1; PostIndex <= loopTo1; PostIndex++)
                    FileSystem.Kill(AppDomain.CurrentDomain.BaseDirectory + "Foros/" + withBlock.ID + PostIndex +
                                    "a.for");


                // Elimino el foro
                FileSystem.Kill(AppDomain.CurrentDomain.BaseDirectory + "Foros/" + withBlock.ID + ".for");
            }
        }
    }

    public static bool SendPosts(short UserIndex, ref string ForoID)
    {
        bool SendPostsRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 22/02/2010
        // Sends all the posts of a required forum
        // ***************************************************

        short ForumIndex;
        short PostIndex;
        bool bEsGm;

        ForumIndex = GetForumIndex(ref ForoID);

        if (ForumIndex > 0)
        {
            {
                ref var withBlock = ref Foros[ForumIndex];

                // Send General posts
                var loopTo = (short)withBlock.CantPosts;
                for (PostIndex = 1; PostIndex <= loopTo; PostIndex++)
                {
                    ref var withBlock1 = ref withBlock.vsPost[PostIndex];
                    Protocol.WriteAddForumMsg(UserIndex,
                        (Declaraciones.eForumType)Declaraciones.eForumMsgType.ieGeneral, withBlock1.sTitulo,
                        withBlock1.Autor, withBlock1.sPost);
                }

                // Send Sticky posts
                var loopTo1 = (short)withBlock.CantAnuncios;
                for (PostIndex = 1; PostIndex <= loopTo1; PostIndex++)
                {
                    ref var withBlock2 = ref withBlock.vsAnuncio[PostIndex];
                    Protocol.WriteAddForumMsg(UserIndex,
                        (Declaraciones.eForumType)Declaraciones.eForumMsgType.ieGENERAL_STICKY, withBlock2.sTitulo,
                        withBlock2.Autor, withBlock2.sPost);
                }
            }

            bEsGm = Extra.EsGM(UserIndex);

            // Caos?
            if (Extra.esCaos(UserIndex) | bEsGm)
            {
                var argsForoID = FORO_CAOS_ID;
                ForumIndex = GetForumIndex(ref argsForoID);

                {
                    ref var withBlock3 = ref Foros[ForumIndex];

                    // Send General Caos posts
                    var loopTo2 = (short)withBlock3.CantPosts;
                    for (PostIndex = 1; PostIndex <= loopTo2; PostIndex++)
                    {
                        ref var withBlock4 = ref withBlock3.vsPost[PostIndex];
                        Protocol.WriteAddForumMsg(UserIndex,
                            (Declaraciones.eForumType)Declaraciones.eForumMsgType.ieCAOS, withBlock4.sTitulo,
                            withBlock4.Autor, withBlock4.sPost);
                    }

                    // Send Sticky posts
                    var loopTo3 = (short)withBlock3.CantAnuncios;
                    for (PostIndex = 1; PostIndex <= loopTo3; PostIndex++)
                    {
                        ref var withBlock5 = ref withBlock3.vsAnuncio[PostIndex];
                        Protocol.WriteAddForumMsg(UserIndex,
                            (Declaraciones.eForumType)Declaraciones.eForumMsgType.ieCAOS_STICKY, withBlock5.sTitulo,
                            withBlock5.Autor, withBlock5.sPost);
                    }
                }
            }

            // Caos?
            if (Extra.esArmada(UserIndex) | bEsGm)
            {
                var argsForoID1 = FORO_REAL_ID;
                ForumIndex = GetForumIndex(ref argsForoID1);

                {
                    ref var withBlock6 = ref Foros[ForumIndex];

                    // Send General Real posts
                    var loopTo4 = (short)withBlock6.CantPosts;
                    for (PostIndex = 1; PostIndex <= loopTo4; PostIndex++)
                    {
                        ref var withBlock7 = ref withBlock6.vsPost[PostIndex];
                        Protocol.WriteAddForumMsg(UserIndex,
                            (Declaraciones.eForumType)Declaraciones.eForumMsgType.ieREAL, withBlock7.sTitulo,
                            withBlock7.Autor, withBlock7.sPost);
                    }

                    // Send Sticky posts
                    var loopTo5 = (short)withBlock6.CantAnuncios;
                    for (PostIndex = 1; PostIndex <= loopTo5; PostIndex++)
                    {
                        ref var withBlock8 = ref withBlock6.vsAnuncio[PostIndex];
                        Protocol.WriteAddForumMsg(UserIndex,
                            (Declaraciones.eForumType)Declaraciones.eForumMsgType.ieREAL_STICKY, withBlock8.sTitulo,
                            withBlock8.Autor, withBlock8.sPost);
                    }
                }
            }

            SendPostsRet = true;
        }

        return SendPostsRet;
    }

    public static bool EsAnuncio(byte ForumType)
    {
        bool EsAnuncioRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 22/02/2010
        // Returns true if the post is sticky.
        // ***************************************************
        switch (ForumType)
        {
            case (byte)Declaraciones.eForumMsgType.ieCAOS_STICKY:
            {
                EsAnuncioRet = true;
                break;
            }

            case (byte)Declaraciones.eForumMsgType.ieGENERAL_STICKY:
            {
                EsAnuncioRet = true;
                break;
            }

            case (byte)Declaraciones.eForumMsgType.ieREAL_STICKY:
            {
                EsAnuncioRet = true;
                break;
            }
        }

        return EsAnuncioRet;
    }

    public static byte ForumAlignment(byte yForumType)
    {
        byte ForumAlignmentRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 01/03/2010
        // Returns the forum alignment.
        // ***************************************************
        switch (yForumType)
        {
            case (byte)Declaraciones.eForumMsgType.ieCAOS:
            case (byte)Declaraciones.eForumMsgType.ieCAOS_STICKY:
            {
                ForumAlignmentRet = Convert.ToByte((byte)Declaraciones.eForumType.ieCAOS);
                break;
            }

            case (byte)Declaraciones.eForumMsgType.ieGeneral:
            case (byte)Declaraciones.eForumMsgType.ieGENERAL_STICKY:
            {
                ForumAlignmentRet = Convert.ToByte((byte)Declaraciones.eForumType.ieGeneral);
                break;
            }

            case (byte)Declaraciones.eForumMsgType.ieREAL:
            case (byte)Declaraciones.eForumMsgType.ieREAL_STICKY:
            {
                ForumAlignmentRet = Convert.ToByte((byte)Declaraciones.eForumType.ieREAL);
                break;
            }
        }

        return ForumAlignmentRet;
    }

    public static void ResetForums()
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 22/02/2010
        // Resets forum info
        // ***************************************************
        // UPGRADE_WARNING: Es posible que la matriz Foros necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
        // UPGRADE_WARNING: El límite inferior de la matriz Foros ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        Foros = new tForo[2];
        ArrayInitializers.InitializeStruct(Foros);
        NumForos = 0;
    }

    private static void MoveArray(short ForumIndex, bool Sticky)
    {
        int i;

        {
            ref var withBlock = ref Foros[ForumIndex];
            if (Sticky)
                for (i = withBlock.CantAnuncios; i >= 2; i -= 1)
                {
                    withBlock.vsAnuncio[i].sTitulo = withBlock.vsAnuncio[i - 1].sTitulo;
                    withBlock.vsAnuncio[i].sPost = withBlock.vsAnuncio[i - 1].sPost;
                    withBlock.vsAnuncio[i].Autor = withBlock.vsAnuncio[i - 1].Autor;
                }
            else
                for (i = withBlock.CantPosts; i >= 2; i -= 1)
                {
                    withBlock.vsPost[i].sTitulo = withBlock.vsPost[i - 1].sTitulo;
                    withBlock.vsPost[i].sPost = withBlock.vsPost[i - 1].sPost;
                    withBlock.vsPost[i].Autor = withBlock.vsPost[i - 1].Autor;
                }
        }
    }

    public struct tPost
    {
        public string sTitulo;
        public string sPost;
        public string Autor;
    }

    public struct tForo
    {
        [VBFixedArray(MAX_MENSAJES_FORO)] public tPost[] vsPost;
        [VBFixedArray(MAX_ANUNCIOS_FORO)] public tPost[] vsAnuncio;
        public byte CantPosts;
        public byte CantAnuncios;
        public string ID;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz vsPost ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            vsPost = new tPost[MAX_MENSAJES_FORO + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz vsAnuncio ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            vsAnuncio = new tPost[MAX_ANUNCIOS_FORO + 1];
        }
    }
}