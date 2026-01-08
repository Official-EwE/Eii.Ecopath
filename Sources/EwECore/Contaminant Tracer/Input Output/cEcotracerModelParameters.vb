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
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcotracerModelParameters
    Inherits cCoreInputOutputBase

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcotracerModelParameters)()

#Region " Constructor "

    Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)

        Dim val As cValue

        Try

            Me.m_dataType = eDataTypes.EcotracerModelParameters
            Me.m_coreComponent = eCoreComponentType.Ecotracer

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' CZero
            val = New cValue(theCore, New Single, eVarNameFlags.CZero, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' CInflow
            val = New cValue(theCore, New Single, eVarNameFlags.CInflow, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' COutflow
            val = New cValue(theCore, New Single, eVarNameFlags.COutflow, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' CDecay
            val = New cValue(theCore, New Single, eVarNameFlags.CPhysicalDecayRate, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'ConForceNumber
            val = New cValue(theCore, New Integer, eVarNameFlags.ConForceNumber, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            'Max number of time steps
            val = New cValue(theCore, New Integer, eVarNameFlags.ConMaxTimeSteps, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            'set status flags to default values
            Me.ResetStatusFlags()
            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcotracerScenario.")
            Me.m_logger.LogError(ex, "Error creating new cEcotracerScenario.")
        End Try

    End Sub

#End Region ' Constructor

#Region " Overrides "

    'Friend Overrides Function ResetStatusFlags() As Boolean

    '    If Not MyBase.ResetStatusFlags() Then Return False

    '    Me.SetStatusFlags(eVarNameFlags.CInflow, eStatusFlags.NotEditable, 0)
    '    Me.SetStatusFlags(eVarNameFlags.COutflow, eStatusFlags.NotEditable, 0)

    '    Return False

    'End Function

#End Region ' Overrides

#Region " Variable via dot(.) operator"

    Public Property CZero() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.CZero))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CZero, value)
        End Set
    End Property

    Public Property CInflow() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.CInflow))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CInflow, value)
        End Set
    End Property

    Public Property COutflow() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.COutflow))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.COutflow, value)
        End Set
    End Property

    Public Property CDecay() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.CPhysicalDecayRate))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CPhysicalDecayRate, value)
        End Set
    End Property

    ''' <summary>
    ''' Concentration forcing function number
    ''' </summary>
    Public Property ConForceNumber() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.ConForceNumber))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.ConForceNumber, value)
        End Set
    End Property

    'ConMaxTimeSteps


    Public Property MaxTimeSteps() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.ConMaxTimeSteps))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.ConMaxTimeSteps, value)
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

    Public Property CInflowStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CInflow)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CInflow, value)
        End Set

    End Property

    Public Property COutflowStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.COutflow)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.COutflow, value)
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

    Public Property MaxTimeStepsStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.ConMaxTimeSteps)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.ConMaxTimeSteps, value)
        End Set

    End Property

#End Region ' Status Flags via dot(.) operator

End Class
