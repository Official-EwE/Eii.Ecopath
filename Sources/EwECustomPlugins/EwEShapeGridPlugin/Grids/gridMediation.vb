' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore



''' ---------------------------------------------------------------------------
''' <summary>
''' Base class grid for showing <see cref="cMediationBaseFunction">mediation</see>-derived
''' functions.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class gridMediation
    Inherits gridForcingBase

    Protected Overrides Function Label(iPoint As Integer) As String
        Return CStr(iPoint + 1)
    End Function

    Public Overrides ReadOnly Property IsMonthly As Boolean
        Get
            Return False
        End Get
    End Property

End Class
