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

        ''' <inheritdoc cref="DataLayer"/>
        Protected Overrides Function DataLayerVariable() As EwEUtils.Core.eVarNameFlags
            Return eVarNameFlags.LayerAdvection
        End Function

        ''' <inheritdoc cref="IsDataInput"/>
        Protected Overrides Function IsDataInput() As Boolean
            Return False
        End Function

    End Class

End Namespace
