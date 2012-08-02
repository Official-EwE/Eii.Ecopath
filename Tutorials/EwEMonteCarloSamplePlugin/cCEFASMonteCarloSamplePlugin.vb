Imports EwEPlugin
Imports EwECore
Imports EwECore.Ecosim


Public Class cCEFASMonteCarloSamplePlugin
    Implements ICorePlugin
    Implements IEcosimInitializedPlugin
    Implements IEcopathPlugin
    Implements IMenuItemPlugin

    Private _core As cCore
    Private _ecosim As EwECore.Ecosim.cEcoSimModel
    Private _simdata As cEcosimDatastructures
    Private _ecopath As Ecopath.cEcoPathModel

#Region "Sample MonteCarlo"


    Private Sub TestMonteCarlo()
        Dim nLiving As Integer = Core.nLivingGroups
        Dim MonteCarlo As cMonteCarloManager = Core.EcosimMonteCarlo

        _ecopath.suppressMessages = True

        If Me.InitMonteCarloParameters() Then
            'Succeeded in intitializing Monte Carlo Parameters

            'Dump out the Limits on Biomass
            System.Console.WriteLine("Upper and Lower bounds")
            For igrp = 1 To nLiving
                Dim mcGrp As cMonteCarloGroup = MonteCarlo.Groups(igrp)
                System.Console.Write("grp=" & igrp.ToString & ", " & mcGrp.BLower & ", " & mcGrp.BUpper & ", ")
            Next

            For iter As Integer = 1 To 10

                'Set the Ecopath parameters using the Monte Carlo input parameters set above
                If MonteCarlo.selectNewEcopathParameters() Then

                    'write some of the new Ecopath parameters to the console window
                    Me.dumpEcopathParameters(iter)

                    'This runs Ecosim
                    If Me.RunEcosim() Then
                        'dumps out some Ecosim results
                        Me.getEcosimResults()
                    End If

                Else
                    System.Console.WriteLine("Failed to find balanced Ecopath model")
                End If

            Next iter

        End If 'Me.InitMonteCarloParameters()


    End Sub


    Private Function InitMonteCarloParameters() As Boolean
        Try

            Dim MonteCarlo As cMonteCarloManager = Core.EcosimMonteCarlo
            Dim MCGroup As cMonteCarloGroup
            'Initialize Monte Carlo parameters for B, PB, QB, EE and BA
            'These are the group parameters in the EwE Monte Carlo runs form
            'CV Lower and Upper Limit
            'Mean is set to default as the Ecopath value and probable should not be changed here?

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

                'Ok Set a lower and upper limit on Biomass after CV
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
            Me._ecosim.EcosimData.bTimestepOutput = True

            'No timestep call back
            Me._ecosim.TimeStepDelegate = Nothing

            'Run on the same thread 
            'this means Me._ecosim.Run() will block until Ecosim has finished running
            Me._ecosim.EcosimData.bMultiThreaded = False

            'Run Ecosim without Core support 
            'This means Core Input/ouput objects will not be populate 
            'So you can not use cCore.EcoSimGroupOutputs() to retrieve the results
            Me._ecosim.Init(True)
            Return Me._ecosim.Run()

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
                    sumb(igrp) += Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, itime)
                Next itime

                System.Console.WriteLine("Average Biomass for " & Me._ecopath.EcopathData.GroupName(igrp) & " = " & (sumb(igrp) / Core.nEcosimTimeSteps).ToString)

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

    Private ReadOnly Property Core As cCore
        Get
            Debug.Assert(Me._core IsNot Nothing, "Core failed to initialize properly. Check  Sub Initialize(ByVal core As Object)")
            Return Me._core
        End Get
    End Property


#End Region

#Region "Interface Menu Events"

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick
        Try
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
                _core = DirectCast(core, cCore)
            End If

        Catch ex As Exception

        End Try

    End Sub

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized

        Debug.Assert(TypeOf objEcoSim Is EwECore.Ecosim.cEcoSimModel, "CoreInitialized() failed to pass in a valid EcosimModel!")
        If TypeOf objEcoSim Is EwECore.Ecosim.cEcoSimModel Then
            _ecosim = DirectCast(objEcoSim, EwECore.Ecosim.cEcoSimModel)
        End If

        Debug.Assert(TypeOf objEcoPath Is EwECore.Ecopath.cEcoPathModel, "CoreInitialized() failed to pass in a valid EcopathModel!")
        If TypeOf objEcoPath Is EwECore.Ecopath.cEcoPathModel Then
            _ecopath = DirectCast(objEcoPath, EwECore.Ecopath.cEcoPathModel)
        End If

    End Sub

    Public Sub EcosimInitialized(EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        Debug.Assert(TypeOf EcosimDatastructures Is cEcosimDatastructures, "EcosimInitialized() failed to pass in valid Ecosim Data!")
        If TypeOf EcosimDatastructures Is cEcosimDatastructures Then
            _simdata = DirectCast(EcosimDatastructures, cEcosimDatastructures)
        End If
    End Sub

    Public Function LoadModel(dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.LoadModel

    End Function

    Public Function SaveModel(dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.SaveModel

    End Function



#End Region

#Region "Core Plugin Stuff that needs to be here"

    Public ReadOnly Property Author() As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Mark Platts CEFAS"
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
            Return "CEFAS MonteCarlo Sample Plugin"
        End Get
    End Property


    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "CEFAS Monte Carlo Sample"
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
