' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities



Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eIUCNConservationStatusTypes"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cIUCNConservationTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eIUCNConservationStatusTypes)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim val As eIUCNConservationStatusTypes = DirectCast(value, eIUCNConservationStatusTypes)

            If val = eIUCNConservationStatusTypes.NotSet Then Return ""

            Dim strValue As String = val.ToString
            Dim strDescr As String = cResourceUtils.LoadString("CONSERVATIONSTATUS_" & strValue.ToUpper, My.Resources.ResourceManager)
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

End Namespace
