'==============================================================================
'
' $Log: IEcospaceBeginTimestepPostPlugin.vb,v $
' Revision 1.1  2009/03/10 18:22:34  jeroens
' Initial version
'
'==============================================================================

''' <summary>
''' 
''' </summary>
Public Interface IEcospaceBeginTimestepPostPlugin
    Inherits IPlugin

    Sub EcospaceBeginTimeStepPost(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer)

End Interface
