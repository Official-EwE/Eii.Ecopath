' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities



Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of core variables.
    ''' </summary>
    ''' <remarks>
    ''' <para>This class tries to obtain a string from the ScientificShared resources.
    ''' The resource string is expected to be named and formatted as follows:</para>
    ''' <para>VARIABLE_[varname] = "[symbol]|[abbr]|[name]|[description]"</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cVarnameTypeFormatter
        Implements ITypeFormatter

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim vn As eVarNameFlags = eVarNameFlags.NotSet

            Try
                vn = DirectCast(value, eVarNameFlags)
            Catch ex As Exception
                Return ""
            End Try

            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
            Dim strVar As String = cin.GetVarName(vn)
            Dim strDescr As String = cResourceUtils.LoadString("VARIABLE_" & strVar.ToUpper, My.Resources.CoreDefaults.ResourceManager)
            Dim bits As String() = Nothing
            Dim iNumBits As Integer = 0
            Dim strBit As String = ""

            If (Not String.IsNullOrWhiteSpace(strDescr)) Then
                bits = strDescr.Split("|"c)
                iNumBits = bits.Length
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
                    If Not String.IsNullOrEmpty(bits(i)) Then
                        ' #Yes: update bit
                        strBit = bits(i).Trim
                    End If
                End If

            Next
            Return strBit

        End Function

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eVarNameFlags)
        End Function

    End Class

End Namespace ' Style
