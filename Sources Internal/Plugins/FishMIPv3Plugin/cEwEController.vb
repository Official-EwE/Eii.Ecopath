' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEPlugin

#End Region ' Imports

Public Class cEwEController

    Public Sub New(core As cCore, data As cConfiguration)
        Me.Core = core
        Me.Data = data
    End Sub

    Public ReadOnly Property Core As cCore
    Public ReadOnly Property Data As cConfiguration

    Public ReadOnly Property EcospaceData As cEcospaceDataStructures
        Get
            Return Me.Core.m_EcoSpaceData
        End Get
    End Property

    Public Sub ConfigEwE()

        Try

            Dim effman As cFishingEffortShapeManger = Me.Core.FishingEffortShapeManager
            Dim datman As cSpatialDataConnectionManager = Me.Core.SpatialDataConnectionManager
            Dim setman As cSpatialDataSetManager = datman.DatasetManager

            effman.ResetToDefaults()

            ' This code presumes that external data connections are consistently named:
            ' <esm>_<climscenario>_<layer>, where
            ' - esm:      gfdl, ipsl (first characters up to '-')
            ' - scenario: historical, pi, esm*
            ' - layer:    layer name, including stratification

            ' Find datasets and apply 'em
            For Each var As String In Me.Data.DriverLayerNames

                Dim datasets As New List(Of ISpatialDataSet)
                Dim bIsScalar As Boolean = var.EndsWith("phy")
                Dim i As Integer = Me.Data.GCMVarDriverLayerMapping(var)

                If (i >= 0) Then

                    For iPeriod As Integer = 0 To Me.Data.Periods.Count - 1
                        Dim p As cPeriod = Me.Data.Periods(iPeriod)
                        Dim ds As ISpatialDataSet = FindDataset(Me.Data.ClimateModel, Me.Data.ClimateScenarioForPeriod(p.Name), var)

                        If (ds IsNot Nothing) Then
                            Dim bDuplicate As Boolean = False
                            For Each dsTest As ISpatialDataSet In datasets
                                bDuplicate = bDuplicate Or Guid.Equals(ds.GUID, dsTest)
                            Next
                            If Not bDuplicate Then
                                datasets.Add(ds)
                            End If
                        End If
                    Next

                    Console.WriteLine("Applying {0} dataset(s) to {1}", datasets.Count, var)
                    If bIsScalar Then
                        Me.Apply(datman.Adapter(eVarNameFlags.LayerBiomassForcing), i,
                                 datasets.ToArray(), Me.Data.LayerScaling(Me.Data.ClimateModel, var))
                    Else
                        Me.Apply(datman.Adapter(eVarNameFlags.LayerDriver), i, datasets.ToArray())
                    End If
                End If

            Next

            Dim iTS As Integer = Me.Data.Fishing
            If (iTS > 0) Then
                Me.Core.LoadTimeSeries(iTS, True)
            Else
                Me.Core.LoadTimeSeries(0)
                For iShape As Integer = 0 To effman.Shapes.Count - 1
                    Dim shape As cForcingFunction = effman(iShape)
                    Dim bIsLast As Boolean = (iShape = effman.Shapes.Count - 1)
                    shape.LockUpdates()
                    For i As Integer = 0 To shape.nPoints
                        shape.ShapeData(i) = 0
                    Next i
                    ' Cheat: update every shape, but only force an update NOTIFICATION on the very last shape
                    If (bIsLast) Then
                        shape.Update()
                    End If
                    shape.UnlockUpdates(bIsLast)
                Next
            End If
            datman.Update(ForceUpdate:=True)
        Catch ex As Exception

        End Try

    End Sub

    Public Function CalculatePhyScalar(gcm As String, var As String) As Single

        Dim bIsScalar As Boolean = var.EndsWith("phy")
        If Not bIsScalar Then Return cCore.NULL_VALUE

        Dim datman As cSpatialDataConnectionManager = Me.Core.SpatialDataConnectionManager
        Dim setman As cSpatialDataSetManager = datman.DatasetManager
        Dim adt As cSpatialScalarDataAdapterBase = DirectCast(datman.Adapter(eVarNameFlags.LayerBiomassForcing), cSpatialScalarDataAdapterBase)
        Dim iLayer As Integer = Me.Data.GCMVarDriverLayerMapping(var)
        Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters
        Dim sBiomass As Single = Me.Core.EcoPathGroupOutputs(iLayer).Biomass
        Dim iPeriod As Integer = Me.Data.GetPeriodNo(Me.Core.RunStartYear)
        If (iPeriod < 0) Then Return cCore.NULL_VALUE

        Dim p As cPeriod = Me.Data.Periods(iPeriod)
        Dim ds As ISpatialDataSet = FindDataset(gcm, Me.Data.ClimateScenarioForPeriod(p.Name), var)
        If (ds Is Nothing) Then Return cCore.NULL_VALUE

        adt.BackupConnections(iLayer)

        Dim dScalar As Double = 1.0
        Dim conn As cSpatialDataConnection = adt.AddConnection(iLayer)
        conn.Dataset = ds
        conn.Scale = 1
        conn.ScaleType = cSpatialScalarDataAdapterBase.eScaleType.Relative

        Dim comp As cDatasetCompatilibity.eCompatibilityTypes = adt.CalculateScaleFromEcopathTimePeriod(iLayer, conn, 1, dScalar)
        If comp >= cDatasetCompatilibity.eCompatibilityTypes.PartialSpatial And dScalar <> 0 Then
            dScalar = (sBiomass / dScalar)
        Else
            dScalar = cCore.NULL_VALUE
        End If

        adt.RestoreConnections(iLayer)

        Return CSng(dScalar)

    End Function

    ''' <summary>
    ''' Applies drivers to a given adapter and layer.
    ''' </summary>
    ''' <param name="adt">The adt.</param>
    ''' <param name="iLayer">The i layer.</param>
    ''' <param name="drivers">The drivers for each period, where periods are ordered in time.</param>
    ''' <param name="scaling">The scaling.</param>
    Private Sub Apply(adt As cSpatialDataAdapter, iLayer As Integer, drivers As ISpatialDataSet(), Optional scaling As Single = 1)
        Try
            If (iLayer <= 0) Then Return

            Dim conn As cSpatialDataConnection = Nothing
            Dim conns As cSpatialDataConnection() = adt.Connections(iLayer)
            For Each conn In conns
                adt.RemoveConnection(iLayer, conn)
            Next

            For i As Integer = 0 To drivers.Count - 1
                conn = adt.AddConnection(iLayer)
                conn.Dataset = drivers(i)
                conn.Scale = scaling
                conn.ScaleType = cSpatialScalarDataAdapterBase.eScaleType.Relative
            Next
        Catch ex As Exception

        End Try

    End Sub

    Private Function FindDataset(gcm As String, clim As String, var As String) As ISpatialDataSet

        Dim effman As cFishingEffortShapeManger = Me.Core.FishingEffortShapeManager
        Dim datman As cSpatialDataConnectionManager = Me.Core.SpatialDataConnectionManager
        Dim setman As cSpatialDataSetManager = datman.DatasetManager

        Dim dsname As String = gcm & "_" & clim & "_" & var
        Return setman.Find(dsname)

    End Function

End Class
