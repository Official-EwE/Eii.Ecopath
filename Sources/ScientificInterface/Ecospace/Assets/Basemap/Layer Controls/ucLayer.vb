#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports System.Globalization
Imports System.Threading
Imports ScientificInterface.Ecospace.Basemap.Layers

#End Region ' Imports

Namespace Ecospace

    Partial Public Class ucLayer

        Private m_uic As cUIContext = Nothing
        ''' <summary>Layer</summary>
        Private m_layer As cLayer = Nothing
        ''' <summary>Parent layer group</summary>
        Private m_lgParent As ucLayerGroup = Nothing
        ''' <summary>States whether the mouse is hovering over the control.</summary>
        Private m_bHovering As Boolean = False

        ' Images cache for faster rendering
        Protected Shared g_imgEye0 As Image = My.Resources.Eye_open
        Protected Shared g_imgEye1 As Image = My.Resources.Eye_closed
        Protected Shared g_imgPen0 As Image = My.Resources.Editable
        Protected Shared g_imgPen1 As Image = My.Resources.NotEditable
        Protected Shared g_imgLock As Image = My.Resources.ProtectFormHS

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext, ByVal l As cLayer)

            ' This call is required by the Windows Form Designer.
            Me.InitializeComponent()

            'Enable double buffering
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.UserPaint, True)

            Me.m_uic = uic
            Me.m_layer = l

            AddHandler m_layer.LayerChanged, AddressOf OnLayerChanged
            ' Kick off
            Me.OnLayerChanged(l, cLayer.eChangeFlags.Descriptive)

        End Sub

#End Region ' Constructor

#Region " Properties "

        Public ReadOnly Property Layer() As cLayer
            Get
                Return Me.m_layer
            End Get
        End Property

        Public Property LayerGroup() As ucLayerGroup
            Get
                Return Me.m_lgParent
            End Get
            Set(ByVal value As ucLayerGroup)
                Me.m_lgParent = value
            End Set
        End Property

#End Region ' Properties

#Region " Internal implementation "

        Private Sub OnLayerChanged(ByVal l As cLayer, ByVal updateType As cLayer.eChangeFlags)
            ' Ignore sole map changes
            If (updateType = cLayer.eChangeFlags.Map) Then Return

            If ((updateType And cLayer.eChangeFlags.Selected) = cLayer.eChangeFlags.Selected) Then
                ' Provide instant feedback
                Me.Refresh()
            Else
                ' Just redraw whenever there is time
                Me.Invalidate()

                Dim pm As cPropertyManager = cPropertyManager.GetInstance()
                Dim prop As cProperty = pm.GetProperty(Me.Layer.Source, eVarNameFlags.Name)

                If prop IsNot Nothing Then
                    cToolTipShared.GetInstance().SetToolTip(Me, prop.GetRemark)
                End If
            End If
        End Sub

        Public Sub EditLayer(ByVal openType As dlgEditLayer.eOpenDialogTypes)
            Dim dlg As New dlgEditLayer(Me.m_uic, Me.m_layer, Nothing, openType)
            dlg.ShowDialog()
        End Sub

        ''' <summary>
        ''' Enum to identify areas in the control
        ''' </summary>
        ''' <remarks></remarks>
        Private Enum eAreaTypes As Byte
            ''' <summary>Area not in this control.</summary>
            None
            ''' <summary>Background area of this control.</summary>
            Background
            ''' <summary>Editable area of this control.</summary>
            Editable
            ''' <summary>Visible area of this control.</summary>
            Visible
            ''' <summary>Label area of this control.</summary>
            Label
            ''' <summary>Preview area of this control.</summary>
            Preview
        End Enum

        Private Sub GetRectangles(ByVal rcControl As Rectangle, ByRef rcEditable As Rectangle, ByRef rcVisible As Rectangle, ByRef rcLabel As Rectangle, ByRef rcPreview As Rectangle)

            Dim ci As CultureInfo = Thread.CurrentThread.CurrentUICulture
            Dim iAvgPad As Integer = 3

            If (ci.TextInfo.IsRightToLeft) Then
                ' [ [prev][label    ][vis][edt] ]
                rcEditable.X = rcControl.Width - iAvgPad - 16
                rcEditable.Y = CInt((rcControl.Height - 16) / 2)
                rcEditable.Width = 16
                rcEditable.Height = 16

                rcVisible.X = rcEditable.X - rcEditable.Width - iAvgPad
                rcVisible.Y = rcEditable.Y
                rcVisible.Width = 16
                rcVisible.Height = 16

                rcPreview.X = 2
                rcPreview.Y = 2
                rcPreview.Width = 24
                rcPreview.Height = rcControl.Height - 4

                rcLabel.X = rcPreview.X + rcPreview.Width + iAvgPad
                rcLabel.Y = 0
                rcLabel.Width = rcVisible.X - rcLabel.X - iAvgPad
                rcLabel.Height = rcControl.Height
            Else
                ' [ [edt][vis][label    ][prev] ]
                rcEditable.X = iAvgPad
                rcEditable.Y = CInt((rcControl.Height - 16) / 2)
                rcEditable.Width = 16
                rcEditable.Height = 16

                rcVisible.X = rcEditable.X + rcEditable.Width + iAvgPad
                rcVisible.Y = rcEditable.Y
                rcVisible.Width = 16
                rcVisible.Height = 16

                rcPreview.X = rcControl.Width - 2 - 24
                rcPreview.Y = 2
                rcPreview.Width = 24
                rcPreview.Height = rcControl.Height - 4

                rcLabel.X = rcVisible.X + rcVisible.Width + iAvgPad
                rcLabel.Y = 0
                rcLabel.Width = rcPreview.X - rcLabel.X - iAvgPad
                rcLabel.Height = rcControl.Height
            End If

        End Sub

        Private Function GetArea(ByVal pt As Point) As eAreaTypes
            Dim rcControl As Rectangle = New Rectangle(0, 0, Me.Width, Me.Height)
            Dim rcEditable As Rectangle = Nothing
            Dim rcVisible As Rectangle = Nothing
            Dim rcLabel As Rectangle = Nothing
            Dim rcPreview As Rectangle = Nothing

            Me.GetRectangles(rcControl, rcEditable, rcVisible, rcLabel, rcPreview)

            If rcEditable.Contains(pt) Then Return eAreaTypes.Editable
            If rcVisible.Contains(pt) Then Return eAreaTypes.Visible
            If rcLabel.Contains(pt) Then Return eAreaTypes.Label
            If rcPreview.Contains(pt) Then Return eAreaTypes.Preview
            If rcControl.Contains(pt) Then Return eAreaTypes.Background
            Return eAreaTypes.None

        End Function

#End Region ' Internal implementation

#Region " Events "

        Protected Overrides Sub OnHandleDestroyed(ByVal e As System.EventArgs)
            ' Remove from event handler
            RemoveHandler m_layer.LayerChanged, AddressOf OnLayerChanged

            Me.m_layer = Nothing
            Me.m_lgParent = Nothing
            MyBase.OnHandleDestroyed(e)
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)

            If Me.m_uic Is Nothing Then Return

            Dim rcControl As Rectangle = New Rectangle(0, 0, Me.Width, Me.Height)
            Dim rcEditable As Rectangle = Nothing
            Dim rcVisible As Rectangle = Nothing
            Dim rcLabel As Rectangle = Nothing
            Dim rcPreview As Rectangle = Nothing
            Dim prop As cProperty = Me.m_uic.PropertyManager.GetProperty(Me.Layer.Source, eVarNameFlags.Name)
            Dim img As Image = Nothing
            Dim fmt As New StringFormat()

            Me.GetRectangles(rcControl, rcEditable, rcVisible, rcLabel, rcPreview)

            ' Paint background
            If m_layer.IsSelected Then
                e.Graphics.FillRectangle(SystemBrushes.Highlight, rcControl)
            Else
                e.Graphics.FillRectangle(SystemBrushes.Control, rcControl)
            End If

            ' Draw editable indicator (only when selected or hovering)
            If Me.m_layer.Editor.IsReadOnly Then
                img = g_imgLock
            Else
                If Me.m_bHovering Or Me.m_layer.IsSelected Then
                    If Me.m_layer.Editor.IsEditable Then
                        img = g_imgPen0
                    Else
                        img = g_imgPen1
                    End If
                End If
            End If
            If img IsNot Nothing Then e.Graphics.DrawImage(img, rcEditable)

            ' Draw visible indicator
            If Me.Layer.Renderer.IsVisible Then
                img = g_imgEye0
            Else
                img = g_imgEye1
            End If
            e.Graphics.DrawImage(img, rcVisible)

            ' Draw label
            fmt.LineAlignment = StringAlignment.Center
            fmt.Alignment = StringAlignment.Near
            If Me.m_layer.IsSelected Then
                e.Graphics.DrawString(Me.Layer.Name, Me.Font, SystemBrushes.HighlightText, rcLabel, fmt)
            Else
                e.Graphics.DrawString(Me.Layer.Name, Me.Font, SystemBrushes.ControlText, rcLabel, fmt)
            End If

            ' Draw preview
            ' - Render representation
            e.Graphics.FillRectangle(Brushes.White, rcPreview)
            Me.m_layer.Renderer.RenderPreview(e.Graphics, rcPreview, Me.Layer.Data)
            ' - Render remarks indicator
            cRemarksIndicator.Paint(rcPreview, e.Graphics, prop.HasRemark())
            ' - Render border
            ControlPaint.DrawBorder3D(e.Graphics, rcPreview, Border3DStyle.Sunken, _
                Border3DSide.Bottom Or Border3DSide.Left Or Border3DSide.Top Or Border3DSide.Right)

            ' Draw button borders only when hovering
            If Me.m_bHovering Then
                ControlPaint.DrawBorder(e.Graphics, rcEditable, SystemColors.ControlDark, ButtonBorderStyle.Solid)
                ControlPaint.DrawBorder(e.Graphics, rcVisible, SystemColors.ControlDark, ButtonBorderStyle.Solid)
            End If

            ' Highlight line at the top
            e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0, 0, rcControl.Width, 0)
            ' Shadow line at the bottom
            e.Graphics.DrawLine(SystemPens.ButtonShadow, 0, rcControl.Height - 1, rcControl.Width, rcControl.Height - 1)

        End Sub

        Private Sub ucLayer_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseClick

            Dim flag As cLayer.eChangeFlags = 0

            ' Select layer first
            If Not m_layer.IsSelected Then
                Me.m_layer.IsSelected = True
                flag = flag Or cLayer.eChangeFlags.Selected
            End If

            ' After selecting, determine hit area and process further
            Select Case Me.GetArea(e.Location)

                Case eAreaTypes.Preview
                    Me.m_layer.Update(flag) : flag = 0
                    Me.EditLayer(dlgEditLayer.eOpenDialogTypes.Appearance)

                Case eAreaTypes.Editable
                    Me.m_layer.Editor.IsEditable = Not Me.m_layer.Editor.IsEditable
                    flag = flag Or cLayer.eChangeFlags.Editable

                Case eAreaTypes.Label
                Case eAreaTypes.Background

                Case eAreaTypes.Visible
                    Me.m_layer.Renderer.IsVisible = Not Me.m_layer.Renderer.IsVisible
                    flag = flag Or cLayer.eChangeFlags.Visibility

            End Select

            If flag <> 0 Then
                Me.m_layer.Update(flag)
            End If

        End Sub

        Private Sub ucLayer_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DoubleClick
            Me.EditLayer(dlgEditLayer.eOpenDialogTypes.Data)
        End Sub

        Private Sub ucLayer_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
            ' Determine hit area
            Select Case Me.GetArea(e.Location)
                Case eAreaTypes.Preview, eAreaTypes.Visible ', eAreaTypes.Editable 
                    ' Use hand cursor
                    Me.Cursor = Cursors.Hand
                Case Else
                    ' Use default
                    Me.Cursor = Cursors.Default
            End Select
        End Sub

        ''' <summary>
        ''' Start hovering
        ''' </summary>
        Private Sub ucLayer_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseEnter
            Me.m_bHovering = True
            Me.Invalidate()
        End Sub

        ''' <summary>
        ''' Stop hovering
        ''' </summary>
        Private Sub ucLayer_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseLeave
            Me.m_bHovering = False
            Me.Invalidate()
        End Sub

#End Region ' All events 

    End Class

End Namespace