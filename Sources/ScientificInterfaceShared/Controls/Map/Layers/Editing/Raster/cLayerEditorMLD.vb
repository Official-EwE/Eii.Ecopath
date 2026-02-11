' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor base class that supports manual modification of Ecospace 
    ''' layers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorMLD
        Inherits cLayerEditorRange

#Region " Private vars "

        ''' <summary>The depth layer to limit MLD values against.</summary>
        Private m_layerDepth As cEcospaceLayer = Nothing

#End Region ' Private vars

#Region " Construction "

        Public Sub New()
            MyBase.New()
        End Sub

#End Region ' Construction

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.Initialize"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Initialize(uic As cUIContext, layer As cDisplayLayer)
            MyBase.Initialize(uic, layer)

            Dim bm As cEcospaceBasemap = uic.Core.EcospaceBasemap
            If (bm IsNot Nothing) Then
                Me.m_layerDepth = bm.LayerDepth
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set a cell value. Overridden to limit values to the actual map depths.
        ''' </summary>
        ''' <param name="ptSet"></param>
        ''' <param name="value"></param>
        ''' <param name="e"></param>
        ''' <param name="ptClick"></param>
        ''' -------------------------------------------------------------------
        Protected Overrides Function SetCellValue(ptSet As System.Drawing.Point,
                                                  value As Object,
                                                  e As System.Windows.Forms.MouseEventArgs,
                                                  ptClick As System.Drawing.Point) As Boolean
            ' Sanity checks
            Debug.Assert(Me.m_layerDepth IsNot Nothing)

            Return MyBase.SetCellValue(ptSet, Math.Min(CSng(value), CSng(Me.m_layerDepth.Cell(ptSet.Y, ptSet.X))), e, ptClick)

        End Function

#End Region ' Overrides

    End Class

End Namespace
