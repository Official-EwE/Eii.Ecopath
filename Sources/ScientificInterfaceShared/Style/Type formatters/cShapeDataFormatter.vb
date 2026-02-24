' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="cShapeData">shapes</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cShapeDataFormatter
        Implements ITypeFormatter

        Private m_strNone As String = ""

        Public Sub New()
            Me.New(My.Resources.GENERIC_VALUE_NONE)
        End Sub

        Public Sub New(strNone As String)
            Me.m_strNone = strNone
        End Sub

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            If (value Is Nothing) Then Return Me.m_strNone
            If (Not TypeOf value Is cShapeData) Then Return Me.m_strNone

            Try
                Dim obj As cShapeData = DirectCast(value, cShapeData)
                ' Only include index in descriptor only if object has a valid index
                If (obj.Index >= 1) Then
                    Return String.Format(My.Resources.GENERIC_LABEL_INDEXED, obj.Index, obj.Name)
                End If
                Return obj.Name
            Catch ex As Exception
                Debug.Assert(False)
            End Try
            Return value.ToString

        End Function

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(cShapeData)
        End Function

    End Class

End Namespace ' Style
