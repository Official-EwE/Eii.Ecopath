#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing.Imaging
Imports EwEUtils.Utilities
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' User control for implementing a <see cref="ucMap">EwE map</see> that
    ''' can be zoomed onto.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class ucMapZoom
        Implements IUIElement

#Region " Public enums "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type defining position modes for displaying the map.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Enum ePositionModeTypes As Byte
            ''' <summary>Stretch the map to fill the zoom area, not preserving map aspect ratio.</summary>
            Stretch
            ''' <summary>Centers the map in the zoom area, preserving map aspect ratio.</summary>
            Center
        End Enum

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated types defining zoom modes for displaying the map.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Enum eZoomTypes As Byte
            ''' <summary>Increase zoom level.</summary>
            ZoomIn
            ''' <summary>Decrease zoom level.</summary>
            ZoomOut
            ''' <summary>Resets zoom level to exactly fit the zoom area.</summary>
            ZoomReset
        End Enum

#End Region ' Public enums

#Region " Private vars "

        ''' <summary>UI context to connect to.</summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary>Current <see cref="ePositionModeTypes">mode</see> to position the map.</summary>
        Private m_positionMode As ePositionModeTypes = ePositionModeTypes.Center

        Private m_sZoom As Single = 1.0

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Public ReadOnly Property Map() As ucMap
            Get
                Return Me.m_map
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ePositionModeTypes">Position mode</see> 
        ''' for displaying the map.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property PositionMode() As ePositionModeTypes

            Get
                Return Me.m_positionMode
            End Get

            Set(ByVal value As ePositionModeTypes)
                Me.m_positionMode = value
                Me.SetPositionMode()
            End Set

        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the zoom percentage for displaying the map.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ZoomPercentage() As Single
            Get
                Return Me.m_sZoom * 100.0!
            End Get
            Set(ByVal value As Single)
                Me.m_sZoom = value * 0.01!
                Me.SetZoomLevel()
            End Set
        End Property

#End Region ' Public access

#Region " Events "

#Region " Form events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return
            ' Kick off
            Me.SetPositionMode()

        End Sub

        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)
            Me.SetZoomLevel()
        End Sub

#End Region ' Form events

#Region " Postion mode "

        Private Sub OnViewStretch(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiViewStretch2.Click
            Me.PositionMode = ePositionModeTypes.Stretch
        End Sub

        Private Sub OnViewCenter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiViewCenter2.Click
            Me.PositionMode = ePositionModeTypes.Center
        End Sub

#End Region ' Postion mode

#Region " Scroll bars "

        Private Sub OnHScroll(ByVal sender As Object, ByVal e As ScrollEventArgs) _
            Handles m_sbHorz.Scroll
            Me.UpdatePosition()
        End Sub

        Private Sub OnVScroll(ByVal sender As Object, ByVal e As ScrollEventArgs) _
            Handles m_sbVert.Scroll
            Me.UpdatePosition()
        End Sub

#End Region ' Scroll bars

#End Region ' Events

#Region " Internal implementation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update map size and location based on current position mode.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub SetPositionMode()

            If Object.ReferenceEquals(Me.m_map, Nothing) Then Return

            Select Case Me.PositionMode
                Case ePositionModeTypes.Stretch
                    Me.m_map.Dock = DockStyle.Fill
                    ' ToDo: hide scrollbars, repos content

                Case ePositionModeTypes.Center
                    Me.m_map.Dock = DockStyle.None
                    ' ToDo: show scrollbars, repos content

            End Select

            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update the map size based on current zoom level and position mode.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub SetZoomLevel()

            If Object.ReferenceEquals(Me.m_map, Nothing) Then Return

            ' Get map size at 100% in current view mode
            Dim szfSizeMap As SizeF = Me.GetFittedMapSize()
            ' Calc map size corrected for zoom rate
            Dim szMap As Size = New Size(CInt(szfSizeMap.Width * Me.m_sZoom), CInt(szfSizeMap.Height * Me.m_sZoom))
            ' Get zoom area size
            Dim szZoom As Size = Me.GetZoomSize()

            ' Update scroll info
            Me.m_sbHorz.Maximum = Math.Max(0, szMap.Width)
            Me.m_sbHorz.LargeChange = szZoom.Width
            Me.m_sbHorz.SmallChange = CInt(Me.m_sbHorz.Maximum / 4)
            Me.m_sbHorz.Value = 0

            Me.m_sbVert.Maximum = Math.Max(0, szMap.Height)
            Me.m_sbVert.LargeChange = szZoom.Height
            Me.m_sbVert.SmallChange = CInt(Me.m_sbVert.Maximum / 10)
            Me.m_sbVert.Value = 0

            ' Resize map
            Me.m_map.Size = szMap

            ' Place map
            Me.UpdatePosition()

        End Sub

        Public Event OnPositionChanged(ByVal sender As ucMapZoom)
        Private m_bInUpdate As Boolean = False

        Public Sub UpdatePosition(ByVal src As ucMapZoom)
            Me.m_bInUpdate = True
            Me.m_map.Dock = src.m_map.Dock
            Me.m_sbHorz.Value = src.m_sbHorz.Value
            Me.m_sbVert.Value = src.m_sbVert.Value
            Me.m_sZoom = src.m_sZoom
            Me.m_bInUpdate = False
            Me.UpdatePosition()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update the map position based on position mode, current zoom level and 
        ''' scroll bar locations.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub UpdatePosition()

            If Object.ReferenceEquals(Me.m_map, Nothing) Then Return
            If Me.m_bInUpdate Then Return

            ' Get zoom area
            Dim szZoom As Size = Me.GetZoomSize()
            ' Get centered map pos 
            Dim ptCentered As Point = New Point(CInt((szZoom.Width - Me.m_map.Width) / 2), CInt((szZoom.Height - Me.m_map.Height) / 2))
            ' Get scroll map pos
            Dim ptScroll As Point = New Point(-Me.m_sbHorz.Value, -Me.m_sbVert.Value)
            Dim ptMap As New Point

            ptMap.X = CInt(IIf(ptCentered.X > 0, ptCentered.X, ptScroll.X))
            ptMap.Y = CInt(IIf(ptCentered.Y > 0, ptCentered.Y, ptScroll.Y))

            Me.m_map.Location = ptMap

            RaiseEvent OnPositionChanged(Me)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update enabled- and checked states of child controls.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateControls()

            If Me.PositionMode = ePositionModeTypes.Stretch Then
                Me.m_sbHorz.Visible = False
                Me.m_sbVert.Visible = False
                Me.m_plZoom.Size = Me.Size
            Else
                Me.m_sbHorz.Visible = True
                Me.m_sbVert.Visible = True
                Me.m_plZoom.Size = New Size(Me.Size.Width - Me.m_sbVert.Width, _
                                            Me.Size.Height - Me.m_sbHorz.Height)
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the size of the map fitted to the current zoom area with the 
        ''' current view mode. 
        ''' </summary>
        ''' <returns>The size of the map fitted to the current zoom area with the 
        ''' current view mode.</returns>
        ''' -----------------------------------------------------------------------
        Private Function GetFittedMapSize() As SizeF

            ' Find aspect ratio depending on fit
            Dim szZoom As Size = Me.GetZoomSize()

            Select Case Me.m_positionMode

                Case ePositionModeTypes.Center
                    Dim sRatio As Single = Math.Min(CSng(szZoom.Width / Me.m_map.NumCols), CSng(szZoom.Height / Me.m_map.NumRows))
                    Return New SizeF(sRatio * Me.m_map.NumCols, sRatio * Me.m_map.NumRows)

                Case ePositionModeTypes.Stretch
                    Return szZoom

            End Select

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the size of the zoom area.
        ''' </summary>
        ''' <returns>The size of the zoom area</returns>
        ''' -----------------------------------------------------------------------
        Private Function GetZoomSize() As Size
            Return Me.m_plZoom.ClientRectangle.Size()
        End Function

#End Region ' Internal implementation

    End Class

End Namespace
