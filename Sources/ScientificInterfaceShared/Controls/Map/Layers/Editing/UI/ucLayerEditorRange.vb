#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Definitions

#End Region ' Imports

Namespace Controls.Map.Layers

    ''' =======================================================================
    ''' <summary>
    ''' Layer editor interface for editing a layer that may contain a range of values.
    ''' </summary>
    ''' =======================================================================
    Public Class ucLayerEditorRange

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

            If Me.Editor.Layer.ValueType Is GetType(Integer) Then
                Me.m_nudValue.DecimalPlaces = 0
            Else
                Me.m_nudValue.DecimalPlaces = Me.UIContext.StyleGuide.NumDigits
            End If

            ' Set control max value
            If Convert.ToSingle(Decimal.MaxValue) < Me.Editor.CellValueMax Then
                Me.m_nudValue.Maximum = Decimal.MaxValue
            Else
                Me.m_nudValue.Maximum = Convert.ToDecimal(Me.Editor.CellValueMax)
            End If

            ' Set control min value
            If Convert.ToSingle(Decimal.MinValue) > Me.Editor.CellValueMin Then
                Me.m_nudValue.Minimum = Decimal.MaxValue
            Else
                Me.m_nudValue.Minimum = Convert.ToDecimal(Me.Editor.CellValueMin)
            End If

            ' Set increment
            If (Me.m_nudValue.Maximum - Me.m_nudValue.Minimum) <= 1000 Then
                Me.m_nudValue.Increment = (Me.m_nudValue.Maximum - Me.m_nudValue.Minimum) / 100
            End If
        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            ' Sanity check
            If (Me.m_nudValue Is Nothing) Then Return
            If (Me.UIContext Is Nothing) Then Return

            ' Set control value
            Dim sValue As Single = Math.Max(Math.Min(CSng(Me.Editor.CellValue), CSng(Me.m_nudValue.Maximum)), _
                                            CSng(Me.m_nudValue.Minimum))

            Me.m_nudValue.Value = Convert.ToDecimal(sValue)
            Me.m_btnSmooth.Enabled = Me.Editor.CanSmooth

            Dim bmp As New Bitmap(Me.m_pbPreview.Width, Me.m_pbPreview.Height, Imaging.PixelFormat.Format32bppArgb)
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
            Me.m_pbPreview.Image = bmp

            g.Dispose()

            Dim bEditable As Boolean = editor.IsEditable
            Me.m_nudValue.Enabled = bEditable
            Me.m_btnFill.Enabled = bEditable
            Me.m_btnSmooth.Enabled = bEditable
            Me.m_lbValue.Enabled = bEditable

        End Sub

#End Region ' Overrides

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            AddHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.UpdateContent(Me.Editor)
        End Sub

        Private Sub OnValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudValue.ValueChanged

            If Me.Editor.Layer.ValueType Is GetType(Integer) Then
                Me.Editor.CellValue = Me.m_nudValue.Value
            Else
                Me.Editor.CellValue = Me.m_nudValue.Value
            End If
        End Sub

        Private Sub OnStyleGuideChanged(ByVal cf As cStyleGuide.eChangeType)
            If ((cf And cStyleGuide.eChangeType.NumberFormatting) > 0) Then
                Me.UpdateContent(Me.Editor)
            End If
        End Sub

        Private Sub OnSmooth(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSmooth.Click
            Me.Editor.Smooth()
        End Sub

        Private Sub OnFillLayer(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnFill.Click
            Me.Editor.Fill()
        End Sub

        Private Sub OnClickPreview(sender As System.Object, e As System.EventArgs) Handles m_pbPreview.Click
            Me.EditLayer(eLayerEditTypes.EditVisuals)
        End Sub

#End Region ' Events

#Region " Internals "

        Protected Sub EditLayer(ByVal edittype As eLayerEditTypes)
            Try
                Dim cmd As cEditLayerCommand = DirectCast(Me.UIContext.CommandHandler.GetCommand(cEditLayerCommand.cCOMMAND_NAME), cEditLayerCommand)
                cmd.Invoke(Me.Layer, Nothing, edittype)
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Internals

    End Class

End Namespace
