'==============================================================================
'
' $Log: IEcospaceBeginTimestepPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/06/27 02:33:15  jeroens
' Added header
'
'==============================================================================

Public Interface IEcospaceBeginTimestepPlugin
    Inherits IPlugin

    Sub EcospaceBeginTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer)


End Interface
