Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Inputs for EcoSim for a single group.
''' </summary>
''' <remarks>
''' This class wraps the inputs to EcoSim for one group into a single object.
''' </remarks>
Public Class cEcoSimGroupInput
    Inherits cCoreGroupBase

    Private m_nGroups As Integer

    ''' <summary>
    ''' Public access to set the status flags by calling each validator.
    ''' </summary>
    ''' <returns>True is successful. False otherwise</returns>
    ''' <remarks>This is the default behaviour for Input objects. An output will need to override this to provide its own implementation.</remarks>
    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value
                'Status flag for VulMult and VulRate are set in cCore.LoadEcosimGroups
                If value.varName <> eVarNameFlags.VulMult And value.varName <> eVarNameFlags.VulRate Then
                    Select Case value.varType
                        Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.PointArray, eValueTypes.BoolArray, eValueTypes.LayerArray
                            For i = 0 To value.Length
                                If bForceReset Then
                                    value.Status(i) = 0
                                Else
                                    value.setStatusFlag(i)
                                End If
                            Next i
                        Case Else
                            If bForceReset Then
                                value.Status = 0
                            Else
                                value.setStatusFlag()
                            End If
                    End Select
                End If

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue

        Me.m_core.Set_PP_Flags(Me, False)
        Return True

    End Function


#Region "Mapping of variable names"
    'mapping to underlying data structure names
    ' MaxRelPB  =  'PBmaxs max rel P/B
    ' MaxRelFeedingTime  =  ' FtimeMax
    ' FeedingTimeAdjustRate  =  'FtimeAdjust
    ' OtherMortFeedingTime  =  'MoPred
    ' PerdEffectFeedingTime  =  'RiskTime
    ' DenDepCatchability  =  'QmQo
    ' QBMaxQBio  =  'CmCo
    ' SwitchingPower  =  'SwitchPower
    ' VBGF  =  'vbK
    ' VulRate()  =  'vulnerability rates of predation for this group (prey)
    ' VulMult()  =  'vulnerability multiplier
#End Region

#Region "Constructor"


    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Try

            m_nGroups = theCore.nGroups

            m_dataType = eDataTypes.EcoSimGroupInput
            m_coreComponent = eCoreComponentType.EcoSim
            Me.AllowValidation = False
            Me.DBID = DBID

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimGroupInput, eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)

            Dim val As cValue
            Dim meta As cVariableMetaData

            'MaxRelPB
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MaxRelPB, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MaxRelPB))
            m_values.Add(val.varName, val)

            'MaxRelFeedingTime
            meta = New cVariableMetaData(0, 100, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MaxRelFeedingTime, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MaxRelFeedingTime))
            m_values.Add(val.varName, val)

            'FeedingTimeAdjRate
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.FeedingTimeAdjRate, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FeedingTimeAdjRate))
            m_values.Add(val.varName, val)

            'OtherMortFeedingTime
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.OtherMortFeedingTime, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.OtherMortFeedingTime))
            m_values.Add(val.varName, val)

            'PredEffectFeedingTime
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.PredEffectFeedingTime, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.PredEffectFeedingTime))
            m_values.Add(val.varName, val)

            'DenDepCatchability
            meta = New cVariableMetaData(1, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.DenDepCatchability, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.DenDepCatchability))
            m_values.Add(val.varName, val)

            'QBMaxQBio
            meta = New cVariableMetaData(1, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.QBMaxQBio, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.QBMaxQBio))
            m_values.Add(val.varName, val)

            'Switching Power
            meta = New cVariableMetaData(0, 2, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.SwitchingPower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.SwitchingPower))
            m_values.Add(val.varName, val)

            'Salinity Opt
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.SalinityOpt, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.SalinityOpt))
            m_values.Add(val.varName, val)

            'Salinity Spread Left 
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.SalinitySpreadLeft, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.SalinitySpreadLeft))
            m_values.Add(val.varName, val)

            'Salinity Spread Right
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.SalinitySpreadRight, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.SalinitySpreadRight))
            m_values.Add(val.varName, val)

            'Quota per species
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.QuotaSpecies, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.QuotaSpecies))
            m_values.Add(val.varName, val)
            'bBase
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.BBase, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.BBase))
            m_values.Add(val.varName, val)
            'bLim
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.BLim, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.BLim))
            m_values.Add(val.varName, val)
            'FOpt
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.Fopt, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.Fopt))
            m_values.Add(val.varName, val)

            'CVBest
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.RegCVBest, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.RegCVBest))
            m_values.Add(val.varName, val)

            'Kalman Weight
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.RegKalWt, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.RegKalWt))
            m_values.Add(val.varName, val)


            ''arrayed values
            'VulRate
            meta = New cVariableMetaData(1, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.VulRate, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.VulRate))
            m_values.Add(val.varName, val)

            'VulMult
            meta = New cVariableMetaData(1, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.VulMult, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.VulMult))
            m_values.Add(val.varName, val)

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcoSimGroupInfo.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcoSimGroupInfo. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Variable via dot(.) operator"

    Public Property DenDepCatchability() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.DenDepCatchability))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.DenDepCatchability, value)
        End Set
    End Property

    Public Property FeedingTimeAdjustRate() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.FeedingTimeAdjRate))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.FeedingTimeAdjRate, value)
        End Set
    End Property

    Public Property MaxRelFeedingTime() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MaxRelFeedingTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MaxRelFeedingTime, value)
        End Set
    End Property

    Public Property MaxRelPB() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MaxRelPB))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MaxRelPB, value)
        End Set
    End Property


    Public Property OtherMortFeedingTime() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.OtherMortFeedingTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.OtherMortFeedingTime, value)
        End Set
    End Property

    Public Property PredEffectFeedingTime() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.PredEffectFeedingTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.PredEffectFeedingTime, value)
        End Set
    End Property

    Public Property QBMaxQBio() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.QBMaxQBio))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.QBMaxQBio, value)
        End Set
    End Property

    Public Property SwitchingPower() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.SwitchingPower))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.SwitchingPower, value)
        End Set
    End Property


    Public Property SalinityOpt() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.SalinityOpt))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.SalinityOpt, value)
        End Set
    End Property

    Public Property SalinitySpreadLeft() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.SalinitySpreadLeft))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.SalinitySpreadLeft, value)
        End Set
    End Property


    Public Property SalinitySpreadRight() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.SalinitySpreadRight))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.SalinitySpreadRight, value)
        End Set
    End Property

    Public Property Quota() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.QuotaSpecies))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.QuotaSpecies, value)
        End Set
    End Property

    Public Property BLim() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.BLim))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.BLim, value)
        End Set
    End Property

    Public Property BBase() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.BBase))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.BBase, value)
        End Set
    End Property

    Public Property FOpt() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Fopt))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.Fopt, value)
        End Set
    End Property

    Public Property RegCVBest() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.RegCVBest))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.RegCVBest, value)
        End Set

    End Property


    Public Property RegKalWt() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.RegKalWt))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.RegKalWt, value)
        End Set

    End Property


#Region "Indexed variables"

    ''' <summary>
    ''' Vulnerability multiplier vulnerability of this group to predation
    ''' </summary>
    ''' <param name="iGroup"></param>
    ''' <value></value>


    Public Property VulMult(ByVal iGroup As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.VulMult, iGroup))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.VulMult, value, iGroup)
        End Set

    End Property




    'jb remove Vulrate until it is needed
    'Public Property VulRate(ByVal iGroup As Integer) As Single

    '    Get
    '        Return CType(getVariable(eVarNameFlags.VulRate, iGroup), Single)
    '    End Get

    '    Set(ByVal value As Single)
    '        setVariable(eVarNameFlags.VulRate, value, iGroup)
    '    End Set

    'End Property


    'Public Property MedFunctionNumber(ByVal iGroup As Integer) As Integer

    '    Get
    '        Return CType(getVariable(eVarNameFlags.MedFunctNumber, iGroup), Integer)
    '    End Get

    '    Set(ByVal value As Integer)
    '        setVariable(eVarNameFlags.MedFunctNumber, value, iGroup)
    '    End Set

    'End Property


    'Public Property ForcingFunctionNumber(ByVal iGroup As Integer) As Integer

    '    Get
    '        Return CType(getVariable(eVarNameFlags.ForcingFunctNumber, iGroup), Integer)
    '    End Get

    '    Set(ByVal value As Integer)
    '        setVariable(eVarNameFlags.ForcingFunctNumber, value, iGroup)
    '    End Set

    'End Property

    'Public Property IsPredPrey(ByVal iGroup As Integer) As Boolean

    '    Get
    '        Return CBool(getVariable(eVarNameFlags.IsPredPrey, iGroup))
    '    End Get

    '    Friend Set(ByVal value As Boolean)
    '        setVariable(eVarNameFlags.IsPredPrey, value, iGroup)
    '    End Set

    'End Property

#End Region

#End Region

#Region "Status Flags via dot(.) operator"

    Public Property DenDepCatchabilityStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.DenDepCatchability)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.DenDepCatchability, value)
        End Set
    End Property

    Public Property FeedingTimeAdjustRateStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.FeedingTimeAdjRate)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.FeedingTimeAdjRate, value)
        End Set
    End Property

    Public Property MaxRelFeedingTimeStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MaxRelFeedingTime)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MaxRelFeedingTime, value)
        End Set
    End Property

    Public Property MaxRelPBStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MaxRelPB)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MaxRelPB, value)
        End Set
    End Property

    Public Property OtherMortFeedingTimeStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.OtherMortFeedingTime)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.OtherMortFeedingTime, value)
        End Set
    End Property

    Public Property PredEffectFeedingTimeStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.PredEffectFeedingTime)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.PredEffectFeedingTime, value)
        End Set
    End Property

    Public Property QBMaxBioStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.QBMaxQBio)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.QBMaxQBio, value)
        End Set
    End Property

    Public Property SwitchingPowerStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.SwitchingPower)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.SwitchingPower, value)
        End Set
    End Property

    Public Property SalinityOptStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.SalinityOpt)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.SalinityOpt, value)
        End Set
    End Property

    Public Property SalinitySpreadLeftStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.SalinitySpreadLeft)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.SalinitySpreadLeft, value)
        End Set
    End Property

    Public Property SalinitySpreadRightStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.SalinitySpreadRight)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.SalinitySpreadRight, value)
        End Set
    End Property

    Public Property VulMultiStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.VulMult, iGroup)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.VulMult, value, iGroup)
        End Set
    End Property

    Public Property VulRateStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.VulRate, iGroup)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.VulRate, value, iGroup)
        End Set
    End Property

    'Public Property ForcingFunctionNumberStatus(ByVal iGroup As Integer) As eStatusFlags
    '    Get
    '        Return getStatus(eVarNameFlags.ForcingFunctNumber, iGroup)
    '    End Get

    '    Set(ByVal value As eStatusFlags)
    '        setStatus(eVarNameFlags.ForcingFunctNumber, value, iGroup)
    '    End Set
    'End Property

    'Public Property MedFunctionNumberStatus(ByVal iGroup As Integer) As eStatusFlags
    '    Get
    '        Return getStatus(eVarNameFlags.MedFunctNumber, iGroup)
    '    End Get

    '    Set(ByVal value As eStatusFlags)
    '        setStatus(eVarNameFlags.MedFunctNumber, value, iGroup)
    '    End Set
    'End Property

#End Region

End Class
