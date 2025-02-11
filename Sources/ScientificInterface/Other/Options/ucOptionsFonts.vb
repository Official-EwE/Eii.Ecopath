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

Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Graph settings interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsFonts
        Implements IOptionsPage
        Implements IUIElement

#Region " Helper classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class, represents a font type and its current settings
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class cFontTypeItem

            Public Sub New(ByVal strText As String,
                           ByVal ft As cStyleGuide.eApplicationFontType,
                           ByVal sg As cStyleGuide,
                           ByVal bDefault As Boolean)

                Dim astrBits As String() = strText.Split("|"c)
                Me.Name = astrBits(0)
                If astrBits.Length = 2 Then
                    Me.Description = astrBits(1)
                Else
                    Me.Description = ""
                End If

                Me.FontType = ft
                If (bDefault) Then
                    sg.FontFamilyName(ft) = ""
                    sg.FontStyle(ft) = CType(cCore.NULL_VALUE, Drawing.FontStyle)
                    sg.FontSize(ft) = cCore.NULL_VALUE
                End If
                Me.FontFamilyName = sg.FontFamilyName(ft)
                Me.FontStyle = sg.FontStyle(ft)
                Me.FontSize = sg.FontSize(ft)
            End Sub

            Public ReadOnly Property Name() As String = ""

            Public ReadOnly Property Description() As String = ""

            Public ReadOnly Property FontType() As cStyleGuide.eApplicationFontType = cStyleGuide.eApplicationFontType.NotSet

            Public Overrides Function ToString() As String
                Return Me.Name
            End Function

            Public Property FontFamilyName() As String

            Public Property FontStyle() As FontStyle = FontStyle.Regular

            Public Property FontSize() As Single = 8.25

        End Class

        Private Class cFontFamilyItem
            Public Sub New(ByVal family As FontFamily)
                Me.Family = family
            End Sub

            Public ReadOnly Property Family() As FontFamily = Nothing

            Public Overrides Function ToString() As String
                Return Me.Family.Name
            End Function

        End Class

#End Region ' Helper classes

#Region " Variables "

        ''' <summary>Prevent loops.</summary>
        Private m_bInUpdate As Boolean = False

#End Region ' Variables

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)

            Me.InitializeComponent()
            Me.UIContext = uic


        End Sub

#End Region ' Constructors

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Init me!
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            ' Invisible init
            Me.FillFontFamiliesComboBox()
            Me.FillFontTypesListBox(False)

            MyBase.OnLoad(e)

        End Sub

        Private Sub lbItems_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_lbFontTypes.SelectedIndexChanged

            If Me.m_lbFontTypes.SelectedIndex <> -1 Then

                Dim fti As cFontTypeItem = Me.SelectedFontType
                Me.m_bInUpdate = True
                Me.SelectedFontFamilyName = fti.FontFamilyName
                Me.SelectedFontStyle = fti.FontStyle
                Me.SelectedFontSize = fti.FontSize
                Me.m_lblDescription.Text = fti.Description
                Me.m_bInUpdate = False

            End If
            Me.UpdatePreview()

        End Sub

        Private Sub m_cbFontFamily_DrawItem(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) _
            Handles m_cbFontFamily.DrawItem

            e.DrawBackground()
            e.DrawFocusRectangle()

            If e.Index = -1 Then Return

            Dim ffi As cFontFamilyItem = DirectCast(Me.m_cbFontFamily.Items(e.Index), cFontFamilyItem)
            Dim g As Graphics = e.Graphics

            Using f As New Font(ffi.Family, 8.25, FontStyle.Regular, GraphicsUnit.Point)
                Using br As New SolidBrush(e.ForeColor)
                    e.Graphics.DrawString(ffi.ToString, f, br, _
                                          New RectangleF(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height))
                End Using
            End Using

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler to set the new color for an item. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cbFontFamily_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbFontFamily.SelectedIndexChanged

            If Me.m_bInUpdate Then Return
            Me.SelectedFontFamilyName = Me.SelectedFontFamilyName

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler to set the new color for an item. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cbFontStyle_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbFontStyle.SelectedIndexChanged

            If Me.m_bInUpdate Then Return
            Me.SelectedFontStyle = Me.SelectedFontStyle

        End Sub

        Private Sub m_nudFontSize_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudFontSize.ValueChanged

            ' Hackerdihack - NUD controls send events from InitializeComponent
            If Me.UIContext Is Nothing Then Return

            If Me.m_bInUpdate Then Return
            Me.SelectedFontSize = Me.SelectedFontSize

        End Sub

#End Region ' Event handlers

#Region " Public methods "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanApply"/>
        ''' ------------------------------------------------------------------- 
        Public Function CanApply() As Boolean _
            Implements IOptionsPage.CanApply
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.OnChanged"/>
        ''' ------------------------------------------------------------------- 
        Public Event OnOptionsGraphsChanged(sender As IOptionsPage, args As System.EventArgs) _
            Implements IOptionsPage.OnChanged

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' ------------------------------------------------------------------- 
        Public Function Apply() As IOptionsPage.eApplyResultType _
             Implements IOptionsPage.Apply

            If Not Me.CanApply Then Return IOptionsPage.eApplyResultType.Failed

            Dim fti As cFontTypeItem = Nothing
  
            Me.UIContext.StyleGuide.SuspendEvents()

            ' Update fonts
            For i As Integer = 0 To Me.m_lbFontTypes.Items.Count - 1
                fti = DirectCast(Me.m_lbFontTypes.Items(i), cFontTypeItem)
                Me.UIContext.StyleGuide.FontFamilyName(fti.FontType) = fti.FontFamilyName
                Me.UIContext.StyleGuide.FontStyle(fti.FontType) = fti.FontStyle
                Me.UIContext.StyleGuide.FontSize(fti.FontType) = fti.FontSize
            Next

            Me.UIContext.StyleGuide.ResumeEvents()
            Me.UIContext.StyleGuide.FontsChanged()

            Return IOptionsPage.eApplyResultType.Success

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' ------------------------------------------------------------------- 
        Public Sub SetDefaults() _
             Implements IOptionsPage.SetDefaults
            Me.UIContext.StyleGuide.ThumbnailSize = cCore.NULL_VALUE
            Me.FillFontTypesListBox(True)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanSetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Function CanSetDefaults() As Boolean _
            Implements IOptionsPage.CanSetDefaults
            Return True
        End Function

#End Region ' Public methods

#Region " Helper methods "

        Private Sub FillFontTypesListBox(bReset As Boolean)

            Me.m_lbFontTypes.Items.Clear()

            Me.AddFontTypeItem(My.Resources.OPTIONS_FONTDLG_TITLE, cStyleGuide.eApplicationFontType.Title, bReset)
            Me.AddFontTypeItem(My.Resources.OPTIONS_FONTDLG_AXISTITLE, cStyleGuide.eApplicationFontType.SubTitle, bReset)
            Me.AddFontTypeItem(My.Resources.OPTIONS_FONTDLG_AXISSCALE_AND_VALUES, cStyleGuide.eApplicationFontType.Scale, bReset)

            Me.UpdatePreview()

        End Sub

        Private Sub AddFontTypeItem(ByVal strText As String, ByVal ft As cStyleGuide.eApplicationFontType, bDefault As Boolean)
            Dim sg As cStyleGuide = Me.UIContext.StyleGuide
            Me.m_lbFontTypes.Items.Add(New cFontTypeItem(strText, ft, Me.UIContext.StyleGuide, bDefault))
        End Sub

        Private Property SelectedFontType() As cFontTypeItem
            Get
                Return DirectCast(Me.m_lbFontTypes.SelectedItem, cFontTypeItem)
            End Get
            Set(ByVal value As cFontTypeItem)
                Me.m_lbFontTypes.SelectedItem = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to add an array of items into combobox control. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub FillFontFamiliesComboBox()

            Me.m_cbFontFamily.Items.Clear()

            ' Add all known colours
            For Each fam As FontFamily In FontFamily.Families
                If (String.IsNullOrEmpty(fam.Name) = False) And (fam.IsStyleAvailable(FontStyle.Regular) = True) Then
                    Me.m_cbFontFamily.Items.Add(New cFontFamilyItem(fam))
                End If
            Next

        End Sub

        Private Property SelectedFontFamilyName() As String
            Get
                Dim ffi As cFontFamilyItem = DirectCast(Me.m_cbFontFamily.SelectedItem, cFontFamilyItem)
                If ffi Is Nothing Then Return ""
                Return ffi.Family.Name
            End Get
            Set(ByVal value As String)

                'If String.Compare(value, Me.SelectedFontFamilyName, True) = 0 Then Return

                Dim fti As cFontTypeItem = Me.SelectedFontType
                Dim ffiNew As cFontFamilyItem = Nothing

                For Each ffi As cFontFamilyItem In Me.m_cbFontFamily.Items
                    If String.Compare(ffi.Family.Name, value, True) = 0 Then
                        ffiNew = ffi
                        Exit For
                    End If
                Next

                Me.m_cbFontFamily.SelectedItem = ffiNew
                fti.FontFamilyName = ffiNew.Family.Name
                Me.FillFontStyleCombobox()
                Me.UpdatePreview()

            End Set
        End Property

        Private ReadOnly Property SelectedFontFamily() As FontFamily
            Get
                Dim ffi As cFontFamilyItem = DirectCast(Me.m_cbFontFamily.SelectedItem, cFontFamilyItem)
                If ffi Is Nothing Then Return Nothing
                Return ffi.Family
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to select an item in the combo box by name. If the item
        ''' to update was not found, the custom colour item is selected and updated.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub FillFontStyleCombobox()

            Dim astyles As FontStyle() = New FontStyle() {FontStyle.Regular, FontStyle.Bold, FontStyle.Italic}
            Dim fam As FontFamily = Me.SelectedFontFamily

            Me.m_cbFontStyle.Items.Clear()

            If fam IsNot Nothing Then
                For i As Integer = 0 To astyles.Length - 1
                    If fam.IsStyleAvailable(astyles(i)) Then
                        Me.m_cbFontStyle.Items.Add(astyles(i))
                    End If
                Next
            End If

        End Sub

        Private Property SelectedFontStyle() As FontStyle
            Get
                If Me.m_cbFontStyle.SelectedItem Is Nothing Then Return FontStyle.Strikeout
                Return DirectCast(Me.m_cbFontStyle.SelectedItem, FontStyle)
            End Get
            Set(ByVal value As FontStyle)

                Dim fti As cFontTypeItem = Me.SelectedFontType

                For i As Integer = 0 To m_cbFontStyle.Items.Count - 1
                    If (DirectCast(Me.m_cbFontStyle.Items(i), FontStyle) = value) Then
                        Me.m_cbFontStyle.SelectedIndex = i
                        Exit For
                    End If
                Next

                fti.FontStyle = value
                Me.UpdatePreview()

            End Set
        End Property

        Private Property SelectedFontSize() As Single
            Get
                Return Convert.ToSingle(Me.m_nudFontSize.Value)
            End Get
            Set(ByVal value As Single)

                Dim fti As cFontTypeItem = Me.SelectedFontType

                'If (value = Me.SelectedFontSize) Then Return

                Me.m_nudFontSize.Value = Convert.ToDecimal(value)
                fti.FontSize = value
                Me.UpdatePreview()

            End Set
        End Property


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper methods to draw a custom listcontrol item 
        ''' </summary>
        ''' <remarks>This method is called by both Listbox drawItem event handlers</remarks>
        ''' -------------------------------------------------------------------
        Private Sub UpdatePreview()
            Me.m_plPreview.Invalidate()
        End Sub

        Private Sub OnPaintPreview(sender As Object, e As PaintEventArgs) Handles m_plPreview.Paint

            Dim fti As cFontTypeItem = Me.SelectedFontType

            e.Graphics.FillRectangle(SystemBrushes.Window, e.ClipRectangle)

            If (fti Is Nothing) Then Return

            Using ft As New Font(fti.FontFamilyName, fti.FontSize, fti.FontStyle, GraphicsUnit.Point)
                Dim fmt As New StringFormat()
                fmt.Alignment = StringAlignment.Center
                fmt.LineAlignment = StringAlignment.Center
                e.Graphics.DrawString(My.Resources.VALUE_PREVIEW, ft, SystemBrushes.ControlText, Me.m_plPreview.ClientRectangle, fmt)
            End Using

        End Sub

#End Region ' Helper methods

    End Class

End Namespace


