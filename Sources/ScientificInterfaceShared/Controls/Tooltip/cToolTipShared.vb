' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Public accessible but shared tooltip instance for homogenous application
    ''' behaviour and styling. Yeah.
    ''' </summary>
    ''' =======================================================================
    Public Class cToolTipShared
        Inherits ToolTip

#Region " Privates "

        ''' <summary>Singleton instance.</summary>
        Private Shared __inst__ As cToolTipShared

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Singleton enforced constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub New()
            ' Yoho
        End Sub

#End Region ' Privates

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Zhe van einzterfeiz to get zhe tuhltipp.
        ''' </summary>
        ''' <returns>Zhe tuhltipp inschtanz.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetInstance() As cToolTipShared
            If cToolTipShared.__inst__ Is Nothing Then
                cToolTipShared.__inst__ = New cToolTipShared
                cToolTipShared.__inst__.Active = True
            End If
            Return cToolTipShared.__inst__
        End Function

#End Region ' Public interfaces

#Region " Doomed interfaces "

        <Obsolete("Please use ToolTip.SetToolTip instead")>
        Public Overloads Sub Show(text As String, wnd As IWin32Window)
            Debug.Assert(False)
        End Sub

        <Obsolete("Please use ToolTip.SetToolTip instead")>
        Public Overloads Sub Show(text As String, wnd As IWin32Window, iTimeout As Integer)
            Debug.Assert(False)
        End Sub

        <Obsolete("Please use ToolTip.SetToolTip instead")>
        Public Overloads Sub Show(text As String, wnd As IWin32Window, pt As System.Drawing.Point)
            Debug.Assert(False)
        End Sub

        <Obsolete("Please use ToolTip.SetToolTip instead")>
        Public Overloads Sub Show(text As String, wnd As IWin32Window, pt As System.Drawing.Point, iTimeout As Integer)
            Debug.Assert(False)
        End Sub

        <Obsolete("Please use ToolTip.SetToolTip instead")>
        Public Overloads Sub Show(text As String, wnd As IWin32Window, x As Integer, y As Integer)
            Debug.Assert(False)
        End Sub

        <Obsolete("Please use ToolTip.SetToolTip instead")>
        Public Overloads Sub Show(text As String, wnd As IWin32Window, x As Integer, y As Integer, iTimeout As Integer)
            Debug.Assert(False)
        End Sub

#End Region ' Doomed interfaces

    End Class

End Namespace ' Controls
