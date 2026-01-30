' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwEUtils.SystemUtilities



Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A <see cref="TreeView"/>-inherited user control that uses the Windows 7 visual display style.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cThemedTreeView
        Inherits TreeView

        Private m_bShowImages As Boolean = True
        Private m_il As ImageList = Nothing

        Public Sub New()
            Me.ShowImages = True
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' And here we thought to be rid of P/invoke!
        ''' </summary>
        ''' <param name="hWnd"></param>
        ''' <param name="pszSubAppName"></param>
        ''' <param name="pszSubIdList"></param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Declare Unicode Function SetWindowTheme Lib "uxtheme.dll" (hWnd As IntPtr, pszSubAppName As String, pszSubIdList As String) As Integer

        Protected Overrides Sub CreateHandle()
            MyBase.CreateHandle()
            If cSystemUtils.IsWindows And cSystemUtils.IsRunningWin7OrHigher And Not cSystemUtils.Is64BitProcess Then
                Try
                    SetWindowTheme(Me.Handle, "explorer", Nothing)
                Catch ex As Exception
                    ' Whoah!
                End Try
            End If
        End Sub

        Private Const WM_ERASEBKGND As Integer = &H14
        Private Const WM_LBUTTONDOWN As Integer = &H201
        Private Const WM_LBUTTONDBLCLK As Integer = &H203

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overriden to reduce flicker when redrawing.
        ''' </summary>
        ''' <param name="msg"></param>
        ''' <remarks>
        ''' http://forums.codeguru.com/showthread.php?182326-TreeView-Flickering-Problem
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub WndProc(ByRef msg As System.Windows.Forms.Message)

            If (msg.Msg = WM_ERASEBKGND) Then
                ' reduce flicker when redrawing.
                msg.Result = IntPtr.Zero
                Return
            End If

            If (msg.Msg = WM_LBUTTONDBLCLK And Me.CheckBoxes) Then
                ' Fix double-click on checkbox issue
                ' https://stackoverflow.com/questions/14647216/c-sharp-treeview-ignore-double-click-only-at-checkbox
                Dim localPos As Point = Me.PointToClient(Cursor.Position)
                Dim info As TreeViewHitTestInfo = Me.HitTest(localPos)
                If (info.Location = TreeViewHitTestLocations.StateImage) Then
                    msg.Msg = WM_LBUTTONDOWN
                End If
            End If
            MyBase.WndProc(msg)

        End Sub

        <Category("Appearance")>
        <Browsable(True)>
        Public Property ShowImages As Boolean
            Get
                Return Me.m_bShowImages
            End Get
            Set(value As Boolean)
                Me.m_bShowImages = value
                Me.UpdateImageVisibility()
            End Set
        End Property

        Public Overloads Property ImageList As ImageList
            Get
                Return Me.m_il
            End Get
            Set(value As ImageList)
                Me.m_il = value
                Me.UpdateImageVisibility()
            End Set
        End Property

        Private Sub UpdateImageVisibility()

            If (Me.m_bShowImages) And (Me.m_il IsNot Nothing) Then
                MyBase.ImageList = Me.m_il
            Else
                MyBase.ImageList = Nothing
            End If
            Me.Invalidate()

        End Sub

    End Class

End Namespace
