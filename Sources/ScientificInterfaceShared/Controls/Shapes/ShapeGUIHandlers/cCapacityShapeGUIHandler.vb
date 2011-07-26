#Region " Imports "

Option Strict On

Imports EwECore
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Commands
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls



    Public Class cCapacityShapeGUIHandler
        Inherits cMediationShapeGUIHandler


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As cBaseShapeManager
            Return Me.Core.CapacityShapeManager
        End Function

    End Class

End Namespace
