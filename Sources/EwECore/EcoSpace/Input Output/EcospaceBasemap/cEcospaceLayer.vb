'==============================================================================
'
' $Log: cEcospaceLayer.vb,v $
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
    Protected Sub New(ByRef theCore As cCore, ByVal iDBID As Integer, ByVal manager As cEcospaceBasemap, _
            ByVal vnData As eVarNameFlags, ByVal iIndex As Integer, _
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
    Protected Sub New(ByRef theCore As cCore, ByVal data As Object, _
            ByVal iInRow As Integer, ByVal iInCol As Integer, _
            ByVal sCellLength As Single, ByVal sLatitude As Single, ByVal sLongitude As Single, _
            Optional ByVal meta As cVariableMetaData = Nothing)

        Me.New(theCore, cCore.NULL_VALUE, meta)

        Me.m_data = data
        Me.AllowValidation = False
        Me.InRow = iInRow
        Me.InCol = iInCol
        Me.CellLength = sCellLength
        Me.Latitude = sLatitude
        Me.Longitude = sLongitude
        Me.AllowValidation = True

    End Sub

    Private Sub New(ByRef theCore As cCore, ByVal iDBID As Integer, ByVal metaCellData As cVariableMetaData)

        MyBase.New(theCore)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Try
            Me.DBID = iDBID
            Me.m_dataType = eDataTypes.EcospaceBasemapLayer
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.m_mdData = metaCellData

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimGroupInput, eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)

            ' InRow
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.InRow, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' InCol
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.InCol, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' CellLength
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.CellLength, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Latitude (top-left coord of layer)
            meta = New cVariableMetaData(-90, 90, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(0, eVarNameFlags.Latitude, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Longitude (top-left coord of layer)
            meta = New cVariableMetaData(-180, 180, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(0, eVarNameFlags.Longitude, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'set status flags to default values
            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceBasemap.")
            cLog.Write(Me.ToString & ".New(..) Error creating new cEcospaceBasemap. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructors

#Region " Cell manipulation "

    Protected Function ValidateCellPosition(ByVal iRow As Integer, ByVal iCol As Integer) As Boolean
        Return iRow > 0 And iRow <= Me.InRow And iCol > 0 And iCol <= Me.InCol
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

#Region " Variables by dot (.) operator "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.Inrow">InRow</see>
    ''' value for this scenario
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property InRow() As Integer

        Get
            Dim iVal As Integer = CInt(GetVariable(eVarNameFlags.InRow))
            If iVal <= 0 Then
                If Me.m_manager IsNot Nothing Then
                    iVal = Me.m_manager.InRow
                End If
            End If
            Return iVal
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.InRow, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.Incol">InCol</see>
    ''' value for this scenario
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property InCol() As Integer

        Get
            Dim iVal As Integer = CInt(GetVariable(eVarNameFlags.InCol))
            If iVal <= 0 Then
                If Me.m_manager IsNot Nothing Then
                    iVal = Me.m_manager.InCol
                End If
            End If
            Return iVal
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.InCol, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.CellLength">CellLength</see>
    ''' value for this scenario
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CellLength() As Single

        Get
            Dim sVal As Single = CSng(GetVariable(eVarNameFlags.CellLength))
            If sVal <= 0 Then
                If Me.m_manager IsNot Nothing Then
                    sVal = Me.m_manager.CellLength
                End If
            End If
            Return sVal
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CellLength, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the TopLeft latitude value for this layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Latitude() As Single

        Get
            Dim sVal As Single = CSng(GetVariable(eVarNameFlags.Latitude))
            If sVal <= 0 Then
                If Me.m_manager IsNot Nothing Then
                    sVal = Me.m_manager.Latitude
                End If
            End If
            Return sVal
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.Latitude, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the TopLeft longitude value for this layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Longitude() As Single

        Get
            Dim sVal As Single = CSng(GetVariable(eVarNameFlags.Longitude))
            If sVal <= 0 Then
                If Me.m_manager IsNot Nothing Then
                    sVal = Me.m_manager.Longitude
                End If
            End If
            Return sVal
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.Longitude, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the top left position for this layer, expressed in (lon, lat)
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Position() As Drawing.PointF

        Get
            Return New Drawing.PointF(CSng(GetVariable(eVarNameFlags.Longitude)), CSng(GetVariable(eVarNameFlags.Latitude)))
        End Get

        Set(ByVal value As Drawing.PointF)
            SetVariable(eVarNameFlags.Longitude, value.X)
            SetVariable(eVarNameFlags.Latitude, value.Y)
        End Set

    End Property

#End Region ' Variables by dot (.) operator

End Class

