#Region "Imports"
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
#End Region

Namespace MSE


    Public Class cMSEFleetInput
        Inherits cCoreGroupBase

        Public Sub New(ByRef theCore As cCore, ByVal theFleetDBID As Integer)
            MyBase.New(theCore)

            Dim val As cValue
            Dim meta As cVariableMetaData

            m_dataType = eDataTypes.MSEFleetInput
            m_coreComponent = eCoreComponentType.MSE
            Me.AllowValidation = False
            Me.DBID = theFleetDBID

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, m_dataType, m_coreComponent, Index, cCore.NULL_VALUE)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEQIncrease, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEQIncrease))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.MSEFleetCV, eStatusFlags.Null, eCoreCounterTypes.nEcosimYears, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEFleetCV))
            m_values.Add(val.varName, val)


            'meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            'val = New cValue(New Single, eVarNameFlags.MSEFleetCV, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEFleetCV))
            'm_values.Add(val.varName, val)


            meta = New cVariableMetaData(1, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.MSEFleetWeight, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEFleetWeight))
            m_values.Add(val.varName, val)


            'Bounds
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSERefFleetCatchLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSERefFleetCatchLower))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSERefFleetCatchUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSERefFleetCatchUpper))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSERefFleetEffortLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSERefFleetEffortLower))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSERefFleetEffortUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSERefFleetEffortUpper))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.MSYEvaluateFleet, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSYEvaluateFleet))
            val.Stored = False
            m_values.Add(val.varName, val)

            Me.AllowValidation = True

        End Sub

        ''' <summary>
        ''' Edit the CVs in batch mode no messages are sent out when BatchEdit = True when BatchEdit is toggled to False then the core is notified.
        ''' </summary>
        ''' <remarks>This turns off the AllowValidation flag which stops the object from calling core.OnValidate() vastly speeding up the editing</remarks>
        Public Property BatchEdit() As Boolean
            Get
                Return Not Me.AllowValidation
            End Get

            Set(ByVal value As Boolean)

                'if turning the BatchEdit On after it has been OFF tell the core that the values has been edited
                'this will allow the core to update the underlying data and send out a datamodified message
                If Me.BatchEdit = True And value = False Then
                    Me.m_core.OnValidated(m_values.Item(eVarNameFlags.MSEFleetCV), Me)
                End If
                Me.AllowValidation = Not value

            End Set

        End Property


        ''' <summary>
        ''' MSE increase in catchability by group per year (multiplier)
        ''' </summary>
        Public Property QIncrease() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSEQIncrease), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEQIncrease, value)
            End Set
        End Property


        Public Property FleetCV(ByVal iTime As Integer) As Single

            Get
                Return CType(GetVariable(eVarNameFlags.MSEFleetCV, iTime), Single)
            End Get

            Set(ByVal value As Single)

                SetVariable(eVarNameFlags.MSEFleetCV, value, iTime)

            End Set

        End Property


        Public Property CatchRefLower() As Single

            Get
                Return CType(GetVariable(eVarNameFlags.MSERefFleetCatchLower), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSERefFleetCatchLower, value)
            End Set

        End Property

        Public Property CatchRefUpper() As Single

            Get
                Return CType(GetVariable(eVarNameFlags.MSERefFleetCatchUpper), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSERefFleetCatchUpper, value)
            End Set

        End Property


        Public Property EffortRefLower() As Single

            Get
                Return CType(GetVariable(eVarNameFlags.MSERefFleetEffortLower), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSERefFleetEffortLower, value)
            End Set

        End Property

        Public Property EffortRefUpper() As Single

            Get
                Return CType(GetVariable(eVarNameFlags.MSERefFleetEffortUpper), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSERefFleetEffortUpper, value)
            End Set

        End Property



        ''' <summary>
        ''' Importance weight of fleet on a group
        ''' </summary>
        ''' <param name="iGroup">impacted group</param>
        Public Property FleetWeight(ByVal iGroup As Integer) As Single

            Get
                Return CType(GetVariable(eVarNameFlags.MSEFleetWeight, iGroup), Single)
            End Get

            Set(ByVal value As Single)

                SetVariable(eVarNameFlags.MSEFleetWeight, value, iGroup)

            End Set

        End Property


        Public Property MSYEvaluateFleet() As Boolean

            Get
                Return CType(GetVariable(eVarNameFlags.MSYEvaluateFleet), Single)
            End Get

            Set(ByVal value As Boolean)
                SetVariable(eVarNameFlags.MSYEvaluateFleet, value)
            End Set

        End Property

        Public Property QIncreaseStatus() As eStatusFlags
            Get
                Return CType(GetStatus(eVarNameFlags.MSEQIncrease), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEQIncrease, value)
            End Set
        End Property


        Public Property CatchRefUpperStatus() As eStatusFlags
            Get
                Return CType(GetStatus(eVarNameFlags.MSERefFleetCatchUpper), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSERefFleetCatchUpper, value)
            End Set
        End Property

        Public Property CatchRefLowerStatus() As eStatusFlags
            Get
                Return CType(GetStatus(eVarNameFlags.MSERefGroupCatchLower), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSERefGroupCatchLower, value)
            End Set
        End Property


        Public Property FleetCVStatus() As eStatusFlags
            Get
                Return CType(GetStatus(eVarNameFlags.MSEFleetCV), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEFleetCV, value)
            End Set
        End Property

        Public Property FleetWeightStatus(ByVal iGroup As Integer) As eStatusFlags

            Get
                Return CType(GetStatus(eVarNameFlags.MSEFleetWeight, iGroup), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)

                SetStatus(eVarNameFlags.MSEFleetWeight, value, iGroup)

            End Set

        End Property

        Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean

            For Each value As cValue In Me.m_values.Values

                Try
                    Select Case value.varName

                        Case eVarNameFlags.MSEFleetWeight

                            For igrp As Integer = 1 To m_core.nLivingGroups
                                If Me.m_core.m_EcoSimData.relQ(value.Index, igrp) > 0 Then
                                    value.Status(igrp) = eStatusFlags.OK
                                Else
                                    value.Status(igrp) = eStatusFlags.NotEditable Or eStatusFlags.Null
                                End If
                            Next

                        Case eVarNameFlags.MSEFleetCV

                            For i As Integer = 1 To Me.m_core.nEcosimYears
                                value.setStatusFlag(i)
                            Next

                        Case Else

                            value.setStatusFlag()

                    End Select

                Catch ex As Exception
                    Debug.Assert(False, ex.Message)
                    System.Console.WriteLine(Me.ToString & ".ResetStatusFlags() Exception " & value.varName.ToString)
                End Try

            Next value

            Return True

        End Function

    End Class

End Namespace
