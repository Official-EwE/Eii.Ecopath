' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of a <see cref="Char">character</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cCharFormatter
        Implements ITypeFormatter

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Debug.Assert(value.GetType.IsAssignableFrom(Me.GetDescribedType()))

            Dim iChar As Integer = 0
            Dim c As Char = Nothing
            Dim strText As String

            c = DirectCast(value, Char)
            iChar = Convert.ToInt16(c)

            ' Create texttual representation of the char
            Select Case DirectCast(iChar, Keys)

                Case Keys.None, Keys.F1 To Keys.F24
                    strText = DirectCast(iChar, Keys).ToString
                Case Keys.Enter
                    strText = My.Resources.GENERIC_CHAR_ENTER
                Case Keys.Escape
                    strText = My.Resources.GENERIC_CHAR_ESCAPE
                Case Keys.Space
                    strText = My.Resources.GENERIC_CHAR_SPACE
                Case Keys.Tab
                    strText = My.Resources.GENERIC_CHAR_TAB
                Case Else
                    Select Case c
                        Case "."c : strText = My.Resources.GENERIC_CHAR_PERIOD
                        Case ","c : strText = My.Resources.GENERIC_CHAR_COMMA
                        Case ":"c : strText = My.Resources.GENERIC_CHAR_COLON
                        Case ";"c : strText = My.Resources.GENERIC_CHAR_SEMICOLON
                        Case Else
                            strText = c
                    End Select
            End Select
            Return strText

        End Function

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(Char)
        End Function

    End Class

End Namespace ' Style
