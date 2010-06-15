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
            m_coreComponent = eCoreComponentType.EcoPath

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
            val = New cValue(New Integer, eVarNameFlags.NumDigits, eStatusFlags.OK, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NumDigits))
            m_values.Add(val.varName, val)

            ' FirstYear
            meta = New cVariableMetaData(0, 10000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), cCore.NULL_VALUE)
            val = New cValue(New Integer, eVarNameFlags.EcopathFirstYear, eStatusFlags.OK, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NumDigits))
            m_values.Add(val.varName, val)

            ' NumYears
            meta = New cVariableMetaData(1, 10000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), cCore.NULL_VALUE)
            val = New cValue(New Integer, eVarNameFlags.EcopathNumYears, eStatusFlags.OK, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NumDigits))
            m_values.Add(val.varName, val)

            ' North
            meta = New cVariableMetaData(-90, 90, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            val = New cValue(New Single, eVarNameFlags.North, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' South
            meta = New cVariableMetaData(-90, 90, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            val = New cValue(New Single, eVarNameFlags.South, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' East
            meta = New cVariableMetaData(-180, 180, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            val = New cValue(New Single, eVarNameFlags.East, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' West
            meta = New cVariableMetaData(-180, 180, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            val = New cValue(New Single, eVarNameFlags.West, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Author
            meta = New cVariableMetaData(254)
            val = New cValue(New String(desc), eVarNameFlags.AreaName, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' GroupDigits
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.GroupDigits, eStatusFlags.OK, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.GroupDigits))
            m_values.Add(val.varName, val)

            ' Time unit (enum)
            meta = New cVariableMetaData(0, 2, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.UnitTime, eStatusFlags.OK, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.UnitTime))
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
            meta = New cVariableMetaData(0, Double.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Double, eVarNameFlags.LastSaved, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.LastSaved))
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
            SetVariable(eVarNameFlags.NumDigits, iNumDigits)
        End Set
    End Property

    Public Property GroupDigits() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.GroupDigits))
        End Get

        Set(ByVal bGroupDigits As Boolean)
            SetVariable(eVarNameFlags.GroupDigits, bGroupDigits)
        End Set
    End Property

    Public Property UnitTime() As eUnitTimeType
        Get
            Return DirectCast(GetVariable(eVarNameFlags.UnitTime), eUnitTimeType)
        End Get

        Set(ByVal i As eUnitTimeType)
            SetVariable(eVarNameFlags.UnitTime, CInt(i))
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
            SetVariable(eVarNameFlags.UnitCurrency, CInt(i))
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
            SetVariable(eVarNameFlags.UnitMonetary, CInt(i))
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
    ''' Get/set the first year that a model represents.
    ''' </summary>
    Public Property FirstYear() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcopathFirstYear))
        End Get

        Set(ByVal value As Integer)
            Me.SetVariable(eVarNameFlags.EcopathFirstYear, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the number of years that a model represents.
    ''' </summary>
    Public Property NumYears() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcopathNumYears))
        End Get

        Set(ByVal value As Integer)
            Me.SetVariable(eVarNameFlags.EcopathNumYears, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the southern extent of the model bounding box.
    ''' </summary>
    Public Property South() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.South))
        End Get

        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.South, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the northern extent of the model bounding box.
    ''' </summary>
    Public Property North() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.North))
        End Get

        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.North, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the western extent of the model bounding box.
    ''' </summary>
    Public Property West() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.West))
        End Get

        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.West, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the eastern extent of the model bounding box.
    ''' </summary>
    Public Property East() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.East))
        End Get

        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.East, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the name to represent the model area.
    ''' </summary>
    Public Property AreaName() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.AreaName))
        End Get

        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.AreaName, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the Julian date the model was last saved.
    ''' </summary>
    Public Property LastSaved() As Double
        Get
            Return CDbl(GetVariable(eVarNameFlags.LastSaved))
        End Get

        Set(ByVal value As Double)
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
