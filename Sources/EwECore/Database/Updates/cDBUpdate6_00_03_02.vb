'==============================================================================
'
' $Log: cDBUpdate6_00_03_02.vb,v $
' Revision 1.2  2009/06/28 01:33:25  jeroens
' Inherited from cDBUpdate
'
' Revision 1.1  2008/09/26 07:30:15  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2008/02/11 03:20:23  jeroens
' Fixed CLS compliancy
'
' Revision 1.5  2007/11/24 17:51:37  jeroens
' * Fixed desciptions, name
'
' Revision 1.4  2007/10/31 14:15:09  jeroens
' * Fixed descriptions
'
' Revision 1.3  2007/10/30 19:21:01  jeroens
' + Plugins need Author, contact
'
' Revision 1.2  2007/10/12 21:28:42  jeroens
' * Changed a whole whack of numeric columns at risk of overflowing from Integer to Long
'
' Revision 1.1  2007/10/10 16:55:02  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.3.2:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Change EcospaceScenario Description column to MEMO</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_03_02
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
        ' - Ecospace description field has changed to type MEMO
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenario ALTER COLUMN Description MEMO")
        ' - Change a range of numeric columns from Integer to Long
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathFleet ALTER COLUMN Sequence LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathGroup ALTER COLUMN Sequence LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenario ALTER COLUMN TotalTime LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenario ALTER COLUMN NutForceNumber LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimTimeSeries ALTER COLUMN Sequence LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenario ALTER COLUMN Inrow LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenario ALTER COLUMN Incol LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenario ALTER COLUMN IDH_SS LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioHabitat ALTER COLUMN Sequence LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioMPA ALTER COLUMN Sequence LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE Stanza ALTER COLUMN HatchCode LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE StanzaLifeStage ALTER COLUMN Sequence LONG")

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
            Return "Fixes sequence field types, description field types"
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
            ' The DB version does not reflect a EwE release version
            Return 6.013!
        End Get
    End Property

End Class
