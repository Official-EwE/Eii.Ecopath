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
'    Ecopath International Initiative, Barcelona, Spain
'    Scottish Association for Marine Science, Oban, Scotland
'
' Stepwise Fitting Procedure by Sheila Heymans, Erin Scott, Jeroen Steenbeek
' Copyright 2015- Scottish Association for Marine Science, Oban, Scotland
'
' Erin Scott was funded by the Scottish Informatics and Computer Science
' Alliance (SICSA) Postgraduate Industry Internship Programme.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports

' ToDo:
' - Remove scenario storage to parameters, remove from container

''' <summary>
''' A iteration run container to execute SFP on its own core. Runs are asynchronous.
''' </summary>
Public Class cSFPContainer

    Private m_iteration As ISFPIteration = Nothing

    ''' <summary>Local core, which is only valid while a run is in progress</summary>
    Private m_core As cCore = Nothing

    ''' <summary>
    ''' Initializes a new instance of the <see cref="cSFPContainer"/> class.
    ''' </summary>
    ''' <param name="name">The name of the container.</param>
    ''' <param name="model">The model file name to load.</param>
    ''' <param name="params">The parameters.</param>
    Public Sub New(name As String, model As String, params As cSFPParameters)

        Me.Name = name
        Me.Model = model
        Me.Parameters = params

    End Sub

    Public ReadOnly Property Name As String = ""
    Public ReadOnly Property Model As String = ""
    Public ReadOnly Property Parameters As cSFPParameters = Nothing

    Public Overrides Function ToString() As String
        Return Me.Name
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Runs the specified iteration.
    ''' </summary>
    ''' <param name="iteration">The iteration.</param>
    ''' <returns>True if the thread launched successfully.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Run(iteration As ISFPIteration) As Boolean

        If (Me.IsRunning) Then Return False

        Me.m_iteration = iteration

        Dim thread As New Threading.Thread(AddressOf Me.Run)
        thread.Name = "STWF (" & Me.Name & ")"
        thread.Start()

        Return True

    End Function

    Public Event OnIterationUpdated(cnt As cSFPContainer, iter As ISFPIteration, bDone As Boolean)

    Public ReadOnly Property IsRunning As Boolean
        Get
            Return (Me.m_iteration IsNot Nothing)
        End Get
    End Property

    Public Sub StopRun()

        If (Me.IsRunning) Then
            Try
                SyncLock (Me.m_iteration)
                    Me.m_iteration.RunState = ISFPIteration.eRunState.Stopping
                    If (Me.m_core IsNot Nothing) Then
                        Me.m_core.EcosimFitToTimeSeries.StopRun()
                    End If
                End SyncLock
            Catch ex As Exception
                ' ToDo: Log
            End Try
            RaiseEvent OnIterationUpdated(Me, Me.m_iteration, False)
        Else
            RaiseEvent OnIterationUpdated(Me, Me.m_iteration, True)
        End If

    End Sub

#Region " Internals "

    ''' <summary>
    ''' Perform a step-wise run
    ''' </summary>
    Private Sub Run()

        Dim bSuccess As Boolean = True
        Dim sw As New Stopwatch()
        sw.Start()

        ' Intermediate status update
        Me.m_iteration.RunState = ISFPIteration.eRunState.Initializing
        RaiseEvent OnIterationUpdated(Me, Me.m_iteration, False)

        ' Create local core to work on
        SyncLock (Me.m_iteration)
            Me.m_core = New cCore()
            Me.m_core.Name = Me.Name
        End SyncLock

        Debug.WriteLine("Creating core " & Me.Name)

        Try

            ' No need to load plug-ins. Rather not, actually.
            'core.PluginManager = New EwEPlugin.cPluginManager()
            'core.PluginManager.Core = core ' Let's get to know each other, shall we?
            'core.PluginManager.LoadPlugins()

            bSuccess = Me.m_core.LoadModel(Me.Model)
            Debug.Assert(bSuccess = True)

            bSuccess = bSuccess And Me.m_core.LoadEcosimScenario(Me.Parameters.EcosimScenario)
            Debug.Assert(bSuccess = True)

            bSuccess = bSuccess And Me.m_core.LoadTimeSeries(Me.Parameters.TimeSeriesDataset, False)
            Debug.Assert(bSuccess = True)

            Me.m_iteration.Init(Me.m_core, Me.Parameters)

            bSuccess = bSuccess And Me.m_iteration.Load(Me.m_core)
            Debug.Assert(bSuccess = True)

            ' Has stop request been received?
            If Me.m_iteration.RunState = ISFPIteration.eRunState.Stopping Then
                ' Flag as idle and done
                Me.m_iteration.RunState = ISFPIteration.eRunState.Idle
            Else
                ' Start running
                Me.m_iteration.RunState = ISFPIteration.eRunState.Running
                RaiseEvent OnIterationUpdated(Me, Me.m_iteration, False)

                ' Run and complete
                If Me.m_iteration.Run(Me.m_core) Then
                    If (m_iteration.RunState = ISFPIteration.eRunState.Stopping) Then
                        Me.m_iteration.RunState = ISFPIteration.eRunState.Idle
                    Else
                        Me.m_iteration.RunState = ISFPIteration.eRunState.Completed
                        Me.m_iteration.SaveResults(Me.m_core)
                    End If
                Else
                    Me.m_iteration.RunState = ISFPIteration.eRunState.Error
                End If
            End If
        Catch ex As Exception
            Me.m_iteration.RunState = ISFPIteration.eRunState.Error
        End Try

        ' Just making sure
        Debug.Assert(Not Me.m_core.StateMonitor.IsBusy, "Core " & Me.Name & " still working!")

        Debug.WriteLine("Disposed core " & Me.Name)

        ' Free resources prior to sending the last update
        sw.Stop()
        Me.m_iteration.Elapsed = sw.Elapsed
        Me.m_iteration.Completed = Date.Now
        Dim iter As ISFPIteration = Me.m_iteration
        Me.m_iteration = Nothing

        Me.m_core.CloseEcosimScenario()
        Me.m_core.CloseModel()
        Me.m_core.Dispose()
        Me.m_core = Nothing

        ' Notify the world
        RaiseEvent OnIterationUpdated(Me, iter, True)

    End Sub

#End Region ' Internals

End Class
