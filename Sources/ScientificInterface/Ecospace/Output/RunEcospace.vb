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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports System.Threading
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Commands

#End Region

Namespace Ecospace

    ''' <summary>
    ''' Form, implementing the Ecospace Run interface.
    ''' </summary>
    Public Class RunEcospace

        ''' <summary>number of legend bins is arbitrary</summary>
        Private Const cColourBins As Integer = 200

        Private m_FishingMortMax As Single = 2.0

        Public Enum eShowGroupType
            ShowAll = 0
            ShowNonHidden
            ShowSingle
        End Enum

#Region " Variables "

        ''' <summary>The previous number of timesteps UI has drawn.</summary>
        Private m_iTimeStepPrev As Integer
        ''' <summary>The current number of timesteps available to draw.</summary>
        Private m_iTimeStepCur As Integer

        ' === Timestep and derived values ===
        Private m_dataTimeStep As cEcospaceTimestep = Nothing
        ''' <summary>The array to hold the Ecospace average biomass results.</summary>
        Private m_RelBiomassResults(,) As Single
        ''' <summary>The array to hold the Ecospace base biomass results.</summary>
        Private m_BaseBiomassResults() As Single
        ''' <summary>Contaminants over Biomass.</summary>
        Private m_ConcOverB(,,) As Single
        ''' <summary>Effort over Biomass.</summary>
        Private m_FoverB(,,) As Single


        Private m_BaseCatch() As Single
        Private m_BaseBiomass() As Single
        Private m_FishingMortScaler() As Single
        Private m_CBScaler() As Single
        Private m_BaseC() As Single

        Private m_layerDepth As cEcospaceLayer = Nothing

        ''' <summary>The speed of the plotting. 1 is the slowest, 10 is the fastest.</summary>
        Private m_iPlotStepSize As Integer

        ''' <summary>The row and col number of Biomass map plot.</summary>
        Private m_iNumGroupPlotsVert As Integer, m_iNumGroupPlotsHorz As Integer
        ''' <summary>The row and col number of Fleet map plot.</summary>
        Private m_iNumFleetPlotsVert As Integer, m_iNumFleetPlotsHorz As Integer
        ''' <summary>Number of rows and columns if basen</summary>
        ''' <remarks>???</remarks>
        Private m_iInRow As Integer, m_iInCol As Integer

        Private m_drawers As List(Of cMapDrawer)
        Private m_nMapsPerThread As Integer

        Private m_bmpBiomassMap As Bitmap

        'jb added
        Private m_spaceStats As cEcospaceStats

        Private m_bOverlay As Boolean = False
        Private m_bShowMPA As Boolean = True
        Private m_bShowIBM As Boolean = True
        Private m_bShowLabels As Boolean = True
        Private m_bInvertLabelColor As Boolean = False
        Private m_labelposHorz As StringAlignment = StringAlignment.Near
        Private m_labelposVert As StringAlignment = StringAlignment.Near

        Private m_bpConTracing As cBooleanProperty = Nothing

        Private m_showGroupMode As eShowGroupType = eShowGroupType.ShowAll
        Private m_iGroupToShow As Integer = 1
        Private m_zgh As cEcospaceZedGraphHelper = Nothing

        ''' <summary>Exposing m_sMaxEffort to the interface would allow the user to set the Effort legend sensitivity.</summary>
        Private m_sMaxEffort As Single = 5
        Private m_cmdDisplayGroups As cCommand = Nothing

        ' Properties to monitor for setting run mode states
        Private WithEvents m_bpUseIBM As cBooleanProperty = Nothing
        Private WithEvents m_bpUseNewStanza As cBooleanProperty = Nothing

#End Region ' Variables

#Region " Construction and Destruction "

        Public Sub New()

            Me.InitializeComponent()

        End Sub

#End Region ' Construction and Destruction

#Region " Initialization and Updating "

        Private Sub InitCoreParams()

            'Get the basemap
            Me.m_layerDepth = Me.Core.EcospaceBasemap.LayerDepth

            'Redim relative biomass results array
            ReDim Me.m_RelBiomassResults(Me.Core.nGroups, Me.Core.nEcospaceTimeSteps)

            'Redim base biomass base result array
            ReDim Me.m_BaseBiomassResults(Me.Core.nGroups)

            'get the ecospace stats object from the core
            Me.m_spaceStats = Me.Core.EcospaceStats

        End Sub

        Private Sub InitUIParams()

            Dim desc As New cCoreInterfaceFormatter()

            Me.m_iTimeStepCur = 0
            Me.m_iTimeStepPrev = 0

            'Plot speed 1-slowest 10-fastest
            Me.m_iPlotStepSize = 1

            'Load group combo box
            Me.m_cmbDisplayGroup.Items.Clear()
            For i As Integer = 1 To Me.Core.nGroups
                Me.m_cmbDisplayGroup.Items.Add(desc.GetDescriptor(Me.Core.EcospaceGroups(i), eDescriptorTypes.Name))
            Next i
            Me.m_cmbDisplayGroup.SelectedIndex = 0

        End Sub

        ''' <summary>
        ''' Initialization of BioMapPlot
        ''' </summary>
        Private Sub InitMapPlot()

            'Hack warning: For initialization the map dimensions are set to the value supplied by the core base map.
            'The actual size of the map must be set from the EcoSpace Timestep results(See EcospaceTimeStepDelegate())
            'This should not be called once Ecospace has been run because the map dims can be out of sync!

            Me.m_iInCol = Me.Core.EcospaceBasemap.InCol
            Me.m_iInRow = Me.Core.EcospaceBasemap.InRow
            'Core.nGroups --> updated to nLivingGroups? Non - hidden groups? Check EwE5

            Me.CalcMapDimension(Me.Core.nGroups, Me.m_iNumGroupPlotsVert, Me.m_iNumGroupPlotsHorz)
            Me.CalcMapDimension(Me.Core.nFleets, Me.m_iNumFleetPlotsVert, Me.m_iNumFleetPlotsHorz)

        End Sub

        Private Sub CalcMapDimension(ByVal iTotal As Integer, ByRef iNumPlotsVert As Integer, ByRef iNumPlotsHorz As Integer)
            iNumPlotsHorz = CInt(Math.Ceiling(Math.Sqrt(iTotal) * Me.m_iInRow / Me.m_iInCol * Me.m_pbMap.Width / Me.m_pbMap.Height))
            If iNumPlotsHorz = 0 Then
                iNumPlotsVert = iTotal
            Else
                iNumPlotsVert = CInt(Math.Ceiling(iTotal / iNumPlotsHorz))
            End If
        End Sub

        Private Sub InitDrawingThreads()

            Dim drawer As cMapDrawer
            Dim nThreads As Integer = Environment.ProcessorCount
            Dim sg As cStyleGuide = Me.StyleGuide
            Dim lColors As List(Of Color) = sg.GetEwE5ColorRamp(cColourBins)

            Me.m_nMapsPerThread = (Me.Core.nGroups + nThreads - 1) \ nThreads
            If Me.m_drawers Is Nothing Then
                Me.m_drawers = New List(Of cMapDrawer)
            Else
                Me.m_drawers.Clear()
            End If

            Me.InitOutputBitmaps()

            For i As Integer = 1 To nThreads
                drawer = New cMapDrawer(i, Me.Core)
                drawer.Graphics = Graphics.FromImage(Me.m_bmpBiomassMap)
                drawer.Colors = lColors

                Me.m_drawers.Add(drawer)
            Next
        End Sub

        Private Sub InitOutputBitmaps()
            Me.m_bmpBiomassMap = New Bitmap(Me.m_pbMap.Width, Me.m_pbMap.Height)
            For Each drawer As cMapDrawer In Me.m_drawers
                drawer.Graphics = Graphics.FromImage(m_bmpBiomassMap)
            Next
        End Sub

        Protected Overrides Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)
            If ((changeType And cStyleGuide.eChangeType.Colours) = cStyleGuide.eChangeType.Colours) Then
                Me.UpdateStyleColors()
            End If
            If ((changeType And cStyleGuide.eChangeType.GroupVisibility) = cStyleGuide.eChangeType.GroupVisibility) Then
                Me.RefreshPlot()
            End If
        End Sub

        Private Sub UpdateStyleColors()
            Me.m_pbMap.BackColor = Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND)
        End Sub

#End Region ' Initialization and Updating

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)

            If Me.UIContext Is Nothing Then Return

            Dim pm As cPropertyManager = Me.PropertyManager
            Dim ecospaceModelParams As cEcospaceModelParameters = Me.Core.EcospaceModelParameters()

            ' Start listening to props
            Me.m_bpConTracing = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.ConSimOnEcoSpace), cBooleanProperty)
            Me.m_bpUseIBM = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.UseIBM), cBooleanProperty)
            Me.m_bpUseNewStanza = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.UseNewMultiStanza), cBooleanProperty)

            ' Initially collapse labels
            Me.m_hdrLabelOptions.IsCollapsed = True

            Me.InitCoreParams()
            Me.InitUIParams()
            Me.InitMapPlot()
            Me.InitDrawingThreads()

            Me.m_zgh = New cEcospaceZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_zgPlotLarge)
            Me.m_zgh.ShowPointValue = True

            Me.m_cmdDisplayGroups = Me.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            If (Me.m_cmdDisplayGroups IsNot Nothing) Then
                Me.m_cmdDisplayGroups.AddControl(Me.m_btnDisplayGroups)
                AddHandler Me.m_cmdDisplayGroups.OnPostInvoke, AddressOf OnDisplayGroupsInvoked
            End If
            Me.m_cmbLabelPos.SelectedIndex = 0

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace}

            Me.ShowGroupMode = eShowGroupType.ShowAll
            Me.IsRunning = Me.Core.StateMonitor.IsEcospaceRunning

            Dim nGrps As Integer = Me.Core.nGroups
            ReDim Me.m_BaseC(nGrps)
            ReDim Me.m_BaseCatch(nGrps)
            ReDim Me.m_FishingMortScaler(nGrps)
            ReDim Me.m_BaseBiomass(nGrps)

            For igrp As Integer = 1 To nGrps
                Me.m_FishingMortScaler(igrp) = 1
                Me.m_BaseC(igrp) = 1
                Me.m_BaseBiomass(igrp) = Me.Core.StartBiomass(igrp)
                For iflt As Integer = 1 To Me.Core.nFleets
                    Me.m_BaseCatch(igrp) += Me.Core.FleetInputs(iflt).Landings(igrp) + Me.Core.FleetInputs(iflt).Discards(igrp)
                Next
            Next

            'Scaler for the fishing mort map legend
            Me.m_txFMax.Text = Me.m_FishingMortMax.ToString

            'Start tracking ConcTracing setting
            AddHandler Me.m_bpConTracing.PropertyChanged, AddressOf OnPropertyChanged
            ' Start tracking core state monitor for Ecospace run states
            AddHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreStateChanged

            Me.UpdateStyleColors()
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

            Me.m_bpUseIBM = Nothing
            Me.m_bpUseNewStanza = Nothing

            Try

                If (Me.m_cmdDisplayGroups IsNot Nothing) Then
                    Me.m_cmdDisplayGroups.RemoveControl(Me.m_btnDisplayGroups)
                    RemoveHandler Me.m_cmdDisplayGroups.OnPostInvoke, AddressOf OnDisplayGroupsInvoked
                    Me.m_cmdDisplayGroups = Nothing
                End If

                Me.Core.StopEcospace()
                Me.m_drawers.Clear()

                Me.m_zgh.Detach()
                Me.m_zgh = Nothing

                ' Stop tracking core state monitor for Ecospace run states
                RemoveHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreStateChanged
                ' Stop tracking ConcTracing setting
                RemoveHandler Me.m_bpConTracing.PropertyChanged, AddressOf OnPropertyChanged
                Me.m_bpConTracing = Nothing

            Catch ex As Exception
                'make sure something in the interface does not stop the base from cleaning up
                Debug.Assert(False, Me.ToString & ".OnFormClosed() Exception: " & ex.Message)
            End Try

            MyBase.OnFormClosed(e)

        End Sub

        Protected Overrides Sub OnResizeEnd(ByVal e As EventArgs)
            Me.InitOutputBitmaps()
        End Sub

        Public Overrides ReadOnly Property IsRunForm() As Boolean
            Get
                Return True
            End Get
        End Property

        Private Sub OnMapMouseDouble(ByVal sender As Object, ByVal e As EventArgs) _
            Handles m_pbMap.DoubleClick
            ' ToDo: use toolbar (perhaps floating toolbar?)
            Me.SaveMapImage()
        End Sub

        Private Sub OnMapMouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) _
            Handles m_pbMap.MouseClick
            ' ToDo: use toolbar (perhaps floating toolbar?) Right-click should be reserved for a context menu
            If e.Button = Windows.Forms.MouseButtons.Right Then
                Me.SaveMapImage()
            End If
        End Sub

        Private Sub OnPaintMap(ByVal sender As Object, ByVal e As PaintEventArgs) _
            Handles m_pbMap.Paint
            Me.PlotMap(e.Graphics)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Display groups command has been invoked: entirely invalidate the map plot.
        ''' This is rather hack but necessary, since this form is entirely responsible
        ''' for rendering the map picture box.
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' -------------------------------------------------------------------
        Private Sub OnDisplayGroupsInvoked(ByVal cmd As cCommand)
            Me.m_showGroupMode = eShowGroupType.ShowNonHidden
            Me.UpdateControls()
            Me.RefreshMap()
        End Sub

#End Region ' Events 

#Region " Biomass graph "

        Private Sub UpdateBiomassPlot()
            For iGroup As Integer = 1 To Core.nGroups
                For iTimeStep As Integer = Me.m_iTimeStepPrev To Me.m_iTimeStepCur - 1
                    Me.m_zgh.AddValue(iGroup, iTimeStep, Me.m_RelBiomassResults(iGroup, iTimeStep + 1))
                Next
            Next
            Me.m_zgh.RescaleAndRedraw()
        End Sub

        Private Sub SaveMapImage()

            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim bmp As Bitmap = Nothing
            Dim g As Graphics = Nothing
            Dim br As SolidBrush = Nothing

            cmdFS.Invoke(Me.Core.EcospaceOutputFileLocation("map"), SharedResources.FILEFILTER_IMAGE)

            If cmdFS.Result = Windows.Forms.DialogResult.OK Then

                'Set the image format
                Dim imgFormat As Drawing.Imaging.ImageFormat = Drawing.Imaging.ImageFormat.Bmp

                Select Case cmdFS.FilterIndex
                    Case 0
                        imgFormat = Drawing.Imaging.ImageFormat.Bmp
                    Case 1
                        imgFormat = Drawing.Imaging.ImageFormat.Jpeg
                    Case 2
                        imgFormat = Drawing.Imaging.ImageFormat.Gif
                    Case 3
                        imgFormat = Drawing.Imaging.ImageFormat.Png
                    Case 4
                        imgFormat = Drawing.Imaging.ImageFormat.Tiff
                    Case Else
                        Debug.Assert(False)
                End Select

                bmp = New Bitmap(Me.m_pbMap.Width, Me.m_pbMap.Height, Imaging.PixelFormat.Format32bppArgb)
                g = Graphics.FromImage(bmp)
                br = New SolidBrush(Color.White)
                g.FillRectangle(br, 0, 0, bmp.Width, bmp.Height)

                Try
                    Me.PlotMap(g)
                    bmp.Save(cmdFS.FileName, imgFormat)
                    ' ToDo: throw succes
                Catch ex As Exception
                    ' ToDo: throw error
                Finally
                    g.Dispose()
                    g = Nothing
                    br.Dispose()
                    br = Nothing
                End Try
            End If

        End Sub

#End Region ' Biomass graph

#Region " Map plot "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Plot the biomass map via multiple threads.
        ''' </summary>
        ''' <param name="g"></param>
        ''' -------------------------------------------------------------------
        Private Sub PlotBiomassMapThreaded(ByRef g As Graphics)

            ' Sanity check
            If Me.m_dataTimeStep Is Nothing Then Return

            Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters
            Dim sTSpy As Single = parms.NumberOfTimeStepsPerYear
            Dim iYear As Integer = CInt(Math.Floor(Me.m_iTimeStepCur / sTSpy))
            Dim iMonth As Integer = CInt(cCore.N_MONTHS / sTSpy * (Me.m_iTimeStepCur - (iYear * sTSpy)))
            Dim drawer As cMapDrawer = Nothing
            Dim iNumVisGroups As Integer = 0
            Dim lVisGroups As New List(Of Integer)
            Dim bShowGroup As Boolean = False

            For iGroup As Integer = 1 To Me.Core.nGroups

                Select Case Me.ShowGroupMode
                    Case eShowGroupType.ShowAll
                        bShowGroup = True
                    Case eShowGroupType.ShowSingle
                        bShowGroup = (iGroup = Me.GroupToShow)
                    Case eShowGroupType.ShowNonHidden
                        bShowGroup = Me.StyleGuide.GroupVisible(iGroup)
                End Select

                If bShowGroup Then
                    lVisGroups.Add(iGroup)
                    iNumVisGroups += 1
                End If
            Next

            ' JS05Mar10: disabled console output to keep moving fast
            'Console.WriteLine("Step {0} = year {1}, month {2} at {3}", Me.m_iTimeStepCur, iYear, iMonth, Me.Core.EcospaceModelParameters.NumberOfTimeStepsPerYear)

            Select Case Me.ShowGroupMode

                Case eShowGroupType.ShowAll, _
                     eShowGroupType.ShowNonHidden

                    ' Note that the user may have hidden all groups!
                    Me.m_iNumGroupPlotsHorz = CInt(Math.Ceiling(Math.Sqrt(iNumVisGroups) * Me.m_iInRow / Me.m_iInCol * Me.m_pbMap.Width / Me.m_pbMap.Height))
                    Me.m_iNumGroupPlotsVert = CInt(Math.Ceiling(iNumVisGroups / Math.Max(1, Me.m_iNumGroupPlotsHorz)))

                Case eShowGroupType.ShowSingle
                    Me.m_iNumGroupPlotsHorz = 1
                    Me.m_iNumGroupPlotsVert = 1

            End Select

            ' Clear background
            Me.InitOutputBitmaps()

            Try

                Dim originList As New List(Of PointF)
                Dim rectList As New List(Of Rectangle)
                Dim xScale As Double = m_iNumGroupPlotsHorz * (m_iInCol + 1) + 1
                Dim yScale As Double = m_iNumGroupPlotsVert * (m_iInRow + 1) + 1

                If xScale > 0 Then xScale = m_pbMap.Width / xScale
                If yScale > 0 Then yScale = m_pbMap.Height / yScale

                For i As Integer = 0 To m_iNumGroupPlotsVert - 1
                    For j As Integer = 0 To m_iNumGroupPlotsHorz - 1
                        Dim iRect As Integer = i * m_iNumGroupPlotsHorz + j
                        If iRect < Core.nGroups Then
                            Dim origin As PointF = New PointF((m_iInCol + 1) * j + 1, i * (m_iInRow + 1) + 1)
                            Dim rect As Rectangle = New Rectangle(CInt(origin.X * xScale), _
                                                                    CInt(origin.Y * yScale), _
                                                                    CInt(m_iInCol * xScale), _
                                                                    CInt(m_iInRow * yScale))
                            originList.Add(origin)
                            rectList.Add(rect)
                        End If
                    Next
                Next

                Dim maptype As cMapDrawer.eMapType
                Dim RelScaler() As Single
                Dim ifirst As Integer = 0
                Dim ilast As Integer = 0

                For Each drawer In m_drawers

                    If drawer.AllowedToRun Then

                        'init the drawer to the latest values
                        drawer.OriginList = originList
                        drawer.RectList = rectList
                        drawer.Font = Me.StyleGuide.Font(cStyleGuide.eApplicationFontType.Legend)
                        drawer.ShowLabels = Me.m_bShowLabels
                        drawer.InvertLabelColors = Me.m_bInvertLabelColor
                        drawer.SetLabelPosition(Me.m_labelposHorz, Me.m_labelposVert)

                        drawer.StanzaDS = Nothing

                        If Me.m_rbDisplayRelBiomass.Checked Then
                            drawer.Map = Me.m_dataTimeStep.BiomassMap
                            maptype = cMapDrawer.eMapType.RelBiomass
                            RelScaler = Me.m_BaseBiomass

                            If parms.UseIBM And Me.m_bShowIBM Then
                                drawer.StanzaDS = Me.m_dataTimeStep.StanzaDS
                            End If

                        ElseIf Me.m_rbDisplayFOverB.Checked Then
                            drawer.Map = Me.m_FoverB
                            maptype = cMapDrawer.eMapType.FishingMortRate
                            RelScaler = Me.m_FishingMortScaler

                        ElseIf Me.m_rbDisplayF.Checked Then
                            drawer.Map = Me.m_dataTimeStep.CatchMap
                            maptype = cMapDrawer.eMapType.RelCatch
                            RelScaler = Me.m_BaseCatch

                        ElseIf Me.m_rbDisplayContaminantC.Checked Then
                            drawer.Map = Me.m_dataTimeStep.ContaminantMap
                            maptype = cMapDrawer.eMapType.RelContam
                            RelScaler = Me.m_BaseC

                        ElseIf Me.m_rbDisplayCoverB.Checked Then
                            drawer.Map = Me.m_ConcOverB
                            maptype = cMapDrawer.eMapType.ContamRate
                            RelScaler = Me.m_BaseC

                        End If

                        Dim mapArgs As New cMapDrawerArgs(maptype, RelScaler, Me.m_FishingMortMax)

                        drawer.InCol = Me.m_iInCol
                        drawer.InRow = Me.m_iInRow
                        drawer.Month = iMonth

                        ilast = Math.Min(ifirst + Me.m_nMapsPerThread - 1, iNumVisGroups - 1)

                        drawer.ClearGroups()
                        For i As Integer = ifirst To ilast
                            drawer.AddGroup(lVisGroups(i), i)
                        Next
                        drawer.ShowMPA = Me.m_bShowMPA

                        drawer.SignalState.Reset()

                        drawer.AllowedToRun = False
                        ThreadPool.QueueUserWorkItem(AddressOf drawer.Draw, mapArgs)

                        ifirst += m_nMapsPerThread
                    End If
                Next

                For Each drawer In m_drawers
                    drawer.SignalState.WaitOne()
                Next

                g.DrawImage(m_bmpBiomassMap, 0, 0)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

        Private Sub PlotMap(ByVal g As Graphics)
            Try
                If m_rbDisplayFishingEffort.Checked Then
                    PlotFishingEffortMap(g)
                Else
                    PlotBiomassMapThreaded(g)
                End If
            Catch ex As Exception
                ' Whoah!
            End Try
        End Sub


        Private Sub setFleetsForSelGroups()

            'Turn all the fleets off
            For iflt As Integer = 1 To Me.Core.nFleets
                Me.StyleGuide.FleetVisible(iflt) = False
            Next

            'now just the ones for the seleted group
            For igrp As Integer = 1 To Me.Core.nGroups
                If Me.StyleGuide.GroupVisible(igrp) Then
                    For iflt As Integer = 1 To Me.Core.nFleets
                        Dim flt As cFleetInput = Me.Core.FleetInputs(iflt)
                        If flt.Landings(igrp) + flt.Discards(igrp) > 0 Then
                            Me.StyleGuide.FleetVisible(iflt) = True
                        End If
                    Next
                End If
            Next

        End Sub

        Private Sub PlotFishingEffortMap(ByRef g As Graphics)

            Dim iNumVizFleets As Integer = 0
            Dim lVizFleets As New List(Of Integer)

            If m_iTimeStepCur > 0 Then

                For iFleet As Integer = 1 To Me.Core.nFleets
                    If Me.StyleGuide.FleetVisible(iFleet) Then
                        lVizFleets.Add(iFleet)
                        iNumVizFleets += 1
                    End If
                Next

                Me.m_iNumFleetPlotsHorz = CInt(Math.Ceiling(Math.Sqrt(iNumVizFleets) * Me.m_iInRow / Me.m_iInCol * Me.m_pbMap.Width / Me.m_pbMap.Height))
                Me.m_iNumFleetPlotsVert = CInt(Math.Ceiling(iNumVizFleets / Me.m_iNumFleetPlotsHorz))

                Dim xScale As Double = m_iNumFleetPlotsHorz * (m_iInCol + 1) + 1
                Dim yScale As Double = m_iNumFleetPlotsVert * (m_iInRow + 1) + 1
                If xScale > 0 Then xScale = m_pbMap.Width / xScale
                If yScale > 0 Then yScale = m_pbMap.Height / yScale

                For i As Integer = 0 To m_iNumFleetPlotsVert - 1
                    For j As Integer = 0 To m_iNumFleetPlotsHorz - 1
                        Dim cur As Integer = i * m_iNumFleetPlotsHorz + j
                        If cur < iNumVizFleets Then
                            Dim origin As PointF = New PointF((m_iInCol + 1) * j + 1, i * (m_iInRow + 1) + 1)
                            Dim rect As Rectangle = New Rectangle(CInt(origin.X * xScale), _
                                                                    CInt(origin.Y * yScale), _
                                                                    CInt(m_iInCol * xScale), _
                                                                    CInt(m_iInRow * yScale))
                            Try
                                DrawFishingBaseMap(Me.m_dataTimeStep.FishingEffortMap, lVizFleets(cur), rect, g)
                            Catch ex As Exception

                            End Try
                        End If
                    Next
                Next

            End If

        End Sub

        Private Sub DrawFishingBaseMap(ByVal mapFishing(,,) As Single, _
                                       iFleet As Integer, ByVal rcPos As Rectangle, ByRef g As Graphics)

            Dim sg As cStyleGuide = Me.StyleGuide
            Dim lColors As List(Of Color) = sg.GetEwE5ColorRamp(cColourBins)
            Dim cScaler As Single = cColourBins / Me.m_sMaxEffort
            Dim brCell As Brush = Nothing
            Dim sTSpy As Single = Me.Core.EcospaceModelParameters.NumberOfTimeStepsPerYear
            Dim iYear As Integer = CInt(Math.Floor(Me.m_iTimeStepCur / sTSpy))
            Dim iMonth As Integer = CInt(cCore.N_MONTHS / sTSpy * (Me.m_iTimeStepCur - (iYear * sTSpy)))

            For i As Integer = 1 To m_iInRow
                For j As Integer = 1 To m_iInCol
                    Dim icc As Single

                    'Effort for a single fleet
                    icc = mapFishing(iFleet, i, j) * cScaler
                    'Convert to effort per unit of area
                    'icc = baseMap(iFleet, i, j) * cScaler / cEcospaceDataStructures.Width(i)

                    'Boundary check
                    icc = Math.Max(Math.Min(cColourBins, icc), 0)

                    Dim tmpBrush As SolidBrush = Nothing
                    'If it is water
                    If CInt(m_layerDepth.Cell(i, j)) > 0 Then
                        ' #Water
                        tmpBrush = New SolidBrush(lColors(CInt(icc)))
                    Else
                        ' #Land
                        tmpBrush = New SolidBrush(Color.Gray)
                    End If

                    Dim tmpRect As RectangleF = New RectangleF(CSng(rcPos.Left + (j - 1) * rcPos.Width() / m_iInCol), _
                                            CSng(rcPos.Top + (i - 1) * rcPos.Height() / m_iInRow), _
                                            CSng(rcPos.Width() / m_iInCol), _
                                            CSng(rcPos.Height() / m_iInRow))
                    g.FillRectangle(tmpBrush, tmpRect)
                    tmpBrush.Dispose()

                    ' Draw MPA
                    If Me.m_bShowMPA Then
                        Dim iMPA As Integer = CInt(Me.Core.EcospaceBasemap.LayerMPA.Cell(i, j))
                        ' Is MPA cell?
                        If iMPA > 0 Then
                            If Me.Core.EcospaceMPAs(iMPA).MPAMonth(iMonth) Then
                                brCell = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, Color.LightGray, Color.Transparent)
                            Else
                                brCell = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, Color.Black, Color.Transparent)
                            End If
                            g.FillRectangle(brCell, tmpRect)
                            brCell.Dispose()
                        End If
                    End If

                Next
            Next

            'Draw the black frame of base map
            g.DrawRectangle(Pens.Black, rcPos)

            'Display the group name
            If Me.m_bShowLabels Then
                Dim fltName As String = Core.EcospaceFleets(iFleet).Name
                Dim br As Brush = Brushes.Black
                Dim fmt As New StringFormat()

                fmt.Alignment = Me.m_labelposHorz
                fmt.LineAlignment = Me.m_labelposVert

                If Me.m_bInvertLabelColor Then br = Brushes.White

                g.DrawString(fltName, Me.StyleGuide.Font(cStyleGuide.eApplicationFontType.Legend), _
                             br, rcPos, fmt)
            End If

        End Sub

#End Region ' Map plot

#Region " Color chart "

        Private Sub pbColors_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles m_pbColors.Paint

            Dim g As Graphics = e.Graphics
            Dim sg As cStyleGuide = Me.StyleGuide
            Dim lColors As List(Of Color) = sg.GetEwE5ColorRamp(Me.Core.nGroups)
            Dim sHeight As Single = CSng(Me.m_pbColors.Height / Me.Core.nGroups)
            Dim brTmp As SolidBrush = Nothing
            For i As Integer = 1 To Me.Core.nGroups
                brTmp = New SolidBrush(lColors(i))
                g.FillRectangle(brTmp, 0, m_pbColors.Height - sHeight * i, m_pbColors.Width, sHeight)
                brTmp.Dispose()
            Next

        End Sub

#End Region ' Color chart

#Region " Events "

        Private Sub ClearResults()

            Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters

            For i As Integer = 1 To Me.Core.nGroups - 1
                For j As Integer = 1 To Me.Core.nEcospaceTimeSteps - 1
                    Me.m_RelBiomassResults(i, j) = 0
                Next j
                Me.m_BaseBiomassResults(i) = 0
            Next i

            ' Reset plot drawer if overlay is not needed
            If Me.m_bOverlay = False Then
                Me.m_zgh.Reset(Me.Core.nGroups, Me.Core.nEcospaceTimeSteps, Me.Core.EcosimFirstYear, parms.NumberOfTimeStepsPerYear)
            Else
                Me.m_zgh.Overlay(Me.Core.nGroups)
            End If
            Me.RefreshPlot()
            Me.RefreshMap()
        End Sub

        Private Sub btnRun_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnRun.Click

            Me.ClearResults()

            Me.m_iTimeStepCur = 0
            Me.Core.RunEcoSpace(AddressOf onEcospaceTimeStep)
            Me.m_cbOverlay.Enabled = True

        End Sub

        Private Sub m_btnStop_Click(ByVal sender As Object, ByVal e As EventArgs) _
            Handles m_btnStop.Click

            Me.Core.StopEcospace()

            ' Controls wil be updated via Core state monitor events
            'Me.UpdateControls()
        End Sub

        Private Sub OnSelectDataChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbDisplayRelBiomass.CheckedChanged, _
                    m_rbDisplayFishingEffort.CheckedChanged, _
                    m_rbDisplayCoverB.CheckedChanged, _
                    m_rbDisplayContaminantC.CheckedChanged, _
                    m_rbDisplayF.CheckedChanged, m_rbDisplayFOverB.CheckedChanged

            If Me.m_rbDisplayFishingEffort.Checked Then
                If Me.m_ckSelFleets.Checked Then
                    Me.setFleetsForSelGroups()
                End If
            End If

            Me.RefreshPlot()
            Me.RefreshMap()

        End Sub


        Private Sub OnSelFleetsCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_ckSelFleets.CheckedChanged

            If Me.m_ckSelFleets.Checked Then
                If Me.m_rbDisplayFishingEffort.Checked Then

                    Me.setFleetsForSelGroups()

                    Me.RefreshPlot()
                    Me.RefreshMap()

                End If
            End If

        End Sub

        Private Sub OnOverlay(ByVal sender As Object, ByVal e As EventArgs) _
            Handles m_cbOverlay.Click
            Me.m_bOverlay = m_cbOverlay.Checked
        End Sub

        Private Sub m_cbDisplayGroup_GotFocus(ByVal sender As Object, ByVal e As EventArgs) _
            Handles m_cmbDisplayGroup.GotFocus
            Me.ShowGroupMode = eShowGroupType.ShowSingle
        End Sub

        Private Sub OnSelectGroupToShow(ByVal sender As Object, ByVal e As EventArgs) _
            Handles m_cmbDisplayGroup.SelectedIndexChanged
            Me.GroupToShow = (Me.m_cmbDisplayGroup.SelectedIndex + 1)
        End Sub

        Private Sub rbDisplayOption_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
                Handles m_rbShowAll.CheckedChanged, m_rbShowNonHidden.CheckedChanged, m_rbShowSingle.CheckedChanged

            If Me.m_rbShowAll.Checked Then Me.ShowGroupMode = eShowGroupType.ShowAll
            If Me.m_rbShowNonHidden.Checked Then Me.ShowGroupMode = eShowGroupType.ShowNonHidden
            If Me.m_rbShowSingle.Checked Then Me.ShowGroupMode = eShowGroupType.ShowSingle
            Me.UpdateControls()

            Me.RefreshPlot()
            Me.RefreshMap()
        End Sub

        Private Sub m_cbMPA_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbMPA.CheckedChanged

            Me.m_bShowMPA = m_cbMPA.Checked
            Me.UpdateControls()
            Me.RefreshMap()

        End Sub

        Private Sub m_cbShowIBMPackets_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbShowIBMPackets.CheckedChanged

            Me.m_bShowIBM = m_cbShowIBMPackets.Checked
            Me.UpdateControls()
            Me.RefreshMap()

        End Sub

        Private Sub OnShowLabelsChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbShowLabels.CheckedChanged

            Me.m_bShowLabels = Me.m_cbShowLabels.Checked
            Me.m_cbInvertColor.Enabled = Me.m_bShowLabels
            Me.m_cmbLabelPos.Enabled = Me.m_bShowLabels
            Me.RefreshMap()

        End Sub

        Private Sub OnInvertLabelsChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbInvertColor.CheckedChanged

            Me.m_bInvertLabelColor = Me.m_cbInvertColor.Checked
            Me.RefreshMap()

        End Sub

        Private Sub OnChangeLabelPos(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbLabelPos.SelectedIndexChanged

            Dim iSel As Integer = Math.Max(Math.Min(9, Me.m_cmbLabelPos.SelectedIndex), 0)
            Me.m_labelposHorz = DirectCast(CInt(iSel Mod 3), StringAlignment)
            Me.m_labelposVert = DirectCast(CInt(Math.Floor(iSel / 3)), StringAlignment)
            Me.RefreshMap()

        End Sub

        Private Sub OnRunTypeSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbRunType.SelectedIndexChanged

            If Me.m_bInUpdate Then Return

            Select Case Me.m_cmbRunType.SelectedIndex
                Case 0
                    Me.m_bpUseNewStanza.SetValue(True)
                    Me.m_bpUseIBM.SetValue(False)
                Case 1
                    Me.m_bpUseNewStanza.SetValue(False)
                    Me.m_bpUseIBM.SetValue(True)
                Case 2
                    Me.m_bpUseNewStanza.SetValue(False)
                    Me.m_bpUseIBM.SetValue(False)
            End Select
        End Sub

        Private Sub OnPause(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnPause.Click
            If Me.Core.EcospacePaused() Then
                Me.Core.EcospacePaused = False
            Else
                Me.Core.EcospacePaused = True
            End If
            Me.UpdateControls()
        End Sub


#Region "Crap for new FishingMort Legend max value"
        'Added for the EcoOcean model to scale Catch/Bio legend
        'if there is some way to scale legends then this will go
        Private Sub m_txFMax_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles m_txFMax.KeyUp
            If e.KeyCode = Keys.Enter Then
                Dim newMax As Single = CSng(Val(Me.m_txFMax.Text))
                If ValidateFMax(newMax) Then
                    Me.m_FishingMortMax = newMax
                    Me.RefreshPlot()
                    Me.RefreshMap()
                End If
            End If
        End Sub

        Private Function ValidateFMax(ByVal newFMax As Single) As Boolean
            If newFMax > 0 And newFMax < 10 Then
                Return True
            End If
            Return False
        End Function


        Private Sub OntxFMaxValidated(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_txFMax.Validated
            Try
                Dim newMax As Single = CSng(Val(Me.m_txFMax.Text))
                If newMax <> Me.m_FishingMortMax Then
                    Me.m_FishingMortMax = newMax
                    Me.RefreshPlot()
                    Me.RefreshMap()
                End If
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OntxFMaxValidating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles m_txFMax.Validating
            Dim newVal As Single = CSng(Val(Me.m_txFMax.Text))
            If Me.ValidateFMax(newVal) Then
                e.Cancel = False
                Return
            End If
            e.Cancel = True
            Return
        End Sub

#End Region

#End Region ' Events

#Region " cProperty events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when either of the two model state properties changes.
        ''' </summary>
        ''' <param name="prop">The property that changed.</param>
        ''' <param name="changeFlags">The extent of the change.</param>
        ''' -------------------------------------------------------------------
        Private Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags) _
                Handles m_bpUseIBM.PropertyChanged, m_bpUseNewStanza.PropertyChanged
            Me.UpdateControls()
        End Sub

#End Region ' cProperty events

#Region " Ecospace Events/Delegates "

        ''' <summary>
        ''' This GUI event handler will be called for every time step of the Ecospace model run. 
        ''' </summary>
        ''' <param name="TimeStepData">Data from the current time step</param>
        Private Sub onEcospaceTimeStep(ByRef TimeStepData As cEcospaceTimestep)

            Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters()

            ' Biomass plotting
            ' For each time step, we get the Biomass from the core and store it into our array
            ' The following algorithm was extracted from EwE5. Biomass Log plotting, the value between 0.1 to 10. 
            For groupIndex As Integer = 1 To Core.nGroups
                If TimeStepData.iTimeStep = 1 Then
                    m_BaseBiomassResults(groupIndex) = TimeStepData.RelativeBiomass(groupIndex)
                    m_RelBiomassResults(groupIndex, 1) = 0
                Else
                    m_RelBiomassResults(groupIndex, TimeStepData.iTimeStep) = CSng(Math.Log10(TimeStepData.RelativeBiomass(groupIndex)))
                End If
            Next

            'Temporary variables to store the timesteps for plotting. 
            m_iTimeStepPrev = m_iTimeStepCur
            m_iTimeStepCur = TimeStepData.iTimeStep

            'Update the running simulation years progress label.
            cApplicationStatusNotifier.UpdateProgress(Me.Core, My.Resources.STATUS_ECOSPACE_RUNNING, CSng(Me.m_iTimeStepCur / Me.Core.nEcospaceTimeSteps))
            Me.m_dataTimeStep = TimeStepData

            ' Calc C/B, F/B
            ReDim Me.m_ConcOverB(TimeStepData.inRows, TimeStepData.inCols, Me.Core.nGroups)
            ReDim Me.m_FoverB(TimeStepData.inRows, TimeStepData.inCols, Me.Core.nGroups)

            For iRow As Integer = 1 To TimeStepData.inRows
                For iCol As Integer = 1 To TimeStepData.inCols
                    For iGroup As Integer = 1 To Me.Core.nGroups
                        Dim sB As Single = TimeStepData.BiomassMap(iRow, iCol, iGroup)
                        If (sB > 0) Then
                            If (TimeStepData.ContaminantMap IsNot Nothing) Then
                                Me.m_ConcOverB(iRow, iCol, iGroup) = TimeStepData.ContaminantMap(iRow, iCol, iGroup) / sB
                            End If
                        End If
                        Me.m_FoverB(iRow, iCol, iGroup) = TimeStepData.CatchMap(iRow, iCol, iGroup) / sB
                    Next iGroup
                Next iCol
            Next iRow

            'if the size of the map has changed reset the interface
            If m_iInRow <> TimeStepData.inRows Or m_iInCol <> TimeStepData.inCols Then
                'set the map dims these are passed to the drawing threads in PlotBiomassMapThreaded()
                m_iInRow = TimeStepData.inRows
                m_iInCol = TimeStepData.inCols

                CalcMapDimension(Core.nGroups, m_iNumGroupPlotsVert, m_iNumGroupPlotsHorz)
                CalcMapDimension(Core.nFleets, m_iNumFleetPlotsVert, m_iNumFleetPlotsHorz)
            End If


            Me.UpdateBiomassPlot()
            Me.m_pbMap.Invalidate()
            Me.UpdateControls()

        End Sub

#End Region ' Ecospace Delegates

#Region " Overrides "

        Private Sub OnCoreStateChanged(ByVal cms As cCoreStateMonitor)

            If cms.IsEcospaceRunning <> Me.IsRunning Then

                ' Update state flag
                Me.IsRunning = cms.IsEcospaceRunning

                ' Update status feedback
                If Me.IsRunning Then
                    cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_ECOSPACE_RUNNING)
                Else
                    cApplicationStatusNotifier.EndProgress(Me.Core)
                End If

                ' Update controls
                '    Me.m_lblProgress.Text = ""
                Me.UpdateControls()

            End If
        End Sub

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            Dim bHasRunInit As Boolean

            Try

                For Each vStat As cVariableStatus In msg.Variables

                    Select Case vStat.VarName

                        Case eVarNameFlags.TotalTime, eVarNameFlags.NumTimeStepsPerYear, eVarNameFlags.EcoSimNYears

                            If Not bHasRunInit Then
                                Me.InitCoreParams()
                                Me.InitUIParams()
                                bHasRunInit = True
                            End If

                    End Select

                Next

            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".OnCoreMessage() Exception: " & ex.Message)
            End Try

        End Sub

#End Region ' Overrides

#Region " Internal implementation "

        Private Property ShowGroupMode() As eShowGroupType
            Get
                Return Me.m_showGroupMode
            End Get
            Set(ByVal value As eShowGroupType)
                If (value <> Me.m_showGroupMode) Then
                    Me.m_showGroupMode = value
                    Me.UpdateControls()
                    Me.RefreshMap()
                    Me.RefreshPlot()
                End If
            End Set
        End Property

        Private Property GroupToShow() As Integer
            Get
                Return Me.m_iGroupToShow
            End Get
            Set(ByVal value As Integer)
                If (value <> Me.m_iGroupToShow) Then
                    Me.m_iGroupToShow = value
                    Me.RefreshMap()
                    Me.RefreshPlot()
                End If
                Me.ShowGroupMode = eShowGroupType.ShowSingle
            End Set
        End Property

        Private m_bInUpdate As Boolean = False

        Protected Overrides Sub UpdateControls()

            ' Sanity check
            If Me.Core Is Nothing Then Return
            If Me.m_bInUpdate = True Then Return

            Dim csm As cCoreStateMonitor = Me.Core.StateMonitor
            Dim bUseIBM As Boolean = CBool(Me.m_bpUseIBM.GetValue())
            Dim bUseNewStanza As Boolean = CBool(Me.m_bpUseNewStanza.GetValue())

            Me.m_bInUpdate = True

            ' Enable run and stop buttons based on Ecospace run state
            Me.m_btnRun.Enabled = (Me.IsRunning = False)
            Me.m_btnStop.Enabled = (Me.IsRunning = True)

            Me.m_btnPause.Enabled = (Me.IsRunning = True)
            If Me.Core.EcospacePaused Then
                Me.m_btnPause.Text = My.Resources.ECOSPACE_RESUME
            Else
                Me.m_btnPause.Text = My.Resources.ECOSPACE_PAUSE
            End If

            ' Enable contaminant options based on space tracer enabled state
            Me.m_rbDisplayContaminantC.Enabled = CBool(Me.m_bpConTracing.GetValue())
            Me.m_rbDisplayCoverB.Enabled = CBool(Me.m_bpConTracing.GetValue())

            Select Case Me.ShowGroupMode
                Case eShowGroupType.ShowAll
                    Me.m_rbShowAll.Checked = True
                Case eShowGroupType.ShowNonHidden
                    Me.m_rbShowNonHidden.Checked = True
                Case eShowGroupType.ShowSingle
                    Me.m_rbShowSingle.Checked = True
            End Select

            Me.m_cbOverlay.Checked = Me.m_bOverlay
            Me.m_cbOverlay.Enabled = Me.Core.StateMonitor.IsEcospaceRunning

            Me.m_cbMPA.Checked = Me.m_bShowMPA
            Me.m_cbMPA.Enabled = (Me.m_rbDisplayFishingEffort.Checked Or Me.m_rbDisplayRelBiomass.Checked)

            Me.m_cbShowIBMPackets.Checked = Me.m_bShowIBM
            Me.m_cbShowIBMPackets.Enabled = bUseIBM

            Dim iIndex As Integer = 2

            If bUseIBM Then iIndex = 1
            If bUseNewStanza Then iIndex = 0

            Me.m_cmbRunType.SelectedIndex = iIndex
            Me.m_cmbRunType.Enabled = (Me.IsRunning = False)

            Me.m_bInUpdate = False

        End Sub

        Private Sub RefreshMap()

            If Me.Core Is Nothing Then Return
            Me.m_pbMap.Refresh()

        End Sub

        Private Sub RefreshPlot()

            If Me.Core Is Nothing Then Return
            If (Me.m_zgh IsNot Nothing) Then

                Me.m_zgh.GroupShowMode = Me.ShowGroupMode
                Me.m_zgh.GroupToShow = Me.GroupToShow
                Me.m_zgh.UpdateCurveVisibility()
                Me.m_zgh.Redraw()

            End If

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
