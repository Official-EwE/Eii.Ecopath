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
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Xml
Imports DotSpatial.Data
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

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
                Return String.Format("{0}|*.csv", My.Resources.DIALOGFILTER_CSV)
            End Get
        End Property

        Private Property Name As String

        Public Overrides Function LockDataAtT(datetime As Date, dCellSize As Double, ptfTL As System.Drawing.PointF, ptfBR As System.Drawing.PointF) As Boolean
            If (Not Me.IsLocked) Then
                Me.m_reader = New cEcospaceImportExportXYData(Me.m_core.EcospaceBasemap)
            End If
            Return MyBase.LockDataAtT(datetime, dCellSize, ptfTL, ptfBR)
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

            Me.m_bLoaded = False

             If (System.IO.File.Exists(strFileName)) Then
                Me.m_bLoaded = Me.m_reader.ReadXYFile(strFileName)
            End If

            If Me.m_bLoaded Then
                Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
                Me.StoreExtent(New Extent(bm.PosBottomRight.X, bm.PosBottomRight.Y, _
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
            Return Me.m_reader.ToRaster(strLayerName)

        End Function

#End Region ' Overrides

#Region " Plug-in implementation "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Plug-in that provides direct access to CSV files catered to fit the spatial extent and grid size of Ecospace, without requiring GDAL"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "EcospaceCSVFileSetPlugin"
            End Get
        End Property

#End Region ' Plug-in implementation

    End Class

End Namespace
