

Imports System.IO
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Public Class cASCIIReaderWriter

    Public Property Core As cCore

    Public data(,) As Single

    Public Sub New(theCore As cCore)
        Me.Core = theCore
    End Sub

    Public Sub ReadASCFile(ByVal strm As StreamReader)
        Try
            data = New Single(Me.Core.EcospaceBasemap.InRow, Me.Core.EcospaceBasemap.InCol) {}
            Me.ReadASCIIHeader(strm)
            Me.ReadASCIIBody(strm)
        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Too hack to be true
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Sub ReadASCIIHeader(ByVal reader As StreamReader)

        Dim strLine As String = ""

        While (String.IsNullOrWhiteSpace(strLine) Or (Not cStringUtils.BeginsWith(strLine, "NODATA_value", True))) And _
              (Not reader.EndOfStream)
            strLine = reader.ReadLine
        End While

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Too hack to be true
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Function ReadASCIIBody(ByVal reader As StreamReader) As Boolean

        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
        Dim depth As cEcospaceLayerDepth = bm.LayerDepth
        Dim value As Single = 0
        Dim strValue As String = ""
        Dim bSuccess As Boolean = True

        Try

            For ir As Integer = 1 To bm.InRow
                'ASC files written by GDAL contain a space at the start of the line so strip it off
                'this should not affect other ASC file reading
                Dim strLine As String = reader.ReadLine.Trim
                Dim astrBits() As String = strLine.Split(" "c)
                For ic As Integer = 1 To Math.Min(bm.InCol, astrBits.Length)

                    'If depth.IsWaterCell(ir, ic) Or Me.m_layerWork.VarName = eVarNameFlags.LayerDepth Then
                    '    bSuccess = bSuccess And Single.TryParse(astrBits(ic - 1), value)
                    'Else
                    '    value = cCore.NULL_VALUE
                    'End If
                    If Single.TryParse(astrBits(ic - 1), value) Then
                        Me.data(ir, ic) = value
                    Else
                        value = cCore.NULL_VALUE
                    End If
                Next
            Next
        Catch ex As Exception
            bSuccess = False
        End Try
        Return bSuccess

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write an entire ASCII file for a group, time step and variable.
    ''' </summary>
    ''' <param name="strm"></param>
    ''' -----------------------------------------------------------------------
    Public Sub SaveASCFile(ByVal strm As StreamWriter)
        Try
            Me.WriteASCIIHeader(strm)
            Me.WriteASCIIBody(strm)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".WriteResults() Exception: " & ex.Message)
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write ESRI ASCII header block.
    ''' </summary>
    ''' <param name="writer">The <see cref="StreamWriter"/> to write to.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub WriteASCIIHeader(ByVal writer As StreamWriter)

        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
        writer.WriteLine("ncols         " & bm.InCol)
        writer.WriteLine("nrows         " & bm.InRow)
        writer.WriteLine("xllcorner     " & bm.PosTopLeft.X)
        writer.WriteLine("yllcorner     " & bm.PosBottomRight.Y)
        writer.WriteLine("cellsize      " & bm.CellSize)
        writer.WriteLine("NODATA_value  " & cCore.NULL_VALUE)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write ESRI ASCII body block.
    ''' </summary>
    ''' <param name="writer">The <see cref="StreamWriter"/> to write to.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub WriteASCIIBody(ByVal writer As StreamWriter)

        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
        Dim depth As cEcospaceLayerDepth = bm.LayerDepth
        Dim value As Double = 0
        Dim strValue As String = ""

        For ir As Integer = 1 To bm.InRow
            For ic As Integer = 1 To bm.InCol
                If ic > 1 Then writer.Write(" ")
                'If depth.IsWaterCell(ir, ic) Or Me.m_layerWork.VarName = eVarNameFlags.LayerDepth Then
                '    value = CSng(Me.m_layerWork.Value(ir, ic))
                'Else
                '    value = cCore.NULL_VALUE
                'End If

                value = data(ir, ic)

                ' Fix #1321 - always make sure the first cell value is written as floating point
                strValue = cStringUtils.FormatNumber(value)
                If (ir = 1 And ic = 1) Then
                    If (strValue.IndexOf("."c) = -1) Then
                        strValue = strValue + ".0"
                    End If
                End If

                writer.Write(strValue)
            Next
            writer.WriteLine("")
        Next

    End Sub

End Class
