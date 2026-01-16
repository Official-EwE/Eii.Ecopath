' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports SharedResources = ScientificInterfaceShared.My.Resources



Namespace Ecosim

    
    Public Class gridEstimateVs
        Inherits cEwEGrid

#Region " Private vars and declarations "

        Private Enum eColumnTypes As Integer
            ''' <summary>Index column.</summary>
            Index = 0
            ''' <summary>Name column.</summary>
            Name
            ''' <summary>Potential growth column.</summary>
            PotGrowth
            ''' <summary>Vuls w/o FT for potential growth column.</summary>
            PG_VwoFT
            ''' <summary>Vuls with FT for potential growth column.</summary>
            PG_VwithFT
            ''' <summary>FMax column.</summary>
            FMax
            ''' <summary>Vuls w/o FT for FMax column.</summary>
            FMax_VwoFT
            ''' <summary>Vuls with FT for FMax column.</summary>
            FMax_VwithFT
        End Enum

        ''' <summary>Column indices displaying computed vul values.</summary>
        Private Shared c_vulcols As eColumnTypes() = {eColumnTypes.PG_VwithFT, _
                                                      eColumnTypes.PG_VwoFT, _
                                                      eColumnTypes.FMax_VwithFT, _
                                                      eColumnTypes.FMax_VwoFT}

        ''' <summary>Feedback style to use for selected vul cells.</summary>
        Private Const c_styleSelect As cStyleGuide.eStyleFlags = eStyleFlags.Highlight

#End Region ' Private vars and declarations

#Region " Public properties "

        Public Event OnSelectedVulnerabilitiesChanged(sender As gridEstimateVs)

        Public Property SelectedGroupIndex() As Integer
            Get
                Dim iSelectedRow As Integer = -1
                Dim selection As SourceGrid2.Selection = Me.Selection
                Dim arSelection As SourceGrid2.Range = Nothing

                If selection Is Nothing Then Return iSelectedRow
                If selection.Count = 0 Then Return iSelectedRow

                arSelection = selection.Item(0)
                iSelectedRow = arSelection.Start.Row
                Return iSelectedRow
            End Get
            Set(iRow As Integer)
                ' Clear current selection
                If Me.Selection IsNot Nothing Then
                    Dim r As SourceGrid2.Range = Me.Selection.GetRange()
                    If Not r.IsEmpty Then
                        Me.Selection.RemoveRange(r)
                    End If
                    If (iRow >= 0) Then
                        Me.Selection.AddRange(New SourceGrid2.Range(iRow, eColumnTypes.Name, iRow, eColumnTypes.Name))
                        Me.ShowCell(New Position(iRow, 0))
                    End If
                End If
            End Set
        End Property

        Public Function HasSelectedVulnerabilities() As Boolean

            Dim cell As cEwECell = Nothing

            For Each col As eColumnTypes In gridEstimateVs.c_vulcols
                For iRow As Integer = 1 To Me.RowsCount - 1
                    If Me.IsVulCellSelected(iRow, col) Then
                        Return True
                    End If
                Next
            Next
            Return False

        End Function

        Public Sub ApplySelectedVulnerabilities()

            Dim sVul As Single = cCore.NULL_VALUE
            Dim group As cEcoPathGroupInput = Nothing
            Dim groupSim As cEcosimGroupInput = Nothing

            Me.Core.SetBatchLock(cCore.eBatchLockType.Update)

            Try

                For iGroup As Integer = 1 To Me.Core.nGroups

                    ' Get group
                    group = Me.Core.EcopathGroupInputs(iGroup)

                    ' Get selected vul, if any
                    sVul = cCore.NULL_VALUE
                    For Each col As eColumnTypes In gridEstimateVs.c_vulcols
                        If Me.IsVulCellSelected(iGroup, col) Then
                            sVul = CSng(Me(iGroup, col).Value)
                        End If
                    Next

                    ' Has vul?
                    If sVul > 1 Then
                        For i As Integer = 1 To Me.Core.nGroups
                            'Update vulmult(prey,pred)
                            If group.DietComp(i) > 0 Then
                                groupSim = Me.Core.EcosimGroupInputs(i)
                                groupSim.VulMult(iGroup) = sVul
                            End If
                        Next
                    End If

                Next
            Catch ex As Exception

            End Try

            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim)

        End Sub

#End Region ' Public properties

#Region " Overrides "

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.PotGrowth) = New cEwEColumnHeaderCell(SharedResources.HEADER_POTENTIAL_GROWTH)
            Me(0, eColumnTypes.FMax) = New cEwEColumnHeaderCell(SharedResources.HEADER_FMAX)
            Me(0, eColumnTypes.PG_VwoFT) = New cEwEColumnHeaderCell(SharedResources.HEADER_VULNERABILITY_WO_FT)
            Me(0, eColumnTypes.FMax_VwoFT) = New cEwEColumnHeaderCell(SharedResources.HEADER_VULNERABILITY_WO_FT)
            Me(0, eColumnTypes.PG_VwithFT) = New cEwEColumnHeaderCell(SharedResources.HEADER_VULNERABILITY_WITH_FT)
            Me(0, eColumnTypes.FMax_VwithFT) = New cEwEColumnHeaderCell(SharedResources.HEADER_VULNERABILITY_WITH_FT)

            Me.FixedColumnWidths = True ' To accomodate long header labels
            Me.Selection.SelectionMode = GridSelectionMode.Cell

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cEcosimGroupInput = Nothing
            Dim sPotGrowth As Single = 0.0!
            Dim sFMax As Single = 0.0!
            Dim style As cStyleGuide.eStyleFlags = eStyleFlags.OK
            Dim estimates(4) As Single

            For iGroup As Integer = 1 To Me.Core.nLivingGroups

                group = Me.Core.EcosimGroupInputs(iGroup)
                sPotGrowth = cCore.NULL_VALUE ' Col 3 in the EwE5 code
                sFMax = cCore.NULL_VALUE ' Col 7 in the EwE5 code

                Me.Core.EstimateVulnerabilities(iGroup, sPotGrowth, sFMax, estimates)

                Me.AddRow()
                Me(iGroup, eColumnTypes.Index) = New cEwERowHeaderCell(CStr(iGroup))
                Me(iGroup, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                If sPotGrowth >= 0 Then style = eStyleFlags.OK Else style = eStyleFlags.Null Or eStyleFlags.NotEditable
                Me(iGroup, eColumnTypes.PotGrowth) = New cEwECell(sPotGrowth, GetType(Single), style)
                Me(iGroup, eColumnTypes.PotGrowth).Behaviors.Add(Me.EwEEditHandler)

                Me(iGroup, eColumnTypes.FMax) = New cEwECell(sFMax, GetType(Single), eStyleFlags.OK)
                Me(iGroup, eColumnTypes.FMax).Behaviors.Add(Me.EwEEditHandler)

                Me(iGroup, eColumnTypes.PG_VwoFT) = New cEwECell(estimates(0), GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.PG_VwoFT).Behaviors.Add(Me.EwEEditHandler)
                Me(iGroup, eColumnTypes.FMax_VwoFT) = New cEwECell(estimates(1), GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.FMax_VwoFT).Behaviors.Add(Me.EwEEditHandler)
                Me(iGroup, eColumnTypes.PG_VwithFT) = New cEwECell(estimates(2), GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.PG_VwithFT).Behaviors.Add(Me.EwEEditHandler)
                Me(iGroup, eColumnTypes.FMax_VwithFT) = New cEwECell(estimates(3), GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.FMax_VwithFT).Behaviors.Add(Me.EwEEditHandler)

                Me.RecalcVulnerabilities(iGroup)

            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 2
        End Sub

        Protected Overrides Sub OnCellClicked(p As Position, cell As ICellVirtual)
            MyBase.OnCellClicked(p, cell)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.FMax_VwithFT, _
                     eColumnTypes.FMax_VwoFT, _
                     eColumnTypes.PG_VwithFT, _
                     eColumnTypes.PG_VwoFT

                    If Me.UpdateVulSelection(p.Row, DirectCast(p.Column, eColumnTypes)) Then
                        RaiseEvent OnSelectedVulnerabilitiesChanged(Me)
                    End If

                Case Else
                    ' NOP

            End Select

        End Sub

        Protected Overrides Function OnCellEdited(p As Position, cell As ICellVirtual) As Boolean

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.FMax, _
                     eColumnTypes.PotGrowth
                    Me.RecalcVulnerabilities(p.Row)
                    Return True

                Case Else
                    ' NOP
            End Select

            Return MyBase.OnCellEdited(p, cell)

        End Function

#End Region ' Overrides

#Region " Internals "

        Private Sub RecalcVulnerabilities(iRow As Integer)

            Dim sPotGrowth As Single = CSng(Me(iRow, eColumnTypes.PotGrowth).Value)
            Dim sFMax As Single = CSng(Me(iRow, eColumnTypes.FMax).Value)
            Dim estimates(4) As Single

            Me.Core.EstimateVulnerabilities(iRow, sPotGrowth, sFMax, estimates)

            Me.SetVulCell(iRow, eColumnTypes.PG_VwithFT, estimates(0))
            Me.SetVulCell(iRow, eColumnTypes.PG_VwoFT, estimates(1))
            Me.SetVulCell(iRow, eColumnTypes.FMax_VwithFT, estimates(2))
            Me.SetVulCell(iRow, eColumnTypes.FMax_VwoFT, estimates(3))

        End Sub

        Private Sub SetVulCell(iRow As Integer, iCol As eColumnTypes, sValue As Single)

            Dim cell As cEwECell = DirectCast(Me(iRow, iCol), cEwECell)
            Dim style As cStyleGuide.eStyleFlags = cell.Style

            ' Adjust style
            style = style Or eStyleFlags.ValueComputed
            If sValue > 0 Then
                style = style And (Not eStyleFlags.Null)
            Else
                style = style Or eStyleFlags.Null
            End If

            ' Config cell
            cell.Style = style
            cell.Value = sValue

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <param name="col"></param>
        ''' <returns>True if a vulnerability selection was changed.</returns>
        ''' -------------------------------------------------------------------
        Private Function UpdateVulSelection(iRow As Integer, col As eColumnTypes) As Boolean

            If Array.IndexOf(gridEstimateVs.c_vulcols, col) = -1 Then Return False

            ' Validate incoming column
            Dim cell As cEwECell = DirectCast(Me(iRow, col), cEwECell)
            Dim bCellChanged As Boolean = False

            ' Clear column if cell cannot be selected
            If ((cell.Style And eStyleFlags.Null) = eStyleFlags.Null) Then
                col = eColumnTypes.Index
            End If

            ' Toggle cell checked state
            If (Me.IsVulCellSelected(iRow, col)) Then
                col = eColumnTypes.Index
            End If

            ' Update checked cells
            For Each colVuls As eColumnTypes In gridEstimateVs.c_vulcols
                Dim bIsCellSelected As Boolean = Me.IsVulCellSelected(iRow, colVuls)
                Dim bNeedCellSelected As Boolean = (colVuls = col)

                If (bIsCellSelected <> bNeedCellSelected) Then
                    Me.IsVulCellSelected(iRow, colVuls) = bNeedCellSelected
                    bCellChanged = True
                End If
            Next
            Return bCellChanged

        End Function

        Private Property IsVulCellSelected(iRow As Integer, col As eColumnTypes) As Boolean
            Get

                If (iRow < 1) Or (iRow >= Me.RowsCount) Then Return False
                If (Array.IndexOf(c_vulcols, col) = -1) Then Return False

                Dim cell As cEwECell = DirectCast(Me(iRow, col), cEwECell)
                Return ((cell.Style And gridEstimateVs.c_styleSelect) = gridEstimateVs.c_styleSelect)

            End Get
            Set(value As Boolean)

                If (iRow < 1) Or (iRow >= Me.RowsCount) Then Return
                If (Array.IndexOf(c_vulcols, col) = -1) Then Return

                Dim cell As cEwECell = DirectCast(Me(iRow, col), cEwECell)
                If value Then
                    cell.Style = cell.Style Or gridEstimateVs.c_styleSelect
                Else
                    cell.Style = cell.Style And (Not gridEstimateVs.c_styleSelect)
                End If
                cell.Invalidate()

            End Set
        End Property

#End Region ' Internals

    End Class

End Namespace ' Ecosim
