#Region " Imports "

Option Strict On

Imports EwECore
Imports System.IO
Imports System.Reflection
Imports EwEUtils.Utilities
Imports EwEPlugin

#End Region ' Imports

Public Class WebBrowserDC

    Private Shared cBASEURL As String = "http://www.ecopath.org/nonewe/eweexe/index.php"
    Private m_uic As cUIContext = Nothing

    Public Sub New(ByVal uic As cUIContext)

        Me.InitializeComponent()

        Me.m_uic = uic

        Me.Text = My.Resources.GENERIC_LABEL_HOME
        Me.TabText = My.Resources.GENERIC_LABEL_HOME
        Me.URL = ""

    End Sub

    Private Sub OnNavigating(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs) _
        Handles m_browser.Navigating

        Dim appl As AppLauncher = AppLauncher.GetInstance()

        If Path.GetExtension(e.Url.AbsolutePath) = ".eii" Then
            appl.LoadEcopathModel(e.Url.AbsolutePath, AppLauncher.eLoadSourceType.User)
            ' Navigate to default URL
            Me.URL = ""
        End If

    End Sub

    Public Property URL() As String
        Get
            Return m_browser.Url.AbsolutePath
        End Get
        Set(ByVal value As String)
            If String.IsNullOrEmpty(value) Then
                value = Me.EwEBaseURL()
            End If
            m_browser.Navigate(value)
        End Set
    End Property

    Private Function EwEBaseURL() As String

        Dim ac As ApplicationComponents = AppLauncher.GetInstance().ApplicationComponents()
        Dim aAssemblyNames As AssemblyName() = ac.RequiredComponents()
        Dim pm As cPluginManager = Me.m_uic.Core.PluginManager
        Dim ub As New EwEUtils.Utilities.UrlBuilder(cBASEURL)

        For Each an As AssemblyName In aAssemblyNames
            If Not ub.QueryString.ContainsKey(an.Name) Then ub.QueryString(an.Name) = an.Version.ToString
        Next an

        If (Not Object.ReferenceEquals(pm, Nothing)) Then
            aAssemblyNames = pm.PluginAssemblyNames
            For Each an As AssemblyName In aAssemblyNames
                If Not ub.QueryString.ContainsKey(an.Name) Then ub.QueryString(an.Name) = an.Version.ToString
            Next an
        End If

        Return ub.ToString()
    End Function

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        Me.m_browser = Nothing
        MyBase.OnFormClosed(e)
    End Sub

End Class
