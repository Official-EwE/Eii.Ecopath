' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eUnitCurrencyType"/> objects.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cCurrencyUnitFormatter
        Implements ITypeFormatter

        Private m_strCustom As String = ""

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new type <see cref="eUnitCurrencyType"/>formatter.
        ''' </summary>
        ''' <param name="strCustom">Any custom unit text as entered by the user. 
        ''' by the user.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(strCustom As String)
            Me.m_strCustom = strCustom
        End Sub

        Public Function GetDescribedType() As System.Type Implements ITypeFormatter.GetDescribedType
            Return GetType(eUnitCurrencyType)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim unit As eUnitCurrencyType = DirectCast(value, eUnitCurrencyType)

            Select Case unit
                Case eUnitCurrencyType.Calorie
                    Return My.Resources.CoreDefaults.UNIT_CURRENCY_CALORIE
                Case eUnitCurrencyType.Carbon
                    Return My.Resources.CoreDefaults.UNIT_CURRENCY_CARBON
                Case eUnitCurrencyType.DryWeight
                    Return My.Resources.CoreDefaults.UNIT_CURRENCY_DRYWEIGHT
                Case eUnitCurrencyType.Joules
                    Return My.Resources.CoreDefaults.UNIT_CURRENCY_JOULES
                Case eUnitCurrencyType.Nitrogen
                    Return My.Resources.CoreDefaults.UNIT_CURRENCY_NITROGEN
                Case eUnitCurrencyType.Phosporous
                    Return My.Resources.CoreDefaults.UNIT_CURRENCY_PHOSPOROUS
                Case eUnitCurrencyType.WetWeight
                    Return My.Resources.CoreDefaults.UNIT_CURRENCY_WETWEIGHT
            End Select

            If Not String.IsNullOrWhiteSpace(Me.m_strCustom) Then
                Return Me.m_strCustom
            End If

            Return String.Empty
        End Function

    End Class

End Namespace