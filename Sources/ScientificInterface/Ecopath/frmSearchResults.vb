#Region " Imports "

Option Strict On
Imports EwEPlugin.Data

#End Region ' Imports

Public Class frmSearchResults

    Private m_choice As eChoiceTypes = eChoiceTypes.UseSelected
    Private m_result As IDataSearchResults = Nothing

    Public Enum eChoiceTypes As Integer
        Cancel = 0
        UseSelected = 1
        SearchWithSelected = 2
    End Enum

    Public Sub New(ByVal uic As cUIContext, ByVal result As IDataSearchResults)
        MyBase.New()
        Me.InitializeComponent()
        Me.m_result = result
        Try
            Me.m_grid.Init(uic, result)
        Catch ex As Exception
            ' Wow
        End Try
    End Sub

    Public ReadOnly Property Choice() As eChoiceTypes
        Get
            Return Me.m_choice
        End Get
    End Property

    Public ReadOnly Property SelectedResult() As Object
        Get
            Return Me.m_grid.SelectedResult
        End Get
    End Property

#Region " Internals "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.m_lblStatus.Text = String.Format(Me.m_lblStatus.Text, Me.m_result.SearchResults.Length)
        Me.UpdateControls()
        AddHandler Me.m_grid.OnSelectionChanged, AddressOf OnSelectionChanged
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        RemoveHandler Me.m_grid.OnSelectionChanged, AddressOf OnSelectionChanged
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub UpdateControls()
        Me.m_btnUse.Enabled = (Me.SelectedResult IsNot Nothing)
        Me.m_btnSearch.Enabled = (Me.SelectedResult IsNot Nothing)
    End Sub

#End Region ' Internals

#Region " Events "

    Private Sub OnSelectionChanged(ByVal coll As Object)
        Me.UpdateControls()
    End Sub

    Private Sub OnUseSelected(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnUse.Click
        Me.m_choice = eChoiceTypes.UseSelected
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub OnSearchSelected(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnSearch.Click
        Me.m_choice = eChoiceTypes.SearchWithSelected
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

#End Region ' Events

End Class