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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

' JS 09Oct18: Traditionally, the embedded web view control uses the Internet Explorer engine.
'             It will soon be necessary to switch to a newer engine where available. 
'             See https : //blogs.windows.com/msedgedev/2018/05/09/modern-webview-winforms-wpf-apps/
' JS 30Jan19: Perhaps it's just silly to keep the nested browser windows around.
'             We might as well always launch links in an external browser? Let's do a user poll

''' ===========================================================================
''' <summary>
''' Form class showing a browser window and a mini-toolbar for navigation.
''' </summary>
''' ===========================================================================
Public Class frmStartPanel

    Private m_strURL As String = ""

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="uic">UI context to link to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(uic As cUIContext)
        MyBase.New()
        Me.InitializeComponent()
        Me.UIContext = uic
        Me.TabText = Me.Text
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
             Return Me.m_strURL
        End Get
        Set(strURL As String)
            Try
                If String.IsNullOrWhiteSpace(strURL) Then
                    Dim link As New cWebLinks(Me.UIContext.Core)
                    Dim ver As String = cCore.Version(False)
                    If (String.Compare(ver, My.Settings.LastRunVersion, True) <> 0) Then
                        strURL = link.GetURL(cWebLinks.eLinkType.PostInstall)
                        My.Settings.LastRunVersion = ver
                        My.Settings.Save()
                    Else
                        strURL = link.GetURL(cWebLinks.eLinkType.Start)
                    End If
                End If

                If (strURL <> Me.m_strURL) Then
                    Me.m_strURL = strURL
                    Me.m_browser.Navigate(strURL)
                End If
            Catch ex As Exception
                cLog.Write(ex)
            End Try
        End Set
    End Property

#End Region ' Public acess

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

#If BETA = 1 Then
        Me.m_tsbnBetaFeedback.Visible = True
#Else
        Me.m_tsbnBetaFeedback.Visible = False
#End If
        Me.m_tsbnStartPage.Image = SharedResources.HomeHS
        Me.m_tsbnBack.Image = SharedResources.Back
        Me.m_tsbnForward.Image = SharedResources.forward
        Me.m_tsbnRefresh.Image = SharedResources.ResetHS
        Me.m_tsbnEcopathSite.Image = SharedResources.Ecopath_32x32
        Me.m_tsbnEcobase.Image = SharedResources.ecobase
        Me.m_tsbnBugTracker.Image = SharedResources.bug
        Me.m_tsbnBetaFeedback.Image = My.Resources.logo_sm

        AddHandler Me.m_browser.NavigationCompleted, AddressOf Me.OnUpdateNav

        Me.Icon = Icon.FromHandle(ScientificInterfaceShared.My.Resources.HomeHS.GetHicon)

        ' Navigate to current URL
        Me.URL = Me.URL
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        Me.Icon.Dispose()

        RemoveHandler Me.m_browser.NavigationCompleted, AddressOf Me.OnUpdateNav

        MyBase.OnFormClosed(e)

    End Sub

    Public Overrides Function PanelType() As frmEwEDockContent.ePanelType
        Return ePanelType.SystemPanel
    End Function

#End Region ' Form overrides

#Region " Events "

    Private Sub OnBrowserNavBack(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnBack.Click
        Try
            Me.m_browser.GoBack()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnBrowserNavForward(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnForward.Click
        Try
            Me.m_browser.GoForward()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnBrowserRefresh(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnRefresh.Click
        Try
            Me.m_browser.Refresh()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnBrowserStart(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnStartPage.Click
        Try
            Me.Browse(cWebLinks.eLinkType.Start)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnBrowserFacebook(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnFacebook.Click
        Try
            Me.Browse(cWebLinks.eLinkType.Facebook)
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Private Sub OnUpdateNav(sender As Object, e As EventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(New UpdateControlsDelegate(AddressOf Me.UpdateControls))
        Else
            Me.UpdateControls()
        End If
    End Sub

    'Private Sub OnViewRSS(sender As System.Object, e As System.EventArgs) _
    '    Handles m_tsbnRSS.Click
    '    Try
    '        Me.Browse(cWebLinks.eLinkType.HomeRSS)
    '    Catch ex As Exception
    '        cLog.Write(ex)
    '    End Try
    'End Sub

    Private Sub OnGoHome(sender As System.Object, e As System.EventArgs) Handles m_tsbnEcopathSite.Click
        Try
            Me.Browse(cWebLinks.eLinkType.Home)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnVisitEcobase(sender As System.Object, e As System.EventArgs) Handles m_tsbnEcobase.Click
        Try
            Me.Browse(cWebLinks.eLinkType.EcoBase)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnVisitTrac(sender As System.Object, e As System.EventArgs) Handles m_tsbnBugTracker.Click
        Try
            Me.Browse(cWebLinks.eLinkType.Trac)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnVisitSurvey(sender As System.Object, e As System.EventArgs) Handles m_tsbnBetaFeedback.Click
        Try
            Me.Browse(cWebLinks.eLinkType.Feedback)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnBrowserNavigating(sender As Object, e As System.Windows.Forms.WebBrowserNavigatingEventArgs) _


        ' Overridden to intercept ewe-ecobase clicks
        Try
            Dim url As String = e.Url.ToString()
            If (url.ToLower().StartsWith("ewe-ecobase")) Then
                e.Cancel = True
                Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
                Dim cmd As cBrowserCommand = CType(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
                cmd.Invoke(url)
            End If
        Catch ex As Exception

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

    Protected Sub Browse(link As cWebLinks.eLinkType)

        If (Me.UIContext Is Nothing) Then Return

        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)

        If (cmd Is Nothing) Then Return

        cmd.Invoke(link)

    End Sub

#End Region ' Internals

End Class
