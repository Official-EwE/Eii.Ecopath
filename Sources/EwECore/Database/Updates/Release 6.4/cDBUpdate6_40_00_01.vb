' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.40.0.01:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecospace effort multiplier and distribution flags.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_40_00_01
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.400001!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added Ecospace effort multiplier and distribution flags"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return Me.UpdateEcospaceTables(db)
    End Function

    Private Function UpdateEcospaceTables(db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True

        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioFleet ADD COLUMN SEMult SINGLE")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN UseEffortDistrThreshold SHORT")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN EffortDistrThreshold SINGLE")
        Me.LogProgress("UpdateEcospaceTables", bSuccess)
        Return bSuccess

    End Function

End Class
