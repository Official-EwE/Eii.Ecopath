' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that is automatically invoked when
    ''' an Ecospace run is invalidated. This happens when an user input causes
    ''' the current Ecospace results to become invalid, or when an Ecospace scenario
    ''' is closed.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceRunInvalidatedPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Execute an Ecospace Run Invalidated plug-in.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub EcospaceRunInvalidated()

    End Interface

End Namespace