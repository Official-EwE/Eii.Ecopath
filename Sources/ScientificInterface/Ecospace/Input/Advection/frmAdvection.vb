#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterface.Ecospace.Basemap.Layers

#End Region ' Imports

Namespace Ecospace.Advection

    Public Class frmAdvection

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Design time bypass
            If Me.UIContext Is Nothing Then Return

            ' Connect all layers to the zoom toolbar
            For Each uc As ucAdvectionMap In Me.Maps
                Me.m_ucZoomToolbar.AddZoomContainer(uc.ZoomCtrl)
            Next
            Me.m_ucZoomToolbar.PositionMode = ucMapZoom.ePositionModeTypes.Center

            ' Populate month dropdown
            Me.m_tscmMonth.Items.Clear()
            For i As Integer = 1 To cCore.N_MONTHS
                Me.m_tscmMonth.Items.Add(cDateUtils.GetMonthName(i))
            Next
            Me.m_tscmMonth.SelectedIndex = 0

            ' Initialize control values
            Me.m_nudWind.Value = CDec(DirectCast(Me.m_ucWind.LayerEdit.Editor, cLayerEditorVector).ScaleFactor)
            Me.m_nudDepth.Value = CDec(Me.m_ucMLD.LayerEdit.Editor.CellValue)

            ' Config EwEForm
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}

            ' Kick off
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

            For Each uc As ucAdvectionMap In Me.Maps
                Me.m_ucZoomToolbar.RemoveZoomContainer(uc.ZoomCtrl)
            Next

            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Form overrides

#Region " Public bits "

        Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
                MyBase.UIContext = value
                Me.m_ucZoomToolbar.UIContext = Me.UIContext
                For Each uc As ucAdvectionMap In Me.Maps
                    uc.UIContext = value
                Next
            End Set
        End Property

#End Region ' Public bits

#Region " Control events "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            '' Refresh basemap on ANY data added or removed message from Ecospace
            'If ((msg.Source = eCoreComponentType.EcoSpace) And (msg.Type = eMessageType.DataAddedOrRemoved)) Then
            '    ' Refresh it all
            '    Me.Basemap = Me.Core.EcospaceBasemap
            'End If
        End Sub

        Private Sub OnShowOptions(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiShowOptions.Click

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            Me.m_scMain.Panel1Collapsed = Not Me.m_scMain.Panel1Collapsed
            Me.UpdateControls()

        End Sub

        Private Sub OnShowMonth(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tscmMonth.SelectedIndexChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            Dim layer As cLayer = Me.m_ucWind.LayerEdit
            DirectCast(layer.Data, cEcospaceLayerWind).Month = (1 + Me.m_tscmMonth.SelectedIndex)
            layer.Update(cLayer.eChangeFlags.Map, False)

        End Sub

        Private Sub OnCursorSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_sliderCursor.ValueChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            ' Get cursor size
            Dim iCursorSize As Integer = CInt(Me.m_sliderCursor.Value)

            ' Distribute cursor size
            For Each uc As ucAdvectionMap In Me.Maps
                If uc.LayerEdit IsNot Nothing Then
                    uc.LayerEdit.Editor.CursorSize = iCursorSize
                    uc.Map.UpdateCursorFeedback()
                End If
            Next

        End Sub

        Private Sub OnWindValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudWind.ValueChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            With DirectCast(Me.m_ucWind.LayerEdit.Editor, cLayerEditorVector)
                .ScaleFactor = CSng(Me.m_nudWind.Value)
            End With

        End Sub

        Private Sub OnMLDValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudDepth.ValueChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            Me.m_ucMLD.LayerEdit.Editor.CellValue = CSng(Me.m_nudDepth.Value)

        End Sub

#End Region ' Control events

#Region " Internals "

        Private Sub UpdateControls()
            Me.m_tsmiShowOptions.Checked = Not Me.m_scMain.Panel1Collapsed
        End Sub

        Private Function Maps() As ucAdvectionMap()
            Return New ucAdvectionMap() {Me.m_ucMap, Me.m_ucMLD, Me.m_ucUpwelling, Me.m_ucWind}
        End Function

#End Region ' Internals

    End Class

End Namespace
