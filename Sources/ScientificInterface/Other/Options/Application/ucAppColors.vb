#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Color settings interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucAppColors

#Region " Helper classes "

        Private Class cColorItem

            Private m_strName As String = ""
            Private m_strDescription As String = ""
            Private m_ctFore As cStyleGuide.eApplicationColorType
            Private m_clrFore As Color = Nothing
            Private m_ctBack As cStyleGuide.eApplicationColorType
            Private m_clrBack As Color = Nothing

            Public Sub New(ByVal strText As String, ByVal ctFore As cStyleGuide.eApplicationColorType, ByVal ctBack As cStyleGuide.eApplicationColorType, ByVal sg As cStyleGuide)

                Dim astrBits As String() = strText.Split("|"c)
                Me.m_strName = astrBits(0)
                If astrBits.Length = 2 Then
                    Me.m_strDescription = astrBits(1)
                Else
                    Me.m_strDescription = ""
                End If
                Me.m_ctFore = ctFore
                Me.m_ctBack = ctBack

                Me.ForeColor = sg.ApplicationColor(Me.m_ctFore)
                Me.BackColor = sg.ApplicationColor(Me.m_ctBack)
            End Sub

            Public ReadOnly Property Description() As String
                Get
                    Return Me.m_strDescription
                End Get
            End Property

            Public ReadOnly Property Name() As String
                Get
                    Return Me.m_strName
                End Get
            End Property

            Public ReadOnly Property ForeColorType() As cStyleGuide.eApplicationColorType
                Get
                    Return Me.m_ctFore
                End Get
            End Property

            Public ReadOnly Property BackColorType() As cStyleGuide.eApplicationColorType
                Get
                    Return Me.m_ctBack
                End Get
            End Property

            Public Property ForeColor() As Color
                Get
                    Return Me.m_clrFore
                End Get
                Set(ByVal value As Color)
                    If Me.m_ctFore <> cStyleGuide.eApplicationColorType.NotSet Then Me.m_clrFore = value
                End Set
            End Property

            Public Property BackColor() As Color
                Get
                    Return Me.m_clrBack
                End Get
                Set(ByVal value As Color)
                    If Me.m_ctBack <> cStyleGuide.eApplicationColorType.NotSet Then Me.m_clrBack = value
                End Set
            End Property

            Public Overrides Function ToString() As String
                Return Me.m_strName
            End Function

        End Class

        Private Class cKnownColorItem

            Private m_strName As String = ""
            Private m_clr As Color

            Public Sub New(ByVal strName As String, ByVal clr As Color)
                Me.m_strName = strName
                Me.m_clr = clr
            End Sub

            Public ReadOnly Property Name() As String
                Get
                    Return Me.m_strName
                End Get
            End Property

            Public Property Color() As Color
                Get
                    Return Me.m_clr
                End Get
                Set(ByVal value As Color)
                    Me.m_clr = value
                End Set
            End Property

            Public Overrides Function ToString() As String
                Return Me.m_strName
            End Function

        End Class

#End Region ' Helper classes

#Region " Variables "

        ''' <summary>Only ref to core.</summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary>List of known colours.</summary>
        Private m_lciKnownColors As New List(Of cKnownColorItem)

#End Region ' Variables

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)

            Me.m_uic = uic
            Me.InitializeComponent()
            Me.InitKnownColors()

        End Sub

#End Region ' Constructors

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The helper methods define the common used colors to choosing color from for easy access 
        ''' </summary>
        ''' <remarks>Define all known colours</remarks>
        ''' -------------------------------------------------------------------
        Private Sub InitKnownColors()

            Dim astrNames() As String = [Enum].GetNames(GetType(KnownColor))
            Dim kcColor As KnownColor = Nothing

            m_lciKnownColors.Clear()

            ' Iterate through each known color name
            For Each strName As String In astrNames
                ' Cast the color name into a KnownColor
                kcColor = DirectCast([Enum].Parse(GetType(KnownColor), strName), KnownColor)
                ' Check if this is a System color (system color names have no ARGB values)
                If (kcColor > KnownColor.Transparent) Then
                    ' Add it to the internal list of colours
                    m_lciKnownColors.Add(New cKnownColorItem(strName, Color.FromName(strName)))
                End If
            Next strName

            FillColourComboBox(Me.m_cmbItemForeground, m_lciKnownColors)
            FillColourComboBox(Me.m_cmbItemBackground, m_lciKnownColors)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to find the known color name for a given color.
        ''' </summary>
        ''' <param name="clr">the color to find.</param>
        ''' <returns>The name of a known color, or and empty string if no match was not found.</returns>
        ''' -------------------------------------------------------------------
        Private Function GetColorName(ByVal clr As Color) As String

            Dim ciTest As cKnownColorItem = Nothing
            For iKnown As Integer = 0 To Me.m_lciKnownColors.Count - 1
                ciTest = Me.m_lciKnownColors(iKnown)
                If clr.R = ciTest.Color.R And clr.G = ciTest.Color.G And clr.B = ciTest.Color.B Then
                    Return ciTest.Name
                End If
            Next
            Return ""

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to load color items into the listbox. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub FillColorItemsList()

            Me.m_lbItems.Items.Clear()
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_DEFAULT, cStyleGuide.eApplicationColorType.DEFAULT_TEXT, cStyleGuide.eApplicationColorType.DEFAULT_BACKGROUND)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_NAMES, cStyleGuide.eApplicationColorType.NAMES_TEXT, cStyleGuide.eApplicationColorType.NAMES_BACKGROUND)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_MODEL_FAILEDRESULT, cStyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_MODEL_FAILEDVALIDATION, cStyleGuide.eApplicationColorType.FAILEDVALIDATION_TEXT)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_ERROR, cStyleGuide.eApplicationColorType.GENERICERROR_TEXT)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_COMPUTED, cStyleGuide.eApplicationColorType.COMPUTED_TEXT)
            ' JS 02Aug08: disabled, not used
            'Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_ES_PRESSURE, StyleGuide.eApplicationColorType.FISHINGPRESSURE_TEXT)
            'Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_ES_PROFITS, StyleGuide.eApplicationColorType.PROFIT_TEXT)
            'Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_ES_TOTALCATCH, StyleGuide.eApplicationColorType.TOTALCATCH_TEXT)
            'Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_TROPHIC_LINK, StyleGuide.eApplicationColorType.TROPHICLINK_TEXT)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_REMARKS, cStyleGuide.eApplicationColorType.NotSet, cStyleGuide.eApplicationColorType.REMARKS_BACKGROUND)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_SUM, cStyleGuide.eApplicationColorType.NotSet, cStyleGuide.eApplicationColorType.SUM_BACKGROUND)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_READONLY, cStyleGuide.eApplicationColorType.NotSet, cStyleGuide.eApplicationColorType.READONLY_BACKGROUND)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_MODEL_MISSINGPARAM, cStyleGuide.eApplicationColorType.NotSet, cStyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_CHECKED, cStyleGuide.eApplicationColorType.NotSet, cStyleGuide.eApplicationColorType.CHECKED_BACKGROUND)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_PROMPT_HIGHLIGHT, cStyleGuide.eApplicationColorType.NotSet, cStyleGuide.eApplicationColorType.HIGHLIGHT)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_IMG_BACKGROUND_COLOR, cStyleGuide.eApplicationColorType.NotSet, cStyleGuide.eApplicationColorType.IMAGE_BACKGROUND)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_ECOSIM_PLOTS_BACKGROUND_COLOR, cStyleGuide.eApplicationColorType.NotSet, cStyleGuide.eApplicationColorType.PLOT_BACKGROUND)
            Me.AddColorTypeItem(My.Resources.OPTIONS_COLORDLG_ECOSPACE_MAPLOT_BACKGROUND_COLOR, cStyleGuide.eApplicationColorType.NotSet, cStyleGuide.eApplicationColorType.MAP_BACKGROUND)

            ' Kick off
            Me.m_lbItems.SelectedIndex = 0

        End Sub

        Private Sub AddColorTypeItem(ByVal strName As String, ByVal ctFore As cStyleGuide.eApplicationColorType, _
                Optional ByVal ctBack As cStyleGuide.eApplicationColorType = cStyleGuide.eApplicationColorType.NotSet)
            Me.m_lbItems.Items.Add(New cColorItem(strName, ctFore, ctBack, Me.m_uic.StyleGuide))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to add an array of items into combobox control. 
        ''' </summary>
        ''' <param name="cb">The combobox reference</param>
        ''' <param name="lColors">The list of <see cref="cKnownColorItem">color items</see> 
        ''' to be added into the combobox</param>
        ''' -------------------------------------------------------------------
        Private Sub FillColourComboBox(ByVal cb As ComboBox, ByVal lColors As List(Of cKnownColorItem))

            cb.Items.Clear()

            ' Add intial 'custom' item
            cb.Items.Add(New cKnownColorItem(My.Resources.GENERIC_VALUE_CUSTOM, Color.Black))

            ' Add all known colours
            For i As Integer = 0 To lColors.Count - 1
                cb.Items.Add(lColors(i))
            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to select an item in the combo box by name. If the item
        ''' to update was not found, the custom colour item is selected and updated.
        ''' </summary>
        ''' <param name="cb">The comboxbox to update.</param>
        ''' <param name="clr">The color to update.</param>
        ''' -------------------------------------------------------------------
        Private Sub UpdateColorComboboxItem(ByVal cb As ComboBox, ByVal clr As Color)

            Dim ciTest As cKnownColorItem = Nothing

            For i As Integer = 1 To cb.Items.Count - 1
                ciTest = DirectCast(cb.Items(i), cKnownColorItem)
                If (ciTest.Color = clr) Then
                    cb.SelectedIndex = i
                    Return
                End If
            Next

            ' Update item 0: the Custom item
            ciTest = DirectCast(cb.Items(0), cKnownColorItem)
            ciTest.Color = clr
            cb.SelectedIndex = 0

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to enable and update UI controls.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()

            Dim item As cColorItem = DirectCast(m_lbItems.SelectedItem, cColorItem)
            Dim bShowForeground As Boolean = False
            Dim bShowBackground As Boolean = False
            Dim strName As String = ""
            Dim strDescription As String = ""
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide

            If (item IsNot Nothing) Then
                bShowForeground = (item.ForeColorType <> cStyleGuide.eApplicationColorType.NotSet)
                bShowBackground = (item.BackColorType <> cStyleGuide.eApplicationColorType.NotSet)
                strName = item.Name
                strDescription = item.Description
            End If

            'Update the selection in combobox
            If (bShowForeground) Then
                Me.UpdateColorComboboxItem(Me.m_cmbItemForeground, sg.ApplicationColor(item.ForeColorType))
                Me.m_lblExample.ForeColor = sg.ApplicationColor(item.ForeColorType)
            Else
                ' Hiding text w Color.Transparent does not work; show text in background colour instead
                Me.m_lblExample.ForeColor = sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
            End If

            If (bShowBackground) Then
                Me.UpdateColorComboboxItem(Me.m_cmbItemBackground, sg.ApplicationColor(item.BackColorType))
                Me.m_lblExample.BackColor = sg.ApplicationColor(item.BackColorType)
            Else
                Me.m_lblExample.BackColor = sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_BACKGROUND)
            End If

            ' Update name and description
            Me.m_lblSelection.Text = strName
            Me.m_lblDescription.Text = strDescription

            ' Enable/disable foreground color related controls
            Me.m_lblItemForeColor.Enabled = bShowForeground
            Me.m_cmbItemForeground.Enabled = bShowForeground
            Me.m_btnCustomForeColor.Enabled = bShowForeground

            ' Avoid confusion by blanking out the fore color combo if no fore color should be shown
            If Not bShowForeground Then Me.m_cmbItemForeground.SelectedIndex = -1

            ' Enable/disable background color related controls
            Me.lblItemBackColor.Enabled = bShowBackground
            Me.m_cmbItemBackground.Enabled = bShowBackground
            Me.m_btnCustomBackColor.Enabled = bShowBackground

            ' Avoid confusion by blanking out the back color combo if no back color should be shown
            If Not bShowBackground Then Me.m_cmbItemBackground.SelectedIndex = -1

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to update the color in an item. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateForeColor(ByVal tcli As cColorItem, ByVal clr As Color)

            ' Sanity check
            If tcli Is Nothing Then Return

            ' Update the color in the data structure
            tcli.ForeColor = clr
            ' Update the text sammple
            Me.m_lblExample.ForeColor = clr

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to update the color in an item. 
        ''' </summary>
        ''' <param name="clr">The item reference whose color gets updated.</param>
        ''' -------------------------------------------------------------------
        Private Sub UpdateBackColor(ByVal tcli As cColorItem, ByVal clr As Color)

            ' Sanity check
            If tcli Is Nothing Then Return

            ' Update the color in the data structure
            tcli.BackColor = clr
            ' Update the text sammple
            Me.m_lblExample.BackColor = clr

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper methods to draw a custom listcontrol item 
        ''' </summary>
        ''' <param name="e">DrawItemEventArgs sent by DrawItem event handler</param>
        ''' <param name="clr">The colorbox's color</param>
        ''' <param name="txt">The text beside the colorbox</param>
        ''' <remarks>This method is called by both Listbox and Combobox drawItem event handlers</remarks>
        ''' -------------------------------------------------------------------
        Private Sub DrawCustomItem(ByVal e As System.Windows.Forms.DrawItemEventArgs, _
                                   ByVal clr As Color, _
                                   ByVal txt As String, _
                                   ByVal rect As Rectangle)


            ' Do nothing if there is no data
            If e.Index = -1 Then Return

            'If the item is selected, draw the correct background color
            e.DrawBackground()
            e.DrawFocusRectangle()

            'Get the listbox's graphics object
            Dim g As Graphics = e.Graphics

            'Draw color box
            g.FillRectangle(New SolidBrush(clr), rect)
            g.DrawRectangle(Pens.Black, rect)
            'Draw text 
            g.DrawString(txt, e.Font, New SolidBrush(e.ForeColor), _
                            New RectangleF(e.Bounds.X + rect.Width + 4, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height))


        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper methods to draw a custom listcontrol item 
        ''' </summary>
        ''' <param name="e">DrawItemEventArgs sent by DrawItem event handler</param>
        ''' <param name="txt">The text beside the colorbox</param>
        ''' <remarks>This method is called by both Listbox drawItem event handlers</remarks>
        ''' -------------------------------------------------------------------
        Private Sub DrawCustomText(ByVal e As System.Windows.Forms.DrawItemEventArgs, _
                                   ByVal txt As String, _
                                   ByVal rect As Rectangle)
            ' Do nothing if there is no data
            If e.Index = -1 Then Return

            'If the item is selected, draw the correct background color
            e.DrawBackground()
            e.DrawFocusRectangle()

            'Get the listbox's graphics object
            Dim g As Graphics = e.Graphics
            'Draw text 
            g.DrawString(txt, e.Font, New SolidBrush(e.ForeColor), rect)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to show a colorDialog and lets user to select a color.  
        ''' </summary>
        ''' <returns>The chosen color, nothing if no color was chosen.</returns>
        ''' -------------------------------------------------------------------
        Private Function SelectColorByDialog(ByVal clr As Color) As Color

            Dim dlg As New ColorDialog
            Dim iCustomColor As Integer = 0

            ' Pass in the current color
            dlg.Color = clr
            dlg.AllowFullOpen = True
            dlg.AnyColor = True
            ' Work-around for known Color-to-Colorref conversion bug in .NET ColorDialog (ARGB vs XBGR)
            ' http://groups.google.com/group/microsoft.public.dotnet.framework.windowsforms/browse_frm/thread/58cbe7edf7402584
            iCustomColor = clr.R + (clr.G * 256) + (clr.B * 65536)
            dlg.CustomColors() = New Integer() {iCustomColor}

            If dlg.ShowDialog() = DialogResult.OK Then clr = dlg.Color

            Return clr

        End Function

#End Region ' Helper methods

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Control's load event which gets called every time the control gets loaded. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            'Clear the listbox items
            Me.m_lbItems.Items.Clear()

            'Only display text 
            Me.FillColorItemsList()

        End Sub

        Private Sub lbItems_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_lbItems.SelectedIndexChanged
            Me.UpdateControls()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Combobox drawItem method getting called when the drawMode is either OwnerDrawFixed or OwnerDrawVariable
        ''' </summary>
        ''' <remarks>To customize drawing so we can draw colorbox next to text</remarks>
        ''' -------------------------------------------------------------------
        Private Sub cbItemForeground_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) _
            Handles m_cmbItemForeground.DrawItem, m_cmbItemBackground.DrawItem

            Dim cmb As ComboBox = DirectCast(sender, ComboBox)
            If cmb Is Nothing Then Return

            Try
                'Get the current drawn item
                Dim item As cKnownColorItem = DirectCast(cmb.Items(e.Index), cKnownColorItem)
                'The rectangle to draw the color box
                Dim rect As Rectangle = New Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Height, e.Bounds.Height - 4)

                Me.DrawCustomItem(e, item.Color, item.Name, rect)
            Catch ex As Exception
                Return
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' button's click event handler. It provides the functionality for use to select a color from 
        ''' system-defined color dialog. 
        ''' </summary>
        ''' <remarks>For foreground color</remarks>
        ''' -------------------------------------------------------------------
        Private Sub btnCustomForeColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCustomForeColor.Click

            Dim tcli As cColorItem = DirectCast(Me.m_lbItems.SelectedItem, cColorItem)
            Dim clrSelected As Color = Nothing

            If (tcli IsNot Nothing) Then
                clrSelected = SelectColorByDialog(tcli.ForeColor)
                Me.UpdateColorComboboxItem(m_cmbItemForeground, clrSelected)
                Me.UpdateForeColor(tcli, clrSelected)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' button's click event handler. It provides the functionality for use to select a color from 
        ''' system-defined color dialog. 
        ''' </summary>
        ''' <remarks>For background color</remarks>
        ''' -------------------------------------------------------------------
        Private Sub btnCustomBackColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCustomBackColor.Click

            Dim tcli As cColorItem = DirectCast(Me.m_lbItems.SelectedItem, cColorItem)
            Dim clrSelected As Color = Nothing

            If (tcli IsNot Nothing) Then
                clrSelected = Me.SelectColorByDialog(tcli.BackColor)
                Me.UpdateColorComboboxItem(m_cmbItemBackground, clrSelected)
                Me.UpdateBackColor(tcli, clrSelected)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler to set the color prefrence to default colors. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub btnUseDefault_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnResetAll.Click

            Me.m_uic.StyleGuide.LoadDefaultApplicationColors()
            Me.FillColorItemsList()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler to set the new color for an item. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub cbItemForeground_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbItemForeground.SelectedIndexChanged

            Dim tcli As cColorItem = DirectCast(Me.m_lbItems.SelectedItem, cColorItem)
            Dim selClr As cKnownColorItem = DirectCast(Me.m_cmbItemForeground.SelectedItem, cKnownColorItem)

            If tcli.ForeColorType <> cStyleGuide.eApplicationColorType.NotSet Then
                Me.UpdateForeColor(tcli, selClr.Color)
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler to set the new color for an item. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub cbItemBackground_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbItemBackground.SelectedIndexChanged

            Dim tcli As cColorItem = DirectCast(Me.m_lbItems.SelectedItem, cColorItem)
            Dim selClr As cKnownColorItem = DirectCast(Me.m_cmbItemBackground.SelectedItem, cKnownColorItem)

            If tcli.BackColorType <> cStyleGuide.eApplicationColorType.NotSet Then
                Me.UpdateBackColor(tcli, selClr.Color)
            End If
        End Sub

#End Region ' Event handlers

#Region " Public methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save colour selections back to the style guide.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Save()

            Dim ci As cColorItem = Nothing
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide

            ' Apply colors to the style guide
            sg.SuspendEvents()

            For i As Integer = 0 To Me.m_lbItems.Items.Count - 1
                ci = DirectCast(Me.m_lbItems.Items(i), cColorItem)
                If ci.ForeColorType <> cStyleGuide.eApplicationColorType.NotSet Then
                    sg.ApplicationColor(ci.ForeColorType) = ci.ForeColor
                End If
                If ci.BackColorType <> cStyleGuide.eApplicationColorType.NotSet Then
                    sg.ApplicationColor(ci.BackColorType) = ci.BackColor
                End If
            Next

            sg.ResumeEvents()

        End Sub

#End Region ' Public methods

    End Class


End Namespace


