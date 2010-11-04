Option Strict On

''' ===========================================================================
''' <summary>
''' Base interface for defining an EwE6 plug-in. Plug-ins are detected by the
''' presence of this Interface.
''' </summary>
''' ===========================================================================
Public Interface IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the plugin.
    ''' </summary>
    ''' <param name="core">The core this plugin is initialized for.</param>
    ''' -----------------------------------------------------------------------
    Sub Initialize(ByVal core As Object)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Uniquely identifies a plugin.
    ''' </summary>
    ''' <remarks>
    ''' The name field will be used to determine the order of appearance of 
    ''' user interface plug-in elements; user interface elements originating
    ''' from plug-ins will be sorted by this property in ascending order.
    ''' </remarks>
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
