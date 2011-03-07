#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eIUCNConservationStatusTypes"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cIUCNConservationTypeFormatter
        Implements ITypeFormatter(Of eIUCNConservationStatusTypes)

        Public Function GetDescriptor(ByVal value As eIUCNConservationStatusTypes, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter(Of eIUCNConservationStatusTypes).GetDescriptor

            Return value.ToString ' Muahaha

        End Function

    End Class

End Namespace
