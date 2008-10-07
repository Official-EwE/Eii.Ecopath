'==============================================================================
'
' $Log: cDBUpdate6_00_04_022.vb,v $
' Revision 1.5  2008/10/07 00:38:46  jeroens
' Ecosim prey/pred ff table flipped
'
' Revision 1.4  2008/10/06 17:21:54  jeroens
' Fixed flip
'
' Revision 1.3  2008/10/06 16:33:13  jeroens
' Flipped Vulnerabilities matrix in database
'
' Revision 1.2  2008/10/03 19:04:14  jeroens
' Argh, slightly revised Quota table layout
'
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
''' <item><description>Updated group x group index tables.</description></item>
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
            Return "Added Ecosim fisheries quota." & vbNewLine & "Fixed Ecosim vulnerabilities matrix structure."
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

        Return Me.UpdateEcosimFleets(db) And Me.AddQuotaTable(db) And _
            Me.FlipVulMult(db) And Me.FlipPredPreyShapes(db)

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

            bSucces = bSucces And db.Execute("CREATE TABLE EcosimScenarioQuota (ScenarioID LONG, FleetID LONG, EcosimGroupID LONG, Quota SINGLE, PropDiscardMort SINGLE)")
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioQuota ADD PRIMARY KEY (ScenarioID, FleetID, EcosimGroupID)")
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioQuota ADD FOREIGN KEY (ScenarioID) REFERENCES EcosimScenario(ScenarioID)")
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioQuota ADD FOREIGN KEY (FleetID) REFERENCES EcopathFleet(FleetID)")
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioQuota ADD FOREIGN KEY (EcosimGroupID) REFERENCES EcosimScenarioGroup(GroupID)")

        Catch ex As Exception

        End Try

        Return bSucces

    End Function

    Private Structure cVulRowRecord
        Public m_iScenario As Integer
        Public m_iPredator As Integer
        Public m_iPrey As Integer
        Public m_sVulnerability As Single
    End Structure

    Private Function FlipVulMult(ByVal db As cEwEDatabase) As Boolean

        Dim reader As IDataReader = Nothing
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim entry As cVulRowRecord = Nothing
        Dim lEntries As New List(Of cVulRowRecord)
        Dim bSucces As Boolean = True

        Try
            Try
                reader = db.GetReader("SELECT * FROM EcosimScenarioForcingMatrix")
                While reader.Read
                    entry = New cVulRowRecord()
                    entry.m_iScenario = CInt(reader("ScenarioID"))
                    entry.m_iPredator = CInt(reader("PredID"))
                    entry.m_iPrey = CInt(reader("PreyID"))
                    entry.m_sVulnerability = CInt(reader("Vulnerability"))
                    lEntries.Add(entry)
                End While
                db.ReleaseReader(reader)

                db.Execute("DELETE * FROM EcosimScenarioForcingMatrix")

                writer = db.GetWriter("EcoSimScenarioForcingMatrix")
                For Each entry In lEntries
                    drow = writer.NewRow()
                    drow("ScenarioID") = entry.m_iScenario
                    ' FLIP!
                    drow("PredID") = entry.m_iPrey
                    drow("PreyID") = entry.m_iPredator
                    ' Copy vul
                    drow("Vulnerability") = entry.m_sVulnerability
                    writer.AddRow(drow)
                Next
                db.ReleaseWriter(writer)

                bSucces = bSucces And db.Execute("ALTER TABLE EcoSimScenarioForcingMatrix DROP COLUMN flowtype")

            Catch ex As Exception
                ' All good, no sim groups
            End Try

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Structure cPredPreyShapeRowRecord
        Public m_iScenario As Integer
        Public m_iPredator As Integer
        Public m_iPrey As Integer
        Public m_iShapeID As Integer
        Public m_iFunctionType As eForcingFunctionApplication
    End Structure

    Private Function FlipPredPreyShapes(ByVal db As cEwEDatabase) As Boolean

        Dim reader As IDataReader = Nothing
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim entry As cPredPreyShapeRowRecord = Nothing
        Dim lEntries As New List(Of cPredPreyShapeRowRecord)
        Dim bSucces As Boolean = True

        Try
            Try
                reader = db.GetReader("SELECT * FROM EcosimScenarioPredPreyShape")
                While reader.Read
                    entry = New cPredPreyShapeRowRecord()
                    entry.m_iScenario = CInt(reader("ScenarioID"))
                    entry.m_iPredator = CInt(reader("PredID"))
                    entry.m_iPrey = CInt(reader("PreyID"))
                    entry.m_iShapeID = CInt(reader("ShapeID"))
                    entry.m_iFunctionType = DirectCast(reader("FunctionType"), eForcingFunctionApplication)
                    lEntries.Add(entry)
                End While
                db.ReleaseReader(reader)

                db.Execute("DELETE * FROM EcosimScenarioPredPreyShape")

                writer = db.GetWriter("EcosimScenarioPredPreyShape")
                For Each entry In lEntries
                    drow = writer.NewRow()
                    drow("ScenarioID") = entry.m_iScenario
                    ' FLIP!
                    drow("PredID") = entry.m_iPrey
                    drow("PreyID") = entry.m_iPredator
                    ' Copy vul
                    drow("ShapeID") = entry.m_iShapeID
                    drow("FunctionType") = entry.m_iFunctionType
                    writer.AddRow(drow)
                Next
                db.ReleaseWriter(writer)

            Catch ex As Exception
                ' All good, no sim groups
            End Try

        Catch ex As Exception
            bSucces = False
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
