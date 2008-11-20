'==============================================================================
'
' $Log: gridLayerData.vb,v $
' Revision 1.2  2008/11/20 15:18:29  jeroens
' Layer ReadOnly state properly handled
'
' Revision 1.1  2008/11/04 04:58:44  jeroens
' Renamed
'
' Revision 1.2  2008/10/10 18:04:02  jeroens
' Updated to renamed layers classes
'
' Revision 1.1  2008/09/26 07:31:59  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

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

#Region " Helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Administrative unit representing a <see cref="cEcospaceLayerImportance">Importance layer</see>
    ''' in the EwE model.
    ''' </summary>
    ''' <remarks>
    ''' This class can represent existing and new Layers. If this class has its
    ''' <see cref="LayerInfo.Layer">Layer</see> parameter set, a real live
    ''' Layer is represented. If this parameter is not set, a new Layer is
    ''' represented.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Class LayerInfo

        ''' <summary><see cref="cEcospaceLayerImportance">cEcospaceLayerImportance</see> associated with this Layer, if any.</summary>
        Private m_Layer As cEcospaceLayerImportance = Nothing
        ''' <summary>Name for this Layer.</summary>
        Private m_strName As String = ""
        ''' <summary>Description for this Layer.</summary>
        Private m_strDescription As String = ""
        ''' <summary>Weight for this Layer.</summary>
        Private m_sWeight As Single = 0.0
        ''' <summary>Flag stating whether a user action is confirmed</summary>
        Private m_bConfirmed As Boolean = True
        ''' <summary>The status of a Layer in the interface.</summary>
        Private m_status As AddRemoveItemStatus = AddRemoveItemStatus.Original

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="Layer">The <see cref="cEcospaceLayerImportance">cEcospaceLayerImportance</see> to
        ''' initialize this instance from. If set, this instance represents a
        ''' Layer currently active in the EwE model.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Layer As cEcospaceLayerImportance)
            Debug.Assert(Layer IsNot Nothing)
            Me.m_Layer = Layer
            Me.m_strName = Layer.Name
            Me.m_strDescription = Layer.Description
            Me.m_sWeight = Layer.Weight
            Me.m_status = AddRemoveItemStatus.Original
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="strName">Name to assign to this administrative unit.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strName As String, ByVal strDescription As String, ByVal sWeight As Single)
            Me.m_Layer = Nothing
            Me.m_strName = strName
            Me.m_strDescription = strDescription
            Me.m_sWeight = sWeight
            Me.m_status = AddRemoveItemStatus.Added
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the name of this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Name() As String
            Get
                Return Me.m_strName
            End Get
            Set(ByVal value As String)
                Me.m_strName = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the description of this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Description() As String
            Get
                Return Me.m_strDescription
            End Get
            Set(ByVal value As String)
                Me.m_strDescription = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the weight of this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Weight() As Single
            Get
                Return Me.m_sWeight
            End Get
            Set(ByVal value As Single)
                Me.m_sWeight = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cEcospaceLayerImportance">EwE Layer</see> associated
        ''' with this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Layer() As cEcospaceLayerImportance
            Get
                Return Me.m_Layer
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="AddRemoveItemStatus">add/remove item status</see>
        ''' for the layer object.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Status() As AddRemoveItemStatus
            Get
                Return Me.m_status
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the user has confirmed an action on this object.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Confirmed() As Boolean
            Get
                Return Me.m_bConfirmed
            End Get
            Set(ByVal value As Boolean)
                Me.m_bConfirmed = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the Layer has changed.
        ''' </summary>
        ''' <returns>
        ''' True when Layer <see cref="Name">Name</see> value has changed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function IsChanged() As Boolean
            If (Me.IsNew()) Then Return False
            Return (Me.m_Layer.Name <> Me.m_strName) Or _
                   (Me.Layer.Description <> Me.m_strDescription) Or _
                   (Me.Layer.Weight <> Me.m_sWeight)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the Layer is to be created.
        ''' </summary>
        ''' <returns>
        ''' True when Layer <see cref="Name">Name</see> value has changed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function IsNew() As Boolean
            Return (Me.m_Layer Is Nothing)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this layer is flagged for deletion. Toggling this flag
        ''' will update the <see cref="Status">Status</see> of the item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FlaggedForDeletion() As Boolean
            Get
                Return Me.m_status = AddRemoveItemStatus.Removed
            End Get
            Set(ByVal bDelete As Boolean)
                If Me.m_Layer IsNot Nothing Then
                    If bDelete Then
                        Me.m_status = AddRemoveItemStatus.Removed
                    Else
                        Me.m_status = AddRemoveItemStatus.Original
                    End If
                Else
                    If bDelete Then
                        Me.m_status = AddRemoveItemStatus.Invalid
                    Else
                        Me.m_status = AddRemoveItemStatus.Added
                    End If
                End If
            End Set
        End Property

    End Class

#End Region ' Helper classes

    ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
    ''' to trap cell edit events locally in this grid. These events are essential
    ''' for keeping the local Layer administration up to date.</summary>
    Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)

    Private m_core As cCore = Nothing
    Private m_layer As cLayer = Nothing

    Public Sub New()
        MyBase.New()
        Me.m_core = cCore.GetInstance()
    End Sub

    Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
        Return DockStyle.None
    End Function

    Protected Overrides Sub InitLayout()
        If Me.m_layer Is Nothing Then Return
        Me.Redim(Me.m_layer.Data.InRow + 1, Me.m_layer.Data.InCol + 1)

        Me.FixedColumns = 1
        Me.FixedColumnWidths = False

        MyBase.InitLayout()
    End Sub

    Protected Overrides Sub InitStyle()

        Dim data As cEcospaceLayer = Nothing

        MyBase.InitStyle()

        If Me.m_layer Is Nothing Then Return

        ' Grab the data
        data = Me.m_layer.Data

        Me.Redim(1, data.InCol + 1)
        Me(0, 0) = New EwEColumnHeaderCell("")
        For iCol As Integer = 1 To data.InCol
            Me(0, iCol) = New EwEColumnHeaderCell(CStr(iCol))
        Next

        Me.FixedColumns = 1

        If Me.m_layer.Editor IsNot Nothing Then
            Me.Enabled = (Me.Layer.Editor.IsReadOnly() = False)
        End If

    End Sub

    Protected Overrides Sub FillData()

        Dim cell As EwECell = Nothing
        Dim tCell As Type = Nothing
        Dim data As cEcospaceLayer = Nothing
        'Dim dataDepth As cEcospaceLayer = Me.m_core.EcospaceBasemap.LayerDepth

        ' Sanity check
        If Me.m_layer Is Nothing Then Return

        ' Grab the data
        data = Me.m_layer.Data
        ' Grab the type of the data
        If TypeOf data Is cEcospaceIntegerNxNLayer Then
            tCell = GetType(Integer)
        Else
            ' Assume single
            tCell = GetType(Single)
        End If

        ' Prepare grid
        Me.RowsCount = 1

        ' Create cells
        For iRow As Integer = 1 To data.InRow
            ' Add row
            Me.AddRow()
            ' Add row header cell
            Me(iRow, 0) = New EwERowHeaderCell(CStr(iRow))
            ' Add row value cells
            For iCol As Integer = 1 To data.InCol
                ' Properly prepare cell
                If tCell Is GetType(Integer) Then
                    cell = New EwECell(CInt(data.Cell(iRow, iCol)), tCell)
                Else
                    cell = New EwECell(CSng(data.Cell(iRow, iCol)), tCell)
                End If
                cell.Behaviors.Add(Me.m_bm)
                cell.SuppressZero(cCore.NULL_VALUE) = True
                '' Highlight land cells
                'If dataDepth.Cell(iRow, iCol) = 0 Then
                '    cell.Style = StyleGuide.eStyleFlags.Checked
                'Else
                cell.Style = StyleGuide.eStyleFlags.OK
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

        For iRow As Integer = 1 To m_layer.Data.InRow
            For iCol As Integer = 1 To layTarget.Data.InCol
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
