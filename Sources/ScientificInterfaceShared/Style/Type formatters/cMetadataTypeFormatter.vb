' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Text
Imports EwECore.Style
Imports EwEUtils.Utilities



Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="cVariableMetadata"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cMetadataTypeFormatter
        Implements ITypeFormatter

        Private m_sg As cStyleGuide = Nothing
        Private m_units As cUnits = Nothing

        Public Sub New(core As cCore, sg As cStyleGuide)
            Me.m_sg = sg
            Me.m_units = New cUnits(core)
        End Sub

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            If (value Is Nothing) Then Return ""

            Debug.Assert(value.GetType.IsAssignableFrom(Me.GetDescribedType()))

            Dim md As cVariableMetaData = DirectCast(value, cVariableMetaData)

            Dim strUnits As String = Me.UnitText(md)
            Dim strDescr As String = Me.ValueText(md)

            Dim n As Integer = If(String.IsNullOrWhiteSpace(strUnits), 0, 1) + If(String.IsNullOrWhiteSpace(strDescr), 0, 1)
            Select Case n
                Case 1 : Return strUnits & strDescr
                Case 2 : Return cStringUtils.Localize(My.Resources.GENERIC_LABEL_DOUBLE, strDescr, strUnits)
            End Select

            Return ""

        End Function

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(cVariableMetaData)
        End Function

#Region " Internals "

        Private Function ValueText(md As cVariableMetaData) As String

            ' ToDo: globalize this method

            Dim sbDescr As New StringBuilder()

            Select Case md.VarType
                Case ValueWrapper.eValueTypes.Bool, ValueWrapper.eValueTypes.BoolArray
                    sbDescr.Append(My.Resources.METADATA_BOOLEAN)

                Case ValueWrapper.eValueTypes.Int, ValueWrapper.eValueTypes.IntArray
                    If (md.Min > Integer.MinValue) Then
                        sbDescr.Append(CStr(md.Min) & " " & CStr(If((TypeOf md.MinOperator Is cGreaterThan), "<", "≤")) & " ")
                    End If
                    sbDescr.Append(My.Resources.METADATA_INTEGER)
                    If (md.Max < Integer.MaxValue) Then
                        sbDescr.Append(" " & CStr(If((TypeOf md.MinOperator Is cLessThan), "<", "≤")) & " " & CStr(md.Max))
                    End If

                Case ValueWrapper.eValueTypes.Sng, ValueWrapper.eValueTypes.SingleArray
                    If (md.Min > Single.MinValue) Then
                        sbDescr.Append(CStr(md.Min) & " " & CStr(If((TypeOf md.MinOperator Is cGreaterThan), "<", "≤")) & " ")
                    End If
                    sbDescr.Append(My.Resources.METADATA_SINGLE)
                    If (md.Max < Single.MaxValue) Then
                        sbDescr.Append(" " & CStr(If((TypeOf md.MinOperator Is cLessThan), "<", "≤")) & " " & CStr(md.Max))
                    End If

                Case ValueWrapper.eValueTypes.Str
                    Dim iMax As Integer = Math.Min(CInt(2 ^ 16) - 1, md.Length)
                    sbDescr.Append(String.Format(My.Resources.METADATA_TEXT, iMax))

            End Select

            Return sbDescr.ToString()
        End Function

        Private Function UnitText(md As cVariableMetaData) As String
            Return Me.m_units.ToString(md.Units)
        End Function

#End Region ' Internals

    End Class

End Namespace ' Style
