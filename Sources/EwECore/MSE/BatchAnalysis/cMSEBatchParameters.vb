' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Public Class cMSEBatchParameters
    Inherits cCoreGroupBase

    Public Sub New(core As cCore, ByRef MSEBatchData As MSEBatchManager.cMSEBatchDataStructures, DBID As Integer)
        MyBase.New(core)

        Dim val As cValue

        Me.m_dataType = eDataTypes.MSEBatchParameters
        Me.m_coreComponent = eCoreComponentType.MSE
        Me.AllowValidation = False
        Me.DBID = DBID

        'default OK status used for setVariable
        'see comment setVariable(...)
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        val = New cValue(core, New Integer, eVarNameFlags.MSETFMNIteration, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.MSEBatchFNIteration, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.MSEBatchTACNIteration, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.MSEBatchIterCalcType, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Boolean, eVarNameFlags.MSEBatchOutputBiomass, eStatusFlags.Null, eValueTypes.Bool)
        val.Stored = False
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Boolean, eVarNameFlags.MSEBatchOutputConBio, eStatusFlags.Null, eValueTypes.Bool)
        val.Stored = False
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Boolean, eVarNameFlags.MSEBatchOutputFeedingTime, eStatusFlags.Null, eValueTypes.Bool)
        val.Stored = False
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Boolean, eVarNameFlags.MSEBatchOutputPredRate, eStatusFlags.Null, eValueTypes.Bool)
        val.Stored = False
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Boolean, eVarNameFlags.MSEBatchOutputCatch, eStatusFlags.Null, eValueTypes.Bool)
        val.Stored = False
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Boolean, eVarNameFlags.MSEBatchOutputFishingMortRate, eStatusFlags.Null, eValueTypes.Bool)
        val.Stored = False
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, "", eVarNameFlags.MSEBatchOuputDir, eStatusFlags.Null, eValueTypes.Str)
        val.Stored = False
        Me.m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub

    Public Property nTFMIteration As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MSETFMNIteration))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.MSETFMNIteration, value)
        End Set
    End Property

    Public Property nFixedFIteration As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MSEBatchFNIteration))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.MSEBatchFNIteration, value)
        End Set
    End Property

    Public Property nTACIteration As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MSEBatchTACNIteration))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.MSEBatchTACNIteration, value)
        End Set
    End Property

    Public Property IterCalcType As Integer
        Get
            Return CType(Me.GetVariable(eVarNameFlags.MSEBatchIterCalcType), eMSEBatchIterCalcTypes)
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.MSEBatchIterCalcType, value)
        End Set
    End Property

    Public Property bSaveBiomass As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.MSEBatchOutputBiomass))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.MSEBatchOutputBiomass, value)
        End Set
    End Property

    Public Property bSaveCatch As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.MSEBatchOutputCatch))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.MSEBatchOutputCatch, value)
        End Set
    End Property

    Public Property bSaveConsumptBio As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.MSEBatchOutputConBio))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.MSEBatchOutputConBio, value)
        End Set
    End Property

    Public Property bSaveFeedingTime As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.MSEBatchOutputFeedingTime))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.MSEBatchOutputFeedingTime, value)
        End Set
    End Property

    Public Property bSavePredRate As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.MSEBatchOutputPredRate))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.MSEBatchOutputPredRate, value)
        End Set
    End Property
    Public Property bSaveFishingMort As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.MSEBatchOutputFishingMortRate))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.MSEBatchOutputFishingMortRate, value)
        End Set
    End Property

    Public Property OutputDir As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.MSEBatchOuputDir))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.MSEBatchOuputDir, value)
        End Set
    End Property

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        MyBase.ResetStatusFlags(bForceReset)
        Me.AllowValidation = False
        Dim tcatch As Single

        For iflt As Integer = 1 To Me.m_core.nFleets
            Dim fleet As cEcopathFleetInput = Me.m_core.EcopathFleetInputs(iflt)
            tcatch += fleet.Landings(Me.Index) + fleet.Discards(Me.Index)
        Next

        If tcatch = 0.0! Then
            For Each var As cValue In Me.m_values.Values
                If var.varName = eVarNameFlags.MSEBatchGroupRunType Then
                    For igrp As Integer = 1 To Me.m_core.nGroups
                        Me.SetStatusFlags(var.varName, eStatusFlags.Null Or eStatusFlags.NotEditable, igrp)
                    Next
                End If
            Next
        End If
        Return True

    End Function

End Class
