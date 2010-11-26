#Region " Imports "

Option Strict On
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports System.IO

#End Region ' Imports

''' <summary>
''' Class to export an Ecosim time step to a new Ecopath model.
''' </summary>
''' <remarks></remarks>
Public Class cEcopathModelFromEcosim

    Private m_core As cCore = Nothing

    Public Sub New(ByVal core As cCore)
        Me.m_core = core
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a model from the current Ecosim time step.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <param name="strModelName"></param>
    ''' <param name="results"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Create(ByVal strFileName As String, _
                           ByVal strModelName As String, _
                           ByVal results As cEcoSimResults) As eDatasourceAccessType

        Dim coreDest As New cCore()
        Dim db As cEwEDatabase = New cEwEAccessDatabase()
        Dim atResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown
        Dim bSucces As Boolean = False

        coreDest.InitCore()
        coreDest.PluginManager = Nothing

        If String.IsNullOrEmpty(Path.GetExtension(strFileName)) Then
            strFileName &= cDataSourceFactory.GetDefaultExtension(eDataSourceTypes.ACCDB)
        End If

        atResult = db.Create(strFileName, strModelName, True)
        If (atResult <> eDatasourceAccessType.Created) Then Return atResult

        Dim ds As IEwEDataSource = cDataSourceFactory.Create(strFileName)
        If ds.Open(strFileName, coreDest) = eDatasourceAccessType.Opened Then
            If coreDest.LoadModel(ds) Then
                If Me.CreateItems(coreDest) Then
                    Me.PopulateItems(coreDest, results)
                End If
            End If
        End If

        coreDest.CloseModel()

        db = Nothing
        ds = Nothing
        coreDest = Nothing

        Return eDatasourceAccessType.Created

    End Function

    ''' <summary>
    ''' Create all groups and fleets in the target model.
    ''' </summary>
    ''' <param name="coreTgt"></param>
    ''' <returns></returns>
    Private Function CreateItems(ByVal coreTgt As cCore) As Boolean

        Dim bSuccess As Boolean = True
        Dim iNew As Integer = 0
        Dim iIDNew As Integer = 0
        Dim aiGroupID(Me.m_core.nGroups) As Integer

        If Not coreTgt.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

        Try

            For iGroup As Integer = 1 To Me.m_core.nGroups
                Dim grpSrc As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iGroup)
                bSuccess = bSuccess And coreTgt.AddGroup(grpSrc.Name, grpSrc.PP, grpSrc.VBK, iNew, iIDNew)
                aiGroupID(iGroup) = iIDNew
            Next

            For iFleet As Integer = 1 To Me.m_core.nFleets
                Dim fltSrc As cFleetInput = Me.m_core.FleetInputs(iFleet)
                bSuccess = bSuccess And coreTgt.AddFleet(fltSrc.Name, iNew, iIDNew)
            Next

            For iStanza As Integer = 1 To Me.m_core.nStanzas
                Dim sgSrc As cStanzaGroup = Me.m_core.StanzaGroups(iStanza)
                Dim aiGroupIDs(sgSrc.NStanzas) As Integer
                Dim aiStartAges(sgSrc.NStanzas) As Integer
                For iLifeStage As Integer = 1 To sgSrc.NStanzas
                    aiGroupIDs(iLifeStage) = aiGroupID(sgSrc.iGroups(iLifeStage))
                    aiStartAges(iLifeStage) = sgSrc.StartAge(iLifeStage)
                Next
                bSuccess = bSuccess And coreTgt.AppendStanza(sgSrc.Name, aiGroupIDs, aiStartAges, iIDNew)
            Next

        Catch ex As Exception
            bSuccess = False
        End Try

        coreTgt.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath, bSuccess)

        Return bSuccess

    End Function

    Private Function PopulateItems(ByVal coreDest As cCore, _
                                   ByVal results As cEcoSimResults) As Boolean

        Dim bSuccess As Boolean = True
        Dim iTime As Integer = CInt(results.CurrentT)
        Dim pathSrc As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim simSrc As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim pathDest As cEcopathDataStructures = coreDest.m_EcoPathData

        ' Dirty destination core
        coreDest.DataSource.SetChanged(eCoreComponentType.EcoPath)
        coreDest.StateMonitor.UpdateDataState(coreDest.DataSource)

        ' Copy Ecopath data
        pathSrc.copyTo(pathDest, False)

        ' Clear data that is not going to be copied
        pathDest.NumEcosimScenarios = 0
        pathDest.NumEcospaceScenarios = 0
        pathDest.NumEcotracerScenarios = 0
        pathDest.NumPedigreeLevels = 0
        pathDest.NumPedigreeVariables = 0

        ' Overwrite bits with Ecosim data at time step 'iTime'
        Dim sArea As Single = Me.m_core.EwEModel.Area

        ' Populate groups
        For iGroup As Integer = 1 To Me.m_core.nGroups

            ' Bi(i) = DCPct(i, 1)
            pathDest.Binput(iGroup) = simSrc.DCPct(iGroup, 1)
            ' Catch(i) = Bi(i) * FishTime(i)
            pathDest.fCatch(iGroup) = pathDest.Binput(iGroup) * simSrc.FishTime(iGroup)
            ' Ex(i) = Catch(i)
            pathDest.Ex(iGroup) = pathDest.fCatch(iGroup)

            ' PBi(i) = loss(i) / Bi(i)
            pathDest.PBinput(iGroup) = simSrc.loss(iGroup) / pathDest.Binput(iGroup)
            ' QBi(i) = DCPct(i, 2) 'the following has been updated: Eatenby(i) / bb(i)
            pathDest.QBinput(iGroup) = simSrc.DCPct(iGroup, 2)
            ' EEi(i) = -99
            pathDest.EEinput(iGroup) = cCore.NULL_VALUE
            ' BAi(i) = (Bi(i) - DCPct(i, 0)) * StepsPerYear ' / TimeStep 'dcpct() stores the bb() from previous round
            pathDest.BA(iGroup) = (pathDest.Binput(iGroup) - simSrc.DCPct(iGroup, 0)) * simSrc.StepsPerMonth * cCore.N_MONTHS
            ' Emigrationi(i) = Emig(i) * Bi(i) '
            pathDest.Emigration(iGroup) = pathDest.Emig(iGroup) * pathDest.Binput(iGroup)
            ' BHi(i) = Bi(i) / Area(i)
            pathDest.BHinput(iGroup) = pathDest.Binput(iGroup) / pathSrc.Area(iGroup)

        Next

        For i As Integer = 1 To Me.m_core.nGroups
            For j As Integer = 1 To Me.m_core.nGroups
                'DCi(i, j) = 0        'don't leave any dc leftovers
                pathDest.DCInput(i, j) = 0
                'If QBi(i) > 0 Then DCi(i, j) = DCMean(i, j) '/ (QBi(i) * Bi(i))
                If pathDest.QBinput(i) > 0 Then pathDest.DCInput(i, j) = simSrc.DCMean(i, j)
            Next
        Next
        pathDest.SumDCToOne()

        'immigration is constant rate and is not changed by ecosim so no need to change
        For i As Integer = 1 To Me.m_core.nGroups
            Dim SumEf As Single = 0.0
            For j As Integer = 1 To pathSrc.NumFleet
                ' SumEf = SumEf + FishRateGear(j, itime) * FishMGear(j, i)
                SumEf += simSrc.FishRateGear(j, iTime) * simSrc.FishMGear(j, i)
            Next
            For j As Integer = 1 To Me.m_core.nFleets
                Dim Sum As Single = 0
                Dim Z As Single = pathSrc.Landing(j, i) + pathSrc.Discard(j, i)
                ' If SumEf > 0 Then Sum = BB(i) * FishTime(i) * FishRateGear(j, iTime) * FishMGear(j, i) / SumEf
                If SumEf > 0 And Z > 0 Then
                    Dim BB As Single = results.Biomass(i) * simSrc.StartBiomass(i)
                    Sum = BB * simSrc.FishTime(i) * simSrc.FishRateGear(j, iTime) * simSrc.FishMGear(j, i) / SumEf
                    pathDest.Landing(j, i) = Sum * pathSrc.Landing(j, i) / Z
                    pathDest.Discard(j, i) = Sum * pathSrc.Discard(j, i) / Z
                Else
                    pathDest.Landing(j, i) = 0
                    pathDest.Discard(j, i) = 0
                End If
            Next j
        Next i

        coreDest.SaveChanges(True, cCore.eBatchChangeLevelFlags.Ecopath)

        Return True

    End Function

End Class
