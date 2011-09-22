#Region " Imports "

Option Strict On
Imports EwECore

#End Region

Namespace Ecospace.Basemap.Layers

    Public Class cLayerEditorDepth
        Inherits cLayerEditor

        Private m_bProtectLand As Boolean = False

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorDepth))
        End Sub

        Public Property ProtectCoastLine() As Boolean
            Get
                Return Me.m_bProtectLand
            End Get
            Set(ByVal value As Boolean)
                Me.m_bProtectLand = value
            End Set
        End Property

        Protected Overrides Sub SetCellValue(ByVal ptSet As Point, _
                                             ByVal value As Object, _
                                             ByVal e As MouseEventArgs, _
                                             ByVal ptClick As System.Drawing.Point)

            Dim layerDepth As cEcospaceLayerDepth = DirectCast(Me.Layer.Data, cEcospaceLayerDepth)
            Dim bIsLandCell As Boolean = (layerDepth.IsLandCell(ptSet.Y, ptSet.X))
            Dim bIsLandValue As Boolean = (CInt(value) = 0)

            If Me.m_bProtectLand Then
                If (bIsLandCell <> bIsLandValue) Then
                    Return
                End If
            End If

            MyBase.SetCellValue(ptSet, value, e, ptClick)

        End Sub

    End Class

End Namespace
