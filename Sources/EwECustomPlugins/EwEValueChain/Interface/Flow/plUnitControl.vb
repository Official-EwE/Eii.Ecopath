#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Reflection
Imports EwEUtils.Database.cEwEDatabase
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Panel to interact with a single unit in a flow.
''' </summary>
''' ===========================================================================
Public Class plUnitControl
    Inherits Panel

#Region " Private vars "

    Private components As System.ComponentModel.IContainer = Nothing
    Private m_uic As cUIContext = Nothing
    Private m_fp As cFlowPosition = Nothing
    Private m_bInUpdate As Boolean = False
    Private m_sScale As Single = 1.0

#End Region ' Private vars

#Region " Constructors "

    Public Sub New(ByVal uic As cUIContext, ByVal fp As cFlowPosition)

        Debug.Assert(fp IsNot Nothing)

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.AllowDrop = True
        Me.Cursor = Cursors.Hand

        Me.m_fp = fp
        Me.Name = Me.m_fp.Unit.Name
        Me.m_uic = uic

        ' Auto-repos
        Me.OnPositionChanged(Me.m_fp)

        AddHandler Me.m_fp.OnChanged, AddressOf OnPositionChanged
        AddHandler Me.m_fp.Unit.OnChanged, AddressOf OnDataChanged
        AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleguideChanged

    End Sub

#End Region ' Constructors

#Region " Events "

    Private Sub OnDataChanged(ByVal obj As cOOPStorable)
        Me.Invalidate()
    End Sub

    Private Sub OnPositionChanged(ByVal obj As cOOPStorable)

        If Me.m_bInUpdate Then Return
        Me.m_bInUpdate = True

        Debug.Assert(TypeOf obj Is cFlowPosition)
        Me.Location = New Point(CInt(Me.m_fp.Xpos * Me.m_sScale), CInt(Me.m_fp.Ypos * Me.m_sScale))
        Me.Size = New Size(CInt(Me.m_fp.Width * Me.m_sScale), CInt(Me.m_fp.Height * Me.m_sScale))

        Me.m_bInUpdate = False

    End Sub

    Private Sub UnitControl_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Me.Disposed

        If Me.m_fp IsNot Nothing Then

            RemoveHandler Me.m_fp.OnChanged, AddressOf OnPositionChanged
            RemoveHandler Me.m_fp.Unit.OnChanged, AddressOf OnDataChanged
            RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleguideChanged

            Me.m_fp = Nothing
            Me.m_uic = Nothing

        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler to process a mouse click.
    ''' </summary>
    ''' <remarks>
    ''' Handling is outsourced to the master panel which will process the click 
    ''' based on the current interaction mode.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub UnitControl_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
        Handles Me.MouseDown
        Debug.Assert(Me.FlowPanel IsNot Nothing)
        Me.FlowPanel.OnUnitMouseDown(Me)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler to start a mouse hover event.
    ''' </summary>
    ''' <remarks>
    ''' Handling is outsourced to the master panel which will process the hover
    ''' based on the current interaction mode.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub plUnitControl_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) _
         Handles Me.MouseEnter
        If (Me.FlowPanel IsNot Nothing) Then
            Me.FlowPanel.OnUnitMouseHover(Me, True)
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler to start a mouse hover event.
    ''' </summary>
    ''' <remarks>
    ''' Handling is outsourced to the master panel which will process the hover
    ''' based on the current interaction mode.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub plUnitControl_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Me.MouseLeave
        ' Could be due to deletion
        If (Me.FlowPanel IsNot Nothing) Then
            Me.FlowPanel.OnUnitMouseHover(Me, False)
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler to update the underlying flow position instance whenever 
    ''' this control has been repositioned.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnLocationChanged(ByVal e As System.EventArgs)
        MyBase.OnLocationChanged(e)
        Me.m_fp.Xpos = CInt(Me.Location.X / Me.m_sScale)
        Me.m_fp.Ypos = CInt(Me.Location.Y / Me.m_sScale)
    End Sub

    Protected Overrides Sub OnSizeChanged(ByVal e As System.EventArgs)
        MyBase.OnSizeChanged(e)
        Me.m_fp.Width = CInt(Me.Width / Me.m_sScale)
        Me.m_fp.Height = CInt(Me.Height / Me.m_sScale)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim rc As Rectangle = Me.ClientRectangle
        Dim clrBackground As Color = Color.Black
        Dim clrBorder As Color = Color.Black
        Dim clrText As Color = Color.Black
        Dim img As Image = Nothing

        ' Adjust rect
        rc.Width -= 1
        rc.Height -= 1

        ' Get style colors
        Me.m_uic.StyleGuide.GetStyleColors(Me.Unit.Style, clrText, clrBackground)

        If Not Me.Unit.CanCompute Then
            clrBackground = Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND)
        End If

        'Determine border color
        If Me.Selected Then
            clrBorder = Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
        Else
            clrBorder = Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
        End If

        ' Paint background
        Using br As New SolidBrush(clrBackground)
            e.Graphics.FillRectangle(br, rc)
        End Using

        ' Paint border
        Using p As New Pen(clrBorder)
            e.Graphics.DrawRectangle(p, rc)
        End Using

        ' Paint unit type image
        Select Case Me.Unit.UnitType
            Case cUnitFactory.eUnitType.Producer
                img = My.Resources.producer
            Case cUnitFactory.eUnitType.Processing
                img = My.Resources.processing
            Case cUnitFactory.eUnitType.Distribution
                img = My.Resources.distribution
            Case cUnitFactory.eUnitType.Market
                img = My.Resources.market
            Case cUnitFactory.eUnitType.Consumer
                img = My.Resources.consumer
        End Select

        If (img IsNot Nothing) Then
            Dim rcImage As Rectangle = New Rectangle(0, 0, CInt(16 * Me.ZoomFactor), CInt(16 * Me.ZoomFactor))
            If Me.m_uic.StyleGuide.IsRightToLeft Then
                rcImage.Offset(2, Me.Height - rcImage.Height - 2)
            Else
                rcImage.Offset(Me.Width - rcImage.Width - 2, Me.Height - rcImage.Height - 2)
            End If
            e.Graphics.DrawImage(img, rcImage, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel)
        End If

        ' Paint text
        Using br As New SolidBrush(clrText)
            Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
                e.Graphics.DrawString(Me.Unit.Name, ft, br, rc)
            End Using
        End Using

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Copy/paste handling.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub tsmCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Clipboard.SetDataObject(Me.Unit)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Copy/paste handling.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub tsmPaste_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim data As IDataObject = Clipboard.GetDataObject()
        If data.GetDataPresent(GetType(cOOPStorable)) Then
            Me.Unit.CopyFrom(DirectCast(data.GetData(GetType(cOOPStorable)), cOOPStorable))
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' cStyleGuide changed handler, caught to redraw whenever sg colours have
    ''' been modified.
    ''' </summary>
    ''' <param name="changeFlags"></param>
    ''' -----------------------------------------------------------------------
    Private Sub OnStyleguideChanged(ByVal changeFlags As cStyleGuide.eChangeType)
        ' Redraw on color or font changes
        If ((changeFlags And (cStyleGuide.eChangeType.Colours Or cStyleGuide.eChangeType.Fonts)) > 0) Then
            Me.Invalidate(True)
        End If
    End Sub

#End Region ' Events

#Region " Public interfaces "

    Public Function Center() As Point
        Dim pt As Point = Me.Location
        Dim sz As Size = Me.Size
        Return New Point(CInt((pt.X + sz.Width / 2) / m_sScale), CInt((pt.Y + sz.Height / 2) / m_sScale))
    End Function

    Public Property ZoomFactor() As Single
        Get
            Return Me.m_sScale
        End Get
        Set(ByVal value As Single)
            Me.m_sScale = value
            Me.OnPositionChanged(Me.m_fp)
        End Set
    End Property

#End Region ' Public interfaces

#Region " Selection "

    Private m_bSelected As Boolean

    Public Property Selected() As Boolean
        Get
            Return Me.m_bSelected
        End Get
        Set(ByVal value As Boolean)
            Me.m_bSelected = value
            Me.Refresh()
        End Set
    End Property

#End Region ' Selection

#Region " Public properties "

    Public ReadOnly Property Unit() As cUnit
        Get
            Return Me.m_fp.Unit
        End Get
    End Property

    Public ReadOnly Property FlowPos() As cFlowPosition
        Get
            Return Me.m_fp
        End Get
    End Property

#End Region ' Public properties

#Region " Internals "

    Private Function FlowPanel() As plFlow
        Return DirectCast(Me.Parent, plFlow)
    End Function

#End Region ' Internals

#Region " VS "

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'UnitControl
        '
        Me.ResumeLayout(False)

    End Sub

#End Region ' VS

End Class
