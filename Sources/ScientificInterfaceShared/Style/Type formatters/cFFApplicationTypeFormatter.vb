' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eForcingFunctionApplication"/>.
    ''' </summary>
    ''' <remarks>
    ''' <para>This class tries to obtain a string from the ScientificShared resources
    ''' to describe a <see cref="eForcingFunctionApplication">Forcing Function 
    ''' application target types</see>.
    ''' The string is expected to be formatted as follows:</para>
    ''' <para>FFAPPLICATIONTARGET_[varname] = "[symbol]|[abbr]|[name]|[description]"</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cFFApplicationTargetTypeFormatter
        Implements ITypeFormatter

        Public Sub New()
        End Sub

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eForcingFunctionApplication)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Debug.Assert(Me.GetDescribedType() Is value.GetType())

            Dim strValue As String = value.ToString
            Dim strDescr As String = cResourceUtils.LoadString("FFAPPLICATIONTARGET_" & strValue.ToUpper, My.Resources.ResourceManager)
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
