Imports EwEUtils.Core
Imports EwECore

#Region " Imports "

#End Region ' Imports

Namespace Ecospace.Advection

    Public Class frmAdvection

        Public Sub New()
            Me.InitializeComponent()
            Me.m_ucTransportRate.UIContext = Me.UIContext
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}
        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            '' Refresh basemap on ANY data added or removed message from Ecospace
            'If ((msg.Source = eCoreComponentType.EcoSpace) And (msg.Type = eMessageType.DataAddedOrRemoved)) Then
            '    ' Refresh it all
            '    Me.Basemap = Me.Core.EcospaceBasemap
            'End If
        End Sub

    End Class

End Namespace
