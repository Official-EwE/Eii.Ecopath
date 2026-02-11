' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging

Public Class cDietImporter
    Private m_EcopathData As cEcopathDataStructures
    Private m_Core As cCore
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cDietImporter)()

    Public Sub New(EwECore As cCore, EcopathData As cEcopathDataStructures)
        Me.m_Core = EwECore
        Me.m_EcopathData = EcopathData

    End Sub

    Public Sub Run(ExternalModelFileName As String)
        Dim DietPrefs As cDietPreferences
        Dim DBReader As New cDatabaseReader(Me.m_Core, Me.m_EcopathData)
        Dim DietCalculator As New cDietCalculator(Me.m_Core, Me.m_EcopathData)

        Try

            If Me.CheckEcopathState() Then
                If DBReader.ImportDietPreferences(ExternalModelFileName, DietPrefs) Then

                    If DietCalculator.DietsFromPreferences(DietPrefs) Then
                        'Yep it worked...
                        'DietCalculator.DietsFromPreferences() posted a message if the diets where loaded
                    End If

                End If
            End If ' If Me.CheckEcopathState() Then

        Catch ex As Exception
            m_logger.LogError(ex, "Exception while importing diets")
            'Message that the model needs to balancing
            Me.m_Core.Messages.SendMessage(New EwECore.cMessage("Exception while importing diets: " + ex.Message,
                                                                eMessageType.DataImport, eCoreComponentType.Plugin, eMessageImportance.Critical))
        End Try

    End Sub

    Private Function CheckEcopathState() As Boolean

        'Ok If Ecopath hasn't run this can not be run
        'In the current implementation this was handled by the UI
        If Me.m_Core.StateMonitor.HasEcopathRan Then
            Return True
        End If

        'shouldn't happen
        Me.m_Core.Messages.SendMessage(New EwECore.cMessage("You must run Ecopath to balance the current model before Importing Diets",
                                                                eMessageType.DataImport, eCoreComponentType.Plugin, eMessageImportance.Critical))

        Return False

    End Function

End Class
