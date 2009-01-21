'==============================================================================
'
' $Log: IPluginData.vb,v $
' Revision 1.1  2009/01/21 19:08:12  jeroens
' Moved and split into separate files
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Base type for data shared by plugins.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IPluginData

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Name of the <see cref="cPluginAssembly">plugin assembly</see> that 
        ''' exposed this data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property AssemblyName() As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Name of the <see cref="IPlugin">plugin</see> that exposed this data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property PluginName() As String

    End Interface

End Namespace
