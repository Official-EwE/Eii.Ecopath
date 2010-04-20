#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the MSE Group Reference Levels interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmGroupRefLevels

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Private Sub OnReset(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbnReset.Click
            Me.Core.ResetMSEGroupRefLevels()
        End Sub

    End Class

End Namespace
