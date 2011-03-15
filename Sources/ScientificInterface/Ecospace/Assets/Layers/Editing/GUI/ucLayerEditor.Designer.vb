Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditor
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditor))
            Me.m_lblCaption = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.SuspendLayout()
            '
            'm_lblCaption
            '
            resources.ApplyResources(Me.m_lblCaption, "m_lblCaption")
            Me.m_lblCaption.Name = "m_lblCaption"
            '
            'ucLayerEditor
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblCaption)
            Me.Name = "ucLayerEditor"
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_lblCaption As ScientificInterfaceShared.Controls.cEwEHeaderLabel

    End Class

End Namespace
