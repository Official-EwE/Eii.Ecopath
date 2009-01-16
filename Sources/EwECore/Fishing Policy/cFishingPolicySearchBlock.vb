Imports EwECore.ValueWrapper
Imports EwEUtils.Core

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

        m_dataType = eDataTypes.FishingPolicySearchBlocks
        m_coreComponent = eCoreComponentType.FishingPolicySearch
        Me.AllowValidation = False
        Me.DBID = DBID

        'default OK status used for setVariable
        'see comment setVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.FishingPolicySearchBlocks, _
                                                    eCoreComponentType.FishingPolicySearch, Index, cCore.NULL_VALUE)


        meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.IntArray, eVarNameFlags.SearchBlock, eStatusFlags.Null, eCoreCounterTypes.nEcosimYears, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.SearchBlock))
        m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub


    Public Property SearchBlocks(ByVal iTimeIndex As Integer) As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.SearchBlock, iTimeIndex))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.SearchBlock, value, iTimeIndex)
        End Set

    End Property

    ''' <summary>
    ''' Edit the SearchBlocks in batch mode no messages are sent out when BatchEdit = True when BatchEdit is toggled to False then the core is notified.
    ''' </summary>
    ''' <remarks>This turns off the AllowValidation flag which stops the object from calling core.OnValidate() vastly speeding up the editing</remarks>
    Public Property BatchEdit() As Boolean
        Get
            Return Not Me.AllowValidation
        End Get

        Set(ByVal value As Boolean)

            'if turning the BatchEdit On after it has been OFF tell the core that the values has been edited
            'this will allow the core to update the underlying data and send out a datamodified message
            If Me.BatchEdit = True And value = False Then
                Me.m_core.OnValidated(m_values.Item(eVarNameFlags.SearchBlock), Me)
            End If
            Me.AllowValidation = Not value

        End Set

    End Property


End Class
