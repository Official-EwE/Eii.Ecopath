'==============================================================================
'
' $Log: cEcospaceLayerAdvection.vb,v $
' Revision 1.3  2009/05/06 12:33:00  jeroens
' Added meaningful datatypes
'
' Revision 1.2  2008/11/05 16:54:36  jeroens
' At least it compiles
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
Public Class cEcospaceLayerAdvection
    Inherits cEcospaceLayerSingleNxM

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

    Private m_asData As Single()(,)
    Private m_iGroup As Integer = 1

    Public Overloads Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Single()
        Get
            If Me.m_asData Is Nothing Then
                Me.Refresh()
            End If
            Return New Single() {Me.m_asData(0)(iRow, iCol), Me.m_asData(1)(iRow, iCol)}
        End Get
        Set(ByVal value As Single())
            Debug.Assert(TypeOf value Is Single())
            Me.m_asData(0)(iRow, iCol) = value(0)
            Me.m_asData(1)(iRow, iCol) = value(1)
            Me.Invalidate()
        End Set
    End Property

    Public Overrides Sub Invalidate()
        Me.m_asData = Nothing
    End Sub

#End Region ' Cell interaction

#Region " Private bits "

    'Private Function PrefRow() As Integer(,)
    '    Dim d As Object = Me.Data
    '    Return DirectCast(d, Integer()(,))(0)
    'End Function

    'Private Function PrefCol() As Integer(,)
    '    Dim d As Object = Me.Data
    '    Return DirectCast(d, Integer()(,))(1)
    'End Function

    Private Sub Refresh()
        '    Dim aiPrefRow As Integer(,) = Me.PrefRow
        '    Dim aiPrefCol As Integer(,) = Me.PrefCol

        '    ReDim m_asData(Me.InRow, Me.InCol)

        '    For iRowTest As Integer = 1 To Me.InRow
        '        For iColTest As Integer = 1 To Me.InCol
        '            Me.m_asData(iRowTest, iColTest) = cCore.NULL_VALUE
        '        Next
        '    Next

        '    For iMonth As Integer = Me.m_iMinValue To Me.m_iMaxValue
        '        Me.m_asData(CInt(aiPrefRow(Me.m_iGroup, iMonth)), CInt(aiPrefCol(Me.m_iGroup, iMonth))) = iMonth
        '    Next
    End Sub

#End Region ' Private bits

End Class