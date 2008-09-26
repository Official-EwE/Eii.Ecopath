'==============================================================================
'
' $Log: ParmBlockCodes.vb,v $
' Revision 1.1  2008/09/26 07:31:52  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.26  2008/07/22 22:04:16  joeh
' Fix bug 441 - Fit to time series number of blocks from sensitivity search not the same as set by user
'
' Revision 1.25  2008/02/04 18:55:30  jeroens
' Fixed colour ramp usage
'
' Revision 1.24  2008/02/03 03:52:30  jeroens
' Rendered to client rectangle size, not control rectangle size to take borders into consideration
'
' Revision 1.23  2007/11/11 16:52:19  jeroens
' * Codes -> Colours
'
' Revision 1.22  2007/11/06 19:24:45  jeroens
' * Broadcasts events
'
' Revision 1.21  2007/11/03 18:02:26  jeroens
' * ParmBlockCodes exposes selected block code
'
' Revision 1.20  2007/11/02 14:32:06  jeroens
' + Uses ucSlider
'
' Revision 1.19  2007/11/01 13:45:51  jeroens
' * Fixed init issue
'
' Revision 1.18  2007/10/15 15:59:21  jeroens
' + Added trackbar, selected indicator
'
' Revision 1.17  2007/10/15 00:42:39  jeroens
' * Fixed blue selection border rendering feature
' - Cleaned-up
'
' Revision 1.16  2007/10/14 23:19:04  jeroens
' * Fixed bug 304
'
' Revision 1.15  2007/10/14 22:02:14  jeroens
' * Updated to styleguide changes
'
' Revision 1.14  2007/10/03 01:54:30  jeroens
' * Reworked styleguide, colormanager
'
' Revision 1.13  2007/09/28 01:15:00  joeh
' Add resize event
'
' Revision 1.12  2007/09/10 23:46:36  fgao
' Expose two more properties for FPS use..
'
' Revision 1.11  2007/08/31 00:35:24  joeh
' Draw border around the selected block code during paint event
'
' Revision 1.10  2007/08/30 19:21:59  joeh
' Make the spin wheel editable
'
' Revision 1.9  2007/08/30 18:19:40  joeh
' Change graphics object from modular to local level
' Change user control dock style from Bottom to Fill
'
' Revision 1.8  2007/08/23 01:05:46  joeh
' Set m_Colors(0).ForeColor  to black
'
' Revision 1.7  2007/08/22 21:20:26  joeh
' Change the nBlockCodes property from ReadOnly to both Read and Set
'
' Revision 1.6  2007/08/21 00:32:43  joeh
' Move some local variables to modular level
'
' Revision 1.5  2007/08/18 03:25:22  joeh
' no message
'
' Revision 1.4  2007/08/16 23:25:47  joeh
' Put a border around the selected block code
'
' Revision 1.3  2007/08/16 00:26:25  joeh
' Use color scheme used in Ecospace and create the public interface
'
' Revision 1.2  2007/08/15 00:25:52  joeh
' Initial implementation
'
' Revision 1.1  2007/08/14 19:37:15  joeh
' Add ParmBlockCodes user control
'
'
'==============================================================================

#Region "Imports directive"

Option Explicit On
Option Strict On

Imports ScientificInterface.Other

#End Region

Namespace Ecosim

    Public Class ParmBlockCodes

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

                Dim sg As StyleGuide = StyleGuide.GetInstance()

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

        Public Event OnNumBlocksChanged(ByVal sender As ParmBlockCodes)
        Public Event OnBlockSelected(ByVal sender As ParmBlockCodes)

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
