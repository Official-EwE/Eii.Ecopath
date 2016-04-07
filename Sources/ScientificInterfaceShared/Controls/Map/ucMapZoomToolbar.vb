' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports System.Drawing.Imaging
Imports EwECore
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Controls.Map

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Toolbar that provides a user interface for synchronized zooming and 
    ''' stretching of one or more <see cref="ucMapZoom">zoom map controls</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucMapZoomToolbar
        Implements IUIElement

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        ''' <summary>Current <see cref="ucMapZoom.ePositionModeTypes">mode</see> to position the map.</summary>
        Private m_positionMode As ucMapZoom.ePositionModeTypes = ucMapZoom.ePositionModeTypes.Center
        ''' <summary>Predefined zoom levels.</summary>
        Private m_aiZoomLevels As Integer() = {50, 66, 75, 100, 125, 150, 200, 250, 300, 400, 500}
        ''' <summary>Index of current <see cref="m_aiZoomLevels">zoom level</see>.</summary>
        Private m_iZoomLevelIndex As Integer = 3
        ''' <summary>Flag to prevent looped updates.</summary>
        Private m_bInUpdate As Boolean = False
        ''' <summary>List of attached zoom maps that need to be synchronized.</summary>
        Private m_lZoomContainers As New List(Of ucMapZoom)
        ''' <summary>List of attached layers.</summary>
        Private m_lLayers As New List(Of cDisplayLayer)

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                ' Just to be sure
                For Each zc As ucMapZoom In Me.m_lZoomContainers.ToArray
                    Me.RemoveZoomContainer(zc)
                Next

                Debug.Assert(Me.m_lLayers.Count = 0)
                Debug.Assert(Me.m_lZoomContainers.Count = 0)

                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

#End Region ' Construction / destruction

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a zoom container to the toolbar.
        ''' </summary>
        ''' <param name="zoomContainer">A <see cref="ucMapZoom">zoom container</see> 
        ''' that this toolbar needs to manage.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddZoomContainer(ByVal zoomContainer As ucMapZoom)

            Debug.Assert(Not Me.m_lZoomContainers.Contains(zoomContainer))

            AddHandler zoomContainer.MouseWheel, AddressOf OnMapMousewheel
            AddHandler zoomContainer.OnPositionChanged, AddressOf OnMapPositionChanged

            Me.m_lZoomContainers.Add(zoomContainer)
            ' All all existing layers manually - 'cause we may have missed addition events
            For Each l As cDisplayLayer In zoomContainer.Map.Layers
                Me.m_lLayers.Add(l)
            Next

            AddHandler zoomContainer.Map.LayerAdded, AddressOf OnMapLayerAdded
            AddHandler zoomContainer.Map.LayerRemoved, AddressOf OnMapLayerRemoved

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Remove a zoom container from the toolbar.
        ''' </summary>
        ''' <param name="zoomContainer">A <see cref="ucMapZoom">zoom container</see> 
        ''' that this toolbar no longer needs to manage.</param>
        ''' -----------------------------------------------------------------------
        Public Sub RemoveZoomContainer(ByVal zoomContainer As ucMapZoom)

            Debug.Assert(Me.m_lZoomContainers.Contains(zoomContainer))

            RemoveHandler zoomContainer.Map.LayerAdded, AddressOf OnMapLayerAdded
            RemoveHandler zoomContainer.Map.LayerRemoved, AddressOf OnMapLayerRemoved

            RemoveHandler zoomContainer.MouseWheel, AddressOf OnMapMousewheel
            RemoveHandler zoomContainer.OnPositionChanged, AddressOf OnMapPositionChanged
            Me.m_lZoomContainers.Remove(zoomContainer)

            ' Remove all layers manually - 'cause we're going to miss removal events
            For Each l As cDisplayLayer In zoomContainer.Map.Layers
                Me.m_lLayers.Remove(l)
            Next

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ucMapZoom.ePositionModeTypes">Position mode</see> 
        ''' for all attached <see cref="ucMapZoom">maps</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property PositionMode() As ucMapZoom.ePositionModeTypes
            Get
                Return Me.m_positionMode
            End Get
            Set(ByVal value As ucMapZoom.ePositionModeTypes)
                Me.m_positionMode = value
                For Each map As ucMapZoom In Me.m_lZoomContainers
                    map.PositionMode = value
                Next
                Me.Zoom(ucMapZoom.eZoomTypes.ZoomReset)
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get all <see cref="ucMapZoom">zoom containers</see> that are
        ''' <see cref="AddZoomContainer">added</see> to this control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property ZoomContainers As ucMapZoom()
            Get
                Return Me.m_lZoomContainers.ToArray
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get all layers that this control has pilfered from its registered
        ''' <see cref="ZoomContainers">containers</see>. The control tracks the 
        ''' layers to provide layer data export and import interface elements.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Layers As cDisplayLayer()
            Get
                Return Me.m_lLayers.ToArray
            End Get
        End Property

#End Region ' Public access

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Populate zoom combo
            Me.m_tscbZoomPercent.Items.Clear()
            For iZoomPercent As Integer = 0 To Me.m_aiZoomLevels.Length - 1
                Me.m_tscbZoomPercent.Items.Add(cStringUtils.Localize(SharedResources.GENERIC_VALUE_PERCENTAGE, Me.m_aiZoomLevels(iZoomPercent)))
            Next
            Me.m_tscbZoomPercent.SelectedIndex = Me.m_iZoomLevelIndex

            ' Kick off
            Me.PositionMode = Me.PositionMode

        End Sub

#End Region ' Overrides

#Region " Child control events "

#Region " Zoom controls "

        Private Sub OnZoomIn(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbZoomIn.Click, m_tsmiZoomIn.Click
            Me.Zoom(ucMapZoom.eZoomTypes.ZoomIn)
            Me.UpdateControls()
        End Sub

        Private Sub OnZoomOut(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbZoomOut.Click, m_tsmiZoomOut.Click
            Me.Zoom(ucMapZoom.eZoomTypes.ZoomOut)
            Me.UpdateControls()
        End Sub

        Private Sub OnZoomReset(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbZoomReset.Click, m_tsmiZoomReset.Click
            Me.Zoom(ucMapZoom.eZoomTypes.ZoomReset)
            Me.UpdateControls()
        End Sub

        Private Sub OnZoomPercentChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tscbZoomPercent.SelectedIndexChanged
            Me.m_iZoomLevelIndex = Me.m_tscbZoomPercent.SelectedIndex

            For Each ctrlZoom As ucMapZoom In Me.m_lZoomContainers
                ctrlZoom.ZoomPercentage = Me.m_aiZoomLevels(Me.m_iZoomLevelIndex)
            Next
            Me.UpdateControls()
        End Sub

        Private Sub OnMapMousewheel(ByVal sender As Object, ByVal e As MouseEventArgs)
            If (Math.Abs(e.Delta) > 20) Then
                If (e.Delta > 0) Then
                    Me.Zoom(ucMapZoom.eZoomTypes.ZoomIn)
                Else
                    Me.Zoom(ucMapZoom.eZoomTypes.ZoomOut)
                End If
            End If
            Me.UpdateControls()
        End Sub

        Private Sub OnMapPositionChanged(ByVal sender As ucMapZoom)
            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True
            For Each ctrlZoom As ucMapZoom In Me.m_lZoomContainers
                If Not Object.ReferenceEquals(ctrlZoom, sender) Then
                    ctrlZoom.UpdatePosition(sender)
                End If
            Next
            Me.m_bInUpdate = False
        End Sub

#End Region ' Zoom controls

#Region " Postion mode "

        Private Sub OnViewStretch(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiViewStretch1.Click
            Me.PositionMode = ucMapZoom.ePositionModeTypes.Stretch
            Me.UpdateControls()
        End Sub

        Private Sub OnViewCenter(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiViewCenter1.Click
            Me.PositionMode = ucMapZoom.ePositionModeTypes.Center
            Me.UpdateControls()
        End Sub

#End Region ' Postion mode

#Region " Save "

        Private Sub m_tsbSaveImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbSaveImage.Click

            If (Me.UIContext Is Nothing) Then Return

            Dim format As ImageFormat = ImageFormat.Bmp
            Dim core As cCore = Me.UIContext.Core
            Dim model As cEwEModel = core.EwEModel
            Dim scenario As cEcospaceScenario = core.EcospaceScenarios(core.ActiveEcospaceScenarioIndex)
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim strFileName As String = ""

            cmdFS.Invoke(cFileUtils.ToValidFileName(cStringUtils.Localize("{0}_{1}", model.Name, scenario.Name), False), SharedResources.FILEFILTER_IMAGE)
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

                For Each ctrlZoom As ucMapZoom In Me.m_lZoomContainers
                    strFileName = cStringUtils.Localize("{0}-{1}", cmdFS.FileName, ctrlZoom.Map.Text)
                    ctrlZoom.Map.SaveToBitmap(cmdFS.FileName, format)
                Next
            End If

        End Sub

#End Region ' Save

#Region " Map events "

        Private Sub OnMapLayerAdded(sender As ucMap, layer As cDisplayLayer)
            Me.m_lLayers.Add(layer)
            AddHandler layer.LayerChanged, AddressOf OnLayerChanged
        End Sub

        Private Sub OnMapLayerRemoved(sender As ucMap, layer As cDisplayLayer)
            RemoveHandler layer.LayerChanged, AddressOf OnLayerChanged
            Me.m_lLayers.Remove(layer)
        End Sub

        Private Sub OnLayerChanged(layer As cDisplayLayer, cf As cDisplayLayer.eChangeFlags)
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Map events

#End Region ' Child control events

#Region " Internal bits "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Set the map <see cref="ucMapZoom.eZoomTypes">Zoom level</see> for displaying the map.
        ''' </summary>
        ''' <param name="zoomType">The <see cref="ucMapZoom.eZoomTypes">Zoom level</see> to use.</param>
        ''' -----------------------------------------------------------------------
        Private Sub Zoom(ByVal zoomType As ucMapZoom.eZoomTypes)

            Select Case zoomType
                Case ucMapZoom.eZoomTypes.ZoomIn
                    ' Increase zoom rate to next increment
                    Me.m_iZoomLevelIndex = Math.Min(Me.m_aiZoomLevels.Length - 1, Me.m_iZoomLevelIndex + 1)
                Case ucMapZoom.eZoomTypes.ZoomOut
                    ' Decrease zoom rate to prev increment
                    Me.m_iZoomLevelIndex = Math.Max(0, Me.m_iZoomLevelIndex - 1)
                Case ucMapZoom.eZoomTypes.ZoomReset
                    ' Zoom to 100%
                    Me.m_iZoomLevelIndex = 3
            End Select

            ' Apply
            If Me.m_tscbZoomPercent.Items.Count > Me.m_iZoomLevelIndex Then
                Me.m_tscbZoomPercent.SelectedIndex = Me.m_iZoomLevelIndex
            End If

        End Sub

        Private Sub UpdateControls()

            If (Me.IsDisposed) Then Return

            Me.m_tsbZoomIn.Enabled = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Center)
            Me.m_tsmiZoomIn.Enabled = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Center)

            Me.m_tsbZoomOut.Enabled = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Center)
            Me.m_tsmiZoomOut.Enabled = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Center)

            Me.m_tsbZoomReset.Enabled = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Center)
            Me.m_tsmiZoomReset.Enabled = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Center)

            Me.m_tscbZoomPercent.Enabled = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Center)

            Me.m_tsmiViewCenter1.Checked = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Center)
            Me.m_tsmiViewCenter2.Checked = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Center)

            Me.m_tsmiViewStretch1.Checked = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Stretch)
            Me.m_tsmiViewStretch2.Checked = (Me.PositionMode = ucMapZoom.ePositionModeTypes.Stretch)

            ' ToDo: update import/export buttons viz + enabled states
            Dim bHasSelectedLayers As Boolean = False
            Dim bHasEditableLayers As Boolean = False
            Dim bHasLayers As Boolean = False

            For Each l As cDisplayLayer In Me.m_lLayers
                bHasSelectedLayers = bHasSelectedLayers Or l.IsSelected
                bHasLayers = True
                If (TypeOf l Is cDisplayRasterLayer) Then
                    bHasEditableLayers = bHasEditableLayers Or (l.IsSelected And DirectCast(l, cDisplayRasterLayer).Editor.IsEditable)
                End If
            Next

            Me.m_tsbnImport.Enabled = bHasEditableLayers
            Me.m_tsbnExport.Enabled = bHasLayers

        End Sub

#End Region ' Internal bits

        Private Sub OnImportLayer(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnImport.Click

            Dim cmd As cImportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cImportLayerCommand.cCOMMAND_NAME), cImportLayerCommand)
            Dim lLayers As New List(Of cEcospaceLayer)
            Dim layer As cEcospaceLayer = Nothing

            ' For all raster layers
            For Each l As cDisplayLayer In Me.m_lLayers
                If (TypeOf l Is cDisplayRasterLayer) Then
                    ' Show all bundled rasters
                    If (TypeOf l Is cDisplayRasterLayerBundle) Then
                        Dim rlb As cDisplayRasterLayerBundle = DirectCast(l, cDisplayRasterLayerBundle)
                        If rlb.Editor.IsEditable Then
                            For i As Integer = 0 To Me.m_uic.Core.GetCoreCounter(rlb.CoreCounter)
                                layer = rlb.Data(i)
                                If (layer IsNot Nothing) Then
                                    lLayers.Add(layer)
                                End If
                            Next
                        End If
                    Else
                        Dim rl As cDisplayRasterLayer = DirectCast(l, cDisplayRasterLayer)
                        If (rl.Editor.IsEditable) Then
                            lLayers.Add(rl.Data)
                        End If
                    End If
                End If
            Next

            If (lLayers.Count > 0) Then cmd.Invoke(lLayers.ToArray)

        End Sub

        Private Sub OnExportLayer(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnExport.Click

            Dim cmd As cExportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cExportLayerCommand.cCOMMAND_NAME), cExportLayerCommand)
            Dim lLayers As New List(Of cEcospaceLayer)
            Dim layer As cEcospaceLayer = Nothing

            For Each l As cDisplayLayer In Me.m_lLayers
                If (TypeOf l Is cDisplayRasterLayer) Then
                    If (TypeOf l Is cDisplayRasterLayer) Then
                        If TypeOf l Is cDisplayRasterLayerBundle Then
                            Dim rlb As cDisplayRasterLayerBundle = DirectCast(l, cDisplayRasterLayerBundle)
                            For i As Integer = 0 To Me.m_uic.Core.GetCoreCounter(rlb.CoreCounter)
                                layer = rlb.Data(i)
                                If (layer IsNot Nothing) Then
                                    lLayers.Add(layer)
                                End If
                            Next
                        Else
                            Dim rl As cDisplayRasterLayer = DirectCast(l, cDisplayRasterLayer)
                            lLayers.Add(rl.Data)
                        End If
                    End If
                End If
            Next

            If (lLayers.Count > 0) Then cmd.Invoke(lLayers.ToArray)

        End Sub

    End Class

End Namespace
