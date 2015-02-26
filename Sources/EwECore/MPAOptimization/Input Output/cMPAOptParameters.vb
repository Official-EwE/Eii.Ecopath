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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cMPAOptParameters
    Inherits cCoreInputOutputBase

    Public Sub New(ByRef m_core As cCore)
        MyBase.New(m_core)

        Try
            'no data validation at this time
            Me.AllowValidation = False
            m_coreComponent = eCoreComponentType.MPAOptimization
            m_dataType = eDataTypes.MPAOptParameters
            Dim status As eStatusFlags = eStatusFlags.Null
            Dim val As cValue
            Dim meta As cVariableMetaData

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            'MPAOptSearchType stored as an integer
            meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eMPAOptimizationModels)).Length - 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MPAOptSearchType, status, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MPAOptSearchType))
            val.Stored = False
            m_values.Add(val.varName, val)

            'BoundaryWeight
            meta = New cVariableMetaData(0.0!, 10.0!, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MPAOptBoundaryWeight, status, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MPAOptBoundaryWeight))
            val.Stored = False
            m_values.Add(val.varName, val)

            'MPAOptStepSize
            meta = New cVariableMetaData(0, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MPAOptStepSize, status, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MPAOptStepSize))
            val.Stored = False
            m_values.Add(val.varName, val)

            'MPAOptIterations
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MPAOptIterations, status, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MPAOptIterations))
            val.Stored = False
            m_values.Add(val.varName, val)

            'MPAOptMaxArea %
            meta = New cVariableMetaData(0, 100, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MPAOptMaxArea, status, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MPAOptMaxArea))
            val.Stored = False
            m_values.Add(val.varName, val)

            'MPAOptMinArea %
            meta = New cVariableMetaData(0, 100, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MPAOptMinArea, status, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MPAOptMinArea))
            val.Stored = False
            m_values.Add(val.varName, val)

            'MPATouse
            meta = New cVariableMetaData(0, 100, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.iMPAOptToUse, status, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.iMPAOptToUse))
            val.Stored = False
            m_values.Add(val.varName, val)

            'MPAbUseCellWeight
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.MPAbUseCellWeight, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MPAbUseCellWeight))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MPAOptStartYear, status, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MPAOptStartYear))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MPAOptEndYear, status, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MPAOptEndYear))
            val.Stored = False
            m_values.Add(val.varName, val)

            Me.AllowValidation = True

        Catch ex As Exception

            Debug.Assert(False, Me.ToString)

        End Try

    End Sub

    Public Property SearchType() As eMPAOptimizationModels
        Get
            Return DirectCast(GetVariable(eVarNameFlags.MPAOptSearchType), eMPAOptimizationModels)
        End Get

        Set(ByVal newValue As eMPAOptimizationModels)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptSearchType, newValue)
            End If
        End Set

    End Property

    Public Property BoundaryWeight() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MPAOptBoundaryWeight))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptBoundaryWeight, newValue)
            End If
        End Set

    End Property


    Public Property StepSize() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MPAOptStepSize))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptStepSize, newValue)
            End If
        End Set

    End Property

    Public Property nIterations() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MPAOptIterations))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptIterations, newValue)
            End If
        End Set

    End Property

    Public Property MaxArea() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MPAOptMaxArea))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptMaxArea, newValue)
            End If
        End Set

    End Property


    Public Property MinArea() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MPAOptMinArea))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptMinArea, newValue)
            End If
        End Set

    End Property

    Public Property iMPAToUse() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.iMPAOptToUse))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.iMPAOptToUse, newValue)
            End If
        End Set

    End Property


    Public Property bUseCellWeight() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MPAbUseCellWeight))
        End Get

        Set(ByVal newValue As Boolean)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAbUseCellWeight, newValue)
            End If
        End Set

    End Property


    Public Property StartYear() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MPAOptStartYear))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptStartYear, newValue)
            End If
        End Set

    End Property
    Public Property EndYear() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MPAOptEndYear))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptEndYear, newValue)
            End If
        End Set

    End Property



    Public Property bUseCellWeightStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAbUseCellWeight)
        End Get

        Set(ByVal newValue As eStatusFlags)
            If Not m_bReadOnly Then
                SetStatus(eVarNameFlags.MPAbUseCellWeight, newValue)
            End If
        End Set

    End Property




    Public Property SearchTypeStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAOptSearchType)
        End Get

        Set(ByVal newValue As eStatusFlags)
            If Not m_bReadOnly Then
                SetStatus(eVarNameFlags.MPAOptSearchType, newValue)
            End If
        End Set

    End Property

    Public Property BoundaryWeightStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAOptBoundaryWeight)
        End Get

        Set(ByVal newValue As eStatusFlags)
            If Not m_bReadOnly Then
                SetStatus(eVarNameFlags.MPAOptBoundaryWeight, newValue)
            End If
        End Set

    End Property


    Public Property StepSizeStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAOptStepSize)
        End Get

        Set(ByVal newValue As eStatusFlags)
            If Not m_bReadOnly Then
                SetStatus(eVarNameFlags.MPAOptStepSize, newValue)
            End If
        End Set

    End Property

    Public Property nIterationsStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAOptIterations)
        End Get

        Set(ByVal newValue As eStatusFlags)
            If Not m_bReadOnly Then
                SetStatus(eVarNameFlags.MPAOptIterations, newValue)
            End If
        End Set

    End Property

    Public Property MaxAreaStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAOptMaxArea)
        End Get

        Set(ByVal newValue As eStatusFlags)
            If Not m_bReadOnly Then
                SetStatus(eVarNameFlags.MPAOptMaxArea, newValue)
            End If
        End Set

    End Property


    Public Property MinAreaStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAOptMinArea)
        End Get

        Set(ByVal newValue As eStatusFlags)
            If Not m_bReadOnly Then
                SetStatus(eVarNameFlags.MPAOptMinArea, newValue)
            End If
        End Set

    End Property


    Public Property iMPAToUseStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.iMPAOptToUse)
        End Get

        Set(ByVal newValue As eStatusFlags)
            If Not m_bReadOnly Then
                SetStatus(eVarNameFlags.iMPAOptToUse, newValue)
            End If
        End Set

    End Property

End Class
