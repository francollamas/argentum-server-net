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
	Public WithEvents mnuCerrar As System.Windows.Forms.ToolStripMenuItem
	Public WithEvents mnuControles As System.Windows.Forms.ToolStripMenuItem
	Public WithEvents MainMenu1 As System.Windows.Forms.MenuStrip
	Public WithEvents txtChat As System.Windows.Forms.TextBox
	Public WithEvents tPiqueteC As System.Windows.Forms.Timer
	Public WithEvents packetResend As System.Windows.Forms.Timer
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.MainMenu1 = New System.Windows.Forms.MenuStrip()
        Me.mnuControles = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuServidor = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCerrar = New System.Windows.Forms.ToolStripMenuItem()
        Me.txtChat = New System.Windows.Forms.TextBox()
        Me.tPiqueteC = New System.Windows.Forms.Timer(Me.components)
        Me.packetResend = New System.Windows.Forms.Timer(Me.components)
        Me.SUPERLOG = New System.Windows.Forms.CheckBox()
        Me.CMDDUMP = New System.Windows.Forms.Button()
        Me.FX = New System.Windows.Forms.Timer(Me.components)
        Me.Auditoria = New System.Windows.Forms.Timer(Me.components)
        Me.GameTimer = New System.Windows.Forms.Timer(Me.components)
        Me.tLluviaEvent = New System.Windows.Forms.Timer(Me.components)
        Me.tLluvia = New System.Windows.Forms.Timer(Me.components)
        Me.AutoSave = New System.Windows.Forms.Timer(Me.components)
        Me.npcataca = New System.Windows.Forms.Timer(Me.components)
        Me.KillLog = New System.Windows.Forms.Timer(Me.components)
        Me.TIMER_AI = New System.Windows.Forms.Timer(Me.components)
        Me.Frame1 = New System.Windows.Forms.GroupBox()
        Me.Command2 = New System.Windows.Forms.Button()
        Me.Command1 = New System.Windows.Forms.Button()
        Me.BroadMsg = New System.Windows.Forms.TextBox()
        Me._Label1_0 = New System.Windows.Forms.Label()
        Me.Escuch = New System.Windows.Forms.Label()
        Me.CantUsuarios = New System.Windows.Forms.Label()
        Me.txStatus = New System.Windows.Forms.Label()
        Me.Label1 = New Microsoft.VisualBasic.Compatibility.VB6.LabelArray(Me.components)
        Me.ConnectionTimer = New System.Windows.Forms.Timer(Me.components)
        Me.MainMenu1.SuspendLayout()
        Me.Frame1.SuspendLayout()
        CType(Me.Label1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MainMenu1
        '
        Me.MainMenu1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuControles})
        Me.MainMenu1.Location = New System.Drawing.Point(0, 0)
        Me.MainMenu1.Name = "MainMenu1"
        Me.MainMenu1.Size = New System.Drawing.Size(346, 24)
        Me.MainMenu1.TabIndex = 11
        '
        'mnuControles
        '
        Me.mnuControles.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuServidor, Me.mnuCerrar})
        Me.mnuControles.Name = "mnuControles"
        Me.mnuControles.Size = New System.Drawing.Size(73, 20)
        Me.mnuControles.Text = "Argentum"
        '
        'mnuServidor
        '
        Me.mnuServidor.Name = "mnuServidor"
        Me.mnuServidor.Size = New System.Drawing.Size(152, 22)
        Me.mnuServidor.Text = "Configuracion"
        '
        'mnuCerrar
        '
        Me.mnuCerrar.Name = "mnuCerrar"
        Me.mnuCerrar.Size = New System.Drawing.Size(152, 22)
        Me.mnuCerrar.Text = "Cerrar Servidor"
        '
        'txtChat
        '
        Me.txtChat.AcceptsReturn = True
        Me.txtChat.BackColor = System.Drawing.SystemColors.Window
        Me.txtChat.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtChat.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtChat.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtChat.Location = New System.Drawing.Point(8, 152)
        Me.txtChat.MaxLength = 0
        Me.txtChat.Multiline = True
        Me.txtChat.Name = "txtChat"
        Me.txtChat.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtChat.Size = New System.Drawing.Size(329, 185)
        Me.txtChat.TabIndex = 10
        '
        'tPiqueteC
        '
        Me.tPiqueteC.Interval = 6000
        '
        'packetResend
        '
        Me.packetResend.Enabled = True
        Me.packetResend.Interval = 10
        '
        'SUPERLOG
        '
        Me.SUPERLOG.BackColor = System.Drawing.SystemColors.Control
        Me.SUPERLOG.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SUPERLOG.ForeColor = System.Drawing.SystemColors.ControlText
        Me.SUPERLOG.Location = New System.Drawing.Point(208, 56)
        Me.SUPERLOG.Name = "SUPERLOG"
        Me.SUPERLOG.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SUPERLOG.Size = New System.Drawing.Size(41, 17)
        Me.SUPERLOG.TabIndex = 9
        Me.SUPERLOG.Text = "log"
        Me.SUPERLOG.UseVisualStyleBackColor = False
        '
        'CMDDUMP
        '
        Me.CMDDUMP.BackColor = System.Drawing.SystemColors.Control
        Me.CMDDUMP.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CMDDUMP.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CMDDUMP.Location = New System.Drawing.Point(248, 56)
        Me.CMDDUMP.Name = "CMDDUMP"
        Me.CMDDUMP.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.CMDDUMP.Size = New System.Drawing.Size(81, 17)
        Me.CMDDUMP.TabIndex = 8
        Me.CMDDUMP.Text = "dump"
        Me.CMDDUMP.UseVisualStyleBackColor = False
        '
        'FX
        '
        Me.FX.Interval = 4000
        '
        'Auditoria
        '
        Me.Auditoria.Interval = 1000
        '
        'GameTimer
        '
        Me.GameTimer.Interval = 40
        '
        'tLluviaEvent
        '
        Me.tLluviaEvent.Interval = 60000
        '
        'tLluvia
        '
        Me.tLluvia.Interval = 500
        '
        'AutoSave
        '
        Me.AutoSave.Interval = 60000
        '
        'npcataca
        '
        Me.npcataca.Interval = 4000
        '
        'KillLog
        '
        Me.KillLog.Interval = 60000
        '
        'TIMER_AI
        '
        '
        'Frame1
        '
        Me.Frame1.BackColor = System.Drawing.SystemColors.Control
        Me.Frame1.Controls.Add(Me.Command2)
        Me.Frame1.Controls.Add(Me.Command1)
        Me.Frame1.Controls.Add(Me.BroadMsg)
        Me.Frame1.Controls.Add(Me._Label1_0)
        Me.Frame1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Frame1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Frame1.Location = New System.Drawing.Point(8, 64)
        Me.Frame1.Name = "Frame1"
        Me.Frame1.Padding = New System.Windows.Forms.Padding(0)
        Me.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Frame1.Size = New System.Drawing.Size(329, 73)
        Me.Frame1.TabIndex = 2
        Me.Frame1.TabStop = False
        Me.Frame1.Text = "BroadCast"
        '
        'Command2
        '
        Me.Command2.BackColor = System.Drawing.SystemColors.Control
        Me.Command2.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Command2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command2.Location = New System.Drawing.Point(168, 48)
        Me.Command2.Name = "Command2"
        Me.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command2.Size = New System.Drawing.Size(153, 17)
        Me.Command2.TabIndex = 6
        Me.Command2.Text = "Broadcast consola"
        Me.Command2.UseVisualStyleBackColor = False
        '
        'Command1
        '
        Me.Command1.BackColor = System.Drawing.SystemColors.Control
        Me.Command1.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Command1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command1.Location = New System.Drawing.Point(8, 48)
        Me.Command1.Name = "Command1"
        Me.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command1.Size = New System.Drawing.Size(153, 17)
        Me.Command1.TabIndex = 5
        Me.Command1.Text = "Broadcast clientes"
        Me.Command1.UseVisualStyleBackColor = False
        '
        'BroadMsg
        '
        Me.BroadMsg.AcceptsReturn = True
        Me.BroadMsg.BackColor = System.Drawing.SystemColors.Window
        Me.BroadMsg.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.BroadMsg.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BroadMsg.ForeColor = System.Drawing.SystemColors.WindowText
        Me.BroadMsg.Location = New System.Drawing.Point(72, 16)
        Me.BroadMsg.MaxLength = 0
        Me.BroadMsg.Name = "BroadMsg"
        Me.BroadMsg.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.BroadMsg.Size = New System.Drawing.Size(249, 20)
        Me.BroadMsg.TabIndex = 4
        '
        '_Label1_0
        '
        Me._Label1_0.BackColor = System.Drawing.SystemColors.Control
        Me._Label1_0.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label1.SetIndex(Me._Label1_0, CType(0, Short))
        Me._Label1_0.Location = New System.Drawing.Point(8, 16)
        Me._Label1_0.Name = "_Label1_0"
        Me._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._Label1_0.Size = New System.Drawing.Size(57, 17)
        Me._Label1_0.TabIndex = 3
        Me._Label1_0.Text = "Mensaje"
        '
        'Escuch
        '
        Me.Escuch.BackColor = System.Drawing.SystemColors.Control
        Me.Escuch.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Escuch.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Escuch.Location = New System.Drawing.Point(216, 40)
        Me.Escuch.Name = "Escuch"
        Me.Escuch.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Escuch.Size = New System.Drawing.Size(89, 17)
        Me.Escuch.TabIndex = 7
        Me.Escuch.Text = "Label2"
        '
        'CantUsuarios
        '
        Me.CantUsuarios.AutoSize = True
        Me.CantUsuarios.BackColor = System.Drawing.Color.Transparent
        Me.CantUsuarios.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CantUsuarios.ForeColor = System.Drawing.Color.Black
        Me.CantUsuarios.Location = New System.Drawing.Point(8, 40)
        Me.CantUsuarios.Name = "CantUsuarios"
        Me.CantUsuarios.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.CantUsuarios.Size = New System.Drawing.Size(122, 13)
        Me.CantUsuarios.TabIndex = 1
        Me.CantUsuarios.Text = "Numero de usuarios:"
        '
        'txStatus
        '
        Me.txStatus.AutoSize = True
        Me.txStatus.BackColor = System.Drawing.Color.Transparent
        Me.txStatus.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txStatus.ForeColor = System.Drawing.Color.Red
        Me.txStatus.Location = New System.Drawing.Point(8, 392)
        Me.txStatus.Name = "txStatus"
        Me.txStatus.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txStatus.Size = New System.Drawing.Size(0, 14)
        Me.txStatus.TabIndex = 0
        '
        'ConnectionTimer
        '
        Me.ConnectionTimer.Enabled = True
        Me.ConnectionTimer.Interval = 5
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(346, 347)
        Me.Controls.Add(Me.txtChat)
        Me.Controls.Add(Me.SUPERLOG)
        Me.Controls.Add(Me.CMDDUMP)
        Me.Controls.Add(Me.Frame1)
        Me.Controls.Add(Me.Escuch)
        Me.Controls.Add(Me.CantUsuarios)
        Me.Controls.Add(Me.txStatus)
        Me.Controls.Add(Me.MainMenu1)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.SystemColors.Menu
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Location = New System.Drawing.Point(130, 121)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Argentum Online"
        Me.MainMenu1.ResumeLayout(False)
        Me.MainMenu1.PerformLayout()
        Me.Frame1.ResumeLayout(False)
        Me.Frame1.PerformLayout()
        CType(Me.Label1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ConnectionTimer As Timer
#End Region
End Class
