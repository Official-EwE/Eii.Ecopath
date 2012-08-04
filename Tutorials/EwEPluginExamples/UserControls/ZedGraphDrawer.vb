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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports EwECore
Imports ZedGraph
Imports ScientificInterfaceShared.Style

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
        Dim sg As New cStyleGuide()
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
            graphArea.AddCurve(m_core.EcoPathGroupInputs(i).Name, x, y, sg.GroupColor(m_core, i), SymbolType.None)

        Next i

        ' Done, redraw the graph
        m_zgc.AxisChange()
    End Sub
End Class
