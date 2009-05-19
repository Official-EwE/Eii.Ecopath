'==============================================================================
'
' $Log: IDisposedPlugin.vb,v $
' Revision 1.1  2009/05/19 13:18:46  jeroens
' Initial version
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plug-in that is explicitly de-initialized.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IDisposedPlugin
    Inherits IPlugin

    Sub Dispose()

End Interface
