'==============================================================================
'
' $Log: cContentManager.vb,v $
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
    Public Overridable Sub Attach(ByVal manager As cNetworkManager, _
                                  ByVal datagrid As DataGridView, _
                                  ByVal graph As ZedGraphControl, _
                                  ByVal plot As ucPlot)

        ' Store all references
        Me.m_manager = manager
        Me.m_datagrid = datagrid
        Me.m_graph = graph
        Me.m_plot = plot

        ' Hide all managed controls
        Me.HideControls()

    End Sub

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

        Me.m_manager = Nothing
        Me.m_datagrid = Nothing
        Me.m_graph = Nothing
        Me.m_plot = Plot

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
    ''' States whether a view requires a toolstrip.
    ''' </summary>
    ''' <returns>True if a toolstrip is required.</returns>
    ''' <remarks>
    ''' The default implementation will not require a toolstrip.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overridable Function RequiresToolstrip() As Boolean
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update toolstrip controls
    ''' </summary>
    ''' <param name="ts"></param>
    ''' <remarks>
    ''' The default implementation will hide all toolstrip controls.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub SetupToolstrip(ByVal ts As ToolStrip)
        For Each tsi As ToolStripItem In ts.Items
            tsi.Visible = False
        Next
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
    ''' Implement to save the content of a view to a CSV file.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub SaveToCSV(ByVal strFileName As String, ByVal bAnnual As Boolean)
        ' NOP
    End Sub

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

        ' Clear grid
        Me.Grid.Rows.Clear()
        Me.Grid.Columns.Clear()
        Me.Grid.ReadOnly = True

    End Sub

#End Region ' Internals

End Class
