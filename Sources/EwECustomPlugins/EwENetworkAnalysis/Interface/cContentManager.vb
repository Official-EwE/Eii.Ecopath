'==============================================================================
'
' $Log: cContentManager.vb,v $
' Revision 1.8  2009/05/30 00:00:49  jeroens
' Toolstrip usage centralized
'
' Revision 1.7  2009/05/28 14:56:15  jeroens
' Responds to styleguide changes by updating content of content managers that display the graph or the grid
'
' Revision 1.6  2009/05/28 13:59:57  jeroens
' Fixed annual averages option in CSV export
'
' Revision 1.5  2009/05/28 12:37:06  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.4  2009/05/19 13:41:05  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.3  2009/05/11 20:34:34  jeroens
' Added monthly / annual averages CVS export
'
' Revision 1.2  2009/05/02 01:46:02  jeroens
' Added HideControls
' Added Filename
'
' Revision 1.1  2009/05/01 17:41:42  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.DataSources
Imports ScientificInterfaceShared.Style
Imports ZedGraph

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Class for populating NA view controls.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class cContentManager

#Region " Private variables "

    ''' <summary></summary>
    Private m_manager As cNetworkManager = Nothing
    ''' <summary></summary>
    Private m_graph As ZedGraphControl = Nothing
    ''' <summary></summary>
    Private m_plot As ucPlot = Nothing
    ''' <summary></summary>
    Private m_datagrid As DataGridView = Nothing
    ''' <summary></summary>
    Private m_toolstrip As ToolStrip = Nothing
    ''' <summary></summary>
    Private m_sg As cStyleGuide = Nothing

#End Region ' Private variables

#Region " Attach / detach "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="manager"></param>
    ''' <param name="datagrid"></param>
    ''' <param name="graph"></param>
    ''' <param name="plot"></param>
    ''' <remarks>
    ''' The default implementation stores all controls and hides them.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Attach(ByVal manager As cNetworkManager, _
                                       ByVal datagrid As DataGridView, _
                                       ByVal graph As ZedGraphControl, _
                                       ByVal plot As ucPlot, _
                                       ByVal toolstrip As ToolStrip) As Boolean

        ' Store all references
        Me.m_manager = manager
        Me.m_datagrid = datagrid
        Me.m_graph = graph
        Me.m_plot = plot
        Me.m_toolstrip = toolstrip

        Me.m_sg = cStyleGuide.GetInstance()
        AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

        ' Hide all managed controls
        Me.HideControls()

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' The default implementation hides all controls and then releases them.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub Detach()

        ' Hide all controls
        Me.HideControls()

        RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
        Me.m_sg = Nothing

        Me.m_manager = Nothing
        Me.m_datagrid = Nothing
        Me.m_graph = Nothing
        Me.m_plot = Nothing
        Me.m_toolstrip = Nothing

    End Sub

#End Region ' Attach / detach

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride Sub DisplayData()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update a 'view' to a selection.
    ''' </summary>
    ''' <param name="iGroup1">One-based EwE group index of the first selected 
    ''' group, if any.</param>
    ''' <param name="iGroup2">One-based EwE group index of the second selected 
    ''' group, if any.</param>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub UpdateData(ByVal iGroup1 As Integer, ByVal iGroup2 As Integer)
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear managed data.
    ''' </summary>
    ''' <remarks>
    ''' The default implementation hides all controls.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub ClearData()
        ' Hide all controls
        Me.HideControls()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the default file name - without extension - for saving the data 
    ''' managed here to a file of any type.
    ''' </summary>
    ''' <remarks>
    ''' Default implementation does not return a file name.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Filename(ByVal bAnnual As Boolean) As String
        Return ""
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Implement to save the content of a view to a EMF file.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub SaveToEMF(ByVal strFileName As String)
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Flag stating whether the data being displayed has a time component.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property IsDataOverTime() As Boolean
        Get
            Return False
        End Get
    End Property

#End Region ' Overrides

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the one and only network manager.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property NetworkManager() As cNetworkManager
        Get
            Return Me.m_manager
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the one and only data grid view control.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property Grid() As DataGridView
        Get
            Return Me.m_datagrid
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the one and only graph control.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property Graph() As ZedGraphControl
        Get
            Return Me.m_graph
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the one and only plot control.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property Plot() As ucPlot
        Get
            Return Me.m_plot
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the one and only tool strip.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property Toolstrip() As ToolStrip
        Get
            Return Me.m_toolstrip
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the one and only style guide.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property StyleGuide() As cStyleGuide
        Get
            Return Me.m_sg
        End Get
    End Property

#End Region ' Properties

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Hide all controls. Override this to do your own magic.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub HideControls()

        ' Hide all controls
        Me.Graph.Visible = False
        Me.Plot.Visible = False
        Me.Grid.Visible = False
        Me.Plot.Visible = False
        Me.Toolstrip.Visible = False

        ' Clear grid
        Me.Grid.Rows.Clear()
        Me.Grid.Columns.Clear()
        Me.Grid.ReadOnly = True

        ' Hide toolstrip items
        For Each tsi As ToolStripItem In Me.Toolstrip.Items
            tsi.Visible = False
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, responding to styleguide changes.
    ''' </summary>
    ''' <param name="cf"></param>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub OnStyleGuideChanged(ByVal cf As cStyleGuide.eChangeType)
        If Me.Graph.Visible Or Me.Grid.Visible Then
            Me.DisplayData()
        End If
    End Sub

    Protected Sub ToolstripShowGroups(Optional ByVal strLabel1 As String = "", _
                                      Optional ByVal strLabel2 As String = "")

        Dim tslbl1 As ToolStripItem = Me.Toolstrip.Items("tslblSelection1")
        Dim tslbl2 As ToolStripItem = Me.Toolstrip.Items("tslblSelection2")
        Dim tscmb1 As ToolStripItem = Me.Toolstrip.Items("tscmbSelection1")
        Dim tscmb2 As ToolStripItem = Me.Toolstrip.Items("tscmbSelection2")

        tslbl1.Text = strLabel1
        tslbl1.Visible = Not String.IsNullOrEmpty(strLabel1)
        tscmb1.Visible = Not String.IsNullOrEmpty(strLabel1)

        tslbl2.Text = strLabel2
        tslbl2.Visible = Not String.IsNullOrEmpty(strLabel2)
        tscmb2.Visible = Not String.IsNullOrEmpty(strLabel2)

    End Sub

    Protected Sub ToolstripShowOptionCSV(Optional ByVal bShow As Boolean = True)
        Dim tsi As ToolStripItem = Me.Toolstrip.Items("tsbtnOutputIndicesCSV")
        tsi.Visible = bShow
    End Sub

    Protected Sub ToolstripShowOptionEMF(Optional ByVal bShow As Boolean = True)
        Dim tsi As ToolStripItem = Me.Toolstrip.Items("tsbtnOutputGraphEMF")
        tsi.Visible = bShow
    End Sub

#End Region ' Internals

End Class
