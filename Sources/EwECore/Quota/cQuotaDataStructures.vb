
Imports EwEUtils.Core

Public Delegate Function CoreCounterDelegate(ByVal SizeType As eCoreCounterTypes) As Integer


Public Class cCoreCounters
    Private m_counterDelegate As CoreCounterDelegate

    Public Sub New(ByVal CounterDelegate As CoreCounterDelegate)
        Me.m_counterDelegate = CounterDelegate
    End Sub

    Public ReadOnly Property nFleets() As Integer
        Get
            Return Me.m_counterDelegate(eCoreCounterTypes.nFleets)
        End Get
    End Property


    Public ReadOnly Property nGroups() As Integer
        Get
            Return Me.m_counterDelegate(eCoreCounterTypes.nGroups)
        End Get
    End Property

End Class


Public Class cQuotaDataStructures

    Private m_counters As cCoreCounters

    Public Sub New(ByVal CounterDelegate As CoreCounterDelegate)
        Me.m_counters = New cCoreCounters(CounterDelegate)
    End Sub

    ' Public DoClosedLoop As Boolean

    Public Bbase() As Single
    Public Blim() As Single
    Public Fopt() As Single
    ' Public KalWt() As Single
    Public FixedEscapement() As Single

    ' Public CVest() As Single

    ''' <summary>Max Fishing Effort for Regulatory Reduction in fishing effort  (by gear)</summary>
    Public MaxEffort() As Single 'gear

    ''' <summary>Type of quota system in effect (by gear) </summary>
    Public QuotaType() As eQuotaTypes 'gear

    ''' <summary>Fishing Quota for regulated fisheries  (by gear group)</summary>
    Public Quota(,) As Single 'gear group

    ''' <summary>Biomass discarded because of regulation  (by gear group)</summary>
    Public RegDiscard(,) As Single ' gear group

    ''' <summary>Proportion of regulated landings (by gear group) for the current time step</summary>
    Public PropLandedTime(,) As Single

    ''' <summary>Proportion of regulated discards (by gear group) for the current time step</summary>
    Public Propdiscardtime(,) As Single

    ''' <summary>Percentage of total catch by at fleet on a group (by fleet, group)</summary>
    ''' <remarks>Sums to one across all fleets for a group</remarks>
    Public Quotashare(,) As Single

    Public QuotaTime(,) As Single

    ''' <summary>
    ''' Init Propdiscardtime(fleet,group) and PropLandedTime(fleet,group) to landing and discard proportions calculated in Ecopath
    ''' </summary>
    ''' <param name="EcopathData">Ecopath data structures to initialize to</param>
    Public Sub InitToEcoPath(ByVal EcopathData As cEcopathDataStructures)
        'Called by Ecosim.Init() at the start of an Ecosim run
        'Propdiscardtime() and PropLandedTime() will not be initialized until Ecosim has been Initialized

        For iflt As Integer = 1 To Me.m_counters.nFleets
            For igrp As Integer = 1 To Me.m_counters.nGroups
                'jb 7-Jan-2010 addded PropDiscardMort() so the default for discards contain only the mort
                Me.Propdiscardtime(iflt, igrp) = EcopathData.PropDiscard(iflt, igrp) * EcopathData.PropDiscardMort(iflt, igrp)
                Me.PropLandedTime(iflt, igrp) = EcopathData.PropLanded(iflt, igrp)
            Next
        Next

    End Sub

    Public Sub RedimVars()
        Dim nFleets As Integer = Me.m_counters.nFleets
        Dim nGroups As Integer = Me.m_counters.nGroups
        'for regulated fisheries
        ReDim QuotaType(nFleets)
        ReDim RegDiscard(nFleets, nGroups)
        ReDim MaxEffort(nFleets)
        ReDim Quota(nFleets, nGroups)
        ReDim PropLandedTime(nFleets, nGroups)
        ReDim Propdiscardtime(nFleets, nGroups)

        ReDim Quotashare(nFleets, nGroups)
        ReDim QuotaTime(nFleets, nGroups)
        ReDim Blim(nGroups)
        ReDim Bbase(nGroups)
        ReDim Fopt(nGroups)
        ReDim FixedEscapement(nGroups)

        For i As Integer = 1 To nGroups
            Blim(i) = cCore.NULL_VALUE
            Bbase(i) = cCore.NULL_VALUE
            Fopt(i) = cCore.NULL_VALUE
            FixedEscapement(i) = 0
        Next

        'Setting regulatory values to NULL will cause them to be set to a default value if the database does not contain values
        'see cEcosimModel.setDefaultValues
        For iflt As Integer = 1 To nFleets
            MaxEffort(iflt) = cCore.NULL_VALUE
            For igrp As Integer = 1 To nGroups
                Quota(iflt, igrp) = cCore.NULL_VALUE
            Next
        Next

    End Sub



    ''' <summary>
    ''' Set default values for regulated fisheries
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub setDefaultRegValues(ByVal EcoSimData As cEcosimDatastructures, ByVal EcoPathData As cEcopathDataStructures)
        Dim igrp As Integer
        Dim iflt As Integer

        'If regulatory values have not been set (by the database) then set them to defaults
        For iflt = 1 To Me.m_counters.nFleets
            If Me.MaxEffort(iflt) = cCore.NULL_VALUE Then Me.MaxEffort(iflt) = 10 '10 times the ecopath base effort
            For igrp = 1 To Me.m_counters.nGroups
                If Me.Quota(iflt, igrp) = cCore.NULL_VALUE Then Me.Quota(iflt, igrp) = EcoSimData.StartBiomass(igrp) * 10 '10 time the ecopath biomass

                'Needs default value????
                If Blim(igrp) = cCore.NULL_VALUE Then Blim(igrp) = EcoSimData.StartBiomass(igrp) * 0.1
                If Bbase(igrp) = cCore.NULL_VALUE Then Bbase(igrp) = EcoSimData.StartBiomass(igrp) * 0.4
                If Fopt(igrp) = cCore.NULL_VALUE Then Fopt(igrp) = EcoSimData.Fish1(igrp)

            Next
        Next

        'set Quota share to Ecopath landings and discards
        Me.setDefaultQuotaShare(EcoPathData)

    End Sub

    ''' <summary>
    ''' Set QuotaShare to default values from Ecopath.Landing and Ecopath.Discards
    ''' </summary>
    ''' <param name="EcoPathData">Ecopath data</param>
    ''' <remarks>QuotaShare(fleet,group) is proportion of catch on a group by a fleet. Should sum to one for a group across fleets.</remarks>
    Public Sub setDefaultQuotaShare(ByVal EcoPathData As cEcopathDataStructures)
        Dim QuotaShareTot As Single
        Dim igrp As Integer
        Dim iflt As Integer

        Try

            If Quotashare Is Nothing Then
                System.Console.WriteLine("Quota data can not set QuotaShare(fleets,groups) because an Ecosim scenario has not been loaded yet!")
                Exit Sub
            End If

            For igrp = 1 To Me.m_counters.nGroups
                QuotaShareTot = 0
                For iflt = 1 To Me.m_counters.nFleets
                    QuotaShareTot += EcoPathData.Landing(iflt, igrp) + EcoPathData.Discard(iflt, igrp)
                Next

                For iflt = 1 To Me.m_counters.nFleets
                    If QuotaShareTot > 0 Then
                        Me.Quotashare(iflt, igrp) = (EcoPathData.Landing(iflt, igrp) + EcoPathData.Discard(iflt, igrp)) / QuotaShareTot
                    Else
                        Me.Quotashare(iflt, igrp) = 0
                    End If
                Next

            Next igrp

        Catch ex As Exception
            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".setDefaultQuotaShare() Exception: " & ex.Message)
        End Try

    End Sub

    Public Sub SumQuotaShareToOne()

        Dim QuotaShareTot As Single
        Dim igrp As Integer
        Dim iflt As Integer

        For igrp = 1 To Me.m_counters.nGroups
            QuotaShareTot = 0
            For iflt = 1 To Me.m_counters.nFleets
                QuotaShareTot += Me.Quotashare(iflt, igrp)
            Next

            If (QuotaShareTot > 0) And (QuotaShareTot <> 1.0!) Then
                For iflt = 1 To Me.m_counters.nFleets
                    Me.Quotashare(iflt, igrp) /= QuotaShareTot
                Next
            End If
        Next igrp

    End Sub

End Class
