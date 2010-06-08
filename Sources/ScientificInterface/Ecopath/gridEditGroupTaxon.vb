#Region " Imports "

Option Strict On
Imports SourceGrid2
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Grid class implementing the Edit Group Taxon interface grid bit.
''' </summary>
''' -----------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class gridEditGroupTaxon
    Inherits EwEGrid

    ''' <summary>Visual model to display original groups.</summary>
    Private m_vmOriginal As VisualModels.Common = New VisualModels.Common(False)
    ''' <summary>Visual model to display newly created groups.</summary>
    Private m_vmAdded As VisualModels.Common = New VisualModels.Common(False)
    ''' <summary>Visual model to display groups that are about be deleted.</summary>
    Private m_vmRemoved As VisualModels.Common = New VisualModels.Common(False)

    ''' <summary>Enumerated type defining the columns in this grid.</summary>
    Private Enum eColumnTypes
        GroupIndex = 0
        GroupName
        Proportion
    End Enum

#Region " Private helper classes "

    Private Class cTaxonInfo
        Implements ITaxonData

        Public Sub New(ByVal src As cTaxon)

        End Sub

        Public Sub New(ByVal src As ITaxonData)

        End Sub

        Public ReadOnly Property [Class]() As String Implements EwEUtils.Core.ITaxonData.Class
            Get

            End Get
        End Property

        Public ReadOnly Property Code3A() As String Implements EwEUtils.Core.ITaxonData.Code3A
            Get

            End Get
        End Property

        Public ReadOnly Property CodeISSCAAP() As String Implements EwEUtils.Core.ITaxonData.CodeISSCAAP
            Get

            End Get
        End Property

        Public ReadOnly Property CodeTaxon() As String Implements EwEUtils.Core.ITaxonData.CodeTaxon
            Get

            End Get
        End Property

        Public ReadOnly Property Common() As String Implements EwEUtils.Core.ITaxonData.Common
            Get

            End Get
        End Property

        Public ReadOnly Property Family() As String Implements EwEUtils.Core.ITaxonData.Family
            Get

            End Get
        End Property

        Public ReadOnly Property Genus() As String Implements EwEUtils.Core.ITaxonData.Genus
            Get

            End Get
        End Property

        Public ReadOnly Property LastUpdated() As Date Implements EwEUtils.Core.ITaxonData.LastUpdated
            Get

            End Get
        End Property

        Public ReadOnly Property Order() As String Implements EwEUtils.Core.ITaxonData.Order
            Get

            End Get
        End Property

        Public ReadOnly Property Source() As String Implements EwEUtils.Core.ITaxonData.Source
            Get

            End Get
        End Property

        Public ReadOnly Property SourceKey() As String Implements EwEUtils.Core.ITaxonData.SourceKey
            Get

            End Get
        End Property

        Public ReadOnly Property Species() As String Implements EwEUtils.Core.ITaxonData.Species
            Get

            End Get
        End Property
    End Class

#End Region ' Private helper classes

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create the grid
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        MyBase.New()

        ' Set up visual models for reflecting group modification status
        With Me.m_vmOriginal
            .ForeColor = Color.FromArgb(255, 0, 0, 0)
            .TextAlignment = ContentAlignment.MiddleCenter
            .MakeReadOnly()
        End With

        With Me.m_vmAdded
            .ForeColor = Color.FromArgb(255, 8, 128, 12)
            .TextAlignment = ContentAlignment.MiddleCenter
            .MakeReadOnly()
        End With

        With Me.m_vmRemoved
            .ForeColor = Color.FromArgb(255, 255, 22, 12)
            .TextAlignment = ContentAlignment.MiddleCenter
            .MakeReadOnly()
        End With

    End Sub


#Region " Grid interaction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        'Me.Selection.SelectionMode = GridSelectionMode.Row
        Me.Selection.EnableMultiSelection = False

        ' Redim columns
        Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

        ' Group index cell
        Me(0, eColumnTypes.GroupIndex) = New EwEColumnHeaderCell()
        ' Group name cell, editable this time
        Me(0, eColumnTypes.GroupName) = New EwEColumnHeaderCell(My.Resources.HEADER_NAME)
        Me(0, eColumnTypes.Proportion) = New EwEColumnHeaderCell("Proportion")

        Me.FixedColumns = 1

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to first create a snapshot of the group/stanza configuration
    ''' in the current EwE model. The grid will be populated from this local
    ''' administration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        Dim iRow As Integer = 0
        Dim cells() As Cells.ICellVirtual = Nothing
        Dim pos As SourceGrid2.Position = Nothing
        Dim vm As VisualModels.Common = Nothing
        Dim ewec As EwECell = Nothing
        Dim grp As cEcoPathGroupInput = Nothing
        Dim taxon As cTaxon = Nothing

        Me.RowsCount = 1

        ' Create rows
        For iGroup As Integer = 1 To Me.Core.nGroups

            iRow = Me.AddRow()

            grp = Me.Core.EcoPathGroupInputs(iGroup)

            Me(iRow, eColumnTypes.GroupIndex) = New EwERowHeaderCell(iGroup)
            Me(iRow, eColumnTypes.GroupName) = New EwERowHeaderCell(grp.Name)
            Me(iRow, eColumnTypes.Proportion) = New EwERowHeaderCell("")
            Me(iRow, eColumnTypes.GroupIndex).Tag = grp

            For iTaxon As Integer = 1 To Me.Core.nTaxon

                taxon = Me.Core.Taxon(iTaxon)
                If taxon.Group = grp.Index Then

                    iRow = Me.AddRow()

                    Me(iRow, eColumnTypes.GroupIndex) = New EwERowHeaderCell("")
                    Me(iRow, eColumnTypes.GroupName) = New EwECell(taxon.Name, GetType(String))
                    Me(iRow, eColumnTypes.Proportion) = New EwECell(taxon.Proportion, GetType(Single))

                End If
            Next
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to configure column widths.
    ''' </summary>
    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.AutoSizeColumnRange(1, Me.ColumnsCount - 1, 1, Me.RowsCount - 1)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; called when a cell has received focus. Overriden to notify
    ''' our parent that the selection has changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnCellGotFocus(ByVal e As SourceGrid2.PositionCancelEventArgs)
        MyBase.OnCellGotFocus(e)
        Me.RaiseSelectionChangeEvent()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; called when a cell has lost focus. Overriden to notify
    ''' our parent that the selection has changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnCellLostFocus(ByVal e As SourceGrid2.PositionCancelEventArgs)
        MyBase.OnCellLostFocus(e)
        Me.Selection.Clear()
        Me.RaiseSelectionChangeEvent()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Brute-force resize the gird if necessary, and repopulate with data from 
    ''' the local administration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub UpdateGrid()


    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the content of the Row with the given index.
    ''' </summary>
    ''' <param name="iRow">The index of the row to refresh.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateRow(ByVal iRow As Integer)


    End Sub



    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Called Update local admin based on cell value changes.
    ''' </summary>
    ''' <returns>
    ''' True if the value change is allowed, False to block the value change.
    ''' </returns>
    ''' <remarks>
    ''' This method differs from OnCellValueEdited; during a cell value 
    ''' change notification (at the end of an edit operation) it is unsafe
    ''' to modify the value of the cell being edited. However, the end edit 
    ''' event will not be triggered for particular specialized cells which
    ''' makes this method mandatory. We once again apologize for the confusion; )
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function OnCellValueChanged(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Called when the user has finished editing a cell. Handled to update 
    ''' local admin based on cell value changes.
    ''' </summary>
    ''' <returns>
    ''' True if the edit operation is allowed, False to cancel the edit operation.
    ''' </returns>
    ''' <remarks>
    ''' This method differs from OnCellValueChanged; at the end of an edit
    ''' operation it is once again safe to alter the value of the cell that was
    ''' just edited for text and combo box controls. *sigh*
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function OnCellEdited(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

        Return True

    End Function

#End Region ' Grid interaction

End Class
