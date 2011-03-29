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

            Dim bIsLandCell As Boolean = (CInt(Me.Layer.Value(ptSet.Y, ptSet.X)) = 0)
            Dim bIsLandValue As Boolean = (CInt(value) = 0)

            If Me.m_bProtectLand And (bIsLandCell <> bIsLandValue) Then Return

            MyBase.SetCellValue(ptSet, value, e, ptClick)

        End Sub

    End Class

End Namespace
