' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'







''' <summary>
''' Public variables need by the <see cref="cContaminantTracer">contaminant tracing model</see>. 
''' </summary>
Public Class cContaminantTracerDataStructures

    'EwE5 ReadEcoTracer() for variables that are saved to database
    Public m_nGroups As Integer = 0
    Public m_nTime As Integer = 0
    Public m_nRegions As Integer = 0

    ''' <summary>
    ''' Decay rate per year
    ''' </summary>
    ''' <remarks>
    ''' The zero element holds the Enviroment value 
    ''' 1 to ngroups is for the groups
    ''' </remarks>
    Public cdecay() As Single

    ''' <summary>
    ''' Initial concentration of contaminants in t/km2
    ''' </summary>
    ''' <remarks>
    ''' The zero element holds the Environment value 
    ''' 1 to ngroups is for the groups
    ''' </remarks>
    Public Czero() As Single

    ''' <summary>
    ''' Base inflow rate to environment t/km2/year in Zero element
    ''' </summary>
    Public Cinflow() As Single

    ''' <summary>
    ''' Absorption rate t/t/t/year (concentration in prey/consumption/year ???)
    ''' </summary>
    Public Cenv() As Single

    ''' <summary>
    ''' Concentration in immigrating groups t/t
    ''' </summary>
    Public Cimmig() As Single

    ''' <summary>
    ''' Volume exchange loss rate zero element for environment
    ''' </summary>
    Public CoutFlow() As Single

    ''' <summary>
    ''' Pool excretion rate.
    ''' </summary>
    ''' <remarks>
    ''' <para>C.J.Walters, per email 25Nov2007:</para>
    ''' <para>
    ''' One way to improve ecotracer would be to add a pool-specific instantaneous
    ''' excretion rate, with the excretion flow going back into the 0 (environment)
    ''' pool. Such flows should be very low for lipophilic compounds, close to the
    ''' metabolic rate for compounds that are lost in proportion to routine tissue
    ''' metabolism (eg conversion of carbon 14 organic form to c14-02).
    ''' </para>
    ''' <para>Villy, per email 7Jun2016:</para>
    ''' <para>
    ''' CexcretionRate is renamed in the database to: CassimProp, (so existing 
    ''' models don’t need to be changed).
    ''' </para>
    ''' </remarks>
    Public CassimProp() As Single

    ''' <summary>
    ''' Added cMetabolismRate (unit: per year)
    ''' </summary>
    Public CmetabolismRate() As Single

    Public ConForceNumber As Integer

    ''' <summary>
    ''' Plot contr/biomass
    ''' </summary>
    ''' <remarks></remarks>
    Public TracePlotCB As Boolean

    ''' <summary>Results over time (group x time step)</summary>
    Public TracerConc(,) As Single
    ''' <summary>Concentration over time by region (region x group x time).</summary>
    Public TracerConcByRegion(,,) As Single
    ''' <summary>Concentration over biomass (region x group x time).</summary>
    Public TracerCBRegion(,,) As Single

    Public TracerCB(,) As Single

    ''' <summary>Max concentration of contaminant at the current time step by group</summary>
    Public ConcMax() As Single

    ''' <summary>Ecosim tracer enabled state flag.</summary>
    Friend Property EcoSimConSimOn As Boolean

    ''' <summary>Ecospace tracer enabled state flag.</summary>
    Friend Property EcoSpaceConSimOn As Boolean

    Public MaxTimeSteps As Integer

    Public Sub New()
        Me.MaxTimeSteps = 1000
    End Sub

    Friend Sub RedimByNGroups(nGroups As Integer)
        Me.m_nGroups = nGroups
        ReDim Me.Czero(nGroups)
        ReDim Me.Cimmig(nGroups)
        ReDim Me.Cenv(nGroups)
        ReDim Me.cdecay(nGroups)
        ReDim Me.Cinflow(nGroups)
        ReDim Me.CoutFlow(nGroups)
        ReDim Me.CassimProp(nGroups)
        ReDim Me.CmetabolismRate(nGroups)
    End Sub

    Public Sub redimForEcosimRun(nGroups As Integer, nTime As Integer)
        Me.m_nTime = nTime
        Me.m_nRegions = 0
        ReDim Me.TracerConc(nGroups + 1, nTime)
        ReDim Me.TracerCB(nGroups + 1, nTime)

    End Sub

    Public Sub redimForEcospaceRun(nRegions As Integer, nGroups As Integer, nTime As Integer)
        Me.m_nTime = nTime
        Me.m_nRegions = nRegions
        ReDim Me.TracerConcByRegion(nRegions, nGroups + 1, nTime)
        ReDim Me.TracerCBRegion(nRegions, nGroups + 1, nTime)
    End Sub

    Public Sub Clear()
        Me.m_nGroups = 0

        Me.TracerConc = Nothing '(nGroups + 1, nTime)
        Me.TracerCB = Nothing
        Me.TracerConcByRegion = Nothing '(nRegions, nGroups + 1, nTime)
        Me.TracerCBRegion = Nothing '(nRegions, nGroups + 1, nTime)

        Me.Czero = Nothing '(nGroups)
        Me.Cimmig = Nothing '(nGroups)
        Me.Cenv = Nothing '(nGroups)
        Me.cdecay = Nothing '(nGroups)
        Me.Cinflow = Nothing '(nGroups)
        Me.CoutFlow = Nothing '(nGroups)
        Me.CassimProp = Nothing '(nGroups)

        'Me.RedimByNGroups(0)
    End Sub

    Public Sub CopyTo(ByRef d As cContaminantTracerDataStructures)
        'EwE5 ReadEcoTracer() for variables that are saved to database

        d.cdecay = CType(Me.cdecay.Clone, Single())
        d.Czero = CType(Me.Czero.Clone, Single())
        d.Cinflow = CType(Me.Cinflow.Clone, Single())
        d.Cenv = CType(Me.Cenv.Clone, Single())
        d.Cimmig = CType(Me.Cimmig.Clone, Single())
        d.CoutFlow = CType(Me.CoutFlow.Clone, Single())
        d.CassimProp = CType(Me.CassimProp.Clone, Single())
        d.ConForceNumber = Me.ConForceNumber
        d.EcoSimConSimOn = Me.EcoSimConSimOn
        d.EcoSpaceConSimOn = Me.EcoSpaceConSimOn

        d.CmetabolismRate = Me.CmetabolismRate

    End Sub

End Class



