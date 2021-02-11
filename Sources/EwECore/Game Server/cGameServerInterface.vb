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

Imports EwEUtils.Core

''' <summary>
''' Core data collector for the OceanViz game server.
''' </summary>
Public Class cGameServerInterface

    Private m_core As cCore
    ' Private m_dctDataTypes As Dictionary(Of EwEUtils.Core.eDataTypes, Object)
    Private m_dctCoreListData As Dictionary(Of EwEUtils.Core.eDataTypes, cCoreInputOutputList(Of EwECore.cCoreInputOutputBase))

    ' JS 14Jan01: Core data objects no longer cached due to new core cleanup dynamics
    '             More vigilant cleanup code requires these objects to be created only when needed
    Private m_dctCoreData As Dictionary(Of EwEUtils.Core.eDataTypes, GetCoreIOObjectDelegate)

    Public Sub New(ByRef theCore As cCore)
        Me.m_core = theCore
    End Sub

    Public Delegate Function GetCoreIOObjectDelegate() As cCoreInputOutputBase

    Friend Sub Init()

        Me.m_dctCoreListData = New Dictionary(Of EwEUtils.Core.eDataTypes, cCoreInputOutputList(Of EwECore.cCoreInputOutputBase))
        Me.m_dctCoreData = New Dictionary(Of EwEUtils.Core.eDataTypes, GetCoreIOObjectDelegate)
        'ecopath
        Me.m_dctCoreListData.Add(eDataTypes.EcoPathGroupInput, Me.m_core.m_EcoPathInputs)
        Me.m_dctCoreListData.Add(eDataTypes.EcoPathGroupOutput, Me.m_core.m_EcoPathOutputs)

        Me.m_dctCoreListData.Add(eDataTypes.FleetInput, Me.m_core.m_EcopathFleetsInput)

        'ecosim
        Me.m_dctCoreListData.Add(eDataTypes.EcoSimGroupOutput, Me.m_core.m_EcoSimGroupOutputs)
        Me.m_dctCoreListData.Add(eDataTypes.EcosimFleetOutput, Me.m_core.m_EcosimFleetOutputs)
        Me.m_dctCoreListData.Add(eDataTypes.EcoSimScenario, Me.m_core.m_EcoSimScenarios)
        Me.m_dctCoreListData.Add(eDataTypes.MSEFleetInput, Me.m_core.MSEManager.EcopathFleetInputs)
        Me.m_dctCoreListData.Add(eDataTypes.EcoSimGroupInput, Me.m_core.m_EcoSimGroups)

        'EcoSpace
        Me.m_dctCoreListData.Add(eDataTypes.EcospaceRegionResults, Me.m_core.m_EcospaceRegionSummaries)
        Me.m_dctCoreListData.Add(eDataTypes.EcospaceGroupOuput, Me.m_core.m_EcospaceGroupOuputs)
        Me.m_dctCoreListData.Add(eDataTypes.EcospaceFleetOuput, Me.m_core.m_EcospaceFleetOutputs)
        Me.m_dctCoreListData.Add(eDataTypes.EcospaceMPA, Me.m_core.m_EcospaceMPAs)
        Me.m_dctCoreListData.Add(eDataTypes.EcospaceHabitat, Me.m_core.m_EcospaceHabitats)

        'MSE 
        Me.m_dctCoreListData.Add(eDataTypes.MSEGroupOutputs, Me.m_core.MSEManager.GroupOutputs)
        Me.m_dctCoreListData.Add(eDataTypes.MSEBiomassStats, Me.m_core.MSEManager.BiomassStats)

        Me.m_dctCoreListData.Add(eDataTypes.MSEGroupInput, Me.m_core.MSEManager.GroupInputs)

        Me.m_dctCoreData.Add(eDataTypes.MSEOutput, New GetCoreIOObjectDelegate(AddressOf Me.m_core.MSEManager.Output))
        Me.m_dctCoreData.Add(eDataTypes.EcosimOutput, New GetCoreIOObjectDelegate(AddressOf Me.m_core.EcosimOutputs))

    End Sub

    Public ReadOnly Property CoreDataList(DataType As EwEUtils.Core.eDataTypes) As cCoreInputOutputList(Of EwECore.cCoreInputOutputBase)
        Get
            Dim data As cCoreInputOutputList(Of EwECore.cCoreInputOutputBase)
            If Me.m_dctCoreListData.ContainsKey(DataType) Then
                data = Me.m_dctCoreListData.Item(DataType)
            End If
            Return data
        End Get
    End Property

    Public ReadOnly Property CoreData(DataType As EwEUtils.Core.eDataTypes) As EwECore.cCoreInputOutputBase
        Get
            Dim data As EwECore.cCoreInputOutputBase
            If Me.m_dctCoreData.ContainsKey(DataType) Then
                data = Me.m_dctCoreData.Item(DataType).Invoke
            End If
            Debug.Assert(data IsNot Nothing, Me.ToString & ".CoreData( " & DataType.ToString & " ) not found in core data!")
            Return data
        End Get
    End Property

    Public ReadOnly Property CoreData(DataType As EwEUtils.Core.eDataTypes, Index As Integer) As EwECore.cCoreInputOutputBase
        Get
            Dim data As EwECore.cCoreInputOutputBase
            If Me.m_dctCoreListData.ContainsKey(DataType) Then
                data = Me.m_dctCoreListData.Item(DataType).Item(Index)
            End If
            Debug.Assert(data IsNot Nothing, Me.ToString & ".CoreData( " & DataType.ToString & ", " & Index.ToString & " ) not found in core data!")
            Return data
        End Get
    End Property

    Public Function ContainKey(DataType As EwEUtils.Core.eDataTypes) As Boolean

        If Me.m_dctCoreListData.ContainsKey(DataType) Or Me.m_dctCoreData.ContainsKey(DataType) Then
            Return True
        End If
        Return False

    End Function

    ''' <summary>
    ''' Clear all game server data structures.
    ''' </summary>
    Public Sub Clear()

        Try
            ' Server may not have been initialized!
            If (Me.m_dctCoreListData IsNot Nothing) Then
                Me.m_dctCoreListData.Clear()
            End If
            If (Me.m_dctCoreData IsNot Nothing) Then
                Me.m_dctCoreData.Clear()
            End If
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Clear() Exception: " & ex.Message)
        End Try

    End Sub

End Class
