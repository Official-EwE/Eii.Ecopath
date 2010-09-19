#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Base layer providing access to Ecospace data as cells, each representing a
''' vector with a X and Y component.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class cEcospaceLayerVector
    Inherits cEcospaceLayer

#Region " Private variables "

    ''' <summary>Layer max vector value.</summary>
    Protected m_sMaxValue As Single = 0.0!

    ''' <summary>States whether layer max value should be recalculated.</summary>
    ''' <remarks>True at startup to make sure that the max vector size is properly 
    ''' calculated when first queried.</remarks>
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
    Public Sub New(ByVal theCore As cCore, _
                   ByVal manager As cEcospaceBasemap, _
                   ByVal varName As eVarNameFlags, _
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, varName, iIndex, GetType(Single))
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
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE)

        MyBase.New(theCore, iDBID, manager, varName, iIndex, GetType(Single))

    End Sub

#End Region ' Construction

#Region " Cell interaction "

    ''' <summary>
    ''' Get/set a cell value in the form of Single(2), where index 0 represents
    ''' the X velocity, and index 1 represents the Y velocity of the value.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <param name="iCol"></param>
    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Return New Single() {Me.XVelocity(iRow, iCol), Me.YVelocity(iRow, iCol)}
        End Get
        Set(ByVal value As Object)
            Dim asValues As Single() = DirectCast(value, Single())
            Me.XVelocity(iRow, iCol) = asValues(0)
            Me.YVelocity(iRow, iCol) = asValues(1)
        End Set
    End Property

    ''' <summary>
    ''' Get X velocity data
    ''' </summary>
    Public MustOverride Property XVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single

    ''' <summary>
    ''' Get Y velocity data
    ''' </summary>
    Public MustOverride Property YVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single

    ''' <summary>
    ''' Get the max magnitude of all cells in the layer.
    ''' </summary>
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calc max vector size in data layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub RecalcMax()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap

        Me.m_sMaxValue = 0

        For iRow As Integer = 1 To bm.InRow
            For iCol As Integer = 1 To bm.InCol
                Me.m_sMaxValue = Math.Max(Me.m_sMaxValue, _
                                          Math.Max(Math.Abs(Me.XVelocity(iRow, iCol)), _
                                                   Math.Abs(Me.YVelocity(iRow, iCol))))
            Next iCol
        Next iRow
        Me.m_bInvalidateMax = False

    End Sub

#End Region ' Internals

End Class
