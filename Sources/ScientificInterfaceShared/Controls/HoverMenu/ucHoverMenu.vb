#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Hover menu for EwE controls.
    ''' </summary>
    ''' <remarks>
    ''' Rigt now, the controls shows zoom in, zoom out capabilities.
    ''' </remarks>
    ''' =======================================================================
    Public Class ucHoverMenu
        Implements IUIElement

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_ctrlTarget As Control = Nothing
        Private m_ctrlParent As Control = Nothing
        Private m_filter As cMouseHoverFilter = Nothing

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)
            MyBase.New()
            Me.m_uic = uic
            Me.InitializeComponent()
        End Sub

        Private Property UIContext() As cUIContext Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                Me.Detach()
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

#End Region ' Constructor

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event for notifying the world that the user executed a command.
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' -------------------------------------------------------------------
        Public Event OnUserCommand(ByVal cmd As eCommandTypes)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type stating possible menu hover commands
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eCommandTypes As Byte
            ''' <summary>User wants to zoom in.</summary>
            ZoomIn = &H1
            ''' <summary>User wants to zoom out.</summary>
            ZoomOut = &H2
            ''' <summary>User wants to reset zoom.</summary>
            ZoomReset = &H4
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Attach the hover menu to a <see cref="Control">Windows control</see>.
        ''' </summary>
        ''' <param name="target">The windows control to attach the hover menu
        ''' to.</param>
        ''' <param name="style">A bitwise OR combination of <see cref="eCommandTypes">commands</see>
        ''' that the hover menu should support.</param>
        ''' -------------------------------------------------------------------
        Public Sub Attach(ByVal target As Control, Optional ByVal style As eCommandTypes = CType(&HFF, eCommandTypes))

            Me.Detach()

            Me.m_ctrlTarget = target
            Me.m_ctrlParent = DirectCast(IIf(target.Parent Is Nothing, target, target.Parent), Control)

            Me.m_ctrlParent.Controls.Add(Me)
            Me.BringToFront()
            Me.ShowHover(False)

            ' Show/hide buttons
            Me.m_tsbnZoomIn.Visible = ((style And eCommandTypes.ZoomIn) > 0)
            Me.m_tsbnZoomOut.Visible = ((style And eCommandTypes.ZoomOut) > 0)
            Me.m_tsbnZoomReset.Visible = ((style And eCommandTypes.ZoomReset) > 0)

            ' Fit entire control to the preferred size of the toolstrip.
            Me.Size = Me.m_ts.PreferredSize

            ' Set up mouse movement message filter
            Me.m_filter = New cMouseHoverFilter(Me)
            Application.AddMessageFilter(Me.m_filter)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Detach the hover menu from a previously <see cref="Attach">attached</see>
        ''' <see cref="Control">Windows control</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Detach()

            If (Not Me.IsAttached()) Then Return

            Application.RemoveMessageFilter(Me.m_filter)
            Me.m_filter = Nothing

            Me.m_ctrlParent.Controls.Remove(Me)
            Me.m_ctrlTarget = Nothing

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether the hover menu is currently attached to a 
        ''' <see cref="Control">Windows control</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsAttached() As Boolean
            Get
                Return (Me.m_ctrlTarget IsNot Nothing)
            End Get
        End Property

#End Region ' Public interfaces

#Region " Event handling "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 'Zoom in' button press handler.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnZoomIn(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbnZoomIn.Click
            Try
                Me.InvokeCallback(eCommandTypes.ZoomIn)
            Catch ex As Exception
                ' Woops
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 'Zoom out' button press handler.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnZoomOut(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbnZoomOut.Click
            Try
                Me.InvokeCallback(eCommandTypes.ZoomOut)
            Catch ex As Exception
                ' Woops
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 'Zoom reset' button press handler.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnZoomReset(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbnZoomReset.Click
            Try
                Me.InvokeCallback(eCommandTypes.ZoomReset)
            Catch ex As Exception
                ' Woops
            End Try
        End Sub

#End Region ' Event handling

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Evaluate the hover menu state anew.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Sub UpdateHoverMenuState()
            Me.ShowHover(Me.IsMouseOverMyself() Or Me.IsMouseOverTarget())
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Show or hide the hover menu.
        ''' </summary>
        ''' <param name="bShow">Flag stating whether the hover menu should be 
        ''' shown (True) or hidden (False).</param>
        ''' -------------------------------------------------------------------
        Private Sub ShowHover(ByVal bShow As Boolean)

            ' Optimization
            If bShow = Me.Visible Then Return

            Dim ptHover As Point = Me.m_ctrlTarget.ClientRectangle.Location

            If (Me.m_uic IsNot Nothing) Then

                ' Express my target control (0,0) location in the coordinate system of my parent
                If Not Object.ReferenceEquals(Me.m_ctrlTarget, Me.m_ctrlParent) Then
                    ptHover = Me.m_ctrlParent.PointToClient(Me.m_ctrlTarget.PointToScreen(ptHover))
                End If

                ' Calc horizontal hover menu pos
                If Me.m_uic.StyleGuide.IsRightToLeft Then
                    ptHover.X += Me.m_ctrlTarget.ClientRectangle.Width - Me.Width - Me.Margin.Right - Me.m_ctrlTarget.Padding.Right
                Else
                    ptHover.X += Me.Margin.Left + Me.m_ctrlTarget.Padding.Left
                End If
                ' Calc vertical hover menu pos
                ptHover.Y += (Me.m_ctrlTarget.ClientRectangle.Height - Me.Height - Me.Margin.Bottom - Me.m_ctrlTarget.Padding.Bottom)

            Else

                bShow = False

            End If

            ' Update visuals
            Me.Location = ptHover
            Me.Visible = (bShow Or IsMouseOverMyself())

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, returns whether the mouse is over the hover menu.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function IsMouseOverMyself() As Boolean
            Return Me.ClientRectangle.Contains(MousePosition)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, returns whether the mouse is over the target control.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function IsMouseOverTarget() As Boolean
            Dim pt As Point = Me.m_ctrlTarget.PointToClient(MousePosition)
            Return Me.m_ctrlTarget.ClientRectangle.Contains(pt)
        End Function

        Private Sub InvokeCallback(ByVal cmd As eCommandTypes)
            RaiseEvent OnUserCommand(cmd)
        End Sub

#End Region ' Internals

#Region " Mouse message filter "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class to trap mouse movement messages for telling
        ''' an attached hover menu to evaluate its hover state.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class cMouseHoverFilter
            Implements IMessageFilter

            Private m_hovermenu As ucHoverMenu = Nothing

            Public Sub New(ByVal hovermenu As ucHoverMenu)
                Me.m_hovermenu = hovermenu
            End Sub

            Public Function PreFilterMessage(ByRef m As System.Windows.Forms.Message) As Boolean _
                Implements System.Windows.Forms.IMessageFilter.PreFilterMessage

                If m.Msg = &H200 Then
                    Me.m_hovermenu.UpdateHoverMenuState()
                End If

            End Function

        End Class

#End Region ' Mouse message filter

    End Class

End Namespace
