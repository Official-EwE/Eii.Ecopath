' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.cEcopathModelFromEcosim
Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="cEcopathModelFromEcosim.eBACalcTypes">Biomass Accumulation calculation types</see>.
    ''' </summary>
    ''' <remarks>
    ''' <para>Resource strings are expected to be formatted as follows:</para>
    ''' <para>BA_CALC_[varname] = "[symbol]|[abbr]|[name]|[description]"</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cBACalcTypeFormatter
        Implements ITypeFormatter

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim vn As eBACalcTypes

            Try
                vn = DirectCast(value, eBACalcTypes)
            Catch ex As Exception
                Return ""
            End Try

            Dim strVar As String = vn.ToString()
            Dim strDescr As String = cResourceUtils.LoadString("BA_CALC_" & strVar.ToUpper, My.Resources.ResourceManager)
            Dim astrBits As String() = Nothing
            Dim iNumBits As Integer = 0
            Dim strBit As String = ""

            If (strDescr IsNot Nothing) Then
                astrBits = strDescr.Split("|"c)
                iNumBits = astrBits.Length
            Else
                Return strVar
            End If

            For i As Integer = 0 To descriptor

                ' Is first part?
                If (i = 0) Then
                    ' #Yes: remember default
                    strBit = strVar
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

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eBACalcTypes)
        End Function

    End Class

End Namespace
