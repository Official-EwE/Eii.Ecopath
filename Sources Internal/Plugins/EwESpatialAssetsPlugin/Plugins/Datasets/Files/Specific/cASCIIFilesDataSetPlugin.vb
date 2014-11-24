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
Imports System.IO
Imports System.Windows.Forms
Imports DotSpatial.Data
Imports EwECore
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

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
                Return cStringUtils.Localize("{0}|*.asc", My.Resources.DIALOGFILTER_ASCII)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.LoadSource"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Function LoadSource() As Boolean

            Dim strFileName As String = Me.SourceFileName()
            Dim rs As IRaster = New DotSpatial.Data.Raster(Of Single)
            Dim reader As StreamReader = Nothing

            ' Already read? Ok!
            If (Me.m_raster IsNot Nothing) Then Return True

            ' File missing?
            If (Not System.IO.File.Exists(strFileName)) Then
                ' #Yes: report error
                Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FILENOTFOUND, strFileName), eStatusFlags.MissingParameter)
                ' Run away
                Return False
            End If

            Try
                ' Try to get reader
                reader = New StreamReader(strFileName)
            Catch ex As Exception
                ' Panic!
                Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FAILED, ex.Message), eStatusFlags.MissingParameter)
                Return False
            End Try

            Try
                ' Able to read header?
                If (Not Me.ReadHeader(reader, rs)) Then
                    ' #No: log error
                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FAILED_ASCIIHEADER, strFileName), eStatusFlags.MissingParameter)
                Else
                    ' Able to read body?
                    If (Not Me.ReadBody(reader, rs)) Then
                        ' #No: log error
                        Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FAILED_ASCIIBODY, strFileName), eStatusFlags.MissingParameter)
                    Else
                        ' #Yes: create internal raster to wrap the data
                        Me.m_raster = New cSpatialRaster(rs)
                        ' Update index
                        Me.StoreExtent(rs.Extent)
                        ' Log success
                        Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOADED, strFileName), eStatusFlags.OK)
                    End If
                End If
            Catch ex As Exception
                ' Log generic panic message
                Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FAILED, ex.Message), eStatusFlags.MissingParameter)
            End Try

            ' Clean up
            reader.Close()

            ' Report all over success
            Return (Me.m_raster IsNot Nothing)

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.UnlockData"/>
        ''' -------------------------------------------------------------------
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

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.ConversionFormat"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property ConversionFormat As String
            Get
                ' No conversion needed
                Return String.Empty
            End Get
        End Property

#End Region ' Overrides

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Read the ASCII header from a text reader.
        ''' </summary>
        ''' <param name="reader">The open stream reader to read from.</param>
        ''' <param name="rs">The raster to read the data into.</param>
        ''' <returns>True if successful.</returns>
        ''' <remarks>
        ''' This method aims to read a complete raster header as described in
        ''' http://resources.esri.com/help/9.3/arcgisengine/com_cpp/GP_ToolRef/Spatial_Analyst_Tools/esri_ascii_raster_format.htm.
        ''' If any of the header fields is missing the method will fail.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Function ReadHeader(ByVal reader As StreamReader, ByRef rs As IRaster) As Boolean

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

                Me.StoreExtent(New Extent(sXLLpos, sYLLpos, sXLLpos + sCellSize * nCols, sYLLpos + nRows * sCellSize))

                ' Generate raster
                rs = New Raster(Of Single)(nRows, nCols)
                rs.Bounds = cDotSpatialUtils.EcospaceToBounds(New PointF(sXLLpos, sYLLpos + nRows * sCellSize), _
                                                              New PointF(sXLLpos + nCols * sCellSize, sYLLpos), _
                                                              sCellSize)
                rs.NoDataValue = sValueNone
            Else
                ' #No: trash raster
                rs = Nothing
            End If

            ' Done
            Return (rs IsNot Nothing)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Read the ASCII body from a text reader.
        ''' </summary>
        ''' <param name="reader">The open stream reader to read from.</param>
        ''' <param name="rs">The raster to read the data into.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Protected Function ReadBody(ByVal reader As StreamReader, ByRef rs As IRaster) As Boolean

            If (rs Is Nothing) Then Return False

            Dim iCol As Integer = 0
            Dim iRow As Integer = 0
            Dim strLine As String = ""
            Dim bRowCountError As Boolean = False
            Dim bColCountError As Boolean = False
            Dim bValueError As Boolean = False
            Dim bDataCorrect As Boolean = True

            Try
                While Not reader.EndOfStream And bDataCorrect
                    ' Read line
                    strLine = reader.ReadLine()
                    'GDAL .asc writer adds a space to the start of the data rows strip this off
                    'Then split the string by space
                    Dim bits As String() = strLine.Trim().Split(" "c)
                    ' Exact number of columns encountered?
                    If (bits.Length <> rs.NumColumns) Then
                        ' #No: do not accept this data
                        bDataCorrect = False
                        bColCountError = True
                    Else
                        ' #Yes: process row data
                        For iCol = 0 To rs.NumColumns - 1
                            bValueError = bValueError Or Not Double.TryParse(bits(iCol), rs.Value(iRow, iCol))
                            bDataCorrect = bDataCorrect And Not bValueError
                        Next iCol
                        iRow += 1
                    End If

                    If (iRow > rs.NumRows) Then
                        bRowCountError = True
                        bDataCorrect = False
                    End If

                End While

                bDataCorrect = bDataCorrect And (iRow = rs.NumRows)
            Catch ex As Exception
                bDataCorrect = False
            End Try

            If (Not bDataCorrect) Then
                rs = Nothing
            End If

            If bRowCountError Then Me.LogMessage("", eStatusFlags.MissingParameter)
            If bColCountError Then Me.LogMessage("", eStatusFlags.MissingParameter)
            If bValueError Then Me.LogMessage("", eStatusFlags.FailedValidation)

            Return bDataCorrect

        End Function

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
                Return "DotSpatial.DataSet.0400"
            End Get
        End Property

#End Region ' Plug-in implementation

    End Class

End Namespace
