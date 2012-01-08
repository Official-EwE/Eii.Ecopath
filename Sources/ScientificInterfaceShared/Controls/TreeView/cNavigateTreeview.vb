#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls

    ''' <summary>
    ''' Treeview-derived class that can contain tree items with a URL link. An
    ''' <see cref="cNavigateTreeview.Navigate"/> event is thrown 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class cNavigateTreeview
        Inherits TreeView

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' <see cref="TreeNode"/>-derived class that additionally maintains a 
        ''' <see cref="cHyperlinkTreeNode.Hyperlink"/>
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Class cHyperlinkTreeNode
            Inherits TreeNode

            Private m_strHyperlink As String = ""

             Public Sub New(strText As String, _
                           strHyperlink As String)
                MyBase.New(strText)
                Me.m_strHyperlink = strHyperlink
            End Sub

            Public Sub New(strText As String, _
                           strHyperlink As String, _
                           children() As TreeNode)
                MyBase.New(strText, children)
                Me.m_strHyperlink = strHyperlink
            End Sub

            Public Sub New(strText As String, _
                           strHyperlink As String, _
                           imageindex As Integer, _
                           selectedImageIndex As Integer)
                MyBase.new(strText, imageindex, selectedImageIndex)
                Me.m_strHyperlink = strHyperlink
            End Sub

            Public Sub New(strText As String, _
                           strHyperlink As String, _
                           imageindex As Integer, _
                           selectedImageIndex As Integer, _
                           children() As TreeNode)
                MyBase.New(strText, imageindex, selectedImageIndex, children)
                Me.m_strHyperlink = strHyperlink
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set the hyperlink attached to the control.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property Hyperlink As String
                Get
                    Return Me.m_strHyperlink
                End Get
                Set(strHyperlink As String)
                    Me.m_strHyperlink = strHyperlink
                End Set
            End Property

        End Class

        Public Class TreeViewNavigateEventArgs
            Inherits TreeViewEventArgs

            Public Sub New(node As cHyperlinkTreeNode, action As TreeViewAction)
                MyBase.New(node, action)
            End Sub
            Shadows ReadOnly Property Node As cHyperlinkTreeNode
                Get
                    Return DirectCast(MyBase.Node, cHyperlinkTreeNode)
                End Get
            End Property

        End Class

        Public Event Navigate(sender As Object, e As TreeViewNavigateEventArgs)

        Protected Overrides Sub OnAfterSelect(e As System.Windows.Forms.TreeViewEventArgs)

            If (Me.HasHyperlink(e.Node)) Then
                Try
                    Dim args As New TreeViewNavigateEventArgs(DirectCast(e.Node, cHyperlinkTreeNode), e.Action)
                    Me.OnNavigate(args)
                Catch ex As Exception

                End Try
            Else
                MyBase.OnAfterSelect(e)
            End If

        End Sub

        Protected Overrides Sub OnDrawNode(e As System.Windows.Forms.DrawTreeNodeEventArgs)

            Dim bIsURL As Boolean = Me.HasHyperlink(e.Node)
            Dim rc As Rectangle = e.Bounds

            rc.Inflate(2, 0)
            rc.Offset(1, 0)

            Dim br As Brush = SystemBrushes.ControlText
            Dim ft As Font = Nothing

            If (e.State And TreeNodeStates.Selected) > 0 Then
                br = SystemBrushes.HighlightText
            ElseIf (e.State And TreeNodeStates.Hot) > 0 Then
                br = SystemBrushes.HotTrack
            Else
                br = SystemBrushes.ControlText
            End If

            If bIsURL Then
                ft = New Font(Me.Font, FontStyle.Underline)
            Else
                ft = New Font(Me.Font, FontStyle.Regular)
            End If
            e.Graphics.DrawString(e.Node.Text, ft, br, rc)

            ft.Dispose()

        End Sub

        ' JS: Cursor hover feedback is overkill

        'Protected Overrides Sub OnMouseMove(e As System.Windows.Forms.MouseEventArgs)
        '    MyBase.OnMouseMove(e)
        '    If (Me.HasHyperlink(Me.GetNodeAt(e.Location))) Then
        '        Me.Cursor = Cursors.Hand
        '    Else
        '        Me.Cursor = Cursors.Default
        '    End If
        'End Sub

        Protected Overridable Sub OnNavigate(e As TreeViewNavigateEventArgs)
            RaiseEvent Navigate(Me, e)
        End Sub

        Protected Function HasHyperlink(node As TreeNode) As Boolean
            If (node Is Nothing) Then Return False
            If (Not TypeOf node Is cHyperlinkTreeNode) Then Return False
            Return Not String.IsNullOrWhiteSpace(DirectCast(node, cHyperlinkTreeNode).Hyperlink)
        End Function

    End Class

End Namespace ' Controls
