' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style
Imports EwEUtils.Utilities



''' <summary>
''' 
''' </summary>
Public Class cAggregationModeTypeFormatter
    Implements ITypeFormatter

    Public Function GetDescribedType() As System.Type _
        Implements ITypeFormatter.GetDescribedType
        Return GetType(cParameters.eAggregationModeType)
    End Function

    Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String Implements _
        ITypeFormatter.ToString

        Dim strValue As String = value.ToString
        Dim strDescr As String = cResourceUtils.LoadString("AGGREGATION_MODE_" & strValue.ToUpper, Me.GetType.Assembly)
        Dim astrBits As String() = Nothing
        Dim iNumBits As Integer = 0
        Dim strBit As String = ""

        If (strDescr IsNot Nothing) Then
            astrBits = strDescr.Split("|"c)
            iNumBits = astrBits.Length
        End If

        For i As Integer = 0 To Math.Min(descriptor, iNumBits)

            ' Is first part?
            If (i = 0) Then
                ' #Yes: remember default
                strBit = strValue
            End If

            If i < iNumBits Then
                ' Has a part?
                If Not String.IsNullOrEmpty(astrBits(i)) Then
                    ' #Yes: update bit
                    strBit = astrBits(i).Trim
                End If
            End If

        Next
        Return strBit
    End Function

End Class
