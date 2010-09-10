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

        Protected Overrides Function EditableLayer() As EwEUtils.Core.eVarNameFlags
            Return eVarNameFlags.LayerMLD
        End Function

    End Class

End Namespace
