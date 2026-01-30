' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style



Namespace Controls.Map.Layers

    Public Class cStyleguideImageLayer
        Inherits cDisplayLayerImage

        Private m_bStyleGuideChanged As Boolean = False

        Public Sub New(uic As cUIContext)
            MyBase.New(uic)
            AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
            Me.UpdateImage()
        End Sub

        Protected Overrides Sub Dispose(bDisposing As Boolean)
            ' Stop listening
            RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
            ' Base class cleanup
            MyBase.Dispose(bDisposing)
        End Sub

        Public Overrides Property Image As Image
            Get
                Return Me.m_uic.StyleGuide.MapReferenceImage
            End Get
            Set(value As Image)
                ' NOP
            End Set
        End Property

        Private Sub UpdateImage()
            Try
                With Me.m_uic.StyleGuide
                    Me.Image = .MapReferenceImage
                    Me.ImageTL = .MapReferenceLayerTL
                    Me.ImageBR = .MapReferenceLayerBR
                End With
            Catch ex As Exception

            End Try
            Me.m_bStyleGuideChanged = False
            Me.Update(eChangeFlags.Map, False)
        End Sub

        Private Sub OnStyleGuideChanged(ct As cStyleGuide.eChangeType)
            If (ct And cStyleGuide.eChangeType.Map) > 0 Then
                Me.UpdateImage()
            End If
        End Sub
    End Class

End Namespace
