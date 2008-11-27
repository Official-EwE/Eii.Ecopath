'==============================================================================
'
' $Log: AboutEwEGrid.vb,v $
' Revision 1.2  2008/11/27 19:45:51  jeroens
' Renamed ApplicationComponents interfaces to more properly reflect their function
'
' Revision 1.1  2008/09/26 07:32:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.7  2008/08/02 03:04:07  jeroens
' Renamed resources
'
' Revision 1.6  2008/06/02 00:07:44  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.5  2007/10/20 22:55:42  jeroens
' * Made designer-safe
'
' Revision 1.4  2007/10/10 16:52:47  jeroens
' + Added plugins
'
' Revision 1.3  2007/09/27 18:04:16  jeroens
' + Uses ApplicationComponents
'
' Revision 1.2  2007/09/17 02:44:41  jeroens
' * Fixed ambiguities
'
' Revision 1.1  2007/07/08 07:37:44  jeroens
' * Moved, revamped
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwECore
Imports EwEPlugin
Imports System.Reflection
Imports SourceGrid2

#End Region ' Imports directive

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid showing loaded EwE assembly details
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class AboutEwEGrid
    Inherits SourceGrid2.Grid

    Public Sub New()

        Dim core As cCore = Nothing
        Dim ac As ApplicationComponents = Nothing
        Dim pm As cPluginManager = Nothing
        Dim aanLoaded As AssemblyName() = Nothing
        Dim aanPlugins As AssemblyName() = Nothing

        core = cCore.GetInstance()

        If (core Is Nothing) Then Return

        ac = AppLauncher.GetInstance().ApplicationComponents()
        pm = core.PluginManager()
        aanLoaded = ac.RequiredComponents()
        aanPlugins = pm.PluginAssemblyNames()

        ' Control face colour to use for grid rows and grid background
        Dim clrBack As Color = SystemColors.Control
        ' Row counter
        Dim iRow As Integer = 0
        ' Cell to populate grid with
        Dim cell As SourceGrid2.Cells.Real.Cell = Nothing
        ' Data model for cell
        Dim dm As New SourceGrid2.DataModels.DataModelBase(GetType(String))
        ' Visual model for cell
        Dim vm As New SourceGrid2.VisualModels.Common(False)

        ' Configure data model
        dm.EnableEdit = False

        ' Configure visual model
        vm.BackColor = clrBack
        vm.FocusBackColor = clrBack
        vm.SelectionBackColor = clrBack

        ' Prepare grid
        Me.Redim(aanLoaded.Length + aanPlugins.Length + 2, 2)

        ' Create header cells
        cell = New Cells.Real.Cell(My.Resources.ABOUT_HEADER_SYSTEMCOMPONENTS, dm)
        cell.VisualModel = New VisualModels.Header()
        Me(iRow, 0) = cell

        cell = New Cells.Real.Cell(My.Resources.ABOUT_HEADER_VERSION, dm)
        cell.VisualModel = New VisualModels.Header()
        Me(iRow, 1) = cell

        ' Add assembly cells
        iRow += 1
        For Each an As AssemblyName In aanLoaded
            cell = New Cells.Real.Cell(an.Name, dm)
            cell.VisualModel = vm
            Me(iRow, 0) = cell

            cell = New Cells.Real.Cell(an.Version.ToString(), dm)
            cell.VisualModel = vm
            Me(iRow, 1) = cell

            ' Next
            iRow += 1
        Next

        ' Create header cells
        cell = New Cells.Real.Cell(My.Resources.HEADER_PLUGINS, dm)
        cell.VisualModel = New VisualModels.Header()
        Me(iRow, 0) = cell

        cell = New Cells.Real.Cell(My.Resources.ABOUT_HEADER_VERSION, dm)
        cell.VisualModel = New VisualModels.Header()
        Me(iRow, 1) = cell

        ' Add plugin cells
        iRow += 1
        For Each an As AssemblyName In aanPlugins
            cell = New Cells.Real.Cell(an.Name, dm)
            cell.VisualModel = vm
            Me(iRow, 0) = cell

            cell = New Cells.Real.Cell(an.Version.ToString(), dm)
            cell.VisualModel = vm
            Me(iRow, 1) = cell

            ' Next
            iRow += 1
        Next

        ' Finalize grid for generic assimilation
        Me.Dock = DockStyle.Fill
        Me.BackColor = clrBack
        Me.BorderStyle = BorderStyle.Fixed3D

        ' Column 1 w version numbers must be fully visible. Column 0 will occupy the rest of the space
        Me.Columns(0).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.Columns(1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize Or SourceGrid2.AutoSizeMode.EnableStretch

    End Sub

    ''' <summary>
    ''' Grid resize: resize the columns
    ''' </summary>
    Private Sub AboutEwEGrid_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.AutoSizeAll()
        Me.Columns(0).Width = Me.ClientRectangle.Width - Me.Columns(1).Width
    End Sub

End Class
