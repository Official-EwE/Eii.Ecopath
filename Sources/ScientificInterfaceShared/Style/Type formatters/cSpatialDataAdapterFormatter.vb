#Region " Imports "

Option Strict On
Imports EwEUtils.SpatialData

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="ISpatialDataAdapter"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cSpatialDataAdapterFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(ISpatialDataAdapter)
        End Function

        Public Function GetDescriptor(ByVal value As Object, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter.GetDescriptor

            Try
                If (value IsNot Nothing) Then
                    Dim fmt As New cVarnameTypeFormatter()
                    Dim obj As ISpatialDataAdapter = DirectCast(value, ISpatialDataAdapter)
                    Return fmt.GetDescriptor(obj.VarName)
                End If

                Return My.Resources.GENERIC_VALUE_NONE

            Catch ex As Exception
                Debug.Assert(False)
            End Try

            Return ""

        End Function

    End Class

End Namespace
