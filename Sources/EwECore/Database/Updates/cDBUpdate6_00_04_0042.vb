'==============================================================================
'
' $Log: cDBUpdate6_00_04_0042.vb,v $
' Revision 1.1  2008/09/26 07:30:16  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/07/23 18:14:24  jeroens
' Fixed custom monetary unit field
'
' Revision 1.7  2008/07/23 16:56:40  jeroens
' Release
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
''' <item><description>Fixes Ecotracer.</description></item>
''' <item><description>Add Pedigree table.</description></item>
''' <item><description>Add monetary unit to models.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_04_00042
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

        Return Me.FixEcotracer(db) And Me.AddPedigree(db) And Me.AddMonetaryUnit(db)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property UpdateDescription() As String Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateDescription
        Get
            Return "Adds Description to Ecotracer Scenarios" & vbNewLine & _
                "Adds Pedigree" & vbNewLine & _
                "Adds monetary unit"
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
            Return 6.040042!
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

    Private Function FixEcotracer(ByVal db As cEwEDatabase) As Boolean
        Try
            db.Execute("ALTER TABLE EcotracerScenario ADD COLUMN Description MEMO")
        Catch
            ' All good
        End Try
        Return True
    End Function

    Private Function AddPedigree(ByVal db As cEwEDatabase) As Boolean

        ' Supported pedigree variables
        Dim vnPedigree As eVarNameFlags() = {eVarNameFlags.Biomass, eVarNameFlags.PBInput, eVarNameFlags.QBInput, eVarNameFlags.DietComp, eVarNameFlags.Landings}
        Dim strValueID As String = ""
        Dim readerPedigree As IDataReader = Nothing
        Dim iPedigree As Integer = 0
        Dim writerRemark As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Just in case
        db.Execute("DROP TABLE Pedigree")

        bSucces = db.Execute("CREATE TABLE PedigreeLevel (VarName TEXT(50), Sequence INTEGER, IndexValue SINGLE, Confidence SINGLE, Description MEMO)")
        bSucces = bSucces And db.Execute("ALTER TABLE PedigreeLevel ADD PRIMARY KEY (VarName, Sequence)")
        bSucces = db.Execute("ALTER TABLE Remark ADD COLUMN Pedigree INTEGER)")

        ' Copy existing pedigree levels from group info, if any
        Try
            readerPedigree = db.GetReader("SELECT GroupID, Pedigree1, Pedigree2, Pedigree3, Pedigree4 FROM EcopathGroup")

            writerRemark = db.GetWriter("Remark")
            dt = writerRemark.GetDataTable()

            While readerPedigree.Read

                For i As Integer = 1 To 4
                    Try
                        ' Get pedigree level
                        iPedigree = CInt(readerPedigree("Pedigree" & i))
                        ' Correct to Ewe6 way of life
                        If (iPedigree < 0) Then iPedigree = cCore.NULL_VALUE
                        ' Concoct ID for remark entry that will hold pedigree data
                        strValueID = cValueID.GenerateAbstract(eDataTypes.EcoPathGroupInput, CInt(readerPedigree("GroupID")), vnPedigree(i - 1))
                        ' Find remark row
                        drow = dt.Rows.Find(strValueID)

                        ' Row already esists?
                        If (drow IsNot Nothing) Then
                            ' #Yes: add pedigree to existing row
                            drow.BeginEdit()
                            drow("Pedigree") = iPedigree
                            drow.EndEdit()
                        Else
                            ' #No: create new row
                            drow = writerRemark.NewRow
                            drow("Pedigree") = iPedigree
                            writerRemark.AddRow(drow)
                        End If

                    Catch ex As Exception

                    End Try
                Next i

            End While
            db.ReleaseReader(readerPedigree)
            readerPedigree = Nothing

        Catch ex As Exception

        End Try

        ' Destroy obsolete pedigree columns
        db.Execute("ALTER TABLE EcopathGroup DROP COLUMN Pedigree1")
        db.Execute("ALTER TABLE EcopathGroup DROP COLUMN Pedigree2")
        db.Execute("ALTER TABLE EcopathGroup DROP COLUMN Pedigree3")
        db.Execute("ALTER TABLE EcopathGroup DROP COLUMN Pedigree4")
        db.Execute("ALTER TABLE EcopathGroup DROP COLUMN Pedigree5")

        '' Populate PedigreeLevels table with defaults
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 0, 0, 80, 'Estimated by Ecopath')", vnPedigree(0).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 1, 0, 80, 'From other model')", vnPedigree(0).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 2, 0, 80, 'Guesstimates')", vnPedigree(0).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 3, 0.4, 50, 'Approximate or indirect method')", vnPedigree(0).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 4, 0.7, 30, 'Sampling based, low precision')", vnPedigree(0).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 5, 1, 10, 'Sampling based, high precision')", vnPedigree(0).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 6, 0, 0, '')", vnPedigree(0).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 7, 0, 0, '')", vnPedigree(0).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 0, 0, 80, 'Estimated by Ecopath')", vnPedigree(1).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 1, 0.1, 70, 'Guesstimates')", vnPedigree(1).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 2, 0.2, 60, 'From other model')", vnPedigree(1).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 3, 0.5, 50, 'Empirical relationships')", vnPedigree(1).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 4, 0.6, 40, 'Similar group/species, similar system')", vnPedigree(1).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 5, 0.7, 30, 'Similar group/species, same system')", vnPedigree(1).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 6, 0.8, 20, 'Same group/species, similar system')", vnPedigree(1).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 7, 1, 10, 'Same group/species, same system')", vnPedigree(1).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 0, 0, 80, 'Estimated by Ecopath')", vnPedigree(2).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 1, 0.1, 70, 'Guesstimates')", vnPedigree(2).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 2, 0.2, 60, 'From other model')", vnPedigree(2).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 3, 0.5, 50, 'Empirical relationships')", vnPedigree(2).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 4, 0.6, 40, 'Similar group/species, similar system')", vnPedigree(2).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 5, 0.7, 30, 'Similar group/species, same system')", vnPedigree(2).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 6, 0.8, 20, 'Same group/species, similar system')", vnPedigree(2).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 7, 1, 10, 'Same group/species, same system')", vnPedigree(2).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 0, 0, 80, 'General knowledge of related group/species')", vnPedigree(3).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 1, 0, 80, 'From other model')", vnPedigree(3).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 2, 0.2, 60, 'General knowledge of same group/species')", vnPedigree(3).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 3, 0.5, 50, 'Qualitative diet composition study')", vnPedigree(3).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 4, 0.7, 30, 'Quantitative but limited diet composition study')", vnPedigree(3).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 5, 1, 10, 'Quantitative, detailed, diet composition study')", vnPedigree(3).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 0, 0.1, 70, 'Guesstimates')", vnPedigree(4).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 1, 0.1, 70, 'From other model')", vnPedigree(4).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 2, 0.2, 80, 'FAO statistics')", vnPedigree(4).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 3, 0.5, 50, 'National statistics')", vnPedigree(4).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 4, 0.7, 30, 'Local study, low precision/incomplete')", vnPedigree(4).ToString()))
        'db.Execute(String.Format("INSERT INTO PedigreeLevel (VariableName, Plevel, Pvalue, Pvar, Description) VALUES('{0}', 5, 1, 10, 'Local study, high precision/complete')", vnPedigree(4).ToString()))

        Return bSucces
    End Function

    Private Function AddMonetaryUnit(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = db.Execute("ALTER TABLE EcopathModel ADD COLUMN UnitMonetary LONG")
        Return bSucces And db.Execute("ALTER TABLE EcopathModel ADD COLUMN UnitMonetaryCustom TEXT(30)")

    End Function

#End Region ' Internals

End Class
