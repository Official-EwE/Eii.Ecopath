' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Globalization
Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="RegionInfo"/> 
    ''' currency information.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cMonetaryTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(RegionInfo)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim ri As RegionInfo = DirectCast(value, RegionInfo)
            Dim strDescr As String = ""

            Select Case descriptor
                Case eDescriptorTypes.Symbol
                    strDescr = ri.CurrencySymbol

                Case eDescriptorTypes.Abbreviation
                    strDescr = ri.ISOCurrencySymbol

                Case eDescriptorTypes.Name
                    strDescr = ri.CurrencyEnglishName()

                Case eDescriptorTypes.Description
                    strDescr = ri.CurrencyNativeName

            End Select
            Return strDescr

        End Function

    End Class

End Namespace
