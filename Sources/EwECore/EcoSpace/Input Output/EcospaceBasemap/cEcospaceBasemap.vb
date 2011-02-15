#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
Public Class cEcospaceBasemap
    Inherits cCoreInputOutputBase

    ''' <summary>The layers maintained in a basemap.</summary>
    Private m_dictLayers As New Dictionary(Of eVarNameFlags, cEcospaceLayer)
    ''' <summary>Importance layers maintained in a basemap.</summary>
    Private m_lstLayerImportance As New List(Of cEcospaceLayerImportance)

    ''' <summary>Equator length in km.</summary>
    ''' <remarks>http://en.wikipedia.org/wiki/Equator#Exact_length_of_the_Equator</remarks>
    Private Shared c_sEquatorLength As Single = 40007.862917

#Region " Constructor "

    Sub New(ByRef theCore As cCore)

        MyBase.New(theCore)

        Dim data As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing
        Dim layer As cEcospaceLayer = Nothing
        Dim lData As cEcospaceDataStructures.cLayerImportanceData = Nothing

        Me.AllowValidation = False

        Try
            Me.DBID = DBID
            m_dataType = eDataTypes.EcospaceBasemap
            m_coreComponent = eCoreComponentType.EcoSpace

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

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
            meta = New cVariableMetaData(0, c_sEquatorLength, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.CellLength, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Latitude (top-left coord of layer)
            meta = New cVariableMetaData(-90.0!, 90.0!, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(0, eVarNameFlags.Latitude, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Longitude (top-left coord of layer)
            meta = New cVariableMetaData(-180.0!, 180.0!, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(0, eVarNameFlags.Longitude, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' *************************************************************************************** '
            ' Variables for layers, providing metadata and an anchor point for remarks, visual styles '
            ' *************************************************************************************** '

            ' LayerRelPP
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerRelPP, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerRelCin
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerRelCin, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerDepth
            meta = New cVariableMetaData(Integer.MinValue, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerDepth, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerHabitat
            meta = New cVariableMetaData(0, Me.m_core.GetCoreCounter(eCoreCounterTypes.nHabitats), cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            val = New cValue(0, eVarNameFlags.LayerHabitat, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerMPA
            meta = New cVariableMetaData(0, Me.m_core.GetCoreCounter(eCoreCounterTypes.nMPAs), cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            val = New cValue(0, eVarNameFlags.LayerMPA, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerRegion
            meta = New cVariableMetaData(0, Me.m_core.GetCoreCounter(eCoreCounterTypes.nRegions), cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            val = New cValue(0, eVarNameFlags.LayerRegion, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerMigration
            meta = New cVariableMetaData(1, cCore.N_MONTHS, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerMigration, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerDistribution
            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerDistribution, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' MPASeed
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerMPASeed, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' IBMPackets
            val = New cValueArray(eValueTypes.LayerArray, eVarNameFlags.LayerIBMPackets, eStatusFlags.OK, eCoreCounterTypes.nStanzas, AddressOf m_core.GetCoreCounter)
            m_values.Add(val.varName, val)

            ' LayerPort
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerPort, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerSail
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerSail, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Advection interface
            ' LayerAdvection
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerAdvection, eStatusFlags.Null, eValueTypes.SingleArray, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerWind
            meta = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerWind, eStatusFlags.Null, eValueTypes.SingleArray, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerUpwelling
            meta = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerUpwelling, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' LayerMLD
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerMLD, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' ----------------
            ' Init layers
            ' ----------------

            ' Depth layer
            meta = New cVariableMetaData(Integer.MinValue, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            layer = New cEcospaceLayerDepth(theCore, Me, meta)
            Me.Layers(layer.VarName) = layer

            ' Habitat layer
            layer = New cEcospaceLayerHabitat(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' MPA layer
            layer = New cEcospaceLayerMPA(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' Region layer
            meta = New cVariableMetaData(0, Me.m_core.GetCoreCounter(eCoreCounterTypes.nRegions), cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            layer = New cEcospaceLayerRegion(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' RelPP layer
            layer = New cEcospaceLayerRelPP(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' RelCin layer
            layer = New cEcospaceLayerRelCin(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' MPA Seed
            layer = New cEcospaceLayerMPASeed(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' Importance layers
            For i As Integer = 0 To Me.m_core.nImportanceLayers - 1
                lData = data.ImportanceLayers(i)
                m_lstLayerImportance.Add(New cEcospaceLayerImportance(theCore, lData.DBID, Me, i))
            Next

            ' Migration
            layer = New cEcospaceLayerMigration(theCore, Me, eVarNameFlags.LayerMigration)
            Me.Layers(layer.VarName) = layer

            ' Port
            layer = New cEcospaceLayerPort(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' Sailing cost
            layer = New cEcospaceLayerSail(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' IBM layers
            For i As Integer = 1 To Me.m_core.nStanzas
                Me.SetVariable(eVarNameFlags.LayerIBMPackets, New cEcospaceLayerIBMPackets(theCore, Me, i), i)
            Next

            layer = New cEcospaceLayerDistribution(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' Advection
            layer = New cEcospaceLayerAdvection(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' Wind
            layer = New cEcospaceLayerWind(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' Upwelling
            layer = New cEcospaceLayerUpwelling(theCore, Me)
            Me.Layers(layer.VarName) = layer

            ' MLD
            layer = New cEcospaceLayerMLD(theCore, Me)
            Me.Layers(layer.VarName) = layer

            'set status flags to default values
            ResetStatusFlags()

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceBasemap.")
            cLog.Write(Me.ToString & ".New(..) Error creating new cEcospaceBasemap. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructor

#Region " Variables by dot (.) operator "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.Inrow">InRow</see>
    ''' value for this scenario
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property InRow() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.InRow))
        End Get
        Friend Set(ByVal value As Integer)
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
            Return CInt(GetVariable(eVarNameFlags.InCol))
        End Get

        Friend Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.InCol, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.CellLength">CellLength</see>
    ''' value for this scenario in km
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CellLength() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.CellLength))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CellLength, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.CellLength">CellLength</see>
    ''' value for this scenario in decimal degrees
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CellSize() As Single

        Get
            Return cEcospaceBasemap.ToCellSize(Me.CellLength)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CellLength, cEcospaceBasemap.ToCellLength(value))
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the TopLeft latitude value for this layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Latitude() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.Latitude))
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
            Return CSng(GetVariable(eVarNameFlags.Longitude))
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a LayerImportance
    ''' </summary>
    ''' <param name="index">Index from 1 to nLayerImportance</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerImportance(ByVal index As Integer) As cEcospaceLayerImportance
        Get
            Try
                Return Me.m_lstLayerImportance(index - 1)
            Catch ex As Exception
                cLog.Write(Me.ToString & ".New(..) Unable to access LayerImportance of index:" & index & ". Error: " & ex.Message)
                m_core.Messages.AddMessage(New cMessage("Unable to access LayerImportance of index", eMessageType.DataValidation, eCoreComponentType.EcoSpace, eMessageImportance.Critical, eDataTypes.EcospaceBasemap))
                Return Nothing
            End Try
        End Get
    End Property

#End Region ' Variables by dot (.) operator

#Region " Layer interface "

    Public Property Layers(ByVal varName As eVarNameFlags) As cEcospaceLayer
        Get
            If Me.m_dictLayers.ContainsKey(varName) Then
                Return Me.m_dictLayers(varName)
            End If
            Return Nothing
        End Get
        Private Set(ByVal value As cEcospaceLayer)
            If Me.m_dictLayers.ContainsKey(varName) Then
                Debug.Assert(False, String.Format("Layer (0) already defined", varName))
                Return
            End If
            Me.m_dictLayers(varName) = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a copy of the layers maintained by this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Function LayerCollection() As List(Of cEcospaceLayer)
        Dim l As New List(Of cEcospaceLayer)
        For Each o As cEcospaceLayer In Me.m_dictLayers.Values
            l.Add(o)
        Next
        Return l
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace Depth layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerDepth() As cEcospaceLayerDepth
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerDepth), cEcospaceLayerDepth)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace port layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerPort() As cEcospaceLayerPort
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerPort), cEcospaceLayerPort)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace sailing cost layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerSailingCost() As cEcospaceLayerSail
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerSail), cEcospaceLayerSail)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace Habitat layer.
    ''' </summary>
    ''' <remarks>
    ''' This layer provides access to the one and only array that holds all
    ''' Habitats in Ecospace. At the moment (Nov '08), Habitats cannot overlap
    ''' and are stored in one two-dimensional array.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerHabitat() As cEcospaceLayerHabitat
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerHabitat), cEcospaceLayerHabitat)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace MPA layer.
    ''' </summary>
    ''' <remarks>
    ''' This layer provides access to the one and only array that holds all
    ''' MPAs in Ecospace. At the moment (Nov '08), MPAs cannot overlap
    ''' and are stored in one two-dimensional array.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerMPA() As cEcospaceLayerMPA
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerMPA), cEcospaceLayerMPA)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace Region layer.
    ''' </summary>
    ''' <remarks>
    ''' This layer provides access to the one and only array that holds all
    ''' Regions in Ecospace. At the moment (Nov '08), Regions cannot overlap
    ''' and are stored in one two-dimensional array.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerRegion() As cEcospaceLayerRegion
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerRegion), cEcospaceLayerRegion)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Relative Primary Production layer in Ecospace.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerRelPP() As cEcospaceLayerRelPP
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerRelPP), cEcospaceLayerRelPP)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Advection layer in Ecospace.
    ''' </summary>
    ''' <remarks>
    ''' This layer is a tricky one since it provides uniform access to
    ''' both advection directional as well as velocity information. See the
    ''' actual layer for details.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerAdvection() As cEcospaceLayerAdvection
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerAdvection), cEcospaceLayerAdvection)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace wind layer.
    ''' </summary>
    ''' <remarks>
    ''' This layer is a tricky one since it provides uniform access to
    ''' two amplitude components, XVel and YVel. See the actual layer for details.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerWind() As cEcospaceLayerWind
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerWind), cEcospaceLayerWind)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace Mixed Layer Depths layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerMixedLayerDepths() As cEcospaceLayerSingle
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerMLD), cEcospaceLayerSingle)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the flow layer in Ecospace for the current month.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerUpwelling() As cEcospaceLayerSingle
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerUpwelling), cEcospaceLayerSingle)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerMigration() As cEcospaceLayerMigration
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerMigration), cEcospaceLayerMigration)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerRelCin() As cEcospaceLayerRelCin
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerRelCin), cEcospaceLayerRelCin)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerMPASeed() As cEcospaceLayerMPASeed
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerMPASeed), cEcospaceLayerMPASeed)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerIBMPackets() As cEcospaceLayerIBMPackets
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerIBMPackets), cEcospaceLayerIBMPackets)
        End Get
    End Property

    Public ReadOnly Property LayerDistribution() As cEcospaceLayerDistribution
        Get
            Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerDistribution), cEcospaceLayerDistribution)
        End Get
    End Property

    Friend Function GetLayerData(ByVal varName As eVarNameFlags, Optional ByVal iIndex As Integer = cCore.NULL_VALUE) As Object
        Select Case varName
            Case eVarNameFlags.LayerDepth
                Return Me.m_core.m_EcoSpaceData.Depth
            Case eVarNameFlags.LayerHabitat
                Return Me.m_core.m_EcoSpaceData.HabType
            Case eVarNameFlags.LayerMPA
                Return Me.m_core.m_EcoSpaceData.MPA
            Case eVarNameFlags.LayerRegion
                Return Me.m_core.m_EcoSpaceData.Region
            Case eVarNameFlags.LayerRelPP
                Return Me.m_core.m_EcoSpaceData.RelPP
            Case eVarNameFlags.LayerRelCin
                Return Me.m_core.m_EcoSpaceData.RelCin
            Case eVarNameFlags.LayerMPASeed
                Return Me.m_core.MPAOptData.MPASeed
            Case eVarNameFlags.LayerAdvection
                Return New Single()(,) {Me.m_core.m_EcoSpaceData.Xvel, Me.m_core.m_EcoSpaceData.Yvel}
            Case eVarNameFlags.LayerMigration
                Return New Integer()(,) {Me.m_core.m_EcoSpaceData.PrefRow, Me.m_core.m_EcoSpaceData.Prefcol}
            Case eVarNameFlags.LayerWind
                Return New Single()(,,) {Me.m_core.m_EcoSpaceData.Xv, Me.m_core.m_EcoSpaceData.Yv}
            Case eVarNameFlags.LayerUpwelling
                Return Me.m_core.m_EcoSpaceData.flow
            Case eVarNameFlags.LayerMLD
                Return Me.m_core.m_EcoSpaceData.DepthA
            Case eVarNameFlags.LayerImportance
                If iIndex < 0 Or iIndex > Me.m_core.m_EcoSpaceData.ImportanceLayers.Count - 1 Then
                    Debug.Assert(False, "cCore message: Index out of bounds error for ImportanceLayers")
                    Return Nothing
                End If
                Return Me.m_core.m_EcoSpaceData.ImportanceLayers(iIndex).Data
            Case eVarNameFlags.LayerIBMPackets
                Return Me.m_core.m_Stanza
            Case eVarNameFlags.LayerPort
                Return Me.m_core.m_EcoSpaceData.Port
            Case eVarNameFlags.LayerSail
                Return Me.m_core.m_EcoSpaceData.Sail
            Case eVarNameFlags.LayerDistribution
                Return Me.m_core.m_EcoSpaceData.DistributionEnvelope
        End Select
        Return Nothing
    End Function

#End Region ' Layer interface

#Region " Cell position calculations "

    Public Shared ReadOnly Property DegreeToKm() As Single
        Get
            Return c_sEquatorLength / 360.0!
        End Get
    End Property

    Public Shared Function ToCellSize(ByVal sCellLength As Single) As Single
        Return sCellLength / DegreeToKm
    End Function

    Public Shared Function ToCellLength(ByVal sCellSize As Single) As Single
        Return sCellSize * DegreeToKm
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the top-left latitude position of the given row.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function RowToLat(ByVal iRow As Integer) As Single
        Return Me.Latitude - (iRow - 1) * Me.CellSize
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the one-based index of the row that contains a given latitude value.
    ''' </summary>
    ''' <param name="sLat"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function LatToRow(ByVal sLat As Single) As Integer
        Return CInt(Math.Floor((Me.Latitude - sLat) / Me.CellSize)) + 1
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the top-left longitude position of the given row.
    ''' </summary>
    ''' <param name="iCol"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function ColToLon(ByVal iCol As Integer) As Single
        Return Me.Longitude + (iCol - 1) * Me.CellSize
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the one-based index of the column that contains a given longitude 
    ''' value.
    ''' </summary>
    ''' <param name="sLon"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function LonToCol(ByVal sLon As Single) As Integer
        ' Coarse wrap check
        If (sLon < Me.Longitude) Then
            sLon += 360
        ElseIf (sLon > (sLon + Me.InCol * Me.CellSize)) Then
            sLon += 360
        End If
        Return CInt(Math.Floor((sLon - Me.Longitude) / Me.CellSize)) + 1
    End Function

#End Region ' Cell calculations

End Class
