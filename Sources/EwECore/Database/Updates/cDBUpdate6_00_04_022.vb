'==============================================================================
'
' $Log: cDBUpdate6_00_04_022.vb,v $
' Revision 1.1  2008/10/03 18:13:53  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.4.022:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecosim fisheries regulation tables.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_04_022
    Implements IDatabaseUpdatePlugin

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
    Public ReadOnly Property UpdateVersion() As Single Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateVersion
        Get
            Return 6.04022!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property UpdateDescription() As String Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateDescription
        Get
            Return "Added Ecosim fisheries quota."
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The actual update logic.
    ''' </summary>
    ''' <param name="db">Database to modify.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean _
            Implements EwEPlugin.IDatabaseUpdatePlugin.ApplyUpdate

        Return Me.UpdateEcosimFleets(db) And Me.AddQuotaTable(db)

    End Function

    Private Function UpdateEcosimFleets(ByVal db As cEwEDatabase) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim bSucces As Boolean = True

        Try
            db.Execute("ALTER TABLE EcosimScenarioFleet ADD COLUMN MaxEffort SINGLE")
            db.Execute("ALTER TABLE EcosimScenarioFleet ADD COLUMN QuotaType INTEGER")

            writer = db.GetWriter("EcosimScenarioFleet")
            dt = writer.GetDataTable()

            For Each drow As DataRow In dt.Rows
                drow.BeginEdit()
                drow("MaxEffort") = -9999
                drow("QuotaType") = 0
                drow.EndEdit()
            Next
            db.ReleaseWriter(writer)

        Catch ex As Exception

        End Try

        Return bSucces

    End Function

    Private Function AddQuotaTable(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        Try

            bSucces = bSucces And db.Execute("CREATE TABLE EcosimScenarioQuota (FleetID LONG, EcosimGroupID LONG, Quota SINGLE, PropDiscardMort SINGLE)")
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioQuota ADD PRIMARY KEY (FleetID, EcosimGroupID)")
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioQuota ADD FOREIGN KEY (FleetID) REFERENCES EcopathFleet(FleetID)")
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioQuota ADD FOREIGN KEY (EcosimGroupID) REFERENCES EcosimScenarioGroup(GroupID)")

        Catch ex As Exception

        End Try

        Return bSucces

    End Function

#Region " Standard bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="IPlugin.Description">IPlugin.Description</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return Me.UpdateDescription
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="IPlugin.Initialize">IPlugin.Initialize</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        ' Void
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="IPlugin.Name">IPlugin.Name</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "Database update " & Me.UpdateVersion
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Author">IPlugin.Author</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Contact">IPlugin.Contact</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:support@ecopath.org"
        End Get
    End Property

#End Region ' Standard bits

End Class
