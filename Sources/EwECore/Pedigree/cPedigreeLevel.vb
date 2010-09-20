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
    Private m_ID As Integer = 0

    Friend Sub New(ByVal core As cCore, ByVal manager As cPedigreeManager, ByVal iDBID As Integer)
        MyBase.New(core)

        Dim val As cValue
        Dim meta As cVariableMetaData

        Me.DBID = iDBID
        Me.m_data = core.m_EcoPathData
        Me.m_manager = manager
        Me.m_dataType = eDataTypes.PedigreeLevel

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

    End Sub

    ''' <summary>
    ''' Get/set the index of the level in its <see cref="cPedigreeManager">manager</see>.
    ''' </summary>

    Public Property ID() As Integer
        Get
            Return Me.m_ID
        End Get
        Set(ByVal value As Integer)
            Me.m_ID = value
        End Set
    End Property

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

End Class
