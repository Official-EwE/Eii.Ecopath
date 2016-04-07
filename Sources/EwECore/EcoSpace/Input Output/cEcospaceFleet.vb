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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceFleet
    Inherits cCoreInputOutputBase

#Region " Constructor "

    Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Try

            Me.m_dataType = eDataTypes.EcospaceFleet
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.DBID = iDBID

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' EffectivePower
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.EffectivePower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' SEmult
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.SEmult, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Array variables

            ' HabitatFishery
            meta = New cVariableMetaData(False)
            val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.HabitatFishery, eStatusFlags.Null, eCoreCounterTypes.nHabitats, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' MPAFishery
            meta = New cVariableMetaData(False)
            val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.MPAFishery, eStatusFlags.Null, eCoreCounterTypes.nMPAs, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceFleet.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceFleet. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructor

#Region " Properties by dot (.) operator "

    Public Property EffectivePower() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EffectivePower))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EffectivePower, value)
        End Set
    End Property

    Public Property HabitatFishery(ByVal iHabitat As Integer) As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.HabitatFishery, iHabitat))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.HabitatFishery, value, iHabitat)
        End Set
    End Property

    Public Property MPAFishery(ByVal iMPA As Integer) As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MPAFishery, iMPA))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MPAFishery, value, iMPA)
        End Set
    End Property

    Public Property TotalEffMultiplier() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.SEmult))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.SEmult, value)
        End Set
    End Property


#End Region ' Properties by dot (.) operator

#Region " Status by dot (.) operator "

    Public Property EffectivePowerStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EffectivePower)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EffectivePower, value)
        End Set
    End Property

    Public Property HabitatFisheryStatus(ByVal iHabitat As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.HabitatFishery, iHabitat)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.HabitatFishery, value, iHabitat)
        End Set
    End Property

    Public Property MPAFisheryStatus(ByVal iMPA As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAFishery, iMPA)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MPAFishery, value, iMPA)
        End Set
    End Property

 

#End Region ' Status by dot (.) operator 

End Class
