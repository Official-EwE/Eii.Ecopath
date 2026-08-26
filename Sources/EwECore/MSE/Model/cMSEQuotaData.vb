Imports EwECore.MSE

Public Class cMSEQuotaData
    Implements IMSEQuotaData    'interface with all the data needed for the quota calculator. This allows the quota calculator to be used without the full core data structures

    Private m_data As cMSEDataStructures
    Private m_Search As cSearchDatastructures

    Public Sub New(EPdata As cMSEDataStructures,
               SearchData As cSearchDatastructures)

        Me.m_data = EPdata
        Me.m_Search = SearchData
    End Sub

    Public Property nGroups As Integer Implements IMSEQuotaData.nGroups
        Get
            Return Me.m_data.nGroups
        End Get
        Set(value As Integer)
            Throw New NotSupportedException("nGroups is read-only on cMSEDataStructures.")
        End Set
    End Property

    Public Property nLiving As Integer Implements IMSEQuotaData.nLiving
        Get
            Return Me.m_data.nLiving
        End Get
        Set(value As Integer)
            Throw New NotSupportedException("nLiving is read-only on cMSEDataStructures.")
        End Set
    End Property

    Public Property nFleets As Integer Implements IMSEQuotaData.nFleets
        Get
            Return Me.m_data.nFleets
        End Get
        Set(value As Integer)
            Throw New NotSupportedException("nFleets is read-only on cMSEDataStructures.")
        End Set
    End Property

    Public Property TAC As Single() Implements IMSEQuotaData.TAC
        Get
            Return Me.m_data.TAC
        End Get
        Set(value As Single())
            Me.m_data.TAC = value
        End Set
    End Property

    Public Property FixedEscapement As Single() Implements IMSEQuotaData.FixedEscapement
        Get
            Return Me.m_data.FixedEscapement
        End Get
        Set(value As Single())
            Me.m_data.FixedEscapement = value
        End Set
    End Property

    Public Property FixedF As Single() Implements IMSEQuotaData.FixedF
        Get
            Return Me.m_data.FixedF
        End Get
        Set(value As Single())
            Me.m_data.FixedF = value
        End Set
    End Property

    Public Property Fopt As Single() Implements IMSEQuotaData.Fopt
        Get
            Return Me.m_data.Fopt
        End Get
        Set(value As Single())
            Me.m_data.Fopt = value
        End Set
    End Property

    Public Property Fmin As Single() Implements IMSEQuotaData.Fmin
        Get
            Return Me.m_data.Fmin
        End Get
        Set(value As Single())
            Me.m_data.Fmin = value
        End Set
    End Property

    Public Property Bbase As Single() Implements IMSEQuotaData.Bbase
        Get
            Return Me.m_data.Bbase
        End Get
        Set(value As Single())
            Me.m_data.Bbase = value
        End Set
    End Property

    Public Property Blim As Single() Implements IMSEQuotaData.Blim
        Get
            Return Me.m_data.Blim
        End Get
        Set(value As Single())
            Me.m_data.Blim = value
        End Set
    End Property

    Public Property Bestimate As Single() Implements IMSEQuotaData.Bestimate
        Get
            Return Me.m_data.Bestimate
        End Get
        Set(value As Single())
            Me.m_data.Bestimate = value
        End Set
    End Property

    Public Property CVbiomEst As Single() Implements IMSEQuotaData.CVbiomEst
        Get
            Return Me.m_data.CVbiomEst
        End Get
        Set(value As Single())
            Me.m_data.CVbiomEst = value
        End Set
    End Property

    Public Property FTarget As Single() Implements IMSEQuotaData.FTarget
        Get
            Return Me.m_data.FTarget
        End Get
        Set(value As Single())
            Me.m_data.FTarget = value
        End Set
    End Property

    Public Property Quotashare As Single(,) Implements IMSEQuotaData.Quotashare
        Get
            Return Me.m_data.Quotashare
        End Get
        Set(value As Single(,))
            Me.m_data.Quotashare = value
        End Set
    End Property

    Public Property QuotaTime As Single(,) Implements IMSEQuotaData.QuotaTime
        Get
            Return Me.m_data.QuotaTime
        End Get
        Set(value As Single(,))
            Me.m_data.QuotaTime = value
        End Set
    End Property

    Public Property CatchYearGroup As Single() Implements IMSEQuotaData.CatchYearGroup
        Get
            Return Me.m_Search.CatchYearGroup
        End Get
        Set(value As Single())
            Me.m_Search.CatchYearGroup = value
        End Set
    End Property

    Public Property BestimateLast As Single() Implements IMSEQuotaData.BestimateLast
        Get
            Return Me.m_data.BestimateLast
        End Get
        Set(value As Single())
            Me.m_data.BestimateLast = value
        End Set
    End Property

    Public Property Fish1 As Single() Implements IMSEQuotaData.Fish1
        Get
            Return Me.m_data.Fish1
        End Get
        Set(value As Single())
            Throw New NotSupportedException("Fish1 is read-only on cMSEDataStructures.")
        End Set
    End Property

    Public Property GstockPred As Single() Implements IMSEQuotaData.GstockPred
        Get
            Return Me.m_data.GstockPred
        End Get
        Set(value As Single())
            Me.m_data.GstockPred = value
        End Set
    End Property

    Public Property RstockRatio As Single() Implements IMSEQuotaData.RstockRatio
        Get
            Return Me.m_data.RstockRatio
        End Get
        Set(value As Single())
            Me.m_data.RstockRatio = value
        End Set
    End Property

    Public Property KalmanGain As Single() Implements IMSEQuotaData.KalmanGain
        Get
            Return Me.m_data.KalmanGain
        End Get
        Set(value As Single())
            Me.m_data.KalmanGain = value
        End Set
    End Property

    Public Property BhalfT As Single() Implements IMSEQuotaData.BhalfT
        Get
            Return Me.m_data.BhalfT
        End Get
        Set(value As Single())
            Me.m_data.BhalfT = value
        End Set
    End Property

    Public Property Rmax As Single() Implements IMSEQuotaData.Rmax
        Get
            Return Me.m_data.Rmax
        End Get
        Set(value As Single())
            Me.m_data.Rmax = value
        End Set
    End Property

    Public Property cvRec As Single() Implements IMSEQuotaData.cvRec
        Get
            Return Me.m_data.cvRec
        End Get
        Set(value As Single())
            Me.m_data.cvRec = value
        End Set
    End Property

    Public Property BioEstStats As IMSESummaryStats Implements IMSEQuotaData.BioEstStats
        Get
            If Me.m_bioEstStatsAdapter Is Nothing OrElse
               Not Object.ReferenceEquals(Me.m_bioEstStatsAdapter.Stats, Me.m_data.BioEstStats) Then
                Me.m_bioEstStatsAdapter = New cSummaryStatsAdapter(Me.m_data.BioEstStats)
            End If
            Return Me.m_bioEstStatsAdapter
        End Get
        Set(value As IMSESummaryStats)
            Throw New NotSupportedException("BioEstStats must be set via cMSEDataStructures.BioEstStats.")
        End Set
    End Property

    Private m_bioEstStatsAdapter As cSummaryStatsAdapter

    ''' <summary>Adapts <see cref="cMSESummaryStats"/> to <see cref="IMSESummaryStats"/> without the stats class implementing the interface.</summary>
    Private NotInheritable Class cSummaryStatsAdapter
        Implements IMSESummaryStats

        Public ReadOnly Stats As cMSESummaryStats

        Public Sub New(stats As cMSESummaryStats)
            Me.Stats = stats
        End Sub

        Public Sub AddValue(index As Integer, TimeIndex As Integer, Value As Single) Implements IMSESummaryStats.AddValue
            Me.Stats.AddValue(index, TimeIndex, Value)
        End Sub

    End Class
End Class
