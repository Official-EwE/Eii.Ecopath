#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore

#End Region

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Dialog, implementing the Ecospace Edit Regions user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgEditRegions

#Region " Private variables "

        Private Enum AllocationModeType As Integer
            None = 0
            Habitat = 1
            Cell = 2
        End Enum

        Private m_uic As cUIContext = Nothing
        Private m_eAllocateRegions As AllocationModeType = AllocationModeType.None

#End Region

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)

            Me.m_uic = uic
            Me.InitializeComponent()

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.m_grid.UIContext = Me.m_uic
            Me.UpdateControls()
        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

            ' Try to apply grid changes
            If Me.m_grid.Apply() = False Then
                ' Abort! Abort!
                Return
            End If

            ' Close dialog
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnAddRegion(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnAddRegion.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub OnRemoveRegion(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnRemoveRegion.Click
            Me.m_grid.SetSelectedRowsDeleteState(True)
            Me.UpdateControls()
        End Sub

        Private Sub OnPreserveRegion(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnKeep.Click
            Me.m_grid.SetSelectedRowsDeleteState(False)
            Me.UpdateControls()
        End Sub

        Private Sub m_RegionGrid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) _
            Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

        Private Sub OnCreateRegionsFromHabitats(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnHabToRegion.Click
            Me.Cursor = Cursors.WaitCursor
            Me.m_grid.CreateHabitatRegions()
            Me.Cursor = Cursors.Default
            Me.m_eAllocateRegions = AllocationModeType.Habitat
        End Sub

        Private Sub OnCreateRegionsFromMPAs(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnMPAtoRegion.Click
            Me.Cursor = Cursors.WaitCursor
            Me.m_grid.CreateMPARegions()
            Me.Cursor = Cursors.Default
            Me.m_eAllocateRegions = AllocationModeType.Habitat
        End Sub

        Private Sub OnCreateRegionsFromCells(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnFromCells.Click
            Me.Cursor = Cursors.WaitCursor
            Me.m_grid.CreateCellRegions()
            Me.Cursor = Cursors.Default
            m_eAllocateRegions = AllocationModeType.Cell
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnAddRegion.Enabled = Me.m_grid.CanAddRow()
            Me.m_btnRemoveRegion.Enabled = (Not Me.m_grid.HasDeletedRegionsSelected())
            Me.m_btnKeep.Enabled = Me.m_grid.HasDeletedRegionsSelected()
        End Sub

#End Region ' Updating

    End Class

End Namespace

#Region " Ye olde code "
#If 0 Then

Namespace Ecospace

    Public Class dlgDefineRegions

        ' The core reference
        Private m_Core As cCore
        Private WithEvents m_Grid As DefineRegionsEwEGrid

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            m_Core = cCore.GetInstance()
            ' Initialize grid
            m_Grid = New DefineRegionsEwEGrid
            plRegionsGrid.Controls.Clear()
            plRegionsGrid.Controls.Add(m_Grid)

        End Sub

        Private Sub Close_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Close_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub dlgDefineRegions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            txbNumRegions.Text = CStr(m_Core.nRegions)

        End Sub

        ''' <summary>
        ''' Validating the user input
        ''' </summary>
        Private Sub txbNumRegions_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txbNumRegions.Validating

            Try
                'If the user enters the non-positive number, we remind them with an red icon.
                If CInt(txbNumRegions.Text) <= 0 Then
                    Me.epNumRegions.SetError(Me.txbNumRegions, My.Resources.INVALID_NUMBER_ENTERED)
                    e.Cancel = True
                Else
                    Me.epNumRegions.SetError(Me.txbNumRegions, "")
                End If

            Catch ex As Exception
                Me.epNumRegions.SetError(Me.txbNumRegions, My.Resources.INVALID_NUMBER_ENTERED)
                e.Cancel = True
            End Try

        End Sub

        Private Sub txbNumRegions_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txbNumRegions.Validated

            Dim cntR As Integer = m_Core.nRegions
            Dim defR As Integer = CInt(txbNumRegions.Text)

            ' Insert more regions
            If defR > cntR Then
                For i As Integer = cntR + 1 To defR
                    m_Grid.InsertNewRegion(i)
                Next
            ElseIf defR < cntR Then 'Delete some regions

                Dim msg As String = String.Format(My.Resources.REGION_BATCH_DELETE_CONFIRMATION, (defR + 1).ToString, (cntR).ToString)
                Dim caption As String = My.Resources.REGION_DELETE_CAPTION
                Dim btns As MessageBoxButtons = MessageBoxButtons.YesNo

                Dim result As DialogResult

                result = MessageBox.Show(msg, caption, btns)

                'Confirm with the user about deletion action
                If result = Windows.Forms.DialogResult.Yes Then
                    For j As Integer = cntR To defR + 1 Step -1
                        m_Grid.DeleteRegion(j)
                    Next
                End If
            End If

        End Sub

        ''' <summary>
        ''' Set regions = regions event handler. Based on EwE5 logic, it removes all existing regions first 
        ''' then set the number of regions equal to regions.
        ''' </summary>
        Private Sub btnSetRH_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetRH.Click

            'Delete existing regions from database and ui
            m_Grid.DeleteAllRegions()

            Dim source As cEcospaceRegion = Nothing
            'Insert new regions
            ' VERIFY_JS: CHeck if ALL region needs to be converted too
            For i As Integer = 0 To m_Core.nRegions - 1
                source = m_Core.EcospaceRegions(i)

                Dim rName As String = String.Format(My.Resources.DEFAULT_NEWREGION_NUM, source.Name)
                m_Grid.InsertNewRegion(i, rName)
            Next

            'Update the number of regions
            txbNumRegions.Text = CStr(m_Core.nRegions)

        End Sub

        ''' <summary>
        ''' Set regions = number of cells event handler. Based on EwE5 logic, it removes all existing regions first 
        ''' then set the number of regions equal to number of cells.
        ''' </summary>
        Private Sub btnSetRC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetRC.Click

            'Delete existing regions from database and ui
            m_Grid.DeleteAllRegions()

            'Get the basemap dimensions
            Dim col As Integer = m_Core.EcospaceBasemap.InCol
            Dim row As Integer = m_Core.EcospaceBasemap.InRow

            For i As Integer = 1 To row
                For j As Integer = 1 To col
                    Dim rName As String = String.Format("Reg.row:{0} ,col:{1}", (i).ToString, (j).ToString)
                    m_Grid.InsertNewRegion((i - 1) * col + j, rName)
                Next
            Next

            txbNumRegions.Text = CStr(m_Core.nRegions)

        End Sub

        Private Sub GridRowChanged_EventHandler(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_Grid.GridRowChanged
            'Update num of regions

            txbNumRegions.Text = CStr(m_Core.nRegions)

        End Sub



    End Class
End Namespace

#End If
#End Region ' Ye olde code
