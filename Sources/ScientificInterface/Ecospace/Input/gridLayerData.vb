#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterface.Ecospace
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SourceGrid2
Imports ScientificInterface.Ecospace.Basemap.Layers

#End Region ' Imports

<CLSCompliant(False)> _
Public Class gridLayerData
    Inherits EwEGrid

    ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
    ''' to trap cell edit events locally in this grid. These events are essential
    ''' for keeping the local Layer administration up to date.</summary>
    Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
    Private m_basemap As cEcospaceBasemap = Nothing
    Private m_layer As cLayer = Nothing

    Public Sub New()
        MyBase.New()
        Me.TrackPropertySelection = False
    End Sub

    Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
            If (Me.UIContext IsNot Nothing) Then
                Me.m_basemap = Nothing
            End If
            MyBase.UIContext = value
            If (Me.UIContext IsNot Nothing) Then
                Me.m_basemap = value.Core.EcospaceBasemap
            End If
        End Set
    End Property

    Protected Overrides Sub InitLayout()
        If Me.m_layer Is Nothing Then Return
        Me.Redim(Me.m_basemap.InRow + 1, Me.m_basemap.InCol + 1)

        Me.FixedColumns = 1
        Me.FixedColumnWidths = False

        MyBase.InitLayout()
    End Sub

    Protected Overrides Sub InitStyle()

        Dim data As cEcospaceLayer = Nothing

        MyBase.InitStyle()

        ' Test for UI context to prevent core from being accessed
        If (Me.UIContext Is Nothing) Then Return
        If (Me.m_layer Is Nothing) Then Return

        ' Grab the data
        data = Me.m_layer.Data

        Me.Redim(1, Me.m_basemap.InCol + 1)
        Me(0, 0) = New EwEColumnHeaderCell("")
        For iCol As Integer = 1 To Me.m_basemap.InCol
            Me(0, iCol) = New EwEColumnHeaderCell(CStr(iCol))
        Next

        Me.FixedColumns = 1

        If Me.m_layer.Editor IsNot Nothing Then
            Me.Enabled = Me.Layer.Editor.IsEditable()
        End If

    End Sub

    Protected Overrides Sub FillData()

        Dim cell As Cells.ICell = Nothing
        Dim tCell As Type = Nothing
        Dim data As cEcospaceLayer = Nothing
        'Dim dataDepth As cEcospaceLayer = Me.m_core.EcospaceBasemap.LayerDepth

        ' Sanity check
        If Me.m_layer Is Nothing Then Return

        ' Grab the data
        data = Me.m_layer.Data
        ' Grab the type of the data
        If TypeOf data Is cEcospaceLayerIntegerNxM Then
            tCell = GetType(Integer)
        Else
            ' Assume single
            tCell = GetType(Single)
        End If

        ' Prepare grid
        Me.RowsCount = 1

        ' Create cells
        For iRow As Integer = 1 To Me.m_basemap.InRow
            ' Add row
            Me.AddRow()
            ' Add row header cell
            Me(iRow, 0) = New EwERowHeaderCell(CStr(iRow))
            ' Add row value cells
            For iCol As Integer = 1 To Me.m_basemap.InCol
                ' Properly prepare cell
                If tCell Is GetType(Integer) Then
                    cell = New Cells.Real.Cell(CInt(data.Cell(iRow, iCol)), tCell)
                Else
                    cell = New EwECell(CSng(data.Cell(iRow, iCol)), tCell)
                End If
                cell.Behaviors.Add(Me.m_bm)
                'cell.SuppressZero(cCore.NULL_VALUE) = True
                '' Highlight land cells
                'If dataDepth.Cell(iRow, iCol) = 0 Then
                '    cell.Style = StyleGuide.eStyleFlags.Checked
                'Else
                'cell.Style = cStyleGuide.eStyleFlags.OK
                'End If
                Me(iRow, iCol) = cell
            Next iCol
        Next iRow

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the layer to display in the grid
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Layer() As cLayer
        Get
            Return Me.m_layer
        End Get
        Set(ByVal value As cLayer)
            If Not Object.ReferenceEquals(Me.m_layer, value) Then
                Me.m_layer = value
                Me.RefreshContent()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply the grid data
    ''' </summary>
    ''' <param name="layTarget"></param>
    ''' <returns>True when the layer data was changed.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Apply(Optional ByVal layTarget As cLayer = Nothing) As Boolean
        Dim p As SourceGrid2.Position = Nothing
        Dim sNew As Single = 0.0!
        Dim sOrg As Single = 0.0!
        Dim data As cEcospaceLayer = Nothing
        Dim bChanged As Boolean = False

        If Me.m_layer.Editor IsNot Nothing Then
            If (Me.m_layer.Editor.IsReadOnly() = True) Then
                Return False
            End If
        End If

        If (layTarget Is Nothing) Then layTarget = Me.m_layer
        If (layTarget Is Nothing) Then Return False

        data = layTarget.Data

        For iRow As Integer = 1 To Me.m_basemap.InRow
            For iCol As Integer = 1 To Me.m_basemap.InCol
                ' Get original value
                sOrg = data.Cell(iRow, iCol)
                ' Get grid value
                p = New SourceGrid2.Position(iRow, iCol)
                sNew = CSng(Me(iRow, iCol).GetValue(p))
                ' Has the user modified this value?
                If (sNew <> sOrg) Then
                    ' #Yes: set it
                    data.Cell(iRow, iCol) = sNew
                    ' Remember the change
                    bChanged = True
                End If
            Next iCol
        Next iRow

        Return bChanged

    End Function

End Class
