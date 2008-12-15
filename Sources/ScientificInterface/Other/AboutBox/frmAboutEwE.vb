'==============================================================================
'
' $Log: frmAboutEwE.vb,v $
' Revision 1.2  2008/12/15 15:54:31  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:32:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.7  2008/09/03 13:39:45  jeroens
' Added .NET version indicator
'
' Revision 1.6  2007/10/31 13:14:59  jeroens
' - Removed links from Technical tab
'
' Revision 1.5  2007/10/23 23:15:07  sherman
' Created Modules tab for AboutEwE
'
' Revision 1.4  2007/10/23 18:53:14  sherman
' Centralized EwE Incident reporting to App Launcher
' added Menu Item under help
'
' Revision 1.3  2007/10/21 01:32:03  jeroens
' + Uses credits.rtf
'
' Revision 1.2  2007/10/20 22:56:21  jeroens
' * Revamped: added 3 tabs Generic, Technical, Credits
'
' Revision 1.1  2007/07/08 07:37:23  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System.Reflection
Imports System.Deployment
Imports System.Text
Imports EwEUtils.Utilities

#End Region ' Imports 

Namespace Other
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class frmAboutEwE

        Private Sub AboutEwE_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim an As AssemblyName = Assembly.GetExecutingAssembly().GetName()
            Dim strTitle As String = My.Application.Info.Title
            Dim grid As New AboutEwEGrid()

            ' Format generic page
            Me.Text = String.Format(My.Resources.ABOUT_CAPTION, strTitle)
            Me.lbTitle.Text = strTitle
            Me.lbVersion.Text = String.Format(My.Resources.ABOUT_VALUE_VERSION, an.Version.ToString())
            Me.lbCopyright.Text = String.Format(My.Resources.ABOUT_VALUE_COPYRIGHT, My.Application.Info.Copyright, My.Application.Info.CompanyName)

            ' Format technical page
            Me.pGrid.Controls.Add(grid)
            Me.m_lblNetVersion.Text = String.Format(m_lblNetVersion.Text, System.Environment.Version.ToString())

            ' Format credits page
            Me.rtbCredits.Rtf = My.Resources.credits

            ' Format modules page
            Me.rtbModules.Rtf = My.Resources.modules

        End Sub

        Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
            Me.Close()
        End Sub

    End Class

End Namespace

