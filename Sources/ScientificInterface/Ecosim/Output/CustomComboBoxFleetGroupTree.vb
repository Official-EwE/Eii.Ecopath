#Region " Imports "

Option Strict On

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

''' ---------------------------------------------------------------------------
''' <summary>
''' Fleet + Group tree control for use with a <see cref="CustomComboBox">CustomComboBox</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cCustomComboBoxFleetGroupTree
    Inherits TreeView

    Private m_core As cCore = Nothing
    Private m_tscb As cCustomToolstripComboBox = Nothing

    Public Sub New(ByVal core As cCore, ByVal ccb As cCustomToolstripComboBox)

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

    Private Sub CustomComboBoxDropDownTree_AfterSelect(ByVal sender As Object, ByVal e As TreeViewEventArgs) _
        Handles Me.AfterSelect

        ' Is a node without children selected?
        If (Me.SelectedNode.Nodes.Count = 0) Then
            ' #Yes: update parent combo to apply selection and close
            Me.UpdateParentCombo()
        End If

    End Sub

    Public ReadOnly Property SelectedItem() As ICoreInterface
        Get
            If Me.SelectedNode Is Nothing Then Return Nothing
            Return DirectCast(Me.SelectedNode.Tag, ICoreInterface)
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
        tnParent = Me.Nodes.Add(SharedResources.HEADER_FISHINGEFFORT)
        For Each shp As cFishingRateShape In m_core.FishingEffortShapeManager
            tnChild = New TreeNode(shp.Name)
            tnChild.Tag = shp
            tnParent.Nodes.Add(tnChild)
        Next

        'Load the group drop down list
        tnParent = Me.Nodes.Add(SharedResources.HEADER_FISHINGMORTALITY)
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
