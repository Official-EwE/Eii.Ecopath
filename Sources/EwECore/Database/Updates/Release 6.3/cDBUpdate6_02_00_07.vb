' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.120007:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added table to save spatial data configuration.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_12_00007
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.120007!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added storage for external spatial data."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = db.Execute("CREATE TABLE EcospaceScenarioDataAdapters (ScenarioID LONG, VarName TEXT(50), LayerIndex INTEGER, [Dataset] TEXT(140), [Converter] TEXT(255), ConverterCfg MEMO)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioDataAdapters ADD PRIMARY KEY (ScenarioID, VarName, LayerIndex)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioDataAdapters ADD FOREIGN KEY (ScenarioID) REFERENCES EcospaceScenario(ScenarioID)")

        Return bSucces

    End Function

End Class
