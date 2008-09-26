'==============================================================================
'
' $Log: IEcosimBeginTimestepPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:06  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2007/06/20 17:45:21  sherman
' Put CVS header
'
'
'
'==============================================================================

Public Interface IEcosimBeginTimestepPlugin
    Inherits IPlugin

    Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer)

End Interface
