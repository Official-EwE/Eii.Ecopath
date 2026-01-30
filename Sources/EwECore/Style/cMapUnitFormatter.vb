' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities



Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eUnitAreaType"/> objects.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cMapUnitFormatter
        Implements ITypeFormatter

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new type <see cref="cMapUnitFormatter"/>formatter.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
        End Sub

        Public Function GetDescribedType() As System.Type Implements ITypeFormatter.GetDescribedType
            Return GetType(eUnitMapRefType)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim unit As eUnitMapRefType = DirectCast(value, eUnitMapRefType)

            Select Case unit
                Case eUnitMapRefType.m
                    Return My.Resources.CoreDefaults.UNIT_METER
                Case eUnitMapRefType.km
                    Return My.Resources.CoreDefaults.UNIT_KILOMETER
                Case eUnitMapRefType.dd
                    Return My.Resources.CoreDefaults.UNIT_DECIMALDEGREE
            End Select

            Return String.Empty
        End Function

    End Class
End Namespace