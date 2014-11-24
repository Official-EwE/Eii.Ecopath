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
Imports System.Drawing
Imports DotSpatial.Data
Imports EwECore
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    Public Class cCSVFilesDataSetPlugin
        Inherits cMultiFileDataSetPlugin

        Private m_reader As cEcospaceImportExportXYData = Nothing
        Private m_bLoaded As Boolean = False

        Public Sub New()
            MyBase.New()
            ' Default name and description
            Me.m_strName = My.Resources.DATASET_CSV_NAME
            Me.DataDescription = My.Resources.DATASET_CSV_DESCRIPTION
        End Sub

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the dialog read filter for files supported by the AAAS reader.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property DialogReadFilter(ByVal bRaster As Boolean, _
                                                            ByVal bImage As Boolean, _
                                                            ByVal bVector As Boolean) As String
            Get
                Return cStringUtils.Localize("{0}|*.csv", My.Resources.DIALOGFILTER_CSV)
            End Get
        End Property

        Public Overrides Function LockDataAtT(datetime As Date, dCellSize As Double, ptfTL As System.Drawing.PointF, ptfBR As System.Drawing.PointF) As Boolean

            ' First set file time etc to find correct file index
            If Not MyBase.LockDataAtT(datetime, dCellSize, ptfTL, ptfBR) then Return false

            If (Not Me.IsLocked) Then
                Dim reader As New cEcospaceImportExportXYData(Me.m_core.EcospaceBasemap)
                If reader.ReadXYFields(Me.SourceFileName) Then
                    Me.m_reader = reader
                    Return True
                End If
            End If

            Return False

        End Function

        Public Overrides Function IsLocked() As Boolean
            Return (Me.m_reader IsNot Nothing)
        End Function

        Public Overrides Function UnlockData() As Boolean
            Me.m_reader = Nothing
            Return MyBase.UnlockData()
        End Function

        Protected Overrides Function LoadSource() As Boolean

            Dim strFileName As String = Me.SourceFileName()

            Me.m_bLoaded = Me.m_reader.ReadXYFile(strFileName, Me.RowField, Me.ColumnField)
            If Me.m_bLoaded Then
                Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
                Me.StoreExtent(New Extent(bm.PosTopLeft.X, bm.PosBottomRight.Y, _
                                          bm.PosBottomRight.X, bm.PosTopLeft.Y))
                Me.LogMessage("Loaded Ecospace CSV from " & strFileName, eStatusFlags.OK)
            Else
                Me.LogMessage("Failed to load Ecospace CSV " & strFileName, eStatusFlags.ErrorEncountered)
            End If

            Return Me.m_bLoaded

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetRaster"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetRaster(converter As ISpatialDataConverter, strLayerName As String) As ISpatialRaster

            If (Not Me.IsLocked) Then Return Nothing

            Dim strDataField As String = strLayerName

            'If (converter Is Nothing) Then Return Nothing
            'If (Not TypeOf converter Is cCSVFileConverterPlugin) Then Return Nothing
            'Dim cv As cCSVFileConverterPlugin = DirectCast(converter, cCSVFileConverterPlugin)
            'If Not String.IsNullOrWhiteSpace(cv.DataField) Then
            '    strDataField = cv.DataField
            'End If

            If Me.LoadSource() Then
                Return Me.m_reader.ToRaster(strDataField)
            Else
                Return Nothing
            End If

        End Function

        Public Overrides Function IsConfigured() As Boolean
            Return MyBase.IsConfigured() 
        End Function

#End Region ' Overrides

#Region " Plug-in implementation "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                ' ToDo: globalize this
                Return "Plug-in that provides direct access to row x column data in CSV format, catered to fit the spatial extent and grid size of Ecospace, without requiring GDAL"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.DataSet.0410"
            End Get
        End Property

        Public Overrides ReadOnly Property ConversionFormat As String
            Get
                Return ""
                ' Do not use conversion (yet)
                'Return "DotSpatialRaster.Special.CSVFile"
            End Get
        End Property

#End Region ' Plug-in implementation

#Region " Public "

        '        Public ReadOnly Property Reader As cEcospaceImportExportXYData
        '            Get
        '                ' For Converter
        '                Return Me.m_reader
        '            End Get
        '        End Property

        Public Property RowField As String = "Row"
        Public Property ColumnField As String = "Column"

#End Region ' Public

    End Class

End Namespace
