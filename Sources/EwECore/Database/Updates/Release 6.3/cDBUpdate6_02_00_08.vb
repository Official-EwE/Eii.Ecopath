' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

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
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.120008!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added Ecosim effort conversion factor" & Environment.NewLine & "Added taxon growth parameters"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.UpdateTaxa(db) And Me.UpdateEcosimGroups(db)

    End Function

    Public Function UpdateTaxa(ByRef db As cEwEDatabase) As Boolean

        Return db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN Winf SINGLE") And
               db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN vbgfK SINGLE")

    End Function

    Public Function UpdateEcosimGroups(ByRef db As cEwEDatabase) As Boolean

        Return db.Execute("ALTER TABLE EcoSimScenarioFleet ADD COLUMN EffortConversionFactor SINGLE")

    End Function

End Class
