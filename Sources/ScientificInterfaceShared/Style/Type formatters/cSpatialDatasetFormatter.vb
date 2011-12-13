#Region " Imports "

Option Strict On
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="ISpatialDataSet"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cSpatialDatasetFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(ISpatialDataSet)
        End Function

        Public Function GetDescriptor(ByVal value As Object, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter.GetDescriptor

            ' ToDo: localize this

            Try
                If (value IsNot Nothing) Then
                    Dim obj As ISpatialDataSet = DirectCast(value, ISpatialDataSet)
                    Select Case descriptor
                        Case eDescriptorTypes.Name
                            If obj.TimeStart <> DateTime.MaxValue Then
                                If obj.TimeEnd <> DateTime.MinValue Then
                                    Return String.Format("{0} ({1} - {2})", obj.Name, obj.TimeStart.ToShortDateString, obj.TimeEnd.ToShortDateString)
                                End If
                                Return String.Format("{0} ({1}-)", obj.Name, obj.TimeStart.ToShortDateString)
                            End If
                        Case eDescriptorTypes.Description
                            Return obj.Description
                    End Select
                    Return obj.Name
                End If

                Return My.Resources.GENERIC_VALUE_NONE

            Catch ex As Exception
                Debug.Assert(False)
            End Try

            Return ""

        End Function

    End Class

End Namespace
