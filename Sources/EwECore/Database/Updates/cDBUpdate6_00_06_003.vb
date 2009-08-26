Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.6.003:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed mediation issue.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_06_003
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
            Return 6.06003!
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
            Return "Complemented missing fleets for Ecosim on import of old EwE5 models."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean
        Return Me.FixEcosimFleets(db) And Me.FixDoubleLinkedEffortShapes(db)
    End Function

    Private Function FixEcosimFleets(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True
        Dim reader As IDataReader = Nothing
        Dim liFleets As New List(Of Integer)
        Dim liScenarios As New List(Of Integer)
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing

        ' 1) get list of all Ecopath fleets
        reader = db.GetReader("SELECT FleetID FROM EcopathFleet")
        While reader.Read
            liFleets.Add(CInt(reader("FleetID")))
        End While
        db.ReleaseReader(reader)

        ' 2) get list of all Ecosim scenarios
        reader = db.GetReader("SELECT ScenarioID FROM EcosimScenario")
        While reader.Read
            liScenarios.Add(CInt(reader("ScenarioID")))
        End While
        db.ReleaseReader(reader)

        ' 3) if not exists fleet (sim, path) then create it. Leave effort shape empty, loader will fix this
        For Each iFleetID As Integer In liFleets
            For Each iScenarioID As Integer In liScenarios
                Dim iNumFleets As Integer = CInt(db.GetValue(String.Format("SELECT * FROM EcoSimScenarioFleet WHERE (ScenarioID={0}) AND (EcopathFleetID={1})", iScenarioID, iFleetID)))
                If iNumFleets = 0 Then
                    writer = db.GetWriter("EcosimScenarioFleet")
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioID
                    drow("EcopathFleetID") = iFleetID
                    drow("FishRateShapeID") = 0 ' To be fixed when the app reloads
                    drow("MaxEffort") = cCore.NULL_VALUE
                    drow("QuotaType") = eQuotaTypes.NotUsed
                    writer.AddRow(drow)
                    db.ReleaseWriter(writer)
                End If
            Next iScenarioID
        Next iFleetID
        Return bSucces

    End Function

    Private Function FixDoubleLinkedEffortShapes(ByVal db As cEwEDatabase) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = db.GetWriter("EcosimScenarioFleet")
        Dim dt As DataTable = writer.GetDataTable()
        Dim drow As DataRow = Nothing
        Dim liShapes As New List(Of Integer)
        Dim iShape As Integer
        Dim bSucces As Boolean = True

        For Each drow In dt.Rows
            iShape = CInt(drow("FishRateShapeID"))
            If liShapes.Contains(iShape) Then
                drow.BeginEdit()
                drow("FishRateShapeID") = 0
                drow.EndEdit()
            Else
                liShapes.Add(iShape)
            End If
        Next

        writer.Commit()
        db.ReleaseWriter(writer)
        Return bSucces

    End Function

End Class
