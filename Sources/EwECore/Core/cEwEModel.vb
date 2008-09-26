'==============================================================================
'
' $Log: cEwEModel.vb,v $
' Revision 1.1  2008/09/26 07:30:12  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.14  2008/07/17 19:19:55  jeroens
' Added MonetaryUnit
'
' Revision 1.13  2008/07/10 18:23:47  jeroens
' Fixed units to properly behave
'
' Revision 1.12  2008/05/29 22:22:47  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.11  2008/05/26 18:07:13  jeroens
' Contains time unit, currency unit
'
' Revision 1.10  2008/03/07 18:19:59  jeroens
' Added Ecopath Area
'
' Revision 1.9  2008/01/11 09:53:44  jeroens
' LastSaved date changed to Single to include time
'
' Revision 1.8  2008/01/08 23:13:44  jeroens
' Added LastSaved date
'
' Revision 1.7  2007/10/30 18:40:28  jeroens
' + Added author, contact
'
' Revision 1.6  2007/09/15 21:22:24  jeroens
' no message
'
' Revision 1.5  2006/12/14 23:33:41  jeroens
' - Removed SetStatusFlags; relying on baseclass implementation
'
' Revision 1.4  2006/08/18 15:11:19  joeb
' Renamed ICoreInputOutput.CurrentStatus to ValidationStatus
'
' Revision 1.3  2006/07/20 14:07:02  joeb
' Validation using MetaData and operator classes
'
' Revision 1.2  2006/07/13 19:10:04  joeb
' ICoreInputOutputBase uses a reference to the core instead of a delegates to communicate with the core.
'
' Revision 1.1  2006/07/03 12:29:21  jeroens
' * Was cModel
'
' Revision 1.1  2006/06/30 04:52:17  jeroens
' + Initial version
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

            m_DataType = eDataTypes.EwEModel
            m_messageSource = eMessageSource.Core

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EwEModel, eMessageSource.Core, Index, cCore.NULL_VALUE)

            ' Description
            meta = New cVariableMetaData(250)
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
