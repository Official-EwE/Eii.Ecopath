'==============================================================================
'
' $Log: dlgEditHabitats.vb,v $
' Revision 1.2  2008/12/15 15:54:30  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:57  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/08/09 02:54:22  jeroens
' Cleaned up
'
' Revision 1.2  2008/06/02 00:01:23  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.1  2007/09/18 14:38:14  jeroens
' * Renamed
'
' Revision 1.17  2007/09/14 16:55:36  jeroens
' * Revamped, in progress
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore

#End Region

Namespace Ecospace

    Public Class dlgEditHabitats

#Region "Private variables"

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

        Private Sub Dialog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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

        Private Sub m_btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnAddHabitat.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnRemoveHabitat.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnPreserve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnKeep.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_HabitatGrid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnAddHabitat.Enabled = Me.m_grid.CanAddRow()
            Me.m_btnRemoveHabitat.Enabled = Me.m_grid.IsHabitatRow() And (Not Me.m_grid.IsFlaggedForDeletionRow())
            Me.m_btnKeep.Enabled = Me.m_grid.IsHabitatRow() And Me.m_grid.IsFlaggedForDeletionRow()
        End Sub

#End Region ' Updating

    End Class

End Namespace
