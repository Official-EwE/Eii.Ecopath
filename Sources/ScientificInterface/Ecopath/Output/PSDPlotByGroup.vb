' =============================================================================
'
' $Log: PSDPlotByGroup.vb,v $
' Revision 1.15  2009/03/19 16:55:56  jeroens
' Renamed LegendListBox to GroupListBox
'
' Revision 1.14  2009/03/18 13:32:05  jeroens
' Uses implemented PSD classes
'
' Revision 1.13  2009/03/17 23:37:34  joeh
' Add codes for the Selected Group feature
'
' Revision 1.12  2009/03/17 02:25:49  joeh
' Add Lorenzen mortality type
'
' Revision 1.11  2009/03/16 21:37:19  joeh
' Incorporate StartTime into the computation of EcopathWeight, EcopathNumber and EcopathBiomass
'
' Revision 1.10  2009/03/14 20:10:24  joeh
' Add Contribution To PSD plot to master pane
'
' Revision 1.9  2009/03/14 18:33:12  joeh
' Change dXValue of double type to sXValue of single type
'
' Revision 1.8  2009/03/13 21:37:42  joeh
' Rename nEcopathTimeSteps to nAgeSteps
'
' Revision 1.7  2009/03/12 23:51:06  joeh
' Add codes for tabulation of PSD contribution data
'
' Revision 1.6  2009/03/12 01:50:29  joeh
' Add codes for PSD histogram (PSDContributionPlot)
'
' Revision 1.5  2009/03/11 00:14:28  joeh
' Add PSD calculation
'
' Revision 1.4  2009/03/06 17:54:01  joeh
' Minor changes in the computation of Weight, Number and Biomass
'
' Revision 1.3  2009/03/06 00:47:57  joeh
' Add Ecopath output data (Weight, Number, Biomass) over time
'
' Revision 1.2  2009/02/21 00:23:07  jeroens
' Added headers
'
' =============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands
Imports ZedGraph
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    Public Class PSDPlotByGroup

#Region "Variables"
        Private m_core As cCore = Nothing
        Private m_MasterPane As MasterPane
        Private m_zgh As ZedGraphHelper = Nothing
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

#End Region 'Variables

#Region "Constructor"
        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_core = cCore.GetInstance()
            Me.m_MasterPane = New MasterPane
            Me.m_zgh = New ZedGraphHelper(Me.zgcZedGraphCntl)
        End Sub
#End Region 'Constructor

#Region "Event handlers"

        Private Sub PSDPlotByGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            PopulateGroupBoxes()
            InitMasterPane()

            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters

            CreatePane(ePaneTypes.Weight, My.Resources.HEADER_WEIGHT)
            CreatePane(ePaneTypes.Number, My.Resources.HEADER_SURVIVAL)
            CreatePane(ePaneTypes.Biomass, My.Resources.HEADER_BIOMASS)
            CreatePane(ePaneTypes.PSD, My.Resources.HEADER_CONTRIBPSD)
            If parms.MortalityType = ePSDMortalityTypes.Lorenzen Then
                CreatePane(ePaneTypes.LorenzenMortality, My.Resources.HEADER_MORTALITY)
            End If
            llbGroups.SelectedIndex = 0
        End Sub

        Private Sub llbGroups_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles llbGroups.SelectedIndexChanged
            AddCurves()
            UpdatePlots()
        End Sub
#End Region 'Event handlers

#Region "Helper methods"
        Private Sub PopulateGroupBoxes()

            Dim group As cEcoPathGroupInput = Nothing

            llbGroups.SuspendLayout()
            llbGroups.Items.Clear()
            'llbGroups.Items.Add(New LegendListBox.EcopathGroupItem(Nothing))

            For i As Integer = 1 To m_core.nLivingGroups
                group = Me.m_core.EcoPathGroupInputs(i)
                If group.PSDIncluded Then
                    llbGroups.Items.Add(New GroupListBox.GroupItem(m_core.EcoPathGroupInputs(i)))
                End If
            Next

            llbGroups.ResumeLayout()
        End Sub

        Private Sub InitMasterPane()
            'Get the master pane
            m_MasterPane = Me.zgcZedGraphCntl.MasterPane

            m_MasterPane.PaneList.Clear()
            'Disable the master pane legend
            m_MasterPane.Legend.IsVisible = False
            'Make the border invisible
            m_MasterPane.Border.IsVisible = False

            m_MasterPane.Title.IsVisible = True
            m_MasterPane.Title.FontSpec.Size = 12
            m_MasterPane.IsFontsScaled = False
        End Sub

        Private Sub CreatePane(ByVal PaneNo As ePaneTypes, ByVal strTitle As String)
            'Define a new graph pane
            Dim pane As New GraphPane

            Debug.Assert(m_MasterPane.PaneList.Count = PaneNo)

            InitGraphPane(strTitle, PaneNo, pane)

            'Add the graphPane to the masterPane
            m_MasterPane.Add(pane)
        End Sub

        Private Sub InitGraphPane(ByVal strTitle As String, ByVal paneType As ePaneTypes, ByRef pane As GraphPane)

            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters

            pane.Title.Text = strTitle
            pane.Title.FontSpec.IsBold = True
            pane.Title.FontSpec.Size = 12

            pane.XAxis.Scale.FontSpec.Size = 12
            pane.XAxis.Title.FontSpec.Size = 12

            pane.YAxis.Scale.FontSpec.Size = 12
            pane.YAxis.Title.FontSpec.Size = 12

            Select Case paneType
                Case ePaneTypes.PSD
                    pane.XAxis.Scale.Min = Math.Log10(parms.FirstWeightClass)
                    pane.XAxis.Scale.Max = Math.Log10(parms.FirstWeightClass * 2 ^ (m_core.nWeightClasses - 1))
                    pane.YAxis.Scale.Min = 0
                    'pane.YAxis.Scale.Max = 8 if PSDPlotByGroup has the same scale as that of PSDContributionPlot
                Case Else
                    pane.XAxis.Scale.Min = 0
                    'pane.XAxis.Scale.Max = CDbl(Me.m_core.EcosimFirstYear + (m_core.nEcosimTimeSteps / cCore.N_MONTHS))
                    pane.YAxis.Scale.Min = 0
            End Select

            pane.Border.IsVisible = False
            pane.Legend.IsVisible = False

            pane.Chart.Border.IsVisible = False
            pane.YAxis.MajorTic.IsOpposite = False
            pane.XAxis.MajorTic.IsOpposite = False
            pane.YAxis.MinorTic.IsOpposite = False
            pane.XAxis.MinorTic.IsOpposite = False
            pane.YAxis.MinorTic.IsAllTics = False
            pane.XAxis.MinorTic.IsAllTics = False

            pane.IsFontsScaled = False

            'Me.UpdateColors()
        End Sub

        Private Sub AddCurves()

            'Add single curve into graph first
            'Results data structure

            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim resultLists As New List(Of PointPairList)
            Dim sXValue As Single = 0
            Dim grpOutput As cEcoPathGroupOutput = Nothing
            Dim sgStyleGuide As StyleGuide = StyleGuide.GetInstance
            Dim sSystemPSD(m_core.nWeightClasses) As Single

            grpOutput = m_core.EcoPathGroupOutputs(llbGroups.SelectedIndex + 1)
            Select Case parms.MortalityType
                Case ePSDMortalityTypes.GroupZ
                    InitLists(resultLists, 4)
                Case ePSDMortalityTypes.Lorenzen
                    InitLists(resultLists, 5)
            End Select

            For iTimeStep As Integer = 1 To m_core.nAgeSteps

                sXValue = (iTimeStep - 1) * grpOutput.TmaxOutput / (m_core.nAgeSteps - 1)

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

            For iWtClass As Integer = 1 To m_core.nWeightClasses
                sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))
                If sSystemPSD(iWtClass) * 100000 > 0 Then
                    'group contribution to the system PSD is Math.Log10(sSystemPSD(iWtClass) * 100000) * grpOutput.PSD(iWtClass) / sSystemPSD(iWtClass)
                    '* 100000 for plotting purpose
                    resultLists(3).Add(Math.Log10(sXValue), Math.Log10(sSystemPSD(iWtClass) * 100000) * grpOutput.PSD(iWtClass) / sSystemPSD(iWtClass))
                Else
                    resultLists(3).Add(Math.Log10(sXValue), 0)
                End If
            Next

            'Set the master pane title
            m_MasterPane.Title.Text = CStr(llbGroups.SelectedItem.ToString)

            ' Clear all panes
            For Each gp As GraphPane In m_MasterPane.PaneList
                gp.CurveList.Clear()
            Next

            AddCurveToGraphPane(ePaneTypes.Weight, resultLists(0), sgStyleGuide.GroupColor(m_core, llbGroups.SelectedIndex))
            AddCurveToGraphPane(ePaneTypes.Number, resultLists(1), sgStyleGuide.GroupColor(m_core, llbGroups.SelectedIndex))
            AddCurveToGraphPane(ePaneTypes.Biomass, resultLists(2), sgStyleGuide.GroupColor(m_core, llbGroups.SelectedIndex))
            AddCurveToGraphPane(ePaneTypes.PSD, resultLists(3), sgStyleGuide.GroupColor(m_core, llbGroups.SelectedIndex))

            'Lorenzen mortality plot if mortality type is Lorenzen
            If parms.MortalityType = ePSDMortalityTypes.Lorenzen Then
                AddCurveToGraphPane(ePaneTypes.LorenzenMortality, resultLists(4), sgStyleGuide.GroupColor(m_core, llbGroups.SelectedIndex))
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
            Dim gp As GraphPane = m_MasterPane.PaneList(CInt(paneType))
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
            Me.zgcZedGraphCntl.AxisChange()

            'Tell ZedGraph to auto layout the new GraphPanes
            'Cannot move that part up to the InitMasterPane, Title is dynamic here..??
            Dim g As Graphics = Me.CreateGraphics()
            m_MasterPane.SetLayout(g, PaneLayout.SquareColPreferred)
            g.Dispose()

            Me.zgcZedGraphCntl.Refresh()
        End Sub

        Private Sub FindSystemPSD(ByVal sSystemPSD() As Single)
            Dim grpOutput As cEcoPathGroupOutput = Nothing

            'Find the system PSD by summing the group PSD
            For iGroup As Integer = 1 To m_core.nLivingGroups
                grpOutput = m_core.EcoPathGroupOutputs(iGroup)
                For iWtClass As Integer = 1 To m_core.nWeightClasses
                    sSystemPSD(iWtClass) = sSystemPSD(iWtClass) + grpOutput.PSD(iWtClass)
                Next
            Next
        End Sub
#End Region 'Helper methods

    End Class

End Namespace