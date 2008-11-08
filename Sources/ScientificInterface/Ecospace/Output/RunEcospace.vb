'==============================================================================
'
' $Log: RunEcospace.vb,v $
' Revision 1.2  2008/11/08 23:51:19  jeroens
' Renamed file commands
'
' Revision 1.1  2008/09/26 07:32:02  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.68  2008/09/09 14:44:54  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.67  2008/08/22 00:20:26  joeh
' Fix graph overlay. Overlay is possible now.
'
' Revision 1.66  2008/08/13 18:58:36  joeb
' Change Effort map legend
'
' Revision 1.65  2008/08/11 04:36:22  jeroens
' Simplified  Ecospace core class names
'
' Revision 1.64  2008/08/08 01:10:27  jeroens
' Simplifying
'
' Revision 1.63  2008/07/02 18:50:42  jeroens
' Plot responds to group show selections
'
' Revision 1.62  2008/06/18 18:28:50  joeb
' Changes setting of Ecospace call back
'
' Revision 1.61  2008/06/02 00:01:34  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.60  2008/05/29 22:23:03  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.59  2008/05/14 01:49:35  jeroens
' Stripped out obsolete bitmaps
'
' Revision 1.58  2008/05/13 18:01:15  jeroens
' Fixed bug 468
'
' Revision 1.57  2008/05/13 17:43:55  jeroens
' Added Conc / B
'
' Revision 1.56  2008/05/13 17:20:09  jeroens
' Revamping for showing contaminants
'
' Revision 1.55  2008/05/07 01:39:06  jeroens
' Fixed bugs 281, 378, 470
'
' Revision 1.54  2008/03/25 14:43:01  jeroens
' Basemap cell vars exposed as Layer*
'
' Revision 1.53  2008/02/29 17:18:40  jeroens
' Single map also drawn by cMapDrawer (removed dual implementation)
'
' Revision 1.52  2008/02/26 19:10:47  joeb
' Removed Ecoseed debug button
'
' Revision 1.51  2008/02/26 18:49:47  joeb
' Fixed bug 380 Ecospace crashes if map resized
'
' Revision 1.50  2008/01/23 17:38:29  joeb
' debugging for seed
'
' Revision 1.49  2008/01/08 20:12:11  joeb
' Added Button for EcoSeed
'
' Revision 1.48  2007/10/18 22:08:59  joeb
' Inherit from EwEForm
'
' Revision 1.47  2007/10/15 20:03:14  joeb
' Removed Core message handler
' Added EwEForm as baseclass
'
' Revision 1.46  2007/10/14 22:02:34  jeroens
' * Updated to styleguide changes
'
' Revision 1.45  2007/10/03 01:54:28  jeroens
' * Reworked styleguide, colormanager
'
' Revision 1.44  2007/09/30 20:52:40  jeroens
' * Renamed resource(s)
'
' Revision 1.43  2007/07/23 17:37:34  jeroens
' - Reverted
'
' Revision 1.41  2007/06/29 23:14:58  jeroens
' + Added overlay checkbox
' * Enlarge plot temporarily changed to big button
'
' Revision 1.40  2007/06/28 23:06:19  willw
' changed some variable names
'
' Revision 1.39  2007/06/28 22:06:04  willw
' added boolean flag to not clear the biomass plots, for an overlay function (needs to come out to interface)
'
' Revision 1.38  2007/06/24 01:36:11  jeroens
' + Localizing
'
' Revision 1.37  2007/06/22 16:55:13  joeb
' Added SS output
'
' Revision 1.36  2007/06/21 18:30:11  willw
' no message
'
' Revision 1.35  2007/06/20 17:47:35  sherman
' Put CVS header
'
'==============================================================================

#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports System.Threading
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region

Namespace Ecospace

    ''' <summary>
    ''' The Run Ecospace UI class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RunEcospace

        Public Enum eShowGroupType
            ShowAll = 0
            ShowNonHidden
            ShowSingle
        End Enum

#Region " Variables "

        'The core reference
        Private m_core As cCore = Nothing

        'The previous number of timesteps UI has drawn
        Private m_iTimeStepPrev As Integer
        ' The current number of timesteps available to draw.
        Private m_iTimeStepCur As Integer

        ' === Timestep and derived values ===
        Private m_dataTimeStep As cEcospaceTimestep = Nothing
        ''' <summary>The array to hold the Ecospace average biomass results.</summary>
        Private m_as2RelBiomassResults(,) As Single
        ''' <summary>The array to hold the Ecospace base biomass results.</summary>
        Private m_asBaseBiomassResults() As Single
        ''' <summary>Contaminants over Biomass.</summary>
        Private m_as2ConcOverB(,,) As Single

        'The ref to hold the ecospace basemap
        ' ToDo: change this to a snapshot layer obtained from time step results
        Private m_BaseMapDepthLayer As cEcospaceLayer

        ' The speed of the plotting. 1 is the slowest, 10 is the fastest
        Private m_iPlotStepSize As Integer

        ' The row and col number of Biomass map plot
        Private m_iNumGroupPlotsVert As Integer, m_iNumGroupPlotsHorz As Integer
        ' The row and col number of Fleet map plot
        Private m_iNumFleetPlotsVert As Integer, m_iNumFleetPlotsHorz As Integer
        'Number of rows and columns if basen
        Private m_iInRow As Integer, m_iInCol As Integer

        Private m_drawers As List(Of cMapDrawer)
        Private m_nMapsPerThread As Integer

        Private m_pdBiomass As cPlotDrawer = Nothing

        Private m_bmpBiomassMap As Bitmap

        'jb added
        Private m_spaceStats As cEcospaceStats

        Private m_bOverlay As Boolean
        Private m_bpConTracing As cBooleanProperty = Nothing

        ''' <summary>Styleguide to listen to.</summary>
        Private m_sg As StyleGuide = StyleGuide.GetInstance()
        Private m_showGroupMode As eShowGroupType = eShowGroupType.ShowAll
        Private m_iGroupToShow As Integer = 0

        'Legend colors for the Effort map
        Private m_nEffortBins As Single = 100 'number of legend bins is arbitrary
        'Exposing m_MaxEffort to the interface would allow the user to set the Effort legend sensitivity
        Private m_MaxEffort As Single = 5 '

#End Region ' Variables

#Region " Construction and Destruction "

        Public Sub New()

            Me.InitializeComponent()

            Me.InitCoreParams()
            Me.InitUIParams()
            Me.InitMapPlot()
            Me.InitDrawingThreads()

        End Sub

        Public Sub New(ByRef text As String)

            'Call the default constructor
            Me.New()

            'Define the text of the content
            Me.Text = text
            'Define the tab text of the content
            Me.TabText = text

        End Sub

#End Region ' Construction and Destruction

#Region " Initialization and Updating "

        Private Sub InitCoreParams()

            ' Get the core reference
            Me.m_core = cCore.GetInstance()

            'Get the basemap
            Me.m_BaseMapDepthLayer = Me.m_core.EcospaceBasemap.LayerDepth

            'Redim relative biomass results array
            ReDim Me.m_as2RelBiomassResults(Me.m_core.nGroups, Me.m_core.nEcospaceTimeSteps)

            'Redim base biomass base result array
            ReDim Me.m_asBaseBiomassResults(Me.m_core.nGroups)

            'get the ecospace stats object from the core
            Me.m_spaceStats = Me.m_core.EcospaceStats

        End Sub

        Private Sub InitUIParams()

            Me.m_iTimeStepCur = 0
            Me.m_iTimeStepPrev = 0

            'Plot speed 1-slowest 10-fastest
            Me.m_iPlotStepSize = 1

            Me.m_btnViewResults.Enabled = False

            Me.m_lbPlotTime.Text = String.Format(My.Resources.ECOSPACE_RUN_PROGRESS, 0, Me.m_core.nEcospaceYears)

            'Load group combo box
            Me.m_cbDisplayGroup.Items.Clear()
            For i As Integer = 1 To Me.m_core.nGroups
                Me.m_cbDisplayGroup.Items.Add(Me.m_core.EcospaceGroups(i).Name)
            Next
            Me.m_cbDisplayGroup.SelectedIndex = 0
            Me.m_iGroupToShow = 0

            Me.m_lblPoolName.Visible = False
            Me.m_lblLargePoolName.Visible = False

        End Sub

        Private Sub InitMapPlot()

            ' Initialization of BioMapPlot

            'Hack warning: For initialization the map dimensions are set to the value supplied by the core's EcoSpaceBaseMap
            'the actual size of the map must be set from the EcoSpace Timestep results(See EcospaceTimeStepDelegate())
            'this should not be call once Ecospace has been run because the map dims can be out of sync
            Me.m_iInCol = Me.m_core.EcospaceBasemap.InCol
            Me.m_iInRow = Me.m_core.EcospaceBasemap.InRow
            'm_Core.nGroups --> updated to nLivingGroups? Non - hidden groups? Check EwE5

            Me.CalcMapDimension(Me.m_core.nGroups, Me.m_iNumGroupPlotsVert, Me.m_iNumGroupPlotsHorz)
            Me.CalcMapDimension(Me.m_core.nFleets, Me.m_iNumFleetPlotsVert, Me.m_iNumFleetPlotsHorz)

            Me.m_pdBiomass = New cPlotDrawer(Me.m_core)

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
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim lColors As List(Of Color) = sg.GetColorRamp(Me.m_core.nGroups)

            Me.m_nMapsPerThread = (Me.m_core.nGroups + nThreads - 1) \ nThreads
            If Me.m_drawers Is Nothing Then
                Me.m_drawers = New List(Of cMapDrawer)
            Else
                Me.m_drawers.Clear()
            End If

            Me.InitOutputBitmaps()

            For i As Integer = 1 To nThreads
                drawer = New cMapDrawer(i, Me.m_core)
                drawer.Graphics = Graphics.FromImage(Me.m_bmpBiomassMap)
                drawer.GroupColors = lColors

                Me.m_drawers.Add(drawer)
            Next
        End Sub

        Private Sub InitOutputBitmaps()
            Me.m_bmpBiomassMap = New Bitmap(Me.m_pbMap.Width, Me.m_pbMap.Height)
            For Each drawer As cMapDrawer In Me.m_drawers
                drawer.Graphics = Graphics.FromImage(m_bmpBiomassMap)
            Next
        End Sub

        Private Sub OnStyleGuideChanged(ByVal changeType As StyleGuide.eChangeType)
            If ((changeType And StyleGuide.eChangeType.Colours) = StyleGuide.eChangeType.Colours) Then
                Me.UpdateStyleColors()
            End If
        End Sub

        Private Sub UpdateStyleColors()
            Me.m_pbMap.BackColor = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.PLOT_BACKGROUND)
            Me.m_pbSmallPlot.BackColor = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.IMAGE_BACKGROUND)
            Me.m_pbLargePlot.BackColor = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.IMAGE_BACKGROUND)
        End Sub

#End Region ' Initialization and Updating

#Region " Events "

        Private Sub RunEcospace_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
            Dim ecospaceModelParams As cEcospaceModelParameters = Me.m_core.EcospaceModelParameters()

            'Start tracking ConcTracing setting
            Me.m_bpConTracing = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.ConSimOnEcoSpace), cBooleanProperty)
            AddHandler Me.m_bpConTracing.PropertyChanged, AddressOf OnPropertyChanged

            ' Start tracking styleguide changes for colour feedback
            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

            Me.MessageSources = New eMessageSource() {eMessageSource.EcoSim, eMessageSource.EcoSpace}

            Me.UpdateStyleColors()
            Me.UpdateControls()

        End Sub

        Private Sub RunEcospace_Disposed(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Disposed

            Me.m_core.StopEcospace()

            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.m_sg = Nothing

            RemoveHandler Me.m_bpConTracing.PropertyChanged, AddressOf OnPropertyChanged
            Me.m_bpConTracing = Nothing

            Me.MessageSources = Nothing
        End Sub

        Private Sub RunEcospace_ResizeEnd(ByVal sender As Object, ByVal e As EventArgs) Handles Me.ResizeEnd
            Me.InitOutputBitmaps()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when either of the two model state properties changes.
        ''' </summary>
        ''' <param name="prop">The property that changed.</param>
        ''' <param name="changeFlags">The extent of the change.</param>
        ''' -------------------------------------------------------------------
        Private Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)
            Me.UpdateControls()
        End Sub

#End Region ' Events 

#Region " Biomass graph "

        Private Sub UpdateBiomassPlot()
            For iGroup As Integer = 1 To m_core.nGroups
                For iTimeStep As Integer = Me.m_iTimeStepPrev To Me.m_iTimeStepCur - 1
                    Me.m_pdBiomass.AddValue(iGroup, Me.m_iTimeStepCur, Me.m_as2RelBiomassResults(iGroup, iTimeStep + 1))
                Next
            Next
        End Sub

        ''' <summary>
        ''' Display the group name when mouse moves over the rendered line
        ''' </summary>
        Private Sub pbBioPlot_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles m_pbSmallPlot.MouseMove
            Me.DisplayPoolName(Me.m_lblPoolName, DirectCast(Me.m_pbSmallPlot.Image, Bitmap), Me.m_pbSmallPlot, e.X, e.Y)
        End Sub

        ''' <summary>
        ''' Display the group name when mouse moves over the rendered line
        ''' </summary>       
        Private Sub pbLargeBioPlot_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles m_pbLargePlot.MouseMove
            Me.DisplayPoolName(m_lblLargePoolName, DirectCast(Me.m_pbLargePlot.Image, Bitmap), Me.m_pbLargePlot, e.X, e.Y)
        End Sub

        Private Sub DisplayPoolName(ByRef lbl As Label, ByRef img As Bitmap, ByRef pb As PictureBox, _
                                    ByVal x As Integer, ByVal y As Integer)

            'Dim imgX As Integer = CInt(x * img.Width / pb.Width)
            'Dim imgY As Integer = CInt(y * img.Height / pb.Height)
            'Dim sg As StyleGuide = StyleGuide.GetInstance()
            'Dim group As cEcoPathGroupInput = Nothing

            'If imgX >= img.Width Or imgY >= img.Height Then lbl.Visible = False : Return

            'Dim clr As Color = img.GetPixel(imgX, imgY)

            'Dim iGroup As Integer = sg.GetGroupColorIndex(Me.m_core, clr)
            'If iGroup <= 0 Then lbl.Visible = False : Return

            'group = Me.m_core.EcoPathGroupInputs(iGroup)

            'lbl.Visible = True
            'lbl.ForeColor = clr
            'lbl.Text = group.Name

        End Sub

        ''' <summary>
        ''' When mouse double click the image, we will save the image.
        ''' </summary>
        Private Sub pbBioPlot_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_pbSmallPlot.DoubleClick
            Me.SaveImage(m_pbSmallPlot.Image)
        End Sub

        ''' <summary>
        ''' When mouse double click the image, we will save the image.
        ''' </summary>
        Private Sub pbLargeBioPlot_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_pbLargePlot.DoubleClick
            Me.SaveImage(m_pbLargePlot.Image)
        End Sub

        Private Sub SaveImage(ByRef img As Image)

            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(My.Resources.FILEFILTER_IMAGE)

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

                img.Save(cmdFS.FileName, imgFormat)
            End If

        End Sub

#End Region ' Biomass graph

#Region " Map plot "

        Private Sub PlotBiomassMapThreaded(ByRef g As Graphics)

            Dim drawer As cMapDrawer = Nothing

            ' Sanity check
            If Me.m_dataTimeStep Is Nothing Then Return

            If Me.m_iTimeStepCur = 1 Then
                'caution: hack)
                Me.m_iNumGroupPlotsHorz = CInt(Math.Ceiling(Math.Sqrt(Me.m_core.nGroups) * Me.m_iInRow / Me.m_iInCol * Me.m_pbMap.Width / Me.m_pbMap.Height))
                Me.m_iNumGroupPlotsVert = CInt(Math.Ceiling(Me.m_core.nGroups / Me.m_iNumGroupPlotsHorz))
            End If

            ' Clear background
            Me.InitOutputBitmaps()

            If Me.m_iTimeStepCur > 0 And (Me.m_showGroupMode <> eShowGroupType.ShowSingle) Then
                Try

                    Dim originList As New List(Of PointF)
                    Dim rectList As New List(Of Rectangle)
                    Dim xScale As Double = m_iNumGroupPlotsHorz * (m_iInCol + 1) + 1
                    Dim yScale As Double = m_iNumGroupPlotsVert * (m_iInRow + 1) + 1
                    If xScale > 0 Then xScale = m_pbMap.Width / xScale
                    If yScale > 0 Then yScale = m_pbMap.Height / yScale

                    For i As Integer = 0 To m_iNumGroupPlotsVert - 1
                        For j As Integer = 0 To m_iNumGroupPlotsHorz - 1
                            Dim iGroup As Integer = i * m_iNumGroupPlotsHorz + j
                            If iGroup < m_core.nGroups Then
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
                    Dim ifirst As Integer = 1
                    Dim ilast As Integer = 0
                    For Each drawer In m_drawers
                        If drawer.AllowedToRun Then
                            'init the drawer to the latest values
                            drawer.OriginList = originList
                            drawer.RectList = rectList
                            drawer.Font = Me.Font

                            If Me.m_rbDisplayRelBiomass.Checked Then
                                drawer.Map = Me.m_dataTimeStep.BiomassMap
                            ElseIf Me.m_rbDisplayContaminantC.Checked Then
                                drawer.Map = Me.m_dataTimeStep.ContaminantMap
                            ElseIf Me.m_rbDisplayCoverB.Checked Then
                                drawer.Map = Me.m_as2ConcOverB
                            End If
                            drawer.InCol = Me.m_iInCol
                            drawer.InRow = Me.m_iInRow

                            ilast = Math.Min(ifirst + Me.m_nMapsPerThread - 1, Me.m_core.nGroups)

                            drawer.GroupFirst = ifirst
                            drawer.GroupLast = ilast

                            drawer.SignalState.Reset()

                            drawer.AllowedToRun = False
                            ThreadPool.QueueUserWorkItem(AddressOf drawer.Draw)

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

            ElseIf (Me.m_showGroupMode = eShowGroupType.ShowSingle) Then

                Dim sg As StyleGuide = StyleGuide.GetInstance()
                Dim lColors As List(Of Color) = sg.GetColorRamp(Me.m_core.nGroups)

                'Show one group at a time
                If (Me.m_iGroupToShow > 0) Then
                    Dim rect As Rectangle = New Rectangle(m_pbMap.Top + 10, _
                                                          m_pbMap.Left + 10, _
                                                          m_pbMap.Width - 20, _
                                                          m_pbMap.Height - 20)

                    drawer = New cMapDrawer(0, Me.m_core)
                    With drawer
                        If Me.m_rbDisplayRelBiomass.Checked Then
                            .Map = Me.m_dataTimeStep.BiomassMap
                        ElseIf Me.m_rbDisplayContaminantC.Checked Then
                            .Map = Me.m_dataTimeStep.ContaminantMap
                        ElseIf Me.m_rbDisplayCoverB.Checked Then
                            drawer.Map = Me.m_as2ConcOverB
                        End If
                        .InRow = Me.m_iInRow
                        .InCol = Me.m_iInCol
                        .GroupColors = lColors
                        .Font = Me.Font
                        .Graphics = g
                    End With
                    drawer.DrawBiomassBaseMap(Me.m_iGroupToShow, rect)
                End If
            End If

        End Sub

        Private Sub pbMapPlot_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles m_pbMap.Paint

            Dim g As Graphics = e.Graphics

            If m_rbDisplayFishingEffort.Checked Then
                PlotFishingEffortMap(g)
            Else
                PlotBiomassMapThreaded(g)
            End If

        End Sub

        Private Sub PlotFishingEffortMap(ByRef g As Graphics)

            If m_iTimeStepCur > 0 Then

                Dim xScale As Double = m_iNumFleetPlotsHorz * (m_iInCol + 1) + 1
                Dim yScale As Double = m_iNumFleetPlotsVert * (m_iInRow + 1) + 1
                If xScale > 0 Then xScale = m_pbMap.Width / xScale
                If yScale > 0 Then yScale = m_pbMap.Height / yScale

                For i As Integer = 0 To m_iNumFleetPlotsVert - 1
                    For j As Integer = 0 To m_iNumFleetPlotsHorz - 1
                        Dim cur As Integer = i * m_iNumFleetPlotsHorz + j
                        If cur < m_core.nFleets Then
                            Dim origin As PointF = New PointF((m_iInCol + 1) * j + 1, i * (m_iInRow + 1) + 1)
                            Dim rect As Rectangle = New Rectangle(CInt(origin.X * xScale), _
                                                                    CInt(origin.Y * yScale), _
                                                                    CInt(m_iInCol * xScale), _
                                                                    CInt(m_iInRow * yScale))
                            DrawFishingBaseMap(Me.m_dataTimeStep.FishingEffortMap, cur, rect, g)
                        End If
                    Next
                Next

            End If

        End Sub

        Private Sub DrawFishingBaseMap(ByRef baseMap(,,) As Single, ByVal iFleet As Integer, ByVal rcPos As Rectangle, ByRef g As Graphics)

            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim lColors As List(Of Color) = sg.GetColorRamp(CInt(Me.m_nEffortBins))
            Dim cScaler As Single = Me.m_nEffortBins / Me.m_MaxEffort

            For i As Integer = 1 To m_iInRow
                For j As Integer = 1 To m_iInCol
                    Dim icc As Single

                    icc = baseMap(iFleet + 1, i, j) * cScaler

                    'Boundary check
                    icc = Math.Max(Math.Min(Me.m_nEffortBins, icc), 0)

                    Dim tmpBrush As SolidBrush = Nothing
                    'If it is land
                    If CInt(m_BaseMapDepthLayer.Cell(i, j)) = 0 Then
                        tmpBrush = New SolidBrush(Color.Black)
                    Else
                        tmpBrush = New SolidBrush(lColors(CInt(icc)))
                    End If

                    Dim tmpRect As RectangleF = New RectangleF(CSng(rcPos.Left + (j - 1) * rcPos.Width() / m_iInCol), _
                                            CSng(rcPos.Top + (i - 1) * rcPos.Height() / m_iInRow), _
                                            CSng(rcPos.Width() / m_iInCol), _
                                            CSng(rcPos.Height() / m_iInRow))
                    g.FillRectangle(tmpBrush, tmpRect)
                    tmpBrush.Dispose()
                Next
            Next

            'Draw the black frame of base map
            g.DrawRectangle(Pens.Black, rcPos)

            'Display the group name
            Dim fltName As String = m_core.EcospaceFleets(iFleet + 1).Name
            g.DrawString(fltName, Me.Font, Brushes.Black, rcPos)

        End Sub

#End Region ' Map plot

#Region " Color chart "

        Private Sub pbColors_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles m_pbColors.Paint

            Dim g As Graphics = e.Graphics
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim lColors As List(Of Color) = sg.GetColorRamp(Me.m_core.nGroups)
            Dim sHeight As Single = CSng(Me.m_pbColors.Height / Me.m_core.nGroups)
            Dim brTmp As SolidBrush = Nothing
            For i As Integer = 1 To Me.m_core.nGroups
                brTmp = New SolidBrush(lColors(i))
                g.FillRectangle(brTmp, 0, m_pbColors.Height - sHeight * i, m_pbColors.Width, sHeight)
                brTmp.Dispose()
            Next

        End Sub

#End Region ' Color chart

#Region " Events "

        Private Sub btnRun_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnRun.Click

            'Redim relative biomass results array
            ReDim m_as2RelBiomassResults(m_core.nGroups, Me.m_core.nEcospaceTimeSteps)
            'Redim base biomass base result array
            ReDim m_asBaseBiomassResults(m_core.nGroups)

            ' Reset plot drawer if overlay is not needed
            If Me.m_bOverlay = False Then
                Me.m_pdBiomass.Reset(Me.m_core.nGroups, Me.m_core.nEcospaceTimeSteps)
            Else
                Me.m_pdBiomass.Overlay(Me.m_core.nGroups)
            End If

            Me.m_iTimeStepCur = 0

            Me.m_core.RunEcoSpace(AddressOf onEcospaceTimeStep)

            Me.m_cbOverlay.Enabled = True

        End Sub

        Private Sub m_btnStop_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnStop.Click
            Me.m_core.StopEcospace()
            Me.UpdateControls()
        End Sub

        Private Sub btnResults_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnViewResults.Click
            Dim results As New EcospaceResults
            results.ShowDialog()
        End Sub

        Private Sub OnOverlay(ByVal sender As Object, ByVal e As EventArgs) Handles m_cbOverlay.Click
            Me.m_bOverlay = m_cbOverlay.Checked
        End Sub

        Private Sub m_cbDisplayGroup_GotFocus(ByVal sender As Object, ByVal e As EventArgs) _
                Handles m_cbDisplayGroup.GotFocus
            Me.m_showGroupMode = eShowGroupType.ShowSingle
            Me.m_iGroupToShow = Me.m_cbDisplayGroup.SelectedIndex + 1
            Me.UpdateControls()
            Me.m_pbMap.Refresh()
            Me.m_pbLargePlot.Refresh()
            Me.m_pbSmallPlot.Refresh()
        End Sub

        Private Sub cbGroups_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
                Handles m_cbDisplayGroup.SelectedIndexChanged

            If (Me.m_showGroupMode = eShowGroupType.ShowSingle) Then
                Me.m_iGroupToShow = Me.m_cbDisplayGroup.SelectedIndex + 1
                Me.m_pbMap.Refresh()
                Me.m_pbLargePlot.Refresh()
                Me.m_pbSmallPlot.Refresh()
            End If
        End Sub

        Private Sub rbDisplayOption_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
                Handles m_rbShowAll.CheckedChanged, m_rbShowNonHidden.CheckedChanged, m_rbShowSingle.CheckedChanged

            If Me.m_rbShowAll.Checked Then Me.m_showGroupMode = eShowGroupType.ShowAll
            If Me.m_rbShowNonHidden.Checked Then Me.m_showGroupMode = eShowGroupType.ShowNonHidden
            If Me.m_rbShowSingle.Checked Then Me.m_showGroupMode = eShowGroupType.ShowSingle
            Me.UpdateControls()

            Me.m_pbMap.Refresh()
            Me.m_pbLargePlot.Refresh()
            Me.m_pbSmallPlot.Refresh()
        End Sub

        Private Sub rbShowMapOption_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
                Handles m_rbDisplayRelBiomass.CheckedChanged, m_rbDisplayFishingEffort.CheckedChanged, m_rbDisplayContaminantC.CheckedChanged, m_rbDisplayCoverB.CheckedChanged
            Me.UpdateControls()
            Me.m_pbMap.Refresh()
        End Sub

        Private Sub m_pbLargePlot_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) _
                Handles m_pbLargePlot.Paint
            Me.m_pdBiomass.Draw(e.Graphics, Me.m_pbLargePlot.ClientRectangle)
        End Sub

        Private Sub m_pbSmallPlot_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) _
                Handles m_pbSmallPlot.Paint

            Me.m_pdBiomass.GroupShowMode = Me.m_showGroupMode
            Me.m_pdBiomass.GroupToShow = Me.m_iGroupToShow
            Me.m_pdBiomass.Draw(e.Graphics, Me.m_pbSmallPlot.ClientRectangle)

        End Sub

#End Region ' Events

#Region " Ecospace Events/Delegates "

        ''' <summary>
        ''' This GUI event handler will be called for every time step of the Ecospace model run. 
        ''' </summary>
        ''' <param name="dataTimeStep">Data from the current time step</param>
        ''' <remarks></remarks>
        Private Sub onEcospaceTimeStep(ByRef dataTimeStep As cEcospaceTimestep)

            ' Biomass plotting
            ' For each time step, we get the Biomass from the core and store it into our array
            ' The following algorithm was extracted from EwE5. Biomass Log plotting, the value between 0.1 to 10. 
            For groupIndex As Integer = 1 To m_core.nGroups
                If dataTimeStep.iTimeStep = 1 Then
                    m_asBaseBiomassResults(groupIndex) = dataTimeStep.RelativeBiomass(groupIndex)
                    m_as2RelBiomassResults(groupIndex, 1) = 0
                Else
                    m_as2RelBiomassResults(groupIndex, dataTimeStep.iTimeStep) = dataTimeStep.RelativeBiomass(groupIndex)
                    If dataTimeStep.RelativeBiomass(groupIndex) < 0.1 * m_asBaseBiomassResults(groupIndex) Then
                        m_as2RelBiomassResults(groupIndex, dataTimeStep.iTimeStep) = CSng(Math.Log10(0.1))
                    ElseIf m_as2RelBiomassResults(groupIndex, dataTimeStep.iTimeStep) > 10 * m_asBaseBiomassResults(groupIndex) Then
                        m_as2RelBiomassResults(groupIndex, dataTimeStep.iTimeStep) = CSng(Math.Log10(10))
                    Else
                        m_as2RelBiomassResults(groupIndex, dataTimeStep.iTimeStep) = CSng(Math.Log10(dataTimeStep.RelativeBiomass(groupIndex) / m_asBaseBiomassResults(groupIndex)))
                    End If
                End If
            Next

            'Temporary variables to store the timesteps for plotting. 
            m_iTimeStepPrev = m_iTimeStepCur
            m_iTimeStepCur = dataTimeStep.iTimeStep

            'Update the running simulation years progress label.
            ' ToDo_JS: Localize this
            m_lbPlotTime.Text = String.Format("{0:f2} of {1} years", m_iTimeStepCur * m_core.nEcospaceYears / m_core.nEcospaceTimeSteps, m_core.nEcospaceYears)

            ' Store time step data
            Me.m_dataTimeStep = dataTimeStep

            ' Calc C/B
            If (dataTimeStep.ContaminantMap IsNot Nothing) Then
                ReDim Me.m_as2ConcOverB(dataTimeStep.inRows, dataTimeStep.inCols, Me.m_core.nGroups)
                For iRow As Integer = 1 To dataTimeStep.inRows
                    For iCol As Integer = 1 To dataTimeStep.inCols
                        For iGroup As Integer = 1 To Me.m_core.nGroups
                            Dim sB As Single = dataTimeStep.BiomassMap(iRow, iCol, iGroup)
                            If (sB > 0) Then
                                Me.m_as2ConcOverB(iRow, iCol, iGroup) = dataTimeStep.ContaminantMap(iRow, iCol, iGroup) / sB
                            End If
                        Next iGroup
                    Next iCol
                Next iRow
            End If

            'if the size of the map has changed reset the interface
            If m_iInRow <> dataTimeStep.inRows Or m_iInCol <> dataTimeStep.inCols Then
                'set the map dims these are passed to the drawing threads in PlotBiomassMapThreaded()
                m_iInRow = dataTimeStep.inRows
                m_iInCol = dataTimeStep.inCols

                CalcMapDimension(m_core.nGroups, m_iNumGroupPlotsVert, m_iNumGroupPlotsHorz)
                CalcMapDimension(m_core.nFleets, m_iNumFleetPlotsVert, m_iNumFleetPlotsHorz)
            End If


            Me.UpdateBiomassPlot()

            m_pbMap.Invalidate()
            m_pbLargePlot.Invalidate()
            Me.m_pbSmallPlot.Invalidate()

            Me.UpdateControls()

            Application.DoEvents()

        End Sub

#End Region ' Ecospace Delegates

#Region " Overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

            Select Case msg.Type

                Case eMessageType.EcospaceRunCompleted
                    'the Ecospace run has completed
                    'this message will be sent before RunEcospace has returned!!!!
                    Me.UpdateControls()

                Case eMessageType.EcosimNYearsChanged
                    Me.InitUIParams()

                Case eMessageType.ErrorEncountered
                    'Console.WriteLine("Ecospace Error: " & msg.Message)

            End Select

        End Sub

#End Region ' Overrides

#Region " Internal implementation "

        Private Sub UpdateControls()

            'Sanity check
            If Me.m_core Is Nothing Then Return

            Dim csm As cCoreStateMonitor = Me.m_core.StateMonitor
            ' Enable run and stop buttons based on Ecospace run state
            Me.m_btnRun.Enabled = (csm.HasEcospaceLoaded = True) And (csm.IsEcospaceRunning = False)
            Me.m_btnStop.Enabled = (csm.HasEcospaceLoaded = True) And (csm.IsEcospaceRunning = True)
            ' Enable results button based on Ecospace run state
            Me.m_btnViewResults.Enabled = (csm.HasEcospaceLoaded = True) And (csm.HasEcospaceRan = True)
            ' Enable display options for non-fleet maps
            Me.m_gpbDisplayOptions.Enabled = (m_rbDisplayFishingEffort.Checked = False)

            ' Enable contaminant options based on space tracer enabled state
            Me.m_rbDisplayContaminantC.Enabled = CBool(Me.m_bpConTracing.GetValue())
            Me.m_rbDisplayCoverB.Enabled = CBool(Me.m_bpConTracing.GetValue())

            Select Case Me.m_showGroupMode
                Case eShowGroupType.ShowAll
                    Me.m_rbShowAll.Checked = True
                Case eShowGroupType.ShowNonHidden
                    Me.m_rbShowNonHidden.Checked = True
                Case eShowGroupType.ShowSingle
                    Me.m_rbShowSingle.Checked = True

            End Select
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
