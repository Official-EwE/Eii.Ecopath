' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Plugins.Database

    ''' =======================================================================
    ''' <summary>
    ''' Plug-in point for implementing <see cref="IModelImporter">model import logic</see>.
    ''' </summary>
    ''' =======================================================================
    Public Interface IModelImportPlugin
        Inherits IPlugin
        Inherits IModelImporter

    End Interface

End Namespace ' Data
