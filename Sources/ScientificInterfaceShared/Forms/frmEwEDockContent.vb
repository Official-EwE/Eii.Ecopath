#Region " Imports "

Option Strict On
Imports WeifenLuo.WinFormsUI.Docking

#End Region ' Imports

Namespace Forms

    ''' =======================================================================
    ''' <summary>
    ''' <see cref="DockContent">DockContent</see>-derived
    ''' foundation class for EwE forms and panels, extending the Docking library by
    ''' adding a simple AutoHide toggle.
    ''' </summary>
    ''' =======================================================================
    Public Class frmEwEDockContent
        Inherits DockContent

        Public Enum ePanelType As Byte
            Document = 0
            SystemPanel = 1
        End Enum

        Public Property AutoHide() As Boolean
            Get
                Return Me.IsHiding()
            End Get
            Set(ByVal value As Boolean)
                MyBase.DockState = Me.TranslateDockState(MyBase.DockState, value)
            End Set
        End Property

        Public Shadows Property DockState() As DockState
            Get
                Return MyBase.DockState
            End Get
            Set(ByVal value As DockState)
                MyBase.DockState = Me.TranslateDockState(value, Me.IsHiding)
            End Set
        End Property

        Private Function TranslateDockState(ByVal state As DockState, ByVal bHide As Boolean) As DockState
            Select Case state
                Case DockState.DockBottom
                    If bHide Then state = DockState.DockBottomAutoHide
                Case DockState.DockBottomAutoHide
                    If bHide Then state = DockState.DockBottom
                Case DockState.DockLeft
                    If bHide Then state = DockState.DockLeftAutoHide
                Case DockState.DockLeftAutoHide
                    If bHide Then state = DockState.DockLeft
                Case DockState.DockRight
                    If bHide Then state = DockState.DockRightAutoHide
                Case DockState.DockRightAutoHide
                    If bHide Then state = DockState.DockRight
                Case DockState.DockTop
                    If bHide Then state = DockState.DockTopAutoHide
                Case DockState.DockTopAutoHide
                    If bHide Then state = DockState.DockTop
            End Select
            Return state
        End Function

        Public Function IsHiding() As Boolean
            Return (Me.DockState = DockState.DockTopAutoHide) Or _
                   (Me.DockState = DockState.DockBottomAutoHide) Or _
                   (Me.DockState = DockState.DockLeftAutoHide) Or _
                   (Me.DockState = DockState.DockRightAutoHide)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States the type of content taht this form displays.
        ''' </summary>
        ''' <returns>Returns <see cref="ePanelType.Document"/> by default.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function PanelType() As ePanelType
            Return ePanelType.Document
        End Function

    End Class

End Namespace
