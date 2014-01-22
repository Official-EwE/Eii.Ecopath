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
Imports EwECore.SpatialData
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwECore

#End Region ' Imports

' ToDo: implement create and delete in different tabs
' ToDo: add check on delete if dataset is applied in this model. Could also be applied to other models, have no idea
' ToDo: add support for switching dataset files
' ToDo: add import / export (package / unpackage) features
' ToDo: add indexing overview
' ToDo: add cache overview

Namespace Ecospace.Controls

    ''' <summary>
    ''' Dialog for defining Ecospace spatial temporal datasets
    ''' </summary>
    Public Class dlgDefineExternalSpatialData

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_man As cSpatialDataConnectionManager = Nothing
        Private m_manSets As cSpatialDataSetManager = Nothing
        Private m_bHasDatasetTemplates As Boolean = False
        Private m_bHasCachedData As Boolean = False

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Standard bits "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Overrides Property UIContext As cUIContext
            Get
                Return Me.m_uic
            End Get
            Set(uic As cUIContext)

                If (Me.m_uic IsNot Nothing) Then
                    ' Disconnect from data objects first; we do not want disconnecting UI elements from screwing up the last configuration
                    Me.m_gridDatasets.UIContext = Nothing

                    Me.m_manSets.Save()
                    Me.m_manSets = Nothing
                    Me.m_man = Nothing
                End If

                Me.m_uic = uic

                If (Me.m_uic IsNot Nothing) Then
                    ' Set new
                    Me.m_man = Me.m_uic.Core.SpatialDataConnectionManager
                    Me.m_manSets = Me.m_man.DatasetManager
                    Me.m_gridDatasets.UIContext = Me.m_uic
                End If
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.FillTemplateDatasetBox()
            Me.m_gridDatasets.Fill()

            ' Update cache state (will also update controls)
            Me.EvaluateCache()

            AddHandler Me.m_gridDatasets.OnSelectionChanged, AddressOf OnGridSelectionChanged

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            RemoveHandler Me.m_gridDatasets.OnSelectionChanged, AddressOf OnGridSelectionChanged
            Me.UIContext = Nothing
            MyBase.OnFormClosed(e)
        End Sub

        Protected Overrides Sub UpdateControls()
            MyBase.UpdateControls()

            Dim bHasTemp As Boolean = (Me.m_cmbNewDS.SelectedItem IsNot Nothing)
            Dim bHasDS As Boolean = (Me.m_gridDatasets.SelectedDataset IsNot Nothing)

            Me.m_cmbNewDS.Enabled = bHasTemp

            Me.m_btnDelete.Enabled = bHasDS
            Me.m_btnConfigure.Enabled = bHasDS

        End Sub

#End Region ' Standard bits

#Region " Event handlers "

        Private Sub OnSelectDSTemplate(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbNewDS.SelectedIndexChanged
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnAddDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnAdd.Click
            Me.Cursor = Cursors.WaitCursor
            Try
                Me.CreateDS()
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
            Me.Cursor = Cursors.Default
        End Sub

        Private Sub OnDeleteDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnDelete.Click
            Try
                Me.DeleteDS(Me.SelectedDataset)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' User wants to configure the currently selected dataset.
        ''' </summary>
        Private Sub OnConfigDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnConfigure.Click

            Me.Cursor = Cursors.WaitCursor
            Try
                Me.ConfigDS(Me.SelectedDataset)
                Me.m_gridDatasets.Fill(Me.SelectedDataset)
                Me.m_manSets.IndexDataset = Me.SelectedDataset
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex, "ucConficAdapter::OnConfigureDS")
            End Try
            Me.Cursor = Cursors.Default

        End Sub

        Private Sub OnGridSelectionChanged(sender As Object)
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
            Handles m_btnOK.Click
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.m_manSets.Save()
            Me.Close()
        End Sub

#End Region ' Event handlers 

#Region " Internals "

        Private Property Varname As eVarNameFlags
            Get
                Return eVarNameFlags.NotSet
            End Get
            Set(value As eVarNameFlags)

            End Set
        End Property

        Private Property SelectedDataset As ISpatialDataSet
            Get
                Return Me.m_gridDatasets.SelectedDataset
            End Get
            Set(dataset As ISpatialDataSet)
                Me.m_gridDatasets.SelectedDataset = dataset
                Me.UpdateControls()
            End Set
        End Property

        ''' <summary>
        ''' Fill UI with available dataset templates
        ''' </summary>
        Private Sub FillTemplateDatasetBox()

            Me.m_cmbNewDS.Items.Clear()
            For Each ds As ISpatialDataSet In Me.m_man.DatasetTemplates
                Me.m_cmbNewDS.Items.Add(ds)
            Next

            If (Me.m_cmbNewDS.Items.Count = 0) Then
                Me.m_cmbNewDS.Items.Add("")
                Me.m_bHasDatasetTemplates = False
            Else
                Me.m_bHasDatasetTemplates = True
            End If
            Me.m_cmbNewDS.SelectedIndex = 0

        End Sub

        Private Sub EvaluateCache()
            Me.m_bHasCachedData = (cSpatialDataCache.DefaultDataCache.GetSize > 0)
            Me.UpdateControls()
        End Sub

        Private Sub CreateDS()

            Dim item As Object = Me.m_cmbNewDS.SelectedItem
            If Not TypeOf (item) Is ISpatialDataSet Then Return

            Dim dsSelected As ISpatialDataSet = DirectCast(item, ISpatialDataSet)
            Dim dsNew As ISpatialDataSet = Nothing

            If (dsSelected Is Nothing) Then Return

            dsNew = CType(Activator.CreateInstance(dsSelected.GetType()), ISpatialDataSet)
            If (dsNew Is Nothing) Then Return

            Try
                dsNew.VarName = Me.Varname
                If Me.ConfigDS(dsNew) Then
                    Me.m_manSets.Add(dsNew)
                    Me.m_gridDatasets.Fill(dsNew)
                    Me.m_manSets.IndexDataset = dsNew
                End If
            Catch ex As Exception
                cLog.Write(ex, "ucConficAdapter::CreateDS")
            End Try

        End Sub

        Private Function ConfigDS(ds As ISpatialDataSet) As Boolean

            If (ds Is Nothing) Then Return False
            If (Not TypeOf ds Is IConfigurable) Then Return True

            If (TypeOf ds Is IPlugin) Then
                DirectCast(ds, IPlugin).Initialize(Me.m_uic.Core)
            End If

            Dim dsConf As IConfigurable = DirectCast(ds, IConfigurable)
            Dim ctrl As Control = dsConf.GetConfigUI()

            If (ctrl Is Nothing) Then Return dsConf.IsConfigured

            Dim dlg As New dlgConfig()
            dlg.UIContext = Me.UIContext
            dlg.ShowDialog(Me.FindForm, My.Resources.CAPTION_EXTERNAL_DATASET_CONFIGURE, ctrl)

            Me.EvaluateCache()

            Return (dsConf.IsConfigured)

        End Function

        Public Sub DeleteDS(ds As ISpatialDataSet)
            Me.SelectedDataset = Nothing
            Me.m_manSets.Remove(ds)
            Me.m_gridDatasets.Fill()
            Me.UpdateControls()
        End Sub

        ''' <summary>
        ''' User wants to clear the spatial data cache.
        ''' </summary>
        Private Sub OnClearCache(sender As System.Object, e As System.EventArgs)


            Dim cache As cSpatialDataCache = cSpatialDataCache.DefaultDataCache
            Dim dSizeTot As Double = cache.GetSize() / 1024
            Dim dSizeUnused As Double = cache.GetUnusedSize(Me.m_manSets) / 1024
            Dim strPrompt As String = My.Resources.PROMPT_CACHE_CLEAR
            Dim bSucces As Boolean = True

            Try
                If (dSizeUnused > 0) Then
                    Dim fmsg As New cFeedbackMessage(String.Format(strPrompt, Me.m_uic.StyleGuide.FormatNumber(dSizeTot), Me.m_uic.StyleGuide.FormatNumber(dSizeUnused)), _
                                                     EwEUtils.Core.eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO_CANCEL)
                    Me.m_uic.Core.Messages.SendMessage(fmsg)

                    Select Case fmsg.Reply
                        Case eMessageReply.YES
                            bSucces = cSpatialDataCache.DefaultDataCache.Clear(Me.m_manSets)
                        Case eMessageReply.NO
                            bSucces = cSpatialDataCache.DefaultDataCache.Clear()
                        Case eMessageReply.CANCEL
                    End Select
                Else
                    bSucces = cSpatialDataCache.DefaultDataCache.Clear()
                End If
            Catch ex As Exception
                bSucces = False
            End Try

            ' Repopulate grid to reflect cache sizes
            Me.m_gridDatasets.Fill(Me.SelectedDataset)

            Dim dSizeTot2 As Double = cache.GetSize() / 1024
            Dim msg As New cMessage(String.Format(My.Resources.STATUS_CACHECLEARED, Me.m_uic.StyleGuide.FormatNumber(dSizeTot - dSizeTot2)), _
                                    eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Information)
            Me.m_uic.Core.Messages.SendMessage(msg)

        End Sub
#End Region ' Internals 

    End Class

End Namespace
