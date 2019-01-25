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
Imports System.IO
Imports EwECore
Imports EwEUtils.Core
Imports EwEShell.IO

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Data for a single map in MSP.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cGrid
    Implements IMELItem

    Private m_data As Double(,) = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new <see cref="cGrid"/>.
    ''' </summary>
    ''' <param name="name">The name for the grid.</param>
    ''' <param name="iWidth">The width or number of columns in the grid.</param>
    ''' <param name="iHeight">The height or number of rows in the grid.</param>
    ''' <param name="data">Data for the grid, if any.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(name As String, iWidth As Integer, iHeight As Integer, Optional data As Double(,) = Nothing)

        Me.Name = name
        Me.Width = iWidth
        Me.Height = iHeight

        ReDim Me.m_data(Math.Max(0, Width - 1), Math.Max(0, Height - 1))
        If (data IsNot Nothing) Then
            Array.Copy(data, Me.m_data, data.Length)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the map.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String Implements IMELItem.Name

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the width or number of columns in the grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Width As Integer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the height or number of rows in the grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Height As Integer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the lowest value in the output grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Min As Double = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the highest value in the output grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Max As Double = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the mean value in the output grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Mean As Double = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the units in which grid values are expressed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Units As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of non-zero value cells in the output.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumValueCells As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the zero-base indexed data for the grid, dimensioned as (width x height)
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Cell As Double(,)
        Get
            Return Me.m_data
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether the content of the grid is valid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property IsValid As Boolean = True

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Erase the content of the grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Clear()
        Try
            Array.Clear(Me.Cell, 0, Me.Cell.Length)
            Me.IsValid = True
        Catch ex As Exception
            Me.IsValid = False
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Loads the map from file.
    ''' </summary>
    ''' <param name="strFile">The file name to load from. File names with 
    ''' the .asc extension, as well as non-compressed image files, are supported</param>
    ''' <param name="core">The core that provides spatial contextual information.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Load(strFile As String, core As cCore) As Boolean

        Dim r As IEcospaceImportExport = Nothing

        If (String.IsNullOrWhiteSpace(strFile)) Then Return False
        If (Not File.Exists(strFile)) Then Return False

        Select Case Path.GetExtension(strFile.ToLower())

            Case ".asc"
                Dim imp As New cEcospaceImportExportASCIIData(core)
                If Not imp.Read(strFile) Then Return False
                r = imp

            Case ".tif", ".tiff", ".bmp", ".png"
                Dim imp As New cEcospaceImportExportBitmap(core)
                If Not imp.Load(strFile) Then Return False
                r = imp

            Case Else
                Return False

        End Select

        Me.Width = r.InCol
        Me.Height = r.InRow
        ReDim Me.m_data(Math.Max(0, Width - 1), Math.Max(0, Height - 1))
        For y As Integer = 0 To Me.Height - 1
            For x As Integer = 0 To Me.Width - 1
                Dim d As Double = CDbl(r.Value(y + 1, x + 1))
                If ((d = r.NoDataValue) Or (d = cCore.NULL_VALUE)) Then d = cCore.NULL_VALUE
                Me.m_data(x, y) = d
            Next x
        Next y
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves the map to file.
    ''' </summary>
    ''' <param name="strFile">The file name to save to. Only file names with 
    ''' the .asc extension are supported</param>
    ''' <param name="core">The core that provides spatial contextual information.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Save(strFile As String, core As cCore) As Boolean

        If (String.IsNullOrWhiteSpace(strFile)) Then Return False
        If (Not Path.GetExtension(strFile.ToLower()) = ".asc") Then Return False

        Dim r As New cEcospaceImportExportASCIIData(core)
        Dim bOK As Boolean = False

        For y As Integer = 0 To Me.Height - 1
            For x As Integer = 0 To Me.Width - 1
                Dim d As Double = Me.m_data(x, y)
                If (d = r.NoDataValue) Then d = r.NoDataValue
                r.Value(y + 1, x + 1) = d
            Next x
        Next y
        bOK = r.Save(strFile)

        ' Save real data too min, max, mean data
        strFile = Path.ChangeExtension(strFile, ".txt")
        Try
            Using sw As New StreamWriter(strFile)
                sw.WriteLine("Name:  {0}", Me.Name)
                sw.WriteLine("Min:   {0}", Me.Min)
                sw.WriteLine("Max:   {0}", Me.Max)
                sw.WriteLine("Mean:  {0}", Me.Mean)
                sw.WriteLine("Units: {0}", Me.Units)
                sw.WriteLine("Water: {0}", Me.NumValueCells)
                sw.Flush()
            End Using
        Catch ex As Exception
            bOK = False
        End Try
        Return bOK

    End Function

End Class
