'==============================================================================
'
' $Log: IEcospaceEndTimestepPostPlugin.vb,v $
' Revision 1.1  2009/03/10 18:22:34  jeroens
' Initial version
'
'==============================================================================

''' <summary>
''' 
''' </summary>
Public Interface IEcospaceEndTimestepPostPlugin
    Inherits IPlugin

    Sub EcospaceEndTimeStepPost(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer)

End Interface


