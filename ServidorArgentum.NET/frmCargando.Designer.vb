<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmCargando
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
	Public WithEvents _Label1_3 As System.Windows.Forms.Label
	Public WithEvents Picture1 As System.Windows.Forms.Panel
	Public WithEvents porcentaje As System.Windows.Forms.Label
	Public WithEvents _Label1_2 As System.Windows.Forms.Label
	Public WithEvents Label1 As Microsoft.VisualBasic.Compatibility.VB6.LabelArray
	'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
	'Se puede modificar mediante el Diseñador de Windows Forms.
	'No lo modifique con el editor de código.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmCargando))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.Picture1 = New System.Windows.Forms.Panel
		Me._Label1_3 = New System.Windows.Forms.Label
		Me.porcentaje = New System.Windows.Forms.Label
		Me._Label1_2 = New System.Windows.Forms.Label
		Me.Label1 = New Microsoft.VisualBasic.Compatibility.VB6.LabelArray(components)
		Me.Picture1.SuspendLayout()
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		CType(Me.Label1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
		Me.Text = "Argentum"
		Me.ClientSize = New System.Drawing.Size(430, 207)
		Me.Location = New System.Drawing.Point(94, 200)
		Me.ControlBox = False
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.ShowInTaskbar = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Enabled = True
		Me.KeyPreview = False
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "frmCargando"
		Me.Picture1.Size = New System.Drawing.Size(449, 185)
		Me.Picture1.Location = New System.Drawing.Point(-8, -8)
		Me.Picture1.TabIndex = 0
		Me.Picture1.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Picture1.Dock = System.Windows.Forms.DockStyle.None
		Me.Picture1.BackColor = System.Drawing.SystemColors.Control
		Me.Picture1.CausesValidation = True
		Me.Picture1.Enabled = True
		Me.Picture1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Picture1.Cursor = System.Windows.Forms.Cursors.Default
		Me.Picture1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Picture1.TabStop = True
		Me.Picture1.Visible = True
		Me.Picture1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Picture1.Name = "Picture1"
		Me._Label1_3.TextAlign = System.Drawing.ContentAlignment.TopCenter
		Me._Label1_3.Text = "Cargando, por favor espere..."
		Me._Label1_3.ForeColor = System.Drawing.Color.Red
		Me._Label1_3.Size = New System.Drawing.Size(163, 15)
		Me._Label1_3.Location = New System.Drawing.Point(152, 136)
		Me._Label1_3.TabIndex = 2
		Me._Label1_3.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._Label1_3.BackColor = System.Drawing.Color.Transparent
		Me._Label1_3.Enabled = True
		Me._Label1_3.Cursor = System.Windows.Forms.Cursors.Default
		Me._Label1_3.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me._Label1_3.UseMnemonic = True
		Me._Label1_3.Visible = True
		Me._Label1_3.AutoSize = True
		Me._Label1_3.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me._Label1_3.Name = "_Label1_3"
		Me.porcentaje.BackColor = System.Drawing.Color.Transparent
		Me.porcentaje.Text = "0 %"
		Me.porcentaje.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.porcentaje.ForeColor = System.Drawing.SystemColors.WindowText
		Me.porcentaje.Size = New System.Drawing.Size(49, 16)
		Me.porcentaje.Location = New System.Drawing.Point(24, 184)
		Me.porcentaje.TabIndex = 3
		Me.porcentaje.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.porcentaje.Enabled = True
		Me.porcentaje.Cursor = System.Windows.Forms.Cursors.Default
		Me.porcentaje.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.porcentaje.UseMnemonic = True
		Me.porcentaje.Visible = True
		Me.porcentaje.AutoSize = True
		Me.porcentaje.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.porcentaje.Name = "porcentaje"
		Me._Label1_2.TextAlign = System.Drawing.ContentAlignment.TopRight
		Me._Label1_2.Text = " aa"
		Me._Label1_2.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._Label1_2.ForeColor = System.Drawing.Color.Black
		Me._Label1_2.Size = New System.Drawing.Size(17, 13)
		Me._Label1_2.Location = New System.Drawing.Point(408, 184)
		Me._Label1_2.TabIndex = 1
		Me._Label1_2.BackColor = System.Drawing.Color.Transparent
		Me._Label1_2.Enabled = True
		Me._Label1_2.Cursor = System.Windows.Forms.Cursors.Default
		Me._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me._Label1_2.UseMnemonic = True
		Me._Label1_2.Visible = True
		Me._Label1_2.AutoSize = True
		Me._Label1_2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me._Label1_2.Name = "_Label1_2"
		Me.Controls.Add(Picture1)
		Me.Controls.Add(porcentaje)
		Me.Controls.Add(_Label1_2)
		Me.Picture1.Controls.Add(_Label1_3)
		Me.Label1.SetIndex(_Label1_3, CType(3, Short))
		Me.Label1.SetIndex(_Label1_2, CType(2, Short))
		CType(Me.Label1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.Picture1.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class
