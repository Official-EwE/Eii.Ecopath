'==============================================================================
'
' $Log: cMSEGroupInput.vb,v $
' Revision 1.2  2009/01/16 18:30:32  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:27  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/05/29 22:22:50  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.4  2008/04/24 14:53:41  joeb
' Added CVS Log header
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

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


            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEBioCV, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBioCV))
            m_values.Add(val.varName, val)


            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSELowerRisk, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSELowerRisk))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEUpperRisk, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEUpperRisk))
            m_values.Add(val.varName, val)

            Me.AllowValidation = True


        End Sub


        Public Property BiomassCV() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSEBioCV), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEBioCV, value)
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

    End Class

End Namespace
