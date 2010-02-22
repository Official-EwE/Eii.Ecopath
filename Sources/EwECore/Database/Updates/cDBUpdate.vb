Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data
Imports EwEUtils.Core
Imports System.Xml

''' --------------------------------------------------------------------------
''' <summary>
''' Database update base class.
''' </summary>
''' --------------------------------------------------------------------------
Public MustInherit Class cDBUpdate

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
    Public MustOverride ReadOnly Property UpdateVersion() As Single 

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property UpdateDescription() As String 
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply the actual update
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean 

End Class
