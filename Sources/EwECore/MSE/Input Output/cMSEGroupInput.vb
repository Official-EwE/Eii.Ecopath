#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region

Namespace MSE

    Public Class cMSEGroupInput
        Inherits cCoreGroupBase

        Public Sub New(ByRef theCore As cCore, ByVal theGroupDBID As Integer)
            MyBase.New(theCore)

            Dim val As cValue
            Dim meta As cVariableMetaData

            m_dataType = eDataTypes.MSEGroupInput
            m_coreComponent = eCoreComponentType.MSE
            Me.AllowValidation = False
            Me.DBID = theGroupDBID

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.MSEGroupInput, eCoreComponentType.MSE, Index, cCore.NULL_VALUE)


            'MSEBioCV
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.MSEBioCV, eStatusFlags.Null, eCoreCounterTypes.nEcosimYears, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBioCV))
            m_values.Add(val.varName, val)


            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSELowerRisk, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSELowerRisk))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEUpperRisk, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEUpperRisk))
            m_values.Add(val.varName, val)

            'Fixed Escapement
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEFixedEscapement, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEFixedEscapement))
            m_values.Add(val.varName, val)

            ''Kalman Gain/Weight
            'meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            'val = New cValue(New Single, eVarNameFlags.MSEKalmanGain, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEKalmanGain))
            'm_values.Add(val.varName, val)

            'Ref levels Groups
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSERefBioLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSERefBioLower))
            m_values.Add(val.varName, val)

            'Ref levels
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSERefBioUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSERefBioUpper))
            m_values.Add(val.varName, val)

            'Fleets ref levels
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSERefGroupCatchLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSERefGroupCatchLower))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSERefGroupCatchUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSERefGroupCatchUpper))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEForcastGain, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEForcastGain))
            m_values.Add(val.varName, val)


            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.RHalfB0Ratio, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.RHalfB0Ratio))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEFixedF, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEFixedF))
            m_values.Add(val.varName, val)

            Me.AllowValidation = True

        End Sub

        ''' <summary>
        ''' Edit the SearchBlocks in batch mode no messages are sent out when BatchEdit = True when BatchEdit is toggled to False then the core is notified.
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
                    Me.m_core.OnValidated(m_values.Item(eVarNameFlags.MSEBioCV), Me)
                End If
                Me.AllowValidation = Not value

            End Set

        End Property


        Public Property BiomassCV(ByVal TimeIndex As Integer) As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSEBioCV, TimeIndex), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEBioCV, value, TimeIndex)
            End Set
        End Property

        Public Property LowerRisk() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSELowerRisk), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSELowerRisk, value)
            End Set
        End Property


        Public Property UpperRisk() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSEUpperRisk), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEUpperRisk, value)
            End Set
        End Property

        Public Property BiomassRefLower() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSERefBioLower), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSERefBioLower, value)
            End Set
        End Property

        Public Property BiomassRefUpper() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSERefBioUpper), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSERefBioUpper, value)
            End Set
        End Property

        Public Property CatchRefLower() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSERefGroupCatchLower), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSERefGroupCatchLower, value)
            End Set
        End Property

        Public Property CatchRefUpper() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSERefGroupCatchUpper), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSERefGroupCatchUpper, value)
            End Set
        End Property

        Public Property FixedEscapement() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSEFixedEscapement), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEFixedEscapement, value)
            End Set
        End Property

        Public Property ForcastGain() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSEForcastGain), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEForcastGain, value)
            End Set
        End Property


        Public Property RHalfB0Ratio() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.RHalfB0Ratio), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.RHalfB0Ratio, value)
            End Set
        End Property

        Public Property FixedF() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSEFixedF), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEFixedF, value)
            End Set
        End Property

        Public Property FixedFStatus() As eStatusFlags
            Get
                Return DirectCast(GetStatus(eVarNameFlags.MSEFixedF), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEFixedF, value)
            End Set
        End Property

        Public Property FixedEscapementStatus() As eStatusFlags
            Get
                Return DirectCast(GetStatus(eVarNameFlags.MSEFixedEscapement), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEFixedEscapement, value)
            End Set
        End Property

        Public Property BiomassCVStatus() As eStatusFlags
            Get
                Return DirectCast(GetStatus(eVarNameFlags.MSEBioCV), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEBioCV, value)
            End Set
        End Property

        Public Property LowerRiskStatus() As eStatusFlags
            Get
                Return DirectCast(GetStatus(eVarNameFlags.MSELowerRisk), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSELowerRisk, value)
            End Set
        End Property

        Public Property UpperRiskStatus() As eStatusFlags
            Get
                Return DirectCast(GetStatus(eVarNameFlags.MSEUpperRisk), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEUpperRisk, value)
            End Set
        End Property

        Public Property BiomassRefLowerStatus() As eStatusFlags
            Get
                Return DirectCast(GetStatus(eVarNameFlags.MSERefBioLower), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSERefBioLower, value)
            End Set
        End Property

        Public Property BiomassRefUpperStatus() As eStatusFlags
            Get
                Return DirectCast(GetStatus(eVarNameFlags.MSERefBioLower), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSERefBioLower, value)
            End Set
        End Property

        Public Property ForcastGainStatus() As eStatusFlags
            Get
                Return CType(GetStatus(eVarNameFlags.MSEForcastGain), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEForcastGain, value)
            End Set
        End Property

        Public Property RHalfB0RatioStatus() As eStatusFlags
            Get
                Return CType(GetStatus(eVarNameFlags.RHalfB0Ratio), eStatusFlags)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.RHalfB0Ratio, value)
            End Set
        End Property

#Region " Overrides "

        Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
            MyBase.ResetStatusFlags(bForceReset)

            Me.AllowValidation = False
            Dim tcatch As Single

            For iflt As Integer = 1 To Me.m_core.nFleets
                Dim fleet As cFleetInput = Me.m_core.FleetInputs(iflt)
                tcatch += fleet.Landings(Me.Index) + fleet.Discards(Me.Index)
            Next

            If tcatch = 0.0! Then
                Me.SetStatusFlags(eVarNameFlags.MSEFixedEscapement, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEFixedF, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEBioCV, eStatusFlags.Null Or eStatusFlags.NotEditable)

                Me.SetStatusFlags(eVarNameFlags.MSERefGroupCatchUpper, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSERefGroupCatchLower, eStatusFlags.Null Or eStatusFlags.NotEditable)
            Else
                Me.ClearStatusFlags(eVarNameFlags.MSEFixedEscapement, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSEFixedF, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSEBioCV, eStatusFlags.Null Or eStatusFlags.NotEditable)

                Me.ClearStatusFlags(eVarNameFlags.MSERefGroupCatchUpper, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSERefGroupCatchLower, eStatusFlags.Null Or eStatusFlags.NotEditable)
            End If

            'If bSendMessage Then
            '    Me.m_publisher.SendMessage(New cMessage("", eMessageType.DataModified, _
            '            eCoreComponentType.EcoPath, eMessageImportance.Maintenance, eDataTypes.EcosimFisheriesRegulation))
            'End If

            Me.AllowValidation = True

        End Function

#End Region ' Overrides

    End Class

End Namespace
