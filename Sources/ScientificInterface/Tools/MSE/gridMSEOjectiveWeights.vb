#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.SearchObjectives
Imports EwEUtils.Core

#End Region

''' <summary>
''' gridMSEOjectiveWeights provides a wrapper around gridSearchObjectivesGroup, for the MSE, so it has a constructor with no arguments and can be created by the NavigationPanel.
''' </summary>
''' <remarks></remarks>
<CLSCompliant(False)> _
Public Class gridMSEOjectiveWeights
    : Inherits Ecosim.gridSearchObjectivesWeight

    Public Sub New()
        MyBase.New(cCore.GetInstance.FishingPolicyManager)

    End Sub

End Class
