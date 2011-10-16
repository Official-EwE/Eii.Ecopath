Namespace Controls.Map.Layers

    Public Class ucLayerEditorSailCost

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)
            Me.m_btnCalculate.Enabled = Me.IsAttached
        End Sub

        Private Sub OnCalculate(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCalculate.Click
            Me.UIContext.Core.CalcEcospaceCostOfSailing()
        End Sub

        Private Sub OnSmooth(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSmooth.Click
            Me.Editor.Smooth()
        End Sub

    End Class

End Namespace
