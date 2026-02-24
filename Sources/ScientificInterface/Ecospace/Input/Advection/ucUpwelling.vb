' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecospace.Advection

    ''' <summary>
    ''' Upwelling velocities control for advection form.
    ''' </summary>
    Public Class ucUpwelling

        Protected Overrides Function DataLayerVariable() As eVarNameFlags
            Return eVarNameFlags.LayerUpwelling
        End Function

        ''' <inheritdoc cref="IsDataInput"/>
        Protected Overrides Function IsDataInput() As Boolean
            Return False
        End Function

    End Class

End Namespace
