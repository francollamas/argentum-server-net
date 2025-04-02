<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmTrafic
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
	Public WithEvents Command1 As System.Windows.Forms.Button
	Public WithEvents lstTrafico As System.Windows.Forms.ListBox
	'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
	'Se puede modificar mediante el Diseñador de Windows Forms.
	'No lo modifique con el editor de código.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmTrafic))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.Command1 = New System.Windows.Forms.Button
		Me.lstTrafico = New System.Windows.Forms.ListBox
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Text = "Trafico"
		Me.ClientSize = New System.Drawing.Size(312, 213)
		Me.Location = New System.Drawing.Point(3, 22)
		Me.ControlBox = False
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation
		Me.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.BackColor = System.Drawing.SystemColors.Control
		Me.Enabled = True
		Me.KeyPreview = False
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.ShowInTaskbar = True
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "frmTrafic"
		Me.Command1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.Command1.Text = "Cerrar"
		Me.Command1.Size = New System.Drawing.Size(64, 39)
		Me.Command1.Location = New System.Drawing.Point(6, 150)
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
		Me.lstTrafico.Size = New System.Drawing.Size(297, 137)
		Me.lstTrafico.Location = New System.Drawing.Point(4, 9)
		Me.lstTrafico.TabIndex = 0
		Me.lstTrafico.Font = New System.Drawing.Font("Arial", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.lstTrafico.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.lstTrafico.BackColor = System.Drawing.SystemColors.Window
		Me.lstTrafico.CausesValidation = True
		Me.lstTrafico.Enabled = True
		Me.lstTrafico.ForeColor = System.Drawing.SystemColors.WindowText
		Me.lstTrafico.IntegralHeight = True
		Me.lstTrafico.Cursor = System.Windows.Forms.Cursors.Default
		Me.lstTrafico.SelectionMode = System.Windows.Forms.SelectionMode.One
		Me.lstTrafico.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.lstTrafico.Sorted = False
		Me.lstTrafico.TabStop = True
		Me.lstTrafico.Visible = True
		Me.lstTrafico.MultiColumn = False
		Me.lstTrafico.Name = "lstTrafico"
		Me.Controls.Add(Command1)
		Me.Controls.Add(lstTrafico)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class
