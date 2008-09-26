'==============================================================================
'
' $Log: ucFormSeparator.vb,v $
' Revision 1.1  2008/09/26 07:31:18  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:10  jeroens
' Separated from Scientific Interface
'
' Revision 1.4  2007/12/11 15:04:43  jeroens
' * Light colour was too dark still
'
' Revision 1.3  2007/12/10 16:54:40  jeroens
' - Simplified
'
'==============================================================================

Option Strict On

Namespace Controls

    <ToolboxBitmap(GetType(ucFormSeparator), "ucFormSeparator.ico")> _
    Public Class ucFormSeparator

        Private m_bHorizontal As Boolean = True

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

            Using br As New SolidBrush(Me.BackColor)
                e.Graphics.FillRectangle(br, 0, 0, Width, Height)
            End Using

            If Me.m_bHorizontal Then
                Using p As New Pen(SystemColors.ControlDark, 1)
                    e.Graphics.DrawLine(p, 0, 0, Me.Width, 0)
                End Using
                Using p As New Pen(SystemColors.ControlLightLight, 1)
                    e.Graphics.DrawLine(p, 0, 1, Me.Width, 1)
                End Using
            Else
                Using p As New Pen(SystemColors.ControlDark, 1)
                    e.Graphics.DrawLine(p, 0, 0, 0, Me.Height)
                End Using
                Using p As New Pen(SystemColors.ControlLightLight, 1)
                    e.Graphics.DrawLine(p, 1, Me.Height, 1, Me.Height)
                End Using
            End If

        End Sub

    End Class

End Namespace
