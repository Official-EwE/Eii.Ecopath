' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' <summary>
''' Multi-stanza group
''' </summary>
''' <remarks>This class acts as a buffer for the data. The core data is updated expilcitly by a user not implicitly by the core.
'''  The user of this class is responsible for calculating the stanza parameters and saving any changes.
''' </remarks>
Public Class cStanzaGroup
    Inherits cCoreInputOutputBase

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cStanzaGroup)()

#Region "Constuction"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="core"></param>
    ''' <param name="DBID"></param>
    ''' <param name="nStanzas"></param>
    ''' <remarks></remarks>
    Public Sub New(ByRef core As cCore, DBID As Integer, nStanzas As Integer, iStanza As Integer)
        MyBase.New(core)

        Dim val As cValue = Nothing

        Me.DBID = DBID
        Me.Index = iStanza
        Me.m_core = core
        Me.m_dataType = eDataTypes.Stanza
        Me.m_coreComponent = eCoreComponentType.Ecopath

        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        ' EwE5 variables mirrored here:
        ' ReDim Bio(Stanza)             ' Biomass values for groups within a stanza cfg
        ' ReDim Bat(Stanza) As Single   ' Output variable BaB * Bio(iLifeStage)
        ' ReDim Z(Stanza)               ' Mortality
        ' ReDim cb(Stanza)
        ' ReDim FirstAge(Stanza)
        ' ReDim SecondAge(Stanza) 'last month of age by spp, stanza (set in ecopath)
        ' ReDim LocalName(Stanza)
        ' LocalName(0) = StanzaName(CurrentStanza, 0)
        ' ReDim Remark(Stanza)
        ' ReDim EcopathGroup(Stanza)
        ' vbgfK = vbK(GrpNo)

        ''vbgfK
        'meta = New cVariableMetadata(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
        'val = New cValue(core, New Single, eVarNameFlags.StanzaVBGF, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.StanzaVBGF))
        'm_values.Add(val.varName, val)

        'LeadingBiomass
        val = New cValue(core, New Integer, eVarNameFlags.LeadingBiomass, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        'LeadingQB
        val = New cValue(core, New Integer, eVarNameFlags.LeadingCB, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' RecruitmentPower
        val = New cValue(core, New Single, eVarNameFlags.RecPowerSplit, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' RecruitmentStanza
        val = New cValue(core, New Integer, eVarNameFlags.RecruitmentStanza, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' Relative biomass accumulation rate (BaB)
        val = New cValue(core, New Single, eVarNameFlags.BABsplit, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' Weight maturity over Weight infancy (WmatWinf)
        val = New cValue(core, New Single, eVarNameFlags.WmatWinf, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' Forcing function for hatchery stocking
        val = New cValue(core, New Integer, eVarNameFlags.HatchCode, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' Fixed fecundity
        val = New cValue(core, New Boolean, eVarNameFlags.FixedFecundity, eStatusFlags.Null, eValueTypes.Bool)
        Me.m_values.Add(val.varName, val)

        ' Recruit where spawned (Ecospace)
        val = New cValue(core, New Boolean, eVarNameFlags.EggAtSpawn, eStatusFlags.Null, eValueTypes.Bool)
        Me.m_values.Add(val.varName, val)

        ' IsFished
        val = New cValue(core, New Boolean, eVarNameFlags.IsFished, eStatusFlags.OK, eValueTypes.Bool)
        val.AffectsRunState = False

        Me.m_values.Add(val.varName, val)

        ' Weight maturity over Weight infancy (WmatWinf)
        val = New cValue(core, New Single, eVarNameFlags.MultiStanzaAge0Numbers, eStatusFlags.Null, eValueTypes.Sng)
        val.Stored = False
        Me.m_values.Add(val.varName, val)

        ' === Array variables for groups within a stanza config ===

        ' Bat
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.Bat, eStatusFlags.Null, eCoreCounterTypes.nMaxStanza)
        Me.m_values.Add(val.varName, val)

        ' Ages
        val = New cValueArray(core, eValueTypes.IntArray, eVarNameFlags.StartAge, eStatusFlags.Null, eCoreCounterTypes.nMaxStanza)
        Me.m_values.Add(val.varName, val)

        'number at age
        val = New cValueArrayIndexed(core, eValueTypes.SingleArray, eVarNameFlags.StanzaNumberAtAge, eStatusFlags.Null, eCoreCounterTypes.nMaxStanzaAge, Me.Index, eDataTypes.Stanza)
        Me.m_values.Add(val.varName, val)

        'weight at age
        val = New cValueArrayIndexed(core, eValueTypes.SingleArray, eVarNameFlags.StanzaWeightAtAge, eStatusFlags.Null, eCoreCounterTypes.nMaxStanzaAge, Me.Index, eDataTypes.Stanza)
        Me.m_values.Add(val.varName, val)

        'biomass at age
        val = New cValueArrayIndexed(core, eValueTypes.SingleArray, eVarNameFlags.StanzaBiomassAtAge, eStatusFlags.Null, eCoreCounterTypes.nMaxStanzaAge, Me.Index, eDataTypes.Stanza)
        Me.m_values.Add(val.varName, val)

        'iGroup dimensioned by nStanza(iLifeStage)
        val = New cValueArrayIndexed(core, eValueTypes.SingleArray, eVarNameFlags.StanzaGroup, eStatusFlags.Null, eCoreCounterTypes.nStanzasForStanzaGroup, Me.Index, eDataTypes.Stanza)
        Me.m_values.Add(val.varName, val)

        'Bio by nStanza(iLifeStage)
        val = New cValueArrayIndexed(core, eValueTypes.SingleArray, eVarNameFlags.StanzaBiomass, eStatusFlags.Null, eCoreCounterTypes.nStanzasForStanzaGroup, Me.Index, eDataTypes.Stanza)
        Me.m_values.Add(val.varName, val)

        'CB by nStanza(iLifeStage)
        val = New cValueArrayIndexed(core, eValueTypes.SingleArray, eVarNameFlags.StanzaCB, eStatusFlags.Null, eCoreCounterTypes.nStanzasForStanzaGroup, Me.Index, eDataTypes.Stanza)
        Me.m_values.Add(val.varName, val)

        'Z Mort by nStanza(iLifeStage)
        val = New cValueArrayIndexed(core, eValueTypes.SingleArray, eVarNameFlags.StanzaMortaility, eStatusFlags.Null, eCoreCounterTypes.nStanzasForStanzaGroup, Me.Index, eDataTypes.Stanza)
        Me.m_values.Add(val.varName, val)

        ' Spawming
        val = New cValueArrayIndexed(core, eValueTypes.SingleArray, eVarNameFlags.SpawnProp, eStatusFlags.Null, eCoreCounterTypes.nStanzasForStanzaGroup, Me.Index, eDataTypes.Stanza)
        Me.m_values.Add(val.varName, val)

        Me.AllowValidation = False ' Why?

    End Sub

#End Region

#Region "Public methods unique to this class for calculation of stanza parameters"

    ''' <summary>
    ''' Calculate the Stanza parameters for this stanza group
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This does not save changes to the core data it only calculates the stanza parameters for this object. 
    ''' Saving is handled by Apply().</remarks>
    Public Function CalculateParameters() As Boolean

        Try
            Return Me.m_core.CalculateStanza(Me)
        Catch ex As Exception
            m_logger.LogError(ex, "Error calculating stanza parameters for stanza group {StanzaGroupIndex}", Me.Index)
            Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STANZA_CALCULATEPARMS_DATAERROR, ex.Message),
                    eMessageType.ErrorEncountered, eCoreComponentType.Core, eMessageImportance.Critical))
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Apply Stanza Calculation
    ''' </summary>
    ''' <remarks>If edits have been made then this will implicitly run the stanza calculations.</remarks>
    Public Function Apply() As Boolean

        Try
            'If a user has changed a parameter then called CalculateParameters() before trying to save the data
            'so that all the data is up to date
            If Me.isDirty Then
                Me.CalculateParameters()
                Me.m_core.onChanged(Me)
                Me.isDirty = False
            End If
            Return True

        Catch ex As Exception
            m_logger.LogError(ex, "Error applying stanza parameters for stanza group {StanzaGroupIndex}", Me.Index)
            Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STANZA_APPLY_DATAERROR, ex.Message),
                    eMessageType.ErrorEncountered, eCoreComponentType.Core, eMessageImportance.Critical))
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Cancel edits made by a user by reloading the underlying Core data and re-calculating the stanza parameters.
    ''' </summary>
    Public Function Cancel() As Boolean

        Try
            Me.m_core.LoadStanza(Me)
            Return Me.CalculateParameters()
        Catch ex As Exception
            m_logger.LogError(ex, "Error cancelling stanza parameters for stanza group {StanzaGroupIndex}", Me.Index)
            Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STANZA_CANCEL_DATAERROR, ex.Message),
                eMessageType.ErrorEncountered, eCoreComponentType.Core, eMessageImportance.Critical))
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Is it ok to run stanza calculation, cEcosim.CalculateStanzaParameters(), on this object.  Have the leading stanza parameters been set.
    ''' </summary>
    ''' <param name="msg">Message to decorate with missing variable statuses, if provided</param>
    ''' <returns>True if it is Ok to calculate the stanza parameters on the Stanza Group</returns>
    ''' <remarks>When a new stanza group is first created its leading parameters (oldest group) and z will be NULL_VALUE (-9999). 
    ''' The leading B and CB and Mortality need to be set by the user before calculateParameter can be called. </remarks>
    Public ReadOnly Property OkToCalculate(Optional msg As cMessage = Nothing) As Boolean

        Get
            Dim bOk As Boolean = True
            Dim vs As cVariableStatus = Nothing
            Dim grp As cEcoPathGroupInput = Nothing

            'first Z mortality
            For ist As Integer = 1 To Me.nLifeStages
                If Me.Mortality(ist) < 0 Then
                    If (msg Is Nothing) Then Return False

                    bOk = False
                    grp = Me.m_core.EcopathGroupInputs(Me.iGroups(ist))
                    vs = New cVariableStatus(grp, eStatusFlags.MissingParameter,
                                             cStringUtils.Localize(My.Resources.CoreMessages.STANZA_MORT_MISSING, grp.Name),
                                             eVarNameFlags.StanzaMortaility,
                                             Me.m_dataType, Me.m_coreComponent, grp.Index, cCore.NULL_VALUE)
                    msg.AddVariable(vs)
                End If
            Next

            'leading b and cb
            If Me.Biomass(Me.LeadingB) < 0 Or Me.CB(Me.LeadingCB) < 0 Then
                If (msg Is Nothing) Then Return bOk

                bOk = False
                vs = New cVariableStatus(Me, eStatusFlags.MissingParameter,
                                         cStringUtils.Localize(My.Resources.CoreMessages.STANZA_LEADING_MISSING, Me.Name),
                                         eVarNameFlags.LeadingBiomass,
                                         Me.m_dataType, Me.m_coreComponent, Me.Index, cCore.NULL_VALUE)
                msg.AddVariable(vs)
            End If

            Return bOk
        End Get

    End Property

    ''' <summary>
    ''' Overloaded to set the isDirty flag
    ''' </summary>
    ''' <param name="VarName"></param>
    ''' <param name="newValue"></param>
    ''' <param name="iSecondaryIndex"></param>
    ''' <returns></returns>
    Public Overrides Function SetVariable(VarName As eVarNameFlags, newValue As Object, Optional iSecondaryIndex As Integer = -9999, Optional iThirdIndex As Integer = -9999) As Boolean
        Dim bSucces As Boolean = MyBase.SetVariable(VarName, newValue, iSecondaryIndex)
        Me.isDirty = Me.isDirty Or bSucces
        Return bSucces
    End Function

    Public Property isDirty() As Boolean = False

#End Region

#Region "Variables by dot (.) operator"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the zero-based index of the life stage that determines B for this stanza.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property LeadingB() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.LeadingBiomass))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.LeadingBiomass, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the zero-based index of the life stage that determines CB for this stanza.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property LeadingCB() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.LeadingCB))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.LeadingCB, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eVarNameFlags.RecPowerSplit">recruitment power</see>
    ''' for this stanza configuration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property RecruitmentPower() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.RecPowerSplit))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.RecPowerSplit, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eVarNameFlags.RecruitmentStanza"/>
    ''' for this stanza configuration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property RecruitmentStanza() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.RecruitmentStanza))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.RecruitmentStanza, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eVarNameFlags.BABsplit">relative biomass accumulation rate</see>
    ''' for this stanza configuration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BiomassAccumulationRate() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.BABsplit))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.BABsplit, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eVarNameFlags.WmatWinf">weight at maturity over weight at infancy ratio</see>
    ''' for this stanza configuration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property WmatWinf() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.WmatWinf))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.WmatWinf, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eVarNameFlags.HatchCode">hatchery stocking forcing function number</see>
    ''' for this stanza configuration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property HatchCode() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.HatchCode))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.HatchCode, value)
        End Set
    End Property

    Public Property FixedFecundity() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.FixedFecundity))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.FixedFecundity, value)
        End Set
    End Property

    Public Property EggAtSpawn() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EggAtSpawn))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EggAtSpawn, value)
        End Set
    End Property

    '### ARRAY VARIABLES ###

    Public Property StartAge(iLifeStage As Integer) As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.StartAge, iLifeStage))
        End Get
        Set(iValue As Integer)
            Me.SetVariable(eVarNameFlags.StartAge, iValue, iLifeStage)
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

    ''' -----------------------------------------------------------------------
    Public Property Age0Numbers() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MultiStanzaAge0Numbers))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.MultiStanzaAge0Numbers, value)
        End Set
    End Property

#Region "Variable by Stanza iGroup & NStanza"

    ''' <summary>
    ''' Get the number of life stages in this Multi Stanza grouping. 
    ''' </summary>
    Public ReadOnly Property nLifeStages As Integer
        Get
            Return Me.m_core.GetCoreCounter(eCoreCounterTypes.nStanzasForStanzaGroup, Me.Index)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cCoreGroupBase.Index">index</see> of a life stage 
    ''' in this multi-stanza grouping. Groups are stored in a one-based array.
    ''' </summary>
    ''' <param name="iLifeStage">The one-based index of a group within this Stanza group
    ''' to obtain the group index for.</param>
    ''' <returns>Index of a group that belongs to this multi-stanza grouping.</returns>
    ''' <remarks>
    ''' <para>The returned index identifies the Indexes of group that belong
    ''' to this multi-stanza grouping.</para>
    ''' <code>
    ''' Dim stanzaGrp As cStanzaGroup = Nothing
    ''' Dim input As cEcoPathGroupInputs = Nothing
    ''' 
    ''' ' Get the first stanza group. StanzaGroups are zero based
    ''' stanzaGrp = core.StanzaGroups(0)
    ''' 
    ''' ' Iterate over the groups in this stanza grouping using NStanzas and iGroup(i)
    ''' For iLifeStage As Integer = 1 To stanzaGrp.NStanzas
    '''    input = core.EcoPathGroupInputs(stanzaGrp.iGroup(iLifeStage))
    ''' Next iLifeStage
    ''' </code>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Property iGroups(iLifeStage As Integer) As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.StanzaGroup, iLifeStage))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.StanzaGroup, value, iLifeStage)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the life stage index of a <see cref="cCoreGroupBase.Index">group index</see>
    ''' within this stanza configuration.
    ''' </summary>
    ''' <param name="iGroup">The group index to find the one-based life stage index for.</param>
    ''' <returns>A one-based index of a life stage, or <see cref="cCore.NULL_VALUE"/> 
    ''' if the group was not found.</returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property iLifeStage(iGroup As Integer) As Integer
        Get
            For i As Integer = 1 To Me.nLifeStages
                If (Me.iGroups(i) = iGroup) Then
                    Return i
                End If
            Next
            Return cCore.NULL_VALUE
        End Get
    End Property

    Public Property Biomass(iLifeStage As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.StanzaBiomass, iLifeStage))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.StanzaBiomass, value, iLifeStage)
        End Set
    End Property

    Public Property Mortality(iLifeStage As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.StanzaMortaility, iLifeStage))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.StanzaMortaility, value, iLifeStage)
        End Set
    End Property

    Public Property CB(iLifeStage As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.StanzaCB, iLifeStage))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.StanzaCB, value, iLifeStage)
        End Set
    End Property

    Public Property SpawnProp(iLifeStage As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.SpawnProp, iLifeStage))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.SpawnProp, value, iLifeStage)
        End Set
    End Property

#End Region

#Region "Age arrayed variables"

    Public ReadOnly Property MaxAge() As Integer
        Get
            Return Me.m_core.GetCoreCounter(eCoreCounterTypes.nMaxStanzaAge, Me.Index)
        End Get
    End Property

    Public Property NumberAtAge(iAge As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.StanzaNumberAtAge, iAge))
        End Get
        Friend Set(Value As Single)
            Me.SetVariable(eVarNameFlags.StanzaNumberAtAge, Value, iAge)
        End Set
    End Property

    Public Property WeightAtAge(iAge As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.StanzaWeightAtAge, iAge))
        End Get
        Friend Set(Value As Single)
            Me.SetVariable(eVarNameFlags.StanzaWeightAtAge, Value, iAge)
        End Set
    End Property

    Public Property BiomassAtAge(iAge As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.StanzaBiomassAtAge, iAge))
        End Get
        Friend Set(Value As Single)
            Me.SetVariable(eVarNameFlags.StanzaBiomassAtAge, Value, iAge)
        End Set
    End Property

#End Region

#End Region 'Variables by dot (.) operator

#Region "Status by dot (.) operator"

#If 0 Then ' JS 24Mar07: probably do not need this

    Public Property WmatWinfStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.WmatWinf)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.WmatWinf, value)
        End Set

    End Property

#End If

    '### ARRAY VARIABLES ###

#End Region 'Status by dot (.) operator

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        MyBase.ResetStatusFlags() 'for name and other default status flags

        Dim i As Integer
        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue

        Dim Status As eStatusFlags = eStatusFlags.Null
        If Me.OkToCalculate Then
            Status = eStatusFlags.OK
        End If

        For Each keyvalue In Me.m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.BoolArray
                        For i = 0 To value.Length : value.Status(i) = Status : Next i
                    Case eValueTypes.Sng
                        value.Status = Status
                End Select
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue

        Return True

    End Function

#Region " Obsolete "

    ''' <summary>
    ''' Get/set the number of groups in this Multi Stanza grouping. 
    ''' </summary>
    <Obsolete("Use nLifeStages instead; NStanzas is too confusing")>
    Public Property NStanzas() As Integer
        Get
            Return Me.nLifeStages
        End Get
        Private Set(value As Integer)
            'I don't see how this can work from here
            'if the number of stanzas has changed all the data will need to be reloaded 
            Debug.Assert(False, Me.ToString & ".NStanzas() What are you trying to do here!!!!!")
        End Set
    End Property

#End Region ' Obsolete

End Class
