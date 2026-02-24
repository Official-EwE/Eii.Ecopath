' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a user interface for editing raster layers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ILayerEditorGUI

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize an editor GUI to a raster layer.
        ''' </summary>
        ''' <param name="editor"></param>
        ''' -------------------------------------------------------------------
        Sub Initialize(editor As cLayerEditorRaster)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Inform the editor GUI that the user has started editing the 
        ''' raster layer.
        ''' </summary>
        ''' <param name="editor"></param>
        ''' -------------------------------------------------------------------
        Sub StartEdit(editor As cLayerEditorRaster)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Inform the editor GUI that the user has finished editing the raster 
        ''' layer.
        ''' </summary>
        ''' <param name="editor"></param>
        ''' -------------------------------------------------------------------
        Sub EndEdit(editor As cLayerEditorRaster)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the content of the editor GUI because something in the
        ''' underlying <see cref="cDisplayLayer">display layer</see> or
        ''' <see cref="cLayerEditorRaster">raster layer editor</see> has changed.
        ''' </summary>
        ''' <param name="editor"></param>
        ''' -------------------------------------------------------------------
        Sub UpdateContent(editor As cLayerEditorRaster)

    End Interface

End Namespace
