Option Strict On
Imports System.Windows.Forms
Imports EwEUtils.Database.cEwEDatabase

Public Class ucDefault
    Inherits UserControl

    Private m_bSelected As Boolean = False
    Private m_obj As cOOPStorable = Nothing

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

End Class
