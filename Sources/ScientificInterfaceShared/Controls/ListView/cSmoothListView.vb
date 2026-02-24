' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' <summary>
    ''' <see cref="ListView"/>-derived class for rendering without flickering.
    ''' </summary>
    Public Class cSmoothListView
        Inherits ListView

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
            ' Do NOT set the styles WMPaint, UserPaint unless the derived listview is entirely ownerdrawn
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

    End Class

End Namespace ' Controls
