' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
