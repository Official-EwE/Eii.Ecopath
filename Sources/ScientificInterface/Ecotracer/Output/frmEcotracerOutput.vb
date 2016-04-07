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
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Commands
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ZedGraph

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Form class, implementing the Ecotracer (contaminant tracing) output interface.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmEcotracerOutput

#Region " Definitions "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type, indicates the form result display mode.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Enum eDisplayModeTypes As Byte
        ''' <summary>No mode has been set yet this make it easier to init the form.</summary>
        NotInitialized
        ''' <summary>No results have been computed yet.</summary>
        NoResults
        ''' <summary>Show Ecosim results.</summary>
        Ecosim
        ''' <summary>Show Ecospace results.</summary>
        Ecospace
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type, indicates possible plot types.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Enum ePlotTypes As Byte
        ''' <summary>Plot type has not been initialized yet.</summary>
        NotSet = 0
        ''' <summary>Concentration plot.</summary>
        Conc
        ''' <summary>Concentration over biomass plot.</summary>
        CB
    End Enum

#End Region ' Public definitions

#Region " Private vars "

    ''' <summary>Zed graph helper to make the graph look purdy with.</summary>
    Private m_zgh As cZedGraphHelper = Nothing
    ''' <summary>Form display mode.</summary>
    Private m_curDisplayMode As eDisplayModeTypes = eDisplayModeTypes.NotInitialized
    ''' <summary>Form type of plot.</summary>
    Private m_plottype As ePlotTypes = ePlotTypes.NotSet
    ''' <summary>Thing to gather the data for the form.</summary>
    Private m_DisplayHelper As IDisplayModeHelper = Nothing
    ''' <summary>Update loop prevention flag.</summary>
    Private m_bInUpdate As Boolean = False

    ''' <summary>Value tracker for Conc Sim.</summary>
    Private m_propConcSimOn As cProperty = Nothing
    ''' <summary>Value tracker for Conc Space.</summary>
    Private m_propConcSpaceOn As cProperty = Nothing

#End Region ' Private vars

    Public Sub New()
        MyBase.new()
        Me.InitializeComponent()
    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.UIContext, Me.m_zgc)
        Me.m_zgh.ConfigurePane("", "", "", True)
        Me.m_lbGroups.Attach(Me.UIContext)

        Me.RefreshData()

        Me.PlotType = ePlotTypes.CB

        Me.m_propConcSimOn = Me.PropertyManager.GetProperty(Me.Core.EcoSimModelParameters, eVarNameFlags.ConSimOnEcoSim)
        Me.m_propConcSpaceOn = Me.PropertyManager.GetProperty(Me.Core.EcospaceModelParameters, eVarNameFlags.ConSimOnEcoSpace)
        AddHandler Me.m_propConcSimOn.PropertyChanged, AddressOf OnConcPropChanged
        AddHandler Me.m_propConcSpaceOn.PropertyChanged, AddressOf OnConcPropChanged

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.Core, eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace}

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

        RemoveHandler Me.m_propConcSimOn.PropertyChanged, AddressOf OnConcPropChanged
        RemoveHandler Me.m_propConcSpaceOn.PropertyChanged, AddressOf OnConcPropChanged
        Me.m_propConcSimOn = Nothing
        Me.m_propConcSpaceOn = Nothing

        Me.m_lbGroups.Detach()
        Me.m_zgh.Detach()
        Me.m_zgh = Nothing

        MyBase.OnFormClosed(e)

    End Sub

    Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

        ' JS10Apr10: this probably needs to be refined to ONLY include run completed states
        If (msg.Source = eCoreComponentType.EcoSim) Or _
           (msg.Source = eCoreComponentType.EcoSpace) Then
            'let the interface update to all core states
            Me.RefreshData()
        End If

        If (msg.Source = eCoreComponentType.Core And msg.Type = eMessageType.GlobalSettingsChanged) Then
            Me.m_cbAutosaveResults.Checked = Me.Core.Autosave(eAutosaveTypes.Ecotracer)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="ScientificInterfaceShared.Forms.frmEwE.IsRunForm" />
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property IsRunForm() As Boolean
        Get
            Return True
        End Get
    End Property

#End Region ' Form overrides

#Region " Events "

    Private Sub OnGroupSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_lbGroups.SelectedIndexChanged
        Me.PlotSelectedGroups()
    End Sub

    Protected Overrides Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)
        If ((changeType And cStyleGuide.eChangeType.Colours) > 0) Then
            ' Respond to group colour changes
            Me.PlotSelectedGroups()
        End If
    End Sub

    Private Sub OnRunEcosim(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnRunSim.Click

        Try
            'An Ecosim scenario was loaded when this form was loaded
            'so there is no need to check
            Me.m_bInUpdate = True
            Me.Core.EcoSimModelParameters.ContaminantTracing = True
            Me.m_bInUpdate = False
            Me.StartModelRun()
            Me.Core.RunEcoSim(AddressOf Me.EcosimCallback)
            ' Restore state
            Me.RefreshGraph()

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".btRunSim_Click() Error: " & ex.Message)
        End Try

    End Sub

    Private Sub OnRunEcospace(ByVal sender As Object, ByVal e As System.EventArgs) _
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
                Me.m_bInUpdate = True
                Me.Core.EcospaceModelParameters.ContaminantTracing = True
                Me.m_bInUpdate = False
                Me.StartModelRun()
                Me.Core.RunEcoSpace(AddressOf Me.EcospaceCallback)
                Me.RefreshGraph()
            End If

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".btRunSpace_Click() Error: " & ex.Message)
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when a plot type radio button checked state has changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnPlotTypeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_rbConc.CheckedChanged, m_rbCB.CheckedChanged

        If (Me.m_DisplayHelper Is Nothing) Then Return

        If Me.m_rbConc.Checked Then
            Me.PlotType = ePlotTypes.Conc
        ElseIf Me.m_rbCB.Checked Then
            Me.PlotType = ePlotTypes.CB
        End If

    End Sub

    Private Sub OnRegionSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbRegions.SelectedIndexChanged
        Me.RefreshGraph()
    End Sub

    Private Sub OnSortedChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        Me.RefreshData()
    End Sub

    Private Sub OnDisplayGroups(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnShowHideGroups.Click
        Dim cmd As cCommand = Me.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
        Debug.Assert(cmd IsNot Nothing, Me.ToString & ".OnDisplayGroups() DisplayGroups Command could not be found.")
        cmd.Invoke()
    End Sub

    Private Sub OnConcPropChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)

        If Me.m_bInUpdate Then Return
        Me.RefreshData()

    End Sub

    Private Sub OnAutoSaveChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cbAutosaveResults.CheckedChanged
        Try
            If (Me.UIContext Is Nothing) Then Return
            Me.Core.Autosave(eAutosaveTypes.Ecotracer) = Me.m_cbAutosaveResults.Checked
        Catch ex As Exception

        End Try
    End Sub
#End Region ' Events

#Region " Internal bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the graph plot type.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Property PlotType() As ePlotTypes
        Get
            Return Me.m_plottype
        End Get
        Set(ByVal value As ePlotTypes)
            If (value <> Me.m_plottype) Then
                Me.m_plottype = value
                Me.UpdateControls()
                Me.RefreshData()
                Me.RefreshGraph()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Prepare the UI for running Ecosim or Ecospace.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub StartModelRun()
        ' Reset progress
        cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_ECOTRACER_RUNNING)
        Me.UpdateControls()
        Me.IsRunning = True
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the progress bar in response to a model time step.
    ''' </summary>
    ''' <param name="sProgress">Progress to set [0, 1].</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateProgess(ByVal sProgress As Single)

        'the rounding is for Ecospace it never actually gets to 1
        If (Math.Round(sProgress, 3) < 0.999F) Then
            cApplicationStatusNotifier.UpdateProgress(Me.Core, My.Resources.STATUS_ECOTRACER_RUNNING, sProgress)
        Else
            cApplicationStatusNotifier.EndProgress(Me.Core)
            Me.IsRunning = False
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub UpdateControls()

        If Me.m_bInUpdate Then Return

        Me.m_bInUpdate = True

        Me.m_rbCB.Checked = (Me.PlotType = ePlotTypes.CB)
        Me.m_rbConc.Checked = (Me.PlotType = ePlotTypes.Conc)

        Me.m_cbAutosaveResults.Checked = (Me.Core.Autosave(eAutosaveTypes.Ecotracer))

        ' Config controls based on the display helper
        Me.m_zgc.GraphPane.Title.Text = Me.m_DisplayHelper.Title
        Me.m_cmbRegions.Enabled = m_DisplayHelper.EnabledForSpace

        Me.m_btnRunSim.Enabled = (Not Me.IsRunning)
        Me.m_btnRunSpace.Enabled = (Not Me.IsRunning)

        Me.m_bInUpdate = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the current display mode.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private ReadOnly Property DisplayMode() As eDisplayModeTypes
        Get
            Dim mode As eDisplayModeTypes = eDisplayModeTypes.NoResults

            'Ecosim selected
            If Me.Core.EcoSimModelParameters.ContaminantTracing Then
                mode = eDisplayModeTypes.Ecosim
            End If

            'Ecospace
            'this is nested because EcospaceModelParameters will be Null if an Ecospace scenario has not been loaded
            If Me.Core.StateMonitor.HasEcospaceLoaded Then
                If Me.Core.EcospaceModelParameters.ContaminantTracing Then
                    mode = eDisplayModeTypes.Ecospace
                End If
            End If

            Return mode
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Re-populate the interface 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub RefreshData()

        If Me.UIContext Is Nothing Then Return

        Dim modeNew As eDisplayModeTypes = Me.DisplayMode

        Try
            'build the correct display mode helper based on the new display mode flag from getDisplayMode
            Me.m_DisplayHelper = Me.DisplayHelperFactory(modeNew)

            'refresh the display mode helper
            'if no new displayhelper was built this will refresh the current one based on the core state
            Me.m_DisplayHelper.Refresh()

            'keep the display mode for next time 
            'it is used in DisplayHelperFactory()
            Me.m_curDisplayMode = modeNew

            Me.UpdateControls()
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Redraw the graph
    ''' </summary>
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
        Me.PlotSelectedGroups()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub PlotSelectedGroups()

        Dim lLinesPlot As New List(Of LineItem)
        Dim aLinesGroup() As LineItem = Nothing
        Dim source As cCoreInputOutputBase = Nothing

        ' ToDo_JS: validate tracer run status. This needs extending the core state monitor

        If Not Me.m_DisplayHelper.bCanPlot Then Return

        Try

            ' Iterate over all selected listbox items
            For Each iListboxItem As Integer In Me.m_lbGroups.SelectedIndices
                ' Get source at this item
                source = Me.m_lbGroups.GetGroupAt(iListboxItem)
                ' Is environment node?
                If (source Is Nothing) Then
                    ' #Yes: get environment lines
                    aLinesGroup = Me.m_DisplayHelper.GetGroupLines(0)
                Else
                    ' #No: get group lines
                    aLinesGroup = Me.m_DisplayHelper.GetGroupLines(source.Index)
                End If
                ' Add all lines
                For Each li As LineItem In aLinesGroup
                    ' Is a line?
                    If (li IsNot Nothing) Then
                        ' #Yes: add it
                        lLinesPlot.Add(li)
                    End If
                Next
            Next

            ' Plot all encountered lines
            Me.m_zgh.PlotLines(lLinesPlot.ToArray, , , , False)

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private Sub EcosimCallback(ByVal iTime As Long, ByVal data As cEcoSimResults)
        Try
            If (iTime Mod cCore.N_MONTHS) = 0 Then
                Me.UpdateProgess(CSng(iTime / Me.m_DisplayHelper.nStepPerYear))
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Private Sub EcospaceCallback(ByRef EcospaceResults As cEcospaceTimestep)
        Try
            Me.UpdateProgess(CSng(EcospaceResults.TimeStepinYears / Me.m_DisplayHelper.nYears))
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

#Region " Helper methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a Display mode helper object based on the newDisplayMode parameter
    ''' </summary>
    ''' <param name="newDisplayMode"></param>
    ''' <returns>If the current display mode matches the newDisplayMode parameter this will return the current IDisplayModeHelper object</returns>
    ''' -----------------------------------------------------------------------
    Private Function DisplayHelperFactory(ByVal newDisplayMode As eDisplayModeTypes) As IDisplayModeHelper

        'This will only build a new IDisplayModeHelper if newDisplayMode is different from the current m_curDisplayMode
        If newDisplayMode <> Me.m_curDisplayMode Then

            'build a new IDisplayModeHelper object
            Select Case newDisplayMode
                Case eDisplayModeTypes.NoResults
                    Return New cNoResultsDisplayHelper(Me.UIContext)
                Case eDisplayModeTypes.Ecosim
                    Return New cEcoSimDisplayHelper(Me.UIContext, Me.m_zgh)
                Case eDisplayModeTypes.Ecospace
                    Return New cEcoSpaceDisplayHelper(Me.UIContext, Me.m_zgh)
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

#End Region ' Overrides

#Region "Display Mode Helper Classes"

#Region "Interface definition"

    ''' =======================================================================
    ''' <summary>
    ''' Interface for an Ecotracer display mode helper implementation.
    ''' </summary>
    ''' =======================================================================
    Private Interface IDisplayModeHelper
        Inherits IUIElement

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the line(s) to draw on the graph for a single group.
        ''' </summary>
        ''' <param name="iGroup">Index of the group to get line(s) for.</param>
        ''' <remarks>For Ecospace results, lines may be returned for every 
        ''' relevant region.</remarks>
        ''' -------------------------------------------------------------------
        Function GetGroupLines(ByVal iGroup As Integer) As LineItem()

        Function GetGroupMax(ByVal iGroup As Integer) As Single

        ''' <summary>Update the object base on the current core run state.</summary>
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

        ''' <summary>
        ''' Number of time steps per year in the current model
        ''' </summary>
        ReadOnly Property nStepPerYear() As Integer


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

        Public Function GetGroupLines(ByVal iGroup As Integer) As LineItem() _
            Implements IDisplayModeHelper.GetGroupLines
            Debug.Assert(False, Me.ToString & ".GetGroupLine() Warning this should not be called!")
            Return New LineItem() {}
        End Function

        Public Function GetGroupMax(ByVal iGroup As Integer) As Single _
            Implements IDisplayModeHelper.GetGroupMax
            Return 0.0
        End Function

        Public ReadOnly Property Enabled() As Boolean _
            Implements IDisplayModeHelper.Enabled
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Title() As String _
            Implements IDisplayModeHelper.Title
            Get
                Return SharedResources.GENERIC_VALUE_NO_DATA_AVAILABLE
            End Get
        End Property

        Public Sub Refresh() _
            Implements IDisplayModeHelper.Refresh
            ' NOP
        End Sub

        Public ReadOnly Property nYears() As Integer _
            Implements IDisplayModeHelper.nYears
            Get
                Return 1
            End Get
        End Property

        Public Property PlotType() As ePlotTypes _
            Implements IDisplayModeHelper.PlotType
            Get
                Return ePlotTypes.NotSet
            End Get
            Set(ByVal value As ePlotTypes)
                ' NOP
            End Set
        End Property

        Public WriteOnly Property RegionIndex() As Integer _
            Implements IDisplayModeHelper.RegionIndex
            Set(ByVal value As Integer)
                ' NOP
            End Set
        End Property

        Public ReadOnly Property EnabledForSpace() As Boolean _
            Implements IDisplayModeHelper.EnabledForSpace
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property XAxisLabel() As String _
            Implements IDisplayModeHelper.XAxisLabel
            Get
                ' ToDo: Globalize this
                Return "X Axis"
            End Get
        End Property

        Public ReadOnly Property YAxisLabel() As String _
            Implements IDisplayModeHelper.YAxisLabel
            Get
                ' ToDo: Globalize this
                Return "Y Axis"
            End Get
        End Property

        Public ReadOnly Property bCanPlot() As Boolean _
            Implements IDisplayModeHelper.bCanPlot
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property nStepPerYear() As Integer Implements IDisplayModeHelper.nStepPerYear
            Get
                Return 0
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
        Private m_zgh As cZedGraphHelper

        Sub New(ByRef uic As cUIContext, ByVal ZedGraphHelper As cZedGraphHelper)
            ' Sanity check
            Debug.Assert(uic IsNot Nothing)
            Me.UIContext = uic
            Me.m_bEnabled = False
            Me.m_zgh = ZedGraphHelper
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
            Dim strLabel As String = SharedResources.HEADER_ENVIRONMENT
            Dim clrLine As Color = Color.Black
            Dim yVal As Double
            Dim dPos As Double

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
                    dPos = Me.Core.EcosimFirstYear + (iTimeStep / cCore.N_MONTHS)
                    yVal = CDbl(td.Concentration(iGroup, iTimeStep) / SimBio.Biomass(iTimeStep))
                    vList.Add(dPos, yVal)
                Next iTimeStep

            Else

                For iTimeStep As Integer = 1 To Me.Core.nEcosimTimeSteps
                    dPos = Me.Core.EcosimFirstYear + (iTimeStep / cCore.N_MONTHS)
                    yVal = CDbl(td.Concentration(iGroup, iTimeStep))
                    vList.Add(dPos, yVal)
                Next iTimeStep

            End If

            Return Me.m_zgh.CreateLineItem(strLabel, eLineType.ModelData, clrLine, vList)

        End Function

        Public Function GetGroupLines(ByVal iGroup As Integer) As LineItem() _
            Implements IDisplayModeHelper.GetGroupLines

            Return New LineItem() {buildLine(iGroup)}

        End Function

        Public Function GetGroupMax(ByVal iGroup As Integer) As Single _
            Implements IDisplayModeHelper.GetGroupMax

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
                    Return My.Resources.GENERIC_ECOSIM
                Else
                    Return My.Resources.GENERIC_ECOSIM_NO_DATA_AVAILABLE
                End If

            End Get
        End Property

        Public Sub Refresh() _
            Implements IDisplayModeHelper.Refresh

            Me.m_bEnabled = False

            'make sure Ecosim is the selected model and it has run
            If Me.Core.EcoSimModelParameters.ContaminantTracing And Me.Core.StateMonitor.HasEcosimRan Then
                Me.m_bEnabled = True
            End If

        End Sub

        Public ReadOnly Property nYears() As Integer _
            Implements IDisplayModeHelper.nYears
            Get
                Return Me.Core.nEcosimYears
            End Get
        End Property

        Public Property PlotType() As ePlotTypes _
            Implements IDisplayModeHelper.PlotType
            Get
                Return Me.m_plottype
            End Get
            Set(ByVal value As ePlotTypes)
                Me.m_plottype = value
            End Set
        End Property

        Public WriteOnly Property RegionIndex() As Integer _
            Implements IDisplayModeHelper.RegionIndex
            Set(ByVal value As Integer)
                'ecosim does not use regions
            End Set
        End Property

        Public ReadOnly Property EnabledForSpace() As Boolean _
            Implements IDisplayModeHelper.EnabledForSpace
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property XAxisLabel() As String _
            Implements IDisplayModeHelper.XAxisLabel
            Get
                Return SharedResources.HEADER_ECOSIM_YEARS
            End Get
        End Property

        Public ReadOnly Property YAxisLabel() As String _
            Implements IDisplayModeHelper.YAxisLabel
            Get
                Dim lb As String

                If Me.m_plottype = ePlotTypes.CB Then
                    lb = SharedResources.HEADER_CONCENTRATION_OVER_B
                Else
                    lb = SharedResources.HEADER_CONCENTRATION
                End If
                Return lb
            End Get
        End Property

        Public ReadOnly Property bCanPlot() As Boolean _
            Implements IDisplayModeHelper.bCanPlot
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property nStepPerYear() As Integer Implements IDisplayModeHelper.nStepPerYear
            Get
                Return cCore.N_MONTHS
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
        Private m_zgh As cZedGraphHelper

        Sub New(ByVal uic As cUIContext, ByVal ZedGraphHelper As cZedGraphHelper)
            ' Sanity check
            Debug.Assert(uic IsNot Nothing)
            Me.UIContext = uic
            Me.m_zgh = ZedGraphHelper
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

        Public Function GetGroupLines(ByVal iGroup As Integer) As LineItem() _
            Implements IDisplayModeHelper.GetGroupLines

            If iGroup < 0 Then Return Nothing ' Safety first

            Dim lstLines As New List(Of LineItem)

            'm_rgn1 and m_rgn2 were set in RegionIndex
            For ireg As Integer = Me.m_rgn1 To Me.m_rgn2
                lstLines.Add(buildLine(iGroup, ireg))
            Next

            Return lstLines.ToArray

        End Function

        Private Function buildLine(ByVal iGroup As Integer, ByVal iRegion As Integer) As LineItem

            Dim td As cEcotracerRegionGroupOutput = Me.Core.EcotracerRegionGroupResults
            Dim list As PointPairList
            Dim clrLine As Color = Color.Black
            Dim strFilter As String = ""
            Dim strRegionName As String = ""
            Dim strLabel As String
            Dim dPos As Double
            Dim sY As Single
            Dim ntsYear As Single

            ntsYear = Me.Core.EcospaceModelParameters.NumberOfTimeStepsPerYear

            ' Build the line label
            If iGroup > 0 Then
                strFilter = Me.Core.EcoPathGroupInputs(iGroup).Name
                clrLine = Me.StyleGuide.GroupColor(Me.Core, iGroup)
            Else
                strFilter = SharedResources.HEADER_ENVIRONMENT
            End If

            If iRegion > 0 Then
                strLabel = String.Format(SharedResources.GENERIC_LABEL_DETAILED, strFilter, "Region")
            Else
                strLabel = strFilter
            End If

            'this will figure out which varname to display 
            'base on the selected group and the ePlotTypes enum
            Dim varName As eVarNameFlags = getVarName(iGroup)

            list = New PointPairList()

            For iTimeStep As Integer = 1 To Me.Core.nEcospaceTimeSteps
                dPos = Me.Core.EcosimFirstYear + (iTimeStep / ntsYear)
                sY = td.GetVariable(varName, iRegion, iGroup, iTimeStep)
                list.Add(dPos, CDbl(sY))
            Next iTimeStep

            '  Return New LineItem(strLabel, list, clrLine, SymbolType.None, 1)
            Return Me.m_zgh.CreateLineItem(strLabel, eLineType.ModelData, clrLine, list)

        End Function

        ''' <summary>
        ''' Get the correct variable to display based on the selected Group and the ePlotTypes
        ''' </summary>
        ''' <param name="iGroup"></param>
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
                    Return My.Resources.GENERIC_ECOSPACE
                Else
                    Return My.Resources.GENERIC_ECOSPACE_NO_DATA_AVAILABLE
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

        Public ReadOnly Property XAxisLabel() As String Implements IDisplayModeHelper.XAxisLabel
            Get
                Return SharedResources.HEADER_ECOSPACE_YEARS
            End Get
        End Property

        Public ReadOnly Property YAxisLabel() As String Implements IDisplayModeHelper.YAxisLabel

            Get
                Dim lb As String
                If Me.m_plottype = ePlotTypes.CB Then
                    lb = SharedResources.HEADER_CONCENTRATION_OVER_B
                Else
                    lb = SharedResources.HEADER_CONCENTRATION
                End If
                Return lb
            End Get

        End Property

        Public ReadOnly Property bCanPlot() As Boolean Implements IDisplayModeHelper.bCanPlot
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property nStepPerYear() As Integer Implements IDisplayModeHelper.nStepPerYear
            Get
                Try
                    Return Me.Core.nEcospaceTimeSteps \ Me.Core.nEcospaceYears
                Catch ex As Exception
                    Return 0
                End Try
            End Get
        End Property
    End Class

#End Region

#End Region

End Class