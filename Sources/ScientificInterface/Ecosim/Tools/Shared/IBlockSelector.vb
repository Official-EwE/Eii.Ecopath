' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



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