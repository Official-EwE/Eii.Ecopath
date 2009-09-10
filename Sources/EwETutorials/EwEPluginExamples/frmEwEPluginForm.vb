Imports EwECore

''' <summary>
''' frmEwEPlugin shows you various examples of how plugins could be used
''' </summary>
''' <remarks></remarks>
Public Class frmEwEPlugin

    ''' <summary> All mighty core </summary>
    Private m_core As cCore




    ''' <summary>
    ''' New constructor, is called everytime this object is created
    ''' </summary>
    Public Sub New(ByVal Core As cCore)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_core = Core
    End Sub




    ''' <summary>
    ''' Changes a single value upon button click
    ''' </summary>
    Private Sub ChangeEcopathVariable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChangeVariables.Click
        m_core.EcoPathGroupInputs(2).BiomassAreaInput = 2
    End Sub



    ''' <summary>
    ''' This plugin does NOTHING
    ''' </summary>
    Private Sub btnMakeOwnPlugin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMakeOwnPlugin.Click
        ' Pops up a message box

        If m_core.ActiveEcosimScenarioIndex = -1 Then Return
        Dim val As Single = 2

        Do
            If MessageBox.Show("Continue", "Error", MessageBoxButtons.OKCancel) = Windows.Forms.DialogResult.Cancel Then Return
            If val = 2 Then
                val = 100
            Else
                val = 2
            End If

            m_core.EcoSimGroupInputs(4).VulMult(2) = val

            m_core.RunEcoSim()


        Loop Until False



    End Sub



    ''' <summary>
    ''' Runs ecosim upon button click
    ''' </summary>
    Private Sub btnRunEcosim_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRunEcosim.Click
        If m_core.ActiveEcosimScenarioIndex = -1 Then Return
        m_core.RunEcoSim()
    End Sub



    ''' <summary>
    ''' Makes a new datagrid and adds it on the right
    ''' </summary>
    Private Sub btnMakeDataGrid_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMakeDataGrid.Click
        ' Clear up old controls
        Panel1.Controls.Clear()
        ' Load a new User Control... see EcopathDataGrid defintion control
        Panel1.Controls.Add(New EcopathDataGrid(m_core))
    End Sub



    ''' <summary>
    ''' Makes a new biomass graph and plots it on the right
    ''' </summary>
    Private Sub btnPlotEcosim_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPlotEcosim.Click
        If m_core.ActiveEcosimScenarioIndex = -1 Then Return
        ' Clear up old controls
        Panel1.Controls.Clear()
        ' Load a new User Control... see Zedgraph control
        Panel1.Controls.Add(New ZedGraphDrawer(m_core))
    End Sub

End Class