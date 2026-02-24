' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map

    Public Class cMapDrawerArgs

        Private m_maptype As cMapDrawerBase.eMapType
        Private m_relscaler() As Single
        Private m_sMaxLegendF As Single

        Public Sub New(maptype As cMapDrawerBase.eMapType,
                       theRelScaler() As Single,
                       MaxLegendF As Single)

            Dim data As Single() = Nothing

            If (theRelScaler IsNot Nothing) Then
                ReDim data(theRelScaler.Length)
                theRelScaler.CopyTo(data, 0)
            End If

            Me.m_maptype = maptype
            Me.m_relscaler = data
            Me.m_sMaxLegendF = MaxLegendF
        End Sub

        Public ReadOnly Property MapType As cMapDrawerBase.eMapType
            Get
                Return Me.m_maptype
            End Get
        End Property

        Public ReadOnly Property RelMapScaler As Single()
            Get
                Return Me.m_relscaler
            End Get
        End Property

        Public ReadOnly Property FishingMortLegendMax As Single
            Get
                Return Me.m_sMaxLegendF
            End Get
        End Property

    End Class

End Namespace
