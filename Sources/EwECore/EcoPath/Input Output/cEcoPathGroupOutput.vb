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
Imports EwEUtils.Utilities

''' <summary>
''' Results from EcoPath for a single group.
''' </summary>
''' <remarks>
''' This class wraps the outputs from EcoPath for one group into a single object.
''' </remarks>
Public Class cEcopathGroupOutput
    Inherits cCoreGroupBase

    Private m_nGroups As Integer

#Region "Functionality specific to this class"

    Private Enum eNullTestTypes As Integer
        ''' <summary>Value is not allowed to be 0 or core Null.</summary>
        NonZero
        ''' <summary>Value must be larger than 0 (and not core Null).</summary>
        GreaterThanZero
        ''' <summary>Value must not be core Null.</summary>
        NonCoreNull
    End Enum

    ''' <summary>
    ''' Set the status flag of this variable to NULL if it is less than zero
    ''' </summary>
    ''' <param name="varName">Name of the variable that will get the status flag set</param>
    ''' <param name="sValueToTest">
    ''' <para>Value of the variable to test.</para>
    ''' <para>The value is passed in so that the calling method can use either the core data structures or the output object. 
    ''' If just the eVarNameFlags is used then only the getVariable() method can be used to retrieve the value.</para>
    ''' </param>
    ''' <param name="Index">Optional variable index.</param>
    ''' <param name="nullTest">Flag stating how to test for NULL values.</param>
    Private Sub SetNullFlag(varName As eVarNameFlags, sValueToTest As Single,
            Optional Index As Integer = -9999, Optional nullTest As eNullTestTypes = eNullTestTypes.GreaterThanZero)

        Dim bIsNull As Boolean = False

        Select Case nullTest
            Case eNullTestTypes.NonZero
                'jb added test for NULL_VALUE
                bIsNull = (sValueToTest = 0.0!) Or (sValueToTest = cCore.NULL_VALUE)
            Case eNullTestTypes.GreaterThanZero
                bIsNull = (sValueToTest <= 0.0!)
            Case eNullTestTypes.NonCoreNull
                bIsNull = (sValueToTest = cCore.NULL_VALUE)
        End Select

        Try
            If bIsNull Then
                Me.SetStatusFlags(varName, eStatusFlags.Null, Index)
            Else
                Me.ClearStatusFlags(varName, eStatusFlags.Null, Index)
            End If
        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub


#End Region

#Region "Must Override Methods"

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        Dim sg As cStanzaGroup = Nothing

        MyBase.ResetStatusFlags(bForceReset)

        Try

            'Set the Status Flags to ValueComputed for input/output pairs 
            'if the modeled value is different than the input value.
            'The original data structure is needed to perform this.

            ' JS 2022-06-30: This should really be set by the core, as the I/O objects should not have any knowledge about data structures.

            If (Not cNumberUtils.Approximates(Me.m_core.m_EcopathData.EE(Me.Index), Me.m_core.m_EcopathData.EEinput(Me.Index), 0.0001)) And
               (Me.m_core.m_EcopathData.EE(Me.Index) <> (1 - Me.m_core.m_EcopathData.OtherMortinput(Me.Index))) Then
                Me.SetStatusFlags(eVarNameFlags.EEOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.EEOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.EEOutput, Me.m_core.m_EcopathData.EE(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            If Me.m_core.m_EcopathData.PB(Me.Index) <> Me.m_core.m_EcopathData.PBinput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.PBOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.PBOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.PBOutput, Me.m_core.m_EcopathData.PB(Me.Index))

            If Me.m_core.m_EcopathData.QB(Me.Index) <> Me.m_core.m_EcopathData.QBinput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.QBOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.QBOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.QBOutput, Me.m_core.m_EcopathData.QB(Me.Index))

            If Me.m_core.m_EcopathData.GE(Me.Index) <> Me.m_core.m_EcopathData.GEinput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.GEOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.GEOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.GEOutput, Me.m_core.m_EcopathData.GE(Me.Index))

            If Me.m_core.m_EcopathData.B(Me.Index) <> Me.m_core.m_EcopathData.Binput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.Biomass, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.Biomass, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.Biomass, Me.m_core.m_EcopathData.B(Me.Index))

            If Me.m_core.m_EcopathData.BH(Me.Index) <> Me.m_core.m_EcopathData.BHinput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.BiomassAreaOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.BiomassAreaOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.BiomassAreaOutput, Me.m_core.m_EcopathData.BH(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'Joeh
            'A in LW
            If Me.m_core.m_PSDData.AinLW(Me.Index) <> Me.m_core.m_PSDData.AinLWInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.AinLWOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.AinLWOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.AinLWOutput, Me.m_core.m_PSDData.AinLW(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'B in LW
            If Me.m_core.m_PSDData.BinLW(Me.Index) <> Me.m_core.m_PSDData.BinLWInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.BinLWOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.BinLWOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.BinLWOutput, Me.m_core.m_PSDData.BinLW(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'Loo 
            If Me.m_core.m_PSDData.Loo(Me.Index) <> Me.m_core.m_PSDData.LooInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.LooOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.LooOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.LooOutput, Me.m_core.m_PSDData.Loo(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'Winf 
            If Me.m_core.m_PSDData.Winf(Me.Index) <> Me.m_core.m_PSDData.WinfInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.WinfOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.WinfOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.WinfOutput, Me.m_core.m_PSDData.Winf(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            't0
            If Me.m_core.m_PSDData.t0(Me.Index) <> Me.m_core.m_PSDData.t0Input(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.t0Output, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.t0Output, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.t0Output, Me.m_core.m_PSDData.t0(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'Tcatch
            If Me.m_core.m_PSDData.Tcatch(Me.Index) <> Me.m_core.m_PSDData.TcatchInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.TCatchOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.TCatchOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.TCatchOutput, Me.m_core.m_PSDData.Tcatch(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)

            'Tmax
            If Me.m_core.m_PSDData.Tmax(Me.Index) <> Me.m_core.m_PSDData.TmaxInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.TmaxOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.TmaxOutput, eStatusFlags.ValueComputed)
            End If
            Me.SetNullFlag(eVarNameFlags.TmaxOutput, Me.m_core.m_PSDData.Tmax(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)
            'End Joeh

            If Me.m_core.m_EcopathData.BA(Me.Index) <> Me.m_core.m_EcopathData.BAInput(Me.Index) Then
                Me.SetStatusFlags(eVarNameFlags.BioAccumOutput, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.BioAccumOutput, eStatusFlags.ValueComputed)
            End If

            If Me.m_core.m_EcopathData.BaBi(Me.Index) <> (Me.m_core.m_EcopathData.B(Me.Index) / Me.m_core.m_EcopathData.B(Me.Index)) Then
                Me.SetStatusFlags(eVarNameFlags.BioAccumRatePerYear, eStatusFlags.ValueComputed)
            Else
                Me.ClearStatusFlags(eVarNameFlags.BioAccumRatePerYear, eStatusFlags.ValueComputed)
            End If

            Me.SetNullFlag(eVarNameFlags.BioAccumOutput, Me.m_core.m_EcopathData.BA(Me.Index), cCore.NULL_VALUE, eNullTestTypes.NonZero)

            'test for NULL values in other variables
            Me.SetNullFlag(eVarNameFlags.BioAccumRatePerYear, Me.BioAccumRatePerYear, cCore.NULL_VALUE, eNullTestTypes.NonZero)

            Me.SetNullFlag(eVarNameFlags.MortCoBioAcumRate, Me.MortCoBioAcumRate, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            Me.SetNullFlag(eVarNameFlags.MortCoFishRate, Me.MortCoFishRate, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            Me.SetNullFlag(eVarNameFlags.MortCoNetMig, Me.MortCoNetMig, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            ' This value can be negative
            Me.SetNullFlag(eVarNameFlags.MortCoOtherMort, Me.MortCoOtherMort, cCore.NULL_VALUE, eNullTestTypes.NonCoreNull)
            Me.SetNullFlag(eVarNameFlags.MortCoPB, Me.MortCoPB)
            Me.SetNullFlag(eVarNameFlags.MortCoPredMort, Me.MortCoPredMort)

            ' Key indices
            Me.SetNullFlag(eVarNameFlags.NetMigration, Me.NetMigration, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            Me.SetNullFlag(eVarNameFlags.FlowToDet, Me.FlowToDet, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            Me.SetNullFlag(eVarNameFlags.NetEfficiency, Me.NetEfficiency, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            Me.SetNullFlag(eVarNameFlags.OmnivoryIndex, Me.OmnivoryIndex, cCore.NULL_VALUE, eNullTestTypes.NonZero)

            Me.SetNullFlag(eVarNameFlags.Assimilation, Me.Assimilation)
            Me.SetNullFlag(eVarNameFlags.Respiration, Me.Respiration)
            Me.SetNullFlag(eVarNameFlags.RespAssim, Me.RespAssim, cCore.NULL_VALUE, eNullTestTypes.NonZero)
            Me.SetNullFlag(eVarNameFlags.ProdResp, Me.ProdResp)
            Me.SetNullFlag(eVarNameFlags.RespBiom, Me.RespBiom)

            For i As Integer = 1 To Me.m_nGroups
                Me.SetNullFlag(eVarNameFlags.Consumption, Me.Consumption(i), i)
                Me.SetNullFlag(eVarNameFlags.PredMort, Me.PredMort(i), i, eNullTestTypes.NonZero)
                Me.SetNullFlag(eVarNameFlags.SearchRate, Me.SearchRate(i), i, eNullTestTypes.NonZero)

                ' Set highlight on cannibalism cells (fixes bug 435)
                If i = Me.Index Then
                    Me.SetStatusFlags(eVarNameFlags.PredMort, eStatusFlags.CoreHighlight, i)
                End If
            Next

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Function

#End Region

#Region "Construction and Initialization"

    Sub New(core As cCore, DBID As Integer)
        MyBase.New(core)

        Dim val As cValue

        'default is readonly
        Me.m_bReadOnly = True
        Me.AllowValidation = False

        'todo_jb f/z and m/z

        'get the number of groups from the core delegate
        Me.m_nGroups = Me.m_core.GetCoreCounter(eCoreCounterTypes.nGroups)
        Me.m_dataType = eDataTypes.EcoPathGroupOutput

        ' Outputs should never send out messages
        Me.m_coreComponent = eCoreComponentType.NotSet

        'default OK status used for SetVariable
        'see comment SetVariable(...)
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        Me.DBID = DBID
        'jb 25-Aug-2017 Allow EE-Output to be zero in the UI.
        'There is something strane going on here. This worked for some models with the default metadata operator, EE would show as zero, but not others...
        'val = New cValue(core, New Single, eVarNameFlags.EEOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        val = New cValue(core, New Single, eVarNameFlags.EEOutput, eStatusFlags.NotEditable, eValueTypes.Sng, New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)))
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.PBOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.QBOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.GEOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.HabitatArea, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.BioAccumOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.Biomass, eStatusFlags.NotEditable, eValueTypes.Sng, Nothing)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.BiomassAreaOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.BioAccumRatePerYear, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.DetImp, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.GS, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.TTLX, eStatusFlags.NotEditable, eValueTypes.Sng, Nothing)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.ImportedConsumption, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' -- mortalities --
        val = New cValue(core, New Single, eVarNameFlags.MortCoPB, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.MortCoFishRate, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.MortCoPredMort, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.MortCoBioAcumRate, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.MortCoNetMig, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.MortCoOtherMort, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.NetMigration, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.FlowToDet, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.NetEfficiency, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.OmnivoryIndex, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.Respiration, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.Assimilation, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.ProdResp, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.RespAssim, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.RespBiom, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' -- arrayed values --
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.Consumption, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.PredMort, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.SearchRate, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.Hlap, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.Plap, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.Alpha, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)

        ' -- TODO: VALIDATE UNITS --

        val = New cValue(core, New Single, eVarNameFlags.VBK, eStatusFlags.NotEditable, eValueTypes.Sng, Nothing)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.BiomassAvgSzWt, eStatusFlags.NotEditable, eValueTypes.Sng, Nothing)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.BiomassSzWt, eStatusFlags.NotEditable, eValueTypes.Sng, Nothing)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.TCatchOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.AinLWOutput, eStatusFlags.NotEditable, eValueTypes.Sng, Nothing)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.BinLWOutput, eStatusFlags.NotEditable, eValueTypes.Sng, Nothing)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.LooOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.WinfOutput, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.t0Output, eStatusFlags.NotEditable, eValueTypes.Sng, Nothing)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.TmaxOutput, eStatusFlags.NotEditable, eValueTypes.Sng, Nothing)
        Me.m_values.Add(val.varName, val)

        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcopathWeight, eStatusFlags.NotEditable, eCoreCounterTypes.nEcopathAgeSteps)
        Me.m_values.Add(val.varName, val)
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcopathNumber, eStatusFlags.NotEditable, eCoreCounterTypes.nEcopathAgeSteps)
        Me.m_values.Add(val.varName, val)
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcopathBiomass, eStatusFlags.NotEditable, eCoreCounterTypes.nEcopathAgeSteps)
        Me.m_values.Add(val.varName, val)
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.LorenzenMortality, eStatusFlags.NotEditable, eCoreCounterTypes.nEcopathAgeSteps)
        Me.m_values.Add(val.varName, val)

        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.PSD, eStatusFlags.NotEditable, eCoreCounterTypes.nWeightClasses)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.FishMortTotMort, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        val = New cValue(core, New Single, eVarNameFlags.NatMortPerTotMort, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

    End Sub

#End Region

#Region "Variables as Public Properties Via dot(.) operator"

    Public Property Area() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.HabitatArea))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.HabitatArea, newValue)
            End If
        End Set

    End Property

    Public Property Biomass() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Biomass))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Biomass, newValue)
            End If
        End Set

    End Property

    Public Property BiomassArea() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BiomassAreaOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.BiomassAreaOutput, newValue)
            End If
        End Set

    End Property

    Public Property BioAccum() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BioAccumOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.BioAccumOutput, newValue)
            End If
        End Set

    End Property

    Public Property BioAccumRatePerYear() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BioAccumRatePerYear))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.BioAccumRatePerYear, newValue)
            End If
        End Set

    End Property

    Public Property QBOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.QBOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.QBOutput, newValue)
            End If
        End Set

    End Property

    Public Property PBOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.PBOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.PBOutput, newValue)
            End If
        End Set

    End Property

    Public Property EEOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EEOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.EEOutput, newValue)
            End If
        End Set

    End Property

    Public Property GEOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.GEOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.GEOutput, newValue)
            End If
        End Set

    End Property

    'Joeh
    Public Property VBK() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.VBK))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.VBK, newValue)
            End If
        End Set
    End Property

    Public Property BiomassAvgSzWt() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BiomassAvgSzWt))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.BiomassAvgSzWt, newValue)
            End If
        End Set

    End Property

    Public Property BiomassSzWt() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BiomassSzWt))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.BiomassSzWt, newValue)
            End If
        End Set

    End Property

    Public Property TcatchOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TCatchOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.TCatchOutput, newValue)
            End If
        End Set

    End Property

    Public Property AinLWOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.AinLWOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.AinLWOutput, newValue)
            End If
        End Set

    End Property

    Public Property BinLWOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BinLWOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.BinLWOutput, newValue)
            End If
        End Set

    End Property

    Public Property LooOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.LooOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.LooOutput, newValue)
            End If
        End Set

    End Property

    Public Property WinfOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.WinfOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.WinfOutput, newValue)
            End If
        End Set

    End Property

    Public Property t0Output() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.t0Output))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.t0Output, newValue)
            End If
        End Set

    End Property

    Public Property TmaxOutput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TmaxOutput))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.TmaxOutput, newValue)
            End If
        End Set

    End Property

    Public Property EcopathWeight(iTimeStep As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathWeight, iTimeStep))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.EcopathWeight, newValue, iTimeStep)
            End If
        End Set
    End Property

    Public Property EcopathWeight() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcopathWeight), Single())
        End Get
        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.EcopathWeight, newValue)
            End If
        End Set
    End Property

    Public Property EcopathNumber(iTimeStep As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathNumber, iTimeStep))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.EcopathNumber, newValue, iTimeStep)
            End If
        End Set
    End Property

    Public Property EcopathNumber() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcopathNumber), Single())
        End Get
        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.EcopathNumber, newValue)
            End If
        End Set
    End Property

    Public Property EcopathBiomass(iTimeStep As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathBiomass, iTimeStep))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.EcopathBiomass, newValue, iTimeStep)
            End If
        End Set
    End Property

    Public Property EcopathBiomass() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcopathBiomass), Single())
        End Get
        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.EcopathBiomass, newValue)
            End If
        End Set
    End Property

    Public Property LorenzenMortality(iTimeStep As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.LorenzenMortality, iTimeStep))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.LorenzenMortality, newValue, iTimeStep)
            End If
        End Set
    End Property

    Public Property LorenzenMortality() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.LorenzenMortality), Single())
        End Get
        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.LorenzenMortality, newValue)
            End If
        End Set
    End Property

    Public Property PSD(iWeightClass As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.PSD, iWeightClass))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.PSD, newValue, iWeightClass)
            End If
        End Set
    End Property

    Public Property PSD() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.PSD), Single())
        End Get
        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.PSD, newValue)
            End If
        End Set
    End Property

    Public Property GS() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.GS))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.GS, newValue)
            End If
        End Set
    End Property

    Public Property TTLX() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TTLX))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.TTLX, newValue)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Predation mortality on this group caused by this ipred
    ''' </summary>
    ''' <param name="iPred">iPredator group </param>
    ''' <value>Returns predation mortality on this group caused by this iPredator</value>
    ''' <remarks>
    ''' B(pred) * QB(pred) * DC(pred, prey) / B(prey) 
    '''</remarks>
    Public Property PredMort(iPred As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.PredMort, iPred))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.PredMort, newValue, iPred)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Predation mortality array
    ''' </summary>
    ''' <value>Returns an array of predation mortalities for this group</value>
    ''' <remarks> B(pred) * QB(pred) * DC(pred, prey) / B(prey) </remarks>
    Public Property PredMort() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.PredMort), Single())
        End Get

        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.PredMort, newValue)
            End If
        End Set
    End Property

    ''' <summary> PB(iGroup) </summary>
    Public Property MortCoPB() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MortCoPB))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MortCoPB, newValue)
            End If
        End Set
    End Property

    ''' <summary> Catch(i) / B(i) </summary>
    Public Property MortCoFishRate() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MortCoFishRate))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MortCoFishRate, newValue)
            End If
        End Set
    End Property

    ''' <summary> M2(i) </summary>
    Public Property MortCoPredMort() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MortCoPredMort))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MortCoPredMort, newValue)
            End If
        End Set

    End Property

    Public Property MortCoOtherMort() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MortCoOtherMort))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MortCoOtherMort, newValue)
            End If
        End Set

    End Property

    ''' <summary> BA(i) / B(i) </summary>
    Public Property MortCoBioAcumRate() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MortCoBioAcumRate))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MortCoBioAcumRate, newValue)
            End If
        End Set

    End Property

    ''' <summary> (Emigration(i) - Immig(i)) / B(i) </summary>
    Public Property MortCoNetMig() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MortCoNetMig))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MortCoNetMig, newValue)
            End If
        End Set

    End Property

    Public Property Consumption() As Single()

        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.Consumption), Single())
        End Get

        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Consumption, newValue)
            End If
        End Set

    End Property

    Public Property Consumption(iGroup As Integer) As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Consumption, iGroup))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Consumption, newValue, iGroup)
            End If
        End Set
    End Property

    Public Property ImportedConsumption() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.ImportedConsumption))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.ImportedConsumption, newValue)
            End If
        End Set

    End Property

    Public Property NetMigration() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.NetMigration))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.NetMigration, newValue)
            End If
        End Set
    End Property

    Public Property FlowToDet() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.FlowToDet))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.FlowToDet, newValue)
            End If
        End Set
    End Property

    Public Property NetEfficiency() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.NetEfficiency))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.NetEfficiency, newValue)
            End If
        End Set
    End Property

    Public Property OmnivoryIndex() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.OmnivoryIndex))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.OmnivoryIndex, newValue)
            End If
        End Set
    End Property

    Public Property Respiration() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Respiration))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Respiration, newValue)
            End If
        End Set
    End Property

    Public Property Assimilation() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Assimilation))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Assimilation, newValue)
            End If
        End Set
    End Property

    Public Property RespAssim() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.RespAssim))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.RespAssim, newValue)
            End If
        End Set
    End Property

    Public Property ProdResp() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.ProdResp))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.ProdResp, newValue)
            End If
        End Set
    End Property

    Public Property RespBiom() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.RespBiom))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.RespBiom, newValue)
            End If
        End Set
    End Property

    Public Property SearchRate(iPred As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.SearchRate, iPred))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.SearchRate, newValue, iPred)
            End If
        End Set
    End Property

    Public Property SearchRate() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.SearchRate), Single())
        End Get
        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.SearchRate, newValue)
            End If
        End Set
    End Property

    Public Property Hlap(iPred As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Hlap, iPred))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Hlap, newValue, iPred)
            End If
        End Set
    End Property

    Public Property Hlap() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.Hlap), Single())
        End Get
        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Hlap, newValue)
            End If
        End Set
    End Property

    Public Property Plap(iPred As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Plap, iPred))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Plap, newValue, iPred)
            End If
        End Set
    End Property

    Public Property Plap() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.Plap), Single())
        End Get
        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Plap, newValue)
            End If
        End Set
    End Property

    Public Property Alpha(iPred As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Alpha, iPred))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Alpha, newValue, iPred)
            End If
        End Set
    End Property

    ''' <summary>
    '''  Fishing mort / total mort 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>output.FishMortPerTotMort = output.MortCoFishRate / (m0 + m_EcoPathData.M2(iGroup) + output.MortCoFishRate) 'F/Z</remarks>
    Public Property FishMortPerTotMort() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.FishMortTotMort))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.FishMortTotMort, newValue)
            End If
        End Set
    End Property


    ''' <summary>
    '''  Proportion of mortality due to predation and other mort 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>output.NatMortPerTotMort = CSng(1.0 - output.FishMortPerTotMort) 'M/Z</remarks>
    Public Property NatMortPerTotMort() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.NatMortPerTotMort))
        End Get
        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.NatMortPerTotMort, newValue)
            End If
        End Set
    End Property

    Public Property Alpha() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.Alpha), Single())
        End Get
        Set(newValue As Single())
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.Alpha, newValue)
            End If
        End Set
    End Property

#End Region

#Region "Status Flags Via dot (.) operator"

    Public Property AreaStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.HabitatArea)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.HabitatArea, value)
        End Set

    End Property

    Public Property BiomassAccumStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.BioAccumOutput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.BioAccumOutput, value)
        End Set

    End Property

    Public Property BiomassStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.Biomass)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Biomass, value)
        End Set

    End Property

    Public Property BiomassAreaStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.BiomassAreaOutput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.BiomassAreaOutput, value)
        End Set

    End Property

    Public Property EEOutputStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.EEOutput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EEOutput, value)
        End Set

    End Property

    Public Property GEOutputStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.GEOutput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.GEOutput, value)
        End Set

    End Property

    Public Property GSStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.GS)

        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.GS, value)
        End Set

    End Property

    Public Property ImportedConsumptionStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.ImportedConsumption)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.ImportedConsumption, value)
        End Set

    End Property

    Public Property MortCoBioAcumRateStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.MortCoBioAcumRate)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MortCoBioAcumRate, value)
        End Set

    End Property

    Public Property MortCoFishRateStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.MortCoFishRate)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MortCoFishRate, value)
        End Set


    End Property

    Public Property MortCoNetMigStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.MortCoNetMig)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MortCoNetMig, value)
        End Set

    End Property

    Public Property MortCoOtherMortStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.MortCoOtherMort)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MortCoOtherMort, value)
        End Set

    End Property

    Public Property MostCoPBStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.MortCoPB)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MortCoPB, value)
        End Set

    End Property

    Public Property MostCoPredMortStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.MortCoPredMort)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MortCoPredMort, value)
        End Set

    End Property

    Public Property PBStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.PBOutput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.PBInput, value)
        End Set

    End Property

    <Obsolete("Use PBStatus instead")>
    Public Property PBOutputStatus() As eStatusFlags

        Get
            Return Me.PBStatus
        End Get

        Friend Set(value As eStatusFlags)
            Me.PBStatus = value
        End Set

    End Property

    Public Property QBStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.QBOutput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.QBOutput, value)
        End Set

    End Property

    Public Property TTLXStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.TTLX)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.TTLX, value)
        End Set

    End Property

    Public Property PredMortStatus(iPred As Integer) As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.PredMort)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.PredMort, value)
        End Set

    End Property

    Public Property NetMigrationStatus(iGroup As Integer) As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.NetMigration)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NetMigration, value)
        End Set

    End Property

    Public Property FlowToDetStatus(iGroup As Integer) As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.FlowToDet)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.FlowToDet, value)
        End Set

    End Property

    Public Property NetEfficiencyStatus(iGroup As Integer) As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.NetEfficiency)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NetEfficiency, value)
        End Set

    End Property

    Public Property OmnivoryIndexStatus(iGroup As Integer) As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.OmnivoryIndex)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.OmnivoryIndex, value)
        End Set

    End Property

    Public Property RespirationStatus(iGroup As Integer) As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.Respiration)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Respiration, value)
        End Set

    End Property

    Public Property AssimilationStatus(iGroup As Integer) As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.Assimilation)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Assimilation, value)
        End Set

    End Property

    Public Property SearchRateStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.SearchRate)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.SearchRate, value)
        End Set

    End Property

    Public Property SearchRateStatus(iPred As Integer) As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.SearchRate, iPred)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.SearchRate, value, iPred)
        End Set

    End Property

#End Region

End Class


