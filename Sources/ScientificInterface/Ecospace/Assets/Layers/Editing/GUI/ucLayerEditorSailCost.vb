Namespace Ecospace.Basemap.Layers

    Public Class ucLayerEditorSailCost

        Public Overrides Sub UpdateContent()
            MyBase.UpdateContent()
            Me.m_btnCalculate.Enabled = Me.IsAttached
        End Sub

        Private Sub m_btnCalculate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCalculate.Click
            Me.UIContext.Core.CalcEcospaceCostOfSailing()
        End Sub

    End Class

End Namespace
