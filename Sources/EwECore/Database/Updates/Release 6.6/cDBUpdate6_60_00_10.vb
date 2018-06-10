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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Utilities

#End Region ' Imports 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.10:</para>
''' <para>
''' Added mediation driven by forcing functions.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_10
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.60001!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added mediation driven by forcing functions"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        bSucces = bSucces And db.Execute("CREATE TABLE EcosimScenarioShapeMedWeightsForcing (ScenarioID LONG, ShapeID LONG, ForcingShapeID LONG, MedWeights SINGLE)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioShapeMedWeightsForcing ADD CONSTRAINT PK_INDEX PRIMARY KEY (ScenarioID, ShapeID, ForcingShapeID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioShapeMedWeightsForcing ADD FOREIGN KEY (ScenarioID) REFERENCES EcosimScenario(ScenarioID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioShapeMedWeightsForcing ADD FOREIGN KEY (ShapeID) REFERENCES EcosimShapeMediation(ShapeID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioShapeMedWeightsForcing ADD FOREIGN KEY (ForcingShapeID) REFERENCES EcosimShapeTime(ShapeID)")

        Return bSucces

    End Function

End Class
