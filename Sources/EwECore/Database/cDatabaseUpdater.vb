'==============================================================================
'
' $Log: cDatabaseUpdater.vb,v $
' Revision 1.1  2008/09/26 07:30:17  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2007/07/26 12:21:50  jeroens
' - Simplified; updates now performed by PluginManager and IDatabaseUpdatePlugins
'
' Revision 1.4  2007/07/25 03:08:37  jeroens
' * Moved cEwEDatabase to EwEUtils
'
' Revision 1.3  2006/07/10 01:40:40  jeroens
' + Added versioning diagnostics
'
' Revision 1.2  2006/05/03 04:31:54  cvsuser
' Initial version
'
'==============================================================================

Option Strict On

Imports System.IO
Imports System.Data
Imports System.Data.OleDb
Imports EwEPlugin
Imports EwEUtils.Database

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Utility class to update a database across minor versions within one major version.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cDatabaseUpdater

#Region " Public interfaces "

        ''' <summary>The baseline database version that this updater can update from</summary>
        Private m_sBaselineVersion As Single = 0.0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal sBaselineVersion As Single)
            ' Store baseline version number
            Me.m_sBaselineVersion = sBaselineVersion
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform a database update
        ''' </summary>
        ''' <param name="db">The <see cref="cEwEDatabase">database</see> to update</param>
        ''' <returns>True if succesful</returns>
        ''' <remarks>
        ''' More elaborate status info may be required to populate a tracking GUI.
        ''' This could be implemented either via a public accessible status object 
        ''' that gets populated during every update step, or via delegates.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Function UpdateDatabase(ByVal db As cEwEDatabase, ByVal pm As cPluginManager) As Boolean

            Dim bSucces As Boolean = True

            ' Invoke database update plugin point prior to loading
            If (pm IsNot Nothing) Then
                Try
                    bSucces = pm.UpdateDatabase(db, m_sBaselineVersion)
                Catch ex As Exception
                    ' Throw new exception?
                    bSucces = False
                End Try
            End If

            Return bSucces

        End Function

#End Region ' Updating

    End Class

End Namespace
