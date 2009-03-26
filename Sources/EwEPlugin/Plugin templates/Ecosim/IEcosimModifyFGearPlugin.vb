'==============================================================================
'
' $Log: IEcosimModifyFGearPlugin.vb,v $
' Revision 1.1  2009/03/26 02:06:22  sherman
' Added Plugin point EcosimModifyFGear
'
'==============================================================================
Public Interface IEcosimModifyFGearPlugin
    Inherits IPlugin

    Sub EcosimModifyFGear(ByVal FGear As Object, ByVal EcosimDataStructures As Object)

End Interface
