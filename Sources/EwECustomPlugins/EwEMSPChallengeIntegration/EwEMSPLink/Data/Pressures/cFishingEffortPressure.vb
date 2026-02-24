' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cFishingEffortPressure
    Inherits cPressure

    Public Sub New(name As String)
        MyBase.New(name)
    End Sub

    Public Sub New(name As String, effortscalar As Single)
        Me.New(name)
        Me.EffortScalar = effortscalar
    End Sub

    Public Property EffortScalar As Single = 0

End Class
