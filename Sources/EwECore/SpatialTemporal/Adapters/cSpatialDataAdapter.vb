' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Base spatial data adapter for inserting external spatial/temporal raster data into
    ''' Ecospace map data structures.
    ''' </summary>
    Public Class cSpatialDataAdapter
        Inherits cCoreInputOutputBase

#Region " Private vars "

        ''' <summary>Converter for each layer.</summary>
        Protected m_converters(,) As ISpatialDataConverter
        ''' <summary>Dataset for each layer.</summary>
        Protected m_datasets(,) As ISpatialDataSet
        ''' <summary>Dataset enabled flags.</summary>
        Protected m_bEnabled() As Boolean

        ''' <summary>Ecospace variable to operate onto.</summary>
        Protected m_varName As eVarNameFlags = Nothing
        ''' <summary>Core counter that this adapter operates onto.</summary>
        Protected m_coreCounter As eCoreCounterTypes = eCoreCounterTypes.NotSet

        ''' <summary>File names of preserved layers.</summary>
        Private m_astrLayerBackupFiles(,) As String

#End Region ' Private vars

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance of this class.
        ''' </summary>
        ''' <param name="core">The core to use.</param>
        ''' <param name="varName">The ecospace layer, identified by <see cref="eVarNameFlags">varname</see>,
        ''' that this adapter will interface with.</param>
        ''' <param name="cc">The <see cref="eCoreCounterTypes">core counter</see> that states the
        ''' number of layers that this adapter will interface with.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, cc As eCoreCounterTypes)

            MyBase.New(core)

            Me.m_dataType = eDataTypes.EcospaceSpatialDataSource
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.m_coreCounter = cc
            Me.m_varName = varName
            Me.AllowSaveIntermediateResults = False
            Me.DBID = -1
            Me.AllowValidation = True

        End Sub

#End Region ' Constructor

#Region " Basic bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether intermediate rasters will be saved to disk when
        ''' obtained from an exernal data connection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property AllowSaveIntermediateResults As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the adapter in response to an Ecospace scenario (re)load.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Overridable Sub Initialize()

            Me.m_converters = Nothing
            Me.m_datasets = Nothing

            Dim iNumItems As Integer = Math.Max(0, Me.m_core.GetCoreCounter(Me.m_coreCounter)) + 1

            ReDim Me.m_converters(iNumItems, cSpatialDataStructures.cMAX_CONN)
            ReDim Me.m_datasets(iNumItems, cSpatialDataStructures.cMAX_CONN)
            ReDim Me.m_astrLayerBackupFiles(iNumItems, cSpatialDataStructures.cMAX_CONN)
            ReDim Me.m_bEnabled(iNumItems)

            For i As Integer = 1 To iNumItems
                Me.m_bEnabled(i) = True
            Next

        End Sub


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the maximum number of layers for this adapter.
        ''' </summary>
        ''' <returns>The number of layers for this adapter.</returns>
        ''' -------------------------------------------------------------------
        Public Function MaxLength() As Integer
            Return Math.Max(1, Me.m_core.GetCoreCounter(Me.m_coreCounter))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether a data connection is allowed to exchange data.
        ''' </summary>
        ''' <param name="iIndex"></param>
        ''' -------------------------------------------------------------------
        Public Property IsEnabled(ByVal iIndex As Integer) As Boolean
            Get
                Return Me.m_bEnabled(iIndex)
            End Get
            Set(value As Boolean)
                Me.m_bEnabled(iIndex) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return whether a layer in this adapter is connected to external data.
        ''' </summary>
        ''' <param name="iIndex">The one-based index of the layer to query, or
        ''' <see cref="cCore.NULL_VALUE"/> if irrelevant.</param>
        ''' -------------------------------------------------------------------
        Public Function IsConnected(ByVal iIndex As Integer, _
                                    Optional ByVal iConnection As Integer = -1) As Boolean

            If (Me.m_datasets Is Nothing) Then Me.Initialize()
            If (Me.m_converters Is Nothing) Then Me.Initialize()

            Dim cv As ISpatialDataConverter = Nothing
            Dim ds As ISpatialDataSet = Nothing
            Dim bConnected As Boolean = False
            Dim iMin As Integer = 1
            Dim iMax As Integer = cSpatialDataStructures.cMAX_CONN

            If iConnection > 0 Then
                iMin = Math.Max(iConnection, 1) : iMax = Math.Min(iConnection, cSpatialDataStructures.cMAX_CONN)
            End If

            For i As Integer = iMin To iMax

                cv = Me.Converter(iIndex, i)
                ds = Me.Dataset(iIndex, i)

                If (ds IsNot Nothing) Then
                    If (ds.IsConfigured()) Then
                        If Not String.IsNullOrWhiteSpace(ds.ConversionFormat) Then
                            If (cv IsNot Nothing) Then
                                bConnected = bConnected Or cv.IsConfigured()
                            End If
                        Else
                            bConnected = True
                        End If
                    End If
                End If

            Next
            Return bConnected

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a <see cref="ISpatialDataSet">data set</see> for layer <paramref name="iIndex"/>.
        ''' </summary>
        ''' <param name="iLayer">Layer index [0, <see cref="MaxLength"/>&lt;.</param>
        ''' <param name="iConnection">One-based index of the connection</param>
        ''' -------------------------------------------------------------------
        Public Property Converter(iLayer As Integer, iConnection As Integer) As ISpatialDataConverter
            Get
                Debug.Assert(iLayer <= Me.MaxLength, "Index out of range")
                Return Me.m_converters(Math.Max(0, iLayer), iConnection - 1)
            End Get
            Set(ByVal value As ISpatialDataConverter)
                If (Me.m_converters Is Nothing) Then Me.Initialize()
                Debug.Assert(iLayer <= Me.MaxLength, "Index out of range")

                ' Is a change?
                If Not Object.ReferenceEquals(Me.m_converters(Math.Max(0, iLayer), iConnection - 1), value) Then
                    Me.m_converters(Math.Max(0, iLayer), iConnection - 1) = value
                    ' Connect converter and dataset, if possible
                    If (value IsNot Nothing) Then
                        value.Dataset = Me.Dataset(iLayer, iConnection)
                    End If
                    Me.OnChanged()
                End If
                If (value IsNot Nothing) Then
                    value.Dataset = Me.Dataset(iLayer, iConnection)
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a <see cref="ISpatialDataConverter">data converter</see> 
        ''' for layer <paramref name="iIndex"/>.
        ''' </summary>
        ''' <param name="iLayer">Layer index [0, <see cref="MaxLength"/>&lt;.</param>
        ''' <param name="iConnection">One-based index of the connection</param>
        ''' -------------------------------------------------------------------
        Public Property Dataset(iLayer As Integer, iConnection As Integer) As ISpatialDataSet
            Get
                Debug.Assert(iLayer <= Me.MaxLength, "Index out of range")
                Return Me.m_datasets(Math.Max(0, iLayer), iConnection - 1)
            End Get
            Set(ByVal value As ISpatialDataSet)
                If (Me.m_datasets Is Nothing) Then Me.Initialize()
                Debug.Assert(iLayer <= Me.MaxLength, "Index out of range")

                ' Is a change?
                If Not Object.ReferenceEquals(Me.m_datasets(Math.Max(0, iLayer), iConnection - 1), value) Then
                    Me.m_datasets(Math.Max(0, iLayer), iConnection - 1) = value
                    Me.OnChanged()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eVarNameFlags">variable name</see> for the type
        ''' of layer that this adapter operates onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property VarName() As eVarNameFlags
            Get
                Return Me.m_varName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate the core data that this adapter is responsible for.
        ''' </summary>
        ''' <param name="iTime">The one-based Ecospace time step to populate data for.</param>
        ''' <param name="dNoData">The no data value for the Ecospace layer.</param>
        ''' <param name="layer">The layers to populate. If left to null, all layers
        ''' for the implicit <see cref="VarName"/> will be populated.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function Populate(ByVal iTime As Integer, ByVal dNoData As Double, _
                                             Optional ByVal layer As cEcospaceLayer = Nothing) As Boolean

            Dim msg As cMessage = Nothing
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim dataExternal As ISpatialRaster = Nothing
            Dim dCellSize As Double = Math.Round(CDbl(bm.CellSize), 8)
            Dim layers As cEcospaceLayer() = Nothing
            Dim dt As Date
            Dim bSuccess As Boolean = True

            ' Decide which layers to update
            If (layer Is Nothing) Then
                layers = bm.Layers(Me.VarName)
            Else
                layers = New cEcospaceLayer() {layer}
            End If

            For Each layer In layers

                If Me.IsEnabled(layer.Index) Then

                    For iConnection As Integer = 1 To cSpatialDataStructures.cMAX_CONN

                        ' Is ready to go?
                        If Me.IsConnected(layer.Index, iConnection) Then

                            ' Get dataset and converter
                            Dim ds As ISpatialDataSet = Me.Dataset(layer.Index, iConnection)
                            Dim cv As ISpatialDataConverter = Me.Converter(layer.Index, iConnection)

                            ' #Yes: has data for this time step?
                            dt = Me.m_core.EcospaceTimestepToAbsoluteTime(iTime)

                            If (ds.HasDataAtT(dt)) Then
                                ' #Yes: Can lock that data?
                                If (ds.LockDataAtT(dt, dCellSize, bm.PosTopLeft, bm.PosBottomRight)) Then
                                    ' #Yes: start process of extracting external data
                                    Me.m_core.SpatialOperationLog.BeginLayerLog(iTime, dt, layer)

                                    ' Sanity check
                                    Debug.Assert(ds.IsLocked, "Dataset is not locked - something is wrong")

                                    Try
                                        ' The raster returned here MUST have the extent and projection compatible with Ecospace
                                        dataExternal = ds.GetRaster(cv, cValueID.getDataTypeID(layer.DataType, layer.DBID))
                                    Catch ex As Exception
                                        Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
                                        cLog.Write(ex, "cSpatialDataAdapter::Populate(" & layer.ToString() & ")")
                                        bSuccess = False
                                    End Try

                                    If (dataExternal IsNot Nothing) Then

                                        ' Stop any validation
                                        Dim bAllow As Boolean = layer.AllowValidation
                                        layer.AllowValidation = False

                                        Me.SaveIntermediateResults(iTime, dataExternal)

                                        ' Notify world
                                        If (Me.m_core.PluginManager IsNot Nothing) Then
                                            Me.m_core.PluginManager.EcospaceBeginLayerChange(iTime, dt, layer)
                                        End If

                                        ' Integrate data
                                        Me.Adapt(bm, layer, iConnection, iTime, dt, dataExternal, dNoData)

                                        ' Notify world
                                        If (Me.m_core.PluginManager IsNot Nothing) Then
                                            Me.m_core.PluginManager.EcospaceEndLayerChange(iTime, dt, layer)
                                        End If

                                        ' Restore layer validation
                                        layer.AllowValidation = bAllow

                                        ' Done, clean up
                                        dataExternal.Dispose()
                                        dataExternal = Nothing

                                        ' Notify core - use AddedOrRemoved flag to not dirty the DB; just broadcast the layer change
                                        ' Me.m_core.onChanged(layer, eMessageType.DataAddedOrRemoved)

                                    Else
                                        Dim strMsg As String = "cSpatialDataAdapter::Populate({0}) external data missing for T{2}, ext({3},{4}) to ({5},{6}), cell size {7}"
                                        cLog.Write(String.Format(strMsg, layer.ToString(), ds.DisplayName, iTime, bm.PosTopLeft.X, bm.PosTopLeft.Y, bm.PosBottomRight.X, bm.PosBottomRight.Y, dCellSize))
                                        Me.m_core.SpatialOperationLog.LogOperation(strMsg, eStatusFlags.ErrorEncountered)
                                        bSuccess = False

                                    End If

                                    ' Unlock dataset
                                    ds.Unlock()
                                    Me.m_core.SpatialOperationLog.EndLayerLog()
                                Else
                                    Dim strMsg As String = "cSpatialDataAdapter::Populate({0}) dataset {1} failed to load data for T{2}, ext({3},{4}) to ({5},{6}), cell size {7}"
                                    cLog.Write(String.Format(strMsg, layer.ToString(), ds.DisplayName, iTime, bm.PosTopLeft.X, bm.PosTopLeft.Y, bm.PosBottomRight.X, bm.PosBottomRight.Y, dCellSize))
                                End If
                            Else
                                Dim strMsg As String = "cSpatialDataAdapter::Populate({0}) dataset {1} missing data for T{2}, ext({3},{4}) to ({5},{6})"
                                cLog.Write(String.Format(strMsg, layer.ToString(), ds.DisplayName, iTime, bm.PosTopLeft.X, bm.PosTopLeft.Y, bm.PosBottomRight.X, bm.PosBottomRight.Y), eVerboseLevel.Detailed)
                            End If
                        End If
                    Next iConnection

                End If ' IsEnabled(layer.Index)

            Next layer

            Return bSuccess

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform pre-run initializations for all adapters such as preserving
        ''' layer data prior to a run. Individual adapters can perform their 
        ''' own initialization in.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub InitRun()
            Me.SaveLayerData()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform post-run cleanup for all adapters such as restoring
        ''' layer data after to a run, as an accompanying method to.
        ''' Individual adapters can perform their own cleanup in.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub EndRun()
            Me.RestoreLayerData()
        End Sub


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load data from an external data raster into an Ecospace array.
        ''' </summary>
        ''' <param name="bm">The <see cref="cEcospaceBasemap"/> for the scenario to load data into.</param>
        ''' <param name="layer">The <see cref="cEcospaceLayer"/> that will receive the data.</param>
        ''' <param name="iTime">The Ecospace time step to load data for.</param>
        ''' <param name="dataExternal">The <see cref="ISpatialRaster"/> that holds the loaded external data.</param>
        ''' <returns>True if successful.</returns>
        ''' <remarks>Note that this method writes values straight into the underlying data structures!</remarks>
        ''' -------------------------------------------------------------------
        Protected Friend Overridable Function Adapt(ByVal bm As cEcospaceBasemap, _
                                                    ByVal layer As cEcospaceLayer, _
                                                    ByVal iConnection As Integer, _
                                                    ByVal iTime As Integer, _
                                                    ByVal dt As Date, _
                                                    ByVal dataExternal As ISpatialRaster, _
                                                    ByVal dNoData As Double) As Boolean

            ' To ensure proper usage by inherited classes
            Debug.Assert(bm IsNot Nothing)
            Debug.Assert(layer IsNot Nothing)
            Debug.Assert(dataExternal IsNot Nothing)

            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
            Dim msg As cMessage = Nothing
            Dim sValue As Double = 0
            Dim bSuccess As Boolean = True ' Think positive. Really
            Dim iNumRows As Integer = bm.InRow
            Dim iNumCols As Integer = bm.InCol
            Dim iRow As Integer
            Dim iCol As Integer

            Dim sum As Double = 0
            Dim n As Integer = 0

            Try
                ' For all rows
                iRow = 1
                While (iRow <= iNumRows) And (bSuccess = True)
                    ' For all columns
                    iCol = 1
                    While (iCol <= iNumCols) And (bSuccess = True)
                        ' Is a water cell or is this layer affecting depth?
                        If layerDepth.IsWaterCell(iRow, iCol) Or (Me.m_varName = eVarNameFlags.LayerDepth) Then
                            ' #Yes: get value
                            sValue = dataExternal.Cell(iRow, iCol, dNoData)
                            ' Is a valid value?
                            If (sValue <> cCore.NULL_VALUE) Or (Me.m_varName = eVarNameFlags.LayerDepth) Then
                                ' #Yes: set value
                                bSuccess = bSuccess And Me.SetCell(layer, iConnection, iRow, iCol, sValue)
                                'sum += CDbl(layer.Cell(iRow, iCol))
                                n += 1
                            End If
                        Else
                            bSuccess = bSuccess And Me.SetCell(layer, iConnection, iRow, iCol, dNoData)
                        End If
                        iCol += 1
                    End While ' iCol
                    iRow += 1
                End While ' iRow

                'System.Console.WriteLine(layer.Name + " mean = " + (sum / n).ToString)

                If bSuccess Then
                    Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_APPLIED, dataExternal.ToString()), eStatusFlags.OK)
                End If

            Catch ex As Exception
                Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
                cLog.Write(ex, "cSpatialDataAdapter::Adapt(" & layer.ToString() & ")")
                bSuccess = False
            End Try

            Return bSuccess

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set a cell value into the underlying layer.
        ''' </summary>
        ''' <param name="layer">The layer to set the value into.</param>
        ''' <param name="iRow">One-based row index for setting the value.</param>
        ''' <param name="iCol">One-based column index for setting the value.</param>
        ''' <param name="sCellValueAtT">The value to set in the cell, as obtained from 
        ''' external data.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function SetCell(ByVal layer As cEcospaceLayer, _
                                               ByVal iConnection As Integer, _
                                               ByVal iRow As Integer, _
                                               ByVal iCol As Integer, _
                                               ByVal sCellValueAtT As Double) As Boolean
            Try
                layer.Cell(iRow, iCol) = sCellValueAtT
            Catch ex As Exception

                Dim strMsg As String = "cSpatialDataAdapter::SetCell({0}) at ({1},{2})={3}: exception {4}"
                cLog.Write(ex, String.Format(strMsg, layer.ToString, iCol, iRow, sCellValueAtT))

                Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_ADAPTERROR, iRow, iCol, sCellValueAtT, ex.Message), eStatusFlags.ErrorEncountered)
                Return False
            End Try
            Return True

        End Function

        Public Sub OnChanged()
            If (Me.AllowValidation) Then
                Me.m_core.onChanged(Me, eMessageType.DataModified)
            End If
        End Sub

#End Region ' Basic bits

#Region " Layer rescue "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the content of adapter-managed layers to a temporary file.
        ''' <seealso cref="RestoreLayerData"/>
        ''' </summary>
        ''' <remarks>
        ''' Note that only the content of layers <see cref="cEcospaceLayer.IsExternalData">configured to receive external data</see>
        ''' will be preserved, and only for layers of type single, integer or boolean.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Friend Sub SaveLayerData()

            ' Wipe, just in case
            For i As Integer = 0 To Me.m_core.GetCoreCounter(Me.m_coreCounter)
                For j As Integer = 1 To cSpatialDataStructures.cMAX_CONN
                    Me.m_astrLayerBackupFiles(i, j) = String.Empty
                Next
            Next

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim iNumRow As Integer = bm.InRow
            Dim iNumCol As Integer = bm.InCol
            Dim strFileName As String = ""
            Dim sw As StreamWriter = Nothing
            Dim tData As Type = Nothing

            ' For all layers
            For Each layer As cEcospaceLayer In bm.Layers(Me.m_varName)
                For iConnection As Integer = 1 To cSpatialDataStructures.cMAX_CONN

                    ' Is driven by external data?
                    If (layer.IsExternalData(iConnection)) Then
                        ' #Yes: set up a temp file and save the layer content to the file
                        Try
                            strFileName = cFileUtils.MakeTempFile(".ewetmp")
                            tData = layer.ValueType
                            sw = New StreamWriter(strFileName)
                            For iRow As Integer = 1 To iNumRow
                                For iCol As Integer = 1 To iNumCol
                                    If (iCol > 1) Then sw.Write(",")
                                    If tData Is GetType(Single) Or tData Is GetType(Integer) Then
                                        sw.Write(cStringUtils.FormatNumber(layer.Cell(iRow, iCol)))
                                    ElseIf tData Is GetType(Boolean) Then
                                        sw.Write(layer.Cell(iRow, iCol).ToString())
                                    End If
                                Next iCol
                                sw.WriteLine()
                            Next iRow

                            ' Clean up
                            sw.Flush()
                            sw.Close()
                            sw = Nothing

                            ' Store the name of the file where this layer's data was preserved
                            Me.m_astrLayerBackupFiles(layer.Index, iConnection) = strFileName
#If DEBUG Then
                            Console.WriteLine("Adapter " & Me.ToString & " saved content of layer " & layer.ToString & ", connection " & iConnection & " to " & strFileName)
#End If
                            cLog.Write("cSpatialDataAdapter::SaveLayerData successful for " & layer.Name & " into " & strFileName, eVerboseLevel.Detailed)

                        Catch ex As Exception
                            ' Log failure, plod along
                            cLog.Write(ex, "cSpatialDataAdapter::SaveLayerData " & Me.ToString & ", layer " & layer.ToString & ", file " & strFileName)
                        End Try

                    End If
                Next iConnection
            Next layer

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Restore the content of layers from a temporary file.
        ''' <seealso cref="SaveLayerData"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Sub RestoreLayerData()

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim iNumRow As Integer = bm.InRow
            Dim iNumCol As Integer = bm.InCol
            Dim tData As Type = Nothing
            Dim sr As StreamReader = Nothing
            Dim strFileName As String = ""

            For Each layer As cEcospaceLayer In bm.Layers(Me.m_varName)
                For j As Integer = 1 To cSpatialDataStructures.cMAX_CONN

                    strFileName = Me.m_astrLayerBackupFiles(layer.Index, j)
                    tData = layer.ValueType

                    If (Not String.IsNullOrWhiteSpace(strFileName) And layer.IsExternalData(j)) Then
                        Try
#If DEBUG Then
                            Console.WriteLine("Adapter " & Me.ToString & " restoring layer " & layer.ToString & " from " & strFileName)
#End If
                            sr = New StreamReader(strFileName)
                            For iRow As Integer = 1 To iNumRow
                                Dim strLine As String = sr.ReadLine
                                Dim astrFields As String() = strLine.Split(","c)
                                For iCol As Integer = 1 To iNumCol

                                    If tData Is GetType(Single) Or tData Is GetType(Integer) Then
                                        layer.Cell(iRow, iCol) = cStringUtils.ConvertToNumber(astrFields(iCol - 1), tData)
                                    ElseIf tData Is GetType(Boolean) Then
                                        layer.Cell(iRow, iCol) = Boolean.Parse(astrFields(iCol - 1))
                                    End If

                                Next iCol
                            Next iRow
                            sr.Close()
                            sr = Nothing
                            cLog.Write("cSpatialDataAdapter::RestoreLayerData successful for " & layer.Name & " from " & strFileName, eVerboseLevel.Detailed)
                        Catch ex As Exception
                            ' Whoah!
                            cLog.Write(ex, "cSpatialDataAdapter::RestoreLayerData " & Me.ToString & ", layer " & layer.ToString & ", file " & strFileName)
                        End Try
                        ' Remove this temp file
                        cFileUtils.PurgeTempFile(strFileName)
                    End If
                    Me.m_astrLayerBackupFiles(layer.Index, j) = String.Empty
                Next j
            Next layer

        End Sub

#End Region ' Layer rescue

#Region " Debugging "

        Protected Sub SaveIntermediateResults(iTime As Integer, dataExternal As ISpatialRaster)

            If Not Me.AllowSaveIntermediateResults Then Return

            Dim strPath As String = Me.getIntermediateOutputDir()
            Dim strFile As String = Me.getIntermediateFile(strPath, iTime)

            If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then Return

            dataExternal.Save(strFile)

        End Sub

#End Region ' Debugging

#Region " Intermediate output files "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the directory for storing intermedite results for debugging.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IntermediateSubDirectory As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the file name for storing intermedite results for debugging.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IntermediateFileName As String

        Protected Function getIntermediateOutputDir() As String
            If Not String.IsNullOrWhiteSpace(Me.IntermediateSubDirectory) Then
                Return Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.EcospaceMaps), Me.IntermediateSubDirectory)
            End If
            Return Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.EcospaceMaps), "_debug_")
        End Function

        Protected Function getIntermediateFile(ByVal thePath As String, iTime As Integer) As String
            If Not String.IsNullOrWhiteSpace(Me.IntermediateFileName) Then
                Return Path.Combine(thePath, cFileUtils.ToValidFileName(Me.IntermediateFileName + "_" + Me.m_core.EcospaceTimestepToAbsoluteTime(iTime).ToShortDateString + ".asc", False))
            End If
            Return Path.Combine(thePath, cFileUtils.ToValidFileName("in_" & Me.m_varName.ToString & "_" & Me.Index & "_" & iTime & ".asc", False))
        End Function

#End Region ' Intermediate output files

    End Class

End Namespace
