'==============================================================================
'
' $Log: frmApplyFFConsumer.vb,v $
' Revision 1.1  2008/12/15 19:54:05  jeroens
' *** empty log message ***
'
' Revision 1.2  2008/12/15 15:58:58  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/06/02 00:01:40  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.1  2008/05/23 15:44:24  jeroens
' Moved
'
' Revision 1.1  2008/01/22 02:41:41  jeroens
' Properly fixed grid apply mode
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterface.Other

#End Region

Namespace Ecosim

    Public Class frmApplyFFConsumer
        Inherits frmApplyShapeBase

#Region "Constructor"

        Public Sub New()
            MyBase.New()
            InitializeComponent()
            plApplyFFGrid.Controls.Add(Me.Grid())
        End Sub

        Public Sub New(ByVal text As String)

            Me.New()
            'Set tab text
            Me.TabText = text
            'Set window text
            Me.Text = text

        End Sub

#End Region

#Region "Event handlers"

        Private Sub tsBtnClearAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnClearAll.Click
            Me.ClearAllPairs()
        End Sub

        Private Sub tsBtnSetAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnSetAll.Click
            Me.SetAllPairs()
        End Sub

#End Region ' Event handlers

#Region " Mandatory overrides "

        Protected Overrides Function ApplyTargetMode() As eApplyTargetTypes
            Return eApplyTargetTypes.Consumer
        End Function

        Protected Overrides Function ApplyShapeMode() As eApplyShapeTypes
            Return eApplyShapeTypes.Forcing
        End Function

#End Region ' Mandatory overrides

    End Class

End Namespace
