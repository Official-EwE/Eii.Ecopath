#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Update core data at a given timestep.
    ''' Provide connection information.
    ''' </summary>
    Public Class cSpatialDataAdapter
        Inherits cCoreInputOutputBase

#Region " Private vars "

        ''' <summary>Converter for each layer.</summary>
        Private m_converters() As ISpatialDataConverter
        ''' <summary>Dataset for each layer.</summary>
        Private m_datasets() As ISpatialDataSet
        ''' <summary>Ecospace variable to operate onto.</summary>
        Private m_varName As eVarNameFlags = Nothing
        ''' <summary>Core counter that this adapter operates onto.</summary>
        Private m_coreCounter As eCoreCounterTypes = eCoreCounterTypes.NotSet
        ''' <summary>Flag stating wether dataset date and core data have to match</summary>
        Private m_bSyncDate As Boolean = False

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, cc As eCoreCounterTypes)

            MyBase.New(core)

            Me.m_dataType = eDataTypes.SpatialDataSource
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.m_coreCounter = cc
            Me.m_varName = varName

            Me.DBID = -1
            Me.AllowValidation = True
        End Sub

#End Region ' Constructor

#Region " Basic bits "

        Friend Sub SetDefaults()
            Dim iNumItems As Integer = Math.Max(0, Me.m_core.GetCoreCounter(Me.m_coreCounter))
            ReDim Me.m_converters(iNumItems)
            ReDim Me.m_datasets(iNumItems)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of layers for this adapter.
        ''' </summary>
        ''' <returns>The number of layers for this adapter.</returns>
        ''' -------------------------------------------------------------------
        Public Function Length() As Integer
            Return Me.m_converters.Length
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return whether a layer is connected to external data.
        ''' </summary>
        ''' <param name="iIndex">The one-based index of the layer to query, or
        ''' <see cref="cCore.NULL_VALUE"/> if irrelevant.</param>
        ''' -------------------------------------------------------------------
        Public Function IsConnected(iIndex As Integer) As Boolean

            Dim cv As ISpatialDataConverter = Me.Converter(iIndex)
            Dim ds As ISpatialDataSet = Me.Dataset(iIndex)

            ' ToDo: check if both converter and dataset are configured?
            Return (cv IsNot Nothing) And (ds IsNot Nothing)

        End Function

        Public Property Converter(iIndex As Integer) As ISpatialDataConverter
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_converters(Math.Max(0, iIndex))
            End Get
            Set(ByVal value As ISpatialDataConverter)
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_converters(Math.Max(0, iIndex)) = value
            End Set
        End Property

        Public Property Dataset(iIndex As Integer) As ISpatialDataSet
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_datasets(Math.Max(0, iIndex))
            End Get
            Set(ByVal value As ISpatialDataSet)
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_datasets(Math.Max(0, iIndex)) = value
            End Set
        End Property

        Public ReadOnly Property VarName() As eVarNameFlags
            Get
                Return Me.m_varName
            End Get
        End Property

        Protected Friend Overridable Function Populate(ByVal iTime As Integer) As Boolean

            ' Place a given raster into the core data
            ' Note that this method writes values straight into the underlying data structures.

            ' ToDo: split this method up in parts

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
            Dim layer As cEcospaceLayer = Nothing
            Dim dataExternal As ISpatialRaster = Nothing
            Dim dCellSize As Double = CDbl(bm.CellSize)
            Dim sValue As Single
            Dim dt As Date
            Dim bSuccess As Boolean = False

            ' For each layer for this adapter
            For Each layer In bm.Layers(Me.m_varName)

                ' Get dataset and converter
                Dim ds As ISpatialDataSet = Me.Dataset(layer.Index)
                Dim cv As ISpatialDataConverter = Me.Converter(layer.Index)

                ' Has both?
                If (ds IsNot Nothing) And (cv IsNot Nothing) Then
                    ' #Yes: has data for this time step?
                    dt = Me.ToDataSetTime(ds, iTime)
                    If (ds.HasDataAtT(dt, bm.PosTopLeft, bm.PosBottomRight)) Then
                        ' #Yes: Can load that data?
                        If (ds.LoadDataAtT(dt, dCellSize, bm.PosTopLeft, bm.PosBottomRight)) Then
                            ' #Yes: extract external data
                            Try
                                ' The raster returned here MUST have the extent and projection compatible with Ecospace
                                dataExternal = ds.GetRaster(cv, layer.Name)
                            Catch ex As Exception
                                ' User should know this
                                cLog.Write(ex)
                            End Try

                            If (dataExternal IsNot Nothing) Then

                                Dim bAllow As Boolean = layer.AllowValidation
                                layer.AllowValidation = False

                                Try
                                    ' For all rows
                                    For iRow As Integer = 1 To bm.InRow
                                        ' For all columns
                                        For iCol As Integer = 1 To bm.InCol
                                            ' Is a water cell or is this layer affecting depth?
                                            If layerDepth.IsWaterCell(iRow, iCol) Or (Me.m_varName = eVarNameFlags.LayerDepth) Then
                                                ' #Yes: get value
                                                sValue = CSng(dataExternal.Cell(iRow, iCol))
                                                ' Is a valid value?
                                                If (sValue <> cCore.NULL_VALUE) Then
                                                    ' #Yes: set value
                                                    ' Hack and slash for now
                                                    layer.Cell(iRow, iCol) = sValue
                                                End If
                                            End If
                                        Next iCol
                                    Next iRow

                                    bSuccess = True

                                Catch ex As Exception
                                    ' Whoah!
                                    cLog.Write(ex)
                                End Try

                                ' Restore layer validation
                                layer.AllowValidation = bAllow
                                ' Update visuals
                                layer.Invalidate()
                                ' Done
                                dataExternal.Dispose()
                                dataExternal = Nothing
                            End If
                            ' Clean up
                            ds.Unload()
                        End If
                    End If
                End If
            Next

            Return bSuccess

        End Function

#End Region ' Basic bits

#Region " Translations "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a time step to a time usable by the attached dataset.
        ''' </summary>
        ''' <param name="iTime">The Ecospace time step to populate data for.</param>
        ''' <returns></returns>
        ''' <remarks>Takes relative dates into account.</remarks>
        ''' -------------------------------------------------------------------
        Protected Function ToDataSetTime(ByVal ds As ISpatialDataSet, ByVal iTime As Integer) As DateTime

            ' Get Ecopath start year
            Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
            Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
            Dim spatialDS As cSpatialDataStructures = Me.m_core.m_SpatialData
            Dim dateTimeStep As DateTime

            Try

                ' Translate ecospace time step to year and month
                ' *** Note that time steps that are fractions of months are rounded up to the first of the month! ***
                Dim dTimeStepYearFraction As Double = iTime * ecospaceDS.TimeStep
                Dim iTimeStepYear As Integer = CInt(Math.Floor(dTimeStepYearFraction))
                Dim iTimeStepMonth As Integer = CInt(((dTimeStepYearFraction - iTimeStepYear) * 12))

                ' Should iTime be interpreted as relative to the dataset start time?
                If spatialDS.AdapterUseRelativeTime Then
                    ' #Yes: use year and month as relative to dataset start time
                    Dim dateSetStart As DateTime = ds.TimeStart
                    If (dateSetStart < DateTime.MaxValue) Then
                        ' Add run offset to dataset start time
                        dateTimeStep = New DateTime(dateSetStart.Ticks).AddYears(iTimeStepYear).AddMonths(iTimeStepMonth)
                    End If
                Else
                    ' #No: Return absolute date
                    dateTimeStep = New DateTime(Math.Max(ecopathDS.FirstYear, 1) + iTimeStepYear, iTimeStepMonth + 1, 1)
                End If

                Return dateTimeStep

            Catch ex As Exception

            End Try
            Return Date.Now

        End Function

#End Region ' Translations

    End Class

End Namespace
