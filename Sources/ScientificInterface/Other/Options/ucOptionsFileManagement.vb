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
Imports EwECore
Imports EwEUtils.Utilities
Imports EwEUtils.Core

#End Region ' Imports

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > file management interface
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsFileManagement
        Implements IOptionsPage

        Private m_uic As cUIContext = Nothing

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)
            Me.m_uic = uic
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            'Me.m_tsddFields.DropDown.Items.Clear()
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            ' ToDo: globalize this
            Dim strEcosimScenarioName As String = "{scenario}"
            Dim strEcospaceScenarioName As String = "{scenario}"
            Dim strEcotracerScenarioName As String = "{scenario}"
            Dim core As cCore = Me.m_uic.Core

            If (core.ActiveEcosimScenarioIndex > -1) Then
                strEcosimScenarioName = core.EcosimScenarios(core.ActiveEcosimScenarioIndex).Name
            End If
            If (core.ActiveEcospaceScenarioIndex > -1) Then
                strEcospaceScenarioName = core.EcospaceScenarios(core.ActiveEcospaceScenarioIndex).Name
            End If
            If (core.ActiveEcotracerScenarioIndex > -1) Then
                strEcotracerScenarioName = core.EcotracerScenarios(core.ActiveEcotracerScenarioIndex).Name
            End If

            Me.m_cbAutosaveAll.CheckState = CheckState.Indeterminate
            Me.m_cbEcosim.CheckState = CheckState.Indeterminate
            Me.m_cbEcospace.CheckState = CheckState.Indeterminate

            Me.m_tbxEcosim.Text = core.EcosimOutputFileLocation(strScenarioName:=strEcosimScenarioName)
            Me.m_tbxMC.Text = core.EcosimOutputFileLocation(strFilter:=eAutosaveTypes.MonteCarlo.ToString(), strScenarioName:=strEcosimScenarioName)
            Me.m_tbxMSE.Text = core.EcosimOutputFileLocation(strFilter:=eAutosaveTypes.MSE.ToString(), strScenarioName:=strEcosimScenarioName)

            Me.m_tbxASCII.Text = core.EcospaceOutputFileLocation(strScenarioName:=strEcospaceScenarioName, strExt:=".asc")
            Me.m_tbxCSV.Text = core.EcospaceOutputFileLocation(strScenarioName:=strEcospaceScenarioName, strExt:=".csv")

            Me.m_tbxTracer.Text = core.EcotracerOutputFileLocation(strScenarioName:=strEcotracerScenarioName)

            Me.m_cbEcosimRun.Checked = core.Autosave(eAutosaveTypes.EcosimRun)
            Me.m_cbMonteCarlo.Checked = core.Autosave(eAutosaveTypes.MonteCarlo)
            Me.m_cbMSE.Checked = core.Autosave(eAutosaveTypes.MSE)
            Me.m_cbSpaceCSV.Checked = core.Autosave(eAutosaveTypes.EcospaceCSV)
            Me.m_cbSpaceASCII.Checked = core.Autosave(eAutosaveTypes.EcospaceASC)
            Me.m_cbEcotracer.Checked = core.Autosave(eAutosaveTypes.Ecotracer)

            ' Output path
            Me.m_fieldpickOutput.UIContext = Me.m_uic
            Me.m_fieldpickOutput.Fields = [Enum].GetValues(GetType(cPathUtility.ePathPlaceholderTypes))
            Me.m_tbOutputMask.Text = My.Settings.OutputPathMask

            ' Backup path masks
            Me.m_fieldpickBackup.UIContext = Me.m_uic
            Me.m_fieldpickBackup.Fields = [Enum].GetValues(GetType(cPathUtility.ePathPlaceholderTypes))
            Me.m_tbBackupMask.Text = My.Settings.BackupFileMask

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType Implements IOptionsPage.Apply

            Dim core As cCore = Me.m_uic.Core
            Dim result As IOptionsPage.eApplyResultType = IOptionsPage.eApplyResultType.Success

            Try

                core.Autosave(eAutosaveTypes.EcosimRun) = Me.m_cbEcosimRun.Checked
                core.Autosave(eAutosaveTypes.MonteCarlo) = Me.m_cbMonteCarlo.Checked
                core.Autosave(eAutosaveTypes.MSE) = Me.m_cbMSE.Checked
                core.Autosave(eAutosaveTypes.EcospaceCSV) = Me.m_cbSpaceCSV.Checked
                core.Autosave(eAutosaveTypes.EcospaceASC) = Me.m_cbSpaceASCII.Checked
                core.Autosave(eAutosaveTypes.Ecotracer) = Me.m_cbEcotracer.Checked

                My.Settings.BackupFileMask = Me.m_tbBackupMask.Text
                My.Settings.OutputPathMask = Me.m_tbOutputMask.Text

            Catch ex As Exception
                cLog.Write(ex, "ucOptionsAutosave::Apply")
                result = IOptionsPage.eApplyResultType.Failed
            End Try

            Return result

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() Implements IOptionsPage.SetDefaults

            Try
                Me.m_tbOutputMask.Text = CStr(My.Settings.GetDefaultValue("OutputPathMask"))
                Me.m_tbBackupMask.Text = CStr(My.Settings.GetDefaultValue("BackupFileMask"))
            Catch ex As Exception
                cLog.Write(ex, "ucOptionsAutosave::SetDefaults")
            End Try

        End Sub

#End Region ' Overrides

#Region " Event handlers "

        Private Sub SaveAllClicked(sender As System.Object, e As System.EventArgs) _
            Handles m_cbAutosaveAll.Click

            Me.m_cbEcosim.Checked = Me.m_cbAutosaveAll.Checked
            Me.m_cbEcospace.Checked = Me.m_cbAutosaveAll.Checked
            Me.m_cbEcotracer.Checked = Me.m_cbAutosaveAll.Checked
            Me.EcosimClicked(sender, e)
            Me.EcospaceClicked(sender, e)

        End Sub

        Private Sub EcosimClicked(sender As System.Object, e As System.EventArgs) _
            Handles m_cbEcosim.Click

            Me.m_cbEcosimRun.Checked = Me.m_cbEcosim.Checked
            Me.m_cbMonteCarlo.Checked = Me.m_cbEcosim.Checked
            Me.m_cbMSE.Checked = Me.m_cbEcosim.Checked

        End Sub

        Private Sub EcospaceClicked(sender As System.Object, e As System.EventArgs) _
            Handles m_cbEcospace.Click

            Me.m_cbSpaceASCII.Checked = Me.m_cbEcospace.Checked
            Me.m_cbSpaceCSV.Checked = Me.m_cbEcospace.Checked

        End Sub

        Private Sub OnOutputFieldPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal value As Object) _
            Handles m_fieldpickOutput.OnFieldPicked

            Me.InsertText(Me.m_tbOutputMask, "{" & value.ToString & "}")
            Me.UpdateControls()

        End Sub

        Private Sub OnOutputDirectoryPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal strDirectory As String) _
            Handles m_fieldpickOutput.OnDirectoryPicked

            Me.m_tbOutputMask.SelectionStart = 0
            Me.m_tbOutputMask.SelectionLength = Math.Max(0, Me.m_tbOutputMask.Text.LastIndexOf("\"c))
            Me.InsertText(Me.m_tbOutputMask, strDirectory)
            Me.UpdateControls()

        End Sub

        Private Sub OnBackupFieldPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal value As Object) _
            Handles m_fieldpickBackup.OnFieldPicked

            Me.InsertText(Me.m_tbBackupMask, "{" & value.ToString & "}")
            Me.UpdateControls()

        End Sub

        Private Sub OnBackupDirectoryPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal strDirectory As String) _
            Handles m_fieldpickBackup.OnDirectoryPicked

            Me.m_tbBackupMask.SelectionStart = 0
            Me.m_tbBackupMask.SelectionLength = Math.Max(0, Me.m_tbBackupMask.Text.LastIndexOf("\"c))
            Me.InsertText(Me.m_tbBackupMask, strDirectory)
            Me.UpdateControls()

        End Sub

        Private Sub OnMaskChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tbBackupMask.TextChanged, m_tbOutputMask.TextChanged

            Me.UpdateControls()

        End Sub

#End Region ' Event handlers

#Region " Internals "

        Private Sub UpdateControls()

            Me.UpdateSample(Me.m_tbxOutputSample, Me.m_tbOutputMask.Text)
            Me.UpdateSample(Me.m_tbxBackupSample, Me.m_tbBackupMask.Text)

        End Sub

        Private Sub UpdateSample(ByVal tbx As TextBox, ByVal strMask As String)

            Dim strVersion As String = Application.ProductVersion.ToString
            Dim strDocDir As String = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
            Dim strSample As String = ""

            If Not cPathUtility.ResolvePath(strMask, Me.m_uic.Core, strSample) Then
                cPathUtility.ResolvePath(strMask, "{model}", strDocDir, ".eweaccdb", strVersion, strSample)
            End If
            tbx.Text = cStringUtils.CompactString(strSample, tbx.ClientRectangle.Width, tbx.Font, TextFormatFlags.PathEllipsis)

        End Sub

        Private Sub InsertText(ByVal tb As TextBox, ByVal strText As String)
            Dim strSrc As String = tb.Text
            Dim strDest As String
            Dim iSelStart As Integer = tb.SelectionStart
            Dim iSelLen As Integer = tb.SelectionLength
            Dim iItemLen As Integer = strText.Length

            If (iSelLen = 0) Then
                strDest = strSrc & strText
                iSelStart = strDest.Length
            Else
                strDest = strSrc.Substring(0, iSelStart) & strText & strSrc.Substring(iSelStart + iSelLen)
                iSelStart += iItemLen
            End If

            tb.Text = strDest
            tb.SelectionStart = iSelStart
            tb.SelectionLength = 0
        End Sub

        Private Sub ReplaceText(ByVal tb As TextBox, ByVal strText As String)
            tb.Text = strText
        End Sub

#End Region ' Internals

    End Class

End Namespace
