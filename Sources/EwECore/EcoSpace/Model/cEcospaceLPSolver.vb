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
Imports System.Collections.Generic
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities

#End Region ' Imports



Public Class cEcospaceLPSolver
    Private m_EcospaceData As cEcospaceDataStructures
    Private m_core As cCore

    Public Sub New(EcospaceData As cEcospaceDataStructures)
        Me.m_EcospaceData = EcospaceData
    End Sub

    Public Sub EcospaceLPinitandrun()
        '  Start by making an Ecospace run so that we have a first estimate of spatial fleet effort


        '  Make a list with water cells True False
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim cellCount As Integer = 0
        Dim waterCells(m_EcospaceData.InRow * m_EcospaceData.InCol) As Boolean

        Dim iNo(m_EcospaceData.InRow) As Double
        Dim jNo(m_EcospaceData.InCol) As Double

        For i = 1 To m_EcospaceData.InRow
            For j = 1 To m_EcospaceData.InCol
                k = k + 1
                If (m_EcospaceData.Depth(i, j) <> 0 Then
                    waterCells(k) = True
                    cellCount += 1
                Else
                    waterCells(k) = False
                End If
            Next
        Next

        '  creating a list structure k=1…nf*ncells, with efforts F(f,i,j) for the fleets f and areas i,j arranged as
        '  F = {F(1, 1, 1),…F(1, m, n),F(2,1,1)…F(2,m,n),…,F(nf,1,1)…F(nf,m,n)}

        Dim effort(m_EcospaceData.nFleets * cellCount) As Double
        Dim value(m_EcospaceData.nFleets * cellCount) As Double
        ' f For each l, you will need to store fleetno(k)=fleet code for cell l, along with ino(l)=cell row, jno(lk)=cell column,
        ' ' so that you an reference each l by its fleet, i, and j combination. 
        Dim rowNo(m_EcospaceData.nFleets * cellCount) As Integer
        Dim colNo(m_EcospaceData.nFleets * cellCount) As Integer
        Dim fltNo(m_EcospaceData.nFleets * cellCount) As Integer
        Dim l As Integer = 0
        For i = 1 To m_EcospaceData.InRow
            For j = 1 To m_EcospaceData.InCol
                If (m_EcospaceData.Depth(i, j) <> 0 Then
                    For k = 1 To Me.m_nFleets
                        l += 1
                        rowNo(l) = i
                        colNo(l) = j
                        fltNo(l) = k
                        effort(l) = m_EcospaceData.EffortSpace(k, i, j)
                        'For each element in this list, then calculate net value per unit effort summed over groups, as
                        'V_k =∑_g▒〖P_(f,g)×B_(g,i,j)×q_(f,g) 〗
                        'V(k) = sum over groups g Of {price(f,g)*B(g,i,j)*q(f,g)} For the kth list element's fleet f
                        'where q(f, g) Is the fishing rate on group g per unit effort by fleet f (basic ecosim parameter matrix qfish?).  
                        For (m = 1 To m_EcospaceData.nLiving)
                            'Net value per unit effort: calculate it from the Ecopath baseline, where effort of 1
                            'with a given B gives a certain catch for each fleet by group,
                            value(l) = value(l) + Price(k, m) * EcospaceB(i, j, m) * Ecopath.Catch(k, m) / Ecopath.biomass(m)
                        Next
                        'mabye subtract cost of fishing from value
                        If value(l) > 0 Then
                            value(l) = value(l) - m_EcospaceData.Sail(i, j, k)
                            ' Check the units for sail cost, above assumes it's in $
                            'will negative values of value() work in LP optim ?  Otherwise,
                            'If value(l) < 0 Then value(l) = 0
                        End If
                    Next
                End If
            Next

        Next

        ' Now somehow (i.e. JB) run the LPSolver and output is effort by fleet by water cell
        ' assume this is called LPout
        Dim LPout(m_EcospaceData.nFleets * cellCount) As Double

        For l = 1 To m_EcospaceData.nFleets * cellCount
            m_EcospaceData.EffortSpace(fltNo(l), rowNo(l), colNo(l)) = CSng(LPout(l))
        Next

        '  
    End Sub


End Class
