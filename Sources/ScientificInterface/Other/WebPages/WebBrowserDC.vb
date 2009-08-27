'==============================================================================
'
' $Log: WebBrowserDC.vb,v $
' Revision 1.2  2008/11/27 19:45:51  jeroens
' Renamed ApplicationComponents interfaces to more properly reflect their function
'
' Revision 1.1  2008/09/26 07:32:11  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.13  2008/05/22 17:31:49  jeroens
' Added command-line support for opening a model
'
' Revision 1.12  2007/11/06 14:17:36  sherman
' Jereon fixed disposing of form and objects within it
'
' Revision 1.11  2007/10/10 19:47:01  jeroens
' * URL includes names of loaded plugins
'
' Revision 1.10  2007/09/27 18:04:23  jeroens
' + Uses ApplicationComponents
'
' Revision 1.9  2007/09/27 17:02:21  jeroens
' * Oooh! Pretty header!
'
'==============================================================================

Option Strict On

Imports EwECore
Imports System.IO
Imports System.Reflection
Imports EwEUtils.Utilities
Imports EwEPlugin

Public Class WebBrowserDC

    Private Shared cBASEURL As String = "http://www.ecopath.org/nonewe/eweexe/index.php"

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Setting the label
        Me.Text = My.Resources.GENERIC_LABEL_HOME
        Me.TabText = My.Resources.GENERIC_LABEL_HOME
        ' Navigate to default URL
        Me.URL = ""

    End Sub

    Private Sub OnNavigating(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs) Handles m_browser.Navigating

        Dim appl As AppLauncher = AppLauncher.GetInstance()

        If Path.GetExtension(e.Url.AbsolutePath) = ".eii" Then
            appl.LoadEcopathModel(e.Url.AbsolutePath)
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
        Dim core As cCore = cCore.GetInstance()
        Dim ac As ApplicationComponents = AppLauncher.GetInstance().ApplicationComponents()
        Dim aAssemblyNames As AssemblyName() = ac.RequiredComponents()
        Dim pm As cPluginManager = core.PluginManager
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

    Private Sub WebBrowserDC_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Me.m_browser = Nothing
    End Sub

    Private Sub WebBrowserDC_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '
    End Sub
End Class
