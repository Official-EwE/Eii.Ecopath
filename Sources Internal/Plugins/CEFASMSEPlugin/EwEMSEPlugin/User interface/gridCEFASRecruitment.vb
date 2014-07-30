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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports ScientificInterfaceShared.Controls.EwEGrid

Imports ScientificInterfaceShared.Style


#End Region ' Imports



''' ===========================================================================
''' <summary>
''' Grid to allow species quota interaction.
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class gridCEFASRecruitment
    Inherits EwEGrid

    Private m_Assessment As cStockAssessmentModel

#Region " Internal defs "

    Private Enum eColumnTypes As Integer
        Index = 0
        Name
        ForcastGain
        RHalfB
        RecruitmentCV
    End Enum

#End Region ' Internal defs

#Region " Constructor "

    Public Sub New()
        MyBase.new()

    End Sub

    Public Sub Init(ByVal StockAssessmentModel As cStockAssessmentModel)
        m_Assessment = StockAssessmentModel
    End Sub

#End Region ' Constructor

#Region " Public interfaces "

    Public Property Group() As cStockAssessmentParameters
        Get
            Try

                Dim iRow As Integer = Me.SelectedRow
                If (iRow > 0) Then
                    Return DirectCast(Me.Rows(iRow).Tag, cStockAssessmentParameters)
                End If
            Catch ex As Exception
                Debug.Assert(False, "Invalid cast!!!! maybe..." & ex.Message)
            End Try
            Return Nothing

        End Get
        Set(ByVal value As cStockAssessmentParameters)
            Me.Selection.Clear()
            If value IsNot Nothing Then
                Me.Selection.Add(New Position(value.iGroupIndex, 0))
            End If
            Me.RaiseSelectionChangeEvent()
        End Set
    End Property

#End Region ' Public interfaces

#Region " Overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length

        Me.Redim(1, iNumCols)

        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
        Me(0, eColumnTypes.RHalfB) = New EwEColumnHeaderCell(SharedResources.HEADER_RHALFB0RATIO)
        Me(0, eColumnTypes.ForcastGain) = New EwEColumnHeaderCell(SharedResources.HEADER_FORCASTGAIN)
        Me(0, eColumnTypes.RecruitmentCV) = New EwEColumnHeaderCell(SharedResources.HEADER_RECRUITMENT_CV)

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()

        Dim group As cStockAssessmentParameters
        Dim Cell As ICell
        Dim irow As Integer
        Dim style As cStyleGuide.eStyleFlags

        ' For each group
        For iGroup As Integer = 1 To Core.nLivingGroups

            'Get the group info!!!!
            group = Me.m_Assessment.Parameter(iGroup)

            irow = Me.AddRow()

            Me(iGroup, eColumnTypes.Index) = New EwERowHeaderCell(CStr(iGroup))
            Cell = New EwECell(group.Name, GetType(String), cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Names)
            Me(irow, eColumnTypes.Name) = Cell

            If group.isFished Then

                Cell = New EwECell(group.RHalfB0Ratio, GetType(Double))
                Cell.Behaviors.Add(Me.EwEEditHandler)
                Me(irow, eColumnTypes.RHalfB) = Cell

                Cell = New EwECell(group.ForcastGain, GetType(Double))
                Cell.Behaviors.Add(Me.EwEEditHandler)
                Me(irow, eColumnTypes.ForcastGain) = Cell

                Cell = New EwECell(group.cvRec, GetType(Double))
                Cell.Behaviors.Add(Me.EwEEditHandler)
                Me(irow, eColumnTypes.RecruitmentCV) = Cell

                Me.Rows(iGroup).Tag = group

            Else
                style = cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.NotEditable
                Cell = New EwECell(cCore.NULL_VALUE, GetType(Double), style)
                Me(irow, eColumnTypes.RHalfB) = Cell

                Cell = New EwECell(cCore.NULL_VALUE, GetType(Double), style)
                Me(irow, eColumnTypes.ForcastGain) = Cell

                Cell = New EwECell(cCore.NULL_VALUE, GetType(Double), style)
                Me(irow, eColumnTypes.RecruitmentCV) = Cell

            End If

        Next iGroup

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.Selection.SelectionMode = GridSelectionMode.Row
    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.MSE
        End Get
    End Property


    Protected Overrides Function OnCellValueChanged(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

        Try

            If Rows(p.Row).Tag Is Nothing Then
                'No Group in this row
                Return True
            End If

            Select Case p.Column

                Case eColumnTypes.RHalfB
                    DirectCast(Rows(p.Row).Tag, cStockAssessmentParameters).RHalfB0Ratio = CSng(cell.GetValue(p))

                Case eColumnTypes.ForcastGain
                    DirectCast(Rows(p.Row).Tag, cStockAssessmentParameters).ForcastGain = CSng(cell.GetValue(p))

                Case eColumnTypes.RecruitmentCV
                    DirectCast(Rows(p.Row).Tag, cStockAssessmentParameters).cvRec = CSng(cell.GetValue(p))

            End Select

            Try

            Catch ex As Exception
                Debug.Assert(False, Me.ToString + " onEdited Event Exception: " + ex.Message)
            End Try

        Catch ex As Exception
            Debug.Assert(False, Me.ToString + ".OnCellValueChanged() Exception: " + ex.Message)
        End Try

        Return True
    End Function


#End Region ' Overrides

End Class


