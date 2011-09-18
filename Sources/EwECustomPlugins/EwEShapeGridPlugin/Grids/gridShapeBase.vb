#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.DataModels

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Foundation class for showing <see cref="cShapeData"/> in a grid.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class gridShapeBase
    Inherits EwEGrid

    Private m_bIsSeasonal As Boolean = False
    Private m_lInvalidatedShapes As New List(Of cShapeData)

    Public Sub New()
    End Sub

    Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
            If (MyBase.UIContext IsNot Nothing) Then
                RemoveHandler Me.Handler.OnRefreshed, AddressOf OnRefreshed
                Me.Handler.Detach()
            End If
            MyBase.UIContext = value
            If (MyBase.UIContext IsNot Nothing) Then
                Me.Handler.Attach(Me.UIContext, Nothing, Nothing, Nothing, Nothing)
                AddHandler Me.Handler.OnRefreshed, AddressOf OnRefreshed
            End If
        End Set
    End Property

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()
        Me.FixedColumnWidths = False
        Me.FixedColumns = 1
    End Sub

    Public Property IsSeasonal() As Boolean
        Get
            Return Me.m_bIsSeasonal
        End Get
        Set(ByVal value As Boolean)
            If (Me.m_bIsSeasonal = value) Then Return
            Me.m_bIsSeasonal = value
            Me.RefreshContent()
        End Set
    End Property

    Public MustOverride ReadOnly Property Handler() As cShapeGUIHandler
    Public MustOverride ReadOnly Property Manager() As IEnumerable
    Protected MustOverride Sub OnRefreshed(ByVal sender As cShapeGUIHandler)

    Public ReadOnly Property Shapes() As EwECore.cShapeData()
        Get
            Dim lShapes As New List(Of cShapeData)
            Dim shape As cShapeData = Nothing
            Dim man As IEnumerable = Me.Manager

            If man IsNot Nothing Then
                For Each item As Object In man
                    If TypeOf item Is cShapeData Then
                        shape = DirectCast(item, cShapeData)
                        If shape.IsSeasonal = Me.IsSeasonal Then
                            lShapes.Add(shape)
                        End If
                    End If
                Next
            End If

            Return lShapes.ToArray
        End Get
    End Property

    Protected Property Shape(ByVal iCol As Integer) As cShapeData
        Get
            Return DirectCast(Me.Columns(iCol).Tag, cShapeData)
        End Get
        Set(ByVal value As cShapeData)
            Me.Columns(iCol).Tag = value
        End Set
    End Property

    Protected Function CreateComboCell(ByVal value As Object, ByVal aiValuesOrg() As Integer, ByVal astrValuesDisplay As String()) As Cell

        Dim editor As New EditorComboBox(value.GetType)
        Dim mapping As New SourceLibrary.ComponentModel.Validator.ValueMapping()

        editor.StandardValues = aiValuesOrg
        editor.StandardValuesExclusive = True
        editor.AllowStringConversion = False

        mapping.ValueList = aiValuesOrg
        mapping.DisplayStringList = astrValuesDisplay
        mapping.BindValidator(editor)

        Return New Cell(value, editor)

    End Function

    Protected Sub InvalidateShape(ByVal iCol As Integer)
        Me.InvalidateShape(Me.Shape(iCol))
    End Sub

    Protected Sub InvalidateShape(ByVal shape As cShapeData)

        If Not Me.m_lInvalidatedShapes.Contains(shape) Then
            Me.m_lInvalidatedShapes.Add(shape)
        End If

    End Sub

    Public Overrides Sub BeginBatchEdit()
        Me.m_lInvalidatedShapes.Clear()
        MyBase.BeginBatchEdit()
    End Sub

    Public Overrides Sub EndBatchEdit()
        Try
            For Each sh As cShapeData In Me.m_lInvalidatedShapes
                sh.Update()
            Next
        Catch ex As Exception

        End Try
        Me.m_lInvalidatedShapes.Clear()
        MyBase.EndBatchEdit()
    End Sub

End Class
