#Region " Imports "

Option Strict On
Imports WeifenLuo.WinFormsUI.Docking

#End Region ' Imports

Namespace Forms

    ''' =======================================================================
    ''' <summary>
    ''' <see cref="WeifenLuo.WinFormsUI.Docking.DockContent">DockContent</see>-derived
    ''' foundation class for EwE forms and panels, extending the Docking library by
    ''' adding a simple AutoHide toggle.
    ''' </summary>
    ''' =======================================================================
    Public Class frmEwEDockContent
        Inherits DockContent

        Private m_bAutoHide As Boolean = False

        Public Property AutoHide() As Boolean
            Get
                Return Me.m_bAutoHide
            End Get
            Set(ByVal value As Boolean)
                If (value <> Me.m_bAutoHide) Then
                    Me.m_bAutoHide = value
                    MyBase.DockState = Me.TranslateDockState(MyBase.DockState)
                End If
            End Set
        End Property

        Public Shadows Property DockState() As WeifenLuo.WinFormsUI.Docking.DockState
            Get
                Return MyBase.DockState
            End Get
            Set(ByVal value As WeifenLuo.WinFormsUI.Docking.DockState)
                MyBase.DockState = Me.TranslateDockState(value)
            End Set
        End Property

        Private Function TranslateDockState(ByVal state As WeifenLuo.WinFormsUI.Docking.DockState) As WeifenLuo.WinFormsUI.Docking.DockState
            Select Case state
                Case WeifenLuo.WinFormsUI.Docking.DockState.DockBottom
                    If Me.m_bAutoHide Then state = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
                Case WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
                    If Me.m_bAutoHide Then state = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom
                Case WeifenLuo.WinFormsUI.Docking.DockState.DockLeft
                    If Me.m_bAutoHide Then state = WeifenLuo.WinFormsUI.Docking.DockState.DockLeftAutoHide
                Case WeifenLuo.WinFormsUI.Docking.DockState.DockLeftAutoHide
                    If Me.m_bAutoHide Then state = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft
                Case WeifenLuo.WinFormsUI.Docking.DockState.DockRight
                    If Me.m_bAutoHide Then state = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
                Case WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
                    If Me.m_bAutoHide Then state = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
                Case WeifenLuo.WinFormsUI.Docking.DockState.DockTop
                    If Me.m_bAutoHide Then state = WeifenLuo.WinFormsUI.Docking.DockState.DockTopAutoHide
                Case WeifenLuo.WinFormsUI.Docking.DockState.DockTopAutoHide
                    If Me.m_bAutoHide Then state = WeifenLuo.WinFormsUI.Docking.DockState.DockTop
            End Select
            Return state
        End Function
    End Class

End Namespace
