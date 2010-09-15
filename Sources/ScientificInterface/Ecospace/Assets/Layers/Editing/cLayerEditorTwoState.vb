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
    ''' have two values: set or cleared.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorTwoState
        Inherits cLayerEditor

        Public Sub New()
            Me.New(Nothing)
        End Sub

        Public Sub New(ByVal typeGUI As Type)
            MyBase.New(typeGUI)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.Initialize"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Initialize(ByVal uic As cUIContext, ByVal layer As cLayer)
            MyBase.Initialize(uic, layer)
            Me.CellValueMax = CDec(Math.Max(layer.ValueSet, layer.ValueClear))
            Me.CellValueMin = CDec(Math.Min(layer.ValueSet, layer.ValueClear))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.StartEdit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub StartEdit(ByVal ptClick As Point, ByVal buttons As MouseEventArgs)

            ' Clicked on data cell?
            If Object.Equals(Layer.Value(ptClick.Y, ptClick.X), Layer.ValueSet) Then
                ' #Yes: start clearing values
                Me.CellValue = CDec(Layer.ValueClear)
            Else
                ' #No: start setting values
                Me.CellValue = CDec(Layer.ValueSet)
            End If

            If Me.GUI IsNot Nothing Then
                ' Trigger GUI to update to the changes
                Me.GUI.UpdateContent()
            End If

        End Sub

    End Class

End Namespace