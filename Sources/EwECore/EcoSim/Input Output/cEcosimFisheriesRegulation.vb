'==============================================================================
'
' $Log: cEcosimFisheriesRegulation.vb,v $
' Revision 1.5  2008/10/22 15:54:41  joeb
' ResetStatusFlags handled by the core
'
' Revision 1.4  2008/10/09 17:21:04  jeroens
' Moved discard mort data from Ecosim to Ecopath
'
' Revision 1.3  2008/10/08 17:54:22  jeroens
' DiscardMortality about to be moved to Ecopath, removed from this class
'
' Revision 1.2  2008/10/06 21:11:54  jeroens
' Added Fisheries Regulation data status flags
'
' Revision 1.1  2008/10/03 23:07:43  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' 
''' </summary>
Public Class cEcosimFisheriesRegulation
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Try

            Dim val As cValue = Nothing
            Dim meta As cVariableMetaData = Nothing

            Me.m_DataType = eDataTypes.EcosimFisheriesRegulation
            Me.m_messageSource = eMessageSource.EcoSim
            Me.DBID = DBID
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, m_DataType, eMessageSource.EcoSim, Index, cCore.NULL_VALUE)

            Me.AllowValidation = False

            'MaxEffort
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), cCore.NULL_VALUE)
            val = New cValue(New Single, eVarNameFlags.MaxEffort, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MaxEffort))
            m_values.Add(val.varName, val)

            'QuotaType
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.QuotaType, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.QuotaType))
            m_values.Add(val.varName, val)

            ' === arrays ===
            'Quota
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.Quota, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.Quota))
            m_values.Add(val.varName, val)

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcosimFisheriesRegulation.")
            cLog.Write(Me.ToString & " Error creating new cEcosimFisheriesRegulation. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region " Overrides "

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        '   If Not MyBase.ResetStatusFlags(bForceReset) Then Return False
        Return Me.m_core.Set_Quota_Flags(Me, False)
    End Function

#End Region ' Overrides

#Region "Variable via dot(.) operator"

    Public Property MaxEffort() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MaxEffort))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MaxEffort, value)
        End Set
    End Property

    Public Property QuotaType() As eQuotaTypes
        Get
            Return DirectCast(GetVariable(eVarNameFlags.QuotaType), eQuotaTypes)
        End Get

        Set(ByVal value As eQuotaTypes)
            SetVariable(eVarNameFlags.QuotaType, value)
        End Set
    End Property

    Public Property Quota(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Quota, iGroup))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.Quota, value, iGroup)
        End Set
    End Property

#End Region

#Region "Status Flags via dot(.) operator"

    Public Property MaxEffortStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MaxEffort)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MaxEffort, value)
        End Set
    End Property

    Public Property QuotaTypeStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.QuotaType)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.QuotaType, value)
        End Set
    End Property

    Public Property QuotaStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.Quota, iGroup)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Quota, value, iGroup)
        End Set
    End Property

#End Region

End Class
