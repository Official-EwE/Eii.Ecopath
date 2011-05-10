#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of 
    ''' <see cref="cCoreInputOutputBase">cCoreInputOutputBase-derived</see> objects.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cCoreInterfaceFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(cCoreInputOutputBase)
        End Function

        Public Function GetDescriptor(ByVal value As Object, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter.GetDescriptor
            Try
                Dim obj As ICoreInterface = DirectCast(value, ICoreInterface)
                Return String.Format(My.Resources.GENERIC_LABEL_INDEXED, obj.Index, obj.Name)
            Catch ex As Exception
                Debug.Assert(False)
            End Try
            Return value.ToString

        End Function

    End Class

End Namespace
