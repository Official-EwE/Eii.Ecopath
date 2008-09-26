'==============================================================================
'
' Controller for collapsible panels
' Based on 'Sliding Panel User Control', Sufian Mehmood Sheikh, sufian@my.web.pk
'
' $Log: ucCollapsiblePanelController.vb,v $
' Revision 1.1  2008/09/26 07:31:14  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/09/15 18:42:19  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports System.ComponentModel

Namespace Controls

    <DefaultProperty("Caption")> _
    Public Class ucCollapsiblePanelController

#Region " Private vars "

        Private m_iExpandedHeight As Integer = 100
        Private m_iAnimationRate As Integer = 2
        Private m_bCollapsed As Boolean = False

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            InitializeComponent()

            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

#End Region ' Constructor

#Region " Events "

        Private Sub DoLoad(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles Me.Load

            Me.m_iExpandedHeight = Me.Parent.Height
            Me.Dock = DockStyle.Top

        End Sub

        Private Sub DoPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
                Handles Me.Paint

            Dim xImg As Integer = Me.Padding.Left
            Dim xText As Integer = xImg + 16
            Dim yImg As Integer = Math.Max(1, CInt((Me.Height - 16) / 2))
            Dim img As Image = Nothing
            Dim sft As New StringFormat(StringFormatFlags.NoWrap)

            Using br As New SolidBrush(Me.BackColor)
                e.Graphics.FillRectangle(br, Me.ClientRectangle)
            End Using

            If Me.m_bCollapsed = False Then
                img = My.Resources.plus
            Else
                img = My.Resources.minus
            End If
            e.Graphics.DrawImageUnscaledAndClipped(img, New Rectangle(xImg, yImg, img.Width, Math.Min(Me.Height - Me.Padding.Bottom - yImg, img.Height)))

            Using br As New SolidBrush(Me.ForeColor)
                e.Graphics.DrawString(Me.Text, Me.Font, br, _
                    New Rectangle(xText, Me.Padding.Top, Me.Width - xText - Me.Padding.Right, Me.Height - Me.Padding.Vertical), sft)
            End Using

        End Sub

        Private Sub OnCollapseClick(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles Me.Click
            Me.Collapsed = (Not Me.Collapsed)
        End Sub

#End Region ' Events

#Region " Properties "

        <Category("Collapsible")> _
        Public Property Caption() As String
            Get
                If String.IsNullOrEmpty(Me.Text) Then Return Me.Name
                Return Me.Text
            End Get
            Set(ByVal value As String)
                Me.Text = value
                Me.Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets/Sets Animation Rate
        ''' </summary>
        <Category("Collapsible")> _
        Public Property AnimationRate() As Integer
            Get
                Return Me.m_iAnimationRate
            End Get
            Set(ByVal value As Integer)
                Me.m_iAnimationRate = value
            End Set
        End Property

        <Category("Collapsible")> _
        Public Property Collapsed() As Boolean
            Get
                Return Me.m_bCollapsed
            End Get
            Set(ByVal value As Boolean)
                If value <> Me.m_bCollapsed Then
                    Me.m_bCollapsed = value
                    Me.Invalidate()
                    Me.Animate()
                End If
            End Set
        End Property

        <Browsable(False)> _
        Public Overrides Property Dock() As System.Windows.Forms.DockStyle
            Get
                Return MyBase.Dock
            End Get
            Set(ByVal value As System.Windows.Forms.DockStyle)
                MyBase.Dock = value
            End Set
        End Property

#End Region ' Properties

#Region " Internals "

        Private Sub Animate()

            If Me.m_bCollapsed Then

                ' Borders, etc
                Dim iHeightOffset As Integer = Me.Parent.Height - Me.Parent.ClientRectangle.Height

                While Me.Parent.Height > Me.Height + iHeightOffset
                    Application.DoEvents()
                    Me.Parent.Height -= AnimationRate
                End While
                Me.Parent.Height = Me.Height + iHeightOffset

            Else

                While Me.Parent.Height < Me.m_iExpandedHeight
                    Application.DoEvents()
                    Me.Parent.Height += AnimationRate
                End While
                Me.Parent.Height = Me.m_iExpandedHeight

            End If

        End Sub

#End Region ' Internals

    End Class

End Namespace ' Controls
