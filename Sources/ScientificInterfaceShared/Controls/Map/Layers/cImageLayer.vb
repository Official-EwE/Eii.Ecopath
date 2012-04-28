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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.Map.Layers

    Public Class cImageLayer
        Inherits cLayer

        Private m_img As Image = Nothing
        Private m_bImageUpdated As Boolean = False

        Public Sub New(uic As cUIContext)
            MyBase.New(uic, New cImageLayerRenderer(Nothing))
            AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
        End Sub

        Protected Overrides Sub Dispose(bDisposing As Boolean)
            If (Me.m_img IsNot Nothing) Then
                Me.m_img.Dispose()
                Me.m_img = Nothing
            End If
            RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            MyBase.Dispose(bDisposing)
        End Sub

        Public ReadOnly Property Image As Image
            Get
                If (Not m_bImageUpdated) Then Me.UpdateImage()
                Return Me.m_img
            End Get
        End Property

        Public ReadOnly Property ImageTL As PointF
            Get
                Return Me.m_uic.StyleGuide.MapReferenceLayerTL
            End Get
        End Property

        Public ReadOnly Property ImageBR As PointF
            Get
                Return Me.m_uic.StyleGuide.MapReferenceLayerBR
            End Get
        End Property

        Public ReadOnly Property IsValid As Boolean
            Get
                If (Me.m_uic Is Nothing) Then Return False
                Return not String.IsNullOrWhiteSpace(Me.m_uic.StyleGuide.MapReferenceLayerFile)
            End Get
        End Property

        Private Sub UpdateImage()
            Try
                Me.m_img = Image.FromFile(Me.m_uic.StyleGuide.MapReferenceLayerFile)
            Catch ex As Exception

            End Try
            Me.m_bImageUpdated = True
        End Sub

        Private Sub OnStyleGuideChanged(ct As cStyleGuide.eChangeType)
            If (ct And cStyleGuide.eChangeType.Map) > 0 Then
                Me.m_img.Dispose()
                Me.m_img = Nothing
                Me.m_bImageUpdated = False
                Me.Update(eChangeFlags.Map, False)
            End If
        End Sub

    End Class

End Namespace
