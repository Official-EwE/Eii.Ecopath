'==============================================================================
'
' $Log: cEcospaceBasemap.vb,v $
' Revision 1.8  2009/05/15 14:17:42  jeroens
' Removed obsolete method
'
' Revision 1.7  2009/05/06 12:32:59  jeroens
' Added meaningful datatypes
'
' Revision 1.6  2009/01/16 18:30:22  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.5  2009/01/08 16:18:53  jeroens
' Fixed issue 582
'
' Revision 1.4  2008/11/06 01:09:47  jeroens
' deppaws loc dna loc reyal noitargiM
'
' Revision 1.3  2008/11/04 05:42:06  jeroens
' Fixed migration data
'
' Revision 1.2  2008/10/15 23:59:46  jeroens
' Added migration layer
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
''' 
''' </summary>
''' ===========================================================================
Public Class cEcospaceBasemap
    Inherits cCoreInputOutputBase

    ''' <summary>The layers maintained in a basemap.</summary>
    Private m_dictLayers As New Dictionary(Of eVarNameFlags, cEcospaceLayer)
    ''' <summary>Importance layers maintained in a basemap.</summary>
    Private m_lstLayerImportance As New List(Of cEcospaceLayerImportance)

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
            meta = New cVariableMetaData(0, 360.0!, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
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
            ' Variables used to proved an achor point for tying remarks etc to derived basemap layers '
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

            ' LayerMigration
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerMigration, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' MPASeed
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.LayerMPASeed, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' ----------------
            ' Init layers
            ' ----------------

            ' Depth layer
            meta = New cVariableMetaData(Integer.MinValue, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            layer = New cEcospaceLayerDepth(theCore, Me, meta)
            Me.Layers(eVarNameFlags.LayerDepth) = layer

            ' Habitat layer
            meta = New cVariableMetaData(0, Me.m_core.GetCoreCounter(eCoreCounterTypes.nHabitats), cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            layer = New cEcospaceLayerHabitat(theCore, Me, meta)
            Me.Layers(eVarNameFlags.LayerHabitat) = layer

            ' MPA layer
            meta = New cVariableMetaData(0, Me.m_core.GetCoreCounter(eCoreCounterTypes.nMPAs), cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            layer = New cEcospaceLayerMPA(theCore, Me, meta)
            Me.Layers(eVarNameFlags.LayerMPA) = layer

            ' Region layer
            meta = New cVariableMetaData(0, Me.m_core.GetCoreCounter(eCoreCounterTypes.nRegions), cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
            layer = New cEcospaceLayerRegion(theCore, Me)
            Me.Layers(eVarNameFlags.LayerRegion) = layer

            ' RelPP layer
            layer = New cEcospaceLayerRelPP(theCore, Me)
            Me.Layers(eVarNameFlags.LayerRelPP) = layer

            ' RelCin layer
            layer = New cEcospaceLayerSingleNxM(theCore, Me, eVarNameFlags.LayerRelCin)
            Me.Layers(eVarNameFlags.LayerRelCin) = layer

            ' MPA Seed
            layer = New cEcospaceLayerMPASeed(theCore, Me)
            Me.Layers(eVarNameFlags.LayerMPASeed) = layer

            ' Importance layers
            For i As Integer = 0 To Me.m_core.nImportanceLayers - 1
                lData = data.ImportanceLayers(i)
                m_lstLayerImportance.Add(New cEcospaceLayerImportance(theCore, lData.DBID, Me, i))
            Next

            ' Migration
            layer = New cEcospaceLayerMigration(theCore, Me, eVarNameFlags.LayerMigration)
            Me.Layers(eVarNameFlags.LayerMigration) = layer

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
    ''' value for this scenario
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
    Public ReadOnly Property LayerDepth() As cEcospaceLayer
        Get
            Return Me.m_dictLayers(eVarNameFlags.LayerDepth)
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
    Public ReadOnly Property LayerHabitat() As cEcospaceLayer
        Get
            Return Me.m_dictLayers(eVarNameFlags.LayerHabitat)
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
    Public ReadOnly Property LayerMPA() As cEcospaceLayer
        Get
            Return Me.m_dictLayers(eVarNameFlags.LayerMPA)
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
    Public ReadOnly Property LayerRegion() As cEcospaceLayer
        Get
            Return Me.m_dictLayers(eVarNameFlags.LayerRegion)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Relative Primary Production layer in Ecospace.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerRelPP() As cEcospaceLayer
        Get
            Return Me.m_dictLayers(eVarNameFlags.LayerRelPP)
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
    Public ReadOnly Property LayerAdvection() As cEcospaceLayer
        Get
            Return Me.m_dictLayers(eVarNameFlags.LayerAdvection)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerMigration() As cEcospaceLayer
        Get
            Return Me.m_dictLayers(eVarNameFlags.LayerMigration)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerRelCin() As cEcospaceLayer
        Get
            Return Me.m_dictLayers(eVarNameFlags.LayerRelCin)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerMPASeed() As cEcospaceLayer
        Get
            Return Me.m_dictLayers(eVarNameFlags.LayerMPASeed)
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
            Case eVarNameFlags.LayerMigration
                Return New Integer()(,) {Me.m_core.m_EcoSpaceData.PrefRow, Me.m_core.m_EcoSpaceData.Prefcol}
            Case eVarNameFlags.LayerAdvection
                'Return New Single()() {Me.m_core.m_EcoSpaceData.AdvectSpeed}
            Case eVarNameFlags.LayerImportance
                If iIndex < 0 Or iIndex > Me.m_core.m_EcoSpaceData.ImportanceLayers.Count - 1 Then
                    Debug.Assert(True, "cCore message: Index out of bounds error for ImportanceLayers")
                    Return Nothing
                End If
                Return Me.m_core.m_EcoSpaceData.ImportanceLayers(iIndex).Data
        End Select
        Return Nothing
    End Function

#End Region ' Layer interface

End Class
