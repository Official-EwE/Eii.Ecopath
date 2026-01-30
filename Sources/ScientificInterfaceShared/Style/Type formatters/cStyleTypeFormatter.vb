' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Text
Imports EwEUtils.Utilities



Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="cStyleGuide.eStyleFlags"/>.
    ''' </summary>
    ''' <remarks>
    ''' <para>This class tries to obtain a string from the ScientificShared resources.
    ''' The resource string is expected to be named and formatted as follows:</para>
    ''' <para>AUTOSAVE_[varname] = "[symbol]|[abbr]|[name]|[description]"</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cStyleTypeFormatter
        Implements ITypeFormatter

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Debug.Assert(value.GetType.IsAssignableFrom(Me.GetDescribedType()))

            Dim sb As New StringBuilder()
            Dim flags As cStyleGuide.eStyleFlags = DirectCast(value, cStyleGuide.eStyleFlags)

            ' Perform bitwise comparision of status flags
            For Each sf As cStyleGuide.eStyleFlags In [Enum].GetValues(GetType(cStyleGuide.eStyleFlags))
                If ((flags And sf) = sf) Then

                    Dim strValue As String = sf.ToString
                    Dim strDescr As String = cResourceUtils.LoadString("STYLEFLAGS_" & strValue.ToUpper, My.Resources.ResourceManager)
                    Dim astrBits As String() = Nothing
                    Dim iNumBits As Integer = 0
                    Dim strBit As String = ""

                    If (strDescr IsNot Nothing) Then
                        astrBits = strDescr.Split("|"c)
                        iNumBits = astrBits.Length
                    End If

                    For i As Integer = 0 To Math.Min(descriptor, iNumBits)

                        If i < iNumBits Then
                            ' Has a part?
                            If Not String.IsNullOrEmpty(astrBits(i)) Then
                                ' #Yes: update bit
                                strBit = astrBits(i).Trim
                            End If
                        End If

                    Next

                    If Not String.IsNullOrWhiteSpace(strBit) Then
                        sb.AppendLine(strBit)
                    End If
                End If
            Next

            Return sb.ToString()

        End Function

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(cStyleGuide.eStyleFlags)
        End Function

    End Class

End Namespace ' Style
