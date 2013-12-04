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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
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
        Protected m_converters() As ISpatialDataConverter
        ''' <summary>Dataset for each layer.</summary>
        Protected m_datasets() As ISpatialDataSet

        ''' <summary>Ecospace variable to operate onto.</summary>
        Protected m_varName As eVarNameFlags = Nothing
        ''' <summary>Core counter that this adapter operates onto.</summary>
        Protected m_coreCounter As eCoreCounterTypes = eCoreCounterTypes.NotSet

        ''' <summary>Flag, indicating whether the content of input layers needs
        ''' to be preserved: layer data is then preserved on first overwrite,
        ''' and restored when a run finished. Preserved layer data is maintained
        ''' in temporary files.</summary>
        Private m_bRestoreLayerContent As Boolean = True
        ''' <summary>File names of preserved layers.</summary>
        Private m_astrLayerFiles() As String

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

            Dim iNumItems As Integer = Math.Max(0, Me.m_core.GetCoreCounter(Me.m_coreCounter))

            ReDim Me.m_converters(iNumItems)
            ReDim Me.m_datasets(iNumItems)
            ReDim Me.m_astrLayerFiles(iNumItems)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the maximum number of layers for this adapter.
        ''' </summary>
        ''' <returns>The number of layers for this adapter.</returns>
        ''' -------------------------------------------------------------------
        Public Function Length() As Integer
            Return Me.m_converters.Length
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return whether a layer in this adapter is connected to external data.
        ''' </summary>
        ''' <param name="iIndex">The one-based index of the layer to query, or
        ''' <see cref="cCore.NULL_VALUE"/> if irrelevant.</param>
        ''' -------------------------------------------------------------------
        Public Function IsConnected(iIndex As Integer) As Boolean

            Dim cv As ISpatialDataConverter = Me.Converter(iIndex)
            Dim ds As ISpatialDataSet = Me.Dataset(iIndex)

            If (ds Is Nothing) Then Return False
            If (Not ds.IsConfigured()) Then Return False

            If String.IsNullOrWhiteSpace(ds.ConversionFormat) Then Return True

            If (cv Is Nothing) Then Return False
            Return cv.IsConfigured()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a <see cref="ISpatialDataSet">data set</see> for layer <paramref name="iIndex"/>.
        ''' </summary>
        ''' <param name="iIndex">Layer index [0, <see cref="Length"/>&lt;.</param>
        ''' -------------------------------------------------------------------
        Public Property Converter(iIndex As Integer) As ISpatialDataConverter
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_converters(Math.Max(0, iIndex))
            End Get
            Set(ByVal value As ISpatialDataConverter)
                If (Me.m_converters Is Nothing) Then Me.Initialize()
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_converters(Math.Max(0, iIndex)) = value

                ' Connect converter and dataset, if possible
                If (value IsNot Nothing) Then
                    value.Dataset = Me.Dataset(iIndex)
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a <see cref="ISpatialDataConverter">data converter</see> 
        ''' for layer <paramref name="iIndex"/>.
        ''' </summary>
        ''' <param name="iIndex">Layer index [0, <see cref="Length"/>&lt;.</param>
        ''' -------------------------------------------------------------------
        Public Property Dataset(iIndex As Integer) As ISpatialDataSet
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_datasets(Math.Max(0, iIndex))
            End Get
            Set(ByVal value As ISpatialDataSet)
                If (Me.m_datasets Is Nothing) Then Me.Initialize()
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_datasets(Math.Max(0, iIndex)) = value
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
        ''' Perform pre-run initializations. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub InitRun()
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Me.SaveLayerData(bm)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform post-run cleanup. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub EndRun()
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Me.RestoreLayerData(bm)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate the core data that this adapter is responsible for.
        ''' </summary>
        ''' <param name="iTime">The one-based Ecospace time step to populate data for.</param>
        ''' <param name="dNoData">The no data value for the Ecospace layer.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function Populate(ByVal iTime As Integer, dNoData As Double) As Boolean

            Dim msg As cMessage = Nothing
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim layer As cEcospaceLayer = Nothing
            Dim dataExternal As ISpatialRaster = Nothing
            Dim dCellSize As Double = Math.Round(CDbl(bm.CellSize), 8)
            Dim dt As Date
            Dim bSuccess As Boolean = True

            ' For each layer for this adapter
            For Each layer In bm.Layers(Me.m_varName)

                ' Get dataset and converter
                Dim ds As ISpatialDataSet = Me.Dataset(layer.Index)
                Dim cv As ISpatialDataConverter = Me.Converter(layer.Index)

                ' Is ready to go?
                If Me.IsConnected(layer.Index) Then

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
                                Me.Adapt(bm, layer, iTime, dt, dataExternal, dNoData)

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
            Next

            Return bSuccess

        End Function

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
                            If (sValue <> cCore.NULL_VALUE) Then
                                ' #Yes: set value
                                bSuccess = bSuccess And Me.SetCell(layer, iRow, iCol, sValue)
                            End If
                        Else
                            bSuccess = bSuccess And Me.SetCell(layer, iRow, iCol, dNoData)
                        End If
                        iCol += 1
                    End While ' iCol
                    iRow += 1
                End While ' iRow

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

#End Region ' Basic bits

#Region " Layer rescue "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the content of an externally driven layer should be
        ''' preserved when external data is received, and should be restored
        ''' after a run has completed. If set to true, the content of any layer 
        ''' that is configured to receive external data will be preserved in a 
        ''' temporary file, from which the content is restored at the end of a 
        ''' run.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property RestoreLayerContent As Boolean
            Get
                Return Me.m_bRestoreLayerContent
            End Get
            Set(value As Boolean)
                Me.m_bRestoreLayerContent = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the content of adapter-managed layers to a temporary file.
        ''' <seealso cref="RestoreLayerData"/>
        ''' <seealso cref="RestoreLayerContent"/>
        ''' </summary>
        ''' <param name="bm">Ecospace base map that states the size of the layer grid.</param>
        ''' <remarks>
        ''' Note that only the content of layers <see cref="cEcospaceLayer.IsExternalData">configured to receive external data</see>
        ''' will be preserved, and only for layers of type single, integer or boolean.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub SaveLayerData(bm As cEcospaceBasemap)

            ' Wipe, just in case
            For i As Integer = 0 To Me.m_astrLayerFiles.Length - 1
                Me.m_astrLayerFiles(i) = String.Empty
            Next

            ' Early bail out
            If (Not Me.RestoreLayerContent) Then Return

            Dim iNumRow As Integer = bm.InRow
            Dim iNumCol As Integer = bm.InCol
            Dim strFileName As String = ""
            Dim sw As StreamWriter = Nothing

            ' For all layers
            For Each layer As cEcospaceLayer In bm.Layers(Me.m_varName)
                ' Is driven by external data?
                If (layer.IsExternalData) Then
                    ' #Yes: set up a temp file and save the layer content to the file
                    Try
                        strFileName = cFileUtils.MakeTempFile(".ewetmp")
                        sw = New StreamWriter(strFileName)
                        For iRow As Integer = 1 To iNumRow
                            For iCol As Integer = 1 To iNumCol
                                If (iCol > 1) Then sw.Write(",")

                                sw.Write(layer.Cell(iRow, iCol).ToString())
                               
                            Next iCol
                            sw.WriteLine()
                        Next iRow

                        ' Clean up
                        sw.Flush()
                        sw.Close()
                        sw = Nothing

                        ' Store temp file name for restoration later on
                        Me.m_astrLayerFiles(layer.Index) = strFileName

                        Console.WriteLine("Adapter " & Me.ToString & " saved content of layer " & layer.ToString & " to " & strFileName)

                    Catch ex As Exception
                        ' Log failure, plod along
                        cLog.Write(ex, "cSpatialDataAdapter::SaveLayerData " & Me.ToString & ", layer " & layer.ToString & ", file " & strFileName)
                    End Try

                End If

            Next layer

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Restore the content of layers from a temporary file.
        ''' <seealso cref="SaveLayerData"/>
        ''' <seealso cref="RestoreLayerContent"/>
        ''' </summary>
        ''' <param name="bm">Ecospace base map that states the size of the layer grid.</param>
        ''' -------------------------------------------------------------------
        Private Sub RestoreLayerData(bm As cEcospaceBasemap)

            Dim iNumRow As Integer = bm.InRow
            Dim iNumCol As Integer = bm.InCol
            Dim tData As Type = Nothing
            Dim sr As StreamReader = Nothing
            Dim strFileName As String = ""

            For Each layer As cEcospaceLayer In bm.Layers(Me.m_varName)

                strFileName = Me.m_astrLayerFiles(layer.Index)
                tData = layer.ValueType

                If (Not String.IsNullOrWhiteSpace(strFileName) And layer.IsExternalData) Then
                    Try
                        Console.WriteLine("Adapter " & Me.ToString & " restoring layer " & layer.ToString & " from " & strFileName)
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
                    Catch ex As Exception
                        ' Whoah!
                        cLog.Write(ex, "cSpatialDataAdapter::RestoreLayerData " & Me.ToString & ", layer " & layer.ToString & ", file " & strFileName)
                    End Try
                    Me.m_astrLayerFiles(layer.Index) = ""
                    cFileUtils.PurgeTempFile(strFileName)
                End If

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
