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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports System.IO
Imports EwEUtils.Core
Imports EwECore.SpatialData
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands
Imports EwEUtils.Utilities

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Spatail temporal data interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsSpatialTemporal
        Implements IOptionsPage
        Implements IUIElement

#Region " Private vars "

        Private m_strConfigPath As String = ""

#End Region ' Private vars

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)

            Me.UIContext = uic
            Me.InitializeComponent()

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Control's load event which gets called every time the control gets loaded. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Dim core As cCore = Me.UIContext.Core
            Dim man As cSpatialDataSetManager = core.SpatialDataConnectionManager.DatasetManager

            Me.m_strConfigPath = man.ConfigFile

            If String.IsNullOrWhiteSpace(Me.m_strConfigPath) Or cFileUtils.Equals(Me.m_strConfigPath, cSpatialDataSetManager.DefaultConfigFile) Then
                Me.m_rbDefault.Checked = True
            Else
                Me.m_rbCustom.Checked = True
            End If

            Me.m_cbAllowIndexing.Checked = man.IsIndexingAllowed
            Me.UpdateControls()

        End Sub

        Private Sub OnOptionChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_rbDefault.CheckedChanged, m_rbCustom.CheckedChanged
            Me.UpdateControls()
        End Sub

        Private Sub OnSelectFile(sender As System.Object, e As System.EventArgs) _
            Handles m_btnChoose.Click

            If (Me.UIContext Is Nothing) Then Return

            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            cmdFO.Title = My.Resources.PROMPT_SELECT_REFIMAGE
            cmdFO.Invoke(Me.m_strConfigPath, "Dataset config files|*.xml", 0)

            If (cmdFO.Result = DialogResult.OK) Then
                Me.m_strConfigPath = cmdFO.FileName
                Me.m_rbCustom.Checked = True
                Me.UpdateControls()
            End If

        End Sub

        Private Sub OnViewDefault(sender As System.Object, e As System.EventArgs) _
            Handles m_btnVisitFolder.Click

            If (Me.UIContext IsNot Nothing) Then
                Try
                    Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
                    Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
                    cmd.Invoke(Path.GetDirectoryName(cSpatialDataSetManager.DefaultConfigFile))
                Catch ex As Exception
                    cLog.Write(ex, "ucOptionsSpatialTemporal::OnViewDefault")
                End Try
            End If

        End Sub

        Private Sub OnViewCache(sender As System.Object, e As System.EventArgs) _
            Handles m_btnViewCache.Click

            If (Me.UIContext IsNot Nothing) Then
                Try
                    Dim cache As cSpatialDataCache = cSpatialDataCache.DefaultDataCache
                    Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
                    Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
                    cmd.Invoke(cache.RootFolder)
                Catch ex As Exception
                    cLog.Write(ex, "ucOptionsSpatialTemporal::OnViewCache")
                End Try
            End If

        End Sub

        Private Sub OnClearCache(sender As System.Object, e As System.EventArgs) _
            Handles m_btnClearCache.Click

            If (Me.UIContext IsNot Nothing) Then
                Try
                    Dim man As cSpatialDataSetManager = Me.UIContext.Core.SpatialDataConnectionManager.DatasetManager
                    Dim cache As cSpatialDataCache = cSpatialDataCache.DefaultDataCache
                    Dim sg As cStyleGuide = Me.UIContext.StyleGuide
                    Dim core As cCore = Me.UIContext.Core
                    Dim lSizeBefore As Long = cache.GetSize()
                    Dim lSizeUnused As Long = cache.GetUnusedSize(man)
                    Dim strPrompt As String = My.Resources.PROMPT_CACHE_CLEAR

                    If (lSizeUnused > 0) Then
                        Dim fmsg As New cFeedbackMessage(String.Format(strPrompt, sg.FormatMemory(lSizeBefore), sg.FormatMemory(lSizeUnused)), _
                                                         eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO_CANCEL)
                        core.Messages.SendMessage(fmsg)

                        Select Case fmsg.Reply
                            Case eMessageReply.YES
                                cache.Clear(man)
                            Case eMessageReply.NO
                                cache.Clear()
                            Case eMessageReply.CANCEL
                                Return
                        End Select
                    Else
                        cache.Clear()
                    End If

                    Dim msg As New cMessage(String.Format(My.Resources.STATUS_CACHECLEARED, sg.FormatMemory(lSizeBefore - cache.GetSize())), _
                         eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Information)
                    core.Messages.SendMessage(msg)

                    Me.UpdateControls()
                Catch ex As Exception
                    cLog.Write(ex, "ucOptionsSpatialTemporal::OnClearCache")
                End Try
            End If

        End Sub

        Protected Overrides Sub OnResize(e As System.EventArgs)
            MyBase.OnResize(e)
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers

#Region " Public methods "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext As cUIContext _
                 Implements IUIElement.UIContext

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanApply"/>
        ''' -------------------------------------------------------------------
        Public Function CanApply() As Boolean _
              Implements IOptionsPage.CanApply
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.OnChanged"/>
        ''' -------------------------------------------------------------------
        Public Event OnOptionsPedigreeChanged(sender As IOptionsPage, args As System.EventArgs) _
              Implements IOptionsPage.OnChanged

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            Dim core As cCore = Me.UIContext.Core
            Dim man As cSpatialDataSetManager = core.SpatialDataConnectionManager.DatasetManager
            Dim strFile As String = ""
            Dim bSuccess As Boolean = True

            If Not Me.CanApply Then Return IOptionsPage.eApplyResultType.Failed

            Try

                If (Me.m_rbCustom.Checked) Then
                    strFile = Me.m_strConfigPath
                End If

                Me.UIContext.Core.SetBatchLock(cCore.eBatchLockType.Restructure)
                Try
                    bSuccess = man.Load(strFile, True)
                Catch ex As Exception
                    bSuccess = False
                End Try
                core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace, bSuccess)

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex, "ucOptionsSpatialTemporal::Apply")
            End Try

            man.IsIndexingAllowed = Me.m_cbAllowIndexing.Checked

            If bSuccess Then Return IOptionsPage.eApplyResultType.Success
            Return IOptionsPage.eApplyResultType.Failed

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() _
                Implements IOptionsPage.SetDefaults

            Try
                Me.m_rbDefault.Checked = True
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Public methods

#Region " Internals "

        Private Sub UpdateControls()

            If (Me.UIContext Is Nothing) Then Return

            Dim sg As cStyleGuide = Me.UIContext.StyleGuide
            Dim cache As cSpatialDataCache = cSpatialDataCache.DefaultDataCache
            Dim man As cSpatialDataSetManager = Me.UIContext.Core.SpatialDataConnectionManager.DatasetManager
            Dim strPath As String = ""

            strPath = String.Copy(Me.m_strConfigPath)
            TextRenderer.MeasureText(strPath, Me.Font, New Drawing.Size(Me.m_lblPath.ClientSize.Width, 0), _
                                     TextFormatFlags.SingleLine Or TextFormatFlags.PathEllipsis)
            Me.m_lblPath.Text = strPath

            strPath = cache.RootFolder
            TextRenderer.MeasureText(strPath, Me.Font, New Drawing.Size(Me.m_lblCacheLocationValue.ClientSize.Width, 0), _
                                     TextFormatFlags.SingleLine Or TextFormatFlags.PathEllipsis)
            Me.m_lblCacheLocationValue.Text = strPath

            Me.m_lblCacheSizeValue.Text = String.Format(My.Resources.GENERIC_VALUE_CACHEMEMORY, _
                                                        sg.FormatMemory(cache.GetSize()), _
                                                        sg.FormatMemory(cache.GetUnusedSize(man)))

            Me.m_btnViewCache.Enabled = Directory.Exists(cache.RootFolder)
            Me.m_btnClearCache.Enabled = (cache.GetSize() > 0)

        End Sub

#End Region ' Internals

    End Class

End Namespace


