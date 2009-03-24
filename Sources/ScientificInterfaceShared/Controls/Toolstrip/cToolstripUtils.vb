'==============================================================================
'
' $Log: cToolstripUtils.vb,v $
' Revision 1.1  2009/03/24 14:06:53  jeroens
' Moved, why not again. Renamed class as well
'
' Revision 1.1  2007/10/14 22:17:37  jeroens
' * Moved
'
' Revision 1.1  2007/09/15 00:16:15  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports System.Windows.Forms

Namespace Controls

    Public Class cToolstripUtils

        Shared Sub HideRepeatingSeparators(ByVal ts As ToolStrip)

            Dim tsi As ToolStripItem = Nothing
            Dim tsiLastSeparator As ToolStripItem = Nothing

            ts.SuspendLayout()
            ' For all toolbar items
            For i As Integer = 0 To ts.Items.Count - 1
                ' Get item
                tsi = ts.Items(i)
                ' Is a separator?
                If (TypeOf tsi Is ToolStripSeparator) Then
                    ' #Yes: ok, show it
                    tsi.Visible = True
                    ' Was previous separator visible?
                    If tsiLastSeparator IsNot Nothing Then
                        tsiLastSeparator.Visible = False
                    End If
                    ' Remember last separator
                    tsiLastSeparator = tsi
                Else
                    ' #No: regular control. Is this control visible?
                    If tsi.Visible = True Then
                        ' #Yes: forget last separator since it separates a visible control
                        tsiLastSeparator = Nothing
                    End If
                End If
            Next
            ts.ResumeLayout()

        End Sub

    End Class

End Namespace ' Controls
