#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid for showing mediation shapes that interact on predator/prey pairs.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class gridPredPreyMediation
    Inherits gridMediation

    Private m_handler As New cMediationShapeGUIHandler()

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides ReadOnly Property Handler() As ScientificInterfaceShared.Controls.cShapeGUIHandler
        Get
            Return Me.m_handler
        End Get
    End Property

    Public Overrides ReadOnly Property Manager() As System.Collections.IEnumerable
        Get
            Return Me.Core.MediationShapeManager
        End Get
    End Property

End Class
