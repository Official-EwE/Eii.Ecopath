#Region " Imports "

Option Strict On
Imports System.Reflection
Imports System.Deployment
Imports System.Text
Imports EwEUtils.Utilities
Imports EwECore

#End Region ' Imports 

Namespace Other

    ''' =======================================================================
    ''' <summary>
    ''' EwE about box form.
    ''' </summary>
    ''' =======================================================================
    Public Class frmAboutEwE

        Private m_uic As cUIContext = Nothing

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
            Me.m_uic = uic
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.m_uic Is Nothing) Then Return

            ' Report CORE assembly
            Dim an As AssemblyName = Assembly.GetAssembly(GetType(cCore)).GetName
            Dim strTitle As String = My.Application.Info.Title

            ' Format generic page
            Me.Text = String.Format(My.Resources.ABOUT_CAPTION, strTitle)
            Me.m_lbTitle.Text = strTitle
            Me.m_lbVersion.Text = String.Format(My.Resources.ABOUT_VALUE_VERSION, an.Version.ToString())
            Me.m_lbCopyright.Text = String.Format(My.Resources.ABOUT_VALUE_COPYRIGHT, My.Application.Info.Copyright, My.Application.Info.CompanyName)

            ' Format technical page
            Me.m_gridTechnical.Populate(Me.m_uic)
            Me.m_lblNetVersion.Text = String.Format(m_lblNetVersion.Text, System.Environment.Version.ToString())

            ' Format credits page
            Me.m_rtbCredits.Rtf = My.Resources.credits

            ' Format modules page
            Me.m_rtbModules.Rtf = My.Resources.modules

        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOK.Click
            Me.Close()
        End Sub

    End Class

End Namespace

