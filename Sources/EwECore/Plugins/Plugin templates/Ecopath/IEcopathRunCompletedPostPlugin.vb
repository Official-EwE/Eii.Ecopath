' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecopath

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that is automatically invoked when
    ''' Ecopath has ran succesfully - after all IEcopathRunCompletedPlugin instances
    ''' have been called.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcopathRunCompletedPostPlugin
        Inherits IPlugin

        Sub EcopathRunCompletedPost(ByRef EcopathDataStructures As Object)

    End Interface

End Namespace