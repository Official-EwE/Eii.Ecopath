' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cFishingEcoPressure
    Inherits cPressure

    Public Sub New(name As String)
        MyBase.New(name)
    End Sub

    Public Sub New(name As String, bIsEcological As Boolean)
        Me.New(name)
        Me.bIsEcological = bIsEcological
    End Sub

    Public Property bIsEcological As Boolean = False

End Class
