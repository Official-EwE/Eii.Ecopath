'==============================================================================
'
' $Log: cEcospaceLayer.vb,v $
' Revision 1.4  2009/05/06 12:36:00  jeroens
' Default datatype is NotSet
'
' Revision 1.3  2009/05/05 15:09:29  jeroens
' Removed cEcospaceBasemapLayer variables
'
' Revision 1.2  2009/01/16 18:30:23  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:20  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/08/26 18:51:14  jeroens
' Added means to invalidate value range
'
' Revision 1.3  2008/08/12 22:15:23  jeroens
' Layers can carry metadata to control what values are accepted into their data
'
' Revision 1.2  2008/08/11 18:34:56  jeroens
' Lat, Lon etc obtained from manager if available and not overridden
'
' Revision 1.1  2008/08/11 02:00:35  jeroens
' Simplified class names
'
' Revision 1.1  2008/08/09 00:01:02  jeroens
' Renamed
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports directive

''' ===========================================================================
''' <summary>
''' Base class for exposing a single Ecospace data layer.
''' </summary>
''' <remarks>
''' <para>This class can be used in two ways:</para>
''' <para><list type="bullet">
''' <item><description>In conjunction with a manager, who will link this layer
''' to the actual data</description></item>
''' <item><description>Directly linked to a data array holding the data. In that
''' case the manager is obsolete.</description></item>
''' </list></para>
''' </remarks>
''' ===========================================================================
Public MustInherit Class cEcospaceLayer
    Inherits cCoreInputOutputBase

#Region " Private variables "

    ''' <summary>Manager delivering the data, if any.</summary>
    Private m_manager As cEcospaceBasemap = Nothing
    ''' <summary>If set, this flag will direct the manager how to get to the actual map data.</summary>
    Private m_vnData As eVarNameFlags = eVarNameFlags.NotSet
    ''' <summary>Secundary index used to direct the manager how to get to the actual map data.</summary>
    Private m_iData As Integer = cCore.NULL_VALUE
    ''' <summary>Metadata to restrict values that can enter a layer.</summary>
    Private m_mdData As cVariableMetaData = Nothing
    ''' <summary>If set, a hard-linked reference to an array.</summary>
    Private m_data As Object = Nothing

#End Region ' Private variables

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for definining a layer that dynamically obtains its data.
    ''' </summary>
    ''' <param name="theCore">The core to notify of changes.</param>
    ''' <param name="iDBID">Database ID to assign to the layer.</param>
    ''' <param name="manager">The manager providing data for this layer.</param>
    ''' <param name="vnData">The variable name identifying what data to obtain
    ''' from the manager.</param>
    ''' <param name="iIndex">Secundary index for obtaining the data.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub New(ByRef theCore As cCore, _
                      ByVal iDBID As Integer, _
                      ByVal manager As cEcospaceBasemap, _
                      ByVal vnData As eVarNameFlags, _
                      ByVal iIndex As Integer, _
                      Optional ByVal meta As cVariableMetaData = Nothing)

        Me.New(theCore, iDBID, meta)

        Me.m_manager = manager
        Me.m_vnData = vnData
        Me.m_iData = iIndex

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for defining a layer that is connected directly to its data.
    ''' </summary>
    ''' <param name="theCore">The core to notify of changes.</param>
    ''' <param name="data">The data to link to this layer.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub New(ByRef theCore As cCore, _
                      ByVal data As Object, _
                      Optional ByVal meta As cVariableMetaData = Nothing)

        Me.New(theCore, cCore.NULL_VALUE, meta)

        Me.m_data = data

    End Sub

    Private Sub New(ByRef theCore As cCore, _
                    ByVal iDBID As Integer, _
                    ByVal metaCellData As cVariableMetaData)

        MyBase.New(theCore)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Try
            Me.DBID = iDBID
            Me.m_dataType = eDataTypes.NotSet
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.m_mdData = metaCellData

            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceLayer.")
            cLog.Write(Me.ToString & ".New(..) Error creating new cEcospaceLayer. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructors

#Region " Cell manipulation "

    Protected Function ValidateCellPosition(ByVal iRow As Integer, ByVal iCol As Integer) As Boolean
        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Return iRow > 0 And iRow <= bm.InRow And iCol > 0 And iCol <= bm.InCol
    End Function

    Protected Function ValidateCellValue(ByVal sValue As Single) As Boolean
        If Me.m_mdData Is Nothing Then Return True
        Return Me.m_mdData.MinOperator.Compare(sValue, Me.m_mdData.Min) And Me.m_mdData.MaxOperator.Compare(sValue, Me.m_mdData.Max)
    End Function

    Protected ReadOnly Property Data() As Object
        Get
            Dim d As Object = Me.m_data
            If (Me.m_data Is Nothing) Then d = Me.m_manager.GetLayerData(Me.m_vnData, Me.m_iData)
            Return d
        End Get
    End Property

    ' This function does not require a GetVariable/SetVariable counterpart
    Public MustOverride Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Single

    Public Property MetadataCell() As cVariableMetaData
        Get
            Return Me.m_mdData
        End Get
        Friend Set(ByVal value As cVariableMetaData)
            Me.m_mdData = value
        End Set
    End Property

    Public MustOverride ReadOnly Property MinValue() As Single
    Public MustOverride ReadOnly Property MaxValue() As Single

    Public MustOverride Sub Invalidate()

#End Region ' Cell manipulation

End Class


''' ===========================================================================
''' <summary>
''' Layer for the Ecospace basemap, providing cell-based access to a 2 dimensional
''' array of Integer values.
''' </summary>
''' ===========================================================================
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
    Public Sub New(ByVal theCore As cCore, _
                   ByVal iDBID As Integer, _
                   ByVal manager As cEcospaceBasemap, _
                   ByVal varName As eVarNameFlags, _
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE, _
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
    Public Sub New(ByVal theCore As cCore, _
                   ByVal data As Integer(,), _
                   Optional ByVal meta As cVariableMetaData = Nothing)

        MyBase.New(theCore, CObj(data), meta)

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

    End Sub

#End Region ' Internals

End Class


''' ===========================================================================
''' <summary>
''' Layer for the Ecospace basemap, providing cell-based access to a 2 dimensional
''' array of Single values.
''' </summary>
''' ===========================================================================
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
    Public Sub New(ByVal theCore As cCore, _
                   ByVal iDBID As Integer, _
                   ByVal manager As cEcospaceBasemap, _
                   ByVal varName As eVarNameFlags, _
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE, _
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
                   Optional ByVal meta As cVariableMetaData = Nothing)

        MyBase.New(theCore, CObj(data), meta)

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

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim d As Single(,) = DirectCast(Me.Data, Single(,))
        Me.m_sMaxValue = Single.MinValue
        Me.m_sMinValue = Single.MaxValue
        For iRow As Integer = 1 To bm.InRow
            For iCol As Integer = 1 To bm.InCol
                If d(iRow, iCol) <> cCore.NULL_VALUE Then
                    Me.m_sMaxValue = Math.Max(d(iRow, iCol), Me.m_sMaxValue)
                    Me.m_sMinValue = Math.Min(d(iRow, iCol), Me.m_sMinValue)
                End If
            Next iCol
        Next iRow

    End Sub

#End Region ' Internals

End Class
