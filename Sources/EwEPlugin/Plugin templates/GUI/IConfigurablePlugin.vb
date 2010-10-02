#Region " Imports "

Option Strict On

Imports System.Windows.Forms
Imports System.Xml

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plugin point that provides a configuration interface as a form.
''' </summary>
''' ===========================================================================
Public Interface IConfigurablePlugin
    Inherits IGUIPlugin

    Function IsConfigured() As Boolean
    Function GetConfigUI() As Form

End Interface
