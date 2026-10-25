' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel

Namespace ValueChainMigrator.LegacyData

    ''' ===========================================================================
    ''' <summary>
    ''' Legacy landings class. Maintained for Access -> SQLite database conversion. 
    ''' Not used in the current implementation 
    ''' of the value chain.
    ''' </summary>
    ''' ===========================================================================
    <Obsolete("cLinkLandings is only maintained for Access -> SQLite database conversion; DO NOT USE otherwise"), Serializable()>
    Public Class cLinkLandings
        Inherits cLink

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property EcopathGroupID() As Integer
            Get
                If (String.IsNullOrWhiteSpace(Me.SpeciesCode)) Then Return 0
                Return CInt(Me.SpeciesCode)
            End Get
            Set(id As Integer)
                Me.SpeciesCode = CStr(id)
            End Set
        End Property

        <Browsable(False)>
        Public Property SpeciesCode As String

    End Class

End Namespace
