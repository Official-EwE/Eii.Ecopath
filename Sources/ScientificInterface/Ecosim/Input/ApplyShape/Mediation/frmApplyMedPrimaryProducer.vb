#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterface.Other

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecosim 'Apply Mediation to Primary Producer' 
    ''' interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmApplyMedPrimaryProducer
        Inherits frmApplyShapeBase

#Region " Constructor "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.plApplyFFGrid.Controls.Add(Me.Grid())
        End Sub

#End Region ' Constructor

#Region " Event handlers "

        Private Sub tsBtnClearAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles tsBtnClearAll.Click
            Me.ClearAllPairs()
        End Sub

        Private Sub tsBtnSetAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles tsBtnSetAll.Click
            Me.SetAllPairs()
        End Sub

#End Region ' Event handlers

#Region " Mandatory overrides "

        Protected Overrides Function ApplyTargetMode() As eApplyTargetTypes
            Return eApplyTargetTypes.PrimaryProducer
        End Function

        Protected Overrides Function ApplyShapeMode() As eApplyShapeTypes
            Return eApplyShapeTypes.Mediation
        End Function

#End Region ' Mandatory overrides

    End Class

End Namespace
