'==============================================================================
'
' $Log: cTimeSeriesClipboardReader.vb,v $
' Revision 1.1  2008/09/26 07:30:33  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2007/10/02 18:55:30  jeroens
' * Fixed major TS mis-allocation category issue
'
' Revision 1.3  2007/07/12 16:29:39  jeroens
' + Moved
'
' Revision 1.1  2007/07/12 15:49:39  jeroens
' * Moved
'
' Revision 1.5  2007/07/08 18:22:30  jeroens
' * Fixing globalization todo's
'
' Revision 1.4  2007/05/17 03:02:16  jeroens
' * Commented
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
Imports System.Windows.Forms
Imports System.IO

#End Region ' Imports directive

''' ---------------------------------------------------------------------------
''' <summary>
''' Reads one or more time series from the clipboard.
''' </summary>
''' <remarks>
''' For a description of the clipboard text layout, refer to 
''' <see cref="cTimeSeriesTextReader">cTimeSeriesTextReader</see>.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cTimeSeriesClipboardReader
    Inherits cTimeSeriesTextReader

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
    ''' Access the content of the <see cref="Clipboard">Clipboard</see> via 
    ''' a <see cref="TextReader">TextReader</see>.
    ''' </summary>
    ''' <returns>A TextReader connected to the clipboard text.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function GetReader() As TextReader
        Return New StringReader(Me.GetClipboardText(TextDataFormat.Text))
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
    ''' Get a description of the clipboard.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property Dataset() As String
        Get
            Return My.Resources.CoreDefaults.TIMESERIES_SOURCE_CLIPBOARD
        End Get
    End Property

#Region " Private helper bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, returns any text currently on the clipboard.
    ''' </summary>
    ''' <param name="tdf"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function GetClipboardText(ByVal tdf As TextDataFormat) As String
        Dim strText As String = ""

        If Clipboard.ContainsText(tdf) Then
            ' Get the text
            strText = Clipboard.GetText(tdf)
            ' Post-process
        End If
        Return strText

    End Function

#End Region ' Private helper bits

End Class
