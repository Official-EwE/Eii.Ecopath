#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports 

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the MSE Quota Shared interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmQuotaShare
        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Private Sub OnSumSharesToOne(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsSumtoOneBtn.Click
            Me.Core.NormalizeQuotaShare()
        End Sub
    End Class

End Namespace
