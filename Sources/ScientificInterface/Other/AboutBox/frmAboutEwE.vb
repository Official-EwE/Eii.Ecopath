#Region " Imports "

Option Strict On
Imports System.Reflection
Imports System.Deployment
Imports System.Text
Imports EwEUtils.Utilities

#End Region ' Imports 

Namespace Other

    ''' =======================================================================
    ''' <summary>
    ''' EwE about box form.
    ''' </summary>
    ''' =======================================================================
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

