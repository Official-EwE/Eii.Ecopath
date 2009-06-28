'==============================================================================
'
' $Log: cDBUpdate6_00_04_003.vb,v $
' Revision 1.2  2009/06/28 01:33:28  jeroens
' Inherited from cDBUpdate
'
' Revision 1.1  2008/09/26 07:30:16  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/07/10 19:11:46  jeroens
' Stopped adding unnecessary columns
' Fixed bug in remove column statement
'
' Revision 1.2  2008/06/08 20:44:35  jeroens
' Removed EcopathGroup.vbK
'
' Revision 1.1  2008/05/26 20:31:57  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.4.003:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Removed Ecopath Group vbK, stored in Stanza life stage instead.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_04_0003
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The actual update logic.
    ''' </summary>
    ''' <param name="db">Database to modify.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean 

        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        Try
            db.Execute("ALTER TABLE EcopathGroup DROP COLUMN vbK")
        Catch ex As Exception

        End Try

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed group vbK issue"
        End Get
    End Property

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
            Return 6.04003!
        End Get
    End Property

End Class
