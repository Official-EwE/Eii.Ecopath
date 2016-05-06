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
' Copyright 1991- UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls.Map.Layers

#End Region ' Imports

Namespace Controls.Map.Layers

    Public Class ucLayerEditorAdvection

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overloads Property Editor() As cLayerEditorAdvection
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorAdvection)
            End Get
            Set(ByVal editor As cLayerEditorAdvection)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorAdvection, "ucLayerEditorAdvection connected to wrong editor class")
                ' Set
                MyBase.Editor = editor
            End Set
        End Property

    End Class

End Namespace

