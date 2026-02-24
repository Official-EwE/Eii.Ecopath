' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.MSE

    ''' <summary>
    ''' Interface for MSE initialization plugin points that are invoked once the MSE model has been loaded
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IMSEInitializedPlugin
        Inherits IPlugin

        ''' <summary>
        ''' MSE model has been initialized
        ''' </summary>
        ''' <param name="MSEModel">MSE model</param>
        ''' <param name="MSEDataStructure">MSE data structures</param>
        ''' <param name="EcosimDatastructures">Ecosim data structures</param>
        ''' <remarks></remarks>
        Sub MSEInitialized(MSEModel As Object, MSEDataStructure As Object, EcosimDatastructures As Object)

    End Interface

End Namespace