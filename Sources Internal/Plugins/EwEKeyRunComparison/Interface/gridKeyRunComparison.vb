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
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Style.cStyleGuide

#End Region ' Imports

Public Class gridKeyRunComparison
    Inherits EwEGrid

#Region " Private classes "

    Private Class cHashResultPairSort
        Implements IComparer(Of cHashResultPair)

        Public Function Compare(x As cHashResultPair, y As cHashResultPair) As Integer _
            Implements System.Collections.Generic.IComparer(Of cHashResultPair).Compare

            If (x.MatchedState < y.MatchedState) Then Return -1
            If (x.MatchedState > y.MatchedState) Then Return 1

            If (x.SortOrder < y.SortOrder) Then Return -1
            If (x.SortOrder > y.SortOrder) Then Return 1

            Return 0

        End Function

    End Class
#End Region ' Private classes

#Region " Private vars "

    Private Enum eColumnTypes As Integer
        Alert = 0
        Component
        Name
        Status
    End Enum

#End Region ' Private vars

#Region " Construction "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region ' Construction

#Region " Grid overrides "

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Alert) = New EwEColumnHeaderCell(My.Resources.HEADER_ALERT)
        Me(0, eColumnTypes.Component) = New EwEColumnHeaderCell(My.Resources.HEADER_COMPONENT)
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_ITEM)
        Me(0, eColumnTypes.Status) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)

        Me.FixedColumns = 1
        Me.Columns(eColumnTypes.Alert).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.Columns(eColumnTypes.Alert).Width = 20

        Me.AutoStretchColumnsToFitWidth = True
        Me.AllowBlockSelect = False

    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return
        If (Me.ComparisonManager Is Nothing) Then Return
        If (Me.ComparisonManager.Results Is Nothing) Then Return

        Dim results As cHashResultPair() = Me.ComparisonManager.Results.HashPairs.ToArray()
        Dim result As cHashResultPair = Nothing

        Array.Sort(results, New cHashResultPairSort())

        For i = 0 To results.Length - 1
            result = results(i)
            If (Me.ShowErrorsOnly = False) Or (result.MatchedState <> cHashResultPair.eMatchState.Equal) Then
                Me.AddHashRow(result)
            End If
        Next

    End Sub

    Protected Overrides Sub FinishStyle()

        MyBase.FinishStyle()

    End Sub

#End Region ' Grid overrides

#Region " Public interfaces "

    Public Property ComparisonManager As cCompareManager

    Public Property ShowErrorsOnly As Boolean

#End Region ' Public interfaces

#Region " Internals "

    Private Sub AddHashRow(pair As cHashResultPair)

        Dim iRow As Integer = Me.AddRow()
        Dim fmtC As New cComponentFormatter()
        Dim fmtV As New cVariableFormatter()
        Dim fmtM As New cMatchStateTypeFormatter()
        Dim viz As New cAlertVisualizer()

        ' ToDo: add images to column Alert

        Me(iRow, eColumnTypes.Component) = New EwERowHeaderCell(fmtC.GetDescriptor(pair.Component))
        Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(fmtV.GetDescriptor(pair.VariableID))
        Me(iRow, eColumnTypes.Alert) = New EwECell(CBool(pair.isMatch), GetType(Boolean), eStyleFlags.NotEditable)
        Me(iRow, eColumnTypes.Alert).VisualModel = viz
        Me(iRow, eColumnTypes.Status) = New EwECell(fmtM.GetDescriptor(pair.MatchedState), GetType(String), eStyleFlags.NotEditable)

    End Sub

#End Region ' Internals

End Class
