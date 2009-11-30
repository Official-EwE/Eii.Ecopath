
Option Strict On
Imports System
Imports System.xml
Imports EwEUtils.Database

Public Interface IDatabaseUpdatePlugin
    Inherits IPlugin

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Implement this method to specify the version number of this update.
    ''' The version number returned here is used to check whether an update
    ''' is required to run.
    ''' </summary>
    ''' <remarks>
    ''' If -9999 is returned here, the update will run regardless of the
    ''' database version number.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    ReadOnly Property UpdateVersion() As Single

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Implement this method to specify a brief description of this update.
    ''' </summary>
    ''' -------------------------------------------------------------------
    ReadOnly Property UpdateDescription() As String

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Override this method to apply the update.
    ''' </summary>
    ''' <param name="db">The <see cref="cEwEDatabase">EwEDatabase</see> that needs updating</param>
    ''' <returns>True if succesful. Only return False if an update could not
    ''' complete or encountered an unresolvable error, since returning False
    ''' may halt an entire chain of updates.</returns>
    ''' -------------------------------------------------------------------
    Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

End Interface
