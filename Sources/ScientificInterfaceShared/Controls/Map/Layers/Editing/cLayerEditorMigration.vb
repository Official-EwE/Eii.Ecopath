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
Imports EwECore
Imports ScientificInterfaceShared.Controls.Map.Layers

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modification of Ecospace migration data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorMigration
        Inherits cLayerEditor

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorMigration))
            Me.CellValue = 1
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the Ecopath group whose migration data
        ''' is being edited.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Group() As Integer
            Get
                Dim layerCore As cDisplayRasterLayerBundle = DirectCast(Me.Layer, cDisplayRasterLayerBundle)
                Return layerCore.iLayer
            End Get
            Set(ByVal value As Integer)
                Dim layerCore As cDisplayRasterLayerBundle = DirectCast(Me.Layer, cDisplayRasterLayerBundle)
                ' Will Group index change?
                If value <> layerCore.iLayer Then
                    ' #Yes: update Group index in the underlying Ecospace layer
                    layerCore.iLayer = value
                    ' Force map update
                    Me.Layer.Update(cDisplayLayer.eChangeFlags.Map Or cDisplayLayer.eChangeFlags.Selected, False)
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' This editor requires a 1 pt cursor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property CursorSize As Integer
            Get
                Return 1
            End Get
            Set(value As Integer)
                'NOP
            End Set
        End Property

        Protected Overrides Sub SetCellValue(ptSet As System.Drawing.Point, _
                                             value As Object, _
                                             e As System.Windows.Forms.MouseEventArgs, _
                                             ptClick As System.Drawing.Point)
            Dim layerCore As cDisplayRasterLayerBundle = DirectCast(Me.Layer, cDisplayRasterLayerBundle)
            Dim grp As cEcospaceGroup = Me.UIContext.Core.EcospaceGroups(layerCore.iLayer)
            grp.IsMigratory = True
            MyBase.SetCellValue(ptSet, value, e, ptClick)
        End Sub

#End Region ' Public interfaces

    End Class

End Namespace