#Region " Imports "

Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Data for one time series contained in an Ecosim scenario.
''' </summary>
''' <remarks>
''' This class is implemented as a <see cref="cShapeData">cShapeData</see>.
''' </remarks>
''' ---------------------------------------------------------------------------
Public MustInherit Class cTimeSeries
    Inherits cShapeData

#Region " Protected variables "

    ''' <summary>The <see cref="eTimeSeriesType">type</see> of this time series.</summary>
    Protected m_timeSeriesType As eTimeSeriesType = eTimeSeriesType.NotSet
    ''' <summary>The weight of time for this time series.</summary>
    Protected m_sWtType As Single = 1.0!
    ''' <summary>The index of the target that this time series applies to.</summary>
    Protected m_iDatPool As Integer = 0
    ''' <summary>Applied flag</summary>
    Protected m_bEnabled As Boolean = False
    ''' <summary>Sum of squares for this TS.</summary>
    Protected m_sDatSS As Single = 0.0!
    ''' <summary>Average zstat sumof(Log(observed/predicted))/nobs.</summary>
    Protected m_sDataQ As Single = 0.0!

    ''' <summary>exp(DataQ)</summary>
    Protected m_eDataQ As Single

    ''' <summary>The core this TS belongs to.</summary>
    Protected m_core As cCore = Nothing
    ''' <summary>Time series validity flag.</summary>
    Private m_bIsValid As Boolean = True

#End Region ' Protected variables

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByRef core As cCore, ByVal DBID As Integer)
        MyBase.New(0)

        Me.m_core = core
        Me.m_datatype = eDataTypes.NotSet
        Me.DBID = DBID

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eTimeSeriesType">type</see> of this time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property TimeSeriesType() As eTimeSeriesType
        Get
            Return Me.m_timeSeriesType
        End Get

        Set(ByVal tstype As eTimeSeriesType)
            ' It is not allowed to switch between group- and fleet based TS once a type has been assigned
            Dim tscatCurr As eTimeSeriesCategoryType = cTimeSeriesFactory.TimeSeriesCategory(Me.m_timeSeriesType)
            Select Case tscatCurr
                Case eTimeSeriesCategoryType.NotSet
                    Me.m_timeSeriesType = tstype
                Case Else
                    If cTimeSeriesFactory.TimeSeriesCategory(tstype) = tscatCurr Then
                        Me.m_timeSeriesType = tstype
                    Else
                        Debug.Assert(False, "Illegal assignment; a TS cannot switch categories")
                    End If
            End Select
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the weight of time for this time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property WtType() As Single
        Get
            Return Me.m_sWtType
        End Get

        Set(ByVal sWtType As Single)
            Me.m_sWtType = sWtType
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the target that this time series applies to. The
    ''' type of the target is implied by the <see cref="TimeSeriesType">type</see>
    ''' of the time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DatPool() As Integer
        Get
            Return Me.m_iDatPool
        End Get

        Set(ByVal iDatPool As Integer)
            Me.m_iDatPool = iDatPool
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the annual values for this time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DatVal(ByVal iIndex As Integer) As Single
        Get
            Return CSng(Me.ShapeData(iIndex))
        End Get

        Set(ByVal sValue As Single)
            Me.ShapeData(iIndex) = sValue
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the apply flag on the Time Series. Call <see cref="cCore.UpdateTimeSeries">cCore.UpdateTimeSeries</see>
    ''' to enable all flagged time series to the Ecosim model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Enabled() As Boolean
        Get
            Return (Me.m_bEnabled) And Me.CanEnable
        End Get

        Set(ByVal bEnable As Boolean)
            Me.m_bEnabled = bEnable
        End Set
    End Property


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Sum of squares for the fit of this data set to the predicted value DatSS
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DataSS() As Single
        Get
            Return Me.m_sDatSS
        End Get

        Friend Set(ByVal sValue As Single)
            Me.m_sDatSS = sValue
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' average  zstat sumof(Log(observed/predicted))/nobs Datq
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DataQ() As Single
        Get
            Return Me.m_sDataQ
        End Get

        Friend Set(ByVal sDataQ As Single)
            Me.m_sDataQ = sDataQ
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' exp(DataQ) average prediction error
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property eDataQ() As Single
        Get
            Return Me.m_eDataQ
        End Get

        Friend Set(ByVal eDataQ As Single)
            Me.m_eDataQ = eDataQ
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, call this to inform the EwE core that a Time Series has changed.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Update() As Boolean
        Try
            Me.m_core.onChanged(Me, eMessageType.DataModified)
            Return True
        Catch ex As Exception
            Debug.Assert(False, String.Format("Failed to update time series {0}", Me.Name))
            Return False
        End Try
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether this TS can be applied, e.g. is has all required data to
    ''' be applied.
    ''' </summary>
    ''' <returns>True if a TS can be enabled.</returns>
    ''' -----------------------------------------------------------------------
    Public Property CanEnable() As Boolean
        Get
            Return (Me.m_iDatPool > 0) And Me.m_bIsValid
        End Get
        Friend Set(ByVal value As Boolean)
            Me.m_bIsValid = value
        End Set
    End Property

End Class
