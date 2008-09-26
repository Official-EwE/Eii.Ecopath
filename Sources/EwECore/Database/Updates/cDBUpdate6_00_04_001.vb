'==============================================================================
'
' $Log: cDBUpdate6_00_04_001.vb,v $
' Revision 1.1  2008/09/26 07:30:16  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/04/07 22:33:43  jeroens
' Removed area to next update
'
' Revision 1.4  2008/04/07 17:00:51  jeroens
' Transactions committed properly
'
' Revision 1.3  2008/03/17 22:33:00  jeroens
' Hmm
'
' Revision 1.2  2008/03/07 18:20:00  jeroens
' Added Ecopath Area
'
' Revision 1.1  2008/02/22 21:42:19  jeroens
' Fixes vbK issue
'
'==============================================================================

Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.4.001:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed stanza life stage vbK issue.</description></item>
''' <item><description>Added Ecopath Area.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_04_0001
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

        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        db.BeginTransaction()

        ' Move vbK from EcopathGroup to StanzaLifeStage
        bSucces = bSucces And db.Execute("ALTER TABLE StanzaLifeStage ADD COLUMN vbK SINGLE")
        ' Access AARGH moment:
        ' - Nested SET queries do not work 
        'bSucces = bSucces And db.Execute("UPDATE StanzaLifeStage SET vbK=EcopathGroup.vbK FROM EcopathGroup WHERE EcopathGroup.GroupID=StanzaLifeStage.GroupID")
        ' - Updates using INNER JOIN will lock tables temporarily, causing the following ALTER TABLE command to fail
        'bSucces = bSucces And db.Execute("UPDATE StanzaLifeStage INNER JOIN EcopathGroup ON EcopathGroup.GroupID=StanzaLifeStage.GroupID SET StanzaLifeStage.vbK=EcopathGroup.vbK")
        ' - This also does not work (throws "Operation must be an updatable query")
        'bSucces = bSucces And db.Execute("UPDATE StanzaLifeStage SET vbK= (SELECT EcopathGroup.vbK FROM EcopathGroup WHERE EcopathGroup.GroupID=StanzaLifeStage.GroupID)")

        ' *DEEP sigh*
        reader = db.GetReader("SELECT EcopathGroup.GroupID, EcopathGroup.vbK FROM EcopathGroup, StanzaLifeStage WHERE EcopathGroup.GroupID=StanzaLifeStage.GroupID")
        If reader IsNot Nothing Then
            While reader.Read
                bSucces = bSucces And db.Execute(String.Format("UPDATE StanzaLifeStage SET vbK={0} WHERE GroupID={1}", reader("vbK"), reader("GroupID")))
            End While
        End If
        db.ReleaseReader(reader)
        ' Now drop the vbK column from GroupInfo
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathGroup DROP COLUMN vbK")

        If bSucces Then
            bSucces = db.CommitTransaction(True)
        Else
            db.RollbackTransaction()
        End If

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property UpdateDescription() As String Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateDescription
        Get
            Return "Fixes stanza life stage vbK issue." + vbNewLine + "Added Ecopath Area"
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
            Return 6.04001!
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

End Class
