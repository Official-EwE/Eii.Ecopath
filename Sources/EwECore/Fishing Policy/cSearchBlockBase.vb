Imports EwECore.ValueWrapper

''' <summary>
''' Blocks for Fishing Policy Search
''' </summary>
''' <remarks>This provides the CodeBlocks(iTimeIndex) interface</remarks>
Public Class cFishingPolicySearchBlock
    Inherits cCoreGroupBase


    Public Sub New(ByVal theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData

        m_DataType = eDataTypes.FishingPolicySearchBlocks
        m_messageSource = eMessageSource.FishingPolicySearch
        Me.AllowValidation = False
        Me.DBID = DBID

        'default OK status used for setVariable
        'see comment setVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.FishingPolicySearchBlocks, _
                                                    eMessageSource.FishingPolicySearch, Index, cCore.NULL_VALUE)


        meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.IntArray, eVarNameFlags.SearchBlock, eStatusFlags.Null, eCoreCounterTypes.nEcosimYears, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.SearchBlock))
        m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub


    Public Property SearchBlocks(ByVal iTimeIndex As Integer) As Integer

        Get
            Return CType(GetVariable(eVarNameFlags.SearchBlock, iTimeIndex), Integer)
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.SearchBlock, value, iTimeIndex)
        End Set

    End Property


End Class
