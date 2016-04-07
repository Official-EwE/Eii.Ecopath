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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports System.IO
Imports System.Text
Imports EwEUtils.Utilities
Imports EwEUtils.SpatialData
Imports EwEUtils.Core

#End Region ' Imports

' ToDo: bring in ASCII reader and writer logic from SpatialAssets Plugin

''' -----------------------------------------------------------------------
''' <summary>
''' Helper class for importing and exporting data from ASCII grid files
''' directly to and from Ecospace, without GIS intervention.
''' </summary>
''' -----------------------------------------------------------------------
Friend Class cEcospaceImportExportASCIIData
    Implements IEcospaceImportExport

    Private m_bm As cEcospaceBasemap = Nothing
    Private m_rs As cEcospaceImportExportRaster = Nothing

#Region " Construction "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Construct a new instance of this class.
    ''' </summary>
    ''' <param name="bm">The <see cref="cEcospaceBasemap"/> to operate onto.</param>
    ''' -------------------------------------------------------------------
    Public Sub New(bm As cEcospaceBasemap)

        Debug.Assert(bm IsNot Nothing)

        Me.m_bm = bm
        Me.m_rs = New cEcospaceImportExportRaster(Me, Me.m_bm)

        Throw New NotImplementedException("Nothing here yet")
    End Sub

#End Region ' Construction

    Public Function Read(strFile As String) As Boolean

    End Function

    Public Property Value(iRow As Integer, iCol As Integer, Optional strField As String = "") As Object Implements EwEUtils.Core.IEcospaceImportExport.Value
        Get
            Throw New NotImplementedException("Nothing here yet")
        End Get
        Set(value As Object)
            Throw New NotImplementedException("Nothing here yet")
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns data in the form of a <see cref="ISpatialRaster"/>>
    ''' </summary>
    ''' <returns>A raster.</returns>
    ''' -------------------------------------------------------------------
    Public Function ToRaster(Optional ByVal strField As String = "") As ISpatialRaster _
        Implements IEcospaceImportExport.ToRaster
        Return Me.m_rs
    End Function


    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Read the ASCII header from a text reader.
    ''' </summary>
    ''' <param name="reader">The open stream reader to read from.</param>
    ''' <returns>True if successful.</returns>
    ''' <remarks>
    ''' This method aims to read a complete raster header as described in
    ''' http://resources.esri.com/help/9.3/arcgisengine/com_cpp/GP_ToolRef/Spatial_Analyst_Tools/esri_ascii_raster_format.htm.
    ''' If any of the header fields is missing the method will fail.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Protected Function ReadHeader(ByVal reader As StreamReader) As Boolean

        Dim nCols As Integer = 0
        Dim nRows As Integer = 0
        Dim sXLLpos As Single = 0.0
        Dim bIsCenterX As Boolean = False
        Dim sYLLpos As Single = 0.0
        Dim bIsCenterY As Boolean = False
        Dim sCellSize As Single = 0.0
        Dim sValueNone As Single = -9999

        Dim strField As String = ""
        Dim strValue As String = ""
        Dim strLine As String
        Dim bIsComplete As Boolean = False
        Dim bIsError As Boolean = False
        Dim checksum As Byte = 0

        ' Read the file until EOF or all header fields are read without any errors
        While (Not reader.EndOfStream) And (Not bIsComplete) And (Not bIsError)

            ' Read a line
            strLine = reader.ReadLine()

            ' Be nice
            If Not String.IsNullOrWhiteSpace(strLine) Then

                ' Remove all double spaces
                While strLine.IndexOf("  ") > 0
                    strLine = strLine.Replace("  ", " ")
                End While

                ' Split by space
                Dim astrBits() As String = strLine.Split(" "c)
                strField = astrBits(0)
                strValue = astrBits(1)

                ' Check header field (eliminating tabs etc, lower case)
                Select Case strField.Trim().ToLower

                    Case "ncols"
                        bIsError = Not Integer.TryParse(strValue, nCols)
                        bIsError = bIsError Or (nCols <= 0)
                        checksum = CByte(checksum Or &H1)

                    Case "nrows"
                        bIsError = Not Integer.TryParse(strValue, nRows)
                        bIsError = bIsError Or (nRows <= 0)
                        checksum = CByte(checksum Or &H2)

                    Case "xllcorner"
                        bIsError = Not Single.TryParse(strValue, sXLLpos)
                        bIsCenterX = False
                        checksum = CByte(checksum Or &H4)

                    Case "xllcenter"
                        bIsError = Not Single.TryParse(strValue, sXLLpos)
                        bIsCenterX = True
                        checksum = CByte(checksum Or &H4)

                    Case "yllcorner"
                        bIsError = Not Single.TryParse(strValue, sYLLpos)
                        bIsCenterY = False
                        checksum = CByte(checksum Or &H8)

                    Case "yllcenter"
                        bIsError = Not Single.TryParse(strValue, sYLLpos)
                        bIsCenterY = True
                        checksum = CByte(checksum Or &H8)

                    Case "cellsize"
                        bIsError = Not Single.TryParse(strValue, sCellSize)
                        bIsError = bIsError Or (sCellSize <= 0)
                        checksum = CByte(checksum Or &H10)

                    Case "nodatavalue", "nodata_value"
                        bIsError = Not Single.TryParse(strValue, sValueNone)
                        checksum = CByte(checksum Or &H20)

                    Case Else
                        ' Unexpected bogusness
                        bIsError = True

                End Select
            End If

            ' Header is complete if all field have been read
            bIsComplete = (checksum >= &H2F)

        End While

        ' All good?
        If (bIsComplete = True) And (bIsError = False) Then
            ' #Yes: offset header positions if need be
            If (bIsCenterX) Then sXLLpos -= sCellSize / 2
            If (bIsCenterY) Then sYLLpos -= sCellSize / 2

            Dim sz As Single = Me.m_bm.CellSize
            If cNumberUtils.Approximates(sXLLpos, Me.m_bm.PosBottomRight.X, sz / 100) And _
               cNumberUtils.Approximates(sYLLpos, Me.m_bm.PosBottomRight.Y, sz / 100) And _
               cNumberUtils.Approximates(sCellSize, Me.m_bm.CellSize, sz / 100) Then

            End If

        Else
            ' #No: trash raster
        End If

        ' Done
        Return False

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Read the ASCII body from a text reader.
    ''' </summary>
    ''' <param name="reader">The open stream reader to read from.</param>
    ''' <returns>True if successful.</returns>
    ''' -------------------------------------------------------------------
    Protected Function ReadBody(ByVal reader As StreamReader) As Boolean

        'If (rs Is Nothing) Then Return False

        'Dim iCol As Integer = 0
        'Dim iRow As Integer = 0
        'Dim strLine As String = ""
        'Dim bDataCorrect As Boolean = True

        'Try
        '    While Not reader.EndOfStream And bDataCorrect
        '        ' Read line
        '        strLine = reader.ReadLine()
        '        ' Split by space
        '        Dim bits As String() = strLine.Split(" "c)
        '        ' Exact number of columns encountered?
        '        If (bits.Length <> rs.NumColumns) Then
        '            ' #No: do not accept this data
        '            bDataCorrect = False
        '        Else
        ' #Yes: process row data
        'For iCol = 0 To rs.NumColumns - 1
        '    bDataCorrect = bDataCorrect And Double.TryParse(bits(iCol), rs.Value(iRow, iCol))
        'Next iCol
        'iRow += 1
        '        End If

        'If (iRow > rs.NumRows) Then
        '    bDataCorrect = False
        'End If

        '    End While

        'bDataCorrect = bDataCorrect And (iRow = rs.NumRows)
        'Catch ex As Exception
        '    bDataCorrect = False
        'End Try

        'If (Not bDataCorrect) Then
        '    rs = Nothing
        'End If

        'Return bDataCorrect
        Return False

    End Function

End Class
