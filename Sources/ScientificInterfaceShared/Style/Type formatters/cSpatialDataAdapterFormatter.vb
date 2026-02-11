' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.SpatialData
Imports EwECore.Style
Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="cSpatialDataAdapter"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cSpatialDataAdapterFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(cSpatialDataAdapter)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Try
                If (value IsNot Nothing) Then
                    Dim fmt As New cVarnameTypeFormatter()
                    Dim obj As cSpatialDataAdapter = DirectCast(value, cSpatialDataAdapter)
                    Return fmt.ToString(obj.VarName)
                End If

                Return My.Resources.GENERIC_VALUE_NONE

            Catch ex As Exception
                Debug.Assert(False)
            End Try

            Return ""

        End Function

    End Class

End Namespace
