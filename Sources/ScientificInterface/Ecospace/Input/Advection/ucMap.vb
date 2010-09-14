#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Namespace Ecospace.Advection

    ''' <summary>
    ''' Map control for advection form.
    ''' </summary>
    Public Class ucMap
        Inherits ucAdvectionMap

        ''' <inheritdoc cref="BackgroundLayers"/>
        Protected Overrides Function BackgroundLayers() As eVarNameFlags()
            Return New eVarNameFlags() {eVarNameFlags.LayerAdvection}
        End Function

    End Class

End Namespace
