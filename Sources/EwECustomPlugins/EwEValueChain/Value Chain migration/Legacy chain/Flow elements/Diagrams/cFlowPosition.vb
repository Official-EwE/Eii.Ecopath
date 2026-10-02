' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace ValueChainMigrator.LegacyData

    ''' ===========================================================================
    ''' <summary>
    ''' Position of a single unit in a flow diagram.
    ''' </summary>
    ''' ===========================================================================
    <Obsolete(), Serializable()>
    Public Class cFlowPosition
        Inherits cOOPStorable

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the diagram this flow position belongs to.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Diagram() As cFlowDiagram

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the unit that this position belongs to.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Unit() As cUnit

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the X position.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Property Xpos() As Integer

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the Y position.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Property Ypos() As Integer

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the width.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Property Width() As Integer

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the height.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Property Height() As Integer

    End Class

End Namespace