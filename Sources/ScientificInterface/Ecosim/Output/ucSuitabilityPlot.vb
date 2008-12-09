'==============================================================================
'
' $Log: ucSuitabilityPlot.vb,v $
' Revision 1.1  2008/12/09 00:30:02  joeh
' Add node for the three Suitability curves (Electivity, Functional response and Suitability)
'
'

#Region " Imports directive "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwENetworkAnalysis

#End Region ' Imports directive

Public Class ucSuitabilityPlot

#Region " Helper classes "

    Private Class cPredatorItem
        Private m_group As cEcoPathGroupInput = Nothing
        Public Sub New(ByVal group As cEcoPathGroupInput)
            Me.m_group = group
        End Sub
        Public Overrides Function ToString() As String
            Return Me.m_group.Name
        End Function
        Public ReadOnly Property Predator() As cEcoPathGroupInput
            Get
                Return Me.m_group
            End Get
        End Property
    End Class

#End Region ' Helper classes

#Region " Private variables "

    Private m_zgh As ZedGraphHelper = Nothing
    Private m_core As cCore = cCore.GetInstance()
    Private m_sg As StyleGuide = StyleGuide.GetInstance()
    'Private m_manager As cNetworkManager = Nothing

#End Region ' Private variables

#Region " Constructor "

    Public Sub New() 'ByVal manager As cNetworkManager)
        'Me.m_manager = manager
        Me.InitializeComponent()
    End Sub

#End Region ' Constructor

#Region " Events "

    Private Sub ucSuitabilityPlot_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Load

        Me.m_zgh = New ZedGraphHelper(Me.m_graph)

        Me.UpdatePlotTypeDropdown()
        Me.UpdatePredatorDropdown()

        AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

    End Sub

    Private Sub ucSuitabilityPlot_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Disposed

        Me.m_zgh = Nothing

        RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
        Me.m_sg = Nothing

    End Sub

    Private Sub m_tscmbPlotType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tscmbPlotType.SelectedIndexChanged
        Me.UpdateGraph()
    End Sub

    'Private Sub m_cmbPredator_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    '        Handles m_cmbPredator.SelectedIndexChanged
    '    Me.UpdateGraph()
    'End Sub

    Private Sub m_tscmbPredator_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tscmbPredator.SelectedIndexChanged
        Me.UpdateGraph()
    End Sub

    Private Sub OnStyleGuideChanged(ByVal changeType As StyleGuide.eChangeType)
        ' Blunt!
        Me.UpdateGraph()
    End Sub

#End Region ' Events

#Region " Internals "

    Private Function SelectedPredator() As cEcoPathGroupInput
        'If (Me.m_cmbPredator.SelectedIndex > -1) Then
        '    Return DirectCast(Me.m_cmbPredator.SelectedItem, cPredatorItem).Predator
        'End If
        If (Me.m_tscmbPredator.SelectedIndex > -1) Then
            Return DirectCast(Me.m_tscmbPredator.SelectedItem, cPredatorItem).Predator
        End If
        Return Nothing
    End Function

    Private Sub UpdatePlotTypeDropdown()
        Me.m_tscmbPlotType.SelectedIndex = 0
    End Sub

    Private Sub UpdatePredatorDropdown()
        Dim group As cEcoPathGroupInput = Nothing
        'Me.m_cmbPredator.Items.Clear()
        'For iGroup As Integer = 1 To Me.m_core.nGroups
        '    group = Me.m_core.EcoPathGroupInputs(iGroup)
        '    If group.IsConsumer Then Me.m_cmbPredator.Items.Add(New cPredatorItem(group))
        'Next
        'If Me.m_cmbPredator.Items.Count > 0 Then Me.m_cmbPredator.SelectedIndex = 0
        Me.m_tscmbPredator.Items.Clear()
        For iGroup As Integer = 1 To Me.m_core.nGroups
            group = Me.m_core.EcoPathGroupInputs(iGroup)
            If group.IsConsumer Then Me.m_tscmbPredator.Items.Add(New cPredatorItem(group))
        Next
        If Me.m_tscmbPredator.Items.Count > 0 Then Me.m_tscmbPredator.SelectedIndex = 0
    End Sub

    Private Sub UpdateGraph()

        ' ToDo_JS: Globalize this method

        Dim predIn As cEcoPathGroupInput = Me.SelectedPredator()
        Dim predOut As cEcosimGroupOutput = Nothing
        Dim preyIn As cEcoPathGroupOutput = Nothing
        Dim preyOut As cEcosimGroupOutput = Nothing
        Dim asX As Double()
        Dim asY As Double()
        Dim Xmax As Double
        Dim Ymax As Double

        If predIn Is Nothing Then
            With Me.m_graph.GraphPane
                .Title.Text = ""
                .CurveList.Clear()
            End With
        Else
            With Me.m_graph.GraphPane()
                .Title.Text = String.Format("Functional Response {0}", predIn.Name)
                .XAxis.Title.Text = "Prey biomass relative to Ecopath biomass"
                .YAxis.Title.Text = "Q prey / B pred"
                .CurveList.Clear()

                For iPrey As Integer = 1 To Me.m_core.nGroups
                    preyIn = Me.m_core.EcoPathGroupOutputs(iPrey)
                    predOut = Me.m_core.EcoSimGroupOutputs(predIn.Index)
                    preyOut = Me.m_core.EcoSimGroupOutputs(iPrey)

                    If (predIn.DietComp(iPrey) > 0.0!) And (preyIn.Biomass > 0.0) Then

                        ReDim asX(Me.m_core.nEcosimTimeSteps - 1)
                        ReDim asY(Me.m_core.nEcosimTimeSteps - 1)

                        For iTime As Integer = 1 To Me.m_core.nEcosimTimeSteps
                            asX(iTime - 1) = preyOut.Biomass(iTime) / preyIn.Biomass
                            Select Case Me.m_tscmbPlotType.SelectedItem.ToString
                                Case "Electivity"
                                    asY(iTime - 1) = preyOut.Electivity(predIn.Index, iTime) / predOut.Biomass(iTime)
                                Case "Functional response"
                                    asY(iTime - 1) = preyOut.Consumption(predIn.Index, iTime) / predOut.Biomass(iTime)
                                Case "Suitability"
                                    asY(iTime - 1) = preyOut.Consumption(predIn.Index, iTime) / predOut.Biomass(iTime)
                            End Select
                            If asX(iTime - 1) > Xmax Then Xmax = asX(iTime - 1)
                            If asY(iTime - 1) > Ymax Then Ymax = asY(iTime - 1)
                        Next

                        .AddCurve(preyIn.Name, asX, asY, Me.m_sg.GroupColor(Me.m_core, iPrey), ZedGraph.SymbolType.None)
                    End If
                Next
                .XAxis.Scale.Max = CInt(Xmax)
                .YAxis.Scale.Max = CInt(Ymax)
            End With
        End If

        Me.m_graph.AxisChange()
        Me.m_graph.Invalidate()

    End Sub

#End Region ' Internals

End Class
