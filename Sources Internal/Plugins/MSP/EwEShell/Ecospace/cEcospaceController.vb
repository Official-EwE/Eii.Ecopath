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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Threading
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Single-threaded Ecospace execution controller for MSP game connectivity. 
''' All control functions are blocking for the purpose of MEL, waiting for the
''' threaded Ecospace model to complete a requested operation.
''' </summary>
''' <remarks>
''' This class is implemented as a plug-in to obtain access to the <see cref="cEcospaceDataStructures">
''' Ecospace datastructures</see>. 
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cEcospaceController
    Implements IEcospaceInitializedPlugin

#Region " Private vars "

    ''' <summary>The eevnt wait handle to synchronize Ecospace threading.</summary>
    Private Shared s_pausewait As New EventWaitHandle(False, EventResetMode.AutoReset)

    ' -- EwE --
    Private m_core As cCore = Nothing
    Private m_thread As Thread = Nothing
    Private m_spaceDS As cEcospaceDataStructures = Nothing
    Private m_bRunning As Boolean = False
    Private m_bStopping As Boolean = False

    ' -- MSP --
    Private m_game As cGame = Nothing
    Private m_pressures As cPressure()
    Private m_outcomes As cGrid()

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new instance of the <see cref="cEcospaceController"/> class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        Me.IsSaveOutput = False
    End Sub

#End Region ' Constructor

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Start an Ecospace run.
    ''' </summary>
    ''' <param name="game">The <see cref="cGame"/> to launch Ecospace for.</param>
    ''' <remarks>Note that this only prepares the threads needed to run. Ecospace
    ''' will only start executing when Continue is called."/>
    ''' </remarks>
    ''' <seealso cref="[Continue](cPressure(), cGrid())"/>
    ''' <seealso cref="[Stop]()"/>
    ''' -----------------------------------------------------------------------
    Public Sub Start(game As cGame)

        Debug.WriteLine("@@ Ecospace controller: starting")

        Me.m_game = game

        Debug.Assert(Me.m_core.ActiveEcospaceScenarioIndex > 0)

        If (Me.m_thread IsNot Nothing) Then
            Dim ts As ThreadState = Me.m_thread.ThreadState
            If (ts = ThreadState.Running Or ts = ThreadState.WaitSleepJoin) Then
                Me.Stop()
            End If
            Me.m_thread = Nothing
        End If

        Debug.Assert(Me.m_thread Is Nothing)

        Me.m_bRunning = True

        Debug.WriteLine("@@ Ecospace controller: started")

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Perform a single Ecospace time step.
    ''' </summary>
    ''' <param name="pressures">The pressures to apply to the time step.</param>
    ''' <param name="outcomes">The outcomes to fill during the time step.</param>
    ''' <seealso cref="Start(cPressure(), cGrid())"/>
    ''' <seealso cref="[Stop]()"/>
    ''' -----------------------------------------------------------------------
    Public Sub [Continue](pressures As cPressure(), outcomes As cGrid())

        Debug.WriteLine("@@ Ecospace controller: continuing")

        ' Bail out
        If (Not Me.m_bRunning) Then Return

        Me.m_pressures = pressures
        Me.m_outcomes = outcomes

        If (Me.m_pressures IsNot Nothing) Then
            Me.m_game.ApplyPressures(Me.m_pressures.ToArray())
        End If

        Try
            If (Me.m_thread Is Nothing) Then
                Me.m_thread = New Thread(New ThreadStart(AddressOf RunModel))
                Me.m_thread.Start()
            End If
        Catch ex As Exception
            cLog.Write(ex, "cEcospaceController.Continue")
        End Try

        Me.m_core.EcospacePaused = False
        s_pausewait.WaitOne()

        Debug.WriteLine("@@ Ecospace controller: continued")

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Terminate an Ecospace run.
    ''' </summary>
    ''' <seealso cref="[Continue](cPressure(), cGrid())"/>
    ''' <seealso cref="[Start](cGame)"/>
    ''' -----------------------------------------------------------------------
    Public Sub [Stop]()

        Debug.WriteLine("@@ Ecospace controller: stopping")

        ' Bail out
        If (Not Me.m_bRunning) Then Return

        Try
            Me.m_bStopping = True
            Me.m_core.StopEcospace()
            s_pausewait.WaitOne()
        Catch ex As Exception
            cLog.Write(ex, "cEcospaceController.Stop")
        End Try

        Me.m_bStopping = False
        Me.m_bRunning = False
        Me.m_game = Nothing

        Debug.WriteLine("@@ Ecospace controller: stopped")

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State flag, returns whether Ecospace is running.
    ''' </summary>
    ''' <returns>True if running.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsRunning() As Boolean
        Return Me.m_bRunning
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State flag, returns whether Ecospace is stopping.
    ''' </summary>
    ''' <returns>True if stopped.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsStopping() As Boolean
        Return Me.IsRunning And Me.m_bStopping
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether outcome grids need to be saved to file.
    ''' </summary>
    ''' <returns>True if outputs need saving to file.</returns>
    ''' -----------------------------------------------------------------------
    Public Property IsSaveOutput As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the output location for <see cref="IsSaveOutput">saved outputs</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property OutputPath As String = ".\"

#End Region ' Public access

#Region " Plug-in bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the plugin.
    ''' </summary>
    ''' <param name="core">The core this plugin is initialized for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(core As Object) Implements IPlugin.Initialize

        Me.m_core = CType(core, cCore)

        Dim syncContext As SynchronizationContext = SynchronizationContext.Current
        If (syncContext Is Nothing) Then syncContext = New SynchronizationContext()

        Me.m_core.Messages.AddMessageHandler(New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.EcoSpace, eMessageType.EcospaceRunCompleted, syncContext))
        ' Debug.WriteLine("@@ Ecospace controller: initialized")

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that is called when Ecospace has loaded a new scenario, is
    ''' initialized, and is ready to be used. Implemented to receive the <see cref="cEcospaceDataStructures">
    ''' Ecospace data structures</see>.
    ''' </summary>
    ''' <param name="EcospaceDatastructures">The ecospace datastructures that
    ''' just received new scenario data.</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
        Me.m_spaceDS = CType(EcospaceDatastructures, cEcospaceDataStructures)
        'Console.WriteLine("Ecospace controller: got data structures")
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the unique name for this plug-in point.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "MSP"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the description of this plug-in point.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Plug-in for controlling the Ecospace execution flow."
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the author of this plug-in point.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the contact information of this plug-in point.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "jeroen@ecopathinternational.org"
        End Get
    End Property

#End Region ' Plug-in bits

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Core message handler to respond to Ecospace run events.
    ''' </summary>
    ''' <param name="msg">The core message that may be the one we're waiting for.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnCoreMessage(ByRef msg As cMessage)

        Select Case msg.Type
            Case eMessageType.EcospaceRunCompleted

                Debug.WriteLine("@@ Ecospace controller: Ecospace run completed")

                s_pausewait.Set()
                Me.m_bStopping = False
                Me.m_bRunning = False

                Me.m_core.DiscardChanges()

        End Select

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Launch Ecospace.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub RunModel()

        Dim parms As cEcospaceModelParameters = Me.m_core.EcospaceModelParameters
        parms.TotalTime = Me.m_game.RunYears

        Me.m_spaceDS.SpinUpYears = Me.m_game.SpinupYears
        Me.m_spaceDS.UseSpinUp = (Me.m_spaceDS.SpinUpYears > 0)
        Me.m_spaceDS.bCalTrophicLevel = Me.m_game.CalculateIndicators

        Me.m_core.RunEcoSpace(New cCore.EcoSpaceInterfaceDelegate(AddressOf EcoSpaceCallback))
        'Debug.WriteLine("@@ Ecospace controller: Ecospace launched")

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace time step callback.
    ''' </summary>
    ''' <param name="results">The results.</param>
    ''' -----------------------------------------------------------------------
    Private Sub EcoSpaceCallback(ByRef results As cEcospaceTimestep)

        Try
            ' Just making sure. Ecospace is currently configured not to invoke the callback
            ' when a spin-up is active, but that could change. At least we're accounting for that eventuality here
            If (Me.m_spaceDS.bInSpinUp) Then
                'Debug.WriteLine("@@ Ecospace controller: time step " & results.iTimeStep & " (spinup)")
                Return
            End If

            ' Grab outputs
            'Debug.WriteLine("@@ Ecospace controller: time step " & results.iTimeStep)

            If (Me.m_outcomes IsNot Nothing) Then
                Me.m_game.LoadOutcomes(Me.m_outcomes, results)
                Me.SaveOutcomesToDisk(results.iTimeStep)
            End If

        Catch ex As Exception

        End Try

        s_pausewait.Set()
        Me.m_core.EcospacePaused = Not Me.m_bStopping

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves the outcomes to disk if <see cref="IsSaveOutput"/> is enabled.
    ''' </summary>
    ''' <param name="iTime">The time step to write the outputs for.</param>
    ''' -----------------------------------------------------------------------
    Private Sub SaveOutcomesToDisk(iTime As Integer)

        If (Not IsSaveOutput) Then Return
        If (Not cFileUtils.IsDirectoryAvailable(Me.OutputPath, True)) Then Return

        Debug.WriteLine("@@ Ecospace controller: Saving outcomes to " & Me.OutputPath)

        Try

            For Each grid As cGrid In m_outcomes
                If (grid.IsValid) Then
                    Dim strFile As String = cFileUtils.ToValidFileName(grid.Name & "-" & iTime.ToString("D5") & ".asc", False)
                    grid.Save(Path.Combine(Me.OutputPath, strFile), Me.m_core)
                End If
            Next
        Catch ex As Exception
            cLog.Write(ex, "cEcospaceController.SaveOutcomesToDisk(" & iTime & ")")
            Debug.WriteLine("@@ Ecospace controller: Exception saving outcomes " & ex.Message & ". Outcome saving disabled")
            Me.IsSaveOutput = False
        End Try

    End Sub

#End Region ' Internals

End Class
