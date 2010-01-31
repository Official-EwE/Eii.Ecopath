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
            Index = 0
            Name
            FMax
            ApplyVwoFT
            VwoFT
            ApplyVwithFT
            VwithFT
        End Enum

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.FMax) = New EwEColumnHeaderCell("FMax")
            Me(0, eColumnTypes.ApplyVwoFT) = New EwEColumnHeaderCell("Apply")
            Me(0, eColumnTypes.VwoFT) = New EwEColumnHeaderCell("V without FT")
            Me(0, eColumnTypes.ApplyVwithFT) = New EwEColumnHeaderCell("Apply")
            Me(0, eColumnTypes.VwithFT) = New EwEColumnHeaderCell("V with FT")

            Me.FixedColumns = 1
            Me.FixedColumnWidths = False

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

                Me(iGroup, eColumnTypes.ApplyVwoFT) = New Cells.Real.CheckBox(False)
                Me(iGroup, eColumnTypes.ApplyVwoFT).Behaviors.Add(Me.m_bm)

                Me(iGroup, eColumnTypes.VwoFT) = New EwECell(1, GetType(Single), eStyleFlags.NotEditable)

                Me(iGroup, eColumnTypes.ApplyVwithFT) = New Cells.Real.CheckBox(False)
                Me(iGroup, eColumnTypes.ApplyVwithFT).Behaviors.Add(Me.m_bm)

                Me(iGroup, eColumnTypes.VwithFT) = New EwECell(2, GetType(Single), eStyleFlags.NotEditable)

                Me(iGroup, eColumnTypes.FMax) = New EwECell(3, GetType(Single), eStyleFlags.NotEditable)

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

    End Class

End Namespace ' Ecosim
