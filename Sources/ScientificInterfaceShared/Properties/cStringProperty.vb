'==============================================================================
'
' $Log: cStringProperty.vb,v $
' Revision 1.2  2009/04/02 15:50:38  jeroens
' Removed assert on set_Value
'
' Revision 1.1  2009/04/02 13:22:09  jeroens
' Separated derived classes out of cProperty.vb
'
'==============================================================================

Option Strict On
Imports EwECore
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style

Namespace Properties

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' cProperty derived class providing access to a strong-typed 
    ''' <see cref="String">String</see> value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cStringProperty
        : Inherits cProperty

        Private m_strValue As String = ""
        Private m_Style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see>
        ''' instance that is the data source for this property.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">Variable name</see> in
        ''' <paramref name="Source">Source</paramref> that is the data source for this
        ''' property.</param>
        ''' <param name="SourceSec">The object acting as index on <paramref name="VarName">VarName</paramref> in case this is an indexed variable.</param>
        ''' <param name="iSecIndexOffset">
        ''' <para>An optional offset that defines the diffence between the index provided by
        ''' <paramref name="srcSec">srcSec</paramref> and the actual storage position in the underlying arrays.
        ''' </para>
        ''' <para>For a detailed description of this variable refer to the constructor description of
        ''' <see cref="cProperty">cProperty</see>
        ''' </para>
        ''' </param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As EwECore.cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As EwECore.cCoreInputOutputBase = Nothing, _
                Optional ByVal iSecIndexOffset As Integer = 0)
            MyBase.New(Source, VarName, SourceSec, iSecIndexOffset)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property
        ''' </summary>
        ''' <param name="id">The ID to assign to the property</param>
        ''' <remarks>This Constructor is provided to allow for manual creation</remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal id As String)
            MyBase.New(id)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Strings anyone? Fresh strings! Going for the gentleman in the blue hat. Going once, going twice...
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetValueType() As System.Type
            Return GetType(String)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the property value
        ''' </summary>
        ''' <param name="bHonourNull">Flag stating whether NULL status flags 
        ''' should return a NULL value.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Value(Optional ByVal bHonourNull As Boolean = True) As Object
            Get
                ' Is this a NULL value?
                If bHonourNull And ((Me.m_Style And StyleGuide.eStyleFlags.Null) = StyleGuide.eStyleFlags.Null) Then
                    ' #Yes: return empty string
                    Return ""
                End If
                Return Me.m_strValue
            End Get
            Set(ByVal value As Object)
                Dim str As String = ""
                Try
                    ' Try to convert to string
                    str = Convert.ToString(value)
                Catch ex As Exception
                    'Debug.Assert(False, "Unable to convert value to String")
                End Try
                Me.m_strValue = str
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given value to the the property value
        ''' </summary>
        ''' <param name="value">The value to compare against the value in the property</param>
        ''' <returns>True if the values can be considered equal</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsValue(ByVal value As Object) As Boolean
            Try
                Return (String.Compare(Me.m_strValue, CStr(value), StringComparison.Ordinal) = 0)
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the Style for the property
        ''' </summary>
        ''' <remarks>This will need to change to StyleGuide.DisplayStyle</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return m_Style
            End Get
            Set(ByVal Style As StyleGuide.eStyleFlags)
                Me.m_Style = Style
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given Style to the property Style
        ''' </summary>
        ''' <param name="Style">The Style to compare</param>
        ''' <returns>True if the Style equal</returns>
        ''' <remarks>This will need to change to StyleGuide.DisplayStyle</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IsStyle(ByVal Style As StyleGuide.eStyleFlags) As Boolean
            Return Me.m_Style = Style
        End Function

    End Class

End Namespace
