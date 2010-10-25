
Option Strict On
Imports EwECore
Imports ScientificInterface.Other
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.ComponentModel

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control that implements a grid of coloured cells. Every cell can be 
    ''' assigned a block value in a graphical interface. The number of available 
    ''' blocks and the graphical representation of each block is defined via 
    ''' <see cref="ucVulnerabiltyBlocks.BlockColors">BlockColors</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucVulnerabiltyBlocks
        Implements IUIElement

        Private m_uic As cUIContext = Nothing

        ''' <summary>Two-dim arr of integer representing vulnerability blocks layout.</summary>
        Private m_a2iVulBlocks(,) As Integer
        ''' <summary>Block colours to show.</summary>
        Private m_acolors As Color()
        ''' <summary>Index of the selected block with the list of block colours.</summary>
        Private m_iSelectedBlockCodeIndex As Integer = 0
        ''' <summary>Helper var; remembers the last processed mouse position while drawing.</summary>
        ''' <remarks>When drawing, all grid cells on a line between the previous mouse position
        ''' and the current mouse position are considered.</remarks>
        Private m_ptPosPrevious As Point = Nothing

#Region " Constructor "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)

        End Sub

#End Region ' Constructor

#Region " Public Interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the control to a given instance of the EwE core.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(False)> _
        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
                If (Me.m_uic IsNot Nothing) Then
                    Me.RefreshContent()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the control to the core content.
        ''' </summary>
        ''' <remarks>
        ''' This method should be called whenever this number of groups in the 
        ''' core has changed. The control will not keep track of the number of
        ''' groups itself.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub RefreshContent()
            ReDim Me.m_a2iVulBlocks(Me.m_uic.Core.nGroups, Me.m_uic.Core.nGroups)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the block colours that this control should reflect. The
        ''' number of available block colours is set to the number of colours
        ''' passed to this property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property BlockColors() As Color()
            Get
                Return Me.m_acolors
            End Get
            Set(ByVal value As Color())
                Me.m_acolors = value
                Me.Invalidate()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the index of block in the list of 
        ''' <see cref="BlockColors">block colours</see> that the user will be 
        ''' drawing with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property SelectedBlockNum() As Integer
            Get
                Return Me.m_iSelectedBlockCodeIndex
            End Get
            Set(ByVal value As Integer)
                Me.m_iSelectedBlockCodeIndex = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the 2-dimensional array of vulnerability block values that 
        ''' this control maintains. This one-based array is dimensioned by 
        ''' the number of groups in the core.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Vulblocks() As Integer(,)
            Get
                Return Me.m_a2iVulBlocks
            End Get
        End Property

#End Region ' Public interfaces

#Region " Events "

        Protected Overrides Sub OnMouseHover(ByVal e As System.EventArgs)
            MyBase.OnMouseHover(e)
            Me.ProcessMouseHover(Me.PointToClient(Cursor.Position))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Start drawing
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.OnMouseDown(e)

            If (Me.m_uic Is Nothing) Then Return

            Me.Capture = True

            ' Release the last mouse pos
            Me.m_ptPosPrevious = Nothing
            ' Process mouse input
            Me.ProcessMouseInput(e)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Process a draw step or hover information.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.OnMouseMove(e)

            ' Process mouse hover info
            Me.ProcessMouseHover(e.Location)

            If (Me.Capture = False) Then Return

            ' Process mouse input
            Me.ProcessMouseInput(e)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Stop drawing.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.OnMouseUp(e)

            If Not Me.Capture Then Return
            Me.Capture = False
            Me.Invalidate()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Paint the control
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)

            ' Possible performance boost:
            ' - Render onto bitmap, render ichanged cells only (like basemap)

            Try

                If (Me.m_uic Is Nothing) Then Return

                Dim szCell As SizeF = Me.CellSize()
                Dim ppi As cPPIManager = Me.m_uic.Core.PPInteractionManager
                Dim iBlock As Integer = 0

                ' Clear the picture box
                Using tmpBrush As New SolidBrush(Color.White)
                    e.Graphics.FillRectangle(tmpBrush, Me.ClientRectangle)
                End Using

                ' Draw vulnerability blocks
                For i As Integer = 0 To Me.m_uic.Core.nLivingGroups
                    For j As Integer = 0 To Me.m_uic.Core.nGroups
                        If (i = 0 Or j = 0) Then
                            ' Draw row and/or column header cell
                            e.Graphics.FillRectangle(SystemBrushes.Control, i * szCell.Width, j * szCell.Height, szCell.Width, szCell.Height)
                        Else
                            ' Draw content cell
                            If (ppi.isPredPrey(i, j)) Then
                                iBlock = Me.m_a2iVulBlocks(i, j)
                                If iBlock <= Me.m_acolors.Count - 1 Then
                                    ' Render solid block
                                    Using tmpBrush As New SolidBrush(Me.m_acolors(iBlock))
                                        e.Graphics.FillRectangle(tmpBrush, i * szCell.Width, j * szCell.Height, szCell.Width, szCell.Height)
                                    End Using
                                Else
                                    ' JS 22mar08: added crash protection. The m_a2iVulBlocks array contains block codes that exceed the number
                                    '             of blocks that this interface is supposed to use. This fix displays a black X in the cell.
                                    '             This is no solution; the m_a2iVulBlocks array should be re-binned instead.
                                    e.Graphics.DrawLine(Pens.Black, i * szCell.Width, j * szCell.Height, (i + 1) * szCell.Width, (j + 1) * szCell.Height)
                                    e.Graphics.DrawLine(Pens.Black, i * szCell.Width, (j + 1) * szCell.Height, (i + 1) * szCell.Width, j * szCell.Height)
                                End If
                            End If
                        End If
                    Next j
                Next i

                ' Draw grid lines
                For i As Integer = 1 To Me.m_uic.Core.nGroups
                    e.Graphics.DrawLine(Pens.LightGray, 0, i * szCell.Height, Me.ClientRectangle.Width, i * szCell.Height) '(0, i)-(NumLiving + 1, i)
                Next
                For i As Integer = 1 To Me.m_uic.Core.nLivingGroups
                    e.Graphics.DrawLine(Pens.LightGray, i * szCell.Width, 0, i * szCell.Width, Me.ClientRectangle.Height) '(i, 0)-(i, NumGroups + 1)
                Next

                ' Draw row and column labels
                For i As Integer = 1 To Me.m_uic.Core.nGroups
                    e.Graphics.DrawString(CStr(i), Me.Font, SystemBrushes.ControlText, 0, i * szCell.Height)
                Next
                For i As Integer = 1 To Me.m_uic.Core.nLivingGroups
                    e.Graphics.DrawString(CStr(i), Me.Font, SystemBrushes.ControlText, i * szCell.Width, 0)
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, ex.StackTrace)
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Invalidate control when resized.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnSizeChanged(ByVal e As System.EventArgs)
            MyBase.OnSizeChanged(e)
            Me.Invalidate()
        End Sub

#End Region ' Events

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Process mouse input to affect colour blocks in this control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub ProcessMouseInput(ByVal e As System.Windows.Forms.MouseEventArgs)

            If Not Me.Enabled Then Return
            If Not Me.Capture Then Return

            Dim bLeftBtnDown As Boolean = (e.Button = MouseButtons.Left)
            Dim ptPredPreyFrom As Point = Nothing
            Dim ptPredPreyTo As Point = Nothing
            Dim ptPosCurrent As Point = New Point(e.X, e.Y)
            Dim pfPredPrey As PointF = Nothing
            Dim pfIncrement As New PointF(0, 0)
            Dim iNumSteps As Integer = 0

            If (Me.m_ptPosPrevious = Nothing) Then Me.m_ptPosPrevious = ptPosCurrent

            If bLeftBtnDown Then

                ' Translate m_ptPosPrevious into pred/prey prev
                ptPredPreyFrom = Me.PointToPredPrey(Me.m_ptPosPrevious)
                ' Translate ptPosCurrent into pred/prey curr
                ptPredPreyTo = Me.PointToPredPrey(ptPosCurrent)

                ' Calc number of steps to draw
                ' - Determine number of steps to draw (horz or vert)
                Dim iDX As Integer = (ptPredPreyTo.X - ptPredPreyFrom.X)
                Dim iDY As Integer = (ptPredPreyTo.Y - ptPredPreyFrom.Y)
                iNumSteps = Math.Abs(Math.Max(iDX, iDY))
                ' - Determine stepwise pred/prey increment
                If (iNumSteps > 0) Then
                    pfIncrement = New PointF(CSng(iDX / iNumSteps), CSng(iDY / iNumSteps))
                End If

                ' - Start
                pfPredPrey = New PointF(ptPredPreyFrom.X, ptPredPreyFrom.Y)
                ' For each step:
                For iStep As Integer = 0 To iNumSteps
                    ' Set pred/prey block
                    Me.FillBlocks(CInt(Math.Floor(pfPredPrey.X + 0.5)), CInt(Math.Floor(pfPredPrey.Y + 0.5)), Me.m_iSelectedBlockCodeIndex)
                    ' Next pred/prey
                    pfPredPrey.X += pfIncrement.X
                    pfPredPrey.Y += pfIncrement.Y
                Next
                ' End
            End If

            m_ptPosPrevious = ptPosCurrent

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, fills a range of block code cells with a given block
        ''' code value.
        ''' </summary>
        ''' <param name="iPred">The index of the predator block to fill.</param>
        ''' <param name="iPrey">The index of the prey block to fill.</param>
        ''' <param name="iBlockCode">The block code value to fill with.</param>
        ''' <remarks>
        ''' <para>Fill will behave as follows:</para>
        ''' <list type="bullet">
        ''' <item><description>When a non-zero predator and prey cell are given, a single cell is filled.</description></item>
        ''' <item><description>When a non-zero predator and zero prey cell are given, a predator column is filled.</description></item>
        ''' <item><description>When a zero predator and non-zero prey cell are given, a prey row is filled.</description></item>
        ''' <item><description>When both predator and prey are zero, the entire grid is filled with the given block code value.</description></item>
        ''' </list>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub FillBlocks(ByVal iPred As Integer, ByVal iPrey As Integer, ByVal iBlockCode As Integer)

            Dim ppi As cPPIManager = Me.m_uic.Core.PPInteractionManager

            ' Sanity check
            If iPrey < 0 Then Return
            If iPred < 0 Then Return
            If iPrey > Me.m_uic.Core.nGroups Then Return
            If iPred > Me.m_uic.Core.nLivingGroups Then Return

            ' Row or col header clicked?
            If iPrey = 0 Or iPred = 0 Then
                ' #Yes: Col header clicked?
                If iPred = 0 Then
                    ' #Yes: Also row header clicked?
                    If iPrey = 0 Then
                        ' #Yes: fill entire grid
                        For iPred = 1 To Me.m_uic.Core.nLivingGroups
                            For iPrey = 1 To Me.m_uic.Core.nGroups
                                If ppi.isPredPrey(iPred, iPrey) Then Me.m_a2iVulBlocks(iPred, iPrey) = iBlockCode
                            Next iPrey
                        Next iPred
                    Else
                        ' #No: Fill entire prey column
                        For iPred = 1 To Me.m_uic.Core.nLivingGroups
                            If ppi.isPredPrey(iPred, iPrey) Then Me.m_a2iVulBlocks(iPred, iPrey) = iBlockCode
                        Next iPred
                    End If
                Else
                    ' #No: Fill entire predator row
                    For iPrey = 1 To Me.m_uic.Core.nGroups
                        If ppi.isPredPrey(iPred, iPrey) Then Me.m_a2iVulBlocks(iPred, iPrey) = iBlockCode
                    Next iPrey
                End If
            Else
                ' #No: fill single cell
                If ppi.isPredPrey(iPred, iPrey) Then Me.m_a2iVulBlocks(iPred, iPrey) = iBlockCode
            End If

            ' Redraw at your leasure
            Me.Invalidate()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Process mouse hover info to show and populate a tooltip or to hide
        ''' when applicable.
        ''' </summary>
        ''' <param name="ptHover">The hover point to update the tooltip for.</param>
        ''' <remarks>
        ''' The tooltip is hidden if the hover point is absent, or the given 
        ''' hover location indicates an invalid predator and prey index.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub ProcessMouseHover(ByVal ptHover As Point)

            Dim ptPredPrey As New Point(0, 0)
            Dim strToolTip As String = ""
            Dim strPred As String = ""
            Dim strPrey As String = ""

            ' Get hover info, if any
            If (ptHover <> Nothing) Then
                ptPredPrey = Me.PointToPredPrey(ptHover)
            End If

            ' Sanity checks
            If ptPredPrey.X > Me.m_uic.Core.nGroups Then Return
            If ptPredPrey.X < 0 Then Return
            If ptPredPrey.Y > Me.m_uic.Core.nGroups Then Return
            If ptPredPrey.Y < 0 Then Return

            If ptPredPrey.X > 0 Then strPred = String.Format(SharedResources.GENERIC_LABEL_INDEXEDLABEL, ptPredPrey.X, Me.m_uic.Core.EcoPathGroupInputs(ptPredPrey.X).Name)
            If ptPredPrey.Y > 0 Then strPrey = String.Format(SharedResources.GENERIC_LABEL_INDEXEDLABEL, ptPredPrey.Y, Me.m_uic.Core.EcoPathGroupInputs(ptPredPrey.Y).Name)

            ' Format tooltip
            If (ptPredPrey.X <> 0) Or (ptPredPrey.Y <> 0) Then
                If (ptPredPrey.X = 0) Then
                    strToolTip = String.Format(My.Resources.GENERIC_TOOLTIP_PREY, strPrey)
                ElseIf (ptPredPrey.Y = 0) Then
                    strToolTip = String.Format(My.Resources.GENERIC_TOOLTIP_PREDATOR, strPred)
                Else
                    strToolTip = String.Format(My.Resources.GENERIC_TOOLTIP_PREDPREY, strPred, strPrey)
                End If
            End If

            ' Show or hide tooltip
            cToolTipShared.GetInstance().SetToolTip(Me, strToolTip)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; calculates pred/prey index from a given point in the control.
        ''' </summary>
        ''' <param name="pt">The point to request predator and prey index for.</param>
        ''' <returns>A point holding the predator index (X) and prey index (Y) for the
        ''' requested point.</returns>
        ''' -------------------------------------------------------------------
        Private Function PointToPredPrey(ByVal pt As Point) As Point
            Dim szCell As SizeF = Me.CellSize()
            Return New Point(CInt(Math.Max(0, Math.Floor(pt.X / szCell.Width))), _
                             CInt(Math.Max(0, Math.Floor(pt.Y / szCell.Height))))
        End Function

        Private Function CellSize() As SizeF
            Return New SizeF(CSng(Me.ClientRectangle.Width / (Me.m_uic.Core.nLivingGroups + 1)), CSng(Me.ClientRectangle.Height / (Me.m_uic.Core.nGroups + 1)))
        End Function

#End Region ' Internals

    End Class

End Namespace
