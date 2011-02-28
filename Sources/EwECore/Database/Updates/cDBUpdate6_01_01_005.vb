Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.1.005:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added other mort input</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_01_005
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
            Return 6.101005!
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
            Return "Added Ecopath other mort field."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return db.Execute("ALTER TABLE EcopathGroup ADD COLUMN OtherMort SINGLE")
    End Function

End Class
