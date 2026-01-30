' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Core

    ''' ===========================================================================
    ''' <summary>
    ''' Interface for implementing plug-ins that are licensed.
    ''' </summary>
    ''' ===========================================================================
    Public Interface ILicensePlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point to obtain the expiry date.
        ''' </summary>
        ''' <param name="dt"></param>
        ''' -----------------------------------------------------------------------
        Sub Expiry(ByRef dt As DateTime)

    End Interface

End Namespace