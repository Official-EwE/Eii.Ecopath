Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports ScientificInterfaceShared.Style.cStyleGuide

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class gridEstimateVs
        Inherits EwEGrid

#Region " Internals "

        ''' <summary>Feedback style to use for selected vul cells.</summary>
        Private Const c_styleSelect As cStyleGuide.eStyleFlags = eStyleFlags.Highlight

        ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
        ''' to trap cell edit events locally in this grid. These events are essential
        ''' for keeping the local Fleet administration up to date.</summary>
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)

        Private m_calc As cEstimateVsCalc = Nothing

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

#End Region ' Internals

#Region " Public properties "

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As cUIContext)
                If (value IsNot Nothing) Then
                    Me.m_calc = New cEstimateVsCalc(value.Core)
                End If
                MyBase.UIContext = value
            End Set
        End Property

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
                        Me.Selection.AddRange(New SourceGrid2.Range(iRow, eColumnTypes.Name, iRow, eColumnTypes.Name))
                        Me.ShowCell(New Position(iRow, 0))
                    End If
                End If
            End Set
        End Property

#End Region ' Public properties

#Region " Overrides "

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

            Me.FixedColumnWidths = True ' To accomodate long header labels
            Me.Selection.SelectionMode = GridSelectionMode.Cell

        End Sub

        Protected Overrides Sub FillData()

            If Me.m_calc Is Nothing Then Return

            Dim group As cEcoSimGroupInput = Nothing
            Dim sPotGrowth As Single = 0.0!
            Dim sFMax As Single = 0.0!
            Dim style As cStyleGuide.eStyleFlags = eStyleFlags.OK

            For iGroup As Integer = 1 To Me.Core.nLivingGroups

                group = Me.Core.EcoSimGroupInputs(iGroup)
                sPotGrowth = 0.0! ' Col 3 in the EwE5 code
                sFMax = 0.0! ' Col 7 in the EwE5 code

                If Me.m_calc.Fish1(iGroup) = 0 Then
                    sPotGrowth = cCore.NULL_VALUE
                    sFMax = 1.2!
                Else
                    sPotGrowth = 2.0!
                    If (Me.m_calc.mo(iGroup) + Me.m_calc.StartEatenOf(iGroup) > 0) Then
                        sFMax = 1.1! * Me.m_calc.Fish1(iGroup) / (Me.m_calc.mo(iGroup) + Me.m_calc.StartEatenOf(iGroup)) / Me.Core.StartBiomass(iGroup)
                    End If
                End If

                Me.AddRow()
                Me(iGroup, eColumnTypes.Index) = New EwERowHeaderCell(iGroup)
                Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(group, eVarNameFlags.Name)

                If sPotGrowth >= 0 Then style = eStyleFlags.OK Else style = eStyleFlags.Null Or eStyleFlags.NotEditable
                Me(iGroup, eColumnTypes.PotGrowth) = New EwECell(sPotGrowth, GetType(Single), style)
                Me(iGroup, eColumnTypes.PotGrowth).Behaviors.Add(Me.m_bm)

                Me(iGroup, eColumnTypes.FMax) = New EwECell(sFMax, GetType(Single), eStyleFlags.OK)
                Me(iGroup, eColumnTypes.FMax).Behaviors.Add(Me.m_bm)

                Me(iGroup, eColumnTypes.PG_VwoFT) = New EwECell(0.0!, GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.PG_VwoFT).Behaviors.Add(Me.m_bm)
                Me(iGroup, eColumnTypes.FMax_VwoFT) = New EwECell(0.0!, GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.FMax_VwoFT).Behaviors.Add(Me.m_bm)
                Me(iGroup, eColumnTypes.PG_VwithFT) = New EwECell(0.0!, GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.PG_VwithFT).Behaviors.Add(Me.m_bm)
                Me(iGroup, eColumnTypes.FMax_VwithFT) = New EwECell(0.0!, GetType(Single), eStyleFlags.NotEditable)
                Me(iGroup, eColumnTypes.FMax_VwithFT).Behaviors.Add(Me.m_bm)

                Me.RecalcVulnerabilities(iGroup)

            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 2
        End Sub

        Protected Overrides Sub OnCellClicked(ByVal p As Position, ByVal cell As ICellVirtual)
            MyBase.OnCellClicked(p, cell)

            Select Case DirectCast(p.Column, eColumnTypes)
                Case eColumnTypes.FMax_VwithFT, _
                     eColumnTypes.FMax_VwoFT, _
                     eColumnTypes.PG_VwithFT, _
                     eColumnTypes.PG_VwoFT
                    Me.UpdateVulSelection(p.Row, p.Column)
                Case Else
                    ' NOP
            End Select

        End Sub

        Protected Overrides Function OnCellEdited(ByVal p As Position, ByVal cell As ICellVirtual) As Boolean

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

        Private Sub RecalcVulnerabilities(ByVal iRow As Integer)

            Dim sPotGrowth As Single = CSng(Me(iRow, eColumnTypes.PotGrowth).Value)
            Dim sFMax As Single = CSng(Me(iRow, eColumnTypes.FMax).Value)

            ' ToDo: Make calculator obsolete, use Ecosim instead
            '       Fish1 etc could be exposed by EcosimGroupInputs as read-only computed values?
            If (Me.m_calc.Fish1(iRow) > 0) And (Me.m_calc.SimGE(iRow) > 0) Then
                Me.SetVulCell(iRow, eColumnTypes.PG_VwithFT, Me.Core.CalcEcosimVulBo(sPotGrowth, iRow, True))
                Me.SetVulCell(iRow, eColumnTypes.PG_VwoFT, Me.Core.CalcEcosimVulBo(sPotGrowth, iRow, False))
            Else
                Me.SetVulCell(iRow, eColumnTypes.PG_VwithFT, cCore.NULL_VALUE)
                Me.SetVulCell(iRow, eColumnTypes.PG_VwoFT, cCore.NULL_VALUE)
            End If

            If (Me.m_calc.Fish1(iRow) > 0) Then
                Me.SetVulCell(iRow, eColumnTypes.FMax_VwithFT, Me.Core.CalcEcosimVulFMax(sFMax, iRow, True))
                Me.SetVulCell(iRow, eColumnTypes.FMax_VwoFT, Me.Core.CalcEcosimVulFMax(sFMax, iRow, False))
            Else
                Me.SetVulCell(iRow, eColumnTypes.FMax_VwithFT, cCore.NULL_VALUE)
                Me.SetVulCell(iRow, eColumnTypes.FMax_VwoFT, cCore.NULL_VALUE)
            End If

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

        Private Sub UpdateVulSelection(ByVal iRow As Integer, ByVal iColSelect As Integer)

            Dim cols As eColumnTypes() = {eColumnTypes.PG_VwithFT, _
                                          eColumnTypes.PG_VwoFT, _
                                          eColumnTypes.FMax_VwithFT, _
                                          eColumnTypes.FMax_VwoFT}
            If Array.IndexOf(cols, DirectCast(iColSelect, eColumnTypes)) = -1 Then Return

            ' Validate incoming column
            Dim cell As EwECell = DirectCast(Me(iRow, CInt(iColSelect)), EwECell)

            ' Clear column if cell cannot be selected
            If ((cell.Style And eStyleFlags.Null) = eStyleFlags.Null) Then
                iColSelect = -1
            End If

            ' Toggle cell checked state
            If ((cell.Style And gridEstimateVs.c_styleSelect) = gridEstimateVs.c_styleSelect) Then
                iColSelect = -1
            End If

            ' Update checked cells
            For Each column As eColumnTypes In cols
                cell = DirectCast(Me(iRow, CInt(column)), EwECell)
                If CInt(column) = iColSelect Then
                    cell.Style = cell.Style Or gridEstimateVs.c_styleSelect
                Else
                    cell.Style = cell.Style And (Not gridEstimateVs.c_styleSelect)
                End If
                cell.Invalidate()
            Next

        End Sub

#End Region ' Internals

    End Class

End Namespace ' Ecosim
