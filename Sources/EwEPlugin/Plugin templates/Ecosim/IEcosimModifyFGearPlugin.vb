'==============================================================================
'
' $Log: IEcosimModifyFGearPlugin.vb,v $
' Revision 1.2  2009/04/02 01:30:23  sherman
' Passed BB into ModifyFGearPlugin
'
' Revision 1.1  2009/03/26 02:06:22  sherman
' Added Plugin point EcosimModifyFGear
'
'==============================================================================
Public Interface IEcosimModifyFGearPlugin
    Inherits IPlugin

    Sub EcosimModifyFGear(ByVal FGear As Object, ByVal BB As Object, ByVal EcosimDataStructures As Object, ByVal CurrentTime As Object)

End Interface
