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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' NEW CONCEPTIONAL ORGANIZATION FOR VARIABLE METADATA
''' Advantages: 
'''  - Central location for variable metadata: value type, allowed value range, null value, and units
'''  - Define all variables of EwE in one location
'''  - Accessible by core, output writers, UIs, plug-ins, etc
''' Disadvantages:
'''  - Disconnects variable definition in IO objects and the metadata of that variable. Harder to debug
''' </summary>
Public Class cVariableMetadataFactory

    Private Shared s_inst As cVariableMetadataFactory = Nothing

    Public Shared Function GetInstance() As cVariableMetadataFactory
        If (s_inst Is Nothing) Then
            s_inst = New cVariableMetadataFactory()
        End If
        Return s_inst
    End Function

    Private m_dtMetadata As Dictionary(Of eVarNameFlags, cVariableMetaData)

    Private Sub New()
        Me.m_dtMetadata = New Dictionary(Of eVarNameFlags, cVariableMetaData)
        Me.Init()
    End Sub

    Private Sub Init()

        ' -- Predefine units --
        Dim unitsNotAssesed As eUnitType() = New eUnitType() {}
        Dim unitsNone As eUnitType() = New eUnitType() {}
        Dim unitsProp As eUnitType() = New eUnitType() {eUnitType.Proportion}
        Dim unitPropTime As eUnitType() = New eUnitType() {eUnitType.Proportion, eUnitType.Time}
        Dim unitsCurr As eUnitType() = New eUnitType() {eUnitType.Currency}
        Dim unitsCurrTime As eUnitType() = New eUnitType() {eUnitType.Currency, eUnitType.Time}
        Dim unitsCurrArea As eUnitType() = New eUnitType() {eUnitType.Currency, eUnitType.Area}
        Dim unitsCurrAreaTime As eUnitType() = New eUnitType() {eUnitType.Currency, eUnitType.Area, eUnitType.Time}

        ' -- Ecopath group inputs --
        Me.Metadata(eVarNameFlags.Area) = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsProp)
        Me.Metadata(eVarNameFlags.BioAccumInput) = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.Biomass) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed) ' When value missing set this input to CORE_NULL
        Me.Metadata(eVarNameFlags.BiomassAreaInput) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsCurr)
        Me.Metadata(eVarNameFlags.DetImp) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsCurrTime)
        Me.Metadata(eVarNameFlags.EEInput) = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed) ' When value missing set this input to CORE_NULL
        Me.Metadata(eVarNameFlags.OtherMortInput) = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed) ' When value missing set this input to CORE_NULL
        Me.Metadata(eVarNameFlags.Emig) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.EmigRate) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.GEInput) = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), _
                                                        cCore.NULL_VALUE) ' When value missing set this to CORE_NULL
        Me.Metadata(eVarNameFlags.GS) = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.PBInput) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), _
                                                        cCore.NULL_VALUE, _
                                                        unitPropTime) ' When value missing set this to CORE_NULL
        Me.Metadata(eVarNameFlags.Immig) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.QBInput) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), _
                                                        cCore.NULL_VALUE, _
                                                        unitPropTime) ' When value missing set this to CORE_NULL
        Me.Metadata(eVarNameFlags.BioAccumRate) = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.ImpDiet) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.PoolColor) = New cVariableMetaData(-4294967295, 4294967295, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.NonMarketValue) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.DietComp) = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        , _
                                                        unitsProp)
        Me.Metadata(eVarNameFlags.DetritusFate) = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        , _
                                                        unitsProp)
        Me.Metadata(eVarNameFlags.VBK) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        , _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.TCatchInput) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.AinLWInput) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.BinLWInput) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.LooInput) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.WinfInput) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.t0Input) = New cVariableMetaData(-1, 0, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.TmaxInput) = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                                                        cCore.NULL_VALUE, _
                                                        unitsNotAssesed)
        Me.Metadata(eVarNameFlags.IsFished) = New cVariableMetaData()

        ' -- Group outputs --
        Me.Metadata(eVarNameFlags.EEOutput) = Me.Default(eValueTypes.Sng, unitsNone)
        Me.Metadata(eVarNameFlags.PBOutput) = Me.Default(eValueTypes.Sng, unitPropTime)
        Me.Metadata(eVarNameFlags.QBOutput) = Me.Default(eValueTypes.Sng, unitPropTime)
        Me.Metadata(eVarNameFlags.GEOutput) = Me.Default(eValueTypes.Sng, unitPropTime)
        Me.Metadata(eVarNameFlags.BiomassAreaOutput) = Me.Default(eValueTypes.Sng, unitsCurr)
        Me.Metadata(eVarNameFlags.BioAccumRatePerYear) = Me.Default(eValueTypes.Sng, unitPropTime)
        Me.Metadata(eVarNameFlags.TTLX) = Me.Default(eValueTypes.Sng, unitsNone)
        Me.Metadata(eVarNameFlags.ImportedConsumption) = Me.Default(eValueTypes.Sng, unitsCurr)
        Me.Metadata(eVarNameFlags.MortCoPB) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.MortCoFishRate) = Me.Default(eValueTypes.Sng, unitPropTime)
        Me.Metadata(eVarNameFlags.MortCoPredMort) = Me.Default(eValueTypes.Sng, unitPropTime)
        Me.Metadata(eVarNameFlags.MortCoBioAcumRate) = Me.Default(eValueTypes.Sng, unitPropTime)
        Me.Metadata(eVarNameFlags.MortCoNetMig) = Me.Default(eValueTypes.Sng, unitPropTime)
        Me.Metadata(eVarNameFlags.MortCoOtherMort) = Me.Default(eValueTypes.Sng, unitPropTime)
        Me.Metadata(eVarNameFlags.NetMigration) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.FlowToDet) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.NetEfficiency) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.OmnivoryIndex) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.Respiration) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.Assimilation) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.ProdResp) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.RespAssim) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.RespBiom) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.BiomassAvgSzWt) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.BiomassSzWt) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.TCatchOutput) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.AinLWOutput) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.BinLWOutput) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.LooOutput) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.WinfOutput) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.t0Output) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.TmaxOutput) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.FishMortTotMort) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.NatMortPerTotMort) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.Consumption) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.PredMort) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.SearchRate) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.Hlap) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.Plap) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.Alpha) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.EcopathWeight) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.EcopathNumber) = Me.Default(eValueTypes.Sng, unitsNone)
        Me.Metadata(eVarNameFlags.EcopathBiomass) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.LorenzenMortality) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
        Me.Metadata(eVarNameFlags.PSD) = Me.Default(eValueTypes.Sng, unitsNotAssesed)
    End Sub

    Public Property Metadata(vn As eVarNameFlags) As cVariableMetaData
        Get
            If Me.m_dtMetadata.ContainsKey(vn) Then Return Me.m_dtMetadata(vn)
            Return Nothing
        End Get
        Friend Set(value As cVariableMetaData)
            Debug.Assert(Not Me.m_dtMetadata.ContainsKey(vn), "Metadata already defined")
            Me.m_dtMetadata(vn) = value
        End Set
    End Property

#Region " Internals "

    Private Function [Default](vartype As eValueTypes, units As eUnitType()) As cVariableMetaData

        Dim md As cVariableMetaData = Nothing

        Select Case vartype
            Case eValueTypes.Bool, eValueTypes.BoolArray
                md = New cVariableMetaData()
            Case eValueTypes.Int, eValueTypes.IntArray
                md = New cVariableMetaData(Integer.MinValue, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), units:=units)
            Case eValueTypes.Sng, eValueTypes.SingleArray
                md = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), units:=units)
            Case eValueTypes.Str
                md = New cVariableMetaData(32)
        End Select

        Return md

    End Function

#End Region ' Internals 

End Class

''' ---------------------------------------------------------------------------
''' <summary>
''' Meta data for a variable, describing its value range and default value.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cVariableMetaData

    ' -- Variables for numeric values --

    ''' <summary>Minimum value for a variable.</summary>
    Private m_min As Single = 0
    ''' <summary>Minimum value operator.</summary>
    Private m_operatorMin As cOperatorBase = Nothing
    ''' <summary>Maximum value for a variable.</summary>
    Private m_max As Single = 0
    ''' <summary>Maximum value operator.</summary>
    Private m_operatorMax As cOperatorBase = Nothing
    ''' <summary>Default value for variable when a value is missing or in error.</summary>
    Private m_nullvalue As Object = Nothing
    ''' <summary>Length of array items</summary>
    Private m_iArrayLength As Integer = cCore.NULL_VALUE

    ' -- Variables for string values --
    ''' <summary>Allowed length of string values.</summary>
    Private m_iStringLength As Integer = 0

    Private m_vartype As eValueTypes

    ' -- Variable units --
    Private m_units As eUnitType()

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor use boolean values.
    ''' </summary>
    ''' <param name="bValueDefault">Default value to assign to variable when in error.</param>
    ''' <remarks>Booleans do not have min or max values.</remarks>
    ''' -----------------------------------------------------------------------
    Sub New(Optional ByVal bValueDefault As Boolean = False)
        Me.m_nullvalue = bValueDefault
        Me.m_units = New eUnitType() {}
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constuctor for string values.
    ''' </summary>
    ''' <param name="iLength">The max allowed string length.</param>
    ''' <param name="strValueDefault">
    ''' Default value to assign to variable when in error.</param>
    ''' <remarks>Strings do not have min or max values.</remarks>
    ''' -----------------------------------------------------------------------
    Sub New(ByVal iLength As Integer, Optional ByVal strValueDefault As String = "")
        Me.m_iStringLength = iLength
        Me.m_nullvalue = strValueDefault
        Me.m_units = New eUnitType() {}
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for numeric values.
    ''' </summary>
    ''' <param name="sMin">Lowest value a variable can contain.</param>
    ''' <param name="sMax">Highest value a variable can contain.</param>
    ''' <param name="operatorMin"><see cref="cOperatorBase">Operator</see>
    ''' stating how the <paramref name="sMin">minimum value</paramref> is included
    ''' in the variable value range.</param>
    ''' <param name="operatorMax"><see cref="cOperatorBase">Operator</see>
    ''' stating how the <paramref name="sMax">maximum value</paramref> is included
    ''' in the variable value range.</param>
    ''' <param name="sValueDefault">Default value to assign to variable when in error.</param>
    ''' <param name="units">Units of the value.</param>
    ''' -----------------------------------------------------------------------
    Sub New(ByVal sMin As Single, ByVal sMax As Single, _
            ByVal operatorMin As cOperatorBase, ByVal operatorMax As cOperatorBase, _
            Optional ByVal sValueDefault As Single = 0.0!, _
            Optional ByVal units As eUnitType() = Nothing)
        Me.m_min = sMin
        Me.m_max = sMax
        Me.m_operatorMin = operatorMin
        Me.m_operatorMax = operatorMax
        Me.m_nullvalue = sValueDefault
        Me.m_units = units
    End Sub

#End Region ' Constructors

#Region " Operators "

    Friend Sub Attach(ByVal value As cValue)

        Me.m_vartype = value.varType

        Select Case Me.m_vartype
            Case eValueTypes.Bool, eValueTypes.Int, eValueTypes.Sng
                Debug.Assert(value.Length = 0, "Variable malformed")
            Case eValueTypes.BoolArray, eValueTypes.IntArray, eValueTypes.SingleArray
                Me.m_iArrayLength = value.Length
            Case eValueTypes.Str
                ' NOP
            Case Else
                Throw New NotImplementedException()
        End Select

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the minimum value <see cref="cOperatorBase">operator</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MinOperator() As cOperatorBase
        Get
            Return Me.m_operatorMin
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the maximum value <see cref="cOperatorBase">operator</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MaxOperator() As cOperatorBase
        Get
            Return Me.m_operatorMax
        End Get
    End Property

#End Region ' Operators

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the minimum value for a variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Min() As Single
        Get
            Return Me.m_min
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the maximum value for a variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Max() As Single
        Get
            Return Me.m_max
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the default value for a variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NullValue() As Object
        Get
            Return Me.m_nullvalue
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the maximum allowed string length for variables.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Length() As Integer
        Get
            Return Me.m_iStringLength
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="eValueTypes">value type</see> of the variable 
    ''' that this metadata represents.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property VarType As eValueTypes
        Get
            Return Me.m_vartype
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the maximum allowed index for the variable, or 0 if the variable
    ''' is not an array.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ArrayLength As Integer
        Get
            Return Me.m_iArrayLength
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the array of units for this variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Units As eUnitType()
        Get
            Return Me.m_units
        End Get
    End Property

#End Region ' Properties

End Class

