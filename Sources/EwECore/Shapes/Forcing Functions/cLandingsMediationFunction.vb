' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

#Region " cLandingsMediationFunction "

''' <summary>
''' A derived mediation function, dedicated to modelling fleet-group interactions
''' based on landings.
''' </summary>
''' <remarks></remarks>
Public Class cLandingsMediationFunction
    Inherits cMediationBaseFunction

    Private m_groups As New List(Of cLandingsMediatingGroup)

#Region " Constructors "

    Friend Sub New(EcoSimData As cEcosimDatastructures, Manager As cBaseShapeManager,
                   data As cMediationDataStructures, DBID As Integer, DataType As eDataTypes)
        'mediation data arrays from EcoSim
        'Public MedWeights(nGroups + nGear, MediationShapes) As Single 'defines biomass weights for med X
        'Public NMedXused() As Integer 'number of biomasses (mediation weights) in an iMediation
        'Public IMedUsed(,) As Integer 'groups used in med function X IMedUsed(nGroups + nGear, MediationShapes)
        'Public MedXbase() As Single 'ecopath base value of med function X
        'Public MedYbase() As Single 'value of med function at ecopath base X
        'Public MedIsUsed() As Boolean 'true if med function iMediation is used
        MyBase.New(EcoSimData, Manager, data, DBID, DataType)

        Try

            Me.m_datatype = eDataTypes.PriceMediation
            Dim iShape As Integer = Me.m_iEcoSimIndex 'just for clarity

            Me.m_manager = Manager 'keep a reference to the manager for this shape

            Dim grp As cLandingsMediatingGroup = Nothing

            ' Groups: if this mediation shape has any weights applied to it then load the weight and group into an object
            For iGrp As Integer = 1 To Me.m_data.nGroups
                For iflt As Integer = 0 To Me.m_data.nGear
                    If Me.m_medData.MedPriceWeights(iGrp, iflt, iShape) > 0 Then
                        grp = New cLandingsMediatingGroup(iGrp, iflt, Me.m_medData.MedPriceWeights(iGrp, iflt, iShape))
                        Me.m_groups.Add(grp)
                    End If
                Next
            Next

            Me.m_bInInit = False
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".New() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".New() Error: " & ex.Message, ex)
        End Try

    End Sub

#End Region ' Constructors

#Region " Updating "

    ''' <summary>
    ''' Update the underlying EcoSim data structures
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update() As Boolean
        MyBase.Update()

        'do not update during initialization
        If Me.m_bInInit Then
            Return False
        End If

        Dim nused As Integer
        For Each grp As cLandingsMediatingGroup In Me.m_groups
            nused += 1
            Me.m_medData.IMedFltUsed(grp.iFleetIndex, Me.m_iEcoSimIndex) = grp.iFleetIndex
            Me.m_medData.IMedUsed(grp.iGroupIndex, Me.m_iEcoSimIndex) = grp.iGroupIndex
            Me.m_medData.MedPriceWeights(grp.iGroupIndex, grp.iFleetIndex, Me.m_iEcoSimIndex) = grp.Weight
        Next grp

        'nused = 0

        Me.m_medData.NMedXused(Me.m_iEcoSimIndex) = nused

        'tell the manager that a shape has changed it's data
        Me.ShapeChanged()

        Return True

    End Function

    ''' <summary>
    ''' Clear all the data, in the underlying ecosim data, out of the MedWeights for this mediation shape.
    ''' </summary>
    ''' <remarks>
    ''' This is used if a mediating group is removed to clear the ecosim data before the group is removed from the list. 
    ''' This must be used in conjuction the Update() to restore the data
    ''' </remarks>
    Private Sub clearMedWeights()

        Try

            For Each grp As cLandingsMediatingGroup In Me.m_groups
                Me.m_medData.IMedFltUsed(grp.iFleetIndex, Me.m_iEcoSimIndex) = 0
                Me.m_medData.IMedUsed(grp.iGroupIndex, Me.m_iEcoSimIndex) = 0
                Me.m_medData.MedPriceWeights(grp.iGroupIndex, grp.iFleetIndex, Me.m_iEcoSimIndex) = 0
            Next grp

        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub

#End Region ' Updating

#Region " Implementation of Must Override properties "

    ''' <inheritdocs cref="cMediationBaseFunction.AddGroup"/>
    ''' <param name="iFleet"></param>
    Public Overloads Overrides Function AddGroup(iGroup As Integer, weight As Single, Optional iFleet As Integer = cCore.NULL_VALUE) As Boolean
        'ToDo: data validation
        Debug.Assert(iFleet >= 0, Me.ToString & ".AddGroup() Invalid Fleet index")
        Me.m_groups.Add(New cLandingsMediatingGroup(iGroup, iFleet, weight))
        Me.Update()
        Return True

    End Function

    ''' <inheritdocs cref="cMediationBaseFunction.NumGroups"/>
    Public Overrides ReadOnly Property NumGroups() As Integer
        Get
            Return Me.m_groups.Count
        End Get
    End Property

    ''' <inheritdocs cref="cMediationBaseFunction.Group"/>
    Public Overrides Property Group(iGroup As Integer) As cMediatingGroup
        Get
            Return Me.m_groups(iGroup)
        End Get

        Set(value As cMediatingGroup)
            Try
                Dim grp As cLandingsMediatingGroup = DirectCast(value, cLandingsMediatingGroup)
                Me.m_groups.Item(iGroup) = grp
                Me.Update()
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Group() Failed to add group Invalid group type!")
            End Try
        End Set

    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="iIndex">Zero-based index [0, <see cref="NumGroups"/>-1] of the 
    ''' mediating group to remove.</param>
    ''' <returns></returns>
    Public Function RemoveGroup(iIndex As Integer) As Boolean

        Try
            'clear the ecosim data
            Me.clearMedWeights()
            'remove the group from the list
            Me.m_groups.RemoveAt(iIndex)
            'update the ecosim data with the remaining group(s)
            Me.Update()

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function RemoveGroup(ByRef group As cLandingsMediatingGroup) As Boolean

        Try
            'clear the ecosim data
            Me.clearMedWeights()
            'remove the group from the list
            Me.m_groups.Remove(group)
            'update the ecosim data with the remaining group(s)
            Me.Update()
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    ''' <inheritdocs cref="cForcingFunction.Clear"/>
    Public Overrides Sub Clear()

        Try
            'clear the ecosim data
            Me.clearMedWeights()
            Me.m_groups.Clear()

            MyBase.Clear()

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub

    Public Overrides Function AddFleet(iFleet As Integer, weight As Single) As Boolean
        Debug.Assert(False, Me.ToString & ".AddFleet() Property not supported.")
        Return False
    End Function

    Public Overrides ReadOnly Property NumFleet() As Integer
        Get
            ' Debug.Assert(False, Me.ToString & ".CountFleet() Property not supported.")
            Return 0
        End Get
    End Property

    Public Overrides Property Fleet(iGroup As Integer) As cMediatingFleet
        Get
            Return Nothing
        End Get
        Set(value As cMediatingFleet)
            Debug.Assert(False, Me.ToString & ".Fleet() Property not supported.")
        End Set
    End Property

#End Region '  List Interfaces

End Class

#End Region

