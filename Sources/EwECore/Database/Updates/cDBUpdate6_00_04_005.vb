'==============================================================================
'
' $Log: cDBUpdate6_00_04_005.vb,v $
' Revision 1.1  2008/09/26 07:30:16  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/08/16 18:09:26  jeroens
' Aargh!
'
' Revision 1.4  2008/08/15 03:44:19  jeroens
' MPAaaaaargh
'
' Revision 1.3  2008/08/14 01:36:18  jeroens
' Fixed dataset import crash
'
' Revision 1.2  2008/08/08 15:39:44  jeroens
' Added Sequence field to preserve layer order
'
' Revision 1.1  2008/08/07 18:36:43  jeroens
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
''' <para>Database update 6.0.4.005:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Adds Ecospace weight layer tables.</description></item>
''' <item><description>Fixed field lengths.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_04_0005
    Implements IDatabaseUpdatePlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The actual update logic.
    ''' </summary>
    ''' <param name="db">Database to modify.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean _
            Implements EwEPlugin.IDatabaseUpdatePlugin.ApplyUpdate

        Return Me.AddEcospaceWeightTables(db) And Me.FixFieldLengths(db)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property UpdateDescription() As String Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateDescription
        Get
            Return "Adds Ecospace weight layer tables" & vbNewLine & "Fixed field lengths"
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
    Public ReadOnly Property UpdateVersion() As Single Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateVersion
        Get
            Return 6.04005!
        End Get
    End Property

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

#Region " Internals "

    Private Function AddEcospaceWeightTables(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        db.BeginTransaction()

        ' + Add EcospaceScenarioWeightLayer
        bSucces = bSucces And db.Execute("CREATE TABLE EcospaceScenarioWeightLayer (ScenarioID LONG, LayerID LONG, Sequence INTEGER, Name TEXT(50), Description MEMO, Weight SINGLE)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioWeightLayer ADD PRIMARY KEY (LayerID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioWeightLayer ADD FOREIGN KEY (ScenarioID) REFERENCES EcospaceScenario(ScenarioID)")
        ' + Add EcospaceScenarioWeightLayerCell
        bSucces = bSucces And db.Execute("CREATE TABLE EcospaceScenarioWeightLayerCell (ScenarioID LONG, LayerID LONG, InRow INTEGER, InCol INTEGER, Weight SINGLE)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioWeightLayerCell ADD PRIMARY KEY (ScenarioID, LayerID, InRow, InCol)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioWeightLayerCell ADD FOREIGN KEY (LayerID) REFERENCES EcospaceScenarioWeightLayer(LayerID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioWeightLayerCell ADD FOREIGN KEY (ScenarioID) REFERENCES EcospaceScenario(ScenarioID)")

        If bSucces Then
            bSucces = bSucces And db.CommitTransaction(True)
        Else
            db.RollbackTransaction()
        End If

        Return bSucces

    End Function

    Private Function FixFieldLengths(ByVal db As cEwEDatabase) As Boolean
        Return db.Execute("ALTER TABLE EcosimTimeSeriesDataset ALTER COLUMN DatasetName TEXT(255)")
    End Function

#End Region ' Internals

End Class
