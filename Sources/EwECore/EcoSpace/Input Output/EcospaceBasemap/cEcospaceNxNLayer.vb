#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Base layer providing access to Ecospace data as cells of integer values.
''' </summary>
Public Class cEcospaceLayerIntegerNxM
    Inherits cEcospaceLayer

#Region " Private variables "

    Private m_iMinValue As Integer = 0
    Private m_iMaxValue As Integer = 0
    ''' <summary>States whether min/max should be recalculated</summary>
    ''' <remarks>True at startup to make sure that min/max are properly calculated
    ''' when first queried.</remarks>
    Private m_bInvalidateMinMax As Boolean = True

#End Region ' Private variables

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a migration layer that derives its data and identity from 
    ''' a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' <param name="varName"></param>
    ''' <param name="iIndex"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, _
                   ByVal manager As cEcospaceBasemap, _
                   ByVal varName As eVarNameFlags, _
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE, _
                   Optional ByVal meta As cVariableMetaData = Nothing)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, varName, iIndex, GetType(Integer), meta)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a NxN layer that derives its data from a manager, but
    ''' that is a unique data entity in the EwE core.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="iDBID"></param>
    ''' <param name="manager"></param>
    ''' <param name="varName"></param>
    ''' <param name="iIndex"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, _
                   ByVal iDBID As Integer, _
                   ByVal manager As cEcospaceBasemap, _
                   ByVal varName As eVarNameFlags, _
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE, _
                   Optional ByVal meta As cVariableMetaData = Nothing)

        MyBase.New(theCore, iDBID, manager, varName, iIndex, GetType(Integer), meta)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a NxN layer that is hard-linked to an array of data.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="data"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, _
                   ByVal data As Integer(,), _
                   Optional ByVal meta As cVariableMetaData = Nothing)

        MyBase.New(theCore, CObj(data), GetType(Integer), meta)

    End Sub

#End Region ' Construction

#Region " Cell interaction "

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            Dim d As Integer(,) = DirectCast(Me.Data, Integer(,))
            If Me.ValidateCellPosition(iRow, iCol) Then Return d(iRow, iCol) Else Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Single)
            Dim d As Integer(,) = DirectCast(Me.Data, Integer(,))
            Dim i As Integer = CInt(value)
            If Me.ValidateCellValue(value) Then
                If Me.ValidateCellPosition(iRow, iCol) Then
                    d(iRow, iCol) = i
                    If (Me.m_bInvalidateMinMax = False) Then
                        Me.m_bInvalidateMinMax = (i < Me.m_iMinValue Or i > Me.m_iMaxValue)
                    End If
                End If
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            If Me.m_bInvalidateMinMax Then Me.RecalcMinMax()
            Return Me.m_iMaxValue
        End Get
    End Property

    Public Overrides ReadOnly Property MinValue() As Single
        Get
            If Me.m_bInvalidateMinMax Then Me.RecalcMinMax()
            Return Me.m_iMinValue
        End Get
    End Property

    Public Overrides Sub Invalidate()
        Me.m_bInvalidateMinMax = True
    End Sub

#End Region ' Cell interaction

#Region " Internals "

    Protected Overridable Sub RecalcMinMax()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim d As Integer(,) = DirectCast(Me.Data, Integer(,))
        Me.m_iMaxValue = Integer.MinValue
        Me.m_iMinValue = Integer.MaxValue
        For iRow As Integer = 1 To bm.InRow
            For iCol As Integer = 1 To bm.InCol
                If d(iRow, iCol) <> cCore.NULL_VALUE Then
                    Me.m_iMaxValue = Math.Max(d(iRow, iCol), Me.m_iMaxValue)
                    Me.m_iMinValue = Math.Min(d(iRow, iCol), Me.m_iMinValue)
                End If
            Next iCol
        Next iRow
        Me.m_bInvalidateMinMax = False

    End Sub

#End Region ' Internals

End Class

''' <summary>
''' Base layer providing access to Ecospace data as cells of single values.
''' </summary>

Public Class cEcospaceLayerSingleNxM
    Inherits cEcospaceLayer

#Region " Private variables "

    Private m_sMinValue As Single = 0.0!
    Private m_sMaxValue As Single = 0.0!
    ''' <summary>States whether min/max should be recalculated</summary>
    ''' <remarks>True at startup to make sure that min/max are properly calculated
    ''' when first queried.</remarks>
    Private m_bInvalidateMinMax As Boolean = True

#End Region ' Private variables

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer that derives its data and identity from 
    ''' a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' <param name="varName"></param>
    ''' <param name="iIndex"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef theCore As cCore, ByRef manager As cEcospaceBasemap, _
            ByVal varName As eVarNameFlags, Optional ByVal iIndex As Integer = cCore.NULL_VALUE, _
            Optional ByVal meta As cVariableMetaData = Nothing)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, varName, iIndex, GetType(Single), meta)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a NxN layer that derives its data from a manager, but
    ''' that is a unique data entity in the EwE core.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="iDBID"></param>
    ''' <param name="manager"></param>
    ''' <param name="varName"></param>
    ''' <param name="iIndex"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, _
                   ByVal iDBID As Integer, _
                   ByVal manager As cEcospaceBasemap, _
                   ByVal varName As eVarNameFlags, _
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE, _
                   Optional ByVal meta As cVariableMetaData = Nothing)

        MyBase.New(theCore, iDBID, manager, varName, iIndex, GetType(Single), meta)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a NxN layer that is hard-linked to an array of data.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="data"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef theCore As cCore, _
                   ByRef data As Single(,), _
                   Optional ByVal meta As cVariableMetaData = Nothing)

        MyBase.New(theCore, CObj(data), GetType(Single), meta)

    End Sub

#End Region ' Construction

#Region " Cell interaction "

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            Dim d As Single(,) = DirectCast(Me.Data, Single(,))
            If Me.ValidateCellPosition(iRow, iCol) Then Return d(iRow, iCol) Else Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Single)
            Dim d As Single(,) = DirectCast(Me.Data, Single(,))
            If Me.ValidateCellValue(value) Then
                If Me.ValidateCellPosition(iRow, iCol) Then
                    d(iRow, iCol) = value
                    If (Me.m_bInvalidateMinMax = False) Then
                        Me.m_bInvalidateMinMax = (value < Me.m_sMinValue Or value > Me.m_sMaxValue)
                    End If
                End If
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            If Me.m_bInvalidateMinMax Then Me.RecalcMinMax()
            Return Me.m_sMaxValue
        End Get
    End Property

    Public Overrides ReadOnly Property MinValue() As Single
        Get
            If Me.m_bInvalidateMinMax Then Me.RecalcMinMax()
            Return Me.m_sMinValue
        End Get
    End Property

    Public Overrides Sub Invalidate()
        Me.m_bInvalidateMinMax = True
    End Sub

#End Region ' Cell interaction

#Region " Internals "

    Protected Overridable Sub RecalcMinMax()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim s As Single = 0.0!
        Me.m_sMaxValue = Single.MinValue
        Me.m_sMinValue = Single.MaxValue
        For iRow As Integer = 1 To bm.InRow
            For iCol As Integer = 1 To bm.InCol
                s = Me.Cell(iRow, iCol)
                If s <> cCore.NULL_VALUE Then
                    Me.m_sMaxValue = Math.Max(s, Me.m_sMaxValue)
                    Me.m_sMinValue = Math.Min(s, Me.m_sMinValue)
                End If
            Next iCol
        Next iRow
        Me.m_bInvalidateMinMax = False

    End Sub

#End Region ' Internals

End Class
