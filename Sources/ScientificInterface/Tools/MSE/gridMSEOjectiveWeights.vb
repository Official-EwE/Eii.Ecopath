#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.SearchObjectives
Imports EwEUtils.Core

#End Region

''' ===========================================================================
''' <summary>
''' gridMSEOjectiveWeights provides a wrapper around gridSearchObjectivesGroup,
''' for the MSE, so it has a constructor with no arguments and can be created 
''' by the NavigationPanel.
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class gridMSEOjectiveWeights
    : Inherits Ecosim.gridSearchObjectivesWeight

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Property UIContext() As cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As cUIContext)
            MyBase.UIContext = value
            Me.Manager = cCore.GetInstance.FishingPolicyManager
        End Set
    End Property

End Class
