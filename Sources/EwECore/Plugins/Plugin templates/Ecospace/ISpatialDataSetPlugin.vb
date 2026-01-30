' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for providing a spatial data set as a plugin.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface ISpatialDataSetPlugin
        Inherits IPlugin
        Inherits ISpatialDataSet

    End Interface

End Namespace