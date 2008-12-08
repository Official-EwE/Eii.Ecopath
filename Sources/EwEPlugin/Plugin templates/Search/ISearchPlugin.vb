'==============================================================================
'
' $Log: ISearchPlugin.vb,v $
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
Public Interface ISearchPlugin

    Sub SearchInitialized(ByVal SearchDatastructures As Object)

End Interface
