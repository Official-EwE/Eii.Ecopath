' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Database

#End Region ' Imports 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.06:</para>
''' <para>
''' Made capacity / mortality driver assignments robust.
''' </para>
''' </summary>
''' <remarks>
''' This update ensures that users can simultaneously apply the same 
''' var/group/shape combination to drive capacity and mortalities. It is highly 
''' unlikely that this combination would be used for both scenarios, but let's 
''' not eliminate the use case. This update fixes both Ecosim and Ecospace.
''' </remarks>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_06
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700006!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Made capacity & mortality driver handling robust in Ecosim and Ecospace"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return Me.UpdateEcosimCapacityDrivers(db) And
               Me.UpdateEcospaceCapacityDrivers(db) And Me.UpdateEcospaceDriverDisabling(db)
    End Function

    Private Function UpdateEcosimCapacityDrivers(db As cEwEDatabase) As Boolean

        Dim strPK As String = ""
        Dim strSQL As String = ""
        Dim bSucces As Boolean = True

        strPK = db.GetPkKeyName("EcosimScenarioCapacityDrivers")
        If Not String.IsNullOrEmpty(strPK) Then
            strSQL = String.Format("DROP INDEX {0} ON EcosimScenarioCapacityDrivers", strPK)
            bSucces = db.Execute(strSQL)
        End If

        strSQL = "ALTER TABLE EcosimScenarioCapacityDrivers ADD PRIMARY KEY (ScenarioID, GroupID, DriverID, ResponseID, Target)"
        db.Execute(strSQL)

        Return bSucces
    End Function

    Private Function UpdateEcospaceCapacityDrivers(db As cEwEDatabase) As Boolean

        Dim strPK As String = ""
        Dim strSQL As String = ""
        Dim bOk As Boolean = True

        strPK = db.GetPkKeyName("EcospaceScenarioCapacityDrivers")
        If Not String.IsNullOrEmpty(strPK) Then
            strSQL = String.Format("DROP INDEX {0} ON EcospaceScenarioCapacityDrivers", strPK)
            bOk = db.Execute(strSQL)
        End If

        strSQL = "ALTER TABLE EcospaceScenarioCapacityDrivers ADD PRIMARY KEY (ScenarioID, VarDBID, GroupID, ShapeID, Target)"
        db.Execute(strSQL)

        Return bOk
    End Function

    Private Function UpdateEcospaceDriverDisabling(db As cEwEDatabase) As Boolean

        Dim strPK As String = ""
        Dim strSQL As String = ""
        Dim bYeahThatTotallyWorked As Boolean = True

        db.Execute("ALTER TABLE EcospaceScenarioDriverDisabled ADD COLUMN Target INTEGER")
        db.Execute("UPDATE EcospaceScenarioDriverDisabled SET Target=" & CInt(eDataTypes.EcospaceEnviroCapacityResponse))

        strPK = db.GetPkKeyName("EcospaceScenarioDriverDisabled")
        If Not String.IsNullOrEmpty(strPK) Then
            strSQL = String.Format("DROP INDEX {0} ON EcospaceScenarioDriverDisabled", strPK)
            bYeahThatTotallyWorked = db.Execute(strSQL)
        End If

        strSQL = "ALTER TABLE EcospaceScenarioDriverDisabled ADD PRIMARY KEY (ScenarioID, LayerID, Target)"
        db.Execute(strSQL)

        Return bYeahThatTotallyWorked
    End Function

End Class
