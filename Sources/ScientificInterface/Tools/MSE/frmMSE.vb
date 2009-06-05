
'==============================================================================
'
' $Log: frmMSE.vb,v $
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

Imports ZedGraph

#End Region

Public Class frmMSE

    Private Enum eMSEStates
        InActive
        Running
        Completed
    End Enum

#Region "Private variables"

    Dim m_core As cCore
    Dim m_MSE As cMSEManager

    Private m_fpNTrials As cPropertyFormatProvider

    Private m_paneMaster As MasterPane = Nothing
    Private m_sg As cStyleGuide = Nothing
    Private m_zgh As cZedGraphHelper = Nothing


#End Region

#Region "Construction Initialization and Distruction"

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        Me.m_core = cCore.GetInstance
        Me.m_MSE = Me.m_core.MSEManager

        Me.m_fpNTrials = New cPropertyFormatProvider(Me.txNTrials, Me.m_MSE.ModelParameters, eVarNameFlags.MSENTrials)

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE, eCoreComponentType.SearchObjective}

    End Sub

    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)
        MyBase.OnFormClosing(e)

        Me.CoreComponents = Nothing
        Me.m_MSE.Disconnect()

    End Sub

    Private Sub OnfrmMSELoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.m_paneMaster = Me.zdGraph.MasterPane
        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.m_core, Me.zdGraph, Me.m_core.nGroups)

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

        Me.m_MSE.Connect(AddressOf Me.onMSECallBack)

        Me.prgProgress.Maximum = Me.m_MSE.ModelParameters.NTrials
        Me.prgProgress.Value = 0

        Me.m_MSE.Run()

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

            Case eCallBackTypes.IterationCompleted
                Me.onMSEProgress()

            Case eCallBackTypes.RunCompleted
                Me.onMSECompleted()
                state = eMSEStates.Completed

        End Select

        UpdateControls(state)

    End Sub

    Private Sub onMSEProgress()


        Try
            Me.prgProgress.Value = Me.m_MSE.Output.TrialNumber
        Catch ex As Exception

        End Try
    End Sub


#End Region

#Region "Interface events"

    Private Sub btRun_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRun.Click

        Try
            Me.runMSE()
        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Interface"


#Region "Graphs"

    Private Sub LoadGraphPanes()

        For igrp As Integer = 1 To Me.m_core.nGroups
            Dim grp As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(igrp)
            Me.ConfigurePane(igrp, grp.Name)
        Next

    End Sub

#End Region


    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Configure a plot on the main graph
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub ConfigurePane(ByVal iPane As Integer, ByVal strTitle As String)

        Me.m_zgh.ConfigurePane(strTitle, _
                   "", CDbl(Me.m_core.EcosimFirstYear), CDbl(Me.m_core.EcosimFirstYear + (m_core.nEcosimTimeSteps / cCore.N_MONTHS)), _
                   "", 0, 0, _
                   False, LegendPos.Top, iPane)

    End Sub


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

End Class