Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cMonteCarloGroup
    Inherits cCoreGroupBase

#Region "constructor"

    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)


        m_dataType = eDataTypes.MonteCarlo
        m_coreComponent = eCoreComponentType.EcoSim
        Me.AllowValidation = False
        Me.DBID = DBID

        'default OK status used for setVariable
        'see comment setVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimGroupInput, eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)

        Dim val As cValue
        Dim meta As cVariableMetaData

        'biomass
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcB, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'PB
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcPB, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'ba
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcBA, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'QB
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcQB, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'EE
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcEE, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'VU
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcVU, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'biomassLower
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcBLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'PBLower
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcPBLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'baLower
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcBALower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'QBLower
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcQBLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'EELower
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcEELower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'VULower
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcVULower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Best fit
        'biomassBF
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcBbf, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'PBBF
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcPBbf, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'baBF
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcBAbf, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'QBBF
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcQBbf, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'EEBF
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcEEbf, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'VUBF
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcVUbf, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Upper

        'biomassUpper
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcBUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'PBUpper
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcPBUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'baUpper
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcBAUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'QBUpper
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcQBUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'EEUpper
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcEEUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'VUUpper
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcVUUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'cv

        'biomasscv
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcBcv, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'PBcv
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcPBcv, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'bacv
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcBAcv, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'QBcv
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcQBcv, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'EEcv
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcEEcv, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'VUcv
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.mcVUcv, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

    End Sub


    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value

                Select Case value.varType

                    Case eValueTypes.Sng
                        value.Status = eStatusFlags.OK

                        If value.varName = eVarNameFlags.mcB Or value.varName = eVarNameFlags.mcBA _
                        Or value.varName = eVarNameFlags.mcEE Or value.varName = eVarNameFlags.mcPB _
                        Or value.varName = eVarNameFlags.mcBbf Or value.varName = eVarNameFlags.mcBAbf _
                        Or value.varName = eVarNameFlags.mcEEbf Or value.varName = eVarNameFlags.mcPBbf Or value.varName = eVarNameFlags.mcQBbf Then

                            value.Status = eStatusFlags.NotEditable

                        End If

                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.PointArray, eValueTypes.BoolArray, eValueTypes.LayerArray
                        Debug.Assert(False, "cMonteCarloGroup should not contain array values.")

                    Case Else
                        'name and other variables
                        value.Status = eStatusFlags.NotEditable

                End Select
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function

#End Region

#Region "dot (.) operators"

    Public Property B() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcB), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcB, value)
        End Set
    End Property

    Public Property BA() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcBA), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcBA, value)
        End Set
    End Property

    Public Property PB() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcPB), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcPB, value)
        End Set
    End Property

    Public Property QB() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcQB), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcQB, value)
        End Set
    End Property

    Public Property EE() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcEE), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcEE, value)
        End Set
    End Property

    'VU = vulnerability (or VulMult) by predator
    Public Property VU() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcVU), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcVU, value)
        End Set
    End Property


    Public Property BLower() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcBLower), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcBLower, value)
        End Set
    End Property

    Public Property BALower() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcBALower), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcBALower, value)
        End Set
    End Property

    Public Property PBLower() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcPBLower), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcPBLower, value)
        End Set
    End Property

    Public Property QBLower() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcQBLower), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcQBLower, value)
        End Set
    End Property

    Public Property EELower() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcEELower), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcEELower, value)
        End Set
    End Property

    Public Property VULower() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcVULower), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcVULower, value)
        End Set
    End Property


    Public Property BUpper() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcBUpper), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcBUpper, value)
        End Set
    End Property

    Public Property BAUpper() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcBAUpper), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcBAUpper, value)
        End Set
    End Property

    Public Property PBUpper() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcPBUpper), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcPBUpper, value)
        End Set
    End Property

    Public Property QBUpper() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcQBUpper), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcQBUpper, value)
        End Set
    End Property

    Public Property EEUpper() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcEEUpper), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcEEUpper, value)
        End Set
    End Property

    Public Property VUUpper() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcVUUpper), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcVUUpper, value)
        End Set
    End Property


    Public Property Bcv() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcBcv), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcBcv, value)
        End Set
    End Property

    Public Property BAcv() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcBAcv), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcBAcv, value)
        End Set
    End Property

    Public Property PBcv() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcPBcv), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcPBcv, value)
        End Set
    End Property

    Public Property QBcv() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcQBcv), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcQBcv, value)
        End Set
    End Property

    Public Property EEcv() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcEEcv), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcEEcv, value)
        End Set
    End Property

    Public Property VUcv() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcVUcv), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcVUcv, value)
        End Set
    End Property


    Public Property Bbf() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcBbf), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcBbf, value)
        End Set
    End Property

    Public Property BAbf() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcBAbf), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcBAbf, value)
        End Set
    End Property

    Public Property PBbf() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcPBbf), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcPBbf, value)
        End Set
    End Property

    Public Property QBbf() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcQBbf), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcQBbf, value)
        End Set
    End Property

    Public Property EEbf() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcEEbf), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcEEbf, value)
        End Set
    End Property

    Public Property VUbf() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.mcVUbf), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.mcVUbf, value)
        End Set
    End Property

#End Region

End Class

#Region "Not used"


'Public Class cMonteCarloGroup
'    Implements EwECore.ICoreInterface

'    Private m_igrp As Integer
'    Private m_name As String
'    Private m_dict As Dictionary(Of eMCTypes, cMCParameter)

'    Friend Sub New()
'        Dim ob As cMCParameter

'        m_dict = New Dictionary(Of eMCTypes, cMCParameter)

'        ob = New cMCParameter(eMCTypes.Biomass)
'        m_dict.Add(ob.Type, ob)

'        ob = New cMCParameter(eMCTypes.BA)
'        m_dict.Add(ob.Type, ob)

'        ob = New cMCParameter(eMCTypes.EE)
'        m_dict.Add(ob.Type, ob)

'        ob = New cMCParameter(eMCTypes.PB)
'        m_dict.Add(ob.Type, ob)

'        ob = New cMCParameter(eMCTypes.QB)
'        m_dict.Add(ob.Type, ob)

'    End Sub


'    Public ReadOnly Property Biomass() As cMCParameter
'        Get
'            Return m_dict.Item(eMCTypes.Biomass)
'        End Get
'    End Property


'    Public ReadOnly Property PB() As cMCParameter
'        Get
'            Return m_dict.Item(eMCTypes.PB)
'        End Get
'    End Property


'    Public ReadOnly Property Parameters() As Dictionary(Of eMCTypes, cMCParameter)
'        Get
'            Return m_dict
'        End Get
'    End Property



'#Region "ICoreInterface"

'    Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
'        Get
'            Return eDataTypes.NotSet
'        End Get
'    End Property

'    Public Property DBID() As Integer Implements ICoreInterface.DBID
'        Get
'            Return cCore.NULL_VALUE
'        End Get
'        Set(ByVal value As Integer)

'        End Set
'    End Property

'    Public Function GetID() As String Implements ICoreInterface.GetID
'        Return Me.ToString
'    End Function

'    Public Property Index() As Integer Implements ICoreInterface.Index
'        Get
'            Return m_igrp
'        End Get
'        Friend Set(ByVal value As Integer)
'            m_igrp = value
'        End Set
'    End Property

'    Public Property Name() As String Implements ICoreInterface.Name
'        Get
'            Return m_name
'        End Get
'        Friend Set(ByVal value As String)
'            m_name = value
'        End Set
'    End Property

'#End Region

'End Class



'Public Class cMCParameter

'    Private m_value As Single
'    Private m_ll As Single
'    Private m_ul As Single
'    Private m_cv As Single
'    Private m_type As eMCTypes


'    Friend Sub New(ByVal theType As eMCTypes)
'        m_type = theType
'    End Sub

'    Public Property Type() As eMCTypes
'        Get
'            Return m_type
'        End Get
'        Set(ByVal newType As eMCTypes)
'            m_type = newType
'        End Set
'    End Property

'    Public Property LowerLimit() As Single
'        Get
'            Return m_ll
'        End Get
'        Set(ByVal value As Single)
'            m_ll = value
'        End Set
'    End Property


'    Public Property UpperLimit() As Single
'        Get
'            Return m_ul
'        End Get
'        Set(ByVal value As Single)
'            m_ul = value
'        End Set
'    End Property

'    Public Property CV() As Single
'        Get
'            Return m_cv
'        End Get
'        Set(ByVal value As Single)
'            m_cv = value
'        End Set
'    End Property

'    Public Property Value() As Single
'        Get
'            Return m_value
'        End Get
'        Set(ByVal value As Single)
'            m_value = value
'        End Set
'    End Property
'End Class

#End Region