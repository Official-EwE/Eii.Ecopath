'==============================================================================
'
' $Log: IEcosimEndTimestepPlugin.vb,v $
' Revision 1.2  2009/03/10 18:22:17  jeroens
' Minimal housekeeping
'
' Revision 1.1  2008/09/26 07:31:06  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2007/06/29 22:41:39  sherman
' Added IEcosimEndTimestepPlugin
'
' Revision 1.3  2007/06/20 17:45:21  sherman
' Put CVS header
'
'==============================================================================

Public Interface IEcosimEndTimestepPlugin
    Inherits IPlugin

    Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object)

End Interface


