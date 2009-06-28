'==============================================================================
'
' $Log: cDBUpdate6_00_05_005.vb,v $
' Revision 1.3  2009/06/28 01:33:29  jeroens
' Inherited from cDBUpdate
'
' Revision 1.2  2009/03/27 20:48:16  jeroens
' IncludePSD no longer stored in DB
'
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
            Return 6.05005!
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
            Return "Added PSD parameters table."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean 

        Return Me.AddPSDParameters(db)

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

End Class
