#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Xml

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plugin point that allows a plug-in to store and retrieve persistent 
''' configuration settings.
''' </summary>
''' ===========================================================================
Public Interface IConfigurablePersistPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initializes the configurable plug-in with an xml document and node for
    ''' storing and retrieving persistent configuration.
    ''' </summary>
    ''' <param name="doc">The XML document, provided by the EwE framework, where
    ''' the plug-in can store and retrieve its persistent configuration from.</param>
    ''' <param name="node">The XML node the plug-in should operate onto.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Function SetConfigutationNode(ByVal doc As XmlDocument, ByVal node As XmlNode) As Boolean

End Interface
