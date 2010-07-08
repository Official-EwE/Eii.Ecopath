#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Xml

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plugin point that allows a configurabe plug-in to store and retrieve its
''' settings
''' </summary>
''' ===========================================================================
Public Interface IConfigurablePersistPlugin
    Inherits IConfigurablePlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enables a configurable plug-in to read its configuration from an XML
    ''' node. This node will have the same name as the plug-in.
    ''' </summary>
    ''' <param name="node">The node to read configuration information from.</param>
    ''' -----------------------------------------------------------------------
    Sub ReadConfiguration(ByVal node As XmlNode)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enables a configurable plug-in to write its configuration to an XML
    ''' node. This node will have the same name as the plug-in.
    ''' </summary>
    ''' <param name="node">The node to write configuration information to.</param>
    ''' -----------------------------------------------------------------------
    Sub WriteConfiguration(ByVal node As XmlNode)

End Interface
