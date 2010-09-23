#Region " Imports "

Option Strict On

Imports EwEUtils.Core
Imports EwECore
Imports SAUPUtil.Misc.Colours

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

    Friend Class cPedigreeVisualizer

        Private Shared c_colorramp As New SAUPColorRamp()

        Public Enum eRenderStyleTypes As Integer
            Colors
            Indicators
            Values
            ConfidenceIntervals
        End Enum

        Shared Sub Draw(ByVal rc As Rectangle, ByVal level As cPedigreeLevel, ByVal style As eRenderStyleTypes)

        End Sub

    End Class

#End Region ' Helper classes

#Region " Private vars "

    ''' <summary>Varname currently 'selected' in the grid.</summary>
    Private m_varname As eVarNameFlags = eVarNameFlags.NotSet

#End Region ' Private vars

    Public Sub New()
        Me.InitializeComponent()
    End Sub

#Region " Form overloads "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        AddHandler Me.m_grid.OnSelectionChanged, AddressOf OnGridSelectionChanged

        Me.SelectedVariable = eVarNameFlags.Biomass

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

        RemoveHandler Me.m_grid.OnSelectionChanged, AddressOf OnGridSelectionChanged

        ' Clean up
        Me.SelectedVariable = eVarNameFlags.NotSet

        ' Done
        MyBase.OnFormClosed(e)

    End Sub

#End Region ' Form overloads

#Region " Events "

    Private Sub OnGridSelectionChanged(ByVal sel As SourceGrid2.CellVirtualCollection)

        ' ToDo:
        '   Allow only selections that span 1 column, no headers
        '   Extract var name from column
        '   Apply var name

    End Sub

    Private Sub OnViewAsChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tscmbViewAs.Click

        ' ToDo:
        '   Store display style somewhere
        '   Invalidate listbox to reflect this style
        '   Inform grid of new style

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

    Private Sub UpdateControls()

    End Sub

#End Region ' Internal implementation

End Class
