'==============================================================================
'
' $Log: IEcospaceEndTimestepPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/06/27 02:33:15  jeroens
' Added header
'
' Revision 1.2  2007/06/20 17:45:22  sherman
' Put CVS header
'
'==============================================================================

Public Interface IEcospaceEndTimestepPlugin
    Inherits IPlugin

    Sub EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer)

End Interface


