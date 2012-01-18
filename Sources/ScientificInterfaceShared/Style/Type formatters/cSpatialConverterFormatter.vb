#Region " Imports "

Option Strict On
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="ISpatialDataConverter"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cSpatialConverterFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(ISpatialDataConverter)
        End Function

        Public Function GetDescriptor(ByVal value As Object, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter.GetDescriptor

            Try
                If (value IsNot Nothing) Then
                    Dim obj As ISpatialDataConverter = DirectCast(value, ISpatialDataConverter)
                    If (descriptor = eDescriptorTypes.Description) Then Return obj.Description
                    Return obj.DisplayName
                End If
                Return My.Resources.GENERIC_VALUE_NONE
            Catch ex As Exception
                Debug.Assert(False)
            End Try

            Return ""

        End Function

    End Class

End Namespace
