#Region " Imports "

Option Strict On

Imports EwECore
Imports System.IO
Imports System.Reflection
Imports EwEUtils.Utilities
Imports EwEPlugin

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Form class showing a browser window and a mini-toolbar for navigation.
''' </summary>
''' ===========================================================================
Public Class frmWebBrowser

#Region " Private vars "

    Private Const cBASEURL As String = "http://www.ecopath.org/nonewe/eweexe/index.php"
    Private m_uic As cUIContext = Nothing

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="uic">UI context to link to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext)

        Me.InitializeComponent()

        Me.m_uic = uic

        Me.Text = My.Resources.GENERIC_LABEL_HOME
        Me.TabText = My.Resources.GENERIC_LABEL_HOME

    End Sub

#End Region ' Constructor

#Region " Public acess "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the URL that the web browser is currently displaying.
    ''' </summary>
    ''' <remarks>
    ''' If left emtpy, the browser will navigate to the EwE start page.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Property URL() As String
        Get
            Return Me.m_browser.Url.AbsolutePath
        End Get
        Set(ByVal strURL As String)
            If String.IsNullOrEmpty(strURL) Then
                strURL = Me.EwEBaseURL()
            End If
            Me.m_browser.Navigate(strURL)
        End Set
    End Property

#End Region ' Public acess

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        AddHandler Me.m_browser.CanGoBackChanged, AddressOf OnUpdateNav
        AddHandler Me.m_browser.CanGoForwardChanged, AddressOf OnUpdateNav

        ' Start navigating
        Me.URL = ""
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_browser.CanGoBackChanged, AddressOf OnUpdateNav
        RemoveHandler Me.m_browser.CanGoForwardChanged, AddressOf OnUpdateNav

        Me.m_browser = Nothing
        MyBase.OnFormClosed(e)

    End Sub

#End Region ' Form overrides

#Region " Events "

    Private Sub OnBrowserNavBack(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnBack.Click
        Me.m_browser.GoBack()
    End Sub

    Private Sub OnBrowserNavForward(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnForward.Click
        Me.m_browser.GoForward()
    End Sub

    Private Sub OnBrowserRefresh(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnRefresh.Click
        Me.m_browser.Refresh()
    End Sub

    Private Sub OnBrowserHome(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnHome.Click
        Me.m_browser.Navigate(Me.EwEBaseURL)
    End Sub

    Private Sub OnUpdateNav(ByVal sender As Object, ByVal e As EventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(New UpdateControlsDelegate(AddressOf UpdateControls))
        Else
            Me.UpdateControls()
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Undead method, allows direct opening of models from within the browser. Cool stuff,
    ''' should definitely be re-activated, but not now...
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnNavigating(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs)

        Dim appl As AppLauncher = AppLauncher.GetInstance()

        If Path.GetExtension(e.Url.AbsolutePath) = ".eii" Then
            appl.LoadEcopathModel(e.Url.AbsolutePath, AppLauncher.eLoadSourceType.User)
            ' Navigate to default URL
            Me.URL = ""
        End If

    End Sub

#End Region ' Events

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delegate for marshalling browser events.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Delegate Sub UpdateControlsDelegate()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update control states in the form
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()

        Me.m_tsbnBack.Enabled = (Me.m_browser.CanGoBack)
        Me.m_tsbnForward.Enabled = (Me.m_browser.CanGoForward)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Conjure the EwE base URL for invoking the EwE start page, including
    ''' version check.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function EwEBaseURL() As String

        Dim aAssemblyNames As AssemblyName() = cAssemblyUtils.GetSummary(Assembly.GetExecutingAssembly)
        Dim pm As cPluginManager = Me.m_uic.Core.PluginManager
        Dim ub As New UrlBuilder(cBASEURL)

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

#End Region ' Internals

End Class
