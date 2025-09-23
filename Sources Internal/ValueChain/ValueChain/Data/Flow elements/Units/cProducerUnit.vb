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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwECore
Imports ValueChain.Utilities
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style
Imports System.Text
Imports System.Runtime.CompilerServices

#End Region ' Imports


''' <summary>
''' 
''' </summary>
<TypeConverter(GetType(cPropertySorter)),
    DefaultProperty("Name"),
    Serializable()>
Public Class cProducerUnit
    Inherits cEconomicUnit

    Private Class cLandingsInput
        Public Sub Clear()
            Me.Landings = 0
            Me.Value = 0
        End Sub
        Property Landings As Single
        Property Value As Single
    End Class

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Private vars "

    Private m_fleet As String = ""
    Private m_records As New Dictionary(Of String, cLandingsInput)
    Private m_sEffort As Single = 1

    Private m_sObserverCost As Single = 0.0!
    Private m_sObserverRate As Single = 1.0!
    Private m_sOriginalOutputBiomass As Single = 0.0!

    Private m_sTicketProducts As Single = 0


#End Region ' Public vars

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the unit for a new run.
    ''' </summary>
    ''' <param name="iSequence"></param>
    ''' -----------------------------------------------------------------------
    Friend Overrides Sub InitRun(iSequence As Integer)
        MyBase.InitRun(iSequence)
        ' Reset local vars for the next run
        Me.m_sOriginalOutputBiomass = 0.0!
        m_records.Clear()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the unit for a new time step.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Overrides Sub Clear()
        MyBase.Clear()
        ' Clear totals prior to a run!
        m_records.Clear()
    End Sub

    Public Function HasTarget(unit As cUnit, species As String) As Boolean

        ' Follow each output link
        For iLink As Integer = 0 To Me.LinkOutCount - 1
            Dim link As cLink = Me.LinkOut(iLink)
            If TypeOf link Is cLinkLandings Then
                Dim linkSpec As cLinkLandings = DirectCast(link, cLinkLandings)
                If ReferenceEquals(linkSpec.Target, unit) And (String.Compare(linkSpec.Species, species, StringComparison.OrdinalIgnoreCase) = 0) Then Return True
            Else
                ' See the target link is the requesting unit
                If ReferenceEquals(link.Target, unit) Then Return True
            End If
        Next iLink
        Return False

    End Function

#End Region ' Overrides

#Region " Calculations "

    Protected Overrides Function Calculate(results As cResults, sInputBiomass As Single, sInputValue As Single, sOutputBiomass As Single, sOutputValue As Single, iTimeStep As Integer) As Boolean

        Dim bSucces As Boolean

        'VC090310: Producer cost needs to reflect ecosim effort. 
        'We need to calculate the base cost from the standard calculations
        'below, but then change the effort-related cost based on Ecosim effort.

        ' First time step?
        ' VC090808: problem with this is that the user may have changed effort even in the first time step.
        ' this will mess up calculations, but can't find an easy way to calculate ecopath baseline???????? 
        '
        ' VC:  because of the problem above, I force the effort to be 1 at timestep 1.

        '' JS110325: Added sanity check
        'If (results.RunType = eRunTypes.Snapshot) Then
        '    Debug.Assert(iTimeStep = 1, "Snapshot should use time step 1 only")
        'End If

        ' JS250916: effort needs to be spoon-fed; cannot be automatically obtained anymore

        If (iTimeStep = 1) Then
            ' #Yes: store base biomass
            Me.m_sOriginalOutputBiomass = sOutputBiomass
            ' Do not use effort this time step
            Me.m_sEffort = 1
        End If

        'The production unit needs to do the same calculations as the MyBase=cEconomicUnit, but:
        bSucces = MyBase.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

        ' Calc AddsObserver costs
        bSucces = bSucces And Me.CalcObserverCost(results, sOutputBiomass, iTimeStep)

        'VC090310: Categories of costs and how they are handled:
        'Commercial fisheries
        'Related to tonnes: Pay/Share, all taxes, revenue (apart from subsidies), certification cost
        'Related to effort: Energy, Industrial, services, capital, observers, management, license, subsidies

        'Recreational fisheries
        'Effort: related to biomass of target species (sigmoid relationship)
        'Income: related to effort (for guide operations); 0 if private boats
        'Cost: modeled same way as for commercial fisheries

        'Eco tours
        'Effort: related to biomass of target species (sigmoid relationship)
        'Income: related to effort; using ticket revenue: m_sTicketProducts 
        'Cost: modeled same way as for commercial fisheries 

        Return bSucces

    End Function

    Protected Overrides Function CalcProducts(results As cResults, sInputBiomass As Single, sInputValue As Single, sOutputBiomass As Single, sOutputValue As Single, iTimeStep As Integer) As Boolean

        'Now add to this the revenue from paying customers
        Dim sSum As Single = Me.m_sEffort * Me.m_sTicketProducts
        results.Store(Me, cResults.eVariableType.RevenueTickets, sSum, iTimeStep)

        ' Use standard calculations, which is desirable so we do not have to keep 
        ' updating formulas in different places in case standard calculations were 
        ' to change       '
        'Last part is the usual biomass related part:
        'Dim sSum As Single = sOutputBiomass * (Me.EnergyProducts + Me.IndustrialProducts + Me.ServiceProducts)
        Return MyBase.CalcProducts(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

    End Function

    Protected Overrides Function CalcRawmaterialCost(results As cResults, sInputBiomass As Single, sInputValue As Single, sOutputBiomass As Single, sOutputValue As Single, iTimeStep As Integer) As Boolean

        Return results.Store(Me, cResults.eVariableType.CostRawmaterial, 0, iTimeStep)

    End Function

    Protected Overrides Function CalcInputCost(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        ' Need to include effort in our calculations
        If (Me.m_sEffort <> 1) Then
            ' #Yes: do NOT use sOutputBiomass, but instead use base biomass x effort
            Dim sSum As Single = Me.m_sOriginalOutputBiomass * Me.m_sEffort *
                                 (Me.CapitalInput + Me.EnergyCost + Me.IndustrialCost + Me.ServiceCost)
            Return results.Store(Me, cResults.eVariableType.CostInput, sSum, iTimeStep)
        Else
            Return MyBase.CalcInputCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        End If

    End Function

    Protected Overrides Function CalcManagementRoyaltyCertificationCost(results As cResults,
               sInputBiomass As Single, sInputValue As Single,
               sOutputBiomass As Single, sOutputValue As Single,
               iTimeStep As Integer) As Boolean

        'the costs for management and royalties are proportional to effort
        If (Me.m_sEffort <> 1) Then
            Dim sSum As Single = Me.m_sEffort * Me.m_sOriginalOutputBiomass * (Me.ManagementCost + Me.RoyaltyCost)

            'the cost for certification is assumed proportional to landings, so add this
            sSum += sOutputBiomass * Me.CertificationCost

            Return results.Store(Me, cResults.eVariableType.CostManagementRoyaltyCertification, sSum, iTimeStep)
        Else  'just like other calculations:
            Return MyBase.CalcManagementRoyaltyCertificationCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        End If

    End Function

    Protected Overrides Function CalcSubsidy(results As cResults,
               sInputBiomass As Single, sInputValue As Single,
               sOutputBiomass As Single, sOutputValue As Single,
               iTimeStep As Integer) As Boolean

        If (Me.m_sEffort <> 1) Then
            Dim sSum As Single = Me.m_sEffort * Me.m_sOriginalOutputBiomass * (Me.SubsidyEnergy + Me.SubsidyOther)
            results.Store(Me, cResults.eVariableType.RevenueSubsidies, sSum, iTimeStep)
            Return True
        Else
            Return MyBase.CalcSubsidy(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        End If
    End Function

    Protected Overridable Function CalcObserverCost(results As cResults, sOutputBiomass As Single,
                iTimeStep As Integer) As Boolean

        Dim sObsCost As Single = 0
        If (Me.m_sEffort <> 1) Then
            sObsCost = Me.m_sOriginalOutputBiomass * Me.m_sEffort * (Me.ObserverCost * Me.ObserverRate)
        Else
            sObsCost = sOutputBiomass * (Me.ObserverCost * Me.ObserverRate)
        End If
        Return results.Store(Me, cResults.eVariableType.CostObserver, sObsCost, iTimeStep)

    End Function

    ''' <summary>
    ''' The number of jobs for producers is a function of effort, while their salary isn't
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="sInputBiomass"></param>
    ''' <param name="sInputValue"></param>
    ''' <param name="sOutputBiomass"></param>
    ''' <param name="sOutputValue"></param>
    ''' <param name="iTimeStep"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function CalcWorkerFemales(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean
        If (Me.m_sEffort <> 1) Then
            Dim sSum As Single = Me.m_sEffort * Me.m_sOriginalOutputBiomass * Me.WorkerFemale
            Return results.Store(Me, cResults.eVariableType.NumberOfWorkerFemales, sSum, iTimeStep)
        Else
            Return MyBase.CalcWorkerFemales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        End If

    End Function

    Protected Overrides Function CalcWorkerMales(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        If (Me.m_sEffort <> 1) Then
            Dim sSum As Single = Me.m_sEffort * Me.m_sOriginalOutputBiomass * Me.WorkerMale
            Return results.Store(Me, cResults.eVariableType.NumberOfWorkerMales, sSum, iTimeStep)
        Else
            Return MyBase.CalcWorkerMales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        End If

    End Function

    Protected Overrides Function CalcOwnerMales(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        If (Me.m_sEffort <> 1) Then
            Dim sSum As Single = Me.m_sEffort * Me.m_sOriginalOutputBiomass * Me.OwnerMale
            Return results.Store(Me, cResults.eVariableType.NumberOfOwnerMales, sSum, iTimeStep)
        Else
            Return MyBase.CalcOwnerMales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        End If

    End Function

    Protected Overrides Function CalcOwnerFemales(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        If (Me.m_sEffort <> 1) Then
            Dim sSum As Single = Me.m_sEffort * Me.m_sOriginalOutputBiomass * Me.OwnerFemale
            Return results.Store(Me, cResults.eVariableType.NumberOfOwnerFemales, sSum, iTimeStep)
        Else
            Return MyBase.CalcOwnerFemales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        End If

    End Function

#End Region ' Calculations

#Region " Overrides "

    <Browsable(False)>
    Public Overrides ReadOnly Property HasError() As Boolean
        Get
            Return (Me.m_fleet Is Nothing) Or (Not String.IsNullOrWhiteSpace(Me.UnlikelyOutputs))
        End Get
    End Property

    '<Browsable(False)>
    'Public Overrides ReadOnly Property Style() As cStyleGuide.eStyleFlags
    '    Get
    '        Dim st As cStyleGuide.eStyleFlags = MyBase.Style
    '        If (Me.m_fleet IsNot Nothing) Then st = st Or cStyleGuide.eStyleFlags.ValueComputed
    '        If (Me.HasError) Then st = st Or cStyleGuide.eStyleFlags.ErrorEncountered
    '        Return st
    '    End Get
    'End Property

#End Region ' Overrides

#Region " Alternate name "

    Private Function GenerateName() As String
        If (String.IsNullOrWhiteSpace(Me.m_fleet)) Then Return "! No fleet"
        Return Me.m_fleet
    End Function

    Public Overrides Property Name() As String
        Get
            Dim strName As String = MyBase.Name
            If String.IsNullOrEmpty(strName) Then
                strName = Me.GenerateName()
            End If
            Return strName
        End Get
        Set(value As String)
            ' Setting generated name?
            If (String.Compare(value, Me.GenerateName()) = 0) Then
                ' #Yes: Clear the base name
                MyBase.Name = ""
            Else
                ' #No: Set the base name
                MyBase.Name = value
            End If
        End Set
    End Property

#End Region ' Alternate name

#Region " Properties "

    Public Overrides ReadOnly Property BiomassRatio As String
        Get
            ' Count # of active links
            Dim iNumActiveLinks As Integer = 0
            For i As Integer = 0 To Me.LinkOutCount - 1
                If Me.LinkOut(i).BiomassRatio > 0 Then
                    iNumActiveLinks += 1
                End If
            Next
            Return MyBase.BiomassRatio & " / " & iNumActiveLinks.ToString()
        End Get
    End Property

    <Browsable(True),
        Category(sPROPCAT_VALIDATION),
        DisplayName("Unlikely outputs"),
        Description("Names of groups that are landed and transferred through the chain with an unlikely biomass ratios that exceed 1"),
        cPropertySorter.PropertyOrder(7)>
    Public ReadOnly Property UnlikelyOutputs As String
        Get

            Dim totals As New Dictionary(Of String, Single)
            Dim sbError As New StringBuilder()

            For i As Integer = 0 To Me.LinkOutCount - 1
                Dim ll As cLinkLandings = DirectCast(Me.LinkOut(i), cLinkLandings)
                If (Not String.IsNullOrWhiteSpace(ll.Species)) Then
                    Dim stotal As Single = 0
                    totals.TryGetValue(ll.Species, stotal)
                    totals(ll.Species) = stotal + ll.BiomassRatio
                End If
            Next

            For Each spp In totals.Keys
                If totals(spp) > 1.0! Then
                    If (sbError.Length > 0) Then
                        sbError.Append(",")
                    End If
                    sbError.Append(spp & ": " & totals(spp).ToString("R"))
                End If
            Next
            Return sbError.ToString
        End Get
    End Property

    <Browsable(True),
    Category(sPROPCAT_INPUTCOST),
    DisplayName("Monitoring cost"),
    Description("Cost for monitors (if on board) per tonnes. Assumed to vary with effort"),
    DefaultValue(0.0!),
    cPropertySorter.PropertyOrder(20)>
    Public Property ObserverCost() As Single
        Get
            Return Me.m_sObserverCost
        End Get
        Set(value As Single)
            Me.m_sObserverCost = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
     Category(sPROPCAT_INPUTCOST),
     DisplayName("Monitor coverage rate"),
     Description("Monitor coverage rate, (proportion of boats with observers onboard)"),
     DefaultValue(0.0!),
     cPropertySorter.PropertyOrder(21)>
    Public Property ObserverRate() As Single
        Get
            Return Me.m_sObserverRate
        End Get
        Set(value As Single)
            Me.m_sObserverRate = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
     Category(sPROPCAT_REVENUE),
     DisplayName("Ticket revenue"),
     Description("Revenue from paying customers at Ecopath baseline effort (unity effort). Revenue assumed proportional to effort."),
     DefaultValue(0.0!),
     cPropertySorter.PropertyOrder(1)>
    Public Property TicketProducts() As Single
        Get
            Return Me.m_sTicketProducts
        End Get
        Set(value As Single)
            Me.m_sTicketProducts = value
            Me.SetChanged()
        End Set
    End Property

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "Producer"
        End Get
    End Property

    <Browsable(False)>
    Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
        Get
            Return cUnitFactory.eUnitType.Producer
        End Get
    End Property

    <Browsable(False)>
    Public Overrides ReadOnly Property CanCompute() As Boolean
        Get
            Return True
        End Get
    End Property

#Region " Ecopath integration "

    <Browsable(False)>
    Public Overridable Property Fleet() As String
        Get
            Return Me.m_fleet
        End Get
        Set(value As String)
            Me.m_fleet = value
        End Set
    End Property

#End Region ' Ecopath integration

#End Region ' Properties

#Region " Landings "

    Public Sub SetEffort(sEffort As Single)
        Me.m_sEffort = sEffort
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="species"></param>
    ''' <param name="sBiomass">Total biomass landed in area</param>
    ''' <param name="sValue">Total value landed in area</param>
    Public Sub SetLandings(species As String, sBiomass As Single, sValue As Single)

        If (String.IsNullOrWhiteSpace(species)) Then Return

        Dim record As cLandingsInput = Nothing
        If Not Me.m_records.TryGetValue(species, record) Then record = New cLandingsInput()
        record.Landings = sBiomass
        record.Value = sValue
        Me.m_records(species) = record

    End Sub

    Public Overloads Sub Process(results As cResults, iTimeStep As Integer, iItem As Integer)

        Dim sTotalOutputBiomass As Single = 0
        Dim sTotalOutputValue As Single = 0

        Dim sBTot As Single = 0
        Dim sValTot As Single = 0
        For Each r As cLandingsInput In Me.m_records.Values
            sBTot += r.Landings
            sValTot += r.Value
        Next

        ' No item specified?
        If iItem = 0 Then
            ' #Yes: perform all calculations
            Me.Calculate(results, sBTot, 0, sBTot, sValTot, iTimeStep)
        End If

        ' Determine outgoing biomass ratios for each species
        Dim totalSppB As New Dictionary(Of String, Single)
        For Each link As cLink In Me.m_llinkOutput
            ' Sanity check
            If (TypeOf link Is cLinkLandings) Then
                Dim ll As cLinkLandings = DirectCast(link, cLinkLandings)
                If (Not String.IsNullOrWhiteSpace(ll.Species)) And (ll.IsVisible) Then
                    Dim s As Single = 0
                    totalSppB.TryGetValue(ll.Species, s)
                    s += ll.BiomassRatio
                    totalSppB(ll.Species) = s
                End If
            End If
        Next

        ' Determine outgoing biomass
        For Each link As cLink In Me.m_llinkOutput

            Dim sBiomass As Single = 0.0
            Dim sValue As Single = 0.0
            'the above was called sPrice, but it is value, so renamed

            Debug.Assert(TypeOf link Is cLinkLandings)

            Dim ll As cLinkLandings = DirectCast(link, cLinkLandings)
            If (Not String.IsNullOrWhiteSpace(ll.Species)) And (ll.IsVisible) Then
                Dim s As Single = 0
                totalSppB.TryGetValue(ll.Species, s)
                If (s > 0) Then
                    Dim r As cLandingsInput = Me.m_records(ll.Species)
                    sBiomass += r.Landings * ll.BiomassRatio / totalSppB(ll.Species)

                    If (ll.ValueRatio = 1.0!) Then
                        sValue += r.Value * ll.BiomassRatio / totalSppB(ll.Species)
                    Else
                        sValue += ll.ValueRatio * r.Landings * ll.BiomassRatio / totalSppB(ll.Species)
                    End If

                End If
            End If

            ' Process every link to ensure that target units receive all inputs!
            If (sBiomass > 0) Then
                'VC: I changed the process line to pass sPrice/sBiomass as the third parameter (instead of sPrice). 
                'it is supposed to be the price per unit biomass
                'it was multiplying an extra time with the total catches (sBiomass) as it was.
                link.Target.Process(results, New cInput(Me, sBiomass, sValue), iTimeStep, iItem)
            Else
                ' Process link to make the chain work, even though no data travels over this link!
                link.Target.Process(results, New cInput(Me, sBiomass, sValue), iTimeStep, iItem)
            End If

            sTotalOutputBiomass += sBiomass
            sTotalOutputValue += sValue '* sBiomass

        Next

        results.StoreContribution(iItem, Me, iTimeStep, sValTot, sBTot)

    End Sub

#End Region ' Landings

    Public Overrides ReadOnly Property IsDefault As Boolean
        Get
            Return False
        End Get
    End Property

End Class
