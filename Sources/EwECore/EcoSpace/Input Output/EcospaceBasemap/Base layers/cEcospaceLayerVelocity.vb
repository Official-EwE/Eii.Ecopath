' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Core



''' ---------------------------------------------------------------------------
''' <summary>
''' Base layer providing access to Ecospace data as cells, each representing a
''' vector with a X and Y component.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class cEcospaceLayerVelocity
    Inherits cEcospaceLayer

#Region " Private variables "

    ''' <summary>Layer max velocity value.</summary>
    Protected m_sMam_xvel As Single = 0.0!
    ''' <summary>Layer min velocity value.</summary>
    Protected m_sMinValue As Single = 0.0!
    ''' <summary>Layer num of cells with a value.</summary>
    Private m_iNumValueCells As Integer = 0

    Protected m_xvel(cCore.N_MONTHS) As cEcospaceLayerSingle
    Protected m_yvel(cCore.N_MONTHS) As cEcospaceLayerSingle

#End Region ' Private variables

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer of vectors that derives its data and identity 
    ''' from a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(theCore As cCore, manager As IEcospaceLayerManager, strName As String, vn As eVarNameFlags)
        MyBase.New(theCore, Nothing, strName, GetType(Single), Nothing, vn)

        Me.m_manager = manager
        For i As Integer = 1 To cCore.N_MONTHS
            Me.m_xvel(i) = New cEcospaceLayerSingle(Me.m_core,
                                                    DirectCast(Me.Manager.LayerData(Me.VarName, 1), Single()(,))(i),
                                                    My.Resources.CoreDefaults.CORE_DEFAULT_X_VELOCITY)
            Me.m_yvel(i) = New cEcospaceLayerSingle(Me.m_core,
                                                    DirectCast(Me.Manager.LayerData(Me.VarName, 2), Single()(,))(i),
                                                    My.Resources.CoreDefaults.CORE_DEFAULT_Y_VELOCITY)
        Next

    End Sub

#End Region ' Construction

#Region " Cell interaction "

    Public Property Month As Integer
        Get
            Return Me.SecundaryIndex
        End Get
        Set(value As Integer)
            Me.SecundaryIndex = value
        End Set
    End Property

    ''' <summary>
    ''' Get/set a cell value in the form of Single(2), where index 0 represents
    ''' the X velocity, and index 1 represents the Y velocity of the value.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <param name="iCol"></param>
    ''' <param name="iIndexSec">ignored</param>
    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Return New Single() {Me.XVelocity(iRow, iCol), Me.YVelocity(iRow, iCol)}
        End Get
        Set(value As Object)
            Dim asValues As Single() = DirectCast(value, Single())
            Me.XVelocity(iRow, iCol) = asValues(0)
            Me.YVelocity(iRow, iCol) = asValues(1)
        End Set
    End Property

    Public Property XVelocity(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Single
        Get
            If iIndexSec = cCore.NULL_VALUE Then iIndexSec = Me.SecundaryIndex
            Return CSng(Me.m_xvel(iIndexSec).Cell(iRow, iCol))
        End Get
        Set(value As Single)
            If iIndexSec = cCore.NULL_VALUE Then iIndexSec = Me.SecundaryIndex
            Me.m_xvel(iIndexSec).Cell(iRow, iCol) = value
        End Set
    End Property

    Public Property YVelocity(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Single
        Get
            If iIndexSec = cCore.NULL_VALUE Then iIndexSec = Me.SecundaryIndex
            Return CSng(Me.m_yvel(iIndexSec).Cell(iRow, iCol))
        End Get
        Set(value As Single)
            If iIndexSec = cCore.NULL_VALUE Then iIndexSec = Me.SecundaryIndex
            Me.m_yvel(iIndexSec).Cell(iRow, iCol) = value
        End Set
    End Property

    ''' <summary>
    ''' Get the max magnitude of all cells in the layer.
    ''' </summary>
    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            If Me.m_bInvalidateStats Then Me.RecalcStats()
            Return Me.m_sMam_xvel
        End Get
    End Property

    ''' <summary>
    ''' Get the min magnitude of all cells in the layer.
    ''' </summary>
    Public Overrides ReadOnly Property MinValue() As Single
        Get
            If Me.m_bInvalidateStats Then Me.RecalcStats()
            Return Me.m_sMinValue
        End Get
    End Property

    ''' <inheritdocs cref="cEcospaceLayer.NumValueCells"/>
    Public Overrides ReadOnly Property NumValueCells As Integer
        Get
            If Me.m_bInvalidateStats Then Me.RecalcStats()
            Return Me.m_iNumValueCells
        End Get
    End Property

    Public ReadOnly Property VelocityLayers() As cEcospaceLayerSingle()
        Get
            Return New cEcospaceLayerSingle() {Me.m_xvel(Me.SecundaryIndex), Me.m_yvel(Me.SecundaryIndex)}
        End Get
    End Property

    Public Overrides Sub Invalidate()
        Me.m_bInvalidateStats = True
    End Sub

    Protected Overrides Function ValidateCellValue(value As Object) As Boolean
        Return True
    End Function

    Protected Overridable Sub RecalcStats()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim s As Single = 0.0!
        Dim sTot As Single = 0
        Dim iRows As Integer = bm.InRow
        Dim iCols As Integer = bm.InCol

        Debug.Assert(Me.Manager IsNot Nothing)

        Me.m_sMam_xvel = Single.MinValue
        Me.m_sMinValue = Single.MaxValue
        Me.m_iNumValueCells = 0

        For iRow As Integer = 1 To iRows
            For iCol As Integer = 1 To iCols
                If (bm.IsModelledCell(iRow, iCol)) Then
                    Dim dx As Single = Me.XVelocity(iRow, iCol)
                    Dim dy As Single = Me.YVelocity(iRow, iCol)
                    If (dx <> cCore.NULL_VALUE And dy <> cCore.NULL_VALUE) Then
                        s = CSng(Math.Sqrt(dx * dx + dy * dy))
                        Me.m_sMam_xvel = Math.Max(s, Me.m_sMam_xvel)
                        Me.m_sMinValue = Math.Min(s, Me.m_sMinValue)
                        Me.m_iNumValueCells += 1
                        sTot += s
                    End If
                End If
            Next iCol
        Next iRow

        'If (Me.m_iNumValueCells > 0) Then
        '    Me.m_sMeanValue = sTot / Me.m_iNumValueCells
        'Else
        '    Me.m_sMeanValue = cCore.NULL_VALUE
        'End If

        Me.m_bInvalidateStats = False

    End Sub
#End Region ' Cell interaction

End Class

