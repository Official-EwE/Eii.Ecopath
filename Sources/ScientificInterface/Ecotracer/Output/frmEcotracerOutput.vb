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
''' Form class, implementing the Ecotracer (contaminant tracing) output interface.
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
    Private m_zgh As cZedGraphHelper = Nothing
    ''' <summary></summary>
    Private m_curDisplayMode As eDisplayModeTypes = eDisplayModeTypes.NotInitialized

    ''' <summary></summary>
    Private m_asScaling() As Single

    Private m_DisplayHelper As IDisplayModeHelper

    Private m_sortOrder() As Integer
    Private m_bSorted As Boolean
    Private m_plottype As ePlotTypes = ePlotTypes.CB

#End Region ' Private vars

#Region " Events "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.UIContext, Me.m_zgc)

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace}

        AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
        Me.m_zgh.Detach()
        Me.m_zgh = Nothing
        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub OnActivated(ByVal e As System.EventArgs)
        MyBase.OnActivated(e)
        Me.RefreshData()
    End Sub

    Private Sub m_lbGroups_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_lbGroups.SelectedIndexChanged, m_lbGroups.SelectedIndexChanged
        PlotGroup()
    End Sub

    Private Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)
        If ((changeType And cStyleGuide.eChangeType.Colours) > 0) Then
            ' Respond to group colour changes
            Me.PlotGroup()
            ' Invalidate group list box
            Me.m_lbGroups.Invalidate()
        End If
    End Sub

    Private Sub btRunSim_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnRunSim.Click

        Try
            'An Ecosim scenario was loaded when this form was loaded
            'so there is no need to check
            Me.Core.EcoSimModelParameters.ContaminantTracing = True
            Me.startModelRun()
            Me.Core.RunEcoSim(AddressOf Me.EcosimCallback)
            Me.RefreshGraph()

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".btRunSim_Click() Error: " & ex.Message)
        End Try

    End Sub

    Private Sub btRunSpace_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnRunSpace.Click

        Try
            'No Ecospace scenario has been load
            If Me.Core.StateMonitor.HasEcospaceLoaded = False Then
                'Ask the user for a Ecospace scenario via the command
                Dim cmd As cCommand = Me.CommandHandler.GetCommand("LoadEcospaceScenario")
                Debug.Assert(cmd IsNot Nothing, Me.ToString & ".btRunSpace_Click() LoadEcospaceScenario Command could not be found.")
                cmd.Invoke()
            End If

            'Make sure the scenario loaded successfully before trying to run Ecospace
            If Me.Core.StateMonitor.HasEcospaceLoaded Then
                Me.Core.EcospaceModelParameters.ContaminantTracing = True
                Me.startModelRun()
                Me.Core.RunEcoSpace(AddressOf Me.EcospaceCallback)
                Me.RefreshGraph()
            End If

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".btRunSpace_Click() Error: " & ex.Message)
        End Try

    End Sub

    Private Sub rbConc_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_rbConc.CheckedChanged

        If m_DisplayHelper Is Nothing Then Return

        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        If rb.Checked Then
            Me.m_plottype = ePlotTypes.Conc
            If Me.m_bSorted Then Me.RefreshData()
            Me.RefreshGraph()
        End If

    End Sub

    Private Sub rbCB_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_rbCB.CheckedChanged

        If m_DisplayHelper Is Nothing Then Return

        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        If rb.Checked Then
            Me.m_plottype = ePlotTypes.CB
            If Me.m_bSorted Then Me.RefreshData()
            Me.RefreshGraph()
        End If

    End Sub

    Private Sub cmbRegions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbRegions.SelectedIndexChanged
        Me.RefreshGraph()
    End Sub

    Private Sub ckSorted_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_chkSortGroups.Click
        Me.RefreshData()
    End Sub

    Private Sub OnDisplayGroups(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnShowHideGroups.Click
        Dim cmd As cCommand = Me.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
        Debug.Assert(cmd IsNot Nothing, Me.ToString & ".OnDisplayGroups() DisplayGroups Command could not be found.")
        cmd.Invoke()
    End Sub

#End Region ' Events

#Region " Internal bits "

    Private m_iProgress As Integer = 0

    ''' <summary>
    ''' Start a model run Ecosim or Ecospace
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub startModelRun()

        Me.m_iProgress = 0

        'clear out the progress bar
        cApplicationStatusNotifier.SetStatusText("Running Ecotracer...", _
                                                 TriState.UseDefault, _
                                                 CSng(Me.m_iProgress / Me.m_DisplayHelper.nYears))

    End Sub

    ''' <summary>
    ''' Update the progress bar in response to a model time step
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateProgess()

        Me.m_iProgress += 1

        If (Me.m_iProgress < Me.m_DisplayHelper.nYears) Then
            cApplicationStatusNotifier.SetStatusText("Running Ecotracer...", TriState.UseDefault, CSng(Me.m_iProgress / Me.m_DisplayHelper.nYears))
        Else
            cApplicationStatusNotifier.SetStatusText("", TriState.UseDefault)
        End If

    End Sub

    Private Function getDisplayMode() As eDisplayModeTypes
        Dim dmode As eDisplayModeTypes

        dmode = eDisplayModeTypes.NoResults

        'Ecosim selected
        If Me.Core.EcoSimModelParameters.ContaminantTracing Then
            dmode = eDisplayModeTypes.Ecosim
        End If

        'Ecospace
        'this is nested because EcospaceModelParameters will be Null if an Ecospace scenario has not been loaded
        If Me.Core.StateMonitor.HasEcospaceLoaded Then
            If Me.Core.EcospaceModelParameters.ContaminantTracing Then
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

        If Me.UIContext Is Nothing Then Return

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
        Me.m_lbGroups.Enabled = m_DisplayHelper.Enabled
        Me.m_cmbRegions.Enabled = m_DisplayHelper.EnabledForSpace

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
        Me.m_DisplayHelper.RegionIndex = Me.m_cmbRegions.SelectedIndex

        'Now get data from the display helper
        'Text for graph
        Me.m_zgc.GraphPane.Title.Text = m_DisplayHelper.Title
        Me.m_zgc.GraphPane.XAxis.Title.Text = m_DisplayHelper.XAxisLabel
        Me.m_zgc.GraphPane.YAxis.Title.Text = m_DisplayHelper.YAxisLabel

        'scale of graph
        Me.m_zgc.GraphPane.XAxis.Scale.Min = CDbl(Me.Core.EcosimFirstYear)
        Me.m_zgc.GraphPane.XAxis.Scale.Max = CDbl(Me.Core.EcosimFirstYear + m_DisplayHelper.nYears)

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

            If iGroupSelected = cCore.NULL_VALUE And m_lbGroups.SelectedIndices.Count = 0 Then
                'nothing to draw
                Exit Sub
            End If

            'can the display helper plot 
            If Me.m_DisplayHelper.bCanPlot Then

                ' If not forcing to draw a single item draw all selected
                If iGroupSelected = cCore.NULL_VALUE Then
                    lines = Me.m_DisplayHelper.GetGroupLines(m_lbGroups.SelectedItems)
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

        Me.m_cmbRegions.Items.Clear()

        If Me.m_DisplayHelper.EnabledForSpace Then
            'only populate the region list if space is enabled
            Me.m_cmbRegions.Items.Add("Undefined area")
            For irgn As Integer = 1 To Me.Core.nRegions
                Me.m_cmbRegions.Items.Add("region " & irgn) ' Me.Core.EcospaceRegions(irgn).Name)
            Next
            Me.m_cmbRegions.Items.Add("All Regions")

            Me.m_cmbRegions.SelectedIndex = Me.Core.nRegions + 1
        End If

    End Sub


    Private Sub CalcSortOrder()
        Dim maxVal As Single, iSort As Integer, gMax As Single
        Dim GrpTaken() As Boolean
        Dim ngrps As Integer = Me.Core.nGroups

        ReDim Me.m_sortOrder(ngrps)
        ReDim GrpTaken(ngrps)

        'get the sorted checked state
        Me.m_bSorted = Me.m_chkSortGroups.Checked

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

        m_lbGroups.Items.Clear()

        'Add "All groups" at the top
        m_lbGroups.Items.Add(0)
        For i As Integer = 1 To Me.Core.nGroups
            m_lbGroups.Items.Add(Me.m_sortOrder(i))
        Next


    End Sub

    Private Sub m_lbGroups_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) _
        Handles m_lbGroups.DrawItem

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
            If iGroup <= Me.Core.nGroups Then
                If iGroup = 0 Then
                    strItemText = My.Resources.HEADER_ENVIRONMENT
                    clr = Color.Black
                Else
                    Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iGroup)
                    strItemText = group.Name
                    clr = Me.StyleGuide.GroupColor(Me.Core, iGroup)
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
                    Return New cNoResultsDisplayHelper(Me.UIContext)
                Case eDisplayModeTypes.Ecosim
                    Return New cEcoSimDisplayHelper(Me.UIContext)
                Case eDisplayModeTypes.Ecospace
                    Return New cEcoSpaceDisplayHelper(Me.UIContext)
            End Select

            'something went wrong
            'the arg DisplayMode was not valid return the cNoResultsDisplayHelper object 
            'this will let the interface run without data
            Debug.Assert(False, "DisplayHelperFactory() Invalid DisplayMode")
            Return New cNoResultsDisplayHelper(Me.UIContext)

        Else
            'return the current IDisplayModeHelper object
            'make sure there is one
            Debug.Assert(m_DisplayHelper IsNot Nothing, Me.ToString & ".DisplayHelperFactory() Current display mode has not been set! Something is wrong!")
            Return Me.m_DisplayHelper
        End If

    End Function

#End Region ' Helper methods

#End Region ' Internal bits

#Region " Overrides "

    Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

        If msg.Source = eCoreComponentType.EcoSim Or msg.Source = eCoreComponentType.EcoSpace Then
            Me.RefreshData()
        End If

    End Sub

    Public Overrides ReadOnly Property IsRunForm() As Boolean
        Get
            Return True
        End Get
    End Property

#End Region ' Overrides

#Region "Display Mode Helper Classes"

#Region "Interface definition"

    Private Interface IDisplayModeHelper
        Inherits IUIElement

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

        ReadOnly Property Core() As cCore
        ReadOnly Property StyleGuide() As cStyleGuide

        ''' <summary>Title of the Graph.</summary>
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

        Sub New(ByVal uic As cUIContext)
            ' Sanity check
            Debug.Assert(uic IsNot Nothing)
        End Sub

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Nothing
            End Get
            Set(ByVal value As cUIContext)
                ' NOP
            End Set
        End Property

        Public ReadOnly Property Core() As cCore _
            Implements IDisplayModeHelper.Core
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property StyleGuide() As cStyleGuide _
            Implements IDisplayModeHelper.StyleGuide
            Get
                Return Nothing
            End Get
        End Property

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

        Private m_uic As cUIContext = Nothing
        Private m_bEnabled As Boolean
        Private m_plottype As ePlotTypes
        Private m_Ymax As Single

        Sub New(ByRef uic As cUIContext)
            ' Sanity check
            Debug.Assert(uic IsNot Nothing)
            Me.UIContext = uic
            Me.m_bEnabled = False
        End Sub

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        ReadOnly Property Core() As cCore _
            Implements IDisplayModeHelper.Core
            Get
                Return Me.UIContext.Core
            End Get
        End Property

        Public ReadOnly Property StyleGuide() As cStyleGuide _
            Implements IDisplayModeHelper.StyleGuide
            Get
                Return Me.UIContext.StyleGuide
            End Get
        End Property

        Private Function buildLine(ByVal iGroup As Integer) As LineItem

            If iGroup < 0 Then Return Nothing ' Safety first

            Dim td As cEcotracerGroupOutput = Me.Core.EcotracerGroupResults
            Dim SimBio As cEcosimGroupOutput
            Dim vList As New PointPairList()
            Dim strLabel As String = My.Resources.HEADER_ENVIRONMENT
            Dim clrLine As Color = Color.Black
            Dim yVal As Double
            Dim dx As Double

            If iGroup > 0 Then
                Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iGroup)
                strLabel = group.Name
                clrLine = Me.m_uic.StyleGuide.GroupColor(Me.Core, iGroup)
            End If

            'decide the plot type outside the loop 
            'so that there does not have to be an "If Me.m_plottype = ePlotTypes.CB And iGroup > 0 Then" inside the loop
            If Me.m_plottype = ePlotTypes.CB And iGroup > 0 Then

                SimBio = Me.Core.EcoSimGroupOutputs(iGroup)

                For iTimeStep As Integer = 1 To Me.Core.nEcosimTimeSteps
                    dx = Me.Core.EcosimFirstYear + (iTimeStep / cCore.N_MONTHS)
                    yVal = CDbl(td.Concentration(iGroup, iTimeStep) / SimBio.Biomass(iTimeStep))
                    Me.m_Ymax = CSng(Math.Max(m_Ymax, yVal))
                    vList.Add(dx, yVal)
                Next iTimeStep

            Else

                For iTimeStep As Integer = 1 To Me.Core.nEcosimTimeSteps
                    dx = Me.Core.EcosimFirstYear + (iTimeStep / cCore.N_MONTHS)
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

                    Dim grpbio As cEcosimGroupOutput = Me.Core.EcoSimGroupOutputs(iGroup)
                    For iTimeStep As Integer = 1 To Me.Core.nEcosimTimeSteps
                        smax = Math.Max(Me.Core.EcotracerGroupResults.Concentration(iGroup, iTimeStep) / grpbio.Biomass(iTimeStep), smax)
                    Next

                Else

                    For iTimeStep As Integer = 1 To Me.Core.nEcosimTimeSteps
                        smax = Math.Max(Me.Core.EcotracerGroupResults.Concentration(iGroup, iTimeStep), smax)
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
            If Me.Core.EcoSimModelParameters.ContaminantTracing And Me.Core.StateMonitor.HasEcosimRan Then
                Me.m_bEnabled = True
            End If

        End Sub


        Public ReadOnly Property nYears() As Integer Implements IDisplayModeHelper.nYears
            Get
                Return Me.Core.nEcosimYears
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

        Private m_uic As cUIContext = Nothing
        Private m_bEnabled As Boolean
        Private m_plottype As ePlotTypes
        Private m_iRegion As Integer
        Private m_bAllRgns As Boolean
        Private m_rgn1 As Integer
        Private m_rgn2 As Integer
        Private m_Ymax As Single

        Sub New(ByVal uic As cUIContext)
            ' Sanity check
            Debug.Assert(uic IsNot Nothing)
            Me.UIContext = uic
        End Sub

        Private Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Private ReadOnly Property Core() As cCore _
            Implements IDisplayModeHelper.Core
            Get
                Return Me.UIContext.Core
            End Get
        End Property

        Public ReadOnly Property StyleGuide() As cStyleGuide _
            Implements IDisplayModeHelper.StyleGuide
            Get
                Return Me.m_uic.StyleGuide
            End Get
        End Property

        Public Function GetGroupMax(ByVal iGroup As Integer) As Single Implements IDisplayModeHelper.GetGroupMax
            Dim smax As Single

            If Me.m_plottype = ePlotTypes.Conc Then
                For ireg As Integer = 0 To Me.UIContext.Core.nRegions
                    For iTimeStep As Integer = 1 To Me.Core.nEcosimTimeSteps
                        smax = Math.Max(Me.Core.EcotracerRegionGroupResults.Concentration(ireg, iGroup, iTimeStep), smax)
                    Next iTimeStep
                Next ireg
            Else
                For ireg As Integer = 0 To Me.Core.nRegions
                    For iTimeStep As Integer = 1 To Me.Core.nEcosimTimeSteps
                        smax = Math.Max(Me.Core.EcotracerRegionGroupResults.CB(ireg, iGroup, iTimeStep), smax)
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
            Dim td As cEcotracerRegionGroupOutput = Me.Core.EcotracerRegionGroupResults
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

            ntsYear = Me.Core.EcospaceModelParameters.NumberOfTimeStepsPerYear

            'build the label group and region name
            If iGroup > 0 Then
                name = Me.Core.EcoPathGroupInputs(iGroup).Name
                clrLine = Me.StyleGuide.GroupColor(Me.Core, iGroup)
            End If

            'If iregion > 0 Then
            '    rgName = Me.Core.EcospaceRegions(iregion).Name
            'End If

            strLabel = name & ", " & rgName

            'this will figure out which varname to display 
            'base on the selected group and the ePlotTypes enum
            Dim varName As eVarNameFlags = getVarName(iGroup)

            vList = New PointPairList()

            For iTimeStep As Integer = 1 To Me.Core.nEcospaceTimeSteps
                dx = Me.Core.EcosimFirstYear + (iTimeStep / ntsYear)
                yVal = td.GetVariable(varName, iregion, iGroup, iTimeStep)
                Me.m_Ymax = CSng(Math.Max(m_Ymax, yVal))

                vList.Add(dx, CDbl(yVal))
            Next iTimeStep

            'line symbol and line size
            linesize = Me.Core.nRegions * 2 - iregion
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
            If Me.Core.StateMonitor.HasEcospaceRan Then
                If Me.Core.EcospaceModelParameters.ContaminantTracing Then
                    Me.m_bEnabled = True
                End If
            End If

        End Sub

        Public ReadOnly Property nYears() As Integer Implements IDisplayModeHelper.nYears
            Get
                Return Me.Core.nEcospaceYears
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
                'Me.Core.nRegions + 1 is all Regions
                If value < 0 Or value > Me.Core.nRegions + 1 Then
                    Exit Property
                End If

                Me.m_rgn1 = value
                Me.m_rgn2 = value

                If value > Me.Core.nRegions Then
                    Me.m_rgn1 = 0
                    Me.m_rgn2 = Me.Core.nRegions
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