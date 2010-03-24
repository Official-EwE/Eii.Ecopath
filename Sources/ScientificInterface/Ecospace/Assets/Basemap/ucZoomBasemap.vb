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
    ''' 
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class ucZoomBaseMap
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

        Private m_uic As cUIContext = Nothing
        ''' <summary>Current <see cref="ePositionModeTypes">mode</see> to position the map.</summary>
        Private m_positionMode As ePositionModeTypes = ePositionModeTypes.Center
        ''' <summary>Predefined zoom levels.</summary>
        Private m_aiZoomLevels As Integer() = {50, 66, 75, 100, 125, 150, 200, 250, 300, 400, 500}
        ''' <summary>Index of current <see cref="m_aiZoomLevels">zoom level</see>.</summary>
        Private m_iZoomLevelIndex As Integer = 3

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Public access "

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Public ReadOnly Property Map() As ucBaseMap
            Get
                Return Me.m_map
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ePositionModeTypes">Position mode</see> 
        ''' for displaying the map.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property PositionMode() As ePositionModeTypes

            Get
                Return Me.m_positionMode
            End Get

            Set(ByVal value As ePositionModeTypes)
                Me.m_positionMode = value
                Me.SetPositionMode()
            End Set

        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Set the map <see cref="eZoomTypes">Zoom level</see> for displaying the map.
        ''' </summary>
        ''' <param name="zoomType">The <see cref="eZoomTypes">Zoom level</see> to use.</param>
        ''' -----------------------------------------------------------------------
        Public Sub Zoom(ByVal zoomType As eZoomTypes)
            Select Case zoomType
                Case eZoomTypes.ZoomIn
                    ' Increase zoom rate to next increment
                    Me.m_iZoomLevelIndex = Math.Min(Me.m_aiZoomLevels.Length - 1, Me.m_iZoomLevelIndex + 1)
                Case eZoomTypes.ZoomOut
                    ' Decrease zoom rate to prev increment
                    Me.m_iZoomLevelIndex = Math.Max(0, Me.m_iZoomLevelIndex - 1)
                Case eZoomTypes.ZoomReset
                    ' Zoom to 100%
                    Me.m_iZoomLevelIndex = 3
            End Select
            ' Apply
            If Me.m_tscbZoomPercent.Items.Count > Me.m_iZoomLevelIndex Then Me.m_tscbZoomPercent.SelectedIndex = Me.m_iZoomLevelIndex
        End Sub

#End Region ' Public access

#Region " Events "

#Region " Form events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Populate zoom combo
            Me.m_tscbZoomPercent.Items.Clear()
            For iZoomPercent As Integer = 0 To Me.m_aiZoomLevels.Length - 1
                Me.m_tscbZoomPercent.Items.Add(String.Format("{0}%", Me.m_aiZoomLevels(iZoomPercent)))
            Next
            Me.m_tscbZoomPercent.SelectedIndex = Me.m_iZoomLevelIndex
            ' Kick off
            Me.SetPositionMode()

        End Sub

        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)

            Me.SetZoomLevel()
        End Sub

        Private Sub m_tsbSaveImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbSaveImage.Click

            If (Me.UIContext Is Nothing) Then Return

            Dim format As ImageFormat = ImageFormat.Bmp
            Dim core As cCore = Me.UIContext.Core
            Dim model As cEwEModel = core.EwEModel
            Dim scenario As cEcospaceScenario = core.EcospaceScenarios(core.ActiveEcospaceScenarioIndex)
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(FileUtilities.ToValidFileName(String.Format("{0}_{1}", model.Name, scenario.Name), False), "", My.Resources.FILEFILTER_IMAGE)
            If cmdFS.Result = Windows.Forms.DialogResult.OK Then

                Select Case cmdFS.FilterIndex
                    Case 0, 1
                        format = ImageFormat.Bmp
                    Case 2
                        format = ImageFormat.Jpeg
                    Case 3
                        format = ImageFormat.Gif
                    Case 4
                        format = ImageFormat.Png
                    Case 5
                        format = ImageFormat.Tiff
                    Case Else
                        Debug.Assert(False)
                End Select
                Me.m_map.SaveToBitmap(cmdFS.FileName, format)
            End If

        End Sub

#End Region ' Form events

#Region " Zoom controls "

        Private Sub m_tsbZoomIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbZoomIn.Click, m_tsmiZoomIn.Click
            Me.Zoom(eZoomTypes.ZoomIn)
        End Sub

        Private Sub m_tsbZoomOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbZoomOut.Click, m_tsmiZoomOut.Click
            Me.Zoom(eZoomTypes.ZoomOut)
        End Sub

        Private Sub m_tsbReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbZoomReset.Click, m_tsmiZoomReset.Click
            Me.Zoom(eZoomTypes.ZoomReset)
        End Sub

        Private Sub m_tscbZoom_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tscbZoomPercent.SelectedIndexChanged
            Me.m_iZoomLevelIndex = Me.m_tscbZoomPercent.SelectedIndex
            Me.SetZoomLevel()
        End Sub

        'Private Sub ucZoomControl_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel
        '    If (e.Delta > 0) Then Me.Zoom(eZoomTypes.ZoomIn) Else Me.Zoom(eZoomTypes.ZoomOut)
        'End Sub

#End Region ' Zoom controls

#Region " Postion mode "

        Private Sub OnViewStretch(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiViewStretch1.Click, m_tsmiViewStretch2.Click
            Me.PositionMode = ePositionModeTypes.Stretch
        End Sub

        Private Sub OnViewCenter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiViewCenter1.Click, m_tsmiViewCenter2.Click
            Me.PositionMode = ePositionModeTypes.Center
        End Sub

#End Region ' Postion mode

#Region " Scroll bars "

        Private Sub m_sbHorz_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles m_sbHorz.Scroll
            Me.SetPosition()
        End Sub

        Private Sub m_sbVert_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles m_sbVert.Scroll
            Me.SetPosition()
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

                Case ePositionModeTypes.Center
                    Me.m_map.Dock = DockStyle.None
                    Me.Zoom(eZoomTypes.ZoomReset)

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
            ' Get current zoom level
            Dim sZoom As Single = Me.m_aiZoomLevels(Me.m_iZoomLevelIndex) * 0.01!
            ' Calc map size corrected for zoom rate
            Dim szMap As Size = New Size(CInt(szfSizeMap.Width * sZoom), CInt(szfSizeMap.Height * sZoom))
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
            Me.SetPosition()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update the map position based on position mode, current zoom level and 
        ''' scroll bar locations.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub SetPosition()

            If Object.ReferenceEquals(Me.m_map, Nothing) Then Return

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

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update enabled- and checked states of child controls.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateControls()

            Me.m_tsbZoomIn.Enabled = (Me.PositionMode <> ePositionModeTypes.Stretch)
            Me.m_tsmiZoomIn.Enabled = (Me.PositionMode <> ePositionModeTypes.Stretch)
            Me.m_tsbZoomOut.Enabled = (Me.PositionMode <> ePositionModeTypes.Stretch)
            Me.m_tsmiZoomOut.Enabled = (Me.PositionMode <> ePositionModeTypes.Stretch)
            Me.m_tsbZoomReset.Enabled = (Me.PositionMode <> ePositionModeTypes.Stretch)
            Me.m_tsmiZoomReset.Enabled = (Me.PositionMode <> ePositionModeTypes.Stretch)
            Me.m_tscbZoomPercent.Enabled = (Me.PositionMode <> ePositionModeTypes.Stretch)

            Me.m_tsmiViewStretch1.Checked = (Me.PositionMode = ePositionModeTypes.Stretch)
            Me.m_tsmiViewStretch2.Checked = (Me.PositionMode = ePositionModeTypes.Stretch)
            Me.m_tsmiViewCenter1.Checked = (Me.PositionMode = ePositionModeTypes.Center)
            Me.m_tsmiViewCenter2.Checked = (Me.PositionMode = ePositionModeTypes.Center)

            Me.m_sbHorz.Enabled = (Me.PositionMode <> ePositionModeTypes.Stretch)
            Me.m_sbVert.Enabled = (Me.PositionMode <> ePositionModeTypes.Stretch)

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
