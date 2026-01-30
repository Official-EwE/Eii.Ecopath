' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.2.0.05:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed Capacity driver PK</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_02_00_05
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.120005!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed PK in Capacity driver map storage; needs to include shape"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Dim bOkidoki As Boolean = db.Execute("ALTER TABLE EcospaceScenarioCapacitDrivers DROP CONSTRAINT " & db.GetPkKeyName("EcospaceScenarioCapacitDrivers"))
        Return bOkidoki And db.Execute("ALTER TABLE EcospaceScenarioCapacitDrivers ADD PRIMARY KEY (ScenarioID, GroupID, VarName, VarDBID, ShapeID)")
    End Function


End Class
