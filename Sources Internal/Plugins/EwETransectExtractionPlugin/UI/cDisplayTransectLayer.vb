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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map.Layers

#End Region ' Imports

Public Class cDisplayLayerTransect
    Inherits cDisplayLayer

    Public Sub New(uic As cUIContext)
        MyBase.New(uic, New cTransectVectorRenderer(Nothing))
        Me.m_editor = New cTransectVectorEditor()
    End Sub

    Public Property Data As cTransectDatastructures = Nothing

    Public ReadOnly Property IsValid As Boolean
        Get
            If (Me.m_uic Is Nothing) Then Return False
            Return (Me.Data IsNot Nothing)
        End Get
    End Property

End Class

