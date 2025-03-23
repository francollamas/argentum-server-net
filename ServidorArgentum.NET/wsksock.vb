Option Strict Off
Option Explicit On
Module WSKSOCK
	'**************************************************************
	' WSKSOCK.bas
	'
	'**************************************************************
	
	'**************************************************************************
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
	'**************************************************************************
	
#If UsarQueSocket = 1 Then
	
	'date stamp: sept 1, 1996 (for version control, please don't remove)
	
	'Visual Basic 4.0 Winsock "Header"
	'   Alot of the information contained inside this file was originally
	'   obtained from ALT.WINSOCK.PROGRAMMING and most of it has since been
	'   modified in some way.
	'
	'Disclaimer: This file is public domain, updated periodically by
	'   Topaz, SigSegV@mail.utexas.edu, Use it at your own risk.
	'   Neither myself(Topaz) or anyone related to alt.programming.winsock
	'   may be held liable for its use, or misuse.
	'
	'Declare check Aug 27, 1996. (Topaz, SigSegV@mail.utexas.edu)
	'   All 16 bit declarations appear correct, even the odd ones that
	'   pass longs inplace of in_addr and char buffers. 32 bit functions
	'   also appear correct. Some are declared to return integers instead of
	'   longs (breaking MS's rules.) however after testing these functions I
	'   have come to the conclusion that they do not work properly when declared
	'   following MS's rules.
	'
	'NOTES:
	'   (1) I have never used WS_SELECT (select), therefore I must warn that I do
	'       not know if fd_set and timeval are properly defined.
	'   (2) Alot of the functions are declared with "buf as any", when calling these
	'       functions you may either pass strings, byte arrays or UDT's. For 32bit I
	'       I recommend Byte arrays and the use of memcopy to copy the data back out
	'   (3) The async functions (wsaAsync*) require the use of a message hook or
	'       message window control to capture messages sent by the winsock stack. This
	'       is not to be confused with a CallBack control, The only function that uses
	'       callbacks is WSASetBlockingHook()
	'   (4) Alot of "helper" functions are provided in the file for various things
	'       before attempting to figure out how to call a function, look and see if
	'       there is already a helper function for it.
	'   (5) Data types (hostent etc) have kept there 16bit definitions, even under 32bit
	'       windows due to the problem of them not working when redfined following the
	'       suggested rules.
	
	Public Const FD_SETSIZE As Short = 64
	Structure fd_set
		Dim fd_count As Short
		<VBFixedArray(FD_SETSIZE)> Dim fd_array() As Short
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			ReDim fd_array(FD_SETSIZE)
		End Sub
	End Structure
	
	Structure timeval
		Dim tv_sec As Integer
		Dim tv_usec As Integer
	End Structure
	
	Structure HostEnt
		Dim h_name As Integer
		Dim h_aliases As Integer
		Dim h_addrtype As Short
		Dim h_length As Short
		Dim h_addr_list As Integer
	End Structure
	Public Const hostent_size As Short = 16
	
	Structure servent
		Dim s_name As Integer
		Dim s_aliases As Integer
		Dim s_port As Short
		Dim s_proto As Integer
	End Structure
	Public Const servent_size As Short = 14
	
	Structure protoent
		Dim p_name As Integer
		Dim p_aliases As Integer
		Dim p_proto As Short
	End Structure
	Public Const protoent_size As Short = 10
	
	Public Const IPPROTO_TCP As Short = 6
	Public Const IPPROTO_UDP As Short = 17
	
	Public Const INADDR_NONE As Integer = &HFFFFFFFF
	Public Const INADDR_ANY As Integer = &H0
	
	Structure sockaddr
		Dim sin_family As Short
		Dim sin_port As Short
		Dim sin_addr As Integer
		'UPGRADE_WARNING: El tamaño de la cadena de longitud fija debe caber en el búfer. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(8),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=8)> Public sin_zero() As Char
	End Structure
	Public Const sockaddr_size As Short = 16
	Public saZero As sockaddr
	
	
	Public Const WSA_DESCRIPTIONLEN As Short = 256
	Public Const WSA_DescriptionSize As Integer = WSA_DESCRIPTIONLEN + 1
	
	Public Const WSA_SYS_STATUS_LEN As Short = 128
	Public Const WSA_SysStatusSize As Integer = WSA_SYS_STATUS_LEN + 1
	
	Structure WSADataType
		Dim wVersion As Short
		Dim wHighVersion As Short
		'UPGRADE_WARNING: El tamaño de la cadena de longitud fija debe caber en el búfer. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(WSA_DescriptionSize),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=WSA_DescriptionSize)> Public szDescription() As Char
		'UPGRADE_WARNING: El tamaño de la cadena de longitud fija debe caber en el búfer. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(WSA_SysStatusSize),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=WSA_SysStatusSize)> Public szSystemStatus() As Char
		Dim iMaxSockets As Short
		Dim iMaxUdpDg As Short
		Dim lpVendorInfo As Integer
	End Structure
	
	'Agregado por Maraxus
	Structure WSABUF
		Dim dwBufferLen As Integer
		Dim lpBuffer As Integer
	End Structure
	
	'Agregado por Maraxus
	Structure FLOWSPEC
		Dim TokenRate As Integer 'In Bytes/sec
		Dim TokenBucketSize As Integer 'In Bytes
		Dim PeakBandwidth As Integer 'In Bytes/sec
		Dim Latency As Integer 'In microseconds
		Dim DelayVariation As Integer 'In microseconds
		Dim ServiceType As Short 'Guaranteed, Predictive,
		'Best Effort, etc.
		Dim MaxSduSize As Integer 'In Bytes
		Dim MinimumPolicedSize As Integer 'In Bytes
	End Structure
	
	'Agregado por Maraxus
	Public Const WSA_FLAG_OVERLAPPED As Integer = &H1
	
	'Agregados por Maraxus
	Public Const CF_ACCEPT As Integer = &H0
	Public Const CF_REJECT As Integer = &H1
	
	'Agregado por Maraxus
	Public Const SD_RECEIVE As Integer = &H0
	Public Const SD_SEND As Integer = &H1
	Public Const SD_BOTH As Integer = &H2
	
	Public Const INVALID_SOCKET As Short = -1
	Public Const SOCKET_ERROR As Short = -1
	
	Public Const SOCK_STREAM As Short = 1
	Public Const SOCK_DGRAM As Short = 2
	
	Public Const MAXGETHOSTSTRUCT As Short = 1024
	
	Public Const AF_INET As Short = 2
	Public Const PF_INET As Short = 2
	
	Structure LingerType
		Dim l_onoff As Short
		Dim l_linger As Short
	End Structure
	' Windows Sockets definitions of regular Microsoft C error constants
	Public Const WSAEINTR As Short = 10004
	Public Const WSAEBADF As Short = 10009
	Public Const WSAEACCES As Short = 10013
	Public Const WSAEFAULT As Short = 10014
	Public Const WSAEINVAL As Short = 10022
	Public Const WSAEMFILE As Short = 10024
	' Windows Sockets definitions of regular Berkeley error constants
	Public Const WSAEWOULDBLOCK As Short = 10035
	Public Const WSAEINPROGRESS As Short = 10036
	Public Const WSAEALREADY As Short = 10037
	Public Const WSAENOTSOCK As Short = 10038
	Public Const WSAEDESTADDRREQ As Short = 10039
	Public Const WSAEMSGSIZE As Short = 10040
	Public Const WSAEPROTOTYPE As Short = 10041
	Public Const WSAENOPROTOOPT As Short = 10042
	Public Const WSAEPROTONOSUPPORT As Short = 10043
	Public Const WSAESOCKTNOSUPPORT As Short = 10044
	Public Const WSAEOPNOTSUPP As Short = 10045
	Public Const WSAEPFNOSUPPORT As Short = 10046
	Public Const WSAEAFNOSUPPORT As Short = 10047
	Public Const WSAEADDRINUSE As Short = 10048
	Public Const WSAEADDRNOTAVAIL As Short = 10049
	Public Const WSAENETDOWN As Short = 10050
	Public Const WSAENETUNREACH As Short = 10051
	Public Const WSAENETRESET As Short = 10052
	Public Const WSAECONNABORTED As Short = 10053
	Public Const WSAECONNRESET As Short = 10054
	Public Const WSAENOBUFS As Short = 10055
	Public Const WSAEISCONN As Short = 10056
	Public Const WSAENOTCONN As Short = 10057
	Public Const WSAESHUTDOWN As Short = 10058
	Public Const WSAETOOMANYREFS As Short = 10059
	Public Const WSAETIMEDOUT As Short = 10060
	Public Const WSAECONNREFUSED As Short = 10061
	Public Const WSAELOOP As Short = 10062
	Public Const WSAENAMETOOLONG As Short = 10063
	Public Const WSAEHOSTDOWN As Short = 10064
	Public Const WSAEHOSTUNREACH As Short = 10065
	Public Const WSAENOTEMPTY As Short = 10066
	Public Const WSAEPROCLIM As Short = 10067
	Public Const WSAEUSERS As Short = 10068
	Public Const WSAEDQUOT As Short = 10069
	Public Const WSAESTALE As Short = 10070
	Public Const WSAEREMOTE As Short = 10071
	' Extended Windows Sockets error constant definitions
	Public Const WSASYSNOTREADY As Short = 10091
	Public Const WSAVERNOTSUPPORTED As Short = 10092
	Public Const WSANOTINITIALISED As Short = 10093
	Public Const WSAHOST_NOT_FOUND As Short = 11001
	Public Const WSATRY_AGAIN As Short = 11002
	Public Const WSANO_RECOVERY As Short = 11003
	Public Const WSANO_DATA As Short = 11004
	Public Const WSANO_ADDRESS As Short = 11004
	'---ioctl Constants
	Public Const FIONREAD As Integer = &H8004667F
	Public Const FIONBIO As Integer = &H8004667E
	Public Const FIOASYNC As Integer = &H8004667D
	
#If Win16 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	'---Windows System functions
	Public Declare Function PostMessage Lib "User" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, lParam As Any) As Integer
	Public Declare Sub MemCopy Lib "Kernel" Alias "hmemcpy" (Dest As Any, Src As Any, ByVal cb&)
	Public Declare Function lstrlen Lib "Kernel" (ByVal lpString As Any) As Integer
	'---async notification constants
	Public Const SOL_SOCKET = &HFFFF
	Public Const SO_LINGER = &H80
	Public Const SO_RCVBUFFER = &H1002              ' Agregado por Maraxus
	Public Const SO_SNDBUFFER = &H1001              ' Agregado por Maraxus
	Public Const SO_CONDITIONAL_ACCEPT = &H3002    ' Agregado por Maraxus
	Public Const FD_READ = &H1
	Public Const FD_WRITE = &H2
	Public Const FD_OOB = &H4
	Public Const FD_ACCEPT = &H8
	Public Const FD_CONNECT = &H10
	Public Const FD_CLOSE = &H20
	'---SOCKET FUNCTIONS
	Public Declare Function accept Lib "ws2_32.DLL" (ByVal S As Integer, addr As sockaddr, AddrLen As Integer) As Integer
	Public Declare Function bind Lib "ws2_32.DLL" (ByVal S As Integer, addr As sockaddr, ByVal namelen As Integer) As Integer
	Public Declare Function apiclosesocket Lib "ws2_32.DLL" Alias "closesocket" (ByVal S As Integer) As Integer
	Public Declare Function connect Lib "ws2_32.DLL" (ByVal S As Integer, addr As sockaddr, ByVal namelen As Integer) As Integer
	Public Declare Function ioctlsocket Lib "ws2_32.DLL" (ByVal S As Integer, ByVal Cmd As Long, argp As Long) As Integer
	Public Declare Function getpeername Lib "ws2_32.DLL" (ByVal S As Integer, sName As sockaddr, namelen As Integer) As Integer
	Public Declare Function getsockname Lib "ws2_32.DLL" (ByVal S As Integer, sName As sockaddr, namelen As Integer) As Integer
	Public Declare Function getsockopt Lib "ws2_32.DLL" (ByVal S As Integer, ByVal level As Integer, ByVal optname As Integer, optval As Any, optlen As Integer) As Integer
	Public Declare Function htonl Lib "ws2_32.DLL" (ByVal hostlong As Long) As Long
	Public Declare Function htons Lib "ws2_32.DLL" (ByVal hostshort As Integer) As Integer
	Public Declare Function inet_addr Lib "ws2_32.DLL" (ByVal cp As String) As Long
	Public Declare Function inet_ntoa Lib "ws2_32.DLL" (ByVal inn As Long) As Long
	Public Declare Function listen Lib "ws2_32.DLL" (ByVal S As Integer, ByVal backlog As Integer) As Integer
	Public Declare Function ntohl Lib "ws2_32.DLL" (ByVal netlong As Long) As Long
	Public Declare Function ntohs Lib "ws2_32.DLL" (ByVal netshort As Integer) As Integer
	Public Declare Function recv Lib "ws2_32.DLL" (ByVal S As Integer, ByRef buf As Any, ByVal buflen As Integer, ByVal flags As Integer) As Integer
	Public Declare Function recvfrom Lib "ws2_32.DLL" (ByVal S As Integer, buf As Any, ByVal buflen As Integer, ByVal flags As Integer, from As sockaddr, fromlen As Integer) As Integer
	Public Declare Function ws_select Lib "ws2_32.DLL" Alias "select" (ByVal nfds As Integer, readfds As Any, writefds As Any, exceptfds As Any, timeout As timeval) As Integer
	Public Declare Function send Lib "ws2_32.DLL" (ByVal S As Integer, buf As Any, ByVal buflen As Integer, ByVal flags As Integer) As Integer
	Public Declare Function sendto Lib "ws2_32.DLL" (ByVal S As Integer, buf As Any, ByVal buflen As Integer, ByVal flags As Integer, to_addr As sockaddr, ByVal tolen As Integer) As Integer
	Public Declare Function setsockopt Lib "ws2_32.DLL" (ByVal S As Integer, ByVal level As Integer, ByVal optname As Integer, optval As Any, ByVal optlen As Integer) As Integer
	Public Declare Function ShutDown Lib "ws2_32.DLL" Alias "shutdown" (ByVal S As Integer, ByVal how As Integer) As Integer
	Public Declare Function Socket Lib "ws2_32.DLL" Alias "socket" (ByVal af As Integer, ByVal s_type As Integer, ByVal Protocol As Integer) As Integer
	'---DATABASE FUNCTIONS
	Public Declare Function gethostbyaddr Lib "ws2_32.DLL" (addr As Long, ByVal addr_len As Integer, ByVal addr_type As Integer) As Long
	Public Declare Function gethostbyname Lib "ws2_32.DLL" (ByVal host_name As String) As Long
	Public Declare Function gethostname Lib "ws2_32.DLL" (ByVal host_name As String, ByVal namelen As Integer) As Integer
	Public Declare Function getservbyport Lib "ws2_32.DLL" (ByVal Port As Integer, ByVal proto As String) As Long
	Public Declare Function getservbyname Lib "ws2_32.DLL" (ByVal serv_name As String, ByVal proto As String) As Long
	Public Declare Function getprotobynumber Lib "ws2_32.DLL" (ByVal proto As Integer) As Long
	Public Declare Function getprotobyname Lib "ws2_32.DLL" (ByVal proto_name As String) As Long
	'---WINDOWS EXTENSIONS
	Public Declare Function WSAStartup Lib "ws2_32.DLL" (ByVal wVR As Integer, lpWSAD As WSADataType) As Integer
	Public Declare Function WSACleanup Lib "ws2_32.DLL" () As Integer
	Public Declare Sub WSASetLastError Lib "ws2_32.DLL" (ByVal iError As Integer)
	Public Declare Function WSAGetLastError Lib "ws2_32.DLL" () As Integer
	Public Declare Function WSAIsBlocking Lib "ws2_32.DLL" () As Integer
	Public Declare Function WSAUnhookBlockingHook Lib "ws2_32.DLL" () As Integer
	Public Declare Function WSASetBlockingHook Lib "ws2_32.DLL" (ByVal lpBlockFunc As Long) As Long
	Public Declare Function WSACancelBlockingCall Lib "ws2_32.DLL" () As Integer
	Public Declare Function WSAAsyncGetServByName Lib "ws2_32.DLL" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal serv_name As String, ByVal proto As String, buf As Any, ByVal buflen As Integer) As Integer
	Public Declare Function WSAAsyncGetServByPort Lib "ws2_32.DLL" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal Port As Integer, ByVal proto As String, buf As Any, ByVal buflen As Integer) As Integer
	Public Declare Function WSAAsyncGetProtoByName Lib "ws2_32.DLL" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal proto_name As String, buf As Any, ByVal buflen As Integer) As Integer
	Public Declare Function WSAAsyncGetProtoByNumber Lib "ws2_32.DLL" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal Number As Integer, buf As Any, ByVal buflen As Integer) As Integer
	Public Declare Function WSAAsyncGetHostByName Lib "ws2_32.DLL" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal host_name As String, buf As Any, ByVal buflen As Integer) As Integer
	Public Declare Function WSAAsyncGetHostByAddr Lib "ws2_32.DLL" (ByVal hWnd As Integer, ByVal wMsg As Integer, addr As Long, ByVal addr_len As Integer, ByVal addr_type As Integer, buf As Any, ByVal buflen As Integer) As Integer
	Public Declare Function WSACancelAsyncRequest Lib "ws2_32.DLL" (ByVal hAsyncTaskHandle As Integer) As Integer
	Public Declare Function WSAAsyncSelect Lib "ws2_32.DLL" (ByVal S As Integer, ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal lEvent As Long) As Integer
	Public Declare Function WSARecvEx Lib "ws2_32.DLL" (ByVal S As Integer, buf As Any, ByVal buflen As Integer, ByVal flags As Integer) As Integer
	'Agregado por Maraxus
	Declare Function WSAAccept Lib "ws2_32.DLL" (ByVal S As Integer, pSockAddr As sockaddr, AddrLen As Integer, ByVal lpfnCondition As Long, ByVal dwCallbackData As Long) As Integer
	
	Public Const SOMAXCONN As Integer = &H7FFF            ' Agregado por Maraxus
	
#ElseIf Win32 Then
	'---Windows System Functions
	Public Declare Function PostMessage Lib "user32"  Alias "PostMessageA"(ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Sub MemCopy Lib "kernel32"  Alias "RtlMoveMemory"(ByRef Dest As Any, ByRef Src As Any, ByVal cb As Integer)
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function lstrlen Lib "kernel32"  Alias "lstrlenA"(ByVal lpString As Any) As Integer
	'---async notification constants
	Public Const SOL_SOCKET As Integer = &HFFFF
	Public Const SO_LINGER As Integer = &H80
	Public Const SO_RCVBUFFER As Integer = &H1002 ' Agregado por Maraxus
	Public Const SO_SNDBUFFER As Integer = &H1001 ' Agregado por Maraxus
	Public Const SO_CONDITIONAL_ACCEPT As Integer = &H3002 ' Agregado por Maraxus
	Public Const FD_READ As Integer = &H1
	Public Const FD_WRITE As Integer = &H2
	Public Const FD_OOB As Integer = &H4
	Public Const FD_ACCEPT As Integer = &H8
	Public Const FD_CONNECT As Integer = &H10
	Public Const FD_CLOSE As Integer = &H20
	'---SOCKET FUNCTIONS
	'UPGRADE_WARNING: La estructura sockaddr puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Public Declare Function accept Lib "wsock32.dll" (ByVal S As Integer, ByRef addr As sockaddr, ByRef AddrLen As Integer) As Integer
	'UPGRADE_WARNING: La estructura sockaddr puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Public Declare Function bind Lib "wsock32.dll" (ByVal S As Integer, ByRef addr As sockaddr, ByVal namelen As Integer) As Integer
	Public Declare Function apiclosesocket Lib "wsock32.dll"  Alias "closesocket"(ByVal S As Integer) As Integer
	'UPGRADE_WARNING: La estructura sockaddr puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Public Declare Function connect Lib "wsock32.dll" (ByVal S As Integer, ByRef addr As sockaddr, ByVal namelen As Integer) As Integer
	Public Declare Function ioctlsocket Lib "wsock32.dll" (ByVal S As Integer, ByVal Cmd As Integer, ByRef argp As Integer) As Integer
	'UPGRADE_WARNING: La estructura sockaddr puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Public Declare Function getpeername Lib "wsock32.dll" (ByVal S As Integer, ByRef sName As sockaddr, ByRef namelen As Integer) As Integer
	'UPGRADE_WARNING: La estructura sockaddr puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Public Declare Function getsockname Lib "wsock32.dll" (ByVal S As Integer, ByRef sName As sockaddr, ByRef namelen As Integer) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function getsockopt Lib "wsock32.dll" (ByVal S As Integer, ByVal level As Integer, ByVal optname As Integer, ByRef optval As Any, ByRef optlen As Integer) As Integer
	Public Declare Function htonl Lib "wsock32.dll" (ByVal hostlong As Integer) As Integer
	Public Declare Function htons Lib "wsock32.dll" (ByVal hostshort As Integer) As Short
	Public Declare Function inet_addr Lib "wsock32.dll" (ByVal cp As String) As Integer
	Public Declare Function inet_ntoa Lib "wsock32.dll" (ByVal inn As Integer) As Integer
	Public Declare Function listen Lib "wsock32.dll" (ByVal S As Integer, ByVal backlog As Integer) As Integer
	Public Declare Function ntohl Lib "wsock32.dll" (ByVal netlong As Integer) As Integer
	Public Declare Function ntohs Lib "wsock32.dll" (ByVal netshort As Integer) As Short
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function recv Lib "wsock32.dll" (ByVal S As Integer, ByRef buf As Any, ByVal buflen As Integer, ByVal flags As Integer) As Integer
	'UPGRADE_WARNING: La estructura sockaddr puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function recvfrom Lib "wsock32.dll" (ByVal S As Integer, ByRef buf As Any, ByVal buflen As Integer, ByVal flags As Integer, ByRef from As sockaddr, ByRef fromlen As Integer) As Integer
	'UPGRADE_WARNING: La estructura timeval puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	'UPGRADE_WARNING: La estructura fd_set puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	'UPGRADE_WARNING: La estructura fd_set puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	'UPGRADE_WARNING: La estructura fd_set puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Public Declare Function ws_select Lib "wsock32.dll"  Alias "select"(ByVal nfds As Integer, ByRef readfds As fd_set, ByRef writefds As fd_set, ByRef exceptfds As fd_set, ByRef timeout As timeval) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function send Lib "wsock32.dll" (ByVal S As Integer, ByRef buf As Any, ByVal buflen As Integer, ByVal flags As Integer) As Integer
	'UPGRADE_WARNING: La estructura sockaddr puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function sendto Lib "wsock32.dll" (ByVal S As Integer, ByRef buf As Any, ByVal buflen As Integer, ByVal flags As Integer, ByRef to_addr As sockaddr, ByVal tolen As Integer) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function setsockopt Lib "wsock32.dll" (ByVal S As Integer, ByVal level As Integer, ByVal optname As Integer, ByRef optval As Any, ByVal optlen As Integer) As Integer
	Public Declare Function ShutDown Lib "wsock32.dll"  Alias "shutdown"(ByVal S As Integer, ByVal how As Integer) As Integer
	Public Declare Function Socket Lib "wsock32.dll"  Alias "socket"(ByVal af As Integer, ByVal s_type As Integer, ByVal Protocol As Integer) As Integer
	'---DATABASE FUNCTIONS
	Public Declare Function gethostbyaddr Lib "wsock32.dll" (ByRef addr As Integer, ByVal addr_len As Integer, ByVal addr_type As Integer) As Integer
	Public Declare Function gethostbyname Lib "wsock32.dll" (ByVal host_name As String) As Integer
	Public Declare Function gethostname Lib "wsock32.dll" (ByVal host_name As String, ByVal namelen As Integer) As Integer
	Public Declare Function getservbyport Lib "wsock32.dll" (ByVal Port As Integer, ByVal proto As String) As Integer
	Public Declare Function getservbyname Lib "wsock32.dll" (ByVal serv_name As String, ByVal proto As String) As Integer
	Public Declare Function getprotobynumber Lib "wsock32.dll" (ByVal proto As Integer) As Integer
	Public Declare Function getprotobyname Lib "wsock32.dll" (ByVal proto_name As String) As Integer
	'---WINDOWS EXTENSIONS
	'UPGRADE_WARNING: La estructura WSADataType puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Public Declare Function WSAStartup Lib "wsock32.dll" (ByVal wVR As Integer, ByRef lpWSAD As WSADataType) As Integer
	Public Declare Function WSACleanup Lib "wsock32.dll" () As Integer
	Public Declare Sub WSASetLastError Lib "wsock32.dll" (ByVal iError As Integer)
	Public Declare Function WSAGetLastError Lib "wsock32.dll" () As Integer
	Public Declare Function WSAIsBlocking Lib "wsock32.dll" () As Integer
	Public Declare Function WSAUnhookBlockingHook Lib "wsock32.dll" () As Integer
	Public Declare Function WSASetBlockingHook Lib "wsock32.dll" (ByVal lpBlockFunc As Integer) As Integer
	Public Declare Function WSACancelBlockingCall Lib "wsock32.dll" () As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function WSAAsyncGetServByName Lib "wsock32.dll" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal serv_name As String, ByVal proto As String, ByRef buf As Any, ByVal buflen As Integer) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function WSAAsyncGetServByPort Lib "wsock32.dll" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal Port As Integer, ByVal proto As String, ByRef buf As Any, ByVal buflen As Integer) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function WSAAsyncGetProtoByName Lib "wsock32.dll" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal proto_name As String, ByRef buf As Any, ByVal buflen As Integer) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function WSAAsyncGetProtoByNumber Lib "wsock32.dll" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal Number As Integer, ByRef buf As Any, ByVal buflen As Integer) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function WSAAsyncGetHostByName Lib "wsock32.dll" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal host_name As String, ByRef buf As Any, ByVal buflen As Integer) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function WSAAsyncGetHostByAddr Lib "wsock32.dll" (ByVal hWnd As Integer, ByVal wMsg As Integer, ByRef addr As Integer, ByVal addr_len As Integer, ByVal addr_type As Integer, ByRef buf As Any, ByVal buflen As Integer) As Integer
	Public Declare Function WSACancelAsyncRequest Lib "wsock32.dll" (ByVal hAsyncTaskHandle As Integer) As Integer
	Public Declare Function WSAAsyncSelect Lib "wsock32.dll" (ByVal S As Integer, ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal lEvent As Integer) As Integer
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Public Declare Function WSARecvEx Lib "wsock32.dll" (ByVal S As Integer, ByRef buf As Any, ByVal buflen As Integer, ByVal flags As Integer) As Integer
	'Agregado por Maraxus
	'UPGRADE_WARNING: La estructura sockaddr puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Declare Function WSAAccept Lib "ws2_32.DLL" (ByVal S As Integer, ByRef pSockAddr As sockaddr, ByRef AddrLen As Integer, ByVal lpfnCondition As Integer, ByVal dwCallbackData As Integer) As Integer
	Public Const SOMAXCONN As Integer = &H7FFFFFFF ' Agregado por Maraxus
	
	
#End If
	
	
	'SOME STUFF I ADDED
	Public MySocket As Short
	Public SockReadBuffer As String
	Public Const WSA_NoName As String = "Unknown"
	Public WSAStartedUp As Boolean 'Flag to keep track of whether winsock WSAStartup wascalled
	
	
	Public Function WSAGetAsyncBufLen(ByVal lParam As Integer) As Integer
		If (lParam And &HFFFF) > &H7FFF Then
			WSAGetAsyncBufLen = CShort(lParam And &HFFFF) - &H10000
		Else
			WSAGetAsyncBufLen = lParam And &HFFFF
		End If
	End Function
	
	Public Function WSAGetSelectEvent(ByVal lParam As Integer) As Short
		If (lParam And &HFFFF) > &H7FFF Then
			WSAGetSelectEvent = CShort(lParam And &HFFFF) - &H10000
		Else
			WSAGetSelectEvent = lParam And &HFFFF
		End If
	End Function
	
	
	
	Public Function WSAGetAsyncError(ByVal lParam As Integer) As Short
		WSAGetAsyncError = (lParam And &HFFFF0000) \ &H10000
	End Function
	
	
	
	Public Function AddrToIP(ByVal AddrOrIP As String) As String
		Dim T() As String
		Dim Tmp As String
		
		Tmp = GetAscIP(GetHostByNameAlias(AddrOrIP))
		T = Split(Tmp, ".")
		AddrToIP = T(3) & "." & T(2) & "." & T(1) & "." & T(0)
		
	End Function
	
	'this function should work on 16 and 32 bit systems
#If Win16 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	Function ConnectSock(ByVal Host$, ByVal Port%, retIpPort$, ByVal HWndToMsg%, ByVal Async%) As Integer
	Dim S%, SelectOps%, dummy%
#ElseIf Win32 Then
	Function ConnectSock(ByVal Host As String, ByVal Port As Integer, ByRef retIpPort As String, ByVal HWndToMsg As Integer, ByVal Async As Short) As Integer
		Dim SelectOps, S, dummy As Integer
#End If
		Dim sockin As sockaddr
		SockReadBuffer = vbNullString
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto sockin. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		sockin = saZero
		sockin.sin_family = AF_INET
		sockin.sin_port = htons(Port)
		If sockin.sin_port = INVALID_SOCKET Then
			ConnectSock = INVALID_SOCKET
			Exit Function
		End If
		
		sockin.sin_addr = GetHostByNameAlias(Host)
		If sockin.sin_addr = INADDR_NONE Then
			ConnectSock = INVALID_SOCKET
			Exit Function
		End If
		retIpPort = GetAscIP(sockin.sin_addr) & ":" & ntohs(sockin.sin_port)
		
		S = Socket(PF_INET, SOCK_STREAM, IPPROTO_TCP)
		If S < 0 Then
			ConnectSock = INVALID_SOCKET
			Exit Function
		End If
		If SetSockLinger(S, 1, 0) = SOCKET_ERROR Then
			If S > 0 Then
				dummy = apiclosesocket(S)
			End If
			ConnectSock = INVALID_SOCKET
			Exit Function
		End If
		If Not Async Then
			If Not connect(S, sockin, sockaddr_size) = 0 Then
				If S > 0 Then
					dummy = apiclosesocket(S)
				End If
				ConnectSock = INVALID_SOCKET
				Exit Function
			End If
			If HWndToMsg <> 0 Then
				SelectOps = FD_READ Or FD_WRITE Or FD_CONNECT Or FD_CLOSE
				If WSAAsyncSelect(S, HWndToMsg, 1025, SelectOps) Then
					If S > 0 Then
						dummy = apiclosesocket(S)
					End If
					ConnectSock = INVALID_SOCKET
					Exit Function
				End If
			End If
		Else
			SelectOps = FD_READ Or FD_WRITE Or FD_CONNECT Or FD_CLOSE
			If WSAAsyncSelect(S, HWndToMsg, 1025, SelectOps) Then
				If S > 0 Then
					dummy = apiclosesocket(S)
				End If
				ConnectSock = INVALID_SOCKET
				Exit Function
			End If
			If connect(S, sockin, sockaddr_size) <> -1 Then
				If S > 0 Then
					dummy = apiclosesocket(S)
				End If
				ConnectSock = INVALID_SOCKET
				Exit Function
			End If
		End If
		ConnectSock = S
	End Function
	
#If Win32 Then
	Public Function SetSockLinger(ByVal SockNum As Integer, ByVal OnOff As Short, ByVal LingerTime As Short) As Integer
#Else
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Else no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Public Function SetSockLinger(ByVal SockNum%, ByVal OnOff%, ByVal LingerTime%) As Integer
#End If
		Dim Linger As LingerType
		Linger.l_onoff = OnOff
		Linger.l_linger = LingerTime
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Linger. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		If setsockopt(SockNum, SOL_SOCKET, SO_LINGER, Linger, 4) Then
			Debug.Print("Error setting linger info: " & WSAGetLastError())
			SetSockLinger = SOCKET_ERROR
		Else
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Linger. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			If getsockopt(SockNum, SOL_SOCKET, SO_LINGER, Linger, 4) Then
				Debug.Print("Error getting linger info: " & WSAGetLastError())
				SetSockLinger = SOCKET_ERROR
			Else
				Debug.Print("Linger is on if nonzero: " & Linger.l_onoff)
				Debug.Print("Linger time if linger is on: " & Linger.l_linger)
			End If
		End If
	End Function
	
	Sub EndWinsock()
		Dim Ret As Integer
		If WSAIsBlocking() Then
			Ret = WSACancelBlockingCall()
		End If
		Ret = WSACleanup()
		WSAStartedUp = False
	End Sub
	
	Public Function GetAscIP(ByVal inn As Integer) As String
#If Win32 Then
		Dim nStr As Integer
#Else
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Else no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Dim nStr%
#End If
		Dim lpStr As Integer
		Dim retString As String
		retString = New String(Chr(0), 32)
		lpStr = inet_ntoa(inn)
		If lpStr Then
			nStr = lstrlen(lpStr)
			If nStr > 32 Then nStr = 32
			MemCopy(retString, lpStr, nStr)
			retString = Left(retString, nStr)
			GetAscIP = retString
		Else
			GetAscIP = "255.255.255.255"
		End If
	End Function
	
	Public Function GetHostByAddress(ByVal addr As Integer) As String
		Dim phe As Integer
		Dim heDestHost As HostEnt
		Dim HostName As String
		phe = gethostbyaddr(addr, 4, PF_INET)
		If phe Then
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto heDestHost. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			MemCopy(heDestHost, phe, hostent_size)
			HostName = New String(Chr(0), 256)
			MemCopy(HostName, heDestHost.h_name, 256)
			GetHostByAddress = Left(HostName, InStr(HostName, Chr(0)) - 1)
		Else
			GetHostByAddress = WSA_NoName
		End If
	End Function
	
	'returns IP as long, in network byte order
	Public Function GetHostByNameAlias(ByVal HostName As String) As Integer
		'Return IP address as a long, in network byte order
		Dim phe As Integer
		Dim heDestHost As HostEnt
		Dim addrList As Integer
		Dim retIP As Integer
		retIP = inet_addr(HostName)
		If retIP = INADDR_NONE Then
			phe = gethostbyname(HostName)
			If phe <> 0 Then
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto heDestHost. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				MemCopy(heDestHost, phe, hostent_size)
				MemCopy(addrList, heDestHost.h_addr_list, 4)
				MemCopy(retIP, addrList, heDestHost.h_length)
			Else
				retIP = INADDR_NONE
			End If
		End If
		GetHostByNameAlias = retIP
	End Function
	
	'returns your local machines name
	Public Function GetLocalHostName() As String
		Dim sName As String
		sName = New String(Chr(0), 256)
		If gethostname(sName, 256) Then
			sName = WSA_NoName
		Else
			If InStr(sName, Chr(0)) Then
				sName = Left(sName, InStr(sName, Chr(0)) - 1)
			End If
		End If
		GetLocalHostName = sName
	End Function
	
#If Win16 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	Public Function GetPeerAddress(ByVal S%) As String
	Dim AddrLen%
#ElseIf Win32 Then
	Public Function GetPeerAddress(ByVal S As Integer) As String
		Dim AddrLen As Integer
#End If
		Dim sa As sockaddr
		AddrLen = sockaddr_size
		If getpeername(S, sa, AddrLen) Then
			GetPeerAddress = vbNullString
		Else
			GetPeerAddress = SockAddressToString(sa)
		End If
	End Function
	
#If Win16 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	Public Function GetPortFromString(ByVal PortStr$) As Integer
#ElseIf Win32 Then
	Public Function GetPortFromString(ByVal PortStr As String) As Integer
#End If
		'sometimes users provide ports outside the range of a VB
		'integer, so this function returns an integer for a string
		'just to keep an error from happening, it converts the
		'number to a negative if needed
		If Val(PortStr) > 32767 Then
			GetPortFromString = CShort(Val(PortStr) - &H10000)
		Else
			GetPortFromString = Val(PortStr)
		End If
		If Err.Number Then GetPortFromString = 0
	End Function
	
#If Win16 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	Function GetProtocolByName(ByVal Protocol$) As Integer
	Dim tmpShort%
#ElseIf Win32 Then
	Function GetProtocolByName(ByVal Protocol As String) As Integer
		Dim tmpShort As Integer
#End If
		Dim ppe As Integer
		Dim peDestProt As protoent
		ppe = getprotobyname(Protocol)
		If ppe Then
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto peDestProt. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			MemCopy(peDestProt, ppe, protoent_size)
			GetProtocolByName = peDestProt.p_proto
		Else
			tmpShort = Val(Protocol)
			If tmpShort Then
				GetProtocolByName = htons(tmpShort)
			Else
				GetProtocolByName = SOCKET_ERROR
			End If
		End If
	End Function
	
#If Win16 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	Function GetServiceByName(ByVal service$, ByVal Protocol$) As Integer
	Dim Serv%
#ElseIf Win32 Then
	Function GetServiceByName(ByVal service As String, ByVal Protocol As String) As Integer
		Dim Serv As Integer
#End If
		Dim pse As Integer
		Dim seDestServ As servent
		pse = getservbyname(service, Protocol)
		If pse Then
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto seDestServ. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			MemCopy(seDestServ, pse, servent_size)
			GetServiceByName = seDestServ.s_port
		Else
			Serv = Val(service)
			If Serv Then
				GetServiceByName = htons(Serv)
			Else
				GetServiceByName = INVALID_SOCKET
			End If
		End If
	End Function
	
	'this function DOES work on 16 and 32 bit systems
#If Win16 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	Function GetSockAddress(ByVal S%) As String
	Dim AddrLen%
	Dim Ret%
#ElseIf Win32 Then
	Function GetSockAddress(ByVal S As Integer) As String
		Dim AddrLen As Integer
		Dim Ret As Integer
#End If
		Dim sa As sockaddr
		Dim szRet As String
		szRet = New String(Chr(0), 32)
		AddrLen = sockaddr_size
		If getsockname(S, sa, AddrLen) Then
			GetSockAddress = vbNullString
		Else
			GetSockAddress = SockAddressToString(sa)
		End If
	End Function
	
	'this function should work on 16 and 32 bit systems
	Function GetWSAErrorString(ByVal errnum As Integer) As String
		On Error Resume Next
		Select Case errnum
			Case 10004 : GetWSAErrorString = "Interrupted system call."
			Case 10009 : GetWSAErrorString = "Bad file number."
			Case 10013 : GetWSAErrorString = "Permission Denied."
			Case 10014 : GetWSAErrorString = "Bad Address."
			Case 10022 : GetWSAErrorString = "Invalid Argument."
			Case 10024 : GetWSAErrorString = "Too many open files."
			Case 10035 : GetWSAErrorString = "Operation would block."
			Case 10036 : GetWSAErrorString = "Operation now in progress."
			Case 10037 : GetWSAErrorString = "Operation already in progress."
			Case 10038 : GetWSAErrorString = "Socket operation on nonsocket."
			Case 10039 : GetWSAErrorString = "Destination address required."
			Case 10040 : GetWSAErrorString = "Message too long."
			Case 10041 : GetWSAErrorString = "Protocol wrong type for socket."
			Case 10042 : GetWSAErrorString = "Protocol not available."
			Case 10043 : GetWSAErrorString = "Protocol not supported."
			Case 10044 : GetWSAErrorString = "Socket type not supported."
			Case 10045 : GetWSAErrorString = "Operation not supported on socket."
			Case 10046 : GetWSAErrorString = "Protocol family not supported."
			Case 10047 : GetWSAErrorString = "Address family not supported by protocol family."
			Case 10048 : GetWSAErrorString = "Address already in use."
			Case 10049 : GetWSAErrorString = "Can't assign requested address."
			Case 10050 : GetWSAErrorString = "Network is down."
			Case 10051 : GetWSAErrorString = "Network is unreachable."
			Case 10052 : GetWSAErrorString = "Network dropped connection."
			Case 10053 : GetWSAErrorString = "Software caused connection abort."
			Case 10054 : GetWSAErrorString = "Connection reset by peer."
			Case 10055 : GetWSAErrorString = "No buffer space available."
			Case 10056 : GetWSAErrorString = "Socket is already connected."
			Case 10057 : GetWSAErrorString = "Socket is not connected."
			Case 10058 : GetWSAErrorString = "Can't send after socket shutdown."
			Case 10059 : GetWSAErrorString = "Too many references: can't splice."
			Case 10060 : GetWSAErrorString = "Connection timed out."
			Case 10061 : GetWSAErrorString = "Connection refused."
			Case 10062 : GetWSAErrorString = "Too many levels of symbolic links."
			Case 10063 : GetWSAErrorString = "File name too long."
			Case 10064 : GetWSAErrorString = "Host is down."
			Case 10065 : GetWSAErrorString = "No route to host."
			Case 10066 : GetWSAErrorString = "Directory not empty."
			Case 10067 : GetWSAErrorString = "Too many processes."
			Case 10068 : GetWSAErrorString = "Too many users."
			Case 10069 : GetWSAErrorString = "Disk quota exceeded."
			Case 10070 : GetWSAErrorString = "Stale NFS file handle."
			Case 10071 : GetWSAErrorString = "Too many levels of remote in path."
			Case 10091 : GetWSAErrorString = "Network subsystem is unusable."
			Case 10092 : GetWSAErrorString = "Winsock DLL cannot support this application."
			Case 10093 : GetWSAErrorString = "Winsock not initialized."
			Case 10101 : GetWSAErrorString = "Disconnect."
			Case 11001 : GetWSAErrorString = "Host not found."
			Case 11002 : GetWSAErrorString = "Nonauthoritative host not found."
			Case 11003 : GetWSAErrorString = "Nonrecoverable error."
			Case 11004 : GetWSAErrorString = "Valid name, no data record of requested type."
			Case Else
		End Select
	End Function
	
	'this function DOES work on 16 and 32 bit systems
	Function IpToAddr(ByVal AddrOrIP As String) As String
		On Error Resume Next
		IpToAddr = GetHostByAddress(GetHostByNameAlias(AddrOrIP))
		If Err.Number Then IpToAddr = WSA_NoName
	End Function
	
	'this function DOES work on 16 and 32 bit systems
	Function IrcGetAscIp(ByVal IPL As String) As String
		'this function is IRC specific, it expects a long ip stored in Network byte order, in a string
		'the kind that would be parsed out of a DCC command string
		On Error GoTo IrcGetAscIPError
		Dim lpStr As Integer
#If Win16 Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Dim nStr%
#ElseIf Win32 Then
		Dim nStr As Integer
#End If
		Dim retString As String
		Dim inn As Integer
		If Val(IPL) > 2147483647 Then
			inn = Val(IPL) - 4294967296#
		Else
			inn = Val(IPL)
		End If
		inn = ntohl(inn)
		retString = New String(Chr(0), 32)
		lpStr = inet_ntoa(inn)
		If lpStr = 0 Then
			IrcGetAscIp = "0.0.0.0"
			Exit Function
		End If
		nStr = lstrlen(lpStr)
		If nStr > 32 Then nStr = 32
		MemCopy(retString, lpStr, nStr)
		retString = Left(retString, nStr)
		IrcGetAscIp = retString
		Exit Function
IrcGetAscIPError: 
		IrcGetAscIp = "0.0.0.0"
		Exit Function
		Resume 
	End Function
	
	Public Function GetLongIp(ByVal IPS As String) As Integer
		GetLongIp = inet_addr(IPS)
	End Function
	
	
	'this function DOES work on 16 and 32 bit systems
	Function IrcGetLongIp(ByVal AscIp As String) As String
		'this function converts an ascii ip string into a long ip in network byte order
		'and stick it in a string suitable for use in a DCC command.
		On Error GoTo IrcGetLongIpError
		Dim inn As Integer
		inn = inet_addr(AscIp)
		inn = htonl(inn)
		If inn < 0 Then
			IrcGetLongIp = CObj(inn + 4294967296#)
			Exit Function
		Else
			IrcGetLongIp = CObj(inn)
			Exit Function
		End If
		Exit Function
IrcGetLongIpError: 
		IrcGetLongIp = "0"
		Exit Function
		Resume 
	End Function
	
	'this function should work on 16 and 32 bit systems
#If Win16 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	Public Function ListenForConnect(ByVal Port%, ByVal HWndToMsg%, ByVal Enlazar As String) As Integer
	Dim S%, dummy%
	Dim SelectOps%
#ElseIf Win32 Then
	Public Function ListenForConnect(ByVal Port As Integer, ByVal HWndToMsg As Integer, ByVal Enlazar As String) As Integer
		Dim S, dummy As Integer
		Dim SelectOps As Integer
#End If
		Dim sockin As sockaddr
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto sockin. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		sockin = saZero 'zero out the structure
		sockin.sin_family = AF_INET
		sockin.sin_port = htons(Port)
		If sockin.sin_port = INVALID_SOCKET Then
			ListenForConnect = INVALID_SOCKET
			Exit Function
		End If
		'UPGRADE_ISSUE: No se admite la función LenB. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"'
		If LenB(Enlazar) = 0 Then
			sockin.sin_addr = htonl(INADDR_ANY)
		Else
			sockin.sin_addr = inet_addr(Enlazar)
		End If
		If sockin.sin_addr = INADDR_NONE Then
			ListenForConnect = INVALID_SOCKET
			Exit Function
		End If
		S = Socket(PF_INET, SOCK_STREAM, 0)
		If S < 0 Then
			ListenForConnect = INVALID_SOCKET
			Exit Function
		End If
		
		'Agregado por Maraxus
		'If setsockopt(s, SOL_SOCKET, SO_CONDITIONAL_ACCEPT, True, 2) Then
		'    LogApiSock ("Error seteando conditional accept")
		'    Debug.Print "Error seteando conditional accept"
		'Else
		'    LogApiSock ("Conditional accept seteado")
		'    Debug.Print "Conditional accept seteado ^^"
		'End If
		
		If bind(S, sockin, sockaddr_size) Then
			If S > 0 Then
				dummy = apiclosesocket(S)
			End If
			ListenForConnect = INVALID_SOCKET
			Exit Function
		End If
		'    SelectOps = FD_READ Or FD_WRITE Or FD_CLOSE Or FD_ACCEPT
		SelectOps = FD_READ Or FD_CLOSE Or FD_ACCEPT
		If WSAAsyncSelect(S, HWndToMsg, 1025, SelectOps) Then
			If S > 0 Then
				dummy = apiclosesocket(S)
			End If
			ListenForConnect = SOCKET_ERROR
			Exit Function
		End If
		
		'If listen(s, 5) Then
		If listen(S, SOMAXCONN) Then
			If S > 0 Then
				dummy = apiclosesocket(S)
			End If
			ListenForConnect = INVALID_SOCKET
			Exit Function
		End If
		ListenForConnect = S
	End Function
	
	'this function should work on 16 and 32 bit systems
#If Win16 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Win16 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	Public Function kSendData(ByVal S%, vMessage As Variant) As Integer
#ElseIf Win32 Then
	Public Function kSendData(ByVal S As Integer, ByRef vMessage As Object) As Integer
#End If
		Dim TheMsg() As Byte
		Dim sTemp As String
		'UPGRADE_TODO: El código se actualizó para usar System.Text.UnicodeEncoding.Unicode.GetBytes(), que podría no tener el mismo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93DD716C-10E3-41BE-A4A8-3BA40157905B"'
		TheMsg = System.Text.UnicodeEncoding.Unicode.GetBytes(vbNullString)
		'UPGRADE_WARNING: VarType tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		Select Case VarType(vMessage)
			Case 8209 'byte array
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto vMessage. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				sTemp = vMessage
				'UPGRADE_TODO: El código se actualizó para usar System.Text.UnicodeEncoding.Unicode.GetBytes(), que podría no tener el mismo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93DD716C-10E3-41BE-A4A8-3BA40157905B"'
				TheMsg = System.Text.UnicodeEncoding.Unicode.GetBytes(sTemp)
			Case 8 'string, if we recieve a string, its assumed we are linemode
#If Win32 Then
				'UPGRADE_ISSUE: No se actualizó la constante vbFromUnicode. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"'
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto vMessage. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				sTemp = StrConv(vMessage, vbFromUnicode)
#Else
				'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Else no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
				sTemp = vMessage
#End If
			Case Else
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto vMessage. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				sTemp = CStr(vMessage)
#If Win32 Then
				'UPGRADE_ISSUE: No se actualizó la constante vbFromUnicode. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"'
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto vMessage. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				sTemp = StrConv(vMessage, vbFromUnicode)
#Else
				'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Else no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
				sTemp = vMessage
#End If
		End Select
		'UPGRADE_TODO: El código se actualizó para usar System.Text.UnicodeEncoding.Unicode.GetBytes(), que podría no tener el mismo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93DD716C-10E3-41BE-A4A8-3BA40157905B"'
		TheMsg = System.Text.UnicodeEncoding.Unicode.GetBytes(sTemp)
		If UBound(TheMsg) > -1 Then
			kSendData = send(S, TheMsg(0), UBound(TheMsg) + 1, 0)
		End If
	End Function
	
	Public Function SockAddressToString(ByRef sa As sockaddr) As String
		SockAddressToString = GetAscIP(sa.sin_addr) & ":" & ntohs(sa.sin_port)
	End Function
	
	Public Function StartWinsock(ByRef sDescription As String) As Boolean
		Dim StartupData As WSADataType
		If Not WSAStartedUp Then
			'If Not WSAStartup(&H101, StartupData) Then
			If Not WSAStartup(&H202, StartupData) Then 'Use sockets v2.2 instead of 1.1 (Maraxus)
				WSAStartedUp = True
				'            Debug.Print "wVersion="; StartupData.wVersion, "wHighVersion="; StartupData.wHighVersion
				'            Debug.Print "If wVersion == 257 then everything is kewl"
				'            Debug.Print "szDescription="; StartupData.szDescription
				'            Debug.Print "szSystemStatus="; StartupData.szSystemStatus
				'            Debug.Print "iMaxSockets="; StartupData.iMaxSockets, "iMaxUdpDg="; StartupData.iMaxUdpDg
				sDescription = StartupData.szDescription
			Else
				WSAStartedUp = False
			End If
		End If
		StartWinsock = WSAStartedUp
	End Function
	
	Public Function WSAMakeSelectReply(ByRef TheEvent As Short, ByRef TheError As Short) As Integer
		WSAMakeSelectReply = (TheError * &H10000) + CShort(TheEvent And &HFFFF)
	End Function
	
#End If
End Module