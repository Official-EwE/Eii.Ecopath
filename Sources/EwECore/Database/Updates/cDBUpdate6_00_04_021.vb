'==============================================================================
'
' $Log: cDBUpdate6_00_04_021.vb,v $
' Revision 1.1  2008/09/26 07:30:16  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/09/25 02:33:51  jeroens
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
''' <para>Database update 6.0.4.021:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecosim group max fishing mortality.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_04_021
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

        Return Me.AddMaxFishingMortality(db) And Me.SplitSDSal(db)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property UpdateDescription() As String Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateDescription
        Get
            Return "Added Ecosim group max fishing mortality." & vbNewLine & "Split salinity fields."
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
            Return 6.04021!
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

    Private Function AddMaxFishingMortality(ByVal db As cEwEDatabase) As Boolean

        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        Try
            db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN FishMortMax SINGLE")
        Catch ex As Exception

        End Try

        Return bSucces

    End Function

    Private Function SplitSDSal(ByVal db As cEwEDatabase) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim bSucces As Boolean = True

        Try
            db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN SdSalLeft SINGLE")
            db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN SdSalRight SINGLE")

            ' Copy SDSal to both
            writer = db.GetWriter("EcosimScenarioGroup")
            dt = writer.GetDataTable()

            For Each drow As DataRow In dt.Rows
                drow.BeginEdit()
                If Convert.IsDBNull(drow("SdSal")) Then
                    drow("SdSalLeft") = 0
                    drow("SdSalRight") = 0
                Else
                    drow("SdSalLeft") = drow("SdSal")
                    drow("SdSalRight") = drow("SdSal")
                End If
                drow.EndEdit()
            Next
            db.ReleaseWriter(writer)
            db.Execute("ALTER TABLE EcosimScenarioGroup DROP COLUMN SdSal")

        Catch ex As Exception

        End Try

        Return bSucces

    End Function

#End Region ' Internals

End Class
