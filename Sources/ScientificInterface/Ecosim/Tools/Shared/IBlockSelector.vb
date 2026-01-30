' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On




Namespace Ecosim

    Public Interface IBlockSelector
        Inherits IUIElement

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event notifying that the number of blocks have changed.
        ''' </summary>
        ''' <param name="sender">
        ''' The <see cref="ucParmBlockCodes">block code parameters control</see>
        ''' that sent this event.
        ''' </param>
        ''' -------------------------------------------------------------------
        Event OnNumBlocksChanged(sender As IBlockSelector)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event notifying that selected block has changed.
        ''' </summary>
        ''' <param name="sender">
        ''' The <see cref="ucParmBlockCodes">block code parameters control</see>
        ''' that sent this event.
        ''' </param>
        ''' -------------------------------------------------------------------
        Event OnBlockSelected(sender As IBlockSelector)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Value of a cell (CV) has changed
        ''' </summary>
        ''' -------------------------------------------------------------------
        Event OnValueChanged(newValue As Single, Index As Integer)

        Property NumBlocks() As Integer
        Property SelectedBlock() As Integer
        ReadOnly Property BlockColors() As Color()
        ReadOnly Property BlockColor(iBlock As Integer) As Color
        ReadOnly Property SelectedBlockColor() As Color

        Function ValuetoBlock(cv As Single) As Integer
        Function BlocktoValue(iBlock As Integer) As Single

    End Interface

End Namespace