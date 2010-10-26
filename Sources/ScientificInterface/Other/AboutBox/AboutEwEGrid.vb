#Region " Imports "

Option Strict On
Imports EwEPlugin
Imports EwEUtils.Utilities
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
        Me(iRow, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_SYSTEMCOMPONENTS)
        Me(iRow, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_VERSION)

        ' Add assembly cells
        iRow += 1
        For Each an As AssemblyName In aanLoaded
            Me(iRow, 0) = New EwERowHeaderCell(an.Name)
            Me(iRow, 1) = New EwECell(an.Version.ToString, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
            ' Next
            iRow += 1
        Next

        ' Plug-ins section
        Me(iRow, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_PLUGINS)
        Me(iRow, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_VERSION)

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
        Me.AutoSizeAll()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid resize: resize the columns
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)

        Dim iWidth As Integer = 0

        Me.AutoSizeAll()

        If Me.ColumnsCount > 0 Then
            Me.Columns(0).Width = Me.ClientRectangle.Width - Me.Columns(1).Width - Me.VScrollBar.Width - 1
        End If
    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.FixedColumnWidths = False
    End Sub

End Class
