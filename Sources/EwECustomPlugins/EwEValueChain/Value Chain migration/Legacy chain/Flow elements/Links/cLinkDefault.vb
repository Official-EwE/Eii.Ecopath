' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel

Namespace ValueChainMigrator.LegacyData

    ''' ===========================================================================
    ''' <summary>
    ''' Legacy class for holding default link properties, used when forging new links 
    ''' between units in the flow. Maintained for Access -> SQLite database conversion. 
    ''' Not used in the current implementation 
    ''' of the value chain.
    ''' </summary>
    ''' ===========================================================================
    <Obsolete(), Serializable()>
    Public Class cLinkDefault
        Inherits cOOPStorable

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overridable Property Name() As String

        <Browsable(False)>
        Public Property LinkType() As Integer

        Public Property BiomassRatio() As Single

        Public Property ValuePerTon() As Single

        Public Property ValueRatio() As Single

    End Class

End Namespace
