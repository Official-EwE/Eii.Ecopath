Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports EwEUtils.Utilities

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.2.0.04:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added multiple taxon codes.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_02_00_04
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
            Return 6.120004!
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
            Return "Restructured taxonomy codes"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Dim bSucces As Boolean = db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN CodeSAUP LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN CodeFB LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN CodeSLB LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN CodeLCID TEXT(255)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN CodeFAO TEXT(13)")
        db.Execute("ALTER TABLE EcopathTaxon DROP COLUMN CodeISCAAP, Code3A")
        Return bSucces
    End Function


End Class
