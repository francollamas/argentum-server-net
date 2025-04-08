<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class FrmInterv
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
	Public WithEvents Command2 As System.Windows.Forms.Button
	Public WithEvents Command1 As System.Windows.Forms.Button
	Public WithEvents txtAI As System.Windows.Forms.TextBox
	Public WithEvents txtNPCPuedeAtacar As System.Windows.Forms.TextBox
	Public WithEvents Label7 As System.Windows.Forms.Label
	Public WithEvents Label9 As System.Windows.Forms.Label
	Public WithEvents Frame4 As System.Windows.Forms.GroupBox
	Public WithEvents Frame11 As System.Windows.Forms.GroupBox
	Public WithEvents txtCmdExec As System.Windows.Forms.TextBox
	Public WithEvents txtIntervaloPerdidaStaminaLluvia As System.Windows.Forms.TextBox
	Public WithEvents txtIntervaloWAVFX As System.Windows.Forms.TextBox
	Public WithEvents txtIntervaloFrio As System.Windows.Forms.TextBox
	Public WithEvents Label20 As System.Windows.Forms.Label
	Public WithEvents Label19 As System.Windows.Forms.Label
	Public WithEvents Label13 As System.Windows.Forms.Label
	Public WithEvents Label12 As System.Windows.Forms.Label
	Public WithEvents Frame7 As System.Windows.Forms.GroupBox
	Public WithEvents Frame12 As System.Windows.Forms.GroupBox
	Public WithEvents txtIntervaloParaConexion As System.Windows.Forms.TextBox
	Public WithEvents txtTrabajo As System.Windows.Forms.TextBox
	Public WithEvents Label14 As System.Windows.Forms.Label
	Public WithEvents Label16 As System.Windows.Forms.Label
	Public WithEvents Frame9 As System.Windows.Forms.GroupBox
	Public WithEvents txtPuedeAtacar As System.Windows.Forms.TextBox
	Public WithEvents txtIntervaloLanzaHechizo As System.Windows.Forms.TextBox
	Public WithEvents Label17 As System.Windows.Forms.Label
	Public WithEvents Label15 As System.Windows.Forms.Label
	Public WithEvents Frame8 As System.Windows.Forms.GroupBox
	Public WithEvents txtIntervaloHambre As System.Windows.Forms.TextBox
	Public WithEvents txtIntervaloSed As System.Windows.Forms.TextBox
	Public WithEvents Label5 As System.Windows.Forms.Label
	Public WithEvents Label6 As System.Windows.Forms.Label
	Public WithEvents Frame3 As System.Windows.Forms.GroupBox
	Public WithEvents txtSanaIntervaloDescansar As System.Windows.Forms.TextBox
	Public WithEvents txtSanaIntervaloSinDescansar As System.Windows.Forms.TextBox
	Public WithEvents Label3 As System.Windows.Forms.Label
	Public WithEvents Label1 As System.Windows.Forms.Label
	Public WithEvents Frame1 As System.Windows.Forms.GroupBox
	Public WithEvents txtStaminaIntervaloSinDescansar As System.Windows.Forms.TextBox
	Public WithEvents txtStaminaIntervaloDescansar As System.Windows.Forms.TextBox
	Public WithEvents Label2 As System.Windows.Forms.Label
	Public WithEvents Label4 As System.Windows.Forms.Label
	Public WithEvents Frame2 As System.Windows.Forms.GroupBox
	Public WithEvents Frame6 As System.Windows.Forms.GroupBox
	Public WithEvents txtInvocacion As System.Windows.Forms.TextBox
	Public WithEvents txtIntervaloInvisible As System.Windows.Forms.TextBox
	Public WithEvents txtIntervaloParalizado As System.Windows.Forms.TextBox
	Public WithEvents txtIntervaloVeneno As System.Windows.Forms.TextBox
	Public WithEvents Label18 As System.Windows.Forms.Label
	Public WithEvents Label11 As System.Windows.Forms.Label
	Public WithEvents Label10 As System.Windows.Forms.Label
	Public WithEvents Label8 As System.Windows.Forms.Label
	Public WithEvents Frame10 As System.Windows.Forms.GroupBox
	Public WithEvents Frame5 As System.Windows.Forms.GroupBox
	Public WithEvents ok As System.Windows.Forms.Button
	'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
	'Se puede modificar mediante el Diseñador de Windows Forms.
	'No lo modifique con el editor de código.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(FrmInterv))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.Command2 = New System.Windows.Forms.Button
		Me.Command1 = New System.Windows.Forms.Button
		Me.Frame11 = New System.Windows.Forms.GroupBox
		Me.Frame4 = New System.Windows.Forms.GroupBox
		Me.txtAI = New System.Windows.Forms.TextBox
		Me.txtNPCPuedeAtacar = New System.Windows.Forms.TextBox
		Me.Label7 = New System.Windows.Forms.Label
		Me.Label9 = New System.Windows.Forms.Label
		Me.Frame12 = New System.Windows.Forms.GroupBox
		Me.Frame7 = New System.Windows.Forms.GroupBox
		Me.txtCmdExec = New System.Windows.Forms.TextBox
		Me.txtIntervaloPerdidaStaminaLluvia = New System.Windows.Forms.TextBox
		Me.txtIntervaloWAVFX = New System.Windows.Forms.TextBox
		Me.txtIntervaloFrio = New System.Windows.Forms.TextBox
		Me.Label20 = New System.Windows.Forms.Label
		Me.Label19 = New System.Windows.Forms.Label
		Me.Label13 = New System.Windows.Forms.Label
		Me.Label12 = New System.Windows.Forms.Label
		Me.Frame6 = New System.Windows.Forms.GroupBox
		Me.Frame9 = New System.Windows.Forms.GroupBox
		Me.txtIntervaloParaConexion = New System.Windows.Forms.TextBox
		Me.txtTrabajo = New System.Windows.Forms.TextBox
		Me.Label14 = New System.Windows.Forms.Label
		Me.Label16 = New System.Windows.Forms.Label
		Me.Frame8 = New System.Windows.Forms.GroupBox
		Me.txtPuedeAtacar = New System.Windows.Forms.TextBox
		Me.txtIntervaloLanzaHechizo = New System.Windows.Forms.TextBox
		Me.Label17 = New System.Windows.Forms.Label
		Me.Label15 = New System.Windows.Forms.Label
		Me.Frame3 = New System.Windows.Forms.GroupBox
		Me.txtIntervaloHambre = New System.Windows.Forms.TextBox
		Me.txtIntervaloSed = New System.Windows.Forms.TextBox
		Me.Label5 = New System.Windows.Forms.Label
		Me.Label6 = New System.Windows.Forms.Label
		Me.Frame1 = New System.Windows.Forms.GroupBox
		Me.txtSanaIntervaloDescansar = New System.Windows.Forms.TextBox
		Me.txtSanaIntervaloSinDescansar = New System.Windows.Forms.TextBox
		Me.Label3 = New System.Windows.Forms.Label
		Me.Label1 = New System.Windows.Forms.Label
		Me.Frame2 = New System.Windows.Forms.GroupBox
		Me.txtStaminaIntervaloSinDescansar = New System.Windows.Forms.TextBox
		Me.txtStaminaIntervaloDescansar = New System.Windows.Forms.TextBox
		Me.Label2 = New System.Windows.Forms.Label
		Me.Label4 = New System.Windows.Forms.Label
		Me.Frame5 = New System.Windows.Forms.GroupBox
		Me.Frame10 = New System.Windows.Forms.GroupBox
		Me.txtInvocacion = New System.Windows.Forms.TextBox
		Me.txtIntervaloInvisible = New System.Windows.Forms.TextBox
		Me.txtIntervaloParalizado = New System.Windows.Forms.TextBox
		Me.txtIntervaloVeneno = New System.Windows.Forms.TextBox
		Me.Label18 = New System.Windows.Forms.Label
		Me.Label11 = New System.Windows.Forms.Label
		Me.Label10 = New System.Windows.Forms.Label
		Me.Label8 = New System.Windows.Forms.Label
		Me.ok = New System.Windows.Forms.Button
		Me.Frame11.SuspendLayout()
		Me.Frame4.SuspendLayout()
		Me.Frame12.SuspendLayout()
		Me.Frame7.SuspendLayout()
		Me.Frame6.SuspendLayout()
		Me.Frame9.SuspendLayout()
		Me.Frame8.SuspendLayout()
		Me.Frame3.SuspendLayout()
		Me.Frame1.SuspendLayout()
		Me.Frame2.SuspendLayout()
		Me.Frame5.SuspendLayout()
		Me.Frame10.SuspendLayout()
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.Text = "Intervalos"
		Me.ClientSize = New System.Drawing.Size(510, 314)
		Me.Location = New System.Drawing.Point(4, 23)
		Me.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation
		Me.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.BackColor = System.Drawing.SystemColors.Control
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
		Me.ControlBox = True
		Me.Enabled = True
		Me.KeyPreview = False
		Me.MaximizeBox = True
		Me.MinimizeBox = True
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.ShowInTaskbar = True
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "FrmInterv"
		Me.Command2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command2.Text = "Guardar Intervalos"
		Me.Command2.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command2.Size = New System.Drawing.Size(217, 17)
		Me.Command2.Location = New System.Drawing.Point(280, 288)
		Me.Command2.TabIndex = 36
		Me.Command2.BackColor = System.Drawing.SystemColors.Control
		Me.Command2.CausesValidation = True
		Me.Command2.Enabled = True
		Me.Command2.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command2.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command2.TabStop = True
		Me.Command2.Name = "Command2"
		Me.Command1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command1.Text = "Aplicar"
		Me.Command1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command1.Size = New System.Drawing.Size(137, 17)
		Me.Command1.Location = New System.Drawing.Point(144, 288)
		Me.Command1.TabIndex = 0
		Me.Command1.BackColor = System.Drawing.SystemColors.Control
		Me.Command1.CausesValidation = True
		Me.Command1.Enabled = True
		Me.Command1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command1.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command1.TabStop = True
		Me.Command1.Name = "Command1"
		Me.Frame11.Text = "NPCs"
		Me.Frame11.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame11.Size = New System.Drawing.Size(113, 137)
		Me.Frame11.Location = New System.Drawing.Point(192, 144)
		Me.Frame11.TabIndex = 49
		Me.Frame11.BackColor = System.Drawing.SystemColors.Control
		Me.Frame11.Enabled = True
		Me.Frame11.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame11.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame11.Visible = True
		Me.Frame11.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame11.Name = "Frame11"
		Me.Frame4.Text = "A.I"
		Me.Frame4.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame4.Size = New System.Drawing.Size(91, 105)
		Me.Frame4.Location = New System.Drawing.Point(10, 16)
		Me.Frame4.TabIndex = 50
		Me.Frame4.BackColor = System.Drawing.SystemColors.Control
		Me.Frame4.Enabled = True
		Me.Frame4.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame4.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame4.Visible = True
		Me.Frame4.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame4.Name = "Frame4"
		Me.txtAI.AutoSize = False
		Me.txtAI.Size = New System.Drawing.Size(70, 19)
		Me.txtAI.Location = New System.Drawing.Point(10, 72)
		Me.txtAI.TabIndex = 52
		Me.txtAI.Text = "0"
		Me.txtAI.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtAI.AcceptsReturn = True
		Me.txtAI.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtAI.BackColor = System.Drawing.SystemColors.Window
		Me.txtAI.CausesValidation = True
		Me.txtAI.Enabled = True
		Me.txtAI.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtAI.HideSelection = True
		Me.txtAI.ReadOnly = False
		Me.txtAI.Maxlength = 0
		Me.txtAI.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtAI.MultiLine = False
		Me.txtAI.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtAI.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtAI.TabStop = True
		Me.txtAI.Visible = True
		Me.txtAI.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtAI.Name = "txtAI"
		Me.txtNPCPuedeAtacar.AutoSize = False
		Me.txtNPCPuedeAtacar.Size = New System.Drawing.Size(70, 19)
		Me.txtNPCPuedeAtacar.Location = New System.Drawing.Point(9, 34)
		Me.txtNPCPuedeAtacar.TabIndex = 51
		Me.txtNPCPuedeAtacar.Text = "0"
		Me.txtNPCPuedeAtacar.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtNPCPuedeAtacar.AcceptsReturn = True
		Me.txtNPCPuedeAtacar.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtNPCPuedeAtacar.BackColor = System.Drawing.SystemColors.Window
		Me.txtNPCPuedeAtacar.CausesValidation = True
		Me.txtNPCPuedeAtacar.Enabled = True
		Me.txtNPCPuedeAtacar.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtNPCPuedeAtacar.HideSelection = True
		Me.txtNPCPuedeAtacar.ReadOnly = False
		Me.txtNPCPuedeAtacar.Maxlength = 0
		Me.txtNPCPuedeAtacar.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtNPCPuedeAtacar.MultiLine = False
		Me.txtNPCPuedeAtacar.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtNPCPuedeAtacar.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtNPCPuedeAtacar.TabStop = True
		Me.txtNPCPuedeAtacar.Visible = True
		Me.txtNPCPuedeAtacar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtNPCPuedeAtacar.Name = "txtNPCPuedeAtacar"
		Me.Label7.Text = "AI"
		Me.Label7.Size = New System.Drawing.Size(10, 13)
		Me.Label7.Location = New System.Drawing.Point(11, 56)
		Me.Label7.TabIndex = 54
		Me.Label7.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label7.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label7.BackColor = System.Drawing.SystemColors.Control
		Me.Label7.Enabled = True
		Me.Label7.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label7.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label7.UseMnemonic = True
		Me.Label7.Visible = True
		Me.Label7.AutoSize = True
		Me.Label7.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label7.Name = "Label7"
		Me.Label9.Text = "Puede atacar"
		Me.Label9.Size = New System.Drawing.Size(64, 13)
		Me.Label9.Location = New System.Drawing.Point(10, 17)
		Me.Label9.TabIndex = 53
		Me.Label9.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label9.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label9.BackColor = System.Drawing.SystemColors.Control
		Me.Label9.Enabled = True
		Me.Label9.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label9.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label9.UseMnemonic = True
		Me.Label9.Visible = True
		Me.Label9.AutoSize = True
		Me.Label9.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label9.Name = "Label9"
		Me.Frame12.Text = "Clima && Ambiente"
		Me.Frame12.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame12.Size = New System.Drawing.Size(191, 137)
		Me.Frame12.Location = New System.Drawing.Point(312, 144)
		Me.Frame12.TabIndex = 39
		Me.Frame12.BackColor = System.Drawing.SystemColors.Control
		Me.Frame12.Enabled = True
		Me.Frame12.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame12.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame12.Visible = True
		Me.Frame12.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame12.Name = "Frame12"
		Me.Frame7.Text = "Frio y Fx Ambientales"
		Me.Frame7.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame7.Size = New System.Drawing.Size(175, 110)
		Me.Frame7.Location = New System.Drawing.Point(8, 16)
		Me.Frame7.TabIndex = 40
		Me.Frame7.BackColor = System.Drawing.SystemColors.Control
		Me.Frame7.Enabled = True
		Me.Frame7.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame7.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame7.Visible = True
		Me.Frame7.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame7.Name = "Frame7"
		Me.txtCmdExec.AutoSize = False
		Me.txtCmdExec.Size = New System.Drawing.Size(61, 19)
		Me.txtCmdExec.Location = New System.Drawing.Point(88, 74)
		Me.txtCmdExec.TabIndex = 44
		Me.txtCmdExec.Text = "0"
		Me.txtCmdExec.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtCmdExec.AcceptsReturn = True
		Me.txtCmdExec.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtCmdExec.BackColor = System.Drawing.SystemColors.Window
		Me.txtCmdExec.CausesValidation = True
		Me.txtCmdExec.Enabled = True
		Me.txtCmdExec.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtCmdExec.HideSelection = True
		Me.txtCmdExec.ReadOnly = False
		Me.txtCmdExec.Maxlength = 0
		Me.txtCmdExec.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtCmdExec.MultiLine = False
		Me.txtCmdExec.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtCmdExec.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtCmdExec.TabStop = True
		Me.txtCmdExec.Visible = True
		Me.txtCmdExec.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtCmdExec.Name = "txtCmdExec"
		Me.txtIntervaloPerdidaStaminaLluvia.AutoSize = False
		Me.txtIntervaloPerdidaStaminaLluvia.Size = New System.Drawing.Size(62, 20)
		Me.txtIntervaloPerdidaStaminaLluvia.Location = New System.Drawing.Point(88, 32)
		Me.txtIntervaloPerdidaStaminaLluvia.TabIndex = 43
		Me.txtIntervaloPerdidaStaminaLluvia.Text = "0"
		Me.txtIntervaloPerdidaStaminaLluvia.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloPerdidaStaminaLluvia.AcceptsReturn = True
		Me.txtIntervaloPerdidaStaminaLluvia.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloPerdidaStaminaLluvia.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloPerdidaStaminaLluvia.CausesValidation = True
		Me.txtIntervaloPerdidaStaminaLluvia.Enabled = True
		Me.txtIntervaloPerdidaStaminaLluvia.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloPerdidaStaminaLluvia.HideSelection = True
		Me.txtIntervaloPerdidaStaminaLluvia.ReadOnly = False
		Me.txtIntervaloPerdidaStaminaLluvia.Maxlength = 0
		Me.txtIntervaloPerdidaStaminaLluvia.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloPerdidaStaminaLluvia.MultiLine = False
		Me.txtIntervaloPerdidaStaminaLluvia.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloPerdidaStaminaLluvia.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloPerdidaStaminaLluvia.TabStop = True
		Me.txtIntervaloPerdidaStaminaLluvia.Visible = True
		Me.txtIntervaloPerdidaStaminaLluvia.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloPerdidaStaminaLluvia.Name = "txtIntervaloPerdidaStaminaLluvia"
		Me.txtIntervaloWAVFX.AutoSize = False
		Me.txtIntervaloWAVFX.Size = New System.Drawing.Size(62, 20)
		Me.txtIntervaloWAVFX.Location = New System.Drawing.Point(10, 32)
		Me.txtIntervaloWAVFX.TabIndex = 42
		Me.txtIntervaloWAVFX.Text = "0"
		Me.txtIntervaloWAVFX.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloWAVFX.AcceptsReturn = True
		Me.txtIntervaloWAVFX.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloWAVFX.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloWAVFX.CausesValidation = True
		Me.txtIntervaloWAVFX.Enabled = True
		Me.txtIntervaloWAVFX.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloWAVFX.HideSelection = True
		Me.txtIntervaloWAVFX.ReadOnly = False
		Me.txtIntervaloWAVFX.Maxlength = 0
		Me.txtIntervaloWAVFX.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloWAVFX.MultiLine = False
		Me.txtIntervaloWAVFX.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloWAVFX.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloWAVFX.TabStop = True
		Me.txtIntervaloWAVFX.Visible = True
		Me.txtIntervaloWAVFX.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloWAVFX.Name = "txtIntervaloWAVFX"
		Me.txtIntervaloFrio.AutoSize = False
		Me.txtIntervaloFrio.Size = New System.Drawing.Size(61, 19)
		Me.txtIntervaloFrio.Location = New System.Drawing.Point(12, 72)
		Me.txtIntervaloFrio.TabIndex = 41
		Me.txtIntervaloFrio.Text = "0"
		Me.txtIntervaloFrio.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloFrio.AcceptsReturn = True
		Me.txtIntervaloFrio.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloFrio.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloFrio.CausesValidation = True
		Me.txtIntervaloFrio.Enabled = True
		Me.txtIntervaloFrio.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloFrio.HideSelection = True
		Me.txtIntervaloFrio.ReadOnly = False
		Me.txtIntervaloFrio.Maxlength = 0
		Me.txtIntervaloFrio.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloFrio.MultiLine = False
		Me.txtIntervaloFrio.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloFrio.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloFrio.TabStop = True
		Me.txtIntervaloFrio.Visible = True
		Me.txtIntervaloFrio.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloFrio.Name = "txtIntervaloFrio"
		Me.Label20.Text = "TimerExec"
		Me.Label20.Size = New System.Drawing.Size(50, 13)
		Me.Label20.Location = New System.Drawing.Point(88, 56)
		Me.Label20.TabIndex = 48
		Me.Label20.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label20.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label20.BackColor = System.Drawing.SystemColors.Control
		Me.Label20.Enabled = True
		Me.Label20.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label20.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label20.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label20.UseMnemonic = True
		Me.Label20.Visible = True
		Me.Label20.AutoSize = True
		Me.Label20.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label20.Name = "Label20"
		Me.Label19.Text = "Stamina Lluvia"
		Me.Label19.Size = New System.Drawing.Size(69, 13)
		Me.Label19.Location = New System.Drawing.Point(90, 18)
		Me.Label19.TabIndex = 47
		Me.Label19.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label19.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label19.BackColor = System.Drawing.SystemColors.Control
		Me.Label19.Enabled = True
		Me.Label19.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label19.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label19.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label19.UseMnemonic = True
		Me.Label19.Visible = True
		Me.Label19.AutoSize = True
		Me.Label19.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label19.Name = "Label19"
		Me.Label13.Text = "FxS"
		Me.Label13.Size = New System.Drawing.Size(18, 13)
		Me.Label13.Location = New System.Drawing.Point(12, 18)
		Me.Label13.TabIndex = 46
		Me.Label13.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label13.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label13.BackColor = System.Drawing.SystemColors.Control
		Me.Label13.Enabled = True
		Me.Label13.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label13.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label13.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label13.UseMnemonic = True
		Me.Label13.Visible = True
		Me.Label13.AutoSize = True
		Me.Label13.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label13.Name = "Label13"
		Me.Label12.Text = "Frio"
		Me.Label12.Size = New System.Drawing.Size(17, 13)
		Me.Label12.Location = New System.Drawing.Point(13, 54)
		Me.Label12.TabIndex = 45
		Me.Label12.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label12.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label12.BackColor = System.Drawing.SystemColors.Control
		Me.Label12.Enabled = True
		Me.Label12.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label12.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label12.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label12.UseMnemonic = True
		Me.Label12.Visible = True
		Me.Label12.AutoSize = True
		Me.Label12.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label12.Name = "Label12"
		Me.Frame6.Text = "Usuarios"
		Me.Frame6.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame6.Size = New System.Drawing.Size(497, 137)
		Me.Frame6.Location = New System.Drawing.Point(8, 0)
		Me.Frame6.TabIndex = 3
		Me.Frame6.BackColor = System.Drawing.SystemColors.Control
		Me.Frame6.Enabled = True
		Me.Frame6.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame6.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame6.Visible = True
		Me.Frame6.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame6.Name = "Frame6"
		Me.Frame9.Text = "Otros"
		Me.Frame9.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame9.Size = New System.Drawing.Size(94, 114)
		Me.Frame9.Location = New System.Drawing.Point(6, 14)
		Me.Frame9.TabIndex = 24
		Me.Frame9.BackColor = System.Drawing.SystemColors.Control
		Me.Frame9.Enabled = True
		Me.Frame9.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame9.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame9.Visible = True
		Me.Frame9.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame9.Name = "Frame9"
		Me.txtIntervaloParaConexion.AutoSize = False
		Me.txtIntervaloParaConexion.Size = New System.Drawing.Size(62, 20)
		Me.txtIntervaloParaConexion.Location = New System.Drawing.Point(3, 33)
		Me.txtIntervaloParaConexion.TabIndex = 26
		Me.txtIntervaloParaConexion.Text = "0"
		Me.txtIntervaloParaConexion.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloParaConexion.AcceptsReturn = True
		Me.txtIntervaloParaConexion.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloParaConexion.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloParaConexion.CausesValidation = True
		Me.txtIntervaloParaConexion.Enabled = True
		Me.txtIntervaloParaConexion.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloParaConexion.HideSelection = True
		Me.txtIntervaloParaConexion.ReadOnly = False
		Me.txtIntervaloParaConexion.Maxlength = 0
		Me.txtIntervaloParaConexion.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloParaConexion.MultiLine = False
		Me.txtIntervaloParaConexion.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloParaConexion.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloParaConexion.TabStop = True
		Me.txtIntervaloParaConexion.Visible = True
		Me.txtIntervaloParaConexion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloParaConexion.Name = "txtIntervaloParaConexion"
		Me.txtTrabajo.AutoSize = False
		Me.txtTrabajo.Size = New System.Drawing.Size(62, 20)
		Me.txtTrabajo.Location = New System.Drawing.Point(4, 68)
		Me.txtTrabajo.TabIndex = 25
		Me.txtTrabajo.Text = "0"
		Me.txtTrabajo.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtTrabajo.AcceptsReturn = True
		Me.txtTrabajo.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtTrabajo.BackColor = System.Drawing.SystemColors.Window
		Me.txtTrabajo.CausesValidation = True
		Me.txtTrabajo.Enabled = True
		Me.txtTrabajo.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtTrabajo.HideSelection = True
		Me.txtTrabajo.ReadOnly = False
		Me.txtTrabajo.Maxlength = 0
		Me.txtTrabajo.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtTrabajo.MultiLine = False
		Me.txtTrabajo.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtTrabajo.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtTrabajo.TabStop = True
		Me.txtTrabajo.Visible = True
		Me.txtTrabajo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtTrabajo.Name = "txtTrabajo"
		Me.Label14.Text = "IntervaloCon"
		Me.Label14.Size = New System.Drawing.Size(60, 13)
		Me.Label14.Location = New System.Drawing.Point(8, 18)
		Me.Label14.TabIndex = 28
		Me.Label14.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label14.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label14.BackColor = System.Drawing.SystemColors.Control
		Me.Label14.Enabled = True
		Me.Label14.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label14.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label14.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label14.UseMnemonic = True
		Me.Label14.Visible = True
		Me.Label14.AutoSize = True
		Me.Label14.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label14.Name = "Label14"
		Me.Label16.Text = "Trabajo"
		Me.Label16.Size = New System.Drawing.Size(36, 13)
		Me.Label16.Location = New System.Drawing.Point(11, 52)
		Me.Label16.TabIndex = 27
		Me.Label16.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label16.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label16.BackColor = System.Drawing.SystemColors.Control
		Me.Label16.Enabled = True
		Me.Label16.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label16.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label16.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label16.UseMnemonic = True
		Me.Label16.Visible = True
		Me.Label16.AutoSize = True
		Me.Label16.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label16.Name = "Label16"
		Me.Frame8.Text = "Combate"
		Me.Frame8.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame8.Size = New System.Drawing.Size(94, 114)
		Me.Frame8.Location = New System.Drawing.Point(103, 14)
		Me.Frame8.TabIndex = 19
		Me.Frame8.BackColor = System.Drawing.SystemColors.Control
		Me.Frame8.Enabled = True
		Me.Frame8.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame8.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame8.Visible = True
		Me.Frame8.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame8.Name = "Frame8"
		Me.txtPuedeAtacar.AutoSize = False
		Me.txtPuedeAtacar.Size = New System.Drawing.Size(62, 20)
		Me.txtPuedeAtacar.Location = New System.Drawing.Point(9, 80)
		Me.txtPuedeAtacar.TabIndex = 22
		Me.txtPuedeAtacar.Text = "0"
		Me.txtPuedeAtacar.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtPuedeAtacar.AcceptsReturn = True
		Me.txtPuedeAtacar.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtPuedeAtacar.BackColor = System.Drawing.SystemColors.Window
		Me.txtPuedeAtacar.CausesValidation = True
		Me.txtPuedeAtacar.Enabled = True
		Me.txtPuedeAtacar.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtPuedeAtacar.HideSelection = True
		Me.txtPuedeAtacar.ReadOnly = False
		Me.txtPuedeAtacar.Maxlength = 0
		Me.txtPuedeAtacar.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtPuedeAtacar.MultiLine = False
		Me.txtPuedeAtacar.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtPuedeAtacar.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtPuedeAtacar.TabStop = True
		Me.txtPuedeAtacar.Visible = True
		Me.txtPuedeAtacar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtPuedeAtacar.Name = "txtPuedeAtacar"
		Me.txtIntervaloLanzaHechizo.AutoSize = False
		Me.txtIntervaloLanzaHechizo.Size = New System.Drawing.Size(62, 20)
		Me.txtIntervaloLanzaHechizo.Location = New System.Drawing.Point(10, 35)
		Me.txtIntervaloLanzaHechizo.TabIndex = 20
		Me.txtIntervaloLanzaHechizo.Text = "0"
		Me.txtIntervaloLanzaHechizo.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloLanzaHechizo.AcceptsReturn = True
		Me.txtIntervaloLanzaHechizo.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloLanzaHechizo.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloLanzaHechizo.CausesValidation = True
		Me.txtIntervaloLanzaHechizo.Enabled = True
		Me.txtIntervaloLanzaHechizo.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloLanzaHechizo.HideSelection = True
		Me.txtIntervaloLanzaHechizo.ReadOnly = False
		Me.txtIntervaloLanzaHechizo.Maxlength = 0
		Me.txtIntervaloLanzaHechizo.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloLanzaHechizo.MultiLine = False
		Me.txtIntervaloLanzaHechizo.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloLanzaHechizo.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloLanzaHechizo.TabStop = True
		Me.txtIntervaloLanzaHechizo.Visible = True
		Me.txtIntervaloLanzaHechizo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloLanzaHechizo.Name = "txtIntervaloLanzaHechizo"
		Me.Label17.Text = "Puede Atacar"
		Me.Label17.Size = New System.Drawing.Size(65, 13)
		Me.Label17.Location = New System.Drawing.Point(9, 62)
		Me.Label17.TabIndex = 23
		Me.Label17.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label17.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label17.BackColor = System.Drawing.SystemColors.Control
		Me.Label17.Enabled = True
		Me.Label17.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label17.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label17.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label17.UseMnemonic = True
		Me.Label17.Visible = True
		Me.Label17.AutoSize = True
		Me.Label17.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label17.Name = "Label17"
		Me.Label15.Text = "Lanza Spell"
		Me.Label15.Size = New System.Drawing.Size(55, 13)
		Me.Label15.Location = New System.Drawing.Point(10, 19)
		Me.Label15.TabIndex = 21
		Me.Label15.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label15.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label15.BackColor = System.Drawing.SystemColors.Control
		Me.Label15.Enabled = True
		Me.Label15.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label15.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label15.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label15.UseMnemonic = True
		Me.Label15.Visible = True
		Me.Label15.AutoSize = True
		Me.Label15.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label15.Name = "Label15"
		Me.Frame3.Text = "Hambre y sed"
		Me.Frame3.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame3.Size = New System.Drawing.Size(94, 114)
		Me.Frame3.Location = New System.Drawing.Point(395, 14)
		Me.Frame3.TabIndex = 14
		Me.Frame3.BackColor = System.Drawing.SystemColors.Control
		Me.Frame3.Enabled = True
		Me.Frame3.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame3.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame3.Visible = True
		Me.Frame3.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame3.Name = "Frame3"
		Me.txtIntervaloHambre.AutoSize = False
		Me.txtIntervaloHambre.Size = New System.Drawing.Size(70, 19)
		Me.txtIntervaloHambre.Location = New System.Drawing.Point(10, 34)
		Me.txtIntervaloHambre.TabIndex = 16
		Me.txtIntervaloHambre.Text = "0"
		Me.txtIntervaloHambre.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloHambre.AcceptsReturn = True
		Me.txtIntervaloHambre.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloHambre.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloHambre.CausesValidation = True
		Me.txtIntervaloHambre.Enabled = True
		Me.txtIntervaloHambre.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloHambre.HideSelection = True
		Me.txtIntervaloHambre.ReadOnly = False
		Me.txtIntervaloHambre.Maxlength = 0
		Me.txtIntervaloHambre.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloHambre.MultiLine = False
		Me.txtIntervaloHambre.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloHambre.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloHambre.TabStop = True
		Me.txtIntervaloHambre.Visible = True
		Me.txtIntervaloHambre.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloHambre.Name = "txtIntervaloHambre"
		Me.txtIntervaloSed.AutoSize = False
		Me.txtIntervaloSed.Size = New System.Drawing.Size(70, 19)
		Me.txtIntervaloSed.Location = New System.Drawing.Point(10, 79)
		Me.txtIntervaloSed.TabIndex = 15
		Me.txtIntervaloSed.Text = "0"
		Me.txtIntervaloSed.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloSed.AcceptsReturn = True
		Me.txtIntervaloSed.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloSed.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloSed.CausesValidation = True
		Me.txtIntervaloSed.Enabled = True
		Me.txtIntervaloSed.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloSed.HideSelection = True
		Me.txtIntervaloSed.ReadOnly = False
		Me.txtIntervaloSed.Maxlength = 0
		Me.txtIntervaloSed.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloSed.MultiLine = False
		Me.txtIntervaloSed.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloSed.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloSed.TabStop = True
		Me.txtIntervaloSed.Visible = True
		Me.txtIntervaloSed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloSed.Name = "txtIntervaloSed"
		Me.Label5.Text = "Hambre"
		Me.Label5.Size = New System.Drawing.Size(37, 13)
		Me.Label5.Location = New System.Drawing.Point(12, 17)
		Me.Label5.TabIndex = 18
		Me.Label5.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label5.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label5.BackColor = System.Drawing.SystemColors.Control
		Me.Label5.Enabled = True
		Me.Label5.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label5.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label5.UseMnemonic = True
		Me.Label5.Visible = True
		Me.Label5.AutoSize = True
		Me.Label5.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label5.Name = "Label5"
		Me.Label6.Text = "Sed"
		Me.Label6.Size = New System.Drawing.Size(19, 13)
		Me.Label6.Location = New System.Drawing.Point(11, 62)
		Me.Label6.TabIndex = 17
		Me.Label6.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label6.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label6.BackColor = System.Drawing.SystemColors.Control
		Me.Label6.Enabled = True
		Me.Label6.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label6.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label6.UseMnemonic = True
		Me.Label6.Visible = True
		Me.Label6.AutoSize = True
		Me.Label6.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label6.Name = "Label6"
		Me.Frame1.Text = "Sanar"
		Me.Frame1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame1.Size = New System.Drawing.Size(94, 114)
		Me.Frame1.Location = New System.Drawing.Point(298, 14)
		Me.Frame1.TabIndex = 9
		Me.Frame1.BackColor = System.Drawing.SystemColors.Control
		Me.Frame1.Enabled = True
		Me.Frame1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame1.Visible = True
		Me.Frame1.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame1.Name = "Frame1"
		Me.txtSanaIntervaloDescansar.AutoSize = False
		Me.txtSanaIntervaloDescansar.Size = New System.Drawing.Size(70, 19)
		Me.txtSanaIntervaloDescansar.Location = New System.Drawing.Point(10, 34)
		Me.txtSanaIntervaloDescansar.TabIndex = 11
		Me.txtSanaIntervaloDescansar.Text = "0"
		Me.txtSanaIntervaloDescansar.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtSanaIntervaloDescansar.AcceptsReturn = True
		Me.txtSanaIntervaloDescansar.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtSanaIntervaloDescansar.BackColor = System.Drawing.SystemColors.Window
		Me.txtSanaIntervaloDescansar.CausesValidation = True
		Me.txtSanaIntervaloDescansar.Enabled = True
		Me.txtSanaIntervaloDescansar.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtSanaIntervaloDescansar.HideSelection = True
		Me.txtSanaIntervaloDescansar.ReadOnly = False
		Me.txtSanaIntervaloDescansar.Maxlength = 0
		Me.txtSanaIntervaloDescansar.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtSanaIntervaloDescansar.MultiLine = False
		Me.txtSanaIntervaloDescansar.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtSanaIntervaloDescansar.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtSanaIntervaloDescansar.TabStop = True
		Me.txtSanaIntervaloDescansar.Visible = True
		Me.txtSanaIntervaloDescansar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtSanaIntervaloDescansar.Name = "txtSanaIntervaloDescansar"
		Me.txtSanaIntervaloSinDescansar.AutoSize = False
		Me.txtSanaIntervaloSinDescansar.Size = New System.Drawing.Size(70, 19)
		Me.txtSanaIntervaloSinDescansar.Location = New System.Drawing.Point(10, 79)
		Me.txtSanaIntervaloSinDescansar.TabIndex = 10
		Me.txtSanaIntervaloSinDescansar.Text = "0"
		Me.txtSanaIntervaloSinDescansar.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtSanaIntervaloSinDescansar.AcceptsReturn = True
		Me.txtSanaIntervaloSinDescansar.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtSanaIntervaloSinDescansar.BackColor = System.Drawing.SystemColors.Window
		Me.txtSanaIntervaloSinDescansar.CausesValidation = True
		Me.txtSanaIntervaloSinDescansar.Enabled = True
		Me.txtSanaIntervaloSinDescansar.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtSanaIntervaloSinDescansar.HideSelection = True
		Me.txtSanaIntervaloSinDescansar.ReadOnly = False
		Me.txtSanaIntervaloSinDescansar.Maxlength = 0
		Me.txtSanaIntervaloSinDescansar.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtSanaIntervaloSinDescansar.MultiLine = False
		Me.txtSanaIntervaloSinDescansar.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtSanaIntervaloSinDescansar.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtSanaIntervaloSinDescansar.TabStop = True
		Me.txtSanaIntervaloSinDescansar.Visible = True
		Me.txtSanaIntervaloSinDescansar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtSanaIntervaloSinDescansar.Name = "txtSanaIntervaloSinDescansar"
		Me.Label3.Text = "Descansando"
		Me.Label3.Size = New System.Drawing.Size(66, 13)
		Me.Label3.Location = New System.Drawing.Point(12, 17)
		Me.Label3.TabIndex = 13
		Me.Label3.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label3.BackColor = System.Drawing.SystemColors.Control
		Me.Label3.Enabled = True
		Me.Label3.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label3.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label3.UseMnemonic = True
		Me.Label3.Visible = True
		Me.Label3.AutoSize = True
		Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label3.Name = "Label3"
		Me.Label1.Text = "Sin descansar"
		Me.Label1.Size = New System.Drawing.Size(67, 13)
		Me.Label1.Location = New System.Drawing.Point(11, 62)
		Me.Label1.TabIndex = 12
		Me.Label1.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label1.BackColor = System.Drawing.SystemColors.Control
		Me.Label1.Enabled = True
		Me.Label1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label1.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label1.UseMnemonic = True
		Me.Label1.Visible = True
		Me.Label1.AutoSize = True
		Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label1.Name = "Label1"
		Me.Frame2.Text = "Stamina"
		Me.Frame2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame2.Size = New System.Drawing.Size(94, 114)
		Me.Frame2.Location = New System.Drawing.Point(201, 14)
		Me.Frame2.TabIndex = 4
		Me.Frame2.BackColor = System.Drawing.SystemColors.Control
		Me.Frame2.Enabled = True
		Me.Frame2.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame2.Visible = True
		Me.Frame2.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame2.Name = "Frame2"
		Me.txtStaminaIntervaloSinDescansar.AutoSize = False
		Me.txtStaminaIntervaloSinDescansar.Size = New System.Drawing.Size(70, 19)
		Me.txtStaminaIntervaloSinDescansar.Location = New System.Drawing.Point(10, 79)
		Me.txtStaminaIntervaloSinDescansar.TabIndex = 6
		Me.txtStaminaIntervaloSinDescansar.Text = "0"
		Me.txtStaminaIntervaloSinDescansar.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtStaminaIntervaloSinDescansar.AcceptsReturn = True
		Me.txtStaminaIntervaloSinDescansar.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtStaminaIntervaloSinDescansar.BackColor = System.Drawing.SystemColors.Window
		Me.txtStaminaIntervaloSinDescansar.CausesValidation = True
		Me.txtStaminaIntervaloSinDescansar.Enabled = True
		Me.txtStaminaIntervaloSinDescansar.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtStaminaIntervaloSinDescansar.HideSelection = True
		Me.txtStaminaIntervaloSinDescansar.ReadOnly = False
		Me.txtStaminaIntervaloSinDescansar.Maxlength = 0
		Me.txtStaminaIntervaloSinDescansar.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtStaminaIntervaloSinDescansar.MultiLine = False
		Me.txtStaminaIntervaloSinDescansar.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtStaminaIntervaloSinDescansar.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtStaminaIntervaloSinDescansar.TabStop = True
		Me.txtStaminaIntervaloSinDescansar.Visible = True
		Me.txtStaminaIntervaloSinDescansar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtStaminaIntervaloSinDescansar.Name = "txtStaminaIntervaloSinDescansar"
		Me.txtStaminaIntervaloDescansar.AutoSize = False
		Me.txtStaminaIntervaloDescansar.Size = New System.Drawing.Size(70, 19)
		Me.txtStaminaIntervaloDescansar.Location = New System.Drawing.Point(11, 34)
		Me.txtStaminaIntervaloDescansar.TabIndex = 5
		Me.txtStaminaIntervaloDescansar.Text = "0"
		Me.txtStaminaIntervaloDescansar.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtStaminaIntervaloDescansar.AcceptsReturn = True
		Me.txtStaminaIntervaloDescansar.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtStaminaIntervaloDescansar.BackColor = System.Drawing.SystemColors.Window
		Me.txtStaminaIntervaloDescansar.CausesValidation = True
		Me.txtStaminaIntervaloDescansar.Enabled = True
		Me.txtStaminaIntervaloDescansar.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtStaminaIntervaloDescansar.HideSelection = True
		Me.txtStaminaIntervaloDescansar.ReadOnly = False
		Me.txtStaminaIntervaloDescansar.Maxlength = 0
		Me.txtStaminaIntervaloDescansar.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtStaminaIntervaloDescansar.MultiLine = False
		Me.txtStaminaIntervaloDescansar.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtStaminaIntervaloDescansar.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtStaminaIntervaloDescansar.TabStop = True
		Me.txtStaminaIntervaloDescansar.Visible = True
		Me.txtStaminaIntervaloDescansar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtStaminaIntervaloDescansar.Name = "txtStaminaIntervaloDescansar"
		Me.Label2.Text = "Sin descansar"
		Me.Label2.Size = New System.Drawing.Size(67, 13)
		Me.Label2.Location = New System.Drawing.Point(11, 62)
		Me.Label2.TabIndex = 8
		Me.Label2.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label2.BackColor = System.Drawing.SystemColors.Control
		Me.Label2.Enabled = True
		Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label2.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label2.UseMnemonic = True
		Me.Label2.Visible = True
		Me.Label2.AutoSize = True
		Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label2.Name = "Label2"
		Me.Label4.Text = "Descansando"
		Me.Label4.Size = New System.Drawing.Size(66, 13)
		Me.Label4.Location = New System.Drawing.Point(12, 17)
		Me.Label4.TabIndex = 7
		Me.Label4.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label4.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label4.BackColor = System.Drawing.SystemColors.Control
		Me.Label4.Enabled = True
		Me.Label4.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label4.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label4.UseMnemonic = True
		Me.Label4.Visible = True
		Me.Label4.AutoSize = True
		Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label4.Name = "Label4"
		Me.Frame5.Text = "Magia"
		Me.Frame5.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame5.Size = New System.Drawing.Size(177, 137)
		Me.Frame5.Location = New System.Drawing.Point(8, 144)
		Me.Frame5.TabIndex = 2
		Me.Frame5.BackColor = System.Drawing.SystemColors.Control
		Me.Frame5.Enabled = True
		Me.Frame5.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame5.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame5.Visible = True
		Me.Frame5.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame5.Name = "Frame5"
		Me.Frame10.Text = "Duracion Spells"
		Me.Frame10.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Frame10.Size = New System.Drawing.Size(160, 110)
		Me.Frame10.Location = New System.Drawing.Point(9, 18)
		Me.Frame10.TabIndex = 29
		Me.Frame10.BackColor = System.Drawing.SystemColors.Control
		Me.Frame10.Enabled = True
		Me.Frame10.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame10.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame10.Visible = True
		Me.Frame10.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame10.Name = "Frame10"
		Me.txtInvocacion.AutoSize = False
		Me.txtInvocacion.Size = New System.Drawing.Size(60, 20)
		Me.txtInvocacion.Location = New System.Drawing.Point(78, 78)
		Me.txtInvocacion.TabIndex = 37
		Me.txtInvocacion.Text = "0"
		Me.txtInvocacion.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtInvocacion.AcceptsReturn = True
		Me.txtInvocacion.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtInvocacion.BackColor = System.Drawing.SystemColors.Window
		Me.txtInvocacion.CausesValidation = True
		Me.txtInvocacion.Enabled = True
		Me.txtInvocacion.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtInvocacion.HideSelection = True
		Me.txtInvocacion.ReadOnly = False
		Me.txtInvocacion.Maxlength = 0
		Me.txtInvocacion.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtInvocacion.MultiLine = False
		Me.txtInvocacion.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtInvocacion.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtInvocacion.TabStop = True
		Me.txtInvocacion.Visible = True
		Me.txtInvocacion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtInvocacion.Name = "txtInvocacion"
		Me.txtIntervaloInvisible.AutoSize = False
		Me.txtIntervaloInvisible.Size = New System.Drawing.Size(60, 20)
		Me.txtIntervaloInvisible.Location = New System.Drawing.Point(78, 33)
		Me.txtIntervaloInvisible.TabIndex = 34
		Me.txtIntervaloInvisible.Text = "0"
		Me.txtIntervaloInvisible.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloInvisible.AcceptsReturn = True
		Me.txtIntervaloInvisible.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloInvisible.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloInvisible.CausesValidation = True
		Me.txtIntervaloInvisible.Enabled = True
		Me.txtIntervaloInvisible.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloInvisible.HideSelection = True
		Me.txtIntervaloInvisible.ReadOnly = False
		Me.txtIntervaloInvisible.Maxlength = 0
		Me.txtIntervaloInvisible.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloInvisible.MultiLine = False
		Me.txtIntervaloInvisible.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloInvisible.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloInvisible.TabStop = True
		Me.txtIntervaloInvisible.Visible = True
		Me.txtIntervaloInvisible.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloInvisible.Name = "txtIntervaloInvisible"
		Me.txtIntervaloParalizado.AutoSize = False
		Me.txtIntervaloParalizado.Size = New System.Drawing.Size(53, 20)
		Me.txtIntervaloParalizado.Location = New System.Drawing.Point(13, 78)
		Me.txtIntervaloParalizado.TabIndex = 31
		Me.txtIntervaloParalizado.Text = "0"
		Me.txtIntervaloParalizado.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloParalizado.AcceptsReturn = True
		Me.txtIntervaloParalizado.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloParalizado.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloParalizado.CausesValidation = True
		Me.txtIntervaloParalizado.Enabled = True
		Me.txtIntervaloParalizado.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloParalizado.HideSelection = True
		Me.txtIntervaloParalizado.ReadOnly = False
		Me.txtIntervaloParalizado.Maxlength = 0
		Me.txtIntervaloParalizado.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloParalizado.MultiLine = False
		Me.txtIntervaloParalizado.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloParalizado.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloParalizado.TabStop = True
		Me.txtIntervaloParalizado.Visible = True
		Me.txtIntervaloParalizado.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloParalizado.Name = "txtIntervaloParalizado"
		Me.txtIntervaloVeneno.AutoSize = False
		Me.txtIntervaloVeneno.Size = New System.Drawing.Size(53, 20)
		Me.txtIntervaloVeneno.Location = New System.Drawing.Point(13, 34)
		Me.txtIntervaloVeneno.TabIndex = 30
		Me.txtIntervaloVeneno.Text = "0"
		Me.txtIntervaloVeneno.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtIntervaloVeneno.AcceptsReturn = True
		Me.txtIntervaloVeneno.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtIntervaloVeneno.BackColor = System.Drawing.SystemColors.Window
		Me.txtIntervaloVeneno.CausesValidation = True
		Me.txtIntervaloVeneno.Enabled = True
		Me.txtIntervaloVeneno.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtIntervaloVeneno.HideSelection = True
		Me.txtIntervaloVeneno.ReadOnly = False
		Me.txtIntervaloVeneno.Maxlength = 0
		Me.txtIntervaloVeneno.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtIntervaloVeneno.MultiLine = False
		Me.txtIntervaloVeneno.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtIntervaloVeneno.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtIntervaloVeneno.TabStop = True
		Me.txtIntervaloVeneno.Visible = True
		Me.txtIntervaloVeneno.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtIntervaloVeneno.Name = "txtIntervaloVeneno"
		Me.Label18.Text = "Invocacion"
		Me.Label18.Size = New System.Drawing.Size(53, 13)
		Me.Label18.Location = New System.Drawing.Point(78, 64)
		Me.Label18.TabIndex = 38
		Me.Label18.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label18.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label18.BackColor = System.Drawing.SystemColors.Control
		Me.Label18.Enabled = True
		Me.Label18.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label18.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label18.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label18.UseMnemonic = True
		Me.Label18.Visible = True
		Me.Label18.AutoSize = True
		Me.Label18.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label18.Name = "Label18"
		Me.Label11.Text = "Invisible"
		Me.Label11.Size = New System.Drawing.Size(38, 13)
		Me.Label11.Location = New System.Drawing.Point(78, 19)
		Me.Label11.TabIndex = 35
		Me.Label11.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label11.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label11.BackColor = System.Drawing.SystemColors.Control
		Me.Label11.Enabled = True
		Me.Label11.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label11.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label11.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label11.UseMnemonic = True
		Me.Label11.Visible = True
		Me.Label11.AutoSize = True
		Me.Label11.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label11.Name = "Label11"
		Me.Label10.Text = "Paralizado"
		Me.Label10.Size = New System.Drawing.Size(49, 13)
		Me.Label10.Location = New System.Drawing.Point(15, 64)
		Me.Label10.TabIndex = 33
		Me.Label10.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label10.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label10.BackColor = System.Drawing.SystemColors.Control
		Me.Label10.Enabled = True
		Me.Label10.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label10.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label10.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label10.UseMnemonic = True
		Me.Label10.Visible = True
		Me.Label10.AutoSize = True
		Me.Label10.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label10.Name = "Label10"
		Me.Label8.Text = "Veneno"
		Me.Label8.Size = New System.Drawing.Size(37, 12)
		Me.Label8.Location = New System.Drawing.Point(15, 20)
		Me.Label8.TabIndex = 32
		Me.Label8.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label8.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label8.BackColor = System.Drawing.SystemColors.Control
		Me.Label8.Enabled = True
		Me.Label8.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Label8.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label8.UseMnemonic = True
		Me.Label8.Visible = True
		Me.Label8.AutoSize = True
		Me.Label8.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label8.Name = "Label8"
		Me.ok.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.ok.Text = "OK"
		Me.ok.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.ok.Size = New System.Drawing.Size(137, 17)
		Me.ok.Location = New System.Drawing.Point(8, 288)
		Me.ok.TabIndex = 1
		Me.ok.BackColor = System.Drawing.SystemColors.Control
		Me.ok.CausesValidation = True
		Me.ok.Enabled = True
		Me.ok.ForeColor = System.Drawing.SystemColors.ControlText
		Me.ok.Cursor = System.Windows.Forms.Cursors.Default
		Me.ok.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.ok.TabStop = True
		Me.ok.Name = "ok"
		Me.Controls.Add(Command2)
		Me.Controls.Add(Command1)
		Me.Controls.Add(Frame11)
		Me.Controls.Add(Frame12)
		Me.Controls.Add(Frame6)
		Me.Controls.Add(Frame5)
		Me.Controls.Add(ok)
		Me.Frame11.Controls.Add(Frame4)
		Me.Frame4.Controls.Add(txtAI)
		Me.Frame4.Controls.Add(txtNPCPuedeAtacar)
		Me.Frame4.Controls.Add(Label7)
		Me.Frame4.Controls.Add(Label9)
		Me.Frame12.Controls.Add(Frame7)
		Me.Frame7.Controls.Add(txtCmdExec)
		Me.Frame7.Controls.Add(txtIntervaloPerdidaStaminaLluvia)
		Me.Frame7.Controls.Add(txtIntervaloWAVFX)
		Me.Frame7.Controls.Add(txtIntervaloFrio)
		Me.Frame7.Controls.Add(Label20)
		Me.Frame7.Controls.Add(Label19)
		Me.Frame7.Controls.Add(Label13)
		Me.Frame7.Controls.Add(Label12)
		Me.Frame6.Controls.Add(Frame9)
		Me.Frame6.Controls.Add(Frame8)
		Me.Frame6.Controls.Add(Frame3)
		Me.Frame6.Controls.Add(Frame1)
		Me.Frame6.Controls.Add(Frame2)
		Me.Frame9.Controls.Add(txtIntervaloParaConexion)
		Me.Frame9.Controls.Add(txtTrabajo)
		Me.Frame9.Controls.Add(Label14)
		Me.Frame9.Controls.Add(Label16)
		Me.Frame8.Controls.Add(txtPuedeAtacar)
		Me.Frame8.Controls.Add(txtIntervaloLanzaHechizo)
		Me.Frame8.Controls.Add(Label17)
		Me.Frame8.Controls.Add(Label15)
		Me.Frame3.Controls.Add(txtIntervaloHambre)
		Me.Frame3.Controls.Add(txtIntervaloSed)
		Me.Frame3.Controls.Add(Label5)
		Me.Frame3.Controls.Add(Label6)
		Me.Frame1.Controls.Add(txtSanaIntervaloDescansar)
		Me.Frame1.Controls.Add(txtSanaIntervaloSinDescansar)
		Me.Frame1.Controls.Add(Label3)
		Me.Frame1.Controls.Add(Label1)
		Me.Frame2.Controls.Add(txtStaminaIntervaloSinDescansar)
		Me.Frame2.Controls.Add(txtStaminaIntervaloDescansar)
		Me.Frame2.Controls.Add(Label2)
		Me.Frame2.Controls.Add(Label4)
		Me.Frame5.Controls.Add(Frame10)
		Me.Frame10.Controls.Add(txtInvocacion)
		Me.Frame10.Controls.Add(txtIntervaloInvisible)
		Me.Frame10.Controls.Add(txtIntervaloParalizado)
		Me.Frame10.Controls.Add(txtIntervaloVeneno)
		Me.Frame10.Controls.Add(Label18)
		Me.Frame10.Controls.Add(Label11)
		Me.Frame10.Controls.Add(Label10)
		Me.Frame10.Controls.Add(Label8)
		Me.Frame11.ResumeLayout(False)
		Me.Frame4.ResumeLayout(False)
		Me.Frame12.ResumeLayout(False)
		Me.Frame7.ResumeLayout(False)
		Me.Frame6.ResumeLayout(False)
		Me.Frame9.ResumeLayout(False)
		Me.Frame8.ResumeLayout(False)
		Me.Frame3.ResumeLayout(False)
		Me.Frame1.ResumeLayout(False)
		Me.Frame2.ResumeLayout(False)
		Me.Frame5.ResumeLayout(False)
		Me.Frame10.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class
