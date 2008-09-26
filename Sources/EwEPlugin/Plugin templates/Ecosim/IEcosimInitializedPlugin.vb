'==============================================================================
'
' $Log: IEcosimInitializedPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:06  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2007/06/20 17:45:21  sherman
' Put CVS header
'
'
'
'==============================================================================

Public Interface IEcosimInitializedPlugin
    Inherits IPlugin

    Sub EcosimInitialized(ByVal EcosimDatastructures As Object)

End Interface
