'==============================================================================
'
' $Log: cDBUpdate6_00_05_005.vb,v $
' Revision 1.1  2009/03/18 15:22:25  jeroens
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
''' <para>Database update 6.0.5.005:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added PSD parameters.</description></item>
''' <item><description>Added PSD fields to groups.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_05_005
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
            Return 6.05005!
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
            Return "Added PSD parameters table." & vbNewLine & _
                   "Added PSD fields to groups."
        End Get
    End Property

    Public Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean _
            Implements EwEPlugin.IDatabaseUpdatePlugin.ApplyUpdate

        Return Me.AddPSDParameters(db) And _
               Me.AddPSDFields(db)

    End Function

    Private Function AddPSDParameters(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        Try

            bSucces = db.Execute("CREATE TABLE EcopathPSD (ModelID LONG, MortalityType INTEGER, NumAgeSteps INTEGER, NumWeightClasses INTEGER, FirstWeightClass SINGLE, LatNWCorner SINGLE, LatSECorner SINGLE)")
            bSucces = bSucces And db.Execute("ALTER TABLE EcopathPSD ADD CONSTRAINT PK_INDEX PRIMARY KEY (ModelID)")
            bSucces = bSucces And db.Execute("ALTER TABLE EcopathPSD ADD FOREIGN KEY (ModelID) REFERENCES EcopathModel(ModelID)")

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function AddPSDFields(ByVal db As cEwEDatabase) As Boolean

        Try
            Return db.Execute("ALTER TABLE EcopathGroup ADD COLUMN PSDInclude INTEGER")
        Catch ex As Exception
        End Try
        Return False

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
