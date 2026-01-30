' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cENARDataStructures

    Public nGroups As Integer

    Public b() As Single
    Public Resp() As Single
    Public Consumpt(,) As Single

    ''' <summary>
    ''' Catch for all fished groups. Exported detritus for detrius groups
    ''' </summary>
    Public CatchExport() As Single

    ''' <summary>
    ''' Production for Primary Producers. Imported diet/consumption of consumer groups
    ''' </summary>
    Public Import() As Single


    Public Sub New(NumberOfGroups As Integer)

        Me.nGroups = NumberOfGroups

        Me.b = New Single(Me.nGroups) {}
        Me.Resp = New Single(Me.nGroups) {}
        Me.CatchExport = New Single(Me.nGroups) {}
        Me.Import = New Single(Me.nGroups) {}
        Me.Consumpt = New Single(Me.nGroups, Me.nGroups) {}

    End Sub

End Class

