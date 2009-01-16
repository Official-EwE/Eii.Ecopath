'==============================================================================
'
' $Log: cEcospaceMPA.vb,v $
' Revision 1.2  2009/01/16 18:30:24  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:21  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/05/29 22:22:45  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.3  2007/09/14 16:54:19  jeroens
' + Added validation status
'
' Revision 1.2  2007/05/22 13:24:36  jeroens
' * Nitty-gritty
'
' Revision 1.1  2007/05/01 17:12:33  joeb
' Changed directory structure
'
' Revision 1.5  2007/04/04 23:44:19  jeroens
' + Added validator for MPAMonth
'
' Revision 1.4  2007/03/28 01:16:33  jeroens
' * Changed all status modification access from Public to Friend
'
' Revision 1.3  2007/02/25 05:22:48  jeroens
' + Exposed MPAmonth
'
' Revision 1.2  2007/01/23 16:58:33  jeroens
' + Added DBID to constructor
'
' Revision 1.1  2007/01/18 17:48:05  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceMPA
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Try

            m_dataType = eDataTypes.EcospaceMPA
            m_coreComponent = eCoreComponentType.EcoSpace
            Me.DBID = iDBID

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcospaceMPA, eCoreComponentType.EcoSpace, Index, cCore.NULL_VALUE)

            ResetStatusFlags()

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Array variables

            ' MPAMonth
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.MPAMonth, eStatusFlags.OK, eCoreCounterTypes.nMonths, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceMPA.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceMPA. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region " Variables by dot '.' operator "

    ''' <summary>
    ''' <see cref="eVarNameFlags.MPAMonth">MPAMonth</see>
    ''' </summary>
    Public Property MPAMonth(ByVal iMonth As Integer) As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MPAMonth, iMonth))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MPAMonth, value, iMonth)
        End Set
    End Property

#End Region ' Variables by dot '.' operator

#Region " Status by dot (.) operator "

    Public Property MPAMonthStatus(ByVal iMonth As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAMonth, iMonth)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MPAMonth, value, iMonth)
        End Set
    End Property

#End Region ' Status by dot (.) operator

End Class
