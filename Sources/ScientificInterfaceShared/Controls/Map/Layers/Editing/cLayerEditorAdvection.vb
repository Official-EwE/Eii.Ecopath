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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports EwECore

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modification of Ecospace advection data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorAdvection
        Inherits cLayerEditorVector

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorRange))
        End Sub

#End Region ' Construction

        Public Overrides Sub Initialize(uic As cUIContext, layer As cDisplayRasterLayer)
            MyBase.Initialize(uic, layer)
            Me.CellValueMin = 0
            Me.CellValueMax = 1000
        End Sub

    End Class

End Namespace