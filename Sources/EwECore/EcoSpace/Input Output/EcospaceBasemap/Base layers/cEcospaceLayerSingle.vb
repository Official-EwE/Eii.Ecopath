#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Base layer providing access to Ecospace data as cells of single values.
''' </summary>

Public Class cEcospaceLayerSingle
    Inherits cEcospaceLayer

#Region " Private variables "

    ''' <summary>Layer max value.</summary>
    Private m_sMaxValue As Single = 0.0!

    ''' <summary>States whether the layer max value should be recalculated.</summary>
    ''' <remarks>True at startup to make sure that min/max are properly calculated
    ''' when first queried.</remarks>
    Private m_bInvalidateMax As Boolean = True

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

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Dim d As Single(,) = DirectCast(Me.Data, Single(,))
            If Me.ValidateCellPosition(iRow, iCol) Then Return d(iRow, iCol) Else Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Object)
            Dim d As Single(,) = DirectCast(Me.Data, Single(,))
            Dim s As Single = Convert.ToSingle(value)
            If Me.ValidateCellValue(value) Then
                If Me.ValidateCellPosition(iRow, iCol) Then
                    d(iRow, iCol) = s
                    If (Me.m_bInvalidateMax = False) Then
                        Me.m_bInvalidateMax = (Math.Abs(s) > Me.m_sMaxValue)
                    End If
                End If
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            If Me.m_bInvalidateMax Then Me.RecalcMax()
            Return Me.m_sMaxValue
        End Get
    End Property

    Public Overrides Sub Invalidate()
        Me.m_bInvalidateMax = True
    End Sub

#End Region ' Cell interaction

#Region " Internals "

    Protected Overridable Sub RecalcMax()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim s As Single = 0.0!
        Me.m_sMaxValue = Single.MinValue
        For iRow As Integer = 1 To bm.InRow
            For iCol As Integer = 1 To bm.InCol
                s = CSng(Me.Cell(iRow, iCol))
                If s <> cCore.NULL_VALUE Then
                    Me.m_sMaxValue = Math.Max(Math.Abs(s), Me.m_sMaxValue)
                End If
            Next iCol
        Next iRow
        Me.m_bInvalidateMax = False

    End Sub

#End Region ' Internals

End Class
