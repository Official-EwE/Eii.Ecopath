' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'


Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Utilities

Public Class Map

    Public Enum eStatsType As Integer
        Data
        Positive
        PositiveOrZero
    End Enum

    Private m_cells(,) As Decimal
    Private m_filename As String = ""
    Private m_bCellCenter As Boolean = False

    Private m_nValCells As Integer = 0
    Private m_mean As Decimal = 0
    Private m_min As Decimal = 0
    Private m_max As Decimal = 0
    Private m_statsType As eStatsType = eStatsType.Data

    Public Sub New()
        ' NOP
    End Sub

    Public Sub New(strFile As String, Optional bHeaderOnly As Boolean = False)
        Me.New()
        Me.Load(strFile, bHeaderOnly)
    End Sub

    Property Tag As Object = Nothing

    ''' <summary>
    ''' Initialize map to a data array (nCols, nRows).
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub New(data(,) As Decimal)
        Me.NumCols = data.GetUpperBound(0) + 1
        Me.NumRows = data.GetUpperBound(1) + 1
        Me.Resize(0D)
        For x As Integer = 0 To Me.NumCols - 1
            For y As Integer = 0 To Me.NumRows - 1
                Me(x, y) = data(x, y)
            Next y
        Next x
    End Sub

    ''' <summary>
    ''' Initialize map to a data array. Format (nCols, nRows) is
    ''' expected, unless <paramref name="bEcospace"/> is set.
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub New(data(,) As Integer, bEcospace As Boolean)
        Me.NumCols = data.GetUpperBound(If(bEcospace, 1, 0)) + If(bEcospace, 0, 1)
        Me.NumRows = data.GetUpperBound(If(bEcospace, 0, 1)) + If(bEcospace, 0, 1)
        Me.Resize(0D)
        For x As Integer = 0 To Me.NumCols - 1
            For y As Integer = 0 To Me.NumRows - 1
                If (bEcospace) Then
                    Me(x, y) = CDec(data(y + 1, x + 1))
                Else
                    Me(x, y) = CDec(data(x, y))
                End If
            Next y
        Next x
    End Sub

    ''' <summary>
    ''' Initialize map to a data array. Format (nCols, nRows) is
    ''' expected, unless <paramref name="bEcospace"/> is set.
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub New(data(,) As Single, bEcospace As Boolean)
        Me.NumCols = data.GetUpperBound(If(bEcospace, 1, 0)) + If(bEcospace, 0, 1)
        Me.NumRows = data.GetUpperBound(If(bEcospace, 0, 1)) + If(bEcospace, 0, 1)
        Me.Resize(0D)
        For x As Integer = 0 To Me.NumCols - 1
            For y As Integer = 0 To Me.NumRows - 1
                If (bEcospace) Then
                    Me(x, y) = CDec(data(y + 1, x + 1))
                Else
                    Me(x, y) = CDec(data(x, y))
                End If
            Next y
        Next x
    End Sub

    ''' <summary>
    ''' Initialize map to a data array. Format (nCols, nRows) is
    ''' expected, unless <paramref name="bEcospace"/> is set.
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub New(data(,) As Boolean, bEcospace As Boolean)
        Me.NumCols = data.GetUpperBound(If(bEcospace, 1, 0)) + If(bEcospace, 0, 1)
        Me.NumRows = data.GetUpperBound(If(bEcospace, 0, 1)) + If(bEcospace, 0, 1)
        Me.Resize(0D)
        For x As Integer = 0 To Me.NumCols - 1
            For y As Integer = 0 To Me.NumRows - 1
                If (bEcospace) Then
                    Me(x, y) = CDec(data(y + 1, x + 1))
                Else
                    Me(x, y) = CDec(data(x, y))
                End If
            Next y
        Next x
    End Sub

    ''' <summary>
    ''' Copy constructor; initializes a new map to the properties of another.
    ''' </summary>
    ''' <param name="map"></param>
    ''' <param name="value"></param>
    Public Sub New(map As Map, Optional value As Decimal = 0)
        Me.New()
        Me.Init(map, value)
    End Sub

    Public Sub New(bm As cEcospaceBasemap)
        Me.New()
        Me.NumCols = bm.InCol
        Me.NumRows = bm.InRow
        Me.XllCorner = CDec(bm.PosTopLeft.X)
        Me.YllCorner = CDec(bm.PosBottomRight.Y)
        Me.CellSize = CDec(bm.CellSize)
        Me.NoDataValue = CDec(cCore.NULL_VALUE)
        Me.Resize(0D)
    End Sub

    Public Property NumRows As Integer = 0
    Public Property NumCols As Integer = 0
    Public Property CellSize As Decimal = 0.0D
    Public Property XllCorner As Decimal = 0.0D
    Public Property YllCorner As Decimal = 0.0D
    Public Property NoDataValue As Decimal = -9999D

    Private Property HeaderOnly As Boolean = False

    Public Sub Init(map As Map, Optional value As Decimal = 0)
        If (map IsNot Nothing) Then
            Me.NumCols = map.NumCols
            Me.NumRows = map.NumRows
            Me.CellSize = map.CellSize
            Me.NoDataValue = map.NoDataValue
            Me.XllCorner = map.XllCorner
            Me.YllCorner = map.YllCorner
        End If
        Me.Resize(value)
    End Sub

    Public Sub Resize(Optional value As Decimal = 0)
        If Me.HeaderOnly Then Return
        ReDim Me.m_cells(Me.NumCols - 1, Me.NumRows - 1)
        Me.Fill(value)
    End Sub

    Public Function Matches(map As Map) As Boolean
        Return cNumberUtils.Approximates(Me.CellSize, map.CellSize, 0.0001) And
               (Me.NumCols = map.NumCols) And
               (Me.NumRows = map.NumRows) And
               cNumberUtils.Approximates(Me.XllCorner, map.XllCorner, 0.0001) And
               cNumberUtils.Approximates(Me.YllCorner, map.YllCorner, 0.0001)
    End Function

    Public ReadOnly Property Header As String
        Get
            Dim sb As New StringBuilder()
            sb.AppendLine("ncols        " & Me.NumCols)
            sb.AppendLine("nrows        " & Me.NumRows)
            sb.AppendLine("xllcorner    " & Me.XllCorner)
            sb.AppendLine("yllcorner    " & Me.YllCorner)
            sb.AppendLine("cellsize     " & Me.CellSize)
            sb.AppendLine("NODATA_value " & Me.NoDataValue)
            Return sb.ToString()
        End Get
    End Property

    Public Overridable Function Load(strFile As String, Optional bHeaderOnly As Boolean = False) As Boolean

        Dim bits() As String = Nothing
        Dim xllCorner As Decimal = 0
        Dim yllCorner As Decimal = 0
        Dim xllCenter As Decimal = 0
        Dim yllCenter As Decimal = 0
        Dim strLine As String = ""
        Dim bSuccess As Boolean = True
        Dim separators() As String = {" ", vbTab}

        Me.m_filename = strFile
        Me.HeaderOnly = bHeaderOnly

        Try
            Using sr As New StreamReader(strFile)
                For i As Integer = 1 To 6
                    strLine = sr.ReadLine()
                    bits = strLine.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    Select Case bits(0).ToLower
                        Case "ncols" : Me.NumCols = cStringUtils.ConvertToInteger(bits(1))
                        Case "nrows" : Me.NumRows = cStringUtils.ConvertToInteger(bits(1))
                        Case "cellsize" : Me.CellSize = cStringUtils.ConvertToDecimal(bits(1))
                        Case "nodata_value" : Me.NoDataValue = cStringUtils.ConvertToDecimal(bits(1))
                        Case "xllcorner" : xllCorner = cStringUtils.ConvertToDecimal(bits(1))
                        Case "yllcorner" : yllCorner = cStringUtils.ConvertToDecimal(bits(1))
                        Case "xllcenter" : xllCenter = cStringUtils.ConvertToDecimal(bits(1))
                        Case "yllcenter" : yllCenter = cStringUtils.ConvertToDecimal(bits(1))
                    End Select
                Next

                If Not bHeaderOnly Then
                    Me.Resize()
                    For row As Integer = 0 To Me.NumRows - 1
                        strLine = sr.ReadLine()
                        bits = strLine.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                        For col As Integer = 0 To Me.NumCols - 1
                            Me.Value(col, row) = cStringUtils.ConvertToDecimal(bits(col))
                        Next
                    Next
                End If
            End Using

            Me.XllCorner = If(xllCenter = 0, xllCorner, xllCenter - Me.CellSize / 2)
            Me.YllCorner = If(yllCenter = 0, yllCorner, yllCenter - Me.CellSize / 2)

        Catch ex As Exception
            bSuccess = False
        End Try
        Return bSuccess
    End Function

    Public Sub Fill()
        Me.Fill(Me.NoDataValue)
    End Sub

    Public Sub Fill(value As Decimal)
        If Me.HeaderOnly Then Return
        For row As Integer = 0 To Me.NumRows - 1
            For col As Integer = 0 To Me.NumCols - 1
                Me.m_cells(col, row) = value
            Next
        Next
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="col">Zero-based column index, starting at top-left.</param>
    ''' <param name="row">Zero-based row index, starting at top-left.</param>
    ''' <returns></returns>
    Default Public Property Value(col As Integer, row As Integer) As Decimal
        Get
            If Me.HeaderOnly Then Return Me.NoDataValue

            If (col < 0 Or col > Me.NumCols - 1) Then Return Me.NoDataValue
            If (row < 0 Or row > Me.NumRows - 1) Then Return Me.NoDataValue
            Return Me.m_cells(col, row)
        End Get
        Set(value As Decimal)
            If Me.HeaderOnly Then Return

            If (col < 0 Or col > Me.NumCols - 1) Then Return
            If (row < 0 Or row > Me.NumRows - 1) Then Return
            Me.m_cells(col, row) = value
        End Set
    End Property

    Public Function LatToRow(lat As Single) As Integer
        lat = (Me.YllCorner + Me.NumRows * Me.CellSize) - lat
        Return CInt(Math.Floor(lat / Me.CellSize))
    End Function

    Public Function RowToLat(row As Integer) As Single
        Return Me.YllCorner + (Me.NumRows - row - 0.5!) * Me.CellSize
    End Function

    Public Function LonToCol(lon As Single) As Integer
        Return CInt(Math.Floor((lon - Me.XllCorner) / Me.CellSize))
    End Function

    Public Function ColToLon(col As Integer) As Single
        Return Me.XllCorner + (col + 0.5!) * Me.CellSize
    End Function

    Public Overridable Sub Write(strFile As String)
        If Me.HeaderOnly Then Return

        Using wr As New StreamWriter(strFile)
            wr.Write(Me.Header)
            For row As Integer = 0 To Me.NumRows - 1
                For col As Integer = 0 To Me.NumCols - 1
                    Dim strVal As String
                    If (row = 0 And col = 0) Then
                        strVal = cStringUtils.FormatDecimal(Me.m_cells(0, 0))
                        If Not strVal.Contains(".") Then strVal = strVal & ".0"
                    Else
                        strVal = cStringUtils.FormatDecimal(Me.m_cells(col, row))
                    End If
                    If (col > 0) Then wr.Write(" ")
                    wr.Write(strVal)
                Next col
                wr.WriteLine()
            Next row
        End Using
        Me.m_filename = strFile
    End Sub

    Public Property Filename As String
        Get
            Return Me.m_filename
        End Get
        Set(value As String)
            Me.Load(value)
        End Set
    End Property

    Public Property Stats As eStatsType
        Get
            Return Me.m_statsType
        End Get
        Set(value As eStatsType)
            If (Me.m_statsType <> value) Then
                Me.m_statsType = value
                Me.m_nValCells = 0
            End If
        End Set
    End Property

    Public ReadOnly Property NumValueCells As Integer
        Get
            Me.RecalcStats()
            Return Me.m_nValCells
        End Get
    End Property

    Public ReadOnly Property Mean As Decimal
        Get
            Me.RecalcStats()
            Return Me.m_mean
        End Get
    End Property

    Public ReadOnly Property Min As Decimal
        Get
            Me.RecalcStats()
            Return Me.m_min
        End Get
    End Property

    Public ReadOnly Property Max As Decimal
        Get
            Me.RecalcStats()
            Return Me.m_max
        End Get
    End Property

    Public Function RowColToSeq(row As Integer, col As Integer) As Integer
        Return col + row * Me.NumCols
    End Function

    Public Function SeqToRowCol(seq As Integer, ByRef row As Integer, ByRef col As Integer) As Boolean
        row = CInt(Math.Floor(seq / Me.NumCols))
        col = seq - row * Me.NumCols
        Return True
    End Function

    Public ReadOnly Property HasData As Boolean
        Get
            Return (Me.m_cells IsNot Nothing)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("Map {0} cols x {1} rows, {2} cellsize", Me.NumCols, Me.NumRows, Me.CellSize)
    End Function

    ''' <summary>
    ''' Returns a map that identifies horizontally and vertically connected cells
    ''' as unique clusters.
    ''' </summary>
    ''' <returns></returns>
    Public Function Clusters() As Map
        Dim d As Decimal = 1D
        Dim conn As New Map(Me, 0D)
        For row As Integer = 0 To Me.NumRows
            For col As Integer = 0 To Me.NumCols
                Dim val As Decimal = Me(col, row)
                If ((val > 0) And (conn(col, row) = 0)) Then
                    Me.FloodFill(val, conn, col, row, d)
                    d += 1
                End If
            Next
        Next
        Return conn
    End Function

    Public Function ClusterCount() As Map
        Dim count As New Map(Me, 0)
        Dim zonecount As New Dictionary(Of Decimal, Integer)
        For row As Integer = 0 To Me.NumRows
            For col As Integer = 0 To Me.NumCols
                Dim val As Decimal = Me(col, row)
                If (val > 0) Then
                    If (Not zonecount.ContainsKey(val)) Then
                        zonecount(val) = 1
                    Else
                        zonecount(val) += 1
                    End If
                End If
            Next
        Next
        For row As Integer = 0 To Me.NumRows
            For col As Integer = 0 To Me.NumCols
                Dim val As Decimal = Me(col, row)
                If (val > 0) Then
                    count(col, row) = zonecount(val)
                End If
            Next
        Next
        Return count
    End Function

    Public Function Data() As Decimal(,)
        Return Me.m_cells
    End Function

    Public Function ToCSV(file As String, Optional valuename As String = "value", Optional time As Integer = 0) As Boolean

        Using sw As New StreamWriter(file)
            If (time > 0) Then sw.Write("Time,")
            sw.WriteLine("Latitude,Longitude,{0}", cStringUtils.ToCSVField(valuename))

            For y As Integer = 0 To Me.NumRows - 1
                For x As Integer = 0 To Me.NumCols - 1
                    If (Me.m_cells(x, y) <> Me.NoDataValue) Then
                        If (time > 0) Then sw.Write("{0},", time)
                        sw.WriteLine("{0},{1},{2}", Me.RowToLat(y), Me.ColToLon(x), Me.m_cells(x, y))
                    End If
                Next
            Next
            sw.Flush()
            sw.Close()
        End Using
        Return True

    End Function

    Public Function FromCSV(file As String) As Boolean

        If Not Me.HasData Then Return False
        Me.Fill()

        Using sr As New StreamReader(file)
            Dim line As String = sr.ReadLine()
            ' For now, assume {time,} lat, lon, value
            Dim bits() As String = Header.Split(","c)
            Dim nCols As Integer = bits.Count

            While Not sr.EndOfStream
                bits = sr.ReadLine().Split(","c)
                Dim x As Integer = Me.LonToCol(CSng(bits(nCols - 2)))
                Dim y As Integer = Me.LatToRow(CSng(bits(nCols - 3)))
                Dim val As Decimal = CDec(bits(nCols - 1))
                Me.Value(x, y) = val
            End While
        End Using
        Return True

    End Function

    ''' <summary>
    ''' Offsets a global map by 180 degrees
    ''' </summary>
    ''' <returns></returns>
    Public Function Wrap() As Map

        ' ToDo: perform checks if map is actually global

        Dim mapNew As New Map(Me)
        mapNew.XllCorner = (mapNew.XllCorner - 180D) Mod 360D

        For y As Integer = 0 To Me.NumRows - 1
            For x As Integer = 0 To Me.NumCols - 1
                Dim xnew As Integer = mapNew.LonToCol(Me.ColToLon(x))
                mapNew((xnew + Me.NumCols) Mod Me.NumCols, y) = Me(x, y)
            Next
        Next
        Return mapNew

    End Function

#Region " Internals "

    Private Sub RecalcStats()

        If Me.HeaderOnly Then Return

        If (Me.m_nValCells = 0) Then
            Dim tot As Double = 0
            Dim min As Decimal = Decimal.MaxValue
            Dim max As Decimal = Decimal.MinValue
            For row As Integer = 0 To Me.NumRows - 1
                For col As Integer = 0 To Me.NumCols - 1
                    Dim val As Decimal = Me.Value(col, row)
                    Dim bAcceptValue As Boolean = (val <> Me.NoDataValue)

                    Select Case Me.m_statsType
                        Case eStatsType.Data
                            ' NOP
                        Case eStatsType.Positive
                            bAcceptValue = bAcceptValue And (val > 0)
                        Case eStatsType.PositiveOrZero
                            bAcceptValue = bAcceptValue And (val >= 0)
                    End Select

                    If (bAcceptValue) Then
                        tot += val
                        Me.m_nValCells += 1
                        min = Math.Min(min, val)
                        max = Math.Max(max, val)
                    End If
                Next
            Next
            Me.m_mean = CDec(tot / Math.Max(1, Me.m_nValCells))
            Me.m_min = If(min = Decimal.MaxValue, Me.NoDataValue, min)
            Me.m_max = If(min = Decimal.MinValue, Me.NoDataValue, max)
        End If

    End Sub

    Private Sub FloodFill(val As Decimal, conn As Map, x As Integer, y As Integer, n As Decimal)
        If (x < 0 Or x >= Me.NumCols Or y < 0 Or y > Me.NumRows) Then Return
        If ((Me(x, y) <> val) Or (conn(x, y) <> 0)) Then Return
        conn(x, y) = n
        FloodFill(val, conn, x - 1, y, n)
        FloodFill(val, conn, x + 1, y, n)
        FloodFill(val, conn, x, y - 1, n)
        FloodFill(val, conn, x, y + 1, n)
    End Sub

#End Region ' Internals

End Class
