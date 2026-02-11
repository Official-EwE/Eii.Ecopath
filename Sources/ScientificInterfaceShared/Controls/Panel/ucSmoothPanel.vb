' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    Public Class ucSmoothPanel
        Inherits Panel

        Public Sub New()
            MyBase.New()
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                        ControlStyles.OptimizedDoubleBuffer Or
                        ControlStyles.UserPaint Or
                        ControlStyles.ResizeRedraw, True)
        End Sub

    End Class

End Namespace ' Controls
