
''' <summary>
''' Interface for defining Ecospace Environmental Input maps
''' </summary>
''' <remarks></remarks>
Public Interface IEnviroInputMap

    ''' <summary>
    ''' Name of the underlying Map
    ''' </summary>
    Property Name() As String

    ''' <summary>
    ''' Return the value of the map as a function of the applied Response Function
    ''' </summary>
    ''' <param name="igrp">Index of the Group that this Response is for</param>
    ''' <param name="iRow">Row of the map</param>
    ''' <param name="iCol">Column of the map</param>
    Function ResponseFunction(ByVal igrp As Integer, ByVal iRow As Integer, ByVal iCol As Integer) As Single

    ''' <summary>
    ''' Initialize the map with the cMediationDataStructures containing all the available response functions and cEcospaceDataStructures
    ''' </summary>
    ''' <param name="MediationData">cMediationDataStructures that contains the Response Function (mediation functions) that can be used by this Map</param>
    ''' <param name="SpaceData"></param>
    Function Init(ByVal MediationData As cMediationDataStructures, ByVal SpaceData As cEcospaceDataStructures) As Boolean

    ''' <summary>
    ''' Get or Set the index of the Response function applied to a Group
    ''' </summary>
    ''' <param name="GroupIndex">Index of the Group that the response function is applied to</param>
    ''' <value></value>
    ''' <returns>Index of a response function.</returns>
    ''' <remarks>
    ''' <code>
    ''' dim ResponseIndex as integer
    ''' dim GroupIndex as integer
    ''' GroupIndex = 1
    ''' 'Set the Response function index for GroupIndex
    '''  IEnviroInputMap.ResponseIndexForGroup(GroupIndex) = 2
    ''' 'Get the Response functon index for GroupIndex
    ''' ResponseIndex = IEnviroInputMap.ResponseIndexForGroup(GroupIndex) 
    ''' </code>
    ''' </remarks>
    Property ResponseIndexForGroup(ByVal GroupIndex As Integer) As Integer

    ''' <summary>
    ''' Max value of the map
    ''' </summary>
    ReadOnly Property Max() As Single

    ''' <summary>
    ''' Minimum value of the map
    ''' </summary>
    ReadOnly Property Min() As Single

    ''' <summary>
    ''' Mean value of the map
    ''' </summary>
    ReadOnly Property Mean() As Single

    ''' <summary>
    ''' Histogram of the map values
    ''' </summary>
    ''' <remarks>
    ''' Values in the Histogram will be normalized.
    ''' Re-computed on each call to Histogram.
    ''' </remarks>
    Function Histogram() As Drawing.PointF()

    ''' <summary>
    ''' Updates the map stats on the underlying data
    ''' </summary>
    ''' <remarks>Caluculates Min, Max and Mean</remarks>
    Function Update() As Boolean

    ''' <summary>
    ''' Set the cMapResponseInteractionManager that this map uses
    ''' </summary>
    ''' <param name="theManager"></param>
    Sub setManager(ByVal theManager As cMapResponseInteractionManager)

End Interface
