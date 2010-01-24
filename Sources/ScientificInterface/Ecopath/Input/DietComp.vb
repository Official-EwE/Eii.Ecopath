'==============================================================================
'
' $Log: DietComp.vb,v $
' Revision 1.1  2008/09/26 07:31:31  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2007/07/04 17:08:26  jeroens
' * Fixed runtime error on parameterless constructor
'
' Revision 1.3  2007/07/03 21:25:05  jeroens
' * Reactivated 'Set' in derived grid panels
'
' Revision 1.2  2006/10/15 02:56:16  jeroens
' + Hooked up 'Sum to one' button
'
'==============================================================================

Option Strict On

Imports EwECore

Namespace Ecopath.Input

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class DietComp

        Public Sub New()
            MyBase.New(New DietCompositionEwEGrid)
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            plDietCompGrid.Controls.Add(Me.Grid)
        End Sub

        Private Sub tsSumtoOneBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles tsSumtoOneBtn.Click
            cCore.GetInstance().NormalizeDietInput()
        End Sub
    End Class

End Namespace

