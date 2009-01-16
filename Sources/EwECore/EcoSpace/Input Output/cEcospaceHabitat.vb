'==============================================================================
'
' $Log: cEcospaceHabitat.vb,v $
' Revision 1.2  2009/01/16 18:30:23  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:21  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/05/29 22:22:45  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.4  2007/09/24 18:55:34  jeroens
' * Fixed datatype bug
'
' Revision 1.3  2007/09/14 16:54:07  jeroens
' * Fixed validation status setup error
'
' Revision 1.2  2007/05/31 16:34:26  jeroens
' * Exposed Habitat Area Proportion
'
' Revision 1.1  2007/05/01 17:12:33  joeb
' Changed directory structure
'
' Revision 1.2  2007/01/20 00:28:38  joeb
' Added Variables
'
' Revision 1.1  2007/01/14 21:18:24  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceHabitat
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Me.DBID = DBID
        m_dataType = eDataTypes.EcospaceHabitat
        m_coreComponent = eCoreComponentType.EcoSpace

        Dim val As cValue
        Dim meta As cVariableMetaData

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcospaceHabitat, eCoreComponentType.EcoSpace, Index, cCore.NULL_VALUE)

            ' HabAreaProportion
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.HabAreaProportion, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceHabitat.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceHabitat. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Properties by dot (.) operator "

    Public Property HabAreaProportion() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.HabAreaProportion))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.HabAreaProportion, value)
        End Set
    End Property

#End Region

#Region "Status by dot (.) operator"

    Public Property HabAreaProportionStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.HabAreaProportion)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.HabAreaProportion, value)
        End Set
    End Property

#End Region

End Class
