Option Strict On
Imports EwEUtils.Core

#Region "cCoreEnumNamesIndex"


''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class; creates and maintains quick lookup tables of string 
''' representations of enumerated types defined in the Core.
''' </summary>
''' <remarks>
''' The dotNET mechanism for converting enum values to a string representation is
''' dreadfully slow. This class provides a redundant but bloody fast way to
''' find this string representation by indexing all string representations once.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cCoreEnumNamesIndex

    ''' <summary>Singleton instance</summary>
    Private Shared __inst__ As cCoreEnumNamesIndex = New cCoreEnumNamesIndex()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the one and only instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetInstance() As cCoreEnumNamesIndex
        Return cCoreEnumNamesIndex.__inst__
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Private constructor to enforce singleton.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub New()
        ' Make indexes
        Me.IndexEnum(GetType(eVarNameFlags), Me.m_dictVarEnumToName, Me.m_dictVarNameToEnum)
        Me.IndexEnum(GetType(eDataTypes), Me.m_dictDataTypeEnumToName, Me.m_dictDataTypeNameToEnum)
    End Sub

    ''' <summary>Index of eVarNameFlags enum names, by enum value.</summary>
    Private m_dictVarEnumToName As New Dictionary(Of Integer, String)
    ''' <summary>Index of eVarNameFlags enum values, by name.</summary>
    Private m_dictVarNameToEnum As New Dictionary(Of String, Integer)
    ''' <summary>Index of eDataType enum names, by enum value.</summary>
    Private m_dictDataTypeEnumToName As New Dictionary(Of Integer, String)
    ''' <summary>Index of eDataType enum values, by name.</summary>
    Private m_dictDataTypeNameToEnum As New Dictionary(Of String, Integer)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Make a name index of a given enumerated type.
    ''' </summary>
    ''' <param name="t">The enumerated type to generate the enum name index for.</param>
    ''' <param name="dict1">A dictionary to store the value/name pairs in.</param>
    ''' <param name="dict2">A dictionary to store the name/value pairs in.</param>
    ''' -----------------------------------------------------------------------
    Private Sub IndexEnum(ByVal t As Type, ByRef dict1 As Dictionary(Of Integer, String), ByRef dict2 As Dictionary(Of String, Integer))

        Dim aEnum As Array = System.Enum.GetValues(t)
        Dim strName As String = ""
        Dim iValue As Integer = 0
        ' Iterate through enum
        For i As Integer = aEnum.GetLowerBound(0) To aEnum.GetUpperBound(0)
            ' Acquire and store name for quick lookup
            iValue = CInt(aEnum.GetValue(i))
            strName = CStr(System.Enum.GetName(t, iValue))
            dict1(iValue) = strName
            dict2(strName) = iValue
        Next i

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a eVarNameFlags enum name.
    ''' </summary>
    ''' <param name="e">The <see cref="eVarNameFlags">eVarNameFlags</see> 
    ''' enumerated value to retrieve the name for.</param>
    ''' -----------------------------------------------------------------------
    Public Function GetVarName(ByVal e As eVarNameFlags) As String
        Return Me.m_dictVarEnumToName(e)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a eVarNameFlags enum value.
    ''' </summary>
    ''' <param name="strVarName">The string representation for a variable name.</param>
    ''' -----------------------------------------------------------------------
    Public Function GetVarName(ByVal strVarName As String) As eVarNameFlags
        If Me.m_dictVarNameToEnum.ContainsKey(strVarName) Then
            Return DirectCast(Me.m_dictVarNameToEnum(strVarName), eVarNameFlags)
        Else
            Return eVarNameFlags.NotSet
        End If
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a eDataTypes enum name.
    ''' </summary>
    ''' <param name="e">The <see cref="eDataTypes">eDataTypes</see> 
    ''' enumerated value to retrieve the name for.</param>
    ''' -----------------------------------------------------------------------
    Public Function GetDataTypeName(ByVal e As eDataTypes) As String
        Return Me.m_dictDataTypeEnumToName(e)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a eDataTypes enum value.
    ''' </summary>
    ''' <param name="strDataType">The string representation for a data type.</param>
    ''' -----------------------------------------------------------------------
    Public Function GetDataType(ByVal strDataType As String) As eDataTypes
        If Me.m_dictDataTypeNameToEnum.ContainsKey(strDataType) Then
            Return DirectCast(Me.m_dictDataTypeNameToEnum(strDataType), eDataTypes)
        Else
            Return eDataTypes.NotSet
        End If
    End Function

End Class

#End Region

#Region "Message Type, Importance "

#Region "MessageType"

''' ---------------------------------------------------------------------------
''' <summary>
''' Enumerated type, identifying types of messages being broadcasted by the Core.
''' </summary>
''' <remarks>
''' <para>Used by <see cref="cMessage">cMessage</see> to identify the type of 
''' message being passed out.</para>
''' <para>Used by <see cref="cMessageHandler">cMessageHandler</see> to identify
''' the type of message the handler can handle.</para>
''' </remarks>
''' ---------------------------------------------------------------------------
Public Enum eMessageType
    ''' <summary>Message type has not been set.</summary>
    NotSet = 0
    ''' <summary>This message could be of any message type.</summary>
    ''' <remarks>This flag is used by <see cref="cMessageHandler">cMessageHandler</see>
    ''' as the default message handler.</remarks>
    Any
    ''' <summary>Diet Comp out of range.</summary>
    DietComp
    ''' <summary>Diet Comp correct to 15 percent prompt.</summary>
    DietComp_CorrectTo15Perc
    ''' <summary>EE out of range.</summary>
    EE
    ''' <summary>Parameters could not be computed because of missing data in input parameters.</summary>
    TooManyMissingParameters
    ''' <summary>No Catch for a Fishing Fleet.</summary>
    NoCatchForFleet
    ''' <summary>Error encountered during model run.</summary>
    ErrorEncountered
    ''' <summary>Data validation message.</summary>
    DataValidation
    ''' <summary>Data from the source has been modified.</summary>
    DataModified
    ''' <summary>Data has been added to, or removed from, the source.</summary>
    DataAddedOrRemoved
    ''' <summary>Data import related issue.</summary>
    DataImport
    ''' <summary>Data export related issue.</summary>
    DataExport

    '''' <summary>Time step in Ecospace</summary>
    '''' <remarks>This was added for testing and is not used at this time</remarks>
    'EcospaceTimeStep

    ''' <summary>Ecospace has completed a model run </summary>
    EcospaceRunCompleted

    ''' <summary>Sent by any message source when the State Monitor's state not met to run a method </summary>
    StateNotMet

    Progress

    EcosimRunCompleted

    EcosimNYearsChanged
    MassBalance_InsufficientData
    RespirationExceeedsDetritus
    InvalidModel_PB0_Generic
    InvalidModel_QB0_Generic
    InvalidModel_B_Detritus

    ''' <summary>MSE has completed a model run of some sort </summary>
    MSERunCompleted

End Enum

#End Region

#Region "Message Importance"

''' ---------------------------------------------------------------------------
''' <summary>
''' Flag indicating the relative importance/severity of a <see cref="cMessage">Message</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Enum eMessageImportance
    ''' <summary>Maintenance messages typically indicate a synchronization event
    ''' in the EwE application.</summary>
    Maintenance
    ''' <summary>Information messages typically indicate an event that may be of
    ''' interest to a human user of EwE.</summary>
    Information
    ''' <summary>Warning messages indicating that the system has run in a problem
    ''' and could not complete an operation.</summary>
    Warning
    ''' <summary>Critical messages indicate the the system has run into an error
    ''' that it could not recover from. This is the most severe type of message.</summary>
    Critical
    ''' <summary>Progress messages typically indicate incremental status
    ''' information about a lengthy operation.</summary>
    Progress
End Enum

#End Region

#End Region

#Region "Progress State"

Public Enum eProgressState
    ''' <summary>Process has just started this is the first call</summary>
    Start
    ''' <summary>Process is running </summary>
    Running
    ''' <summary>Process has finished </summary>
    Finished
End Enum

#End Region

#Region "Status Flags"


''' ---------------------------------------------------------------------------
''' <summary>
''' Public enumerator stating the status of a variable used by cVariableStatus class to state the status of the parameter.
''' Used by the data wrapper classes to state the status of a variable see cEcoPathGroupInputs.EEStatus
''' </summary>
''' <remarks>
''' <para>Can be used in combination with eVarNameFlags to tell the <see cref="cVariableStatus.Status">status</see> of a parameter,
''' I.e. cVariableStatus.Status = eStatusFlags.InvalidModelResult and cVariableStatus.VarType = eVarNameFlags.EE:
''' the model computed an invalid result for EE.</para>
''' <para>Mulitple eStatusFlags can be joined together using the bitwise OR operator to signify 
''' multiple statuses for a variable.</para>
''' </remarks>
''' ---------------------------------------------------------------------------
Public Enum eStatusFlags

    ''' <summary>
    ''' All is well.
    ''' </summary>
    OK = 1

    ''' <summary>
    ''' Failed data validation.
    ''' </summary>
    FailedValidation = 2

    ''' <summary>
    ''' Value is computed from other values.
    ''' </summary>
    ValueComputed = 4

    ''' <summary>
    ''' Model computed an invalid result.
    ''' </summary>
    InvalidModelResult = 8

    ''' <summary>
    ''' Value is not editable because other related variables imply their value.
    ''' </summary>
    ''' <remarks>
    ''' This flag is also known as ReadOnly (Windows) or BlockedForInput (EwE5).
    ''' </remarks>
    NotEditable = 16

    ''' <summary>
    ''' Unknown error encountered.
    ''' </summary>
    ''' 
    ErrorEncountered = 32

    ''' <summary>
    ''' Value should have been provided at the start of a model run.
    ''' </summary>
    ''' <remarks>
    ''' This flag resembles <see cref="eStatusFlags.FailedValidation">FailedValidation</see>
    ''' but the reason for the failure is specific to the flag.
    ''' </remarks>
    MissingParameter = 64

    ''' <summary>
    ''' Value should be highlighted as decreed by the core for whatever reason.
    ''' </summary>
    ''' <remarks>
    ''' This can occur when the core determines that particular values have relevant
    ''' links to other values. The core can only know this and can request any GUI
    ''' to hightlight such values.
    ''' </remarks>
    CoreHighlight = 128

    ''' <summary>
    ''' Variable is null, its value has not been set.
    ''' </summary>
    Null = 256

End Enum

#End Region

#Region "Forcing function Pred Prey Interation"

''' ---------------------------------------------------------------------------
''' <summary>
''' Enumerator for forcing functions, describing to which Predator/Prey
''' interaction a forcing function is applied.
''' </summary>
''' ---------------------------------------------------------------------------
Public Enum eForcingFunctionApplication
    ProductionRate = 1
    SearchRate = 1
    Vulnerability = 2
    ArenaArea = 3
    VulAndArea = 4
End Enum

#End Region

#Region "Ecopath Parameter Estimation type"

''' ---------------------------------------------------------------------------
''' <summary>
''' Enumerated type that indicates for which purpose Ecopath parameters are being estimated.
''' </summary>
''' ---------------------------------------------------------------------------
Public Enum eEstimateParameterFor
    ''' <summary>
    ''' Indicates that parameters are being estimated for the 
    ''' main parameter estimation routine.
    ''' </summary>
    ParameterEstimation

    ''' <summary>
    ''' Indicates that parameters are being estimated for the 
    ''' sensitivity loop.
    ''' </summary>
    Sensitivity
End Enum

#End Region

#Region "Operators for cOperatorBase"

''' ---------------------------------------------------------------------------
''' <summary>
''' Enumerated type indicating logical operators.
''' </summary>
''' ---------------------------------------------------------------------------
Public Enum eOperators
    ''' <summary>
    ''' Logical 'less than' operator.
    ''' </summary>
    LessThan

    ''' <summary>
    ''' Logical 'less than or equal to' operator.
    ''' </summary>
    LessThanOrEqualTo

    ''' <summary>
    ''' Logical 'greater than' operator.
    ''' </summary>
    GreaterThan

    ''' <summary>
    ''' Logical 'greater than or equal to' operator.
    ''' </summary>
    GreaterThanOrEqualTo

    ''' <summary>
    ''' Logical 'equal to' operator.
    ''' </summary>
    EqualTo
End Enum

#End Region

#Region "Primary Production Types"

''' ---------------------------------------------------------------------------
''' <summary>
''' Enumerated type specifying Group Primary Production types
''' </summary>
''' ---------------------------------------------------------------------------
Public Enum ePrimaryProductionTypes
    Consumer = 0
    Producer = 1
    Detritus = 2
End Enum

#End Region

#Region "Cost Index"

''' ---------------------------------------------------------------------------
''' <summary>
''' Enumerator for CostPct(nFleets, 3) array, 
''' i.e. fleet.FixedCost = CostPct(1, eCostIndex.Fixed) is the fixed cost for 
''' variable 'fleet' at index 1.
''' </summary>
''' ---------------------------------------------------------------------------
Friend Enum eCostIndex
    Profit = 0
    Fixed = 1
    CUPE = 2
    Sail = 3
End Enum

#End Region

#Region "Ecospace results index"

''' ---------------------------------------------------------------------------
''' <summary>
''' Index of results from Ecospace saved over time by group
''' </summary>
''' <remarks>This data will be exposed by the core so it needs to know the index that the data is stored in</remarks>
''' ---------------------------------------------------------------------------
Friend Enum eSpaceResultsGroups
    Biomass
    RelativeBiomass
    CatchBio
End Enum

Friend Enum eSpaceResultsFleets
    SailingEffort
    FishingEffort
    CatchBio
    Value
End Enum


Friend Enum eSpaceResultsFleetsGroups
    CatchBio
    Value
End Enum

#End Region

#Region " Time series types "

''' ---------------------------------------------------------------------------
''' <summary>
''' Types of time series
''' </summary>
''' <remarks>The enumerated values follow the original EwE5 scheme.</remarks>
''' ---------------------------------------------------------------------------
Public Enum eTimeSeriesType As Integer
    BiomassRel = 0
    BiomassAbs = 1
    BiomassForcing = -1
    TimeForcing = 2
    FishingEffort = 3
    FishingMortality = 4
    TotalMortality = 5
    ConstantTotalMortality = -5
    Catches = 6
    CatchesForcing = -6
    AverageWeight = 7
    EcotracerConcRel = 8
    EcotracerConcAbs = 9
    FishingMortalityRef = 104
    NotSet = cCore.NULL_VALUE
End Enum

''' -----------------------------------------------------------------------
''' <summary>
''' Enumerated type, defining aliases for <see cref="eTimeSeriesType">time series types</see>.
''' </summary>
''' -----------------------------------------------------------------------
Public Enum eTimeSeriesAliases As Integer
    BRel = 0
    BAbs = 1
    BForced = -1
    Forcing = 2
    Effort = 3
    Z = 4
    F = 5
    FConst = -5
    C = 6
    [Catch] = 6
    CForced = -6
    WAvg = 7
    ConcRel = 8
    ConcAbs = 9
End Enum
#End Region ' Time series types

#Region " PSD mortality types "

''' -----------------------------------------------------------------------
''' <summary>
''' Mortality types for PSD analysis
''' </summary>
''' -----------------------------------------------------------------------
Public Enum ePSDMortalityTypes As Integer
    ''' <summary>Group P/B</summary>
    GroupZ = 0
    ''' <summary>Lorenzen-variable</summary>
    Lorenzen = 1
End Enum

#End Region ' PSD mortality types

#Region " PSD climate types "

''' -----------------------------------------------------------------------
''' <summary>
''' The three climate zones for PSD analysis.
''' </summary>
''' -----------------------------------------------------------------------
Public Enum eClimateTypes As Integer
    ''' <summary>Tropical climate</summary>
    Tropical = 0
    ''' <summary>Temperate climate</summary>
    Temperate = 1
    ''' <summary>Polar climate</summary>
    Polar = 2
End Enum

#End Region ' PSD climate types

