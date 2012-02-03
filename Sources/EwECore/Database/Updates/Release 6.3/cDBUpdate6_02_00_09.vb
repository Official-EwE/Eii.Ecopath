Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Utilities

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.120009:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added cell size to Ecospace scenario.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_12_00009
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
            Return 6.120009!
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
            Return "Added Ecospace cell size for spatial referencing"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN CellSize SINGLE")
    End Function

End Class
