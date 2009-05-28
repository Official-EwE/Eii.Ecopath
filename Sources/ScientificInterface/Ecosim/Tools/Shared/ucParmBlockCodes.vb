'==============================================================================
'
' $Log: ucParmBlockCodes.vb,v $
' Revision 1.3  2009/05/28 12:37:54  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.2  2008/12/15 15:56:02  jeroens
' no message
'
' Revision 1.1  2008/11/19 14:42:23  jeroens
' Moved and renamed
'
' Revision 1.1  2008/09/26 07:31:52  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports ScientificInterface.Other

#End Region

Namespace Ecosim

    Public Class ucParmBlockCodes

#Region "Private variables"
        'Color Ramp array
        Private m_lclrColors As New List(Of Color)
        'The selected color number
        Private m_nBlockCodes As Integer = 0
        Private m_iSelectedBlockCode As Integer = 0

#End Region

#Region " Constructor "

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()
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

                Dim sg As cStyleGuide = cStyleGuide.GetInstance()

                Me.m_nBlockCodes = value

                ' Use ramp fully
                Me.m_lclrColors.Clear()
                Me.m_lclrColors.Add(Color.FromArgb(255, 0, 0, 0))
                'Fix bug 441 by JoeH
                'Change
                'Me.m_lclrColors.AddRange(sg.GetColorRamp(Me.m_nBlockCodes))
                Me.m_lclrColors.AddRange(sg.GetColorRamp(Me.m_nBlockCodes - 1))
                'End change

                Me.nudNumBlockCodes.Value = value
                Me.nudSelectedBlockCode.Maximum = value
                Me.slSelectedBlockCode.Maximum = value

                RaiseEvent OnNumBlocksChanged(Me)

                Me.SelectedBlockNum = 0

            End Set
        End Property

        Public ReadOnly Property BlockColors() As List(Of Color)
            Get
                Return Me.m_lclrColors
            End Get
        End Property

        Public ReadOnly Property BlockColor(ByVal i As Integer) As Color
            Get
                If i >= 0 And i <= Me.m_lclrColors.Count - 1 Then
                    Return Me.m_lclrColors(i)
                End If
                Return Me.m_lclrColors(0)
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

                Me.m_iSelectedBlockCode = value
                Me.nudSelectedBlockCode.Value = Me.m_iSelectedBlockCode
                Me.slSelectedBlockCode.Value = Me.m_iSelectedBlockCode

                Dim sBlockWidth As Single = Me.BlockWidth()
                Dim g As Graphics = pbxBlockCodes.CreateGraphics
                Me.DrawBlocks(g)
                Me.DrawBorderSelectedBlockCode(g, Me.m_iSelectedBlockCode * sBlockWidth, sBlockWidth)
                g.Dispose()
                g = Nothing

                RaiseEvent OnBlockSelected(Me)

            End Set
        End Property

#End Region ' Public interfaces

#Region " Public events "

        Public Event OnNumBlocksChanged(ByVal sender As ucParmBlockCodes)
        Public Event OnBlockSelected(ByVal sender As ucParmBlockCodes)

#End Region

#Region " Private event handlers "

        Private Sub ParmBlockCodes_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.nBlockCodes = 30
            Me.SelectedBlockNum = 15
        End Sub

        Private Sub ParmBlockCodes_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize

            If Me.m_nBlockCodes > 0 Then

                Dim sBlockWidth As Single = Me.BlockWidth()
                Dim g As Graphics = Me.pbxBlockCodes.CreateGraphics

                Me.DrawBlocks(g)
                Me.DrawBorderSelectedBlockCode(g, Me.m_iSelectedBlockCode * sBlockWidth, sBlockWidth)

                g.Dispose()
                g = Nothing
            End If

        End Sub

        Private Sub nudNumBlockCodes_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudNumBlockCodes.ValueChanged
            Me.nBlockCodes = Convert.ToInt32(Me.nudNumBlockCodes.Value)
        End Sub

        Private Sub nudSelectedBlockCode_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudSelectedBlockCode.ValueChanged
            Me.SelectedBlockNum = Convert.ToInt32(Me.nudSelectedBlockCode.Value)
        End Sub

        Private Sub pbxBlockCodes_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pbxBlockCodes.Paint
            Dim sBlockWidth As Single = Me.BlockWidth()
            DrawBlocks(e.Graphics)
            DrawBorderSelectedBlockCode(e.Graphics, m_iSelectedBlockCode * sBlockWidth, sBlockWidth)
        End Sub

        Private Sub pbxBlockCodes_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbxBlockCodes.MouseDown
            Me.SelectedBlockNum = CInt(Int(CSng(e.X) / Me.BlockWidth()))
        End Sub

        Private Sub tbSelectedBlockCode_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles slSelectedBlockCode.ValueChanged
            Me.SelectedBlockNum = slSelectedBlockCode.Value
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
