#Region " Imports "

Option Strict On

Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid for showing regular Forcing Functions.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class gridForcing
    Inherits gridForcingBase

    Public Sub New()
        MyBase.New()
    End Sub

    Private m_handler As New cForcingShapeGUIHandler()

    Public Overrides ReadOnly Property Handler() As ScientificInterfaceShared.Controls.cShapeGUIHandler
        Get
            Return Me.m_handler
        End Get
    End Property

    Public Overrides ReadOnly Property Manager() As System.Collections.IEnumerable
        Get
            Return Me.UIContext.Core.ForcingShapeManager
        End Get
    End Property

End Class
