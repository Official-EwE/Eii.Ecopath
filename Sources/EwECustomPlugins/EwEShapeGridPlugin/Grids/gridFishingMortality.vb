#Region " Imports "

Option Strict On

Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid for showing fishing mortality shapes.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class gridFishingMortality
    Inherits gridForcingBase

    Public Sub New()
        MyBase.New()
    End Sub

    Private m_handler As New cFishingMortalityShapeGUIHandler()

    Public Overrides ReadOnly Property Handler() As ScientificInterfaceShared.Controls.cShapeGUIHandler
        Get
            Return Me.m_handler
        End Get
    End Property

    Public Overrides ReadOnly Property Manager() As System.Collections.IEnumerable
        Get
            Return Me.UIContext.Core.FishMortShapeManager
        End Get
    End Property

End Class
