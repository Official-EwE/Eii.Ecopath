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

    Public Class cASCIIFilesDataSetPlugin
        Inherits cMultiFileDataSetPlugin

        Private m_raster As ISpatialRaster = Nothing

        Public Sub New()
            MyBase.New()
            ' Default name and description
            Me.m_strName = My.Resources.DATASET_ASCII_NAME
            Me.DataDescription = My.Resources.DATASET_ASCII_DESCRIPTION
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
                Return String.Format("{0}|*.asc", My.Resources.DIALOGFILTER_ASCII)
            End Get
        End Property

        Private Property Name As String

        Protected Overrides Function LoadSource() As Boolean

            Dim strFileName As String = Me.SourceFileName()
            Dim rs As IRaster = New DotSpatial.Data.Raster(Of Single)

            If (Me.m_raster IsNot Nothing) Then Return True

            If (System.IO.File.Exists(strFileName)) Then
                Dim reader As New StreamReader(strFileName)
                Me.ReadHeader(reader, rs)
                Me.ReadBody(reader, rs)
                Me.StoreExtent(rs.Extent)
                Me.m_raster = New cSpatialRaster(rs)

                Me.LogMessage("Loaded ASCII " + Me.m_raster.ToString + cStringUtils.vbTab + strFileName, eStatusFlags.OK)
            Else
                Me.LogMessage("Failed to find ASCII raster" + cStringUtils.vbTab + strFileName, eStatusFlags.ErrorEncountered)
            End If
            Return (Me.m_raster IsNot Nothing)

        End Function

        Public Overrides Function UnlockData() As Boolean
            Me.m_raster = Nothing
            Return MyBase.UnlockData()
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetRaster"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetRaster(converter As ISpatialDataConverter, strLayerName As String) As ISpatialRaster

            If (Not Me.IsLocked) Then Return Nothing
            Me.LoadSource()
            Return Me.m_raster

        End Function

#End Region ' Overrides

#Region " Internals "

        Protected Sub ReadHeader(ByVal reader As TextReader, ByRef rs As IRaster)

            Dim nCols As Integer = 0
            Dim nRows As Integer = 0
            Dim xllcorner As Single = 0.0
            Dim yllcorner As Single = 0.0
            Dim sCellSize As Single = 0.0
            Dim sValueNone As Single = -9999
            Dim strHeadName As String = ""
            Dim strValue As String = ""
            Dim strLine As String

            Do
                strLine = reader.ReadLine()
                If Not String.IsNullOrEmpty(strLine) Then
                    While strLine.IndexOf("  ") > 0
                        strLine = strLine.Replace("  ", " ")
                    End While

                    Dim astrBits() As String = strLine.Split(" "c)
                    strHeadName = astrBits(0).Trim().ToLower
                    strValue = astrBits(1).Trim().ToLower
                End If

                Select Case strHeadName

                    Case "ncols" : nCols = CInt(strValue)
                    Case "nrows" : nRows = CInt(strValue)
                    Case "xllcorner" : xllcorner = CSng(strValue)
                    Case "yllcorner" : yllcorner = CSng(strValue)
                    Case "cellsize" : sCellSize = CSng(strValue)
                    Case "nodatavalue", "nodata_value" : sValueNone = CSng(strValue)

                End Select
                ' Debug.Assert(Not String.IsNullOrEmpty(strLine), Me.ToString + ".ReadHeader() file contains no data.")
            Loop Until (strHeadName = "nodatavalue" Or strHeadName = "nodata_value" Or String.IsNullOrEmpty(strLine))

            rs = New Raster(Of Single)(nRows, nCols)
            rs.Bounds = cDotSpatialUtils.EcospaceToBounds(New PointF(xllcorner, yllcorner + nRows * sCellSize), New PointF(xllcorner + nCols * sCellSize, yllcorner), sCellSize)
            rs.NoDataValue = sValueNone

        End Sub

        Protected Sub ReadBody(ByVal reader As TextReader, ByRef rs As IRaster)

            Dim nCells As Integer = rs.NumRows * rs.NumColumns
            Dim iRow As Integer = 0
            Dim strLine As String = ""

            strLine = reader.ReadLine()
            While Not String.IsNullOrEmpty(strLine) And iRow < rs.NumRows
                Dim bits As String() = strLine.Split(" "c)
                For iCol As Integer = 0 To Math.Min(bits.Length - 1, rs.NumColumns)
                    If Not String.IsNullOrEmpty(bits(iCol).Trim) Then
                        rs.Value(iRow, iCol) = CDbl(bits(iCol))
                    End If
                Next
                iRow += 1
                strLine = reader.ReadLine()
            End While

        End Sub

#End Region ' Internals

#Region " Plug-in implementation "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Plug-in that provides direct access to ASCII files, without requiring GDAL"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.AAASFileSetPlugin"
            End Get
        End Property

#End Region ' Plug-in implementation

    End Class

End Namespace
