<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmMain
#Region "Código generado por el Diseñador de Windows Forms "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'Llamada necesaria para el Diseñador de Windows Forms.
		InitializeComponent()
	End Sub
	'Form invalida a Dispose para limpiar la lista de componentes.
	<System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Requerido por el Diseñador de Windows Forms
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
	Public WithEvents mnuServidor As System.Windows.Forms.ToolStripMenuItem
	Public WithEvents mnuSystray As System.Windows.Forms.ToolStripMenuItem
	Public WithEvents mnuCerrar As System.Windows.Forms.ToolStripMenuItem
	Public WithEvents mnuControles As System.Windows.Forms.ToolStripMenuItem
	Public WithEvents mnuMostrar As System.Windows.Forms.ToolStripMenuItem
	Public WithEvents mnuSalir As System.Windows.Forms.ToolStripMenuItem
	Public WithEvents mnuPopUp As System.Windows.Forms.ToolStripMenuItem
	Public WithEvents MainMenu1 As System.Windows.Forms.MenuStrip
	Public WithEvents txtChat As System.Windows.Forms.TextBox
	Public WithEvents tPiqueteC As System.Windows.Forms.Timer
	Public WithEvents packetResend As System.Windows.Forms.Timer
	Public WithEvents securityTimer As System.Windows.Forms.Timer
	Public WithEvents SUPERLOG As System.Windows.Forms.CheckBox
	Public WithEvents CMDDUMP As System.Windows.Forms.Button
	Public WithEvents FX As System.Windows.Forms.Timer
	Public WithEvents Auditoria As System.Windows.Forms.Timer
	Public WithEvents GameTimer As System.Windows.Forms.Timer
	Public WithEvents tLluviaEvent As System.Windows.Forms.Timer
	Public WithEvents tLluvia As System.Windows.Forms.Timer
	Public WithEvents AutoSave As System.Windows.Forms.Timer
	Public WithEvents npcataca As System.Windows.Forms.Timer
	Public WithEvents KillLog As System.Windows.Forms.Timer
	Public WithEvents TIMER_AI As System.Windows.Forms.Timer
	Public WithEvents Command2 As System.Windows.Forms.Button
	Public WithEvents Command1 As System.Windows.Forms.Button
	Public WithEvents BroadMsg As System.Windows.Forms.TextBox
	Public WithEvents _Label1_0 As System.Windows.Forms.Label
	Public WithEvents Frame1 As System.Windows.Forms.GroupBox
	Public WithEvents Escuch As System.Windows.Forms.Label
	Public WithEvents CantUsuarios As System.Windows.Forms.Label
	Public WithEvents txStatus As System.Windows.Forms.Label
	Public WithEvents Label1 As Microsoft.VisualBasic.Compatibility.VB6.LabelArray
	'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
	'Se puede modificar mediante el Diseñador de Windows Forms.
	'No lo modifique con el editor de código.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmMain))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.MainMenu1 = New System.Windows.Forms.MenuStrip
		Me.mnuControles = New System.Windows.Forms.ToolStripMenuItem
		Me.mnuServidor = New System.Windows.Forms.ToolStripMenuItem
		Me.mnuSystray = New System.Windows.Forms.ToolStripMenuItem
		Me.mnuCerrar = New System.Windows.Forms.ToolStripMenuItem
		Me.mnuPopUp = New System.Windows.Forms.ToolStripMenuItem
		Me.mnuMostrar = New System.Windows.Forms.ToolStripMenuItem
		Me.mnuSalir = New System.Windows.Forms.ToolStripMenuItem
		Me.txtChat = New System.Windows.Forms.TextBox
		Me.tPiqueteC = New System.Windows.Forms.Timer(components)
		Me.packetResend = New System.Windows.Forms.Timer(components)
		Me.securityTimer = New System.Windows.Forms.Timer(components)
		Me.SUPERLOG = New System.Windows.Forms.CheckBox
		Me.CMDDUMP = New System.Windows.Forms.Button
		Me.FX = New System.Windows.Forms.Timer(components)
		Me.Auditoria = New System.Windows.Forms.Timer(components)
		Me.GameTimer = New System.Windows.Forms.Timer(components)
		Me.tLluviaEvent = New System.Windows.Forms.Timer(components)
		Me.tLluvia = New System.Windows.Forms.Timer(components)
		Me.AutoSave = New System.Windows.Forms.Timer(components)
		Me.npcataca = New System.Windows.Forms.Timer(components)
		Me.KillLog = New System.Windows.Forms.Timer(components)
		Me.TIMER_AI = New System.Windows.Forms.Timer(components)
		Me.Frame1 = New System.Windows.Forms.GroupBox
		Me.Command2 = New System.Windows.Forms.Button
		Me.Command1 = New System.Windows.Forms.Button
		Me.BroadMsg = New System.Windows.Forms.TextBox
		Me._Label1_0 = New System.Windows.Forms.Label
		Me.Escuch = New System.Windows.Forms.Label
		Me.CantUsuarios = New System.Windows.Forms.Label
		Me.txStatus = New System.Windows.Forms.Label
		Me.Label1 = New Microsoft.VisualBasic.Compatibility.VB6.LabelArray(components)
		Me.MainMenu1.SuspendLayout()
		Me.Frame1.SuspendLayout()
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		CType(Me.Label1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.Text = "Argentum Online"
		Me.ClientSize = New System.Drawing.Size(346, 347)
		Me.Location = New System.Drawing.Point(130, 121)
		Me.ControlBox = False
		Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.ForeColor = System.Drawing.SystemColors.Menu
		Me.Icon = CType(resources.GetObject("frmMain.Icon"), System.Drawing.Icon)
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Enabled = True
		Me.KeyPreview = False
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.ShowInTaskbar = True
		Me.HelpButton = False
		Me.Name = "frmMain"
		Me.mnuControles.Name = "mnuControles"
		Me.mnuControles.Text = "Argentum"
		Me.mnuControles.Checked = False
		Me.mnuControles.Enabled = True
		Me.mnuControles.Visible = True
		Me.mnuServidor.Name = "mnuServidor"
		Me.mnuServidor.Text = "Configuracion"
		Me.mnuServidor.Checked = False
		Me.mnuServidor.Enabled = True
		Me.mnuServidor.Visible = True
		Me.mnuSystray.Name = "mnuSystray"
		Me.mnuSystray.Text = "Systray Servidor"
		Me.mnuSystray.Checked = False
		Me.mnuSystray.Enabled = True
		Me.mnuSystray.Visible = True
		Me.mnuCerrar.Name = "mnuCerrar"
		Me.mnuCerrar.Text = "Cerrar Servidor"
		Me.mnuCerrar.Checked = False
		Me.mnuCerrar.Enabled = True
		Me.mnuCerrar.Visible = True
		Me.mnuPopUp.Name = "mnuPopUp"
		Me.mnuPopUp.Text = "PopUpMenu"
		Me.mnuPopUp.Visible = False
		Me.mnuPopUp.Checked = False
		Me.mnuPopUp.Enabled = True
		Me.mnuMostrar.Name = "mnuMostrar"
		Me.mnuMostrar.Text = "&Mostrar"
		Me.mnuMostrar.Checked = False
		Me.mnuMostrar.Enabled = True
		Me.mnuMostrar.Visible = True
		Me.mnuSalir.Name = "mnuSalir"
		Me.mnuSalir.Text = "&Salir"
		Me.mnuSalir.Checked = False
		Me.mnuSalir.Enabled = True
		Me.mnuSalir.Visible = True
		Me.txtChat.AutoSize = False
		Me.txtChat.Size = New System.Drawing.Size(329, 185)
		Me.txtChat.Location = New System.Drawing.Point(8, 152)
		Me.txtChat.MultiLine = True
		Me.txtChat.TabIndex = 10
		Me.txtChat.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtChat.AcceptsReturn = True
		Me.txtChat.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtChat.BackColor = System.Drawing.SystemColors.Window
		Me.txtChat.CausesValidation = True
		Me.txtChat.Enabled = True
		Me.txtChat.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtChat.HideSelection = True
		Me.txtChat.ReadOnly = False
		Me.txtChat.Maxlength = 0
		Me.txtChat.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtChat.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtChat.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtChat.TabStop = True
		Me.txtChat.Visible = True
		Me.txtChat.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtChat.Name = "txtChat"
		Me.tPiqueteC.Enabled = False
		Me.tPiqueteC.Interval = 6000
		Me.packetResend.Interval = 10
		Me.packetResend.Enabled = True
		Me.securityTimer.Enabled = False
		Me.securityTimer.Interval = 10000
		Me.SUPERLOG.Text = "log"
		Me.SUPERLOG.Size = New System.Drawing.Size(41, 17)
		Me.SUPERLOG.Location = New System.Drawing.Point(208, 56)
		Me.SUPERLOG.TabIndex = 9
		Me.SUPERLOG.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.SUPERLOG.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft
		Me.SUPERLOG.FlatStyle = System.Windows.Forms.FlatStyle.Standard
		Me.SUPERLOG.BackColor = System.Drawing.SystemColors.Control
		Me.SUPERLOG.CausesValidation = True
		Me.SUPERLOG.Enabled = True
		Me.SUPERLOG.ForeColor = System.Drawing.SystemColors.ControlText
		Me.SUPERLOG.Cursor = System.Windows.Forms.Cursors.Default
		Me.SUPERLOG.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.SUPERLOG.Appearance = System.Windows.Forms.Appearance.Normal
		Me.SUPERLOG.TabStop = True
		Me.SUPERLOG.CheckState = System.Windows.Forms.CheckState.Unchecked
		Me.SUPERLOG.Visible = True
		Me.SUPERLOG.Name = "SUPERLOG"
		Me.CMDDUMP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.CMDDUMP.Text = "dump"
		Me.CMDDUMP.Size = New System.Drawing.Size(81, 17)
		Me.CMDDUMP.Location = New System.Drawing.Point(248, 56)
		Me.CMDDUMP.TabIndex = 8
		Me.CMDDUMP.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.CMDDUMP.BackColor = System.Drawing.SystemColors.Control
		Me.CMDDUMP.CausesValidation = True
		Me.CMDDUMP.Enabled = True
		Me.CMDDUMP.ForeColor = System.Drawing.SystemColors.ControlText
		Me.CMDDUMP.Cursor = System.Windows.Forms.Cursors.Default
		Me.CMDDUMP.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.CMDDUMP.TabStop = True
		Me.CMDDUMP.Name = "CMDDUMP"
		Me.FX.Enabled = False
		Me.FX.Interval = 4000
		Me.Auditoria.Enabled = False
		Me.Auditoria.Interval = 1000
		Me.GameTimer.Enabled = False
		Me.GameTimer.Interval = 40
		Me.tLluviaEvent.Enabled = False
		Me.tLluviaEvent.Interval = 60000
		Me.tLluvia.Enabled = False
		Me.tLluvia.Interval = 500
		Me.AutoSave.Enabled = False
		Me.AutoSave.Interval = 60000
		Me.npcataca.Enabled = False
		Me.npcataca.Interval = 4000
		Me.KillLog.Enabled = False
		Me.KillLog.Interval = 60000
		Me.TIMER_AI.Enabled = False
		Me.TIMER_AI.Interval = 100
		Me.Frame1.Text = "BroadCast"
		Me.Frame1.Size = New System.Drawing.Size(329, 73)
		Me.Frame1.Location = New System.Drawing.Point(8, 64)
		Me.Frame1.TabIndex = 2
		Me.Frame1.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame1.BackColor = System.Drawing.SystemColors.Control
		Me.Frame1.Enabled = True
		Me.Frame1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame1.Visible = True
		Me.Frame1.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame1.Name = "Frame1"
		Me.Command2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command2.Text = "Broadcast consola"
		Me.Command2.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command2.Size = New System.Drawing.Size(153, 17)
		Me.Command2.Location = New System.Drawing.Point(168, 48)
		Me.Command2.TabIndex = 6
		Me.Command2.BackColor = System.Drawing.SystemColors.Control
		Me.Command2.CausesValidation = True
		Me.Command2.Enabled = True
		Me.Command2.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command2.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command2.TabStop = True
		Me.Command2.Name = "Command2"
		Me.Command1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command1.Text = "Broadcast clientes"
		Me.Command1.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command1.Size = New System.Drawing.Size(153, 17)
		Me.Command1.Location = New System.Drawing.Point(8, 48)
		Me.Command1.TabIndex = 5
		Me.Command1.BackColor = System.Drawing.SystemColors.Control
		Me.Command1.CausesValidation = True
		Me.Command1.Enabled = True
		Me.Command1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command1.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command1.TabStop = True
		Me.Command1.Name = "Command1"
		Me.BroadMsg.AutoSize = False
		Me.BroadMsg.Size = New System.Drawing.Size(249, 21)
		Me.BroadMsg.Location = New System.Drawing.Point(72, 16)
		Me.BroadMsg.TabIndex = 4
		Me.BroadMsg.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.BroadMsg.AcceptsReturn = True
		Me.BroadMsg.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.BroadMsg.BackColor = System.Drawing.SystemColors.Window
		Me.BroadMsg.CausesValidation = True
		Me.BroadMsg.Enabled = True
		Me.BroadMsg.ForeColor = System.Drawing.SystemColors.WindowText
		Me.BroadMsg.HideSelection = True
		Me.BroadMsg.ReadOnly = False
		Me.BroadMsg.Maxlength = 0
		Me.BroadMsg.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.BroadMsg.MultiLine = False
		Me.BroadMsg.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.BroadMsg.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.BroadMsg.TabStop = True
		Me.BroadMsg.Visible = True
		Me.BroadMsg.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.BroadMsg.Name = "BroadMsg"
		Me._Label1_0.Text = "Mensaje"
		Me._Label1_0.Size = New System.Drawing.Size(57, 17)
		Me._Label1_0.Location = New System.Drawing.Point(8, 16)
		Me._Label1_0.TabIndex = 3
		Me._Label1_0.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._Label1_0.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me._Label1_0.BackColor = System.Drawing.SystemColors.Control
		Me._Label1_0.Enabled = True
		Me._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText
		Me._Label1_0.Cursor = System.Windows.Forms.Cursors.Default
		Me._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me._Label1_0.UseMnemonic = True
		Me._Label1_0.Visible = True
		Me._Label1_0.AutoSize = False
		Me._Label1_0.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me._Label1_0.Name = "_Label1_0"
		Me.Escuch.Text = "Label2"
		Me.Escuch.Size = New System.Drawing.Size(89, 17)
		Me.Escuch.Location = New System.Drawing.Point(216, 40)
		Me.Escuch.TabIndex = 7
		Me.Escuch.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Escuch.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Escuch.BackColor = System.Drawing.SystemColors.Control
		Me.Escuch.Enabled = True
		Me.Escuch.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Escuch.Cursor = System.Windows.Forms.Cursors.Default
		Me.Escuch.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Escuch.UseMnemonic = True
		Me.Escuch.Visible = True
		Me.Escuch.AutoSize = False
		Me.Escuch.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Escuch.Name = "Escuch"
		Me.CantUsuarios.BackColor = System.Drawing.Color.Transparent
		Me.CantUsuarios.Text = "Numero de usuarios:"
		Me.CantUsuarios.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.CantUsuarios.ForeColor = System.Drawing.Color.Black
		Me.CantUsuarios.Size = New System.Drawing.Size(115, 13)
		Me.CantUsuarios.Location = New System.Drawing.Point(8, 40)
		Me.CantUsuarios.TabIndex = 1
		Me.CantUsuarios.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.CantUsuarios.Enabled = True
		Me.CantUsuarios.Cursor = System.Windows.Forms.Cursors.Default
		Me.CantUsuarios.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.CantUsuarios.UseMnemonic = True
		Me.CantUsuarios.Visible = True
		Me.CantUsuarios.AutoSize = True
		Me.CantUsuarios.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.CantUsuarios.Name = "CantUsuarios"
		Me.txStatus.ForeColor = System.Drawing.Color.Red
		Me.txStatus.Size = New System.Drawing.Size(3, 14)
		Me.txStatus.Location = New System.Drawing.Point(8, 392)
		Me.txStatus.TabIndex = 0
		Me.txStatus.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txStatus.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.txStatus.BackColor = System.Drawing.Color.Transparent
		Me.txStatus.Enabled = True
		Me.txStatus.Cursor = System.Windows.Forms.Cursors.Default
		Me.txStatus.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txStatus.UseMnemonic = True
		Me.txStatus.Visible = True
		Me.txStatus.AutoSize = True
		Me.txStatus.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txStatus.Name = "txStatus"
		Me.Controls.Add(txtChat)
		Me.Controls.Add(SUPERLOG)
		Me.Controls.Add(CMDDUMP)
		Me.Controls.Add(Frame1)
		Me.Controls.Add(Escuch)
		Me.Controls.Add(CantUsuarios)
		Me.Controls.Add(txStatus)
		Me.Frame1.Controls.Add(Command2)
		Me.Frame1.Controls.Add(Command1)
		Me.Frame1.Controls.Add(BroadMsg)
		Me.Frame1.Controls.Add(_Label1_0)
		Me.Label1.SetIndex(_Label1_0, CType(0, Short))
		CType(Me.Label1, System.ComponentModel.ISupportInitialize).EndInit()
		MainMenu1.Items.AddRange(New System.Windows.Forms.ToolStripItem(){Me.mnuControles, Me.mnuPopUp})
		mnuControles.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem(){Me.mnuServidor, Me.mnuSystray, Me.mnuCerrar})
		mnuPopUp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem(){Me.mnuMostrar, Me.mnuSalir})
		Me.Controls.Add(MainMenu1)
		Me.MainMenu1.ResumeLayout(False)
		Me.Frame1.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class