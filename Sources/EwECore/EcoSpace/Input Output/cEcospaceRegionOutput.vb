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


Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cEcospaceRegionOutput
    Inherits cCoreInputOutputBase

    Private m_spacedata As cEcospaceDataStructures
    Private m_CoreArrays As New Dictionary(Of eVarNameFlags, IResultsWrapper)
    Private m_CatchFleetGroup(,,) As Single
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceRegionOutput)()

#Region "Constructor"

    Public Sub New(core As cCore, EcospaceData As cEcospaceDataStructures, iRegion As Integer)
        MyBase.New(core)

        Me.m_spacedata = EcospaceData

        Me.DBID = iRegion '????
        Me.Index = iRegion
        Me.m_dataType = eDataTypes.EcospaceRegionResults

        Dim val As cValue

        'Weirdness
        'There are three ways of managing data
        'If the data has a core array then use that directly via the m_CoreArrays dictionary
        'If no core array and the data can fit into a cValue object then use that, only one variable index
        'If no core array and the data contains more then one variable index then use a local buffer

        'cValue objects
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionBiomassStart, eStatusFlags.OK, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)

        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionBiomassEnd, eStatusFlags.OK, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)

    End Sub


    Public Sub Init()

        Try
            Me.m_CoreArrays.Clear()
            Me.m_CoreArrays.Add(eVarNameFlags.EcospaceRegionBiomass, New c3DResultsWrapper(Me.m_spacedata.ResultsRegionGroup, Me.Index))
            Me.m_CoreArrays.Add(eVarNameFlags.EcospaceRegionBiomassYear, New c3DResultsWrapper(Me.m_spacedata.ResultsRegionGroupYear, Me.Index))
            Me.m_CoreArrays.Add(eVarNameFlags.EcospaceRegionFleetGroupCatch, New c4DResultsWrapperFirstFixed(Me.m_spacedata.ResultsCatchRegionGearGroup, Me.Index))
            Me.m_CoreArrays.Add(eVarNameFlags.EcospaceRegionFleetGroupCatchYear, New c4DResultsWrapperFirstFixed(Me.m_spacedata.ResultsCatchRegionGearGroup, Me.Index))
            Me.m_CoreArrays.Add(eVarNameFlags.EcospaceRegionConsumption, New c4DResultsWrapperFirstFixed(Me.m_spacedata.ResultsRegionConsumptionPredPrey, Me.Index))
            Me.m_CoreArrays.Add(eVarNameFlags.EcospaceRegionLandings, New c4DResultsWrapperFirstFixed(Me.m_spacedata.ResultsLandingsRegionGearGroup, Me.Index))
            Me.m_CoreArrays.Add(eVarNameFlags.EcospaceRegionValue, New c4DResultsWrapperFirstFixed(Me.m_spacedata.ResultsValueRegionGearGroup, Me.Index))
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Init() Error: " & ex.Message)
            m_logger.LogError(ex, "cEcospaceRegionOutput.Init() Error")
        End Try

    End Sub

#End Region

#Region "Implementation of GetVariable() SetVariable() GetStatus() SetStatus()"

    Public Overrides Function GetVariable(varName As eVarNameFlags, Optional iFirstIndex As Integer = cCore.NULL_VALUE, Optional iSecondIndex As Integer = cCore.NULL_VALUE, Optional iIndex3 As Integer = cCore.NULL_VALUE) As Object
        Try

            If Not Me.m_CoreArrays.ContainsKey(varName) Then
                Debug.Assert(iSecondIndex = cCore.NULL_VALUE, Me.ToString & ".GetVariable() called with optional argument iSecondIndex for variable " & varName.ToString & " this can not be handled for this variable.")
                'NOT in list of sim vars so get the value from the base class GetVariable(...)
                Return MyBase.GetVariable(varName, iFirstIndex)
            Else
                'Varname is access directly via the core data
                Return Me.m_CoreArrays.Item(varName).Value(iFirstIndex, iSecondIndex, iIndex3)
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return cCore.NULL_VALUE

    End Function


    Public Overloads Function GetStatus(varName As eVarNameFlags, iFleet As Integer, iGroup As Integer) As eStatusFlags
        Return eStatusFlags.OK 'Oh Yeah 
    End Function

    Public Overloads Function SetStatus(varName As eVarNameFlags, newValue As eStatusFlags, iFleet As Integer, iGroup As Integer) As Boolean
        Debug.Assert(False, "Not implemented yet.")
    End Function

    Friend Overrides Function Resize() As Boolean
        MyBase.Resize()

        'resize local buffer
        ReDim Me.m_CatchFleetGroup(1, Me.m_core.nFleets, Me.m_core.nGroups)
        Return True
    End Function


#End Region

#Region "Variable via dot '.' operator"

    Public Property BiomassStart(iGroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceRegionBiomassStart, iGroup))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceRegionBiomassStart, value, iGroup)
        End Set
    End Property

    Public Property BiomassEnd(iGroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceRegionBiomassEnd, iGroup))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceRegionBiomassEnd, value, iGroup)
        End Set
    End Property

    Public Property CatchFleetGroupStart(iFleet As Integer, iGroup As Integer) As Single
        Get
            Try
                Return Me.m_CatchFleetGroup(0, iFleet, iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try
        End Get

        Set(value As Single)
            Try
                Me.m_CatchFleetGroup(0, iFleet, iGroup) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property


    Public Property CatchFleetGroupEnd(iFleet As Integer, iGroup As Integer) As Single
        Get
            Try
                Return Me.m_CatchFleetGroup(1, iFleet, iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try
        End Get

        Set(value As Single)
            Try
                Me.m_CatchFleetGroup(1, iFleet, iGroup) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property

    Public ReadOnly Property EcospaceRegionConsumptionByTime(iPred As Integer, iPrey As Integer, iTimestep As Integer) As Single
        Get
            Try
                Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceRegionConsumption, iPred, iPrey, iTimestep), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try
        End Get
    End Property

    Public ReadOnly Property BiomassByTime(iGroup As Integer, iTimeStep As Integer) As Single
        Get
            Try
                Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceRegionBiomass, iGroup, iTimeStep), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try
        End Get
    End Property

    Public ReadOnly Property BiomassByYear(iGroup As Integer, iYear As Integer) As Single
        Get
            Try
                Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceRegionBiomassYear, iGroup, iYear), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try
        End Get
    End Property

    Public ReadOnly Property CatchFleetGroupTime(iFleet As Integer, iGroup As Integer, iTimeStep As Integer) As Single
        Get
            Try
                Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceRegionFleetGroupCatch, iFleet, iGroup, iTimeStep), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try
        End Get
    End Property

    Public ReadOnly Property CatchFleetGroupYear(iFleet As Integer, iGroup As Integer, iYear As Integer) As Single
        Get
            Try
                Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceRegionFleetGroupCatchYear, iFleet, iGroup, iYear), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try
        End Get
    End Property

#End Region

#Region "Status Flags via dot '.' operator"

    Public Property BiomassStartStatus(iGroup As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceRegionBiomassStart, iGroup)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcospaceRegionBiomassStart, value, iGroup)
        End Set
    End Property

    Public Property BiomassEndStatus(iGroup As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceRegionBiomassEnd, iGroup)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcospaceRegionBiomassEnd, value, iGroup)
        End Set
    End Property


    Public Property CatchFleetGroupStartStatus(iGroup As Integer, iFleet As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceRegionCatchStart, iGroup, iFleet)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcospaceRegionCatchStart, value, iGroup, iFleet)
        End Set
    End Property


    Public Property CatchFleetGroupEndStatus(iGroup As Integer, iFleet As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceRegionCatchEnd, iGroup, iFleet)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcospaceRegionCatchEnd, value, iGroup, iFleet)
        End Set
    End Property

#End Region

End Class
