Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core

Public Class EditGroupTaxon

    Private m_uic As cUIContext = Nothing

#Region " Constructor "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Create a new instance of this class.
    ''' </summary>
    ''' <param name="uic">The <see cref="cUIContext">UI context</see> to connect to.</param>
    ''' <param name="group">A group to select, if any.</param>
    ''' -------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext, _
                          Optional ByVal group As cEcoPathGroupInput = Nothing)

        Me.InitializeComponent()

        Me.m_uic = uic
        Me.m_grid.UIContext = uic

    End Sub

#End Region ' Constructor

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.PopulateDetailBoxes()

        AddHandler Me.m_grid.OnSelectionChanged, AddressOf OnRowSelectionChanged

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        RemoveHandler Me.m_grid.OnSelectionChanged, AddressOf OnRowSelectionChanged
        MyBase.OnFormClosed(e)
    End Sub

#End Region ' Form overrides

#Region " Events "

    Private Sub OnRowSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection)
        Me.UpdateControls()
    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub m_btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAdd.Click
        Me.m_grid.AddTaxon()
    End Sub

    Private Sub m_btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnRemove.Click

    End Sub

    Private Sub m_btnKeep_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnKeep.Click

    End Sub

    Private Sub m_btnMoveUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnMoveUp.Click
        ' Only when a taxon row is selected
    End Sub

    Private Sub m_btnMoveDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnMoveDown.Click
        ' Only when a taxon row is selected
    End Sub

    Private Sub m_btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnUpdate.Click

    End Sub

    Private Sub m_btnUpdateAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnUpdateAll.Click, m_btnConfigure.Click, m_btnSearch.Click

    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub UpdateControls()

        Dim taxon As ITaxonData = Me.m_grid.SelectedTaxon
        Dim group As cEcoPathGroupInput = Me.m_grid.SelectedGroup

        If (taxon Is Nothing) Then
            Me.m_tbCommon.Enabled = False : Me.m_tbCommon.Text = ""
            Me.m_cmbClass.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_cmbOrder.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_cmbFamily.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_cmbGenus.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_cmbSpecies.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_cmbSource.Enabled = False : Me.m_cmbSource.SelectedIndex = -1
            Me.m_btnAdd.Enabled = (group IsNot Nothing)
            Me.m_btnRemove.Enabled = False
            Me.m_btnKeep.Enabled = False
            Me.m_btnMoveUp.Enabled = False
            Me.m_btnMoveDown.Enabled = False
            Me.m_btnSearch.Enabled = False
        Else
            Me.m_tbCommon.Enabled = True : Me.m_tbCommon.Text = taxon.Common
            Me.m_cmbClass.Enabled = True : Me.m_cmbClass.Text = taxon.Class
            Me.m_cmbOrder.Enabled = True : Me.m_cmbClass.Text = taxon.Order
            Me.m_cmbFamily.Enabled = True : Me.m_cmbClass.Text = taxon.Family
            Me.m_cmbGenus.Enabled = True : Me.m_cmbClass.Text = taxon.Genus
            Me.m_cmbSpecies.Enabled = True : Me.m_cmbClass.Text = taxon.Species
            Me.m_cmbSource.Enabled = True : Me.m_cmbSource.Text = taxon.Source
            Me.m_btnAdd.Enabled = True
            Me.m_btnRemove.Enabled = True
            Me.m_btnMoveUp.Enabled = (group.Index > 1)
            Me.m_btnMoveDown.Enabled = (group.Index < Me.m_uic.Core.nGroups)
            Me.m_btnSearch.Enabled = (taxon IsNot Nothing)
            Me.m_btnAdd.Enabled = (group IsNot Nothing)
        End If

        Me.m_btnSearch.Enabled = (Me.m_cmbSource.Text <> "")
        Me.m_btnConfigure.Enabled = False

    End Sub

    Private Sub PopulateDetailBoxes()

        Dim taxon As cTaxon = Nothing

        ' For now: populate data from actual taxa
        For i As Integer = 1 To Me.m_uic.Core.nTaxon
            taxon = Me.m_uic.Core.Taxon(i)
            Me.AddText(Me.m_cmbClass, taxon.Class)
            Me.AddText(Me.m_cmbOrder, taxon.Order)
            Me.AddText(Me.m_cmbGenus, taxon.Genus)
            Me.AddText(Me.m_cmbFamily, taxon.Family)
            Me.AddText(Me.m_cmbSpecies, taxon.Species)
        Next

    End Sub

    Private Sub AddText(ByVal cmb As ComboBox, ByVal strText As String)

        If cmb.FindStringExact(strText) = -1 Then
            cmb.Items.Add(strText)
        End If

    End Sub

#End Region ' Internals

End Class
