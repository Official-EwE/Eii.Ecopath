'==============================================================================
'
' $Log: IEcospacePostFishingEffortModTimestepPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/09/04 06:42:48  sherman
' Added IEcospacePostFishingEffortModTimestepPlugin
'
'==============================================================================

Public Interface IEcospacePostFishingEffortModTimestepPlugin
    Inherits IPlugin

    Sub EcospacePostFishingEffortModTimestep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer)

End Interface
