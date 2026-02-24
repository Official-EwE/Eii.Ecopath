' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells for the <see cref="eVarNameFlags.LayerExclusion">exclusion layer</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererExclusion
        Inherits cLayerRendererHatch

        Public Sub New(uic As cUIContext, vs As cVisualStyle)
            MyBase.New(uic, vs)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Draw the cell as an arrow with a given angle and scale.
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="rc"></param>
        ''' <param name="value">A two-dimensional array of singles, 
        ''' holding the angle [0, 360] as the first index, and the scale
        ''' [0, 1] as the second index.</param>
        ''' <param name="style"></param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub RenderCell(g As Graphics,
                                        rc As RectangleF,
                                        layer As cEcospaceLayer,
                                        value As Object,
                                        style As cStyleGuide.eStyleFlags)

            If CBool(value) Then Me.RenderPreview(g, rc)

        End Sub

        Public Overrides Function GetDisplayText(value As Object) As String
            Return ""
        End Function

    End Class

End Namespace