#Region " Imports "

Option Strict On

Imports EwEUtils.Core
Imports EwECore
Imports SAUPUtil.Misc.Colours
Imports EwECore.Auxiliary
Imports EwEUtils.Utilities

#End Region ' Imports

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
            Return Me.m_level.Name
        End Function

    End Class

    Public Class cPedigreeVisualizer

        Private m_colorramp As New SAUPColorRamp()
        Private m_renderstyle As eRenderStyleTypes = eRenderStyleTypes.Colors

        Public Enum eRenderStyleTypes As Integer
            Colors
            Indicators
            Values
            ConfidenceIntervals
        End Enum

        Public Sub Draw(ByVal g As Graphics, _
                        ByVal rc As Rectangle, _
                        ByVal level As cPedigreeLevel, _
                        ByVal style As eRenderStyleTypes)

            Select Case style
                Case eRenderStyleTypes.Colors
                    Using br As New SolidBrush(cColorUtils.IntToColor(level.PoolColor))
                        g.FillRectangle(br, rc)
                    End Using
            End Select
        End Sub

        Public Event OnRenderStyleChanged(ByVal sender As cPedigreeVisualizer)

        Public Property RenderStyle() As eRenderStyleTypes
            Get
                Return Me.m_renderstyle
            End Get
            Set(ByVal value As eRenderStyleTypes)
                If (value <> Me.m_renderstyle) Then
                    Me.m_renderstyle = value
                    RaiseEvent OnRenderStyleChanged(Me)
                End If
            End Set
        End Property

    End Class

#End Region ' Helper classes

#Region " Private vars "

    ''' <summary>Varname currently 'selected' in the grid.</summary>
    Private m_varname As eVarNameFlags = eVarNameFlags.NotSet
    Private m_viz As New cPedigreeVisualizer()

#End Region ' Private vars

    Public Sub New()
        Me.InitializeComponent()
        Me.Grid = Me.m_grid
    End Sub

#Region " Form overloads "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        AddHandler Me.m_grid.OnSelectionChanged, AddressOf OnGridSelectionChanged
        AddHandler Me.m_viz.OnRenderStyleChanged, AddressOf OnRenderStyleChanged
        AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

        Me.SelectedVariable = eVarNameFlags.Biomass

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

        RemoveHandler Me.m_grid.OnSelectionChanged, AddressOf OnGridSelectionChanged
        RemoveHandler Me.m_viz.OnRenderStyleChanged, AddressOf OnRenderStyleChanged
        RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

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

    Private Sub OnGridSelectionChanged(ByVal sel As SourceGrid2.CellVirtualCollection)

        ' ToDo:
        '   Allow only selections that span 1 column, no headers
        '   Extract var name from column
        '   Apply var name

        ' Beware
        '   Column change should only update UI to get ready for new selection but should
        '   NOT update pedigree assignments

    End Sub

    Private Sub OnViewAsChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tscmbViewAs.Click

        ' ToDo:
        ' v Store display style somewhere
        ' v Invalidate listbox to reflect this style
        ' v Inform grid of new style
        Me.SelectedRenderStyle = DirectCast(Me.m_tscmbViewAs.SelectedIndex, cPedigreeVisualizer.eRenderStyleTypes)

    End Sub

    Private Sub OnRenderStyleChanged(ByVal viz As cPedigreeVisualizer)

        Me.m_lbLevels.Invalidate()
        Me.UpdateControls()

    End Sub

    Private Sub OnDrawPedigreeListboxItem(ByVal sender As Object, ByVal e As DrawItemEventArgs) _
        Handles m_lbLevels.DrawItem

        ' Sanity check
        If Me.UIContext Is Nothing Then Return

        Dim item As cPedigreeLevelListboxItem = DirectCast(Me.m_lbLevels.Items(e.Index), cPedigreeLevelListboxItem)

        ' Render default background 
        e.DrawBackground()

        ' Render default text, bumped to the right by 22 pixels
        Using br As New SolidBrush(e.ForeColor)
            e.Graphics.DrawString(item.ToString(), e.Font, br, e.Bounds.X + 22, e.Bounds.Y)
        End Using

        Me.m_viz.Draw(e.Graphics, _
                      New Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4), _
                      item.Level, _
                      cPedigreeVisualizer.eRenderStyleTypes.Colors)

        ' Render default focus rectangle
        e.DrawFocusRectangle()

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
                Debug.Assert(cPedigreeManager.SupportVariables.Contains(value))
                Me.BuildPedigreeControls()
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
    ''' Get/set the selected <see cref="cPedigreeVisualizer.eRenderStyleTypes">render style</see> in
    ''' the entire interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Property SelectedRenderStyle() As cPedigreeVisualizer.eRenderStyleTypes
        Get
            Return Me.m_viz.RenderStyle
        End Get
        Set(ByVal value As cPedigreeVisualizer.eRenderStyleTypes)
            Me.m_viz.RenderStyle = value
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

        Me.m_tscmbViewAs.SelectedIndex = CInt(Me.SelectedRenderStyle)

    End Sub

#End Region ' Internal implementation

End Class
