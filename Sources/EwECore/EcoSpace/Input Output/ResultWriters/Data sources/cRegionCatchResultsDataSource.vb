' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style
Imports EwEUtils.Utilities

''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for averaged catch by region.
''' </summary>
''' <remarks></remarks>
Public Class cRegionCatchResultsDataSource
    Inherits cEcospaceResultsWriterDataSourceBase

    ''' <summary>
    ''' Local helper class for remembering region and group/fleet info
    ''' </summary>
    Private Class cRegion

        Public Sub New(f As cEcopathFleetInput, g As cCoreGroupBase, RegionIndex As Integer)
            Me.FleetName = f.Name
            Me.FleetIndex = f.Index
            Me.GroupName = g.Name
            Me.GroupIndex = g.Index
            Me.RegionIndex = RegionIndex
        End Sub

        Public Property GroupName As String
        Public Property GroupIndex As Integer
        Public Property FleetName As String
        Public Property FleetIndex As Integer
        Public Property RegionIndex As Integer

    End Class

    Private m_lstRegions As List(Of cRegion)
    Private m_RegionIndex As Integer

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Try
            Dim r As cRegion = Me.m_lstRegions.Item(OneBasedIndex - 1)
            Return Me.m_spaceData.ResultsCatchRegionGearGroup(r.RegionIndex, r.FleetIndex, r.GroupIndex, TimeIndex)
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try

        Return 0.0

    End Function


    Public Overrides Sub Init(Optional OptionalIndex As Integer = 0)

        Me.m_RegionIndex = OptionalIndex
        Me.m_lstRegions = New List(Of cRegion)

        Dim fleet As cEcopathFleetInput = Nothing
        Dim group As cCoreGroupBase = Nothing

        For iFleet As Integer = 1 To Me.m_core.nFleets
            fleet = Me.m_core.EcopathFleetInputs(iFleet)
            For iGroup As Integer = 1 To Me.m_core.nGroups
                group = Me.m_core.EcopathGroupInputs(iGroup)
                If (fleet.Landings(iGroup) + fleet.Discards(iGroup)) > 0 Then
                    'Save the Fleet and group indexes
                    Me.m_lstRegions.Add(New cRegion(fleet, group, Me.m_RegionIndex))
                End If
            Next iGroup
        Next iFleet

    End Sub

    Public Overrides ReadOnly Property nResults As Integer
        Get
            Return Me.m_lstRegions.Count
        End Get
    End Property

    Public Overrides Function FieldName(OneBasedIndex As Integer) As String

        Try
            Dim region As cRegion = Me.m_lstRegions.Item(OneBasedIndex - 1)
            Return region.FleetName + "|" + region.GroupName
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try

        Return ""
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            Return "Region_" + Me.m_RegionIndex.ToString + "_Catch"
        End Get
    End Property

    Public Overrides ReadOnly Property AreaDescriptor As String
        Get
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_REGION, Me.m_RegionIndex)
        End Get
    End Property

    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            Dim u As New cUnits(Me.m_core)
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_REGAVG_CATCH_UNIT, u.ToString(cUnits.Currency))
        End Get
    End Property

    Public Overrides ReadOnly Property nWaterCells As Integer
        Get
            Return Me.m_core.m_EcospaceData.RegionCells(Me.m_RegionIndex)
        End Get
    End Property

    Public Overrides ReadOnly Property AreaIndex As Integer
        Get
            Return Me.m_RegionIndex
        End Get
    End Property

End Class