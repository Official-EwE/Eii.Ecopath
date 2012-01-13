#Region " Imports "

Option Strict On
Imports EwEUtils.SpatialData
Imports EwEUtils.Core
Imports System.Drawing
Imports EwEUtils

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Update core data at a given timestep.
    ''' Provide connection information.
    ''' </summary>
    Public Class cDefaultSpatialDataAdapter
        Inherits cCoreInputOutputBase
        Implements ISpatialDataAdapter

        ''' <summary>Converter to transform incoming data.</summary>
        Private m_converter As ISpatialDataConverter = Nothing
        ''' <summary>Dataset for accessing incoming data.</summary>
        Private m_dataset As ISpatialDataSet = Nothing
        ''' <summary>Ecospace variable to operate onto.</summary>
        Private m_varName As eVarNameFlags = Nothing
        ''' <summary>Core counter that provides an optional index to operate onto.</summary>
        Private m_coreCounter As eCoreCounterTypes = eCoreCounterTypes.NotSet
          ''' <summary>Flag stating wether dataset date and core data have to match</summary>
        Private m_bSyncDate As Boolean = False

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, Optional ByVal corecounter As eCoreCounterTypes = eCoreCounterTypes.NotSet)

            MyBase.New(core)

            Me.m_dataType = eDataTypes.SpatialDataSource
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.m_varName = varName
            Me.m_coreCounter = corecounter

            Me.Converter = Nothing

            Me.DBID = -1

            Me.AllowValidation = False

            Me.AllowValidation = True
        End Sub

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataAdapter.IsConnected"/>"
        ''' -------------------------------------------------------------------
        Public Overridable ReadOnly Property IsConnected() As Boolean _
            Implements ISpatialDataAdapter.IsConnected
            Get
                ' ToDo: check if both converter and dataset are configured?
                Return (Me.m_converter IsNot Nothing) And (Me.m_dataset IsNot Nothing)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataAdapter.Converter"/>"
        ''' -------------------------------------------------------------------
        Public Property Converter() As ISpatialDataConverter _
            Implements ISpatialDataAdapter.Converter
            Get
                Return m_converter
            End Get
            Set(ByVal value As ISpatialDataConverter)
                m_converter = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataAdapter.Dataset"/>"
        ''' -------------------------------------------------------------------
        Public Property Dataset() As ISpatialDataSet _
            Implements ISpatialDataAdapter.Dataset
            Get
                Return Me.m_dataset
            End Get
            Set(ByVal value As ISpatialDataSet)
                Me.m_dataset = value
            End Set
        End Property

#End Region ' Public interfaces

#Region " Mandatory overrides "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataAdapter.VarName"/>"
        ''' -------------------------------------------------------------------
        Public ReadOnly Property VarName() As eVarNameFlags _
            Implements ISpatialDataAdapter.VarName
            Get
                Return Me.m_varName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataAdapter.Populate"/>
        ''' -------------------------------------------------------------------
        Friend Overridable Function Populate(ByVal iTime As Integer) As Boolean _
            Implements ISpatialDataAdapter.Populate

            ' Place a given raster into the core data
            ' Note that this method writes values straight into the underlying data structures.

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
            Dim layer As cEcospaceLayer = Nothing
            Dim dataExternal As ISpatialRaster = Nothing
            Dim sValue As Single
            Dim iStart As Integer = cCore.NULL_VALUE
            Dim iEnd As Integer = cCore.NULL_VALUE
            Dim dt As Date = Me.ToDataSetTime(iTime)
            Dim bSuccess As Boolean = False

            If (Me.Converter Is Nothing) Then Return bSuccess
            If (Me.Dataset Is Nothing) Then Return bSuccess
            If (Not Me.Dataset.HasDataAtT(dt, bm.PosTopLeft, bm.PosBottomRight)) Then Return bSuccess

            ' Is an indexed layer?
            If (Me.m_coreCounter <> eCoreCounterTypes.NotSet) Then
                ' #Yes: determine index iteration range
                iStart = 0 : iEnd = Me.m_core.GetCoreCounter(Me.m_coreCounter) - 1
            Else
                ' #No: iterate once
                iStart = cCore.NULL_VALUE : iEnd = cCore.NULL_VALUE
            End If

            If Me.Dataset.LoadDataAtT(dt, bm.PosTopLeft, bm.PosBottomRight) Then

                ' For all indexes
                For iIndex As Integer = iStart To iEnd
                    ' Get layer
                    layer = bm.Layer(Me.VarName, iIndex)
                    ' Is layer found?
                    If (layer IsNot Nothing) Then
                        ' #Yes: extract external data
                        Try
                            ' The raster returned here MUST have the extent and projection compatible with Ecospace
                            dataExternal = Me.Dataset.GetRaster(bm.CellSize, Me.Converter, layer.Name)
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

                        End If

                    End If
                Next

            End If
            Return bSuccess

        End Function

#End Region ' Mandatory overrides

#Region " Translations "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a time step to a time usable by the attached dataset.
        ''' </summary>
        ''' <param name="iTime">The Ecospace time step to populate data for.</param>
        ''' <returns></returns>
        ''' <remarks>Takes relative dates into account.</remarks>
        ''' -------------------------------------------------------------------
        Protected Function ToDataSetTime(ByVal iTime As Integer) As DateTime

            ' Get Ecopath start year
            Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
            Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
            Dim dateTimeStep As DateTime

            Try

                ' Translate ecospace time step to year and month
                ' *** Note that time steps that are fractions of months are rounded up to the first of the month! ***
                Dim dTimeStepYearFraction As Double = iTime * ecospaceDS.TimeStep
                Dim iTimeStepYear As Integer = CInt(Math.Floor(dTimeStepYearFraction))
                Dim iTimeStepMonth As Integer = CInt(((dTimeStepYearFraction - iTimeStepYear) * 12))

                ' Should iTime be interpreted as relative to the dataset start time?
                If ecospaceDS.AdapterUseRelativeTime Then
                    ' #Yes: use year and month as relative to dataset start time
                    If (Me.m_dataset IsNot Nothing) Then
                        Dim dateSetStart As DateTime = Me.m_dataset.TimeStart
                        If (dateSetStart < DateTime.MaxValue) Then
                            ' Add run offset to dataset start time
                            dateTimeStep = New DateTime(dateSetStart.Ticks)
                            dateTimeStep.AddYears(iTimeStepYear)
                            dateTimeStep.AddMonths(iTimeStepMonth)
                        End If
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

#Region " Variables by dot operator "

#End Region ' Variables by dot operator

    End Class

End Namespace
