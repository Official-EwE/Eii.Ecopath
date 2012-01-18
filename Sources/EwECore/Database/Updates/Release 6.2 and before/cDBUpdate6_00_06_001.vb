Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.6.001:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Reworked PSD fields.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_00_06_001
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
            Return 6.06001!
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
            Return "Reworked PSD fields."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean

        Return Me.AddPSDParameters(db) And Me.FixPSDDefaults(db)

    End Function

    Private Function AddPSDParameters(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        Try

            bSucces = db.Execute("ALTER TABLE EcopathPSD ADD COLUMN ClimateType INTEGER")
            ' No need to convert Lat corners to climate zones since PSD logic has not made it 'live' yet
            bSucces = bSucces And db.Execute("ALTER TABLE EcopathPSD DROP COLUMN LatNWCorner")
            bSucces = bSucces And db.Execute("ALTER TABLE EcopathPSD DROP COLUMN LatSECorner")

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function FixPSDDefaults(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        Try

            bSucces = db.Execute("UPDATE EcopathGroup SET t0=-9999 WHERE t0>1")

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

End Class
