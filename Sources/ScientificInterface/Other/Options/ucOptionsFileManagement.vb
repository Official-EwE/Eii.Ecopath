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
Imports SharedResources = ScientificInterfaceShared.My.Resources

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
        Private m_strVersion As String = Application.ProductVersion.ToString
        Private m_strDocDir As String = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
        Private m_cbh As cCheckboxHierarchy = Nothing

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

            Dim strDefault As String = ("{" & SharedResources.HEADER_SCENARIO & "}").ToLower()
            Dim strEcosimScenario As String = strDefault
            Dim strEcospaceScenario As String = strDefault
            Dim strEcotracerScenario As String = strDefault
            Dim strOutput As String = ("{" & SharedResources.HEADER_OUTPUT_LOCATION & "}").ToLower() & IO.Path.DirectorySeparatorChar
            Dim core As cCore = Me.m_uic.Core

            If (core.ActiveEcosimScenarioIndex > -1) Then
                strEcosimScenario = core.EcosimScenarios(core.ActiveEcosimScenarioIndex).Name
            End If
            If (core.ActiveEcospaceScenarioIndex > -1) Then
                strEcospaceScenario = core.EcospaceScenarios(core.ActiveEcospaceScenarioIndex).Name
            End If
            If (core.ActiveEcotracerScenarioIndex > -1) Then
                strEcotracerScenario = core.EcotracerScenarios(core.ActiveEcotracerScenarioIndex).Name
            End If

            Me.m_tbxEcosim.Text = strOutput & core.OutputFileLocation(eAutosaveTypes.Ecosim, strScenario:=strEcosimScenario)
            Me.m_tbxMC.Text = strOutput & core.OutputFileLocation(eAutosaveTypes.MonteCarlo, strScenario:=strEcosimScenario)
            Me.m_tbxMSE.Text = strOutput & core.OutputFileLocation(eAutosaveTypes.MSE, strScenario:=strEcosimScenario)

            Me.m_tbxASCII.Text = strOutput & core.OutputFileLocation(eAutosaveTypes.EcospaceASC, strScenario:=strEcospaceScenario, strExt:=".asc")
            Me.m_tbxCSV.Text = strOutput & core.OutputFileLocation(eAutosaveTypes.EcospaceCSV, strScenario:=strEcospaceScenario, strExt:=".csv")

            Me.m_tbxTracer.Text = strOutput & core.OutputFileLocation(eAutosaveTypes.Ecotracer, strScenario:=strEcotracerScenario)

            Me.m_cbEcosimRun.Checked = core.Autosave(eAutosaveTypes.Ecosim)
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

            ' Configure checkbox hierarchy
            Me.m_cbh = New cCheckboxHierarchy(Me.m_cbAutosaveAll)
            Me.m_cbh.Add(Me.m_cbEcosim, Me.m_cbAutosaveAll)
            Me.m_cbh.Add(Me.m_cbEcosimRun, Me.m_cbEcosim)
            Me.m_cbh.Add(Me.m_cbMonteCarlo, Me.m_cbEcosim)
            Me.m_cbh.Add(Me.m_cbMSE, Me.m_cbEcosim)
            Me.m_cbh.Add(Me.m_cbEcospace, Me.m_cbAutosaveAll)
            Me.m_cbh.Add(Me.m_cbSpaceASCII, Me.m_cbEcospace)
            Me.m_cbh.Add(Me.m_cbSpaceCSV, Me.m_cbEcospace)
            Me.m_cbh.Add(Me.m_cbEcotracer, Me.m_cbAutosaveAll)
            Me.m_cbh.ManageCheckedStates = True

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType Implements IOptionsPage.Apply

            Dim core As cCore = Me.m_uic.Core
            Dim result As IOptionsPage.eApplyResultType = IOptionsPage.eApplyResultType.Success

            Try

                core.Autosave(eAutosaveTypes.Ecosim) = Me.m_cbEcosimRun.Checked
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

            Dim strSample As String = ""

            If Not cPathUtility.ResolvePath(strMask, Me.m_uic.Core, strSample) Then
                cPathUtility.ResolvePath(strMask, "{model}", m_strDocDir, ".eweaccdb", m_strVersion, strSample)
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
