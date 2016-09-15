Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms

Public Class frmUI
    Inherits frmEwEGrid

    Public Sub New(uic As cUIContext, plugin As cFishMIPResultWriterPlugin)
        Me.UIContext = uic
        Me.InitializeComponent()
        Me.plugin = plugin
        Me.m_grid.Plugin = plugin
        Me.Grid = m_grid
    End Sub

    Public Property plugin As cFishMIPResultWriterPlugin = Nothing

    Private Sub m_tsbnFill_Click(sender As Object, e As EventArgs) Handles m_tsbnPopulate.Click

        Dim smalluns As Integer() = New Integer() {1, 4, 7, 10, 13, 16}
        For Each cat As cFishMIPResultWriterPlugin.eResultTypes In [Enum].GetValues(GetType(cFishMIPResultWriterPlugin.eResultTypes))

            For igroup As Integer = 1 To Me.Core.nGroups

                Dim bChecked As Boolean = False
                Dim grp As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(igroup)
                Dim grpOut As cEcoPathGroupOutput = Me.Core.EcoPathGroupOutputs(igroup)
                Dim name As String = grp.Name.ToLower()

                Select Case cat
                    Case cFishMIPResultWriterPlugin.eResultTypes.tsb
                        bChecked = grp.IsProducer() Or grp.IsConsumer()
                    Case cFishMIPResultWriterPlugin.eResultTypes.tcb
                        bChecked = grp.IsConsumer() And grpOut.TTLX() > 1
                    Case cFishMIPResultWriterPlugin.eResultTypes.b10cm
                        bChecked = grp.Index <= 24
                    Case cFishMIPResultWriterPlugin.eResultTypes.b30cm
                        bChecked = grp.Index <= 24 And Array.IndexOf(smalluns, grp.Index) = -1
                    Case cFishMIPResultWriterPlugin.eResultTypes.tc
                        bChecked = grp.IsFished()
                End Select

                Me.m_plugin.m_config(igroup, cat) = bChecked
            Next
        Next

        Me.Grid.RefreshContent()
        Me.Plugin.ConfigChanged

    End Sub

End Class