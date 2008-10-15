'==============================================================================
'
' $Log: cEcospaceNxNLayer.vb,v $
' Revision 1.2  2008/10/15 23:54:11  jeroens
' Added cEcospaceMigrationLayer to uniquely wrap the migration data
'
' Revision 1.1  2008/09/26 07:30:21  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/08/26 18:51:14  jeroens
' Added means to invalidate value range
'
' Revision 1.3  2008/08/12 22:15:26  jeroens
' Layers can carry metadata to control what values are accepted into their data
'
' Revision 1.2  2008/08/11 18:37:09  jeroens
' Custom layers now have basic map properties set via constructor
'
' Revision 1.1  2008/08/11 02:00:35  jeroens
' Simplified class names
'
' Revision 1.2  2008/08/09 00:04:41  jeroens
' Added optional indexes to the constructors
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
''' array of Integer values.
''' </summary>
''' ===========================================================================
Public Class cEcospaceIntegerNxNLayer
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
    Public Sub New(ByRef theCore As cCore, ByVal manager As cEcospaceBasemap, _
            ByVal varName As eVarNameFlags, Optional ByVal iIndex As Integer = cCore.NULL_VALUE, _
            Optional ByVal meta As cVariableMetaData = Nothing)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, varName, iIndex, meta)
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
    Public Sub New(ByRef theCore As cCore, ByVal iDBID As Integer, ByVal manager As cEcospaceBasemap, _
            ByVal varName As eVarNameFlags, Optional ByVal iIndex As Integer = cCore.NULL_VALUE, _
            Optional ByVal meta As cVariableMetaData = Nothing)
        MyBase.New(theCore, iDBID, manager, varName, iIndex, meta)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a NxN layer that is hard-linked to an array of data.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="data"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef theCore As cCore, ByRef data As Integer(,), _
            ByVal iInRow As Integer, ByVal iInCol As Integer, _
            ByVal sCellLength As Single, ByVal sLatitude As Single, ByVal sLongitude As Single, _
            Optional ByVal meta As cVariableMetaData = Nothing)
        MyBase.New(theCore, CObj(data), iInRow, iInCol, sCellLength, sLatitude, sLongitude, meta)
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
            If Me.m_bInvalidateMinMax Then Me.RecalcMinMax() : Me.m_bInvalidateMinMax = False
            Return Me.m_iMaxValue
        End Get
    End Property

    Public Overrides ReadOnly Property MinValue() As Single
        Get
            If Me.m_bInvalidateMinMax Then Me.RecalcMinMax() : Me.m_bInvalidateMinMax = False
            Return Me.m_iMinValue
        End Get
    End Property

    Public Overrides Sub Invalidate()
        Me.m_bInvalidateMinMax = True
    End Sub

#End Region ' Cell interaction

#Region " Internals "

    Protected Overridable Sub RecalcMinMax()
        Dim d As Integer(,) = DirectCast(Me.Data, Integer(,))
        Me.m_iMaxValue = Integer.MinValue
        Me.m_iMinValue = Integer.MaxValue
        For iRow As Integer = 1 To Me.InRow
            For iCol As Integer = 1 To Me.InCol
                If d(iRow, iCol) <> cCore.NULL_VALUE Then
                    Me.m_iMaxValue = Math.Max(d(iRow, iCol), Me.m_iMaxValue)
                    Me.m_iMinValue = Math.Min(d(iRow, iCol), Me.m_iMinValue)
                End If
            Next iCol
        Next iRow
    End Sub

#End Region ' Internals

End Class


''' ===========================================================================
''' <summary>
''' Layer for the Ecospace basemap, providing cell-based access to a 2 dimensional
''' array of Single values.
''' </summary>
''' ===========================================================================
Public Class cEcospaceSingleNxNLayer
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
        MyBase.New(theCore, cCore.NULL_VALUE, manager, varName, iIndex, meta)
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
    Public Sub New(ByRef theCore As cCore, ByVal iDBID As Integer, ByRef manager As cEcospaceBasemap, _
            ByVal varName As eVarNameFlags, Optional ByVal iIndex As Integer = cCore.NULL_VALUE, _
            Optional ByVal meta As cVariableMetaData = Nothing)
        MyBase.New(theCore, iDBID, manager, varName, iIndex, meta)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a NxN layer that is hard-linked to an array of data.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="data"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef theCore As cCore, ByRef data As Single(,), _
            ByVal iInRow As Integer, ByVal iInCol As Integer, _
            ByVal sCellLength As Single, ByVal sLatitude As Single, ByVal sLongitude As Single, _
            Optional ByVal meta As cVariableMetaData = Nothing)
        MyBase.New(theCore, CObj(data), iInRow, iInCol, sCellLength, sLatitude, sLongitude, meta)
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
            If Me.m_bInvalidateMinMax Then Me.RecalcMinMax() : Me.m_bInvalidateMinMax = False
            Return Me.m_sMaxValue
        End Get
    End Property

    Public Overrides ReadOnly Property MinValue() As Single
        Get
            If Me.m_bInvalidateMinMax Then Me.RecalcMinMax() : Me.m_bInvalidateMinMax = False
            Return Me.m_sMinValue
        End Get
    End Property

    Public Overrides Sub Invalidate()
        Me.m_bInvalidateMinMax = True
    End Sub

#End Region ' Cell interaction

#Region " Internals "

    Protected Overridable Sub RecalcMinMax()
        Dim d As Single(,) = DirectCast(Me.Data, Single(,))
        Me.m_sMaxValue = Single.MinValue
        Me.m_sMinValue = Single.MaxValue
        For iRow As Integer = 1 To Me.InRow
            For iCol As Integer = 1 To Me.InCol
                If d(iRow, iCol) <> cCore.NULL_VALUE Then
                    Me.m_sMaxValue = Math.Max(d(iRow, iCol), Me.m_sMaxValue)
                    Me.m_sMinValue = Math.Min(d(iRow, iCol), Me.m_sMinValue)
                End If
            Next iCol
        Next iRow
    End Sub

#End Region ' Internals

End Class


''' ===========================================================================
''' <summary>
''' Layer for the Ecospace basemap, providing cell-based access to a 2 dimensional
''' array of migration values.
''' </summary>
''' ===========================================================================
Public Class cEcospaceMigrationLayer
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

            Me.MigRow(CInt(value)) = iRow
            Me.MigCol(CInt(value)) = iCol
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

    Private Function MigRow() As Single()
        Dim d As Object = Me.Data
        Return DirectCast(d, Single()())(0)
    End Function

    Private Function MigCol() As Single()
        Dim d As Object = Me.Data
        Return DirectCast(d, Single()())(1)
    End Function

    Private Sub Refresh()
        Dim asMigRow As Single() = Me.MigRow
        Dim asMigCol As Single() = Me.MigCol
        ReDim m_asData(Me.InRow, Me.InCol)

        For iRowTest As Integer = 1 To Me.InRow
            For iColTest As Integer = 1 To Me.InCol
                Me.m_asData(iRowTest, iColTest) = cCore.NULL_VALUE
            Next
        Next

        For iMonth As Integer = Me.m_iMinValue To Me.m_iMaxValue
            Me.m_asData(CInt(asMigRow(iMonth)), CInt(asMigCol(iMonth))) = iMonth
        Next
    End Sub

#End Region ' Private bits

End Class