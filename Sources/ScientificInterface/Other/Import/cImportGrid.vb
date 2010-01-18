#Region " Imports "

Option Strict On
Imports EwECore.Database
Imports EwEUtils.Utilities
Imports SourceGrid2
Imports SourceGrid2.Cells

#End Region ' Imports

Namespace Import

    <CLSCompliant(False)> _
    Public Class cImportGrid
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            EwE5Model = 0
            Import
            EwE6Model
        End Enum

        ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
        ''' to trap cell edit events locally in this grid.</summary>
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
        Private m_wizard As cImportWizard = Nothing

        Public Event OnEdited(ByVal grid As cImportGrid)

        Public Sub Init(ByVal wizard As cImportWizard)
            Me.m_wizard = wizard
            Me.RefreshContent()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the grid.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Selection.SelectionMode = GridSelectionMode.Cell
            Me.Selection.EnableMultiSelection = False

            ' Redim columns
            Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.EwE5Model) = New EwEColumnHeaderCell("Old model")
            Me(0, eColumnTypes.Import) = New EwEColumnHeaderCell("Import")
            Me(0, eColumnTypes.EwE6Model) = New EwEColumnHeaderCell("New model name")

            ' Configure columns
            Me.FixedColumns = 1
            Me.Columns(eColumnTypes.EwE6Model).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch

        End Sub

        Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
            Return DockStyle.None
        End Function

        Protected Overrides Sub FillData()

            If (Me.m_wizard Is Nothing) Then Return

            Dim iRow As Integer = 1
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim ewec As EwECell = Nothing

            ' Clear existing rows
            Me.RowsCount = 1

            For Each imp As cImportWizard.cImportSettings In Me.m_wizard.ImportSettings

                iRow = Me.AddRow()

                ewec = New EwECell(imp.ModelInfo.Name, GetType(String))
                ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.EwE5Model) = ewec

                Me(iRow, eColumnTypes.Import) = New Cells.Real.CheckBox(False)
                Me(iRow, eColumnTypes.Import).Behaviors.Add(m_bm)

                ewec = New EwECell("", GetType(String))
                ewec.Style = cStyleGuide.eStyleFlags.NotEditable
                ewec.Behaviors.Add(Me.m_bm)
                Me(iRow, eColumnTypes.EwE6Model) = ewec

                Me.ImportSettings(iRow) = imp

            Next

        End Sub

        Protected Overrides Function OnCellEdited(ByVal p As Position, _
                                                  ByVal cell As ICellVirtual) As Boolean

            Dim settings As cImportWizard.cImportSettings = Me.ImportSettings(p.Row)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.EwE6Model
                    ' Update the name
                    settings.EwE6ModelName = CStr(cell.GetValue(p))
                    ' Refresh the cell since the model name may have been 
                    ' altered in the assignment
                    Me.UpdateEwE6ModelCell(p.Row)
                    RaiseEvent OnEdited(Me)

            End Select

            Return True

        End Function

        Protected Overrides Function OnCellValueChanged(ByVal p As Position, _
                                                        ByVal cell As ICellVirtual) As Boolean

            Dim settings As cImportWizard.cImportSettings = Me.ImportSettings(p.Row)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.Import
                    settings.Import = CBool(cell.GetValue(p))
                    Me.UpdateEwE6ModelCell(p.Row)
                    RaiseEvent OnEdited(Me)

            End Select
            Return True

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set import settings associated with a row in the grid.
        ''' </summary>
        ''' <param name="iRow">The row associated with the import settings.</param>
        ''' -----------------------------------------------------------------------
        Private Property ImportSettings(ByVal iRow As Integer) As cImportWizard.cImportSettings
            Get
                Dim ri As RowInfo = Me.Rows(iRow)
                Return DirectCast(ri.Tag, cImportWizard.cImportSettings)
            End Get
            Set(ByVal value As cImportWizard.cImportSettings)
                Dim ri As RowInfo = Me.Rows(iRow)
                ri.Tag = value
            End Set
        End Property

        Private Sub UpdateEwE6ModelCell(ByVal iRow As Integer)

            Dim settings As cImportWizard.cImportSettings = Me.ImportSettings(iRow)
            Dim cellEwE As EwECell = DirectCast(Me(iRow, eColumnTypes.EwE6Model), EwECell)

            If settings.Import Then
                cellEwE.Value = settings.EwE6ModelName
                cellEwE.Style = cStyleGuide.eStyleFlags.OK
            Else
                cellEwE.Value = ""
                cellEwE.Style = cStyleGuide.eStyleFlags.NotEditable
            End If
            cellEwE.Invalidate()

        End Sub

        Private Sub cImportGrid_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.SizeChanged
        End Sub

    End Class

End Namespace