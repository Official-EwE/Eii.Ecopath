' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecopath

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that is automatically invoked when
    ''' Ecopath has ran succesfully.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcopathRunCompletedPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Execute an Ecopath Run Completed plug-in.
        ''' </summary>
        ''' <param name="EcoPathDataStructures">A reference to the Ecopath data 
        ''' structures as defined in the EwE project.</param>
        ''' <remarks>This plug-in point is non-exclusive, meaning that multiple
        ''' plug-ins can respond to this event.</remarks>
        ''' -----------------------------------------------------------------------
        Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object)

    End Interface

End Namespace