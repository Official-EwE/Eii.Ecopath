' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecospace.Advection

    ''' <summary>
    ''' Wind control for advection form.
    ''' </summary>
    Public Class ucWind

        Protected Overrides Function DataLayerVariable() As eVarNameFlags
            Return eVarNameFlags.LayerWind
        End Function

    End Class

End Namespace
