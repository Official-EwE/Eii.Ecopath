' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'








Public Interface IEnvironmentalResponseManager

    ReadOnly Property nEnviroData As Integer

    ReadOnly Property EnviroData(iIndex As Integer) As IEnviroInputData

    ReadOnly Property EnviroData(layer As cEcospaceLayer) As IEnviroInputData

    ReadOnly Property MediationData() As cMediationDataStructures

    ReadOnly Property SpaceData() As cEcospaceDataStructures

    ReadOnly Property SimData() As cEcosimDatastructures

    Function onChanged() As Boolean

    Sub UpdateLayer(ByVal layer As cEcospaceLayer)

End Interface
