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
''' Grid for showing Egg Production shapes.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class gridEggProduction
    Inherits gridForcingBase

    Public Sub New()
        MyBase.New()
        Me.IsSeasonal = True
    End Sub

    Private m_handler As New cEggProductionShapeGUIHandler()

    Public Overrides ReadOnly Property Handler() As ScientificInterfaceShared.Controls.cShapeGUIHandler
        Get
            Return Me.m_handler
        End Get
    End Property

    Public Overrides ReadOnly Property Manager() As System.Collections.IEnumerable
        Get
            Return Me.Core.EggProdShapeManager
        End Get
    End Property

End Class
