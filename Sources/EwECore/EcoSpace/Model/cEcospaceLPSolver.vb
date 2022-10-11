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
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities

#End Region ' Imports



Public Class cEcospaceLPSolver
    Private ReadOnly m_EcopathData As cEcopathDataStructures
    Private ReadOnly m_EcospaceData As cEcospaceDataStructures

    Public Sub New(EcospaceData As cEcospaceDataStructures)
        Me.m_EcospaceData = EcospaceData
    End Sub

    Public Sub EcospaceLPinitandrun()
        '  Start by making an Ecospace run so that we have a first estimate of spatial fleet effort


        '  Make a list with water cells True False
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim l As Integer = 0
        Dim m As Integer
        Dim cellCount As Integer = 0
        Dim waterCells(m_EcospaceData.InRow * m_EcospaceData.InCol) As Boolean

        Dim iNo(m_EcospaceData.InRow) As Double
        Dim jNo(m_EcospaceData.InCol) As Double

        For i = 1 To m_EcospaceData.InRow
            For j = 1 To m_EcospaceData.InCol
                k = k + 1
                If (m_EcospaceData.Depth(i, j) <> 0) Then
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
        For i = 1 To m_EcospaceData.InRow
            For j = 1 To m_EcospaceData.InCol
                If (m_EcospaceData.Depth(i, j) <> 0) Then
                    For k = 1 To m_EcospaceData.nFleets
                        l += 1
                        rowNo(l) = i
                        colNo(l) = j
                        fltNo(l) = k
                        effort(l) = m_EcospaceData.EffortSpace(k, i, j)
                        'For each element in this list, then calculate net value per unit effort summed over groups, as
                        'V_k =∑_g▒〖P_(f,g)×B_(g,i,j)×q_(f,g) 〗
                        'V(k) = sum over groups g Of {price(f,g)*B(g,i,j)*q(f,g)} For the kth list element's fleet f
                        'where q(f, g) Is the fishing rate on group g per unit effort by fleet f (basic ecosim parameter matrix qfish?).  
                        For m = 1 To m_EcospaceData.nLiving
                            'Net value per unit effort: calculate it from the Ecopath baseline, where effort of 1
                            'with a given B gives a certain catch for each fleet by group,
                            value(l) = value(l) + effort(l) * m_EcospaceData.Bcell(i, j, m) *
                                m_EcopathData.Landing(k, m) + m_EcopathData.Discard(k, m) / m_EcopathData.B(m) * _   'partial F for this fleet 
                                m_EcopathData.Market(k, m)
                        Next
                        'maybe subtract cost of fishing from value
                        If value(l) > 0 Then
                            ' value(l) = value(l) - m_EcospaceData.Sail(i, j, k)
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



#If False Then
 Carls code from email "carl code for ecospace LP" Sept 28 2022
 'see the word doc for color coding

 Guys,

Here’s a better version of the word doc I sent yesterday, with my recommended code changes to set up the ecospace LP shown in yellow and code to be replaced shown in red.  I’ve also added code bit to load the LP effort solution into EffortSpace to replace predicted values from gravity model.

CJ

    Dim ifleet(nwatercells) As Integer, irow(nwatercells As Integer),icol(nwatercells) As Integer
Dim Numvars As Integer
    Dim totB(Num_living) As Double
    Private Sub InitLPSolver()
        This routine Is called in cMSE.InitForRun(), which in turn Is called in cMSE.Run()
Dim iv As Integer 'index of LP variable name
&#39;Add the Fleets as Variables and get the Variable ID&#39;s into m_FleetCode
For iflt As Integer = 1 To Me.m_data.nFleets
            For i As Integer = 1 To nrows : For j = 1 To ncols
                    If depth(i, j) & gt;0 Then NOTE CAN ALSO EXCLUDE CELLS Set As MPAS With Not
FISHING ALLOWED AT ALL—NO EFFORT NEED BE CALCULATED FOR SUCH CELLS

iv = iv + 1
                        ifleet(iv) = iflt
                        irow(iv) = i
                        Icol(iv) = j
                        'add up total biomass for all groups
                        For igrp = 1 To num_living
                            totB(igrp) += Bcell(igrp, i, j)
                        Next
                        Me.m_LPSolver.AddVariable(STR(iv), STR(iv)) 'name the variable just to
                        be String(iv)

&#39;Set the bounds

Me.m_LPSolver.SetBounds(STR(iv), Me.m_data.LowLPEffort(iflt),
Me.m_data.UpperLPEffort(iflt)) &#39;Me.m_data.MaxEffort(iflt)
End If
                Next j : Next i

        Next
        Numvars = iv 'set number of LP variables to last iv in loops above
        For igrp As Integer = 1 To Me.m_data.nLiving
            Me.m_LPSolver.AddRow(Me.m_epdata.GroupName(igrp), Me.m_GroupCode(igrp))
        Next
        Me.m_LPSolver.AddRow(& quot;VALUE&quot;, Me.m_GoalRowID)
        Me.m_LPSolver.AddGoal(Me.m_GoalRowID, 1, False)

In Friend Sub RegulateEffort(Biomass() As Single, QMult() As Single, QYear() As Single, t As Integer, imonth
As Integer)
There 's a call every month=1 to:
Private Sub RegulateLPEffort(Biomass() As Single, QMult() As Single, QYear() As Single, t As Integer)
&#39;Get value for the LP Solver
For iFlt = 1 To Me.m_data.nFleets
            For iGrp = 1 To Me.m_data.nLiving
                VPerEffort(iFlt) += Me.m_data.QStar(iGrp, iFlt) * Biomass(iGrp) * Me.m_epdata.Market(iFlt, iGrp) *
                Me.m_esData.PropLandedTime(iFlt, iGrp)
            Next iGrp
        Next iFlt

        'replace the code in red above with this loop over all LP variables iv

        For iv = 1 To Numvars
            For iGrp = 1 To Me.m_data.nLiving
                VPerEffort(iv) += Me.m_data.QStar(iGrp, ifleet(iv)) * Bcell(iGrp, irow(iv), icol(iv)) *
                Me.m_epdata.Market(ifleet(iv), iGrp) * Me.m_esData.PropLandedTime(ifleet(iv), iGrp)

            Next
        Next
        Dim sumF As Single
        For iGrp = 1 To Me.m_data.nLiving
            sumF = 0
            For iFlt = 1 To Me.m_data.nFleets
                Me.m_LPSolver.SetCoefficient(Me.m_GroupCode(iGrp), Me.m_FleetCode(iFlt),
                Me.m_data.QStar(iGrp, iFlt))
                sumF += Me.m_data.QStar(iGrp, iFlt)
            Next

            'replace code in red with this
            For iv = 1 To Numvars
                Me.m_LPSolver.SetCoefficient(Me.m_GroupCode(iGrp), STR(iv),

Me.m_data.QStar(iGrp, ifleet(iv))*Bcell(iGrp,irow(iv),icol(iv)))
Next

&#39;Debug.Assert(sumF &lt;= Me.m_data.FTarget(iGrp))
Me.m_LPSolver.SetBounds(Me.m_GroupCode(iGrp), Me.m_data.FTarget(iGrp) * totB(iGrp))
            'NOTE THIS IMPORTANT CHANGE TO BOUND FOR GROUP ALLOWABLE TOTAL CATCH
        Next
        For iFlt = 1 To Me.m_data.nFleets
            Me.m_LPSolver.SetCoefficient(Me.m_GoalRowID, Me.m_FleetCode(iFlt), VPerEffort(iFlt))
            Me.m_LPSolver.SetBounds(Me.m_GoalRowID, 0, Double.PositiveInfinity)
        Next

        'replace red code with this
        For iv = 1 To Numvars
            Me.m_LPSolver.SetCoefficient(Me.m_GoalRowID, STR(iv), VPerEffort(iv))

            Me.m_LPSolver.SetBounds(Me.m_GoalRowID, 0, Double.PositiveInfinity)
        Next
        Next

        Dim lpSolveReturnValue As EwEUtils.Core.eSolverReturnValues
        lpSolveReturnValue = Me.m_LPSolver.Solve(t)
&#39;Dual Or Shadow variables
&#39;Effort Is regulated once a year at the first time step of the month
&#39;This populates all the time steps for this year with the dual values
For iGrp = 1 To Me.m_data.nLiving
            Dim dv As Single = Math.Abs(CSng(Me.m_LPSolver.GetDualValue(Me.m_GroupCode(iGrp))))
&#39;t Is the first month of this year
For it As Integer = t To t + 11
                Me.m_data.FLPDualValue.AddValue(iGrp, it, dv)
            Next
        Next
        If lpSolveReturnValue = eSolverReturnValues.OPTIMAL Then
            For iFlt = 1 To Me.m_data.nFleets
                Me.m_esData.FishRateGear(iFlt, t) = CSng(Me.m_LPSolver.GetValue(Me.m_FleetCode(iFlt)))
&#39;System.Console.Write(&quot;Fleet ID &quot; &amp; Me.m_LPSolver.GetValue(Me.m_FleetCode(iFlt)).ToString)
Next

            'replace red lines above to load solution into ecospace efforts
            For iv = 1 To Numvars

                m_Data.EffortSpace(ifleet(iv), irow(iv), Icol(iv)) =

CSng(Me.m_LPSolver.GetValue(iv)

Next

        Else
&#39;LP Solver failed to find an optimized solution
&#39;add the failed time step to the list of non optimal solutions
Me.m_data.lstNonOptSolutions.Add(t)

'can just remove red code below so as not to change EffortSpace values

&#39;populate Effort with the effort from the last time step
Dim tNonOpt As Integer = t - 1
            If t = 1 Then tNonOpt = 1
            For iFlt = 1 To Me.m_data.nFleets
                Me.m_esData.FishRateGear(iFlt, t) = Me.m_esData.FishRateGear(iFlt, tNonOpt)
&#39;System.Console.Write(&quot;Fleet ID &quot; &amp; Me.m_LPSolver.GetValue(Me.m_FleetCode(iFlt)).ToString)
Next
&#39;&#39;&#39; &lt;summary&gt;
&#39;&#39;&#39; Get the LP Solution from the lpsolve55 API directly, instead of from the cLPSolver wrapper
&#39;&#39;&#39; &lt;/summary&gt;
&#39;&#39;&#39; &lt;param name=&quot;Biomass&quot;&gt;&lt;/param&gt;
&#39;&#39;&#39; &lt;param name=&quot;QMult&quot;&gt;&lt;/param&gt;
&#39;&#39;&#39; &lt;param name=&quot;QYear&quot;&gt;&lt;/param&gt;
&#39;&#39;&#39; &lt;param name=&quot;t&quot;&gt;&lt;/param&gt;
&#39;&#39;&#39; &lt;remarks&gt;This Is for debugging the setup of LPSolve via the API&lt;/remarks&gt;

In Private Sub RegulateLPEffort there's a call to:
Private Sub RegulateEffortViaLPSolve(Biomass() As Single, QMult() As Single, QYear() As Single, t As
Integer)
Dim iFlt As Integer, iGrp As Integer
        Dim VPerEffort() As Double
        Try
            cLPSolver.lpsolve55.Init()
            ReDim VPerEffort(Me.m_data.nFleets)
            Dim ptrLp As Integer = cLPSolver.lpsolve55.make_lp(0, Me.m_data.nFleets)
            Dim badded As Boolean
&#39;Add the Fleets as Variables and get the Variable ID&#39;s into m_FleetCode
For iFlt = 1 To Me.m_data.nFleets
                badded = cLPSolver.lpsolve55.set_bounds(ptrLp, iFlt, CDbl(Me.m_data.LowLPEffort(iFlt)),
                CDbl(Me.m_data.UpperLPEffort(iFlt)))
            Next
&#39;Get fishing mortality at this time step
For iFlt = 1 To Me.m_data.nFleets
                For iGrp = 1 To Me.m_data.NGroups
                    If t & gt; 1 Then
&#39;QStar(iGrp, iFlt) = Me.m_esData.FishMGear(iFlt, iGrp) * QYear(iFlt) * QMult(iGrp)
&#39;Using Kalman filter to update catchability estimate
Me.m_data.Qest(iGrp, iFlt) = (1 - Me.m_data.KalGainQ(iFlt)) * (Me.m_data.CatchYear(iFlt,
iGrp) / 12) / Me.m_data.BestimateLast(iGrp) / (Me.m_esData.FishRateGear(iFlt, t - 12) + 1.0E-20F) +
Me.m_data.KalGainQ(iFlt) * Me.m_data.Qest(iGrp, iFlt)

                    End If
                    Me.m_data.Qest(iGrp, iFlt) = Me.m_esData.FishMGear(iFlt, iGrp) * QYear(iFlt) * QMult(iGrp)
                    Me.m_data.QStar(iGrp, iFlt) = Me.m_data.Qest(iGrp, iFlt) *
                    (Me.m_esData.PropLandedTime(iFlt, iGrp) + (1 - Me.m_esData.PropLandedTime(iFlt, iGrp)) *
                    Me.m_epdata.PropDiscardMort(iFlt, iGrp))
                Next iGrp
            Next iFlt
&#39;Get value for the LP Solver
For iFlt = 1 To Me.m_data.nFleets
                For iGrp = 1 To Me.m_data.NGroups
                    VPerEffort(iFlt) += Me.m_data.QStar(iGrp, iFlt) * Biomass(iGrp) * Me.m_epdata.Market(iFlt,
                    iGrp) * Me.m_esData.PropLandedTime(iFlt, iGrp)
                Next iGrp
            Next iFlt
&#39;Added the objective/goal before adding rows/constraints
badded = cLPSolver.lpsolve55.set_obj_fn(ptrLp, VPerEffort)
            Dim constraint() As Double
            ReDim constraint(Me.m_data.nFleets)
            For iGrp = 1 To Me.m_data.NGroups
                For iFlt = 1 To Me.m_data.nFleets
                    constraint(iFlt) = CDbl(Me.m_data.QStar(iGrp, iFlt))
                Next
                badded = cLPSolver.lpsolve55.add_constraint(ptrLp, constraint,
                cLPSolver.lpsolve55.lpsolve_constr_types.LE, Me.m_data.FTarget(iGrp))
            Next
            cLPSolver.lpsolve55.set_maxim(ptrLp)
            Dim rv As cLPSolver.lpsolve55.lpsolve_return
            rv = cLPSolver.lpsolve55.solve(ptrLp)
            If rv & lt;&gt; cLPSolver.lpsolve55.lpsolve_return.OPTIMAL Then
System.Console.WriteLine(& quot;LP Solver Non Optimal Solution: &quot; &amp; rv.ToString &amp; &quot; Timestep = &quot; &amp;
t.ToString)
End If
            Dim solution() As Double
            ReDim solution(1 + cLPSolver.lpsolve55.get_Ncolumns(ptrLp) +
            cLPSolver.lpsolve55.get_Nrows(ptrLp))
            cLPSolver.lpsolve55.get_primal_solution(ptrLp, solution)
            Dim dualValues() As Double
            ReDim dualValues(1 + cLPSolver.lpsolve55.get_Ncolumns(ptrLp) +
            cLPSolver.lpsolve55.get_Nrows(ptrLp))
            cLPSolver.lpsolve55.get_dual_solution(ptrLp, dualValues)
            For iFlt = 1 To Me.m_data.nFleets
                Me.m_esData.FishRateGear(iFlt, t) = CSng(solution(Me.m_data.NGroups + iFlt))
&#39; System.Console.Write(&quot;Fleet ID &quot; &amp; Me.m_LPSolver.GetValue(Me.m_FleetCode(iFlt)).ToString)
Next
            For iGrp = 1 To Me.m_data.nLiving
                For it As Integer = t To t + 11
                    Me.m_data.FLPDualValue.AddValue(iGrp, it, CSng(Math.Abs(dualValues(iGrp))))
                Next
            Next
            cLPSolver.lpsolve55.delete_lp(ptrLp)

        Catch ex As Exception
        End Try
    End Sub


    
#End If

End Class
