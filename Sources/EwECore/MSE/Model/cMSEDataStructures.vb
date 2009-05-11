'==============================================================================
'
' $Log: cMSEDataStructures.vb,v $
' Revision 1.2  2009/05/11 21:28:09  joeb
' Adding MSE data to Decision Support Tool (Multi Player Game)
'
' Revision 1.1  2008/09/26 07:30:27  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.9  2008/05/12 18:59:52  joeb
' Restructure of search objects to use ISearchObjective interface
'
' Revision 1.8  2008/05/05 16:21:46  joeb
' CurrentIteration is now Integer instead of Boolen????
'
' Revision 1.7  2008/05/01 20:35:15  joeb
' Moved summary varaibles from MSE to here
'
' Revision 1.6  2008/04/28 18:00:01  joeb
' Initialization
'
' Revision 1.5  2008/04/24 14:51:18  joeb
' Added mean results varaibles
'


Public Class cMSEDataStructures

    Public NTrials As Integer

    ''' <summary>
    ''' Importance weight of a fleet on a group
    ''' </summary>
    ''' <remarks> fleets, groups</remarks>
    Public Fweight(,) As Single 'Fishing weight set by user. Weight/importance of a fleet

    ''' <summary>
    ''' Weighted relative catchablility for closed loop FWeight * RelQ
    ''' Use to update fishing effort during an Ecosim run 
    ''' </summary>
    ''' <remarks>
    ''' Fwc(ifleet, 0) initialized to Ecopath base value in Me.InitForRun
    ''' Fwc(iFleet, 1) = Updated for each year in MSE.AccessFs
    ''' FishingEffort(iFleet) = FishingEffort(iFleet) * m_data.Fwc(iFleet, 0) / m_data.Fwc(iFleet, 1)
    ''' </remarks>
    Public Fwc(,) As Single
    Public Wftot() As Single 'sum of fishing weight for all species caught by a fleet  Wftot(iflt) = Wftot(iflt) + Fweight(iflt, igrp)

    Public Qgrow() As Single 'Max catchability increase. Catchability increase over time due to improved fishing efficiency
    Public BioRiskValue(,) As Single 'Lower and Upper boundry for Biomass risk
    Public CVbiomEst() As Single 'Biomass coefficient of variation
    Public CVFest() As Single 'Fishing effort coefficient of variation

    Public VarQest() As Single 'Estimated variation in the estimation of fishing effort. Use in the first year of the simulation to vary effort. See Init
    Public KalGainQ() As Single
    Public VarQyear() As Single
    Public VarQgrow() As Single 'variation in catchability

    ''' <summary>
    ''' T/F flag tells the MSE if a trial has exceeded the lower biomasss risk boundry
    ''' </summary>
    Public BioR0() As Boolean
    ''' <summary>
    ''' T/F flag tells the MSE if a trial has exceeded the upper biomasss risk boundry
    ''' </summary>
    Public BioR1() As Boolean
    ''' <summary>
    ''' Number of trials biomass was outside the lower or upper risk boundry
    ''' </summary>
    Public BioRiskCount(,) As Integer

    Public AssessPower As Single
    Public GstockPred() As Single
    Public RstockPred() As Single
    Public KalmanGain() As Single
    Public QGrowUsed() As Single

    Public MeanEmploy As Single
    Public MeanVal As Single
    Public MeanManVal As Single
    Public MeanEcoVal As Single
    Public MeanTotalValue As Single
    Public BestTotalValue As Single

    Public BaseEmployVal As Single
    Public BaseTotalVal As Single
    Public BaseManValue As Single
    Public BaseEcoVal As Single

    ''' <summary>
    ''' Use for Closed Loop Fishing Rate Assesment method
    ''' </summary>
    ''' <remarks></remarks>
    Public AssessMethod As eAssessmentMethods

    Private m_curIter As Integer

    Private m_core As cCore

    Public Property CurrentIteration() As Integer
        Get
            Return m_curIter
        End Get
        Friend Set(ByVal value As Integer)
            m_curIter = value
        End Set
    End Property

    ''' <summary>
    ''' Set default values for the Management Strategy Evaluation model cMSE
    ''' </summary>
    Public Sub Init(ByRef theCore As cCore)
        Dim i As Integer, j As Integer

        Try

            m_core = theCore

            Me.Dimension()

            'default assessment method
            ' Fs from biomass estimates by pool
            AssessMethod = 1

            AssessPower = 1

            For i = 1 To m_core.nLivingGroups
                GstockPred(i) = 0.6
                KalmanGain(i) = 0.65
                BioRiskValue(i, 0) = 0.5 'lower
                BioRiskValue(i, 1) = 2 'upper
            Next

            'set default values
            For i = 1 To m_core.nLivingGroups
                CVbiomEst(i) = 0.2
                CVFest(i) = 0.3
                For j = 1 To m_core.nFleets
                    If m_core.m_EcoSimData.relQ(j, i) > 0 Then
                        Fweight(j, i) = 1
                    End If
                Next
            Next

            For iFlt As Integer = 1 To m_core.nFleets
                Qgrow(iFlt) = 0.1
            Next iFlt

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("Init() " & ex.Message, ex)
        End Try

    End Sub

    Private Sub Dimension()

        ReDim GstockPred(m_core.nLivingGroups)
        ReDim RstockPred(m_core.nLivingGroups)
        ReDim KalmanGain(m_core.nLivingGroups)
        ReDim VarQest(m_core.nFleets), KalGainQ(m_core.nFleets), VarQyear(m_core.nFleets)
        ReDim VarQgrow(m_core.nFleets)
        ReDim Wftot(m_core.nFleets)

        ReDim Fweight(m_core.nFleets, m_core.nLivingGroups)
        ReDim Qgrow(m_core.nFleets)
        ReDim Fwc(m_core.nFleets, 1)

        ReDim BioR0(m_core.nLivingGroups)
        ReDim BioR1(m_core.nLivingGroups)
        ReDim BioRiskValue(m_core.nLivingGroups, 1)
        ReDim BioRiskCount(m_core.nLivingGroups, 1)

        ReDim CVbiomEst(m_core.nLivingGroups)
        ReDim CVFest(m_core.nLivingGroups)

        ReDim QGrowUsed(m_core.nFleets)

    End Sub

    Public Sub New()
        NTrials = 20 'default number of trials
    End Sub

    ''' <summary>
    ''' Set variable to default values for a trial
    ''' </summary>
    ''' <remarks>Sets Wftot(), Fwc(), VarQyear(), VarQgrow(), VarQest(), KalGainQ() </remarks>
    Friend Sub InitForTrial()
        Dim iFlt As Integer, iGrp As Integer

        Array.Clear(BioR0, 0, BioR0.Length)
        Array.Clear(BioR1, 0, BioR1.Length)
        Array.Clear(Wftot, 0, Wftot.Length)
        Array.Clear(Fwc, 0, Fwc.Length)

        For iFlt = 1 To m_core.nFleets
            For iGrp = 1 To m_core.nLivingGroups
                Wftot(iFlt) = Wftot(iFlt) + Fweight(iFlt, iGrp)
                Fwc(iFlt, 0) = Fwc(iFlt, 0) + Fweight(iFlt, iGrp) * m_core.m_EcoSimData.relQ(iFlt, iGrp)
            Next
            If Wftot(iFlt) > 0 Then Fwc(iFlt, 0) = Fwc(iFlt, 0) / Wftot(iFlt)
            Fwc(iFlt, 1) = Fwc(iFlt, 0)
        Next iFlt

        For iFlt = 1 To m_core.nFleets

            If AssessMethod = 1 Then
                VarQyear(iFlt) = CSng((Fwc(iFlt, 0) * CVbiomEst(iFlt)) ^ 2.0F)
            Else
                VarQyear(iFlt) = CSng((Fwc(iFlt, 0) * CVFest(iFlt)) ^ 2)
            End If
            VarQgrow(iFlt) = CSng((1 / 3 - 1 / 4) * Qgrow(iFlt) ^ 2) ' var of uniform 0-qgrow
            If VarQgrow(iFlt) = 0 Then VarQgrow(iFlt) = 0.0001
            VarQest(iFlt) = VarQgrow(iFlt) * CSng((1 + Math.Sqrt(1 + 4 * VarQyear(iFlt) / VarQgrow(iFlt))) / 2)
            KalGainQ(iFlt) = VarQest(iFlt) / (VarQest(iFlt) + VarQyear(iFlt))
        Next iFlt

    End Sub


End Class
