'==============================================================================
'
' $Log: IEcopathRunCompletedPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:06  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2007/03/14 00:51:27  jeroens
' - Extracted from IEcopathPlugin now plugin-points are implemented as objects instead of function calls.
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that is automatically invoked when
''' Ecopath has ran succesfully.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcopathRunCompletedPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execute an Ecopath Run Completed plug-in.
    ''' </summary>
    ''' <param name="EcoPathDataStructures">A reference to the Ecopath data 
    ''' structures as defined in the EwE project.</param>
    ''' <remarks>This plug-in point is non-exclusive, meaning that multiple
    ''' plug-ins can respond to this event.</remarks>
    ''' -----------------------------------------------------------------------
    Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object)

End Interface
