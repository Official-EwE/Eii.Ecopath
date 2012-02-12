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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Threading
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwECore

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Form, implements the interface that triggers a component update
''' </summary>
''' <remarks>
''' This form will start updating components automatically once shown, and 
''' will close automatically when all components have been verified and updated.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class frmUpdateComponents

#Region " Private vars "

    ''' <summary>The plug-in manager used to updates components.</summary>
    Private m_pm As cPluginManager = Nothing
    ''' <summary>The ui context to operate on.</summary>
    Private m_uic As cUIContext = Nothing
    ''' <summary>The update thread.</summary>
    Private m_thrd As Thread = Nothing

    ''' <summary>Overall update result status.</summary>
    Private m_bSuccess As Boolean = True
    ''' <summary>The update result message.</summary>
    Private m_lvs As New List(Of cVariableStatus)

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor; initializes a new instance of the update form.
    ''' </summary>
    ''' <param name="pm">The plug-in manager used to updates components.</param>
    ''' <param name="uic">The ui context to operate on.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext, ByVal pm As cPluginManager)
        Me.InitializeComponent()
        Me.m_pm = pm
        Me.m_uic = uic
    End Sub

#End Region ' Constructor

#Region " Framework overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        MyBase.OnLoad(e)

        ' Start listening to update events
        AddHandler Me.m_pm.AssemblyUpdating, AddressOf OnAssemblyUpdating
        AddHandler Me.m_pm.AssemblyUpdated, AddressOf OnAssemblyUpdated

        ' Set initial message
        Me.UpdateControls("", 0)

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        ' Stop listening to update events
        RemoveHandler Me.m_pm.AssemblyUpdating, AddressOf OnAssemblyUpdating
        RemoveHandler Me.m_pm.AssemblyUpdated, AddressOf OnAssemblyUpdated

        ' Send summary message
        Me.SendSummaryMessage()

        ' Done
        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub OnShown(ByVal e As System.EventArgs)
        MyBase.OnShown(e)

        ' Kick off update process
        If (Me.m_thrd Is Nothing) Then
            Me.m_thrd = New Thread(AddressOf UpdatePluginsThread)
            Me.m_thrd.Start()
        End If

    End Sub

#End Region ' Framework overrides

#Region " Events "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Abort button has been clicked.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnAbort(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAbort.Click
        If Me.m_thrd IsNot Nothing Then
            Try
                Me.m_thrd.Abort()
            Catch ex As Exception

            End Try
            Me.Close()
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly update event handler.
    ''' </summary>
    ''' <param name="strName">Name of the component that is updated.</param>
    ''' <param name="sProgress">Update progress [0, 1]</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnAssemblyUpdating(ByVal strName As String, ByVal sProgress As Single)

        If Me.InvokeRequired Then
            Me.Invoke(New UpdateControlsDelegate(AddressOf UpdateControls), New Object() {strName, sProgress})
        Else
            Me.UpdateControls(strName, sProgress)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly updated event handler.
    ''' </summary>
    ''' <param name="strName">Name of the component that is updated.</param>
    ''' <param name="result">Updateresult.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnAssemblyUpdated(ByVal strName As String, ByVal result As eAutoUpdateResultTypes)

        If Me.InvokeRequired Then
            Me.Invoke(New UpdateStatusDelegate(AddressOf UpdateStatus), New Object() {strName, result})
        Else
            Me.UpdateStatus(strName, result)
        End If

    End Sub

#End Region ' Events

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delegate to marshall updates from the update thread to the form.
    ''' </summary>
    ''' <param name="strName">Name of the component that is updated.</param>
    ''' <param name="sProgress">Update progress [0, 1]</param>
    ''' -----------------------------------------------------------------------
    Private Delegate Sub UpdateControlsDelegate(ByVal strName As String, ByVal sProgress As Single)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reflect updates from the update thread to the controls in the form.
    ''' </summary>
    ''' <param name="strName">Name of the component that is updated.</param>
    ''' <param name="sProgress">Update progress [0, 1]</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls(ByVal strName As String, ByVal sProgress As Single)

        If String.IsNullOrEmpty(strName) Then
            Me.m_lblInfo.Text = My.Resources.STATUS_UPDATE_CHECKING
        Else
            Me.m_lblInfo.Text = String.Format(My.Resources.STATUS_UPDATE_DOWNLOADING, strName)
        End If
        Me.m_pbProgress.Value = CInt(100 * sProgress)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delegate to marshall updates from the update thread to the form.
    ''' </summary>
    ''' <param name="strName">Name of the component that is updated.</param>
    ''' <param name="result">Update <see cref="eAutoUpdateResultTypes">result</see>.</param>
    ''' -----------------------------------------------------------------------
    Private Delegate Sub UpdateStatusDelegate(ByVal strName As String, ByVal result As eAutoUpdateResultTypes)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reflect updates from the update thread to the controls in the form.
    ''' </summary>
    ''' <param name="strName">Name of the component that is updated.</param>
    ''' <param name="result">Update <see cref="eAutoUpdateResultTypes">result</see>.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateStatus(ByVal strName As String, ByVal result As eAutoUpdateResultTypes)

        Dim vs As cVariableStatus = Nothing

        Select Case result

            Case eAutoUpdateResultTypes.Error_Connection
                If Me.m_lvs.Count = 0 Then
                    vs = New cVariableStatus(eStatusFlags.NotEditable, _
                                             My.Resources.STATUS_UPDATE_ERROR_CONNECTION, _
                                             eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
                End If
            Case eAutoUpdateResultTypes.Error_Download
                vs = New cVariableStatus(eStatusFlags.NotEditable, _
                                        String.Format(My.Resources.STATUS_UPDATE_ERROR_DOWNLOAD, strName), _
                                        eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
            Case eAutoUpdateResultTypes.Error_Generic
                vs = New cVariableStatus(eStatusFlags.NotEditable, _
                                        String.Format(My.Resources.STATUS_UPDATE_ERROR_GENERIC, strName), _
                                        eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
            Case eAutoUpdateResultTypes.Error_Replace
                vs = New cVariableStatus(eStatusFlags.NotEditable, _
                                         String.Format(My.Resources.STATUS_UPDATE_ERROR_WRITE, strName), _
                                         eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
            Case eAutoUpdateResultTypes.Success_NoActionRequired
                vs = New cVariableStatus(eStatusFlags.NotEditable, _
                                        String.Format(My.Resources.STATUS_UPDATE_NO_ACTION, strName), _
                                        eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
            Case eAutoUpdateResultTypes.Success_Updated
                vs = New cVariableStatus(eStatusFlags.NotEditable, _
                                        String.Format(".", strName), _
                                        eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)

        End Select
        If (vs IsNot Nothing) Then Me.m_lvs.Add(vs)

    End Sub

    Private Sub SendSummaryMessage()

        Dim msg As cMessage = Nothing

        ' Create final message
        If (Me.m_bSuccess) Then
            msg = New cMessage(My.Resources.STATUS_UPDATE_SUCCESS, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)
        Else
            msg = New cMessage(My.Resources.STATUS_UPDATE_SUCCESS, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Warning)
        End If
        For Each vs As cVariableStatus In Me.m_lvs
            msg.AddVariable(vs)
        Next

        Me.m_uic.Core.Messages.SendMessage(msg)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delegate to marshall a close request to the form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Delegate Sub CloseDelegate()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delegate to marshall an overwrite prompt request to the form.
    ''' </summary>
    ''' <param name="strPlugin">The plug-in to overwrite.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Delegate Function OverwritePromptDelegate(ByVal strPlugin As String) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, implements a plug-in overwrite prompt.
    ''' </summary>
    ''' <param name="strPlugin"></param>
    ''' <returns>True if </returns>
    ''' -----------------------------------------------------------------------
    Private Function OverwritePrompt(ByVal strPlugin As String) As Boolean

        Dim strPrompt As String = String.Format(My.Resources.PROMPT_UPDATE_MIGRATION, strPlugin)
        Dim bCheck As Boolean = False
        Dim bOverwrite As Boolean = False

        If Not SuppressPrompt(strPlugin) Then
            bOverwrite = cCustomMessageBox.Show(strPrompt, Me.Text, _
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                                bCheck, My.Resources.PROMPT_UPDATE_MIGRATION_SUPPRESS) = Windows.Forms.DialogResult.Yes
            If bCheck Then SuppressPrompt(strPlugin) = True
        End If
        Return bOverwrite

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a plug-in overwrite prompt should be suppressed.
    ''' </summary>
    ''' <param name="strPlugin"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Property SuppressPrompt(ByVal strPlugin As String) As Boolean
        Get
            If String.IsNullOrEmpty(My.Settings.SuppressedOverwritePrompts) Then Return False
            Dim astrSuppressed() As String = My.Settings.SuppressedOverwritePrompts.Split(","c)
            For Each str As String In astrSuppressed
                If (String.Compare(str.Trim, strPlugin.Trim, True) = 0) Then Return True
            Next
            Return False
        End Get
        Set(ByVal value As Boolean)
            If Not Me.SuppressPrompt(strPlugin) Then
                My.Settings.SuppressedOverwritePrompts &= (strPlugin & ",")
            End If
        End Set
    End Property

#End Region ' Internals

#Region " Update thread "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Thread procedure to run updates.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdatePluginsThread()
        ' Run updates
        Me.m_pm.UpdatePlugins(New cPluginManager.OnConfirmOverwrite(AddressOf OverwriteConfirmCallback))
        ' Done, close form
        Me.Invoke(New CloseDelegate(AddressOf Me.Close))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Updater plugg-in overwrite callback.
    ''' </summary>
    ''' <param name="strPlugin">The plug-in to overwrite.</param>
    ''' <returns>True if allowed to overwrite.</returns>
    ''' -----------------------------------------------------------------------
    Private Function OverwriteConfirmCallback(ByVal strPlugin As String) As Boolean
        Return CBool(Me.Invoke(New OverwritePromptDelegate(AddressOf OverwritePrompt), New Object() {strPlugin}))
    End Function

#End Region ' Update thread

End Class

