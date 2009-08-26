'==============================================================================
'
' $Log: ShowAllFitsPlotData.vb,v $
' Revision 1.1  2008/09/26 07:31:50  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/02/12 23:06:55  jeroens
' Revised and debugged
'
'==============================================================================

Option Strict On
Imports EwECore

Namespace Ecosim

    Public Class ShowAllFitsPlotData

        Private m_ts As cTimeSeries
        Private m_asSimData As Single()
        Private m_sYScale As Single = 1.0
        Private m_sYScaleDefault As Single = 1.0
        Private m_sTSDataScale As Single = 1.0
        Private m_bVisible As Boolean = True
        Private m_bSelected As Boolean = True

        Public Sub New(ByVal ts As cTimeSeries, ByVal asSimData As Single())

            ' Sanity check(s)
            Debug.Assert(ts IsNot Nothing)

            Me.m_ts = ts
            Me.m_asSimData = asSimData

            Me.CalculateScale()

        End Sub

        Public Function TimeSeries() As cTimeSeries
            Return Me.m_ts
        End Function

        Public Function SimData() As Single()
            Return Me.m_asSimData
        End Function

        Public Property YMax() As Single
            Get
                Return Me.m_sYScale
            End Get
            Set(ByVal value As Single)
                Me.m_sYScale = value
            End Set
        End Property

        Public Property YMaxDefault() As Single
            Get
                Return Me.m_sYScaleDefault
            End Get
            Set(ByVal value As Single)
                Me.m_sYScaleDefault = value
            End Set
        End Property

        Public Property TSDataScale() As Single
            Get
                Return Me.m_sTSDataScale
            End Get
            Private Set(ByVal value As Single)
                Me.m_sTSDataScale = value
            End Set
        End Property

        Public Property Visible() As Boolean
            Get
                Return Me.m_bVisible
            End Get
            Set(ByVal value As Boolean)
                Me.m_bVisible = value
            End Set
        End Property

        ''' <summary>
        ''' States whether the user has selected this plot or viewing
        ''' </summary>
        Public Property Selected() As Boolean
            Get
                Return Me.m_bSelected
            End Get
            Set(ByVal value As Boolean)
                Me.m_bSelected = value
            End Set
        End Property

        Private Sub CalculateScale()

            Dim asData As Single() = Nothing
            Dim sMax As Single = 0

            ' Find data max across sim results
            asData = Me.m_asSimData
            If Not Object.ReferenceEquals(asData, Nothing) Then
                For j As Integer = 1 To asData.Length - 1
                    sMax = Math.Max(asData(j), sMax)
                Next
            End If

            Me.m_sTSDataScale = 1.0

            ' Find data max across time series
            If (Not Object.ReferenceEquals(Me.m_ts, Nothing)) Then
                If ((Me.m_ts.TimeSeriesType = eTimeSeriesType.BiomassRel) Or _
                    (Me.m_ts.TimeSeriesType = eTimeSeriesType.TotalMortality) Or _
                    (Me.m_ts.TimeSeriesType = eTimeSeriesType.AverageWeight)) Then

                    ' JS 12feb08: EwE5 uses eDatQ here, which is e^DataQ!
                    'jb 26Aug09 added eDatQ to timeseries data
                    If (Me.m_ts.DataQ <> 0) Then Me.TSDataScale = CSng(1.0! / Me.m_ts.eDataQ)
                End If

                asData = Me.m_ts.ShapeData
                For j As Integer = 1 To asData.Length - 1
                    sMax = Math.Max(asData(j) * Me.TSDataScale, sMax)
                Next
            End If

            ' Store
            Me.m_sYScale = (sMax / 0.8!)
            Me.m_sYScaleDefault = Me.m_sYScale

        End Sub

    End Class

End Namespace ' Ecosim
