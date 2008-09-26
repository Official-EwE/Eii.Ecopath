'==============================================================================
'
' $Log: CustomComboBoxFleetGroupTree.vb,v $
' Revision 1.1  2008/09/26 07:31:46  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/04/07 02:31:10  jeroens
' Cleaning up resources
'
' Revision 1.3  2007/12/14 15:45:35  jeroens
' * Uses toolstrip combo
'
' Revision 1.2  2007/10/13 22:37:57  jeroens
' + Added UpdateContent to allow external logic to trigger refreshes
'
' Revision 1.1  2007/09/07 13:33:42  jeroens
' Initial version
'
'==============================================================================

#Region "Imports Directive"

Option Strict On

Imports EwECore

#End Region

''' ---------------------------------------------------------------------------
''' <summary>
''' Fleet + Group tree control for use with a <see cref="CustomComboBox">CustomComboBox</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class CustomComboBoxFleetGroupTree
    Inherits TreeView

    Private m_core As cCore = Nothing
    Private m_tscb As CustomToolstripComboBox = Nothing

    Public Sub New(ByVal core As cCore, ByVal ccb As CustomToolstripComboBox)

        ' Sanity checks
        Debug.Assert(core IsNot Nothing)
        Debug.Assert(ccb IsNot Nothing)

        ' Store refs
        Me.m_core = core
        Me.m_tscb = ccb
        ' Hook up to parent combo
        ccb.DropdownControl = Me

        Me.UpdateContent()

    End Sub

    Private Sub CustomComboBoxDropDownTree_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles Me.AfterSelect
        Me.UpdateParentCombo()
    End Sub

    Public ReadOnly Property SelectedItem() As ICoreInterface
        Get
            If Me.SelectedNode Is Nothing Then Return Nothing
            Return CType(Me.SelectedNode.Tag, ICoreInterface)
        End Get
    End Property

    Private Sub UpdateParentCombo()
        Me.m_tscb.Items.Clear()
        Me.m_tscb.Items.Add(Me.SelectedNode.Text)
        Me.m_tscb.SelectedIndex = 0
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the group and fleet tree
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub UpdateContent()

        ' Populate the tree
        Dim tnParent As TreeNode = Nothing
        Dim tnChild As TreeNode = Nothing

        Me.Nodes.Clear()

        'Load the fleet drop down list
        tnParent = Me.Nodes.Add(My.Resources.HEADER_FLEETS)
        For Each shp As cFishingRateShape In m_core.FishingRateShapeManager
            tnChild = New TreeNode(shp.Name)
            tnChild.Tag = shp
            tnParent.Nodes.Add(tnChild)
        Next

        'Load the group drop down list
        tnParent = Me.Nodes.Add(My.Resources.HEADER_GROUPS)
        For i As Integer = 1 To m_core.nGroups
            Dim group As cEcoPathGroupInput = m_core.EcoPathGroupInputs(i)
            tnChild = New TreeNode(group.Name)
            tnChild.Tag = group
            tnParent.Nodes.Add(tnChild)
        Next

        Me.SelectedNode = Me.Nodes(0)
        Me.UpdateParentCombo()

    End Sub

End Class
