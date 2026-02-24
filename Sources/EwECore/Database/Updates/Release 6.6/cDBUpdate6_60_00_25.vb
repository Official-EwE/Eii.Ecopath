' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database
Imports Microsoft.Extensions.Logging

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.25:</para>
''' <para>
''' This update addresses an issue present since the very start of EwE6, where 
''' deleted fleets left behind orphaned fishing effort shapes. This update does
''' not alter the structure of the EwE database.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_25
    Inherits cDBUpdate

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cDBUpdate6_60_00_25)()

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600025!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed potential ecosampler saving problem"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delete orphaned fishing effort shapes
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        Try
            ' Get IDs for all orphaned shapes
            Dim reader As IDataReader = db.GetReader("SELECT ShapeID FROM EcosimShapeFishRate AS R WHERE NOT EXISTS (SELECT FishRateShapeID FROM EcosimScenarioFleet F WHERE F.FishRateShapeID = R.ShapeID)")
            Dim ShapeIDs As New List(Of Integer)
            While reader.Read
                ShapeIDs.Add(CInt(reader("ShapeID")))
            End While
            db.ReleaseReader(reader)

            ' Delete 'em all
            For Each iShapeID As Integer In ShapeIDs
                bSucces = bSucces And db.Execute(String.Format("DELETE FROM EcosimShapeFishRate WHERE ShapeID={0}", iShapeID))
                bSucces = bSucces And db.Execute(String.Format("DELETE FROM EcosimShape WHERE ShapeID={0}", iShapeID))
            Next

            m_logger.LogInformation("DBUpdate_{0}: Deleted {1} orphaned shapes", Me.UpdateVersion, ShapeIDs.Count)

        Catch ex As Exception
            ' Life is fantastic. Carry on.
        End Try
        Return True

    End Function

End Class
