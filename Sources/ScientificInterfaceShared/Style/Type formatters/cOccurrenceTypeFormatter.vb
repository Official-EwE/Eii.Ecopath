#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eOccurrenceStatusTypes"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cOccurrenceTypeFormatter
        Implements ITypeFormatter(Of eOccurrenceStatusTypes)

        Public Function GetDescriptor(ByVal value As eOccurrenceStatusTypes, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter(Of eOccurrenceStatusTypes).GetDescriptor

            Return value.ToString ' Muahaha

        End Function

    End Class

End Namespace
