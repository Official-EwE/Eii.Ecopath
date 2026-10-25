' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace ValueChainMigrator.LegacyData

    ''' ===========================================================================
    ''' <summary>
    ''' Legacy base class for holding link information in the flow. Maintained for
    ''' Access -> SQLite database conversion. Not used in the current implementation 
    ''' of the value chain.
    ''' </summary>
    ''' <remarks>
    ''' Note that this class does not hold the actual references to flow units.
    ''' This class is a mere holder of shared behaviour between cUnitLinks and
    ''' cLinkDefaults
    ''' </remarks>
    ''' ===========================================================================
    <Obsolete("cLink is only maintained for Access -> SQLite database conversion; DO NOT USE otherwise"), Serializable()>
    Public Class cLink
        Inherits cLinkDefault

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Property Name() As String

        Public Property Source() As cUnit

        Public Property Target() As cUnit

    End Class

End Namespace
