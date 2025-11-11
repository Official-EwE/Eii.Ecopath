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
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcotracerGroupInput
    Inherits cCoreGroupBase

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcotracerGroupInput)()

#Region " Constructor "

    Friend Sub New(ByRef theCore As cCore, iDBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue

        Try

            Me.DBID = iDBID
            Me.m_dataType = eDataTypes.EcotracerGroupInput
            Me.m_coreComponent = eCoreComponentType.Ecotracer

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' CZero
            val = New cValue(theCore, New Single, eVarNameFlags.CZero, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' CImmig
            val = New cValue(theCore, New Single, eVarNameFlags.CImmig, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' CEnvironment
            val = New cValue(theCore, New Single, eVarNameFlags.CEnvironment, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' CDecay
            val = New cValue(theCore, New Single, eVarNameFlags.CPhysicalDecayRate, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' CAssimilationProp
            val = New cValue(theCore, New Single, eVarNameFlags.CAssimilationProp, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' CMetablismRate
            Dim meta As cVariableMetaData = New cVariableMetaData(0.0, 1.0E+20, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(theCore, New Single, eVarNameFlags.CMetablismRate, eStatusFlags.Null, eValueTypes.Sng, meta)
            Me.m_values.Add(val.varName, val)

            'set status flags to default values
            Me.ResetStatusFlags()
            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcotracerScenarioGroup.")
            m_logger.LogError(ex, "Error creating new cEcotracerScenarioGroup.")
        End Try

    End Sub

#End Region ' Constructor

#Region " Variable via dot(.) operator"

    Public Property CZero() As Single
        Get
            Return CType(Me.GetVariable(eVarNameFlags.CZero), Single)
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CZero, value)
        End Set
    End Property

    Public Property CImmig() As Single
        Get
            Return CType(Me.GetVariable(eVarNameFlags.CImmig), Single)
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CImmig, value)
        End Set
    End Property

    Public Property CEnvironment() As Single
        Get
            Return CType(Me.GetVariable(eVarNameFlags.CEnvironment), Single)
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CEnvironment, value)
        End Set
    End Property

    Public Property CDecay() As Single
        Get
            Return CType(Me.GetVariable(eVarNameFlags.CPhysicalDecayRate), Single)
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CPhysicalDecayRate, value)
        End Set
    End Property

    Public Property CAssimilationProp() As Single
        Get
            Return CType(Me.GetVariable(eVarNameFlags.CAssimilationProp), Single)
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CAssimilationProp, value)
        End Set
    End Property

    Public Property CMetablismRate() As Single
        Get
            Return CType(Me.GetVariable(eVarNameFlags.CMetablismRate), Single)
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CMetablismRate, value)
        End Set
    End Property

#End Region ' Variable via dot(.) operator

#Region " Status Flags via dot(.) operator"

    Public Property CZeroStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CZero)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CZero, value)
        End Set

    End Property

    Public Property CImmigStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CImmig)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CImmig, value)
        End Set

    End Property

    Public Property CEnvironmentStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CEnvironment)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CEnvironment, value)
        End Set

    End Property

    Public Property CDecayStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CPhysicalDecayRate)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CPhysicalDecayRate, value)
        End Set

    End Property

    Public Property CExcretionRateStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CAssimilationProp)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CAssimilationProp, value)
        End Set

    End Property

#End Region ' Status Flags via dot(.) operator

End Class
