' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.12:</para>
''' <para>
''' Added ecospacce fitness response type field.
''' Added stanza spawn proportion
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_12
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600012!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added ecospace fitness response type, stanza spawn proportion"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        If db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN FitResponseType BYTE") And
           db.Execute("ALTER TABLE StanzaLifeStage ADD COLUMN SpawnProp SINGLE") Then

            ' Set all stage values to True by default
            Dim wr As cEwEDatabase.cEwEDbWriter = db.GetWriter("StanzaLifeStage")
            Dim dt As DataTable = wr.GetDataTable()
            For Each drow As DataRow In dt.Rows
                drow.BeginEdit()
                drow("SpawnProp") = 1.0
                drow.EndEdit()
            Next
            db.ReleaseWriter(wr)
            Return True

        End If
        Return False

    End Function

End Class
