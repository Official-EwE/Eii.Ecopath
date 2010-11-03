Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.m_ilNavigation = New System.Windows.Forms.ImageList(Me.components)
        Me.SuspendLayout()
        '
        'm_ilNavigation
        '
        Me.m_ilNavigation.ImageStream = CType(resources.GetObject("m_ilNavigation.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.m_ilNavigation.TransparentColor = System.Drawing.Color.Transparent
        Me.m_ilNavigation.Images.SetKeyName(0, "application_get.png")
        Me.m_ilNavigation.Images.SetKeyName(1, "application_put.png")
        Me.m_ilNavigation.Images.SetKeyName(2, "run.bmp")
        Me.m_ilNavigation.Images.SetKeyName(3, "tools.bmp")
        Me.m_ilNavigation.Images.SetKeyName(4, "Ecopath.bmp")
        Me.m_ilNavigation.Images.SetKeyName(5, "output_extend.png")
        Me.m_ilNavigation.Images.SetKeyName(6, "input_extend.png")
        Me.m_ilNavigation.Images.SetKeyName(7, "wi0064-16.ico")
        Me.m_ilNavigation.Images.SetKeyName(8, "wi0126-16.ico")
        Me.m_ilNavigation.Images.SetKeyName(9, "wi0122-16.ico")
        Me.m_ilNavigation.Images.SetKeyName(10, "wi0054-16.ico")
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(999, 583)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.TabText = "<title>"
        Me.Text = "<title>"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents m_ilNavigation As System.Windows.Forms.ImageList
End Class
