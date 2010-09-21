#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports System.Collections.Generic

#End Region ' Imports

Public Class cPedigreeLevel
    Inherits cCoreInputOutputBase

    Private m_data As cEcopathDataStructures = Nothing
    Private m_manager As cPedigreeManager = Nothing

    Friend Sub New(ByVal core As cCore, ByVal manager As cPedigreeManager, ByVal iDBID As Integer)
        MyBase.New(core)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim desc() As Char

        Me.DBID = iDBID
        Me.m_data = core.m_EcoPathData
        Me.m_manager = manager
        Me.m_dataType = eDataTypes.PedigreeLevel
        Me.m_coreComponent = eCoreComponentType.EcoPath

        Me.m_ValidationStatus = New cVariableStatus
        Me.m_ValidationStatus.CoreDataObject = Me

        'VarName
        meta = New cVariableMetaData(0, 1000, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Integer, eVarNameFlags.VariableName, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'IndexValue
        meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.IndexValue, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'ConfidenceInterval
        meta = New cVariableMetaData(0, 100, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.ConfidenceInterval, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        ' Description
        meta = New cVariableMetaData(60000)
        val = New cValue(New String(desc), eVarNameFlags.Description, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                            meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

    End Sub

    Public Property VariableName() As eVarNameFlags
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.VariableName), eVarNameFlags)
        End Get
        Set(ByVal value As eVarNameFlags)
            Me.SetVariable(eVarNameFlags.VariableName, value)
        End Set
    End Property

    Public Property IndexValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.IndexValue))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.IndexValue, value)
        End Set
    End Property

    Public Property ConfidenceInterval() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.ConfidenceInterval))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.ConfidenceInterval, value)
        End Set
    End Property

    Public Property Description() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Description))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Description, value)
        End Set
    End Property

End Class
