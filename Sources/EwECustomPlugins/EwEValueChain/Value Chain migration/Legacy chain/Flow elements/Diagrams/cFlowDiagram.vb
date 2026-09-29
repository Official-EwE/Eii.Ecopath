' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwEUtils.Utilities

Namespace ValueChainMigrator.LegacyData

    ''' ===========================================================================
    ''' <summary>
    ''' One single flow diagram.
    ''' </summary>
    ''' ===========================================================================
    <Obsolete(), Serializable()>
    Public Class cFlowDiagram
        Inherits cOOPStorable

        <Browsable(True), DisplayName("Name"), Description("Name of this diagram"), cPropertySorter.PropertyOrder(1)>
        Public Overridable Property Name() As String = "Default"

    End Class

End Namespace
