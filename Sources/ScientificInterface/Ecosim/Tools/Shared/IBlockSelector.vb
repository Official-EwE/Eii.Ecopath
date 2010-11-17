#Region " Imports "

Option Explicit On
Option Strict On

Imports ScientificInterface.Other

#End Region ' Imports

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
        Event OnNumBlocksChanged(ByVal sender As IBlockSelector)


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event notifying that selected block has changed.
        ''' </summary>
        ''' <param name="sender">
        ''' The <see cref="ucParmBlockCodes">block code parameters control</see>
        ''' that sent this event.
        ''' </param>
        ''' -------------------------------------------------------------------
        Event OnBlockSelected(ByVal sender As IBlockSelector)

        ''' <summary>
        ''' Value of a cell (CV) has changed
        ''' </summary>
        Event onValueChanged(ByVal newValue As Single, ByVal Index As Integer)

        Property NumBlocks() As Integer
        Property SelectedBlock() As Integer
        ReadOnly Property BlockColors() As Color()
        ReadOnly Property BlockColor(ByVal iBlock As Integer) As Color
        ReadOnly Property SelectedBlockColor() As Color

        Function ValuetoBlock(ByVal cv As Single) As Integer
        Function BlocktoValue(ByVal iBlock As Integer) As Single


    End Interface

End Namespace