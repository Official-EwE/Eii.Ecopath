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




Imports EwECore.Style
Imports EwEUtils.Utilities



''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for averaged landings by region.
''' </summary>
''' <remarks>
''' This code was built for EU project FutureMares on behalf of Chris Lynam.
''' </remarks>
Public Class cRegionLandingsResultsDataSource
    Inherits cEcospaceResultsWriterDataSourceBase

    ''' <summary>
    ''' Local helper class for remembering bits of a landing record.
    ''' </summary>
    Private Class cRegion

        Public Sub New(fleet As cCoreInputOutputBase, target As cCoreGroupBase, RegionIndex As Integer)
            Me.FleetName = fleet.Name
            Me.FleetIndex = fleet.Index
            Me.TargetName = target.Name
            Me.TargetIndex = target.Index
            Me.RegionIndex = RegionIndex
        End Sub

        Public Property FleetName As String = ""
        Public Property FleetIndex As Integer
        Public Property TargetName As String = ""
        Public Property TargetIndex As Integer
        Public Property RegionIndex As Integer

    End Class

    Private m_regions As List(Of cRegion)
    Private m_RegionIndex As Integer

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Try
            Dim r As cRegion = Me.m_regions.Item(OneBasedIndex - 1)
            Return Me.m_spaceData.ResultsLandingsRegionGearGroup(r.RegionIndex, r.FleetIndex, r.TargetIndex, TimeIndex)
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try

        Return 0.0
    End Function

    Public Overrides Sub Init(Optional iRegion As Integer = 0)
        Me.m_RegionIndex = iRegion
        Me.m_regions = New List(Of cRegion)
        For iFleet As Integer = 1 To Me.m_core.nFleets
            Dim flt As cEcopathFleetInput = Me.m_core.EcopathFleetInputs(iFleet)
            For iTarget As Integer = 1 To Me.m_core.nGroups
                If flt.Landings(iTarget) > 0 Then
                    Me.m_regions.Add(New cRegion(flt, Me.m_core.EcopathGroupInputs(iTarget), iRegion))
                End If
            Next iTarget
        Next iFleet
    End Sub

    Public Overrides ReadOnly Property nResults As Integer
        Get
            Return Me.m_regions.Count
        End Get
    End Property

    Public Overrides Function FieldName(OneBasedIndex As Integer) As String
        Try
            Dim region As cRegion = Me.m_regions.Item(OneBasedIndex - 1)
            Return region.FleetName + "|" + region.TargetName
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try
        Return ""
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            Return "Region_" + Me.m_RegionIndex.ToString + "_Landings"
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
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_REGAVG_LANDINGS_UNIT, u.ToString(cUnits.Currency))
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