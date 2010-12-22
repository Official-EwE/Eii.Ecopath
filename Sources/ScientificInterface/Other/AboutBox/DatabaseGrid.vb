#Region " Imports "

Option Strict On
Imports EwEPlugin
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Reflection
Imports SourceGrid2
Imports EwEUtils.Database
Imports EwECore.DataSources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid showing EwE database details.
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class DatabaseGrid
    Inherits EwEGrid

    Protected Class cDescriptionVisualizer
        Inherits cEwECellVisualizer

        Public Sub New()
            MyBase.New()
            Me.TextAlignment = ContentAlignment.MiddleLeft
        End Sub

    End Class

    Private m_viz As New cDescriptionVisualizer()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Populate the grid with data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        If (Not Me.UIContext.Core.StateMonitor.HasEcopathLoaded) Then Return

        Dim ds As IEwEDataSource = Me.UIContext.Core.DataSource
        Dim db As cEwEDatabase = Nothing
        Dim aHistory As cEwEDatabase.cHistoryItem() = Nothing
        Dim item As cEwEDatabase.cHistoryItem = Nothing
        Dim iRow As Integer = 0

        If Not TypeOf ds.Connection Is cEwEDatabase Then Return

        db = DirectCast(ds.Connection, cEwEDatabase)
        aHistory = db.GetHistory

        ' Prepare grid
        Me.Redim(aHistory.Length + 1, 3)

        ' Create header cells
        Me(iRow, 0) = New EwEColumnHeaderCell(SharedResources.HEADER_VERSION)
        Me(iRow, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_DATE)
        Me(iRow, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_DESCRIPTION)
        iRow += 1

        ' Add history items
        For iHist As Integer = 0 To aHistory.Length - 1
            item = aHistory(iHist)
            Me(iRow, 0) = New EwECell(item.Version, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
            Me(iRow, 1) = New EwECell(item.Date.ToShortDateString(), GetType(String), cStyleGuide.eStyleFlags.NotEditable)
            Me(iRow, 2) = New EwECell(item.Comments, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
            Me(iRow, 2).VisualModel = Me.m_viz
            ' Next
            iRow += 1
        Next

        Me.Columns(0).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize Or SourceGrid2.AutoSizeMode.EnableStretch
        Me.Columns(1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize Or SourceGrid2.AutoSizeMode.EnableStretch
        Me.Columns(2).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.FitColumns()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid resize: resize the columns
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)
        Me.FitColumns()
    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.FixedColumnWidths = False
    End Sub

    Private Sub FitColumns()
        If Me.ColumnsCount > 0 Then
            Me.AutoSizeAll()
            Dim iWidth As Integer = Me.ClientRectangle.Width - Me.Columns(0).Width - Me.Columns(1).Width - 2
            If (Me.VScrollBar IsNot Nothing) Then
                iWidth -= Me.VScrollBar.Width
            End If
            Me.Columns(2).Width = iWidth
        End If
    End Sub

End Class
