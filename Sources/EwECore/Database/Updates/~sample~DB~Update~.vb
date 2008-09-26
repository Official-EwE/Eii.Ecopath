'==============================================================================
'
' $Log: ~sample~DB~Update~.vb,v $
' Revision 1.1  2008/09/26 07:30:17  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2007/09/24 18:54:31  jeroens
' * Renamed
'
' Revision 1.2  2007/07/26 18:05:09  jeroens
' - Disabled; is for sample purpose
'
' Revision 1.1  2007/07/26 12:25:56  jeroens
' - Experimental plugin-based database update
'
'==============================================================================

Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

' Database update example

#If 0 Then

''' ---------------------------------------------------------------------------
''' <summary>
''' Database update to version 6.011
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class cDBUpdate6011
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

        ' MS Access does not support setting column defaults via Alter Table.
        '    This fails: "ALTER TABLE EcosimParameters ADD Trivial SINGLE NOT NULL DEFAULT 1.0"
        Dim strSQL As String = "ALTER TABLE EcosimParameters ADD Trivial SINGLE NOT NULL"
        Try
            Return db.Execute(strSQL)
        Catch ex As Exception
            ' Oops
        End Try
        Return False

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property UpdateDescription() As String Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateDescription
        Get
            Return "Ecosim run parameters"
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
            Return 6.011!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="IPlugin.Description">IPlugin.Description</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Database update 6.011"
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
            Return Me.Description()
        End Get
    End Property

End Class

#End If