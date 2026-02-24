' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Override for the .NET Color dialog, overridden to manage custom colours.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEwEColorDialog
        Inherits ColorDialog

        Private Shared s_lCustomColors As New List(Of Integer)

        Public Sub New()
            MyBase.New()
            Me.AllowFullOpen = True
            Me.FullOpen = True
            Me.CustomColors = s_lCustomColors.ToArray
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            s_lCustomColors.Clear()
            s_lCustomColors.AddRange(Me.CustomColors)
            MyBase.Dispose(disposing)
        End Sub

    End Class

End Namespace ' Controls
