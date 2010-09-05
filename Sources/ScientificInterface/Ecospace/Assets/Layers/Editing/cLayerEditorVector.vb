#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Windows.Forms
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports EwEUtils.Core

#End Region ' Imports 

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of fleets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorVector
        Inherits cLayerEditor

#Region " Construction "

        Public Sub New()
            Me.New(Nothing)
        End Sub

        Public Sub New(ByVal t As Type)
            MyBase.New(t)
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.StartEdit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub StartEdit(ByVal ptClick As Point, ByVal buttons As MouseEventArgs)
            MyBase.StartEdit(ptClick, buttons)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.Edit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Edit(ByVal ptFrom As Point, _
                                  ByVal ptTo As Point, _
                                  ByVal args As MouseEventArgs, _
                                  ByRef ptUpdateMin As Point, _
                                  ByRef ptUpdateMax As Point)
            MyBase.Edit(ptFrom, ptTo, args, ptUpdateMin, ptUpdateMax)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.EndEdit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub EndEdit()
            MyBase.EndEdit()
        End Sub

        Protected Overrides Sub SetCellValue(ByVal ptSet As Point, _
                                             ByVal e As MouseEventArgs, _
                                             ByVal ptClick As Point)

            Dim r As New Random
            Me.Layer.Value(ptSet.Y, ptSet.X) = New Single() {CSng(r.NextDouble * 10) - 5.0!, CSng(r.NextDouble * 10) - 5.0!}
        End Sub

        Public Overrides Property CellValue() As Object
            Get
                Return New Single() {1, -1}
            End Get
            Set(ByVal value As Object)
                'MyBase.CellValue = value
            End Set
        End Property

#End Region ' Public interfaces

    End Class

End Namespace