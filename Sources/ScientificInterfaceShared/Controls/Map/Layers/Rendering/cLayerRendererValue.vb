#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports SAUPUtil.Misc.Colours
Imports ScientificInterfaceShared.Style

#End Region 'Imports

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells displaying the actual cell value number,
    ''' and scaling the cell background colour across a colour gradient based
    ''' on the cell value in relation to the layer min/max value range.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererValue
        Inherits cLayerRenderer

        Private m_brFore As Brush = Nothing
        Private m_ft As Font = Nothing
        Private m_bDrawAlways As Boolean = False

        Private m_colorRamp As cColorRamp = New cEwEColorRamp()

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor Or _
                    cVisualStyle.eVisualStyleTypes.Font Or _
                    cVisualStyle.eVisualStyleTypes.Gradient)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this attached layer should always rendered (True),
        ''' or only when the layer is <see cref="cLayer.IsSelected"/> (False).
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property DrawAlways() As Boolean
            Get
                Return Me.m_bDrawAlways
            End Get
            Set(ByVal value As Boolean)
                Me.m_bDrawAlways = value
            End Set
        End Property

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle, _
                                           ByVal layer As cEcospaceLayer)

            If Me.m_brFore Is Nothing Then Me.Update()

            If Me.IsStyleValid Then
                cColorRampIndicator.DrawColorRamp(g, Me.m_colorRamp, rc, False)
            Else
                Me.RenderError(g, rc)
            End If

        End Sub

        Public Overrides Sub RenderCell(ByVal g As System.Drawing.Graphics, _
                                        ByVal rc As System.Drawing.Rectangle, _
                                        ByVal layer As cEcospaceLayer, _
                                        ByVal value As Object, _
                                        ByVal style As cStyleGuide.eStyleFlags)

            Dim sValMax As Single = Me.ScaleMax
            Dim sValMin As Single = Me.ScaleMin

            If (sValMax = cCore.NULL_VALUE) Then sValMax = layer.MaxValue
            If (sValMin = cCore.NULL_VALUE) Then sValMin = layer.MinValue

            Try
                If Me.m_brFore Is Nothing Then Me.Update()

                ' Draw background
                ' Render value on top for highlighted layers
                If ((style And cStyleGuide.eStyleFlags.Highlight) = cStyleGuide.eStyleFlags.Highlight) Or _
                   (Me.m_bDrawAlways = True) Then

                    If (value IsNot Nothing) And (Me.m_ft IsNot Nothing) Then

                        Dim sValue As Single = CSng(value)
                        Dim sValRange As Single = (sValMax - sValMin)

                        ' Has a range? draw background
                        If (sValRange > 0.0) Then
                            ' Calculate the cell color based on the cell value RELATIVE TO [sValueMin, sValueMax),
                            ' not (0, sValueMax)!!!
                            Using br As New SolidBrush(m_colorRamp.GetColor(sValue - sValMin, sValMax - sValMin))
                                g.FillRectangle(br, rc)
                            End Using
                        Else
                            Using br As New SolidBrush(m_colorRamp.GetColor(sValue, sValMax))
                                g.FillRectangle(br, rc)
                            End Using
                        End If
                        ' Draw value
                        g.DrawString(String.Format("{0}", value), Me.m_ft, Me.m_brFore, rc)
                    End If
                End If
            Catch ex As Exception
                ' Boom
            End Try
        End Sub

        Public Overrides Sub Update()

            Dim vs As cVisualStyle = Me.VisualStyle

            If vs Is Nothing Then
                Me.m_brFore = cLayerRenderer.brDEFAULT
            Else
                Me.m_brFore = New SolidBrush(vs.ForeColour)

                If (Me.m_ft IsNot Nothing) Then Me.m_ft.Dispose()
                Me.m_ft = New Font(vs.FontName, Me.VisualStyle.FontSize, Me.VisualStyle.FontStyle)

                If (vs.GradientBreaks IsNot Nothing) And (vs.GradientColors IsNot Nothing) Then
                    Me.m_colorRamp = New cARGBColorRamp(vs.GradientColors, vs.GradientBreaks)
                Else
                    Me.m_colorRamp = New cEwEColorRamp()
                End If
            End If

        End Sub

        Protected Overrides Function IsStyleValid() As Boolean
            If Not MyBase.IsStyleValid() Then Return False
            Return (Not String.IsNullOrEmpty(Me.VisualStyle.FontName) Or (Me.VisualStyle.FontSize > 1))
        End Function

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(disposing)
            Me.m_ft.Dispose()
            Me.m_ft = Nothing
            Me.m_brFore.Dispose()
            Me.m_brFore = Nothing
        End Sub

    End Class

End Namespace
