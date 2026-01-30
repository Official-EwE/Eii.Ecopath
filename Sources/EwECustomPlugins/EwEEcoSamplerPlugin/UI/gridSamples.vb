' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Samples
Imports EwECore.Style
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources



#Const ShowRatings = 0
#Const ShowPerturbations = 0

Public Class gridSamples
    Inherits cEwEGrid

    Private Enum eColumnTypes As Integer
        Index
        Loaded
        SS
        NumInvalidEE
#If ShowPerturbations Then
        Perturbed
#End If
#If ShowRatings Then
        Rating
#End If
        [Date]
        Source
    End Enum

    ' ToDo: show system column only wben having samples from multiple systems

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        ' ToDo: globalize this
        Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell()
        Me(0, eColumnTypes.Loaded) = New cEwEColumnHeaderCell(My.Resources.HEADER_LOADED)
        Me(0, eColumnTypes.SS) = New cEwEColumnHeaderCell("SS")
        Me(0, eColumnTypes.NumInvalidEE) = New cEwEColumnHeaderCell(My.Resources.HEADER_NUM_INVALID_EE)
#If ShowPerturbations Then
        Me(0, eColumnTypes.Perturbed) = New EwEColumnHeaderCell(My.Resources.HEADER_PERTURBED)
#End If
#If ShowRatings Then
        Me(0, eColumnTypes.Rating) = New EwEColumnHeaderCell(My.Resources.HEADER_RATING)
#End If
        Me(0, eColumnTypes.Date) = New cEwEColumnHeaderCell(My.Resources.HEADER_DATE)
        Me(0, eColumnTypes.Source) = New cEwEColumnHeaderCell(My.Resources.HEADER_SYSTEM)

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False
        Me.Selection.SelectionMode = SourceGrid2.GridSelectionMode.Row
        Me.AllowBlockSelect = False

    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return

        Dim man As cEcopathSampleManager = Me.Core.SampleManager
        Dim strLastSource As String = ""
        Dim bMixedSources As Boolean = False
        Dim strSource As String = ""
        Dim fmt As New cVarnameTypeFormatter()

        For i As Integer = 1 To man.nSamples
            Dim iRow As Integer = Me.AddRow()
            Dim s As cEcopathSample = man.Sample(i)
            Dim cell As cEwECell = Nothing

            Me(iRow, eColumnTypes.Index) = New cPropertyRowHeaderCell(Me.PropertyManager, s, eVarNameFlags.Index)
            Me(iRow, eColumnTypes.Loaded) = New cEwECell("", cStyleGuide.eStyleFlags.NotEditable)

            cell = New cEwECell(s.SS, cStyleGuide.eStyleFlags.NotEditable)
            cell.SuppressZero(0) = True
            Me(iRow, eColumnTypes.SS) = cell

            cell = New cEwECell(s.NumInvalidEE, cStyleGuide.eStyleFlags.NotEditable)
            cell.SuppressZero(0) = True
            Me(iRow, eColumnTypes.NumInvalidEE) = cell

#If ShowPerturbations Then
            Dim sb As New StringBuilder()
            For Each vn As eVarNameFlags In s.PerturbedValues
                If sb.Length > 0 Then sb.Append(" ")
                sb.Append(fmt.GetDescriptor(vn, eDescriptorTypes.Symbol))
            Next
            Me(iRow, eColumnTypes.Perturbed) = New EwECell(sb.ToString, cStyleGuide.eStyleFlags.NotEditable)
#End If
#If ShowRatings Then
            Me(iRow, eColumnTypes.Rating) = New PropertyCell(Me.PropertyManager, s, eVarNameFlags.SampleRating)
#End If
            Me(iRow, eColumnTypes.Date) = New cEwECell(s.Generated, cStyleGuide.eStyleFlags.NotEditable)

            strSource = s.Source
            If (String.Compare(strSource, man.MachineName) = 0) Then strSource = My.Resources.VALUE_THISCOMPUTER

            Me(iRow, eColumnTypes.Source) = New cEwECell(strSource, cStyleGuide.eStyleFlags.NotEditable)
            Me.Sample(iRow) = s

            If Not String.IsNullOrWhiteSpace(s.Source) Then
                If (String.Compare(s.Source, strLastSource) <> 0) Then
                    If Not String.IsNullOrWhiteSpace(strLastSource) Then
                        bMixedSources = True
                    End If
                    strLastSource = s.Source
                End If
            End If
        Next

        Me.UpdateLoadState()

        'Me.Columns(eColumnTypes.Source).Visible = bMixedSources

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
    End Sub

    Public Sub UpdateLoadState()

        Dim man As cEcopathSampleManager = Me.Core.SampleManager

        For iRow As Integer = 1 To Me.RowsCount - 1
            Dim s As cEcopathSample = Me.Sample(iRow)
            Me(iRow, eColumnTypes.Loaded).Value = If(man.IsLoaded(s), SharedResources.GENERIC_VALUE_YES, "")
            Me(iRow, eColumnTypes.SS).Value = s.SS
#If ShowRatings Then
            Me(iRow, eColumnTypes.Rating).Value = s.Rating
#End If
        Next

    End Sub

    Public Property Sample(iRow As Integer) As cEcopathSample
        Get
            If (iRow < 1) Or (iRow >= Me.RowsCount) Then Return Nothing
            Return DirectCast(Me.Rows(iRow).Tag, cEcopathSample)
        End Get
        Private Set(value As cEcopathSample)
            If (iRow < 1) Or (iRow >= Me.RowsCount) Then Return
            Me.Rows(iRow).Tag = value
        End Set
    End Property

End Class
