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
Imports System.Net
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' MEL contact point for interacting with Ecospace.
''' </summary>
Public Class cEwEShell

#Region " Private vars "

    Private m_game As cGame = Nothing

#End Region ' Private vars

#Region " Construction / destruction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Stand-alone constructor when used in an EwE console environment.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        Me.New(New cCore())

        Dim pm As New cPluginManager()
        Me.Core.PluginManager = pm
        pm.LoadPlugins()

        Try
            Me.Controller = CType(pm.GetPlugins(GetType(cEcospaceController))(0), cEcospaceController)
        Catch ex As Exception
            RaiseException("EwEShell.Constructor failed to locate Ecospace controller plug-in. " & ex.Message, False)
        End Try

        'Me.Controller.IsSaveOutput = True
        'Me.Controller.OutputPath = ".\outcomes"

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Nested constructor when used in an EwE plug-in.
    ''' </summary>
    ''' <param name="core">The core, obtained from the master plug-in, to initailize 
    ''' EwE shell against</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore)

        Me.Core = core
        Me.Data = New cShellData(core)

    End Sub

#End Region ' Construction / destruction

#Region " MEL API "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the game configuration from a JSON file.
    ''' </summary>
    ''' <param name="strFile">The path/URI to the JSON file to (down)load.</param>
    ''' <param name="effortstartvalues"><see cref="cScalar"/> values with effort intensity start values in EwE</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ConfigurationFromFile(strFile As String, effortstartvalues As List(Of cScalar)) As Boolean

        Try
            Dim sr As New StreamReader(strFile)
            Dim strJSON As String = sr.ReadToEnd
            sr.Close()
            Return Configuration(strJSON, effortstartvalues)
        Catch ex As Exception
            RaiseException("EwEShell.ConfigurationFromFile unable to read JSON file. " & ex.Message, False)
            Return False
        End Try
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load a new game and validate its configuration against the expectations of MEL.
    ''' </summary>
    ''' <param name="strJSON">Actual JSON game contract text (not the file!)</param>
    ''' <param name="effortstartvalues"><see cref="cScalar"/> values with effort intensity start values in EwE</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Configuration(strJSON As String, effortstartvalues As List(Of cScalar)) As Boolean

        Dim cfg As New cJSONGameConfig()
        Try
            cfg.Load(strJSON)
        Catch ex As Exception
            RaiseException("EwEShell.Configuration unable to parse JSON text. " & ex.Message, False)
        End Try

        Return Me.Configuration(cfg.EwEModelFile, cfg.Mode, cfg.Timestep,
                                cfg.Longitude, cfg.Latitude, cfg.CellSize, cfg.NumColumns, cfg.NumRows,
                                cfg.Pressures, cfg.Outcomes, effortstartvalues, cfg.OutcomeRange)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load a new game and validate its configuration against the expectations of MEL.
    ''' </summary>
    ''' <param name="strEwEModelFile">The path or URI to the EwE model to (down)load.</param>
    ''' <param name="mode">The name of the game to load. Leaving the name empty will load the first available game.</param>
    ''' <param name="timestep">Check: the number of time steps per year.</param>
    ''' <param name="longitude">Check: the model longitude origin to validate. Ignored for now because of slight numerical precision differences between MSP, MEL and EwE.</param>
    ''' <param name="latitude">Check: the model latitude origin to validate. Ignored for now because of slight numerical precision differences between MSP, MEL and EwE.</param>
    ''' <param name="size">Check: the model cell size. Ignored for now because of slight numerical precision differences between MSP, MEL and EwE.</param>
    ''' <param name="ncolumns">Check: the number of columns in the model.</param>
    ''' <param name="nrows">Check: the number of rows in the model.</param>
    ''' <param name="pressures">Check: the pressures that the model should support.</param>
    ''' <param name="outcomelayers">Check: the outcomes that the model should support.</param>
    ''' <param name="outcomerange">Biomass / catch fluctuations for outcomes to display</param>
    ''' <param name="effortstartvalues"><see cref="cScalar"/> values with effort intensity start values in EwE</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Configuration(strEwEModelFile As String, mode As String, timestep As Integer,
                                  longitude As Double, latitude As Double, size As Double, ncolumns As Integer, nrows As Integer,
                                  pressures As ICollection(Of cPressure), outcomelayers As ICollection(Of cGrid),
                                  effortstartvalues As List(Of cScalar),
                                  Optional outcomerange As Double = 10.0!) As Boolean

        Terminate()

        strEwEModelFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), strEwEModelFile)

        ' Load model, or download if needed
        If Not File.Exists(strEwEModelFile) Then
            Try
                Dim sc As New WebClient()
                Dim f As String = cFileUtils.MakeTempFile(Path.GetExtension(strEwEModelFile))
                sc.DownloadFile(strEwEModelFile, f)
                strEwEModelFile = f
            Catch ex As Exception
                RaiseException("EwEShell.Configuration: Failed to download model from " & strEwEModelFile & ". " & ex.Message, False)
                Return False
            End Try
        End If

        If (Not File.Exists(strEwEModelFile)) Then
            RaiseException("EwEShell.Configuration: Model " & strEwEModelFile & " not found", False)
            Return False
        End If
        Debug.WriteLine("Model file " & strEwEModelFile & " exists")

        If Not Core.LoadModel(strEwEModelFile) Then
            RaiseException("EwEShell.Configuration: Model " & strEwEModelFile & " cannot be loaded by the EwE core.", True)
            Return False
        End If
        Debug.WriteLine("EwEShell model " & Core.EwEModel.Name & " loaded")

        If Not LoadConfiguration() Then
            RaiseException("EwEShell.Configuration: Game descriptions not found in model, or could not be loaded.", False)
            Return False
        End If
        Debug.WriteLine("EwEShell configuration loaded: " & Me.Data.Games.Count & " games")

        m_game = Data.Game(mode)

        If (m_game Is Nothing) Then
            RaiseException("EwEShell.Configuration: There is no game defined for mode '" & mode & "'", True)
            Return False
        End If

        If (Not m_game.Load()) Then
            ' Raise no exception: game.load will do this
            Me.Terminate()
            Return False
        End If
        Debug.WriteLine("EwEShell game '" & Me.m_game.Name & "' loaded, version " & Me.m_game.Version)

        ' *shudder*
        Me.m_game.OutcomeRange = outcomerange

        If (Not m_game.Validate(timestep, longitude, latitude, size, ncolumns, nrows, pressures, outcomelayers)) Then
            ' Raise no exception: game.validate will do this
            Me.Terminate()
            Return False
        End If

        ' Provide effort start values scales as configured in the game
        If (effortstartvalues IsNot Nothing) Then
            effortstartvalues.Clear()
            effortstartvalues.AddRange(Me.m_game.EffortStartValues())
        End If

        Debug.WriteLine("EwEShell configuration validated")

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' MEL API call to start a game.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Startup()

        ' JS 07Mar17: Startup will just prepare the controller but will NOT start Ecospace.
        '             Ecospace will be launched on the first Run command, along with pressure
        '             layer content which is needed for any Spinup period

        If (Me.Controller Is Nothing) Then
            RaiseException("EwEShell.Startup can only execute in console mode.", False)
            Return
        End If

        If (Me.m_game Is Nothing) Then
            RaiseException("EwEShell.Startup game undefined, call Configuration first. Startup aborted.", False)
            Return
        End If

        ' Run!
        Me.Controller.Start(Me.m_game)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run a single time step.
    ''' </summary>
    ''' <param name="pressures"></param>
    ''' <param name="outcomelayers"></param>
    ''' -----------------------------------------------------------------------
    Public Sub Tick(pressures As List(Of cPressure), outcomelayers As List(Of cGrid))

        If (Me.Controller Is Nothing) Then
            RaiseException("EwEShell.Run can only execute in console mode", False)
            Return
        End If

        If (Me.m_game Is Nothing) Then
            RaiseException("EwEShell.Run game undefined, call Configuration first. Run aborted.", False)
            Return
        End If

        If (Not Me.Controller.IsRunning) Then
            RaiseException("EwEShell.Run game not running, call Startup first. Run aborted.", False)
            Return
        End If

        Me.Controller.Continue(pressures.ToArray(), outcomelayers.ToArray())
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Stop a game.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Terminate()

        If (Me.Controller Is Nothing) Then
            RaiseException("EwEShell.Terminate can only execute in console mode", False)
            Return
        End If

        ' Safety tests
        If (Me.m_game Is Nothing) Then Return
        If (Not Me.Controller.IsRunning) Then Return

        Me.Controller.Stop()

        Me.m_game = Nothing

        Me.Core.DiscardChanges()
        Me.Core.CloseModel()

        GC.Collect()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether outcomes should be saved to disk.
    ''' </summary>
    ''' <seealso cref="SaveOutcomeLocation"/>
    ''' -----------------------------------------------------------------------
    Public Property SaveOutcomes As Boolean
        Get
            Return Me.Controller.IsSaveOutput
        End Get
        Set(value As Boolean)
            Me.Controller.IsSaveOutput = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the directory where outcomes should be saved.
    ''' </summary>
    ''' <seealso cref="SaveOutcomes"/>
    ''' -----------------------------------------------------------------------
    Public Property SaveOutcomeLocation As String
        Get
            Return Me.Controller.OutputPath
        End Get
        Set(value As String)
            Me.Controller.OutputPath = value
        End Set
    End Property

#End Region ' MEL API

#Region " Data lifespan "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Unique key for accessing <see cref="cAuxiliaryData"/> that contains the game XML data
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Const cAuxKey As String = "MSPChallenge2050Config"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the EwE <see cref="cCore">core</see> to operate onto.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Core() As cCore = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cShellData">EwE shell data</see> to use.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Data() As cShellData = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cEcospaceController">Ecospace controller</see> to use.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Controller As cEcospaceController = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the currently active <see cref="cGame">game</see>, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property CurrentGame As cGame
        Get
            Return Me.m_game
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load game configurations embedded within a EwE model.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadConfiguration() As Boolean

        Dim aux As cAuxiliaryData = Me.Core.AuxillaryData(cAuxKey)
        Return Me.Data.FromXML(aux.Remark)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the game configuration from a XML file.
    ''' </summary>
    ''' <param name="strFile">The game configuration XML file.</param>
    ''' <param name="strMode">the game mode to use.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadConfiguration(strFile As String, strMode As String) As Boolean

        Try
            Using sr As New StringReader(strFile)
                Me.Data.FromXML(sr.ReadToEnd)
            End Using
        Catch ex As Exception
            ' ToDo: panic
            Return False
        End Try
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the game configuration to a XML file.
    ''' </summary>
    ''' <param name="strFile">The game configuration XML file.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function SaveConfiguration(strFile As String) As Boolean

        Try
            Using sw As New StreamWriter(strFile)
                sw.WriteLine(Me.Data.ToXML)
            End Using
        Catch ex As Exception
            ' ToDo: panic
            Return False
        End Try
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the current configuration in the core, ready for saving. This
    ''' will <see cref="cCore.HasChanges()">dirty the core</see> for saving.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub OnChanged()

        Try
            Dim aux As cAuxiliaryData = Me.Core.AuxillaryData(cAuxKey)
            aux.Remark = Me.Data.ToXML()
        Catch ex As Exception
            ' ToDo: panic
        End Try

    End Sub

#End Region ' Data lifespan

#Region " Internals "

    ''' <summary>
    ''' Aargh! Aarghaargh!
    ''' </summary>
    Friend Shared Sub RaiseException(strError As String, bEwEDetails As Boolean)
        Try
            cLog.Write("MEL exception thrown: " & strError)
            If (bEwEDetails) Then strError = strError & " See EwE error log for details"

            Debug.WriteLine("EwEShell Throwing exception '" & strError & "'")

            Dim ex As New cMELException(strError)
            Throw ex
        Catch ex As Exception
            ' Plop
        End Try
    End Sub

#End Region ' Internals

End Class
