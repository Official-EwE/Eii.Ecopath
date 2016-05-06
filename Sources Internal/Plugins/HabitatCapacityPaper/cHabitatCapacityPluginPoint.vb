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
' Copyright 1991-2012 UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region

''' <summary>
''' Base code that can be used as a template to create a new plug-in.
''' </summary>
''' <remarks>
''' <para>This plugin responds to:</para>
''' <list type="bullet">
''' <item><description>loading a model,</description>></item>
''' <item><description>saving a model,</description>></item>
''' <item><description>closing a model,</description>></item>
''' <item><description>initialization of the Core,</description>></item>
''' <item><description>initialization of Ecopath,</description>></item>
''' <item><description>initialization of Ecosim,</description>></item>
''' <item><description>initialization of Ecospace.</description>></item>
''' </list>
''' <para>In order to run and test this plugin it must be integrated within the EwE6 scientific interface. 
''' To achieve this, add this project to the EwE6 solution, and reference this project from within the 
''' ScientificInterface. This ensures that your plug-in will be built with EwE6, and will be loaded by the 
''' EwE6 plug-in manager when you run EwE6.</para>
''' </remarks>
''' 
Public Class cHabitatCapacityPluginPoint
    Implements EwEPlugin.IPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IEcopathPlugin
    Implements EwEPlugin.IEcopathRunInitializedPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcospaceInitializedPlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.IEcospaceBeginTimestepPlugin


    ' ToDo Add your own EwEPlugin interface implementations here
    ' With the cursor at the end of the new Implements line press the enter key
    ' and one or more empty place holder methods will be added to the bottom of the code

#Region " Local variables"

    ''' <summary>The core that this plug-in can use</summary>
    Private m_core As cCore

    Private m_EcoPath As cEcoPathModel
    Private m_EcoSim As cEcoSimModel
    Private m_EcoSpace As cEcoSpace
    Private m_EcoPathData As cEcopathDataStructures
    Private m_EcoSimData As cEcosimDatastructures
    Private m_EcoSpaceData As cEcospaceDataStructures

    Private m_uic As cUIContext = Nothing
    Private m_form As frmHabCap = Nothing

    Private Const DEAFULT_DATAPATH As String = "c:\Ecopath\HabitatCapacity"
    Private m_OutputDataPath As String

#End Region

#Region "Run the Capacity Analysis"

    Public bStopRun As Boolean
    Public Message As String

    Public Sub HabitatCapacityModel()

        'Try
        Me.bStopRun = False

        Me.setOutputDataPath()

        PostMessage("Running Ecospace to get the baseline values")
        'Run Ecospace to get the base line TRUE biomass distributions
        m_core.RunEcoSpace(Nothing, False)

        Dim KeyGrp As Integer = 4

        ' Load model, load sim, load space, run, etc
        Dim l As cEcospaceLayerRelPP = m_core.EcospaceBasemap.LayerRelPP
        Dim d As cEcospaceLayerDepth = m_core.EcospaceBasemap.LayerDepth

        'Get a Enviromental Driver Layer by Name
        'The names must match or getLayerByName() will assert and this will Assert
        Dim lyrSalinity As cEcospaceLayerDriver = Me.getLayerByName("Salinity")
        Dim lyrO2 As cEcospaceLayerDriver = Me.getLayerByName("Oxygen")
        Dim lyrSandy As cEcospaceLayerDriver = Me.getLayerByName("Sandy")
        Dim lyrTemp As cEcospaceLayerDriver = Me.getLayerByName("Temperature")

        'Get the following environmental preference functions:
        Dim EnvDepth As cEnviroResponseFunction = Nothing
        Dim EnvTemp As cEnviroResponseFunction = Nothing
        Dim EnvSand As cEnviroResponseFunction = Nothing
        Dim EnvSal As cEnviroResponseFunction = Nothing
        Dim EnvO2 As cEnviroResponseFunction = Nothing

        If KeyGrp = 4 Then
            EnvDepth = Me.getEnviroResponseFunction(2) ' No 2 "Depth whiting"
            EnvTemp = Me.getEnviroResponseFunction(6) '= No 6 "Temp warm"
            EnvSand = Me.getEnviroResponseFunction(12) '= No 12 "Whiting sand bottom"
            EnvSal = Me.getEnviroResponseFunction(11) ' = No 11 "Salinity cod"
            EnvO2 = Me.getEnviroResponseFunction(13) ' = No 13 "DO higher"
        ElseIf KeyGrp = 3 Then
            EnvDepth = Me.getEnviroResponseFunction(3)
            EnvTemp = Me.getEnviroResponseFunction(5)
            EnvSand = Me.getEnviroResponseFunction(10)
            EnvSal = Me.getEnviroResponseFunction(11)
            EnvO2 = Me.getEnviroResponseFunction(13)
        ElseIf KeyGrp = 5 Then
            EnvDepth = Me.getEnviroResponseFunction(4)
            EnvTemp = Me.getEnviroResponseFunction(5)
            EnvSand = Me.getEnviroResponseFunction(10)
            EnvSal = Me.getEnviroResponseFunction(11)
            EnvO2 = Me.getEnviroResponseFunction(13)

        End If
        Dim inR As Integer = m_core.EcospaceBasemap.InRow
        Dim inC As Integer = m_core.EcospaceBasemap.InCol
        Dim iCells As Integer = inR * inC
        Dim TrueEnv(iCells, 5) As Double
        Dim TrueBio(m_core.nGroups, iCells) As Double

        'Biomass after the Ecospace run
        Dim RunBio(m_core.nGroups, iCells) As Double

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'How to assign a response function to a Layer/Group
        'The Response function must already be defined in the core and you need to know its Index
        'Response functions are applied to the group index of a Map/Layer

        'This will assign the first response function to the KeyGrp of the Depth layer

        'Get the response function we want to assign to the Group in a Layer (first one for test)
        Dim FunctToAssign As cEnviroResponseFunction = Me.getEnviroResponseFunction(1)
        'Index of the function
        Dim iFunctionIndex As Integer = FunctToAssign.Index
        'Name of the Depth layer
        Dim LayerName(5) As String
        LayerName(1) = d.Name

        'Response functions are applied to the group index(KeyGrp) of a map/layer(LayerName)
        Me.assignResponseFunctToGroupAndLayer(iFunctionIndex, KeyGrp, LayerName(1))

        'xxxxxxxxxxxxxxxxxxxxxxxxxx


        For ir As Integer = 1 To inR
            For ic As Integer = 1 To inC
                Dim iNo As Integer = (ir - 1) * inR + ic
                For igrp As Integer = 1 To m_core.nGroups
                    TrueBio(igrp, iNo) = Me.m_EcoSpaceData.Bcell(ir, ic, igrp)
                Next igrp
                'Read from the habitat capacity environmental layers:
                TrueEnv(iNo, 1) = CInt(d.Cell(ir, ic))
                TrueEnv(iNo, 2) = CDbl(lyrTemp.Cell(ir, ic))
                TrueEnv(iNo, 3) = CDbl(lyrSandy.Cell(ir, ic))
                TrueEnv(iNo, 4) = CDbl(lyrSalinity.Cell(ir, ic))
                TrueEnv(iNo, 5) = CDbl(lyrO2.Cell(ir, ic))
            Next ic
        Next ir

        'Save the truebio
        WriteBioToCSV(False, TrueBio, 0, 0, 0, iCells, KeyGrp)
        'Other sampled variable
        WriteSamplesToCSV(TrueEnv)


        WriteEnvOrigToCSV(1, EnvDepth.ShapeData(), 0, 0)
        WriteEnvOrigToCSV(2, EnvTemp.ShapeData(), 0, 0)
        WriteEnvOrigToCSV(3, EnvSand.ShapeData(), 0, 0)
        WriteEnvOrigToCSV(4, EnvSal.ShapeData(), 0, 0)
        WriteEnvOrigToCSV(5, EnvO2.ShapeData(), 0, 0)


        'After some fiddling around we're going to change the environmental preference function, one by one based on sampling with uncertainty

        'set env func no X = value
        'sample similar parameters as above (
        Dim RandomClass As New Random()
        Dim UseThis(5) As Boolean

        UseThis(1) = True
        UseThis(2) = True
        UseThis(3) = True
        UseThis(4) = True
        UseThis(5) = True

        For iSampleError As Integer = 2 To 2 Step 2
            For iSampleSize As Integer = 800 To 800 Step 200

                If Me.bStopRun Then Exit For
                PostMessage("Starting trial: sample size = " + iSampleSize.ToString + " error = " + iSampleError.ToString)

                Dim Sample(iSampleSize, 5) As Double

                'Take the right number of samples:
                Dim Taken(iCells) As Boolean
                For iRun As Integer = 1 To iSampleSize
                    'Pick a random cell, but only once
                    Dim FoundOne As Boolean = False
                    Dim Cell As Integer
                    If iSampleSize < iCells Then   'random but no repetitions
                        Do While FoundOne = False
                            Cell = RandomClass.Next(1, iCells)
                            If Taken(Cell) = False Then
                                FoundOne = True
                                Taken(Cell) = True
                            End If
                        Loop
                    ElseIf iSampleSize = iCells Then
                        Cell = iRun   'just take them in sequence
                    Else
                        Cell = RandomClass.Next(1, iCells)
                    End If
                    Dim dV As Double = 1 ' Normal(iSampleError / 10, 1)    ' [0,1]
                    'dV = 1
                    Sample(iRun, 1) = TrueEnv(Cell, 1) * dV
                    'Carl's advice: initially don't use error on env parameters, so:
                    'dV = Normal(iSampleError / 10, 1)
                    Sample(iRun, 2) = TrueEnv(Cell, 2) * dV
                    'dV = Normal(iSampleError / 10, 1)
                    Sample(iRun, 3) = TrueEnv(Cell, 3) * dV
                    'dV = Normal(iSampleError / 10, 1)
                    Sample(iRun, 4) = TrueEnv(Cell, 4) * dV
                    'dV = Normal(iSampleError / 10, 1)
                    Sample(iRun, 5) = TrueEnv(Cell, 5) * dV

                    dV = Normal(iSampleError / 10, 1)
                    'Biomass for Whiting the 4th group
                    Sample(iRun, 0) = TrueBio(KeyGrp, Cell) * dV
                Next
                'Get sample error for each environmental parameter
                'Dim CVdepth As Double = CV(SampleDepth)
                'Dim CVtemp As Double = CV(SampleTemp)
                'Dim CVsand As Double = CV(SampleSand)
                'Dim CVsal As Double = CV(SampleSal)

                'Generate a new environmental preference function for these parameters
                'Depth
                'Make 50 bins distributed over the sample size
                Dim iBins As Integer = CInt(iSampleSize / 50)
                iBins = CInt(IIf(iBins < 10, 10, iBins))
                Dim Left(5) As Double
                Dim Range(5) As Double
                'Dim Right As Double = EnvDepth.ResponseRightLimit '= max of SampleDepth()
                Left(1) = EnvDepth.ResponseLeftLimit '= Min of SampleDepth() 
                Range(1) = EnvDepth.ResponseRightLimit - EnvDepth.ResponseLeftLimit
                Left(2) = EnvTemp.ResponseLeftLimit '= Min of SampleDepth() 
                Range(2) = EnvTemp.ResponseRightLimit - EnvTemp.ResponseLeftLimit
                Left(3) = EnvSand.ResponseLeftLimit '= Min of SampleDepth() 
                Range(3) = EnvSand.ResponseRightLimit - EnvSand.ResponseLeftLimit
                Left(4) = EnvSal.ResponseLeftLimit '= Min of SampleDepth() 
                Range(4) = EnvSal.ResponseRightLimit - EnvSal.ResponseLeftLimit
                Left(5) = EnvO2.ResponseLeftLimit '= Min of SampleDepth() 
                Range(5) = EnvO2.ResponseRightLimit - EnvO2.ResponseLeftLimit

                'bin the samplebio 
                Dim EnvBinSumB(iBins, 5) As Double
                'Keep track of how many samples per strata = bin
                Dim BinCount(iBins, 5) As Integer
                For iPar As Integer = 1 To 5

                    For iRun As Integer = 1 To iSampleSize
                        'What bin = Floor((Depth * NoBins / Range)+1)?

                        'Find the bin number from the variable we are sampling
                        Dim BinNo As Integer = CInt(Math.Floor((Sample(iRun, iPar) - Left(iPar)) * iBins / Range(iPar)) + 1)

                        If BinNo < 1 Then BinNo = 1
                        If BinNo > iBins Then BinNo = iBins
                        'Sum the Biomass into the bin number for this variable
                        EnvBinSumB(BinNo, iPar) += Sample(iRun, 0)
                        BinCount(BinNo, iPar) += 1
                    Next
                Next

                Dim EnvFunc(1200, 5) As Double
                'How many steps per bin?
                Dim iStep As Integer = CInt(1200 / iBins)
                Dim iCount As Integer = 0

                For iPar As Integer = 1 To 5
                    iCount = 0
                    For i As Integer = 1 To iBins
                        For j As Integer = 1 To iStep
                            iCount += 1
                            If BinCount(i, iPar) > 0 And iCount < 1201 Then
                                EnvFunc(iCount, iPar) = EnvBinSumB(i, iPar) / BinCount(i, iPar)
                            End If
                        Next
                    Next
                Next

                If iCount < 1200 Then
                    For iPar As Integer = 1 To 5
                        For i As Integer = iCount + 1 To 1200
                            EnvFunc(i, iPar) = EnvFunc(iCount, iPar)
                        Next
                    Next
                End If

                'Update the environmental response shapes used by the core
                'this also locks the updates then does the update in batch to make it faster
                Me.UpdateEnvShapes(EnvFunc, EnvDepth, EnvTemp, EnvSand, EnvSal, EnvO2, UseThis)

                'Run Ecospace
                m_core.RunEcoSpace(Nothing, False)

                'Get biomass for all the groups from this Ecospace run
                Me.getEcospaceBiomass(RunBio)

                WriteBioToCSV(True, RunBio, iSampleError / 10, iBins, iSampleSize, iCells, KeyGrp)
                WriteEnvFuncToCSV(True, EnvFunc, iSampleError / 10, iBins)

            Next iSampleSize
            If Me.bStopRun Then Exit For
        Next iSampleError

        'Catch ex As Exception

        'PostMessage("WARNING: Exception some place in HabitatCapacityModel " + ex.Message)

        'End Try

        PostMessage("Reloading Ecospace scenario.")
        Me.m_core.DiscardChanges()
        Me.m_core.LoadEcosimScenario(1)
        Me.m_core.LoadEcospaceScenario(1)

        If Me.bStopRun Then
            PostMessage("Stopped before completion")
        Else
            PostMessage("Completed")
        End If

    End Sub


    Private Sub UpdateEnvShapes(ByVal EnvFunc(,) As Double, ByVal EnvDepth As cEnviroResponseFunction, ByVal EnvTemp As cEnviroResponseFunction, _
                                ByVal EnvSand As cEnviroResponseFunction, ByVal EnvSal As cEnviroResponseFunction, _
                                ByVal EnvO2 As cEnviroResponseFunction, ByVal UseThis() As Boolean)

        EnvDepth.LockUpdates()
        EnvDepth.LockUpdates()
        EnvTemp.LockUpdates()
        EnvSand.LockUpdates()
        EnvSal.LockUpdates()
        EnvO2.LockUpdates()


        'How to set Environment/Foraging Response function shape
        For ipt As Integer = 1 To EnvDepth.nPoints
            'Set the Range of the response funciton
            'set the response multiplier to the environmental map input
            If UseThis(1) Then EnvDepth.ShapeData(ipt) = CSng(EnvFunc(ipt, 1))
            If UseThis(2) Then EnvTemp.ShapeData(ipt) = CSng(EnvFunc(ipt, 2))
            If UseThis(3) Then EnvSand.ShapeData(ipt) = CSng(EnvFunc(ipt, 3))
            If UseThis(4) Then EnvSal.ShapeData(ipt) = CSng(EnvFunc(ipt, 4))
            If UseThis(5) Then EnvO2.ShapeData(ipt) = CSng(EnvFunc(ipt, 5))
        Next

        EnvDepth.UnlockUpdates()
        EnvDepth.UnlockUpdates()
        EnvTemp.UnlockUpdates()
        EnvSand.UnlockUpdates()
        EnvSal.UnlockUpdates()
        EnvO2.UnlockUpdates()

    End Sub


    Private Function getEnviroResponseFunction(ByVal iFunctionIndex As Integer) As cEnviroResponseFunction
        Dim shape As cEnviroResponseFunction
        Try
            'Foraging response functions are in a zero base list
            shape = DirectCast(Me.m_core.CapacityShapeManager.Item(iFunctionIndex - 1), cEnviroResponseFunction)
        Catch ex As Exception
            PostMessage("WARNING: Failed to find Foraging Response Function #" + iFunctionIndex.ToString)
        End Try
        Debug.Assert(shape IsNot Nothing, "Failed to find Foraging Response Function #" + iFunctionIndex.ToString)
        Return shape
    End Function


    Private Sub getEcospaceBiomass(ByVal biomass(,) As Double)
        Dim inR As Integer = m_core.EcospaceBasemap.InRow
        Dim inC As Integer = m_core.EcospaceBasemap.InCol
        Dim iCells As Integer = inR * inC

        For ir As Integer = 1 To inR
            For ic As Integer = 1 To inC
                Dim iNo As Integer = (ir - 1) * inR + ic

                For igrp As Integer = 1 To m_core.nGroups
                    biomass(igrp, iNo) = CDbl(Me.m_EcoSpaceData.Bcell(ir, ic, igrp))
                Next igrp

            Next ic
        Next ir

    End Sub

    Private Sub assignResponseFunctToGroupAndLayer(ByVal iFunctionIndex As Integer, ByVal iGroupIndex As Integer, LayerName As String)
        Dim map As IEnviroInputMap

        Debug.Assert(iFunctionIndex <= Me.m_core.CapacityShapeManager.Count, "Functional Response Index out of bounds.")

        'The environmental Maps are stored by index in the same order as they appear in the "Apply environmental response" grid
        'Get the map by Index
        'map = Me.m_core.CapacityMapInteractionManager.Map(iLayerIndex)
        'Get the map by Name
        map = Me.m_core.CapacityMapInteractionManager.Map(LayerName)
        Debug.Assert(map IsNot Nothing, "Failed to find a Layer with the name '" + LayerName + "'")

        'Apply the Response function to a GroupIndex on the Layer
        map.ResponseIndexForGroup(iGroupIndex) = iFunctionIndex

    End Sub

    Private Function getLayerByName(ByVal LayerName As String) As cEcospaceLayerDriver
        Dim layer As cEcospaceLayerDriver

        For iLayer As Integer = 1 To m_core.nEnvironmentalDriverLayers
            layer = m_core.EcospaceBasemap.LayerDriver(iLayer)
            If String.Compare(LayerName, layer.Name, True) = 0 Then
                Return layer
            End If
        Next
        Debug.Assert(False, "Failed to find Ecospace layer " + LayerName)
        Return Nothing

    End Function

    Private Sub PostMessage(ByVal MessageToPost As String)
        Me.Message = MessageToPost
        ' Me.m_form.WorkerThread.ReportProgress(1)
        Me.m_form.Invoke(Me.m_form.updater)
        ' System.Windows.Forms.Application.DoEvents()
    End Sub


    Private Function Normal(Optional ByVal Sigma As Double = 1, Optional ByVal Mean As Double = 0) As Double
        Normal = GetGausse() * Sigma + Mean
    End Function

    Private Function GetGausse() As Double
        ' This Function returns a standard Gaussian random number
        ' based upon the polar form of the Box-Muller transform.

        ' since this calc is capable of returning two calculations per
        ' call, it's been set up to save the second calc for the next
        ' pass through the function, saving some time.

        ' Call the randomize function once (and ONLY once) in the life of the project.

        Static blReturn2 As Boolean  ' Flag to calc new values, or return
        ' previously calculated value.  It defaults
        ' to False on the first pass.
        Static dblReturn2 As Double  ' Second return value

        Dim Work1 As Double, Work2 As Double, Work3 As Double

        Const Two = 2.0#, One = 1.0#

        If blReturn2 Then  ' On odd numbered calls
            GetGausse = dblReturn2
        Else
            Work3 = Two
            Do Until Work3 < One
                Work1 = Two * Rnd() - One
                Work2 = Two * Rnd() - One
                Work3 = Work1 * Work1 + Work2 * Work2
            Loop
            Work3 = Math.Sqrt((-(Two) * Math.Log(Work3)) / Work3)
            GetGausse = Work1 * Work3
            ' a second valid value will be returned by Work2 * Work3.
            ' Calculate it for the next pass.  This will save some processing
            dblReturn2 = Work2 * Work3
        End If

        blReturn2 = Not blReturn2 ' and toggle the return value flag

    End Function

    Private Function StdDev(ByVal elements As IEnumerable(Of Double)) As Double
        If elements Is Nothing Then Return 0
        Dim mean As Double = (Aggregate el As Double In elements Into Average(CDbl(el)))
        Dim squares As IEnumerable(Of Double) = (From el As Double In elements Select (el - mean) ^ 2)
        Dim variance As Double = (Aggregate square_el As Double In squares Into Average(square_el))
        Return Math.Sqrt(variance)
    End Function

    Private Function CV(ByVal elements() As Double) As Double
        If elements Is Nothing Then Return 0
        Dim mean As Double = (Aggregate el As Double In elements Into Average(CDbl(el)))
        Dim squares As IEnumerable(Of Double) = (From el As Double In elements Select (el - mean) ^ 2)
        Dim variance As Double = (Aggregate square_el As Double In squares Into Average(square_el))
        If mean > 0 Then
            Return Math.Sqrt(variance) / mean
        Else
            Return 0
        End If
    End Function

    Private Sub WriteBioToCSV(ByVal Append As Boolean, ByVal bio(,) As Double, ByVal sd As Double, _
                              ByVal Bins As Integer, ByVal SampleSize As Integer, ByVal iCells As Integer, ByVal KeyGrp As Integer)
        'make a file with cell number and LME no
        Dim runsFile As String = Path.Combine(Me.m_OutputDataPath, "Runs.csv")
        Using sw As StreamWriter = New StreamWriter(runsFile, Append)  'true makes it append
            'sw.WriteLine("Cell,LME,Area")
            Dim sStr As String = sd & "," & Bins & "," & SampleSize
            For iC As Integer = 1 To iCells
                sStr += "," & bio(KeyGrp, iC)
            Next
            sw.WriteLine(sStr)
            sw.Close()
        End Using
    End Sub

    Private Sub WriteSamplesToCSV(ByVal samples(,) As Double)
        Dim DataTypes() As String = New String(5) {"", "Depth", "Temperature", "Salinity", "Sand", "Oxygen"}
        Dim runsFile As String = Path.Combine(Me.m_OutputDataPath, "Samples.csv")
        Using sw As StreamWriter = New StreamWriter(runsFile, False)  'true makes it append
            'sw.WriteLine("Cell,LME,Area")
            For itype As Integer = 1 To 5

                Dim sStr As String = DataTypes(itype)
                For iC As Integer = 1 To 400
                    sStr += "," & samples(iC, itype)
                Next

                sw.WriteLine(sStr)
            Next
            sw.Close()
        End Using
    End Sub

    Private Sub WriteEnvOrigToCSV(ByVal iPar As Integer, ByVal env() As Single, ByVal sd As Double, ByVal SampleSize As Integer)
        'make a file with cell number and LME no
        Dim Filen As String = ""
        Select Case iPar
            Case 1 : Filen = "Depth"
            Case 2 : Filen = "Temperature"
            Case 3 : Filen = "Salinity"
            Case 4 : Filen = "Sand"
            Case 5 : Filen = "Oxygen"
        End Select
        Dim outfile As String = Path.Combine(Me.m_OutputDataPath, Filen + ".csv")
        Using sw As StreamWriter = New StreamWriter(outfile, False)  'true makes it append
            'sw.WriteLine("Cell,LME,Area")
            Dim sStr As String = sd & "," & SampleSize
            For iC As Integer = 1 To 1200
                sStr += "," & env(iC)
            Next
            sw.WriteLine(sStr)
            sw.Close()
        End Using
    End Sub

    Private Sub WriteEnvFuncToCSV(ByVal Append As Boolean, ByVal env(,) As Double, ByVal sd As Double, ByVal SampleSize As Integer)
        'make a file with cell number and LME no
        For iPar As Integer = 1 To 5
            Dim Filen As String = ""
            Select Case iPar
                Case 1 : Filen = "Depth"
                Case 2 : Filen = "Temperature"
                Case 3 : Filen = "Salinity"
                Case 4 : Filen = "Sand"
                Case 5 : Filen = "Oxygen"
            End Select
            Dim outfile As String = Path.Combine(Me.m_OutputDataPath, Filen + ".csv")
            Using sw As StreamWriter = New StreamWriter(outfile, Append)  'true makes it append
                'sw.WriteLine("Cell,LME,Area")
                Dim sStr As String = sd & "," & SampleSize
                For iC As Integer = 1 To 1200
                    sStr += "," & env(iC, iPar)
                Next
                sw.WriteLine(sStr)
                sw.Close()
            End Using
        Next

    End Sub

    Private Sub setOutputDataPath()

        If Directory.Exists(DEAFULT_DATAPATH) Then
            Me.m_OutputDataPath = DEAFULT_DATAPATH
        Else
            Me.m_OutputDataPath = Me.m_core.DataSource.Directory
        End If

        Me.PostMessage("Output data will be written to '" + Me.m_OutputDataPath + "'")
    End Sub

#End Region

#Region "Public Methods"

    Public Sub DoSomething(ByVal Value As Single)

        MsgBox("Hi from DoSomething(). Your value = " + Value.ToString, MsgBoxStyle.Information)
        System.Console.WriteLine(Value.ToString)

    End Sub

    Public Sub OpenModel(ByVal filename As String)
        Me.m_core.LoadModel(filename)
    End Sub

#End Region

#Region "Ecopath, Ecosim and Ecospace events"

    ''' <summary>
    ''' Every plug-in is told to initialize to the EwE core as soon as it is loaded. 
    ''' Typically, plug-ins use this opportunity to store a reference to the core
    ''' for later use.
    ''' </summary>
    ''' <param name="CoreAsObject">The core, casted to a generic object</param>
    Public Sub Initialize(ByVal CoreAsObject As Object) Implements EwEPlugin.IPlugin.Initialize
        Try
            m_core = DirectCast(CoreAsObject, cCore)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".Initialize() Exception " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Plug-in point that is called when the core has initialized its models
    ''' Ecopath, Ecosim and Ecospace. This is the only opportunity for plug-ins to grab 
    ''' references to these models.
    ''' </summary>
    ''' <param name="EcopathAsObject"></param>
    ''' <param name="EcoSimAsObject"></param>
    ''' <param name="EcoSpaceAsObject"></param>
    Public Sub CoreInitialized(ByRef EcopathAsObject As Object, ByRef EcoSimAsObject As Object, ByRef EcoSpaceAsObject As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized
        Try

            m_EcoPath = TryCast(EcopathAsObject, cEcoPathModel)
            m_EcoSim = TryCast(EcoSimAsObject, cEcoSimModel)
            m_EcoSpace = TryCast(EcoSpaceAsObject, cEcoSpace)

            Debug.Assert((m_EcoPath IsNot Nothing) And (m_EcoSim IsNot Nothing) And (m_EcoSpace IsNot Nothing), _
                         Me.ToString + ".CoreInitialized() Failed to initialize data.")

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".CoreInitialized() Exception " + ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' An Ecopath model has loaded.
    ''' </summary>
    ''' <param name="dataSource"></param>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    Public Function LoadModel(ByVal dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.LoadModel
        Try

            'Cast the datasource 
            Dim ModelDataBase As EwECore.DataSources.cDBDataSource
            ModelDataBase = DirectCast(dataSource, EwECore.DataSources.cDBDataSource)

            System.Console.WriteLine(Me.ToString + ".LoadModel() " + ModelDataBase.FileName)

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".LoadModel() Exception " + ex.Message)
        End Try

        Return True

    End Function

    ''' <summary>
    ''' An Ecopath model has been saved.
    ''' </summary>
    ''' <param name="dataSource"></param>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    Public Function SaveModel(ByVal dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.SaveModel
        System.Console.WriteLine(Me.ToString + ".SaveModel()")

        Return True
    End Function

    ''' <summary>
    ''' An Ecopath model has been closed.
    ''' </summary>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    Public Function CloseModel() As Boolean Implements EwEPlugin.IEcopathPlugin.CloseModel
        System.Console.WriteLine(Me.ToString + ".CloseModel()")

        Try
            'A user has closed the database
            'Clear out the old data so that we are 
            'not holding on to data that belongs to a closed model
            Me.m_EcoPath = Nothing
            Me.m_EcoPathData = Nothing
            Me.m_EcoSim = Nothing
            Me.m_EcoSimData = Nothing
            Me.m_EcoSpace = Nothing
            Me.m_EcoSpaceData = Nothing
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".CloseModel() Exception " + ex.Message)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' An Ecopath model is about to run.
    ''' </summary>
    ''' <param name="EcopathDataAsObject"></param>
    ''' <param name="TaxonDataAsObject"></param>
    ''' <param name="StanzaDataAsObject"></param>
    Public Sub EcopathRunInitialized(ByVal EcopathDataAsObject As Object, ByVal TaxonDataAsObject As Object, ByVal StanzaDataAsObject As Object) Implements EwEPlugin.IEcopathRunInitializedPlugin.EcopathRunInitialized

        Me.m_EcoPathData = TryCast(EcopathDataAsObject, cEcopathDataStructures)
        Debug.Assert(Me.m_EcoPathData IsNot Nothing, Me.ToString + ".EcopathRunInitialized() Failed to get EcopathDataStructures.")

    End Sub

    Public Sub EcosimInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        System.Console.WriteLine(Me.ToString + ".EcosimInitialized()")

        Me.m_EcoSimData = TryCast(EcosimDatastructures, cEcosimDatastructures)
        Debug.Assert(Me.m_EcoSimData IsNot Nothing, Me.ToString + ".EcosimInitialized() Failed to get EcosimDataStructures.")

    End Sub

    Public Sub EcospaceInitialized(ByVal EcospaceDatastructures As Object) Implements EwEPlugin.IEcospaceInitializedPlugin.EcospaceInitialized
        System.Console.WriteLine(Me.ToString + ".EcospaceInitialized()")

        Me.m_EcoSpaceData = TryCast(EcospaceDatastructures, cEcospaceDataStructures)
        Debug.Assert(Me.m_EcoSpaceData IsNot Nothing, Me.ToString + ".EcospaceInitialized() Failed to get EcosimDataStructures.")
    End Sub


    Public Sub onEcospaceBeginTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep

    End Sub

#End Region

#Region "Core, Ecopath, Ecosim and Ecospace Datastructures"

    Public ReadOnly Property Core As cCore
        Get
            Debug.Assert(Me.m_core IsNot Nothing, Me.ToString + ".Core() EwE Core has not been initialized correctly.")
            Return Me.m_core
        End Get
    End Property

    Public ReadOnly Property EcoPathData As cEcopathDataStructures
        Get
            Debug.Assert(Me.m_EcoPathData IsNot Nothing, Me.ToString + ".EcopathData() Ecopath has not been initialized correctly.")
            Return Me.m_EcoPathData
        End Get
    End Property

    Public ReadOnly Property EcoSimData As cEcosimDatastructures
        Get
            Debug.Assert(Me.m_EcoSimData IsNot Nothing, Me.ToString + ".EcoSimData() EcoSim has not been initialized correctly.")
            Return Me.m_EcoSimData
        End Get
    End Property

    Public ReadOnly Property EcoSpaceData As cEcospaceDataStructures
        Get
            Debug.Assert(Me.m_EcoSpaceData IsNot Nothing, Me.ToString + ".EcoSpaceData() EcoSpace has not been initialized correctly.")
            Return Me.m_EcoSpaceData
        End Get
    End Property

#End Region

#Region " User Interface plug-in implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User Interfaces require a UIContext, which provides not only access to
    ''' a running core, but also to a styleguide, command handler, and other
    ''' aspects that binds user interface elements in the EwE 6 application. 
    ''' </summary>
    ''' <param name="uic">The <see cref="cUIContext"/> to connect to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub UIContext(ByVal uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext

        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception
            Me.m_uic = Nothing
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what text to display in controls that provide access to 
    ''' this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Habitat Capacity Paper"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what image to show for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            ' Use an image from the pool of shared resources
            Return ScientificInterfaceShared.My.Resources.fish
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what text to display when the user hovers the mouse cursor
    ''' over a user interface element for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            ' Show the description as a tooltip text
            Return Me.Description
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provide EwE6 with a method to execute when a user interface control for 
    ''' this plug-in is clicked by the user.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef form As Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        Dim bHasInterface As Boolean = False

        ' Initialized ok?
        If m_uic IsNot Nothing Then

            ' Test if form still exists. This is a two-step test: the interface needs to be defined, and has not been closed previously.
            If Me.m_form IsNot Nothing Then
                If Not Me.m_form.IsDisposed Then
                    bHasInterface = True
                End If
            End If

            ' Create the interface if needed
            If Not bHasInterface Then

                ' Create the EwE form-derived user interface for this plug-in
                Me.m_form = New frmHabCap()
                Me.m_form.Init(Me)
                ' Pass on the UI context to the form
                Me.m_form.UIContext = m_uic

            End If

            ' Activate the interface
            Me.m_form.Show()

            ' Pass a reference to the new interface back to whomever invoked us
            form = Me.m_form

            ' Just to show what can be done: test where this function was invoked from
            If TypeOf sender Is System.Windows.Forms.TreeNode Then
                ' Plug-in was invoked from the EwE6 navigation panel
            ElseIf TypeOf sender Is System.Windows.Forms.ToolStripMenuItem Then
                ' Plug-in was invoked from the EwE6 main menu
            End If
        Else
            Debug.Assert(False, "Plugin was not initialized properly.")
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 where to place an item in its main menu.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            ' For example, a plug-in menu item should be placed in the main the 'Tools' menu. 
            Return "MenuTools"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 when during application execution this plug-in should be accessible 
    ''' to users.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            ' This plug-in is available at any time during EwE execution
            Return EwEUtils.Core.eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 where to place an item in its navigation tree.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NavigationTreeItemLocation() As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            ' As an example, place a navigation tree item under the main 'tools' node.
            Return "ndTools"
        End Get
    End Property

#End Region ' User Interface plug-in implementation

#Region "IPlugin implementation"

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Me"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "you@someplace.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Run analysis for Habitat Capacity"
        End Get
    End Property


    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "Habitat Capacity Paper Plugin"
        End Get
    End Property

#End Region

End Class

