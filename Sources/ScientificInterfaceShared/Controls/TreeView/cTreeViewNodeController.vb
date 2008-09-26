'==============================================================================
'
' $Log: cTreeViewNodeController.vb,v $
' Revision 1.1  2008/09/26 07:31:20  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/07/31 20:45:03  jeroens
' Fixed CLS compliancy state
'
' Revision 1.1  2008/06/01 23:45:11  jeroens
' Separated from Scientific Interface
'
' Revision 1.1  2007/06/14 14:57:07  jeroens
' * Separated off of NavigationPanel
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports EwEUtils.Commands

#End Region ' Imports directive

Namespace Controls

#Region " cTreeViewNodeController "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, maintains a collection of <see cref="cNodeInfo">NodeInfo</see>
    ''' objects.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cTreeViewNodeController

        ''' <summary>List of added nodes</summary>
        Private m_NodeInfoNodes As New List(Of cNodeInfo)
        ''' <summary>TreeView that is being controlled.</summary>
        ''' <remarks>M_TV? haha</remarks>
        Private WithEvents m_tv As TreeView = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Cosntructor, initializes a new instance of this class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal tv As TreeView)
            ' Store ref
            Me.m_tv = tv
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Add node info to this controller.
        ''' </summary>
        ''' <param name="p_treeNodeName"><see cref="TreeNode.Name">Name</see> of the tree node.</param>
        ''' <param name="p_iExecutionState"><see cref="eCoreExecutionState">Core execution state flag</see>
        ''' indicating the state of the EwE Core this node should listen to.</param>
        ''' <param name="p_classType">Class type of the Form to build when invoking this tree node from
        ''' the application navigation tree.</param>
        ''' <param name="p_strHelpURL">Help URL for this node.</param>
        ''' -----------------------------------------------------------------------
        Public Sub Add(ByVal p_treeNodeName As String, ByVal p_iExecutionState As eCoreExecutionState, _
                ByVal p_classType As Type, Optional ByVal p_strHelpURL As String = "")
            m_NodeInfoNodes.Add(New cNodeInfo(p_treeNodeName, p_iExecutionState, p_classType, p_strHelpURL))
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Searches added nodes for <see cref="cNodeInfo">NodeInfo</see> by a given node <see cref="cNodeInfo.NodeName">Name</see>.
        ''' </summary>
        ''' <param name="p_treeNodeName">Name to find</param>
        ''' <returns>The <see cref="cNodeInfo">NodeInfo</see> for the requested node
        ''' <see cref="cNodeInfo.NodeName">Name</see>, or Nothing if no such nodeInfo
        ''' was added.</returns>
        ''' -----------------------------------------------------------------------
        Public Function SearchNodeByName(ByVal p_treeNodeName As String) As cNodeInfo
            For Each eachNode As cNodeInfo In m_NodeInfoNodes
                If p_treeNodeName = eachNode.NodeName Then
                    '' Load the selection
                    Return eachNode
                End If
            Next
            Return Nothing
        End Function

        Public Function SearchNodeByType(ByVal p_treeNodeType As String) As cNodeInfo

            For Each nd As cNodeInfo In m_NodeInfoNodes
                If p_treeNodeType = nd.Type.ToString Then
                    Return nd
                End If
            Next
            Return Nothing

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, expands a nested series of child nodes with one child.
        ''' </summary>
        ''' <param name="node">The node to expand cascading.</param>
        ''' <param name="bExpand">Flag indicating whether node should expand (True)
        ''' or collapse (False).</param>
        ''' -----------------------------------------------------------------------
        Public Sub ExpandCollapseNodes(ByVal node As TreeNode, Optional ByVal bExpand As Boolean = True)
            If bExpand Then
                node.Expand()
                Me.ExpandChildren(node)
            Else
                node.Collapse()
            End If
            node.EnsureVisible()
        End Sub

        Private Sub ExpandChildren(ByVal node As TreeNode)
            If node.GetNodeCount(False) = 1 Then
                node.Expand()
                ExpandChildren(node.FirstNode)
            End If
        End Sub

        ''' -------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; handles a node selection. Invokes a<see cref="NavigationCommand">Navigation command</see>
        ''' for any tree node bearing <see cref="cNodeInfo">rich node information</see>.
        ''' </summary>
        ''' <param name="sender">The tree</param>
        ''' <param name="e">Event info</param>
        ''' -------------------------------------------------------------------------------------------
        Private Sub EmTeeVee_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles m_tv.AfterSelect

            Dim ni As cNodeInfo = Me.SearchNodeByName(e.Node.Name)
            Dim cmdH As CommandHandler = Nothing
            Dim cmd As Command = Nothing
            Dim cmdNav As NavigationCommand = Nothing

            ' Is this a registered node, i.e. does this node have a form attached?
            If (ni IsNot Nothing) Then
                ' #Yes: launch form via central Navigate command
                ' Get command handler
                cmdH = CommandHandler.GetInstance()
                ' Get the navigation command
                cmd = cmdH.GetCommand(NavigationCommand.COMMAND_NAME)
                ' Does this command exist?
                If cmd IsNot Nothing Then
                    ' #Yes: is typeof NavigateCommand?
                    If (TypeOf cmd Is NavigationCommand) Then
                        ' #Yes: Good, now cast it
                        cmdNav = DirectCast(cmd, NavigationCommand)
                        ' ..and launch
                        cmdNav.Invoke(e.Node.Text, ni.NodeName, ni.ExecutionState, ni.Type, ni.HelpURL)
                    End If
                End If
            End If

            Me.m_tv.Visible = True
            Me.ExpandCollapseNodes(e.Node, True)

        End Sub

        Private Sub EmTeeVee_AfterExpand(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles m_tv.AfterExpand
            ExpandCollapseNodes(e.Node)
        End Sub

        ''' -------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; handles a treeview visible state change event. Implemented to make sure
        ''' that the current selected node is visible whenever this control is made visible.
        ''' </summary>
        ''' <param name="sender">The tree</param>
        ''' <param name="e">Event info</param>
        ''' -------------------------------------------------------------------------------------------
        Private Sub EmTeeVee_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tv.VisibleChanged

            If m_tv.Visible Then

                Dim selNd As TreeNode = m_tv.SelectedNode

                If selNd IsNot Nothing Then
                    ExpandCollapseNodes(selNd)
                    selNd.EnsureVisible()
                End If
            End If
        End Sub

    End Class

#End Region ' cTreeViewNodeController

#Region " cNodeInfo "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, maintains information for a single Navigation tree node.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cNodeInfo

        ''' <summary><see cref="TreeNode.Name">Name</see> of the node.</summary>
        Private m_treeNodeName As String = ""
        ''' <summary>Flag indicating the EwE execution state this node belongs to.</summary>
        Private m_executionState As eCoreExecutionState = eCoreExecutionState.Idle
        ''' <summary>Type of the Form class that must be created for this node.</summary>
        Private m_classType As Type
        ''' <summary>Help URL for this node.</summary>
        Private m_strHelpURL As String

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="p_treeNodeName"><see cref="TreeNode.Name">Name</see> of the
        ''' corresponding <see cref="TreeNode">TreeNode</see>.</param>
        ''' <param name="p_iExecutionState">The <see cref="eCoreExecutionState">Core execution state</see>
        ''' that this node belongs to.</param>
        ''' <param name="p_classType">The Type of the Form that needs to be instantiated
        ''' when the corresponding <see cref="TreeNode">TreeNode</see> is selected.</param>
        ''' ---------------------------------------------------------------------------
        Public Sub New(ByVal p_treeNodeName As String, _
                        ByVal p_iExecutionState As eCoreExecutionState, _
                        ByVal p_classType As Type, _
                        ByVal p_strHelpURL As String)
            Me.m_treeNodeName = p_treeNodeName
            Me.m_executionState = p_iExecutionState
            Me.m_classType = p_classType
            Me.m_strHelpURL = p_strHelpURL
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="TreeNode.Name">Name</see> of the
        ''' corresponding <see cref="TreeNode">TreeNode</see>.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property NodeName() As String
            Get
                Return Me.m_treeNodeName
            End Get
        End Property

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eCoreExecutionState">Core execution state</see>
        ''' that this node belongs to.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property ExecutionState() As eCoreExecutionState
            Get
                Return Me.m_executionState
            End Get
        End Property

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get/Set the Type of the Form that needs to be instantiated
        ''' when the corresponding <see cref="TreeNode">TreeNode</see> is selected.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public Property Type() As Type
            Get
                Return Me.m_classType
            End Get
            Set(ByVal classType As Type)
                Me.m_classType = classType
            End Set
        End Property

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the help url for this node.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property HelpURL() As String
            Get
                Return Me.m_strHelpURL
            End Get
        End Property

    End Class

#End Region ' cNodeInfo

End Namespace ' Controls
