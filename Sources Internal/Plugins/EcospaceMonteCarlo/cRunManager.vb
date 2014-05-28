
Imports EwECore


Public Class cRunManager

    Private m_RunSpace As cRunEcospace
    Private m_plugin As cEcospaceMonteCarloPluginPoint

    Private core As cCore


    Public Sub Init(thePlugin As cEcospaceMonteCarloPluginPoint)
        Me.m_plugin = thePlugin
        core = Me.m_plugin.Core
        Me.m_RunSpace = New cRunEcospace

    End Sub

    Public Sub Run()
        m_RunSpace.Init(Me.m_plugin.Core, Me.m_plugin.MonteCarlo, Me.m_plugin.EcoSpace)
        m_RunSpace.SetRunParameters(2000, 2)
        Me.m_RunSpace.Run()
        Dim lastTS As Integer = Me.core.nEcospaceYears * 12

        For igrp As Integer = 1 To m_plugin.Core.nGroups
            System.Console.Write(igrp.ToString + "," + m_plugin.EcoSpaceData.ResultsByGroup(0, igrp, lastTS).ToString + ":")
        Next
        System.Console.WriteLine()
    End Sub

    Public Sub OnEcospaceTimeStep(ByVal iTime As Integer)

    End Sub

End Class
