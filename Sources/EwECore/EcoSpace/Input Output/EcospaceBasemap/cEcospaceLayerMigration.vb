'==============================================================================
'
' $Log: cEcospaceLayerMigration.vb,v $
' Revision 1.3  2009/05/05 15:09:29  jeroens
' Removed cEcospaceBasemapLayer variables
'
' Revision 1.2  2008/11/14 21:43:29  jeroens
' Fixed  crash on migration data outside range of the basemap
'
' Revision 1.1  2008/11/04 05:42:58  jeroens
' New
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports directive

''' ===========================================================================
''' <summary>
''' Layer for the Ecospace basemap, providing cell-based access to a 2 dimensional
''' array of migration values.
''' </summary>
''' ===========================================================================
Public Class cEcospaceLayerMigration
    Inherits cEcospaceLayer

    Private m_iMinValue As Integer = 1
    Private m_iMaxValue As Integer = cCore.N_MONTHS

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer that derives its data and identity from 
    ''' a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' <param name="varName"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef theCore As cCore, ByVal manager As cEcospaceBasemap, _
            ByVal varName As eVarNameFlags)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, varName, cCore.NULL_VALUE, Nothing)
    End Sub

#End Region ' Construction

#Region " Cell interaction "

    Private m_asData As Single(,)
    Private m_iGroup As Integer = 1

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the group that this layer represents.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Group() As Integer
        Get
            Return Me.m_iGroup
        End Get
        Set(ByVal value As Integer)
            If value <> Me.m_iGroup Then
                Me.m_iGroup = value
                Me.Refresh()
            End If
        End Set
    End Property

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            If Me.m_asData Is Nothing Then
                Me.Refresh()
            End If
            Return Me.m_asData(iRow, iCol)
        End Get
        Set(ByVal value As Single)
            Debug.Assert(value >= Me.m_iMinValue)
            Debug.Assert(value <= Me.m_iMaxValue)

            Me.PrefRow(Me.m_iGroup, CInt(value)) = iRow
            Me.PrefCol(Me.m_iGroup, CInt(value)) = iCol
            Me.Invalidate()
        End Set
    End Property

    Public Overrides Sub Invalidate()
        Me.m_asData = Nothing
    End Sub

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return Me.m_iMaxValue
        End Get
    End Property

    Public Overrides ReadOnly Property MinValue() As Single
        Get
            Return Me.m_iMinValue
        End Get
    End Property

#End Region ' Cell interaction

#Region " Private bits "

    Private Function PrefRow() As Integer(,)
        Dim d As Object = Me.Data
        Return DirectCast(d, Integer()(,))(0)
    End Function

    Private Function PrefCol() As Integer(,)
        Dim d As Object = Me.Data
        Return DirectCast(d, Integer()(,))(1)
    End Function

    Private Sub Refresh()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim aiPrefRow As Integer(,) = Me.PrefRow
        Dim aiPrefCol As Integer(,) = Me.PrefCol

        ReDim m_asData(bm.InRow, bm.InCol)

        For iRowTest As Integer = 1 To bm.InRow
            For iColTest As Integer = 1 To bm.InCol
                Me.m_asData(iRowTest, iColTest) = cCore.NULL_VALUE
            Next
        Next

        For iMonth As Integer = Me.m_iMinValue To Me.m_iMaxValue
            Dim iRow As Integer = CInt(aiPrefRow(Me.m_iGroup, iMonth))
            Dim iCol As Integer = CInt(aiPrefCol(Me.m_iGroup, iMonth))
            If Me.ValidateCellPosition(iRow, iCol) Then
                Me.m_asData(iRow, iCol) = iMonth
            End If
        Next
    End Sub

#End Region ' Private bits

End Class