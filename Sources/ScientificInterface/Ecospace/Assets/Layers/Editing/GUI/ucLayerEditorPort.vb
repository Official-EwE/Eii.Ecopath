Namespace Ecospace.Basemap.Layers

    Public Class ucLayerEditorPort

        Private Sub OnClear(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnClear.Click
            Me.UIContext.Core.ClearEcospacePort(Me.FleetIndex)
        End Sub

        Private Sub OnSet(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSet.Click
            Me.UIContext.Core.SetEcospaceAllCoastToPort(Me.FleetIndex)
        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            Me.m_btnClear.Enabled = (Me.IsAttached)
            Me.m_btnSet.Enabled = (Me.IsAttached)

        End Sub

    End Class

End Namespace
