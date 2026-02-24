' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class cShapePreview
    Inherits cShapeData

    Public Sub New()
        MyBase.New(25000)
    End Sub

    ''' <summary>
    ''' Overridden to prevent interactions with the core.
    ''' </summary>
    Public Overrides Function Update() As Boolean
        Return True
    End Function

End Class
