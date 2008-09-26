'==============================================================================
'
' $Log: cDBUpdate6_00_04_004.vb,v $
' Revision 1.1  2008/09/26 07:30:16  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2008/07/30 12:54:09  jeroens
' Fixed update statement
'
' Revision 1.5  2008/07/18 20:30:47  jeroens
' Fixed error in applying units
'
' Revision 1.4  2008/07/17 17:15:11  jeroens
' Reverted last change
'
' Revision 1.3  2008/07/17 17:02:44  jeroens
' Fixes insufficiently updated tracer databases
'
' Revision 1.2  2008/07/10 19:12:30  jeroens
' Updated to unit fixes
'
' Revision 1.1  2008/07/03 20:13:24  jeroens
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
''' <para>Database update 6.0.4.004:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixes relative primary production issue in existing Ecospace scenarios.</description></item>
''' <item><description>Fixes units.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_04_0004
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

        Return Me.FixRelPP(db) And Me.FixUnits(db)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property UpdateDescription() As String Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateDescription
        Get
            Return "Fixes relative primary production issue in existing Ecospace scenarios." & vbNewLine _
                & "Fixes system units." 
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
            Return 6.04004!
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

    Private Function FixRelPP(ByVal db As EwEUtils.Database.cEwEDatabase) As Boolean

        ' Query to count # of non-zero cells in the basemap
        Dim strQueryCheck As String = "SELECT COUNT(*) FROM EcospaceScenarioBasemap WHERE RelPP<>0 AND ScenarioID={0}"
        Dim strQuerySet As String = "UPDATE EcospaceScenarioBasemap SET RelPP=1 WHERE ScenarioID={0} AND Depth>0"
        Dim iScenarioID As Integer = 0
        Dim iNumCells As Integer = 0
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        Try
            reader = db.GetReader("SELECT ScenarioID FROM EcospaceScenario")
            While reader.Read
                iScenarioID = CInt(reader("ScenarioID"))
                iNumCells = CInt(db.GetValue(String.Format(strQueryCheck, iScenarioID)))
                If (iNumCells = 0) Then
                    db.Execute(String.Format(strQuerySet, iScenarioID))
                End If
            End While
            db.ReleaseReader(reader)
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

    Private Structure cUnitMapping
        Public ModelID As Integer
        Public CurrencyUnit As eUnitCurrencyType
        Public CurrencyCustom As String
        Public TimeUnit As eUnitTimeType
        Public TimeCustom As String

        Public Sub New(ByVal id As Integer, ByVal cut As eUnitCurrencyType, ByVal strCurr As String, ByVal utt As eUnitTimeType, ByVal strTime As String)
            Me.ModelID = id
            Me.CurrencyUnit = cut
            Me.CurrencyCustom = strCurr
            Me.TimeUnit = utt
            Me.TimeCustom = strTime
        End Sub

    End Structure

    Private Function FixUnits(ByVal db As EwEUtils.Database.cEwEDatabase) As Boolean

        Dim strSQL As String = ""
        Dim lMappings As New List(Of cUnitMapping)
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        ' Fix previous mistake, if any
        db.Execute("ALTER TABLE EcopathModel DROP COLUMN UnitCurrency")
        db.Execute("ALTER TABLE EcopathModel DROP COLUMN UnitTime")

        ' Add proper columns
        db.Execute("ALTER TABLE EcopathModel ADD COLUMN UnitCurrency LONG")
        db.Execute("ALTER TABLE EcopathModel ADD COLUMN UnitCurrencyCustom TEXT(30)")
        db.Execute("ALTER TABLE EcopathModel ADD COLUMN UnitTime LONG")
        db.Execute("ALTER TABLE EcopathModel ADD COLUMN UnitTimeCustom TEXT(30)")

        ' Transfer unit values
        Try

            reader = db.GetReader("SELECT * FROM EcopathModel")
            While reader.Read

                Dim uct As eUnitCurrencyType = eUnitCurrencyType.WetWeight
                Dim strCurrencyCustom As String = ""
                Dim utt As eUnitTimeType = eUnitTimeType.Year
                Dim strTimeCustom As String = ""

                Try
                    uct = DirectCast(reader("CurrencyIndex"), eUnitCurrencyType)
                Catch ex As Exception
                    uct = eUnitCurrencyType.WetWeight
                End Try
                Try
                    strCurrencyCustom = CStr(reader("CurrencyUnit"))
                Catch ex As Exception
                    strCurrencyCustom = ""
                End Try

                Select Case strCurrencyCustom.Trim.ToLower()
                    Case "", "t/km²" : uct = eUnitCurrencyType.WetWeight : strCurrencyCustom = ""
                    Case "kcal/m²" : uct = eUnitCurrencyType.Calorie : strCurrencyCustom = ""
                    Case "g/m²" : uct = DirectCast(uct + 1, eUnitCurrencyType) : strCurrencyCustom = ""
                    Case "j/m²" : uct = eUnitCurrencyType.Joules : strCurrencyCustom = ""
                    Case "mg n/m²" : uct = eUnitCurrencyType.Nitrogen : strCurrencyCustom = ""
                    Case "mg p/m²" : uct = eUnitCurrencyType.Phosporous : strCurrencyCustom = ""
                End Select

                Try
                    strTimeCustom = CStr(reader("TimeUnit"))
                Catch ex As Exception
                    strTimeCustom = "year"
                End Try
                Select Case strTimeCustom.ToLower()
                    Case "", "year" : utt = eUnitTimeType.Year : strTimeCustom = ""
                    Case "day" : utt = eUnitTimeType.Day : strTimeCustom = ""
                    Case Else : utt = eUnitTimeType.Custom
                End Select

                lMappings.Add(New cUnitMapping(CInt(reader("ModelID")), uct, strCurrencyCustom, utt, strTimeCustom))

            End While

        Catch ex As Exception

        End Try
        db.ReleaseReader(reader)
        reader = Nothing

        ' Now apply
        For Each m As cUnitMapping In lMappings
            strSQL = String.Format("UPDATE EcopathModel SET UnitCurrency={0}, UnitCurrencyCustom='{1}', UnitTime={2}, UnitTimeCustom='{3}' WHERE ModelID={4}", _
                        CInt(m.CurrencyUnit), m.CurrencyCustom, m.TimeUnit, m.TimeCustom, m.ModelID)
            db.Execute(strSQL)
        Next

        db.Execute("ALTER TABLE EcopathModel DROP COLUMN TimeUnit")
        db.Execute("ALTER TABLE EcopathModel DROP COLUMN CurrencyUnit")
        db.Execute("ALTER TABLE EcopathModel DROP COLUMN CurrencyIndex")

        Return bSucces

    End Function

#End Region ' Internals

End Class
