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
''' <remarks>
''' 30Mar11: NOT IN USE YET!
''' </remarks>
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
    Public Function SaveModel(ByVal strFileName As String, _
                              ByVal strModelName As String, _
                              ByVal results As cEcoSimResults) As eDatasourceAccessType

        Dim coreDest As New cCore()
        Dim db As cEwEDatabase = New cEwEAccessDatabase()
        Dim atResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown
        Dim bSucces As Boolean = False

        coreDest.InitCore()
        coreDest.PluginManager = Nothing

        If String.IsNullOrEmpty(Path.GetExtension(strFileName)) Then
            strFileName &= cDataSourceFactory.GetDefaultExtension(eDataSourceTypes.Access2007)
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

        ' Copy Ecopath data but do not redim - preserve original data such as DBIDs
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

#Region " Original code "

#If 0 Then ' From modSimEdit

Public Sub SaveEcopathFromEcosim()
Dim i As Integer
Dim j As Integer
Dim SaveRunFile As String
Dim SBi() As Double
Dim SBHi() As Double   'habitat biomass
Dim SCatch() As Single
Dim SEx() As Single
Dim SPBi() As Double
Dim SQBi() As Double
Dim SDC() As Single
Dim SEE() As Single
Dim SBAi() As Single
Dim SEmi() As Single
Dim SImmi() As Single
Dim SLandi() As Single
Dim SDisci() As Single
Dim titi As String
Dim Response As Variant
    ReDim SBi(NumGroups) As Double
    ReDim SBHi(NumGroups) As Double   'habitat biomass
    ReDim SCatch(NumGroups) As Single
    ReDim SEx(NumGroups) As Single
    ReDim SPBi(NumGroups) As Double
    ReDim SQBi(NumGroups) As Double
    ReDim SDC(NumGroups + 1, NumGroups + 1) As Single
    ReDim SEE(NumGroups) As Single
    ReDim SBAi(NumGroups) As Single
    ReDim SEmi(NumGroups) As Single
    ReDim SImmi(NumGroups) As Single
    ReDim SLandi(NumGear, NumGroups) As Single
    ReDim SDisci(NumGear, NumGroups) As Single
    Dim t As Variant
    For i = 1 To NumGroups
        SBi(i) = Bi(i)
        Bi(i) = DCPct(i, 1)
        SCatch(i) = Catch(i)
        Catch(i) = Bi(i) * FishTime(i)
        SEx(i) = Ex(i)
        Ex(i) = Catch(i)
        SPBi(i) = PBi(i)
        PBi(i) = loss(i) / Bi(i)
        SQBi(i) = QBi(i)
        QBi(i) = DCPct(i, 2) 'the following has been updated: Eatenby(i) / bb(i)
        SEE(i) = EEi(i)
        EEi(i) = -99
        SBAi(i) = BAi(i)
        BAi(i) = (Bi(i) - DCPct(i, 0)) * StepsPerYear ' / TimeStep 'dcpct() stores the bb() from previous round
        'BAi(i) = DCPct(i, 3) * StepsPerYear '/ TimeStep
        SEmi(i) = Emigrationi(i)
        Emigrationi(i) = Emig(i) * Bi(i) '
        SBHi(i) = BHi(i)
        BHi(i) = Bi(i) / Area(i)
    Next
    For i = 1 To NumGroups
        For j = 1 To NumGroups
            SDC(i, j) = DC(i, j)
            DCi(i, j) = 0        'don't leave any dc leftovers
            If QBi(i) > 0 Then DCi(i, j) = DCMean(i, j) '/ (QBi(i) * Bi(i))
        Next
    Next
    'immigration is constant rate and is not changed by ecosim so no need to change
    For i = 1 To NumGear
        For j = 1 To NumGroups
            SLandi(i, j) = Landing(i, j)
            Landing(i, j) = DCMin(i, j)
            SDisci(i, j) = Discard(i, j)
            Discard(i, j) = DCMax(i, j)
        Next j
    Next i
    titi = modelRemarks
    modelRemarks = "Ecosim output file; " + CStr(Date) + "; " + CStr(time) + "; " + modelRemarks

    GetValidFileName SaveRunFile
    If Mid(dbFilepath, Len(dbFilepath), 1) <> "\" Then
        SaveRunFile = dbFilepath + "\" + SaveRunFile + ".eii" 'Left(lastModel, 8) + ".txt"
    Else
        SaveRunFile = dbFilepath + SaveRunFile + ".eii"  'Left(lastModel, 8) + ".txt"
    End If

    'SaveEiiFile SaveRunFile
    Response = "Ecopath file saved to " + SaveRunFile + vbNewLine + vbNewLine + "You can import the file as a text-file (eii) from the File menu" + vbNewLine + "Do you want to keep this file?"
    Response = MsgBox(Response, vbInformation + vbYesNo, "Save Ecopath model from Ecosim")
    If Response = vbYes Then SaveEiiFile SaveRunFile
    modelRemarks = titi
    Erase DCMin(), DCMean(), DCMax()
    'Restore Ecopath parameters
    For i = 1 To NumGroups
        Bi(i) = SBi(i)
        Catch(i) = SCatch(i)
        Ex(i) = SEx(i)
        PBi(i) = SPBi(i)
        QBi(i) = SQBi(i)
        EEi(i) = SEE(i)
        BAi(i) = SBAi(i)
        Emigrationi(i) = SEmi(i)
        BHi(i) = SBHi(i)
    Next
    For i = 1 To NumGroups
        For j = 1 To NumGroups
            DCi(i, j) = SDC(i, j)
        Next
    Next
    For i = 1 To NumGear
        For j = 1 To NumGroups
            Landing(i, j) = SLandi(i, j)
            Discard(i, j) = SDisci(i, j)
        Next j
    Next i
End Sub
#End If
#End Region ' Original code
