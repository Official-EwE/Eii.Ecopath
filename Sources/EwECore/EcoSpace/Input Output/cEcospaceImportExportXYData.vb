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

#End Region ' Imports

' ToDo_JS: merge with EcospaceCSVResultWriter

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

#Region " Private vars "

    ' ToDo: globalize this
    Public Shared cMAPPING_IMPLICIT As String = "(default)"

    Private m_bm As cEcospaceBasemap = Nothing

    Private m_readbuffer As New Dictionary(Of String, Object())
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
    ''' Returns all duplicate field names defined in the import/export data.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function NumDuplicateFields() As String()

        Dim htNames As New HashSet(Of String)
        Dim lstrDuplicates As New List(Of String)
        Dim strField As String = ""

        For Each strField In Me.m_astrFields
            If Not String.IsNullOrWhiteSpace(strField) Then
                If htNames.Contains(strField) Then
                    If Not lstrDuplicates.Contains(strField) Then
                        lstrDuplicates.Add(strField)
                    End If
                Else
                    htNames.Add(strField)
                End If
            End If
        Next

        lstrDuplicates.Sort()
        Return lstrDuplicates.ToArray

    End Function

    Public Function ReadXYFile(strFile As String, Optional ByVal separator As Char = ","c) As Boolean

        Dim tr As TextReader = Nothing
        Dim strLine As String = ""
        Dim astrFields As String() = Nothing
        Dim astrValues As String() = Nothing
        Dim iCell, iField As Integer
        Dim sValue As Single = 0.0!
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

            iCell = 0
            While (tr.Peek() <> -1) And (iCell < Me.NumCells)
                strLine = tr.ReadLine()
                astrValues = strLine.Split(separator)

                For iField = 0 To astrFields.Length - 1
                    Me.Value(iCell, astrFields(iField)) = astrValues(iField)
                Next
                ' Next
                iCell += 1
            End While

        Catch ex As Exception
            bSuccess = False
        End Try

        tr.Close()
        Return bSuccess

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Write data to a XY text file..
    ''' </summary>
    ''' <param name="strFile">The file to write to.</param>
    ''' <returns>True if successful.</returns>
    ''' -------------------------------------------------------------------
    Public Function WriteXYFile(ByVal strFile As String, strColField As String, strRowField As String) As Boolean

        If (Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True)) Then
            Return False
        End If

        Dim strm As StreamWriter = Nothing
        Dim lstrFields As New List(Of String)

        Try
            strm = New StreamWriter(strFile)
        Catch ex As Exception
            Return False
        End Try

        lstrFields.AddRange(Me.m_astrFields)
        lstrFields.Remove(strRowField)
        lstrFields.Remove(strColField)

        ' Write header line
        strm.Write(strColField)
        strm.Write(",")
        strm.Write(strRowField)
        For iField As Integer = 0 To lstrFields.Count - 1
            strm.Write(",")
            strm.Write(cStringUtils.ToCSVField(Me.Fields(iField).Trim))
        Next
        strm.WriteLine()

        ' Write content
        For iRow As Integer = 1 To Me.m_bm.InRow
            For iCol As Integer = 1 To Me.m_bm.InCol
                strm.Write(iCol)
                strm.Write(",")
                strm.Write(iRow)
                For iField As Integer = 0 To Me.Fields.Length - 1
                    strm.Write(",")
                    Dim val As Object = Me.Value(iRow, iCol, Me.Fields(iField))
                    Select Case val.GetType
                        Case GetType(Single), GetType(Double), GetType(Integer)
                            strm.Write(cStringUtils.FormatNumber(val))
                        Case GetType(Boolean), GetType(String)
                            strm.Write(CStr(val))
                        Case Else
                    End Select
                Next iField
                strm.WriteLine()
            Next iCol
        Next iRow

        strm.Flush()
        strm.Close()

        Return True

    End Function

#End Region ' Read & Write

#Region " Properties "

    ''' <summary>
    ''' Get/set the fields that data is associated with.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Property Fields() As String()
        Get
            Return Me.m_astrFields
        End Get
        Set(ByVal value As String())

            If value Is Nothing Then
                Me.m_bRowColImplicit = True
            Else
                Me.m_bRowColImplicit = (value.Length = 0)
            End If

            If (Me.m_bRowColImplicit) Then
                Me.m_astrFields = New String() {cEcospaceImportExportXYData.cMAPPING_IMPLICIT}
            Else
                Me.m_astrFields = value
            End If

            ' Clear
            Me.m_readbuffer.Clear()

            ' Create storage
            For Each strField As String In Me.Fields
                Dim asCells(Me.NumCells) As Object
                Me.m_readbuffer.Add(strField, asCells)
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
            Return Me.Value(Me.Cell(iRow, iCol), strField)
        End Get
        Set(ByVal value As Object)
            Me.Value(Me.Cell(iRow, iCol), strField) = value
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set a value in this class.
    ''' </summary>
    ''' <param name="iCell">The zero-based cell sequential index to access
    ''' a value for.</param>
    ''' <param name="strField">Optional field to access a value for.</param>
    ''' -------------------------------------------------------------------
    Public Property Value(ByVal iCell As Integer, _
                          Optional ByVal strField As String = "") As Object
        Get
            If String.IsNullOrEmpty(strField) Then
                strField = cEcospaceImportExportXYData.cMAPPING_IMPLICIT
            End If
            Return Me.m_readbuffer(strField)(iCell)
        End Get
        Set(ByVal value As Object)
            If String.IsNullOrEmpty(strField) Then
                strField = cEcospaceImportExportXYData.cMAPPING_IMPLICIT
            End If
            Me.m_readbuffer(strField)(iCell) = value
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get a cell sequential index from a (row, col) pair.
    ''' </summary>
    ''' <param name="iRow">One-based row index to get a cell for.</param>
    ''' <param name="iCol">One-based column index to get a cell for.</param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Public Function Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Integer
        If (Me.m_bm Is Nothing) Then Return 0
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
    ''' Returns true if no fields have been defined.
    ''' </summary>
    ''' <returns>True if no fields have been defined.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsRowColImplicit() As Boolean
        Return Me.m_bRowColImplicit
    End Function

#End Region ' Properties

End Class