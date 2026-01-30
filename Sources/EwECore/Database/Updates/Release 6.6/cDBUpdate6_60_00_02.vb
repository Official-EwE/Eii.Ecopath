' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.02:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed branch merge conflict</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_02
    Inherits cDBUpdate6_60_00_01

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600002!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added second pool code to fleet time series, fixed samples"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        MyBase.ApplyUpdate(db)
        db.Execute("ALTER TABLE EcosimTimeSeriesFleet ADD COLUMN GroupID LONG")
        db.Execute("ALTER TABLE EcosimTimeSeriesFleet ADD FOREIGN KEY (GroupID) REFERENCES EcopathGroup(GroupID)")
        Return True

    End Function

End Class
