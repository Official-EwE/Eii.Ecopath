#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="cShapeData">shapes</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cShapeDataFormatter
        Implements ITypeFormatter(Of cShapeData)

        Public Function GetDescriptor(ByVal value As cShapeData, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter(Of cShapeData).GetDescriptor

            Return String.Format(My.Resources.GENERIC_LABEL_INDEXED, value.Index, value.Name)

        End Function

    End Class

End Namespace
