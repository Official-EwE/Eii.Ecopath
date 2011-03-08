#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eOrganismTypes"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cOrganismTypeFormatter
        Implements ITypeFormatter(Of eOrganismTypes)

        Public Function GetDescriptor(ByVal value As eOrganismTypes, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter(Of eOrganismTypes).GetDescriptor

            Return value.ToString ' Muahaha

        End Function

    End Class

End Namespace
