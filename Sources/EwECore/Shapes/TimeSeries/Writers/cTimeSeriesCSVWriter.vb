#Region " Imports "

Option Strict On

Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Threading

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Write a time series dataset to a text output source.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTimeSeriesCSVWriter

#Region " Private vars "

    ''' <summary>The core to read from.</summary>
    Private m_core As cCore = Nothing

#End Region ' Private vars

#Region " Constructor "

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

#End Region ' Constructor

#Region " Writing "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a default CSV file name for the current loaded dataset, if any.
    ''' </summary>
    ''' <returns>A file name for the current loaded dataset in the current
    ''' core <see cref="cCore.OutputPath">output path</see>, or an empty string
    ''' if no time series are loaded.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property DefaultFileName() As String
        Get
            If (Me.m_core.ActiveTimeSeriesDatasetIndex = -1) Then Return ""
            ' Get dataset
            Dim ds As cTimeSeriesDataset = Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex)
            ' Is dataset available?
            If (ds Is Nothing) Then Return ""
            ' 
            Return Path.Combine(Me.m_core.OutputPath, cFileUtils.ToValidFileName(ds.Name, True)) & ".csv"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Writes the current loaded time series dataset to a CSV file.
    ''' </summary>
    ''' <param name="strFileName">Name of the file to save to.</param>
    ''' <param name="strDelimiter">String delimiting character to use when 
    ''' separating the text into different columns.</param>
    ''' <param name="strDecimalSeparator">Decimal separator to use when 
    ''' interpreting floating point values in the text.</param>
    ''' <returns>True when succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Write(ByVal strFileName As String, _
                                      ByVal strDelimiter As String, _
                                      ByVal strDecimalSeparator As String) As Boolean

        Dim ds As cTimeSeriesDataset = Nothing
        Dim ts As cTimeSeries = Nothing
        Dim msg As cMessage = Nothing
        Dim bSucces As Boolean = True

        ' Anything to export?
        If (Me.m_core.ActiveTimeSeriesDatasetIndex = -1) Then Return False

        ' Get dataset
        ds = Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex)
        ' Is dataset available?
        If (ds Is Nothing) Then Return False

        Try
            Using sw As StreamWriter = New StreamWriter(strFileName, False)

                ' Titles
                sw.Write("Title")
                For iTS As Integer = 1 To Me.m_core.nTimeSeries
                    ts = Me.m_core.EcosimTimeSeries(iTS)
                    sw.Write(strDelimiter & " ")
                    sw.Write(ts.Name.Replace(strDelimiter, "_"))
                Next
                sw.WriteLine()

                ' Weights
                sw.Write("Weight")
                For iTS As Integer = 1 To Me.m_core.nTimeSeries
                    ts = Me.m_core.EcosimTimeSeries(iTS)
                    sw.Write(strDelimiter & " ")
                    sw.Write(cStringUtils.FormatSingle(ts.WtType, strDecimalSeparator, ""))
                Next
                sw.WriteLine()

                ' Pool code
                sw.Write("Pool code")
                For iTS As Integer = 1 To Me.m_core.nTimeSeries
                    ts = Me.m_core.EcosimTimeSeries(iTS)

                    sw.Write(strDelimiter & " ")
                    If TypeOf ts Is cGroupTimeSeries Then
                        sw.Write(cStringUtils.FormatInteger(DirectCast(ts, cGroupTimeSeries).GroupIndex, _
                                                           strDecimalSeparator, ""))
                    ElseIf TypeOf ts Is cFleetTimeSeries Then
                        sw.Write(cStringUtils.FormatInteger(DirectCast(ts, cFleetTimeSeries).FleetIndex, _
                                                           strDecimalSeparator, ""))
                    Else
                        ' Should never happen, unless a new type of time series is defined.
                        Debug.Assert(False)
                    End If

                Next
                sw.WriteLine()

                ' Type
                sw.Write("Type")
                For iTS As Integer = 1 To Me.m_core.nTimeSeries
                    ts = Me.m_core.EcosimTimeSeries(iTS)
                    sw.Write(strDelimiter & " ")
                    ' Write time series type as int, not as string
                    sw.Write(cStringUtils.FormatInteger(ts.TimeSeriesType, strDecimalSeparator, ""))
                Next
                sw.WriteLine()

                ' Years
                For iYear As Integer = 1 To ds.NumYears
                    sw.Write(ds.FirstYear + iYear - 1)
                    For iTS As Integer = 1 To Me.m_core.nTimeSeries
                        ts = Me.m_core.EcosimTimeSeries(iTS)
                        sw.Write(strDelimiter & " ")
                        sw.Write(cStringUtils.FormatSingle(ts.ShapeData(iYear), strDecimalSeparator, ""))
                    Next
                    sw.WriteLine()
                Next

                sw.Close()

                ' Create success message
                msg = New cMessage(String.Format(My.Resources.CoreMessages.TIMESERIES_EXPORT_SUCCESS, ds.Name, strFileName), _
                                   eMessageType.DataExport, eCoreComponentType.TimeSeries, eMessageImportance.Information)

            End Using

        Catch ex As Exception

            ' Create error message
            msg = New cMessage(String.Format(My.Resources.CoreMessages.TIMESERIES_EXPORT_FAILED, ds.Name, strFileName, ex.Message), _
                               eMessageType.DataExport, _
                               eCoreComponentType.TimeSeries, _
                               eMessageImportance.Critical)
            bSucces = False

        End Try

        ' Has a message to send?
        If (msg IsNot Nothing) Then
            ' #Yes: send it
            Me.m_core.Messages.SendMessage(msg, False)
        End If

        ' Report succes
        Return bSucces

    End Function

#End Region ' Writing

#Region " Internals "


#End Region ' Internals

End Class
