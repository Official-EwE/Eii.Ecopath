' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.MSE

    ''' <summary>
    ''' Interface for implementing MSY search plugin points that are invoked from the EwE core.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IMSYPlugin
        Inherits IPlugin

        ''' <summary>
        ''' MSY has been initialized
        ''' </summary>
        ''' <param name="MSEDataStructure">MSE data structures</param>
        ''' <param name="EcosimDatastructures">Ecosim data structures</param>
        ''' <remarks></remarks>
        Sub MSYInitialized(MSEDataStructure As Object, EcosimDatastructures As Object)

        ''' <summary>
        ''' The MSY variables have been initialized and search is about to start.
        ''' </summary>
        ''' <param name="MSEDataStructure"></param>
        ''' <param name="EcosimDatastructures"></param>
        ''' <remarks></remarks>
        Sub MSYRunStarted(MSEDataStructure As Object, EcosimDatastructures As Object)

        ''' <summary>
        ''' MSY search has completed all its iteration and computed effort for all fleets. Interface objects have not been populated at this time.
        ''' </summary>
        ''' <param name="MSYEffortByFleet">MSY effort for all fleets</param>
        ''' <param name="MSYFbyGroup">MSY Fishing mortality for groups</param>
        ''' <remarks></remarks>
        Sub MSYEffortCompleted(MSYEffortByFleet() As Single, MSYFbyGroup() As Single)

        ''' <summary>
        ''' MSY search is completed all iterface object have been populated.
        ''' </summary>
        ''' <remarks></remarks>
        Sub MSYRunCompleted()

    End Interface

End Namespace