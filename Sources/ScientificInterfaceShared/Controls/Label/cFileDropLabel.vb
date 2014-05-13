Namespace Controls

    Public Class cFileDropLabel
        Inherits Label

        Private m_bDragOver As Boolean = False

        Public Event OnFilesDropped(sender As Object, astrFiles As String())

        Protected Overrides Sub InitLayout()
            MyBase.InitLayout()
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub OnDragDrop(e As System.Windows.Forms.DragEventArgs)
            Try
                If Not Me.m_bDragOver Then Return
                RaiseEvent OnFilesDropped(Me, CType(e.Data.GetData(DataFormats.FileDrop), String()))
            Catch ex As Exception
            End Try
            Me.m_bDragOver = False
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub OnDragEnter(e As System.Windows.Forms.DragEventArgs)
            Try
                If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
                    e.Effect = DragDropEffects.All
                    Me.m_bDragOver = True
                End If
            Catch ex As Exception
                Me.m_bDragOver = False
            End Try
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub OnDragLeave(e As System.EventArgs)
            Try
                Me.m_bDragOver = False
            Catch ex As Exception

            End Try
            Me.UpdateControls()
        End Sub

        Protected Overridable Sub UpdateControls()
            If Me.m_bDragOver Then
                Me.BackColor = SystemColors.Highlight
                Me.ForeColor = SystemColors.HighlightText
            Else
                Me.BackColor = Drawing.Color.Transparent
                Me.ForeColor = SystemColors.ButtonShadow
            End If
        End Sub

    End Class

End Namespace

