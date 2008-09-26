'==============================================================================
'
' $Log: IEcosimRunCompletedPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2007/06/20 17:45:21  sherman
' Put CVS header
'
'
'
'==============================================================================

Public Interface IEcosimRunCompletedPlugin
    Inherits IPlugin

    Sub EcosimRunCompleted(ByVal EcosimDatastructures As Object)

End Interface
