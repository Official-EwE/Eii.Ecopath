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

''' <summary>
''' Plugin for initialization of Ecosim Scenarios
''' </summary>
''' <remarks>Contains plugin points for initialization of Ecosim Scenarios</remarks>
Public Interface IEcosimInitializedPlugin
    Inherits IPlugin

    ''' <summary>
    ''' Plugin Point called when an Ecosim Scenario has loaded
    ''' </summary>
    ''' <param name="EcosimDatastructures">cEcosimDataStructures passed as an object.</param>
    ''' <remarks>Called after an Ecosim scenario has loaded.</remarks>
    Sub EcosimInitialized(ByVal EcosimDatastructures As Object)

End Interface
