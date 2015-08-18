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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports System.Text

#End Region ' Imports

Public Class cStanzaSummarizer
    Inherits cCoreIOSummarizerBase

    Public Sub New(core As cCore)
        MyBase.New(core)
    End Sub

    Public Overrides Sub Init()

        MyBase.Init()

        For igrp As Integer = 0 To Me.m_core.nStanzas - 1
            Me.m_objects.Add(Me.Core.StanzaGroups(igrp))
        Next

        Me.m_variables.Add(eVarNameFlags.LeadingBiomass)
        Me.m_variables.Add(eVarNameFlags.LeadingCB)
        Me.m_variables.Add(eVarNameFlags.RecPowerSplit)
        Me.m_variables.Add(eVarNameFlags.BABsplit)
        Me.m_variables.Add(eVarNameFlags.WmatWinf)

        ' Hatch code handled explicity, see HatchValues
        ' Me.m_variables.Add(eVarNameFlags.HatchCode) 

        Me.m_variables.Add(eVarNameFlags.FixedFecundity)
        Me.m_variables.Add(eVarNameFlags.EggAtSpawn)

        ' Indexed vars new different treatment
        'Me.m_variables.Add(eVarNameFlags.Bat)
        'Me.m_variables.Add(eVarNameFlags.StartAge)
        'Me.m_variables.Add(eVarNameFlags.StanzaNumberAtAge)
        'Me.m_variables.Add(eVarNameFlags.StanzaWeightAtAge)
        'Me.m_variables.Add(eVarNameFlags.StanzaBiomassAtAge)
        'Me.m_variables.Add(eVarNameFlags.StanzaBiomass)
        'Me.m_variables.Add(eVarNameFlags.StanzaCB)
        'Me.m_variables.Add(eVarNameFlags.StanzaMortaility)

    End Sub

    Public Overrides Function HashValues() As System.Collections.Generic.List(Of cHashValues)

        Dim lResults As New List(Of cHashValues)
        lResults.AddRange(MyBase.getVarResults())

        Dim sb As New StringBuilder()
        Dim shp As cShapeData = Nothing
        Dim man As cEggProductionManager = Me.m_core.EggProdShapeManager
        Dim stz As ICoreInputOutput = Nothing
        Dim iShp As Integer

        For i As Integer = 0 To Me.m_objects.Count - 1
            stz = Me.m_objects(i)
            iShp = CInt(stz.GetVariable(eVarNameFlags.HatchCode))
            If (iShp > 0) Then shp = man.Item(iShp) Else shp = Nothing

            If (i > 0) Then sb.Append("|")
            sb.Append("shp=" & iShp)
            sb.Append(",data=" & cStringConverters.ShapeToString(shp))
        Next
        lResults.Add(New cHashValues(Me.ObjectDescriptor, eVarNameFlags.HatchCode, sb.ToString()))

        Return lResults
    End Function

#Region " Internals "

    Protected Overrides ReadOnly Property ObjectDescriptor As String
        Get
            Return "StanzaDefinitions"
        End Get
    End Property

#End Region ' Internals

End Class
