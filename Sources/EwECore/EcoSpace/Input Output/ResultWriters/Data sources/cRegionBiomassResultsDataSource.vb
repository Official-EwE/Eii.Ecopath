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
#Region " Imports "

Option Strict On
Imports EwECore.Style
Imports EwEUtils.Utilities

#End Region

''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for averaged biomass by region.
''' </summary>
Public Class cRegionBiomassResultsDataSource
    Inherits cEcospaceResultsWriterDataSourceBase

    ''' <summary>
    ''' Local helper class for remembering bits of a landing record.
    ''' </summary>
    Private Class cRegion

        Public Sub New(g As cCoreGroupBase, RegionIndex As Integer)
            Me.GroupName = g.Name
            Me.GroupIndex = g.Index
            Me.RegionIndex = RegionIndex
        End Sub

        Public Property GroupName As String
        Public Property GroupIndex As Integer
        Public Property RegionIndex As Integer

    End Class

    Private m_lstRegions As List(Of cRegion)
    Private m_RegionIndex As Integer

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Try
            Dim RegionOb As cRegion = Me.m_lstRegions.Item(OneBasedIndex - 1)
            Return Me.m_spaceData.ResultsRegionGroup(RegionOb.RegionIndex, RegionOb.GroupIndex, TimeIndex)
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try

        Return 0.0
    End Function

    Public Overrides Sub Init(Optional OptionalIndex As Integer = 0)
        Me.m_RegionIndex = OptionalIndex
        Me.m_lstRegions = New List(Of cRegion)
        For iGroup As Integer = 1 To Me.m_core.nGroups
            Me.m_lstRegions.Add(New cRegion(Me.m_core.EcopathGroupInputs(iGroup), OptionalIndex))
        Next iGroup
    End Sub

    Public Overrides ReadOnly Property nResults As Integer
        Get
            Return Me.m_lstRegions.Count
        End Get
    End Property

    Public Overrides Function FieldName(OneBasedIndex As Integer) As String
        Try
            Return Me.m_lstRegions.Item(OneBasedIndex - 1).GroupName
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try
        Return ""
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            Return "Region_" + Me.m_RegionIndex.ToString + "_Biomass"
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
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_REGAVG_B_UNIT, u.ToString(cUnits.Currency))
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

