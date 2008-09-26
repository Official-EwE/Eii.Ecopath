'==============================================================================
'
' $Log: cTimeSeriesReaderFactory.vb,v $
' Revision 1.1  2008/09/26 07:30:34  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2007/07/12 16:29:25  jeroens
' + Moved
'
' Revision 1.1  2007/07/12 15:50:02  jeroens
' * Moved
'
' Revision 1.3  2007/06/07 11:17:55  jeroens
' + Commented
'
' Revision 1.2  2007/05/16 17:12:38  jeroens
' + Added core ref to reader constructors
'
' Revision 1.1  2007/05/14 15:05:36  jeroens
' Initial version
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class; creates a <see cref="cTimeSeriesTextReader">Time series reader</see>
''' for a given <see cref="cTimeSeriesReaderFactory.eTimeSeriesReaderTypes">type of Time series input source</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTimeSeriesReaderFactory

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type stating all supported time series input formats.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eTimeSeriesReaderTypes
        ''' <summary>Indicates a reader that can read Time Series data from a comma-separated file.</summary>
        CSV
        ''' <summary>Indicates a reader that can read Time Series data from the clipboard.</summary>
        Clipboard
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return a <see cref="cTimeSeriesTextReader">Time series text reader</see>
    ''' for a given <see cref="cTimeSeriesReaderFactory.eTimeSeriesReaderTypes">type of Time series input source</see>.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore">Core</see> instance to obtain the reader for.</param>
    ''' <param name="readerType">The <see cref="cTimeSeriesReaderFactory.eTimeSeriesReaderTypes">type of Time series input source</see>
    ''' to rad from.</param>
    ''' <returns>
    ''' A <see cref="cTimeSeriesTextReader">Time series text reader</see> if succesful, 
    ''' or Nothing/Null/Nada/Zip if an error occurred.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetTimeSeriesReader(ByVal core As cCore, ByVal readerType As eTimeSeriesReaderTypes) As cTimeSeriesTextReader
        Dim reader As cTimeSeriesTextReader = Nothing
        Select Case readerType
            Case eTimeSeriesReaderTypes.CSV
                reader = New cTimeSeriesCSVReader(core)
            Case eTimeSeriesReaderTypes.Clipboard
                reader = New cTimeSeriesClipboardReader(core)
            Case Else
                ' Wtf
                Debug.Assert(False, String.Format("Unable to create Time series text reader for input source {0}", readerType))
        End Select
        Return reader
    End Function

End Class
