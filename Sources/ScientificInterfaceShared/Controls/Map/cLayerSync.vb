#Region " Imports "

Option Explicit On
Option Strict On

Imports System.ComponentModel
Imports System.IO
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.Map

    Public Class cLayerSync

        Private m_lLayers As New List(Of cLayer)

        Public Sub AddLayer(l As cLayer)

        End Sub

        Public Sub RemoveLayer(l As cLayer)

        End Sub

        Private Sub OnLayerChanged(l As cLayer, ct As cLayer.eChangeFlags)

        End Sub

    End Class

End Namespace
