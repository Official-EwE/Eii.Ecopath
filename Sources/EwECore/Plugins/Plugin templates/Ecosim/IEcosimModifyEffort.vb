' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Plugins

''' <summary>
''' Plugin Point to modify effort during an Ecosim run 
''' </summary>
''' <remarks></remarks>
Public Interface IEcosimModifyEffort
    Inherits IPlugin

    ''' <summary>
    ''' Call at each Ecosim timestep before fishing mortality is set. If the bEffortModified = True then a new fishing mortality will be computed from Effort().
    ''' </summary>
    ''' <param name="bEffortModified">
    ''' If True then fishing mortality will be computed from effort. 
    ''' If False then Effort() will be ignored and fishing mortality will not be modified. 
    ''' </param>
    ''' <param name="Effort">Fishing effort at the current timestep. Alter this and set bEffortModified = True to change fishing effort. </param>
    ''' <param name="BB">Biomass at the current timestep</param>
    ''' <param name="iTimeIndex">Time index of the current timestep</param>
    ''' <param name="iYearIndex">Year index of the current timestep</param>
    ''' <param name="EcosimDataStructures">cEcosimDataStructures as an Object</param>
    ''' <remarks></remarks>
    Sub EcosimModifyEffort(ByRef bEffortModified As Boolean, Effort() As Single, BB() As Single, iTimeIndex As Integer, iYearIndex As Integer, EcosimDataStructures As Object)

End Interface

