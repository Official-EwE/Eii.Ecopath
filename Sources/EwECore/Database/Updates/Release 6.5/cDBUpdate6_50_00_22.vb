' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.21:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Removed discontinued migration fields</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_22
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500022!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Removed discontinued migration fields"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        db.Execute("ALTER TABLE EcospaceScenarioGroupMigration DROP COLUMN Concentration")
        db.Execute("ALTER TABLE EcospaceScenarioGroup DROP COLUMN PrefRow")
        db.Execute("ALTER TABLE EcospaceScenarioGroup DROP COLUMN PrefCol")
        db.Execute("ALTER TABLE EcospaceScenarioGroup DROP COLUMN MigConcRow")
        db.Execute("ALTER TABLE EcospaceScenarioGroup DROP COLUMN MigConcCol")
        Return True

    End Function

End Class
