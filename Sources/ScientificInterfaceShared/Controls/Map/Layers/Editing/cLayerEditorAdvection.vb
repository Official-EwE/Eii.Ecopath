#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls.Map.Layers

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modification of Ecospace advection data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorAdvection
        Inherits cLayerEditor

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorAdvection))
        End Sub

#End Region ' Construction

    End Class

End Namespace