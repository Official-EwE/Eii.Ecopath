'==============================================================================
'
' $Log: cDBUpdate6_00_01_01.vb,v $
' Revision 1.2  2009/06/28 01:33:25  jeroens
' Inherited from cDBUpdate
'
' Revision 1.1  2008/09/26 07:30:15  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/03/16 21:54:35  jeroens
' Fixed minor spelling error
'
' Revision 1.7  2008/02/11 03:15:29  jeroens
' Fixed CLS compliancy
'
' Revision 1.6  2007/11/24 17:51:37  jeroens
' * Fixed desciptions, name
'
' Revision 1.5  2007/10/31 14:15:09  jeroens
' * Fixed descriptions
'
' Revision 1.4  2007/10/30 19:21:01  jeroens
' + Plugins need Author, contact
'
' Revision 1.3  2007/10/10 16:48:23  jeroens
' * Plugin execution no longer protected by try/catch, this is handled by plugin manager
'
' Revision 1.2  2007/10/08 03:19:09  jeroens
' * Fixed bug in update query
'
' Revision 1.1  2007/09/24 18:54:45  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

''' --------------------------------------------------------------------------
''' <summary>
''' Database update 6.0.1.1: Add VisualStyles column to table Remark
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_01_01
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The actual update logic.
    ''' </summary>
    ''' <param name="db">Database to modify.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        ' Update(s):
        bSucces = bSucces And db.Execute("ALTER TABLE Remark ADD VisualStyles MEMO")

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
            Return "Added VisualStyles"
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
            Return 6.011!
        End Get
    End Property

End Class
