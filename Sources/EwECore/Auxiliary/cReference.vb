#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper

#End Region ' Imports

Namespace Auxiliary

#If USE_REFERENCES Then

    ''' <summary>
    ''' Helper class to hold a reference to a publication. About to fall over the
    ''' edge of the world, not really supported in EwE6 but also not really
    ''' discarded yet.
    ''' </summary>
    Public Class cReference
        Inherits cCoreInputOutputBase

#Region " Construction "

        Public Sub New(ByRef core As cCore, ByVal iDBID As Integer)
            MyBase.New(core)

            Dim val As cValue = Nothing
            Dim meta As cVariableMetaData = Nothing
            Dim validator As cValidatorDefault = Nothing

            Me.DBID = iDBID
            m_DataType = eDataTypes.Reference

            'all variables use the default validator
            validator = m_core.m_validators.getValidator(eVarNameFlags.NotSet)

            meta = New cVariableMetaData(255)
            val = New cValue("", eVarNameFlags.Authors, eStatusFlags.OK, eValueTypes.Str, meta, validator)
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue("", eVarNameFlags.Year, eStatusFlags.OK, eValueTypes.Str, meta, validator)
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(255)
            val = New cValue("", eVarNameFlags.Title, eStatusFlags.OK, eValueTypes.Str, meta, validator)
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(255)
            val = New cValue("", eVarNameFlags.Source, eStatusFlags.OK, eValueTypes.Str, meta, validator)
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(255)
            val = New cValue("", eVarNameFlags.Keywords, eStatusFlags.OK, eValueTypes.Str, meta, validator)
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(100)
            val = New cValue("", eVarNameFlags.QuickRef, eStatusFlags.OK, eValueTypes.Str, meta, validator)
            m_values.Add(val.varName, val)

        End Sub

#End Region ' Construction

#Region " Properties by dot(.) operator "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="eVarNameFlags.Authors">author(s)</see> of a 
        ''' reference.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Authors() As String
            Get
                Return CStr(GetVariable(eVarNameFlags.Authors))
            End Get

            Set(ByVal newValue As String)
                SetVariable(eVarNameFlags.Authors, newValue)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="eVarNameFlags.year">publication year</see>
        ''' of a reference.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Year() As Integer
            Get
                Return CInt(GetVariable(eVarNameFlags.Year))
            End Get

            Set(ByVal newValue As Integer)
                SetVariable(eVarNameFlags.Year, newValue)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="eVarNameFlags.Title">title</see> of a 
        ''' reference.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Title() As String
            Get
                Return CStr(GetVariable(eVarNameFlags.Title))
            End Get

            Set(ByVal newValue As String)
                SetVariable(eVarNameFlags.Title, newValue)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="eVarNameFlags.Source">source</see> of a 
        ''' reference.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Source() As String
            Get
                Return CStr(GetVariable(eVarNameFlags.Source))
            End Get

            Set(ByVal newValue As String)
                SetVariable(eVarNameFlags.Source, newValue)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the comma-separted sequence of 
        ''' <see cref="eVarNameFlags.Keywords">keywords</see> of a reference.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Keywords() As String
            Get
                Return CStr(GetVariable(eVarNameFlags.Keywords))
            End Get

            Set(ByVal newValue As String)
                SetVariable(eVarNameFlags.Keywords, newValue)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="eVarNameFlags.QuickRef">short representation</see>
        ''' of a reference.
        ''' </summary>
        ''' <remarks>
        ''' Preferrably this field should be left empty so its content can be 
        ''' constructed at runtime. However, EwE5 databases can contain references
        ''' with only a quickref value set. This behaviour unfortunately must
        ''' be carried over to EwE6.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property QuickRef() As String
            Get
                Return CStr(GetVariable(eVarNameFlags.QuickRef))
            End Get

            Set(ByVal newValue As String)
                SetVariable(eVarNameFlags.QuickRef, newValue)
            End Set
        End Property

#End Region ' Properties by dot(.) operator

    End Class

#End If

End Namespace
