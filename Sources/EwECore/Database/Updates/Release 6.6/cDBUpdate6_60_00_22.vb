' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database



''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.22:</para>
''' <para>
''' Fixed potential ecospace connection storage problem
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_22
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600022!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed potential ecospace connection storage problem"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = db.Execute("ALTER TABLE EcospaceScenarioDataConnection DROP CONSTRAINT " & db.GetPkKeyName("EcospaceScenarioDataConnection"))
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioDataConnection ADD PRIMARY KEY (ScenarioID, varName, LayerID, Sequence)")
        Return bSucces

    End Function

End Class
