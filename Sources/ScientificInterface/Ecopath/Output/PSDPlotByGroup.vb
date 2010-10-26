#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ZedGraph
Imports EwEUtils.Core

#End Region 'Imports

Namespace Ecopath.Output

    Public Class PSDPlotByGroup

#Region " Variables "

        Private m_zgh As cZedGraphHelper = Nothing
        Private m_Time() As Single
        Private m_Weight() As Single
        Private m_Number() As Single
        Private m_Biomass() As Single

        Private Enum ePaneTypes As Integer
            Weight = 0
            Number
            Biomass
            PSD
            LorenzenMortality
        End Enum

#End Region ' Variables

#Region " Constructor "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region "Event handlers"

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            Debug.Assert(Me.UIContext IsNot Nothing)

            Dim parms As cPSDParameters = Me.Core.ParticleSizeDistributionParameters
            Dim unitTime As cStyleGuide.eUnitType() = New cStyleGuide.eUnitType() {cStyleGuide.eUnitType.Time}

            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_graph)
            Me.m_zgh.Configure("")

            Me.m_lbGroups.Attach(Me.UIContext)

            Me.m_graph.MasterPane.PaneList.Clear()

            Me.CreatePane(ePaneTypes.Weight, My.Resources.HEADER_WEIGHT, _
                          My.Resources.HEADER_AGE_UNIT, unitTime, My.Resources.HEADER_G, Nothing)
            Me.CreatePane(ePaneTypes.Number, My.Resources.HEADER_SURVIVAL, _
                          My.Resources.HEADER_AGE_UNIT, unitTime, "", Nothing)
            Me.CreatePane(ePaneTypes.Biomass, My.Resources.HEADER_BIOMASS, _
                          My.Resources.HEADER_AGE_UNIT, unitTime, _
                          My.Resources.HEADER_G, Nothing)
            Me.CreatePane(ePaneTypes.PSD, My.Resources.HEADER_CONTRIBUTION_TO_PSD, _
                          My.Resources.HEADER_BODYWEIGHT_LOGg, My.Resources.HEADER_BIOMASS_LOGg)

            If parms.MortalityType = ePSDMortalityTypes.Lorenzen Then
                Me.CreatePane(ePaneTypes.LorenzenMortality, My.Resources.HEADER_MORTALITY, _
                              My.Resources.HEADER_AGE_UNIT, unitTime, _
                              My.Resources.HEADER_PER_UNIT, unitTime)
            End If

            Me.m_lbGroups.SelectedIndex = 0

        End Sub

        Private Sub llbGroups_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_lbGroups.SelectedIndexChanged
            AddCurves()
            UpdatePlots()
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
            Me.m_zgh.Detach()
            Me.m_zgh = Nothing
            Me.m_lbGroups.Detach()
            MyBase.OnFormClosed(e)
        End Sub

#End Region 'Event handlers

#Region "Helper methods"

        Private Sub CreatePane(ByVal PaneNo As ePaneTypes, _
                               ByVal strPaneTitle As String, _
                               ByVal strXaxisTitle As String, _
                               ByVal strYaxisTitle As String)

            Me.CreatePane(PaneNo, strPaneTitle, strXaxisTitle, Nothing, strYaxisTitle, Nothing)

        End Sub

        Private Sub CreatePane(ByVal PaneNo As ePaneTypes, _
                       ByVal strPaneTitle As String, _
                       ByVal strXaxisTitle As String, ByVal aUnitsXAxis() As cStyleGuide.eUnitType, _
                       ByVal strYaxisTitle As String, ByVal aUnitsYAxis() As cStyleGuide.eUnitType)

            'Define a new graph pane
            Dim pane As New GraphPane

            Debug.Assert(Me.m_graph.MasterPane.PaneList.Count = PaneNo)

            'Add the graphPane to the masterPane
            Me.m_graph.MasterPane.Add(pane)

            Me.InitGraphPane(strPaneTitle, strXaxisTitle, aUnitsXAxis, strYaxisTitle, aUnitsYAxis, PaneNo, CInt(PaneNo) + 1)

        End Sub

        Private Sub InitGraphPane(ByVal strPaneTitle As String, _
                                  ByVal strXAxisTitle As String, ByVal aUnitsXAxis() As cStyleGuide.eUnitType, _
                                  ByVal strYAxisTitle As String, ByVal aUnitsYAxis() As cStyleGuide.eUnitType, _
                                  ByVal paneType As ePaneTypes, _
                                  ByVal iPane As Integer)

            Dim parms As cPSDParameters = Me.Core.ParticleSizeDistributionParameters
            Dim gp As GraphPane = Me.m_zgh.ConfigurePane(strPaneTitle, _
                                                         strYAxisTitle, aUnitsYAxis, _
                                                         strXAxisTitle, aUnitsXAxis, _
                                                         False, LegendPos.TopCenter, iPane)

            Select Case paneType
                Case ePaneTypes.PSD
                    gp.XAxis.Scale.Min = Int(Math.Log10(parms.FirstWeightClass))
                    gp.XAxis.Scale.Max = Math.Round(Math.Log10(parms.FirstWeightClass * 2 ^ (Me.Core.nWeightClasses - 1)) + 0.4, 0, MidpointRounding.AwayFromZero)
                    gp.YAxis.Scale.Min = 0
                    'gp.YAxis.Scale.Max = 8 if PSDPlotByGroup has the same scale as that of PSDContributionPlot
                Case Else
                    gp.XAxis.Scale.Min = 0
                    'gp.XAxis.Scale.Max = CDbl(Me.Core.EcosimFirstYear + (Me.Core.nEcosimTimeSteps / cCore.N_MONTHS))
                    gp.YAxis.Scale.Min = 0
            End Select

        End Sub

        Private Sub AddCurves()

            'Add single curve into graph first
            'Results data structure

            Dim parms As cPSDParameters = Me.Core.ParticleSizeDistributionParameters
            Dim resultLists As New List(Of PointPairList)
            Dim sXValue As Single = 0
            Dim grpOutput As cEcoPathGroupOutput = Nothing
            Dim sSystemPSD(Me.Core.nWeightClasses) As Single
            Dim iSelectedGrpNum As Integer = 1

            'Find the selected group number based on the selected index
            For iGroup As Integer = 1 To Me.Core.nLivingGroups
                If Me.Core.EcoPathGroupOutputs(iGroup).Name = m_lbGroups.Items(m_lbGroups.SelectedIndex).ToString() Then
                    iSelectedGrpNum = iGroup
                    Exit For
                End If
            Next

            grpOutput = Me.Core.EcoPathGroupOutputs(iSelectedGrpNum)
            Select Case parms.MortalityType
                Case ePSDMortalityTypes.GroupZ
                    InitLists(resultLists, 4)
                Case ePSDMortalityTypes.Lorenzen
                    InitLists(resultLists, 5)
            End Select

            For iTimeStep As Integer = 1 To Me.Core.nAgeSteps

                sXValue = (iTimeStep - 1) * grpOutput.TmaxOutput / (Me.Core.nAgeSteps - 1)

                'Weight plot
                If grpOutput.EcopathWeight(iTimeStep) > 0 Then
                    resultLists(0).Add(sXValue, grpOutput.EcopathWeight(iTimeStep))
                End If
                'Number plot
                If grpOutput.EcopathNumber(iTimeStep) > 0 Then
                    resultLists(1).Add(sXValue, grpOutput.EcopathNumber(iTimeStep))
                End If
                'Biomass plot
                If grpOutput.EcopathBiomass(iTimeStep) > 0 Then
                    resultLists(2).Add(sXValue, grpOutput.EcopathBiomass(iTimeStep))
                End If
                'Lorenzen mortality plot if mortality type is Lorenzen
                If parms.MortalityType = ePSDMortalityTypes.Lorenzen Then
                    If grpOutput.LorenzenMortality(iTimeStep) > 0 Then
                        resultLists(4).Add(sXValue, grpOutput.LorenzenMortality(iTimeStep))
                    End If
                End If
            Next

            'Find the system PSD by summing the group PSD
            FindSystemPSD(sSystemPSD)

            For iWtClass As Integer = 1 To Me.Core.nWeightClasses
                sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))
                If sSystemPSD(iWtClass) * 1000000000 > 0 Then
                    'group contribution to the system PSD is Math.Log10(sSystemPSD(iWtClass) * 1000000000) * grpOutput.PSD(iWtClass) / sSystemPSD(iWtClass)
                    '* 1000000000 for plotting purpose
                    resultLists(3).Add(Math.Log10(sXValue), Math.Log10(sSystemPSD(iWtClass) * 1000000000) * grpOutput.PSD(iWtClass) / sSystemPSD(iWtClass))
                Else
                    resultLists(3).Add(Math.Log10(sXValue), 0)
                End If
            Next

            'Set the master pane title
            Me.m_graph.MasterPane.Title.Text = CStr(m_lbGroups.SelectedItem.ToString)

            ' Clear all panes
            For Each gp As GraphPane In Me.m_graph.MasterPane.PaneList
                gp.CurveList.Clear()
            Next

            AddCurveToGraphPane(ePaneTypes.Weight, resultLists(0), Me.StyleGuide.GroupColor(Me.Core, iSelectedGrpNum - 1))
            AddCurveToGraphPane(ePaneTypes.Number, resultLists(1), Me.StyleGuide.GroupColor(Me.Core, iSelectedGrpNum - 1))
            AddCurveToGraphPane(ePaneTypes.Biomass, resultLists(2), Me.StyleGuide.GroupColor(Me.Core, iSelectedGrpNum - 1))
            AddCurveToGraphPane(ePaneTypes.PSD, resultLists(3), Me.StyleGuide.GroupColor(Me.Core, iSelectedGrpNum - 1))

            'Lorenzen mortality plot if mortality type is Lorenzen
            If parms.MortalityType = ePSDMortalityTypes.Lorenzen Then
                AddCurveToGraphPane(ePaneTypes.LorenzenMortality, resultLists(4), Me.StyleGuide.GroupColor(Me.Core, iSelectedGrpNum - 1))
            End If
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Sub AddCurveToGraphPane(ByVal paneType As ePaneTypes, ByVal list As PointPairList, ByVal clr As Color)
            Dim gp As GraphPane = Me.m_graph.MasterPane.PaneList(CInt(paneType))
            Dim brItem As BarItem

            Select Case paneType
                Case ePaneTypes.PSD
                    brItem = gp.AddBar(gp.Title.Text, list, clr)
                    brItem.Bar.Fill = New Fill(clr)
                Case Else
                    gp.AddCurve(gp.Title.Text, list, clr, SymbolType.None)
            End Select
        End Sub

        Private Sub UpdatePlots()
            Me.m_graph.AxisChange()

            'Tell ZedGraph to auto layout the new GraphPanes
            'Cannot move that part up to the InitMasterPane, Title is dynamic here..??
            Dim g As Graphics = Me.CreateGraphics()
            Me.m_graph.MasterPane.SetLayout(g, PaneLayout.SquareColPreferred)
            g.Dispose()

            Me.m_graph.Refresh()
        End Sub

        Private Sub FindSystemPSD(ByVal sSystemPSD() As Single)
            Dim grpOutput As cEcoPathGroupOutput = Nothing

            'Find the system PSD by summing the group PSD
            For iGroup As Integer = 1 To Me.Core.nLivingGroups
                grpOutput = Me.Core.EcoPathGroupOutputs(iGroup)
                For iWtClass As Integer = 1 To Me.Core.nWeightClasses
                    sSystemPSD(iWtClass) = sSystemPSD(iWtClass) + grpOutput.PSD(iWtClass)
                Next
            Next
        End Sub

#End Region 'Helper methods

    End Class

End Namespace