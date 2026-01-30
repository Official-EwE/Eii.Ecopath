' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On
Imports ScientificInterfaceShared.Style



Namespace Controls.Map

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cMapDrawerBase"/> for rendering data by fleet.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cMapDrawerFleet
        Inherits cMapDrawerBase

        Public Sub New(core As cCore, sg As cStyleGuide)
            MyBase.New(core, sg)
        End Sub

        Public Overrides Sub DrawMap(iItem As Integer, rcPos As System.Drawing.Rectangle, Args As cMapDrawerArgs)
            Throw New NotImplementedException("ToDo")
        End Sub

    End Class

End Namespace