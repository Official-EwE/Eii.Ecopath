' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Globalization
Imports System.IO
Imports CsvHelper
Imports CsvHelper.Configuration
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells

Namespace Controls.EwEGrid

    ''' <summary>Reads and writes CSV data for the visible cells of an EwE grid.</summary>
    ''' <remarks>
    ''' This adapter operates on the supplied grid; callers are responsible for any model locks and batch-edit scope.
    ''' It leaves the supplied reader or writer open. CSV parsing and quoting are handled by CsvHelper.
    ''' </remarks>
    Public Class cEwEGridCsv

        Private ReadOnly m_grid As cEwEGrid
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEwEGridCsv)()

        ''' <summary>Creates a CSV adapter for a grid.</summary>
        ''' <param name="grid">The grid whose visible rows and columns are imported or exported.</param>
        Public Sub New(grid As cEwEGrid)
            If grid Is Nothing Then Throw New ArgumentNullException(NameOf(grid))
            m_grid = grid
        End Sub

        Private Class PendingCell
            Public Property Cell As ICell
            Public Property Row As Integer
            Public Property Column As Integer
            Public Property Text As String
            Public Property Value As Object
        End Class

        ''' <summary>Validates CSV content and applies it to editable grid cells.</summary>
        ''' <param name="reader">CSV input; remains open after this method returns.</param>
        ''' <param name="errorMessage">Receives the failure reason, including a one-based CSV record and column when available.</param>
        ''' <param name="changesStarted">True once the application phase begins; a failed import may then have changed cells.</param>
        ''' <returns>True when the entire import succeeds; otherwise, false.</returns>
        ''' <remarks>
        ''' Comma, semicolon and tab delimiters are detected. Numbers use invariant formatting, with a decimal point and no grouping.
        ''' Visible dimensions and fixed headers must match. Read-only cells are not changed.
        ''' All editable values are parsed before application, but errors while applying them are not rolled back.
        ''' </remarks>
        Public Function Read(reader As TextReader, ByRef errorMessage As String, ByRef changesStarted As Boolean) As Boolean
            errorMessage = Nothing
            changesStarted = False
            Dim row As Integer = 0
            Dim column As Integer = 0
            Dim failure As Exception = Nothing
            Dim pending As New List(Of PendingCell)()
            Dim visibleRows As New List(Of Integer)()
            Dim visibleColumns As New List(Of Integer)()
            ' CSV positions correspond only to visible grid rows and columns, not their absolute indices.
            For i As Integer = 0 To m_grid.RowsCount - 1
                If m_grid.Rows(i).Visible Then visibleRows.Add(i)
            Next
            For i As Integer = 0 To m_grid.ColumnsCount - 1
                If m_grid.Columns(i).Visible Then visibleColumns.Add(i)
            Next

            ' Blank records count toward the visible layout; the parser leaves the caller's reader open.
            Dim config As New CsvConfiguration(CultureInfo.InvariantCulture) With {
                .DetectDelimiter = True,
                .DetectDelimiterValues = New String() {",", ";", vbTab},
                .IgnoreBlankLines = False
            }
            Try
                Using parser As New CsvParser(reader, config, True)
                    While parser.Read()
                        Dim fields As String() = parser.Record
                        row += 1
                        If row > visibleRows.Count OrElse fields.Length <> visibleColumns.Count Then
                            Throw New FormatException(My.Resources.GRID_CSV_LAYOUT_FAILURE)
                        End If
                        Dim gridRow As Integer = visibleRows(row - 1)
                        For index As Integer = 0 To fields.Length - 1
                            column = index + 1
                            Dim gridColumn As Integer = visibleColumns(index)
                            Dim cell As ICell = m_grid(gridRow, gridColumn)
                            If cell Is Nothing Then Continue For
                            ' Fixed headers identify the layout; matching them prevents values being applied to the wrong rows or columns.
                            If gridRow < m_grid.FixedRows OrElse
                               (gridColumn < m_grid.FixedColumns AndAlso TypeOf cell Is cEwERowHeaderCell) OrElse
                               (gridColumn < m_grid.FixedColumns AndAlso TypeOf cell Is cPropertyRowHeaderCell) Then
                                If Not String.Equals(fields(index), cell.DisplayText, StringComparison.Ordinal) Then
                                    Throw New FormatException(My.Resources.GRID_CSV_LAYOUT_FAILURE)
                                End If
                            ElseIf cell.DataModel IsNot Nothing AndAlso cell.DataModel.EnableEdit AndAlso
                                   cell.DataModel.EditableMode <> EditableMode.None Then
                                Dim text As String = fields(index)
                                Dim value As Object = text
                                Dim valueType As Type = cell.DataModel.ValueType
                                If valueType IsNot GetType(String) Then
                                    text = text.Trim()
                                    ' Empty non-string fields use the model's null sentinel; floating-point parsing excludes grouping separators.
                                    If text.Length = 0 Then text = cCore.NULL_VALUE.ToString(CultureInfo.InvariantCulture)
                                    Dim style As NumberStyles = NumberStyles.Float
                                    If valueType Is GetType(Single) Then
                                        value = Single.Parse(text, style, CultureInfo.InvariantCulture)
                                    ElseIf valueType Is GetType(Double) Then
                                        value = Double.Parse(text, style, CultureInfo.InvariantCulture)
                                    ElseIf valueType Is GetType(Decimal) Then
                                        value = Decimal.Parse(text, style, CultureInfo.InvariantCulture)
                                    ElseIf valueType Is GetType(Integer) Then
                                        value = Integer.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture)
                                    ElseIf valueType Is GetType(Long) Then
                                        value = Long.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture)
                                    ElseIf valueType Is GetType(Boolean) Then
                                        Dim integerValue As Integer
                                        value = If(Integer.TryParse(text, integerValue), Convert.ToBoolean(integerValue), Boolean.Parse(text))
                                    Else
                                        Throw New FormatException(String.Format(My.Resources.GRID_CSV_UNSUPPORTED_TYPE, valueType.Name))
                                    End If
                                End If
                                pending.Add(New PendingCell With {.Cell = cell, .Row = gridRow, .Column = gridColumn, .Text = text, .Value = value})
                            End If
                        Next
                    End While
                End Using
                If row <> visibleRows.Count Then Throw New FormatException(My.Resources.GRID_CSV_LAYOUT_FAILURE)

                ' Apply only after the whole file has been validated; application failures cannot be rolled back here.
                changesStarted = True
                For Each entry As PendingCell In pending
                    row = visibleRows.IndexOf(entry.Row) + 1
                    column = visibleColumns.IndexOf(entry.Column) + 1
                    ' Preserve the grid's existing interception hook for non-string values.
                    If entry.Cell.DataModel.ValueType Is GetType(String) OrElse
                       Not m_grid.InterceptCsvValue(entry.Text, New Position(entry.Row, entry.Column), entry.Cell) Then
                        entry.Cell.Value = entry.Value
                        Dim propertyCell As cPropertyCell = TryCast(entry.Cell, cPropertyCell)
                        If propertyCell IsNot Nothing AndAlso
                           (propertyCell.GetProperty().GetStyle() And cStyleGuide.eStyleFlags.FailedValidation) <> 0 Then
                            Throw New InvalidDataException(My.Resources.GRID_CSV_VALUE_REJECTED)
                        End If
                    End If
                Next
                Return True
                ' Expected input and model validation failures are reported to the caller without error logging.
            Catch ex As InvalidDataException When changesStarted
                failure = ex
            Catch ex As FormatException When Not changesStarted
                failure = ex
            Catch ex As OverflowException When Not changesStarted
                failure = ex
            Catch ex As CsvHelperException When Not changesStarted
                failure = ex
            Catch ex As Exception
                m_logger.LogError(ex, "EwEGrid CSV import failed")
                failure = ex
            End Try
            errorMessage = If(row > 0 AndAlso column > 0,
                              String.Format(My.Resources.GRID_CSV_CELL_FAILURE, row, column, failure.Message), failure.Message)
            Return False
        End Function

        ''' <summary>Writes visible grid cells as comma-separated CSV using invariant numeric formatting.</summary>
        ''' <param name="writer">CSV output; remains open after this method returns.</param>
        ''' <param name="errorMessage">Receives the reason if writing fails.</param>
        ''' <returns>True when writing succeeds; otherwise, false.</returns>
        ''' <remarks>
        ''' Floating-point values use round-trip formatting; decimal and integer values also use invariant formatting.
        ''' Null-styled cells are empty, and remaining cells use their display text. A failure can leave partial output.
        ''' </remarks>
        Public Function Write(writer As TextWriter, ByRef errorMessage As String) As Boolean
            errorMessage = Nothing
            Try
                Using csv As New CsvWriter(writer, CultureInfo.InvariantCulture, True)
                    For row As Integer = 0 To m_grid.RowsCount - 1
                        If Not m_grid.Rows(row).Visible Then Continue For
                        For column As Integer = 0 To m_grid.ColumnsCount - 1
                            If Not m_grid.Columns(column).Visible Then Continue For
                            Dim cell As ICell = m_grid(row, column)
                            Dim text As String = ""
                            If cell IsNot Nothing Then
                                ' Use underlying numeric values rather than rounded or locale-specific display text.
                                Dim value As Object = cell.Value
                                Dim eweCell As cEwECellBase = TryCast(cell, cEwECellBase)
                                If eweCell IsNot Nothing AndAlso (eweCell.Style And cStyleGuide.eStyleFlags.Null) <> 0 Then
                                    text = ""
                                ElseIf TypeOf value Is Single Then
                                    text = DirectCast(value, Single).ToString("R", CultureInfo.InvariantCulture)
                                ElseIf TypeOf value Is Double Then
                                    text = DirectCast(value, Double).ToString("R", CultureInfo.InvariantCulture)
                                ElseIf TypeOf value Is Decimal OrElse TypeOf value Is Integer OrElse TypeOf value Is Long Then
                                    text = Convert.ToString(value, CultureInfo.InvariantCulture)
                                Else
                                    text = cell.DisplayText
                                End If
                            End If
                            ' CsvHelper quotes delimiters, quotes and line breaks as needed.
                            csv.WriteField(text)
                        Next
                        csv.NextRecord()
                    Next
                    csv.Flush()
                End Using
                Return True
            Catch ex As Exception
                m_logger.LogError(ex, "EwEGrid CSV export failed")
                errorMessage = ex.Message
                Return False
            End Try
        End Function

    End Class
End Namespace
