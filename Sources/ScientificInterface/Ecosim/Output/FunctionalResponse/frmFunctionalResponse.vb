#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ZedGraph
Imports EwEUtils.Core
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Ecosim

    Public Class frmFunctionalResponsePlot

#Region " Helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected Class cGroupItem
            Inherits cCoreInputOutputControlItem

            ''' <summary>Optional color for an item.</summary>
            Private m_color As Color = Color.Transparent

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Creates a new item for usage in the GroupListBox.
            ''' </summary>
            ''' <param name="group">Group to link to.</param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal group As cEcoPathGroupInput)
                MyBase.New(group)
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Creates a new item for usage in the GroupListBox.
            ''' </summary>
            ''' <param name="strLabel">Name to display for a non-group item.</param>
            ''' <param name="color">Color for this item, if any.</param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal strLabel As String, ByVal color As Color)
                MyBase.New(strLabel)
                Me.m_color = color
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Gets the group linked to the item.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Shadows ReadOnly Property Source() As cEcoPathGroupInput
                Get
                    Return DirectCast(MyBase.Source, cEcoPathGroupInput)
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Hard-coded color for an item.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property Color() As Color
                Get
                    Return Me.m_color
                End Get
                Set(ByVal value As Color)
                    Me.m_color = value
                End Set
            End Property

        End Class

#End Region ' Helper classes

#Region " Private vars "

        Private m_graphpane As GraphPane = Nothing
        Private m_bEcosimRunning As Boolean = False
        Private m_coreStateMonitor As cCoreStateMonitor = Nothing
        Private m_mhEcosim As cMessageHandler = Nothing
        Private m_zgh As cZedGraphHelper = Nothing

#End Region ' Private vars

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_coreStateMonitor = Me.Core.StateMonitor
            Me.m_graphpane = Me.m_plot.GraphPane
            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_plot)

            Me.m_lbPrey.Attach(Me.UIContext)

            Dim m_SyncObj As System.Threading.SynchronizationContext = System.Threading.SynchronizationContext.Current
            'if there is no current context then create a new one on this thread. 
            If (m_SyncObj Is Nothing) Then m_SyncObj = New System.Threading.SynchronizationContext()

            ' Start listening for core messages
            Me.m_mhEcosim = New cMessageHandler(AddressOf EcosimMessageHandler, eCoreComponentType.EcoSim, eMessageType.Any, m_SyncObj)
            Me.Core.Messages.AddMessageHandler(Me.m_mhEcosim)

            Dim cmd As cCommand = Me.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnShowGroups)

            AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            ' Kick off
            Me.PopulateGroupCombo()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Dim cmd As cCommand = Me.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnShowGroups)

            Me.Core.Messages.RemoveMessageHandler(Me.m_mhEcosim)
            Me.m_mhEcosim = Nothing

            Me.m_zgh.Detach()
            Me.m_lbPrey.Detach()

            RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            MyBase.OnFormClosed(e)

        End Sub

        ''' <summary>
        ''' Keep me open, please!
        ''' </summary>
        Public Overrides ReadOnly Property IsRunForm() As Boolean
            Get
                Return True
            End Get
        End Property

        Private Sub TimeStepFromEcoSim_handler(ByVal iTime As Long, ByVal results As cEcoSimResults)
            'Me.RefreshPlot()
        End Sub

        Private Sub OnStyleGuideChanged(ByVal change As cStyleGuide.eChangeType)

            If (change And cStyleGuide.eChangeType.GroupVisibility) > 0 Then
                Me.PopulateGroupCombo()
                Me.RefreshPlot(True)
                Return
            End If

            If (change And cStyleGuide.eChangeType.Colours) > 0 Then
                Me.RefreshPlot(False)
            End If

        End Sub

        Private Sub EcosimMessageHandler(ByRef msg As cMessage)
            Try
                Select Case msg.Type
                    Case eMessageType.EcosimRunCompleted
                        Me.RefreshPlot(True)
                End Select
            Catch ex As Exception
                cLog.Write(ex)
            End Try
        End Sub

        Private Sub OnSelectedPredatorChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tscmConsumers.SelectedIndexChanged
            Try
                Me.RefreshPlot(True)
            Catch ex As Exception
            End Try
        End Sub


#End Region ' Events

#Region " Internals "

        Private Property SelectedGroup() As cEcoPathGroupInput
            Get
                If Me.m_tscmConsumers.SelectedIndex = -1 Then Return Nothing
                Return DirectCast(Me.m_tscmConsumers.SelectedItem, cGroupItem).Source
            End Get
            Set(ByVal value As cEcoPathGroupInput)
                Debug.Assert(value IsNot Nothing)
                For i As Integer = 0 To Me.m_tscmConsumers.Items.Count - 1
                    Dim item As cGroupItem = DirectCast(Me.m_tscmConsumers.Items(i), cGroupItem)
                    If (value.Index = item.Source.Index) Then
                        Me.m_tscmConsumers.SelectedIndex = i
                        Return
                    End If
                Next
                Me.m_tscmConsumers.SelectedIndex = -1
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate consumer combo box with living consumers.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub PopulateGroupCombo()

            Dim group As cEcoPathGroupInput = Nothing

            ' Reset
            Me.m_tscmConsumers.Items.Clear()

            ' For all potential consumers
            For i As Integer = 1 To Me.Core.nLivingGroups
                ' Get group
                group = Me.Core.EcoPathGroupInputs(i)
                ' Is visible consumer?
                If (group.IsConsumer) And (Me.StyleGuide.GroupVisible(i)) Then
                    ' #Yes: add 'em
                    Me.m_tscmConsumers.Items.Add(New cGroupItem(group))
                End If
            Next

            ' Set selection
            If (Me.m_tscmConsumers.Items.Count > 0) Then
                Me.m_tscmConsumers.SelectedIndex = 0
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add the curves to the pane.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub RefreshPlot(Optional ByVal bRescale As Boolean = False)

            Dim ppl As PointPairList = Nothing
            Dim pred As cEcoPathGroupInput = Me.SelectedGroup
            Dim simPrey As cEcosimGroupOutput = Nothing
            Dim simPred As cEcosimGroupOutput = Nothing
            Dim lPreys As New List(Of Integer)
            Dim lSortValues As New List(Of Single)

            Me.m_graphpane.CurveList.Clear()
            Me.m_lbPrey.Items.Clear()

            If (Not Me.Core.StateMonitor.HasEcosimRan) Then Return

            If (pred Is Nothing) Then

                Me.m_zgh.ConfigurePane(String.Format(My.Resources.FR_PLOT_CAPTION, Me.SelectedGroup.Name), _
                       My.Resources.FR_PLOT_X_AXIS, _
                       My.Resources.FR_PLOT_Y_AXS, False)

            Else

                Dim iPred As Integer = pred.Index
                Dim sElecSum As Single = 0.0!
                simPred = Me.Core.EcoSimGroupOutputs(iPred)

                ' Configure graph panel
                Me.m_zgh.ConfigurePane(String.Format(My.Resources.FR_PLOT_CAPTION, Me.SelectedGroup.Name), _
                                       My.Resources.FR_PLOT_X_AXIS, _
                                       My.Resources.FR_PLOT_Y_AXS, False)

                Me.m_graphpane.XAxis.Scale.MinAuto = True
                Me.m_graphpane.XAxis.Scale.MaxAuto = True
                Me.m_graphpane.YAxis.Scale.MinAuto = True
                Me.m_graphpane.YAxis.Scale.MaxAuto = True

                ' Populate lines for all relevant prey
                For iPrey As Integer = 1 To Me.Core.nGroups
                    ' Is a visible prey with a start biomass?
                    If (Me.StyleGuide.GroupVisible(iPrey)) And _
                       (pred.DietComp(iPrey) > 0) And _
                       (Me.Core.StartBiomass(iPrey) > 0) Then

                        ' #Yes
                        ppl = New PointPairList()
                        simPrey = Me.Core.EcoSimGroupOutputs(iPrey)
                        sElecSum = 0

                        For iTimeStep As Integer = 1 To Core.nEcosimTimeSteps

                            sElecSum += simPrey.Electivity(iPred, iTimeStep)

                            ' Orig formula was:
                            ' picXY.Line -(SimPlot(prey, 0, Tm) / StartBiomass(prey), SimPlotPred(prey, Sel, Tm) / SimPlot(Sel, 0, Tm)), color
                            '  * SimPlot(sel, 0, tm) contains BB, which is exposed in EwE6 as groupOut.Biomass
                            '  * SimPlotPred(prey, pred, tm) read from Consumpt(prey, pred), available in PredPreyResultsOverTime(prey, pred, t)

                            ppl.Add(New PointPair(simPrey.BiomassRel(iTimeStep), _
                                                  simPrey.Consumption(iPred, iTimeStep) / simPred.Biomass(iTimeStep)))

                        Next iTimeStep
                        Me.m_graphpane.AddCurve(simPrey.Name, ppl, _
                                              Me.StyleGuide.GroupColor(Me.Core, iPrey), _
                                              SymbolType.None)

                        lPreys.Add(iPrey)
                        lSortValues.Add(sElecSum)
                    End If
                Next iPrey

                ' Populate prey listbox
                Me.m_lbPrey.Sorted = False
                Me.m_lbPrey.Populate(lPreys.ToArray)
                For i As Integer = 0 To lPreys.Count - 1
                    Me.m_lbPrey.SortValue(lPreys(i)) = lSortValues(i)
                Next
                Me.m_lbPrey.Sorted = True

            End If

            If bRescale Then Me.m_plot.AxisChange()
            Me.m_plot.Refresh()

        End Sub

#End Region 'Internals

#If 0 Then ' The old code

    Private Sub MakeFuncPlot(ByVal picXY As PictureBox, ByVal Wt As Single, ByVal ht As Single, ByVal tp As Single, ByVal lt As Single)
        Dim Cnt As Integer
        Dim color As Object
        Dim MaxBio() As Single
        Dim minX As Single
        Dim maxX As Single
        Dim maxY As Single
        Dim OldX As Single
        Dim OldY As Single
        Dim prey As Integer
        Dim Tm As Integer
        Dim X As Single
        Dim Y As Single

        With picXY
            .Cls()
            .Width = Wt
            .Height = ht
            .Top = tp
            .Left = lt
            .Visible = True
        End With

        'find max x (biomass) value in Simplot(prey, 0, ntimes):
        ReDim MaxBio(Me.Core.nGroups)
        minX = 0 '10000
        For prey = 1 To Me.Core.nGroups
            If val(prey) = -10 And StartBiomass(prey) > 0 Then 'it's a prey in the top 20
                For Tm = 1 To Ntimes
                    If SimPlot(prey, 0, Tm) > MaxBio(prey) Then MaxBio(prey) = SimPlot(prey, 0, Tm)
                    If SimPlot(prey, 0, Tm) / StartBiomass(prey) < minX Then minX = SimPlot(prey, 0, Tm) / StartBiomass(prey)
                Next
                'Find the prey that has changed most relative to the Ecopath biomass:
                If MaxBio(prey) / StartBiomass(prey) > maxX Then maxX = MaxBio(prey) / StartBiomass(prey)
            End If
        Next
        'Get the scaling for the one with the max change:
        minX = (CInt((minX * 10 - 0.5))) / 10
        maxX = (CInt((maxX * 10 - 0.5))) / 10 + 0.1 'maxX + 0.1

        'Scaling for y-axis needs to be calculated, the y-axis displays Q of prey / B prey
        'pred = sel; info is saved in
        'SimPlotPred(prey, pred, itime) and SimPlotPrey(prey, pred, itime) as amounts consumed
        maxY = 0
        For prey = 1 To Me.Core.nGroups : For Tm = 1 To Ntimes
                If SimPlotPred(prey, Sel, Tm) / SimPlot(Sel, 0, Tm) > maxY Then maxY = SimPlotPred(prey, Sel, Tm) / SimPlot(Sel, 0, Tm)
            Next : Next
        maxY = maxY * 1.1

        If maxX = 0 Or maxY = 0 Then Exit Sub
    picXY.Scale (minX - 0.1 * maxX, 1.1 * maxY)-(1.1 * maxX, -0.1 * maxY)
    picXY.Line (minX, maxY)-(minX, 0)
    picXY.Line (minX, 0)-(maxX, 0)
    picXY.Line (maxX, 0)-(maxX, 0.01 * maxY)
    picXY.Line (1, 0)-(1, 0.01 * maxY)
    picXY.Line (minX, maxY)-(minX + 0.01 * maxX, maxY)
        PrintSome(picXY, minX + 0.35 * (maxX - minX), -0.04 * maxY, "Prey biomass relative to Ecopath biomass", QBColor(0))
        PrintSome(picXY, minX - 0.005 * maxX, -0.01 * maxY, Format(minX, "0.0"), QBColor(0))
        PrintSome(picXY, 1 - 0.005 * maxX, -0.01 * maxY, "1", QBColor(0))
        PrintSome(picXY, maxX - 0.015 * maxX, -0.01 * maxY, Format(maxX, "0.0"), QBColor(0))

        PrintSome(picXY, minX - 0.07 * maxX, 1.08 * maxY, "Q prey / B pred", QBColor(0))
        PrintSome(picXY, minX - 0.07 * maxX, 1.02 * maxY, Format(maxY, "0.00"), QBColor(0))
        PrintSome(picXY, minX - 0.07 * maxX, 1.02 * maxY * 2 / 3, Format(maxY * 2 / 3, "0.00"), QBColor(0))
        PrintSome(picXY, minX - 0.07 * maxX, 1.02 * maxY / 3, Format(maxY / 3, "0.00"), QBColor(0))
        PrintSome(picXY, minX - 0.07 * maxX, 0.01 * maxY, "0.00", QBColor(0))
        For prey = 1 To Me.Core.nGroups
            If val(prey) = -10 And StartBiomass(prey) > 0 Then 'it's a prey in the top 20
                OldX = SimPlot(prey, 0, 1) / StartBiomass(prey)
                OldY = SimPlotPred(prey, Sel, 1) / SimPlot(Sel, 0, 1) 'Elect(Sel, prey, 1)
                picXY.PSet(OldX, OldY)
                picXY.DrawWidth = 2
                color = PoolColor(prey)
                For Tm = 2 To Ntimes
                picXY.Line -(SimPlot(prey, 0, Tm) / StartBiomass(prey), SimPlotPred(prey, Sel, Tm) / SimPlot(Sel, 0, Tm)), color
                    ' OldX = SimPlot(prey, 0, Tm) / StartBiomass(prey)
                    ' OldY = SimPlotPred(prey, Sel, Tm) / SimPlot(Sel, 0, Tm)
                Next
            End If
        Next

    End Sub

    Private Sub PrintSome(ByVal pic As PictureBox, ByVal X As Single, ByVal Y As Single, ByVal Text As String, ByVal color As Object)
        pic.CurrentX = X
        pic.CurrentY = Y
        pic.ForeColor = color
        pic.Print(Text)
    End Sub

#End If

    End Class

End Namespace ' Ecosim
