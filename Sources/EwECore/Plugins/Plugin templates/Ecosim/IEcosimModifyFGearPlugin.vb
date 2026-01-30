' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecosim

    ''' <summary>
    ''' Plugin Point to modify Ecosim Fishing Effort during a MSE or Fishing Policy Search.
    ''' </summary>
    ''' <remarks>This will not modify effort during a normal Ecosim run.</remarks>
    Public Interface IEcosimModifyFGearPlugin
        Inherits IPlugin

        ''' <summary>
        ''' Method that gets called when a Fishing Policy or MSE search is modifying Fishing Effort.
        ''' </summary>
        ''' <param name="FGear">Array of Relative Fishing Effort dimensioned by fleet for the current timestep.</param>
        ''' <param name="BB">Array of Biomass by group for the current timestep</param>
        ''' <param name="EcosimDataStructures">Reference to the current EcosimDataStructures passed as an object.</param>
        ''' <param name="CurrentTimeStepIndex">Current timestep index.</param>
        ''' <remarks>At this time this only changes effort during a search there is no easy way to change effort during a normal run. </remarks>
        Sub EcosimModifyFGear(FGear() As Single, BB() As Single, EcosimDataStructures As Object, CurrentTimeStepIndex As Integer)

    End Interface

End Namespace