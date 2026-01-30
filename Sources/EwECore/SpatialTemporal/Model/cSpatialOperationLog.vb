' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Text
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging



Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Central class for logging spatial operations.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The operations log generates <see cref="cMessage">core messages</see>, 
    ''' one message for a  given layer and time step, with <see cref="cMessage.Variables"/> 
    ''' for every spatial operation applied to the data for this layer.
    ''' </para>
    ''' <para>
    ''' To do this, the log behaves in a transaction-based manner. A layer log message 
    ''' is started by calling <see cref="cSpatialOperationLog.BeginLayerLog"/>. 
    ''' Consecutive <see cref="cSpatialOperationLog.LogOperation">LogOperation</see> calls
    ''' will add <see cref="cVariableStatus"/> entries to the message. The message
    ''' is terminated and sent out by calling <see cref="cSpatialOperationLog.EndLayerLog"/>.
    ''' </para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cSpatialOperationLog
        Implements IDisposable

#Region " Private vars "

        Private m_core As cCore = Nothing
        Private m_vn As eVarNameFlags = eVarNameFlags.NotSet
        Private m_iIndex As Integer = cCore.NULL_VALUE

        Private m_msgCurrent As cMessage = Nothing
        Private m_bLogStarted As Boolean = False
        Private m_strLogFileName As String = ""

        Private m_bRunOK As Boolean = True
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cSpatialOperationLog)()

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize a new instance of this class.
        ''' </summary>
        ''' <param name="core">The core to use for sending messages.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(core As cCore)

            Me.m_core = core
            AddHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf Me.OnCoreStateChanged

        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose

            If (Me.m_core IsNot Nothing) Then
                RemoveHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf Me.OnCoreStateChanged
                Me.m_core = Nothing
            End If
            GC.SuppressFinalize(Me)

        End Sub

#End Region ' Construction / destruction

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Start collection run diagnostics.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Sub BeginRun()
            Me.m_bRunOK = True
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Notify the log that the spatial framework has begun processing external
        ''' data for a map layer at a given time step. To finish processing
        ''' the layer call <see cref="EndLayerLog"/>.
        ''' </summary>
        ''' <param name="timestep">The time step that is currently being executed.</param>
        ''' <param name="dt">The absolute time represented by this time step.</param>
        ''' <param name="layer">The layer that is being processed.</param>
        ''' -------------------------------------------------------------------
        Friend Sub BeginLayerLog(timestep As Integer, dt As DateTime, varname As eVarNameFlags, layer As cEcospaceLayer)

            If (Me.m_msgCurrent IsNot Nothing) Then
                Me.EndLayerLog()
            End If

            Dim ftm As New Style.cVarnameTypeFormatter()
            Me.m_msgCurrent = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_LOADING, ftm.ToString(varname), layer.Name, timestep, dt),
                                           eMessageType.GISOperation, eCoreComponentType.External, eMessageImportance.Information)
            Me.m_vn = layer.VarName
            Me.m_iIndex = layer.Index

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Notify the log that the spatial framework is finished processing
        ''' external data for the layer and time step indicated in <see cref="BeginLayerLog"/>.
        ''' </summary>
        ''' <param name="bSendMessage">Flag, stating whether a status message
        ''' should be sent out. True by default.</param>
        ''' -------------------------------------------------------------------
        Friend Sub EndLayerLog(Optional bSendMessage As Boolean = True)

            Try
                If (Me.m_msgCurrent IsNot Nothing) Then
                    If (bSendMessage = True) Then
                        Me.m_core.Messages.SendMessage(Me.m_msgCurrent)
                    End If
                    Me.WriteMessage()
                End If

                Me.m_msgCurrent = Nothing

            Catch ex As Exception
                m_logger.LogError(ex, "cSpatialOperationLog::EndLayerLog() Error ending layer log")
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Log a spatial operation for the current layer.
        ''' </summary>
        ''' <param name="strMsg">The status message describing the operation.</param>
        ''' <param name="status">The result of the operation.</param>
        ''' -------------------------------------------------------------------
        Public Sub LogOperation(strMsg As String, status As eStatusFlags)

            If (Me.m_msgCurrent IsNot Nothing) Then
                Me.m_msgCurrent.AddVariable(New cVariableStatus(status, strMsg, Me.m_vn, eDataTypes.External, eCoreComponentType.External, Me.m_iIndex))
            End If

            Me.m_bRunOK = Me.m_bRunOK And Not (status = eStatusFlags.FailedValidation Or status = eStatusFlags.ErrorEncountered Or status = eStatusFlags.MissingParameter)

            If (Me.m_bRunOK = False) Then
                status = status
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Stop collecting run information.
        ''' </summary>
        ''' <returns>True if all spatial operations completed without errors.</returns>
        ''' -------------------------------------------------------------------
        Friend Function EndRun() As Boolean
            Return Me.m_bRunOK
        End Function

        Public ReadOnly Property LogFileName As String
            Get
                Return Me.m_strLogFileName
            End Get
        End Property

        Public Property LogFilePath As String = ""

#End Region ' Public access

#Region " Internals "

        Private Sub OnCoreStateChanged(csm As cCoreStateMonitor)
            If Not csm.IsEcospaceRunning Then
                If Me.m_bLogStarted Then
                    Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_SAVED, Me.m_strLogFileName),
                                            eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                    msg.Hyperlink = Path.GetDirectoryName(Me.m_strLogFileName)
                    Me.m_core.Messages.SendMessage(msg)

                    Me.m_bLogStarted = False
                    Me.m_strLogFileName = ""
                End If
            End If
        End Sub

        Private Sub WriteMessage()

            Dim sb As New StringBuilder()
            Dim strPath As String = Me.LogFilePath
            Dim strFile As String = ""
            Dim strModel As String = ""
            Dim sep As String = cStringUtils.vbTab

            If (Not Me.m_bLogStarted) Then

                strModel = Me.m_core.DataSource.ToString

                '' Allowed to use smart directory?
                'If Me.m_core.m_EcoSpaceData.UseCoreOutputDir Then
                '    ' #Yes: use core smartness
                '    strPath = Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace)
                'Else
                '    ' #No: use base output directory
                '    strPath = Me.m_core.OutputPath
                'End If

                If (String.IsNullOrWhiteSpace(strPath) Or Not Directory.Exists(strPath)) Then
                    strPath = Path.GetDirectoryName(strModel)
                End If
                strFile = Path.GetFileNameWithoutExtension(strModel)

                Me.m_strLogFileName = Path.Combine(strPath, strFile & "_spatiallog.txt")

                If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(Me.m_strLogFileName), True) Then
                    m_logger.LogError("cSpatialOperationsLog: unable to create output directory " + Me.m_strLogFileName)
                    Return
                Else
                    m_logger.LogInformation("cSpatialOperationsLog: saving to " + Me.m_strLogFileName)
                End If

                If Me.m_core.SaveWithFileHeader Then
                    sb.AppendLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
                    sb.AppendLine("Ecospace spatial operations log")
                End If

            End If

            'Double seperators to keep the columns aligned when loading the data in Excel
            sb.AppendLine("Message" & sep & Me.m_msgCurrent.Importance.ToString & sep & Me.m_msgCurrent.Message)
            For Each vs As cVariableStatus In Me.m_msgCurrent.Variables
                sb.AppendLine("Status" & sep & vs.Status.ToString & sep & vs.Message)
            Next

            If Not String.IsNullOrWhiteSpace(Me.m_msgCurrent.Hyperlink) Then
                sb.AppendLine("Output" + sep + sep + Me.m_msgCurrent.Hyperlink)
            End If

            'cLog.WriteTextToFile(Me.m_strLogFileName, sb, Me.m_bLogStarted)
            Me.m_bLogStarted = True

        End Sub

#End Region ' Internals

    End Class

End Namespace
