#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' GUI via which users configure which colours the application must use
    ''' to provide standard feedback.
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

        ''' <summary>Only ref to core</summary>
        Private m_Core As cCore = Nothing
        ''' <summary>Only ref to styleguide</summary>
        Private m_sg As cStyleGuide = Nothing
        'List of known colours
        Private m_lciKnownColors As New List(Of cKnownColorItem)

#End Region ' Variables

#Region " Constructors "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            m_Core = cCore.GetInstance()
            m_sg = cStyleGuide.GetInstance()

            InitKnownColors()

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
            Dim kcColor As KnownColor

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

            FillColourComboBox(Me.cbItemForeground, m_lciKnownColors)
            FillColourComboBox(Me.cbItemBackground, m_lciKnownColors)

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
        ''' Helper method to load the global color items into listbox. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SetTypeColor()

            Me.lbItems.Items.Clear()
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
            Me.lbItems.SelectedIndex = 0

        End Sub

        Private Sub AddColorTypeItem(ByVal strName As String, ByVal ctFore As cStyleGuide.eApplicationColorType, _
                Optional ByVal ctBack As cStyleGuide.eApplicationColorType = cStyleGuide.eApplicationColorType.NotSet)
            Me.lbItems.Items.Add(New cColorItem(strName, ctFore, ctBack, Me.m_sg))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to add an array of items into combobox control. 
        ''' </summary>
        ''' <param name="cb">The combobox reference</param>
        ''' <param name="lColors">The list of <see cref="cKnownColorItem">color items</see> 
        ''' to be added into the combobox</param>
        ''' -------------------------------------------------------------------
        Private Sub FillColourComboBox(ByRef cb As ComboBox, ByRef lColors As List(Of cKnownColorItem))

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
        Private Sub UpdateColorComboboxItem(ByRef cb As ComboBox, ByRef clr As Color)

            Dim ciTest As cKnownColorItem = Nothing

            For i As Integer = 1 To cb.Items.Count - 1
                ciTest = CType(cb.Items(i), cKnownColorItem)
                If (ciTest.Color = clr) Then
                    cb.SelectedIndex = i
                    Return
                End If
            Next

            ' Update item 0: the Custom item
            ciTest = CType(cb.Items(0), cKnownColorItem)
            ciTest.Color = clr
            cb.SelectedIndex = 0

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to enable or disable UI controls.
        ''' </summary>
        ''' <param name="bEnableForeColorControls">Flag stating whether the foreground color controls need to be enabled.</param>
        ''' <param name="bEnableBackColorControls">Flag stating whether the background color controls need to be enabled.</param>
        ''' -------------------------------------------------------------------
        Private Sub SetUIControlsStatus(ByVal bEnableForeColorControls As Boolean, ByVal bEnableBackColorControls As Boolean)

            ' Enable/disable foreground color related controls
            Me.lblItemForeColor.Enabled = bEnableForeColorControls
            Me.cbItemForeground.Enabled = bEnableForeColorControls
            Me.btnCustomForeColor.Enabled = bEnableForeColorControls

            ' Avoid confusion by blanking out the fore color combo if no fore color should be shown
            If Not bEnableForeColorControls Then Me.cbItemForeground.SelectedIndex = -1

            ' Enable/disable background color related controls
            Me.lblItemBackColor.Enabled = bEnableBackColorControls
            Me.cbItemBackground.Enabled = bEnableBackColorControls
            Me.btnCustomBackColor.Enabled = bEnableBackColorControls

            ' Avoid confusion by blanking out the back color combo if no back color should be shown
            If Not bEnableBackColorControls Then Me.cbItemBackground.SelectedIndex = -1

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to update the color in an item. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateForeColor(ByRef tcli As cColorItem, ByVal clr As Color)

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
        Private Sub UpdateBackColor(ByRef tcli As cColorItem, ByVal clr As Color)

            ' Sanity check
            If tcli Is Nothing Then Return

            ' Update the color in the data structure
            tcli.BackColor = clr
            ' Update the text sammple
            Me.m_lblExample.BackColor = clr

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper methods to draw a custom combobox item 
        ''' </summary>
        ''' <param name="e">DrawItemEventArgs sent by DrawItem event handler</param>
        ''' <remarks>This method is called by  Combobox drawItem event handlers</remarks>
        ''' -------------------------------------------------------------------
        Private Sub cbDrawCustomItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs)

            Dim s As ComboBox = CType(sender, ComboBox)
            If s Is Nothing Then Return

            Try
                'Get the current drawn item
                Dim item As cKnownColorItem = CType(s.Items(e.Index), cKnownColorItem)
                'The rectangle to draw the color box
                Dim rect As Rectangle = New Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Height, e.Bounds.Height - 4)

                DrawCustomItem(e, item.Color, item.Name, rect)
            Catch ex As Exception
                Return
            End Try

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
                                    ByRef txt As String, _
                                    ByRef rect As Rectangle)


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
        Private Sub DrawCustomText(ByVal e As System.Windows.Forms.DrawItemEventArgs, ByRef txt As String, ByRef rect As Rectangle)
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
        Private Sub ucColors_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Clear the listbox items
            lbItems.Items.Clear()

            'Only display text 
            SetTypeColor()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Listbox drawItem method getting called when the drawMode is either OwnerDrawFixed or OwnerDrawVariable
        ''' </summary>
        ''' <remarks>To customize drawing so we can draw colorbox next to text</remarks>
        ''' -------------------------------------------------------------------
        Private Sub lbItems_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles lbItems.DrawItem

            ' get the sender of this event
            Dim s As ListBox = CType(sender, ListBox)
            If s Is Nothing Then Return
            If e.Index = -1 Then Return

            'get the current drawn item
            Dim item As cColorItem = CType(s.Items(e.Index), cColorItem)
            DrawCustomText(e, item.Name, e.Bounds)

        End Sub

        Private Sub lbItems_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles lbItems.SelectedIndexChanged

            Dim item As cColorItem = DirectCast(lbItems.SelectedItem, cColorItem)

            ' Update controls state
            SetUIControlsStatus(item.ForeColorType <> cStyleGuide.eApplicationColorType.NotSet, item.BackColorType <> cStyleGuide.eApplicationColorType.NotSet)

            'Update the selection in combobox
            If (item.ForeColorType <> cStyleGuide.eApplicationColorType.NotSet) Then
                UpdateColorComboboxItem(Me.cbItemForeground, Me.m_sg.ApplicationColor(item.ForeColorType))
                Me.m_lblExample.ForeColor = Me.m_sg.ApplicationColor(item.ForeColorType)
            Else
                ' Hiding text w Color.Transparent does not work; show text in background colour instead
                Me.m_lblExample.ForeColor = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
            End If

            If (item.BackColorType <> cStyleGuide.eApplicationColorType.NotSet) Then
                UpdateColorComboboxItem(Me.cbItemBackground, Me.m_sg.ApplicationColor(item.BackColorType))
                Me.m_lblExample.BackColor = Me.m_sg.ApplicationColor(item.BackColorType)
            Else
                Me.m_lblExample.BackColor = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_BACKGROUND)
            End If

            ' Update name and description
            If String.IsNullOrEmpty(item.Description) Then
                Me.m_lblSelection.Text = ""
                Me.m_lblDescription.Text = ""
            Else
                Me.m_lblSelection.Text = item.Name
                Me.m_lblDescription.Text = item.Description
            End If

            Me.lbItems.Select()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Combobox drawItem method getting called when the drawMode is either OwnerDrawFixed or OwnerDrawVariable
        ''' </summary>
        ''' <remarks>To customize drawing so we can draw colorbox next to text</remarks>
        ''' -------------------------------------------------------------------
        Private Sub cbItemForeground_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles cbItemForeground.DrawItem
            cbDrawCustomItem(sender, e)
        End Sub

        ''' <summary>
        ''' Combobox drawItem method getting called when the drawMode is either OwnerDrawFixed or OwnerDrawVariable
        ''' </summary>
        ''' <remarks>To customize drawing so we can draw colorbox next to text</remarks>
        Private Sub cbItemBackground_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles cbItemBackground.DrawItem
            cbDrawCustomItem(sender, e)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' button's click event handler. It provides the functionality for use to select a color from 
        ''' system-defined color dialog. 
        ''' </summary>
        ''' <remarks>For foreground color</remarks>
        ''' -------------------------------------------------------------------
        Private Sub btnCustomForeColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCustomForeColor.Click

            Dim tcli As cColorItem = CType(lbItems.SelectedItem, cColorItem)
            Dim clrSelected As Color = Nothing

            If (tcli IsNot Nothing) Then
                clrSelected = SelectColorByDialog(tcli.ForeColor)
                UpdateColorComboboxItem(cbItemForeground, clrSelected)
                UpdateForeColor(tcli, clrSelected)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' button's click event handler. It provides the functionality for use to select a color from 
        ''' system-defined color dialog. 
        ''' </summary>
        ''' <remarks>For background color</remarks>
        ''' -------------------------------------------------------------------
        Private Sub btnCustomBackColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCustomBackColor.Click

            Dim tcli As cColorItem = CType(lbItems.SelectedItem, cColorItem)
            Dim clrSelected As Color = Nothing

            If (tcli IsNot Nothing) Then
                clrSelected = SelectColorByDialog(tcli.BackColor)
                UpdateColorComboboxItem(cbItemBackground, clrSelected)
                UpdateBackColor(tcli, clrSelected)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler to set the color prefrence to default colors. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub btnUseDefault_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUseDefault.Click

            Me.m_sg.LoadDefaultApplicationColors()
            Me.SetTypeColor()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler to set the new color for an item. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub cbItemForeground_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbItemForeground.SelectedIndexChanged

            Dim tcli As cColorItem = CType(lbItems.SelectedItem, cColorItem)
            Dim selClr As cKnownColorItem = CType(Me.cbItemForeground.SelectedItem, cKnownColorItem)

            If tcli.ForeColorType <> cStyleGuide.eApplicationColorType.NotSet Then
                UpdateForeColor(tcli, selClr.Color)
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler to set the new color for an item. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub cbItemBackground_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbItemBackground.SelectedIndexChanged

            Dim tcli As cColorItem = DirectCast(lbItems.SelectedItem, cColorItem)
            Dim selClr As cKnownColorItem = DirectCast(Me.cbItemBackground.SelectedItem, cKnownColorItem)

            If tcli.BackColorType <> cStyleGuide.eApplicationColorType.NotSet Then
                UpdateBackColor(tcli, selClr.Color)
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

            ' Apply colors to the style guide
            Me.m_sg.SuspendEvents()

            For i As Integer = 0 To Me.lbItems.Items.Count - 1
                ci = DirectCast(Me.lbItems.Items(i), cColorItem)
                If ci.ForeColorType <> cStyleGuide.eApplicationColorType.NotSet Then
                    Me.m_sg.ApplicationColor(ci.ForeColorType) = ci.ForeColor
                End If
                If ci.BackColorType <> cStyleGuide.eApplicationColorType.NotSet Then
                    Me.m_sg.ApplicationColor(ci.BackColorType) = ci.BackColor
                End If
            Next

            Me.m_sg.ResumeEvents()

        End Sub

#End Region ' Public methods

    End Class


End Namespace


