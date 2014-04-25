' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.ComponentModel

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Numeric up/down derived control to improve editing.
    ''' </summary>
    ''' <remarks>
    ''' http://www.codeproject.com/articles/30899/extended-numericupdown-control
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cEwENumericUpDown
        Inherits NumericUpDown
#If 0 Then

        Implements ISupportInitialize

#Region " Private vars "

        ''' <summary>Most recent mouse position while dragging.</summary>
        Private m_ptLast As System.Drawing.Point = Nothing

        ''' <summary>Reference to the underlying TextBox control.</summary>
        Private m_textbox As TextBox = Nothing
        ''' <summary>Reference to the underlying UpDownButtons control.</summary>
        Private m_btnsUpDown As Control = Nothing

        Private m_interceptMouseWheel As InterceptMouseWheelMode = InterceptMouseWheelMode.Always
        Private m_bAutoSelect As Boolean = False
        Private m_bShowUpDownButtons As ShowUpDownButtonsMode = ShowUpDownButtonsMode.Always
        Private m_bWrapValue As Boolean = False

        ''' <summary>Flag to track mouse position.</summary>
        Private m_bMouseOver As Boolean = False
        Private m_bReady As Boolean = False

#End Region ' Private vars

#Region " Control overrides "

#If 0 Then

        ''' <summary>
        ''' Mouse press override. Used to set the capture for possible dragging.
        ''' </summary>
        Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
            ' Use control unaffected
            MyBase.OnMouseDown(e)
            Me.Capture = True
            Me.m_ptLast = Me.DistanceFromBounds(e.Location)
        End Sub

        ''' <summary>
        ''' Mouse move override. Changes the value of the control while dragging.
        ''' </summary>
        Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)

            If Me.Capture And Not Me.ClientRectangle.Contains(e.Location) Then

                Dim ptCurr As Point = Me.DistanceFromBounds(e.Location)
                Dim dx As Integer = (ptCurr.X - Me.m_ptLast.X)
                Dim dy As Integer = (Me.m_ptLast.Y - ptCurr.Y)
                Dim sIncrement As Single = Me.Increment

                If My.Computer.Keyboard.CtrlKeyDown Then
                    sIncrement *= 10
                End If

                If My.Computer.Keyboard.ShiftKeyDown Then
                    sIncrement /= CSng(Math.Max(Math.Pow(10, Me.DecimalPlaces), 5))
                End If

                Dim sDist As Single = (dx + dy) * (dx + dy) * sIncrement * CSng(Math.Sign(dx + dy))
                Me.Value = Convert.ToDecimal(Math.Max(Me.Minimum, Math.Min(Me.Maximum, Me.Value + sDist)))

                ' Remember last point
                Me.m_ptLast = ptCurr
            Else
                MyBase.OnMouseMove(e)
            End If
        End Sub

        ''' <summary>
        ''' Mouse up override. Used to cancel mouse capture.
        ''' </summary>
        Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
            Me.Capture = False
            'Me.Cursor = Cursors.Default
            MyBase.OnMouseUp(e)
        End Sub

        Protected Function DistanceFromBounds(ByVal pt As Point) As Point
            Dim rc As Rectangle = Me.ClientRectangle
            Dim dx As Integer
            Dim dy As Integer

            If pt.X < 0 Then
                dx = pt.X
            ElseIf pt.X <= rc.Width Then
                dx = 0
            Else
                dx = pt.X - rc.Width
            End If

            If pt.Y < 0 Then
                dy = pt.Y
            ElseIf pt.Y <= rc.Height Then
                dy = 0
            Else
                dy = pt.Y - rc.Height
            End If
            Return New Point(dx, dy)

        End Function

#End If

#End Region ' Control overrides

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            MyBase.New()

            ' Extract a reference to the underlying TextBox field
            Me.m_textbox = GetPrivateField(Of TextBox)(Me, "upDownEdit")
            Debug.Assert(Me.m_textbox IsNot Nothing, "Can't find internal TextBox field.")

            ' Extract a reference to the underlying UpDownButtons field
            Me.m_btnsUpDown = GetPrivateField(Of Control)(Me, "upDownButtons")
            Debug.Assert(Me.m_btnsUpDown IsNot Nothing, "Can't find internal UpDown buttons field.")

            Me.m_textbox.AcceptsReturn = False

            ' Add handlers (MouseEnter and MouseLeave events of NumericUpDown are not working properly)
            AddHandler m_textbox.MouseEnter, AddressOf OnMouseEnterLeave
            AddHandler m_textbox.MouseLeave, AddressOf OnMouseEnterLeave
            AddHandler m_btnsUpDown.MouseEnter, AddressOf OnMouseEnterLeave
            AddHandler m_btnsUpDown.MouseLeave, AddressOf OnMouseEnterLeave
            AddHandler MyBase.MouseEnter, AddressOf OnMouseEnterLeave
            AddHandler MyBase.MouseLeave, AddressOf OnMouseEnterLeave
            'AddHandler Me.m_textbox.TextChanged, AddressOf OnTextBoxTextChanged

        End Sub

#Region " Internals "

        Protected Overrides Sub Dispose(disposing As Boolean)
            RemoveHandler m_textbox.MouseEnter, AddressOf OnMouseEnterLeave
            RemoveHandler m_textbox.MouseLeave, AddressOf OnMouseEnterLeave
            RemoveHandler m_btnsUpDown.MouseEnter, AddressOf OnMouseEnterLeave
            RemoveHandler m_btnsUpDown.MouseLeave, AddressOf OnMouseEnterLeave
            RemoveHandler MyBase.MouseEnter, AddressOf OnMouseEnterLeave
            RemoveHandler MyBase.MouseLeave, AddressOf OnMouseEnterLeave
            'RemoveHandler Me.m_textbox.TextChanged, AddressOf OnTextBoxTextChanged
            MyBase.Dispose(disposing)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extracts a reference to a private underlying field
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Friend Shared Function GetPrivateField(Of T As Control) _
                (ByVal ctrl As cEwENumericUpDown, ByVal fieldName As String) As T
            ' find internal TextBox
            Dim fi As Reflection.FieldInfo _
                = GetType(NumericUpDown).GetField(fieldName, _
                            Reflection.BindingFlags.FlattenHierarchy _
                            Or Reflection.BindingFlags.NonPublic _
                            Or Reflection.BindingFlags.Instance)
            ' take some caution... they could change field name in the future!
            If fi Is Nothing Then
                Return Nothing
            Else
                Return TryCast(fi.GetValue(ctrl), T)
            End If
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="e"></param>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            If Me.m_btnsUpDown.Visible = False Then
                e.Graphics.Clear(Me.BackColor)
            End If
            MyBase.OnPaint(e)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' WndProc override to kill WN_MOUSEWHEEL message
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            Const WM_MOUSEWHEEL As Integer = &H20A

            If m.Msg = WM_MOUSEWHEEL Then
                Select Case m_interceptMouseWheel
                    Case InterceptMouseWheelMode.Always
                        ' standard message
                        MyBase.WndProc(m)
                    Case InterceptMouseWheelMode.WhenMouseOver
                        If m_bMouseOver Then
                            ' standard message
                            MyBase.WndProc(m)
                        End If
                    Case InterceptMouseWheelMode.Never
                        ' kill the message
                        Exit Sub
                End Select
            Else
                MyBase.WndProc(m)
            End If

        End Sub

#End Region ' Internals

#Region " New properties "

        <DefaultValue(False)> _
        <Category("Behavior")> _
        <Description("Automatically select control text when it receives focus.")> _
        Public Property AutoSelect() As Boolean
            Get
                Return Me.m_bAutoSelect
            End Get
            Set(ByVal value As Boolean)
                Me.m_bAutoSelect = value
            End Set
        End Property


        <Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property SelectionStart() As Integer
            Get
                Return Me.m_textbox.SelectionStart
            End Get
            Set(ByVal value As Integer)
                Me.m_textbox.SelectionStart = value
            End Set
        End Property


        <Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property SelectionLength() As Integer
            Get
                Return Me.m_textbox.SelectionLength
            End Get
            Set(ByVal value As Integer)
                Me.m_textbox.SelectionLength = value
            End Set
        End Property


        <Browsable(False)> _
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property SelectedText() As String
            Get
                Return Me.m_textbox.SelectedText
            End Get
            Set(ByVal value As String)
                Me.m_textbox.SelectedText = value
            End Set
        End Property


        <DefaultValue(GetType(InterceptMouseWheelMode), "Always")> _
        <Category("Behavior")> _
        <Description("Enables MouseWheel only under certain conditions.")> _
        Public Property InterceptMouseWheel() As InterceptMouseWheelMode
            Get
                Return Me.m_interceptMouseWheel
            End Get
            Set(ByVal value As InterceptMouseWheelMode)
                Me.m_interceptMouseWheel = value
            End Set
        End Property


        Public Enum InterceptMouseWheelMode
            ''' <summary>MouseWheel always works (defauld behavior)</summary>
            Always
            ''' <summary>MouseWheel works only when mouse is over the (focused) control</summary>
            WhenMouseOver
            ''' <summary>MouseWheel never works</summary>
            Never
        End Enum


        <DefaultValue(GetType(ShowUpDownButtonsMode), "Always")> _
        <Category("Behavior")> _
        <Description("Set UpDownButtons visibility mode.")> _
        Public Property ShowUpDownButtons() As ShowUpDownButtonsMode
            Get
                Return Me.m_bShowUpDownButtons
            End Get
            Set(ByVal value As ShowUpDownButtonsMode)
                Me.m_bShowUpDownButtons = value
                ' update UpDownButtons visibility
                UpdateUpDownButtonsVisibility()
            End Set
        End Property


        Public Enum ShowUpDownButtonsMode
            ''' <summary>UpDownButtons are always visible (defauld behavior)</summary>
            Always
            ''' <summary>UpDownButtons are visible only when mouse is over the control</summary>
            WhenMouseOver
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' If set, incrementing value will cause it to restart from Minimum 
        ''' when Maximum is reached (and viceversa).
        ''' </summary>
        ''' -------------------------------------------------------------------
        <DefaultValue(False)> _
        <Category("Behavior")> _
        <Description("If set, incrementing value will cause it to restart from Minimum when Maximum is reached (and viceversa).")> _
        Public Property WrapValue() As Boolean
            Get
                Return Me.m_bWrapValue
            End Get
            Set(ByVal value As Boolean)
                Me.m_bWrapValue = value
            End Set
        End Property

#End Region ' New properties

#Region " Text selection "

        ' select all the text on focus enter
        Protected Overrides Sub OnGotFocus(ByVal e As System.EventArgs)
            If Me.m_bAutoSelect Then
                Me.m_textbox.SelectAll()
            End If
            MyBase.OnGotFocus(e)
        End Sub


        ' MouseUp will kill the SelectAll made on GotFocus.
        ' Will restore it, but only if user have not made a partial text selection.
        Protected Overrides Sub OnMouseUp(ByVal mevent As System.Windows.Forms.MouseEventArgs)
            If Me.m_bAutoSelect And (Me.m_textbox.SelectionLength = 0) Then
                Me.m_textbox.SelectAll()
            End If
            MyBase.OnMouseUp(mevent)
        End Sub

        Protected Overrides Sub OnKeyUp(e As System.Windows.Forms.KeyEventArgs)
            MyBase.OnKeyUp(e)
            Me.ValidateEditText()
        End Sub

#End Region

#Region " Additional events "

        ' these events will be raised correctly, when mouse enters on the textbox
        Shadows Event MouseEnter As EventHandler(Of EventArgs)
        Shadows Event MouseLeave As EventHandler(Of EventArgs)

        ' Events raised BEFORE value decrement/increment
        Public Event BeforeValueDecrement As CancelEventHandler
        Public Event BeforeValueIncrement As CancelEventHandler

        ' this handler is called at each mouse Enter/Leave movement
        Private Sub OnMouseEnterLeave(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim cr As Drawing.Rectangle = RectangleToScreen(ClientRectangle)
            Dim mp As Drawing.Point = MousePosition

            ' actual state
            Dim isOver As Boolean = cr.Contains(mp)

            ' test if status changed
            If m_bMouseOver Xor isOver Then
                ' update state
                m_bMouseOver = isOver
                If m_bMouseOver Then
                    RaiseEvent MouseEnter(Me, EventArgs.Empty)
                Else
                    RaiseEvent MouseLeave(Me, EventArgs.Empty)
                End If
            End If

            ' update UpDownButtons visibility
            If m_bShowUpDownButtons <> ShowUpDownButtonsMode.Always Then
                UpdateUpDownButtonsVisibility()
            End If

        End Sub

#End Region

#Region " Value increment/decrement management "

        ' raises the two new events
        Public Overrides Sub DownButton()
            Dim e As New CancelEventArgs
            RaiseEvent BeforeValueDecrement(Me, e)
            If e.Cancel Then Exit Sub
            ' decrement with wrap
            If m_bWrapValue AndAlso Value - Increment < Minimum Then
                Value = Maximum
            Else
                MyBase.DownButton()
            End If
        End Sub

        Public Overrides Sub UpButton()
            Dim e As New CancelEventArgs
            RaiseEvent BeforeValueIncrement(Me, e)
            If e.Cancel Then Exit Sub
            ' increment with wrap
            If m_bWrapValue AndAlso Value + Increment > Maximum Then
                Value = Minimum
            Else
                MyBase.UpButton()
            End If
        End Sub

#End Region

#Region " UpDownButtons visibility management "

        ''' <summary>
        ''' Show or hide the UpDownButtons, according to ShowUpDownButtons property value
        ''' </summary>
        Sub UpdateUpDownButtonsVisibility()

            ' test new state
            Dim newVisible As Boolean
            Select Case m_bShowUpDownButtons
                Case ShowUpDownButtonsMode.WhenMouseOver
                    newVisible = m_bMouseOver
                Case Else
                    newVisible = True
            End Select

            ' assign only if needed
            If m_btnsUpDown.Visible <> newVisible Then
                If newVisible Then
                    Me.m_textbox.Width = Me.ClientRectangle.Width - m_btnsUpDown.Width
                Else
                    Me.m_textbox.Width = Me.ClientRectangle.Width
                End If
                Me.m_btnsUpDown.Visible = newVisible
                OnTextBoxResize(m_textbox, EventArgs.Empty)
                Me.Invalidate()
            End If

        End Sub


        ''' <summary>
        ''' Custom textbox size management
        ''' </summary>
        Protected Overrides Sub OnTextBoxResize(ByVal source As Object, ByVal e As System.EventArgs)
            If m_textbox Is Nothing Then Exit Sub
            If m_bShowUpDownButtons = ShowUpDownButtonsMode.Always Then
                ' standard management
                MyBase.OnTextBoxResize(source, e)
            Else
                ' custom management

                ' change position if RTL
                Dim bFixPos As Boolean = Me.RightToLeft = Windows.Forms.RightToLeft.Yes _
                                         Xor Me.UpDownAlign = LeftRightAlignment.Left

                If m_bMouseOver Then
                    Me.m_textbox.Width = Me.ClientSize.Width - Me.m_textbox.Left - Me.m_btnsUpDown.Width - 2
                    If bFixPos Then Me.m_textbox.Location = New Point(16, Me.m_textbox.Location.Y)
                Else
                    If bFixPos Then Me.m_textbox.Location = New Point(2, Me.m_textbox.Location.Y)
                    Me.m_textbox.Width = Me.ClientSize.Width - Me.m_textbox.Left - 2
                End If

            End If

        End Sub

#End Region
#End If

    End Class

End Namespace
