'==============================================================================
'
' $Log: cBooleanProperty.vb,v $
' Revision 1.4  2009/05/28 12:37:03  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.3  2009/04/04 14:07:26  jeroens
' Value type conversion performed on SetValue to prevent type differences from resulting in premature core data changes
'
' Revision 1.2  2009/04/02 15:50:53  jeroens
' Removed assert on set_Value
'
' Revision 1.1  2009/04/02 13:22:08  jeroens
' Separated derived classes out of cProperty.vb
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style
Imports Microsoft.VisualBasic

#End Region ' Imports

Namespace Properties

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' cProperty derived class providing access to a strong-typed 
    ''' <see cref="Boolean">Boolean</see> value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cBooleanProperty
        : Inherits cProperty

        Private m_bValue As Boolean = False
        Private m_Style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see>
        ''' instance that is the data source for this property.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">Variable name</see> in
        ''' <paramref name="Source">Source</paramref> that is the data source for this
        ''' property.</param>
        ''' <param name="SourceSec">The object acting as index on
        ''' <paramref name="VarName">VarName</paramref> in case this is an
        ''' indexed variable.</param>
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
        ''' Returns <see cref="Type">type Boolean</see>, the fixed type of this property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetValueType() As System.Type
            Return GetType(Boolean)
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
                If bHonourNull And ((Me.m_Style And cStyleGuide.eStyleFlags.Null) = cStyleGuide.eStyleFlags.Null) Then
                    ' #Yes: return nothing (NOT 0.0)
                    Return Nothing
                End If
                Return m_bValue
            End Get
            Set(ByVal objValue As Object)
                Try
                    Me.m_bValue = CBool(objValue)
                Catch ex As Exception

                End Try
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set a Boolean value in the property and commit it to the EwE core.
        ''' </summary>
        ''' <param name="newValue">The value to set.</param>
        ''' <param name="notify">Flag that states whether a change notification
        ''' must be sent out. For an detailed description of values see 
        ''' <see cref="cProperty.SetValue">cProperty.SetValue</see>.</param>
        ''' <returns>A Single value</returns>
        ''' <remarks>
        ''' Overridden to make sure a value is passed to the core as a true Boolean.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function SetValue(ByVal newValue As Object, _
                                           Optional ByVal notify As TriState = TriState.UseDefault) As Boolean

            Dim bValue As Boolean = False
            Try
                ' Try to convert to boolean
                bValue = Convert.ToBoolean(newValue)
            Catch ex As Exception
                'Debug.Assert(False, "Unable to convert value to Boolean")
            End Try

            Return MyBase.SetValue(bValue, notify)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given value to the the property value.
        ''' </summary>
        ''' <param name="value">The value to compare against the value in the property.</param>
        ''' <returns>True if the values can be considered equal.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsValue(ByVal value As Object) As Boolean
            Try
                Return m_bValue = CBool(value)
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the Style for the property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Style() As cStyleGuide.eStyleFlags
            Get
                Return m_Style
            End Get
            Set(ByVal Style As cStyleGuide.eStyleFlags)
                Me.m_Style = Style
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given Style to the property Style.
        ''' </summary>
        ''' <param name="Style">The Style to compare.</param>
        ''' <returns>True if the Styles equal.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IsStyle(ByVal Style As cStyleGuide.eStyleFlags) As Boolean
            Return Me.m_Style = Style
        End Function

    End Class

End Namespace
