


#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region

Public Class cMSEBatchParameters
    Inherits cCoreGroupBase


    Public Sub New(ByRef theCore As cCore, ByRef MSEBatchData As MSEBatchManager.cMSEBatchDataStructures, ByVal DBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData

        m_dataType = eDataTypes.MSEBatchParameters
        m_coreComponent = eCoreComponentType.MSE
        Me.AllowValidation = False
        Me.DBID = DBID

        'default OK status used for setVariable
        'see comment setVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.MSETFMNIteration, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMNIteration))
        m_values.Add(val.varName, val)

        Dim nTypes As Integer = [Enum].GetValues(GetType(eMSEBatchIterCalcTypes)).Length
        meta = New cVariableMetaData(0, nTypes, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.MSEBatchIterCalcType, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBatchIterCalcType))
        m_values.Add(val.varName, val)


        Me.AllowValidation = True

    End Sub


    Public Property nTFMIteration As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MSETFMNIteration))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.MSETFMNIteration, value)
        End Set
    End Property

    Public Property IterCalcType As eMSEBatchIterCalcTypes
        Get
            Return CType(GetVariable(eVarNameFlags.MSEBatchIterCalcType), eMSEBatchIterCalcTypes)
        End Get

        Set(ByVal value As eMSEBatchIterCalcTypes)
            SetVariable(eVarNameFlags.MSEBatchIterCalcType, value)
        End Set
    End Property





End Class
