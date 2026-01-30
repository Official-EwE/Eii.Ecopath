' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports ScientificInterfaceShared.Style
Imports ValueChain



''' ===========================================================================
''' <summary>
''' Diagram element to wrap and reflect all existing links between two units in
''' the value chain.
''' </summary>
''' ===========================================================================
Public Class LinkWrapper

    Private Shared _iColorSeed_ As Integer = 0

    Private m_strName As String
    Private m_lLinks As New List(Of cLink)
    Private m_color As Color

    Public Sub New(link As cLink)
        Me.m_lLinks.Add(link)
        Me.m_color = Me.GetNextColor()
    End Sub

    Public Sub AddLink(link As cLink)
        ' Sanity checks
        Debug.Assert(Me.HasLink(link))
        Debug.Assert(Not Me.m_lLinks.Contains(link))

        Me.m_lLinks.Add(link)
    End Sub

    Public Sub RemoveLink(link As cLink)
        ' Sanity checks
        Debug.Assert(Me.HasLink(link))
        Debug.Assert(Me.m_lLinks.Contains(link))

        Me.m_lLinks.Remove(link)
    End Sub

    Public ReadOnly Property Links() As cLink()
        Get
            Return Me.m_lLinks.ToArray
        End Get
    End Property

    Public ReadOnly Property LinkCount() As Integer
        Get
            Return Me.m_lLinks.Count
        End Get
    End Property

    Public ReadOnly Property Color() As Color
        Get
            Return Me.m_color
        End Get
    End Property

    Public ReadOnly Property Source() As cUnit
        Get
            If Me.LinkCount = 0 Then Return Nothing
            Return Me.m_lLinks(0).Source
        End Get
    End Property

    Public ReadOnly Property Target() As cUnit
        Get
            If Me.LinkCount = 0 Then Return Nothing
            Return Me.m_lLinks(0).Target
        End Get
    End Property

    Public ReadOnly Property Width() As Single
        Get
            Dim w As Single = 0
            For Each l As cLink In Me.m_lLinks
                w = Math.Max(w, l.BiomassRatio)
            Next
            Return w
        End Get
    End Property

    Public ReadOnly Property External() As Boolean
        Get
            Dim bExt As Boolean = False
            For Each l As cLink In Me.m_lLinks
                bExt = bExt Or l.External
            Next
            Return bExt
        End Get
    End Property

    Public Function HasLink(obj As Object) As Boolean
        If TypeOf obj Is cLink Then
            Return ReferenceEquals(DirectCast(obj, cLink).Source, Me.Source) And
                   ReferenceEquals(DirectCast(obj, cLink).Target, Me.Target)
        End If
        Return False
    End Function

#Region " Internals "

    Private Function GetNextColor() As Color
        _iColorSeed_ = (_iColorSeed_ + 1) Mod (255 * 255 * 255 - 1)

        Dim iRed As Byte = CByte((_iColorSeed_ >> 16) And &HFF)
        Dim iGreen As Byte = CByte((_iColorSeed_ >> 8) And &HFF)
        Dim iBlue As Byte = CByte(_iColorSeed_ And &HFF)

        Return Color.FromArgb(255, iRed, iGreen, iBlue)
    End Function

#End Region ' Internals

End Class
