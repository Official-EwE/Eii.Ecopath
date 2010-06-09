Imports System.Windows.Forms
Imports EwECore

Public Class EditGroupTaxon

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

        Me.m_grid.UIContext = uic
        'Me.m_grid.SelectGroup(group)

    End Sub

#End Region ' Constructor

#Region " Events "

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
        Handles m_btnUpdateAll.Click, m_btnConfigure.Click, Button1.Click

    End Sub

#End Region ' Events

End Class
