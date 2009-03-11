'==============================================================================
'
' $Log: IEcosimRunCompletedPostPlugin.vb,v $
' Revision 1.1  2009/03/11 14:57:45  jeroens
' Initial version
'
'==============================================================================

''' <summary>
''' 
''' </summary>
Public Interface IEcosimRunCompletedPostPlugin
    Inherits IPlugin

    Sub EcosimRunCompletedPost(ByVal EcosimDatastructures As Object)

End Interface
