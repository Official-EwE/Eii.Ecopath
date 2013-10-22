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
Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Interop
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Form to test the remote execution of R scripts.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmInvokeR

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_dad As cEwEtoRPluginPoint
    Private m_fpR As cEwEFormatProvider
    Private m_fpScript As cEwEFormatProvider
    Private m_fpSCOR As cEwEFormatProvider
    Private m_bInUpdate As Boolean = True

#End Region ' Private vars

    Public Sub New(uic As cUIContext, dad As cEwEtoRPluginPoint)

        Me.InitializeComponent()
        Me.m_uic = uic
        Me.m_dad = dad

    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.Icon = Drawing.Icon.FromHandle(My.Resources.Rlogo_5.GetHicon)

        My.Settings.Reload()

        Me.m_tbxR.Text = My.Settings.RPath
        Me.m_tbxScript.Text = My.Settings.RScript
        Me.m_tbxPlaceholder.Text = My.Settings.RScriptFilePlaceholder
        Me.m_tbxSCOR.Text = My.Settings.SCORFile

        Me.m_fpR = New cEwEFormatProvider(Me.m_uic, Me.m_tbxR, GetType(String))
        Me.m_fpScript = New cEwEFormatProvider(Me.m_uic, Me.m_tbxScript, GetType(String))
        Me.m_fpSCOR = New cEwEFormatProvider(Me.m_uic, Me.m_tbxSCOR, GetType(String))

        Me.m_bInUpdate = False

        Me.UpdateControls()

        Me.CenterToScreen()

    End Sub

    Protected Overrides Sub OnClosed(e As System.EventArgs)

        Me.m_fpR.Release()
        Me.m_fpScript.Release()
        Me.m_fpSCOR.Release()

        MyBase.OnClosed(e)

    End Sub

#End Region ' Overrides

#Region " Events "

    Private Sub OnChooseR_Click(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseR.Click

        Dim cmd As cFileOpenCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
        cmd.Invoke(Me.m_tbxR.Text, My.Resources.FILEFILTER_R_EXE, 0, My.Resources.PROMPT_SELECT_R_EXE)
        If (cmd.Result = Windows.Forms.DialogResult.OK) Then
            Me.m_tbxR.Text = cmd.FileName
        End If

    End Sub

    Private Sub OnChooseScript_Click(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseScript.Click

        Dim cmd As cFileOpenCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
        cmd.Invoke(Me.m_tbxScript.Text, My.Resources.FILEFILTER_R_SCRIPT, 0, My.Resources.PROMPT_SELECT_R_SCRIPT
                   )
        If (cmd.Result = Windows.Forms.DialogResult.OK) Then
            Me.m_tbxScript.Text = cmd.FileName
        End If

    End Sub

    Private Sub OnChooseSCOR_Click(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseSCOR.Click

        Dim cmd As cFileSaveCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
        cmd.Invoke(Me.m_tbxSCOR.Text, My.Resources.FILFILTER_SCOR, 0, My.Resources.PROMPT_SELECT_SCOR_FILE)
        If (cmd.Result = Windows.Forms.DialogResult.OK) Then
            Me.m_tbxSCOR.Text = cmd.FileName
        End If

    End Sub

    Private Sub OnRun(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOK.Click

        Cursor = Cursors.WaitCursor
        Try
            Me.RunR()
        Catch ex As Exception
            ' Whoah
            Debug.Assert(False, ex.Message)
        End Try
        Cursor = Cursors.Default

    End Sub

    Private Sub OnInputText(sender As System.Object, e As System.EventArgs) _
        Handles m_tbxR.TextChanged, m_tbxPlaceholder.TextChanged, m_tbxSCOR.TextChanged, m_tbxScript.TextChanged
        Me.UpdateControls()
    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub UpdateControls()

        If Me.m_bInUpdate Then Return

        Dim bHasUIC As Boolean = (Me.m_uic IsNot Nothing)
        Dim bHasR As Boolean = File.Exists(Me.m_tbxR.Text)
        Dim bHasScript As Boolean = File.Exists(Me.m_tbxScript.Text)
        Dim bHasSCOR As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxSCOR.Text)
        Dim style As cStyleGuide.eStyleFlags

        style = cStyleGuide.eStyleFlags.OK
        If Not bHasR Then style = cStyleGuide.eStyleFlags.FailedValidation
        Me.m_fpR.Style = style

        style = cStyleGuide.eStyleFlags.OK
        If Not bHasScript Then style = cStyleGuide.eStyleFlags.FailedValidation
        Me.m_fpScript.Style = style

        style = cStyleGuide.eStyleFlags.OK
        If Not bHasSCOR Then style = cStyleGuide.eStyleFlags.FailedValidation
        Me.m_fpSCOR.Style = style

        Me.m_btnChooseR.Enabled = bHasUIC
        Me.m_btnChooseSCOR.Enabled = bHasUIC
        Me.m_btnChooseScript.Enabled = bHasUIC

        Me.m_btnOK.Enabled = (bHasR And bHasScript And bHasSCOR)

    End Sub

    Private Sub RunR()

        ' Save settings
        My.Settings.RPath = Me.m_tbxR.Text
        My.Settings.RScript = Me.m_tbxScript.Text
        My.Settings.RScriptFilePlaceholder = Me.m_tbxPlaceholder.Text
        My.Settings.SCORFile = Me.m_tbxSCOR.Text
        My.Settings.Save()

        Me.UpdateIcons()

        ' Run Ecopath
        Dim sm As cCoreStateMonitor = Me.m_uic.Core.StateMonitor
        If Not sm.HasEcopathRan Then
            If Not Me.m_uic.Core.RunEcoPath() Then Return
        End If

        Debug.Assert(Me.m_dad.m_epData IsNot Nothing)

        Dim writer As New cSCORWriter(Me.m_dad.m_epData)
        If writer.Write(Me.m_tbxSCOR.Text) Then
            Dim msg As New cMessage(String.Format(My.Resources.STATUS_SCOR_SAVED_SUCCESS, Me.m_tbxSCOR.Text), _
                                    eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.Plugin, eMessageImportance.Information)
            msg.Hyperlink = Path.GetDirectoryName(Me.m_tbxSCOR.Text)
            Me.m_uic.Core.Messages.SendMessage(msg)
        Else
            Dim msg As New cMessage(String.Format(My.Resources.STATUS_SCOR_SAVED_FAILED, Me.m_tbxSCOR.Text), _
                                    eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.Plugin, eMessageImportance.Warning)
            Me.m_uic.Core.Messages.SendMessage(msg)
            Return
        End If

        Dim bridge As New cRBridge(Me.m_tbxR.Text)
        bridge.Field(Me.m_tbxPlaceholder.Text) = cFileUtils.DosToUnix(Me.m_tbxSCOR.Text)
        bridge.ExecuteFile(Me.m_tbxScript.Text)

        Me.UpdateList(Me.m_lbxScript, bridge.Input)
        Me.UpdateList(Me.m_lbxOutput, bridge.Output)
        Me.UpdateList(Me.m_lbxError, bridge.Errors)

        Me.UpdateIcons()

    End Sub

    Private Sub UpdateIcons()

        Me.UpdateTabIcon(Me.m_tpgScript, Me.m_lbxScript, 0)
        Me.UpdateTabIcon(Me.m_tpgOutput, Me.m_lbxOutput, 0)
        Me.UpdateTabIcon(Me.m_tpgErrors, Me.m_lbxError, 2)

    End Sub

    Private Sub UpdateTabIcon(tpg As TabPage, lbx As ListBox, iIndex As Integer)

        If (lbx.Items.Count = 0) Then
            tpg.ImageIndex = -1
        Else
            tpg.ImageIndex = iIndex
        End If

    End Sub

    Private Sub UpdateList(ByVal lb As ListBox, astrLines As String())

        lb.Items.Clear()
        lb.BeginUpdate()

        For Each strLine As String In astrLines
            strLine = strLine.Replace(cStringUtils.vbCrLf, cStringUtils.vbNewline)
            strLine = strLine.Replace(cStringUtils.vbLf, cStringUtils.vbNewline)
            For Each strBit As String In strLine.Split(CChar(cStringUtils.vbNewline))
                lb.Items.Add(strBit)
            Next
        Next

        lb.EndUpdate()

    End Sub

#End Region ' Internals

End Class