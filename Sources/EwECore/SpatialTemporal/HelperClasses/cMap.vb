' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)


Option Strict On
Imports System.IO
Imports System.Text
Imports EwEUtils.Utilities

Public Class cMap

    Public Enum eStatsType As Integer
        Data
        Positive
        PositiveOrZero
    End Enum

    Private m_cells(,) As Single
    Private m_filename As String = ""

    Private m_nValCells As Integer = 0
    Private m_mean As Single = 0
    Private m_min As Single = 0
    Private m_max As Single = 0
    Private m_statsType As eStatsType = eStatsType.Data

    Public Sub New()
        ' NOP
    End Sub

    Public Sub New(strFile As String)
        Me.New()
        Me.Load(strFile)
    End Sub

    Property Tag As Object = Nothing

    ''' <summary>
    ''' Initialize map to a data array (nCols, nRows).
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub New(data(,) As Single)
        Me.NumCols = data.GetUpperBound(0) + 1
        Me.NumRows = data.GetUpperBound(1) + 1
        Me.Resize(0)
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
        Me.Resize(0)
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
        Me.Resize(0)
        For x As Integer = 0 To Me.NumCols - 1
            For y As Integer = 0 To Me.NumRows - 1
                If (bEcospace) Then
                    Me(x, y) = data(y + 1, x + 1)
                Else
                    Me(x, y) = data(x, y)
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
        Me.Resize(0)
        For x As Integer = 0 To Me.NumCols - 1
            For y As Integer = 0 To Me.NumRows - 1
                If (bEcospace) Then
                    Me(x, y) = CSng(data(y + 1, x + 1))
                Else
                    Me(x, y) = CSng(data(x, y))
                End If
            Next y
        Next x
    End Sub

    ''' <summary>
    ''' Copy constructor; initializes a new map to the properties of another.
    ''' </summary>
    ''' <param name="map"></param>
    ''' <param name="value"></param>
    Public Sub New(map As cMap, Optional value As Single = 0)
        Me.New()
        Me.Init(map, value)
    End Sub

    Public Sub New(bm As cEcospaceBasemap, Optional l As cEcospaceLayerSingle = Nothing)
        Me.New()
        Me.NumCols = bm.InCol
        Me.NumRows = bm.InRow
        Me.XllCorner = CDec(bm.PosTopLeft.X)
        Me.YllCorner = CDec(bm.PosBottomRight.Y)
        Me.CellSize = CDec(bm.CellSize)
        Me.NoDataValue = CDec(cCore.NULL_VALUE)

        Me.Resize(0)

        If (l IsNot Nothing) Then
            For ir As Integer = 1 To bm.InRow
                For ic As Integer = 1 To bm.InCol
                    Me(ic - 1, ir - 1) = CSng(l.Cell(ir, ic))
                Next ic
            Next ir
        End If

    End Sub

    Public Function Clone() As cMap
        Dim m As New cMap(Me)
        For x As Integer = 0 To Me.NumCols - 1
            For y As Integer = 0 To Me.NumRows - 1
                m(x, y) = Me.m_cells(x, y)
            Next y
        Next x
        Return m
    End Function

    Public Property NumRows As Integer = 0
    Public Property NumCols As Integer = 0
    Public Property CellSize As Single = 0.0
    Public Property XllCorner As Single = 0.0
    Public Property YllCorner As Single = 0.0
    Public Property NoDataValue As Single = -9999

    Public Sub Init(map As cMap, Optional value As Single = 0)
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

    Public Sub Resize(Optional value As Single = 0)
        ReDim Me.m_cells(Me.NumCols - 1, Me.NumRows - 1)
        Me.Fill(value)
    End Sub

    Public Function Matches(map As cMap) As Boolean
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

    Public Function LoadFromText(strText As String) As Boolean
        Dim bSuccess As Boolean = False
        Me.m_filename = ""
        Using sr As New StringReader(strText)
            bSuccess = Me.ReadAsASCII(sr)
        End Using
        Return bSuccess
    End Function

    Public Overridable Function Load(strFile As String) As Boolean
        Dim bSuccess As Boolean = False
        Me.m_filename = ""
        If (Not String.IsNullOrEmpty(strFile)) Then
            Using sr As New StreamReader(strFile)
                bSuccess = Me.ReadAsASCII(sr)
                If (bSuccess) Then
                    Me.m_filename = strFile
                End If
            End Using
        End If
        Return bSuccess
    End Function

    Public Sub Fill()
        Me.Fill(Me.NoDataValue)
    End Sub

    Public Sub Fill(value As Single)
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
    Default Public Property Value(col As Integer, row As Integer) As Single
        Get
            If (col < 0 Or col > Me.NumCols - 1) Then Return Me.NoDataValue
            If (row < 0 Or row > Me.NumRows - 1) Then Return Me.NoDataValue
            Return Me.m_cells(col, row)
        End Get
        Set(value As Single)
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
        Return Me.YllCorner + (Me.NumRows - row) * Me.CellSize
    End Function

    Public Function LonToCol(lon As Single) As Integer
        Return CInt(Math.Floor((lon - Me.XllCorner) / Me.CellSize))
    End Function

    Public Function ColToLon(col As Integer) As Single
        Return Me.XllCorner + col * Me.CellSize
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="col"></param>
    ''' <param name="row"></param>
    ''' <returns></returns>
    ''' <seealso cref="SeqToCol(Integer)"/>
    ''' <seealso cref="SeqToRow(Integer)"/>
    ''' -----------------------------------------------------------------------
    Public Function ColRowToSeq(col As Integer, row As Integer) As Integer
        If (col < 0) Or (row < 0) Then Return -1
        If (col >= Me.NumCols) Or (row >= Me.NumRows) Then Return -1
        Return row * Me.NumCols + col
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sec"></param>
    ''' <returns></returns>
    ''' <seealso cref="ColRowToSeq(Integer, Integer)"/>
    ''' <seealso cref="SeqToCol(Integer)"/>
    ''' -----------------------------------------------------------------------
    Public Function SeqToRow(sec As Integer) As Integer
        Return CInt(Math.Floor(sec / Me.NumCols))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sec"></param>
    ''' <returns></returns>
    ''' <seealso cref="ColRowToSeq(Integer, Integer)"/>
    ''' <seealso cref="SeqToRow(Integer)"/>
    ''' -----------------------------------------------------------------------
    Public Function SeqToCol(sec As Integer) As Integer
        Return sec Mod Me.NumCols
    End Function

    Public Overridable Sub Write(strFile As String)
        Using wr As New StreamWriter(strFile)
            wr.Write(Me.Header)
            For row As Integer = 0 To Me.NumRows - 1
                For col As Integer = 0 To Me.NumCols - 1
                    Dim strVal As String
                    If (row = 0 And col = 0) Then
                        strVal = cStringUtils.FormatSingle(Me.m_cells(0, 0))
                        If Not strVal.Contains(".") Then strVal = strVal & ".0"
                    Else
                        strVal = cStringUtils.FormatSingle(Me.m_cells(col, row))
                    End If
                    If (col > 0) Then wr.Write(" ")
                    wr.Write("{0:F6}", strVal)
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

    Public ReadOnly Property Mean As Single
        Get
            Me.RecalcStats()
            Return Me.m_mean
        End Get
    End Property

    Public ReadOnly Property Min As Single
        Get
            Me.RecalcStats()
            Return Me.m_min
        End Get
    End Property

    Public ReadOnly Property Max As Single
        Get
            Me.RecalcStats()
            Return Me.m_max
        End Get
    End Property

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
    Public Function Clusters() As cMap
        Dim d As Single = 1D
        Dim conn As New cMap(Me, 0D)
        For row As Integer = 0 To Me.NumRows
            For col As Integer = 0 To Me.NumCols
                Dim val As Single = Me(col, row)
                If ((val > 0) And (conn(col, row) = 0)) Then
                    Me.FloodFill(val, conn, col, row, d)
                    d += 1
                End If
            Next
        Next
        Return conn
    End Function

    Public Function ClusterCount() As cMap
        Dim count As New cMap(Me, 0)
        Dim zonecount As New Dictionary(Of Single, Integer)
        For row As Integer = 0 To Me.NumRows
            For col As Integer = 0 To Me.NumCols
                Dim val As Single = Me(col, row)
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
                Dim val As Single = Me(col, row)
                If (val > 0) Then
                    count(col, row) = zonecount(val)
                End If
            Next
        Next
        Return count
    End Function

    Public Function Data() As Single(,)
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
            Dim bits() As String = Me.Header.Split(","c)
            Dim nCols As Integer = bits.Count

            While Not sr.EndOfStream
                bits = sr.ReadLine().Split(","c)
                Dim x As Integer = Me.LonToCol(CSng(bits(nCols - 2)))
                Dim y As Integer = Me.LatToRow(CSng(bits(nCols - 3)))
                Dim val As Single = CDec(bits(nCols - 1))
                Me.Value(x, y) = val
            End While
        End Using
        Return True

    End Function

#Region " Utilities "

    Public Function InterpolateDefaults(basemap As cMap, Optional valdefault As Single = 0) As Integer

        Dim nRows As Integer = Me.NumRows
        Dim nCols As Integer = Me.NumCols
        Dim nPass As Integer = 0
        Dim bChanged As Boolean = False

        Do
            Dim interp As New cMap(Me)
            bChanged = False

            For ir As Integer = 0 To nRows - 1
                For ic As Integer = 0 To nCols - 1
                    ' Is water cell without a value?
                    If (basemap(ic, ir) > 0) Then
                        If ((Me(ic, ir) = Me.NoDataValue) Or (Me(ic, ir) = valdefault)) Then

                            Dim d As Single = 0
                            Dim n As Integer = 0

                            For iri As Integer = Math.Max(0, ir - 1) To Math.Min(nRows, ir + 1) - 1
                                For ici As Integer = Math.Max(0, ic - 1) To Math.Min(nCols, ir + 1) - 1
                                    If (basemap(ici, iri) > 0) And (Me(ici, iri) <> Me.NoDataValue) And (Me(ici, iri) <> valdefault) Then
                                        d += Me(ici, iri)
                                        n += 1
                                    End If
                                Next ici
                            Next iri

                            If (n > 0) Then
                                interp(ic, ir) = d / n
                                bChanged = True
                            Else
                                interp(ic, ir) = Me(ic, ir)
                            End If

                        End If

                    End If
                Next ic
            Next ir

            If (bChanged) Then
                For ir As Integer = 0 To nRows - 1
                    For ic As Integer = 0 To nCols - 1
                        Me(ic, ir) = interp(ic, ir)
                    Next ic
                Next ir
                nPass += 1
            End If

        Loop Until bChanged = True

        Return nPass
    End Function

#End Region ' Utilities

#Region " Internals "

    Protected Function ReadAsASCII(reader As TextReader) As Boolean

        Dim xllCorner As Single = 0
        Dim yllCorner As Single = 0
        Dim xllCenter As Single = 0
        Dim yllCenter As Single = 0
        Dim bSuccess As Boolean = True
        Dim separators() As String = {" ", vbTab}

        Try
            For i As Integer = 1 To 6
                Dim strLine As String = reader.ReadLine()
                Dim bits() As String = strLine.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                Select Case bits(0).ToLower
                    Case "ncols" : Me.NumCols = CInt(bits(1))
                    Case "nrows" : Me.NumRows = CInt(bits(1))
                    Case "cellsize" : Me.CellSize = CSng(bits(1))
                    Case "nodata_value" : Me.NoDataValue = CSng(bits(1))
                    Case "xllcorner" : xllCorner = CSng(bits(1))
                    Case "yllcorner" : yllCorner = CSng(bits(1))
                    Case "xllcenter" : xllCenter = CSng(bits(1))
                    Case "yllcenter" : yllCenter = CSng(bits(1))
                End Select
            Next
            ReDim Me.m_cells(Me.NumCols, Me.NumRows)

            For row As Integer = 0 To Me.NumRows - 1
                Dim strLine As String = reader.ReadLine()
                If Not String.IsNullOrWhiteSpace(strLine) Then
                    Dim bits() As String = strLine.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    For col As Integer = 0 To Me.NumCols - 1
                        If col = bits.Count Then Return False
                        Me.Value(col, row) = CSng(bits(col))
                    Next
                Else
                    Return False
                End If
            Next

            Me.XllCorner = If(xllCenter = 0, xllCorner, xllCenter - Me.CellSize / 2)
            Me.YllCorner = If(yllCenter = 0, yllCorner, yllCenter - Me.CellSize / 2)

        Catch ex As Exception
            bSuccess = False
        End Try
        Return bSuccess
    End Function

    Private Sub RecalcStats()

        If (Me.m_nValCells = 0) Then
            Dim tot As Double = 0
            Dim min As Single = Single.MaxValue
            Dim max As Single = Single.MinValue
            For row As Integer = 0 To Me.NumRows - 1
                For col As Integer = 0 To Me.NumCols - 1
                    Dim val As Single = Me.Value(col, row)
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
            Me.m_min = If(min = Single.MaxValue, Me.NoDataValue, min)
            Me.m_max = If(min = Single.MinValue, Me.NoDataValue, max)
        End If

    End Sub

    Private Sub FloodFill(val As Single, conn As cMap, x As Integer, y As Integer, n As Single)
        If (x < 0 Or x >= Me.NumCols Or y < 0 Or y > Me.NumRows) Then Return
        If ((Me(x, y) <> val) Or (conn(x, y) <> 0)) Then Return
        conn(x, y) = n
        Me.FloodFill(val, conn, x - 1, y, n)
        Me.FloodFill(val, conn, x + 1, y, n)
        Me.FloodFill(val, conn, x, y - 1, n)
        Me.FloodFill(val, conn, x, y + 1, n)
    End Sub


#End Region ' Internals

End Class
