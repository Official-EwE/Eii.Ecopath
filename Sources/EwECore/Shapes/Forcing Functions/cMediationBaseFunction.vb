' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

'''<summary>
''' Base Class for a mediation function. 
''' A mediation function "Is A" Forcing function that that contains Ecopath base values along it's X axis, Biomass, Effort or Catch
''' depending on the implementation. The Y axis contains the value of the underlying shape entered by the user. 
''' The value of the Y axis is used to mediate/modify a Group biomass (for a Mediation function) or Group/Fleet price (for a Price Elasticity function) 
''' based on how the mediaton function is applied via the cPredPreyInteraction or cLandingsInteraction that "contain" the mediation function. 
''' </summary>
Public MustInherit Class cMediationBaseFunction
    Inherits cForcingFunction

    Protected m_iMedXBase As Integer
    Protected m_medData As cMediationDataStructures
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cMediationBaseFunction)()

#Region " Constructors "

    Friend Sub New(EcoSimData As cEcosimDatastructures, Manager As cBaseShapeManager, _
                   data As cMediationDataStructures, DBID As Integer, DataType As eDataTypes)
        'mediation data arrays from EcoSim
        'Public MedWeights(nGroups + nGear, MediationShapes) As Single 'defines biomass weights for med X
        'Public NMedXused() As Integer 'number of biomasses (mediation weights) in an iMediation
        'Public IMedUsed(,) As Integer 'groups used in med function X IMedUsed(nGroups + nGear, MediationShapes)
        'Public MedXbase() As Single 'ecopath base value of med function X
        'Public MedYbase() As Single 'value of med function at ecopath base X
        'Public MedIsUsed() As Boolean 'true if med function iMediation is used
        MyBase.New(EcoSimData, Manager, DBID, DataType)

        Try

            'Me.m_datatype = DataType
            Me.m_coreComponent = eCoreComponentType.Ecosim
            Me.m_medData = data
            Me.m_timeresolution = eTSDataSetInterval.TimeStep

            Me.m_bInInit = True
            Me.m_data = EcoSimData
            Me.m_iDBID = DBID
            Me.m_iEcoSimIndex = Array.IndexOf(Me.m_medData.MediationDBIDs, Me.m_iDBID)

            Dim iShape As Integer = Me.m_iEcoSimIndex 'just for clarity

            Me.m_manager = Manager 'keep a reference to the manager for this shape

            Me.m_bInInit = False
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".New() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".New() Error: " & ex.Message, ex)
        End Try

    End Sub

    ''' <summary>
    ''' Initialize the propeties from the underlying EcoSim data structures at the existing array index (iEcoSimIndex)
    ''' </summary>
    ''' <returns>True if successful</returns>
    ''' <remarks>This seperates creation from initialization so that an existing object can be repopluated from its underlying data</remarks>
    Protected Friend Overrides Function Load() As Boolean

        'copy the data from zscale into an array that will be used to create a forcing data object
        Me.m_bInInit = True
        Me.LockUpdates()

        Me.m_iEcoSimIndex = Array.IndexOf(Me.m_medData.MediationDBIDs, Me.m_iDBID)
        Debug.Assert(Me.m_iEcoSimIndex > -1, "mediation shape database ID invalid.")
        If Me.m_iEcoSimIndex < 0 Then Return False

        Me.ResizeData(Me.m_medData.NMedPoints)
        For ipt As Integer = 1 To Me.m_medData.NMedPoints
            Me.ShapeData(ipt) = Me.m_medData.Medpoints(ipt, Me.m_iEcoSimIndex)
        Next ipt

        Me.m_nYears = Me.m_data.NumYears
        Me.Name = Me.m_medData.MediationTitles(Me.m_iEcoSimIndex)

        'shape parameters
        Me.m_ShapeFunctionType = Me.m_medData.MediationShapeParams(Me.m_iEcoSimIndex).ShapeFunctionType
        Me.m_params = CType(Me.m_medData.MediationShapeParams(Me.m_iEcoSimIndex).ShapeFunctionParams.Clone(), Single())

        Me.UnlockUpdates()
        Me.m_bInInit = False
        Return True

    End Function

#End Region ' Constructors

#Region " Must override properties "

    ''' <summary>Returns the number of <see cref="cMediatingGroup">mediating groups</see> attached to this function.</summary>
    Public MustOverride ReadOnly Property NumGroups() As Integer
    ''' <summary>Retrieve a <see cref="cMediatingGroup">mediating group</see>.</summary>
    ''' <param name="iIndex">Zero-based index [0, <see cref="NumGroups"/>-1] of the mediating group to retrieve.</param>
    Public MustOverride Property Group(iIndex As Integer) As cMediatingGroup
    ''' <summary>Add a <see cref="cMediatingGroup">mediating group</see> to this function.</summary>
    ''' <param name="iGroup">The <see cref="cEcoPathGroupInput.Index">ecopath index</see> of the group to add.</param>
    ''' <param name="weight">The weight of the mediating fleet.</param>
    Public MustOverride Function AddGroup(iGroup As Integer, weight As Single, Optional iFleetIndex As Integer = cCore.NULL_VALUE) As Boolean

    ''' <summary>Returns the number of <see cref="cMediatingFleet">mediating fleets</see> attached to this function.</summary>
    Public MustOverride ReadOnly Property NumFleet() As Integer
    ''' <summary>Retrieve a <see cref="cMediatingFleet">mediating fleet</see>.</summary>
    ''' <param name="iIndex">Zero-based index [0, <see cref="NumFleet"/>-1] of the mediating fleet to retrieve.</param>
    Public MustOverride Property Fleet(iIndex As Integer) As cMediatingFleet
    ''' <summary>Add a <see cref="cMediatingFleet">mediating fleet</see> to this function.</summary>
    ''' <param name="iFleet">The <see cref="cFleetInput.Index">ecopath index</see> of the fleet to add.</param>
    ''' <param name="weight">The weight of the mediating fleet.</param>
    Public MustOverride Function AddFleet(iFleet As Integer, weight As Single) As Boolean

#End Region

#Region "Properties shared by all implementations"

    ''' <summary>
    ''' X Axis base index for biomass
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is the vertical green line in the EwE 5 mediation interface</remarks>
    Public Property XBaseIndex() As Integer
        Get
            Try
                Return Me.m_medData.IMedBase(Array.IndexOf(Me.m_medData.MediationDBIDs, Me.m_iDBID))
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                m_logger.LogError(ex, ".XBaseIndex Get() Error: " & ex.Message)
            End Try

        End Get
        Set(value As Integer)
            Try
                If (value <= 0) Or (value >= Me.m_medData.NMedPoints) Then Return
                Me.m_medData.IMedBase(Array.IndexOf(Me.m_medData.MediationDBIDs, Me.m_iDBID)) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                m_logger.LogError(ex, ".XBaseIndex Set() Error: " & ex.Message)
            End Try
        End Set
    End Property


    ''' <summary>
    ''' X Axis base value for sum of x biomass
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is the vertical green line in the EwE 5 mediation interface</remarks>
    Public ReadOnly Property XBase() As Single
        Get
            Try
                Return Me.m_medData.MedXbase(Array.IndexOf(Me.m_medData.MediationDBIDs, Me.m_iDBID))
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                m_logger.LogError(ex, ".XBase Get() Error: " & ex.Message)
            End Try

        End Get

    End Property

#End Region

#Region " Updating of shared properties"

    ''' <summary>
    ''' Update the underlying EcoSim data structures
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update() As Boolean

        'do not update during initialization
        If Me.m_bInInit Then
            Return False
        End If

        Me.m_iEcoSimIndex = Array.IndexOf(Me.m_medData.MediationDBIDs, Me.m_iDBID)
        'can not update if there is not an index to the underlying data structures
        If (Me.m_iEcoSimIndex = cCore.NULL_VALUE) Or (Me.m_iEcoSimIndex > Me.m_medData.MediationShapes) Then
            m_logger.LogError(".update(m_data) index out of bounds. Data not updated.")
            Return False
        End If

        'make sure the shape data is the same size as the EcoSim Shape data
        'this is a double check as the data size was check when the forcing function was added to the Shape Manager
        'however it could have been changed be an interface at a later date
        Me.ResizeData(Me.m_medData.NMedPoints)

        'populate the raw shape data
        For ipt As Integer = 1 To Me.nPoints
            Me.m_medData.Medpoints(ipt, Me.m_iEcoSimIndex) = Me.ShapeData(ipt)
        Next ipt

        Me.m_medData.MediationTitles(Me.m_iEcoSimIndex) = Me.Name

        ' Forcing application type not applicable to mediation functions
        'm_data.ForcingApplicationType(m_iEcoSimIndex) = Me.m_ForcingApplicationType

        'shape parameters
        Me.m_medData.MediationShapeParams(Me.m_iEcoSimIndex).ShapeFunctionType = Me.m_ShapeFunctionType
        Me.m_medData.MediationShapeParams(Me.m_iEcoSimIndex).ShapeFunctionParams = CType(Me.m_params.Clone(), Single())

        Return True

    End Function



#End Region ' Updating

End Class

