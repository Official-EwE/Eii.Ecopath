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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Drawing
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SpatialData

#End Region ' Imports

Namespace IO

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace import/export class for accessing bitmap files. MEL uses TIF files
    ''' for caching computed pressure layers, which can be read via this class.
    ''' </summary>
    ''' <seealso cref="EwEUtils.Core.IEcospaceImportExport" />
    ''' -----------------------------------------------------------------------
    Public Class cEcospaceImportExportBitmap
        Implements IEcospaceImportExport

        Private m_data(,) As Single

        ''' <summary>Enumerated type, defining the colour bands contained in bitmap data.</summary>
        ''' <remarks>Aplha channel access is not supported.</remarks>
        Public Enum eBands As Integer
            ''' <summary>Red pixel value</summary>
            Red
            ''' <summary>Green pixel value</summary>
            Green
            ''' <summary>Blue pixel value</summary>
            Blue
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance.
        ''' </summary>
        ''' <param name="core">The EwE core to operate onto.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(core As cCore)
            Me.Basemap = core.EcospaceBasemap
            Me.InCol = Basemap.InCol
            Me.InRow = Basemap.InRow
            ReDim Me.m_data(Me.InCol, Me.InRow)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Import the specified file with default options: the <see cref="eBands.Red"/>
        ''' band is used, and bands are presumed to have 256 colour values.
        ''' </summary>
        ''' <seealso cref="Load(String, eBands, Single)"/>
        ''' <param name="strFile">The file to import.</param>
        ''' <returns>
        ''' True if successful.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function Load(strFile As String) As Boolean
            Return Me.Load(strFile, eBands.Red, 1 / 256.0!)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load the map from a bitmap file.
        ''' </summary>
        ''' <param name="strFile">The file to import.</param>
        ''' <param name="band">The <see cref="eBands">band</see> to read.</param>
        ''' <param name="sScale">The conversion factor to convert pixel band colours 
        ''' to data values.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Load(strFile As String, band As eBands, Optional sScale As Single = 1 / 256.0!) As Boolean

            Dim bSuccess As Boolean = False
            Dim bmp As Bitmap = Nothing

            Try
                bmp = New Bitmap(strFile)
            Catch ex As Exception
                Return False
            End Try

            If (bmp.Height = Me.InRow) And (bmp.Width = Me.InCol) Then

                For x As Integer = 0 To Me.InCol - 1
                    For y As Integer = 0 To Me.InRow - 1
                        Dim clr As Color = bmp.GetPixel(x, y)
                        Dim val As Byte = 0
                        Select Case band
                            Case eBands.Red : val = clr.R
                            Case eBands.Green : val = clr.G
                            Case eBands.Blue : val = clr.B
                        End Select
                        ' One-based indexing
                        Me.m_data(x + 1, y + 1) = CSng(val * sScale)
                    Next y
                Next x
                bSuccess = True
            End If

            bmp.Dispose()

            Return bSuccess

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the map to a bitmap file.
        ''' </summary>
        ''' <param name="strFile">The file to import.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Save(strFile As String) As Boolean

            Dim bmp As New Bitmap(Me.InCol, Me.InRow, Imaging.PixelFormat.Format8bppIndexed)

            For x As Integer = 0 To Me.InCol - 1
                For y As Integer = 0 To Me.InRow - 1
                    Dim val As Integer = CInt(256 * Me.m_data(x + 1, y + 1))
                    Dim clr As Color = Color.FromArgb(val, val, val)
                    bmp.SetPixel(x, y, clr)
                Next y
            Next x

            Try
                bmp.Save(strFile)
            Catch ex As Exception
                Return False
            End Try
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a grid value.
        ''' </summary>
        ''' <param name="iRow">One-based row index to access a value for.</param>
        ''' <param name="iCol">One-based column index to access a value for.</param>
        ''' <param name="strField">Optional field to access a value for.</param>
        ''' -------------------------------------------------------------------
        Public Property Value(iRow As Integer, iCol As Integer, Optional strField As String = "") As Object _
            Implements IEcospaceImportExport.Value
            Get
                Return Me.m_data(iCol, iRow)
            End Get
            Set(value As Object)
                Me.m_data(iCol, iRow) = Convert.ToSingle(value)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the size of a cell in the Ecospace basemap.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property CellSize As Double _
            Implements IEcospaceImportExport.CellSize
            Get
                Return Me.Basemap.CellSize
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of columns in the Ecospace basemap.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property InCol As Integer _
            Implements IEcospaceImportExport.InCol

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of rows in the Ecospace basemap.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property InRow As Integer = 0 _
            Implements IEcospaceImportExport.InRow

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the NoData value to use.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NoDataValue As Double _
            Implements IEcospaceImportExport.NoDataValue
            Get
                Return cCore.NULL_VALUE
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the top-left (lon, lat) location of the Ecospace basemap.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property PosTopLeft As PointF _
            Implements IEcospaceImportExport.PosTopLeft
            Get
                Return Me.Basemap.PosTopLeft
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the WKT projection string of the Ecospace basemap.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ProjectionString As String _
            Implements IEcospaceImportExport.ProjectionString
            Get
                Return Me.Basemap.ProjectionString
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert the buffered data to a <see cref="ISpatialRaster"/> for use
        ''' by EwE.
        ''' </summary>
        ''' <param name="strField">Ignored.</param>
        ''' <returns>A populated raster.</returns>
        ''' <seealso cref="ISpatialRaster"/>
        ''' -------------------------------------------------------------------
        Public Function ToRaster(Optional strField As String = "") As ISpatialRaster _
            Implements IEcospaceImportExport.ToRaster
            Return New cEcospaceImportExportRaster(Me, strField)
        End Function

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the Ecospace basemap to operate onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private ReadOnly Property Basemap As cEcospaceBasemap

#End Region ' Internals

    End Class

End Namespace
