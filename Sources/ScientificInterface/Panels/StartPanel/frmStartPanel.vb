' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports System.Reflection
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Forms
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Form class showing a browser window and a mini-toolbar for navigation.
''' </summary>
''' ===========================================================================
Public Class frmStartPanel

#Region " Private vars "

    Private Const cBASEURL As String = "http://www.ecopath.org/nonewe/eweexe/index.php"

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="uic">UI context to link to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext)
        MyBase.New()
        Me.InitializeComponent()
        Me.UIContext = uic
        Me.Text = SharedResources.GENERIC_LABEL_HOME
        Me.TabText = SharedResources.GENERIC_LABEL_HOME
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
            Try
                Return Me.m_browser.Url.AbsolutePath
            Catch ex As Exception
                cLog.Write(ex)
            End Try
            Return Me.EwEBaseURL()
        End Get
        Set(ByVal strURL As String)
            If String.IsNullOrEmpty(strURL) Then
                strURL = Me.EwEBaseURL()
            End If
            Try
                Me.m_browser.Navigate(strURL)
            Catch ex As Exception
                cLog.Write(ex)
            End Try
        End Set
    End Property

#End Region ' Public acess

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        AddHandler Me.m_browser.CanGoBackChanged, AddressOf OnUpdateNav
        AddHandler Me.m_browser.CanGoForwardChanged, AddressOf OnUpdateNav

        ' Navigate to default URL
        Me.URL = ""
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_browser.CanGoBackChanged, AddressOf OnUpdateNav
        RemoveHandler Me.m_browser.CanGoForwardChanged, AddressOf OnUpdateNav

        MyBase.OnFormClosed(e)

    End Sub

    Public Overrides Function PanelType() As frmEwEDockContent.ePanelType
        Return ePanelType.SystemPanel
    End Function

#End Region ' Form overrides

#Region " Events "

    Private Sub OnBrowserNavBack(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnBack.Click
        Try
            Me.m_browser.GoBack()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnBrowserNavForward(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnForward.Click
        Try
            Me.m_browser.GoForward()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnBrowserRefresh(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnRefresh.Click
        Try
            Me.m_browser.Refresh()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnBrowserHome(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnHome.Click
        Try
            Me.m_browser.Navigate(Me.EwEBaseURL)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnBrowserFacebook(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnFacebook.Click
        Try
            Me.m_browser.Navigate("http://www.facebook.com/eweconsortium")
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Private Sub OnUpdateNav(ByVal sender As Object, ByVal e As EventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(New UpdateControlsDelegate(AddressOf UpdateControls))
        Else
            Me.UpdateControls()
        End If
    End Sub

    Private Sub OnViewRSS(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnRSS.Click
        Try
            Me.m_browser.Navigate("http://www.ecopath.org/aggregator/categories/1")
        Catch ex As Exception
            cLog.Write(ex)
        End Try
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
    Protected Overrides Sub UpdateControls()

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

        Dim aAssemblyNames As AssemblyName() = cAssemblyUtils.GetSummary()
        Dim pm As cPluginManager = Me.Core.PluginManager
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
