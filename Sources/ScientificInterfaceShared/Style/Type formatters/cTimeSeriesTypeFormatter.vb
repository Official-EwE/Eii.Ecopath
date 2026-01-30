' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities



Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eTimeSeriesType">time series types</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cTimeSeriesTypeFormatter
        Implements ITypeFormatter

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim ts As eTimeSeriesType = DirectCast(value, eTimeSeriesType)
            Dim strDescr As String = cResourceUtils.LoadString("TS_TYPE_" & ts.ToString().ToUpper, My.Resources.ResourceManager)

            Dim strApplication As String = ""
            Dim strScale As String = ""
            Dim strBit As String = ""
            Dim bits As String() = Nothing
            Dim iNumBits As Integer = 0

            If (Not String.IsNullOrWhiteSpace(strDescr)) Then
                bits = strDescr.Split("|"c)
                iNumBits = bits.Length
            Else
                Debug.Assert(False, "ts type " & ts.ToString() & " not localized")
                Return ts.ToString
            End If

            For i As Integer = 0 To descriptor
                ' Is first part?
                If (i = 0) Then
                    ' #Yes: remember default
                    strBit = strDescr
                End If

                If i < iNumBits Then
                    ' Has a part?
                    If Not String.IsNullOrEmpty(bits(i)) Then
                        ' #Yes: update bit
                        strBit = bits(i).Trim
                    End If
                End If
            Next

            Select Case descriptor
                Case eDescriptorTypes.Description
                    strApplication = If(cTimeSeries.IsDriver(ts), My.Resources.VALUE_GENERIC_FORCING, My.Resources.VALUE_GENERIC_REFERENCE)
                    strScale = If(cTimeSeries.IsAbsolute(ts), My.Resources.VALUE_GENERIC_ABSOLUTE, My.Resources.VALUE_GENERIC_RELATIVE)
                Case Else
                    strApplication = If(cTimeSeries.IsDriver(ts), My.Resources.VALUE_GENERIC_FORCING_ABBR, My.Resources.VALUE_GENERIC_REFERENCE_ABBR)
                    strScale = If(cTimeSeries.IsAbsolute(ts), My.Resources.VALUE_GENERIC_ABSOLUTE_ABBR, My.Resources.VALUE_GENERIC_RELATIVE_ABBR)
                    If (descriptor < eDescriptorTypes.Name) Then
                        strApplication = strApplication.Substring(0, 1)
                        strScale = strScale.Substring(0, 1)
                    End If
            End Select

            Return cStringUtils.Localize(My.Resources.GENERIC_LABEL_POINT, strBit, strApplication, strScale)

        End Function

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eTimeSeriesType)
        End Function

    End Class

End Namespace
