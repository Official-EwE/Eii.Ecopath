'==============================================================================
'
' $Log: cDatabaseUpdate.vb,v $
' Revision 1.1  2008/09/26 07:30:17  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2007/07/25 03:08:37  jeroens
' * Moved cEwEDatabase to EwEUtils
'
' Revision 1.1  2006/05/03 03:06:59  cvsuser
' Initial version
'
'
'==============================================================================

Option Strict On

Imports System.Data.OleDb
Imports EwEUtils.Database

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The base class in EwE for implementing database updates
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public MustInherit Class cDatabaseUpdate

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Implement this method to specify the version number of this update
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property Version() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Implement this method to specify a brief description of this update
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property Description() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Override this method to apply the update
        ''' </summary>
        ''' <param name="db">The <see cref="cEwEDatabase">EwEDatabase</see> that needs updating</param>
        ''' <returns>True if succesful. Only return False if an update could not
        ''' complete or encountered an error it could not resolve since a
        ''' negative retun value may halt and possibly undo an entire chain of updates.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function Apply(ByRef db As cEwEDatabase) As Boolean

    End Class

End Namespace
