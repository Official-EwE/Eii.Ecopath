' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.40.0.04:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed ref integrity to Ecospace groups (not Ecopath)</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_40_00_04
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.400004!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed capacity map constraints"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True
        Dim strKey As String = db.GetFkKeyName("EcopathGroup", "EcospaceScenarioCapacityDrivers", "GroupID")

        If Not String.IsNullOrWhiteSpace(strKey) Then
            bSuccess = False
            If db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers DROP CONSTRAINT " & strKey) Then
                bSuccess = db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers ADD CONSTRAINT fkGroupID FOREIGN KEY (GroupID) REFERENCES EcospaceScenarioGroup (GroupID)")
            End If
        End If

        Me.LogProgress("UpdateEcospaceScenarioCapacityDrivers", bSuccess)

        Return bSuccess

    End Function

End Class
