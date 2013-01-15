' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwECore

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of ports for fleets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorPorts
        Inherits cLayerEditorTwoState

#Region " Construction "

        Public Sub New()
            Me.New(GetType(ucLayerEditorPort))
        End Sub

        Public Sub New(ByVal t As Type)
            MyBase.New(t)
            Me.CellValue = 1
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the Ecopath fleet to filter by.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Fleet() As Integer
            Get
                Dim layer As cRasterLayerBundle = DirectCast(Me.Layer, cRasterLayerBundle)
                Return layer.iLayer
            End Get
            Set(ByVal value As Integer)
                Dim layer As cRasterLayerBundle = DirectCast(Me.Layer, cRasterLayerBundle)
                ' Will fleet index change?
                If value <> layer.iLayer Then
                    ' #Yes: update index in the underlying layer collector
                    layer.iLayer = value
                    ' Force map update
                    Me.Layer.Update(cLayer.eChangeFlags.Map, False)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Overridden to set coastal cells only
        ''' </summary>
        ''' <param name="ptSet"></param>
        ''' <param name="value"></param>
        ''' <param name="e"></param>
        ''' <param name="ptClick"></param>
        Protected Overrides Sub SetCellValue(ptSet As System.Drawing.Point, _
                                             value As Object, _
                                             e As System.Windows.Forms.MouseEventArgs, _
                                             ptClick As System.Drawing.Point)

            Dim core As cCore = Me.UIContext.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap
            Dim depth As cEcospaceLayerDepth = bm.LayerDepth

            If depth.IsCoastalCell(ptSet.Y, ptSet.X) Then
                MyBase.SetCellValue(ptSet, value, e, ptClick)
            End If

        End Sub

#End Region ' Public interfaces

    End Class

End Namespace