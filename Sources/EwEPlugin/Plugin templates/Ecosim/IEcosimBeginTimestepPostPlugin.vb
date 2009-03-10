'==============================================================================
'
' $Log: IEcosimBeginTimestepPostPlugin.vb,v $
' Revision 1.1  2009/03/10 18:21:39  jeroens
' Initial version
'
'==============================================================================

''' <summary>
''' 
''' </summary>
Public Interface IEcosimBeginTimestepPostPlugin
    Inherits IPlugin

    Sub EcosimBeginTimeStepPost(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer)

End Interface
