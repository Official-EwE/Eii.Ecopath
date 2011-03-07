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
    Public Class cCoreInputOutputBaseFormatter
        Implements ITypeFormatter(Of cCoreInputOutputBase)

        Public Function GetDescriptor(ByVal value As EwECore.cCoreInputOutputBase, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter(Of EwECore.cCoreInputOutputBase).GetDescriptor

            If descriptor = eDescriptorTypes.Description Then Return value.Remark(eVarNameFlags.Name)
            Return String.Format(My.Resources.GENERIC_LABEL_INDEXED, value.Index, value.Name)

        End Function

    End Class

End Namespace
