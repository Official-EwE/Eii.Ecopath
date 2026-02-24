' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Core
Imports EwECore.SpatialData
Imports EwECore.Style
Imports EwECore.ValueWrapper
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' ---------------------------------------------------------------------------
''' <summary>
''' Base class for providing cell-based interaction with Ecospace data.
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
''' ---------------------------------------------------------------------------
Public MustInherit Class cEcospaceLayer
    Inherits cCoreInputOutputBase

#Region " Private variables "

    ''' <summary>Manager delivering the data, if any.</summary>
    Protected m_manager As IEcospaceLayerManager = Nothing
    ''' <summary>If set, this flag will direct the manager how to get to the actual map data.</summary>
    Protected m_vnData As eVarNameFlags = eVarNameFlags.NotSet
    ''' <summary>Metadata to restrict values that can enter a layer.</summary>
    Protected m_metadata As cVariableMetaData = Nothing
    ''' <summary>If set, a hard-linked reference to an array.</summary>
    Protected m_data As Object = Nothing
    ''' <summary>Type of the data.</summary>
    Protected m_typeValue As Type = Nothing
    ''' <summary>States whether cached statistics should be recalculated.</summary>
    ''' <remarks>True at startup to make sure that stats are properly calculated
    ''' when first queried.</remarks>
    Protected m_bInvalidateStats As Boolean = True

    Protected m_iSecundaryIndex As Integer = 1
    Protected m_ccSecundaryIndex As eCoreCounterTypes = eCoreCounterTypes.NotSet
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceLayer)()

#End Region ' Private variables

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for definining a layer that dynamically obtains its data.
    ''' </summary>
    ''' <param name="core">The core to notify of changes.</param>
    ''' <param name="iDBID">Database ID to assign to the layer.</param>
    ''' <param name="manager">The manager providing data for this layer.</param>
    ''' <param name="vnData">The variable name identifying what data to obtain
    ''' from the manager.</param>
    ''' <param name="iIndex">Secundary index for obtaining the data.</param>
    ''' <param name="typeValue"><see cref="Type">Type</see> of layer values.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub New(core As cCore,
                      iDBID As Integer,
                      manager As IEcospaceLayerManager,
                      strName As String,
                      vnData As eVarNameFlags,
                      iIndex As Integer,
                      typeValue As Type)

        Me.New(core, iDBID, strName, typeValue, Nothing)

        Debug.Assert(vnData <> eVarNameFlags.NotSet)

        Me.AllowValidation = False

        ' Store details
        Me.m_manager = manager
        Me.m_vnData = vnData
        Me.Index = iIndex
        Me.m_ValidationStatus = New cVariableStatus()
        Me.m_ValidationStatus.CoreDataObject = Me

        If (TypeOf manager Is cCoreInputOutputBase) Then
            Me.m_metadata = DirectCast(manager, cCoreInputOutputBase).GetVariableMetadata(Me.m_vnData)
        End If

        Me.AllowValidation = True

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for defining a layer that is connected directly to its data.
    ''' </summary>
    ''' <param name="core">The core to notify of changes.</param>
    ''' <param name="data">The data to link to this layer.</param>
    ''' <param name="typeValue"><see cref="Type">Type</see> of layer values.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub New(core As cCore,
                      data As Object,
                      strName As String,
                      typeValue As Type,
                      Optional meta As cVariableMetaData = Nothing,
                      Optional vn As eVarNameFlags = Nothing)

        Me.New(core, cCore.NULL_VALUE, strName, typeValue, meta)

        Me.m_data = data
        Me.m_vnData = vn
        Me.m_metadata = meta

    End Sub

    Private Sub New(core As cCore,
                    iDBID As Integer,
                    strName As String,
                    typeValue As Type,
                    meta As cVariableMetaData)

        MyBase.New(core)

        Dim val As cValue = Nothing

        Try
            Me.DBID = iDBID
            Me.m_dataType = eDataTypes.NotSet
            Me.m_coreComponent = eCoreComponentType.Ecospace
            Me.m_metadata = meta
            Me.m_typeValue = typeValue

            Me.AllowValidation = False
            Me.Name = strName
            Me.AllowValidation = True

            Me.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceLayer.")
            m_logger.LogError(ex, Me.ToString & ".New(..) Error creating new cEcospaceLayer. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructors

#Region " Cell manipulation "

    Protected Function ValidateCellPosition(iRow As Integer, iCol As Integer) As Boolean
        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Return iRow > 0 And iRow <= bm.InRow And iCol > 0 And iCol <= bm.InCol
    End Function

    Protected MustOverride Function ValidateCellValue(value As Object) As Boolean

    Protected ReadOnly Property Data() As Object
        Get
            If (Me.m_data Is Nothing) Then
                Me.m_data = Me.m_manager.LayerData(Me.m_vnData, Me.Index)
            End If
            Return Me.m_data
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="IEcospaceLayerManager">manager</see> responsible for this layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Manager As IEcospaceLayerManager
        Get
            Return Me.m_manager
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="eVarNameFlags"/> for the variable of this layers' data
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property VarName() As eVarNameFlags
        Get
            Return Me.m_vnData
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="Type">type</see> of layer values.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ValueType() As Type
        Get
            Return Me.m_typeValue
        End Get
    End Property

    Public Overrides Property Units(Optional varName As eVarNameFlags = eVarNameFlags.Name) As String
        Get
            Return MyBase.Units(Me.VarName)
        End Get
        Set(value As String)
        End Set
    End Property

    Public Overridable Property Description() As String
        Get
            Dim fmt As New cVarnameTypeFormatter()
            Return fmt.ToString(Me.VarName, eDescriptorTypes.Description)
        End Get
        Set(value As String)
            ' NOP
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the value of a cell.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <param name="iCol"></param>
    ''' <param name="iIndexSec"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the metadata associated with the values for a cell.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MetadataCell() As cVariableMetaData
        Get
            If (Me.m_metadata Is Nothing) Then Return cVariableMetaData.Get(Me.VarName)
            Return Me.m_metadata
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the maximum value in a layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property MaxValue() As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the minimum value in a layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property MinValue() As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of value cells in the layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property NumValueCells As Integer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Invalidates the content of a layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride Sub Invalidate()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Secundary index type for layers that contain, for instance, monthly data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property SecundaryIndexCounter As eCoreCounterTypes
        Get
            Return Me.m_ccSecundaryIndex
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Secundary index for layers that contain, for instance, monthly data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property SecundaryIndex As Integer
        Get
            Return Me.m_iSecundaryIndex
        End Get
        Set(value As Integer)
            value = Math.Max(1, Math.Min(Me.m_core.GetCoreCounter(Me.SecundaryIndexCounter), value))
            If (value <> Me.m_iSecundaryIndex) Then
                Me.m_iSecundaryIndex = value
                Me.Invalidate()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get if layer is receiving data from an external source.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IsExternalData() As Boolean
        Get
            Dim man As SpatialData.cSpatialDataConnectionManager = Me.m_core.SpatialDataConnectionManager
            If (man Is Nothing) Then Return False
            Dim adapter As cSpatialDataAdapter = man.Adapter(Me.VarName)
            If (adapter Is Nothing) Then Return False
            Return adapter.IsConnected(Me.Index)
        End Get
    End Property

#End Region ' Cell manipulation

#Region " Reset "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the defaulkt value for the layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property [Default]() As Single = cCore.NULL_VALUE

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reset the layer content to its <see cref="[Default]"/> value
    ''' </summary>
    Public Overridable Sub Reset(Optional value As Single = cCore.NULL_VALUE)
        If (value = cCore.NULL_VALUE) Then
            value = Me.[Default]
        End If
        If (value = cCore.NULL_VALUE) Then
            Return
        End If

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim iRows As Integer = bm.InRow
        Dim iCols As Integer = bm.InCol
        For iRow As Integer = 1 To iRows
            For iCol As Integer = 1 To iCols
                If (bm.IsModelledCell(iRow, iCol)) Then
                    Me.Cell(iRow, iCol) = value
                End If
            Next iCol
        Next iRow
        Me.Invalidate()

    End Sub

#End Region ' Reset

#Region " Overrides "

    Public Overrides Function ToString() As String
        Return "cEcospaceLayer " & Me.m_vnData.ToString() ' Cannot show any variables here - may cause deadlocks
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to make sure there is always a name returned for a layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function GetVariable(VarName As eVarNameFlags,
                                          Optional iIndex As Integer = -9999,
                                          Optional iIndex2 As Integer = -9999,
                                          Optional iIndex3 As Integer = -9999) As Object

        If (VarName = eVarNameFlags.Name) Then
            Dim strName As String = CStr(MyBase.GetVariable(VarName, iIndex, iIndex2, iIndex3))
            If Not String.IsNullOrWhiteSpace(strName) Then
                Return strName
            Else
                Return Me.DefaultName
            End If
        End If
        Return MyBase.GetVariable(VarName, iIndex, iIndex2, iIndex3)

    End Function

    Public Overrides Function SetVariable(VarName As eVarNameFlags,
                                          newValue As Object, Optional iSecondaryIndex As Integer = -9999, Optional iThirdIndex As Integer = -9999) As Boolean
        If (VarName = eVarNameFlags.Name) Then
            If (Me.Index > 0) Then
                Try
                    Dim strName As String = CStr(newValue)
                    If String.Compare(strName, Me.DefaultName, True) = 0 Then
                        newValue = ""
                    End If
                Catch ex As Exception
                    ' FAll through to default handling
                End Try
            End If
        End If
        Return MyBase.SetVariable(VarName, newValue, iSecondaryIndex)
    End Function

    Protected Overridable Function DefaultName() As String
        Return cStringUtils.Localize(My.Resources.CoreDefaults.CORE_DEFAULT_LAYER, Me.Index)
    End Function

#End Region ' Overrides

End Class

