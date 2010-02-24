#Region " Imports "

Option Strict On
Imports EwECore
Imports ZedGraph
Imports ScientificInterface.Other
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmEcotracerOutput

#Region " Definitions "

    ''' <summary>
    ''' 
    ''' </summary>
    Private Enum eDisplayModeTypes As Byte
        NotInitialized 'no mode has been set yet this make it easier to init the form
        NoResults '
        Ecosim
        Ecospace
    End Enum

    Private Enum ePlotTypes As Byte
        Conc
        CB
    End Enum

#End Region ' Public definitions

#Region " Private vars "

    ''' <summary></summary>
    Private m_core As cCore = Nothing
    ''' <summary></summary>
    Private m_zgh As cZedGraphHelper = Nothing
    ''' <summary></summary>
    Private m_curDisplayMode As eDisplayModeTypes = eDisplayModeTypes.NotInitialized
    ''' <summary></summary>
    Private m_sg As cStyleGuide = Nothing

    ''' <summary></summary>
    Private m_asScaling() As Single

    Private m_DisplayHelper As IDisplayModeHelper

    Private m_sortOrder() As Integer
    Private m_bSorted As Boolean
    Private m_plottype As ePlotTypes = ePlotTypes.CB

#End Region ' Private vars

#Region " Events "

    Private Sub frmEcotracerOutput_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated

        'build the IDisplayModeHelper and populate the interface
        Me.RefreshData()

    End Sub

    Private Sub frmEcotracerOutput_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.m_core = cCore.GetInstance()
        Me.m_sg = cStyleGuide.GetInstance()
        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.UIContext, Me.m_zgc)

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace}

        AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
    End Sub

    Private Sub frmEcotracerOutput_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
        Me.m_sg = Nothing

        Me.m_zgh.Detach()
        Me.m_zgh = Nothing

        Me.m_core = Nothing
    End Sub

    Private Sub lbGroups_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbGroups.SelectedIndexChanged
        PlotGroup()
    End Sub

    Private Sub lbGroups_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lbGroups.MouseMove
        Dim iHover As Integer = lbGroups.IndexFromPoint(e.Location)
        Dim bDraw As Boolean = True

        ' Brutal to check all the index... has to be a way to short cut this.
        For Each i As Integer In lbGroups.SelectedIndices
            If i = iHover Then
                bDraw = False
            End If
        Next

        ' Yes, now you can draw me, yes -1 means nothing hovering
        If bDraw And iHover <> -1 Then
            PlotGroup(iHover)
        Else
            PlotGroup()
        End If
    End Sub

    Private Sub lbGroups_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbGroups.MouseLeave
        PlotGroup()
    End Sub

    Private Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)
        If ((changeType And cStyleGuide.eChangeType.Colours) > 0) Then
            ' Respond to group colour changes
            Me.PlotGroup()
            ' Invalidate group list box
            Me.lbGroups.Invalidate()
        End If
    End Sub


    Private Sub btRunSim_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRunSim.Click

        Try
            'An Ecosim scenario was loaded when this form was loaded
            'so there is no need to check
            m_core.EcoSimModelParameters.ContaminantTracing = True
            Me.startModelRun()
            m_core.RunEcoSim(AddressOf Me.ecosimCallback)
            Me.RefreshGraph()

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".btRunSim_Click() Error: " & ex.Message)
        End Try

    End Sub

    Private Sub btRunSpace_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRunSpace.Click

        Try

            'No Ecospace scenario has been load
            If Me.m_core.StateMonitor.HasEcospaceLoaded = False Then
                'Ask the user for a Ecospace scenario via the command
                Dim cmd As cCommand = cCommandHandler.GetInstance().GetCommand("LoadEcospaceScenario")
                Debug.Assert(cmd IsNot Nothing, Me.ToString & ".btRunSpace_Click() LoadEcospaceScenario Command could not be found.")
                cmd.Invoke()
            End If

            'Make sure the scenario loaded successfully before trying to run Ecospace
            If Me.m_core.StateMonitor.HasEcospaceLoaded Then
                m_core.EcospaceModelParameters.ContaminantTracing = True
                Me.startModelRun()
                m_core.RunEcoSpace(AddressOf Me.EcospaceCallback)
                Me.RefreshGraph()
            End If

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".btRunSpace_Click() Error: " & ex.Message)
        End Try

    End Sub


    Private Sub rbConc_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbConc.CheckedChanged

        If m_DisplayHelper Is Nothing Then Exit Sub
        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        If rb.Checked Then
            Me.m_plottype = ePlotTypes.Conc
            If Me.m_bSorted Then Me.RefreshData()
            Me.RefreshGraph()
        End If

    End Sub

    Private Sub rbCB_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbCB.CheckedChanged

        If m_DisplayHelper Is Nothing Then Exit Sub

        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        If rb.Checked Then
            Me.m_plottype = ePlotTypes.CB
            If Me.m_bSorted Then Me.RefreshData()
            Me.RefreshGraph()
        End If

    End Sub


    Private Sub cmbRegions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbRegions.SelectedIndexChanged
        Me.RefreshGraph()
    End Sub

    Private Sub ckSorted_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ckSorted.Click
        Me.RefreshData()
    End Sub

    Private Sub OnDisplayGroups(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnShowHideGroups.Click
        Dim cmd As cCommand = cCommandHandler.GetInstance().GetCommand("DisplayGroups")
        Debug.Assert(cmd IsNot Nothing, Me.ToString & ".OnDisplayGroups() DisplayGroups Command could not be found.")
        cmd.Invoke()
    End Sub

#End Region ' Events

#Region " Internal bits "

    ''' <summary>
    ''' Start a model run Ecosim or Ecospace
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub startModelRun()

        'clear out the progress bar
        Me.pbProgress.Value = 0
        Me.pbProgress.Maximum = Me.m_DisplayHelper.nYears
        Me.pbProgress.Step = 1

    End Sub

    ''' <summary>
    ''' Update the progress bar in response to a model time step
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateProgess()

        Try
            Me.pbProgress.PerformStep()
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".UpdateProgess() Error.")
        End Try

    End Sub

    Private Function getDisplayMode() As eDisplayModeTypes
        Dim dmode As eDisplayModeTypes

        dmode = eDisplayModeTypes.NoResults

        'Ecosim selected
        If m_core.EcoSimModelParameters.ContaminantTracing Then
            dmode = eDisplayModeTypes.Ecosim
        End If

        'Ecospace
        'this is nested because EcospaceModelParameters will be Null if an Ecospace scenario has not been loaded
        If m_core.StateMonitor.HasEcospaceLoaded Then
            If m_core.EcospaceModelParameters.ContaminantTracing Then
                dmode = eDisplayModeTypes.Ecospace
            End If
        End If

        Return dmode

    End Function


    ''' <summary>
    ''' Re-populates the interface 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RefreshData()
        Dim newDMode As eDisplayModeTypes

        newDMode = Me.getDisplayMode

        'build the correct display mode helper based on the new display mode flag from getDisplayMode
        Me.m_DisplayHelper = Me.DisplayHelperFactory(newDMode)

        'refresh the display mode helper
        'if no new displayhelper was built this will refresh the current one based on the core state
        Me.m_DisplayHelper.Refresh()

        'keep the display mode for next time 
        'it is used in DisplayHelperFactory()
        Me.m_curDisplayMode = newDMode

        'get the sort order for the groups list box
        Me.CalcSortOrder()

        ' Populate the list box
        Me.UpdateGroups()

        Me.UpdateRegions()

        ' Config controls based on the display helper
        Me.m_zgc.GraphPane.Title.Text = m_DisplayHelper.Title
        Me.m_zgc.Enabled = m_DisplayHelper.Enabled
        Me.lbGroups.Enabled = m_DisplayHelper.Enabled
        Me.cmbRegions.Enabled = m_DisplayHelper.EnabledForSpace

        'This is kind of crude. Reset the progress bar to zero
        startModelRun()

        Me.Refresh()

    End Sub

    ''' <summary>
    ''' Redraw the graph
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RefreshGraph()

        'Set values in the display helper
        'plot type
        Me.m_DisplayHelper.PlotType = Me.m_plottype
        'region to display 
        Me.m_DisplayHelper.RegionIndex = Me.cmbRegions.SelectedIndex

        'Now get data from the display helper
        'Text for graph
        Me.m_zgc.GraphPane.Title.Text = m_DisplayHelper.Title
        Me.m_zgc.GraphPane.XAxis.Title.Text = m_DisplayHelper.XAxisLabel
        Me.m_zgc.GraphPane.YAxis.Title.Text = m_DisplayHelper.YAxisLabel

        'scale of graph
        Me.m_zgc.GraphPane.XAxis.Scale.Min = CDbl(Me.m_core.EcosimFirstYear)
        Me.m_zgc.GraphPane.XAxis.Scale.Max = CDbl(Me.m_core.EcosimFirstYear + m_DisplayHelper.nYears)

        'plot the data
        Me.PlotGroup()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="iGroupSelected"></param>
    ''' -----------------------------------------------------------------------
    Private Sub PlotGroup(Optional ByVal iGroupSelected As Integer = cCore.NULL_VALUE)

        Dim lines As List(Of LineItem)

        ' ToDo_JS: validate tracer run status. This needs extending the core state monitor
        Try

            If iGroupSelected = cCore.NULL_VALUE And lbGroups.SelectedIndices.Count = 0 Then
                'nothing to draw
                Exit Sub
            End If

            'can the display helper plot 
            If Me.m_DisplayHelper.bCanPlot Then

                ' If not forcing to draw a single item draw all selected
                If iGroupSelected = cCore.NULL_VALUE Then
                    lines = Me.m_DisplayHelper.GetGroupLines(lbGroups.SelectedItems)
                Else
                    ' Forced to draw a single item
                    lines = Me.m_DisplayHelper.GetGroupLines(Me.m_sortOrder(iGroupSelected))
                End If

                Debug.Assert(lines IsNot Nothing, Me.ToString, ".PlotGroup() Me.m_DisplayHelper.GetGroupLines() failed!")

                m_zgh.PlotLines(lines.ToArray)

            End If 'If Me.m_DisplayHelper.bCanPlot Then

        Catch ex As Exception
            EwECore.cLog.Write(ex)
        End Try

    End Sub

    Private Sub EcosimCallback(ByVal iTime As Long, ByVal data As cEcoSimResults)
        'Ecosim callback()
        UpdateProgess()
    End Sub

    Private Sub EcospaceCallback(ByRef EcospaceResults As cEcospaceTimestep)
        'Ecospace callback()
        UpdateProgess()
    End Sub


    Private Sub UpdateRegions()

        Me.cmbRegions.Items.Clear()

        If Me.m_DisplayHelper.EnabledForSpace Then
            'only populate the region list if space is enabled
            Me.cmbRegions.Items.Add("Undefined area")
            For irgn As Integer = 1 To m_core.nRegions
                Me.cmbRegions.Items.Add("region " & irgn) ' m_core.EcospaceRegions(irgn).Name)
            Next
            Me.cmbRegions.Items.Add("All Regions")

            Me.cmbRegions.SelectedIndex = m_core.nRegions + 1
        End If

    End Sub


    Private Sub CalcSortOrder()
        Dim maxVal As Single, iSort As Integer, gMax As Single
        Dim GrpTaken() As Boolean
        Dim ngrps As Integer = m_core.nGroups

        ReDim Me.m_sortOrder(ngrps)
        ReDim GrpTaken(ngrps)

        'get the sorted checked state
        Me.m_bSorted = Me.ckSorted.Checked

        If m_bSorted Then
            'sort into m_sortOrder() according to Max of group from the current displayhelper
            For igrp As Integer = 1 To ngrps
                maxVal = -1
                For jgrps As Integer = 1 To ngrps
                    gMax = Me.m_DisplayHelper.GetGroupMax(jgrps)
                    If gMax > maxVal And GrpTaken(jgrps) = False Then
                        maxVal = gMax
                        iSort = jgrps
                    End If
                Next

                m_sortOrder(igrp) = iSort
                GrpTaken(iSort) = True
            Next

        Else 'If m_bSorted Then

            'not sorted just display in default order
            For igrp As Integer = 1 To ngrps
                m_sortOrder(igrp) = igrp
            Next

        End If 'If m_bSorted Then

    End Sub

#Region " Group listbox handling "

    Private Sub UpdateGroups()

        lbGroups.Items.Clear()

        'Add "All groups" at the top
        lbGroups.Items.Add(0)

        'For i As Integer = 0 To m_core.nGroups - 1
        '    lbGroups.Items.Add(i + 1)
        'Next

        For i As Integer = 1 To m_core.nGroups
            lbGroups.Items.Add(Me.m_sortOrder(i))
        Next


    End Sub

    Private Sub lbGroups_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles lbGroups.DrawItem
        ' get the sender of this event
        Dim s As ListBox = CType(sender, ListBox)
        Dim iGroup As Integer = 0
        Dim strItemText As String = ""
        Dim clr As Color = Nothing
        Dim rect As Rectangle = Nothing

        If s Is Nothing Then Return
        If e.Index = -1 Then Return

        Try
            'The rectangle to draw the color box
            rect = New Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Height * 2, e.Bounds.Height - 4)
            iGroup = CInt(s.Items(e.Index))

            ' Sanity check
            If iGroup <= Me.m_core.nGroups Then
                If iGroup = 0 Then
                    strItemText = My.Resources.HEADER_ENVIRONMENT
                    clr = Color.Black
                Else
                    Dim group As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iGroup)
                    strItemText = group.Name
                    clr = cStyleGuide.GetInstance().GroupColor(Me.m_core, iGroup)
                End If
            Else
                strItemText = "" ' Deleted
                clr = Color.Gray
            End If

            Me.DrawCustomItem(e, clr, strItemText, rect)

        Catch ex As Exception
            Debug.Assert(False)
            Return
        End Try
    End Sub

    ''' <summary>
    ''' Helper methods to draw a custom listcontrol item 
    ''' </summary>
    ''' <param name="e">DrawItemEventArgs sent by DrawItem event handler</param>
    ''' <param name="clr">The colorbox's color</param>
    ''' <param name="txt">The text beside the colorbox</param>
    ''' <remarks>This method is called by both Listbox and Combobox drawItem event handlers</remarks>
    Private Sub DrawCustomItem(ByVal e As System.Windows.Forms.DrawItemEventArgs, _
                                ByVal clr As Color, _
                                ByRef txt As String, _
                                ByRef rect As Rectangle)


        ' Do nothing if there is no data
        If e.Index = -1 Then Return

        'If the item is selected, draw the correct background color
        e.DrawBackground()
        e.DrawFocusRectangle()

        'Get the listbox's graphics object
        Dim g As Graphics = e.Graphics

        'Draw color box
        g.FillRectangle(New SolidBrush(clr), rect)
        g.DrawRectangle(Pens.Black, rect)
        'Draw text 
        g.DrawString(txt, e.Font, New SolidBrush(e.ForeColor), _
                        New RectangleF(e.Bounds.X + rect.Width + 4, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height))

    End Sub

#End Region

#Region " Helper methods "

    ''' <summary>
    ''' Get a Display mode helper object based on the newDisplayMode parameter
    ''' </summary>
    ''' <param name="newDisplayMode"></param>
    ''' <returns>If the current display mode matches the newDisplayMode parameter this will return the current IDisplayModeHelper object</returns>
    ''' <remarks></remarks>
    Private Function DisplayHelperFactory(ByVal newDisplayMode As eDisplayModeTypes) As IDisplayModeHelper

        'This will only build a new IDisplayModeHelper if newDisplayMode is different from the current m_curDisplayMode
        If newDisplayMode <> Me.m_curDisplayMode Then

            'build a new IDisplayModeHelper object
            Select Case newDisplayMode
                Case eDisplayModeTypes.NoResults
                    Return New cNoResultsDisplayHelper(m_core)
                Case eDisplayModeTypes.Ecosim
                    Return New cEcoSimDisplayHelper(m_core)
                Case eDisplayModeTypes.Ecospace
                    Return New cEcoSpaceDisplayHelper(m_core)
            End Select

            'something went wrong
            'the arg DisplayMode was not valid return the cNoResultsDisplayHelper object 
            'this will let the interface run without data
            Debug.Assert(False, "DisplayHelperFactory() Invalid DisplayMode")
            Return New cNoResultsDisplayHelper(m_core)

        Else
            'return the current IDisplayModeHelper object
            'make sure there is one
            Debug.Assert(m_DisplayHelper IsNot Nothing, Me.ToString & ".DisplayHelperFactory() Current display mode has not been set! Something is wrong!")
            Return Me.m_DisplayHelper
        End If

    End Function

    'Private Sub CalculateScaling()

    '    Dim sMax As Single
    '    ReDim m_asScaling(Me.m_core.nGroups + 1)

    '    For iGroup As Integer = 0 To Me.m_core.nGroups ' + 1

    '        sMax = Me.m_DisplayHelper.GetGroupMax(iGroup)

    '        'scaling maybe .00013
    '        If sMax < 1 Then
    '            For K As Integer = 0 To 10
    '                If sMax * 10 ^ K > 1 Then m_asScaling(iGroup) = K : Exit For
    '            Next
    '        Else    '>=1
    '            For K As Integer = -10 To 0
    '                If sMax * 10 ^ K > 1 Then m_asScaling(iGroup) = K : Exit For
    '            Next
    '        End If
    '    Next
    'End Sub

#End Region ' Helper methods

#End Region ' Internal bits

#Region " Overrides "

    Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

        If msg.Source = eCoreComponentType.EcoSim Or msg.Source = eCoreComponentType.EcoSpace Then
            Me.RefreshData()
        End If

    End Sub

#End Region ' Overrides

#Region "Display Mode Helper Classes"

#Region "Interface definition"

    Private Interface IDisplayModeHelper

        ''' <summary>
        ''' Get the line(s) to draw on the graph
        ''' </summary>
        ''' <param name="iGroup"></param>
        Function GetGroupLines(ByVal iGroup As Integer) As System.Collections.Generic.List(Of ZedGraph.LineItem)
        Function GetGroupLines(ByVal lstGroups As System.Windows.Forms.ListBox.SelectedObjectCollection) As System.Collections.Generic.List(Of ZedGraph.LineItem)

        Function GetGroupMax(ByVal iGroup As Integer) As Single

        ''' <summary>
        ''' Max value of the current lines
        ''' </summary>
        Function Max() As Single

        ''' <summary>
        ''' Update the object base on the current core run state
        ''' </summary>
        Sub Refresh()

        ''' <summary>
        ''' Title of the Graph
        ''' </summary>
        ReadOnly Property Title() As String

        ReadOnly Property XAxisLabel() As String
        ReadOnly Property YAxisLabel() As String

        ReadOnly Property bCanPlot() As Boolean

        ''' <summary>
        ''' Is this helper enabled
        ''' </summary>
        ReadOnly Property Enabled() As Boolean

        ''' <summary>
        ''' Enabled specific to Ecospace
        ''' </summary>
        ReadOnly Property EnabledForSpace() As Boolean

        ''' <summary>
        ''' Number of years the current model has run for
        ''' </summary>
        ReadOnly Property nYears() As Integer

        ''' <summary>
        ''' Type of Plot Concentration or Concentration / Biomass
        ''' </summary>
        Property PlotType() As ePlotTypes

        ''' <summary>
        ''' Region to display, nRegions + 1 is all regions, Zero is undefined area
        ''' </summary>
        WriteOnly Property RegionIndex() As Integer


    End Interface

#End Region

#Region "No Results implementation"

    Private Class cNoResultsDisplayHelper
        Implements IDisplayModeHelper

        Private m_core As cCore

        Sub New(ByRef theCore As cCore)
            m_core = theCore
        End Sub


        Public Function GetGroupLines(ByVal iGroup As Integer) As System.Collections.Generic.List(Of ZedGraph.LineItem) Implements IDisplayModeHelper.GetGroupLines
            Debug.Assert(False, Me.ToString & ".GetGroupLines() Warning this should not be called!")
            Return Nothing
        End Function

        Public Function GetGroupLines(ByVal lstGroups As System.Windows.Forms.ListBox.SelectedObjectCollection) As System.Collections.Generic.List(Of ZedGraph.LineItem) Implements IDisplayModeHelper.GetGroupLines
            Debug.Assert(False, Me.ToString & ".GetGroupLines() Warning this should not be called!")
            Return Nothing
        End Function


        Public Function GetGroupMax(ByVal iGroup As Integer) As Single Implements IDisplayModeHelper.GetGroupMax

            Return 0.0

        End Function

        Public ReadOnly Property Enabled() As Boolean Implements IDisplayModeHelper.Enabled
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Title() As String Implements IDisplayModeHelper.Title
            Get
                Return "No data available."
            End Get
        End Property

        Public Sub Refresh() Implements IDisplayModeHelper.Refresh

        End Sub

        Public ReadOnly Property nYears() As Integer Implements IDisplayModeHelper.nYears
            Get
                Return 1
            End Get
        End Property

        Public Property PlotType() As ePlotTypes Implements IDisplayModeHelper.PlotType
            Get
                Return ePlotTypes.Conc
            End Get
            Set(ByVal value As ePlotTypes)

            End Set
        End Property

        Public WriteOnly Property RegionIndex() As Integer Implements IDisplayModeHelper.RegionIndex
            Set(ByVal value As Integer)

            End Set
        End Property

        Public ReadOnly Property EnabledForSpace() As Boolean Implements IDisplayModeHelper.EnabledForSpace
            Get
                Return False
            End Get
        End Property

        Public Function Max() As Single Implements IDisplayModeHelper.Max
            Return 0.0
        End Function

        Public ReadOnly Property XAxisLabel() As String Implements IDisplayModeHelper.XAxisLabel
            Get
                Return "X Axis"
            End Get
        End Property

        Public ReadOnly Property YAxisLabel() As String Implements IDisplayModeHelper.YAxisLabel
            Get
                Return "Y Axis"
            End Get
        End Property

        Public ReadOnly Property bCanPlot() As Boolean Implements IDisplayModeHelper.bCanPlot
            Get
                Return False
            End Get
        End Property
    End Class

#End Region

#Region "Ecosim implementation"

    Private Class cEcoSimDisplayHelper
        Implements IDisplayModeHelper

        Private m_core As cCore
        Private m_bEnabled As Boolean
        Private m_plottype As ePlotTypes
        Private m_Ymax As Single

        Sub New(ByRef theCore As cCore)
            m_core = theCore
            m_bEnabled = False
        End Sub


        Private Function buildLine(ByVal iGroup As Integer) As LineItem

            If iGroup < 0 Then Return Nothing ' Safety first

            Dim td As cEcotracerGroupOutput = Me.m_core.EcotracerGroupResults
            Dim SimBio As cEcosimGroupOutput
            Dim vList As New PointPairList()
            Dim strLabel As String = My.Resources.HEADER_ENVIRONMENT
            Dim clrLine As Color = Color.Black
            Dim yVal As Double
            Dim dx As Double

            If iGroup > 0 Then
                Dim group As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iGroup)
                strLabel = group.Name
                clrLine = cStyleGuide.GetInstance().GroupColor(Me.m_core, iGroup)
            End If

            'decide the plot type outside the loop 
            'so that there does not have to be an "If Me.m_plottype = ePlotTypes.CB And iGroup > 0 Then" inside the loop
            If Me.m_plottype = ePlotTypes.CB And iGroup > 0 Then

                SimBio = Me.m_core.EcoSimGroupOutputs(iGroup)

                For iTimeStep As Integer = 1 To Me.m_core.nEcosimTimeSteps
                    dx = Me.m_core.EcosimFirstYear + (iTimeStep / cCore.N_MONTHS)
                    yVal = CDbl(td.Concentration(iGroup, iTimeStep) / SimBio.Biomass(iTimeStep))
                    Me.m_Ymax = CSng(Math.Max(m_Ymax, yVal))
                    vList.Add(dx, yVal)
                Next iTimeStep

            Else

                For iTimeStep As Integer = 1 To Me.m_core.nEcosimTimeSteps
                    dx = Me.m_core.EcosimFirstYear + (iTimeStep / cCore.N_MONTHS)
                    yVal = CDbl(td.Concentration(iGroup, iTimeStep))
                    Me.m_Ymax = CSng(Math.Max(m_Ymax, yVal))
                    vList.Add(dx, yVal)
                Next iTimeStep

            End If

            Return New LineItem(strLabel, vList, clrLine, SymbolType.None, 1)

        End Function

        Public Function GetGroupLines(ByVal iGroup As Integer) As System.Collections.Generic.List(Of ZedGraph.LineItem) Implements IDisplayModeHelper.GetGroupLines

            Dim lstLines As New List(Of LineItem)

            Me.m_Ymax = Single.MinValue
            lstLines.Add(buildLine(iGroup))

            Return lstLines

        End Function

        Public Function GetGroupLines(ByVal lstGroups As System.Windows.Forms.ListBox.SelectedObjectCollection) As System.Collections.Generic.List(Of ZedGraph.LineItem) Implements IDisplayModeHelper.GetGroupLines
            Dim lstLines As New List(Of LineItem)

            Me.m_Ymax = Single.MinValue
            For Each iGroup As Integer In lstGroups
                lstLines.Add(buildLine(iGroup))
            Next iGroup

            Return lstLines

        End Function


        Public Function GetGroupMax(ByVal iGroup As Integer) As Single Implements IDisplayModeHelper.GetGroupMax
            Dim smax As Single

            Try

                'there is no biomass for the environment index so there is no way to compute C/B
                'in that case use Concentration(group,time)
                If Me.m_plottype = ePlotTypes.CB And iGroup > 0 Then

                    Dim grpbio As cEcosimGroupOutput = Me.m_core.EcoSimGroupOutputs(iGroup)
                    For iTimeStep As Integer = 1 To Me.m_core.nEcosimTimeSteps
                        smax = Math.Max(Me.m_core.EcotracerGroupResults.Concentration(iGroup, iTimeStep) / grpbio.Biomass(iTimeStep), smax)
                    Next

                Else

                    For iTimeStep As Integer = 1 To Me.m_core.nEcosimTimeSteps
                        smax = Math.Max(Me.m_core.EcotracerGroupResults.Concentration(iGroup, iTimeStep), smax)
                    Next

                End If

                Return smax

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, ex.StackTrace)
                Return smax
            End Try


        End Function

        Public ReadOnly Property Enabled() As Boolean Implements IDisplayModeHelper.Enabled
            Get
                Me.Refresh()
                'm_bEnabled is set in Refresh
                Return Me.m_bEnabled
            End Get
        End Property

        Public ReadOnly Property Title() As String Implements IDisplayModeHelper.Title
            Get
                'make sure we have the latest state
                Me.Refresh()

                If Enabled Then
                    Return "Ecosim"
                Else
                    Return "Ecosim data not available."
                End If

            End Get
        End Property

        Public Sub Refresh() Implements IDisplayModeHelper.Refresh

            Me.m_bEnabled = False

            'make sure Ecosim is the selected model and it has run
            If m_core.EcoSimModelParameters.ContaminantTracing And m_core.StateMonitor.HasEcosimRan Then
                Me.m_bEnabled = True
            End If

        End Sub


        Public ReadOnly Property nYears() As Integer Implements IDisplayModeHelper.nYears
            Get
                Return m_core.nEcosimYears
            End Get
        End Property

        Public Property PlotType() As ePlotTypes Implements IDisplayModeHelper.PlotType
            Get
                Return Me.m_plottype
            End Get

            Set(ByVal value As ePlotTypes)
                Me.m_plottype = value
            End Set

        End Property

        Public WriteOnly Property RegionIndex() As Integer Implements IDisplayModeHelper.RegionIndex
            Set(ByVal value As Integer)
                'ecosim does not use regions
            End Set
        End Property

        Public ReadOnly Property EnabledForSpace() As Boolean Implements IDisplayModeHelper.EnabledForSpace
            Get
                Return False
            End Get
        End Property

        Public Function Max() As Single Implements IDisplayModeHelper.Max
            Return Me.m_Ymax
        End Function

        Public ReadOnly Property XAxisLabel() As String Implements IDisplayModeHelper.XAxisLabel
            Get
                Return "Ecosim Years"
            End Get
        End Property

        Public ReadOnly Property YAxisLabel() As String Implements IDisplayModeHelper.YAxisLabel
            Get
                Dim lb As String

                If Me.m_plottype = ePlotTypes.CB Then
                    lb = "Concentration / Biomass"
                Else
                    lb = "Concentration"
                End If
                Return lb
            End Get
        End Property

        Public ReadOnly Property bCanPlot() As Boolean Implements IDisplayModeHelper.bCanPlot
            Get
                Return True
            End Get
        End Property
    End Class

#End Region

#Region "EcoSpace implementation"

    Private Class cEcoSpaceDisplayHelper
        Implements IDisplayModeHelper

        Private m_core As cCore
        Private m_bEnabled As Boolean
        Private m_plottype As ePlotTypes
        Private m_iRegion As Integer
        Private m_bAllRgns As Boolean
        Private m_rgn1 As Integer
        Private m_rgn2 As Integer
        Private m_Ymax As Single

        Sub New(ByRef theCore As cCore)
            m_core = theCore
        End Sub


        Public Function GetGroupMax(ByVal iGroup As Integer) As Single Implements IDisplayModeHelper.GetGroupMax
            Dim smax As Single

            If Me.m_plottype = ePlotTypes.Conc Then
                For ireg As Integer = 0 To Me.m_core.nRegions
                    For iTimeStep As Integer = 1 To Me.m_core.nEcosimTimeSteps
                        smax = Math.Max(Me.m_core.EcotracerRegionGroupResults.Concentration(ireg, iGroup, iTimeStep), smax)
                    Next iTimeStep
                Next ireg
            Else
                For ireg As Integer = 0 To Me.m_core.nRegions
                    For iTimeStep As Integer = 1 To Me.m_core.nEcosimTimeSteps
                        smax = Math.Max(Me.m_core.EcotracerRegionGroupResults.CB(ireg, iGroup, iTimeStep), smax)
                    Next iTimeStep
                Next ireg
            End If

            Return smax
        End Function

        Public Function GetGroupLines(ByVal iGroup As Integer) As System.Collections.Generic.List(Of ZedGraph.LineItem) Implements IDisplayModeHelper.GetGroupLines
            If iGroup < 0 Then Return Nothing ' Safety first

            Dim lstLines As New List(Of LineItem)
            Me.m_Ymax = Single.MinValue

            'm_rgn1 and m_rgn2 were set in RegionIndex
            For ireg As Integer = Me.m_rgn1 To Me.m_rgn2
                lstLines.Add(buildLine(iGroup, ireg))
            Next

            Return lstLines

        End Function

        Public Function GetGroupLines(ByVal lstGroups As System.Windows.Forms.ListBox.SelectedObjectCollection) As System.Collections.Generic.List(Of ZedGraph.LineItem) Implements IDisplayModeHelper.GetGroupLines
            Dim lstLines As New List(Of LineItem)

            Me.m_Ymax = Single.MinValue
            For Each iGroup As Integer In lstGroups

                'm_rgn1 and m_rgn2 were set in RegionIndex
                For ireg As Integer = Me.m_rgn1 To Me.m_rgn2
                    lstLines.Add(buildLine(iGroup, ireg))
                Next

            Next iGroup

            Return lstLines

        End Function


        Private Function buildLine(ByVal iGroup As Integer, ByVal iregion As Integer) As LineItem
            Dim td As cEcotracerRegionGroupOutput = Me.m_core.EcotracerRegionGroupResults
            Dim vList As PointPairList
            Dim lstLines As New List(Of LineItem)
            Dim strLabel As String
            Dim clrLine As Color = Color.Black
            Dim grpSym As SymbolType
            Dim linesize As Single
            Dim yVal As Single
            Dim name As String = My.Resources.HEADER_ENVIRONMENT
            Dim rgName As String = "Region " & iregion
            Dim dx As Double
            Dim ntsYear As Single

            ntsYear = m_core.EcospaceModelParameters.NumberOfTimeStepsPerYear

            'build the label group and region name
            If iGroup > 0 Then
                name = Me.m_core.EcoPathGroupInputs(iGroup).Name
                clrLine = cStyleGuide.GetInstance().GroupColor(Me.m_core, iGroup)
            End If

            'If iregion > 0 Then
            '    rgName = m_core.EcospaceRegions(iregion).Name
            'End If

            strLabel = name & ", " & rgName

            'this will figure out which varname to display 
            'base on the selected group and the ePlotTypes enum
            Dim varName As eVarNameFlags = getVarName(iGroup)

            vList = New PointPairList()

            For iTimeStep As Integer = 1 To Me.m_core.nEcospaceTimeSteps
                dx = Me.m_core.EcosimFirstYear + (iTimeStep / ntsYear)
                yVal = td.GetVariable(varName, iregion, iGroup, iTimeStep)
                Me.m_Ymax = CSng(Math.Max(m_Ymax, yVal))

                vList.Add(dx, CDbl(yVal))
            Next iTimeStep

            'line symbol and line size
            linesize = m_core.nRegions * 2 - iregion
            If iregion = 0 Then
                grpSym = SymbolType.None
            Else
                Try 'incase iregion is larger then the largest SymbolType enumerator
                    grpSym = CType(iregion, SymbolType)
                Catch ex As Exception
                    grpSym = SymbolType.XCross
                End Try
            End If

            'I obviously don't understand how the LineItem works
            'setting the linesize does not change the thickness of the line
            'even when the SymbolType is None
            Return New LineItem(strLabel, vList, clrLine, grpSym, linesize)

        End Function

        ''' <summary>
        ''' Get the correct variable to display based on the selected Group and the ePlotTypes
        ''' </summary>
        ''' <param name="iGroup"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function getVarName(ByVal iGroup As Integer) As eVarNameFlags

            If iGroup = 0 Then
                'The zero group is the environment variable 
                If Me.m_plottype = ePlotTypes.Conc Then
                    Return eVarNameFlags.CEnvironment
                Else
                    Return eVarNameFlags.CBEnvironment
                End If

            Else
                'normal groups
                If Me.m_plottype = ePlotTypes.Conc Then
                    Return eVarNameFlags.Concentration
                Else
                    Return eVarNameFlags.ConcBio
                End If

            End If

        End Function

        Public ReadOnly Property Enabled() As Boolean Implements IDisplayModeHelper.Enabled
            Get
                Me.Refresh()
                'm_bEnabled is set in Refresh
                Return m_bEnabled
            End Get
        End Property

        Public ReadOnly Property Title() As String Implements IDisplayModeHelper.Title
            Get
                Me.Refresh()
                If Me.m_bEnabled Then
                    Return "Ecospace"
                Else
                    Return "Ecospace data not available."
                End If
            End Get
        End Property

        Public Sub Refresh() Implements IDisplayModeHelper.Refresh

            Me.m_bEnabled = False

            'make sure Ecospace run before checking the EcospaceModelParameters object
            'if Ecospace has not loaded EcospaceModelParameters will be NULL
            If m_core.StateMonitor.HasEcospaceRan Then
                If m_core.EcospaceModelParameters.ContaminantTracing Then
                    Me.m_bEnabled = True
                End If
            End If

        End Sub

        Public ReadOnly Property nYears() As Integer Implements IDisplayModeHelper.nYears
            Get
                Return m_core.nEcospaceYears
            End Get
        End Property


        Public Property PlotType() As ePlotTypes Implements IDisplayModeHelper.PlotType
            Get
                Return Me.m_plottype
            End Get

            Set(ByVal value As ePlotTypes)
                Me.m_plottype = value
            End Set

        End Property

        Public WriteOnly Property RegionIndex() As Integer Implements IDisplayModeHelper.RegionIndex

            Set(ByVal value As Integer)

                'bounds checking
                'm_core.nRegions + 1 is all Regions
                If value < 0 Or value > m_core.nRegions + 1 Then
                    Exit Property
                End If

                Me.m_rgn1 = value
                Me.m_rgn2 = value

                If value > m_core.nRegions Then
                    Me.m_rgn1 = 0
                    Me.m_rgn2 = m_core.nRegions
                End If

            End Set
        End Property

        Public ReadOnly Property EnabledForSpace() As Boolean Implements IDisplayModeHelper.EnabledForSpace
            Get
                Return Me.m_bEnabled
            End Get
        End Property

        Public Function Max() As Single Implements IDisplayModeHelper.Max
            Return m_Ymax
        End Function

        Public ReadOnly Property XAxisLabel() As String Implements IDisplayModeHelper.XAxisLabel
            Get
                Return "Ecospace Years"
            End Get
        End Property

        Public ReadOnly Property YAxisLabel() As String Implements IDisplayModeHelper.YAxisLabel

            Get
                Dim lb As String
                If Me.m_plottype = ePlotTypes.CB Then
                    lb = "Concentration / Biomass"
                Else
                    lb = "Concentration"
                End If
                Return lb
            End Get

        End Property

        Public ReadOnly Property bCanPlot() As Boolean Implements IDisplayModeHelper.bCanPlot
            Get
                Return True
            End Get
        End Property
    End Class

#End Region

#End Region

End Class






#If 0 Then ' Ye olde code

' Plotting Ecosim data
Private Sub PlotGrp(Grp As Integer)
On Local Error Resume Next
Dim i As Integer
Dim j As Integer
Dim Cnt As Integer
Dim Start As Integer
Dim Title As String
Dim NSeries As Integer
    'poolshow = grp
'    Label2.Caption = IIf(grp <= NumGroups + 1, "Conc. scale: 10^" & Format(-Scaling(grp), "0"), "All groups")
    'Title = "Tracer concentrations"
    Screen.MousePointer = vbHourglass
    frmTracePlot.Refresh
    If Grp <= NumGroups + 1 Then

        ' JS: plot Single group

        Label2.Caption = "Conc. scale: 10^" & Format(-Scaling(Grp), "0")
        'Call a function to count the number of timeseries; add 1 for displaying the conc for the group:
        NSeries = NoOfTimeSeries(8, Grp) + NoOfTimeSeries(9, Grp) + 1
        lblSS.Visible = IIf(NSeries > 1, True, False)
        lblW.Visible = False

        cmdPredObs.Visible = lblSS.Visible
        If NSeries = 1 Then 'there are no time series data for this group
            NSeries = 0
            ReDim GrpData(Ntimes)   ' / 12)
            For i = 1 To Ntimes ' / 12
                GrpData(i) = TracerConc(Grp, i) * 10 ^ Scaling(Grp)
            Next
            PlotSingleSeries JGraph1(0), GrpData(), Title
        Else    'there are time series data, so multiple series plotting
            ReDim GrpData(NSeries, Ntimes)  ' / 12)
            ReDim StyleType(NSeries) As Integer
            'Style Types 0 = Line, 2 = Circles, 1 = Histogram
            'The estimated biomass:
            For i = 1 To Ntimes     '/ 12
                GrpData(0, i) = TracerConc(Grp, i) * 10 ^ Scaling(Grp)
            Next
            StyleType(0) = 0
            'Then plot the time series that there may be
            Cnt = 0
            For i = 1 To NdatType
                If DatPool(i) = Grp Then
                    If DatType(i) = 8 Or DatType(i) = 9 Then
                        Cnt = Cnt + 1
                        StyleType(Cnt) = 2  'Circle
                        For j = 1 To Ntimes     '/ 12
                            If j Mod 12 = 1 Then
                                If CInt(j / 12) <= NdatYear Then
                                    GrpData(Cnt, j) = DatVal(CInt(j / 12) + 1, i) * 10 ^ Scaling(Grp)
                                    lblW.Visible = lblSS.Visible
                                    lblW.AutoSize = True
                                    lblW.Caption = "Time series weight = " + Format(WtType(i), GenNum)
                                End If
                                'GrpData(Cnt, j) = DatVal(j, i)  '(CInt(j / 12) + 1, i)
                                If DatType(i) = 8 And teDatq(i) > 0 Then GrpData(Cnt, j) = GrpData(Cnt, j) / teDatq(i)
                                If GrpData(Cnt, j) <= 0 Then GrpData(Cnt, j) = -100
                            Else
                                GrpData(Cnt, j) = -100
                            End If
                        Next
                    End If
                End If
            Next
            Title = " "
            PlotMultipleTimeSeries JGraph1(0), GrpData(), Title
        End If
    Else

        ' JS: plot a range

        Start = 10 * (Grp - NumGroups - 2)
        With frmTracePlot
            For i = 1 To 10 'IIf(NumGroups <= 10, NumGroups, 10)
                If Start + i > NumGroups Then Exit For
                .CurrentX = JGraph1(0).Left + JGraph1(0).Width + 10
                .CurrentY = JGraph1(0).Top + 1500 + i * 240
                .ForeColor = PoolColor(Sort(i + Start))
                frmTracePlot.Print Specie(Sort(i + Start))
            Next
        End With
        Label2.Caption = "Conc. scale: 10^" & Format(-Scaling(Sort(1 + Start)), "0")
        Title = CStr(Start)
        PlotMultipleSeries JGraph1(0), TracerConc(), Title
    End If
    Screen.MousePointer = vbDefault
End Sub

Private Sub PlotMultipleSeries(graph As JGraph, Ydata() As Single, Title As String)
On Local Error Resume Next
'Calling Arguments are:
    'graph is the jgraph control
    'ydata(N, XX) is a 2 dimensional array of N series, and XX datapoints
Dim i As Integer, j As Integer, iclr As Long
Dim XX As Integer, X As Integer, NSeries As Integer, xN As Integer
Dim y1 As Integer, St As Integer, Lab As Integer
    NSeries = UBound(Ydata, 1)
    xN = LBound(Ydata, 1) + 1
    XX = UBound(Ydata, 2) - 1
    X = LBound(Ydata, 2)
    X = IIf(X = 0, 1, X)
    graph.ClearAllData
    graph.BackColor = SimBackColor
    graph.NumSets = NSeries
    graph.AutoScale = True
    graph.YAxis = 0.0001
    graph.YOrigin = 0
    graph.XAxis = XX
    graph.UseTextLabels = True
    Dim Start As Integer
    Start = CInt(Title)
    Title = ""
    If Title <> "" Then graph.Caption = Title
    'Style Types 0 = Line, 1 = Circles, 2 = Histogram
    If NSeries < 11 Then
        For j = xN To NSeries    'Loop over series
            iclr = IIf(j > 0 And j <= NumGroups, PoolColor(j), QBColor(0))
            graph.SetStyle j, 0, 2, iclr  ', poolcolor(poolshow)   'iclr
            'graph.SetStyleEx j, StyleType(j), IIf(StyleType(j) = 0, 2, 4), poolcolor(poolshow), poolcolor(poolshow)   'iclr
            For i = X To XX
                graph.AddDataToSet j, Ydata(j, i)  'Ydata(j, i)
            Next i
            graph.DrawDataAutoScale
        Next j
    Else
        For j = 1 To 10
            If Start + j > NumGroups Then Exit For
            iclr = PoolColor(Sort(j + Start))
            graph.SetStyle j, 0, 2, iclr  ', poolcolor(poolshow)   'iclr
            For i = X To XX
                graph.AddDataToSet j, Ydata(Sort(j + Start), i)
                graph.DrawDataAutoScale
            Next i
        Next j
    End If
    y1 = FirstYear Mod 100
    St = Ntimes / graph.XTicks / 12
    For j = 0 To graph.XTicks 'XX step st
        Lab = (y1 + j * St) Mod 100
        graph.XLabelString "'" + CStr(Lab), j
    Next

End Sub

Private Sub PlotMultipleTimeSeries(graph As JGraph, Ydata() As Single, Title As String)
On Local Error Resume Next
'Calling Arguments are:
    'graph is the jgraph control
    'ydata(N, XX) is a 2 dimensional array of N series, and XX datapoints
Dim i As Integer, j As Integer, iclr As Long, ii As Integer
Dim XX As Integer, X As Integer, NSeries As Integer, xN As Integer
Dim y1 As Integer, St As Integer, Lab As Integer
    graph.Visible = False
    NSeries = UBound(Ydata, 1)
    xN = LBound(Ydata, 1)

    i = UBound(Ydata, 2) / 12 '=Ntimes / 12 '= number of years
    j = i \ 10
    If i Mod (j + 1) > 0 Then
        graph.XTicks = i \ (j + 1) + 1
    Else
        graph.XTicks = i \ (j + 1)
    End If
    XX = 12 * graph.XTicks * (j + 1)
    If XX > i * 12 Then XX = i * 12
    X = LBound(Ydata, 2)
    X = IIf(X = 0, 1, X)
    graph.ClearAllData
    graph.BackColor = SimBackColor
    graph.NumSets = NSeries
    graph.AutoScale = True
    graph.YAxis = 1E-20
    'graph.YOrigin = 0
    graph.XAxis = XX
    graph.UseTextLabels = True
    If Title <> "" Then graph.Caption = Title
    'Style Types 0 = Line, 1 = Circles, 2 = Histogram
    For j = xN To NSeries    'Loop over series
        'iclr = IIf(j > 0, QBColor(1 + j), poolcolor(poolshow))
            Select Case j
            Case 0
                iclr = QBColor(0)   'poolcolor(poolshow)
            Case 1
                iclr = QBColor(9)   'blue
            Case 2
                iclr = QBColor(12)   'red
            Case 3
                iclr = QBColor(10)   'red
            Case 4
                iclr = QBColor(11)   'red
            Case Else
                iclr = QBColor(7 + IIf(j < 9, j, 8))
            End Select
        graph.SetStyle j, StyleType(j), IIf(StyleType(j) = 0, 2, 4), iclr  ', poolcolor(poolshow)   'iclr
        For i = X To XX
            graph.AddDataToSet j, Ydata(j, i)
            graph.DrawDataAutoScale
        Next i
    Next j

    y1 = FirstYear Mod 100

    If graph.XTicks > 0 Then St = Ntimes / graph.XTicks / 12
    For j = 0 To graph.XTicks 'XX step st
        Lab = (y1 + j * St) Mod 100
        graph.XLabelString "'" + CStr(Lab), j
    Next
    graph.Visible = True
End Sub

Private Sub PlotSingleSeries(graph As JGraph, Ydata() As Single, Title As String)
On Local Error Resume Next
Dim i As Integer    ', iclr As Long
Dim j As Integer
Dim XX As Integer
'This routine plots as single dimension array
Dim y1 As Integer, St As Integer, Lab As Integer

    XX = UBound(Ydata)
    'iclr = QBColor(4)
    graph.ClearAllData
    graph.NumSets = 1
    graph.YAxis = IIf(Title = "Feeding time", 2.001, 0.00000001)
    graph.AutoScale = True
    graph.XAxis = XX
    graph.BackColor = SimBackColor
    graph.UseTextLabels = True
    If Title <> "" Then graph.Caption = Title
    'Style Types 0 = Line, 1 = Circles, 2 = Histogram
    graph.SetStyle 0, 0, 2, QBColor(0)  ' poolcolor(poolshow)    'iclr
    For i = 1 To XX
        graph.AddData Ydata(i)
        graph.DrawDataAutoScale
    Next i

    y1 = FirstYear Mod 100
    If graph.XTicks > 0 Then St = Ntimes / graph.XTicks / 12
    For j = 0 To graph.XTicks 'XX step st
        Lab = (y1 + j * St) Mod 100
        graph.XLabelString "'" + CStr(Lab), j
    Next

End Sub

#End If