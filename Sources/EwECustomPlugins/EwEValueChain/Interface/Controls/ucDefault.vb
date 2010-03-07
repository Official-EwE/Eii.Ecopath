Option Strict On
Imports System.Windows.Forms
Imports EwEUtils.Database.cEwEDatabase
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

Public Class ucDefault
    Inherits UserControl
    Implements IUIElement

    Private m_bSelected As Boolean = False
    Private m_obj As cOOPStorable = Nothing
    Private m_uic As cUIContext = Nothing

    Public Property Selected() As Boolean
        Get
            Return Me.m_bSelected
        End Get
        Set(ByVal value As Boolean)
            Me.m_bSelected = value
            Me.Invalidate()
        End Set
    End Property

    Public Property ObjDefault() As cOOPStorable
        Get
            Return Me.m_obj
        End Get
        Set(ByVal value As cOOPStorable)
            Me.m_obj = value
            Me.Invalidate()
        End Set
    End Property

    Public Property UIContext() As cUIContext _
        Implements IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Set(ByVal value As cUIContext)
            If (Me.m_uic IsNot Nothing) Then
                RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            End If
            Me.m_uic = value
            If (Me.m_uic IsNot Nothing) Then
                AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            End If
        End Set
    End Property

    Protected ReadOnly Property StyleGuide() As cStyleGuide
        Get
            Return Me.m_uic.StyleGuide
        End Get
    End Property

    Protected Overridable Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
        ' NOP
    End Sub

End Class
