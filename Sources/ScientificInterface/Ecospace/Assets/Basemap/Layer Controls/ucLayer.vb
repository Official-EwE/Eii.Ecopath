'==============================================================================
'
' $Log: ucLayer.vb,v $
' Revision 1.1  2008/11/04 04:39:35  jeroens
' Moved
'
' Revision 1.2  2008/10/10 18:04:02  jeroens
' Updated to renamed layers classes
'
' Revision 1.1  2008/09/26 07:31:59  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.27  2008/08/14 13:02:38  jeroens
' Double-click opens edit layer
' Uses different modes
'
' Revision 1.26  2008/08/11 04:35:28  jeroens
' Added safety check
'
' Revision 1.25  2008/08/10 01:44:04  jeroens
' Uses shared singleton tooltip
'
' Revision 1.24  2008/08/09 02:36:47  jeroens
' Displays tooltip for Remarks
'
' Revision 1.23  2008/08/08 23:16:43  jeroens
' Disabled console traces
'
' Revision 1.22  2008/07/31 16:36:30  jeroens
' Fixed closed eye
'
' Revision 1.21  2008/07/30 21:16:48  jeroens
' Improved performance on rendering images
' Improved performance on generic rendering
' Improved disposal logic
' Bundled click notifications
'
' Revision 1.20  2008/06/02 00:01:31  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.19  2008/05/29 22:22:54  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.18  2008/03/28 23:48:28  jeroens
' Click() will first select layer, then process hit area to make sure selected state properly propogates to using code
' Click() will send out combined Selected + Map update (which is a hack)
'
' Revision 1.17  2008/03/28 23:26:42  jeroens
' Renamed layer change flags const
'
' Revision 1.16  2008/03/26 02:54:32  jeroens
' Removed tooltip
'
' Revision 1.15  2008/03/25 14:47:05  jeroens
' Added ToolTip support
'
' Revision 1.14  2008/03/25 00:48:22  jeroens
' Made rendering more smooth: button borders only drawn when hovering
' Editable flag can be toggled
' Editable button only shown when hovering or selected
'
' Revision 1.13  2008/03/24 02:23:51  jeroens
' Fixed potential windows handles problem by simulating child controls rather than embedding real controls
'
' Revision 1.12  2008/02/02 02:51:07  jeroens
' CLS compliant
'
' Revision 1.11  2007/12/11 15:06:01  jeroens
' * Simplified
'
' Revision 1.10  2007/12/09 15:36:51  jeroens
' * Improved layer update handling to enhance performance
' * Made look much better
'
' Revision 1.9  2007/12/03 21:12:46  jeroens
' * Moved name property monitoring into Layer class
'
' Revision 1.8  2007/11/30 21:30:21  jeroens
' * Made group-aware
' * Slightly changed layout
'
' Revision 1.7  2007/11/28 16:43:53  jeroens
' * Label now clickable
'
' Revision 1.6  2007/09/26 03:35:24  jeroens
' * Layer representations will render the preview area
' + Uses layer updateType flags
'
' Revision 1.5  2007/09/24 21:22:57  jeroens
' * Uses cVisualStyle
'
' Revision 1.4  2007/09/18 14:23:40  jeroens
' * OnLayerChanged will refresh property manually. This will have to change; the core will have to take care of this in the near future
'
' Revision 1.3  2007/09/14 13:46:15  jeroens
' + Added remark indicator
'
' Revision 1.2  2007/09/13 02:58:52  jeroens
' + Got drawing and selective updating to work, on to erasing
'
' Revision 1.1  2007/09/12 01:58:19  jeroens
' * Trying to prevent bad CVS stuff from happening....
'
' Revision 1.1  2007/09/12 00:59:18  jeroens
' Moved
'
' Revision 1.18  2007/09/11 22:00:25  jeroens
' * Renamed class to ucLayer
' + Activated basis for layer highlighting
' + Set proper cursors
'
' Revision 1.17  2007/09/11 19:04:31  jeroens
' * Revamping
'
' Revision 1.16  2007/07/13 22:16:49  jeroens
' * Did you know that WithEvents vars will keep object instances referenced?
'
' Revision 1.15  2007/06/05 00:45:57  sherman
' Finishing up tweaks... need more work with layer ordering
'
' Revision 1.14  2007/05/16 22:04:47  sherman
' Adding more features...
'
' Revision 1.13  2007/05/15 21:34:53  sherman
' Changed Basemap Functionality to Events
'
' Revision 1.12  2007/05/11 00:36:21  sherman
' Major Functionality updates for SAUPBasemap.
'
' Revision 1.11  2007/04/18 16:19:49  sherman
' Corrected for full size drawing - version before mouse inputs.
'
' Revision 1.10  2007/04/14 23:07:32  sherman
' - Included Pattern/Brushes capabilities to basemap
' - seperated MPA to individual raster
'
' Revision 1.9  2007/03/07 22:07:25  sherman
' Modified layers dialogue to work properly
'
' Revision 1.8  2007/03/06 08:10:54  sherman
' Linked BasemapController functionality to GUI
'
' Revision 1.7  2007/03/03 00:02:58  sherman
' + Added more functionality and bug fixes
'
' Revision 1.6  2007/03/01 22:38:32  sherman
' Changed some layout issues
'
' Revision 1.5  2007/03/01 01:53:48  sherman
' Modified manner you Edit Layers
'
' Revision 1.4  2007/02/27 02:58:07  jeroens
' * Fixed CLS complicancy compiler warnings
'
'==============================================================================

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

        Public Sub New(ByVal l As cLayer)

            ' This call is required by the Windows Form Designer.
            Me.InitializeComponent()

            'Enable double buffering
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.UserPaint, True)

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
            Dim dlgEditLay As New dlgEditLayer(Me.m_layer, Nothing, openType)
            dlgEditLay.ShowDialog()
            ' Dialog will update the layer
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

        Private Sub ucLayer_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            ' Remove from event handler
            RemoveHandler m_layer.LayerChanged, AddressOf OnLayerChanged

            Me.m_layer = Nothing
            Me.m_lgParent = Nothing
        End Sub

        Private Sub pbLayer_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

            Dim rcControl As Rectangle = New Rectangle(0, 0, Me.Width, Me.Height)
            Dim rcEditable As Rectangle = Nothing
            Dim rcVisible As Rectangle = Nothing
            Dim rcLabel As Rectangle = Nothing
            Dim rcPreview As Rectangle = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
            Dim prop As cProperty = pm.GetProperty(Me.Layer.Source, eVarNameFlags.Name)
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
            Me.m_layer.Renderer.RenderPreview(e.Graphics, rcPreview)
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