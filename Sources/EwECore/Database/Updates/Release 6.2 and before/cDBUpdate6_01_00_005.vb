' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.0.005:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Removed unused time series fields.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_00_005
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the update version number that will be entered in
    ''' the update log of the database. This version number is also used to check
    ''' whether an update should run.
    ''' </summary>
    ''' <remarks>
    ''' If <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> is provided, the
    ''' update is ran regardless of version number.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.100005!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Cleaned up unused time series fields"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.CleanUpTimeSeries(db)

    End Function

    Private Function CleanUpTimeSeries(db As cEwEDatabase) As Boolean

        Return db.Execute("ALTER TABLE EcosimTimeSeriesGroup DROP COLUMN VariableName")

    End Function

End Class
