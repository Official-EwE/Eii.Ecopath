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

Imports System.IO
Imports EwECore
Imports EwEUtils.Commands
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region ' Imports

Namespace Ecospace.Basemap

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class dlgExportLayerData

#Region " Private classes "

        <CLSCompliant(False)> _
        Public Class gridExportMappings
            Inherits EwEGrid

#Region " Private vars "

            ''' <summary>The layers to map upon.</summary>
            Private m_aLayers As cRasterLayer()
            ''' <summary>Mappings. MAPPINGS!</summary>
            Private m_dtLayerMapping As New Dictionary(Of cRasterLayer, String)

            Private Enum eColumnTypes As Integer
                ColumnLayer = 0
                ColumnField
                ' Show datatype columns?
            End Enum

#End Region ' Private vars

#Region " Construction "

            Public Sub New()

            End Sub

#End Region ' Construction

#Region " Public interfaces "

            Public Property Layers() As cRasterLayer()
                Get
                    Return Nothing
                End Get
                Set(ByVal value As cRasterLayer())

                    Me.m_aLayers = value
                    Me.m_dtLayerMapping.Clear()

                    If value IsNot Nothing Then
                        For Each l As cRasterLayer In value
                            Me.m_dtLayerMapping(l) = l.DisplayText.Trim()
                        Next
                    End If

                    Me.RefreshContent()
                End Set
            End Property

            Public Function Mappings() As Dictionary(Of cRasterLayer, String)
                Return Me.m_dtLayerMapping
            End Function

#End Region ' Public interfaces

#Region " Overrides "

            Protected Overrides Sub InitStyle()
                MyBase.InitStyle()

                If Not Me.HasData() Then Return

                Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

                Me(0, eColumnTypes.ColumnLayer) = New EwEColumnHeaderCell(SharedResources.HEADER_LAYER)
                Me(0, eColumnTypes.ColumnField) = New EwEColumnHeaderCell(SharedResources.HEADER_FIELD)

                Me.Columns(eColumnTypes.ColumnLayer).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                Me.Columns(eColumnTypes.ColumnField).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch

                Me.FixedColumns = 1
                Me.FixedColumnWidths = False

            End Sub

            Protected Overrides Sub FillData()

                If Not Me.HasData Then Return

                Me.RowsCount = 1

                Dim layer As cRasterLayer = Nothing
                Dim ewec As EwECell = Nothing
                Dim cmb As Cells.Real.ComboBox = Nothing

                For iLayer As Integer = 0 To Me.m_aLayers.Length - 1

                    Me.AddRow()
                    layer = Me.m_aLayers(iLayer)

                    ewec = New EwECell(layer.Name, GetType(String))
                    ewec.Style = (cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable)
                    Me(iLayer + 1, eColumnTypes.ColumnLayer) = ewec

                    ewec = New EwECell(layer.Name, GetType(String))
                    ewec.Behaviors.Add(Me.EwEEditHandler)
                    Me(iLayer + 1, eColumnTypes.ColumnField) = ewec

                    Me.Rows(iLayer + 1).Tag = layer

                Next iLayer

            End Sub

            Protected Overrides Sub FinishStyle()
                MyBase.FinishStyle()
                Me.StretchColumnsToFitWidth()
            End Sub

            Protected Overrides Function OnCellValueChanged(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

                MyBase.OnCellValueChanged(p, cell)

                Dim strField As String = Me.FieldAtRow(p.Row)
                Dim layer As cRasterLayer = Me.LayerAtRow(p.Row)

                Try
                    If String.IsNullOrWhiteSpace(strField) Then
                        ' May be nothing
                        Me.m_dtLayerMapping(layer) = String.Empty
                    Else
                        Me.m_dtLayerMapping(layer) = strField
                    End If
                Catch ex As Exception
                End Try

                Return True

            End Function

            Private Function LayerAtRow(ByVal iRow As Integer) As cRasterLayer
                If iRow > 0 And iRow < Me.RowsCount Then
                    Return DirectCast(Me.Rows(iRow).Tag, cRasterLayer)
                End If
                Return Nothing
            End Function

            Private Function FieldAtRow(ByVal iRow As Integer) As String
                If iRow > 0 And iRow < Me.RowsCount Then
                    Return CStr(Me(iRow, eColumnTypes.ColumnField).Value)
                End If
                Return ""
            End Function

            Private Function HasData() As Boolean
                Return (Me.m_aLayers IsNot Nothing)
            End Function

#End Region ' Overrides

        End Class

#End Region ' Private classes

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_lLayers As New List(Of cRasterLayer)
        Private m_bDataValid As Boolean = False
        Private m_data As cEcospaceImportExportXYData = Nothing

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
            Me.m_uic = uic
        End Sub

#End Region ' Constructor

#Region " Public properties "

        Public Property Layers() As cRasterLayer()
            Get
                Return Me.m_lLayers.ToArray()
            End Get
            Set(ByVal aLayers As cRasterLayer())
                Me.m_lLayers.Clear()

                If aLayers Is Nothing Then Return
                If aLayers.Length = 0 Then Return

                Me.m_lLayers.AddRange(aLayers)
            End Set
        End Property

#End Region ' Public properties

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.DesignMode = True) Then Return

            Debug.Assert(Me.m_uic IsNot Nothing)

            Dim f As New cLayerFactoryInternal()

            ' Set default file
            Me.m_tbTarget.Text = Path.Combine(Me.m_uic.Core.OutputPath, Me.m_uic.Core.EcospaceOutputFileLocation("layers"))

            ' Get default layers if needed
            If (Me.m_lLayers.Count = 0) Then
                Me.m_lLayers.AddRange(f.BaseRasterLayers(Me.m_uic))
            End If
            Me.m_grid.Layers = Me.m_lLayers.ToArray()
            Me.m_grid.UIContext = Me.m_uic

            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            For Each layer As cRasterLayer In Me.m_lLayers
                If layer IsNot Nothing Then
                    layer.Dispose()
                End If
            Next
            Me.m_lLayers = Nothing

            MyBase.OnFormClosed(e)
        End Sub

        Private Sub OnBrowseTarget(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnBrowseTarget.Click

            ' Browse via EwE6 open file dialog 
            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim fsc As cFileSaveCommand = TryCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim strFileFilter As String = SharedResources.FILEFILTER_CSV

            ' Sanity check
            If fsc Is Nothing Then Return

            If String.IsNullOrEmpty(Me.m_tbTarget.Text) Then
                fsc.Invoke(strFileFilter)
            Else
                fsc.Invoke(Me.m_tbTarget.Text, strFileFilter)
            End If

            If (fsc.Result = Windows.Forms.DialogResult.OK) Then
                Me.m_tbTarget.Text = fsc.FileName
            End If

        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_bntOK.Click

            If Not Me.SaveMappedLayers() Then Return

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

#End Region ' Events

#Region " Internals "

        Private Function SaveMappedLayers() As Boolean

            ' ToDo: localize this method

            Dim dtMappings As Dictionary(Of cRasterLayer, String) = Me.m_grid.Mappings()
            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
            Dim lstrFields As New List(Of String)
            Dim strField As String = ""
            Dim strFile As String = Me.m_tbTarget.Text
            Dim layer As cRasterLayer = Nothing
            Dim iRow As Integer = 0
            Dim iCol As Integer = 0
            Dim iCell As Integer = 0

            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core, My.Resources.STATUS_APPLYVALUES)

            Try

                ' Populate local data
                For Each layer In dtMappings.Keys
                    strField = dtMappings(layer).Trim
                    If Not String.IsNullOrWhiteSpace(strField) Then
                        If (lstrFields.IndexOf(strField) = -1) Then
                            lstrFields.Add(strField)
                        End If
                    End If
                Next

                ' Create data
                Me.m_data = New cEcospaceImportExportXYData(bm, lstrFields.ToArray())

                ' Store layer
                For iRow = 1 To bm.InRow
                    For iCol = 1 To bm.InCol
                        ' Populate data
                        For Each layer In dtMappings.Keys
                            strField = dtMappings(layer)
                            If Not String.IsNullOrEmpty(strField.Trim) Then
                                Me.m_data.Value(iRow, iCol, strField) = CSng(layer.Value(iRow, iCol))
                            End If
                        Next layer
                    Next iCol
                Next iRow

                Me.m_data.WriteXYFile(strFile, Me.ColField, Me.RowField)

            Catch ex As Exception

            End Try

            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

            ' Log this
            ' ToDo: globalize this
            Dim msg As New cMessage(String.Format("Layer data exported to '{0}'", strFile), eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.EcoSpace, eMessageImportance.Information)
            Me.m_uic.Core.Messages.SendMessage(msg)

            Return True

        End Function

        Private Property RowField() As String
            Get
                Return Me.m_tbRow.Text
            End Get
            Set(ByVal value As String)
                Me.m_tbRow.Text = value
            End Set
        End Property

        Private Property ColField() As String
            Get
                Return Me.m_tbCol.Text
            End Get
            Set(ByVal value As String)
                Me.m_tbCol.Text = value
            End Set
        End Property

        Private Sub UpdateControls()

            'Me.m_cmbRow.Enabled = (Me.m_cmbRow.Items.Count > 0)
            'Me.m_cmbCol.Enabled = (Me.m_cmbCol.Items.Count > 0)

            'Me.m_grid.Enabled = Me.m_bDataValid
            'Me.m_bntOK.Enabled = Me.m_bDataValid

        End Sub

#End Region ' Internals

    End Class

End Namespace ' Ecospace.Basemap
