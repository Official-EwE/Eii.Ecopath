' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style



Namespace Controls.EwEGrid

    ''' <summary>
    ''' Base interface for connecting EwE cells, visualizers and grids to share
    ''' the use of the <see cref="cStyleGuide"/>, <see cref="cCore"/> and <see cref="cPropertyManager"/>.
    ''' </summary>
    Public Interface IEwECell
        Inherits IUIElement

        ''' <summary>
        ''' Get the <see cref="cCore"/> catering to the current user interface.
        ''' </summary>
        ReadOnly Property Core As cCore

        ''' <summary>
        ''' Get the <see cref="cPropertyManager"/> catering to the current user interface.
        ''' </summary>
        ReadOnly Property PropertyManager As cPropertyManager

        ''' <summary>
        ''' Get the <see cref="cStyleGuide"/> catering to the current user interface.
        ''' </summary>
        ReadOnly Property StyleGuide As cStyleGuide

        ''' <summary>
        ''' Get the <see cref="cStyleGuide.eStyleFlags"/> cell style.
        ''' </summary>
        Property Style() As cStyleGuide.eStyleFlags

    End Interface

End Namespace
