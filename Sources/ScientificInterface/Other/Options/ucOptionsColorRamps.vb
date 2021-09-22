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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwEUtils.SystemUtilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the interface to change value status feedback colors.
    ''' </summary>
    ''' <remarks>
    ''' Keeps a local copy of color ramps until the user Applies changes.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsColorRamps
        Implements IOptionsPage
        Implements IUIElement

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)

            Me.UIContext = uic
            Me.InitializeComponent()
            Me.DoubleBuffered = True

        End Sub

#End Region ' Constructors

#Region " Helper methods "

        Private m_bInUpdate As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to enable and update UI controls.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()

            Dim sg As cStyleGuide = Me.UIContext.StyleGuide
            Dim item As cColorRamp = DirectCast(Me.m_lbGradients.SelectedItem, cColorRamp)

            Dim bIsEditable As Boolean = False

            If (item IsNot Nothing) Then
                bIsEditable = (TypeOf item Is cARGBColorRamp) And (Not item.IsSystemRamp)
            End If

            Me.m_bInUpdate = True

            Me.m_tsbnAdd.Enabled = True
            Me.m_tsbnDelete.Enabled = bIsEditable
            Me.m_tsbnDuplicate.Enabled = bIsEditable

            Me.m_bInUpdate = False

        End Sub

#End Region ' Helper methods

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Control's load event which gets called every time the control gets loaded. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.InitUI(False)
        End Sub

#End Region ' Event handlers

#Region " Public methods "

        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext

        Public Function CanApply() As Boolean _
            Implements IOptionsPage.CanApply
            Return True
        End Function

        Public Event OnOptionsColorsChanged(sender As IOptionsPage, args As System.EventArgs) _
            Implements IOptionsPage.OnChanged

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save colour selections back to the style guide.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            Dim sg As cStyleGuide = Me.UIContext.StyleGuide

            ' Apply colors to the style guide
            sg.SuspendEvents()
            sg.ClearCustomColorRamps()

            For i As Integer = 0 To Me.m_lbGradients.Items.Count - 1
                Dim item As cColorRamp = DirectCast(Me.m_lbGradients.Items(i), cColorRamp)
                If (TypeOf item Is cARGBColorRamp) Then
                    Dim ramp As cARGBColorRamp = DirectCast(item, cARGBColorRamp)
                    sg.AddCustomColorRamp(ramp)
                End If
            Next

            sg.ResumeEvents()
            Return IOptionsPage.eApplyResultType.Success

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Reset all colours
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() _
            Implements IOptionsPage.SetDefaults
            Me.InitUI(True)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanSetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Function CanSetDefaults() As Boolean _
            Implements IOptionsPage.CanSetDefaults
            Return False
        End Function

#End Region ' Public methods

#Region " Events "

        Private Sub OnColorRampSelected(sender As Object, e As EventArgs) Handles m_lbGradients.SelectedIndexChanged

            Dim item As Object = Me.m_lbGradients.SelectedItem
            If (TypeOf item Is cColorRamp) Then
                Me.m_editor.ColorRamp = DirectCast(item, cColorRamp)
            Else
                Me.m_editor.ColorRamp = Nothing
            End If
            Me.UpdateControls()

        End Sub

        Private Sub OnAdd(sender As Object, e As EventArgs) Handles m_tsbnAdd.Click

            ' ToDo: globalize this
            ' ToDo: use new item name numbering code
            Me.m_lbGradients.SelectedIndex = Me.m_lbGradients.Items.Add(New cARGBColorRamp("New gradient", New Color() {Color.LightBlue, Color.DarkBlue}, New Double() {0, 1}))

        End Sub

        Private Sub OnDuplicate(sender As Object, e As EventArgs) Handles m_tsbnDuplicate.Click

            Dim item As cColorRamp = DirectCast(Me.m_lbGradients.SelectedItem, cColorRamp)
            Debug.Assert(TypeOf item Is cARGBColorRamp)
            Me.m_lbGradients.SelectedIndex = Me.m_lbGradients.Items.Add(New cARGBColorRamp(DirectCast(item, cARGBColorRamp)))

        End Sub

        Private Sub OnDelete(sender As Object, e As EventArgs) Handles m_tsbnDelete.Click

            Dim iIndex As Integer = Me.m_lbGradients.SelectedIndex
            Debug.Assert(iIndex >= 0)
            Me.m_lbGradients.Items.RemoveAt(iIndex)
            Me.m_lbGradients.SelectedIndex = Math.Min(Me.m_lbGradients.Items.Count - 1, iIndex)

        End Sub

        Private Sub OnColorRampEdited(sender As Object, args As cColorRamp) Handles m_editor.OnColorRampChanged
            Me.m_lbGradients.Invalidate()
        End Sub

        Private Sub OnDrawColorRamp(sender As Object, e As DrawItemEventArgs) Handles m_lbGradients.DrawItem

            Dim item As Object = Me.m_lbGradients.Items(e.Index)
            e.DrawBackground()

            If (TypeOf item Is cColorRamp) Then
                Dim rc As Rectangle = e.Bounds
                rc.X += 1 : rc.Y += 1 : rc.Width -= 2 : rc.Height -= 2
                cColorRampIndicator.DrawColorRamp(e.Graphics, DirectCast(item, cColorRamp), rc)
            End If
            e.DrawFocusRectangle()

        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub InitUI(bDefault As Boolean)
            Me.m_lbGradients.Items.Clear()
            If bDefault Then
                Me.m_lbGradients.Items.AddRange(Me.UIContext.StyleGuide.DefaultColorRamps)
            Else
                Me.m_lbGradients.Items.AddRange(Me.UIContext.StyleGuide.ColorRamps)
            End If
            Me.m_lbGradients.SelectedIndex = 0
        End Sub

#End Region ' Internals

    End Class

End Namespace


