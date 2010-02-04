
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

    Public Quotashare(,) As Single

    Public QuotaTime(,) As Single

    ''' <summary>
    ''' Init Propdiscardtime(fleet,group) and PropLandedTime(fleet,group) to landing and discard proportions calculated in Ecopath
    ''' </summary>
    ''' <param name="EcopathData"></param>
    ''' <remarks></remarks>
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
    Public Sub setDefaultRegValues(ByVal EcoSimData As cEcosimDatastructures, ByVal EcoSim As cEcopathDataStructures)

        'If regulatory values have not been set (by the database) then set them to defaults
        For iflt As Integer = 1 To Me.m_counters.nFleets
            If Me.MaxEffort(iflt) = cCore.NULL_VALUE Then Me.MaxEffort(iflt) = 10 '10 times the ecopath base effort
            For igrp As Integer = 1 To Me.m_counters.nGroups
                If Me.Quota(iflt, igrp) = cCore.NULL_VALUE Then Me.Quota(iflt, igrp) = EcoSimData.StartBiomass(igrp) * 10 '10 time the ecopath biomass

                'Needs default value????
                If Blim(igrp) = cCore.NULL_VALUE Then Blim(igrp) = EcoSimData.StartBiomass(igrp) * 0.1
                If Bbase(igrp) = cCore.NULL_VALUE Then Bbase(igrp) = EcoSimData.StartBiomass(igrp) * 0.4
                If Fopt(igrp) = cCore.NULL_VALUE Then Fopt(igrp) = EcoSimData.Fish1(igrp)

            Next
        Next

    End Sub

End Class
