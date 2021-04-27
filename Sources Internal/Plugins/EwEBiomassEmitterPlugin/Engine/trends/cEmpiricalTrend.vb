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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwECore

Public Class cRuleTrend
    Inherits cTrend

    Public Sub New(data As cData, mpa As cEcospaceMPA)
        MyBase.New(data)
        Me.MPA = mpa
    End Sub

    Private ReadOnly Property SizeReduction(clustersize As Integer) As Single
        Get
            '' Effect 1 at 4, 0 at 16
            'Return Math.Max(0, Math.Min(1, CSng(1.333 - MPA.NumCells / 12)))
            ' Effect 1 at 4, 0 at 9
            Return Math.Max(0, Math.Min(1, CSng(9 / 5 - clustersize / 5)))
        End Get
    End Property

    Private ReadOnly Property CellCoverage As Single
        Get
            'Dim cellarea As Single = CSng((10 * Me.Data.Core.EcospaceBasemap.CellLength) ^ 2) * Me.NumCells
            'Return Math.Max(0, Math.Min(1, Me.MPASize / cellarea))
            Return 1
        End Get
    End Property

    Public Function Multiplier(t As Date, clustersize As Integer) As Single

#If 0 Then
        Dim dy As Integer = t.Year - md.YearEstablished

        ' MPA not active yet?
        If (dy < 1) Then Return 1

        ' Y=ln[(mean biomass in moderately protected area)/(mean biomass in surrounding control unprotected area)]
        ' Best option: enter mean biomass at MPA start year in unprotected area. Rel B within MPA is then calculated
        Dim y As Single = 0

        Select Case md.Protection
            Case eProtectionType.Full
                y = CSng(1.19 + 0.08 * dy + 0.3 * Math.Log10(md.Size))
            Case eProtectionType.High
                y = CSng(1.1 + 0.04 * dy + 0.65 * Math.Log10(md.Size))
            Case eProtectionType.Moderate
                y = CSng(0.47 + 0.06 * dy + 0.85 * Math.Log10(md.Size))
            Case eProtectionType.Poor
                ' No effect
        End Select
        ' Inverse of LN
        Dim mult As Single = CSng(Math.Pow(Math.E, y))
        Console.WriteLine("{0},{1} = {2}", Me.Target, Me.Group, mult)
        Return mult
#End If

        ' Determine which fished groups this MPA is preventing from being fished at ths time step. Multiply only those

        ' In Skype discussions with Joachim Claudette we have been yet unable to fnid a working approach that
        ' crosses the divide between the Zupan et al approach and EwE.
        ' - Zupan's formulas account for all MPA effects since establishment: size, age, lack of fishing, etc
        ' - Ecospace implicitly accounts for age, and fishing mortality is already controlled through MPA enforcement
        ' As such, Zupan-derived formulas grossly overestimate biomasses in Ecospace. We need to mitigate this effect

        ' As discussed with Marta Coll, it seems more realistic to have biomass increases in the order of 5% when
        ' in a MPA when the MPA is too small for Ecospace to account for its workings.

        ' How this code should work:
        ' - The MPA size (in HA) must be translated to the amount of area covered by an MPA, per cell, somehow
        ' - As a solution, the MPA size (converted to km2) is divided by the average cell size (in km2) in Ecospace (latitude tapering is igored).
        ' - Cell area coverage determines the magnitue of biomass boost that the cell will receive due to the presence of the effective sub-cell MPA
        ' - The MPA cell area coverage is capped at 1
        ' - The total effect is applied up to 4 cells. From 4 to 16 cells there is a linear decrease to 0. At 16 cells, Ecospace is fully capable of emulating the MPA

        Return 1 + (Me.CellCoverage * Me.SizeReduction(clustersize) * Me.Data.RuleMaxEffect(Me.Protection))

    End Function

    Public ReadOnly Property MPA As cEcospaceMPA

    ''' <summary>
    ''' Get the <see cref="cEcospaceMPA.DBID">MPA database ID</see>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MPAID As Integer
        Get
            Return Me.MPA.DBID
        End Get
    End Property

    ''' <summary>
    ''' Get/set the year that a MPA was established.
    ''' </summary>
    ''' <returns></returns>
    <Obsolete("Not used yet")>
    Public Property YearEstablished As Integer = 2000

    ''' <summary>
    ''' Get/set the <see cref="eProtectionType">type of protection</see> of a MPA
    ''' </summary>
    ''' <returns></returns>
    Public Property Protection As eProtectionType = eProtectionType.Moderate

    Public Overrides Property CanRun As Boolean = False

    Public ReadOnly Property Index As Integer
        Get
            Return Me.MPA.Index
        End Get
    End Property

    Public ReadOnly Property Name As String
        Get
            Return Me.MPA.Name
        End Get
    End Property
End Class