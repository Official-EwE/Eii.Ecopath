Option Strict On
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

Imports EwECore.Style

#End Region ' Imports

#Region " Must inherit Base class "

''' <summary>
''' Base class for data source objects used by the <see cref="cEcospaceRegionAvgResultsWriter">cEcospaceAvgModelAreaResultsWriter</see>
''' to write averaged Ecospace results to a csv file. 
''' </summary>
''' <remarks></remarks>
Public MustInherit Class cEcospaceResultsWriterDataSourceBase
    Protected m_core As cCore
    Protected m_spaceData As cEcospaceDataStructures

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        Me.m_core = Core
        Me.m_spaceData = EcospaceData
    End Sub

    ''' <summary>
    ''' Number of results in the data source. This can be ngroups, nfleets, ngroups * nfleets depending on the data.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    MustOverride ReadOnly Property nResults As Integer

    ''' <summary>
    ''' File identifier use to build the file name
    ''' </summary>
    MustOverride ReadOnly Property FilenameIdentifier As String

    ''' <summary>
    ''' Description of the data used in the header of the file
    ''' </summary>
    MustOverride ReadOnly Property DataDescriptor As String

    ''' <summary>
    ''' Description of the area that is covered by the data. This can be the total area or a region
    ''' </summary>
    MustOverride ReadOnly Property AreaDescriptor As String

    ''' <summary>
    ''' Number of water cells in the area
    ''' </summary>
    ''' <value></value>
    MustOverride ReadOnly Property nWaterCells As Integer

    ''' <summary>
    ''' Init the data source
    ''' </summary>
    MustOverride Sub Init(Optional OptionalIndex As Integer = 0)

    ''' <summary>
    ''' Return the result for this index and time step
    ''' </summary>
    ''' <param name="OneBasedIndex">One based index of the result to return</param>
    ''' <param name="TimeIndex">One based time step of the result</param>
    MustOverride Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single

    ''' <summary>
    ''' Name of the result field. This can be a group name, fleet name, or a combo of both
    ''' </summary>
    MustOverride Function FieldName(OneBasedIndex As Integer) As String

    ''' <summary>
    ''' Four character abbreviation of Variable and Area
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    MustOverride Function FileNameAbbreviation() As String

    ''' <summary>
    ''' Index of the Region for this datasource
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    MustOverride ReadOnly Property AreaIndex As Integer

End Class

#End Region ' Must inherit Base class

#Region " Total modeled area "

#Region " Biomass over model area "

''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for biomass averaged over the total modeled area.
''' </summary>
Public Class cBiomassResultsDataSource
    Inherits cEcospaceResultsWriterDataSourceBase

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Return Me.m_spaceData.ResultsByGroup(EwECore.eSpaceResultsGroups.Biomass, OneBasedIndex, TimeIndex)
    End Function

    Public Overrides Sub Init(Optional OptionalIndex As Integer = 0)

    End Sub

    Public Overrides ReadOnly Property nResults As Integer
        Get
            Return Me.m_core.nGroups
        End Get
    End Property

    Public Overrides Function FieldName(OneBasedIndex As Integer) As String
        Return Me.m_core.m_EcopathData.GroupName(OneBasedIndex)
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            Return "Biomass"
        End Get
    End Property

    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            ' ToDo: localize this
            Dim u As New cUnits(Me.m_core)
            Return "Average biomass across modeled area (" & u.ToString(cUnits.CurrencyOverArea) & ")"
        End Get
    End Property

    Public Overrides ReadOnly Property AreaDescriptor As String
        Get
            ' ToDo: localize this
            Dim u As New cUnits(Me.m_core)
            Return "Modeled area (" & u.ToString(cUnits.Area) & ")"
        End Get
    End Property

    Public Overrides ReadOnly Property nWaterCells As Integer
        Get
            Return Me.m_spaceData.nWaterCells
        End Get
    End Property

    Public Overrides Function FileNameAbbreviation() As String
        'Biomass total model area
        Return "BMFL"
    End Function

    Public Overrides ReadOnly Property AreaIndex As Integer
        Get
            Return 0
        End Get
    End Property
End Class

#End Region ' Biomass over model area

#Region " Catch over modeled area "

''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for catch averaged over the total modeled area.
''' </summary>
''' <remarks></remarks>
Public Class cCatchResultsDataSource
    Inherits cEcospaceResultsWriterDataSourceBase

    ''' <summary>
    ''' Local helper class for remembering bits of a landing record.
    ''' </summary>
    Private Class cCatch

        Public Sub New(f As cEcopathFleetInput, g As cCoreGroupBase)
            Me.FleetName = f.Name
            Me.FleetIndex = f.Index
            Me.GroupName = g.Name
            Me.GroupIndex = g.Index
        End Sub

        Public Property FleetName As String
        Public Property FleetIndex As Integer
        Public Property GroupName As String
        Public Property GroupIndex As Integer

    End Class

    Private m_lstCatch As List(Of cCatch)

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Try
            Dim catchOb As cCatch = Me.m_lstCatch.Item(OneBasedIndex - 1)
            Return Me.m_spaceData.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, catchOb.FleetIndex, catchOb.GroupIndex, TimeIndex)
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try

        Return 0.0

    End Function

    Public Overrides Sub Init(Optional OptionalIndex As Integer = 0)

        Me.m_lstCatch = New List(Of cCatch)
        Dim fleet As cEcopathFleetInput = Nothing
        Dim group As cCoreGroupBase = Nothing

        For iFleet As Integer = 1 To Me.m_core.nFleets
            fleet = Me.m_core.EcopathFleetInputs(iFleet)
            For iGroup As Integer = 1 To Me.m_core.nGroups
                group = Me.m_core.EcopathGroupInputs(iGroup)
                If (fleet.Landings(iGroup) + fleet.Discards(iGroup)) > 0 Then
                    'Save the Fleet and group indexes
                    Me.m_lstCatch.Add(New cCatch(fleet, group))
                End If
            Next iGroup
        Next iFleet

    End Sub

    Public Overrides ReadOnly Property nResults As Integer
        Get
            Return Me.m_lstCatch.Count
        End Get
    End Property

    Public Overrides Function FieldName(OneBasedIndex As Integer) As String

        Try
            Dim catchOb As cCatch = Me.m_lstCatch.Item(OneBasedIndex - 1)
            Return catchOb.FleetName + "|" + catchOb.GroupName
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try
        Return ""
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            Return "Catch"
        End Get
    End Property


    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            ' ToDo: localize this
            Dim u As New cUnits(Me.m_core)
            Return "Catch by Fleet x Group (" & u.ToString(cUnits.Currency) & ")"
        End Get
    End Property

    Public Overrides ReadOnly Property nWaterCells As Integer
        Get
            Return Me.m_core.m_EcospaceData.nWaterCells
        End Get
    End Property

    Public Overrides ReadOnly Property AreaDescriptor As String
        Get
            ' ToDo: localize this, use units!
            Dim u As New cUnits(Me.m_core)
            Return "Modeled area (" & u.ToString(cUnits.Area) & ")"
        End Get
    End Property

    Public Overrides Function FileNameAbbreviation() As String
        'Catch total model area
        Return "CTFL"
    End Function

    Public Overrides ReadOnly Property AreaIndex As Integer
        Get
            Return 0
        End Get
    End Property
End Class

#End Region ' Catch over modeled area

#End Region ' Total modeled area

''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for biomass averaged over the total modeled area.
''' </summary>
Public Class cMOTotalResultsDataSource
    Inherits cBiomassResultsDataSource
    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Return Me.m_spaceData.ResultsByGroup(EwECore.eSpaceResultsGroups.OtherMortalityLoss, OneBasedIndex, TimeIndex)
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            ' ToDo: localize this
            Return "OtherMortalityLoss"
        End Get
    End Property

    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            ' ToDo: localize this (and spell correctly)
            Return "Average Other Mortlatiy biomass across modeled area"
        End Get
    End Property

    Public Overrides Function FileNameAbbreviation() As String
        Return "MOFL"
    End Function

End Class

#Region " By Region "

#Region " Biomass by region "

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
            Me.iRegionIndex = RegionIndex
        End Sub

        Public Property GroupName As String
        Public Property GroupIndex As Integer
        Public Property iRegionIndex As Integer

    End Class

    Private m_lstRegions As List(Of cRegion)
    Private m_iRegionIndex As Integer

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Try
            Dim RegionOb As cRegion = Me.m_lstRegions.Item(OneBasedIndex - 1)
            Return Me.m_spaceData.ResultsRegionGroup(RegionOb.iRegionIndex, RegionOb.GroupIndex, TimeIndex)
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try

        Return 0.0
    End Function

    Public Overrides Sub Init(Optional OptionalIndex As Integer = 0)
        Me.m_iRegionIndex = OptionalIndex
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
            Return "Region_" + Me.m_iRegionIndex.ToString + "_Biomass"
        End Get
    End Property

    Public Overrides ReadOnly Property AreaDescriptor As String
        Get
            ' ToDo: localize this 
            Dim u As New cUnits(Me.m_core)
            Return "Region " + Me.m_iRegionIndex.ToString + " area (" & u.ToString(cUnits.Area) & ")"
        End Get
    End Property

    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            ' ToDo: localize this 
            Dim u As New cUnits(Me.m_core)
            Return "Average biomass by region (" & u.ToString(cUnits.CurrencyOverArea) & ")"
        End Get
    End Property

    Public Overrides ReadOnly Property nWaterCells As Integer
        Get
            Return Me.m_core.m_EcospaceData.nCellsInRegion(Me.m_iRegionIndex)
        End Get
    End Property

    Public Overrides Function FileNameAbbreviation() As String
        'Biomass for region
        Dim ReturnStr As String
        Dim RegStr As String = Me.m_iRegionIndex.ToString
        If RegStr.Length = 1 Then
            RegStr = "0" + RegStr
        End If
        ReturnStr = "BR" + RegStr
        Debug.Assert(ReturnStr.Length = 4, "WOW " + Me.ToString + ".FileNameAbbreviation() not the correct length for ICM abbreviation.")
        Return ReturnStr
    End Function

    Public Overrides ReadOnly Property AreaIndex As Integer
        Get
            Return Me.m_iRegionIndex
        End Get
    End Property
End Class

#End Region ' Biomass by region

#Region " Catch by region "

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
    Private m_iRegionIndex As Integer

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Try
            Dim RegionOb As cRegion = Me.m_lstRegions.Item(OneBasedIndex - 1)
            Return Me.m_spaceData.ResultsCatchRegionGearGroup(RegionOb.RegionIndex, RegionOb.FleetIndex, RegionOb.GroupIndex, TimeIndex)
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try

        Return 0.0

    End Function


    Public Overrides Sub Init(Optional OptionalIndex As Integer = 0)

        Me.m_iRegionIndex = OptionalIndex
        Me.m_lstRegions = New List(Of cRegion)

        Dim fleet As cEcopathFleetInput = Nothing
        Dim group As cCoreGroupBase = Nothing

        For iFleet As Integer = 1 To Me.m_core.nFleets
            fleet = Me.m_core.EcopathFleetInputs(iFleet)
            For iGroup As Integer = 1 To Me.m_core.nGroups
                group = Me.m_core.EcopathGroupInputs(iGroup)
                If (fleet.Landings(iGroup) + fleet.Discards(iGroup)) > 0 Then
                    'Save the Fleet and group indexes
                    Me.m_lstRegions.Add(New cRegion(fleet, group, Me.m_iRegionIndex))
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
            Return "Region_" + Me.m_iRegionIndex.ToString + "_Catch"
        End Get
    End Property

    Public Overrides ReadOnly Property AreaDescriptor As String
        Get
            ' ToDo: localize this, use units!
            Dim u As New cUnits(Me.m_core)
            Return "Region " + Me.m_iRegionIndex.ToString + " area (" & u.ToString(cUnits.Area) & ")"
        End Get
    End Property

    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            ' ToDo: localize this 
            Dim u As New cUnits(Me.m_core)
            Return "Average catch in region by fleet and group (" & u.ToString(cUnits.CurrencyOverArea) & ")"
        End Get
    End Property

    Public Overrides ReadOnly Property nWaterCells As Integer
        Get
            Return Me.m_core.m_EcospaceData.nCellsInRegion(Me.m_iRegionIndex)
        End Get
    End Property

    Public Overrides Function FileNameAbbreviation() As String
        'Biomass for region
        Dim ReturnStr As String
        Dim RegStr As String = Me.m_iRegionIndex.ToString
        If RegStr.Length = 1 Then
            RegStr = "0" + RegStr
        End If
        ReturnStr = "CR" + RegStr
        Debug.Assert(ReturnStr.Length = 4, "WOW " + Me.ToString + ".FileNameAbbreviation() not the correct length for ICM abbreviation.")
        Return ReturnStr
    End Function

    Public Overrides ReadOnly Property AreaIndex As Integer
        Get
            Return Me.m_iRegionIndex
        End Get
    End Property

End Class

#End Region ' Catch by region

#Region " Consumption by region "


''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for averaged biomass by region.
''' </summary>
Public Class cRegionConsuptionResultsDataSource
    Inherits cEcospaceResultsWriterDataSourceBase

    ''' <summary>
    ''' Local helper class for remembering bits of a landing record.
    ''' </summary>
    Private Class cRegion

        Public Sub New(pred As cCoreGroupBase, prey As cCoreGroupBase, RegionIndex As Integer)
            Me.PredName = pred.Name
            Me.PredIndex = pred.Index
            Me.PreyName = prey.Name
            Me.PreyIndex = prey.Index
            Me.iRegionIndex = RegionIndex
        End Sub

        Public Property PredName As String = ""
        Public Property PredIndex As Integer
        Public Property PreyName As String = ""
        Public Property PreyIndex As Integer
        Public Property iRegionIndex As Integer

    End Class

    Private m_lstRegions As List(Of cRegion)
    Private m_iRegionIndex As Integer

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Try
            Dim r As cRegion = Me.m_lstRegions.Item(OneBasedIndex - 1)
            Return Me.m_spaceData.ResultsRegionConsumptionPredPrey(r.iRegionIndex, r.PredIndex, r.PreyIndex, TimeIndex)
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try

        Return 0.0
    End Function

    Public Overrides Sub Init(Optional iRegion As Integer = 0)
        Me.m_iRegionIndex = iRegion
        Me.m_lstRegions = New List(Of cRegion)
        For iPred As Integer = 1 To Me.m_core.nGroups
            Dim grp As cEcoPathGroupInput = Me.m_core.EcopathGroupInputs(iPred)
            For iPrey As Integer = 1 To Me.m_core.nGroups
                If grp.DietComp(iPrey) > 0 Then
                    Me.m_lstRegions.Add(New cRegion(Me.m_core.EcopathGroupInputs(iPred), Me.m_core.EcopathGroupInputs(iPrey), iRegion))
                End If
            Next iPrey
        Next iPred
    End Sub

    Public Overrides ReadOnly Property nResults As Integer
        Get
            Return Me.m_lstRegions.Count
        End Get
    End Property

    Public Overrides Function FieldName(OneBasedIndex As Integer) As String
        Try
            Dim region As cRegion = Me.m_lstRegions.Item(OneBasedIndex - 1)
            Return region.PredName + "|" + region.PreyName
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try
        Return ""
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            Return "Region_" + Me.m_iRegionIndex.ToString + "_Consumption"
        End Get
    End Property

    Public Overrides ReadOnly Property AreaDescriptor As String
        Get
            ' ToDo: localize this, use units!
            Dim u As New cUnits(Me.m_core)
            Return "Region " + Me.m_iRegionIndex.ToString + " area (" & u.ToString(cUnits.Area) & ")"
        End Get
    End Property

    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            ' ToDo: localize this, use units!
            Dim u As New cUnits(Me.m_core)
            Return "Average consumption by region (" & u.ToString(cUnits.CurrencyOverArea) & ")"
        End Get
    End Property

    Public Overrides ReadOnly Property nWaterCells As Integer
        Get
            Return Me.m_core.m_EcospaceData.nCellsInRegion(Me.m_iRegionIndex)
        End Get
    End Property

    Public Overrides Function FileNameAbbreviation() As String
        'Consumption for region
        Dim ReturnStr As String
        Dim RegStr As String = Me.m_iRegionIndex.ToString
        If RegStr.Length = 1 Then
            RegStr = "0" + RegStr
        End If
        ReturnStr = "CoR" + RegStr
        'Debug.Assert(ReturnStr.Length = 4, "WOW " + Me.ToString + ".FileNameAbbreviation() not the correct length for ICM abbreviation.")
        Return ReturnStr
    End Function

    Public Overrides ReadOnly Property AreaIndex As Integer
        Get
            Return Me.m_iRegionIndex
        End Get
    End Property
End Class

#End Region ' Consumption by region

#End Region ' By Region




