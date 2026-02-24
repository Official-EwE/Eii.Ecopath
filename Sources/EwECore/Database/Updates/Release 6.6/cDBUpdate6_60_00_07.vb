' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database
Imports EwEUtils.Utilities

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.06:</para>
''' <para>
''' An error was identified in the database loading logic. This update cannot 
''' apply any fixes as the bug obscures the users intentions. The update thus 
''' merely checks wich scenarios may have been affected and warns the user.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_07
    Inherits cDBUpdate

    Private m_strAction As String = ""

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600007!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Validated Ecospace MPAs"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        ' Abort if no possible issues
        If (CInt(db.Execute("SELECT COUNT(*) FROM EcospaceScenarioMPAFishery")) = 0) Then Return True

        ' Find all scenario IDs where the MPA fishery refers to fleet IDs that differ between Ecopath and Ecospace
        Dim rd As IDataReader = db.GetReader("SELECT DISTINCT(ScenarioID) FROM EcospaceScenarioMPAFishery AS M WHERE EXISTS (SELECT FleetID FROM EcospaceScenarioFleet WHERE ScenarioID = M.ScenarioID AND M.FleetID = FleetID AND M.FleetID <> EcopathFleetID)")
        Dim lID As New List(Of Integer)
        While rd.Read
            lID.Add(CInt(rd("ScenarioID")))
        End While
        db.ReleaseReader(rd)

        ' Abort if no possible issues
        If (lID.Count = 0) Then Return True

        Dim strScenarios As String = ""
        lID.Sort()
        For i As Integer = 0 To lID.Count - 1
            Dim strScenario As String = "'" & CStr(db.GetValue("SELECT ScenarioName FROM EcospaceScenario WHERE ScenarioID=" & lID(i))) & "'"
            If Not String.IsNullOrWhiteSpace(strScenario) Then
                strScenarios = strScenario & ", "
            Else
                strScenarios = strScenario
            End If
        Next

        Me.m_strAction = cStringUtils.Localize(My.Resources.CoreMessages.UPDATE_600007_ERROR, strScenarios)
        Return True

    End Function

    Public Overrides ReadOnly Property UserAction As String
        Get
            Return Me.m_strAction
        End Get
    End Property

End Class
