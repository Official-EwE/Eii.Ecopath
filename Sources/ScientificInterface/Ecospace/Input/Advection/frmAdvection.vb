Imports EwEUtils.Core
Imports EwECore

#Region " Imports "

#End Region ' Imports

Namespace Ecospace.Advection

    Public Class frmAdvection

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Design time bypass
            If Me.UIContext Is Nothing Then Return

            Me.m_ucMap.UIContext = Me.UIContext
            Me.m_ucWind.UIContext = Me.UIContext
            Me.m_ucMLD.UIContext = Me.UIContext
            Me.m_ucUpwelling.UIContext = Me.UIContext

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.m_ucMap.UIContext = Nothing
            Me.m_ucWind.UIContext = Nothing
            Me.m_ucMLD.UIContext = Nothing
            Me.m_ucUpwelling.UIContext = Nothing

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

    End Class

End Namespace
