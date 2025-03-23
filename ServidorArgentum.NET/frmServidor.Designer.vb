<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmServidor
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
	Public WithEvents Command26 As System.Windows.Forms.Button
	Public WithEvents VS1 As System.Windows.Forms.VScrollBar
	Public WithEvents Command27 As System.Windows.Forms.Button
	Public WithEvents Command22 As System.Windows.Forms.Button
	Public WithEvents Command21 As System.Windows.Forms.Button
	Public WithEvents Command17 As System.Windows.Forms.Button
	Public WithEvents Command25 As System.Windows.Forms.Button
	Public WithEvents Command16 As System.Windows.Forms.Button
	Public WithEvents Command28 As System.Windows.Forms.Button
	Public WithEvents Command14 As System.Windows.Forms.Button
	Public WithEvents Command19 As System.Windows.Forms.Button
	Public WithEvents Command15 As System.Windows.Forms.Button
	Public WithEvents Command12 As System.Windows.Forms.Button
	Public WithEvents Command11 As System.Windows.Forms.Button
	Public WithEvents Command10 As System.Windows.Forms.Button
	Public WithEvents Command9 As System.Windows.Forms.Button
	Public WithEvents Command8 As System.Windows.Forms.Button
	Public WithEvents Command7 As System.Windows.Forms.Button
	Public WithEvents Command3 As System.Windows.Forms.Button
	Public WithEvents Command6 As System.Windows.Forms.Button
	Public WithEvents Command1 As System.Windows.Forms.Button
	Public WithEvents picCont As System.Windows.Forms.Panel
	Public WithEvents picFuera As System.Windows.Forms.Panel
	Public WithEvents Command23 As System.Windows.Forms.Button
	Public WithEvents Command5 As System.Windows.Forms.Button
	Public WithEvents Command18 As System.Windows.Forms.Button
	Public WithEvents Command4 As System.Windows.Forms.Button
	Public WithEvents Command2 As System.Windows.Forms.Button
	Public WithEvents Command20 As System.Windows.Forms.Button
	Public WithEvents Shape2 As Microsoft.VisualBasic.PowerPacks.RectangleShape
	Public WithEvents ShapeContainer1 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
	'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
	'Se puede modificar mediante el Diseñador de Windows Forms.
	'No lo modifique con el editor de código.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmServidor))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.ShapeContainer1 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer
		Me.Command26 = New System.Windows.Forms.Button
		Me.picFuera = New System.Windows.Forms.Panel
		Me.VS1 = New System.Windows.Forms.VScrollBar
		Me.picCont = New System.Windows.Forms.Panel
		Me.Command27 = New System.Windows.Forms.Button
		Me.Command22 = New System.Windows.Forms.Button
		Me.Command21 = New System.Windows.Forms.Button
		Me.Command17 = New System.Windows.Forms.Button
		Me.Command25 = New System.Windows.Forms.Button
		Me.Command16 = New System.Windows.Forms.Button
		Me.Command28 = New System.Windows.Forms.Button
		Me.Command14 = New System.Windows.Forms.Button
		Me.Command19 = New System.Windows.Forms.Button
		Me.Command15 = New System.Windows.Forms.Button
		Me.Command12 = New System.Windows.Forms.Button
		Me.Command11 = New System.Windows.Forms.Button
		Me.Command10 = New System.Windows.Forms.Button
		Me.Command9 = New System.Windows.Forms.Button
		Me.Command8 = New System.Windows.Forms.Button
		Me.Command7 = New System.Windows.Forms.Button
		Me.Command3 = New System.Windows.Forms.Button
		Me.Command6 = New System.Windows.Forms.Button
		Me.Command1 = New System.Windows.Forms.Button
		Me.Command23 = New System.Windows.Forms.Button
		Me.Command5 = New System.Windows.Forms.Button
		Me.Command18 = New System.Windows.Forms.Button
		Me.Command4 = New System.Windows.Forms.Button
		Me.Command2 = New System.Windows.Forms.Button
		Me.Command20 = New System.Windows.Forms.Button
		Me.Shape2 = New Microsoft.VisualBasic.PowerPacks.RectangleShape
		Me.picFuera.SuspendLayout()
		Me.picCont.SuspendLayout()
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.Text = "Servidor"
		Me.ClientSize = New System.Drawing.Size(323, 436)
		Me.Location = New System.Drawing.Point(4, 23)
		Me.ControlBox = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation
		Me.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
		Me.Enabled = True
		Me.KeyPreview = False
		Me.MaximizeBox = True
		Me.MinimizeBox = True
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.ShowInTaskbar = True
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "frmServidor"
		Me.Command26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command26.Text = "Reset Listen"
		Me.Command26.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command26.Size = New System.Drawing.Size(97, 17)
		Me.Command26.Location = New System.Drawing.Point(128, 412)
		Me.Command26.TabIndex = 26
		Me.Command26.BackColor = System.Drawing.SystemColors.Control
		Me.Command26.CausesValidation = True
		Me.Command26.Enabled = True
		Me.Command26.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command26.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command26.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command26.TabStop = True
		Me.Command26.Name = "Command26"
		Me.picFuera.BackColor = System.Drawing.Color.Black
		Me.picFuera.ForeColor = System.Drawing.SystemColors.WindowText
		Me.picFuera.Size = New System.Drawing.Size(306, 290)
		Me.picFuera.Location = New System.Drawing.Point(8, 8)
		Me.picFuera.TabIndex = 6
		Me.picFuera.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.picFuera.Dock = System.Windows.Forms.DockStyle.None
		Me.picFuera.CausesValidation = True
		Me.picFuera.Enabled = True
		Me.picFuera.Cursor = System.Windows.Forms.Cursors.Default
		Me.picFuera.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picFuera.TabStop = True
		Me.picFuera.Visible = True
		Me.picFuera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.picFuera.Name = "picFuera"
		Me.VS1.Size = New System.Drawing.Size(17, 289)
		Me.VS1.LargeChange = 50
		Me.VS1.Location = New System.Drawing.Point(288, 0)
		Me.VS1.SmallChange = 17
		Me.VS1.TabIndex = 24
		Me.VS1.CausesValidation = True
		Me.VS1.Enabled = True
		Me.VS1.Maximum = 32816
		Me.VS1.Minimum = 0
		Me.VS1.Cursor = System.Windows.Forms.Cursors.Default
		Me.VS1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.VS1.TabStop = True
		Me.VS1.Value = 0
		Me.VS1.Visible = True
		Me.VS1.Name = "VS1"
		Me.picCont.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.picCont.Size = New System.Drawing.Size(289, 321)
		Me.picCont.Location = New System.Drawing.Point(0, 0)
		Me.picCont.TabIndex = 7
		Me.picCont.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.picCont.Dock = System.Windows.Forms.DockStyle.None
		Me.picCont.CausesValidation = True
		Me.picCont.Enabled = True
		Me.picCont.ForeColor = System.Drawing.SystemColors.ControlText
		Me.picCont.Cursor = System.Windows.Forms.Cursors.Default
		Me.picCont.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picCont.TabStop = True
		Me.picCont.Visible = True
		Me.picCont.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.picCont.Name = "picCont"
		Me.Command27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command27.Text = "Debug UserList"
		Me.Command27.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command27.Size = New System.Drawing.Size(273, 17)
		Me.Command27.Location = New System.Drawing.Point(8, 296)
		Me.Command27.TabIndex = 27
		Me.Command27.BackColor = System.Drawing.SystemColors.Control
		Me.Command27.CausesValidation = True
		Me.Command27.Enabled = True
		Me.Command27.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command27.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command27.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command27.TabStop = True
		Me.Command27.Name = "Command27"
		Me.Command22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command22.Text = "Administración"
		Me.Command22.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command22.Size = New System.Drawing.Size(273, 17)
		Me.Command22.Location = New System.Drawing.Point(8, 280)
		Me.Command22.TabIndex = 8
		Me.Command22.BackColor = System.Drawing.SystemColors.Control
		Me.Command22.CausesValidation = True
		Me.Command22.Enabled = True
		Me.Command22.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command22.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command22.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command22.TabStop = True
		Me.Command22.Name = "Command22"
		Me.Command21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command21.Text = "Pausar el servidor"
		Me.Command21.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command21.Size = New System.Drawing.Size(273, 17)
		Me.Command21.Location = New System.Drawing.Point(8, 264)
		Me.Command21.TabIndex = 9
		Me.Command21.BackColor = System.Drawing.SystemColors.Control
		Me.Command21.CausesValidation = True
		Me.Command21.Enabled = True
		Me.Command21.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command21.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command21.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command21.TabStop = True
		Me.Command21.Name = "Command21"
		Me.Command17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command17.Text = "Actualizar npcs.dat"
		Me.Command17.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command17.Size = New System.Drawing.Size(273, 17)
		Me.Command17.Location = New System.Drawing.Point(8, 248)
		Me.Command17.TabIndex = 10
		Me.Command17.BackColor = System.Drawing.SystemColors.Control
		Me.Command17.CausesValidation = True
		Me.Command17.Enabled = True
		Me.Command17.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command17.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command17.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command17.TabStop = True
		Me.Command17.Name = "Command17"
		Me.Command25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command25.Text = "Reload MD5s"
		Me.Command25.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command25.Size = New System.Drawing.Size(273, 17)
		Me.Command25.Location = New System.Drawing.Point(8, 232)
		Me.Command25.TabIndex = 25
		Me.Command25.BackColor = System.Drawing.SystemColors.Control
		Me.Command25.CausesValidation = True
		Me.Command25.Enabled = True
		Me.Command25.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command25.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command25.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command25.TabStop = True
		Me.Command25.Name = "Command25"
		Me.Command16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command16.Text = "Reload Server.ini"
		Me.Command16.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command16.Size = New System.Drawing.Size(273, 17)
		Me.Command16.Location = New System.Drawing.Point(8, 216)
		Me.Command16.TabIndex = 11
		Me.Command16.BackColor = System.Drawing.SystemColors.Control
		Me.Command16.CausesValidation = True
		Me.Command16.Enabled = True
		Me.Command16.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command16.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command16.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command16.TabStop = True
		Me.Command16.Name = "Command16"
		Me.Command28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command28.Text = "Reload Balance.dat"
		Me.Command28.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command28.Size = New System.Drawing.Size(273, 17)
		Me.Command28.Location = New System.Drawing.Point(8, 200)
		Me.Command28.TabIndex = 28
		Me.Command28.BackColor = System.Drawing.SystemColors.Control
		Me.Command28.CausesValidation = True
		Me.Command28.Enabled = True
		Me.Command28.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command28.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command28.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command28.TabStop = True
		Me.Command28.Name = "Command28"
		Me.Command14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command14.Text = "Update MOTD"
		Me.Command14.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command14.Size = New System.Drawing.Size(273, 17)
		Me.Command14.Location = New System.Drawing.Point(8, 184)
		Me.Command14.TabIndex = 12
		Me.Command14.BackColor = System.Drawing.SystemColors.Control
		Me.Command14.CausesValidation = True
		Me.Command14.Enabled = True
		Me.Command14.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command14.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command14.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command14.TabStop = True
		Me.Command14.Name = "Command14"
		Me.Command19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command19.Text = "Unban All IPs (PELIGRO!)"
		Me.Command19.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command19.Size = New System.Drawing.Size(273, 17)
		Me.Command19.Location = New System.Drawing.Point(8, 168)
		Me.Command19.TabIndex = 13
		Me.Command19.BackColor = System.Drawing.SystemColors.Control
		Me.Command19.CausesValidation = True
		Me.Command19.Enabled = True
		Me.Command19.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command19.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command19.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command19.TabStop = True
		Me.Command19.Name = "Command19"
		Me.Command15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command15.Text = "Unban All (PELIGRO!)"
		Me.Command15.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command15.Size = New System.Drawing.Size(273, 17)
		Me.Command15.Location = New System.Drawing.Point(8, 152)
		Me.Command15.TabIndex = 14
		Me.Command15.BackColor = System.Drawing.SystemColors.Control
		Me.Command15.CausesValidation = True
		Me.Command15.Enabled = True
		Me.Command15.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command15.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command15.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command15.TabStop = True
		Me.Command15.Name = "Command15"
		Me.Command12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command12.Text = "Debug Npcs"
		Me.Command12.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command12.Size = New System.Drawing.Size(273, 17)
		Me.Command12.Location = New System.Drawing.Point(8, 136)
		Me.Command12.TabIndex = 15
		Me.Command12.BackColor = System.Drawing.SystemColors.Control
		Me.Command12.CausesValidation = True
		Me.Command12.Enabled = True
		Me.Command12.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command12.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command12.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command12.TabStop = True
		Me.Command12.Name = "Command12"
		Me.Command11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command11.Text = "Stats de los slots"
		Me.Command11.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command11.Size = New System.Drawing.Size(273, 17)
		Me.Command11.Location = New System.Drawing.Point(8, 120)
		Me.Command11.TabIndex = 16
		Me.Command11.BackColor = System.Drawing.SystemColors.Control
		Me.Command11.CausesValidation = True
		Me.Command11.Enabled = True
		Me.Command11.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command11.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command11.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command11.TabStop = True
		Me.Command11.Name = "Command11"
		Me.Command10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command10.Text = "Trafico"
		Me.Command10.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command10.Size = New System.Drawing.Size(273, 17)
		Me.Command10.Location = New System.Drawing.Point(8, 104)
		Me.Command10.TabIndex = 17
		Me.Command10.BackColor = System.Drawing.SystemColors.Control
		Me.Command10.CausesValidation = True
		Me.Command10.Enabled = True
		Me.Command10.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command10.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command10.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command10.TabStop = True
		Me.Command10.Name = "Command10"
		Me.Command9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command9.Text = "Reload Lista Nombres Prohibidos"
		Me.Command9.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command9.Size = New System.Drawing.Size(273, 17)
		Me.Command9.Location = New System.Drawing.Point(8, 88)
		Me.Command9.TabIndex = 18
		Me.Command9.BackColor = System.Drawing.SystemColors.Control
		Me.Command9.CausesValidation = True
		Me.Command9.Enabled = True
		Me.Command9.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command9.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command9.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command9.TabStop = True
		Me.Command9.Name = "Command9"
		Me.Command8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command8.Text = "Actualizar hechizos"
		Me.Command8.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command8.Size = New System.Drawing.Size(273, 17)
		Me.Command8.Location = New System.Drawing.Point(8, 72)
		Me.Command8.TabIndex = 19
		Me.Command8.BackColor = System.Drawing.SystemColors.Control
		Me.Command8.CausesValidation = True
		Me.Command8.Enabled = True
		Me.Command8.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command8.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command8.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command8.TabStop = True
		Me.Command8.Name = "Command8"
		Me.Command7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command7.Text = "Configurar intervalos"
		Me.Command7.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command7.Size = New System.Drawing.Size(273, 17)
		Me.Command7.Location = New System.Drawing.Point(8, 56)
		Me.Command7.TabIndex = 20
		Me.Command7.BackColor = System.Drawing.SystemColors.Control
		Me.Command7.CausesValidation = True
		Me.Command7.Enabled = True
		Me.Command7.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command7.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command7.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command7.TabStop = True
		Me.Command7.Name = "Command7"
		Me.Command3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command3.Text = "Reiniciar"
		Me.Command3.Enabled = False
		Me.Command3.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command3.Size = New System.Drawing.Size(273, 17)
		Me.Command3.Location = New System.Drawing.Point(8, 40)
		Me.Command3.TabIndex = 21
		Me.Command3.BackColor = System.Drawing.SystemColors.Control
		Me.Command3.CausesValidation = True
		Me.Command3.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command3.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command3.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command3.TabStop = True
		Me.Command3.Name = "Command3"
		Me.Command6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command6.Text = "ReSpawn Guardias en posiciones originales"
		Me.Command6.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command6.Size = New System.Drawing.Size(273, 17)
		Me.Command6.Location = New System.Drawing.Point(8, 24)
		Me.Command6.TabIndex = 22
		Me.Command6.BackColor = System.Drawing.SystemColors.Control
		Me.Command6.CausesValidation = True
		Me.Command6.Enabled = True
		Me.Command6.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command6.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command6.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command6.TabStop = True
		Me.Command6.Name = "Command6"
		Me.Command1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command1.Text = "Actualizar objetos.dat"
		Me.Command1.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command1.Size = New System.Drawing.Size(273, 17)
		Me.Command1.Location = New System.Drawing.Point(8, 8)
		Me.Command1.TabIndex = 23
		Me.Command1.BackColor = System.Drawing.SystemColors.Control
		Me.Command1.CausesValidation = True
		Me.Command1.Enabled = True
		Me.Command1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command1.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command1.TabStop = True
		Me.Command1.Name = "Command1"
		Me.Command23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command23.Text = "Boton Magico para apagar server"
		Me.Command23.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command23.Size = New System.Drawing.Size(273, 17)
		Me.Command23.Location = New System.Drawing.Point(16, 368)
		Me.Command23.TabIndex = 5
		Me.Command23.BackColor = System.Drawing.SystemColors.Control
		Me.Command23.CausesValidation = True
		Me.Command23.Enabled = True
		Me.Command23.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command23.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command23.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command23.TabStop = True
		Me.Command23.Name = "Command23"
		Me.Command5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command5.Text = "Cargar BackUp del mundo"
		Me.Command5.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command5.Size = New System.Drawing.Size(273, 17)
		Me.Command5.Location = New System.Drawing.Point(16, 344)
		Me.Command5.TabIndex = 1
		Me.Command5.BackColor = System.Drawing.SystemColors.Control
		Me.Command5.CausesValidation = True
		Me.Command5.Enabled = True
		Me.Command5.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command5.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command5.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command5.TabStop = True
		Me.Command5.Name = "Command5"
		Me.Command18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command18.Text = "Guardar todos los personajes"
		Me.Command18.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command18.Size = New System.Drawing.Size(273, 17)
		Me.Command18.Location = New System.Drawing.Point(16, 328)
		Me.Command18.TabIndex = 3
		Me.Command18.BackColor = System.Drawing.SystemColors.Control
		Me.Command18.CausesValidation = True
		Me.Command18.Enabled = True
		Me.Command18.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command18.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command18.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command18.TabStop = True
		Me.Command18.Name = "Command18"
		Me.Command4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command4.Text = "Hacer un Backup del mundo"
		Me.Command4.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command4.Size = New System.Drawing.Size(273, 17)
		Me.Command4.Location = New System.Drawing.Point(16, 312)
		Me.Command4.TabIndex = 2
		Me.Command4.BackColor = System.Drawing.SystemColors.Control
		Me.Command4.CausesValidation = True
		Me.Command4.Enabled = True
		Me.Command4.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command4.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command4.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command4.TabStop = True
		Me.Command4.Name = "Command4"
		Me.Command2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command2.Text = "OK"
		Me.AcceptButton = Me.Command2
		Me.Command2.Size = New System.Drawing.Size(63, 17)
		Me.Command2.Location = New System.Drawing.Point(232, 412)
		Me.Command2.TabIndex = 0
		Me.Command2.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command2.BackColor = System.Drawing.SystemColors.Control
		Me.Command2.CausesValidation = True
		Me.Command2.Enabled = True
		Me.Command2.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command2.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command2.TabStop = True
		Me.Command2.Name = "Command2"
		Me.Command20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command20.Text = "Reset sockets"
		Me.Command20.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command20.Size = New System.Drawing.Size(105, 17)
		Me.Command20.Location = New System.Drawing.Point(16, 412)
		Me.Command20.TabIndex = 4
		Me.Command20.BackColor = System.Drawing.SystemColors.Control
		Me.Command20.CausesValidation = True
		Me.Command20.Enabled = True
		Me.Command20.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command20.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command20.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command20.TabStop = True
		Me.Command20.Name = "Command20"
		Me.Shape2.Size = New System.Drawing.Size(289, 89)
		Me.Shape2.Location = New System.Drawing.Point(8, 304)
		Me.Shape2.BackColor = System.Drawing.SystemColors.Window
		Me.Shape2.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Transparent
		Me.Shape2.BorderColor = System.Drawing.SystemColors.WindowText
		Me.Shape2.BorderStyle = System.Drawing.Drawing2D.DashStyle.Solid
		Me.Shape2.BorderWidth = 1
		Me.Shape2.FillColor = System.Drawing.Color.Black
		Me.Shape2.FillStyle = Microsoft.VisualBasic.PowerPacks.FillStyle.Transparent
		Me.Shape2.Visible = True
		Me.Shape2.Name = "Shape2"
		Me.Controls.Add(Command26)
		Me.Controls.Add(picFuera)
		Me.Controls.Add(Command23)
		Me.Controls.Add(Command5)
		Me.Controls.Add(Command18)
		Me.Controls.Add(Command4)
		Me.Controls.Add(Command2)
		Me.Controls.Add(Command20)
		Me.ShapeContainer1.Shapes.Add(Shape2)
		Me.Controls.Add(ShapeContainer1)
		Me.picFuera.Controls.Add(VS1)
		Me.picFuera.Controls.Add(picCont)
		Me.picCont.Controls.Add(Command27)
		Me.picCont.Controls.Add(Command22)
		Me.picCont.Controls.Add(Command21)
		Me.picCont.Controls.Add(Command17)
		Me.picCont.Controls.Add(Command25)
		Me.picCont.Controls.Add(Command16)
		Me.picCont.Controls.Add(Command28)
		Me.picCont.Controls.Add(Command14)
		Me.picCont.Controls.Add(Command19)
		Me.picCont.Controls.Add(Command15)
		Me.picCont.Controls.Add(Command12)
		Me.picCont.Controls.Add(Command11)
		Me.picCont.Controls.Add(Command10)
		Me.picCont.Controls.Add(Command9)
		Me.picCont.Controls.Add(Command8)
		Me.picCont.Controls.Add(Command7)
		Me.picCont.Controls.Add(Command3)
		Me.picCont.Controls.Add(Command6)
		Me.picCont.Controls.Add(Command1)
		Me.picFuera.ResumeLayout(False)
		Me.picCont.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class