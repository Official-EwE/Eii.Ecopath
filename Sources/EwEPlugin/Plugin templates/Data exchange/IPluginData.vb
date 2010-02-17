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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' The <see cref="IRunType">run type</see> that this data was produced with.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property RunType() As IRunType

    End Interface

End Namespace
