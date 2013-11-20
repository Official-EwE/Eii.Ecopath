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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada;
' Copyright 2013 - Ecopath International Initiative, Barcelona, Spain.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports DotSpatial.Data
Imports EwECore
Imports EwESpatialAssetsPlugin
Imports EwESpatialAssetsPlugin.SpatialData
Imports EwEUtils.SpatialData

#End Region ' Imports

Public Class cNonSpatialCSVGridDataset
    Inherits cMultiFileDataSetPlugin

    Private m_raster As ISpatialRaster = Nothing

    Public Sub New()
        MyBase.New()
        Me.m_strName = "Non-spatial CSV grids"
        Me.DataDescription = "A collection of non-spatial CSV grids that match the width and height of an Ecospace scenario."
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
            Return String.Format("{0}|*.csv", "Non-spatial CSV grids")
        End Get
    End Property

    Private Property Name As String

    Protected Overrides Function LoadSource() As Boolean

        Dim strFileName As String = Me.SourceFileName()

        If (Me.m_raster IsNot Nothing) Then Return True

        If (System.IO.File.Exists(strFileName)) Then
            Dim reader As New StreamReader(strFileName)
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap

            Dim rs As IRaster = New DotSpatial.Data.Raster(Of Single)
            rs = New Raster(Of Single)(bm.InRow, bm.InCol)
            rs.Bounds = cDotSpatialUtils.EcospaceToBounds(bm.PosTopLeft, bm.PosBottomRight, bm.CellSize)
            rs.NoDataValue = cCore.NULL_VALUE

            Me.ReadBody(reader, rs)
            Me.StoreExtent(rs.Extent)

            Me.m_raster = New cSpatialRaster(rs)

            Me.LogMessage("Loaded CSV grid file " & Me.m_raster.ToString & " from " & strFileName, eStatusFlags.OK)
        Else
            Me.LogMessage("Failed to find CSV grid file " & strFileName, eStatusFlags.ErrorEncountered)
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


    Protected Sub ReadBody(ByVal reader As TextReader, ByRef rs As IRaster)

        Dim nCells As Integer = rs.NumRows * rs.NumColumns
        Dim iRow As Integer = 0
        Dim strLine As String = ""

        ' Skip column header
        strLine = reader.ReadLine()

        ' Start processing body block
        strLine = reader.ReadLine()
        While Not String.IsNullOrEmpty(strLine) And iRow < rs.NumRows
            Dim bits As String() = strLine.Split(","c)
            For iCol As Integer = 1 To Math.Min(bits.Length - 1, rs.NumColumns)
                If Not String.IsNullOrEmpty(bits(iCol).Trim) Then
                    rs.Value(iRow, iCol - 1) = CDbl(bits(iCol))
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
            Return "Plug-in that provides direct access to non-spatial CSV grids"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IPlugin.Name"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property PluginName As String
        Get
            Return "DotSpatial.NonSpatialCSVGrid"
        End Get
    End Property

#End Region ' Plug-in implementation

End Class
