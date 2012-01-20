Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Utilities

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.120008:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecosim effort conversion factor.</description></item>
''' <item><description>Added taxon growth parameters.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_12_00008
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
            Return 6.120008!
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
            Return "Added Ecosim effort conversion factor" & vbNewLine & "Added taxon growth parameters"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.UpdateTaxa(db) And Me.UpdateEcosimGroups(db)

    End Function

    Public Function UpdateTaxa(ByRef db As cEwEDatabase) As Boolean

        Return db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN Winf SINGLE") And _
               db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN vbgfK SINGLE")

    End Function

    Public Function UpdateEcosimGroups(ByRef db As cEwEDatabase) As Boolean

        Return db.Execute("ALTER TABLE EcoSimScenarioFleet ADD COLUMN EffortConversionFactor SINGLE")

    End Function

End Class
