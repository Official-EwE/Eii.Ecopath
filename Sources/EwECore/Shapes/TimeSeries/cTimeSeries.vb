'==============================================================================
'
' $Log: cTimeSeries.vb,v $
' Revision 1.1  2008/09/26 07:30:34  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.22  2008/09/24 00:53:40  jeroens
' Fixed XML comment errors
'
' Revision 1.21  2008/09/23 16:24:20  jeroens
' TS 'Apply' -> 'Enable'
'
' Revision 1.20  2008/06/06 15:56:08  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.19  2008/02/11 03:28:15  jeroens
' Removed Dataset name, first year, numyears
'
' Revision 1.18  2008/01/21 04:06:38  jeroens
' Fixed shape max scale issues, once and for all
'
' Revision 1.17  2007/11/06 13:22:47  jeroens
' * Default weight = 1
'
' Revision 1.16  2007/10/12 20:35:40  jeroens
' - Removed physical link to dataset
'
' Revision 1.15  2007/10/02 18:20:29  jeroens
' * Fixed major TS mis-allocation category issue
'
' Revision 1.14  2007/09/10 15:30:49  jeroens
' + Added CanApply
'
' Revision 1.13  2007/08/30 02:25:05  jeroens
' + Added link to TS dataset
'
' Revision 1.12  2007/08/28 03:53:47  jeroens
' * Fixed bug in FleetTimeSeries initialization
'
' Revision 1.11  2007/08/08 20:47:48  joeh
' Change the scope of GroupIndex and FleetIndex from Friend Set to Set
'
' Revision 1.10  2007/08/08 00:54:03  jeroens
' * Factory.CreateTimeSeries made public
'
' Revision 1.9  2007/07/30 01:50:18  jeroens
' * Limited TS type changes
'
' Revision 1.8  2007/07/27 19:45:19  joeh
' Change Time Series Type property to public
'
' Revision 1.7  2007/07/20 18:17:25  jeroens
' - cTimeSeriesImport is not allowed to report back to the core any longer
'
' Revision 1.6  2007/07/17 16:21:25  jeroens
' + Implemented cTimeSeries.Update()
'
' Revision 1.5  2007/07/17 02:15:38  jeroens
' * cTimeSeries now inherited from cShapeData
'
' Revision 1.4  2007/07/13 17:24:33  jeroens
' - Removed Forcing namespace
'
' Revision 1.3  2007/07/12 16:29:24  jeroens
' + Moved
'
' Revision 1.1  2007/07/12 15:50:02  jeroens
' * Moved
'
' Revision 1.9  2007/06/11 02:56:06  jeroens
' * cTimeSeries inherits cCoreInputOutputBase
' * cTimeSeriesImport serves as import data
' - Discontinued cForcingTimeSeries
'
' Revision 1.8  2007/06/08 21:09:25  joeb
' Added DataSS and DataQ
'
' Revision 1.7  2007/06/07 16:09:42  jeroens
' * Fixed enum misinterpretation bug
'
' Revision 1.6  2007/06/07 12:44:48  jeroens
' + Added Factory to simplify interpretation of awkward eTimeSeriesType values
'
' Revision 1.5  2007/06/07 11:55:55  jeroens
' * TS type -6 is also Fleet-related
'
' Revision 1.4  2007/05/23 16:36:50  jeroens
' + Added Apply()
'
' Revision 1.3  2007/05/18 03:21:44  jeroens
' * Commented
'
' Revision 1.2  2007/05/16 17:11:40  jeroens
' + Completed base class, added subclasses
'
' Revision 1.1  2007/05/14 03:15:03  jeroens
' Initial version
'
' Revision 1.1  2007/05/10 03:18:58  jeroens
' + Drafting up initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports directive

''' ---------------------------------------------------------------------------
''' <summary>
''' The one access point in EwE to create <see cref="cTimeSeries">cTimeSeries-related objects</see>,
''' and to interpret <see cref="eTimeSeriesType">eTimeSeriesType values</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTimeSeriesFactory

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumeraterd type stating whether a time series is 
    ''' <see cref="cGroupTimeSeries">group-related</see>,  
    ''' <see cref="cFleetTimeSeries">fleet-related</see> or is a
    ''' <see cref="cForcingFunction">forcing function</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eTimeSeriesCategoryType
        ''' <summary>Unknown time series category.</summary>
        NotSet = 0
        ''' <summary>Group-related time series category.</summary>
        Group
        ''' <summary>Fleet-related time series category.</summary>
        Fleet
        ''' <summary>Forcing function time series category.</summary>
        Forcing
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Determine the <see cref="eTimeSeriesCategoryType">Time series category</see>
    ''' based on a <see cref="eTimeSeriesType">Time series type</see>. For instance,
    ''' time series types <see cref="eTimeSeriesType.Catches">eTimeSeriesType.Catches</see>
    ''' and <see cref="eTimeSeriesType.CatchesForcing">eTimeSeriesType.CatchesForcing</see>
    ''' are <see cref="eTimeSeriesCategoryType.Fleet">Fleet</see>-related time series.
    ''' </summary>
    ''' <param name="timeSeriesType"></param>
    ''' <remarks>
    ''' This method was added to centralize interpretation of the awkward enumerator 
    ''' <see cref="eTimeSeriesType">eTimeSeriesType</see>.
    ''' </remarks>
    ''' <returns>
    ''' A time series category for the provided time series type.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function TimeSeriesCategory(ByVal timeSeriesType As eTimeSeriesType) As eTimeSeriesCategoryType

        Select Case timeSeriesType

            Case eTimeSeriesType.NotSet
                Return eTimeSeriesCategoryType.NotSet

            Case eTimeSeriesType.TimeForcing
                Return eTimeSeriesCategoryType.Forcing

            Case eTimeSeriesType.FishingEffort
                Return eTimeSeriesCategoryType.Fleet

            Case Else
                Return eTimeSeriesCategoryType.Group

        End Select

        ' Add this for good manners.
        Return eTimeSeriesCategoryType.NotSet
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Factory method; the only location in EwE where actual <see cref="cTimeSeries">cTimeSeries-derived</see>
    ''' objects are created.
    ''' </summary>
    ''' <param name="timeSeriesType">The <see cref="eTimeSeriesType">type</see> of
    ''' the time series.</param>
    ''' <returns>A Time Series instance, or nothing if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function CreateTimeSeries(ByVal timeSeriesType As eTimeSeriesType, _
            ByVal core As cCore, ByVal iDBID As Integer) As cTimeSeries

        Dim ts As cTimeSeries = Nothing

        Select Case TimeSeriesCategory(timeSeriesType)

            Case eTimeSeriesCategoryType.Forcing
                ts = Nothing ' No can do

            Case eTimeSeriesCategoryType.Fleet
                ts = New cFleetTimeSeries(core, iDBID)

            Case eTimeSeriesCategoryType.Group
                ts = New cGroupTimeSeries(core, iDBID)

            Case eTimeSeriesCategoryType.NotSet
                Debug.Assert(False, String.Format("Unknown category of time series for type {0}", timeSeriesType))

        End Select

        Return ts
    End Function

End Class

#Region " cTimeSeries (base class) "

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
    ''' <summary>The core this TS belongs to.</summary>
    Protected m_core As cCore = Nothing

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
            Dim tscatCurr As cTimeSeriesFactory.eTimeSeriesCategoryType = cTimeSeriesFactory.TimeSeriesCategory(Me.m_timeSeriesType)
            Select Case tscatCurr
                Case cTimeSeriesFactory.eTimeSeriesCategoryType.NotSet
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
    ''' <remarks>
    ''' <para>This property is trying to fly under the radar since DatPool should be
    ''' accessed via <see cref="cGroupTimeSeries.GroupIndex">cGroupTimeSeries.GroupIndex</see>
    ''' or <see cref="cFleetTimeSeries.FleetIndex">cFleetTimeSeries.FleetIndex</see>,
    ''' depending on the <see cref="TimeSeriesType">type</see> of the time series.</para>
    ''' <para>Naturally, DatPool is freely accessible via GetVariable/SetVariable :(</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Friend Property DatPool() As Integer
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
            Return Me.m_bEnabled
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
    ''' Helper method, call this to inform the EwE core that a Time Series has changed.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
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
    ''' States whether this TS can be applied, e.g. is has all required data to
    ''' be applied.
    ''' </summary>
    ''' <returns>True if valid.</returns>
    ''' -----------------------------------------------------------------------
    Public Overridable Function CanEnable() As Boolean
        Return (Me.m_iDatPool > 0)
    End Function

End Class

#End Region ' cTimeSeries (base class)

#Region " cGroupTimeSeries "

''' -----------------------------------------------------------------------
''' <summary>
''' Data for one time series contained in an Ecosim scenario.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cGroupTimeSeries
    Inherits cTimeSeries

#Region " Protected variables "

    ''' <summary>The custom variable name this time series applies to.</summary>
    Protected m_strCustomVariableName As String = ""

#End Region ' Protected variables

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByVal core As cCore, ByVal iDBID As Integer)
        MyBase.New(core, iDBID)
        Me.m_datatype = eDataTypes.GroupTimeSeries
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the Group this time series applies to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GroupIndex() As Integer
        Get
            Return Me.DatPool
        End Get

        Set(ByVal iGroup As Integer)
            Me.DatPool = iGroup
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the custom variable name this time series applies to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CustomVariableName() As String
        Get
            Return Me.m_strCustomVariableName
        End Get

        Set(ByVal strCustomVariableName As String)
            Me.m_strCustomVariableName = strCustomVariableName
        End Set
    End Property

End Class

#End Region ' cGroupTimeSeries

#Region " cFleetTimeSeries "

''' -----------------------------------------------------------------------
''' <summary>
''' Data for one time series contained in an Ecosim scenario.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cFleetTimeSeries
    Inherits cTimeSeries

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByVal core As cCore, ByVal iDBID As Integer)
        MyBase.New(core, iDBID)
        Me.m_datatype = eDataTypes.FleetTimeSeries
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the fleet this time series applies to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property FleetIndex() As Integer
        Get
            Return Me.DatPool
        End Get
        Set(ByVal iFleet As Integer)
            Me.DatPool = iFleet
        End Set
    End Property

End Class

#End Region ' cFleetTimeSeries

#Region " cTimeSeriesImport "

''' ---------------------------------------------------------------------------
''' <summary>
''' TimeSeries import class
''' </summary>
''' <remarks>
''' This reminds me so much about programming COBOL that I'm downright terrified...
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cTimeSeriesImport
    Inherits cGroupTimeSeries

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="iNumYears">Number of years in this time series.</param>
    ''' <param name="timeSeriesType"><see cref="eTimeSeriesType">Type</see> of this time series.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByVal iNumYears As Integer, ByVal timeSeriesType As eTimeSeriesType)
        MyBase.New(Nothing, -1)
        Me.m_strCustomVariableName = ""
        Me.m_timeSeriesType = timeSeriesType
        Me.ResizeData(iNumYears)
    End Sub

    Public Overrides Function Update() As Boolean
        ' Suppress this
        Return True
    End Function

End Class

#End Region ' cTimeSeriesImport
