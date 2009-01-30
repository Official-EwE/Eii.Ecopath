'==============================================================================
'
' $Log: IFishingPolicySearchPlugin.vb,v $
' Revision 1.1  2009/01/30 23:11:45  joeb
' Rename ISearchPlugin to IFishingPolicySearchPlugin
'
' Revision 1.1  2008/12/08 16:43:20  jeroens
' Initial version
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing plugin points that are invoked from the EwE
''' searches
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IFishingPolicySearchPlugin

    Sub SearchInitialized(ByVal SearchDatastructures As Object)

End Interface
