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
    Public Class cAreaUnitFormatter
        Implements ITypeFormatter

        Private m_strCustom As String = ""

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new type <see cref="eUnitCurrencyType"/>formatter.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(strCustom As String)
            Me.m_strCustom = strCustom
        End Sub

        Public Function GetDescribedType() As System.Type Implements ITypeFormatter.GetDescribedType
            Return GetType(eUnitAreaType)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim unit As eUnitAreaType = DirectCast(value, eUnitAreaType)

            Select Case unit
                Case eUnitAreaType.Km2
                    Return My.Resources.CoreDefaults.UNIT_AREA_KM2
                Case eUnitAreaType.Mi2
                    Return My.Resources.CoreDefaults.UNIT_AREA_MI2
                Case eUnitAreaType.Custom
                    Return Me.m_strCustom
            End Select

            Return String.Empty
        End Function

    End Class

End Namespace