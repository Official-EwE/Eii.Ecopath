#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace.Advection

    ''' <summary>
    ''' Mixed Layer Depths control for advection form
    ''' </summary>
    Public Class ucMLD

        ''' <summary>
        ''' Overridden to specify the mixed layer depth layer as 
        ''' the data layer in this control.
        ''' </summary>
        Protected Overrides Function DataLayerVariable() As EwEUtils.Core.eVarNameFlags
            Return eVarNameFlags.LayerMLD
        End Function

        ''' <summary>
        ''' Overridden to show habitats in the background.
        ''' </summary>
        Protected Overrides Function BackgroundLayers() As EwEUtils.Core.eVarNameFlags()
            Return New eVarNameFlags() {eVarNameFlags.LayerHabitat}
        End Function

    End Class

End Namespace
