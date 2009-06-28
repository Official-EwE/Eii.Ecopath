'==============================================================================
'
' $Log: cDBUpdate6_00_06_002.vb,v $
' Revision 1.1  2009/06/28 01:33:07  jeroens
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
''' <para>Database update 6.0.6.0623:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed mediation issue.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_06_002
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
            Return 6.06002!
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
            Return "Fixed mediation storage issue."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean
        Return Me.FixMediationFields(db)
    End Function

    Private Function FixMediationFields(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True
        Try
            bSucces = db.Execute("ALTER TABLE EcosimShapeMediation ADD COLUMN IMedBase INTEGER")
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimShapeMediation DROP COLUMN XBaseLine")
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

End Class
