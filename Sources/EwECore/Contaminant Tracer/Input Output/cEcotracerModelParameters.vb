'==============================================================================
'
' $Log: cEcotracerModelParameters.vb,v $
' Revision 1.2  2009/01/16 18:30:25  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:10  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/05/29 22:22:46  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.4  2008/01/08 11:29:04  jeroens
' Woops
'
' Revision 1.3  2008/01/06 11:00:46  jeroens
' * Inflow and outflow locked for input
'
' Revision 1.2  2007/12/05 03:48:45  jeroens
' * Added forcing no support
'
' Revision 1.1  2007/11/26 02:06:48  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcotracerModelParameters
    Inherits cCoreInputOutputBase

#Region " Constructor "

    Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData

        Try

            m_dataType = eDataTypes.EcotracerModelParameters
            m_coreComponent = eCoreComponentType.Ecotracer

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' CZero
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.CZero, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.CZero))
            m_values.Add(val.varName, val)

            ' CInflow
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.CInflow, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.CInflow))
            m_values.Add(val.varName, val)

            ' COutflow
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.COutflow, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.COutflow))
            m_values.Add(val.varName, val)

            ' CDecay
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.CDecay, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.CDecay))
            m_values.Add(val.varName, val)

            ''integers
            'ConForceNumber
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.ConForceNumber, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.ConForceNumber))
            m_values.Add(val.varName, val)

            'set status flags to default values
            ResetStatusFlags()
            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcotracerScenario.")
            cLog.Write(Me.ToString & ".New Error creating new cEcotracerScenario. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructor

#Region " Overrides "

    'Friend Overrides Function ResetStatusFlags() As Boolean

    '    If Not MyBase.ResetStatusFlags() Then Return False

    '    Me.SetStatusFlags(eVarNameFlags.CInflow, eStatusFlags.NotEditable, 0)
    '    Me.SetStatusFlags(eVarNameFlags.COutflow, eStatusFlags.NotEditable, 0)

    '    Return False

    'End Function

#End Region ' Overrides

#Region " Variable via dot(.) operator"

    Public Property CZero() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.CZero))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CZero, value)
        End Set
    End Property

    Public Property CInflow() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.CInflow))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CInflow, value)
        End Set
    End Property

    Public Property COutflow() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.COutflow))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.COutflow, value)
        End Set
    End Property

    Public Property CDecay() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.CDecay))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CDecay, value)
        End Set
    End Property

    Public Property ConForceNumber() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.ConForceNumber))
        End Get
        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.ConForceNumber, value)
        End Set
    End Property

#End Region ' Variable via dot(.) operator

#Region " Status Flags via dot(.) operator"

    Public Property CZeroStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CZero)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CZero, value)
        End Set

    End Property

    Public Property CInflowStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CInflow)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CInflow, value)
        End Set

    End Property

    Public Property COutflowStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.COutflow)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.COutflow, value)
        End Set

    End Property

    Public Property CDecayStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CDecay)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CDecay, value)
        End Set

    End Property

#End Region ' Status Flags via dot(.) operator

End Class
