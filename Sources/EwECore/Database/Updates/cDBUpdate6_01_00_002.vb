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
''' <para>Database update 6.1.0.002:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added more MSE fields.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_00_002
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
            Return 6.100002!
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
            Return "Added fixed mortality MSE fields."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.UpdateMSETables(db)

    End Function

    Private Function UpdateMSETables(ByVal db As cEwEDatabase) As Boolean
        Return db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN FixedF SINGLE")
    End Function

End Class
