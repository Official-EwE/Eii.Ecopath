#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

''' ===========================================================================
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
    ''' <summary>Type of the data.</summary>
    Private m_typeValue As Type = Nothing

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
    ''' <param name="typeValue"><see cref="Type">Type</see> of layer values.</param>
    ''' <param name="meta"><see cref="cVariableMetaData">Meta data</see> providing
    ''' value range limits.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub New(ByRef theCore As cCore, _
                      ByVal iDBID As Integer, _
                      ByVal manager As cEcospaceBasemap, _
                      ByVal vnData As eVarNameFlags, _
                      ByVal iIndex As Integer, _
                      ByVal typeValue As Type, _
                      Optional ByVal meta As cVariableMetaData = Nothing)

        Me.New(theCore, iDBID, typeValue, meta)

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
    ''' <param name="typeValue"><see cref="Type">Type</see> of layer values.</param>
    ''' <param name="meta"><see cref="cVariableMetaData">Meta data</see> providing
    ''' value range limits.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub New(ByRef theCore As cCore, _
                      ByVal data As Object, _
                      ByVal typeValue As Type, _
                      Optional ByVal meta As cVariableMetaData = Nothing)

        Me.New(theCore, cCore.NULL_VALUE, typeValue, meta)

        Me.m_data = data

    End Sub

    Private Sub New(ByRef theCore As cCore, _
                    ByVal iDBID As Integer, _
                    ByVal typeValue As Type, _
                    ByVal metaCellData As cVariableMetaData)

        MyBase.New(theCore)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Try
            Me.DBID = iDBID
            Me.m_dataType = eDataTypes.NotSet
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.m_mdData = metaCellData
            Me.m_typeValue = typeValue

            Me.ResetStatusFlags()

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

    Protected Function ValidateCellValue(ByVal value As Object) As Boolean
        Dim sValue As Single = 0.0
        If Me.m_mdData Is Nothing Then Return True
        Try
            sValue = Convert.ToSingle(sValue)
        Catch ex As Exception
            Return False
        End Try
        Return Me.m_mdData.MinOperator.Compare(sValue, Me.m_mdData.Min) And _
               Me.m_mdData.MaxOperator.Compare(sValue, Me.m_mdData.Max)
    End Function

    Protected ReadOnly Property Data() As Object
        Get
            Dim d As Object = Me.m_data
            If (Me.m_data Is Nothing) Then d = Me.m_manager.GetLayerData(Me.m_vnData, Me.m_iData)
            Return d
        End Get
    End Property

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

    ' This function does not require a GetVariable/SetVariable counterpart
    Public MustOverride Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the metadata associated with the values for a cell.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property MetadataCell() As cVariableMetaData
        Get
            Return Me.m_mdData
        End Get
        Friend Set(ByVal value As cVariableMetaData)
            Me.m_mdData = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the maximum value in a layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property MaxValue() As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Invalidates the content of a layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride Sub Invalidate()

#End Region ' Cell manipulation

End Class

