'==============================================================================
'
' $Log: cSingleProperty.vb,v $
' Revision 1.3  2009/04/03 12:08:52  jeroens
' Fixed crash on trying to access non-existing meta data
'
' Revision 1.2  2009/04/02 19:14:43  jeroens
' Invalid values set vars to meta NULL
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
    ''' <see cref="Single">Single</see> value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cSingleProperty
        : Inherits cProperty

        ''' <summary></summary>
        Private m_sValue As Single = 0.0
        Private m_Style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property.
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see>
        ''' instance that is the data source for this property.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">Variable name</see>
        ''' in <paramref name="Source">Source</paramref> that is the data source
        ''' for this property.</param>
        ''' <param name="SourceSec">The object acting as index on
        ''' <paramref name="VarName">VarName</paramref> in case this is an indexed
        ''' variable.</param>
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
        ''' Constructor, initializes the property.
        ''' </summary>
        ''' <param name="id">The ID to assign to the property.</param>
        ''' <remarks>This Constructor is provided to allow for manual creation.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal id As String)
            MyBase.New(id)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns <see cref="Type">type Single</see>, the fixed type of this property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetValueType() As System.Type
            Return GetType(Single)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the property value.
        ''' </summary>
        ''' <param name="bHonourNull">Flag stating whether NULL status flags 
        ''' should return a NULL value.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Value(Optional ByVal bHonourNull As Boolean = True) As Object
            Get
                ' Is this a NULL value?
                If bHonourNull And ((Me.m_Style And StyleGuide.eStyleFlags.Null) = StyleGuide.eStyleFlags.Null) Then
                    ' #Yes: return nothing (NOT 0.0)
                    Return Nothing
                End If
                ' Yes: return true value
                Return Me.m_sValue
            End Get

            Set(ByVal value As Object)

                Dim val As cValue = Me.ValueDescriptor
                Dim meta As cVariableMetaData = Nothing
                Dim s As Single = 0

                If val IsNot Nothing Then
                    meta = val.Metadata
                    If meta IsNot Nothing Then
                        s = CSng(meta.NullValue)
                    End If
                End If

                Try
                    ' Try to convert to single
                    s = Convert.ToSingle(value)
                Catch ex As Exception
                    'Debug.Assert(False, "Unable to convert value to Single")
                End Try

                Me.m_sValue = s

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given value to the the property value.
        ''' </summary>
        ''' <param name="value">The value to compare against the value in the property.</param>
        ''' <returns>True if the values can be considered equal.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsValue(ByVal value As Object) As Boolean
            Try
                Return m_sValue = CSng(value)
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
            Set(ByVal val As StyleGuide.eStyleFlags)
                Me.m_Style = val
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares the <see cref="StyleGuide.eStyleFlags">Style</see> maintained
        ''' in the prothe Style mainta the property Style.
        ''' </summary>
        ''' <param name="Style">The Style to compare.</param>
        ''' <returns>True if the Style equal</returns>
        ''' <remarks>This will need to change to StyleGuide.DisplayStyle</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IsStyle(ByVal Style As StyleGuide.eStyleFlags) As Boolean
            Return Me.m_Style = Style
        End Function

    End Class

End Namespace
