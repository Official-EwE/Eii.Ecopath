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




Imports EwECore.Database

 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.02:</para>
''' <para>
''' Added other mortality saving.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_02
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700002!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added other mortality saving"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delete orphaned fishing effort shapes
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True

        Dim key As String = db.GetPkKeyName("EcosimScenarioCapacityDrivers")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers DROP CONSTRAINT " & db.GetPkKeyName("EcosimScenarioCapacityDrivers"))
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers DROP CONSTRAINT " & db.GetPkKeyName("EcospaceScenarioCapacityDrivers"))
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD COLUMN Target INTEGER") And
                                db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers ADD COLUMN Target INTEGER")

        ' Primary keys cannot have null values
        bSuccess = bSuccess And db.Execute("UPDATE EcosimScenarioCapacityDrivers SET Target=" & CInt(eDataTypes.EcosimEnviroResponseFunctionManager))
        bSuccess = bSuccess And db.Execute("UPDATE EcospaceScenarioCapacityDrivers SET Target=" & CInt(eDataTypes.EcospaceEnviroCapacityResponse))

        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD PRIMARY KEY (ScenarioID, GroupID, DriverID, ResponseID, Target)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers ADD PRIMARY KEY (ScenarioID, VarDBID, GroupID, ShapeID, Target)")
        Return bSuccess

    End Function

End Class
