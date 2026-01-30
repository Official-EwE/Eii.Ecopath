' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cDataSheet
    Private mData As Object
    Private mName As String

    Public Property Data() As Object
        Get
            Return Me.mData
        End Get
        Set(value As Object)
            Me.mData = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return Me.mName
        End Get
        Set(value As String)
            Me.mName = value
        End Set
    End Property

End Class
