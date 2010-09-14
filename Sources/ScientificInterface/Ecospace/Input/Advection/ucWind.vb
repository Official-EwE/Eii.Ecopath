#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Namespace Ecospace.Advection

    ''' <summary>
    ''' Wind control for advection form.
    ''' </summary>
    Public Class ucWind

        Protected Overrides Function DataLayerVariable() As EwEUtils.Core.eVarNameFlags
            Return eVarNameFlags.LayerWind
        End Function

    End Class

End Namespace
