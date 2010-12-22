#Region " Imports "

Option Strict On
Imports EwEPlugin
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Reflection
Imports SourceGrid2

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid showing loaded EwE assembly details.
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class AboutEwEGrid
    Inherits EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Populate the grid with data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        Dim pm As cPluginManager = Nothing
        Dim aanLoaded As AssemblyName() = Nothing
        Dim aanPlugins As AssemblyName() = Nothing
        Dim iRow As Integer = 0

        pm = Me.UIContext.Core.PluginManager()
        aanPlugins = pm.PluginAssemblyNames()
        aanLoaded = cAssemblyUtils.GetSummary(Assembly.GetExecutingAssembly)

        ' Prepare grid
        Me.Redim(aanLoaded.Length + aanPlugins.Length + 2, 2)

        ' Create header cells
        Me(iRow, 0) = New EwEColumnHeaderCell(SharedResources.HEADER_SYSTEMCOMPONENTS)
        Me(iRow, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_VERSION)

        ' Add assembly cells
        iRow += 1
        For Each an As AssemblyName In aanLoaded
            Me(iRow, 0) = New EwERowHeaderCell(an.Name)
            Me(iRow, 1) = New EwECell(an.Version.ToString, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
            ' Next
            iRow += 1
        Next

        ' Plug-ins section
        Me(iRow, 0) = New EwEColumnHeaderCell(SharedResources.HEADER_PLUGINS)
        Me(iRow, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_VERSION)

        ' Add plugin cells
        iRow += 1
        For Each an As AssemblyName In aanPlugins
            Me(iRow, 0) = New EwERowHeaderCell(an.Name)
            Me(iRow, 1) = New EwECell(an.Version.ToString, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
            iRow += 1
        Next

        ' Column 1 w version numbers must be fully visible. Column 0 will occupy the rest of the space
        Me.Columns(0).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.Columns(1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize Or SourceGrid2.AutoSizeMode.EnableStretch
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
            Dim iWidth As Integer = Me.ClientRectangle.Width - Me.Columns(1).Width - 2
            If (Me.VScrollBar IsNot Nothing) Then
                iWidth -= Me.VScrollBar.Width
            End If
            Me.Columns(0).Width = iWidth
        End If
    End Sub

End Class
