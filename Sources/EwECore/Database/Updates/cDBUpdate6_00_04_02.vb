'==============================================================================
'
' $Log: cDBUpdate6_00_04_02.vb,v $
' Revision 1.1  2008/09/26 07:30:16  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/09/24 15:52:35  jeroens
' Made fail-safe when Currency units are missing
'
' Revision 1.1  2008/09/17 01:22:48  jeroens
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
''' <para>Database update 6.0.4.02:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed units.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_04_02
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

        Return Me.FixCurrencyUnits(db)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property UpdateDescription() As String Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateDescription
        Get
            Return "Fixed units."
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
            Return 6.0402!
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

    Private Function FixCurrencyUnits(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True
        Dim iCurrentValue As Integer = -1
        Dim unit As eUnitCurrencyType = eUnitCurrencyType.NotSet

        db.BeginTransaction()

        Try
            ' Try to get value, could be DBNull (which is OK)
            iCurrentValue = CInt(db.GetValue("SELECT UnitCurrency FROM EcopathModel WHERE ModelID=1"))
        Catch ex As Exception

        End Try

        Select Case iCurrentValue
            Case 0 : unit = eUnitCurrencyType.CustomEnergy
            Case 1 : unit = eUnitCurrencyType.WetWeight
            Case 2 : unit = eUnitCurrencyType.Joules
            Case 3 : unit = eUnitCurrencyType.Calorie
            Case 4 : unit = eUnitCurrencyType.Carbon
            Case 5 : unit = eUnitCurrencyType.DryWeight
            Case 6 : unit = eUnitCurrencyType.Nitrogen
            Case 7 : unit = eUnitCurrencyType.Phosporous
            Case Else : unit = eUnitCurrencyType.WetWeight
        End Select
        Try
            bSucces = db.Execute(String.Format("UPDATE EcopathModel SET UnitCurrency={0} WHERE ModelID=1", CInt(unit)))
        Catch ex As Exception
            bSucces = False
        End Try

        If bSucces Then
            bSucces = bSucces And db.CommitTransaction(True)
        Else
            db.RollbackTransaction()
        End If

        Return bSucces

    End Function

#End Region ' Internals

End Class
