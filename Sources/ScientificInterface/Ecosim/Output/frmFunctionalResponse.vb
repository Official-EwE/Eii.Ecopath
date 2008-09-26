'==============================================================================
'
' $Log: frmFunctionalResponse.vb,v $
' Revision 1.1  2008/09/26 07:31:48  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/09/02 14:47:29  jeroens
' Simplified ZedGraphHelper wrap interface
'
' Revision 1.2  2008/07/18 17:51:41  jeroens
' Updated to new ZedGraphHelper interface
'
' Revision 1.1  2008/07/16 19:57:59  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Controls
Imports EwEUtils.Core

#End Region ' Imports directive

Namespace Ecosim

    Public Class frmFunctionalResponse

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

#End Region ' Private variables

#Region " Constructor "

        Public Sub New()
            InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Events "

        Private Sub frmFunctionalResponse_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles Me.Load

            Me.m_zgh = New ZedGraphHelper(Me.m_graph)

            Me.UpdatePredatorDropdown()

            Me.MessageSources = New eMessageSource() {eMessageSource.EcoPath, eMessageSource.EcoSim}
            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

        End Sub

        Private Sub frmFunctionalResponse_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles Me.Disposed

            Me.MessageSources = Nothing
            Me.m_zgh = Nothing

            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.m_sg = Nothing

        End Sub

        Private Sub m_cmbPredator_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_cmbPredator.SelectedIndexChanged
            Me.UpdateGraph()
        End Sub

        Private Sub OnStyleGuideChanged(ByVal changeType As StyleGuide.eChangeType)
            ' Blunt!
            Me.UpdateGraph()
        End Sub

#End Region ' Events

#Region " Internals "

        Private Function SelectedPredator() As cEcoPathGroupInput
            If (Me.m_cmbPredator.SelectedIndex > -1) Then
                Return DirectCast(Me.m_cmbPredator.SelectedItem, cPredatorItem).Predator
            End If
            Return Nothing
        End Function

        Private Sub UpdatePredatorDropdown()
            Dim group As cEcoPathGroupInput = Nothing
            Me.m_cmbPredator.Items.Clear()
            For iGroup As Integer = 1 To Me.m_core.nGroups
                group = Me.m_core.EcoPathGroupInputs(iGroup)
                If group.IsConsumer Then Me.m_cmbPredator.Items.Add(New cPredatorItem(group))
            Next
            If Me.m_cmbPredator.Items.Count > 0 Then Me.m_cmbPredator.SelectedIndex = 0
        End Sub

        Private Sub UpdateGraph()

            ' ToDo_JS: Globalize this method

            Dim predIn As cEcoPathGroupInput = Me.SelectedPredator()
            Dim predOut As cEcosimGroupOutput = Nothing
            Dim preyIn As cEcoPathGroupOutput = Nothing
            Dim preyOut As cEcosimGroupOutput = Nothing
            Dim asX As Double()
            Dim asY As Double()

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

                            ReDim asX(Me.m_core.nEcosimTimeSteps)
                            ReDim asY(Me.m_core.nEcosimTimeSteps)

                            For iTime As Integer = 1 To Me.m_core.nEcosimTimeSteps
                                asX(iTime) = preyOut.Biomass(iTime) / preyIn.Biomass
                                ' JS: Electivity (part of Ecosim indicators calculation) not part of EwE6 yet
                                'asY(iTime) = Elect(Sel, prey, Tm)
                            Next
                            .AddCurve(preyIn.Name, asX, asY, Me.m_sg.GroupColor(Me.m_core, iPrey))
                        End If
                    Next
                End With
            End If

            Me.m_graph.Invalidate()

        End Sub

#End Region ' Internals

#Region " Overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            MyBase.OnCoreMessage(msg)
        End Sub

#End Region


    End Class

End Namespace
