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
            Me.New("")
        End Sub

        Public Sub New(ByVal strText As String)
            MyBase.New(strText, New DietCompositionEwEGrid)
            InitializeComponent()
        End Sub

        Private Sub DietComp_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            plDietCompGrid.Controls.Add(Me.Grid)
        End Sub

        Private Sub tsSumtoOneBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsSumtoOneBtn.Click
            cCore.GetInstance().NormalizeDietInput()
        End Sub
    End Class

End Namespace

