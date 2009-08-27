
'==============================================================================
'
' $Log: frmMSE.vb,v $
' Revision 1.6  2009/07/03 23:41:36  joeb
' MSE interface changes
'
' Revision 1.5  2009/06/08 17:17:11  joeb
' More MSE layout
'
' Revision 1.4  2009/06/08 16:49:08  joeb
' More MSE interface
'
' Revision 1.3  2009/06/05 20:20:31  joeb
' MSE
'
' Revision 1.2  2009/06/05 19:01:50  joeb
' Added MSE node to navigation tree
'
'
'=============================================================================
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwECore.SearchObjectives
Imports ScientificInterface.Controls
Imports EwEUtils.Core
Imports ScientificInterface.Ecosim

Imports ZedGraph

#End Region

Public Class frmMSE

    'ToDo_jb frmMSE Add group selecting to the graphs to hide and display graphs
    'ToDo_jb frmMSE Improve the MSE state change code to Enable and Disable controlls based on the MSE state
    'ToDo_jb frmMSE output performance grid needs debugging the values look wrong

#Region "Private Enum definitions"

    ''' <summary>
    ''' MSE state enumerators use by the interface to set control states
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum eMSEStates
        InActive
        Running
        Completed
    End Enum

#End Region

#Region "Private variables"

    Dim m_core As cCore
    Dim m_MSE As cMSEManager

    Private m_fpNTrials As cPropertyFormatProvider
    Private m_fpUsePlugin As cPropertyFormatProvider

    Private m_fpKalman As cPropertyFormatProvider
    Private m_fpForecast As cPropertyFormatProvider
    Private m_fpSBPower As cPropertyFormatProvider


    Private m_paneMaster As MasterPane = Nothing
    Private m_sg As cStyleGuide = Nothing
    Private m_zgh As cZedGraphHelper = Nothing

    Private m_gridObjectiveWeights As gridSearchObjectivesWeight
    Private m_gridGroupObjectives As gridSearchObjectivesGroup



#End Region

#Region "Construction Initialization and Destruction"

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        Me.m_core = cCore.GetInstance
        Me.m_MSE = Me.m_core.MSEManager

        Me.m_fpNTrials = New cPropertyFormatProvider(Me.txNTrials, Me.m_MSE.ModelParameters, eVarNameFlags.MSENTrials)
        Me.m_fpUsePlugin = New cPropertyFormatProvider(Me.ckPlugin, Me.m_MSE.ModelParameters, eVarNameFlags.MSEUseEconomicPlugin)

        Me.m_fpKalman = New cPropertyFormatProvider(Me.txKalman, Me.m_MSE.ModelParameters, eVarNameFlags.MSEKalmanGain)
        Me.m_fpForecast = New cPropertyFormatProvider(Me.txForecast, Me.m_MSE.ModelParameters, eVarNameFlags.MSEForcastGain)
        Me.m_fpSBPower = New cPropertyFormatProvider(Me.txSBPower, Me.m_MSE.ModelParameters, eVarNameFlags.MSEAssessPower)

        Me.m_gridObjectiveWeights = New gridSearchObjectivesWeight(Me.m_core.FishingPolicyManager)
        Me.m_gridGroupObjectives = New gridSearchObjectivesGroup(Me.m_core.FishingPolicyManager)

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE, eCoreComponentType.SearchObjective}

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        Me.CoreComponents = Nothing
        Me.m_MSE.Disconnect()
        MyBase.OnFormClosed(e)

    End Sub

    Private Sub OnfrmMSELoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Assessment methods Catch Estimated Biomass and Direct Exploitation are stored in the tag property of the radio buttons
        'see the Changed event of the radio buttons for setting the parameters
        Me.rbCatchEstBio.Tag = eAssessmentMethods.CatchEstmBio
        Me.rbDirectExp.Tag = eAssessmentMethods.DirectExploitation

        Me.m_paneMaster = Me.zdGraph.MasterPane
        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.m_core, Me.zdGraph, Me.m_core.nLivingGroups)

        Me.tbObjectives.TabPages("pgObjective").Controls.Add(Me.m_gridObjectiveWeights)
        Me.m_gridObjectiveWeights.Dock = DockStyle.Fill

        Me.tbObjectives.TabPages("pgEcoObjectives").Controls.Add(Me.m_gridGroupObjectives)
        Me.m_gridGroupObjectives.Dock = DockStyle.Fill

        Me.LoadGraphPanes()

        Me.UpdateControls(eMSEStates.InActive)

    End Sub

#End Region

#Region "Core interactions"

    Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)


    End Sub

#End Region

#Region "MSE interactions"

    Private Sub runMSE()

        Try

            Me.m_MSE.Connect(AddressOf Me.onMSECallBack)

            Me.prgProgress.Maximum = Me.m_MSE.ModelParameters.NTrials
            Me.prgProgress.Value = 0
            Me.ClearGraphs()

            Me.m_MSE.Run()

        Catch ex As Exception

        End Try


    End Sub


    Private Sub onMSECompleted()

        Me.prgProgress.Value = 0
        Me.m_MSE.Disconnect()

    End Sub

    Private Sub onMSECallBack(ByVal CallBackType As MSE.eCallBackTypes)

        Dim state As eMSEStates
        Select Case CallBackType

            Case eCallBackTypes.Started
                state = eMSEStates.Running

            Case eCallBackTypes.IterationStarted
                state = eMSEStates.Running


            Case eCallBackTypes.IterationCompleted
                Me.onMSEProgress()
                Me.AddLineToGraph()
                state = eMSEStates.Running

            Case eCallBackTypes.RunCompleted
                Me.onMSECompleted()
                state = eMSEStates.Completed

        End Select

        Me.UpdateControls(state)

    End Sub

    Private Sub onMSEProgress()


        Try
            Me.prgProgress.Value = Me.m_MSE.Output.TrialNumber
        Catch ex As Exception

        End Try
    End Sub


#End Region

#Region "Interface events"

    Private Sub onRunClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRun.Click

        Try
            Me.runMSE()
        Catch ex As Exception

        End Try

    End Sub


    ''' <summary>
    ''' Change the biomass assessment method based on the selected radio button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub onAssessmentMethodChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbCatchEstBio.CheckedChanged, rbDirectExp.CheckedChanged
        Try
            Debug.Assert(TypeOf sender Is RadioButton)
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            'This event handler is call for both radio buttons Changed events Checked and UnChecked
            'Use the tag of the Checked radio button to set the MSE.AssessmentMethod
            If rb.Checked = True Then
                Me.m_MSE.ModelParameters.AssessmentMethod = DirectCast(rb.Tag, eAssessmentMethods)
            End If
        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Interface"

#Region "Graphs"

    'xxxxxxxxxxxxxxxxxxxxxxxx
    'Graph plotting code was copied from EcosimPlots
    'xxxxxxxxxxxxxxxxxxxxxx

    Private Sub LoadGraphPanes()

        For igrp As Integer = 1 To Me.m_core.nLivingGroups
            Dim grp As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(igrp)
            Me.ConfigurePane(igrp, grp.Name)
        Next

    End Sub


    Private Sub ClearGraphs()
        For Each Pane As GraphPane In zdGraph.MasterPane.PaneList
            Pane.CurveList.Clear()
        Next
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Configure a plot on the main graph
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub ConfigurePane(ByVal iPane As Integer, ByVal strTitle As String)

        Me.m_zgh.ConfigurePane(strTitle, "", CDbl(Me.m_core.EcosimFirstYear), CDbl(Me.m_core.EcosimFirstYear + (m_core.nEcosimTimeSteps / cCore.N_MONTHS)), _
                   "", 0, 0, False, LegendPos.Top, iPane)

    End Sub


    Private Sub AddLineToGraph()

        Try

            Dim dx As Double
            Dim igrp As Integer
            For Each grp As cMSEGroupOutput In Me.m_MSE.GroupOutputs
                igrp += 1
                If igrp > Me.m_core.nLivingGroups Then Exit For

                Dim ppl As New PointPairList
                For iTime As Integer = 1 To m_core.nEcosimTimeSteps
                    dx = Me.m_core.EcosimFirstYear + (iTime / cCore.N_MONTHS)
                    ppl.Add(dx, grp.Biomass(iTime))
                Next

                Me.AddCurveToGraphPane(igrp, Me.m_zgh.CreateLineItem(cZedGraphHelper.eCurveTypes.EcosimOutput, igrp, ppl), False)

            Next

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".PopulateGraphs() Error: " & ex.Message)
        End Try

    End Sub



    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Add one curve into the graph pane
    ''' </summary>
    Private Sub AddCurveToGraphPane(ByVal PaneIndex As Integer, ByVal li As LineItem, _
                                    Optional ByVal bClearExistingCurves As Boolean = True)

        Dim lli As New List(Of ZedGraph.LineItem)
        lli.Add(li)

        Me.m_zgh.PlotLines(lli, PaneIndex, True, bClearExistingCurves)

    End Sub



#End Region

#Region "Controls"

    Private Sub UpdateControls(ByVal State As eMSEStates)

        Select Case State

            Case eMSEStates.InActive
                Me.btRun.Enabled = True

            Case eMSEStates.Running
                Me.btRun.Enabled = False

            Case eMSEStates.Completed
                Me.btRun.Enabled = True

        End Select

    End Sub

#End Region

#End Region

End Class