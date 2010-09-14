#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Namespace Ecospace.Advection

    ''' <summary>
    ''' Upwelling velocities control for advection form.
    ''' </summary>
    Public Class ucUpwelling

        Protected Overrides Function DataLayerVariable() As EwEUtils.Core.eVarNameFlags
            Return eVarNameFlags.LayerUpwelling
        End Function

    End Class

End Namespace
