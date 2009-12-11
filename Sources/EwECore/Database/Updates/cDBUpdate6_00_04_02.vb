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
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
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
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.0402!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The actual update logic.
    ''' </summary>
    ''' <param name="db">Database to modify.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean


        Return Me.FixCurrencyUnits(db)

    End Function

    Private Function FixCurrencyUnits(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True
        Dim iCurrentValue As Integer = -1
        Dim unit As eUnitCurrencyType = eUnitCurrencyType.NotSet

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

        Return bSucces

    End Function

End Class
