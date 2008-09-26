Imports EwECore

Public Class frmNetworkNav

    Private WithEvents NetworkManager As cNetworkManager


    Public Sub New(ByRef theNetworkManager As cNetworkManager)
        Me.InitializeComponent()

        NetworkManager = theNetworkManager
        NetworkManager.RunMainNetwork()

    End Sub

    Private Sub tvNavigation_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvNavigation.AfterSelect

        Select Case e.Node.Text
            Case "Relative flows"
                Panel1.Controls.Clear()
                Dim uct As UserControl1 = New UserControl1(NetworkManager)
                Panel1.Controls.Add(uct)
                uct.DisplayRelativeFlows()
            Case "Absolute flows"
                Panel1.Controls.Clear()
                Dim uct As UserControl1 = New UserControl1(NetworkManager)
                Panel1.Controls.Add(uct)
                uct.DisplayAbsoluteFlowsGrid()
            Case Else
        End Select

    End Sub

    Private Sub frmNetworkNav_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        'Select the "Relative flows" node
        Dim ndRelativeFlows As Windows.Forms.TreeNode = FindNode(tvNavigation.Nodes, "Relative flows")
        Dim strary() As String
        ReDim strary(10)

        tvNavigation.BackColor = Drawing.Color.GhostWhite
        If Not ndRelativeFlows Is Nothing Then
            tvNavigation.SelectedNode = ndRelativeFlows
            tvNavigation.SelectedNode.BackColor = Drawing.Color.LightGray
        End If
    End Sub

    Private Function FindNode(ByVal root As Windows.Forms.TreeNodeCollection, ByVal strText As String) As Windows.Forms.TreeNode

        Dim ret As Windows.Forms.TreeNode = Nothing

        For Each nd As Windows.Forms.TreeNode In root
            If nd.Text.Equals(strText) Then
                ret = nd
                Exit For
            Else
                If nd.GetNodeCount(False) <> 0 Then
                    ret = FindNode(nd.Nodes, strText)
                    If Not ret Is Nothing Then Exit For
                End If
            End If
        Next

        Return ret

    End Function

End Class
