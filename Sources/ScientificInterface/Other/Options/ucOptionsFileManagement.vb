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
Imports EwEPlugin

#End Region ' Imports

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > file management interface
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsFileManagement
        Implements IOptionsPage

#Region " Private classes "

        Private Class cPluginSorter
            Implements IComparer(Of IAutoSavePlugin)

            Public Function Compare(x As EwEPlugin.IAutoSavePlugin, _
                                    y As EwEPlugin.IAutoSavePlugin) As Integer _
                                Implements IComparer(Of EwEPlugin.IAutoSavePlugin).Compare
                Return String.Compare(x.Name, y.Name)
            End Function

        End Class

        Private Class cAutoSaveItemEngine
            Implements IDisposable

            Private m_uic As cUIContext = Nothing
            Private m_pl As Panel = Nothing
            Private m_cbh As cCheckboxHierarchy = Nothing
            Private m_lControls As List(Of ucAutosaveOption) = Nothing

            Public Sub New(ByVal uic As cUIContext)
                Me.m_uic = uic
                Me.m_lControls = New List(Of ucAutosaveOption)
            End Sub

            Public Sub Attach(pl As Panel)

                Me.m_pl = pl

                Dim core As cCore = Me.m_uic.Core
                Dim pm As cPluginManager = core.PluginManager
                Dim lPlugins([Enum].GetValues(GetType(eAutosaveTypes)).Length - 1) As List(Of IAutoSavePlugin)

                For Each t As eAutosaveTypes In [Enum].GetValues(GetType(eAutosaveTypes))
                    lPlugins(t) = New List(Of IAutoSavePlugin)
                Next

                ' Make inventory of autosave plug-ins
                If (pm IsNot Nothing) Then
                    For Each pi As IPlugin In pm.GetPlugins(GetType(IAutoSavePlugin))
                        Dim aspi As IAutoSavePlugin = DirectCast(pi, IAutoSavePlugin)
                        lPlugins(aspi.AutoSaveType).Add(aspi)
                    Next pi
                End If
                Me.BuildControlTree(eAutosaveTypes.NotSet, Nothing, 0, lPlugins)
                Me.m_cbh.ManageCheckedStates = True

            End Sub

            Public Sub Apply()
                For Each uc As ucAutosaveOption In Me.m_lControls
                    uc.Apply()
                Next
            End Sub

            Public Sub Detach()
                Me.m_pl.SuspendLayout()
                For Each uc As ucAutosaveOption In Me.m_lControls
                    Me.m_pl.Controls.Remove(uc)
                Next
                Me.m_lControls.Clear()
                Me.m_pl.ResumeLayout()
                Me.m_pl = Nothing
                Me.m_cbh.Dispose()
                Me.m_cbh = Nothing

            End Sub

            Public Sub SetOutputMask(ByVal strMask As String)
                For Each uc As ucAutosaveOption In Me.m_lControls
                    uc.SetOutputMask(strMask)
                Next
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                Me.Detach()
                Me.m_cbh.Dispose()
                GC.SuppressFinalize(Me)
            End Sub

            Private Sub BuildControlTree(ByVal t As eAutosaveTypes, _
                                         ByVal parent As CheckBox, _
                                         ByVal iIndent As Integer, _
                                         ByVal lPlugins() As List(Of IAutoSavePlugin))

                Dim cbParent As CheckBox = Nothing
                Dim ctrl As ucAutosaveOption = Nothing

                Select Case t
                    Case eAutosaveTypes.NotSet
                        ctrl = New ucAutosaveOption(Me.m_uic, "Auto-save all", 0)
                        Me.Add(ctrl, Nothing)
                        Dim cbRoot As CheckBox = ctrl.Checkbox

                        ctrl = New ucAutosaveOption(Me.m_uic, "Ecopath", 1)
                        Me.Add(ctrl, cbRoot)
                        Me.BuildControlTree(eAutosaveTypes.Ecopath, ctrl.Checkbox, 2, lPlugins)

                        ctrl = New ucAutosaveOption(Me.m_uic, "Ecosim", 1)
                        Me.Add(ctrl, cbRoot)
                        Me.BuildControlTree(eAutosaveTypes.Ecosim, ctrl.Checkbox, 2, lPlugins)

                        ctrl = New ucAutosaveOption(Me.m_uic, "Ecospace", 1)
                        Me.Add(ctrl, cbRoot)
                        Me.BuildControlTree(eAutosaveTypes.Ecospace, ctrl.Checkbox, 2, lPlugins)

                        Me.BuildControlTree(eAutosaveTypes.Ecotracer, ctrl.Checkbox, 1, lPlugins)

                    Case eAutosaveTypes.Ecopath
                        Me.Add(lPlugins(t), parent, iIndent)

                    Case eAutosaveTypes.Ecosim
                        ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                        Me.Add(ctrl, parent)
                        Me.BuildControlTree(eAutosaveTypes.MonteCarlo, ctrl.Checkbox, iIndent, lPlugins)
                        Me.BuildControlTree(eAutosaveTypes.MSE, ctrl.Checkbox, iIndent, lPlugins)
                        Me.BuildControlTree(eAutosaveTypes.MSY, ctrl.Checkbox, iIndent, lPlugins)
                        Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

                    Case eAutosaveTypes.Ecospace
                        ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                        Me.Add(ctrl, parent)
                        Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

                    Case eAutosaveTypes.Ecotracer
                        ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                        Me.Add(ctrl, parent)

                    Case eAutosaveTypes.MonteCarlo
                        ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                        Me.Add(ctrl, parent)
                        Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

                    Case eAutosaveTypes.MSY
                        ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                        Me.Add(ctrl, parent)
                        Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

                    Case eAutosaveTypes.MSE
                        ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                        Me.Add(ctrl, parent)
                        Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

                End Select
            End Sub

            Private Sub Add(ByVal uc As ucAutosaveOption, ByVal parent As CheckBox)
                Me.m_pl.Controls.Add(uc)
                uc.Location = New Point(0, (Me.m_pl.Controls.Count - 1) * uc.Height)
                uc.Width = Me.m_pl.Width
                uc.Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top

                If (parent IsNot Nothing) Then
                    Me.m_cbh.Add(uc.Checkbox, parent)
                Else
                    Me.m_cbh = New cCheckboxHierarchy(uc.Checkbox)
                End If

                Me.m_lControls.Add(uc)
            End Sub

            Private Sub Add(ByVal l As List(Of IAutoSavePlugin), _
                            ByVal parent As CheckBox, _
                            ByVal iIndent As Integer)

                Dim api As IAutoSavePlugin() = l.ToArray
                Array.Sort(api, New cPluginSorter())
                For Each pi As IAutoSavePlugin In api
                    Me.Add(New ucAutosaveOption(Me.m_uic, pi), parent)
                Next

            End Sub

        End Class

#End Region ' Private class

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_strVersion As String = Application.ProductVersion.ToString
        Private m_strDocDir As String = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
        Private m_cbh As cCheckboxHierarchy = Nothing
        Private m_options As New List(Of ucAutosaveOption)
        Private m_engine As cAutoSaveItemEngine = Nothing

#End Region ' Private vars

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)
            Me.m_uic = uic
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
                Me.m_engine.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_engine = New cAutoSaveItemEngine(Me.m_uic)
            Me.m_engine.Attach(Me.m_plAutoSave)


            'Me.m_cbEcosimRun.Checked = core.Autosave(eAutosaveTypes.Ecosim)
            'Me.m_cbMonteCarlo.Checked = core.Autosave(eAutosaveTypes.MonteCarlo)
            'Me.m_cbMSE.Checked = core.Autosave(eAutosaveTypes.MSE)
            'Me.m_cbMSY.Checked = core.Autosave(eAutosaveTypes.MSY)
            'Me.m_cbSpaceCSV.Checked = core.Autosave(eAutosaveTypes.EcospaceCSV)
            'Me.m_cbSpaceASCII.Checked = core.Autosave(eAutosaveTypes.EcospaceASC)
            'Me.m_cbEcotracer.Checked = core.Autosave(eAutosaveTypes.Ecotracer)

            ' Output path
            Me.m_fieldpickOutput.UIContext = Me.m_uic
            Me.m_fieldpickOutput.Fields = [Enum].GetValues(GetType(cPathUtility.ePathPlaceholderTypes))
            Me.m_tbOutputMask.Text = My.Settings.OutputPathMask

            ' Backup path masks
            Me.m_fieldpickBackup.UIContext = Me.m_uic
            Me.m_fieldpickBackup.Fields = [Enum].GetValues(GetType(cPathUtility.ePathPlaceholderTypes))
            Me.m_tbBackupMask.Text = My.Settings.BackupFileMask

            '' Configure checkbox hierarchy
            'Me.m_cbh.Add(Me.m_cbEcosim, Me.m_cbAutosaveAll)
            'Me.m_cbh.Add(Me.m_cbEcosimRun, Me.m_cbEcosim)
            'Me.m_cbh.Add(Me.m_cbMonteCarlo, Me.m_cbEcosim)
            'Me.m_cbh.Add(Me.m_cbMSE, Me.m_cbEcosim)
            'Me.m_cbh.Add(Me.m_cbMSY, Me.m_cbEcosim)
            'Me.m_cbh.Add(Me.m_cbEcospace, Me.m_cbAutosaveAll)
            'Me.m_cbh.Add(Me.m_cbSpaceASCII, Me.m_cbEcospace)
            'Me.m_cbh.Add(Me.m_cbSpaceCSV, Me.m_cbEcospace)
            'Me.m_cbh.Add(Me.m_cbEcotracer, Me.m_cbAutosaveAll)
            'Me.m_cbh.ManageCheckedStates = True

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType Implements IOptionsPage.Apply

            Dim core As cCore = Me.m_uic.Core
            Dim result As IOptionsPage.eApplyResultType = IOptionsPage.eApplyResultType.Success

            Try

                'core.Autosave(eAutosaveTypes.Ecosim) = Me.m_cbEcosimRun.Checked
                'core.Autosave(eAutosaveTypes.MonteCarlo) = Me.m_cbMonteCarlo.Checked
                'core.Autosave(eAutosaveTypes.MSE) = Me.m_cbMSE.Checked
                'core.Autosave(eAutosaveTypes.MSY) = Me.m_cbMSY.Checked
                'core.Autosave(eAutosaveTypes.EcospaceCSV) = Me.m_cbSpaceCSV.Checked
                'core.Autosave(eAutosaveTypes.EcospaceASC) = Me.m_cbSpaceASCII.Checked
                'core.Autosave(eAutosaveTypes.Ecotracer) = Me.m_cbEcotracer.Checked

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

            Dim core As cCore = Me.m_uic.Core

            Me.UpdateSample(Me.m_tbxOutputSample, Me.m_tbOutputMask.Text)
            Me.UpdateSample(Me.m_tbxBackupSample, Me.m_tbBackupMask.Text)

            Me.m_engine.SetOutputMask(Me.m_tbOutputMask.Text)

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
