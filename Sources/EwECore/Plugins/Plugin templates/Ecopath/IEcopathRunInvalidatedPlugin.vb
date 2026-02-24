' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecopath

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that is automatically invoked when
    ''' an Ecospace run is invalidated. This happens when an user input causes
    ''' the current Ecopath results to become invalid, or when a model is closed.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcopathRunInvalidatedPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Execute an Ecopath Run Invalidated plug-in.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub EcopathRunInvalidated()

    End Interface

End Namespace