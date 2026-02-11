' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Plugins.Ecosim

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing plugin points that provide different types of 
    ''' shape functions.
    ''' <seealso cref="IShapeFunction"/>
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcosimShapeFunctionPlugin
        Inherits IPlugin
        Inherits IShapeFunction

    End Interface

End Namespace