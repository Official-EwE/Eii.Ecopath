Imports EwECore
Imports ZedGraph

Public Class ZedGraphDrawer
    Private m_core As cCore

    Public Sub New(ByVal core As cCore)
        m_core = core

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        ' make sure ecosim is run first
        m_core.RunEcoSim()

        ' Now let's plot the biomass
        plotBiomass()
    End Sub

    ''' <summary>
    ''' Function to plot biomass
    ''' </summary>
    Private Sub plotBiomass()
        Dim graphArea As GraphPane = m_zgc.GraphPane
        Dim x, y As Double()

        ' Clear old graphs
        graphArea.CurveList.RemoveRange(0, graphArea.CurveList.Count)

        '  Set the Titles
        graphArea.Title.Text = "Biomass"
        graphArea.XAxis.Title.Text = "Years"
        graphArea.XAxis.Scale.Max = m_core.nEcosimTimeSteps / 12
        graphArea.YAxis.Title.Text = "Rel. biomass"
        graphArea.YAxis.Scale.Min = 0
        graphArea.Legend.IsVisible = False

        ' Prepare the temp variables
        ReDim x(m_core.nEcosimTimeSteps) : ReDim y(m_core.nEcosimTimeSteps)

        ' Draw the lines for each group
        For i As Integer = 1 To m_core.nGroups

            ' Here is where you store to temp Variables
            For timeStep As Integer = 1 To m_core.nEcosimTimeSteps
                y(timeStep) = m_core.EcoSimGroupOutputs(i).Biomass(timeStep) / m_core.StartBiomass(i)
                x(timeStep) = timeStep / 12
            Next timeStep

            ' Add the curve with the name, temp data, pool color
            graphArea.AddCurve(m_core.EcoPathGroupInputs(i).Name, x, y, m_core.EcoPathGroupInputs(i).PoolColorArgb, SymbolType.None)

        Next i

        ' Done, redraw the graph
        m_zgc.AxisChange()
    End Sub
End Class
