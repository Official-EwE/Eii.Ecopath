' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.2.0.03:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added EwE version to updatelog.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_02_00_03
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.120003!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added ewe version to updatelog, scenarios"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return Me.AddLastSavedEwEVersions(db)
    End Function

    Private Function AddLastSavedEwEVersions(db As cEwEDatabase) As Boolean
        Dim bSucces As Boolean = db.Execute("ALTER TABLE UpdateLog ADD COLUMN EwEVersion TEXT(40)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathModel ADD COLUMN LastSavedVersion TEXT(40)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenario ADD COLUMN LastSavedVersion TEXT(40)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN LastSavedVersion TEXT(40)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcotracerScenario ADD COLUMN LastSavedVersion TEXT(40)")
        Return bSucces
    End Function


End Class
