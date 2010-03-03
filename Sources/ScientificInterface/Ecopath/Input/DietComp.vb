#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports 

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecopath Diet Composition interface.
    ''' </summary>
    ''' =======================================================================
    Public Class DietComp

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub

        Private Sub tsSumtoOneBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles tsSumtoOneBtn.Click
            Me.UIContext.Core.NormalizeDietInput()
        End Sub
    End Class

End Namespace

