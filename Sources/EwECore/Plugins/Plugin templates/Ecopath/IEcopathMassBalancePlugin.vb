' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecopath

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that performs a custom Ecopath 
    ''' Mass Balance calculation. If provided, this plug-in point will replace
    ''' the native Mass Balance calculation provided with EwE6.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcopathMassBalancePlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Execute a Mass balance calculation.
        ''' </summary>
        ''' <param name="EcoPathDataStructures">A reference to the Ecopath data 
        ''' structures as defined in the EwE project.</param>
        ''' <param name="eEstimateFor">Enumerated value, stating the purpose of the mass 
        ''' balance calculation.</param>
        ''' <param name="iResult">The result of the mass balance calculation. For 
        ''' possible values refer to the eStatusFlags enumerated type in the EwE project.
        ''' </param>
        ''' <returns>True if a mass-balance calculation has been performed successfully.</returns>
        ''' <remarks>
        ''' This plug-in point is exclusive, meaning that only one IEcopathMassBalancePlugin 
        ''' plug-in is allowed to successfully perform this calculation. The first plug-in
        ''' of this type that successfully executes blocks the execution of any other
        ''' plug-in of this type.</remarks>
        ''' -----------------------------------------------------------------------
        Function EcopathMassBalance(EcoPathDataStructures As Object, eEstimateFor As Integer, ByRef iResult As Integer) As Boolean

    End Interface

End Namespace