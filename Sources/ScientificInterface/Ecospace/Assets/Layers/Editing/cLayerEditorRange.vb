#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Windows.Forms
Imports ScientificInterface.Ecospace.Basemap.Layers

#End Region ' Imports 

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modifications of layers where cells
    ''' can have a range of values.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorRange
        Inherits cLayerEditor

#Region " Construction "

        Public Sub New()
            Me.New(GetType(ucLayerEditorRange))
        End Sub

        Public Sub New(ByVal typeGUI As Type)
            MyBase.New(typeGUI)
        End Sub

#End Region ' Construction

    End Class

End Namespace
