' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' <summary>
    ''' Treeview-derived class that can contain tree items with a URL link. An
    ''' <see cref="cNavigateTreeview.Navigate"/> event is thrown 
    ''' </summary>
    Public Class cNavigateTreeview
        Inherits cThemedTreeView

        Private m_bShowTime As Boolean = False

        Public Sub New()
            MyBase.New()
            ' Hack to allow a bit more room for rendering items
            Me.Font = New Font(Me.Font, FontStyle.Bold)
            Me.FullRowSelect = True
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint, True)
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' <see cref="TreeNode"/>-derived class that additionally maintains a 
        ''' <see cref="cHyperlinkTreeNode.Hyperlink"/>
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Class cHyperlinkTreeNode
            Inherits TreeNode

            Private m_strHyperlink As String = ""
            Private m_time As Date

            Public Sub New(strText As String,
                           strHyperlink As String,
                           time As Date)
                MyBase.New(strText)
                Me.m_strHyperlink = strHyperlink
                Me.m_time = time
            End Sub

            Public Sub New(strText As String,
                           strHyperlink As String,
                           time As Date,
                           children() As TreeNode)
                MyBase.New(strText, children)
                Me.m_strHyperlink = strHyperlink
            End Sub

            Public Sub New(strText As String,
                           strHyperlink As String,
                           time As Date,
                           imageindex As Integer,
                           selectedImageIndex As Integer)
                MyBase.New(strText, imageindex, selectedImageIndex)
                Me.m_strHyperlink = strHyperlink
            End Sub

            Public Sub New(strText As String,
                           strHyperlink As String,
                           imageindex As Integer,
                           time As Date,
                           selectedImageIndex As Integer,
                           children() As TreeNode)
                MyBase.New(strText, imageindex, selectedImageIndex, children)
                Me.m_strHyperlink = strHyperlink
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set the hyperlink attached to the node.
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

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set the time attached to the node.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Time As Date
                Get
                    Return Me.m_time
                End Get
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

        Public Property ShowTime() As Boolean
            Get
                Return Me.m_bShowTime
            End Get
            Set(value As Boolean)
                Me.m_bShowTime = value
                Me.Invalidate()
            End Set
        End Property

        Public Event Navigate(sender As Object, e As TreeViewNavigateEventArgs)

        Protected Overrides Sub OnNodeMouseClick(e As System.Windows.Forms.TreeNodeMouseClickEventArgs)
            MyBase.OnNodeMouseClick(e)
            ' Total hack to separate expand, collapse and navigation clicks
            If (Me.HasHyperlink(e.Node) And e.Location.X > 25) Then
                Try
                    Dim args As New TreeViewNavigateEventArgs(DirectCast(e.Node, cHyperlinkTreeNode), TreeViewAction.ByMouse)
                    Me.OnNavigate(args)
                Catch ex As Exception

                End Try
            End If
        End Sub

        Protected Overrides Sub OnDrawNode(e As System.Windows.Forms.DrawTreeNodeEventArgs)

            Dim fmt As TextFormatFlags = TextFormatFlags.EndEllipsis Or TextFormatFlags.SingleLine
            Dim bIsURL As Boolean = Me.HasHyperlink(e.Node)
            Dim clrText As Color = SystemColors.ControlText
            Dim rcItem As Rectangle = e.Bounds
            Dim bShowTime As Boolean = Me.m_bShowTime ' And (TypeOf (e.Node) Is cHyperlinkTreeNode)

            Dim dx As Integer = Me.ClientRectangle.Width - e.Bounds.Width
            If Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft Then
                fmt = fmt Or TextFormatFlags.RightToLeft
                rcItem.Width += dx
                rcItem.X -= dx
            Else
                rcItem.Width += dx
            End If
            Dim rcTime As Rectangle = e.Bounds

            Dim rcFull As Rectangle = e.Bounds
            rcFull.Width = Me.ClientRectangle.Width ' And this happily ignores R2L reading order...
            If ((e.State And (TreeNodeStates.Focused)) > 0) Then
                e.Graphics.FillRectangle(SystemBrushes.Highlight, rcFull)
                clrText = SystemColors.HighlightText
            Else
                e.Graphics.FillRectangle(SystemBrushes.Window, rcFull)
            End If

            If Me.m_bShowTime And e.Node.Parent Is Nothing Then
                Dim sz As Size = TextRenderer.MeasureText("XX:XX:XXW", Me.Font)
                rcItem.Width -= sz.Width
                If (fmt And TextFormatFlags.RightToLeft) > 0 Then
                    rcTime.X = e.Bounds.Width - sz.Width
                Else
                    rcItem.X += sz.Width
                End If

                Using ft As New Font(Me.Font, FontStyle.Regular)
                    TextRenderer.DrawText(e.Graphics, DirectCast(e.Node, cHyperlinkTreeNode).Time.ToShortTimeString, ft, rcTime, clrText, fmt)
                End Using
            End If

            Using ft As New Font(Me.Font, If(bIsURL, FontStyle.Underline, FontStyle.Regular))
                TextRenderer.DrawText(e.Graphics, e.Node.Text, ft, rcItem, clrText, fmt)
            End Using

            '  e.DrawDefault = False

        End Sub

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
