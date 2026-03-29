using System;
using System.Collections.Generic;

namespace Legacy;

internal static class wskapiAO
{
    /// <summary>
    ///     Maps socket IDs to user indices
    /// </summary>
    private static readonly Dictionary<int, int> SocketToUserMap = new();

    /// <summary>
    ///     Gets the IP address of a connected client
    /// </summary>
    public static string GetAscIP(int socketID)
    {
        try
        {
            var userIdx = BuscaSlotSock(socketID);
            if (userIdx > 0) return Declaraciones.UserList[userIdx].ip;
        }
        catch
        {
        }

        return "0.0.0.0";
    }

    /// <summary>
    ///     Handles a connection request from a client
    /// </summary>
    private static void HandleConnectionReceived(int socketID, string clientIP)
    {
        // This runs on the main thread via the Windows.Forms event system
        int NewIndex = UsUaRiOs.NextOpenUser();

        if (NewIndex <= Declaraciones.MaxUsers)
        {
            Declaraciones.UserList[NewIndex].incomingData
                .ReadASCIIStringFixed(Declaraciones.UserList[NewIndex].incomingData.length);
            Declaraciones.UserList[NewIndex].outgoingData
                .ReadASCIIStringFixed(Declaraciones.UserList[NewIndex].outgoingData.length);

            Declaraciones.UserList[NewIndex].ip = clientIP;

            for (int i = 0, loopTo = Declaraciones.BanIps.Count - 1; i <= loopTo; i++)
            {
                var bannedIP = Declaraciones.BanIps[i];
                if ((bannedIP ?? "") == (clientIP ?? ""))
                {
                    Protocol.WriteErrorMsg(Convert.ToInt16(NewIndex), "Su IP se encuentra bloqueada en este servidor.");
                    Protocol.FlushBuffer(Convert.ToInt16(NewIndex));
                    SocketManager.CloseSocket(socketID);
                    return;
                }
            }

            if (NewIndex > Declaraciones.LastUser) Declaraciones.LastUser = Convert.ToInt16(NewIndex);

            Declaraciones.UserList[NewIndex].ConnID = Convert.ToInt16(socketID);
            Declaraciones.UserList[NewIndex].ConnIDValida = true;

            AgregaSlotSock(socketID, NewIndex);
        }
        else
        {
            var errorMsg =
                Protocol.PrepareMessageErrorMsg(
                    "El servidor se encuentra lleno en este momento. Disculpe las molestias ocasionadas.");
            var data = SocketManager.StringToBytes(errorMsg);
            SocketManager.SendData(socketID, data);
            SocketManager.CloseSocket(socketID);
        }
    }

    /// <summary>
    ///     Handles data received from a client
    /// </summary>
    private static void HandleDataReceived(int socketID, byte[] data)
    {
        // This now runs on the main thread via the synchronized queue system
        var userIndex = BuscaSlotSock(socketID);

        if (userIndex > 0)
        {
            ref var withBlock = ref Declaraciones.UserList[userIndex];
            withBlock.incomingData.WriteBlock(ref data);

            if (withBlock.ConnID != -1) Protocol.HandleIncomingData(Convert.ToInt16(userIndex));
        }
    }

    /// <summary>
    ///     Handles closure of a socket connection
    /// </summary>
    private static void HandleConnectionClosed(int socketID)
    {
        // This runs on the main thread via the Windows.Forms event system
        var userIndex = BuscaSlotSock(socketID);

        if (userIndex > 0)
        {
            BorraSlotSock(socketID);

            Declaraciones.UserList[userIndex].ConnID = -1;
            Declaraciones.UserList[userIndex].ConnIDValida = false;

            EventoSockClose(userIndex);
        }
    }

    /// <summary>
    ///     Handles connection error
    /// </summary>
    private static void HandleConnectionError(string errorMessage)
    {
        // This runs on the main thread via the Windows.Forms event system
        Console.WriteLine("Connection Error: " + errorMessage);
    }

    /// <summary>
    ///     Closes a socket connection
    /// </summary>
    public static void Winsock_Close(int socketID)
    {
        // This is called from the main thread
        var userIndex = BuscaSlotSock(socketID);

        if (socketID > 0) SocketManager.CloseSocket(socketID);

        if (userIndex > 0)
        {
            BorraSlotSock(socketID);
            Declaraciones.UserList[userIndex].ConnID = -1;
            Declaraciones.UserList[userIndex].ConnIDValida = false;
            EventoSockClose(userIndex);
        }
    }

    /// <summary>
    ///     Initializes the socket API
    /// </summary>
    public static void IniciaWsApi(int port)
    {
        // Set up event handlers to process messages on the main thread
        SocketManager.ConnectionReceived += HandleConnectionReceived;
        SocketManager.DataReceived += HandleDataReceived;
        SocketManager.ConnectionClosed += HandleConnectionClosed;
        SocketManager.ServerError += HandleConnectionError;

        SocketManager.Initialize(port);
    }

    /// <summary>
    ///     Cleans up the socket API
    /// </summary>
    public static void LimpiaWsApi()
    {
        SocketManager.ConnectionReceived -= HandleConnectionReceived;
        SocketManager.DataReceived -= HandleDataReceived;
        SocketManager.ConnectionClosed -= HandleConnectionClosed;
        SocketManager.ServerError -= HandleConnectionError;

        SocketManager.Shutdown();

        SocketToUserMap.Clear();
    }

    /// <summary>
    ///     Sends data through a user's socket connection
    /// </summary>
    public static int WsApiEnviar(int userIndex, ref string message)
    {
        int WsApiEnviarRet = default;
        try
        {
            var returnCode = 0;

            int socketID = Declaraciones.UserList[userIndex].ConnID;

            if (socketID != -1 && Declaraciones.UserList[userIndex].ConnIDValida)
            {
                var data = SocketManager.StringToBytes(message);

                if (!SocketManager.SendData(socketID, data)) returnCode = -1;

                // Process any pending messages that may have arrived
                SocketManager.ProcessPendingMessages();
            }
            else if (socketID != -1 && !Declaraciones.UserList[userIndex].ConnIDValida)
            {
                if (!Declaraciones.UserList[userIndex].Counters.Saliendo) returnCode = -1;
            }

            WsApiEnviarRet = returnCode;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in GetAscIP: " + ex.Message);
            Declaraciones.UserList[userIndex].outgoingData.WriteASCIIStringFixed(message);
        }

        return WsApiEnviarRet;
    }

    /// <summary>
    ///     Process any pending messages in the queue (can be called from main game loop)
    /// </summary>
    public static void ProcessNetworkMessages()
    {
        // Call this from your main game loop or timer to ensure messages get processed
        SocketManager.ProcessPendingMessages();
    }

    /// <summary>
    ///     Reinitializes all sockets
    /// </summary>
    public static void WSApiReiniciarSockets()
    {
        int i;

        var loopTo = (int)Declaraciones.MaxUsers;
        for (i = 1; i <= loopTo; i++)
            if (Declaraciones.UserList[i].ConnID != -1 && Declaraciones.UserList[i].ConnIDValida)
                TCP.CloseSocket(Convert.ToInt16(i));

        var loopTo1 = (int)Declaraciones.MaxUsers;
        for (i = 1; i <= loopTo1; i++)
        {
            Declaraciones.UserList[i].incomingData = null;
            Declaraciones.UserList[i].outgoingData = null;
        }

        Declaraciones.UserList = new Declaraciones.User[Declaraciones.MaxUsers + 1];
        ArrayInitializers.InitializeStruct(Declaraciones.UserList);
        var loopTo2 = (int)Declaraciones.MaxUsers;
        for (i = 1; i <= loopTo2; i++)
        {
            Declaraciones.UserList[i].ConnID = -1;
            Declaraciones.UserList[i].ConnIDValida = false;

            Declaraciones.UserList[i].incomingData = new clsByteQueue();
            Declaraciones.UserList[i].outgoingData = new clsByteQueue();
        }

        Declaraciones.LastUser = 1;
        Declaraciones.NumUsers = 0;

        SocketToUserMap.Clear();

        IniciaWsApi(Admin.Puerto);
    }

    /// <summary>
    ///     Finds the user index associated with a socket ID
    /// </summary>
    public static int BuscaSlotSock(int socketID)
    {
        var userIndex = -1;

        if (SocketToUserMap.TryGetValue(socketID, out userIndex)) return userIndex;

        return -1;
    }

    /// <summary>
    ///     Associates a socket ID with a user index
    /// </summary>
    public static void AgregaSlotSock(int socketID, int userIndex)
    {
        Console.WriteLine("AgregaSlotSock: Socket " + socketID + " -> User " + userIndex);

        if (SocketToUserMap.Count > Declaraciones.MaxUsers)
        {
            TCP.CloseSocket(Convert.ToInt16(userIndex));
            return;
        }

        SocketToUserMap[socketID] = userIndex;
    }

    /// <summary>
    ///     Removes the association between a socket ID and a user index
    /// </summary>
    public static void BorraSlotSock(int socketID)
    {
        var count = SocketToUserMap.Count;

        SocketToUserMap.Remove(socketID);

        Console.WriteLine("BorraSlotSock: " + count + " -> " + SocketToUserMap.Count);
    }

    /// <summary>
    ///     Handles the event of a socket being closed
    /// </summary>
    public static void EventoSockClose(int userIndex)
    {
        if (modCentinela.Centinela.RevisandoUserIndex == userIndex) modCentinela.CentinelaUserLogout();

        if (Declaraciones.UserList[userIndex].flags.UserLogged)
        {
            TCP.CloseSocketSL(Convert.ToInt16(userIndex));
            UsUaRiOs.Cerrar_Usuario(Convert.ToInt16(userIndex));
        }
        else
        {
            TCP.CloseSocket(Convert.ToInt16(userIndex));
        }
    }
}