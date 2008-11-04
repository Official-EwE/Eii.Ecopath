'==============================================================================
'
' $Log: cLayerEditorTwoState.vb,v $
' Revision 1.1  2008/11/04 04:40:16  jeroens
' Split into separate files, moved
'
'==============================================================================

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
            MyBase.New(GetType(ucLayerEditorDefault))
        End Sub

        Public Overrides Sub StartEdit(ByVal ptClick As Point)

            If Me.GUI IsNot Nothing Then
                ' Clicked on data cell?
                If Object.Equals(Layer.Value(ptClick), Layer.ValueSet) Then
                    ' #Yes: start clearing values
                    Me.CellValue = Layer.ValueClear
                Else
                    ' #No: start setting values
                    Me.CellValue = Layer.ValueSet
                End If
                ' Trigger GUI to update to the changes
                Me.GUI.UpdateControls()
            End If

        End Sub

    End Class

End Namespace