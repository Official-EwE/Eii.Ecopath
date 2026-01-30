' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database



''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.1.01:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Updated Ecotracer</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_10_01
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.501001!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Updated Ecotracer"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        If db.Execute("ALTER TABLE EcotracerScenarioGroup ADD COLUMN CassimProp SINGLE") And
           db.Execute("ALTER TABLE EcotracerScenarioGroup ADD COLUMN CmetabolismRate SINGLE") Then

            db.Execute("UPDATE EcotracerScenarioGroup SET CassimProp = Cexcretionrate")
            db.Execute("UPDATE EcotracerScenarioGroup SET CmetabolismRate = 1")
            db.DropColumn("EcotracerScenarioGroup", "Cexcretionrate")

            Return True
        End If
        Return False

    End Function


End Class
