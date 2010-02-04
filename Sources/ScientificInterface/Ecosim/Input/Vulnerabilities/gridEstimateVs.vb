Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports ScientificInterfaceShared.Style.cStyleGuide

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class gridEstimateVs
        Inherits EwEGrid

        ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
        ''' to trap cell edit events locally in this grid. These events are essential
        ''' for keeping the local Fleet administration up to date.</summary>
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)

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

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.PotGrowth) = New EwEColumnHeaderCell("Poten. growth (Bunf/Bo)")
            Me(0, eColumnTypes.FMax) = New EwEColumnHeaderCell("FMax")
            Me(0, eColumnTypes.PG_VwoFT) = New EwEColumnHeaderCell("Vulnerability w/o FT")
            Me(0, eColumnTypes.FMax_VwoFT) = New EwEColumnHeaderCell("Vulnerability w/o FT")
            Me(0, eColumnTypes.PG_VwithFT) = New EwEColumnHeaderCell("Vulnerability w. FT")
            Me(0, eColumnTypes.FMax_VwithFT) = New EwEColumnHeaderCell("Vulnerability w. FT")

            Me.FixedColumns = 1
            Me.FixedColumnWidths = True ' To accomodate long header labels

            Me.Selection.SelectionMode = GridSelectionMode.Row

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cEcoSimGroupInput = Nothing
            Dim cell As EwECell = Nothing

            For iGroup As Integer = 1 To Me.Core.nLivingGroups

                group = Me.Core.EcoSimGroupInputs(iGroup)

                Me.AddRow()
                Me(iGroup, eColumnTypes.Index) = New EwERowHeaderCell(iGroup)
                Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(group, eVarNameFlags.Name)
                Me(iGroup, eColumnTypes.PotGrowth) = New EwECell(3, GetType(Single), eStyleFlags.OK)
                Me(iGroup, eColumnTypes.FMax) = New EwECell(3, GetType(Single), eStyleFlags.OK)
                Me(iGroup, eColumnTypes.PG_VwoFT) = New EwECell(1, GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.PG_VwoFT).Behaviors.Add(Me.m_bm)
                Me(iGroup, eColumnTypes.FMax_VwoFT) = New EwECell(1, GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.FMax_VwoFT).Behaviors.Add(Me.m_bm)
                Me(iGroup, eColumnTypes.PG_VwithFT) = New EwECell(2, GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.PG_VwithFT).Behaviors.Add(Me.m_bm)
                Me(iGroup, eColumnTypes.FMax_VwithFT) = New EwECell(2, GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.FMax_VwithFT).Behaviors.Add(Me.m_bm)

                Me.RecalcVulnerabilities(iGroup)

            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
        End Sub

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
            Set(ByVal iRow As Integer)
                ' Clear current selection
                If Me.Selection IsNot Nothing Then
                    Dim r As SourceGrid2.Range = Me.Selection.GetRange()
                    If Not r.IsEmpty Then
                        Me.Selection.RemoveRange(r)
                    End If
                    If (iRow >= 0) Then
                        Me.Selection.AddRange(New SourceGrid2.Range(iRow, 0, iRow, Me.ColumnsCount))
                        Me.ShowCell(New Position(iRow, 0))
                    End If
                End If
            End Set
        End Property

        Protected Overrides Sub OnCellClicked(ByVal p As Position, ByVal cell As ICellVirtual)
            MyBase.OnCellClicked(p, cell)
            Me.UpdateVulSelection(p.Row, p.Column)
        End Sub

        Protected Overrides Function OnCellValueChanged(ByVal p As Position, ByVal cell As ICellVirtual) As Boolean
            If (p.Column = eColumnTypes.PotGrowth) Or (p.Column = eColumnTypes.FMax) Then
                Me.RecalcVulnerabilities(p.Row)
            End If
            Return MyBase.OnCellValueChanged(p, cell)
        End Function

#Region " Internals "

        Private Sub RecalcVulnerabilities(ByVal iRow As Integer)

            Dim sPotGrowth As Single = CSng(Me(iRow, eColumnTypes.PotGrowth).Value)
            Dim sFMax As Single = CSng(Me(iRow, eColumnTypes.FMax).Value)

            Me.SetVulCell(iRow, eColumnTypes.PG_VwithFT, Me.Core.CalcEcosimVulBo(sPotGrowth, iRow, True))
            Me.SetVulCell(iRow, eColumnTypes.PG_VwoFT, Me.Core.CalcEcosimVulBo(sPotGrowth, iRow, False))
            Me.SetVulCell(iRow, eColumnTypes.FMax_VwithFT, Me.Core.CalcEcosimVulFMax(sPotGrowth, iRow, True))
            Me.SetVulCell(iRow, eColumnTypes.FMax_VwoFT, Me.Core.CalcEcosimVulFMax(sPotGrowth, iRow, False))

        End Sub

        Private Sub SetVulCell(ByVal iRow As Integer, ByVal iCol As eColumnTypes, ByVal sValue As Single)

            Dim cell As EwECell = DirectCast(Me(iRow, iCol), EwECell)
            cell.Value = sValue
            If sValue > 0 Then
                cell.Style = cell.Style And (Not eStyleFlags.Null)
            Else
                cell.Style = cell.Style Or eStyleFlags.Null
            End If

        End Sub

        Private Sub UpdateVulSelection(ByVal iRow As Integer, Optional ByVal iColSelect As Integer = -1)

            Dim cols As eColumnTypes() = {eColumnTypes.PG_VwithFT, _
                                          eColumnTypes.PG_VwoFT, _
                                          eColumnTypes.FMax_VwithFT, _
                                          eColumnTypes.FMax_VwoFT}
            Dim cell As EwECell = Nothing

            If Array.IndexOf(cols, DirectCast(iColSelect, eColumnTypes)) = -1 Then Return

            ' Resolve col to select
            If iColSelect = -1 Then
                For Each column As eColumnTypes In cols
                    cell = DirectCast(Me(iRow, CInt(column)), EwECell)
                    If ((cell.Style And eStyleFlags.Checked) = eStyleFlags.Checked) Then
                        iColSelect = CInt(column)
                    End If
                Next column
            End If

            ' Can cell be selected?
            If (iColSelect > -1) Then
                cell = DirectCast(Me(iRow, CInt(iColSelect)), EwECell)
                If ((cell.Style And eStyleFlags.Checked) = eStyleFlags.Checked) And _
                   ((cell.Style And eStyleFlags.NotEditable) = eStyleFlags.NotEditable) Then
                    iColSelect = -1
                End If
            End If

            ' Update checked cells
            For Each column As eColumnTypes In cols
                cell = DirectCast(Me(iRow, CInt(column)), EwECell)
                If CInt(column) = iColSelect Then
                    cell.Style = cell.Style Or eStyleFlags.Checked
                Else
                    cell.Style = cell.Style And (Not eStyleFlags.Checked)
                End If
                cell.Invalidate()
            Next

        End Sub

#End Region ' Internals

    End Class

End Namespace ' Ecosim
