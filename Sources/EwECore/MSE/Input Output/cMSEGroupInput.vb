' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Namespace MSE

    Public Class cMSEGroupInput
        Inherits cCoreGroupBase

        Public Sub New(core As cCore, theGroupDBID As Integer)
            MyBase.New(core)

            Dim val As cValue

            Me.m_dataType = eDataTypes.MSEGroupInput
            Me.m_coreComponent = eCoreComponentType.MSE
            Me.AllowValidation = False
            Me.DBID = theGroupDBID

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)


            'MSEBioCV
            val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.MSEBioCV, eStatusFlags.Null, eCoreCounterTypes.nEcosimYears)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSELowerRisk, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSEUpperRisk, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'Fixed Escapement
            val = New cValue(core, New Single, eVarNameFlags.MSEFixedEscapement, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ''Kalman Gain/Weight
            'meta = New cVariableMetadata(MSEKalmanGain ,0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            'val = New cValue(core, New Single, eVarNameFlags.MSEKalmanGain, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEKalmanGain))
            'm_values.Add(val.varName, val)

            'Ref levels Groups
            val = New cValue(core, New Single, eVarNameFlags.MSERefBioLower, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'Ref levels
            val = New cValue(core, New Single, eVarNameFlags.MSERefBioUpper, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSERefBioEstLower, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'Ref levels
            val = New cValue(core, New Single, eVarNameFlags.MSERefBioEstUpper, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)


            'Fleets ref levels
            val = New cValue(core, New Single, eVarNameFlags.MSERefGroupCatchLower, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSERefGroupCatchUpper, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSEForcastGain, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.RHalfB0Ratio, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSEFixedF, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSERecruitmentCV, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSETAC, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ''Quota per species
            'meta = New cVariableMetadata(QuotaSpecies ,0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            'val = New cValue(core, New Single, eVarNameFlags.QuotaSpecies, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.QuotaSpecies))
            'm_values.Add(val.varName, val)
            'bBase
            val = New cValue(core, New Single, eVarNameFlags.MSEBBase, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'bLim
            val = New cValue(core, New Single, eVarNameFlags.MSEBLim, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'FOpt
            val = New cValue(core, New Single, eVarNameFlags.MSEFmax, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'Fmin
            val = New cValue(core, New Single, eVarNameFlags.MSEFmin, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            Me.ResetStatusFlags()
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

            Set(value As Boolean)

                'if turning the BatchEdit On after it has been OFF tell the core that the values has been edited
                'this will allow the core to update the underlying data and send out a datamodified message
                If Me.BatchEdit = True And value = False Then
                    Me.m_core.OnValidated(Me.m_values.Item(eVarNameFlags.MSEBioCV), Me)
                End If
                Me.AllowValidation = Not value

            End Set

        End Property


        Public Property BiomassCV(TimeIndex As Integer) As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEBioCV, TimeIndex))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEBioCV, value, TimeIndex)
            End Set
        End Property

        Public Property LowerRisk() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSELowerRisk))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSELowerRisk, value)
            End Set
        End Property


        Public Property UpperRisk() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEUpperRisk))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEUpperRisk, value)
            End Set
        End Property

        Public Property BiomassRefLower() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSERefBioLower))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSERefBioLower, value)
            End Set
        End Property

        Public Property BiomassRefUpper() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSERefBioUpper))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSERefBioUpper, value)
            End Set
        End Property

        Public Property BiomassEstRefUpper() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSERefBioEstUpper))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSERefBioEstUpper, value)
            End Set
        End Property


        Public Property BiomassEstRefLower() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSERefBioEstLower))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSERefBioEstLower, value)
            End Set
        End Property

        Public Property CatchRefLower() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSERefGroupCatchLower))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSERefGroupCatchLower, value)
            End Set
        End Property

        Public Property CatchRefUpper() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSERefGroupCatchUpper))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSERefGroupCatchUpper, value)
            End Set
        End Property

        Public Property FixedEscapement() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEFixedEscapement))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEFixedEscapement, value)
            End Set
        End Property

        Public Property ForcastGain() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEForcastGain))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEForcastGain, value)
            End Set
        End Property


        Public Property RHalfB0Ratio() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.RHalfB0Ratio))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.RHalfB0Ratio, value)
            End Set
        End Property

        Public Property FixedF() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEFixedF))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEFixedF, value)
            End Set
        End Property


        Public Property RecruitmentCV() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSERecruitmentCV))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSERecruitmentCV, value)
            End Set
        End Property


        Public Property BLim() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEBLim))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEBLim, value)
            End Set
        End Property

        Public Property BBase() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEBBase))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEBBase, value)
            End Set
        End Property

        Public Property FOpt() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEFmax))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEFmax, value)
            End Set
        End Property

        Public Property Fmin() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEFmin))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEFmin, value)
            End Set
        End Property

        Public Property TAC() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETAC))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETAC, value)
            End Set
        End Property

        Public Property FixedFStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEFixedF)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEFixedF, value)
            End Set
        End Property

        Public Property FixedEscapementStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEFixedEscapement)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEFixedEscapement, value)
            End Set
        End Property

        Public Property BiomassCVStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEBioCV)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEBioCV, value)
            End Set
        End Property

        Public Property LowerRiskStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSELowerRisk)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSELowerRisk, value)
            End Set
        End Property

        Public Property UpperRiskStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEUpperRisk)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEUpperRisk, value)
            End Set
        End Property

        Public Property BiomassRefLowerStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSERefBioLower)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSERefBioLower, value)
            End Set
        End Property

        Public Property BiomassRefUpperStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSERefBioLower)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSERefBioLower, value)
            End Set
        End Property

        Public Property ForcastGainStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEForcastGain)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEForcastGain, value)
            End Set
        End Property

        Public Property RHalfB0RatioStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.RHalfB0Ratio)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.RHalfB0Ratio, value)
            End Set
        End Property

#Region " Overrides "

        Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
            MyBase.ResetStatusFlags(bForceReset)

            Me.AllowValidation = False
            Dim tcatch As Single

            For iflt As Integer = 1 To Me.m_core.nFleets
                Dim fleet As cEcopathFleetInput = Me.m_core.EcopathFleetInputs(iflt)
                tcatch += fleet.Landings(Me.Index) + fleet.Discards(Me.Index)
            Next

            If tcatch = 0.0! Then
                Me.SetStatusFlags(eVarNameFlags.MSEFixedEscapement, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEFixedF, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSETAC, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEBioCV, eStatusFlags.Null Or eStatusFlags.NotEditable)

                Me.SetStatusFlags(eVarNameFlags.MSERefGroupCatchUpper, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSERefGroupCatchLower, eStatusFlags.Null Or eStatusFlags.NotEditable)

                Me.SetStatusFlags(eVarNameFlags.RHalfB0Ratio, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEForcastGain, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSERecruitmentCV, eStatusFlags.Null Or eStatusFlags.NotEditable)

                Me.SetStatusFlags(eVarNameFlags.MSEBBase, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEBLim, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEFmax, eStatusFlags.Null Or eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEFmin, eStatusFlags.Null Or eStatusFlags.NotEditable)

            Else

                ' JS 28Oct2010: Do NOT clear the NULL flag, because values may hold cCore.NULL_VALUEs.
                '               This status is set by the baseclass ResetStatusFlags call and should not be altered.
                Me.ClearStatusFlags(eVarNameFlags.MSEFixedEscapement, eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSEFixedF, eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSETAC, eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSEBioCV, eStatusFlags.NotEditable)

                Me.ClearStatusFlags(eVarNameFlags.MSERefGroupCatchUpper, eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSERefGroupCatchLower, eStatusFlags.NotEditable)

                Me.ClearStatusFlags(eVarNameFlags.RHalfB0Ratio, eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSEForcastGain, eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSERecruitmentCV, eStatusFlags.NotEditable)

                Me.ClearStatusFlags(eVarNameFlags.MSEBBase, eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSEBLim, eStatusFlags.NotEditable)
                Me.ClearStatusFlags(eVarNameFlags.MSEFmax, eStatusFlags.NotEditable)

            End If

            If Me.FixedEscapement <> 0 Or Me.FixedF <> 0 Then
                Me.SetStatusFlags(eVarNameFlags.MSEBBase, eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEBLim, eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEFmax, eStatusFlags.NotEditable)
                Me.SetStatusFlags(eVarNameFlags.MSEFmin, eStatusFlags.NotEditable)
            End If

            Me.AllowValidation = True

        End Function

#End Region ' Overrides

    End Class

End Namespace
