' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' <summary>
    ''' Helper class that provides formatting of the the units stored in the
    ''' loaded <see cref="cEwEModel"/>.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class cUnitFormatter

        Private m_core As cCore = Nothing

        Public Sub New(core As cCore)
            Me.m_core = core
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a formatted unit string for a given unit type.
        ''' </summary>
        ''' <param name="unitType">The <see cref="eUnitType"/>to retrieve the 
        ''' formatted string for.</param>
        ''' <returns>A string, or an empty string if something went wrong.</returns>
        ''' -------------------------------------------------------------------
        Public Function UnitString(ByVal unitType As eUnitType) As String

            Dim strUnitString As String = "?"
            Dim model As cEwEModel = Me.m_core.EwEModel

            If (model Is Nothing) Then Return ""

            Select Case unitType
                Case eUnitType.Currency
                    Dim fmt As New cCurrencyUnitFormatter(model.UnitCurrencyCustomText)
                    strUnitString = fmt.GetDescriptor(model.UnitCurrency)

                Case eUnitType.Time
                    Dim fmt As New cTimeUnitFormatter(model.UnitTimeCustomText)
                    strUnitString = fmt.GetDescriptor(model.UnitTime)

                Case eUnitType.Monetary
                    strUnitString = model.UnitMonetary

                Case eUnitType.Nominal
                    strUnitString = "#"

                Case eUnitType.Area
                    Dim fmt As New cAreaUnitFormatter(model.UnitAreaCustomText)
                    strUnitString = fmt.GetDescriptor(model.UnitArea)

                Case eUnitType.Biomass
                    ' ToDo: localize this
                    strUnitString = "unit biomass" ' Fixed

                Case eUnitType.Proportion
                    strUnitString = "prop"

                Case eUnitType.None
                    ' NOP

                Case Else
                    Debug.Assert(False)
            End Select

            Return strUnitString
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Format one or more units to a string.
        ''' </summary>
        ''' <param name="aUnitTypes">An array of units to display.</param>
        ''' <param name="cSeparators">Optional separator characters between the 
        ''' units. Separator 0 is inserted between unit 0 and unit 1; separator
        ''' 1 is inserted between unit 1 and 2, etc. If this parameter is no specified
        ''' all units are separated by a divider '/' character.</param>
        ''' <remarks>
        ''' <example>
        ''' <code>
        ''' Dim fmt As New cUnitFormatter(Me.m_core)
        ''' Dim units As eUnitType() = New eUnitType() {eUnitType.Currency, eUnitType.Area, eUnitType.Time)
        ''' Dim seps as cChar() = New Char() {"/"c, "/"c}
        ''' 
        ''' ' Concatenate the three units into a string
        ''' Console.WriteLine(fmt.Format(units, seps))
        ''' ' This produces the same result
        ''' Console.WriteLine(fmt.Format(units))
        ''' 
        ''' ' Change the first separator character
        ''' seps(0) = "·"c
        ''' ' Whee
        ''' Console.WriteLine(fmt.Format(units, seps))
        ''' </code>
        ''' </example>
        ''' </remarks>
        ''' <returns>A formatted unit string.</returns>
        ''' -------------------------------------------------------------------
        Public Function Format(ByVal aUnitTypes As eUnitType(), _
                               Optional ByVal cSeparators As Char() = Nothing) As String

            Dim str As String = ""
            Dim bNeedSeparators As Boolean = True

            If (cSeparators IsNot Nothing) Then
                bNeedSeparators = (cSeparators.Length = 0)
            End If
            If bNeedSeparators Then cSeparators = New Char() {"/"c}

            For i As Integer = 0 To aUnitTypes.Length - 1

                ' Take reading order into account
                If (EwEUtils.SystemUtilities.cSystemUtils.IsRightToLeft()) Then
                    If i > 0 Then str = cSeparators(Math.Max(0, Math.Min(i, cSeparators.Length) - 1)) & str
                    str = Me.UnitString(aUnitTypes(i)) & str
                Else
                    If i > 0 Then str = str & cSeparators(Math.Max(0, Math.Min(i, cSeparators.Length) - 1))
                    str = str & Me.UnitString(aUnitTypes(i))
                End If
            Next

            Return str

        End Function

    End Class

End Namespace
