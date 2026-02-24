' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Interface for defining Ecospace Environmental Input maps
''' </summary>
''' <remarks></remarks>
Public Interface IEnviroInputData

    ''' <summary>
    ''' Return the driver value as a function of the applied Response Function
    ''' </summary>
    ''' <param name="iGroup">Index of the Group that this Response is for</param>
    ''' <param name="iRow">Row of the map</param>
    ''' <param name="iCol">Column of the map</param>
    Function ResponseFunction(iGroup As Integer, iRow As Integer, iCol As Integer) As Single

    ''' <summary>
    ''' Initialize from the cMediationDataStructures containing all the available response functions and cEcospaceDataStructures
    ''' </summary>
    ''' <param name="MediationData">cMediationDataStructures that contains the Response Function (mediation functions) that can be used by this Map</param>
    ''' <param name="SpaceData"></param>
    Function Init(MediationData As cMediationDataStructures, SpaceData As cEcospaceDataStructures) As Boolean

    ''' <summary>
    ''' Get / set the index of the Response function applied to a Group
    ''' </summary>
    ''' <param name="iGroup">One-based index of the Group that the response function is applied to</param>
    ''' <param name="bUpdateMaps">Optional flag to suppress (possibly expensive) map updates</param>
    ''' <value></value>
    ''' <returns>Index of a response function.</returns>
    ''' <remarks>
    ''' <code>
    ''' dim ResponseIndex as integer
    ''' dim iGroup as integer
    ''' iGroup = 1
    ''' 'Set the Response function index for iGroup
    '''  IEnviroInputMap.ResponseIndexForGroup(iGroup) = 2
    ''' 'Get the Response functon index for iGroup
    ''' ResponseIndex = IEnviroInputMap.ResponseIndexForGroup(iGroup) 
    ''' </code>
    ''' </remarks>
    Property ResponseIndexForGroup(iGroup As Integer, Optional bUpdateMaps As Boolean = True) As Integer

    ''' <summary>
    ''' Response function for Ecosim
    ''' </summary>
    ''' <param name="iGroup"></param>
    ''' <param name="iTimeStep"></param>
    ''' <returns></returns>
    Function ResponseFunction(iGroup As Integer, iTimeStep As Integer) As Single

    ''' <summary>
    ''' Initialize from cMediationDataStructures containing all the available response functions and cEcospaceDataStructures
    ''' </summary>
    ''' <param name="MediationData">cMediationDataStructures that contains the Response Function (mediation functions) that can be used by this environmental driver</param>
    ''' <param name="EcosimData"></param>
    Function Init(MediationData As cMediationDataStructures, EcosimData As cEcosimDatastructures) As Boolean

    ''' <summary>
    ''' Max value of the driver
    ''' </summary>
    ReadOnly Property Max() As Single

    ''' <summary>
    ''' Minimum value of the driver
    ''' </summary>
    ReadOnly Property Min() As Single

    ''' <summary>
    ''' Mean value of the driver
    ''' </summary>
    ReadOnly Property Mean() As Single

    ''' <summary>
    ''' Simulation start value of the driver
    ''' </summary>
    ReadOnly Property Start() As Single

    ''' <summary>
    ''' Histogram of driver values
    ''' </summary>
    ''' <remarks>
    ''' Values in the Histogram will be normalized.
    ''' Re-computed on each call to Histogram.
    ''' </remarks>
    Function Histogram() As Drawing.PointF()

    ''' <summary>
    ''' Width of a histogram bin
    ''' </summary>
    ReadOnly Property HistogramBinWidth() As Single

    ''' <summary>
    ''' Updates the map stats on the underlying data
    ''' </summary>
    ''' <remarks>Caluculates Min, Max and Mean</remarks>
    Function Update() As Boolean

    ''' <summary>
    ''' Set the cMapResponseInteractionManager that this map uses
    ''' </summary>
    ''' <param name="theManager"></param>
    Sub SetManager(theManager As IEnvironmentalResponseManager)

    Property IsCapacityEnabled As Boolean

    ReadOnly Property Name As String

End Interface
