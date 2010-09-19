Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a layer editor user interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ILayerEditorGUI

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize an editor GUI to a layer.
        ''' </summary>
        ''' <param name="editor"></param>
        ''' -------------------------------------------------------------------
        Sub Initialize(ByVal editor As cLayerEditor)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Inform the editor GUI that the user has started editing the layer.
        ''' </summary>
        ''' <param name="editor"></param>
        ''' -------------------------------------------------------------------
        Sub StartEdit(ByVal editor As cLayerEditor)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Inform the editor GUI that the user has finished editing the layer.
        ''' </summary>
        ''' <param name="editor"></param>
        ''' -------------------------------------------------------------------
        Sub EndEdit(ByVal editor As cLayerEditor)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the content of the editor GUI because something in the
        ''' underlying <see cref="ScientificInterface.Ecospace.Basemap.Layers.cLayer">layer</see> or
        ''' <see cref="cLayerEditor">layer editor</see> has changed.
        ''' </summary>
        ''' <param name="editor"></param>
        ''' -------------------------------------------------------------------
        Sub UpdateContent(ByVal editor As cLayerEditor)

    End Interface

End Namespace
