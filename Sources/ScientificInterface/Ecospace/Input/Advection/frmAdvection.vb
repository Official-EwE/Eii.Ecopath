#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecospace.Advection

    Public Class frmAdvection

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
                MyBase.UIContext = value
                Me.m_ucZoomToolbar.UIContext = Me.UIContext
                Me.m_ucMap.UIContext = Me.UIContext
                Me.m_ucWind.UIContext = Me.UIContext
                Me.m_ucMLD.UIContext = Me.UIContext
                Me.m_ucUpwelling.UIContext = Me.UIContext
            End Set
        End Property

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Design time bypass
            If Me.UIContext Is Nothing Then Return

            Me.m_ucZoomToolbar.AddZoomContainer(Me.m_ucMap.ZoomCtrl)
            Me.m_ucZoomToolbar.AddZoomContainer(Me.m_ucWind.ZoomCtrl)
            Me.m_ucZoomToolbar.AddZoomContainer(Me.m_ucMLD.ZoomCtrl)
            Me.m_ucZoomToolbar.AddZoomContainer(Me.m_ucUpwelling.ZoomCtrl)

            Me.m_tscmMonth.Items.Clear()
            For i As Integer = 1 To cCore.N_MONTHS
                Me.m_tscmMonth.Items.Add(cDateUtils.GetMonthName(i))
            Next

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}
            Me.UpdateControls()

            ' Kick off
            Me.m_ucZoomToolbar.PositionMode = ucMapZoom.ePositionModeTypes.Center

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.m_ucZoomToolbar.RemoveZoomContainer(Me.m_ucMap.ZoomCtrl)
            Me.m_ucZoomToolbar.RemoveZoomContainer(Me.m_ucWind.ZoomCtrl)
            Me.m_ucZoomToolbar.RemoveZoomContainer(Me.m_ucMLD.ZoomCtrl)
            Me.m_ucZoomToolbar.RemoveZoomContainer(Me.m_ucUpwelling.ZoomCtrl)

            MyBase.OnFormClosed(e)

        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            '' Refresh basemap on ANY data added or removed message from Ecospace
            'If ((msg.Source = eCoreComponentType.EcoSpace) And (msg.Type = eMessageType.DataAddedOrRemoved)) Then
            '    ' Refresh it all
            '    Me.Basemap = Me.Core.EcospaceBasemap
            'End If
        End Sub

        Private Sub OnShowOptions(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiShowOptions.Click
            Me.m_scMain.Panel1Collapsed = Not Me.m_scMain.Panel1Collapsed
            Me.UpdateControls()
        End Sub

        Private Sub UpdateControls()
            Me.m_tsmiShowOptions.Checked = Not Me.m_scMain.Panel1Collapsed
        End Sub

        Private Sub OnShowMonth(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tscmMonth.SelectedIndexChanged

        End Sub

    End Class

End Namespace
