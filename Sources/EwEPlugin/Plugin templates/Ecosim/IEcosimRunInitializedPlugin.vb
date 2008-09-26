'==============================================================================
'
' $Log: IEcosimRunInitializedPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2007/06/20 17:45:22  sherman
' Put CVS header
'
'
'
'==============================================================================

Public Interface IEcosimRunInitializedPlugin
    Inherits IPlugin

    ''' <summary>
    ''' Ecosim has initialized and is about to start the time loop
    ''' </summary>
    ''' <param name="EcosimDatastructures"></param>
    ''' <remarks></remarks>
    Sub EcosimRunInitialized(ByVal EcosimDatastructures As Object)

End Interface
