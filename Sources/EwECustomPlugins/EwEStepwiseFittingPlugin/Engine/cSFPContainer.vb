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

''' <summary>
''' A iteration run container to execute SFP on its own core. Runs are asynchronous.
''' </summary>
Public Class cSFPContainer

    Private m_iScenario As Integer = 0
    Private m_iTS As Integer = 0

    Private m_iteration As ISFPIteration = Nothing

    Private m_core As cCore = Nothing

    ''' <summary>
    ''' Initializes a new instance of the <see cref="cSFPContainer"/> class.
    ''' </summary>
    ''' <param name="name">The name of the container.</param>
    ''' <param name="model">The model.</param>
    ''' <param name="iSim">The i sim.</param>
    ''' <param name="iTS">The i ts.</param>
    ''' <param name="params">The parameters.</param>
    Public Sub New(name As String, model As String, iSim As Integer, iTS As Integer, params As cSFPParameters)

        Me.Name = name
        Me.Model = model
        Me.Parameters = params
        Me.m_iScenario = iSim
        Me.m_iTS = iTS

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
                Me.m_iteration.RunState = ISFPIteration.eRunState.Stopping
                If (Me.m_core IsNot Nothing) Then Me.m_core.EcosimFitToTimeSeries.StopRun()
                RaiseEvent OnIterationUpdated(Me, Me.m_iteration, False)
            Catch ex As Exception
                ' ToDo: Log
            End Try
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

        Try

            ' Intermediate status update
            Me.m_iteration.RunState = ISFPIteration.eRunState.Initializing
            RaiseEvent OnIterationUpdated(Me, Me.m_iteration, False)

            ' Run iteration on local core
            Dim core As New cCore()
            core.Name = Me.Name

            Debug.WriteLine("Creating core " & Me.Name)

            ' No need to load plug-ins. Rather not, actually.
            'core.PluginManager = New EwEPlugin.cPluginManager()
            'core.PluginManager.Core = core ' Let's get to know each other, shall we?
            'core.PluginManager.LoadPlugins()

            bSuccess = core.LoadModel(Me.Model)
            Debug.Assert(bSuccess = True)

            bSuccess = bSuccess And core.LoadEcosimScenario(Me.m_iScenario)
            Debug.Assert(bSuccess = True)

            bSuccess = bSuccess And core.LoadTimeSeries(Me.m_iTS, False)
            Debug.Assert(bSuccess = True)

            Me.m_iteration.Init(core, Me.m_iTS, Me.Parameters.PredOrPredPreySSToV, Me.Parameters, Nothing)

            bSuccess = bSuccess And Me.m_iteration.Load(core)
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
                Me.m_core = core
                If Me.m_iteration.Run(core) Then
                    If (iter.RunState = ISFPIteration.eRunState.Stopping) Then
                        Me.m_iteration.RunState = ISFPIteration.eRunState.Idle
                    Else
                        Me.m_iteration.RunState = ISFPIteration.eRunState.Completed
                    End If
                Else
                    Me.m_iteration.RunState = ISFPIteration.eRunState.Error
                End If
            End If

            ' Just making sure
            Debug.Assert(Not core.StateMonitor.IsBusy, "Core " & Me.Name & " still working!")

            core.CloseEcosimScenario()
            core.CloseModel()
            core.Dispose()

            Debug.WriteLine("Disposed core " & Me.Name)

            ' Unlink
            Me.m_core = Nothing

        Catch ex As Exception
            Me.m_iteration.RunState = ISFPIteration.eRunState.Error
        End Try

        ' Free resources prior to sending the last update
        sw.Stop()
        Me.m_iteration.Elapsed = sw.Elapsed
        Me.m_iteration.Completed = Date.Now
        Me.m_iteration = Nothing

        ' Notify the world
        RaiseEvent OnIterationUpdated(Me, Me.m_iteration, True)

    End Sub

#End Region ' Internals

End Class
