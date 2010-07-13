#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Xml

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plugin point that allows a plug-in to store and retrieve settings in a 
''' framework-provided settings file. Settings are stored in a system-defined
''' node in an XML document that should be managed by the framework that
''' created the plug-in manager.
''' </summary>
''' ===========================================================================
Public Interface ISettingsPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initializes the settings plug-in with an xml document and node for
    ''' storing and retrieving persistent settings.
    ''' </summary>
    ''' <param name="doc">The XML document, provided by the EwE framework, where
    ''' the plug-in can store and retrieve its persistent configuration from.</param>
    ''' <param name="node">The XML node the plug-in should operate onto.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Function InitializeSettings(ByVal doc As XmlDocument, ByVal node As XmlNode) As Boolean

End Interface
