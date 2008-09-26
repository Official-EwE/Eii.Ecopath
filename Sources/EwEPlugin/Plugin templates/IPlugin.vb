'==============================================================================
'
' $Log: IPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:09  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/06/19 08:50:53  jeroens
' *sigh*
'
' Revision 1.3  2007/10/30 19:20:52  jeroens
' + Plugins need Author, contact
'
' Revision 1.2  2007/03/14 00:45:00  jeroens
' + Added Description
'
' Revision 1.1  2006/08/30 20:52:35  jeroens
' * Moved and/or created
'
' Revision 1.1  2006/08/08 14:11:50  jeroens
' + Initial version
'
'==============================================================================

Option Strict On

Public Interface IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initializes the plugin.
    ''' </summary>
    ''' <param name="core">The core this plugin is initialized for.</param>
    ''' -----------------------------------------------------------------------
    Sub Initialize(ByVal core As Object)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Uniquely identifies a plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property Name() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Uniquely describes a plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property Description() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Describes the author of the plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property Author() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provides contact information about the plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property Contact() As String

End Interface
