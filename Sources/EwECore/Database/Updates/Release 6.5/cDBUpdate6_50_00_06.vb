' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.06:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Enabled monthly time series</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_06
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500006!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Cleaned up Ecospace driver tables"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return Me.UpdateEcospaceDriverTable(db)
    End Function

    Public Function UpdateEcospaceDriverTable(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        ' Remove obsolete field 'varname'
        bSucces = db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers DROP CONSTRAINT " & db.GetPkKeyName("EcospaceScenarioCapacityDrivers"))
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers DROP COLUMN VarName")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers ADD PRIMARY KEY (ScenarioID, VarDBID, GroupID, ShapeID)")

        Return bSucces

    End Function

End Class
