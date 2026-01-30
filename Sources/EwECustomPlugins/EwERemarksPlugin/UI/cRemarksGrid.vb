' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Style
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources


Friend Class cRemarksGrid
    Inherits cEwEGrid

#Region " Private vars "

    Private m_data As cProperty() = Nothing

    Private Enum eColumnTypes As Integer
        SourceIndex = 0
        Source
        Parameter
        SourceSec
        Remark
    End Enum

#End Region ' Private vars

#Region " Construction "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region ' Construction

#Region " Public access "

    Public Sub SetData(data() As cProperty)

        Me.m_data = data
        Me.FillData()

    End Sub

#End Region ' Public access

#Region " Grid overrides "

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.SourceIndex) = New cEwEColumnHeaderCell("")
        Me(0, eColumnTypes.Source) = New cEwEColumnHeaderCell(My.Resources.HEADER_SOURCE)
        Me(0, eColumnTypes.Parameter) = New cEwEColumnHeaderCell(My.Resources.HEADER_PARAMETER)
        Me(0, eColumnTypes.SourceSec) = New cEwEColumnHeaderCell(My.Resources.HEADER_SOURCE_SEC)

        Me(0, eColumnTypes.Remark) = New cEwEColumnHeaderCell(SharedResources.HEADER_REMARK)
        Me(0, eColumnTypes.Remark).VisualModel.TextAlignment = Drawing.ContentAlignment.MiddleLeft

        Me.TrackPropertySelection = False
        Me.FixedColumnWidths = False
        Me.AllowBlockSelect = False
        Me.FixedColumns = 4

    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return
        If (Me.m_data Is Nothing) Then Return

        Me.RowsCount = 1

        Dim vfm As New cVarnameTypeFormatter()

        For i As Integer = 0 To Me.m_data.Length - 1

            Dim prop As cProperty = Me.m_data(i)
            Dim cell As cEwECellBase = Nothing
            Dim iRow As Integer = Me.AddRow()

            cell = New cEwERowHeaderCell(CStr(prop.Source.Index))
            cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Names
            Me(iRow, eColumnTypes.SourceIndex) = cell

            cell = New cPropertyRowHeaderCell(Me.PropertyManager, prop.Source, eVarNameFlags.Name)
            cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Names
            Me(iRow, eColumnTypes.Source) = cell

            cell = New cEwERowHeaderCell(vfm.ToString(prop.VarName))
            cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Names
            Me(iRow, eColumnTypes.Parameter) = cell

            cell = New cPropertyRowHeaderCell(Me.PropertyManager, prop.SourceSec, eVarNameFlags.Name)
            cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Names
            Me(iRow, eColumnTypes.SourceSec) = cell

            Me(iRow, eColumnTypes.Remark) = New cRemarkCell(prop)

        Next i

    End Sub

    Protected Overrides Sub FinishStyle()

        Me.Columns(eColumnTypes.SourceIndex).Width = 20
        Me.Columns(eColumnTypes.SourceIndex).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.Columns(eColumnTypes.Source).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.Columns(eColumnTypes.Parameter).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.Columns(eColumnTypes.SourceSec).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.Columns(eColumnTypes.Remark).AutoSizeMode = SourceGrid2.AutoSizeMode.None

        Me.AutoStretchColumnsToFitWidth = True
        Me.StretchColumnsToFitWidth()

        MyBase.FinishStyle()

    End Sub

#End Region ' Grid overrides

End Class
