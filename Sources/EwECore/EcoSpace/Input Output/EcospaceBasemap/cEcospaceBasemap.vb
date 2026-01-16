' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports EwECore.Core
Imports EwECore.Style
Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



''' ===========================================================================
''' <summary>
''' The heart of the Ecospace map interfaces. The basemap manages all foundation
''' Ecospace map layers.
''' </summary>
''' ===========================================================================
Public Class cEcospaceBasemap
    Inherits cCoreInputOutputBase
    Implements IEcospaceLayerManager

    ''' <summary>The layers maintained in a basemap.</summary>
    Private m_layers As New Dictionary(Of eVarNameFlags, cEcospaceLayer())
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceBasemap)()

#Region " Constructor "

    Sub New(core As cCore)

        MyBase.New(core)

        Dim data As cEcospaceDataStructures = Me.m_core.m_EcospaceData
        Dim val As cValue = Nothing

        Me.AllowValidation = False

        Try
            Me.DBID = Me.DBID
            Me.m_dataType = eDataTypes.EcospaceBasemap
            Me.m_coreComponent = eCoreComponentType.Ecospace

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' InRow
            val = New cValue(core, 0, eVarNameFlags.InRow, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' InCol
            val = New cValue(core, 0, eVarNameFlags.InCol, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' CellLength
            val = New cValue(core, 1, eVarNameFlags.CellLength, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' CellSize
            val = New cValue(core, 1, eVarNameFlags.CellSize, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' Latitude (top-left coord of layer)
            val = New cValue(core, 0, eVarNameFlags.Latitude, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' Longitude (top-left coord of layer)
            val = New cValue(core, 0, eVarNameFlags.Longitude, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' Assume square cells
            val = New cValue(core, New Boolean, eVarNameFlags.AssumeSquareCells, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            ' CoordinateSystem
            val = New cValue(core, "", eVarNameFlags.ProjectionString, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' ************************************************************************************************* '
            ' Variables for layers, providing metadata and an anchor point for remarks, visual styles, metadata '
            ' ************************************************************************************************* '

            ' LayerRelPP
            val = New cValue(core, 0, eVarNameFlags.LayerRelPP, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerContaminantForcingRelative (was LayerRelCin)
            val = New cValue(core, 0, eVarNameFlags.LayerContaminantRelativeDistribution, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerContaminantForcingAbsolute
            val = New cValue(core, 0, eVarNameFlags.LayerContaminantForcingAbsolute, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerDepth
            val = New cValue(core, 0, eVarNameFlags.LayerDepth, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerHabitat
            val = New cValue(core, 0, eVarNameFlags.LayerHabitat, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerHabitatCapacity
            val = New cValue(core, 0, eVarNameFlags.LayerHabitatCapacity, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerHabitatCapacityInput
            val = New cValue(core, 0, eVarNameFlags.LayerHabitatCapacityInput, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerMPA
            val = New cValue(core, 0, eVarNameFlags.LayerMPA, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerRegion
            val = New cValue(core, 0, eVarNameFlags.LayerRegion, eStatusFlags.Null, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerEffortZone
            val = New cValue(core, 0, eVarNameFlags.LayerEffortZone, eStatusFlags.Null, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerMigration
            val = New cValue(core, 0, eVarNameFlags.LayerMigration, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerImportance
            val = New cValue(core, 0, eVarNameFlags.LayerImportance, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' MPASeed
            val = New cValue(core, 0, eVarNameFlags.LayerMPASeed, eStatusFlags.Null, eValueTypes.Int)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerPort
            val = New cValue(core, 0, eVarNameFlags.LayerPort, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerSail
            val = New cValue(core, 0, eVarNameFlags.LayerSail, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerAdvection
            val = New cValue(core, 0, eVarNameFlags.LayerAdvection, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerAdvectionForcing
            val = New cValue(core, 0, eVarNameFlags.LayerAdvectionForcing, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerWind
            val = New cValue(core, 0, eVarNameFlags.LayerWind, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerUpwelling
            val = New cValue(core, 0, eVarNameFlags.LayerUpwelling, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerDriver
            val = New cValue(core, 0, eVarNameFlags.LayerDriver, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' BiomassForcing
            val = New cValue(core, 0, eVarNameFlags.LayerBiomassForcing, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' RelativeBiomassForcing
            val = New cValue(core, 0, eVarNameFlags.LayerBiomassRelativeForcing, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerExclusion
            val = New cValue(core, 0, eVarNameFlags.LayerExclusion, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' LayerCellArea
            val = New cValue(core, 0, eVarNameFlags.LayerCellArea, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' nCells
            val = New cValue(core, 0, eVarNameFlags.nCells, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' ----------------
            ' Init layers
            ' ----------------
            Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcospaceData
            Dim llayers As New List(Of cEcospaceLayer)

            ' Depth layer
            Me.m_layers(eVarNameFlags.LayerDepth) = New cEcospaceLayer() {New cEcospaceLayerDepth(core, Me, 1)}

            ' Habitats
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.NoHabitats - 1
                llayers.Add(New cEcospaceLayerHabitat(core, Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerHabitat) = llayers.ToArray

            ' Habitat capacity input layer
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.NGroups
                llayers.Add(New cEcospaceLayerHabitatCapacity(core, Me, eDataTypes.EcospaceLayerHabitatCapacityInput, eVarNameFlags.LayerHabitatCapacityInput, i))
            Next
            Me.m_layers(eVarNameFlags.LayerHabitatCapacityInput) = llayers.ToArray

            ' Habitat capacity output layer
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.NGroups
                ' Hmm, does settings the datatype 'properly' screw up things?
                llayers.Add(New cEcospaceLayerHabitatCapacity(core, Me, eDataTypes.EcospaceLayerHabitatCapacity, eVarNameFlags.LayerHabitatCapacity, i))
            Next
            Me.m_layers(eVarNameFlags.LayerHabitatCapacity) = llayers.ToArray

            ' Biomass Forcing
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.NGroups
                llayers.Add(New cEcospaceLayerBiomassForcing(core, Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerBiomassForcing) = llayers.ToArray

            ' Relative Biomass Forcing
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.NGroups
                llayers.Add(New cEcospaceLayerBiomassRelativeForcing(core, Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerBiomassRelativeForcing) = llayers.ToArray

            ' MPA layer
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.MPAno
                llayers.Add(New cEcospaceLayerMPA(core, Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerMPA) = llayers.ToArray

            ' Region layer
            Me.m_layers(eVarNameFlags.LayerRegion) = New cEcospaceLayer() {New cEcospaceLayerRegion(core, Me)}

            ' RelPP layer
            Me.m_layers(eVarNameFlags.LayerRelPP) = New cEcospaceLayer() {New cEcospaceLayerRelPP(core, Me)}

            ' Relative contaminant forcing layer (RelCin)
            Me.m_layers(eVarNameFlags.LayerContaminantRelativeDistribution) = New cEcospaceLayer() {New cEcospaceLayerContaminantRelativeDistribution(core, Me)}

            ' Absolute contaminant forcing layer
            Me.m_layers(eVarNameFlags.LayerContaminantForcingAbsolute) = New cEcospaceLayer() {New cEcospaceLayerContaminantForcingAbsolute(core, Me)}

            ' MPA Seed
            Me.m_layers(eVarNameFlags.LayerMPASeed) = New cEcospaceLayer() {New cEcospaceLayerMPASeed(core, Me)}

            ' Importance
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.nImportanceLayers
                llayers.Add(New cEcospaceLayerImportance(Me.m_core, ecospaceDS.ImportanceLayerDBID(i), Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerImportance) = llayers.ToArray()

            ' Driver
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.nEnvironmentalDriverLayers
                llayers.Add(New cEcospaceLayerDriver(Me.m_core, Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerDriver) = llayers.ToArray()

            ' Exclusion
            Me.m_layers(eVarNameFlags.LayerExclusion) = New cEcospaceLayer() {New cEcospaceLayerExclusion(core, Me)}

            ' Cell Area
            Me.m_layers(eVarNameFlags.LayerCellArea) = New cEcospaceLayer() {New cEcospaceLayerCellArea(core, Me)}

            ' Effort zones
            Me.m_layers(eVarNameFlags.LayerEffortZone) = New cEcospaceLayer() {New cEcospaceLayerEffortZone(core, Me)}

            ' Migration
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.NGroups
                llayers.Add(New cEcospaceLayerMigration(core, Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerMigration) = llayers.ToArray()

            ' Port
            llayers.Clear()
            For i As Integer = 0 To ecospaceDS.nFleets
                llayers.Add(New cEcospaceLayerPort(core, Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerPort) = llayers.ToArray()

            ' Sailing cost
            llayers.Clear()
            For i As Integer = 1 To ecospaceDS.nFleets
                llayers.Add(New cEcospaceLayerSail(core, Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerSail) = llayers.ToArray()

            ' Advection
            llayers.Clear()
            llayers.Add(New cEcospaceLayerAdvection(core, Me))
            Me.m_layers(eVarNameFlags.LayerAdvection) = llayers.ToArray()

            ' Advection forcing
            llayers.Clear()
            llayers.Add(New cEcospaceLayerAdvectionForcing(core, Me, 1))
            llayers.Add(New cEcospaceLayerAdvectionForcing(core, Me, 2))
            llayers.Add(New cEcospaceLayerAdvectionForcing(core, Me, 3))
            Me.m_layers(eVarNameFlags.LayerAdvectionForcing) = llayers.ToArray()

            ' Wind
            llayers.Clear()
            llayers.Add(New cEcospaceLayerWind(core, Me))
            Me.m_layers(eVarNameFlags.LayerWind) = llayers.ToArray()

            ' Upwelling
            Me.m_layers(eVarNameFlags.LayerUpwelling) = New cEcospaceLayer() {New cEcospaceLayerUpwelling(core, Me)}

            '' MLD
            'Me.m_dictLayers(eVarNameFlags.LayerMLD) = New cEcospaceLayer() {New cEcospaceLayerMLD(theCore, Me)}

            'Me.m_dictLayers(eVarNameFlags.LayerIBMAge1NumbersForcing) = New cEcospaceLayer() {New cEcospaceLayer(theCore, Me)}
            llayers.Clear()
            For i As Integer = 1 To Me.m_core.nStanzas
                llayers.Add(New cEcospaceLayerIBMAge1Forcing(core, Me, i))
            Next
            Me.m_layers(eVarNameFlags.LayerIBMAge1Forcing) = llayers.ToArray

            'set status flags to default values
            Me.ResetStatusFlags()

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceBasemap.")
            m_logger.LogError(ex, Me.ToString & ".New(..) Error creating new cEcospaceBasemap. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructor

#Region " Overrides "

    Public Overrides Function GetVariable(VarName As eVarNameFlags,
                                          Optional iIndex As Integer = -9999,
                                          Optional iIndex2 As Integer = -9999,
                                          Optional iIndex3 As Integer = -9999) As Object

        ' JS 07Jul14: cell size is now a derived value
        If (VarName = eVarNameFlags.CellSize) Then
            Return Me.ToCellSize(CSng(Me.GetVariable(eVarNameFlags.CellLength)), Me.AssumeSquareCells)
        End If
        Return MyBase.GetVariable(VarName, iIndex, iIndex2, iIndex3)
    End Function

    Public Overrides Function SetVariable(VarName As eVarNameFlags,
                                          newValue As Object,
                                          Optional iSecondaryIndex As Integer = -9999, Optional iThirdIndex As Integer = -9999) As Boolean

        ' JS 07Jul14: cell size is now a derived value
        If (VarName = eVarNameFlags.CellSize) Then
            Return Me.SetVariable(eVarNameFlags.CellLength, Me.ToCellLength(CSng(newValue), Me.AssumeSquareCells))
        End If
        Return MyBase.SetVariable(VarName, newValue, iSecondaryIndex)

    End Function

#End Region ' Overrides

#Region " Variables by dot (.) operator "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.InRow">nRows</see>
    ''' value for this scenario
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property InRow As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.InRow))
        End Get
        Friend Set(value As Integer)
            Me.SetVariable(eVarNameFlags.InRow, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.InCol">nCols</see>
    ''' value for this scenario
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property InCol() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.InCol))
        End Get
        Friend Set(value As Integer)
            Me.SetVariable(eVarNameFlags.InCol, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.CellLength">CellLength</see>
    ''' value for this scenario in kilometers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CellLength() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.CellLength))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CellLength, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.CellLength">CellLength</see>
    ''' value for this scenario in map units. The value returned here depends 
    ''' on the setting of the <see cref="AssumeSquareCells"/> flag. If set to false,
    ''' Ecospace returns the cell size in decimal degrees. If set to true, Ecospace 
    ''' returns the cell size in meters.
    ''' </summary>
    ''' <remarks>
    ''' This conversion should be explicitly driven by map projections, of course...
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Property CellSize() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.CellSize))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CellSize, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the TopLeft latitude value for the map.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Latitude() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Latitude))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.Latitude, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the TopLeft longitude value for the map.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Longitude() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Longitude))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.Longitude, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the top-left (NW) extent of the map, expressed in degrees (lon, lat)
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PosTopLeft() As Drawing.PointF

        Get
            Return New Drawing.PointF(CSng(Me.GetVariable(eVarNameFlags.Longitude)), CSng(Me.GetVariable(eVarNameFlags.Latitude)))
        End Get

        Set(value As Drawing.PointF)
            Me.SetVariable(eVarNameFlags.Longitude, value.X)
            Me.SetVariable(eVarNameFlags.Latitude, value.Y)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the bottom-right (SE) extent of the map, expressed in degrees (lon, lat)
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PosBottomRight() As Drawing.PointF

        Get
            Return New Drawing.PointF(CSng(Me.GetVariable(eVarNameFlags.Longitude)) + Me.CellSize * Me.InCol,
                                      CSng(Me.GetVariable(eVarNameFlags.Latitude)) - Me.CellSize * Me.InRow)
        End Get

        Set(value As Drawing.PointF)
            Me.SetVariable(eVarNameFlags.Longitude, value.X - Me.CellSize * Me.InCol)
            Me.SetVariable(eVarNameFlags.Latitude, value.Y - Me.CellSize * Me.InRow)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <para>Get/set whether to assume square cells without latitude tapering correction. 
    ''' Square cells can be assumed on relatively small areas in UTM projections.</para>
    ''' <para>As an additional bonus Ecospace assumes meters as map units when this flag is 
    ''' set; if this flag is cleared map units are expected to be decimal degrees.</para>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property AssumeSquareCells As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.AssumeSquareCells))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.AssumeSquareCells, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <para>Get/set the Proj4 string for the Ecospace map.</para>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ProjectionString As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.ProjectionString))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.ProjectionString, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether a cell is modelled for Ecosystem dynamics.
    ''' </summary>
    ''' <param name="iRow">One-based row index.</param>
    ''' <param name="iCol">One-based column index.</param>
    ''' <returns>
    ''' A cell is modelled when it represent water in the included cell range.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function IsModelledCell(iRow As Integer, iCol As Integer) As Boolean
        Return Me.m_core.m_EcospaceData.Depth(iRow, iCol) > 0
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an intended cell index falls within the map bounds.
    ''' </summary>
    ''' <param name="iRow">One-based row index to validate.</param>
    ''' <param name="iCol">One-based column index to validate.</param>
    ''' <returns>True if the intended cell index falls within the map bounds,
    ''' False otherwise. No shades of grey here, let alone fifty of 'em. Ugh.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsValidCellPosition(iRow As Integer, iCol As Integer) As Boolean
        If (iRow < 1) Then Return False
        If (iCol < 1) Then Return False
        If (iRow > Me.InRow) Then Return False
        If (iCol > Me.InCol) Then Return False
        Return True
    End Function


    Public Property nCells() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.nCells))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.nCells, value)
        End Set
    End Property

#End Region ' Variables by dot (.) operator

#Region " Layer interface "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a LayerImportance
    ''' </summary>
    ''' <param name="index">Index from 1 to nLayerImportance</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerImportance(index As Integer) As cEcospaceLayerImportance
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerImportance)(index - 1), cEcospaceLayerImportance)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an external driver layer
    ''' </summary>
    ''' <param name="index">Index from 1 to nLayerDriver</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerDriver(index As Integer) As cEcospaceLayerDriver
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerDriver)(index - 1), cEcospaceLayerDriver)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace Depth layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerDepth() As cEcospaceLayerDepth
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerDepth)(0), cEcospaceLayerDepth)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace port layer for a given fleet.
    ''' </summary>
    ''' <param name="iFleet">Zero-based fleet index to get the layer for. Fleet
    ''' index 0 will return the ports for All fleets.
    ''' </param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerPort(iFleet As Integer) As cEcospaceLayerPort
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerPort)(iFleet), cEcospaceLayerPort)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace sailing cost layer for a given fleet.
    ''' </summary>
    ''' <param name="iFleet">One-based Fleet index to get the layer for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerSailingCost(iFleet As Integer) As cEcospaceLayerSail
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerSail)(iFleet - 1), cEcospaceLayerSail)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace Habitat layer.
    ''' </summary>
    ''' <param name="iHabitat">One-based habitat index</param>
    ''' <remarks>
    ''' This layer provides access to the one and only array that holds all
    ''' Habitats in Ecospace. At the moment (Nov '08), Habitats cannot overlap
    ''' and are stored in one two-dimensional array.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerHabitat(iHabitat As Integer) As cEcospaceLayerHabitat
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerHabitat)(iHabitat - 1), cEcospaceLayerHabitat)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the habitat foraging capacity input layer (prior)
    ''' </summary>
    ''' <param name="iGroup">One-based group index</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerHabitatCapacityInput(iGroup As Integer) As cEcospaceLayerHabitatCapacity
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerHabitatCapacityInput)(iGroup - 1), cEcospaceLayerHabitatCapacity)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the computed habitat foraging capacity layer
    ''' </summary>
    ''' <param name="iGroup">One-based group index</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerHabitatCapacity(iGroup As Integer) As cEcospaceLayerHabitatCapacity
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerHabitatCapacity)(iGroup - 1), cEcospaceLayerHabitatCapacity)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace MPA layer.
    ''' </summary>
    ''' <param name="iMPA">One-based MPA index</param>
    ''' <remarks>
    ''' This layer provides access to the one and only array that holds all
    ''' MPAs in Ecospace.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerMPA(iMPA As Integer) As cEcospaceLayerMPA
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerMPA)(iMPA - 1), cEcospaceLayerMPA)
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
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerRegion)(0), cEcospaceLayerRegion)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Relative Primary Production layer in Ecospace.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerRelPP() As cEcospaceLayerRelPP
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerRelPP)(0), cEcospaceLayerRelPP)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Advection layers in Ecospace.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerAdvection() As cEcospaceLayer()
        Get
            Return Me.Layers(eVarNameFlags.LayerAdvection)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace wind layers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerWind() As cEcospaceLayer()
        Get
            Return Me.Layers(eVarNameFlags.LayerWind)
        End Get
    End Property

    '''' -----------------------------------------------------------------------
    '''' <summary>
    '''' Get the Ecospace Mixed Layer Depths layer.
    '''' </summary>
    '''' -----------------------------------------------------------------------
    'Public ReadOnly Property LayerMixedLayerDepths() As cEcospaceLayerSingle
    '    Get
    '        Return DirectCast(Me.m_dictLayers(eVarNameFlags.LayerMLD)(0), cEcospaceLayerSingle)
    '    End Get
    'End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the flow layer in Ecospace for the current month.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerUpwelling() As cEcospaceLayerSingle
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerUpwelling)(0), cEcospaceLayerSingle)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the migration layer for a given group.
    ''' </summary>
    ''' <param name="iGroup">One-based group index.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerMigration(iGroup As Integer) As cEcospaceLayerMigration
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerMigration)(iGroup - 1), cEcospaceLayerMigration)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerRelCin() As cEcospaceLayerContaminantRelativeDistribution
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerContaminantRelativeDistribution)(0), cEcospaceLayerContaminantRelativeDistribution)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerContaminantForcingAbsolute() As cEcospaceLayerContaminantForcingAbsolute
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerContaminantForcingAbsolute)(0), cEcospaceLayerContaminantForcingAbsolute)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerMPASeed() As cEcospaceLayerMPASeed
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerMPASeed)(0), cEcospaceLayerMPASeed)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace exclusion layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerExclusion() As cEcospaceLayerExclusion
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerExclusion)(0), cEcospaceLayerExclusion)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace cell area layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerCellArea() As cEcospaceLayerCellArea
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerCellArea)(0), cEcospaceLayerCellArea)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace effort zone layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerEffortZone() As cEcospaceLayerEffortZone
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerEffortZone)(0), cEcospaceLayerEffortZone)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the Ecospace Age1 stanza biomass forcing layer. Good luck.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LayerIBMAge1Forcing(iGroup As Integer) As cEcospaceLayerIBMAge1Forcing
        Get
            Return DirectCast(Me.m_layers(eVarNameFlags.LayerIBMAge1Forcing)(iGroup - 1), cEcospaceLayerIBMAge1Forcing)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceLayerManager.Layers"/>
    ''' -----------------------------------------------------------------------
    Public Function Layers(Optional varName As eVarNameFlags = eVarNameFlags.NotSet) As cEcospaceLayer() _
        Implements IEcospaceLayerManager.Layers

        Select Case varName
            Case eVarNameFlags.NotSet
                Dim l As New List(Of cEcospaceLayer)
                For Each vn As eVarNameFlags In Me.m_layers.Keys
                    Select Case vn
                        Case eVarNameFlags.LayerMigration
                            Dim tmp As cEcospaceLayer() = Me.m_layers(vn)
                            For i As Integer = 1 To Me.m_core.nGroups
                                If Me.m_core.EcospaceGroupInputs(i).IsMigratory Then
                                    l.Add(tmp(i - 1))
                                End If
                            Next
                        Case Else
                            l.AddRange(Me.m_layers(vn))
                    End Select
                Next
                Return l.ToArray
            Case Else
                Return Me.m_layers(varName)
        End Select

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceLayerManager.Layer"/>
    ''' -----------------------------------------------------------------------
    Public Function Layer(varName As eVarNameFlags, Optional iIndex As Integer = cCore.NULL_VALUE) As cEcospaceLayer _
        Implements IEcospaceLayerManager.Layer

        iIndex = Math.Max(iIndex, 1)
        For Each l As cEcospaceLayer In Me.Layers(varName)
            If (l.Index = iIndex) Then Return l
        Next
        Return Nothing

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IEcospaceLayerManager.LayerData"/>
    ''' -----------------------------------------------------------------------
    Friend Function LayerData(varName As eVarNameFlags, iIndex As Integer) As Object _
        Implements IEcospaceLayerManager.LayerData

        Select Case varName
            Case eVarNameFlags.LayerDepth
                Return Me.m_core.m_EcospaceData.DepthInput
            Case eVarNameFlags.LayerHabitat
                Return Me.m_core.m_EcospaceData.PHabType
            Case eVarNameFlags.LayerHabitatCapacity
                Return Me.m_core.m_EcospaceData.HabCap
            Case eVarNameFlags.LayerHabitatCapacityInput
                Return Me.m_core.m_EcospaceData.HabCapInput
            Case eVarNameFlags.LayerMPA
                Return Me.m_core.m_EcospaceData.MPA
            Case eVarNameFlags.LayerRegion
                Return Me.m_core.m_EcospaceData.Region
            Case eVarNameFlags.LayerRelPP
                Return Me.m_core.m_EcospaceData.RelPP
            Case eVarNameFlags.LayerContaminantRelativeDistribution
                Return Me.m_core.m_EcospaceData.RelCin
            Case eVarNameFlags.LayerContaminantForcingAbsolute
                Return Me.m_core.m_EcospaceData.Ccell
            Case eVarNameFlags.LayerMPASeed
                Return Me.m_core.MPAOptData.MPASeed
            Case eVarNameFlags.LayerAdvection
                Return If(iIndex = 1, Me.m_core.m_EcospaceData.MonthlyXvel, Me.m_core.m_EcospaceData.MonthlyYvel)
            Case eVarNameFlags.LayerAdvectionForcing
                Select Case iIndex
                    Case 1 : Return Me.m_core.m_EcospaceData.Xvel
                    Case 2 : Return Me.m_core.m_EcospaceData.Yvel
                    Case 3 : Return Me.m_core.m_EcospaceData.UpVel
                    Case Else
                        Debug.Assert(False)
                End Select
            Case eVarNameFlags.LayerMigration
                Return Me.m_core.m_EcospaceData.MigMaps
            Case eVarNameFlags.LayerWind
                Return If(iIndex = 1, Me.m_core.m_EcospaceData.MonthlyXwind, Me.m_core.m_EcospaceData.MonthlyYwind)
            Case eVarNameFlags.LayerUpwelling
                Return Me.m_core.m_EcospaceData.MonthlyUpWell
            'Case eVarNameFlags.LayerMLD
            '    Return Me.m_core.m_EcoSpaceData.DepthA
            Case eVarNameFlags.LayerImportance
                Return Me.m_core.m_EcospaceData.ImportanceLayerMap
            Case eVarNameFlags.LayerDriver
                Return Me.m_core.m_EcospaceData.EnvironmentalLayerMap
            Case eVarNameFlags.LayerPort
                Return Me.m_core.m_EcospaceData.Port
            Case eVarNameFlags.LayerSail
                Return Me.m_core.m_EcospaceData.Sail
            Case eVarNameFlags.LayerBiomassForcing
                Return Me.m_core.m_EcospaceData.Bcell
            Case eVarNameFlags.LayerBiomassRelativeForcing
                Return Me.m_core.m_EcospaceData.Bcell
            Case eVarNameFlags.LayerExclusion
                Return Me.m_core.m_EcospaceData.Excluded
            Case eVarNameFlags.LayerIBMAge1Forcing
                Return Me.m_core.m_EcospaceData.Bcell
            Case eVarNameFlags.LayerCellArea
                Return Me.m_core.m_EcospaceData.CellArea
            Case eVarNameFlags.LayerEffortZone
                Return Me.m_core.m_EcospaceData.EffZones
        End Select
        Return Nothing
    End Function

#End Region ' Layer interface

#Region " Cell position calculations "

    Public Function ToCellSize(sCellLength As Single, bAssumeSquareCells As Boolean) As Single
        Return cEcospaceDataStructures.ToCellSize(sCellLength, bAssumeSquareCells)
    End Function

    Public Function ToCellLength(sCellSize As Single, bAssumeSquareCells As Boolean) As Single
        Return cEcospaceDataStructures.ToCellLength(sCellSize, bAssumeSquareCells)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the top-left latitude position of the given row.
    ''' </summary>
    ''' <param name="sRow"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function RowToLat(sRow As Single) As Single
        ' ToDo: validate if this correct for basemaps in meters (AssumeSquareCells = true)
        Return Me.Latitude - (sRow - 1) * Me.CellSize
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the one-based index of the row that contains a given latitude value.
    ''' </summary>
    ''' <param name="sLat"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function LatToRow(sLat As Single) As Single
        ' ToDo: validate if this correct for basemaps in meters (AssumeSquareCells = true)
        Return CSng(((Me.Latitude - sLat) / Me.CellSize)) + 1
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the top-left longitude position of the given row.
    ''' </summary>
    ''' <param name="sCol"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function ColToLon(sCol As Single) As Single
        ' ToDo: validate if this correct for basemaps in meters (AssumeSquareCells = true)
        Return Me.Longitude + (sCol - 1) * Me.CellSize
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the one-based index of the column that contains a given longitude 
    ''' value.
    ''' </summary>
    ''' <param name="sLon"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function LonToCol(sLon As Single) As Single
        ' ToDo: validate if this correct for basemaps in meters (AssumeSquareCells = true)
        Return 1 + CSng((sLon - Me.Longitude) / Me.CellSize)
    End Function

    Public Function MapUnits() As String
        ' This can be seriously improved. See ToDo notes in cUnits
        Dim units As New cUnits(Me.m_core)
        If Me.AssumeSquareCells Then
            Return units.ToString("[km]")
        End If
        Return units.ToString(cUnits.Mapping)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Convert a row, col location to a sequential cell number.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <param name="iCol"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function RowColToCell(iRow As Integer, iCol As Integer) As Integer
        Return (iRow - 1) * Me.InCol + iCol
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Convert a sequential cell number to a row, col location in the basemap.
    ''' </summary>
    ''' <param name="i"></param>
    ''' <param name="iRow"></param>
    ''' <param name="iCol"></param>
    ''' -----------------------------------------------------------------------
    Public Sub CellToRowCol(i As Integer, ByRef iRow As Integer, ByRef iCol As Integer)
        iRow = (i - 1) \ Me.InCol + 1
        iCol = (i - 1) Mod Me.InCol + 1
    End Sub
#End Region ' Cell calculations

End Class
