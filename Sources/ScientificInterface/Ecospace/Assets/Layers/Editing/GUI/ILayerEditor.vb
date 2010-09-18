Namespace Ecospace.Basemap.Layers

    Public Interface ILayerEditor
        Sub StartEdit(ByVal editor As cLayerEditor)
        Sub EndEdit(ByVal editor As cLayerEditor)
        Sub UpdateContent(ByVal editor As cLayerEditor)
    End Interface

End Namespace
