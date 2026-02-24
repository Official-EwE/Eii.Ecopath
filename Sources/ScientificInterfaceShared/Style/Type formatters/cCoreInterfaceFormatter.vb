' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of 
    ''' <see cref="cCoreInputOutputBase">cCoreInputOutputBase-derived</see> objects.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cCoreInterfaceFormatter
        Implements ITypeFormatter

        Private m_strNone As String = ""

        Public Sub New()
            Me.New(My.Resources.GENERIC_VALUE_NONE)
        End Sub

        Public Sub New(strNone As String)
            Me.m_strNone = strNone
        End Sub

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(ICoreInterface)
        End Function

        ''' <summary>
        ''' Converts to string.
        ''' </summary>
        ''' <param name="value">The value.</param>
        ''' <param name="descriptor">The descriptor. This flag is interpreted as follows:
        ''' <list>
        ''' <item><term><see cref="eDescriptorTypes.Symbol"/></term><description><see cref="ICoreInterface.Index"/></description></item>
        ''' <item><term><see cref="eDescriptorTypes.Abbreviation"/></term><description><see cref="ICoreInterface.Name"/></description></item>
        ''' <item><term><see cref="eDescriptorTypes.Name"/></term><description>Concatenatio of <see cref="ICoreInterface.Index"/> and <see cref="ICoreInterface.Name"/></description></item>
        ''' <item><term><see cref="eDescriptorTypes.Description"/></term><description>Same as 'name'</description></item>
        ''' </list>
        ''' </param>
        ''' <returns>
        ''' A <see cref="System.String" /> that represents this instance.
        ''' </returns>
        ''' -------------------------------------------------------------------
        ''' -------------------------------------------------------------------
        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            If (value Is Nothing) Then Return Me.m_strNone
            If (Not TypeOf value Is ICoreInterface) Then Return value.ToString

            Try
                Dim obj As ICoreInterface = DirectCast(value, ICoreInterface)
                ' Only include index in desciptor only if object has a valid index
                If (obj.Index >= 1) Then
                    Select Case descriptor
                        Case eDescriptorTypes.Symbol : Return CStr(obj.Index)
                        Case eDescriptorTypes.Abbreviation : Return obj.Name
                        Case Else
                            Return String.Format(My.Resources.GENERIC_LABEL_INDEXED, obj.Index, obj.Name)
                    End Select
                End If
                Return obj.Name
            Catch ex As Exception
                Debug.Assert(False)
            End Try
            Return value.ToString

        End Function

    End Class

End Namespace
