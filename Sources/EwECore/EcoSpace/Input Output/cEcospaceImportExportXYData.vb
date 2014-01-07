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

Imports System.IO
Imports System.Text
Imports EwEUtils.Utilities
Imports EwEUtils.SpatialData

#End Region ' Imports

' ToDo_JS: merge with EcospaceCSVResultWriter
' ToDo_JS: enable data access via ISpatialRaster

' There is a high degree of overlap in the read/write logic here and in cEcospaceCSVResultWriter. That is silly.
' Moreover, it would be really nice if the logic presented here would be available to the spatial assets plugin via datasets.
' This is probably best accomplished by making this class and the EcospaceResultsWriter both use an cEcospaceLayer to provide access to their data.
' The plugin can then wrap this class as a IDataProvider to perform its import and export magic.

''' -----------------------------------------------------------------------
''' <summary>
''' Helper class for importing and exporting XY data from text files.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cEcospaceImportExportXYData

#Region " Private classes "

    Private Class cEcospaceImportExportRaster
        Implements ISpatialRaster

        Private m_parent As cEcospaceImportExportXYData = Nothing
        Private m_strField As String = ""

        Public Sub New(parent As cEcospaceImportExportXYData, strField As String)
            Me.m_parent = parent
            Me.m_strField = strField
        End Sub

        Public Function Cell(iRow As Integer, iCol As Integer, Optional dNoDataValue As Double = -9999.0) As Double _
            Implements EwEUtils.SpatialData.ISpatialRaster.Cell
            Return Convert.ToDouble(Me.m_parent.Value(iRow, iCol, Me.m_strField))
        End Function

        Public Function CellSize() As Double _
            Implements EwEUtils.SpatialData.ISpatialRaster.CellSize
            Return Me.m_parent.m_bm.CellSize
        End Function

        Public Function Max() As Double _
            Implements EwEUtils.SpatialData.ISpatialRaster.Max
            Me.CalculateStats()
            Return Me.m_dMax
        End Function

        Public Function Mean() As Double _
            Implements EwEUtils.SpatialData.ISpatialRaster.Mean
            Me.CalculateStats()
            Return Me.m_dMean
        End Function

        Public Function Min() As Double _
            Implements EwEUtils.SpatialData.ISpatialRaster.Min
            Me.CalculateStats()
            Return Me.m_dMin
        End Function

        Public Function NoData() As Single _
            Implements EwEUtils.SpatialData.ISpatialRaster.NoData
            Return cCore.NULL_VALUE
        End Function

        Public Function NumCols() As Integer _
            Implements EwEUtils.SpatialData.ISpatialRaster.NumCols
            Return Me.m_parent.m_bm.InCol
        End Function

        Public Function NumRows() As Integer _
            Implements EwEUtils.SpatialData.ISpatialRaster.NumRows
            Return Me.m_parent.m_bm.InRow
        End Function

        Public Function NumValueCells() As Long _
            Implements EwEUtils.SpatialData.ISpatialRaster.NumValueCells
            Me.CalculateStats()
            Return Me.m_lNumValueCells
        End Function

        Public Function Save(strFile As String) As Boolean _
            Implements EwEUtils.SpatialData.ISpatialRaster.Save
            Return False
        End Function

        Public Function StandardDeviation() As Double _
            Implements EwEUtils.SpatialData.ISpatialRaster.StandardDeviation
            Return Me.m_dStdDev
        End Function

        Public Function TopLeft() As System.Drawing.PointF _
            Implements EwEUtils.SpatialData.ISpatialRaster.TopLeft
            Return Me.m_parent.m_bm.PosTopLeft
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            GC.SuppressFinalize(Me)
        End Sub

        Private m_bStatsCalculated As Boolean = False
        Private m_lNumValueCells As Long = 0
        Private m_dMax As Double = 0
        Private m_dMin As Double = 0
        Private m_dMean As Double = 0
        Private m_dStdDev As Double = 0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the statistics of the data wrapped by this class.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub CalculateStats()

            If (Me.m_bStatsCalculated) Then Return

            Dim dVal As Double = 0
            Dim dNoData As Double = cCore.NULL_VALUE
            Dim dTot As Double = 0
            Dim dMax As Double = Double.MinValue
            Dim dMin As Double = Double.MaxValue
            Dim dStdDev As Double = cCore.NULL_VALUE
            Dim iNumCols As Integer = Me.NumCols
            Dim iNumRows As Integer = Me.NumRows

            Me.m_lNumValueCells = 0

            Try

                For iRow As Integer = 1 To iNumRows
                    For iCol As Integer = 1 To iNumCols
                        dVal = Me.Cell(iRow, iCol)
                        If (dVal <> dNoData) And (dVal <> cCore.NULL_VALUE) Then
                            Me.m_lNumValueCells += 1
                            dMax = Math.Max(dMax, dVal)
                            dMin = Math.Min(dMin, dVal)
                            dTot += dVal
                        End If
                    Next
                Next

                If (Me.m_lNumValueCells > 0) Then
                    Me.m_dMax = dMax
                    Me.m_dMin = dMin
                    Me.m_dMean = dTot / Me.m_lNumValueCells

                    ' Standard deviation
                    dTot = 0

                    For iRow As Integer = 1 To iNumRows
                        For iCol As Integer = 1 To iNumCols
                            dVal = Me.Cell(iRow, iCol)
                            If (dVal <> dNoData) And (dVal <> cCore.NULL_VALUE) Then
                                dTot += (dVal - Me.m_dMean) * (dVal - Me.m_dMean)
                            End If
                        Next
                    Next
                    Me.m_dStdDev = Math.Sqrt(dTot / Me.m_lNumValueCells)
                Else
                    Me.m_dMin = cCore.NULL_VALUE
                    Me.m_dMax = cCore.NULL_VALUE
                    Me.m_dMean = cCore.NULL_VALUE
                    Me.m_dStdDev = cCore.NULL_VALUE
                End If

            Catch ex As Exception
                ' Overflow?!
            End Try

            Me.m_bStatsCalculated = True

        End Sub

    End Class ' cEcospaceImportExportRaster

#End Region ' Private classes

#Region " Private vars "

    Public Shared cMAPPING_IMPLICIT As String = My.Resources.CoreDefaults.CORE_DEFAULT

    Private m_bm As cEcospaceBasemap = Nothing

    ''' <summary>Buffer that holds the data to read or write.</summary>
    ''' <remarks>To save on memory we allow the use of value callbacks per field as an alternative to the buffer.</remarks>
    Private m_buffer As New Dictionary(Of String, Object())
    ''' <summary>All defined data fieldds.</summary>
    Private m_astrFields As String() = Nothing

    Private m_bRowColImplicit As Boolean = False

#End Region ' Private vars

#Region " Construction "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Construct a new instance of this class.
    ''' </summary>
    ''' <param name="bm">The <see cref="cEcospaceBasemap"/> to operate onto.</param>
    ''' <param name="astrFields">An optional array of field names.</param>
    ''' -------------------------------------------------------------------
    Public Sub New(bm As cEcospaceBasemap, _
                   Optional ByVal astrFields() As String = Nothing)

        Debug.Assert(bm IsNot Nothing)

        Me.m_bm = bm
        Me.Fields = astrFields

    End Sub

#End Region ' Construction

#Region " Read & Write "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns all duplicate <see cref="Fields">field names defined in the import/export data</see>.
    ''' </summary>
    ''' <returns>An array with duplicate <see cref="Fields">field names</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Function DuplicateFields() As String()

        Dim htNames As New HashSet(Of String)
        Dim lstrDuplicates As New List(Of String)
        Dim strField As String = ""

        For Each strField In Me.m_astrFields
            If htNames.Contains(strField) Then
                If Not lstrDuplicates.Contains(strField) Then
                    lstrDuplicates.Add(strField)
                End If
            Else
                htNames.Add(strField)
            End If
        Next

        lstrDuplicates.Sort()
        Return lstrDuplicates.ToArray

    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write data to a XY text file. The format of the file is
    ''' 'col,row[,<see cref="Fields"/>]*', with a configurable the separator character.
    ''' Field names encountered in the file can be found in <see cref="Fields"/>.
    ''' </summary>
    ''' <param name="strFile">The name of the file to write.</param>
    ''' <param name="separator">The separator character to use. By default, CSV
    ''' values are separated by commas.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ReadXYFields(ByVal strFile As String, _
                                 Optional ByVal separator As Char = ","c) As Boolean

        Dim tr As TextReader = Nothing
        Dim strLine As String = ""
        Dim astrFields As String() = Nothing
        Dim bSuccess As Boolean = True

        Try
            tr = New StreamReader(strFile)
        Catch ex As Exception
            Return False
        End Try

        Try
            ' Read fields line
            strLine = tr.ReadLine()
            astrFields = cStringUtils.SplitQualified(strLine, separator)

            ' Clean up
            For i As Integer = 0 To astrFields.Length - 1
                astrFields(i) = astrFields(i).Trim
            Next

            Me.Fields = astrFields

        Catch ex As Exception
            bSuccess = False
        End Try

        tr.Close()
        Return bSuccess

    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write data to a XY text file. The format of the file is
    ''' 'col,row[,<see cref="Fields"/>]*', with a configurable the separator character.
    ''' Field names encountered in the file can be found in <see cref="Fields"/>.
    ''' </summary>
    ''' <param name="strFile">The name of the file to write.</param>
    ''' <param name="separator">The separator character to use. By default, CSV
    ''' values are separated by commas.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ReadXYFile(ByVal strFile As String, _
                               ByVal strRowField As String, _
                               ByVal strColField As String, _
                               Optional ByVal separator As Char = ","c) As Boolean

        If Me.m_astrFields.Length = 0 Then
            If Not Me.ReadXYFields(strFile) Then Return False
        End If

        Dim tr As TextReader = Nothing
        Dim strLine As String = ""
        Dim astrFields As String() = Me.Fields
        Dim astrValues As String() = Nothing
        Dim iField As Integer
        Dim sValue As Single = 0.0!
        Dim iColField As Integer = -1
        Dim iRowField As Integer = -1
        Dim bSuccess As Boolean = True

        Try
            tr = New StreamReader(strFile)
        Catch ex As Exception
            Return False
        End Try

        Try
            ' Read fields line
            strLine = tr.ReadLine()

            iColField = Array.IndexOf(astrFields, strColField)
            iRowField = Array.IndexOf(astrFields, strRowField)

            If (iColField = -1 Or iRowField = -1) Then Return False

            While (tr.Peek() <> -1)
                strLine = tr.ReadLine()
                astrValues = strLine.Split(separator)

                For iField = 0 To astrFields.Length - 1
                    If (iField <> iRowField) And (iField <> iColField) Then
                        Me.Value(CInt(astrValues(iRowField)), CInt(astrValues(iColField)), astrFields(iField)) = astrValues(iField)
                    End If
                Next
            End While

        Catch ex As Exception
            bSuccess = False
        End Try

        tr.Close()
        Return bSuccess

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Write data to a XY text file. The format of the file is
    ''' '<paramref name="strColField"/>,<paramref name="strRowField"/>[,<see cref="Fields"/>]*'
    ''' </summary>
    ''' <param name="strFile">The file to write to.</param>
    ''' <param name="strColField">CSV header for 'col' field</param>
    ''' <param name="strRowField">CSV header for 'row' field</param>
    ''' <param name="bWaterCellsOnly">If true, only water cell data is written to the file.</param>
    ''' <returns>True if successful.</returns>
    ''' -------------------------------------------------------------------
    Public Function WriteXYFile(ByVal strFile As String, _
                                ByVal strColField As String, _
                                ByVal strRowField As String, _
                                Optional bWaterCellsOnly As Boolean = True) As Boolean

        If (Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True)) Then
            Return False
        End If

        Dim strm As StreamWriter = Nothing
        Dim lstrFields As New List(Of String)
        Dim depth As cEcospaceLayerDepth = Me.m_bm.LayerDepth

        Try
            strm = New StreamWriter(strFile)
        Catch ex As Exception
            Return False
        End Try

        lstrFields.AddRange(Me.m_astrFields)
        lstrFields.Remove(strRowField)
        lstrFields.Remove(strColField)

        ' Write header line
        strm.Write(cStringUtils.ToCSVField(strColField))
        strm.Write(",")
        strm.Write(cStringUtils.ToCSVField(strRowField))
        strm.Write(",Lon,Lat")
        For iField As Integer = 0 To lstrFields.Count - 1
            strm.Write(",")
            strm.Write(cStringUtils.ToCSVField(Me.Fields(iField).Trim))
        Next
        strm.WriteLine()

        ' Write content
        For iRow As Integer = 1 To Me.m_bm.InRow
            For iCol As Integer = 1 To Me.m_bm.InCol

                ' Water cell filter
                If depth.IsWaterCell(iRow, iCol) Or Not bWaterCellsOnly Then
                    strm.Write(cStringUtils.FormatNumber(iCol))
                    strm.Write(",")
                    strm.Write(cStringUtils.FormatNumber(iRow))
                    strm.Write(",")
                    strm.Write(cStringUtils.FormatNumber(Me.m_bm.ColToLon(iCol)))
                    strm.Write(",")
                    strm.Write(cStringUtils.FormatNumber(Me.m_bm.RowToLat(iRow)))
                    For iField As Integer = 0 To Me.Fields.Length - 1
                        strm.Write(",")
                        Dim val As Object = Me.Value(iRow, iCol, Me.Fields(iField))
                        If (val IsNot Nothing) Then
                            Select Case val.GetType
                                Case GetType(Single), GetType(Double), GetType(Integer)
                                    strm.Write(cStringUtils.FormatNumber(val))
                                Case GetType(Boolean), GetType(String)
                                    strm.Write(cStringUtils.ToCSVField(CStr(val)))
                                Case Else
                            End Select
                        End If
                    Next iField
                    strm.WriteLine()
                End If
            Next iCol
        Next iRow

        strm.Flush()
        strm.Close()

        Return True

    End Function

#End Region ' Read & Write

#Region " Properties "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the fields that data is associated with.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Property Fields() As String()
        Get
            Return Me.m_astrFields
        End Get
        Set(ByVal value As String())

            If (value Is Nothing) Then
                Me.m_bRowColImplicit = True
            Else
                Me.m_bRowColImplicit = (value.Length = 0)
            End If

            If (Me.m_bRowColImplicit) Then
                Me.m_astrFields = New String() {cEcospaceImportExportXYData.cMAPPING_IMPLICIT}
            Else
                Dim lFields As New List(Of String)
                For Each strField As String In value
                    If Not String.IsNullOrWhiteSpace(strField) Then
                        lFields.Add(strField.Trim)
                    End If
                Next
                Me.m_astrFields = lFields.ToArray
            End If

            ' Clear
            Me.m_buffer.Clear()

            ' Create storage
            For Each strField As String In Me.Fields
                Dim asCells(Me.NumCells) As Object
                Me.m_buffer.Add(strField, asCells)
            Next

        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set a value in this class.
    ''' </summary>
    ''' <param name="iRow">One-based row index to access a value for.</param>
    ''' <param name="iCol">One-based column index to access a value for.</param>
    ''' <param name="strField">Optional field to access a value for.</param>
    ''' -------------------------------------------------------------------
    Public Property Value(ByVal iRow As Integer, ByVal iCol As Integer, _
                          Optional ByVal strField As String = "") As Object
        Get
            Return Me.Value(Me.Seq(iRow, iCol), strField)
        End Get
        Set(ByVal value As Object)
            Me.Value(Me.Seq(iRow, iCol), strField) = value
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set a value in this class.
    ''' </summary>
    ''' <param name="iCell">The one-based cell sequential index to access
    ''' a value for.</param>
    ''' <param name="strField">Optional field to access a value for.</param>
    ''' -------------------------------------------------------------------
    Public Property Value(ByVal iCell As Integer, _
                          Optional ByVal strField As String = "") As Object
        Get
            If String.IsNullOrEmpty(strField) Then
                strField = cEcospaceImportExportXYData.cMAPPING_IMPLICIT
            End If
            Return Me.m_buffer(strField)(iCell)
        End Get
        Set(ByVal value As Object)
            If String.IsNullOrWhiteSpace(strField) Then
                strField = cEcospaceImportExportXYData.cMAPPING_IMPLICIT
            End If
            Me.m_buffer(strField)(iCell) = value
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get a cell sequential number from a (row, col) pair.
    ''' </summary>
    ''' <param name="iRow">One-based row index to get a cell for.</param>
    ''' <param name="iCol">One-based column index to get a cell for.</param>
    ''' <returns>A one-based sequence number for a cell.</returns>
    ''' -------------------------------------------------------------------
    Public Function Seq(ByVal iRow As Integer, ByVal iCol As Integer) As Integer
        If (Me.m_bm Is Nothing) Then Return 0
        'Zero base Cell
        Return (iRow - 1) * Me.m_bm.InCol + (iCol - 1)
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of cells in this data.
    ''' </summary>
    ''' <returns>The number of cells in this data.</returns>
    ''' -------------------------------------------------------------------
    Public Function NumCells() As Integer
        If (Me.m_bm Is Nothing) Then Return 0
        Return Me.m_bm.InCol * Me.m_bm.InRow
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns true if no row and column fields have been defined.
    ''' </summary>
    ''' <returns>True if no row and column fields have been defined.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsRowColImplicit() As Boolean
        Return Me.m_bRowColImplicit
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns data in the form of a <see cref="ISpatialRaster"/>>
    ''' </summary>
    ''' <returns>True if no row and column fields have been defined.</returns>
    ''' -------------------------------------------------------------------
    Public Function ToRaster(Optional ByVal strField As String = "") As ISpatialRaster
        Return New cEcospaceImportExportRaster(Me, strField)
    End Function

#End Region ' Properties

End Class