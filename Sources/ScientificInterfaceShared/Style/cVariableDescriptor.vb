#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports EwECore

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
    ''' <para>VARIABLE_[varname] = "[symbol]|[abbr]|[name]|[description]"</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cVariableDescriptor

#Region " Private vars "

        Private m_varname As eVarNameFlags = eVarNameFlags.NotSet
        Private m_astrBits([Enum].GetValues(GetType(eDescriptorTypes)).Length) As String

#End Region ' Private vars

#Region " Internals "

        Private Sub New(ByVal vn As eVarNameFlags)

            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

            Me.m_varname = vn

            Dim strDescr As String = cResourceUtils.LoadString("VARIABLE_" & vn.ToString.ToUpper, GetType(cVariableDescriptor).Assembly)
            Dim astrBits As String() = Nothing
            Dim iNumBits As Integer = 0

            If (strDescr IsNot Nothing) Then
                astrBits = strDescr.Split("|"c)
                iNumBits = astrBits.Length
            End If

            For i As Integer = 0 To Me.m_astrBits.Length - 1
                Dim strBit As String = ""

                If (i = 0) Then
                    ' #No: is first part, copy varname
                    strBit = cin.GetVarName(vn)
                Else
                    ' #No: is consecutive part, inherit previous part value
                    strBit = Me.m_astrBits(i - 1)
                End If

                If i < iNumBits Then
                    ' Has a part?
                    If Not String.IsNullOrEmpty(astrBits(i)) Then
                        ' #Yes: use this
                        strBit = astrBits(i).Trim
                    End If
                End If

                Me.m_astrBits(i) = strBit
            Next

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
        ''' Enumerated type to identify descriptor types.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Enum eDescriptorTypes As Integer
            Symbol = 0
            Abbreviation
            Name
            Description
        End Enum

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get a variable descriptor part.
        ''' </summary>
        ''' <param name="part"></param>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property GetDescription(ByVal part As eDescriptorTypes) As String
            Get
                Return Me.m_astrBits(part)
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get a humen legible name for a variable.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Name() As String
            Get
                Return Me.GetDescription(eDescriptorTypes.Name)
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get a symbol for a variable.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Symbol() As String
            Get
                Return Me.GetDescription(eDescriptorTypes.Symbol)
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get a humen legible description for a variable.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Abbreviation() As String
            Get
                Return Me.GetDescription(eDescriptorTypes.Abbreviation)
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get a humen legible description for a variable.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Description() As String
            Get
                Return Me.GetDescription(eDescriptorTypes.Description)
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
