'==============================================================================
'
' $Log: IEcosimBeginTimestepPlugin.vb,v $
' Revision 1.2  2009/03/10 18:22:17  jeroens
' Minimal housekeeping
'
'==============================================================================

Public Interface IEcosimBeginTimestepPlugin
    Inherits IPlugin

    Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer)

End Interface
