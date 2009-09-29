Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucAppThumbnails
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucAppThumbnails))
            Me.m_lblFontHeader = New System.Windows.Forms.Label
            Me.m_lblThumbnailSize = New System.Windows.Forms.Label
            Me.m_nudThumbnailSize = New System.Windows.Forms.NumericUpDown
            CType(Me.m_nudThumbnailSize, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lblFontHeader
            '
            Me.m_lblFontHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            resources.ApplyResources(Me.m_lblFontHeader, "m_lblFontHeader")
            Me.m_lblFontHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblFontHeader.Name = "m_lblFontHeader"
            '
            'm_lblThumbnailSize
            '
            resources.ApplyResources(Me.m_lblThumbnailSize, "m_lblThumbnailSize")
            Me.m_lblThumbnailSize.Name = "m_lblThumbnailSize"
            '
            'm_nudThumbnailSize
            '
            resources.ApplyResources(Me.m_nudThumbnailSize, "m_nudThumbnailSize")
            Me.m_nudThumbnailSize.Maximum = New Decimal(New Integer() {240, 0, 0, 0})
            Me.m_nudThumbnailSize.Minimum = New Decimal(New Integer() {32, 0, 0, 0})
            Me.m_nudThumbnailSize.Name = "m_nudThumbnailSize"
            Me.m_nudThumbnailSize.Value = New Decimal(New Integer() {32, 0, 0, 0})
            '
            'ucAppThumbnails
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_nudThumbnailSize)
            Me.Controls.Add(Me.m_lblThumbnailSize)
            Me.Controls.Add(Me.m_lblFontHeader)
            Me.Name = "ucAppThumbnails"
            CType(Me.m_nudThumbnailSize, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_nudThumbnailSize As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblFontHeader As System.Windows.Forms.Label
        Private WithEvents m_lblThumbnailSize As System.Windows.Forms.Label

    End Class
End Namespace

