'==============================================================================
'
' $Log: cVariableMetaData.vb,v $
' Revision 1.1  2008/09/26 07:30:13  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2007/06/25 15:03:44  joeb
' Changed DefaultValue() to NullValue()
'
' Revision 1.3  2007/06/15 14:59:53  jeroens
' - Handled pending VERIFY
'
' Revision 1.2  2006/12/14 23:28:47  jeroens
' + Added DefaultValue
'
'==============================================================================

Option Strict On

''' <summary>
''' Meta Data for a varaible.
''' </summary>
''' <remarks>At this time this is only used by a cValue object during variable validation. It is not exposed to an interface.</remarks>
Public Class cVariableMetaData

    'variables use for numeric values
    Private m_min As Single = 0
    Private m_max As Single = 0
    Private m_MinOperator As cOperatorBase = Nothing
    Private m_MaxOperator As cOperatorBase = Nothing
    ''' <summary>Default value for variable when a value is missing or in error.</summary>
    Private m_nullvalue As Object = Nothing

    'variables for strings
    Private m_length As Integer

#Region "Constructors"

    ''' <summary>
    ''' Default constructor use for boolean values.
    ''' </summary>
    ''' <param name="ValueDefault">Default value to assign to variable when in error.</param>
    ''' <remarks></remarks>
    Sub New(Optional ByVal ValueDefault As Boolean = False)
        m_length = 0
        m_nullvalue = ValueDefault
    End Sub

    ''' <summary>
    ''' Constuctor for a String MetaData object.
    ''' </summary>
    ''' <param name="Length"></param>
    ''' <param name="ValueDefault">Default value to assign to variable when in error.</param>
    ''' <remarks>Strings do not have Min Max or Null values</remarks>
    Sub New(ByVal Length As Integer, Optional ByVal ValueDefault As String = "")
        m_length = Length
        m_nullvalue = ValueDefault
    End Sub

    ''' <summary>
    ''' Constructor for a numeric value that uses the default for NullValue.
    ''' </summary>
    ''' <param name="Min"></param>
    ''' <param name="Max"></param>
    ''' <param name="MinOperator"></param>
    ''' <param name="MaxOperator"></param>
    ''' <param name="ValueDefault">Default value to assign to variable when in error.</param>
    ''' <remarks></remarks>
    Sub New(ByVal Min As Single, ByVal Max As Single, ByRef MinOperator As cOperatorBase, ByRef MaxOperator As cOperatorBase, _
            Optional ByVal ValueDefault As Single = 0)
        m_min = Min
        m_max = Max
        m_MinOperator = MinOperator
        m_MaxOperator = MaxOperator
        m_nullvalue = ValueDefault
    End Sub

#End Region

#Region "Operators"

    Friend Property MinOperator() As cOperatorBase
        Get
            Return m_MinOperator
        End Get
        Set(ByVal value As cOperatorBase)
            m_MinOperator = value
        End Set
    End Property

    Friend Property MaxOperator() As cOperatorBase
        Get
            Return m_MaxOperator
        End Get
        Set(ByVal value As cOperatorBase)
            m_MaxOperator = value
        End Set
    End Property

#End Region

#Region "Properties"
    'Properties are Public read and Friend write at this time this is by design.
    'If the are exposed by the core they should not be editable. However it may be desirable for a datasource to set the value. 
    'Although this may not be practical.

    Public Property Min() As Single
        Get
            Return m_min
        End Get
        Friend Set(ByVal value As Single)
            m_min = value
        End Set
    End Property

    Public Property Max() As Single
        Get
            Return m_max
        End Get
        Friend Set(ByVal value As Single)
            m_max = value
        End Set
    End Property

    Public Property NullValue() As Object
        Get
            Return m_nullvalue
        End Get
        Friend Set(ByVal value As Object)
            m_nullvalue = value
        End Set
    End Property

    Public Property Length() As Integer
        Get
            Return m_length
        End Get
        Friend Set(ByVal value As Integer)
            m_length = value
        End Set
    End Property

#End Region

End Class

