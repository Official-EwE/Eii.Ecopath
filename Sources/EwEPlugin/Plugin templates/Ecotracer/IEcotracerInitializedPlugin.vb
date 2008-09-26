'==============================================================================
'
' $Log: IEcotracerInitializedPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2007/11/25 19:45:34  jeroens
' Initial version
'
'==============================================================================

''' ---------------------------------------------------------------------------
''' <summary>
''' Ecotracer post-initialization plug-in
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcotracerInitializedPlugin
    Inherits IPlugin

    Sub EcotracerInitialized(ByVal ContaminantTracerDatastructures As Object)

End Interface
