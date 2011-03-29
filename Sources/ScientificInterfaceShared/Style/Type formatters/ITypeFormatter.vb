
Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type specifying the types of descriptors that an ITypeFormatter can return.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Enum eDescriptorTypes As Integer
        ''' <summary>A single letter; the briefest of representations.</summary>
        Symbol = 0
        ''' <summary>An abbreviation.</summary>
        Abbreviation
        ''' <summary>A spelled-out name.</summary>
        Name
        ''' <summary>A full description.</summary>
        Description
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing classes that provide string representations
    ''' for objects and enumerated types in EwE.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ITypeFormatter

        Function GetDescribedType() As Type

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtains a textual representation for an object.
        ''' </summary>
        ''' <param name="value">The object to provide a textual representation for.</param>
        ''' <param name="descriptor">The <see cref="eDescriptorTypes">representation</see> to provide.</param>
        ''' <returns>A textual representation.</returns>
        ''' -------------------------------------------------------------------
        Function GetDescriptor(ByVal value As Object, _
                               Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String

    End Interface

End Namespace
