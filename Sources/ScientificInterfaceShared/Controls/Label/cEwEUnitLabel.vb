' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style
Imports ScientificInterfaceShared.Style

Namespace Controls

    Public Class cEwEUnitLabel
        Inherits Label
        Implements IUIElement

        Private m_strTextOrg As String = ""
        Private m_uic As cUIContext = Nothing

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property UIContext As cUIContext Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(uic As cUIContext)
                If (Me.m_uic IsNot Nothing) Then
                    RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
                End If
                Me.m_uic = uic
                If (Me.m_uic IsNot Nothing) Then
                    AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
                End If
            End Set
        End Property

        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)
            Me.UIContext = Nothing
        End Sub

        Public Overrides Property Text As String
            Get
                If Me.DesignMode Or Me.UIContext Is Nothing Then
                    If (Me.m_strTextOrg Is Nothing) Then Return Me.GetType().ToString
                    Return Me.m_strTextOrg
                End If
                Dim unit As New cUnits(Me.UIContext.Core)
                Return unit.ToString(Me.m_strTextOrg)
            End Get
            Set(strText As String)

                Me.m_strTextOrg = strText
                MyBase.Text = strText

            End Set
        End Property

        Private Sub OnStyleGuideChanged(changeType As cStyleGuide.eChangeType)
            Me.Invalidate()
        End Sub

    End Class

End Namespace
