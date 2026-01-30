' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' <summary>
''' Inputs for EcoSim for a single group.
''' </summary>
''' <remarks>
''' This class wraps the inputs to EcoSim for one group into a single object.
''' </remarks>
Public Class cEcosimGroupInput
    Inherits cCoreGroupBase

    Private m_nGroups As Integer
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcosimGroupInput)()

    ''' <summary>
    ''' Public access to set the status flags by calling each validator.
    ''' </summary>
    ''' <returns>True is successful. False otherwise</returns>
    ''' <remarks>This is the default behaviour for Input objects. An output will need to override this to provide its own implementation.</remarks>
    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In Me.m_values
            Try
                value = keyvalue.Value
                'Status flag for VulMult and VulRate are set in cCore.LoadEcosimGroups
                'If value.varName <> eVarNameFlags.VulMult And value.varName <> eVarNameFlags.VulRate Then
                If value.varName <> eVarNameFlags.VulMult Then
                    Select Case value.varType
                        Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.BoolArray
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


    Sub New(core As cCore, DBID As Integer)
        MyBase.New(core)

        Try

            Me.m_nGroups = core.nGroups

            Me.m_dataType = eDataTypes.EcoSimGroupInput
            Me.m_coreComponent = eCoreComponentType.Ecosim
            Me.AllowValidation = False
            Me.DBID = DBID

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            Dim val As cValue

            'MaxRelPB
            val = New cValue(core, New Single, eVarNameFlags.MaxRelPB, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'MaxRelFeedingTime
            val = New cValue(core, New Single, eVarNameFlags.MaxRelFeedingTime, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'FeedingTimeAdjRate
            val = New cValue(core, New Single, eVarNameFlags.FeedingTimeAdjRate, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'OtherMortFeedingTime
            val = New cValue(core, New Single, eVarNameFlags.OtherMortFeedingTime, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'PredEffectFeedingTime
            val = New cValue(core, New Single, eVarNameFlags.PredEffectFeedingTime, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'DenDepCatchability
            val = New cValue(core, New Single, eVarNameFlags.DenDepCatchability, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'QBMaxQBio
            val = New cValue(core, New Single, eVarNameFlags.QBMaxQBio, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'Switching Power
            val = New cValue(core, New Single, eVarNameFlags.SwitchingPower, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'Srrayed values
            ''VulRate
            'meta = New cVariableMetaData(1, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            'val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.VulRate, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.VulRate))
            'm_values.Add(val.varName, val)

            'VulMult
            val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.VulMult, eStatusFlags.Null, eCoreCounterTypes.nGroups)
            Me.m_values.Add(val.varName, val)

            ' Additive predation mortality proportion. Added 26 Nov 2019, CW, JB, VC and JS
            val = New cValue(core, New Single, eVarNameFlags.AdditivePredMortProp, eStatusFlags.Null, eValueTypes.Sng)
            ' This value is NOT stored in the database for the time being
            val.Stored = True
            Me.m_values.Add(val.varName, val)

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcoSimGroupInfo.")
            m_logger.LogError(ex, Me.ToString & ".New(nGroups) Error creating new cEcoSimGroupInfo. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Variable via dot(.) operator"

    Public Property DenDepCatchability() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.DenDepCatchability))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.DenDepCatchability, value)
        End Set
    End Property

    Public Property FeedingTimeAdjustRate() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.FeedingTimeAdjRate))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.FeedingTimeAdjRate, value)
        End Set
    End Property

    Public Property MaxRelFeedingTime() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MaxRelFeedingTime))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.MaxRelFeedingTime, value)
        End Set
    End Property

    Public Property MaxRelPB() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MaxRelPB))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.MaxRelPB, value)
        End Set
    End Property

    Public Property OtherMortFeedingTime() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.OtherMortFeedingTime))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.OtherMortFeedingTime, value)
        End Set
    End Property

    Public Property PredEffectFeedingTime() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.PredEffectFeedingTime))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.PredEffectFeedingTime, value)
        End Set
    End Property

    Public Property QBMaxQBio() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.QBMaxQBio))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.QBMaxQBio, value)
        End Set
    End Property

    Public Property SwitchingPower() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.SwitchingPower))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.SwitchingPower, value)
        End Set
    End Property

    Public Property AdditivePredationMortality As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.AdditivePredMortProp))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.AdditivePredMortProp, value)
        End Set
    End Property

#Region "Indexed variables"

    ''' <summary>
    ''' Vulnerability multiplier vulnerability of this group to predation
    ''' </summary>
    ''' <param name="iPredGroup">Group index of the predator group</param>
    ''' <value></value>
    Public Property VulMult(iPredGroup As Integer) As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.VulMult, iPredGroup))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.VulMult, value, iPredGroup)
        End Set

    End Property

#End Region

#End Region

#Region "Status Flags via dot(.) operator"

    Public Property DenDepCatchabilityStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.DenDepCatchability)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.DenDepCatchability, value)
        End Set
    End Property

    Public Property FeedingTimeAdjustRateStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.FeedingTimeAdjRate)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.FeedingTimeAdjRate, value)
        End Set
    End Property

    Public Property MaxRelFeedingTimeStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MaxRelFeedingTime)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MaxRelFeedingTime, value)
        End Set
    End Property

    Public Property MaxRelPBStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MaxRelPB)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MaxRelPB, value)
        End Set
    End Property

    Public Property OtherMortFeedingTimeStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.OtherMortFeedingTime)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.OtherMortFeedingTime, value)
        End Set
    End Property

    Public Property PredEffectFeedingTimeStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.PredEffectFeedingTime)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.PredEffectFeedingTime, value)
        End Set
    End Property

    Public Property QBMaxBioStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.QBMaxQBio)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.QBMaxQBio, value)
        End Set
    End Property

    Public Property SwitchingPowerStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.SwitchingPower)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.SwitchingPower, value)
        End Set
    End Property

    Public Property VulMultiStatus(iGroup As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.VulMult, iGroup)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.VulMult, value, iGroup)
        End Set
    End Property

    'Public Property VulRateStatus(iGroup As Integer) As eStatusFlags
    '    Get
    '        Return GetStatus(eVarNameFlags.VulRate, iGroup)
    '    End Get

    '    Set(value As eStatusFlags)
    '        SetStatus(eVarNameFlags.VulRate, value, iGroup)
    '    End Set
    'End Property

    Public Property AdditivePredationMortalityStatus As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.AdditivePredMortProp)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.AdditivePredMortProp, value)
        End Set
    End Property

#End Region

End Class
