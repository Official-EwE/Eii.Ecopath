#Region " Imports "

Option Strict On

Imports EwECore
Imports System.Globalization
Imports System.Threading
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Control to group <see cref="ucLayer">ucLayer</see> controls in a 
    ''' <see cref="ucLayersControl">ucLayersControl</see> instance.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Partial Public Class ucLayerGroup

        Private m_uic As cUIContext = Nothing
        ''' <summary>Collapsed/expanded mode.</summary>
        Private m_bCollapsed As Boolean = False
        ''' <summary>Visible/invisible mode.</summary>
        Private m_bAllLayersShown As Boolean = True
        ''' <summary>Locked for updates.</summary>
        Private m_bLocked As Boolean = True
        ''' <summary>States whether the mouse is hovering over the control.</summary>
        Private m_bHovering As Boolean = False

        ' Images cache for faster rendering
        Protected Shared g_imgEye0 As Image = SharedResources.Eye_open
        Protected Shared g_imgEye1 As Image = SharedResources.Eye_intermediate
        Protected Shared g_imgEye2 As Image = SharedResources.Eye_closed

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext, ByVal strText As String)
            Me.InitializeComponent()

            'Enable double buffering
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.UserPaint, True)

            Me.m_uic = uic
            Me.Text = strText

            Me.UpdateControls()
        End Sub

#End Region ' Constructor

#Region " Public properties "

        Public Sub RemoveAllLayers()

            Dim al As New List(Of cLayer)

            ' Get all layers
            For Each uc As UserControl In Me.fpItems.Controls
                Dim lc As ucLayer = DirectCast(uc, ucLayer)
                al.Add(lc.Layer)
            Next

            ' Now nuke 'em
            For Each l As cLayer In al
                Me.RemoveLayer(l, False)
            Next

            Debug.Assert(Me.fpItems.Controls.Count = 0, "Not all controls deleted!")

            Me.UpdateControls()
            Me.UpdateSize()


        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a <see cref="cLayer">layer</see> to this group.
        ''' </summary>
        ''' <param name="l"><see cref="cLayer">Layer</see> to add.</param>
        ''' <param name="lPosition">Layer to position this layer before, if any</param>
        ''' -------------------------------------------------------------------
        Public Sub AddLayer(ByVal l As cLayer, Optional ByVal lPosition As cLayer = Nothing)
            Dim ucl As New ucLayer(Me.m_uic, l)

            Me.fpItems.Controls.Add(ucl)
            ucl.LayerGroup = Me

            ' Fix control order
            If lPosition IsNot Nothing Then
                Dim uclPos As ucLayer = Me.FindLayerControl(lPosition)
                If uclPos IsNot Nothing Then Me.fpItems.Controls.SetChildIndex(ucl, Me.fpItems.Controls.GetChildIndex(uclPos))
            End If

            ' Set layer visible state
            l.Renderer.IsVisible = Me.m_bAllLayersShown

            Me.UpdateControls()
            Me.UpdateSize()

            AddHandler l.LayerChanged, AddressOf OnLayerChanged
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a <see cref="ucLayer">ucLayer</see> instance from this group.
        ''' </summary>
        ''' <param name="l"><see cref="cLayer">Layer</see> to remove.</param>
        ''' -------------------------------------------------------------------
        Public Sub RemoveLayer(ByVal l As cLayer, Optional ByVal bUpdate As Boolean = True)
            Dim ucl As ucLayer = Me.FindLayerControl(l)

            If (ucl IsNot Nothing) Then
                Me.fpItems.Controls.Remove(ucl)
                ucl.Dispose()
                ucl = Nothing

                If bUpdate Then
                    Me.UpdateControls()
                    Me.UpdateSize()
                End If
            End If

            RemoveHandler l.LayerChanged, AddressOf OnLayerChanged
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of <see cref="ucLayer">ucLayer</see> instances
        ''' in this group.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetLayerCount() As Integer
            Return Me.fpItems.Controls.Count
        End Function

        Public Sub LockUpdates()
            Me.m_bLocked = True
            Me.fpItems.SuspendLayout()
        End Sub

        Public Sub UnlockUpdates()
            Me.fpItems.ResumeLayout()
            Me.m_bLocked = False
            Me.UpdateSize()
        End Sub

        Public Sub ShowAllLayers(ByVal bShow As Boolean)
            Dim lc As ucLayer = Nothing

            ' Toggle layer visiblity
            For Each uc As UserControl In Me.fpItems.Controls
                lc = DirectCast(uc, ucLayer)
                With lc.Layer
                    .Renderer.IsVisible = bShow
                    .Update(cLayer.eChangeFlags.Visibility)
                End With
            Next

            Me.m_bAllLayersShown = bShow
            Me.UpdateControls()
        End Sub

        Public Sub EnableAllLayers(ByVal bEditable As Boolean)
            Dim lc As ucLayer = Nothing

            ' Toggle layer visiblity
            For Each uc As UserControl In Me.fpItems.Controls
                lc = DirectCast(uc, ucLayer)
                With lc.Layer
                    .Editor.IsEditable = bEditable
                    .Update(cLayer.eChangeFlags.Visibility)
                End With
            Next
            Me.UpdateControls()
        End Sub

        Public Sub SetCollapsed(ByVal bCollapsed As Boolean)
            Me.fpItems.Visible = Not bCollapsed
            Me.m_bCollapsed = bCollapsed
            Me.UpdateControls()
            Me.UpdateSize()
        End Sub

#End Region ' Public properties

#Region " Events "

        Private Sub OnToggleView(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Me.ShowAllLayers(Not Me.m_bAllLayersShown)
        End Sub

        Private Sub OnToggleCollapse(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseDoubleClick
            ' Determine hit area
            Select Case Me.GetArea(e.Location)
                Case eAreaTypes.Collapse, eAreaTypes.Label, eAreaTypes.Background
                    Me.SetCollapsed(Not Me.m_bCollapsed)
            End Select
        End Sub

        Private Sub ucLayerGroup_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
            Me.UpdateSize()
        End Sub

        Private Sub pbLayer_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            Dim rcControl As Rectangle = New Rectangle(0, 0, Me.Width, Me.fpItems.Location.Y)
            Dim rcCollapse As Rectangle = Nothing
            Dim rcVisible As Rectangle = Nothing
            Dim rcLabel As Rectangle = Nothing
            Dim img As Image = Nothing
            Dim fmt As New StringFormat()

            Me.GetRectangles(rcControl, rcCollapse, rcVisible, rcLabel)

            ' Paint background
            e.Graphics.FillRectangle(SystemBrushes.Control, rcControl)

            ' Draw collapse indicator
            If Me.m_bCollapsed Then
                img = SharedResources.Collapsed
            Else
                img = SharedResources.Expanded
            End If
            e.Graphics.DrawImage(img, rcCollapse)

            ' Draw visible indicator
            Select Case Me.AllLayersVisible()
                Case TriState.True
                    img = g_imgEye0
                    Me.m_bAllLayersShown = True
                Case TriState.False
                    img = g_imgEye2
                    Me.m_bAllLayersShown = False
                Case TriState.UseDefault
                    img = g_imgEye1
            End Select
            e.Graphics.DrawImage(img, rcVisible)

            ' Draw label
            fmt.LineAlignment = StringAlignment.Center
            fmt.Alignment = StringAlignment.Near
            Using ft As New Font(Me.Font, FontStyle.Bold)
                e.Graphics.DrawString(String.Format(SharedResources.GENERIC_LABEL_DETAILED, Me.Text, Me.fpItems.Controls.Count), _
                    ft, SystemBrushes.ControlText, rcLabel, fmt)
            End Using

            ' Draw button borders only when hovering
            If Me.m_bHovering Then
                ControlPaint.DrawBorder(e.Graphics, rcCollapse, SystemColors.ControlDark, ButtonBorderStyle.Solid)
                ControlPaint.DrawBorder(e.Graphics, rcVisible, SystemColors.ControlDark, ButtonBorderStyle.Solid)
            End If

            ' Highlight line at the top
            e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0, 0, rcControl.Width, 0)
            ' Shadow line at the bottom
            e.Graphics.DrawLine(SystemPens.ButtonShadow, 0, rcControl.Height - 1, rcControl.Width, rcControl.Height - 1)

        End Sub

        Private Sub ucLayerGroup_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseClick

            ' Determine hit area
            Select Case Me.GetArea(e.Location)

                Case eAreaTypes.Collapse
                    Me.SetCollapsed(Not Me.m_bCollapsed)

                Case eAreaTypes.Label
                Case eAreaTypes.Background

                Case eAreaTypes.Visible
                    Me.ShowAllLayers(Not Me.m_bAllLayersShown)

            End Select

        End Sub

        Private Sub ucLayerGroup_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseMove

            ' Determine hit area
            Select Case Me.GetArea(e.Location)
                Case eAreaTypes.Collapse, eAreaTypes.Visible
                    ' Use hand cursor
                    Me.Cursor = Cursors.Hand
                Case Else
                    ' Use default
                    Me.Cursor = Cursors.Default
            End Select

        End Sub

        Private Sub ucLayerGroup_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseEnter
            Me.m_bHovering = True
            Me.Invalidate(False)
        End Sub

        Private Sub ucLayerGroup_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseLeave
            Me.m_bHovering = False
            Me.Invalidate(False)
        End Sub

        Private Sub OnLayerChanged(ByVal l As cLayer, ByVal updateFlag As cLayer.eChangeFlags)
            ' Update whenever child layer visiblity changes
            If ((updateFlag And cLayer.eChangeFlags.Visibility) = cLayer.eChangeFlags.Visibility) Then
                ' Redraw at some point
                Me.Invalidate(False)
            End If
        End Sub

#End Region ' Events

#Region " Internal implementation "

        ''' <summary>
        ''' Enum to identify areas in the control
        ''' </summary>
        ''' <remarks></remarks>
        Private Enum eAreaTypes As Byte
            ''' <summary>Area not in this control.</summary>
            None
            ''' <summary>Background area of this control.</summary>
            Background
            ''' <summary>Collapse area of this control.</summary>
            Collapse
            ''' <summary>Visible area of this control.</summary>
            Visible
            ''' <summary>Label area of this control.</summary>
            Label
        End Enum

        Private Sub GetRectangles(ByVal rcControl As Rectangle, ByRef rcCollapse As Rectangle, ByRef rcVisible As Rectangle, ByRef rcLabel As Rectangle)

            Dim iAvgPad As Integer = 3

            If (Me.m_uic.StyleGuide.IsRightToLeft) Then
                ' [ [prev][label    ][vis][edt] ]
                rcCollapse.X = rcControl.Width - iAvgPad - 16
                rcCollapse.Y = 2
                rcCollapse.Width = 16
                rcCollapse.Height = 16

                rcVisible.X = rcCollapse.X - rcCollapse.Width - iAvgPad
                rcVisible.Y = rcCollapse.Y
                rcVisible.Width = 16
                rcVisible.Height = 16

                rcLabel.X = iAvgPad
                rcLabel.Y = 0
                rcLabel.Width = rcVisible.X - rcLabel.X - iAvgPad
                rcLabel.Height = 20
            Else
                ' [ [edt][vis][label    ][prev] ]
                rcCollapse.X = iAvgPad
                rcCollapse.Y = 2
                rcCollapse.Width = 16
                rcCollapse.Height = 16

                rcVisible.X = rcCollapse.X + rcCollapse.Width + iAvgPad
                rcVisible.Y = rcCollapse.Y
                rcVisible.Width = 16
                rcVisible.Height = 16

                rcLabel.X = rcVisible.X + rcVisible.Width + iAvgPad
                rcLabel.Y = 0
                rcLabel.Width = rcControl.Width - iAvgPad - rcLabel.X
                rcLabel.Height = 20
            End If

        End Sub

        Private Function GetArea(ByVal pt As Point) As eAreaTypes
            Dim rcControl As Rectangle = New Rectangle(0, 0, Me.Width, Me.fpItems.Location.Y)
            Dim rcCollapse As Rectangle = Nothing
            Dim rcVisible As Rectangle = Nothing
            Dim rcLabel As Rectangle = Nothing

            Me.GetRectangles(rcControl, rcCollapse, rcVisible, rcLabel)

            If rcCollapse.Contains(pt) Then Return eAreaTypes.Collapse
            If rcVisible.Contains(pt) Then Return eAreaTypes.Visible
            If rcLabel.Contains(pt) Then Return eAreaTypes.Label
            If rcControl.Contains(pt) Then Return eAreaTypes.Background
            Return eAreaTypes.None

        End Function

        Private Sub UpdateControls()
            Me.Invalidate()
        End Sub

        Private Sub UpdateSize()

            ' Nope!
            If Me.m_bLocked Then Return

            Me.fpItems.Width = Me.Width - Me.Margin.Horizontal
            For Each uc As UserControl In Me.fpItems.Controls
                uc.Width = Me.fpItems.Width - Me.fpItems.Margin.Horizontal - uc.Padding.Horizontal
            Next

            If Me.m_bCollapsed Then
                Me.Size = New Size(Me.Width, Me.fpItems.Location.Y)
            Else
                Dim c As Control = Nothing
                Dim i As Integer = Me.fpItems.Controls.Count

                If i = 0 Then
                    Me.fpItems.Height = 0
                Else
                    c = Me.fpItems.Controls(0)
                    Me.fpItems.Height = Me.fpItems.Controls.Count * (c.Height + c.Padding.Vertical + c.Margin.Vertical) + Me.fpItems.Padding.Vertical
                End If
                Me.Size = New Size(Me.Width, Me.fpItems.Location.Y + Me.fpItems.Size.Height)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Find a child layer control for a given layer
        ''' </summary>
        ''' <param name="layer"><see cref="cLayer">Layer</see> to find the
        ''' <see cref="ucLayer">control</see> for.</param>
        ''' <returns>A child layer control, or nothing if the control could
        ''' not be found.</returns>
        ''' -------------------------------------------------------------------
        Private Function FindLayerControl(ByVal layer As cLayer) As ucLayer
            Dim ucl As ucLayer = Nothing
            For Each uc As UserControl In Me.fpItems.Controls
                ucl = DirectCast(uc, ucLayer)
                If Object.ReferenceEquals(ucl.Layer, layer) Then
                    Return ucl
                End If
            Next uc
            Return Nothing
        End Function

        Private Function AllLayersVisible() As TriState
            Dim ucl As ucLayer = Nothing
            Dim iVisible As Integer = 0
            For Each uc As UserControl In Me.fpItems.Controls
                ucl = DirectCast(uc, ucLayer)
                If (ucl.Layer.Renderer.IsVisible) Then iVisible += 1
            Next uc

            ' Return TRUE if all layers visible OR no layers attached
            If iVisible = fpItems.Controls.Count Then Return TriState.True
            ' ELSE return FALSE if no layers visible
            If iVisible = 0 Then Return TriState.False
            ' ELSE return 'partial visible'
            Return TriState.UseDefault

        End Function

#End Region ' Internal implementation

    End Class

End Namespace