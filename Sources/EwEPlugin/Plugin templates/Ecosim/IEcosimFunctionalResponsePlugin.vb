'==============================================================================
'
' $Log: IEcosimFunctionalResponsePlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:06  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2007/06/20 17:45:21  sherman
' Put CVS header
'
'
'
'==============================================================================

Public Interface IEcosimFunctionalResponsePlugin
    Inherits IPlugin

    Function FunctionalResponse(ByVal EcosimDatastructures As Object, _
        ByVal StanzaDataStructures As Object, _
        ByVal WeightAtAge As Single(), _
        ByVal PredPreyAbundance As Single(), _
        ByRef iResult As Integer) As Boolean

End Interface
