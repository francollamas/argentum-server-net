Option Strict Off
Option Explicit On
Friend Class FrmInterv
	Inherits System.Windows.Forms.Form
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
	
	
	Public Sub AplicarIntervalos()
		
		'¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿ Intervalos del main loop ¿?¿?¿?¿?¿?¿?¿?¿?¿
		SanaIntervaloSinDescansar = Val(txtSanaIntervaloSinDescansar.Text)
		StaminaIntervaloSinDescansar = Val(txtStaminaIntervaloSinDescansar.Text)
		SanaIntervaloDescansar = Val(txtSanaIntervaloDescansar.Text)
		StaminaIntervaloDescansar = Val(txtStaminaIntervaloDescansar.Text)
		IntervaloSed = Val(txtIntervaloSed.Text)
		IntervaloHambre = Val(txtIntervaloHambre.Text)
		IntervaloVeneno = Val(txtIntervaloVeneno.Text)
		IntervaloParalizado = Val(txtIntervaloParalizado.Text)
		IntervaloInvisible = Val(txtIntervaloInvisible.Text)
		IntervaloFrio = Val(txtIntervaloFrio.Text)
		IntervaloWavFx = Val(txtIntervaloWAVFX.Text)
		IntervaloInvocacion = Val(txtInvocacion.Text)
		IntervaloParaConexion = Val(txtIntervaloParaConexion.Text)
		
		'///////////////// TIMERS \\\\\\\\\\\\\\\\\\\
		
		IntervaloUserPuedeCastear = Val(txtIntervaloLanzaHechizo.Text)
		'UPGRADE_WARNING: La propiedad Timer npcataca.Interval no puede tener un valor de 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
		frmMain.npcataca.Interval = Val(txtNPCPuedeAtacar.Text)
		'UPGRADE_WARNING: La propiedad Timer TIMER_AI.Interval no puede tener un valor de 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
		frmMain.TIMER_AI.Interval = Val(txtAI.Text)
		IntervaloUserPuedeTrabajar = Val(txtTrabajo.Text)
		IntervaloUserPuedeAtacar = Val(txtPuedeAtacar.Text)
		'UPGRADE_WARNING: La propiedad Timer tLluvia.Interval no puede tener un valor de 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
		frmMain.tLluvia.Interval = Val(txtIntervaloPerdidaStaminaLluvia.Text)
		
		
		
	End Sub
	
	Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click
		On Error Resume Next
		Call AplicarIntervalos()
		
	End Sub
	
	Private Sub Command2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command2.Click
		
		On Error GoTo Err_Renamed
		
		'Intervalos
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "SanaIntervaloSinDescansar", Str(SanaIntervaloSinDescansar))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "StaminaIntervaloSinDescansar", Str(StaminaIntervaloSinDescansar))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "SanaIntervaloDescansar", Str(SanaIntervaloDescansar))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "StaminaIntervaloDescansar", Str(StaminaIntervaloDescansar))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloSed", Str(IntervaloSed))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloHambre", Str(IntervaloHambre))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloVeneno", Str(IntervaloVeneno))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloParalizado", Str(IntervaloParalizado))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloInvisible", Str(IntervaloInvisible))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloFrio", Str(IntervaloFrio))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloWAVFX", Str(IntervaloWavFx))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloInvocacion", Str(IntervaloInvocacion))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloParaConexion", Str(IntervaloParaConexion))
		
		'&&&&&&&&&&&&&&&&&&&&& TIMERS &&&&&&&&&&&&&&&&&&&&&&&
		
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloLanzaHechizo", Str(IntervaloUserPuedeCastear))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloNpcAI", CStr(frmMain.TIMER_AI.Interval))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloNpcPuedeAtacar", CStr(frmMain.npcataca.Interval))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloTrabajo", Str(IntervaloUserPuedeTrabajar))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloUserPuedeAtacar", Str(IntervaloUserPuedeAtacar))
		Call WriteVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloPerdidaStaminaLluvia", CStr(frmMain.tLluvia.Interval))
		
		
		MsgBox("Los intervalos se han guardado sin problemas.")
		
		Exit Sub
Err_Renamed: 
		MsgBox("Error al intentar grabar los intervalos")
	End Sub
	
	Private Sub ok_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles ok.Click
		Me.Visible = False
	End Sub
End Class