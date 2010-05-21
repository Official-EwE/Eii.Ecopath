Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports System.Data
Imports System.Data.OleDb
Imports EwEUtils.Utilities
Imports EwECore.DataSources.cDBDataSource

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.0.003:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecopath spatial context.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_00_003
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
            Return 6.100003!
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
            Return "Added Ecopath model spatial extent"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.UpdateEcopathModel(db)

    End Function

    Private Function UpdateEcopathModel(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        bSucces = bSucces And db.Execute("ALTER TABLE EcopathModel ADD COLUMN MinLat SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathModel ADD COLUMN MinLon SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathModel ADD COLUMN MaxLat SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathModel ADD COLUMN MaxLon SINGLE")

        Return bSucces

    End Function

End Class
