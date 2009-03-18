'==============================================================================
'
' $Log: cEcoPathGroupInput.vb,v $
' Revision 1.8  2009/03/18 13:28:54  jeroens
' Added PSDIncluded flag
'
' Revision 1.7  2009/03/03 01:42:55  joeh
' Tcatch no longer has input and output pair
'
' Revision 1.6  2009/03/03 01:16:23  joeh
' Add Set_Tcatch_Flags
' Add Set_Tmax_Flags
'
' Revision 1.5  2009/03/02 20:09:36  joeh
' VBK no longer has input and output pair
'
' Revision 1.4  2009/02/28 00:17:46  joeh
' Added PSD foundation
'
' Revision 1.3  2009/02/27 07:55:15  jeroens
' Changed vbK placement
'
' Revision 1.2  2009/01/16 18:30:15  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:17  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Inputs for the EcoPath
''' </summary>
''' <remarks>
''' This class wraps the inputs to EcoPath for one group into a single object
''' Data validation and default NULL value from Ewe-5 frmInputData.vaInput_Change
''' </remarks>
Public Class cEcoPathGroupInput
    Inherits cCoreGroupBase

    ' JS Mar-29-07: Private vars not used outside constructor: disabled
    ' Private m_nGroups As Integer
    ' Private m_nliving As Integer
    ' Private m_nDetritus As Integer

#Region "Private stuff"

    ''' <summary>
    ''' Clear the Status/message (CurrentStatus) object for this group 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ClearCurrentStatus()

        m_ValidationStatus.Status = eStatusFlags.OK
        m_ValidationStatus.Source = eCoreComponentType.EcoPath
        m_ValidationStatus.Message = ""
        m_ValidationStatus.VarName = eVarNameFlags.NotSet
        m_ValidationStatus.Index = Index
        m_ValidationStatus.DataType = eDataTypes.EcoPathGroupInput
        m_ValidationStatus.CoreDataObject = Me

    End Sub

#End Region

#Region "Constructor and Initialization"

    Sub New(ByRef core As cCore, ByVal DBID As Integer)
        MyBase.New(core)
        Dim val As cValue
        Dim meta As cVariableMetaData

        m_core = core

        ' JS Mar-29-07: Private vars not used outside constructor. No system is in place to updated these with core counter changes.
        '               To prevent confusion later on it may be better to explicitly disable this logic until needed. If ever.
        ''get the counters from the core via the CoreCounterDelegate() delegate
        'm_ngroups = m_core.getCoreCounter(eCoreCounterTypes.nGroups)
        ''jb June-09-06 added m_ndetritus to the constructor so that detritus fate could be dimensioned
        'm_nDetritus = m_core.getCoreCounter(eCoreCounterTypes.nDetritus)

        m_dataType = eDataTypes.EcoPathGroupInput
        m_coreComponent = eCoreComponentType.EcoPath

        'create and set the status object to this source and OK
        m_ValidationStatus = New cVariableStatus
        m_ValidationStatus.CoreDataObject = Me
        Me.AllowValidation = False

        Me.DBID = DBID

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'NULL VALUES values cleared by a user
        'values that are cleared by a user and then computed by Ecopath need to be set to <0 as there null value (this tells ecopath to compute the value)
        'see EwE5 frmInputData.vaInput_Change() for which values use this mechanism
        'this is handled here by the meta data object and the validator
        'the Meta data tells the validator what the min and max allowable values are
        'the validator decides what to do if a value is < min, set the value to the meta data nullValue or reject the value

        ClearCurrentStatus()

        'Area
        meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.Area, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'BioAccum
        meta = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.BioAccum, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'biomass set to NULL_VALUE when cleared
        meta = New cVariableMetaData(0, Single.MaxValue, _
                cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE) ' When value missing set this input to CORE_NULL
        val = New cValue(New Single, eVarNameFlags.Biomass, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.Biomass))
        m_values.Add(val.varName, val)

        'biomassArea  set to NULL_VALUE when cleared
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.BiomassAreaInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.BiomassAreaInput))
        m_values.Add(val.varName, val)

        'detImp Imported detritus
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.DetImp, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'EE set to NULL_VALUE when cleared
        meta = New cVariableMetaData(0, 1, _
                cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), _
                cCore.NULL_VALUE) ' When value missing set this input to CORE_NULL
        val = New cValue(New Single, eVarNameFlags.EEInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.EEInput))
        m_values.Add(val.varName, val)

        'Emig
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.Emig, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'Emig Rate
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.EmigRate, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'GE
        meta = New cVariableMetaData(0, 1, _
                cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), _
                cCore.NULL_VALUE) ' When value missing set this to CORE_NULL
        val = New cValue(New Single, eVarNameFlags.GEInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.GEInput))
        m_values.Add(val.varName, val)

        'GS
        meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.GS, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.GS))
        m_values.Add(val.varName, val)

        'PB
        meta = New cVariableMetaData(0, Single.MaxValue, _
                cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), _
                cCore.NULL_VALUE) ' When value missing set this to CORE_NULL
        val = New cValue(New Single, eVarNameFlags.PBInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.PBInput))
        m_values.Add(val.varName, val)

        'immig
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.Immig, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'QB
        meta = New cVariableMetaData(0, Single.MaxValue, _
                cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), _
                cCore.NULL_VALUE) ' When value missing set this to CORE_NULL
        val = New cValue(New Single, eVarNameFlags.QBInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.QBInput))
        m_values.Add(val.varName, val)

        'BioAccumRate
        meta = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.BioAccumRate, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'ImpDiet Imported Diet
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.ImpDiet, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'PoolColor
        meta = New cVariableMetaData(-4294967295, 4294967295, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.PoolColor, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'NonMarketValue
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.NonMarketValue, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'Array variables
        'DietComp Null values for diet comp should be zero
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.DietComp, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'detritus fate
        meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.DetritusFate, eStatusFlags.Null, eCoreCounterTypes.nDetritus, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'VBK
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.VBK, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.VBK))
        m_values.Add(val.varName, val)

        'Joeh: PSD
        'Tcatch
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.Tcatch, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.Tcatch))
        m_values.Add(val.varName, val)

        'A in LW
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.AinLWInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.AinLWInput))
        m_values.Add(val.varName, val)

        'B in LW
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.BinLWInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.BinLWInput))
        m_values.Add(val.varName, val)

        'Loo
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.LooInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.LooInput))
        m_values.Add(val.varName, val)

        'Winf
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.WinfInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.WinfInput))
        m_values.Add(val.varName, val)

        't0
        meta = New cVariableMetaData(-1, 0, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.t0Input, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.t0Input))
        m_values.Add(val.varName, val)

        'Tmax
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.TmaxInput, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.TmaxInput))
        m_values.Add(val.varName, val)

        'PSDIncluded
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.PSDIncluded, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.PSDIncluded))
        m_values.Add(val.varName, val)
        'End Joeh: PSD

        Me.AllowValidation = True

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        MyBase.ResetStatusFlags(bForceReset)

        Me.m_core.Set_PB_QB_GE_BA_Flags(Me, False)
        Me.m_core.set_BioAccumRate_Flags(Me, , False)
        Me.m_core.Set_Migration_Flags(Me, False)
        Me.m_core.Set_GS_Flags(Me, False)
        Me.m_core.Set_EE_Flags(Me, False)
        Me.m_core.Set_DetImp_Flags(Me, False)

        'Joeh
        Me.m_core.Set_VBK_Flags(Me, False)
        Me.m_core.Set_Tcatch_Flags(Me, False)
        Me.m_core.Set_Tmax_Flags(Me, False)
        'End Joeh

    End Function

#End Region

#Region "Variables by dot (.) operator"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.BA">biomass accumulation</see>
    ''' value for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BioAccum() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.BioAccum))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.BioAccum, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.PBinput">production per biomass</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PBInput() As Single
        Get
            Return CSng(getVariable(eVarNameFlags.PBInput))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.PBInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.QBinput">consuption per biomass</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property QBInput() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.QBInput))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.QBInput, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.GEinput">production per consuption</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GEInput() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.GEInput))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.GEInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.GS">unassimilation per consumption</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GS() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.GS))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.GS, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.DtImp">detritus import</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DetImport() As Single
        Get
            Return CSng(getVariable(eVarNameFlags.DetImp))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.DetImp, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.Area">Area</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Area() As Single
        Get
            Return CSng(getVariable(eVarNameFlags.Area))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.Area, value)
        End Set

    End Property

    'Public Property Biomass() As Single
    '    Get
    '        Return CSng(getVariable(eVarNameFlags.Biomass))
    '    End Get
    '    Set(ByVal value As Single)
    '        setVariable(eVarNameFlags.Biomass, value)
    '    End Set
    'End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.BH">Biomass per Area</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BiomassAreaInput() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.BiomassAreaInput))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.BiomassAreaInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.EEinput">Ecotrophic efficiency</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EEInput() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.EEInput))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.EEInput, value)
        End Set

    End Property

    Public Property ImpDiet() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.ImpDiet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.ImpDiet, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.DC">Diet composition</see> ratio
    ''' for a particular prey for this group.
    ''' </summary>
    ''' <param name="iPreyGroup">The <see cref="Index">index</see> of the prey (or group)
    ''' that makes up a percentage of this predators diet.</param>
    ''' <remarks>
    ''' <para>How to use:</para>
    ''' <para>Set the diet composition of group 1 to 50% of its diet from group 4</para>
    ''' <code>
    ''' Dim core As cCore = cCore.GetInstance()
    ''' Dim group As cEcoPathGroupInput = Nothing
    ''' 
    ''' ' Get the group
    ''' group = core.EcoPathGroupInputs(1)
    ''' ' Set the diet comp for group 4 to 50%
    ''' EcoPathGroup.DietComp(4) = .5
    ''' ' or
    ''' core.EcoPathGroupInputs(1).DietComp(4) = .5
    ''' </code>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Property DietComp(ByVal iPreyGroup As Integer) As Single
        Get
            Return CSng(getVariable(eVarNameFlags.DietComp, iPreyGroup))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.DietComp, value, iPreyGroup)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.DC">Diet composition</see>
    ''' ratio array for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DietComp() As Single()

        Get
            Return DirectCast(getVariable(eVarNameFlags.DietComp), Single())
        End Get

        Set(ByVal value As Single())
            setVariable(eVarNameFlags.DietComp, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.DF">Detritus fate</see> ratio
    ''' for a particular prey for this group.
    ''' </summary>
    ''' <param name="iDetritusGroup"></param>
    ''' -----------------------------------------------------------------------
    Public Property DetritusFate(ByVal iDetritusGroup As Integer) As Single
        Get
            Return CSng(getVariable(eVarNameFlags.DetritusFate, iDetritusGroup))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.DetritusFate, value, iDetritusGroup)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.DF">Detritus fate</see> ratio
    ''' array for a particular prey for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DetritusFate() As Single()

        Get
            Return DirectCast(getVariable(eVarNameFlags.DetritusFate), Single())
        End Get

        Set(ByVal value As Single())
            setVariable(eVarNameFlags.DetritusFate, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.Emig">emigration rate relative to biomass</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EmigRate() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.EmigRate))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.EmigRate, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.Babi">Biomass accumulation per biomass</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BioAccumRate() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.BioAccumRate))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.BioAccumRate, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.Immig">immigration</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Immigration() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.Immig))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.Immig, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.Emigration">emigration</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Emigration() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.Emig))
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.Emig, value)
        End Set

    End Property

    Public Property VBK() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.VBK))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.VBK, value)
        End Set
    End Property

    Public Property PoolColor() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.PoolColor))
        End Get
        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.PoolColor, value)
        End Set
    End Property

    Public Property NonMarketValue() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.NonMarketValue))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.NonMarketValue, value)
        End Set

    End Property

    'Joeh
    Public Property Tcatch() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Tcatch))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.Tcatch, value)
        End Set
    End Property

    Public Property AinLWInput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.AinLWInput))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.AinLWInput, value)
        End Set
    End Property

    Public Property BinLWInput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.BinLWInput))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.BinLWInput, value)
        End Set
    End Property

    Public Property LooInput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.LooInput))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.LooInput, value)
        End Set
    End Property

    Public Property WinfInput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.WinfInput))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.WinfInput, value)
        End Set
    End Property

    Public Property t0Input() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.t0Input))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.t0Input, value)
        End Set
    End Property

    Public Property TmaxInput() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.TmaxInput))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.TmaxInput, value)
        End Set
    End Property

    Public Property PSDIncluded() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.PSDIncluded))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.PSDIncluded, value)
        End Set
    End Property

    'End Joeh

#End Region

#Region "Status by dot (.) operator"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="DietComp">DietComp value</see> of this group.
    ''' </summary>
    ''' <param name="iGroup">Prey <see cref="Index">index</see>.</param>
    ''' -----------------------------------------------------------------------
    Public Property DietCompStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return getStatus(eVarNameFlags.DietComp, iGroup)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.DietComp, value, iGroup)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="DetritusFate">DestritusFate value</see> of this
    ''' group.
    ''' </summary>
    ''' <param name="iDetritusGroup">Detritus group <see cref="Index">index</see>.</param>
    ''' -----------------------------------------------------------------------
    Public Property DetritusFateStatus(ByVal iDetritusGroup As Integer) As eStatusFlags
        Get
            Return getStatus(eVarNameFlags.DetritusFate, Index)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.DetritusFate, value, Index)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="Area">Area value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property AreaStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Area)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Area, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="BiomassAreaInput">BiomassArea input</see> value of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BiomassAreaStatus() As EwECore.eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.BiomassAreaInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.BiomassAreaInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="PBInput">PBInput value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PBStatus() As EwECore.eStatusFlags

        Get
            Return getStatus(eVarNameFlags.PBInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.PBInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="QBInput">QBInput value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property QBInputStatus() As EwECore.eStatusFlags

        Get
            Return getStatus(eVarNameFlags.QBInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.QBInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="EEInput">EEInput value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EEInputStatus() As EwECore.eStatusFlags

        Get
            Return getStatus(eVarNameFlags.EEInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EEInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="GEInput">GEInput value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GEStatus() As EwECore.eStatusFlags

        Get
            Return getStatus(eVarNameFlags.GEInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.GEInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="GS">GS value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GSStatus() As EwECore.eStatusFlags

        Get
            Return getStatus(eVarNameFlags.GS)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.GS, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="DetImport">DetImport value</see> this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DetImportStatus() As EwECore.eStatusFlags

        Get
            Return getStatus(eVarNameFlags.DetImp)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.DetImp, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="BioAccum">BioAccum value</see> this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BioAccumStatus() As EwECore.eStatusFlags

        Get
            Return getStatus(eVarNameFlags.BioAccum)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.BioAccum, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="EmigRate">EmigRate value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EmigRateStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.EmigRate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EmigRate, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="Emigration">Emigration value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EmigrationStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Emig)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Emig, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="Immigration">Immigration value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ImmigrationStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Immig)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Immig, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="BioAccumRate">BioAccumRate value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BioAccumRateStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.BioAccumRate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.BioAccumRate, value)
        End Set

    End Property

    Public Property VBKStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.VBK)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.VBK, value)
        End Set
    End Property

    'Joeh
    Public Property AinLWInputStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.AinLWInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.AinLWInput, value)
        End Set
    End Property

    Public Property BinLWInputStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.BinLWInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.BinLWInput, value)
        End Set
    End Property

    Public Property LooInputStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.LooInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.LooInput, value)
        End Set
    End Property

    Public Property WinfInputStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.WinfInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.WinfInput, value)
        End Set
    End Property

    Public Property t0InputStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.t0Input)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.t0Input, value)
        End Set
    End Property

    Public Property TcatchStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.Tcatch)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Tcatch, value)
        End Set
    End Property

    Public Property TmaxInputStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.TmaxInput)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.TmaxInput, value)
        End Set
    End Property
    'End Joeh
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="ImpDiet">imported diet</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ImpDietStatus() As EwECore.eStatusFlags

        Get
            Return DietCompStatus(0)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            DietCompStatus(0) = value
        End Set

    End Property

    Public Property NonMarketValueStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.NonMarketValue)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.NonMarketValue, value)
        End Set

    End Property

#If 0 Then

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="cEcopathDataStructures.B">biomass value</see> 
    ''' of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BiomassStatus() As EwECore.eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Biomass)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            setStatus(eVarNameFlags.Biomass, value)
        End Set

    End Property

#End If ' #0

#End Region

End Class
