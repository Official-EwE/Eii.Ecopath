
Imports EwECore


Public Class cRunManager

    Private m_RunSpace As cRunEcospace
    Private m_plugin As cEcospaceMonteCarloPluginPoint

    Private core As cCore

    Private RunType As String


    Public Sub Init(thePlugin As cEcospaceMonteCarloPluginPoint)
        Me.m_plugin = thePlugin
        core = Me.m_plugin.Core
        Me.m_RunSpace = New cRunEcospace

    End Sub

    Public Sub Run()

        m_RunSpace.Init(Me.m_plugin.Core, Me.m_plugin.MonteCarlo, Me.m_plugin.EcoSpace)

        RunType = "Before"
        m_RunSpace.SetRunParameters(2000, 5)
        Me.m_RunSpace.Run()

        RunType = "After"
        m_RunSpace.SetRunParameters(2018, 5)
        Me.m_RunSpace.Run()
      
    End Sub


    Public Sub OnEcospaceRunCompleted()
        Me.getResults()
    End Sub

    Private Sub getResults()

        Dim sumB As Single
        System.Console.WriteLine(RunType)
        For igrp As Integer = 1 To m_plugin.Core.nGroups
            sumB = 0

            For it As Integer = Me.m_RunSpace.StartOfLastYear To Me.m_RunSpace.StartOfLastYear + Me.m_RunSpace.nTimeStepPerYear
                sumB += m_plugin.EcoSpaceData.ResultsByGroup(0, igrp, it)
            Next it

            'Average of the last year
            System.Console.Write(igrp.ToString + "," + (sumB / Me.m_RunSpace.nTimeStepPerYear).ToString + ":")

        Next igrp

        System.Console.WriteLine()


    End Sub


End Class
