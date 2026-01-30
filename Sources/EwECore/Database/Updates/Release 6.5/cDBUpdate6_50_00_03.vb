' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.02:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Re-issued 6.4.04 to fix development time updates</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_03
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500003!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Enable multiple connections for a single dataset layer"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim strKey As String = db.GetPkKeyName("EcospaceScenarioDataAdapters")
        Dim bSuccess As Boolean = Not String.IsNullOrWhiteSpace(strKey)

        Try
            bSuccess = db.Execute("ALTER TABLE EcospaceScenarioDataAdapters ADD COLUMN ConnectionIndex INTEGER")
            bSuccess = bSuccess And db.Execute("UPDATE EcospaceScenarioDataAdapters SET ConnectionIndex=1")
            If db.Execute("ALTER TABLE EcospaceScenarioDataAdapters DROP CONSTRAINT " & strKey) Then
                bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioDataAdapters ADD CONSTRAINT pk PRIMARY KEY (ScenarioID, VarName, LayerIndex, ConnectionIndex)")
            End If
        Catch ex As Exception
            bSuccess = False
        End Try
        Me.LogProgress(Me.UpdateDescription, True)
        Return True

    End Function
End Class
