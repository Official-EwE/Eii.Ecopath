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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Interop
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Extensions
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
    Private m_dad As cEwEtoRPluginPoint = Nothing
    Private m_fpR As cEwEFormatProvider = Nothing
    Private m_fpScript As cEwEFormatProvider = Nothing
    Private m_fpSCOR As cEwEFormatProvider = Nothing
    Private m_fpOutFile As cEwEFormatProvider = Nothing

    Private m_fpOutScript As cEwEFormatProvider = Nothing
    Private m_fpOutput As cEwEFormatProvider = Nothing
    Private m_fpOutError As cEwEFormatProvider = Nothing

    Private m_bInUpdate As Boolean = True
    Private m_ilTabs As New ImageList()

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

        Dim lstrPaths As New List(Of String)
        lstrPaths.AddRange(cRBridge.InstallLocations())
        If Not lstrPaths.Contains(My.Settings.RPath) Then lstrPaths.Add(My.Settings.RPath)

        Me.m_rbManagedSCOR.Checked = My.Settings.SCORmanaged
        Me.m_rbCustomSCOR.Checked = Not My.Settings.SCORmanaged

        ' Collapse SCOR section
        Me.m_hdrSettings.CollapsedParentHeight = Me.m_hdrSCOR.Location.Y
        Me.m_hdrSettings.IsCollapsed = Not My.Settings.AdvancedViz

        Me.m_fpR = New cEwEFormatProvider(Me.m_uic, Me.m_cmbR, GetType(String), lstrPaths.ToArray())
        Me.m_fpR.Value = My.Settings.RPath
        AddHandler Me.m_fpR.OnValueChanged, AddressOf OnValueChanged

        Me.m_fpScript = New cEwEFormatProvider(Me.m_uic, Me.m_tbxScriptFile, GetType(String))
        Me.m_fpScript.Value = My.Settings.RScript
        AddHandler Me.m_fpScript.OnValueChanged, AddressOf OnValueChanged

        Me.m_fpSCOR = New cEwEFormatProvider(Me.m_uic, Me.m_tbxSCORFile, GetType(String))
        Me.m_fpSCOR.Value = My.Settings.SCORFileCustom
        AddHandler Me.m_fpSCOR.OnValueChanged, AddressOf OnValueChanged

        Me.m_fpOutFile = New cEwEFormatProvider(Me.m_uic, Me.m_tbxOutFile, GetType(String))
        Me.m_fpOutFile.Value = Path.Combine(Me.m_uic.Core.DefaultOutputPath(eAutosaveTypes.Ecopath), "enaR_out.txt")
        AddHandler Me.m_fpOutFile.OnValueChanged, AddressOf OnValueChanged

        Me.m_fpOutScript = New cEwEFormatProvider(Me.m_uic, Me.m_tbxScriptOut, GetType(String))

        Me.m_fpOutput = New cEwEFormatProvider(Me.m_uic, Me.m_tbxOutput, GetType(String))
  
        Me.m_fpOutError = New cEwEFormatProvider(Me.m_uic, Me.m_tbxErrors, GetType(String))

        Me.m_ilTabs.Images.Add(cStyleGuide.GetImage(eMessageImportance.Information))
        Me.m_ilTabs.Images.Add(cStyleGuide.GetImage(eMessageImportance.Warning))
        Me.m_ilTabs.Images.Add(cStyleGuide.GetImage(eMessageImportance.Critical))
        Me.m_tcDebug.ImageList = Me.m_ilTabs

        Me.m_bInUpdate = False

        Me.UpdateControls()

        Me.CenterToScreen()

    End Sub

    Protected Overrides Sub OnClosed(e As System.EventArgs)

        Me.SaveSettings()

        RemoveHandler Me.m_fpR.OnValueChanged, AddressOf OnValueChanged
        Me.m_fpR.Release()
        RemoveHandler Me.m_fpScript.OnValueChanged, AddressOf OnValueChanged
        Me.m_fpScript.Release()
        RemoveHandler Me.m_fpSCOR.OnValueChanged, AddressOf OnValueChanged
        Me.m_fpSCOR.Release()
        RemoveHandler Me.m_fpOutFile.OnValueChanged, AddressOf OnValueChanged
        Me.m_fpOutFile.Release()
        Me.m_fpOutScript.Release()
        Me.m_fpOutput.Release()
        Me.m_fpOutError.Release()
        Me.Icon.Destroy()

        MyBase.OnClosed(e)

    End Sub

#End Region ' Overrides

#Region " Events "

    Private Sub OnChooseR_Click(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseR.Click

        Dim cmd As cFileOpenCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
        cmd.Invoke(CStr(Me.m_fpR.Value), My.Resources.FILEFILTER_R_EXE, 0, My.Resources.PROMPT_SELECT_R_EXE)
        If (cmd.Result = Windows.Forms.DialogResult.OK) Then
            Me.m_fpR.Value = cmd.FileName
        End If

    End Sub

    Private Sub OnChooseScript_Click(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseScript.Click

        Dim cmd As cFileOpenCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
        cmd.Invoke(Me.m_tbxScriptFile.Text, My.Resources.FILEFILTER_R_SCRIPT, 0, My.Resources.PROMPT_SELECT_R_SCRIPT)

        If (cmd.Result = Windows.Forms.DialogResult.OK) Then
            Me.m_fpScript.Value = Path.Combine(cmd.Directory, cmd.FileName)
        End If

    End Sub

    Private Sub OnChooseCustomSCOR(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseSCOR.Click

        Dim cmd As cFileSaveCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
        cmd.Invoke(Me.m_tbxSCORFile.Text, My.Resources.FILFILTER_SCOR, 0, My.Resources.PROMPT_SELECT_SCOR_FILE)

        If (cmd.Result = Windows.Forms.DialogResult.OK) Then
            Me.m_fpSCOR.Value = Path.Combine(cmd.Directory, cmd.FileName)
            Me.m_rbCustomSCOR.Checked = True
            Me.UpdateControls()
        End If

    End Sub

    Private Sub OnSCORTypeChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbManagedSCOR.CheckedChanged, m_rbCustomSCOR.CheckedChanged

        If Me.m_bInUpdate Then Return
        Me.UpdateControls()

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

    Private Sub OnValueChanged(sender As Object, args As EventArgs)
        Try
            If (Me.m_bInUpdate) Then Return
            Me.SaveSettings()
            BeginInvoke(New MethodInvoker(AddressOf UpdateControls))
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub UpdateControls()

        If Me.m_bInUpdate Then Return

        Dim strR As String = CStr(Me.m_fpR.Value)
        Dim bHasUIC As Boolean = (Me.m_uic IsNot Nothing)
        Dim bHasR As Boolean = File.Exists(strR)
        Dim bHasScript As Boolean = File.Exists(Me.m_tbxScriptFile.Text)
        Dim bHasSCOR As Boolean = Me.m_rbManagedSCOR.Checked Or (Not String.IsNullOrWhiteSpace(Me.m_tbxSCORFile.Text))

        If bHasR Then
            Me.m_fpR.Style = cStyleGuide.eStyleFlags.OK
        Else
            Me.m_fpR.Style = cStyleGuide.eStyleFlags.FailedValidation
        End If

        If bHasScript Then
            Me.m_fpScript.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Else
            Me.m_fpScript.Style = cStyleGuide.eStyleFlags.FailedValidation Or cStyleGuide.eStyleFlags.NotEditable
        End If

        If bHasSCOR Then
            Me.m_fpSCOR.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Else
            Me.m_fpSCOR.Style = cStyleGuide.eStyleFlags.FailedValidation Or cStyleGuide.eStyleFlags.NotEditable
        End If

        Me.m_fpOutScript.Style = cStyleGuide.eStyleFlags.NotEditable
        Me.m_fpOutput.Style = cStyleGuide.eStyleFlags.NotEditable
        Me.m_fpOutError.Style = cStyleGuide.eStyleFlags.NotEditable

        Me.m_btnChooseR.Enabled = bHasUIC
        Me.m_btnChooseSCOR.Enabled = bHasUIC
        Me.m_btnChooseScript.Enabled = bHasUIC

        Me.m_btnOK.Enabled = (bHasR And bHasScript And bHasSCOR)

    End Sub

    Private Sub SaveSettings()
        My.Settings.RPath = CStr(Me.m_fpR.Value)
        My.Settings.RScript = CStr(Me.m_fpScript.Value)
        My.Settings.SCORFileCustom = CStr(Me.m_fpSCOR.Value)
        My.Settings.SCORmanaged = Me.m_rbManagedSCOR.Checked()
        My.Settings.AdvancedViz = Not Me.m_hdrSettings.IsCollapsed
        My.Settings.Save()
    End Sub

    Private Sub RunR()

        Me.UpdateIcons()
        Me.SaveSettings()

        ' Run Ecopath
        Dim sm As cCoreStateMonitor = Me.m_uic.Core.StateMonitor
        If Not sm.HasEcopathRan Then
            If Not Me.m_uic.Core.RunEcoPath() Then
                ' Send message!
                Return
            End If
        End If

        Debug.Assert(Me.m_dad.m_epData IsNot Nothing)

        Dim writer As New cSCORWriter(Me.m_dad.m_epData)
        Dim strSCOR As String = ""

        If My.Settings.SCORmanaged Then
            strSCOR = cFileUtils.MakeTempFile(".dat")
        Else
            strSCOR = My.Settings.SCORFileCustom
        End If

        If writer.Write(strSCOR) Then
            Dim msg As New cMessage(String.Format(My.Resources.STATUS_SCOR_SAVED_SUCCESS, strSCOR), _
                                    eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.Plugin, eMessageImportance.Information)
            msg.Hyperlink = Path.GetDirectoryName(strSCOR)
            Me.m_uic.Core.Messages.SendMessage(msg)
        Else
            Dim msg As New cMessage(String.Format(My.Resources.STATUS_SCOR_SAVED_FAILED, strSCOR), _
                                    eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.Plugin, eMessageImportance.Warning)
            Me.m_uic.Core.Messages.SendMessage(msg)
            Return
        End If

        Dim bridge As New cRBridge(CStr(Me.m_fpR.Value))
        bridge.Field(My.Settings.SCORPlaceholder) = cFileUtils.DosToUnix(strSCOR)
        bridge.Field(My.Settings.OUTPlaceholder) = cFileUtils.DosToUnix(CStr(Me.m_fpOutFile.Value))
        ' bridge.RunElevated = True
        bridge.ExecuteFile(Me.m_tbxScriptFile.Text)

        Me.UpdateOutput(Me.m_fpOutScript, bridge.Input)
        Me.UpdateOutput(Me.m_fpOutput, bridge.Output)
        Me.UpdateOutput(Me.m_fpOutError, bridge.Errors)

        Me.UpdateIcons()

    End Sub

    Private Sub UpdateIcons()

        Me.UpdateTabIcon(Me.m_tpgScript, Me.m_fpOutScript, 0)
        Me.UpdateTabIcon(Me.m_tpgOutput, Me.m_fpOutput, 0)
        Me.UpdateTabIcon(Me.m_tpgErrors, Me.m_fpOutError, 2)

    End Sub

    Private Sub UpdateTabIcon(tpg As TabPage, fp As cEwEFormatProvider, iIndex As Integer)

        If (String.IsNullOrWhiteSpace(CStr(fp.Value))) Then
            tpg.ImageIndex = -1
        Else
            tpg.ImageIndex = iIndex
        End If

    End Sub

    Private Sub UpdateOutput(ByVal fp As cEwEFormatProvider, astrLines As String())

        Dim sb As New StringBuilder()

        For Each strLine As String In astrLines
            strLine = strLine.Replace(cStringUtils.vbCrLf, cStringUtils.vbNewline)
            strLine = strLine.Replace(cStringUtils.vbLf, cStringUtils.vbNewline)
            strLine = strLine.Replace(CStr(cStringUtils.vbNewline & cStringUtils.vbNewline), cStringUtils.vbNewline) ' Boohoohoo
            For Each strBit As String In strLine.Split(CChar(cStringUtils.vbNewline))
                sb.AppendLine(strBit)
            Next
        Next

        fp.Value = sb.ToString()

    End Sub

#End Region ' Internals

End Class