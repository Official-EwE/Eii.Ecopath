' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Public Class cMPAOptParameters
    Inherits cCoreInputOutputBase

    Public Sub New(core As cCore)
        MyBase.New(core)

        Try
            'no data validation at this time
            Me.AllowValidation = False
            Me.m_coreComponent = eCoreComponentType.MPAOptimization
            Me.m_dataType = eDataTypes.MPAOptParameters
            Dim status As eStatusFlags = eStatusFlags.OK
            Dim val As cValue

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            'MPAOptSearchType stored as an integer
            val = New cValue(core, New Integer, eVarNameFlags.MPAOptSearchType, status, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'BoundaryWeight
            val = New cValue(core, New Single, eVarNameFlags.MPAOptBoundaryWeight, status, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'MPAOptStepSize
            val = New cValue(core, New Integer, eVarNameFlags.MPAOptStepSize, status, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'MPAOptIterations
            val = New cValue(core, New Integer, eVarNameFlags.MPAOptIterations, status, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'MPAOptMaxArea %
            val = New cValue(core, New Integer, eVarNameFlags.MPAOptMaxArea, status, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'MPAOptMinArea %
            val = New cValue(core, New Integer, eVarNameFlags.MPAOptMinArea, status, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'MPATouse
            val = New cValue(core, New Integer, eVarNameFlags.iMPAOptToUse, status, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'MPAbUseCellWeight
            val = New cValue(core, New Boolean, eVarNameFlags.MPAUseCellWeight, eStatusFlags.OK, eValueTypes.Bool)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Integer, eVarNameFlags.MPAOptStartYear, status, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Integer, eVarNameFlags.MPAOptEndYear, status, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'MPAOptUseRegions
            val = New cValue(core, New Boolean, eVarNameFlags.MPAOptUseRegions, eStatusFlags.OK, eValueTypes.Bool)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            Me.AllowValidation = True

        Catch ex As Exception

            Debug.Assert(False, Me.ToString)

        End Try

    End Sub

    Public Property SearchType() As eMPAOptimizationModels
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.MPAOptSearchType), eMPAOptimizationModels)
        End Get

        Set(newValue As eMPAOptimizationModels)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptSearchType, newValue)
            End If
        End Set

    End Property

    Public Property BoundaryWeight() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MPAOptBoundaryWeight))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptBoundaryWeight, newValue)
            End If
        End Set

    End Property

    Public Property StepSize() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptStepSize))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptStepSize, newValue)
            End If
        End Set

    End Property

    Public Property nIterations() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptIterations))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptIterations, newValue)
            End If
        End Set

    End Property

    Public Property MaxArea() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptMaxArea))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptMaxArea, newValue)
            End If
        End Set

    End Property

    Public Property MinArea() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptMinArea))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptMinArea, newValue)
            End If
        End Set

    End Property

    Public Property iMPAToUse() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.iMPAOptToUse))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.iMPAOptToUse, newValue)
            End If
        End Set

    End Property

    Public Property UseCellWeight() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.MPAUseCellWeight))
        End Get

        Set(newValue As Boolean)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAUseCellWeight, newValue)
            End If
        End Set
    End Property

    Public Property UseRegions() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.MPAOptUseRegions))
        End Get
        Set(newValue As Boolean)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptUseRegions, newValue)
            End If
        End Set
    End Property

    Public Property StartYear() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptStartYear))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptStartYear, newValue)
            End If
        End Set

    End Property

    Public Property EndYear() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptEndYear))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptEndYear, newValue)
            End If
        End Set

    End Property

    Public Property bUseCellWeightStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MPAUseCellWeight)
        End Get

        Set(newValue As eStatusFlags)
            If Not Me.m_bReadOnly Then
                Me.SetStatus(eVarNameFlags.MPAUseCellWeight, newValue)
            End If
        End Set

    End Property

    Public Property SearchTypeStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MPAOptSearchType)
        End Get

        Set(newValue As eStatusFlags)
            If Not Me.m_bReadOnly Then
                Me.SetStatus(eVarNameFlags.MPAOptSearchType, newValue)
            End If
        End Set

    End Property

    Public Property BoundaryWeightStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MPAOptBoundaryWeight)
        End Get

        Set(newValue As eStatusFlags)
            If Not Me.m_bReadOnly Then
                Me.SetStatus(eVarNameFlags.MPAOptBoundaryWeight, newValue)
            End If
        End Set

    End Property

    Public Property StepSizeStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MPAOptStepSize)
        End Get

        Set(newValue As eStatusFlags)
            If Not Me.m_bReadOnly Then
                Me.SetStatus(eVarNameFlags.MPAOptStepSize, newValue)
            End If
        End Set

    End Property

    Public Property nIterationsStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MPAOptIterations)
        End Get

        Set(newValue As eStatusFlags)
            If Not Me.m_bReadOnly Then
                Me.SetStatus(eVarNameFlags.MPAOptIterations, newValue)
            End If
        End Set

    End Property

    Public Property MaxAreaStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MPAOptMaxArea)
        End Get

        Set(newValue As eStatusFlags)
            If Not Me.m_bReadOnly Then
                Me.SetStatus(eVarNameFlags.MPAOptMaxArea, newValue)
            End If
        End Set

    End Property

    Public Property MinAreaStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MPAOptMinArea)
        End Get

        Set(newValue As eStatusFlags)
            If Not Me.m_bReadOnly Then
                Me.SetStatus(eVarNameFlags.MPAOptMinArea, newValue)
            End If
        End Set

    End Property

    Public Property iMPAToUseStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.iMPAOptToUse)
        End Get

        Set(newValue As eStatusFlags)
            If Not Me.m_bReadOnly Then
                Me.SetStatus(eVarNameFlags.iMPAOptToUse, newValue)
            End If
        End Set

    End Property

End Class
