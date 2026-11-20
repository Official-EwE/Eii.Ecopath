' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.11:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Add foraging time lower limit flag for Ecosim.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_11
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500011!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Lower foraging time limit for Ecosim can be altered"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        ' All updated models receive the former default of 0.1
        Dim bSuccess As Boolean = db.Execute("ALTER TABLE EcosimScenario ADD COLUMN ForagingTimeLowerLimit SINGLE")
        Dim writer As IEwEDbWriter = db.GetWriter("EcosimScenario")
        Dim dt As DataTable = writer.GetDataTable()
        For Each row As DataRow In dt.Rows
            row.BeginEdit()
            row("ForagingTimeLowerLimit") = 0.1!
            row.EndEdit()
        Next
        db.ReleaseWriter(writer, True)
        Me.LogProgress("Update EcosimScenario foragingtimelowerlimit", bSuccess)
        Return bSuccess

    End Function

End Class
