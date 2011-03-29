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
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type Implements ITypeFormatter.GetDescribedType
            Return GetType(cShapeData)
        End Function

        Public Function GetDescriptor(ByVal value As Object, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter.GetDescriptor

            Dim data As cShapeData = TryCast(value, cShapeData)
            Return String.Format(My.Resources.GENERIC_LABEL_INDEXED, data.Index, data.Name)

        End Function

    End Class

End Namespace
