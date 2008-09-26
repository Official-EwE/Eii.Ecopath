'==============================================================================
'
' $Log: ICorePlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:05  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2007/06/14 18:02:22  joeb
' Added CoreInitialized plugin point
'
' Revision 1.5  2007/03/17 01:59:17  jeroens
' - Removed CoreExecutionState again since Core plugins are most likely system plugins and are thus subject to the regular flow of EwE. They do not need to option to enable or disable with core state changes
'
' Revision 1.4  2007/03/15 14:09:33  jeroens
' + Added CoreExecutionState
'
' Revision 1.3  2007/03/14 00:49:13  jeroens
' - Cleared-out now plugin-points are implemented as objects instead of function calls.
'
'==============================================================================

Option Strict On

Public Interface ICorePlugin
    Inherits IPlugin

    ''' <summary>
    ''' The core has loaded a model and initialized its internal data
    ''' </summary>
    ''' <param name="objEcoPath">The Ecopath model</param>
    ''' <param name="objEcoSim">The Ecosim model</param>
    ''' <param name="objEcoSpace">The Ecospace model</param>
    ''' <remarks></remarks>
    Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object)


End Interface
