'==============================================================================
'
' $Log: cEcospaceLayer.vb,v $
' Revision 1.5  2009/05/06 12:52:39  jeroens
' Cleaned-up
'
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
