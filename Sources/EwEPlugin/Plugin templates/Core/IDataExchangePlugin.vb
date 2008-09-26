'==============================================================================
'
' $Log: IDataExchangePlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:05  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/07/06 17:24:01  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that exposes its data.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IDataExchangePlugin
    : Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialization interface for connecting this type of plugin with the
    ''' plugin manager, from which it can obtain data from other plugins.
    ''' </summary>
    ''' <param name="manager">The plugin manager.</param>
    ''' -----------------------------------------------------------------------
    Sub Manager(ByVal manager As cPluginManager)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface to request the plug-in for data
    ''' </summary>
    ''' <param name="varname">Enumerator-based name of the variable to request.</param>
    ''' -----------------------------------------------------------------------
    Function GetData(ByVal varname As eVarNameFlags, Optional ByVal iIndex As Integer = -9999) As Object

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface to request the plug-in for data
    ''' </summary>
    ''' <param name="strVarName">String-based name of the variable to request.</param>
    ''' -----------------------------------------------------------------------
    Function GetData(ByVal strVarName As String, Optional ByVal iIndex As Integer = -9999) As Object

End Interface
