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
            Dim u As New cUnits(Me.m_core)
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_AVG_CATCH_UNIT, u.ToString(cUnits.Currency))
        End Get
    End Property

    Public Overrides ReadOnly Property nWaterCells As Integer
        Get
            Return Me.m_core.m_EcospaceData.nWaterCells
        End Get
    End Property

    Public Overrides ReadOnly Property AreaDescriptor As String
        Get
            Dim u As New cUnits(Me.m_core)
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_AREA_UNIT, u.ToString(cUnits.Area))
        End Get
    End Property

    Public Overrides ReadOnly Property AreaIndex As Integer
        Get
            Return 0
        End Get
    End Property
End Class