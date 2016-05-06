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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwEPlugin
Imports EwECore
Imports EwECore.Ecosim


Public Class cMonteCarloPlugin
    Implements ICorePlugin
    Implements IEcosimInitializedPlugin
    Implements IMenuItemPlugin

    Private m_core As cCore
    Private m_ecosim As EwECore.Ecosim.cEcoSimModel
    Private m_simdata As cEcosimDatastructures
    Private m_ecopath As Ecopath.cEcoPathModel

    Private m_EcosimTimeStepDelegate As EwECore.Ecosim.EcoSimTimeStepDelegate

#Region "Sample MonteCarlo"

    ''' <summary>
    ''' PROOF OF CONCEPT ONLY Vary and normalize the Diet Matrix 
    ''' </summary>
    Private Sub SampleDietMatrix()

        'This routine is coppied from cEcopathModel.checkDietsSumToOne()
        'and is only a "proof of concept" 
        'you will need to check that it is actually doing what it claims to
        Dim iPred As Integer
        Dim iPrey As Integer

        'The diet matrix is stored in cEcopathDataStructures.DC(pred,prey)
        'Get the cEcopathDataStructures object from the Ecopath Model object
        Dim ecopathData As cEcopathDataStructures = Me.m_ecopath.EcopathData
        Dim randNumGen As New Random

        For iPred = 1 To ecopathData.NumLiving
            For iPrey = 1 To ecopathData.NumLiving

                'does this pred eats this prey?
                If ecopathData.DC(iPred, iPrey) > 0 Then
                    'Yep... change the diet matix
                    'You will have to do better then a uniform distribution!
                    ecopathData.DC(iPred, iPrey) = CSng(randNumGen.NextDouble())
                End If
            Next
        Next

        'Now the diet matrix has been varied
        'Normalize the new diet matrix
        Dim sSum As Single
        Dim sTolerance As Single
        Dim bSumToOne As Boolean = True

        sTolerance = 0.001

        ' Check all diets
        For iPred = 1 To ecopathData.NumLiving
            ' Is consumer?
            If ecopathData.PP(iPred) < 1 Then
                ' #Yes: determine diet sum
                sSum = 0
                For iPrey = 0 To ecopathData.NumGroups
                    sSum = sSum + ecopathData.DC(iPred, iPrey)
                Next
                If sSum <> 0 And Math.Abs(sSum - 1) > sTolerance Then
                    For iPrey = 0 To ecopathData.NumGroups
                        ecopathData.DC(iPred, iPrey) = ecopathData.DC(iPred, iPrey) / sSum
                    Next

                End If
            End If
        Next

    End Sub


    Private Sub TestMonteCarlo()
        Dim nLiving As Integer = Core.nLivingGroups
        Dim MonteCarlo As cMonteCarloManager = Core.EcosimMonteCarlo

        'cMonteCarloManager.selectNewEcopathParameters() will alter the Ecopath Input parameters
        'We need to save the original state of Ecopath so it can be restored when we are done
        Me.SaveOriginalState()

        Try

            'Init some of the Monte Carlo parameters
            If Me.InitMonteCarloParameters() Then
                'Succeeded in intitializing Monte Carlo Parameters

                'Dump out the Limits on Biomass
                'for debuging to make sure InitMonteCarloParameters worked
                System.Console.WriteLine("Group, Mean, Lower, upper")
                For igrp = 1 To nLiving
                    Dim mcGrp As cMonteCarloGroup = MonteCarlo.Groups(igrp)
                    System.Console.Write("grp=" & igrp.ToString & ", " & mcGrp.B & ", " & mcGrp.BLower & ", " & mcGrp.BUpper & ", ")
                Next

                'Loop over the new Ecopath parameters and run Ecosim 
                For iter As Integer = 1 To 10

                    Me.SampleDietMatrix()

                    'Set the Ecopath parameters using the Monte Carlo input parameters set above
                    If MonteCarlo.selectNewEcopathParameters() Then

                        'write some of the new Ecopath parameters to the console window
                        'Again for debugging
                        Me.dumpEcopathParameters(iter)

                        'This runs Ecosim without core support
                        If Me.RunEcosim() Then
                            'dumps out some Ecosim results
                            Me.getEcosimResults()
                        End If 'RunEcosim

                    Else
                        System.Console.WriteLine("Failed to find balanced Ecopath model")
                    End If ' MonteCarlo.selectNewEcopathParameters()

                Next iter

            End If 'Me.InitMonteCarloParameters()

        Catch ex As Exception

        End Try

        Me.RestoreOriginalState()

    End Sub

    Private Function InitMonteCarloParameters() As Boolean
        Try

            Dim MonteCarlo As cMonteCarloManager = Core.EcosimMonteCarlo
            Dim MCGroup As cMonteCarloGroup
            'Initialize Monte Carlo parameters for B, PB, QB, EE and BA
            'These are the group parameters in the EwE Monte Carlo runs form
            'CV Lower and Upper Limit
            'Mean is the Ecopath value and probable should not be changed here?

            For igrp = 1 To Core.nLivingGroups
                MCGroup = MonteCarlo.Groups(igrp)

                'Setting a CV value will automatically set the Lower and Upper limits
                'by Calling cEcosimMonteCarlo.CalculateUpperLowerLimits()
                'If you want to manually set limits it must be done after the CV has been set

                'Biomass CV
                MCGroup.Bcv = 0.05
                'PB CV
                MCGroup.PBcv = 0.05
                'QB CV
                MCGroup.QBcv = 0.05
                'EE CV
                MCGroup.EEcv = 0.05

            Next

            'Ok now that all the CV have been set 
            'set the upper and lower limits
            For igrp = 1 To Core.nLivingGroups
                MCGroup = MonteCarlo.Groups(igrp)

                'Set a lower and upper limit on Biomass after CV
                MCGroup.BLower = MCGroup.B - MCGroup.B * 0.5F
                MCGroup.BUpper = MCGroup.B + MCGroup.B * 0.5F

            Next

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitMonteCarloParameters() Exception: " & ex.Message)
        End Try

        Return False
    End Function

    Private Function RunEcosim() As Boolean

        Try

            'make sure Ecosim computes the output data
            Me.m_ecosim.EcosimData.bTimestepOutput = True

            'No timestep call back
            Me.m_ecosim.TimeStepDelegate = Nothing

            'Run on the same thread 
            'this means Me._ecosim.Run() will block until Ecosim has finished running
            Me.m_ecosim.EcosimData.bMultiThreaded = False

            'Run Ecosim without Core support 
            'This means Core Input/ouput objects will not be populate 
            'So you can not use cCore.EcoSimGroupOutputs() to retrieve the results
            Me.m_ecosim.Init(True)
            Return Me.m_ecosim.Run()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".RunEcosim() Exception: " & ex.Message)
        End Try

        Return False

    End Function


    Private Function getEcosimResults() As Boolean
        Try
            'Because we ran Ecosim directly from cEcosimModel.Run() instead of via the core cCore.RunEcosim()
            'the Core output objects cCore.EcoSimGroupOutputs() will not be populated
            'Instead get the Ecosim results directly from the underlying arrays
            Dim sumb() As Single
            ReDim sumb(Core.nLivingGroups)
            For igrp As Integer = 1 To Core.nLivingGroups
                'sum biomass over all the Ecosim timesteps
                For itime As Integer = 1 To Core.nEcosimTimeSteps
                    'see cEcosimModel.PopulateResults() for how ResultsOverTime(var,group,time) are stored
                    sumb(igrp) += Me.m_simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, itime)
                Next itime

                System.Console.WriteLine("Average Biomass for " & Me.m_ecopath.EcopathData.GroupName(igrp) & " = " & (sumb(igrp) / Core.nEcosimTimeSteps).ToString)

            Next igrp

        Catch ex As Exception

        End Try

    End Function

    Private Sub dumpEcopathParameters(iteration As Integer)
        Dim nliving As Integer = Me.Core.nLivingGroups
        Dim MonteCarlo As cMonteCarloManager = Me.Core.EcosimMonteCarlo

        System.Console.WriteLine("Iteration = " & iteration.ToString)
        For igrp = 1 To nliving
            Dim mcGrp As cMonteCarloGroup = MonteCarlo.Groups(igrp)
            System.Console.Write(mcGrp.Name & " = " & mcGrp.B & " , ")
            'Other parameters...  mcGrp.PB
        Next igrp
        System.Console.WriteLine()

    End Sub

    ''' <summary>
    ''' Save any variable that will be changed so the model can be restore to it's original state 
    ''' </summary>
    ''' <remarks>This just stores a sub set of variable as an example</remarks>
    Private Sub SaveOriginalState()
        Try
            'Have the MonteCarloManager save the values it will alter
            Core.EcosimMonteCarlo.SaveOriginalValues()

            'Now store the variables that this app will change so they can be restored in RestoreOriginalState()

            'The makes sure Ecopath does not make a fuss, popping up message boxes, when it fails to balance a model
            Me.m_ecopath.suppressMessages = True

            'Make sure nothing is listening to Ecosim when we run it
            Me.m_EcosimTimeStepDelegate = Me.m_ecosim.TimeStepDelegate
            Me.m_EcosimTimeStepDelegate = Nothing

            'Save any parameters that we are going to change 
            'This has not been implemented here but...
            'For igrp = 1 To Core.nLivingGroups
            '    MCGroup = MonteCarlo.Groups(igrp)
            '   _orgB(igrp) =  MCGroup.Bcv 
            '    'PB, QB...               
            'Next

        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' Restore the currently loaded model back to it's original state so that it can be run in the interface.
    ''' </summary>
    ''' <remarks>In some cases you may want to save changes you made to the model.</remarks>
    Private Sub RestoreOriginalState()
        Try
            'Have the MonteCarloManager restore it's variables to the original state
            Core.EcosimMonteCarlo.RestoreOriginalValues()

            'Set the State variables that we changed back to their original state
            Me.m_ecopath.suppressMessages = False
            Me.m_ecosim.TimeStepDelegate = Me.m_EcosimTimeStepDelegate

            'Not included here but we should also set any Monte Carlo Parameters back to their original state
            'For example
            'For igrp = 1 To Core.nLivingGroups
            '    MCGroup = MonteCarlo.Groups(igrp)
            '    MCGroup.Bcv = _orgB(igrp)
            '    'PB, QB...               
            'Next

        Catch ex As Exception

        End Try

    End Sub

    Private ReadOnly Property Core As cCore
        Get
            Debug.Assert(Me.m_core IsNot Nothing, "Core failed to initialize properly. Check  Sub Initialize(ByVal core As Object)")
            Return Me.m_core
        End Get
    End Property


#End Region

#Region "Interface Menu Events"

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick
        Try

            'testMultipleEcosimRuns()
            TestMonteCarlo()

        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Initialization"

    Public Sub Initialize(ByVal core As Object) _
        Implements EwEPlugin.IPlugin.Initialize

        Try
            Debug.Assert(TypeOf core Is cCore, "Oh My IPlugin.Initialize() failed to pass in a valid core!")
            If TypeOf core Is cCore Then
                m_core = DirectCast(core, cCore)
            End If

        Catch ex As Exception

        End Try

    End Sub

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized

        Debug.Assert(TypeOf objEcoSim Is EwECore.Ecosim.cEcoSimModel, "CoreInitialized() failed to pass in a valid EcosimModel!")
        If TypeOf objEcoSim Is EwECore.Ecosim.cEcoSimModel Then
            m_ecosim = DirectCast(objEcoSim, EwECore.Ecosim.cEcoSimModel)
        End If

        Debug.Assert(TypeOf objEcoPath Is EwECore.Ecopath.cEcoPathModel, "CoreInitialized() failed to pass in a valid EcopathModel!")
        If TypeOf objEcoPath Is EwECore.Ecopath.cEcoPathModel Then
            m_ecopath = DirectCast(objEcoPath, EwECore.Ecopath.cEcoPathModel)
        End If

    End Sub

    Public Sub EcosimInitialized(EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        Debug.Assert(TypeOf EcosimDatastructures Is cEcosimDatastructures, "EcosimInitialized() failed to pass in valid Ecosim Data!")
        If TypeOf EcosimDatastructures Is cEcosimDatastructures Then
            m_simdata = DirectCast(EcosimDatastructures, cEcosimDatastructures)
        End If
    End Sub

#End Region

#Region "Core Plugin Stuff that needs to be here"

    Public ReadOnly Property Author() As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "EwEDevTeam"
        End Get
    End Property

    Public ReadOnly Property Contact() As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "not me"
        End Get
    End Property

    Public ReadOnly Property Description() As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return Me.Name
        End Get
    End Property

    Public ReadOnly Property Name() As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "MonteCarlo Sample Plugin"
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Monte Carlo Sample"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    Public ReadOnly Property MenuItemLocation As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

#End Region


End Class
