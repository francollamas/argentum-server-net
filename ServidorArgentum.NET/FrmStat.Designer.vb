<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class FrmStat
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
	Public WithEvents Porcentaje As System.Windows.Forms.Label
	Public WithEvents Titu As System.Windows.Forms.Label
	'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
	'Se puede modificar mediante el Diseñador de Windows Forms.
	'No lo modifique con el editor de código.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(FrmStat))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.Porcentaje = New System.Windows.Forms.Label
		Me.Titu = New System.Windows.Forms.Label
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.ControlBox = False
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.ClientSize = New System.Drawing.Size(375, 73)
		Me.Location = New System.Drawing.Point(3, 3)
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.ShowInTaskbar = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Enabled = True
		Me.KeyPreview = False
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "FrmStat"
		Me.Porcentaje.Text = "0 %"
		Me.Porcentaje.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Porcentaje.ForeColor = System.Drawing.Color.Black
		Me.Porcentaje.Size = New System.Drawing.Size(21, 13)
		Me.Porcentaje.Location = New System.Drawing.Point(184, 48)
		Me.Porcentaje.TabIndex = 1
		Me.Porcentaje.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Porcentaje.BackColor = System.Drawing.Color.Transparent
		Me.Porcentaje.Enabled = True
		Me.Porcentaje.Cursor = System.Windows.Forms.Cursors.Default
		Me.Porcentaje.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Porcentaje.UseMnemonic = True
		Me.Porcentaje.Visible = True
		Me.Porcentaje.AutoSize = True
		Me.Porcentaje.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Porcentaje.Name = "Porcentaje"
		Me.Titu.Text = "Procesando mapas..."
		Me.Titu.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Titu.ForeColor = System.Drawing.Color.Black
		Me.Titu.Size = New System.Drawing.Size(120, 13)
		Me.Titu.Location = New System.Drawing.Point(136, 16)
		Me.Titu.TabIndex = 0
		Me.Titu.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Titu.BackColor = System.Drawing.Color.Transparent
		Me.Titu.Enabled = True
		Me.Titu.Cursor = System.Windows.Forms.Cursors.Default
		Me.Titu.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Titu.UseMnemonic = True
		Me.Titu.Visible = True
		Me.Titu.AutoSize = True
		Me.Titu.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Titu.Name = "Titu"
		Me.Controls.Add(Porcentaje)
		Me.Controls.Add(Titu)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class