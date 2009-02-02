'==============================================================================
'
' $Log: cEwEModel.vb,v $
' Revision 1.4  2009/02/02 19:00:16  jeroens
' Hey, why not 60K?! Whoohoo!
'
' Revision 1.3  2009/02/02 18:56:54  jeroens
' Description max length changed to 4K
'
' Revision 1.2  2009/01/16 18:30:11  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:12  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Class to encapsulate and expose ecopath model for a single model
''' </summary>
Public Class cEwEModel
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Sub New(ByRef TheCore As cCore)
        MyBase.New(TheCore)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim desc() As Char

        Try

            m_dataType = eDataTypes.EwEModel
            m_coreComponent = eCoreComponentType.Core

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EwEModel, eCoreComponentType.Core, Index, cCore.NULL_VALUE)

            ' Description
            meta = New cVariableMetaData(60000)
            val = New cValue(New String(desc), eVarNameFlags.Description, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Author
            meta = New cVariableMetaData(60)
            val = New cValue(New String(desc), eVarNameFlags.Author, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Contact
            meta = New cVariableMetaData(250)
            val = New cValue(New String(desc), eVarNameFlags.Contact, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Area
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.Area, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' NumDigits
            meta = New cVariableMetaData(0, 10, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Integer, eVarNameFlags.NumDigits, eStatusFlags.OK, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Time unit (enum)
            meta = New cVariableMetaData(0, 2, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.UnitTime, eStatusFlags.OK, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Time unit (text)
            meta = New cVariableMetaData(20)
            val = New cValue(New String(desc), eVarNameFlags.UnitTimeCustomText, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Currency unit (enum)
            meta = New cVariableMetaData(0, 9, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.UnitCurrency, eStatusFlags.OK, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Currency unit (text)
            meta = New cVariableMetaData(20)
            val = New cValue(New String(desc), eVarNameFlags.UnitCurrencyCustomText, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Monetary unit (enum)
            meta = New cVariableMetaData(0, 161, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.UnitMonetary, eStatusFlags.OK, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Monetary unit (text)
            meta = New cVariableMetaData(20)
            val = New cValue(New String(desc), eVarNameFlags.UnitMonetaryCustomText, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Last saved julian date
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.LastSaved, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.LastSaved))
            m_values.Add(val.varName, val)

            'set status flags to their default values
            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cModel.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cModel. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region " Variable via dot(.) operator"

    Public Property Description() As String
        Get
            Return CStr(getVariable(eVarNameFlags.Description))
        End Get

        Set(ByVal str As String)
            setVariable(eVarNameFlags.Description, str)
        End Set
    End Property

    Public Property Author() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.Author))
        End Get

        Set(ByVal str As String)
            SetVariable(eVarNameFlags.Author, str)
        End Set
    End Property

    Public Property Contact() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.Contact))
        End Get

        Set(ByVal str As String)
            SetVariable(eVarNameFlags.Contact, str)
        End Set
    End Property

    Public Property Area() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.Area))
        End Get

        Set(ByVal sArea As Single)
            SetVariable(eVarNameFlags.Area, sArea)
        End Set
    End Property

    Public Property NumDigits() As Integer
        Get
            Return CInt(getVariable(eVarNameFlags.NumDigits))
        End Get

        Set(ByVal iNumDigits As Integer)
            setVariable(eVarNameFlags.NumDigits, iNumDigits)
        End Set
    End Property

    Public Property UnitTime() As eUnitTimeType
        Get
            Return DirectCast(GetVariable(eVarNameFlags.UnitTime), eUnitTimeType)
        End Get

        Set(ByVal i As eUnitTimeType)
            SetVariable(eVarNameFlags.UnitTime, i)
        End Set
    End Property

    Public Property UnitTimeCustomText() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.UnitTimeCustomText))
        End Get

        Set(ByVal str As String)
            SetVariable(eVarNameFlags.UnitTimeCustomText, str)
        End Set
    End Property

    Public Property UnitCurrency() As eUnitCurrencyType
        Get
            Return DirectCast(GetVariable(eVarNameFlags.UnitCurrency), eUnitCurrencyType)
        End Get

        Set(ByVal i As eUnitCurrencyType)
            SetVariable(eVarNameFlags.UnitCurrency, i)
        End Set
    End Property

    Public Property UnitCurrencyCustomText() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.UnitCurrencyCustomText))
        End Get

        Set(ByVal str As String)
            SetVariable(eVarNameFlags.UnitCurrencyCustomText, str)
        End Set
    End Property

    Public Property UnitMonetary() As eUnitMonetaryType
        Get
            Return DirectCast(GetVariable(eVarNameFlags.UnitMonetary), eUnitMonetaryType)
        End Get

        Set(ByVal i As eUnitMonetaryType)
            SetVariable(eVarNameFlags.UnitMonetary, i)
        End Set
    End Property

    Public Property UnitMonetaryCustomText() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.UnitMonetaryCustomText))
        End Get

        Set(ByVal str As String)
            SetVariable(eVarNameFlags.UnitMonetaryCustomText, str)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the Julian date the model was last saved.
    ''' </summary>
    Public Property LastSaved() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.LastSaved))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.LastSaved, value)
        End Set
    End Property

#End Region ' Variable via dot(.) operator

#Region " Status Flags via dot(.) operator"

    Public Property DescriptionStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Description)
        End Get
        Set(ByVal value As eStatusFlags)
            setStatus(eVarNameFlags.Description, value)
        End Set

    End Property

    Public Property NumDigitsStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.NumDigits)
        End Get
        Set(ByVal value As eStatusFlags)
            setStatus(eVarNameFlags.NumDigits, value)
        End Set

    End Property

#End Region ' Status Flags via dot(.) operator

End Class
