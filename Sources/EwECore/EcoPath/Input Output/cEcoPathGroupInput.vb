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
#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Inputs for EcoPath for a single group.
''' </summary>
''' <remarks>
''' This class wraps the inputs to EcoPath for one group into a single object.
''' </remarks>
Public Class cEcoPathGroupInput
    Inherits cCoreGroupBase

#Region " Private stuff "

    ''' <summary>
    ''' Clear the Status/message (CurrentStatus) object for this group 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ClearCurrentStatus()

        Me.m_ValidationStatus.Status = eStatusFlags.OK
        Me.m_ValidationStatus.Source = eCoreComponentType.Ecopath
        Me.m_ValidationStatus.Message = ""
        Me.m_ValidationStatus.VarName = eVarNameFlags.NotSet
        Me.m_ValidationStatus.Index = Me.Index
        Me.m_ValidationStatus.DataType = eDataTypes.EcoPathGroupInput
        Me.m_ValidationStatus.CoreDataObject = Me

    End Sub

#End Region ' stuff

#Region " Constructor and Initialization "

    Sub New(core As cCore, DBID As Integer, iIndex As Integer)
        MyBase.New(core)

        Dim val As cValue = Nothing

        Me.m_dataType = eDataTypes.EcoPathGroupInput
        Me.m_coreComponent = eCoreComponentType.Ecopath
        Me.Index = iIndex

        Me.AllowValidation = False

        Me.DBID = DBID

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'NULL VALUES values cleared by a user
        'values that are cleared by a user and then computed by Ecopath need to be set to <0 as there null value (this tells ecopath to compute the value)
        'see EwE5 frmInputData.vaInput_Change() for which values use this mechanism
        'this is handled here by the meta data object and the validator
        'the Meta data tells the validator what the min and max allowable values are
        'the validator decides what to do if a value is < min, set the value to the meta data nullValue or reject the value

        Me.ClearCurrentStatus()

        ' HabitatArea
        val = New cValue(core, New Single, eVarNameFlags.HabitatArea, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' BioAccumInput
        val = New cValue(core, New Single, eVarNameFlags.BioAccumInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' Biomass
        val = New cValue(core, New Single, eVarNameFlags.Biomass, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' BiomassAreaInput
        val = New cValue(core, New Single, eVarNameFlags.BiomassAreaInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' DetImp
        val = New cValue(core, New Single, eVarNameFlags.DetImp, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' EEInput
        val = New cValue(core, New Single, eVarNameFlags.EEInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' OtherMortInput
        val = New cValue(core, New Single, eVarNameFlags.OtherMortInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' Emig
        val = New cValue(core, New Single, eVarNameFlags.Emig, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' EmigRate
        val = New cValue(core, New Single, eVarNameFlags.EmigRate, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' GEInput
        val = New cValue(core, New Single, eVarNameFlags.GEInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' GS
        val = New cValue(core, New Single, eVarNameFlags.GS, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' PBInput
        val = New cValue(core, New Single, eVarNameFlags.PBInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' Immig
        val = New cValue(core, New Single, eVarNameFlags.Immig, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' QBInput
        val = New cValue(core, New Single, eVarNameFlags.QBInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' BioAccumRate
        val = New cValue(core, New Single, eVarNameFlags.BioAccumRate, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' ImpDiet
        val = New cValue(core, New Single, eVarNameFlags.ImpDiet, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' PoolColor
        val = New cValue(core, New Integer, eVarNameFlags.Color, eStatusFlags.Null, eValueTypes.Int)
        val.AffectsRunState = False
        Me.m_values.Add(val.varName, val)
        ' NonMarketValue
        val = New cValue(core, New Single, eVarNameFlags.NonMarketValue, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' IsFished
        val = New cValue(core, New Boolean, eVarNameFlags.IsFished, eStatusFlags.OK, eValueTypes.Bool)
        val.AffectsRunState = False
        val.Stored = False
        Me.m_values.Add(val.varName, val)

        ' -- arrays --

        ' DietComp
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.DietComp, eStatusFlags.Null, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)
        ' DetritusFate
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.DetritusFate, eStatusFlags.Null, eCoreCounterTypes.nDetritus)
        Me.m_values.Add(val.varName, val)
        ' VBK
        val = New cValue(core, New Single, eVarNameFlags.VBK, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' TCatchInput
        val = New cValue(core, New Single, eVarNameFlags.TCatchInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' AinLWInput
        val = New cValue(core, New Single, eVarNameFlags.AinLWInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' BinLWInput
        val = New cValue(core, New Single, eVarNameFlags.BinLWInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' LooInput
        val = New cValue(core, New Single, eVarNameFlags.LooInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' WinfInput
        val = New cValue(core, New Single, eVarNameFlags.WinfInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' t0Input
        val = New cValue(core, New Single, eVarNameFlags.t0Input, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' TmaxInput
        val = New cValue(core, New Single, eVarNameFlags.TmaxInput, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)
        ' GroupTaxa - dimensioned by nTaxa(iIndex)

        val = New cValueArray(core, eValueTypes.BoolArray, eVarNameFlags.IsPred, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
        val.Stored = False
        val.AffectsRunState = False
        Me.m_values.Add(val.varName, val)

        val = New cValueArray(core, eValueTypes.BoolArray, eVarNameFlags.IsPrey, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
        val.Stored = False
        val.AffectsRunState = False
        Me.m_values.Add(val.varName, val)

        ' iTaxon dimensioned by nTaxa(iIndex)
        val = New cValueArrayIndexed(core, eValueTypes.IntArray, eVarNameFlags.GroupTaxa, eStatusFlags.Null, eCoreCounterTypes.nTaxonForGroup, Me.Index, Me.DataType)
        Me.m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        MyBase.ResetStatusFlags(bForceReset)

        Me.m_core.Set_PB_QB_GE_BA_Flags(Me, False)
        Me.m_core.Set_Migration_Flags(Me, False)
        Me.m_core.Set_GS_Flags(Me, False)
        Me.m_core.Set_EE_OtherMort_Flags(Me, False)
        Me.m_core.Set_DetImp_Flags(Me, False)

        Me.m_core.Set_VBK_Flags(Me, False)
        Me.m_core.Set_Tcatch_Flags(Me, False)
        Me.m_core.Set_Tmax_Flags(Me, False)

    End Function

#End Region ' Constructor and Initialization

#Region "Variables by dot (.) operator"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.BA">biomass accumulation</see>
    ''' value for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BioAccumInput() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BioAccumInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.BioAccumInput, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.PBInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.PBInput, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.QBInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.QBInput, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.GEInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.GEInput, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.GS))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.GS, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.DetImp))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.DetImp, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.HabitatArea))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.HabitatArea, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.BH">Biomass per Area</see>
    ''' ratio for this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BiomassAreaInput() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BiomassAreaInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.BiomassAreaInput, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.EEInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EEInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the other mortality ratio for this group, defined as 1 - <see cref="EEInput"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property OtherMortInput() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.OtherMortInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.OtherMortInput, value)
        End Set

    End Property

    Public Property ImpDiet() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.ImpDiet))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.ImpDiet, value)
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
    Public Property DietComp(iPreyGroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.DietComp, iPreyGroup))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.DietComp, value, iPreyGroup)
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
            Return DirectCast(Me.GetVariable(eVarNameFlags.DietComp), Single())
        End Get

        Set(value As Single())
            Me.SetVariable(eVarNameFlags.DietComp, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcopathDataStructures.DF">Detritus fate</see> ratio
    ''' for a particular prey for this group.
    ''' </summary>
    ''' <param name="iDetritusGroup"></param>
    ''' -----------------------------------------------------------------------
    Public Property DetritusFate(iDetritusGroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.DetritusFate, iDetritusGroup))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.DetritusFate, value, iDetritusGroup)
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
            Return DirectCast(Me.GetVariable(eVarNameFlags.DetritusFate), Single())
        End Get

        Set(value As Single())
            Me.SetVariable(eVarNameFlags.DetritusFate, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.EmigRate))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EmigRate, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.BioAccumRate))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.BioAccumRate, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.Immig))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.Immig, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.Emig))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.Emig, value)
        End Set

    End Property

    Public Property VBK() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.VBK))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.VBK, value)
        End Set
    End Property

    Public Property PoolColor() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.Color))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.Color, value)
        End Set
    End Property

    Public Property NonMarketValue() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.NonMarketValue))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.NonMarketValue, value)
        End Set

    End Property

    Public Property TcatchInput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TCatchInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TCatchInput, value)
        End Set
    End Property

    Public Property AinLWInput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.AinLWInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.AinLWInput, value)
        End Set
    End Property

    Public Property BinLWInput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BinLWInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.BinLWInput, value)
        End Set
    End Property

    Public Property LooInput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.LooInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.LooInput, value)
        End Set
    End Property

    Public Property WinfInput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.WinfInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.WinfInput, value)
        End Set
    End Property

    Public Property t0Input() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.t0Input))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.t0Input, value)
        End Set
    End Property

    Public Property TmaxInput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TmaxInput))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TmaxInput, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get whether a group is being fished. This value is kept up to date by 
    ''' the core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property IsFished() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.IsFished))
        End Get

        Friend Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.IsFished, value)
        End Set
    End Property

    ''' <summary>
    ''' Get whether this group is predated on by <paramref name="iGroup">group index</paramref>.
    ''' </summary>
    ''' <param name="iGroup">Group index of the predator</param>
    Public Property IsPred(iGroup As Integer) As Boolean

        Get
            Return CBool(Me.GetVariable(eVarNameFlags.IsPred, iGroup))
        End Get

        Friend Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.IsPred, value, iGroup)
        End Set

    End Property

    ''' <summary>
    ''' Get whether this group predates on <paramref name="iGroup">group index</paramref>.
    ''' </summary>
    ''' <param name="iGroup">Group index of the prey</param>
    Public Property IsPrey(iGroup As Integer) As Boolean

        Get
            Return CBool(Me.GetVariable(eVarNameFlags.IsPrey, iGroup))
        End Get

        Friend Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.IsPrey, value, iGroup)
        End Set

    End Property

#End Region

#Region "Status by dot (.) operator"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="DietComp">DietComp value</see> of this group.
    ''' </summary>
    ''' <param name="iGroup">Prey <see cref="Index">index</see>.</param>
    ''' -----------------------------------------------------------------------
    Public Property DietCompStatus(iGroup As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.DietComp, iGroup)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.DietComp, value, iGroup)
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
    Public Property DetritusFateStatus(iDetritusGroup As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.DetritusFate, Me.Index)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.DetritusFate, value, Me.Index)
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
            Return Me.GetStatus(eVarNameFlags.HabitatArea)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.HabitatArea, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="BiomassAreaInput">BiomassArea input</see> value of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BiomassAreaStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.BiomassAreaInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.BiomassAreaInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="PBInput">PBInput value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PBStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.PBInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.PBInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="QBInput">QBInput value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property QBInputStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.QBInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.QBInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="EEInput">EEInput value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EEInputStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.EEInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EEInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="OtherMortInput">OtherMortInput value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property OtherMortInputStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.OtherMortInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.OtherMortInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="GEInput">GEInput value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GEStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.GEInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.GEInput, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="GS">GS value</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GSStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.GS)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.GS, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="DetImport">DetImport value</see> this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DetImportStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.DetImp)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.DetImp, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="BioAccumInput">BioAccum input value</see> this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BioAccumStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.BioAccumInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.BioAccumInput, value)
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
            Return Me.GetStatus(eVarNameFlags.EmigRate)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EmigRate, value)
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
            Return Me.GetStatus(eVarNameFlags.Emig)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Emig, value)
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
            Return Me.GetStatus(eVarNameFlags.Immig)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Immig, value)
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
            Return Me.GetStatus(eVarNameFlags.BioAccumRate)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.BioAccumRate, value)
        End Set

    End Property

    Public Property VBKStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.VBK)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.VBK, value)
        End Set
    End Property

    'Joeh
    Public Property AinLWInputStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.AinLWInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.AinLWInput, value)
        End Set
    End Property

    Public Property BinLWInputStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.BinLWInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.BinLWInput, value)
        End Set
    End Property

    Public Property LooInputStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.LooInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.LooInput, value)
        End Set
    End Property

    Public Property WinfInputStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.WinfInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.WinfInput, value)
        End Set
    End Property

    Public Property t0InputStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.t0Input)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.t0Input, value)
        End Set
    End Property

    Public Property TcatchInputStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.TCatchInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.TCatchInput, value)
        End Set
    End Property

    Public Property TmaxInputStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.TmaxInput)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.TmaxInput, value)
        End Set
    End Property
    'End Joeh
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eStatusFlags">status</see> of the 
    ''' <see cref="ImpDiet">imported diet</see> of this group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ImpDietStatus() As eStatusFlags

        Get
            Return Me.DietCompStatus(0)
        End Get

        Friend Set(value As eStatusFlags)
            Me.DietCompStatus(0) = value
        End Set

    End Property

    Public Property NonMarketValueStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.NonMarketValue)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NonMarketValue, value)
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
    Public Property BiomassStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Biomass)
        End Get

        Friend Set(value As eStatusFlags)
            setStatus(eVarNameFlags.Biomass, value)
        End Set

    End Property

#End If ' #0

#End Region

#Region " Taxa "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of taxa assigned to this group = either directly
    ''' via <see cref="cTaxon.Group"/>, or indirectly via <see cref="cTaxon.Stanza"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NTaxon() As Integer
        Get
            Return Me.m_core.GetCoreCounter(eCoreCounterTypes.nTaxonForGroup, Me.Index)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cCoreInputOutputBase.Index">Index</see> of a taxon
    ''' for this group. Taxa are stored in a one-based array.
    ''' </summary>
    ''' <param name="iIndex">The one-based index to obtain the taxon index for.</param>
    ''' <returns>Index of a taxon that is assigned to this group - either directly
    ''' via <see cref="cTaxon.Group"/>, or indirectly via <see cref="cTaxon.Stanza"/>.</returns>
    ''' <remarks>
    ''' <para>The returned index identifies the index of a particular taxon assigned
    ''' to this group.</para>
    ''' <code>
    ''' Dim grp As cEcoPathGroupInputs = Nothing
    ''' Dim taxon As cTaxon = Nothing
    ''' 
    ''' ' Get the first group
    ''' grp = core.EcopathGroupInputs(1)
    ''' 
    ''' ' Iterate over the taxa that are assigned to this group
    ''' For iIndex As Integer = 1 To grp.NTaxon
    '''    taxon = core.Taxon(grp.iTaxon(iIndex))
    '''    ' Do something with the taxon
    '''    ' ..
    '''    ' ..
    ''' Next iIndex
    ''' </code>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Property iTaxon(iIndex As Integer) As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.GroupTaxa, iIndex))
        End Get
        Friend Set(value As Integer)
            Me.SetVariable(eVarNameFlags.GroupTaxa, value, iIndex)
        End Set
    End Property

#End Region

End Class
