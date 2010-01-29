#Region " Imports "

Option Explicit On
Option Strict On

Imports ScientificInterface.Other

#End Region

Namespace Ecosim

    Public Class ucParmBlockCodes
        Implements IUIElement

#Region "Private variables"

        ''' <summary>UI context.</summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary>Number of blocks to show.</summary>
        Private m_nBlockCodes As Integer = 30
        ''' <summary>Selected color index.</summary>
        Private m_iSelectedBlockCode As Integer = 15

#End Region

#Region " Constructor "

        Public Sub New()
            Me.InitializeComponent()
            Me.Dock = DockStyle.Fill
        End Sub

#End Region ' Constructor

#Region " Public interfaces "

        Public Property nBlockCodes() As Integer
            Get
                Return Me.m_nBlockCodes
            End Get

            Set(ByVal value As Integer)

                If value = Me.m_nBlockCodes Then Return

                Me.m_nBlockCodes = value

                Me.nudNumBlockCodes.Value = value
                Me.nudSelectedBlockCode.Maximum = value
                Me.slSelectedBlockCode.Maximum = value

                RaiseEvent OnNumBlocksChanged(Me)

                Me.SelectedBlockNum = 0
                Me.Invalidate(True)

            End Set
        End Property

        Public ReadOnly Property BlockColors() As List(Of Color)
            Get
                Dim lcolors As List(Of Color) = Me.m_uic.StyleGuide.GetEwE5ColorRamp(Me.m_nBlockCodes - 1)
                lcolors.Insert(0, Color.Black)
                Return lcolors
            End Get
        End Property

        Public ReadOnly Property BlockColor(ByVal i As Integer) As Color
            Get
                If i >= 0 And i <= Me.nBlockCodes Then
                    Return Me.BlockColors(i)
                End If
                Return Color.Black
            End Get
        End Property

        Public ReadOnly Property SelectedBlockCode() As Color
            Get
                Return Me.BlockColor(Me.m_iSelectedBlockCode)
            End Get
        End Property

        Public Property SelectedBlockNum() As Integer
            Get
                Return Me.m_iSelectedBlockCode
            End Get
            Set(ByVal value As Integer)
                Me.m_iSelectedBlockCode = Math.Max(0, Math.Min(Me.m_nBlockCodes, value))

                If (Me.m_uic Is Nothing) Then Return

                ' Update controls
                Me.nudSelectedBlockCode.Value = Me.m_iSelectedBlockCode
                Me.slSelectedBlockCode.Value = Me.m_iSelectedBlockCode

                RaiseEvent OnBlockSelected(Me)

                Me.Invalidate()

            End Set
        End Property

#End Region ' Public interfaces

#Region " Public events "

        Public Event OnNumBlocksChanged(ByVal sender As ucParmBlockCodes)
        Public Event OnBlockSelected(ByVal sender As ucParmBlockCodes)

#End Region ' Public events

#Region " Public properties "

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

#End Region 'Public properties

#Region " Private event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub

        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)
            Me.Invalidate()
        End Sub

        Private Sub nudNumBlockCodes_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles nudNumBlockCodes.ValueChanged
            ' Wait until ready to respond
            If (Me.m_uic Is Nothing) Then Return
            Me.nBlockCodes = Convert.ToInt32(Me.nudNumBlockCodes.Value)
        End Sub

        Private Sub nudSelectedBlockCode_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles nudSelectedBlockCode.ValueChanged
            ' Wait until ready to respond
            If (Me.m_uic Is Nothing) Then Return
            Me.SelectedBlockNum = Convert.ToInt32(Me.nudSelectedBlockCode.Value)
        End Sub

        Private Sub pbxBlockCodes_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
            Handles pbxBlockCodes.MouseDown
            ' Wait until ready to respond
            If (Me.m_uic Is Nothing) Then Return
            Me.SelectedBlockNum = CInt(Int(CSng(e.X) / Me.BlockWidth()))
        End Sub

        Private Sub tbSelectedBlockCode_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles slSelectedBlockCode.ValueChanged
            ' Wait until ready to respond
            If (Me.m_uic Is Nothing) Then Return
            Me.SelectedBlockNum = slSelectedBlockCode.Value
        End Sub

        Private Sub pbxBlockCodes_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles pbxBlockCodes.Paint
            If (Me.m_uic Is Nothing) Then Return
            Dim sBlockWidth As Single = Me.BlockWidth()
            Me.DrawBlocks(e.Graphics)
            Me.DrawBorderSelectedBlockCode(e.Graphics, m_iSelectedBlockCode * sBlockWidth, sBlockWidth)
        End Sub

#End Region

#Region " Internal implementation "

        Private Sub DrawBlocks(ByVal g As Graphics)
            Dim sBlockWidth As Single = Me.BlockWidth()
            For iBlock As Integer = 0 To Me.m_nBlockCodes
                Using tmpBrush As New SolidBrush(Me.BlockColor(iBlock))
                    g.FillRectangle(tmpBrush, sBlockWidth * iBlock, 0, sBlockWidth, Me.pbxBlockCodes.Height)
                End Using
            Next
        End Sub

        Private Sub DrawBorderSelectedBlockCode(ByVal g As Graphics, ByVal sngX As Single, ByVal sngWidth As Single)
            Const nPenWidth As Integer = 3
            Dim penDrawing As New System.Drawing.Pen(Color.Blue, nPenWidth)
            g.DrawRectangle(penDrawing, sngX, 0, sngWidth, Me.pbxBlockCodes.ClientRectangle.Height - nPenWidth + 1)
            penDrawing.Dispose()
            penDrawing = Nothing
        End Sub

        Private Function BlockWidth() As Single
            Return CSng(pbxBlockCodes.ClientRectangle.Width / (Me.m_nBlockCodes + 1)) ' Allow for 0-color black
        End Function

#End Region ' Internal implementation

    End Class

End Namespace
