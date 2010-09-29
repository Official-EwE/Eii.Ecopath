#Region " Imports "

Option Strict On

Imports EwEUtils.Core
Imports EwECore
Imports SAUPUtil.Misc.Colours
Imports EwECore.Auxiliary
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecopath.Tools

    Public Class frmPedigree

#Region " Helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Item for showing a pedigree level in the pedigree level listbox.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class cPedigreeLevelListboxItem

            Private m_level As cPedigreeLevel = Nothing

            Public Sub New(ByVal level As cPedigreeLevel)
                Me.m_level = level
            End Sub

            Public ReadOnly Property Level() As cPedigreeLevel
                Get
                    Return Me.m_level
                End Get
            End Property

            Public Overrides Function ToString() As String
                If (Me.m_level Is Nothing) Then
                    Return My.Resources.GENERIC_VALUE_NONE
                End If
                Return Me.m_level.Name
            End Function

        End Class

#End Region ' Helper classes

#Region " Private vars "

        ''' <summary>Varname currently 'selected' in the grid.</summary>
        Private m_varname As eVarNameFlags = eVarNameFlags.NotSet
        Private m_psg As cPedigreeStyleGuide = Nothing

#End Region ' Private vars

        Public Sub New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

#Region " Form overloads "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_psg = New cPedigreeStyleGuide(Me.UIContext)
            Me.m_grid.PedigreeStyleGuide = Me.m_psg

            For iVariable As Integer = 1 To Me.Core.nPedigreeVariables
                Me.m_cmbCategory.Items.Add(Core.PedigreeVariable(iVariable))
            Next

            AddHandler Me.m_psg.OnRenderStyleChanged, AddressOf OnRenderStyleChanged
            AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            AddHandler Me.m_grid.OnVariableChanged, AddressOf OnGridVariableChanged

            Me.SelectedVariable = eVarNameFlags.Biomass

            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

            RemoveHandler Me.m_psg.OnRenderStyleChanged, AddressOf OnRenderStyleChanged
            RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            RemoveHandler Me.m_grid.OnVariableChanged, AddressOf OnGridVariableChanged

            ' Clean up
            Me.SelectedVariable = eVarNameFlags.NotSet

            ' Done
            MyBase.OnFormClosed(e)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to make the form quick edit handler use the existing toolstrip.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides ReadOnly Property ToolStrip() As System.Windows.Forms.ToolStrip
            Get
                Return Me.m_tsMain
            End Get
        End Property

#End Region ' Form overloads

#Region " Events "

        Private Sub OnViewAsChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbViewAs.SelectedIndexChanged

            ' ToDo:
            ' v Store display style somewhere
            ' v Invalidate listbox to reflect this style
            ' v Inform grid of new style
            Dim iIndex As Integer = Me.m_cmbViewAs.SelectedIndex
            If (iIndex < 0) Then Return

            Me.SelectedRenderStyle = DirectCast(Me.m_cmbViewAs.SelectedIndex + 1, cPedigreeStyleGuide.eRenderStyleTypes)

        End Sub

        Private Sub OnCategoryChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbCategory.SelectedIndexChanged

            Me.SelectedVariable = DirectCast(Me.m_cmbCategory.SelectedItem, eVarNameFlags)
        End Sub

        Private Sub OnRenderStyleChanged(ByVal viz As cPedigreeStyleGuide)

            Me.m_lbLevels.Invalidate()
            Me.m_grid.Invalidate()

            Me.UpdateControls()

        End Sub

        Private Sub OnDrawPedigreeListboxItem(ByVal sender As Object, ByVal e As DrawItemEventArgs) _
            Handles m_lbLevels.DrawItem

            ' Sanity checks
            If (Me.UIContext Is Nothing) Then Return
            If (e.Index < 0) Then Return

            Dim item As cPedigreeLevelListboxItem = DirectCast(Me.m_lbLevels.Items(e.Index), cPedigreeLevelListboxItem)

            ' Render default background 
            e.DrawBackground()

            ' Render default text, bumped to the right by 22 pixels
            Using br As New SolidBrush(e.ForeColor)
                e.Graphics.DrawString(item.ToString(), e.Font, br, e.Bounds.X + 22, e.Bounds.Y)
            End Using

            ' Has level?
            If (item.Level IsNot Nothing) Then
                ' #Yes: Render colour box
                Using br As New SolidBrush(Me.m_psg.BackgroundColor(Me.BackColor, item.Level, cPedigreeStyleGuide.eRenderStyleTypes.Colors))
                    e.Graphics.FillRectangle(br, New Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4))
                End Using
            End If

            ' Render default focus rectangle
            e.DrawFocusRectangle()

        End Sub

        Private Sub OnLevelClick(ByVal sender As Object, ByVal e As MouseEventArgs) _
            Handles m_lbLevels.MouseClick

            Dim item As Object = Me.m_lbLevels.SelectedItem
            Dim level As cPedigreeLevel = Nothing
            Dim iValue As Integer = 0

            If (item IsNot Nothing) Then
                If (TypeOf item Is cPedigreeLevelListboxItem) Then
                    level = DirectCast(item, cPedigreeLevelListboxItem).Level
                    If (level IsNot Nothing) Then
                        iValue = level.ID
                    End If
                End If
            End If
            Me.m_grid.SetValue(iValue)

        End Sub

        Protected Sub OnGridVariableChanged(ByVal sender As Object, ByVal vn As eVarNameFlags)
            Me.SelectedVariable = vn
        End Sub

        Protected Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
            If (ct And cStyleGuide.eChangeType.Colours) > 0 Then
                Me.Invalidate()
            End If
        End Sub

#End Region ' Events

#Region " Internal implementation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected <see cref="eVarNameFlags">variable</see> in the grid.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Property SelectedVariable() As eVarNameFlags
            Get
                Return Me.m_varname
            End Get
            Set(ByVal value As eVarNameFlags)

                ' Sanity check
                If (Me.UIContext Is Nothing) Then Return
                ' Optimization
                If (value = Me.m_varname) Then Return
                ' Clean up
                If (Me.m_varname <> eVarNameFlags.NotSet) Then
                    Me.DestroyPedigreeControls()
                End If
                ' Remember new
                Me.m_varname = value
                ' Build new
                If (Me.m_varname <> eVarNameFlags.NotSet) Then
                    Debug.Assert(Me.Core.IsPedigreeVariableSupported(value), "Pedigree not supported for variable " & Me.m_varname.ToString)
                    Me.BuildPedigreeControls()
                    Me.m_cmbCategory.SelectedItem = Me.m_varname
                    Me.m_grid.SelectedVariable = Me.m_varname
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected <see cref="cPedigreeLevel">pedigree level</see> in
        ''' the listbox with available levels.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Property SelectedLevel() As cPedigreeLevel
            Get
                If (Me.m_lbLevels.SelectedItem Is Nothing) Then Return Nothing
                Return DirectCast(Me.m_lbLevels.SelectedItem, cPedigreeLevelListboxItem).Level
            End Get
            Set(ByVal value As cPedigreeLevel)

            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected <see cref="cPedigreeStyleGuide.eRenderStyleTypes">render style</see> in
        ''' the entire interface.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Property SelectedRenderStyle() As cPedigreeStyleGuide.eRenderStyleTypes
            Get
                Return Me.m_psg.RenderStyle
            End Get
            Set(ByVal value As cPedigreeStyleGuide.eRenderStyleTypes)
                If (value = cPedigreeStyleGuide.eRenderStyleTypes.NotSet) Then Return
                Me.m_psg.RenderStyle = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Lock the current <see cref="SelectedVariable">selected variable</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub BuildPedigreeControls()

            Dim man As cPedigreeManager = Me.Core.GetPedigreeManager(Me.SelectedVariable)
            Dim lvl As cPedigreeLevel = Nothing

            ' Add 'None' item
            Me.m_lbLevels.Items.Add(New cPedigreeLevelListboxItem(Nothing))
            For iLevel As Integer = 0 To man.NumLevels - 1
                lvl = man.Level(iLevel)
                Me.m_lbLevels.Items.Add(New cPedigreeLevelListboxItem(lvl))
            Next iLevel

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Release the current <see cref="SelectedVariable">selected variable</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub DestroyPedigreeControls()
            Me.m_lbLevels.Items.Clear()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update the UI.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateControls()

            If (Me.SelectedRenderStyle <> cPedigreeStyleGuide.eRenderStyleTypes.NotSet) Then
                Me.m_cmbViewAs.SelectedIndex = CInt(Me.SelectedRenderStyle) - 1
            End If

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
