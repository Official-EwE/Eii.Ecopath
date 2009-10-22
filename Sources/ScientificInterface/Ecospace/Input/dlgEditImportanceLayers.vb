'==============================================================================
'
' $Log: dlgEditImportanceLayers.vb,v $
' Revision 1.4  2008/12/15 15:54:30  jeroens
' no message
'
' Revision 1.3  2008/11/11 07:36:52  jeroens
' Added comments
'
' Revision 1.2  2008/11/06 05:59:56  jeroens
' Bypassed bug 530
'
' Revision 1.1  2008/08/10 17:04:41  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore

#End Region

Namespace Ecospace

    Public Class dlgEditImportanceLayers

#Region " Private variables "

        Private m_core As cCore = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New()

            InitializeComponent()
            Me.m_core = cCore.GetInstance()

        End Sub

#End Region ' Constructors

#Region " Events "

        Private Sub Dialog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles MyBase.Load
            Me.UpdateControls()
        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OK_Button.Click

            ' Try to apply grid changes
            If Me.m_grid.Apply() = False Then
                ' Abort! Abort!
                Return
            End If

            ' Close dialog
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles Cancel_Button.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub m_btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnAddHabitat.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnRemoveHabitat.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnPreserve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnKeep.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_HabitatGrid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) _
            Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnAddHabitat.Enabled = Me.m_grid.CanAddRow()
            Me.m_btnRemoveHabitat.Enabled = Me.m_grid.IsLayerRow() And (Not Me.m_grid.IsFlaggedForDeletionRow())
            Me.m_btnKeep.Enabled = Me.m_grid.IsLayerRow() And Me.m_grid.IsFlaggedForDeletionRow()
        End Sub

#End Region ' Updating

    End Class

End Namespace
