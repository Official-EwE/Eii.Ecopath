#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class EcospaceResultsGridGroup
        : Inherits gridResultsBase

        Private m_SelFleetIndex As Integer
        Private m_GroupDisplayFlags() As Boolean
        Private m_DisplayGrpCnt As Integer

        Public Sub New()
            MyBase.new()

            m_GroupDisplayFlags = AppLauncher.GetInstance.GroupDisplayFlags
            m_DisplayGrpCnt = 0

        End Sub

        Public Property SelFleetIndex() As Integer
            Get
                Return m_SelFleetIndex
            End Get
            Set(ByVal value As Integer)
                m_SelFleetIndex = value
                Me.UpdateData()
            End Set
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Define column headers
            Me.Redim(1, 11)
            Me(0, 0) = New EwEColumnHeaderCell("")
            'Group name
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            'Biomass (Start)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSSTART)
            'Biomass (End)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSEND)
            'Biomass (E/S)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSES)
            'Catch (Start)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCHSTART)
            'Catch (End)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCHEND)
            'Catch (E/S)
            Me(0, 7) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCHES)
            'Value (Start)
            Me(0, 8) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUESTART)
            'Value (End)
            Me(0, 9) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUEEND)
            'Value (E/S)
            Me(0, 10) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUEES)

        End Sub

        Protected Overrides Sub FillData()

            'This method init the cells, its visual and data models. 
            Dim core As cCore = cCore.GetInstance()

            Dim lName As New List(Of String)
            lName.Add(String.Empty)

            For i As Integer = 1 To core.nGroups
                If m_GroupDisplayFlags(i) Then
                    lName.Add(core.EcospaceGroupSummary(i).Name)
                    m_DisplayGrpCnt += 1
                End If

            Next

            Dim aCalc() As Integer = {4, 7, 10}

            Me.InitCells(m_DisplayGrpCnt + 1, lName.ToArray, aCalc)

            Me.UpdateData()

        End Sub

        Private Sub UpdateData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cEcospaceGroupSummary = Nothing

            Dim totalValue(0 To 10) As Single
            Me.InitTotalArray(totalValue)

            For groupIndex As Integer = 1 To core.nGroups

                'Only display selected groups
                If m_GroupDisplayFlags(groupIndex) Then

                    source = core.EcospaceGroupSummary(groupIndex)

                    SetCellValue(groupIndex, 2, source.BiomassStart, totalValue)
                    SetCellValue(groupIndex, 3, source.BiomassEnd, totalValue)

                    'The logic was pulled out from EwE5
                    If source.BiomassStart > 0 And source.BiomassEnd > 0 Then
                        SetCellValue(groupIndex, 4, CSng(source.BiomassEnd / source.BiomassStart), totalValue)
                    End If

                    Dim fCS As Single = source.CatchStart(Me.SelFleetIndex)
                    SetCellValue(groupIndex, 5, fCS, totalValue)

                    Dim fCE As Single = source.CatchEnd(Me.SelFleetIndex)
                    SetCellValue(groupIndex, 6, fCE, totalValue)

                    If fCS > 0 And fCE > 0 Then
                        SetCellValue(groupIndex, 7, CSng(fCE / fCS), totalValue)
                    End If

                    Dim fVS As Single = source.ValueStart(Me.SelFleetIndex)
                    SetCellValue(groupIndex, 8, fVS, totalValue)

                    Dim fVE As Single = source.ValueEnd(Me.SelFleetIndex)
                    SetCellValue(groupIndex, 9, fVE, totalValue)

                    If fVS > 0 And fVE > 0 Then
                        SetCellValue(groupIndex, 10, CSng(fVE / fVS), totalValue)
                    End If

                End If

            Next

            'Display total values
            For columnIndex As Integer = 2 To Me.ColumnsCount - 1
                If columnIndex = 4 Or columnIndex = 7 Or columnIndex = 10 Then
                    If totalValue(columnIndex - 2) > 0 And totalValue(columnIndex - 1) > 0 Then
                        Me(Me.RowsCount - 1, columnIndex).Value = totalValue(columnIndex - 1) / totalValue(columnIndex - 2)
                    End If
                Else
                    If totalValue(columnIndex) > 0 Then
                        Me(Me.RowsCount - 1, columnIndex).Value = totalValue(columnIndex)
                    End If
                End If
            Next

        End Sub


    End Class

End Namespace
