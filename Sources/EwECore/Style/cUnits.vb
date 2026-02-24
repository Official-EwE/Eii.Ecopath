' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Text
Imports EwEUtils.Utilities

' ToDo: add Distance unit (km, mi, etc), to be used by Area and Mapping

Namespace Style

    ''' <summary>
    ''' Formatter for string-based units
    ''' </summary>
    Public Class cUnits

        Private m_core As cCore = Nothing

        Public Sub New(core As cCore)
            Me.m_core = core
        End Sub

        Public Overloads Function ToString(md As cVariableMetaData) As String
            If (md Is Nothing) Then Return ""
            Return Me.ToString(md.Units)
        End Function

        Public Overloads Function ToString(strUnits As String) As String

            Dim n As Integer = 0
            Dim iStart As Integer = 0
            Dim sb As New StringBuilder()
            Dim bInSep As Boolean = False

            If String.IsNullOrWhiteSpace(strUnits) Then Return String.Empty

            While n < strUnits.Length
                If strUnits(n) = "["c Or strUnits(n) = "]"c Then
                    bInSep = Not bInSep
                    If Not bInSep Then
                        If (n > iStart) Then
                            Dim strBit As String = Me.Format(strUnits.Substring(iStart, n - iStart))
                            sb.Append(strBit)
                        End If
                    Else
                        If (n > iStart) Then
                            Dim strBit As String = strUnits.Substring(iStart, n - iStart)
                            sb.Append(strBit)
                        End If
                    End If
                    iStart = n + 1
                End If
                n += 1
            End While
            If (n > iStart) Then sb.Append(strUnits.Substring(iStart, n - iStart))

            Return sb.ToString()

        End Function

        Private Function Format(str As String) As String

            Dim model As cEwEModel = Me.m_core.EwEModel
            If (model Is Nothing) Then Return ""

            Dim result As String = ""

            Try
                ' Check for predefined user setting
                Select Case "[" & str & "]"

                    Case cUnits.Biomass, cUnits.Currency
                        Dim fmt As New cCurrencyUnitFormatter(model.UnitCurrencyCustomText)
                        result = fmt.ToString(model.UnitCurrency)

                    Case cUnits.Monetary
                        result = model.UnitMonetary

                    Case cUnits.Time
                        Dim fmt As New cTimeUnitFormatter(model.UnitTimeCustomText)
                        result = fmt.ToString(model.UnitTime)

                    Case cUnits.Area
                        Dim fmt As New cAreaUnitFormatter(model.UnitAreaCustomText)
                        result = fmt.ToString(model.UnitArea)

                    Case cUnits.Mapping
                        ' ToDo: make dynamic, make respond to AssumeSquareCells setting
                        Dim fmt As New cMapUnitFormatter()
                        result = fmt.ToString(eUnitMapRefType.dd)

                    Case cUnits.Depth
                        Dim fmt As New cMapUnitFormatter()
                        result = fmt.ToString(eUnitMapRefType.m)

                    Case cUnits.True
                        result = True.ToString()

                    Case cUnits.False
                        result = False.ToString()

                    Case cUnits.Year
                        result = My.Resources.CoreDefaults.UNIT_TIME_YEAR

                    Case Else
                        result = str

                End Select

                Dim strResource As String = cResourceUtils.LoadString("UNIT_" & str.ToUpper(), My.Resources.CoreDefaults.ResourceManager)
                If (Not String.IsNullOrWhiteSpace(strResource)) Then result = strResource

            Catch ex As Exception
                Debug.Assert(False)
            End Try

            ' Unit undefined
            If String.IsNullOrWhiteSpace(result) Then Return str

            Return result

        End Function

        ' -------------------------------
        ' Hard-coded unit keyword masks
        ' -------------------------------

        Public Shared ReadOnly Property Biomass As String = "[biomass]"
        Public Shared ReadOnly Property Currency As String = "[currency]"
        Public Shared ReadOnly Property Time As String = "[time]"
        Public Shared ReadOnly Property Area As String = "[area]"
        Public Shared ReadOnly Property Monetary As String = "[monetary]"
        Public Shared ReadOnly Property MonetaryOverTime As String = "[monetary]/[time]"
        Public Shared ReadOnly Property OverTime As String = "/[time]"
        Public Shared ReadOnly Property CurrencyOverTime As String = "[currency]/[time]"
        Public Shared ReadOnly Property MonetaryOverKg As String = "[monetary]/[kg]"
        Public Shared ReadOnly Property Proportion As String = "[proportion]"
        Public Shared ReadOnly Property Multiplier As String = "[multiplier]"
        Public Shared ReadOnly Property ProportionOverTime As String = "[proportion]/[time]"
        Public Shared ReadOnly Property Mapping As String = "[location]"
        Public Shared ReadOnly Property Depth As String = "[depth]"
        Public Shared ReadOnly Property [True] As String = "[true]"
        Public Shared ReadOnly Property [False] As String = "[false]"
        Public Shared ReadOnly Property TrueFalse As String = "[true]/[false]"
        Public Shared ReadOnly Property PresenceAbsence As String = "[presence]/[absence]"
        Public Shared ReadOnly Property Velocity As String = "[cm]/[sec]"
        Public Shared ReadOnly Property Number As String = "[number]"
        Public Shared ReadOnly Property Year As String = "[year]"
        Public Shared ReadOnly Property FishingEffort As String = "[kilowattdays]/[time]"
        Public Shared ReadOnly Property Percentage As String = "[%]"
        Public Shared ReadOnly Property Contaminants As String = "[contaminants]"

        Public Shared ReadOnly Property MonetaryCurrency As String = cUnits.MonetaryOverKg & " x " & cUnits.Currency

    End Class

End Namespace
