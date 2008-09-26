'==============================================================================
'
' $Log: EditFleets.vb,v $
' Revision 1.1  2008/09/26 07:31:29  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2007/07/08 02:24:39  jeroens
' - Solved conflict between exiting event and new EwEGrid selection event
'
' Revision 1.5  2007/06/15 14:57:43  jeroens
' - Removed 'Append' logic
'
' Revision 1.4  2007/05/24 15:50:21  jeroens
' + Added Append button
'
' Revision 1.3  2007/05/02 13:45:28  jeroens
' * Redesigned to match Group/Stanza edit dialog
'
' Revision 1.2  2007/03/08 11:53:00  jeroens
' ^ Cosmectics
'
' Revision 1.1  2007/03/07 18:14:34  jeroens
' Initial version (not tested yet; it just compiles)
'
'==============================================================================

#Region "Imports directive"

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Windows.Forms
Imports EwEUtils.Commands

#End Region

Namespace Ecopath

    Public Class EditFleets

#Region "Private variables"
        Private WithEvents m_FleetGrid As New EditFleetsEwEGrid
        Private m_Core As cCore
#End Region

#Region "Constructors"
        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            m_Core = cCore.GetInstance()

        End Sub
#End Region

#Region "Event handlers "

        Private Sub EditGroups_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.m_plFleetGrid.Controls.Add(m_FleetGrid)
            Me.UpdateControls()
        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

            ' Try to apply grid changes
            If Me.m_FleetGrid.Apply() = False Then
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

        Private Sub m_btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnInsert.Click
            Me.m_FleetGrid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnMoveUp.Click
            Me.m_FleetGrid.MoveRowUp()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnMoveDown.Click
            Me.m_FleetGrid.MoveRowDown()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnDelete.Click
            Me.m_FleetGrid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnPreserve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnPreserve.Click
            Me.m_FleetGrid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_FleetGrid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) Handles m_FleetGrid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnMoveUp.Enabled = Me.m_FleetGrid.CanMoveRowUp()
            Me.m_btnMoveDown.Enabled = Me.m_FleetGrid.CanMoveRowDown()
            Me.m_btnInsert.Enabled = Me.m_FleetGrid.CanInsertRow()
            Me.m_btnDelete.Enabled = Me.m_FleetGrid.IsFleetRow() And (Not Me.m_FleetGrid.IsFlaggedForDeletionRow())
            Me.m_btnPreserve.Enabled = Me.m_FleetGrid.IsFleetRow() And Me.m_FleetGrid.IsFlaggedForDeletionRow()
        End Sub

#End Region ' Updating

    End Class

End Namespace