' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database



''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.07:</para>
''' <para>
''' Added disabled external data connections.
''' </para>
''' </summary>
''' <remarks>Note that this logic follows the use fo varnames to identify the 
''' target layer, just like table EcospaceScenarioDataConnection</remarks>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_07
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700007!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added disabled ext connections, per layer"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.ApplyUpdate"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return db.Execute("CREATE TABLE EcospaceScenarioDataConnectionDisabled (ScenarioID LONG, LayerID LONG, Varname TEXT(50))") And
               db.Execute("ALTER TABLE EcospaceScenarioDataConnectionDisabled ADD PRIMARY KEY (ScenarioID, LayerID, Varname)") And
               db.Execute("ALTER TABLE EcospaceScenarioDataConnectionDisabled ADD FOREIGN KEY (ScenarioID) REFERENCES EcospaceScenario(ScenarioID)")
    End Function

End Class
