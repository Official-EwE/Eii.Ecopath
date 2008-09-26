'==============================================================================
'
' $Log: cTimeSeriesCSVReader.vb,v $
' Revision 1.1  2008/09/26 07:30:33  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2007/10/02 18:55:30  jeroens
' * Fixed major TS mis-allocation category issue
'
' Revision 1.3  2007/07/12 16:29:39  jeroens
' + Moved
'
' Revision 1.1  2007/07/12 15:49:38  jeroens
' * Moved
'
' Revision 1.5  2007/07/08 18:22:30  jeroens
' * Fixing globalization todo's
'
' Revision 1.4  2007/05/17 03:02:07  jeroens
' * Commented
' * DataSet description contains cvs file name without path info
'
' Revision 1.3  2007/05/16 17:12:23  jeroens
' + Added core ref to constructor, Dataset implementation
'
' Revision 1.2  2007/05/15 17:15:11  jeroens
' * Separators strings instead of chars
' + Preview implemented as separate class
' * Fixed bunch of bugs
'
' Revision 1.1  2007/05/14 15:28:37  jeroens
' Again?
'
' Revision 1.3  2007/05/14 15:06:11  jeroens
' Getting there
'
' Revision 1.2  2007/05/14 03:15:23  jeroens
' Implemented (to be tested)
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports System.IO

#End Region ' Imports directive

''' ---------------------------------------------------------------------------
''' <summary>
''' Reads one or more time series from a CSV file.
''' </summary>
''' <remarks>
''' For a description of the CSV file layout, refer to 
''' <see cref="cTimeSeriesTextReader">cTimeSeriesTextReader</see>.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cTimeSeriesCSVReader
    Inherits cTimeSeriesTextReader

    ''' <summary>Path to the CSV file that was read.</summary>
    Private m_strFileName As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' <param name="core">A reference to the <see cref="cCore">Core</see> that
    ''' this reader belongs to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal core As cCore)
        MyBase.New(core)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reads any number of Time Series data from a text source. The
    ''' Time Series are exposed by this collection as <see cref="cTimeSeries">cTimeSeries</see>
    ''' objects.
    ''' </summary>
    ''' <param name="strFileName">Path to the CSV file to read.</param>
    ''' <param name="strDelimiter">
    ''' String delimiting character to use when splitting the text into different columns.
    ''' </param>
    ''' <param name="strDecimalSeparator">
    ''' Decimal separator to use when interpreting floating point values in the text.
    ''' </param>
    ''' <returns>True when succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overloads Function Read(ByVal strFileName As String, ByVal strDelimiter As String, ByVal strDecimalSeparator As String) As Boolean
        ' Store file name
        Me.m_strFileName = strFileName
        ' Let the baseclass do the work
        Return MyBase.Read(strDelimiter, strDecimalSeparator)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Access the content of the CSV file via a <see cref="TextReader">TextReader</see>.
    ''' </summary>
    ''' <returns>A TextReader connected to the CSV file.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function GetReader() As TextReader

        ' Sanity checks
        If String.IsNullOrEmpty(Me.m_strFileName) Then Return Nothing
        If Not File.Exists(Me.m_strFileName) Then Return Nothing

        Return New StreamReader(Me.m_strFileName)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The one entry point to release a text reader obtained via
    ''' <see cref="GetReader">GetReader</see>.
    ''' </summary>
    ''' <returns>A TextReader if the connection could be made, or
    ''' Nothing if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ReleaseReader(ByVal reader As TextReader) As Boolean
        reader.Close()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a description of the CSV file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property Dataset() As String
        Get
            Return Path.GetFileNameWithoutExtension(Me.m_strFileName)
        End Get
    End Property

End Class
