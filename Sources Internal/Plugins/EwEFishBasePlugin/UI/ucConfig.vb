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
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Utilities
Imports EwEUtils.SystemUtilities
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Commands
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for configuring the SAUP taxon table connection.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class ucConfig
    Implements IUIElement

    Private m_ppt As cFishBasePlugin = Nothing
    Private m_ddx As cFishBaseConnection = Nothing
    Private m_bLogOnRequested As Boolean
    Private m_bWaiting As Boolean = False
    Private m_bViewPwdChars As Boolean = False

    Public Sub New(pluginpoint As cFishBasePlugin)
        MyBase.New()
        Me.m_ppt = pluginpoint
        Me.Text = My.Resources.ENGINE_NAME
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.UpdateControls()

        Me.m_tbxAccess.Text = My.Settings.AccessPath
        Me.m_tbxWebServer.Text = My.Settings.WSDLserver
        Me.m_tbxWebPort.Text = My.Settings.WSDLport
        Me.m_tbxWebAccount.Text = My.Settings.WSDLuser
        Me.m_tbxWebPwd.Text = cStringUtils.ToInsecureString(cStringUtils.DecryptString(My.Settings.WSDLpassword))

        Select Case My.Settings.ConnectionType
            Case 0 : Me.m_rbAccess.Checked = True
            Case 1 : Me.m_rbWebService.Checked = True
        End Select

        Me.Connection = Me.m_ppt.Connection
        Me.m_cmbMaxResults.Text = Me.m_ppt.MaxResults.ToString

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)

        ' Store settings
        If Me.m_rbAccess.Checked Then My.Settings.ConnectionType = 0
        If Me.m_rbWebService.Checked Then My.Settings.ConnectionType = 2

        My.Settings.AccessPath = Me.m_tbxAccess.Text
        My.Settings.WSDLserver = Me.m_tbxWebServer.Text
        My.Settings.WSDLport = Me.m_tbxWebPort.Text
        My.Settings.WSDLuser = Me.m_tbxWebAccount.Text
        My.Settings.WSDLpassword = cStringUtils.EncryptString(cStringUtils.ToSecureString(Me.m_tbxWebPwd.Text))
        My.Settings.Save()

        ' Configure ppt
        Me.m_ppt.Connection = Me.Connection
        Me.m_ppt = Nothing
        ' Clean up
        Me.Connection = Nothing

        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Public Property UIContext As cUIContext _
        Implements IUIElement.UIContext

    Private Property Connection As cFishBaseConnection
        Get
            Return Me.m_ddx
        End Get
        Set(value As cFishBaseConnection)
            If (Me.m_ddx IsNot Nothing) Then
                RemoveHandler Me.m_ddx.OnConnected, AddressOf OnConnected
            End If
            Me.m_ddx = value
            If (Me.m_ddx IsNot Nothing) Then
                AddHandler Me.m_ddx.OnConnected, AddressOf OnConnected
            End If
        End Set
    End Property

    Private Delegate Sub CompleteLogOnDelegate(ByVal bLogOn As Boolean)

    Private Sub CompleteLogOn(ByVal bLogOn As Boolean)

        ' Update
        Me.m_bWaiting = False
        Me.UpdateControls()

    End Sub

    Private Sub UpdateControls()

        Dim bWebServicesAvailable As Boolean = False
#If DEBUG Then
        bWebServicesAvailable = True
#End If
        Dim bConnected As Boolean = False
        Dim bUseAccess As Boolean = Me.m_rbAccess.Checked
        Dim bUseWebServices As Boolean = Me.m_rbWebService.Checked And bWebServicesAvailable
        Dim bInputsComplete As Boolean = False

        If (Me.Connection IsNot Nothing) Then
            bConnected = Me.Connection.IsConnected
        End If

        If bUseAccess Then
            bInputsComplete = Not String.IsNullOrWhiteSpace(Me.m_tbxAccess.Text)
        End If

        If bUseWebServices Then
            bInputsComplete = (Not String.IsNullOrWhiteSpace(Me.m_tbxWebServer.Text)) And _
                  (Not String.IsNullOrWhiteSpace(Me.m_tbxWebPort.Text)) And _
                  (Not String.IsNullOrWhiteSpace(Me.m_tbxWebAccount.Text)) And _
                  (Not String.IsNullOrWhiteSpace(Me.m_tbxWebPwd.Text))

        End If

        Me.m_rbAccess.Enabled = Not bConnected
        Me.m_rbWebService.Enabled = bWebServicesAvailable And Not bConnected

        Me.m_tbxAccess.Enabled = bUseAccess And Not bConnected
        Me.m_btnPickAccess.Enabled = bUseAccess And Not bConnected

        Me.m_tbxWebServer.Enabled = bUseWebServices And Not bConnected
        Me.m_tbxWebPort.Enabled = bUseWebServices And Not bConnected
        Me.m_tbxWebAccount.Enabled = bUseWebServices And Not bConnected
        Me.m_tbxWebPwd.Enabled = bUseWebServices And Not bConnected
        Me.m_btnToggleViewChars.Enabled = bUseWebServices

        Me.m_btnConnect.Enabled = (Not bConnected) And bInputsComplete
        Me.m_btnDisconnect.Enabled = bConnected

        If (Me.m_bViewPwdChars And bWebServicesAvailable) Then
            Me.m_tbxWebPwd.PasswordChar = cStringUtils.vbCharNull
            Me.m_btnToggleViewChars.Image = SharedResources.Eye_open
        Else
            Me.m_tbxWebPwd.PasswordChar = Me.UIContext.StyleGuide.PasswordChar()
            Me.m_btnToggleViewChars.Image = SharedResources.Eye_closed
        End If

    End Sub

#Region " Generic controls "

    Private Sub OnSourceSelectionChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbWebService.CheckedChanged, m_rbAccess.CheckedChanged
        Try
            Me.UpdateControls()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnConnect(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnConnect.Click

        If (Me.Connection Is Nothing) Then
            ' Connecting
            If (Me.m_rbAccess.Checked) Then
                Dim fbc As New cFishBaseAccessConnnection(Me.m_ppt)
                If (fbc.Connect(Me.m_tbxAccess.Text)) Then
                    Me.Connection = fbc
                End If
            ElseIf (Me.m_rbWebService.Checked) Then
                Dim fbw As New cFishBaseWebserviceConn(Me.m_ppt)
                If (fbw.Connect(Me.m_tbxWebServer.Text, Convert.ToInt16(Me.m_tbxWebPort.Text), Me.m_tbxWebAccount.Text, Me.m_tbxWebPwd.Text)) Then
                    Me.Connection = fbw
                End If
            End If
        End If
        Me.UpdateControls()
    End Sub

    Private Sub OnDisconnect(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnDisconnect.Click
        If Me.Connection.Disconnect() Then
            Me.Connection = Nothing
        End If
        Me.UpdateControls()
    End Sub

    Private Sub OnConnected(ByVal ddx As cFishBaseConnection, ByVal bConnected As Boolean)
        Me.UpdateControls()
    End Sub

    Private Sub OnAuthenticated(ByVal ddx As cFishBaseConnection, _
                                ByVal bLoggedOn As Boolean, _
                                ByVal bError As Boolean, _
                                ByVal strMessage As String)

        ' Leave message display to the message dispatch in the plug-in point

        Try
            If Me.InvokeRequired Then
                Me.Invoke(New CompleteLogOnDelegate(AddressOf Me.CompleteLogOn), New Object() {bLoggedOn})
            Else
                Me.CompleteLogOn(bLoggedOn)
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub OnPickAccess(sender As System.Object, e As System.EventArgs) _
        Handles m_btnPickAccess.Click

        Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog("Choose Fishbase Access database", Me.m_tbxAccess.Text, "Access database|*.accdb;*.mdb")
        If (ofd.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            Me.m_tbxAccess.Text = ofd.FileName
        End If

    End Sub

    Private Sub OnToggleViewChars(sender As System.Object, e As System.EventArgs) _
        Handles m_btnToggleViewChars.Click
        Me.m_bViewPwdChars = Not Me.m_bViewPwdChars
        Me.UpdateControls()
    End Sub

    Private Sub OnMaxResultsChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbMaxResults.SelectedIndexChanged, m_cmbMaxResults.TextChanged

        Me.m_ppt.MaxResults = Math.Min(1000, Math.Max(10, Integer.Parse(Me.m_cmbMaxResults.Text)))
        Me.UpdateControls()

    End Sub

    Private Sub OnAnyTextFieldChanged(sender As Object, e As System.EventArgs) _
        Handles m_tbxWebServer.TextChanged, m_tbxWebPort.TextChanged, m_tbxWebAccount.TextChanged, m_tbxWebPwd.TextChanged, m_tbxAccess.TextChanged
        Try
            Me.UpdateControls()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnVisitFishBase(sender As Object, e As System.EventArgs) _
        Handles m_bpLogo.Click
        Me.VisitSponsor("http://www.fishbase.org")
    End Sub

    Private Sub VisitBlueBridge(sender As System.Object, e As System.EventArgs) _
        Handles m_pbBlueBridge.Click
        Me.VisitSponsor("http://www.i-marine.eu/Content/eLibrary.aspx?id=786ae7dd-f868-4c19-b611-3500b6697bee&li=0")
    End Sub

#End Region ' Generic controls

    Private Sub VisitSponsor(strURL As String)
        Try
            Dim cmd As cBrowserCommand = CType(Me.UIContext.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            cmd.Invoke(strURL)
        Catch ex As Exception
            cLog.Write(ex, "EwEWormsPlugIn.ViewSponsor(" & strURL & ")")
        End Try
    End Sub

End Class