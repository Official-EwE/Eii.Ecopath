'==============================================================================
'
' $Log: Vulnerabilities.vb,v $
' Revision 1.2  2008/12/15 15:56:03  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:45  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2007/07/08 00:56:52  jeroens
' * Fixed runtime error on missing parameterless constructor
'
' Revision 1.11  2007/07/03 21:25:05  jeroens
' * Reactivated 'Set' in derived grid panels
'
' Revision 1.10  2007/07/03 20:19:15  jeroens
' + Reinstituted Vulnerabilities toolbar
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore

#End Region

Namespace Ecosim

    Public Class Vulnerabilities

        Public Sub New()
            MyBase.New(New VulnerabilitiesEwEGrid)
            InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            plVulGrid.Controls.Add(Me.Grid)
        End Sub

        Private Sub tsbEstimateVs_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles tsbEstimateVs.Click
            ' ToDo: fix bug 229 (http://sources.ecopath.org/trac/Ecopath/ticket/229)
            MsgBox("To be implemented...", MsgBoxStyle.Information Or MsgBoxStyle.OkOnly)
        End Sub

    End Class

End Namespace
