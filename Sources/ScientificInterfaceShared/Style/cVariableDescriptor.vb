#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of core variables.
    ''' </summary>
    ''' <remarks>
    ''' <para>This class tries to obtain a string from the ScientificShared resources
    ''' to describe a <see cref="eVarNameFlags">core variable</see>. The string is
    ''' expected to be formatted as follows:</para>
    ''' <para>VARIABLE_[varname] = "[name]|[description]"</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cVariableDescriptor

#Region " Private vars "

        Private m_varname As eVarNameFlags = eVarNameFlags.NotSet
        Private m_strName As String = ""
        Private m_strDescription As String = ""

#End Region ' Private vars

#Region " Internals "

        Private Sub New(ByVal vn As eVarNameFlags)

            Dim strDescr As String = ""

            Me.m_varname = vn
            Me.m_strName = Me.m_varname.ToString

            Try
                strDescr = cResourceUtils.LoadString("VARIABLE_" & vn.ToString.ToUpper, GetType(cVariableDescriptor).Assembly)
                If (strDescr IsNot Nothing) Then
                    Dim astrBits As String() = strDescr.Split("|"c)
                    If astrBits.Length > 0 Then If Not String.IsNullOrEmpty(astrBits(0)) Then Me.m_strName = astrBits(0).Trim
                    If astrBits.Length > 1 Then If Not String.IsNullOrEmpty(astrBits(1)) Then Me.m_strDescription = astrBits(1).Trim
                End If
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Internals

#Region " Public interfaces "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialze a new variable descriptor.
        ''' </summary>
        ''' <param name="vn">The <see cref="eVarNameFlags">variable</see> to describe.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function FromVarname(ByVal vn As eVarNameFlags) As cVariableDescriptor
            Return New cVariableDescriptor(vn)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get a humen legible name for a variable.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Name() As String
            Get
                Return Me.m_strName
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get a humen legible description for a variable.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Description() As String
            Get
                Return Me.m_strDescription
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eVarNameFlags">variable</see> for this descriptor.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Varname() As eVarNameFlags
            Get
                Return Me.m_varname
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Oh! Ah!
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Function ToString() As String
            Return Me.Name
        End Function

#End Region ' Public interfaces

    End Class

End Namespace ' Style
