'==============================================================================
'
' $Log: IDockStatePlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/09/06 16:16:24  jeroens
' Initial version
'
'==============================================================================

Option Strict On

''' ===========================================================================
''' <summary>
''' Plugin point that allows a GUI plugin to state its desired dock location.
''' </summary>
''' ===========================================================================
Public Interface IDockStatePlugin
    Inherits IGUIPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The dockstate for the form of this plugin.
    ''' </summary>
    ''' <remarks>
    ''' Values are interpreted as
    ''' WeifenLuo DockState enumerated values. This project is not linked to
    ''' WeifenLuo's DockPanel suite, but implementing plug-ins can include
    ''' such a reference and return actual DockState enumerated values here.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Function DockState() As Integer

End Interface
