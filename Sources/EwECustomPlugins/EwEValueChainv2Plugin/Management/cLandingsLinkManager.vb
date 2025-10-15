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
Option Explicit On
Imports EwECore
Imports EwEUtils.Core
Imports ValueChain

#End Region ' Imports

Public Class cLandingsLinkManager

    Private m_model As cValueChainController = Nothing
    Private m_data As cValueChainData = Nothing
    Private m_core As cCore = Nothing

    Public Sub New(model As cValueChainController, data As cValueChainData, core As cCore)
        Me.m_model = model
        Me.m_data = data
        Me.m_core = core
    End Sub

    Public Sub OnEcopathMessage(msg As cMessage)

        If (msg.Source <> EwEUtils.Core.eCoreComponentType.Ecopath) Then Return
        If (msg.DataType <> EwEUtils.Core.eDataTypes.FleetInput) Then Return
        If (Not msg.HasVariable(eVarNameFlags.Landings)) Then Return

        Me.ManageLinkLandings()

    End Sub

    ''' <summary>
    ''' Remove discontinued landings links and create new defaults where needed.
    ''' </summary>
    Public Sub ManageLinkLandings()

        ' Iterate over all links and check:
        ' - Do the source and target exist? If not, flag the link for deletion (should not have orphans, but ok)
        ' - Does the source producer still catch the selected species? If no, flag the link for deletion


        Dim links As cLink() = Nothing
        Dim link As cLinkLandings = Nothing
        Dim fleet As cEcopathFleetInput = Nothing
        Dim group As cEcoPathGroupInput = Nothing
        Dim landings As List(Of String) = Nothing
        Dim bDummy As Boolean

        Dim dtTarget As New Dictionary(Of cUnit, List(Of String))

        ' Delete all invisible links
        links = Me.m_data.GetLinks(GetType(cLinkLandings), True)
        For Each link In links
            If (Not link.IsVisible) Then
                ' Delete link
                Console.WriteLine("> VC: Link {0} no longer has landings, delete", link)
                Me.m_data.RemoveLink(link)
            End If
        Next link

        ' Add for missing links to producers
        For Each prod As cProducerUnit In Me.m_data.GetUnits(cUnitFactory.eUnitType.Producer)

            ' Get fleet
            fleet = Me.m_model.FindFleet(prod.GearCode)

            ' Count all existing links by target
            For iLink As Integer = 0 To prod.LinkOutCount - 1
                ' Get link
                link = DirectCast(prod.LinkOut(iLink), cLinkLandings)
                ' Only handle relevant links
                If Not dtTarget.ContainsKey(link.Target) Then
                    dtTarget(link.Target) = New List(Of String)
                End If
                dtTarget(link.Target).Add(link.SpeciesCode)
            Next

            ' Check if has all landings exist for targets
            For Each unit As cUnit In dtTarget.Keys
                ' Get links
                landings = dtTarget(unit)
                ' Check if every landing is represented
                For iGroup As Integer = 1 To Me.m_core.nGroups
                    ' Is Ecopath landing missing a link?
                    Dim strSpecies As String = Me.m_core.EcopathGroupInputs(iGroup).Name
                    If (fleet.Landings(iGroup) > 0) And (landings.IndexOf(strSpecies) = -1) Then
                        ' Get group
                        group = Me.m_core.EcopathGroupInputs(iGroup)
                        ' Create link
                        Console.WriteLine("> VC: Fleet {0}, group {1} missing landings link, added", fleet.Name, group.Name)
                        Me.m_data.CreateLandingsLink(prod, unit, strSpecies, bDummy)
                    End If
                Next
            Next

            ' Reset admin
            dtTarget.Clear()

        Next prod

    End Sub

    'Private Function IsRelevant(link As cLink) As Boolean

    '    If (TypeOf link.Source Is cProducerUnit) Then
    '        Dim fleet As cEcopathFleetInput = DirectCast(link.Source, cProducerUnit).Fleet
    '        Dim group As cEcoPathGroupInput = Me.group
    '        If (fleet IsNot Nothing) And (group IsNot Nothing) Then
    '            Return (fleet.Landings(group.Index) > 0)
    '        End If
    '    End If
    'End Function

End Class
