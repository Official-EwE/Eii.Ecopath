' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    Public Class cDisplayLayerImage
        Inherits cDisplayLayer

        Public Sub New(uic As cUIContext, Optional img As Image = Nothing)
            MyBase.New(uic, New cImageLayerRenderer(uic, Nothing))
            Me.Image = img
            Me.m_editor = Nothing
        End Sub

        Public Overridable Property Image As Image = Nothing

        Public Property ImageTL As PointF = New PointF(0, 0)

        Public Property ImageBR As PointF = New PointF(0, 0)

        Public ReadOnly Property IsValid As Boolean
            Get
                If (Me.m_uic Is Nothing) Then Return False
                Return (Me.Image IsNot Nothing)
            End Get
        End Property

    End Class

End Namespace
