Namespace Ecospace.Basemap.Layers

    Public Class ucLayerEditorDepth

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)
            Try
                If bDisposing Then
                    If (Me.UIContext Is Nothing) Then Return

                    RemoveHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(bDisposing)
            End Try
        End Sub

#End Region ' Construction / destruction

#Region " Overrides "

        Public Overrides Sub Initialize(ByVal editor As cLayerEditor)
            MyBase.Initialize(editor)

            Me.m_nudDepth.DecimalPlaces = 0
            Me.m_nudDepth.Maximum = Convert.ToDecimal(Me.Editor.CellValueMax)
            Me.m_nudDepth.Minimum = Math.Max(1, Convert.ToDecimal(Me.Editor.CellValueMin))

            If CSng(Me.Editor.CellValue) > 0 Then
                Me.m_rbWater.Checked = True
            Else
                Me.m_rbLand.Checked = True
            End If
            Me.UpdatePreview(Me.m_pbPreviewLand, 0)
        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            ' Sanity check
            If (Me.m_nudDepth Is Nothing) Then Return
            If (Me.UIContext Is Nothing) Then Return

            ' Set control value
            Dim sValue As Single =CSng(Me.Editor.CellValue)
            If sValue > 0 Then
                Me.m_nudDepth.Value = Convert.ToDecimal(Math.Max(Math.Min(sValue, CSng(Me.m_nudDepth.Maximum)), CSng(Me.m_nudDepth.Minimum)))
                Me.UpdatePreview(Me.m_pbPreviewWater, sValue)
            End If

        End Sub

#End Region ' Overrides

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            If (Me.UIContext Is Nothing) Then Return
            AddHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.UpdateContent(Me.Editor)
        End Sub

        Private Sub OnWaterSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbWater.CheckedChanged
            Me.UpdateValue()
        End Sub

        Private Sub OnLandSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbLand.CheckedChanged
            Me.UpdateValue()
        End Sub

        Private Sub OnValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudDepth.ValueChanged

            If (Me.UIContext Is Nothing) Then Return

            Me.m_rbWater.Checked = True
            Me.UpdateValue()
        End Sub

        Private Sub OnStyleGuideChanged(ByVal cf As cStyleGuide.eChangeType)
            If ((cf And cStyleGuide.eChangeType.NumberFormatting) > 0) Then
                Me.UpdateContent(Me.Editor)
            End If
        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub UpdatePreview(ByVal pb As PictureBox, ByVal sValue As Single)

            Dim bmp As New Bitmap(pb.Width, pb.Height, Imaging.PixelFormat.Format32bppArgb)
            Dim g As Graphics = Graphics.FromImage(bmp)
            Dim renderer As cLayerRenderer = Nothing

            If Me.Layer IsNot Nothing Then
                renderer = Me.Layer.Renderer
            End If

            If (renderer IsNot Nothing) Then
                renderer.RenderCell(g, New Rectangle(0, 0, bmp.Width, bmp.Height), _
                                    Me.Layer.Data, sValue, _
                                    cStyleGuide.eStyleFlags.Highlight)
            End If
            pb.Image = bmp

            g.Dispose()

        End Sub

        Private Sub UpdateValue()

            If (Me.UIContext Is Nothing) Then Return

            If Me.m_rbWater.Checked Then
                Me.Editor.CellValue = Me.m_nudDepth.Value
            Else
                Me.Editor.CellValue = 0
            End If
        End Sub

#End Region ' Internals

    End Class

End Namespace