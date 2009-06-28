'==============================================================================
'
' $Log: cDBUpdate6_00_03_06.vb,v $
' Revision 1.2  2009/06/28 01:33:26  jeroens
' Inherited from cDBUpdate
'
' Revision 1.1  2008/09/26 07:30:15  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.10  2008/07/17 16:45:23  jeroens
' Added Tracer description
'
' Revision 1.9  2008/02/11 03:20:23  jeroens
' Fixed CLS compliancy
'
' Revision 1.8  2007/12/06 21:26:06  jeroens
' Stripped down to sync w release version
'
' Revision 1.7  2007/12/05 02:35:12  jeroens
' + Added ConForcingNumber to EcotracerScenario
'
' Revision 1.6  2007/12/04 02:25:12  jeroens
' * Added ecosim salinity vars
'
' Revision 1.5  2007/11/26 19:03:53  jeroens
' * Updated XML comments
'
' Revision 1.4  2007/11/26 02:08:57  jeroens
' + Added CExcretionRate
'
' Revision 1.3  2007/11/25 02:13:27  jeroens
' * GroupID -> EcopathGroupID; is more explicit
'
' Revision 1.2  2007/11/25 00:43:56  jeroens
' * One'd think we'd need a scenario name
'
' Revision 1.1  2007/11/24 18:57:38  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.3.6:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecotracer tables</description></item>
''' <item><description>Removed Ecoranger tables</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_03_06
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
        ' - Add Ecotracer tables
        bSucces = bSucces And db.Execute("CREATE TABLE EcotracerScenario (ScenarioID INTEGER, ScenarioName TEXT(50), Czero SINGLE, Cinflow SINGLE, Coutflow SINGLE, Cdecay SINGLE, Author TEXT(64), Contact TEXT(255), Description MEMO)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcotracerScenario ADD PRIMARY KEY (ScenarioID)")

        bSucces = bSucces And db.Execute("CREATE TABLE EcotracerScenarioGroup (ScenarioID INTEGER, EcopathGroupID INTEGER, Czero SINGLE, Cimmig SINGLE, Cenv SINGLE, Cdecay SINGLE, Cexcretionrate SINGLE)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcotracerScenarioGroup ADD PRIMARY KEY (ScenarioID, EcopathGroupID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcotracerScenarioGroup ADD FOREIGN KEY (ScenarioID) REFERENCES EcotracerScenario(ScenarioID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcotracerScenarioGroup ADD FOREIGN KEY (EcopathGroupID) REFERENCES EcopathGroup(GroupID)")

        ' - Discontinue ecoranger info in table EcopathModel if available (not essential)
        db.Execute("ALTER TABLE EcopathModel DROP COLUMN EcoRangerRangerRun")

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
            Return "Adds Ecotracer tables" + vbNewLine + "Removes Ecoranger tables"
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
            Return 6.036!
        End Get
    End Property

End Class
