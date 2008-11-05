'==============================================================================
'
' $Log: cTimeSeriesTextReader.vb,v $
' Revision 1.3  2008/11/05 05:07:13  jeroens
' More leniency
'
' Revision 1.2  2008/11/04 18:52:23  jeroens
' Provided more thorough error feedback on unexpected headers
' Resolved parsing problems on typical clear values
'
' Revision 1.1  2008/09/26 07:30:34  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports EwECore.DataSources
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports System.IO
Imports System.Globalization
Imports System.Text

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Reads one or more time series from a text input source.
''' </summary>
''' <remarks>
''' ToDo_JS: describe time series text input format.
''' </remarks>
''' ---------------------------------------------------------------------------
Public MustInherit Class cTimeSeriesTextReader
    Implements ICollection(Of cTimeSeriesImport)

    Private m_core As cCore = Nothing

    ''' <summary>Start year of the time series.</summary>
    Private m_iFirstYear As Integer = cCore.NULL_VALUE
    ''' <summary>Number of years in the time series.</summary>
    Private m_iNumYears As Integer = cCore.NULL_VALUE

    ''' <summary>Internal list of read time series objects.</summary>
    Private m_ts As New List(Of cTimeSeriesImport)
    ''' <summary>A <see cref="cPreview">preview</see> how the reader has interpreted the text source, allowing a user interface to tune the read process.</summary>
    Private m_tsPreview As cPreview = Nothing
    ''' <summary>String delimiting character to use when splitting the text into different columns.</summary>
    Private m_strDelimiter As String = ""
    ''' <summary>Decimal separator to use when interpreting floating point values in the text.</summary>
    Private m_strDecimalSeparator As String = ""

#Region " Preview class "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class; showing how a <see cref="cTimeSeriesTextReader">cTimeSeriesTextReader</see>
    ''' has interpreted the incoming time series data. The preview allows a user interface to
    ''' interactively adjust the reader to correctly import time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cPreview

        ''' <summary>Number of columns encountered in the external time series text.</summary>
        ''' <remarks>Note that this number does not per definition denote the number of time series in the external text!</remarks>
        Private m_iColumnCount As Integer
        ''' <summary>Original lines of text encountered in the time series text.</summary>
        Private m_alRows As New ArrayList
        ''' <summary>Lines of text from the time series text, split by delimiter.</summary>
        Private m_alRowValues As New ArrayList
        ''' <summary>Errors encountered for each line of text.</summary>
        Private m_alRowErrors As New ArrayList

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="iColumnCount">The number of columns found in the time series text.</param>
        ''' -----------------------------------------------------------------------
        Friend Sub New(ByVal iColumnCount As Integer)
            Me.m_iColumnCount = iColumnCount
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Add a row to the preview.
        ''' </summary>
        ''' <param name="strLine">The original line of text, as read from the time series text.</param>
        ''' <param name="astrValues">The line of text, as split by the requested delimiter.</param>
        ''' -----------------------------------------------------------------------
        Friend Sub AddRow(ByVal strLine As String, ByVal astrValues() As String)
            Me.m_alRows.Add(strLine)
            Me.m_alRowValues.Add(astrValues)
            Me.m_alRowErrors.Add(New Text.StringBuilder)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of rows in the preview.
        ''' </summary>
        ''' <returns>The number of rows in the preview</returns>
        ''' -----------------------------------------------------------------------
        Public Function RowCount() As Integer
            Return Me.m_alRowValues.Count
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of columns in the preview.
        ''' </summary>
        ''' <returns>The number of columns in the preview</returns>
        ''' -----------------------------------------------------------------------
        Public Function ColumnCount() As Integer
            Return Me.m_iColumnCount
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns an original line of text.
        ''' </summary>
        ''' <param name="iRow">The row number to obtain the text for. Note that this
        ''' value is 1-based.</param>
        ''' <returns>An original row of text, as read from the time series text.</returns>
        ''' -----------------------------------------------------------------------
        Public Function Row(ByVal iRow As Integer) As String
            If (iRow > 0 And iRow <= Me.m_alRowErrors.Count) Then Return CStr(Me.m_alRows(iRow - 1))
            Return ""
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns an error for a line from the time series text.
        ''' </summary>
        ''' <param name="iRow">The row number to obtain the error text for. Note that this
        ''' value is 1-based.</param>
        ''' <value></value>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property RowError(ByVal iRow As Integer) As StringBuilder
            Get
                If (iRow > 0 And iRow <= Me.m_alRowErrors.Count) Then Return DirectCast(Me.m_alRowErrors(iRow - 1), StringBuilder)
                Return Nothing
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns a (col, row) value as distilled from the time series text.
        ''' </summary>
        ''' <param name="iColumn">The column number to obtain the error text for. Note that this
        ''' value is 1-based.</param>
        ''' <param name="iRow">The row number to obtain the error text for. Note that this
        ''' value is 1-based.</param>
        ''' <returns>A (col, row) value as distilled from the time series text.</returns>
        ''' -----------------------------------------------------------------------
        Public Property Value(ByVal iColumn As Integer, ByVal iRow As Integer) As String
            Get
                If (iRow > 0 And iRow <= Me.m_alRowErrors.Count) Then
                    Dim astrValues As String() = CType(Me.m_alRowValues(iRow - 1), String())
                    If (iColumn > 0 And iColumn <= astrValues.Length) Then
                        Return astrValues(iColumn - 1)
                    End If
                End If
                Return ("")
            End Get
            Friend Set(ByVal value As String)
                If (iRow > 0 And iRow <= Me.m_alRowErrors.Count) Then
                    Dim astrValues As String() = CType(Me.m_alRowValues(iRow - 1), String())
                    If (iColumn > 0 And iColumn <= astrValues.Length) Then
                        astrValues(iColumn - 1) = value
                        Me.m_alRowValues(iRow - 1) = astrValues
                    End If
                End If
            End Set
        End Property

    End Class

#End Region ' Preview class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' <param name="core">A reference to the <see cref="cCore">Core</see> that
    ''' this reader belongs to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal core As cCore)
        Me.m_core = core
    End Sub

#Region " Reading "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reads any number of Time Series data from a text source. The
    ''' Time Series are exposed by this collection as <see cref="cTimeSeries">cTimeSeries</see>
    ''' objects.
    ''' </summary>
    ''' <param name="strDelimiter">
    ''' String delimiting character to use when splitting the text into different columns.
    ''' </param>
    ''' <param name="strDecimalSeparator">
    ''' Decimal separator to use when interpreting floating point values in the text.
    ''' </param>
    ''' <returns>True when succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Read(ByVal strDelimiter As String, ByVal strDecimalSeparator As String) As Boolean

        ' Reset reader to clear any previous read results.
        Me.Reset()

        ' Sanity check
        If String.Compare(strDelimiter, strDecimalSeparator, True) = 0 Then
            Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_IDENTICALSEPARATORS)
            Return False
        End If

        ' Store delimiter and decimal separator
        Me.m_strDelimiter = strDelimiter
        Me.m_strDecimalSeparator = strDecimalSeparator

        ' Asses data validity and build preview
        If Not Me.AnalyzeData() Then Return False
        ' Read the data
        Return Me.ReadTimeSeriesFromText()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The one entry point to access the source text reader. Override this method 
    ''' to implement a connection to the appropriate text source. Note that the
    ''' reader obtained via this method should be released by overriding
    ''' <see cref="ReleaseReader">ReleaseReader</see>.
    ''' </summary>
    ''' <returns>A TextReader if the connection could be made, or
    ''' Nothing if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function GetReader() As TextReader

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The one entry point to release a text reader obtained via
    ''' <see cref="GetReader">GetReader</see>.
    ''' </summary>
    ''' <returns>A TextReader if the connection could be made, or
    ''' Nothing if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function ReleaseReader(ByVal reader As TextReader) As Boolean

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reset the reader.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub Reset()
        ' Remove read time series
        Me.m_ts.Clear()
        ' Clear preview
        Me.m_tsPreview = Nothing
        Me.m_iFirstYear = cCore.NULL_VALUE
        Me.m_iNumYears = cCore.NULL_VALUE
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Analyze the data
    ''' </summary>
    ''' <returns>True if valid</returns>
    ''' -----------------------------------------------------------------------
    Private Function AnalyzeData() As Boolean

        Dim reader As TextReader = Me.GetReader()
        Dim strLine As String = ""
        Dim astrCols() As String
        Dim iLineNumber As Integer = 0
        Dim iYear As Integer = 0
        Dim iPrevYear As Integer = 0
        Dim bSucces As Boolean = True

        ' Sanity checks
        If (reader Is Nothing) Then Return False

        Try
            ' Count number of captions from header line
            strLine = Me.ReadLine(reader, iLineNumber)
            astrCols = Me.SplitLine(strLine)

            ' Init preview
            Me.m_tsPreview = New cPreview(astrCols.Length)
            ' Add header to preview
            Me.m_tsPreview.AddRow(strLine, astrCols)

            ' Next line
            strLine = Me.ReadLine(reader, iLineNumber)
            astrCols = Me.SplitLine(strLine)
            Me.m_tsPreview.AddRow(strLine, astrCols)

            ' Is this the weight line?
            ' 060613VC: There may be a Weight for each time series from now on
            If StringUtils.BeginsWith(astrCols(0), "weight") Then
                If Not Me.ValidateLine(m_tsPreview.ColumnCount, astrCols) Then
                    Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_WEIGHTVALUEMISSING, iLineNumber)
                    bSucces = False
                End If

                ' Advance to next line
                strLine = Me.ReadLine(reader, iLineNumber)
                astrCols = Me.SplitLine(strLine)
                Me.m_tsPreview.AddRow(strLine, astrCols)
            End If

            ' Pool code
            If Not StringUtils.BeginsWithOneOf(astrCols(0), New String() {"pool", "group", "fleet"}) Then
                Me.ReportError(String.Format(My.Resources.CoreMessages.TIMESERIES_ERROR_POOLLINEMISSING, astrCols(0)), iLineNumber)
                bSucces = False
            End If
            If Not Me.ValidateLine(m_tsPreview.ColumnCount, astrCols) Then
                Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_POOLVALUEMISSING, iLineNumber)
                bSucces = False
            End If

            ' ToDo: validate pool code values

            ' Next line
            strLine = Me.ReadLine(reader, iLineNumber)
            astrCols = Me.SplitLine(strLine)
            Me.m_tsPreview.AddRow(strLine, astrCols)

            ' Dat type
            If Not StringUtils.BeginsWithOneOf(astrCols(0), New String() {"type", "code", "dat"}) Then
                Me.ReportError(String.Format(My.Resources.CoreMessages.TIMESERIES_ERROR_TYPELINEMISSING, astrCols(0)), iLineNumber)
                bSucces = False
            End If

            If Not Me.ValidateLine(m_tsPreview.ColumnCount, astrCols) Then
                Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_TYPEVALUEMISSING, iLineNumber)
                bSucces = False
            End If

            ' ToDo: validate types, etc

            ' Next
            strLine = Me.ReadLine(reader, iLineNumber)
            While Not String.IsNullOrEmpty(strLine)

                astrCols = Me.SplitLine(strLine)
                Me.m_tsPreview.AddRow(strLine, astrCols)

                Try
                    iYear = Integer.Parse(astrCols(0), Globalization.NumberStyles.Integer)
                Catch ex As Exception
                    Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_YEARLINEMISSING, iLineNumber)
                    bSucces = False
                End Try

                ' Fix Start year if not set
                If (Me.m_iFirstYear = cCore.NULL_VALUE) Then Me.m_iFirstYear = iYear

                ' Check year increment
                If iPrevYear <> 0 Then
                    If iYear <> (iPrevYear + 1) Then
                        Me.ReportError(String.Format(My.Resources.CoreMessages.TIMESERIES_ERROR_YEARMISSING, CInt(iPrevYear + 1)), iLineNumber)
                        bSucces = False
                    End If
                    iPrevYear = iYear
                End If

                If Not Me.ValidateLine(m_tsPreview.ColumnCount, astrCols) Then
                    Me.ReportError(String.Format(My.Resources.CoreMessages.TIMESERIES_ERROR_YEARVALUEMISSING, iYear), iLineNumber)
                    bSucces = False
                End If

                ' Next
                strLine = Me.ReadLine(reader, iLineNumber)
            End While

            ' Set number of years
            Me.m_iNumYears = iYear - Me.m_iFirstYear + 1

        Catch ex As Exception
            ' Report generic error
            Me.ReportError(ex.Message)
            ' Abort any attempt to make sense of this
            Me.Reset()
            ' Woops!
            bSucces = False
        End Try

        Me.ReleaseReader(reader)

        ' Bye!
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reads time series in local collection of cTimeSeries objects.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ReadTimeSeriesFromText() As Boolean

        Dim tr As TextReader = Me.GetReader()
        Dim ts As cTimeSeriesImport = Nothing
        Dim iNumSeries As Integer = m_tsPreview.ColumnCount - 1
        Dim strLine As String = ""
        Dim iLineNumber As Integer = 0
        Dim astrCols() As String

        ' Temp buffers for creating Time Series objects
        Dim asWtType(iNumSeries) As Single
        Dim astrNames(iNumSeries) As String
        Dim aiDatPool(iNumSeries) As Integer
        Dim aiType(iNumSeries) As eTimeSeriesType

        ' Culturization ;)
        Dim ci As CultureInfo = System.Globalization.CultureInfo.InstalledUICulture
        Dim ni As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)
        ni.NumberDecimalSeparator = Me.m_strDecimalSeparator

        ' Sanity checks
        If (tr Is Nothing) Then Return False

        ' Init all weights to 1 by default
        For i As Integer = 1 To iNumSeries : asWtType(i) = 1.0! : Next i

        ' Read names from columns
        astrCols = Me.SplitLine(Me.ReadLine(tr, iLineNumber))
        For i As Integer = 1 To iNumSeries : astrNames(i - 1) = astrCols(i) : Next i

        ' Read weight from columns
        astrCols = Me.SplitLine(Me.ReadLine(tr, iLineNumber))
        If (String.Compare(astrCols(0), "weight", True) = 0) Then
            Try
                For i As Integer = 1 To iNumSeries : asWtType(i - 1) = Single.Parse(astrCols(i), ni) : Next i
            Catch ex As Exception
                Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_WEIGHTFORMAT, iLineNumber)
            End Try
            astrCols = Me.SplitLine(Me.ReadLine(tr, iLineNumber))
        End If

        ' Read pool code from columns
        Try
            For i As Integer = 1 To iNumSeries
                aiDatPool(i - 1) = Integer.Parse(astrCols(i), ni)
            Next i
        Catch ex As Exception
            Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_POOLFORMAT, iLineNumber)
        End Try
        astrCols = Me.SplitLine(Me.ReadLine(tr, iLineNumber))

        ' Read type from columns
        Try

            For i As Integer = 1 To iNumSeries
                aiType(i - 1) = CType(Integer.Parse(astrCols(i), ni), eTimeSeriesType)

                ' Validate if encountered pool code fits the corresponding core counter
                Select Case cTimeSeriesFactory.TimeSeriesCategory(CType(aiType(i - 1), eTimeSeriesType))

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group
                        ' Group index cannot exceed core nGroups
                        If aiDatPool(i - 1) >= Me.m_core.GetCoreCounter(eCoreCounterTypes.nGroups) Then
                            Me.ReportError(String.Format(My.Resources.CoreMessages.TIMESERIES_ERROR_INVALIDGROUP, aiDatPool(i - 1), astrNames(i - 1)), iLineNumber - 1)
                        End If

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet
                        'Fleet index cannot exceed core nFleets
                        If aiDatPool(i - 1) >= Me.m_core.GetCoreCounter(eCoreCounterTypes.nFleets) Then
                            Me.ReportError(String.Format(My.Resources.CoreMessages.TIMESERIES_ERROR_INVALIDFLEET, aiDatPool(i - 1), astrNames(i - 1)), iLineNumber - 1)
                        End If

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Forcing
                        ' All good

                End Select
            Next i

        Catch ex As Exception
            Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_TYPEFORMAT, iLineNumber)
        End Try
        astrCols = Me.SplitLine(Me.ReadLine(tr, iLineNumber))

        ' Initialize time series objects
        For i As Integer = 0 To iNumSeries - 1

            ts = New cTimeSeriesImport(Me.m_iNumYears, aiType(i))

            ' Configure time series
            With ts
                .Name = astrNames(i)
                .WtType = asWtType(i)
                .DatPool = aiDatPool(i)
                .ResizeData(Me.m_iNumYears)
            End With

            ' Add it
            Me.m_ts.Add(ts)
        Next

        ' Years
        For iRow As Integer = 1 To Me.m_iNumYears

            For iColumn As Integer = 1 To iNumSeries

                ' Validate single value
                Dim sValue As Single = 0.0
                Dim strValue As String = ""

                ' Has a column value?
                If (iColumn < astrCols.Length) Then
                    ' #Yes: is not an empty string?
                    If (Not String.IsNullOrEmpty(astrCols(iColumn))) Then
                        ' #Yes: get the value
                        strValue = Me.FixKnownInvalidValue(astrCols(iColumn))

                        Try
                            ' Try to parse the value
                            sValue = Single.Parse(strValue, ni)
                            ' Add parsed value to preview
                            Me.m_tsPreview.Value(iColumn + 1, iLineNumber) = CStr(sValue)
                            ' Is value negative?
                            If (sValue < 0.0!) Then
                                ' #Yes: throw an error
                                Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_VALUENEGATIVE)
                            End If
                        Catch ex As Exception
                            ' JS04feb08: error parsing value
                            Me.ReportError(My.Resources.CoreMessages.TIMESERIES_ERROR_VALUEFORMAT, iLineNumber)
                            ' Add original string to preview
                            Me.m_tsPreview.Value(iColumn + 1, iLineNumber) = strValue
                        End Try
                    End If
                End If

                ' Store converted value
                Me.m_ts(iColumn - 1).ShapeData(iRow - 1) = sValue

            Next iColumn

            ' Next line
            astrCols = Me.SplitLine(Me.ReadLine(tr, iLineNumber))

        Next iRow

        Return True

    End Function

#Region " Helper methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Validates the number of columns encountered in a line of text.
    ''' </summary>
    ''' <param name="iNumCols">The number of columns that is expected.</param>
    ''' <param name="astrCols">The columns in the line of text.</param>
    ''' <param name="bAllowMissing">Flag that indicates that validation requires
    ''' whether a row must contain exactly the expected number of columns (false) or
    ''' whether a row is allowed to contain less columns (true).</param>
    ''' <returns>True if the number of columns validated succesfully.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ValidateLine(ByVal iNumCols As Integer, ByVal astrCols() As String, _
                Optional ByVal bAllowMissing As Boolean = False) As Boolean

        If bAllowMissing Then
            Return iNumCols >= astrCols.Length
        Else
            Return iNumCols = astrCols.Length
        End If
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reads a line of text from the time series data.
    ''' </summary>
    ''' <param name="tr">The reader to read from.</param>
    ''' <param name="iLineNumber">The line number that is being read. This number
    ''' is be incremented when a line of text is read succesfully.</param>
    ''' <returns>True when a line of text is read succesfully</returns>
    ''' -----------------------------------------------------------------------
    Private Function ReadLine(ByVal tr As TextReader, ByRef iLineNumber As Integer) As String
        Dim strLine As String = ""

        If tr.Peek() = -1 Then Return strLine
        Try
            strLine = tr.ReadLine()
            iLineNumber += 1
        Catch e As Exception
            Me.ReportError(e.Message, iLineNumber)
        End Try

        Return strLine
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, splits a line of text by the current <see cref="Delimiter">Delimiter</see>.
    ''' </summary>
    ''' <param name="strLine">The line to split.</param>
    ''' <returns>An array of strings.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SplitLine(ByVal strLine As String) As String()
        Dim astrBits As String() = StringUtils.SplitQualified(strLine, Me.m_strDelimiter)
        ' Trim spaces
        For iBit As Integer = 0 To astrBits.Length - 1
            astrBits(iBit) = astrBits(iBit).Trim
        Next
        Return astrBits
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; reports that an error has occurred.
    ''' </summary>
    ''' <param name="strError">Error text to report.</param>
    ''' <param name="iLineNumber">Text line that this error occurred at, or
    ''' <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> if the line number
    ''' is irrelevant.</param>
    ''' -----------------------------------------------------------------------
    Private Sub ReportError(ByVal strError As String, Optional ByVal iLineNumber As Integer = cCore.NULL_VALUE)

        ' Flag line error if possible
        If iLineNumber = cCore.NULL_VALUE Then
            ' Send warning message
            Me.m_core.m_publisher.SendMessage(New cMessage(strError, eMessageType.DataImport, eMessageSource.TimeSeries, eMessageImportance.Warning))
        Else
            Dim sb As StringBuilder = Nothing

            sb = Me.m_tsPreview.RowError(iLineNumber)
            If (sb IsNot Nothing) Then
                If (sb.Length > 0) Then
                    sb.AppendLine()
                End If
                sb.Append(strError)
            End If
        End If

    End Sub

    Private Function FixKnownInvalidValue(ByVal strValue As String) As String
        Select Case strValue.Trim
            Case "-", "_", ""
                strValue = "0"
        End Select
        Return strValue
    End Function

#End Region ' Helper methods

#End Region ' Internals

#End Region ' Reading

#Region " Collection "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an item to the collection.
    ''' </summary>
    ''' <remarks>
    ''' This collection is read-only, only to be manipulated through the <see cref="Read">Read</see> interface.
    ''' </remarks>
    ''' <param name="item">Item NOT to add :P</param>
    ''' -----------------------------------------------------------------------
    Public Sub Add(ByVal item As cTimeSeriesImport) _
            Implements System.Collections.Generic.ICollection(Of cTimeSeriesImport).Add
        ' Read-only
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clears the collection.
    ''' </summary>
    ''' <remarks>
    ''' This collection is read-only, only to be manipulated through the <see cref="Read">Read</see> interface.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub Clear() _
            Implements System.Collections.Generic.ICollection(Of cTimeSeriesImport).Clear
        ' Read-only
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the collection contains a given item.
    ''' </summary>
    ''' <param name="item">The Item to locate in the collection</param>
    ''' <returns>True if the item was found.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Contains(ByVal item As cTimeSeriesImport) As Boolean _
            Implements System.Collections.Generic.ICollection(Of cTimeSeriesImport).Contains
        Return Me.m_ts.Contains(item)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Copies the collection to a strong-typed array of <see cref="cTimeSeries">cTimeSeries</see> objects.
    ''' </summary>
    ''' <param name="array">The array to copy to.</param>
    ''' <param name="arrayIndex">The index to start the copy process at.</param>
    ''' -----------------------------------------------------------------------
    Public Sub CopyTo(ByVal array() As cTimeSeriesImport, ByVal arrayIndex As Integer) Implements _
            System.Collections.Generic.ICollection(Of cTimeSeriesImport).CopyTo
        Me.m_ts.CopyTo(array, arrayIndex)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of items in the collection.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Count() As Integer _
            Implements System.Collections.Generic.ICollection(Of cTimeSeriesImport).Count
        Get
            Return Me.m_ts.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether the collection is read-only.
    ''' </summary>
    ''' <returns>Always true.</returns>
    ''' <remarks>
    ''' This collection is always read-only, only to be manipulated through the <see cref="Read">Read</see> interface.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IsReadOnly() As Boolean _
            Implements System.Collections.Generic.ICollection(Of cTimeSeriesImport).IsReadOnly
        Get
            Return True
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Removes an item to the collection.
    ''' </summary>
    ''' <remarks>
    ''' This collection is read-only, only to be manipulated through the <see cref="Read">Read</see> interface.
    ''' </remarks>
    ''' <param name="item">Item NOT to remove.</param>
    ''' <returns>Always false.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Remove(ByVal item As cTimeSeriesImport) As Boolean _
            Implements System.Collections.Generic.ICollection(Of cTimeSeriesImport).Remove
        ' Read only
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a strong-typed enumerator for this collection.
    ''' </summary>
    ''' <returns>A strong-typed enumerator for this collection.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of cTimeSeriesImport) _
            Implements System.Collections.Generic.IEnumerable(Of cTimeSeriesImport).GetEnumerator
        Return Me.m_ts.GetEnumerator()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a weak-typed enumerator for this collection.
    ''' </summary>
    ''' <remarks>This obligatory override is silly, obsolete and therefore hidden from view.</remarks>
    ''' <returns>A weak-typed enumerator for this collection.</returns>
    ''' -----------------------------------------------------------------------
    Private Function GetEnumeratorObligatoryOverrideWhichWeDoNotNeedAtAllThankYou() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function

#End Region ' Collection

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a double indexed array of strings with a preview of read time
    ''' series data. Data is indexed by (column, row). Indexes are zero based.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Preview() As cPreview
        Return Me.m_tsPreview
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the delimiter used by the reader.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function Delimiter() As String
        Return Me.m_strDelimiter
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the decimal sepearator used by the reader.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function DecimalSeparator() As String
        Return Me.m_strDecimalSeparator
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the first year of time series data found by the reader.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property FirstYear() As Integer
        Get
            Return Me.m_iFirstYear
        End Get
        Friend Set(ByVal iStartYear As Integer)
            Me.m_iFirstYear = iStartYear
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of years of time series data found by the reader.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumYears() As Integer
        Get
            Return Me.m_iNumYears
        End Get
        Friend Set(ByVal iNumYears As Integer)
            Me.m_iNumYears = iNumYears
        End Set
    End Property

    Public MustOverride ReadOnly Property Dataset() As String

#End Region ' Properties

#Region " Ye olde EwE5 code "

    ' Hmm, why do we always manage to blow the number of lines of code when translating to .NET?!

#If 0 Then
    Public Sub ReadRefData(F$)
Dim i As Integer, j As Integer, K As Integer, Tim As Integer, ig As Integer, ip As Integer, Head$
Dim fileErr As Boolean
Dim FilePath As String
fileErr = False
'reads historical abundance comparison data from excel csv (comma delimited text) file
'first determine number of rows and columns in data file
On Local Error GoTo ReadErrorCase
IsDatWtSet = False

Open F$ For Input As 1      'READ CSV FILE:
    Line Input #1, Head$
    Line Input #1, Head$: NdatType = CountCols(Head$)
    '060613VC: There may be a Weight for each time series from now on
    If LCase(Mid(Head, 1, 6)) = "weight" Then
        Line Input #1, Head$: If CountCols(Head$) <> NdatType Then fileErr = True
    End If
    Line Input #1, Head$: If CountCols(Head$) <> NdatType Then fileErr = True
    NdatYear = 0
    Do Until EOF(1)
        Line Input #1, Head$
        If CountCols(Head$) <> NdatType Then fileErr = True ': Stop
        NdatYear = NdatYear + 1
    Loop
    Close 1

    If fileErr Then
        NdatType = 0
        NdatYear = 0
        MsgBox ("CSV file does not have the same number of fields for every data year, so cannot read data from it; you may need to append a dummy data column to the spreadsheet before saving as CSV, with a non-blank value entered for every year (CSV data row")
        Exit Sub
    End If
    TotalTime = NdatYear
    SetCellValue frmSim1.vaSim, 2, 1, TotalTime
    RedimTotalTimeVariables
    RedimCSVvariables

    'now read the data
    TimeSeriesFile = getFilename(F$, FilePath)  'f$
    Open F$ For Input As 1
        Input #1, Head$
        For j = 1 To NdatType: Input #1, DatName(j): Next
        '060613VC: The weights may be read in as a separate line from now on:
        Input #1, Head$
        If LCase(Head) = "weight" Then
            For j = 1 To NdatType: Input #1, WtType(j): Next
            Input #1, Head$
        End If
        For j = 1 To NdatType: Input #1, DatPool(j): Next
        Input #1, Head$
        For j = 1 To NdatType: Input #1, DatType(j): Next
        For i = 1 To NdatYear
            Input #1, DatYear(i)
            For j = 1 To NdatType: Input #1, DatVal(i, j): Next
        Next
        ReDim IsDatShown(NdatType) As Boolean: For i = 1 To NdatType: IsDatShown(i) = True: Next
        DoDatValCalculations NdatYear, NdatType, DatVal(), True
    Close 1
    frmSim1.lblTimeS.Caption = "Timeseries file  " + TimeSeriesFile
    frmMdiEcopath4.mnuSimulationItem(8).Enabled = True 'enableIt    'Ecosim

Exit Sub
#End If

#End Region ' Ye olde EwE5 code

End Class
