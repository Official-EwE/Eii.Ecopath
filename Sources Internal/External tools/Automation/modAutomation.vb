#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.SpatialData
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEEcologicalIndicatorsPlugin
Imports EwEEcospaceSpinupPlugin
Imports EwEMPADynamicsPlugin
Imports System.IO

#End Region ' Imports

' ToDo: add log file
' ToDo: add Safenet-style progress logging
' ToDo: add run validation
' ToDo: include time series from maps logic into programmable flow

Module modAutomation

    Public Sub DefineRuns()

        Engine.Environment.OutputPath = "D:\Runs\Med"
        Engine.Environment.AutosaveAnnual = True

        For Each csvFile As String In Directory.GetFiles("D:\Projects\2019\Med3030\Run\Med\mpa dynamics MED", "*.csv")

            Dim Name As String = "Run_" & Path.GetFileNameWithoutExtension(csvFile)
            Dim Run As New cRun(Name)

            Run.Model = "D:\Projects\2019\Med3030\Run\Med\Mediterraneo_301019.EwEmdb"

            Run.Ecosim.Scenario = "Mediterraneo_3"
            Run.Ecosim.TimeSeries = "MED_Unified_PP_150_NewEff"
            Run.Ecosim.RunYears = 10

            Run.Ecospace.Scenario = "Med coarse_V4_temp"
            Run.Ecospace.SpinupYears = 10
            Run.Ecospace.RunYears = 10
            Run.Ecospace.AutosaveBiomassMaps = True
            Run.Ecospace.AutosaveCatchMaps = True
            Run.Ecospace.AutosaveEffortMaps = True
            Run.Ecospace.ExternalDataConfigFile = "D:\Projects\2019\Med3030\Run\Med\Drivers\JRC_med_coarse-interp.xml"
            Run.Ecospace.DriveEnvironment("med_c-hindcast-surface-temp", "Temp (Surface)")
            Run.Ecospace.DriveEnvironment("med_c-mpi45-surface-temp", "Temp (Surface)")
            Run.Ecospace.DriveEnvironment("med_c-hindcast-top150-temp", "Temp (150m)")
            Run.Ecospace.DriveEnvironment("med_c-mpi45-top150-temp", "Temp (150m)")
            Run.Ecospace.DriveEnvironment("med_c-hindcast-bottom-temp", "Temp (bottom)")
            Run.Ecospace.DriveEnvironment("med_c-mpi45-bottom-temp", "Temp (bottom)")
            Run.Ecospace.DriveAbsoluteBiomass("med_c-hindcast-tot-fla", "Small Phytoplankton", 1)
            Run.Ecospace.DriveAbsoluteBiomass("med_c-mpi45-tot-fla", "Small Phytoplankton", 1)
            Run.Ecospace.DriveAbsoluteBiomass("med_c-hindcast-tot-dia", "Large Phytoplankton", 1)
            Run.Ecospace.DriveAbsoluteBiomass("med_c-mpi45-tot-dia", "Large Phytoplankton", 1)

            Run.EcoIndEnabled = True
            Run.MPADynamicsFile = csvFile

            Engine.Add(Run)

        Next

    End Sub

#Region " Main loop "

    Private ReadOnly Property Engine As New cEngine()

    Sub Main()

        DefineRuns()
        Engine.Run()

    End Sub

#End Region ' Main loop

End Module
