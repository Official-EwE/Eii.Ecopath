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
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceGroupInput
    Inherits cCoreGroupBase

#Region " Constructor "

    Sub New(core As cCore, DBID As Integer)
        MyBase.New(core)

        Me.DBID = DBID
        Me.m_dataType = eDataTypes.EcospaceGroup
        Me.m_coreComponent = eCoreComponentType.Ecospace

        Dim val As cValue

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' Mvel
            val = New cValue(core, New Single, eVarNameFlags.MVel, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' RelMoveBad
            val = New cValue(core, New Single, eVarNameFlags.RelMoveBad, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' RelVulBad
            val = New cValue(core, New Single, eVarNameFlags.RelVulBad, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' IsAdvected
            val = New cValue(core, New Boolean, eVarNameFlags.IsAdvected, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            ' IsMigratory
            val = New cValue(core, New Boolean, eVarNameFlags.IsMigratory, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            ' PredictEffort
            val = New cValue(core, New Boolean, eVarNameFlags.PredictEffort, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            ' Barrier avoidance weight
            val = New cValue(core, New Single, eVarNameFlags.BarrierAvoidanceWeight, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' Capacity calculations
            val = New cValue(core, 1, eVarNameFlags.EcospaceCapCalType, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            'inMigAreaMoveWeight
            val = New cValue(core, New Single, eVarNameFlags.InMigAreaMoveWeight, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' KMoveFitness
            val = New cValue(core, New Single, eVarNameFlags.KMoveFitness, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Array variables

            'PreferredHabitat()
            val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.PreferredHabitat, eStatusFlags.Null, eCoreCounterTypes.nHabitats)
            Me.m_values.Add(val.varName, val)

            'set status flags to their default values
            Me.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceGroup.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceGroup. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region " Overrides "

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        MyBase.ResetStatusFlags(bForceReset)
        Me.m_core.Set_BadHab_Flags(Me)
        Me.m_core.Set_HabPref_Flags(Me)
        Me.m_core.Set_Migratory_Flags(Me)
    End Function

#End Region ' Overrides

#Region "Properties by dot (.) operator "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the <see cref="eEcospaceCapacityCalType">inputs</see> that Ecospace uses to calculate capacity.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CapacityCalculationType() As eEcospaceCapacityCalType

        Get
            Return CType(Me.GetVariable(eVarNameFlags.EcospaceCapCalType), eEcospaceCapacityCalType)
        End Get

        Set(value As eEcospaceCapacityCalType)
            Me.SetVariable(eVarNameFlags.EcospaceCapCalType, value)
        End Set

    End Property

    ''' <summary>Base dispersal</summary>
    Public Property MVel() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MVel))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.MVel, value)
        End Set
    End Property

    ''' <summary>Relative dispersal in bad habitat</summary>
    Public Property RelMoveBad() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.RelMoveBad))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.RelMoveBad, value)
        End Set
    End Property

    ''' <summary>Relative vulnerability in bad habitat</summary>
    Public Property RelVulBad() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.RelVulBad))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.RelVulBad, value)
        End Set
    End Property

    Public Property IsAdvected() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.IsAdvected))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.IsAdvected, value)
        End Set
    End Property

    Public Property IsMigratory() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.IsMigratory))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.IsMigratory, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the fraction that a group can use a habitat.
    ''' </summary>
    ''' <param name="iHabitat">One-based haitat index.</param>
    Public Property PreferredHabitat(iHabitat As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.PreferredHabitat, iHabitat))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.PreferredHabitat, value, iHabitat)
        End Set
    End Property

    Public Property BarrierAvoidanceWeight() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BarrierAvoidanceWeight))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.BarrierAvoidanceWeight, value)
        End Set
    End Property


    Public Property InMigrationAreaMovement() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.InMigAreaMoveWeight))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.InMigAreaMoveWeight, value)
        End Set
    End Property

    ''' <summary>Relative vulnerability in bad habitat</summary>
    Public Property KMoveFitness() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.KMoveFitness))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.KMoveFitness, value)
        End Set
    End Property

#End Region

#Region "Status by dot (.) operator"

    Public Property MVelStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MVel)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MVel, value)
        End Set
    End Property

    Public Property RelMoveBadStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.RelMoveBad)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.RelMoveBad, value)
        End Set
    End Property

    Public Property RelVulBadStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.RelVulBad)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.RelVulBad, value)
        End Set
    End Property

    Public Property IsAdvectedStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.IsAdvected)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.IsAdvected, value)
        End Set
    End Property

    Public Property IsMigratoryStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.IsMigratory)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.IsMigratory, value)
        End Set
    End Property

    Public Property PreferredHabitatStatus(iHabitat As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.PreferredHabitat, iHabitat)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.PreferredHabitat, value, iHabitat)
        End Set
    End Property

    Public Property InMigrationAreaMovementStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.InMigAreaMoveWeight)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.InMigAreaMoveWeight, value)
        End Set
    End Property

    Public Property KMoveFitnessStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.KMoveFitness)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.KMoveFitness, value)
        End Set
    End Property

#End Region

End Class

<Obsolete("Please use cEcospaceGroupInput instead")>
Public Class cEcospaceGroup
    Inherits cEcospaceGroupInput

    Sub New(ByRef theCore As cCore, DBID As Integer)
        MyBase.New(theCore, DBID)
    End Sub

End Class
