Option Strict Off
Option Explicit On
Friend Class clsEstadisticasIPC
	'**************************************************************
	' clsEstadisticasIPC.cls
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
	
	
	Private Declare Function RegisterWindowMessage Lib "user32"  Alias "RegisterWindowMessageA"(ByVal lpString As String) As Integer
	Private Declare Function SendMessageLong Lib "user32"  Alias "SendMessageA"(ByVal hWnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
	
	Private hVentana, sMensaje, hVentanaMia As Integer
	
	Private Declare Function GetWindowText Lib "user32"  Alias "GetWindowTextA"(ByVal hWnd As Integer, ByVal lpString As String, ByVal cch As Integer) As Integer
	Private Declare Function GetWindowTextLength Lib "user32"  Alias "GetWindowTextLengthA"(ByVal hWnd As Integer) As Integer
	Private Declare Function GetWindow Lib "user32" (ByVal hWnd As Integer, ByVal wCmd As Integer) As Integer
	
	Private Const GW_HWNDFIRST As Short = 0
	Private Const GW_HWNDNEXT As Short = 2
	
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
		Dim W As Integer
		Dim L As Integer
		Dim T As String
		
		
		W = GetWindow(Wnd, GW_HWNDFIRST)
		
		While W <> 0
			L = GetWindowTextLength(W)
			
			If L > 0 Then
				T = Space(L + 1)
				L = GetWindowText(W, T, L + 1)
				
				If Left(T, Len(str_Renamed)) = str_Renamed Then
					BuscaVentana = W
					Exit Function
				End If
			End If
			
			W = GetWindow(W, GW_HWNDNEXT)
		End While
		
		BuscaVentana = 0
		
	End Function
	
	Public Function Informar(ByVal QueCosa As EstaNotificaciones, ByVal Parametro As Integer) As Integer
		Call BuscaWndEstadisticas()
		If hVentana <> 0 Then
			Informar = SendMessageLong(hVentana, sMensaje, QueCosa, Parametro)
		End If
		
	End Function
	
	Public Function EstadisticasAndando() As Boolean
		
		Call BuscaWndEstadisticas()
		'Ret = SendNotifyMessage(hVentana, sMensaje, 0, 0)
		EstadisticasAndando = (hVentana <> 0)
		
	End Function
	
	Public Sub Inicializa(ByVal hWnd As Integer)
		hVentanaMia = hWnd
		sMensaje = RegisterWindowMessage("EstadisticasAO")
		
	End Sub
	
	Private Sub BuscaWndEstadisticas()
		hVentana = BuscaVentana(hVentanaMia, "Servidor de estadisticas AO")
		
	End Sub
End Class