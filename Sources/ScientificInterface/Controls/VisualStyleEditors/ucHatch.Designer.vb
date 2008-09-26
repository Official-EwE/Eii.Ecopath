Imports System.Windows.Forms

Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucHatch
        Inherits Panel

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
            Me.SuspendLayout()
            '
            'ucHatch
            '
            Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Name = "ucHatch"
            Me.Size = New System.Drawing.Size(34, 34)
            Me.Margin = New Windows.Forms.Padding(2)
            Me.BorderStyle = Windows.Forms.BorderStyle.None
            Me.ResumeLayout(False)

        End Sub

    End Class

End Namespace
