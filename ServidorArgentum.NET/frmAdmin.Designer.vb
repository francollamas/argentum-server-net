<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmAdmin
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
	Public WithEvents Text1 As System.Windows.Forms.TextBox
	Public WithEvents Command3 As System.Windows.Forms.Button
	Public WithEvents Command2 As System.Windows.Forms.Button
	Public WithEvents cboPjs As System.Windows.Forms.ComboBox
	Public WithEvents Command1 As System.Windows.Forms.Button
	Public WithEvents _Frame1_0 As System.Windows.Forms.GroupBox
	Public WithEvents Frame1 As Microsoft.VisualBasic.Compatibility.VB6.GroupBoxArray
	'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
	'Se puede modificar mediante el Diseñador de Windows Forms.
	'No lo modifique con el editor de código.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmAdmin))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me._Frame1_0 = New System.Windows.Forms.GroupBox
		Me.Text1 = New System.Windows.Forms.TextBox
		Me.Command3 = New System.Windows.Forms.Button
		Me.Command2 = New System.Windows.Forms.Button
		Me.cboPjs = New System.Windows.Forms.ComboBox
		Me.Command1 = New System.Windows.Forms.Button
		Me.Frame1 = New Microsoft.VisualBasic.Compatibility.VB6.GroupBoxArray(components)
		Me._Frame1_0.SuspendLayout()
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		CType(Me.Frame1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.Text = "Administración del servidor"
		Me.ClientSize = New System.Drawing.Size(312, 209)
		Me.Location = New System.Drawing.Point(4, 30)
		Me.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation
		Me.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
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
		Me.Name = "frmAdmin"
		Me._Frame1_0.Text = "Personajes"
		Me._Frame1_0.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._Frame1_0.Size = New System.Drawing.Size(281, 193)
		Me._Frame1_0.Location = New System.Drawing.Point(16, 8)
		Me._Frame1_0.TabIndex = 0
		Me._Frame1_0.BackColor = System.Drawing.SystemColors.Control
		Me._Frame1_0.Enabled = True
		Me._Frame1_0.ForeColor = System.Drawing.SystemColors.ControlText
		Me._Frame1_0.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me._Frame1_0.Visible = True
		Me._Frame1_0.Padding = New System.Windows.Forms.Padding(0)
		Me._Frame1_0.Name = "_Frame1_0"
		Me.Text1.AutoSize = False
		Me.Text1.Enabled = False
		Me.Text1.Size = New System.Drawing.Size(209, 33)
		Me.Text1.Location = New System.Drawing.Point(32, 48)
		Me.Text1.MultiLine = True
		Me.Text1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.Text1.TabIndex = 5
		Me.Text1.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Text1.AcceptsReturn = True
		Me.Text1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.Text1.BackColor = System.Drawing.SystemColors.Window
		Me.Text1.CausesValidation = True
		Me.Text1.ForeColor = System.Drawing.SystemColors.WindowText
		Me.Text1.HideSelection = True
		Me.Text1.ReadOnly = False
		Me.Text1.Maxlength = 0
		Me.Text1.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.Text1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Text1.TabStop = True
		Me.Text1.Visible = True
		Me.Text1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.Text1.Name = "Text1"
		Me.Command3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command3.Text = "Echar todos los PJS no privilegiados"
		Me.Command3.Size = New System.Drawing.Size(209, 25)
		Me.Command3.Location = New System.Drawing.Point(32, 152)
		Me.Command3.TabIndex = 4
		Me.Command3.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command3.BackColor = System.Drawing.SystemColors.Control
		Me.Command3.CausesValidation = True
		Me.Command3.Enabled = True
		Me.Command3.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command3.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command3.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command3.TabStop = True
		Me.Command3.Name = "Command3"
		Me.Command2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command2.Text = "R"
		Me.Command2.Size = New System.Drawing.Size(17, 21)
		Me.Command2.Location = New System.Drawing.Point(248, 24)
		Me.Command2.TabIndex = 3
		Me.Command2.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command2.BackColor = System.Drawing.SystemColors.Control
		Me.Command2.CausesValidation = True
		Me.Command2.Enabled = True
		Me.Command2.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command2.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command2.TabStop = True
		Me.Command2.Name = "Command2"
		Me.cboPjs.Size = New System.Drawing.Size(209, 21)
		Me.cboPjs.Location = New System.Drawing.Point(32, 24)
		Me.cboPjs.TabIndex = 2
		Me.cboPjs.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cboPjs.BackColor = System.Drawing.SystemColors.Window
		Me.cboPjs.CausesValidation = True
		Me.cboPjs.Enabled = True
		Me.cboPjs.ForeColor = System.Drawing.SystemColors.WindowText
		Me.cboPjs.IntegralHeight = True
		Me.cboPjs.Cursor = System.Windows.Forms.Cursors.Default
		Me.cboPjs.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.cboPjs.Sorted = False
		Me.cboPjs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown
		Me.cboPjs.TabStop = True
		Me.cboPjs.Visible = True
		Me.cboPjs.Name = "cboPjs"
		Me.Command1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command1.Text = "Echar"
		Me.Command1.Size = New System.Drawing.Size(209, 25)
		Me.Command1.Location = New System.Drawing.Point(32, 120)
		Me.Command1.TabIndex = 1
		Me.Command1.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Command1.BackColor = System.Drawing.SystemColors.Control
		Me.Command1.CausesValidation = True
		Me.Command1.Enabled = True
		Me.Command1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Command1.Cursor = System.Windows.Forms.Cursors.Default
		Me.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Command1.TabStop = True
		Me.Command1.Name = "Command1"
		Me.Controls.Add(_Frame1_0)
		Me._Frame1_0.Controls.Add(Text1)
		Me._Frame1_0.Controls.Add(Command3)
		Me._Frame1_0.Controls.Add(Command2)
		Me._Frame1_0.Controls.Add(cboPjs)
		Me._Frame1_0.Controls.Add(Command1)
		Me.Frame1.SetIndex(_Frame1_0, CType(0, Short))
		CType(Me.Frame1, System.ComponentModel.ISupportInitialize).EndInit()
		Me._Frame1_0.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class
