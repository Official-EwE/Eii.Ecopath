' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database
Imports Microsoft.Extensions.Logging

''' --------------------------------------------------------------------------
''' <summary>
''' Database update base class.
''' </summary>
''' --------------------------------------------------------------------------
Friend MustInherit Class cDBUpdate
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cDBUpdate)()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the update version number that will be entered in
    ''' the update log of the database. This version number is also used to check
    ''' whether an update should run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property UpdateVersion() As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property UpdateDescription() As String
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply the actual update
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write update progress to the log.
    ''' </summary>
    ''' <param name="strProgress">Progress entry to write.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub LogProgress(strProgress As String, bSucces As Boolean)
        m_logger.LogInformation("Update {0}: {1} {2}",
                                 Me.UpdateVersion,
                                 strProgress,
                                 If(bSucces, "Succes", "Failed"))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Message text to show to the user to take action, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property UserAction As String = ""

End Class
