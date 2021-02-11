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

''' ---------------------------------------------------------------------------
''' <summary>
''' LP solver interface to the unmanaged lp_solve engine version 5.5
''' </summary>
''' <remarks>
''' Please refer to the Microsoft Solver Foundation API reference for using the
''' methods in this class. Note that this solver wraps unmanaged code; this class
''' will only work on Windows.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cLPSolver
    Implements ILPSolver

#Region " Private classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' LPSolve unmanaged library wrapper
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class lpsolve55

        Private Shared g_bInit As Boolean = False
        Private Shared g_bUsable As Boolean = False

        Public Shared Sub Init()
            If g_bInit Then Return
            Dim badded As Boolean = True
            Dim solveDir As String

            Try
                g_bUsable = cSystemUtils.IsWindows
                If cSystemUtils.Is64BitProcess Then
                    solveDir = "Includes\LPSolve\win64"
                Else
                    solveDir = "Includes\LPSolve\win32"
                End If

                lpsolve55.SetDllDirectoryA(solveDir)

                'Make sure lpsolve55.dll exists in the correct directory 
                Dim dllPath As String = System.IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
                Dim lpsolveDLL As String = System.IO.Path.Combine(dllPath, solveDir, "lpsolve55.dll")
                If Not System.IO.File.Exists(lpsolveDLL) Then
                    System.Console.WriteLine("Failed to find lpsolve55.dll in " & lpsolveDLL)
                    cLog.Write("Failed to find lpsolve55.dll in " & lpsolveDLL)
                    g_bUsable = False
                End If

            Catch ex As Exception
                cLog.Write(ex, "lpsolve55::Init")
                System.Console.WriteLine("Exception in lpsolve55.Init() " & ex.Message)
                g_bUsable = False
                Return
            End Try

            g_bInit = True

        End Sub

        Public Shared Function IsUsable() As Boolean
            Return g_bUsable
        End Function

        'lpsolve version 5 routines

        Private Declare Function SetEnvironmentVariableA Lib "kernel32" (lpname As String, lpValue As String) As Integer
        Private Declare Function GetEnvironmentVariableA Lib "kernel32" (lpname As String, lpBuffer As String, nSize As Integer) As Integer

        '-----------------------------------------------------------------------------------------------------------------------------
        Public Declare Function SetDllDirectoryA Lib "kernel32" (lpPathName As String) As Long

        Public Declare Function add_column Lib "lpsolve55.dll" Alias "add_column" (lp As Integer, column() As Double) As Boolean
        Public Declare Function add_columnex Lib "lpsolve55.dll" Alias "add_columnex" (lp As Integer, count As Integer, column() As Double, rowno() As Integer) As Boolean
        Public Declare Function add_constraint Lib "lpsolve55.dll" Alias "add_constraint" (lp As Integer, row() As Double, constr_type As lpsolve_constr_types, rh As Double) As Boolean
        Public Declare Function add_constraintex Lib "lpsolve55.dll" Alias "add_constraintex" (lp As Integer, count As Integer, row() As Double, colno() As Integer, constr_type As lpsolve_constr_types, rh As Double) As Boolean
        Public Declare Function add_lag_con Lib "lpsolve55.dll" Alias "add_lag_con" (lp As Integer, row() As Double, con_type As lpsolve_constr_types, rhs As Double) As Boolean
        Public Declare Function add_SOS Lib "lpsolve55.dll" Alias "add_SOS" (lp As Integer, name As String, sostype As Integer, priority As Integer, count As Integer, sosvars() As Integer, weights() As Double) As Integer
        Public Declare Function column_in_lp Lib "lpsolve55.dll" Alias "column_in_lp" (lp As Integer, column() As Double) As Integer
        Public Declare Function copy_lp Lib "lpsolve55.dll" Alias "copy_lp" (lp As Integer) As Integer
        Public Declare Sub default_basis Lib "lpsolve55.dll" Alias "default_basis" (lp As Integer)
        Public Declare Function del_column Lib "lpsolve55.dll" Alias "del_column" (lp As Integer, column As Integer) As Boolean
        Public Declare Function del_constraint Lib "lpsolve55.dll" Alias "del_constraint" (lp As Integer, del_row As Integer) As Boolean
        Public Declare Sub delete_lp Lib "lpsolve55.dll" Alias "delete_lp" (lp As Integer)
        Public Declare Function dualize_lp Lib "lpsolve55.dll" Alias "dualize_lp" (lp As Integer) As Boolean
        Public Declare Function get_anti_degen Lib "lpsolve55.dll" Alias "get_anti_degen" (lp As Integer) As lpsolve_anti_degen
        Public Declare Function get_basis Lib "lpsolve55.dll" Alias "get_basis" (lp As Integer, bascolumn() As Integer, nonbasic As Boolean) As Boolean
        Public Declare Function get_basiscrash Lib "lpsolve55.dll" Alias "get_basiscrash" (lp As Integer) As lpsolve_basiscrash
        Public Declare Function get_bb_depthlimit Lib "lpsolve55.dll" Alias "get_bb_depthlimit" (lp As Integer) As Integer
        Public Declare Function get_bb_floorfirst Lib "lpsolve55.dll" Alias "get_bb_floorfirst" (lp As Integer) As lpsolve_branch
        Public Declare Function get_bb_rule Lib "lpsolve55.dll" Alias "get_bb_rule" (lp As Integer) As lpsolve_BBstrategies
        Public Declare Function get_bounds_tighter Lib "lpsolve55.dll" Alias "get_bounds_tighter" (lp As Integer) As Boolean
        Public Declare Function get_break_at_value Lib "lpsolve55.dll" Alias "get_break_at_value" (lp As Integer) As Double
        Public Declare Function get_col_name Lib "lpsolve55.dll" Alias "get_col_name" (lp As Integer, column As Integer) As String
        Public Declare Function get_column Lib "lpsolve55.dll" Alias "get_column" (lp As Integer, col_nr As Integer, column() As Double) As Boolean
        Public Declare Function get_columnex Lib "lpsolve55.dll" Alias "get_columnex" (lp As Integer, col_nr As Integer, column() As Double, nzrow() As Integer) As Integer
        Public Declare Function get_constr_type Lib "lpsolve55.dll" Alias "get_constr_type" (lp As Integer, row As Integer) As lpsolve_constr_types
        Public Declare Function get_constr_value Lib "lpsolve55.dll" Alias "get_constr_value" (lp As Integer, row As Integer, count As Integer, primsolution() As Double, nzindex() As Integer) As Double
        Public Declare Function get_constraints Lib "lpsolve55.dll" Alias "get_constraints" (lp As Integer, constr() As Double) As Boolean
        Public Declare Function get_dual_solution Lib "lpsolve55.dll" Alias "get_dual_solution" (lp As Integer, rc() As Double) As Boolean
        Public Declare Function get_epsb Lib "lpsolve55.dll" Alias "get_epsb" (lp As Integer) As Double
        Public Declare Function get_epsd Lib "lpsolve55.dll" Alias "get_epsd" (lp As Integer) As Double
        Public Declare Function get_epsel Lib "lpsolve55.dll" Alias "get_epsel" (lp As Integer) As Double
        Public Declare Function get_epsint Lib "lpsolve55.dll" Alias "get_epsint" (lp As Integer) As Double
        Public Declare Function get_epsperturb Lib "lpsolve55.dll" Alias "get_epsperturb" (lp As Integer) As Double
        Public Declare Function get_epspivot Lib "lpsolve55.dll" Alias "get_epspivot" (lp As Integer) As Double
        Public Declare Function get_improve Lib "lpsolve55.dll" Alias "get_improve" (lp As Integer) As lpsolve_improves
        Public Declare Function get_infinite Lib "lpsolve55.dll" Alias "get_infinite" (lp As Integer) As Double
        Public Declare Function get_lambda Lib "lpsolve55.dll" Alias "get_lambda" (lp As Integer, lambda() As Double) As Boolean
        Public Declare Function get_lowbo Lib "lpsolve55.dll" Alias "get_lowbo" (lp As Integer, column As Integer) As Double
        Public Declare Function get_lp_index Lib "lpsolve55.dll" Alias "get_lp_index" (lp As Integer, orig_index As Integer) As Integer
        Public Declare Function get_lp_name Lib "lpsolve55.dll" Alias "get_lp_name" (lp As Integer) As String
        Public Declare Function get_Lrows Lib "lpsolve55.dll" Alias "get_Lrows" (lp As Integer) As Integer
        Public Declare Function get_mat Lib "lpsolve55.dll" Alias "get_mat" (lp As Integer, row As Integer, column As Integer) As Double
        Public Declare Function get_max_level Lib "lpsolve55.dll" Alias "get_max_level" (lp As Integer) As Integer
        Public Declare Function get_maxpivot Lib "lpsolve55.dll" Alias "get_maxpivot" (lp As Integer) As Integer
        Public Declare Function get_mip_gap Lib "lpsolve55.dll" Alias "get_mip_gap" (lp As Integer, absolute As Boolean) As Double
        Public Declare Function get_Ncolumns Lib "lpsolve55.dll" Alias "get_Ncolumns" (lp As Integer) As Integer
        Public Declare Function get_negrange Lib "lpsolve55.dll" Alias "get_negrange" (lp As Integer) As Double
        Public Declare Function get_nameindex Lib "lpsolve55.dll" Alias "get_nameindex" (lp As Integer, name As String, isrow As Boolean) As Integer
        Public Declare Function get_nonzeros Lib "lpsolve55.dll" Alias "get_nonzeros" (lp As Integer) As Integer
        Public Declare Function get_Norig_columns Lib "lpsolve55.dll" Alias "get_Norig_columns" (lp As Integer) As Integer
        Public Declare Function get_Norig_rows Lib "lpsolve55.dll" Alias "get_Norig_rows" (lp As Integer) As Integer
        Public Declare Function get_Nrows Lib "lpsolve55.dll" Alias "get_Nrows" (lp As Integer) As Integer
        Public Declare Function get_obj_bound Lib "lpsolve55.dll" Alias "get_obj_bound" (lp As Integer) As Double
        Public Declare Function get_objective Lib "lpsolve55.dll" Alias "get_objective" (lp As Integer) As Double
        Public Declare Function get_orig_index Lib "lpsolve55.dll" Alias "get_orig_index" (lp As Integer, lp_index As Integer) As Integer
        Public Declare Function get_origcol_name Lib "lpsolve55.dll" Alias "get_origcol_name" (lp As Integer, column As Integer) As String
        Public Declare Function get_origrow_name Lib "lpsolve55.dll" Alias "get_origrow_name" (lp As Integer, row As Integer) As String
        Public Declare Function get_pivoting Lib "lpsolve55.dll" Alias "get_pivoting" (lp As Integer) As lpsolve_piv_rules
        Public Declare Function get_presolve Lib "lpsolve55.dll" Alias "get_presolve" (lp As Integer) As lpsolve_presolve
        Public Declare Function get_presolveloops Lib "lpsolve55.dll" Alias "get_presolveloops" (lp As Integer) As Integer
        Public Declare Function get_primal_solution Lib "lpsolve55.dll" Alias "get_primal_solution" (lp As Integer, pv_Renamed() As Double) As Boolean
        Public Declare Function get_print_sol Lib "lpsolve55.dll" Alias "get_print_sol" (lp As Integer) As Integer
        Public Declare Function get_PseudoCosts Lib "lpsolve55.dll" Alias "get_PseudoCosts" (lp As Integer, clower() As Double, cupper() As Double, updatelimit() As Integer) As Boolean
        Public Declare Function get_rh Lib "lpsolve55.dll" Alias "get_rh" (lp As Integer, row As Integer) As Double
        Public Declare Function get_rh_range Lib "lpsolve55.dll" Alias "get_rh_range" (lp As Integer, row As Integer) As Double
        Public Declare Function get_row Lib "lpsolve55.dll" Alias "get_row" (lp As Integer, row_nr As Integer, row() As Double) As Boolean
        Public Declare Function get_rowex Lib "lpsolve55.dll" Alias "get_rowex" (lp As Integer, row_nr As Integer, row() As Double, colno() As Integer) As Integer
        Public Declare Function get_row_name Lib "lpsolve55.dll" Alias "get_row_name" (lp As Integer, row As Integer) As String
        Public Declare Function get_scalelimit Lib "lpsolve55.dll" Alias "get_scalelimit" (lp As Integer) As Double
        Public Declare Function get_scaling Lib "lpsolve55.dll" Alias "get_scaling" (lp As Integer) As lpsolve_scales
        Public Declare Function get_sensitivity_obj Lib "lpsolve55.dll" Alias "get_sensitivity_obj" (lp As Integer, objfrom() As Double, objtill() As Double) As Boolean
        Public Declare Function get_sensitivity_objex Lib "lpsolve55.dll" Alias "get_sensitivity_objex" (lp As Integer, objfrom() As Double, objtill() As Double, objfromvalue() As Double, objtillvalue() As Double) As Boolean
        Public Declare Function get_sensitivity_rhs Lib "lpsolve55.dll" Alias "get_sensitivity_rhs" (lp As Integer, duals() As Double, dualsfrom() As Double, dualstill() As Double) As Boolean
        Public Declare Function get_simplextype Lib "lpsolve55.dll" Alias "get_simplextype" (lp As Integer) As lpsolve_simplextypes
        Public Declare Function get_solutioncount Lib "lpsolve55.dll" Alias "get_solutioncount" (lp As Integer) As Integer
        Public Declare Function get_solutionlimit Lib "lpsolve55.dll" Alias "get_solutionlimit" (lp As Integer) As Integer
        Public Declare Function get_status Lib "lpsolve55.dll" Alias "get_status" (lp As Integer) As Integer
        Public Declare Function get_statustext Lib "lpsolve55.dll" Alias "get_statustext" (lp As Integer, statuscode As Integer) As String
        Public Declare Function get_timeout Lib "lpsolve55.dll" Alias "get_timeout" (lp As Integer) As Integer
        Public Declare Function get_total_iter Lib "lpsolve55.dll" Alias "get_total_iter" (lp As Integer) As Long
        Public Declare Function get_total_nodes Lib "lpsolve55.dll" Alias "get_total_nodes" (lp As Integer) As Long
        Public Declare Function get_upbo Lib "lpsolve55.dll" Alias "get_upbo" (lp As Integer, column As Integer) As Double
        Public Declare Function get_var_branch Lib "lpsolve55.dll" Alias "get_var_branch" (lp As Integer, column As Integer) As lpsolve_branch
        Public Declare Function get_var_dualresult Lib "lpsolve55.dll" Alias "get_var_dualresult" (lp As Integer, index As Integer) As Double
        Public Declare Function get_var_primalresult Lib "lpsolve55.dll" Alias "get_var_primalresult" (lp As Integer, index As Integer) As Double
        Public Declare Function get_var_priority Lib "lpsolve55.dll" Alias "get_var_priority" (lp As Integer, column As Integer) As Integer
        Public Declare Function get_variables Lib "lpsolve55.dll" Alias "get_variables" (lp As Integer, var() As Double) As Boolean
        Public Declare Function get_verbose Lib "lpsolve55.dll" Alias "get_verbose" (lp As Integer) As Integer
        Public Declare Function get_working_objective Lib "lpsolve55.dll" Alias "get_working_objective" (lp As Integer) As Double
        Public Declare Function guess_basis Lib "lpsolve55.dll" Alias "guess_basis" (lp As Integer, guessvector() As Double, basisvector() As Integer) As Boolean
        Public Declare Function has_BFP Lib "lpsolve55.dll" Alias "has_BFP" (lp As Integer) As Boolean
        Public Declare Function has_XLI Lib "lpsolve55.dll" Alias "has_XLI" (lp As Integer) As Boolean
        Public Declare Function is_add_rowmode Lib "lpsolve55.dll" Alias "is_add_rowmode" (lp As Integer) As Boolean
        Public Declare Function is_anti_degen Lib "lpsolve55.dll" Alias "is_anti_degen" (lp As Integer, testmask As lpsolve_anti_degen) As Boolean
        Public Declare Function is_binary Lib "lpsolve55.dll" Alias "is_binary" (lp As Integer, column As Integer) As Boolean
        Public Declare Function is_break_at_first Lib "lpsolve55.dll" Alias "is_break_at_first" (lp As Integer) As Boolean
        Public Declare Function is_constr_type Lib "lpsolve55.dll" Alias "is_constr_type" (lp As Integer, row As Integer, mask As Integer) As Boolean
        Public Declare Function is_debug Lib "lpsolve55.dll" Alias "is_debug" (lp As Integer) As Boolean
        Public Declare Function is_feasible Lib "lpsolve55.dll" Alias "is_feasible" (lp As Integer, values() As Double, threshold As Double) As Boolean
        Public Declare Function is_infinite Lib "lpsolve55.dll" Alias "is_infinite" (lp As Integer, value As Double) As Boolean
        Public Declare Function is_int Lib "lpsolve55.dll" Alias "is_int" (lp As Integer, column As Integer) As Boolean
        Public Declare Function is_integerscaling Lib "lpsolve55.dll" Alias "is_integerscaling" (lp As Integer) As Boolean
        Public Declare Function is_lag_trace Lib "lpsolve55.dll" Alias "is_lag_trace" (lp As Integer) As Boolean
        Public Declare Function is_maxim Lib "lpsolve55.dll" Alias "is_maxim" (lp As Integer) As Boolean
        Public Declare Function is_nativeBFP Lib "lpsolve55.dll" Alias "is_nativeBFP" (lp As Integer) As Boolean
        Public Declare Function is_nativeXLI Lib "lpsolve55.dll" Alias "is_nativeXLI" (lp As Integer) As Boolean
        Public Declare Function is_negative Lib "lpsolve55.dll" Alias "is_negative" (lp As Integer, column As Integer) As Boolean
        Public Declare Function is_piv_mode Lib "lpsolve55.dll" Alias "is_piv_mode" (lp As Integer, testmask As lpsolve_piv_rules) As Boolean
        Public Declare Function is_piv_rule Lib "lpsolve55.dll" Alias "is_piv_rule" (lp As Integer, rule As lpsolve_piv_rules) As Boolean
        Public Declare Function is_presolve Lib "lpsolve55.dll" Alias "is_presolve" (lp As Integer, testmask As lpsolve_presolve) As Boolean
        Public Declare Function is_scalemode Lib "lpsolve55.dll" Alias "is_scalemode" (lp As Integer, testmask As lpsolve_scales) As Boolean
        Public Declare Function is_scaletype Lib "lpsolve55.dll" Alias "is_scaletype" (lp As Integer, scaletype As lpsolve_scales) As Boolean
        Public Declare Function is_semicont Lib "lpsolve55.dll" Alias "is_semicont" (lp As Integer, column As Integer) As Boolean
        Public Declare Function is_SOS_var Lib "lpsolve55.dll" Alias "is_SOS_var" (lp As Integer, column As Integer) As Boolean
        Public Declare Function is_trace Lib "lpsolve55.dll" Alias "is_trace" (lp As Integer) As Boolean
        Public Declare Function is_unbounded Lib "lpsolve55.dll" Alias "is_unbounded" (lp As Integer, column As Integer) As Boolean
        Public Declare Function is_use_names Lib "lpsolve55.dll" Alias "is_use_names" (lp As Integer, isrow As Boolean) As Boolean
        Public Declare Sub version Lib "lpsolve55.dll" Alias "lp_solve_version" (ByRef majorversion As Integer, ByRef minorversion As Integer, ByRef release As Integer, ByRef build As Integer)
        Public Declare Function make_lp Lib "lpsolve55.dll" Alias "make_lp" (rows As Integer, columns As Integer) As Integer
        Public Declare Function resize_lp Lib "lpsolve55.dll" Alias "resize_lp" (lp As Integer, rows As Integer, columns As Integer) As Boolean
        Public Declare Sub print_constraints Lib "lpsolve55.dll" Alias "print_constraints" (lp As Integer, columns As Integer)
        Public Declare Function print_debugdump Lib "lpsolve55.dll" Alias "print_debugdump" (lp As Integer, filename As String) As Boolean
        Public Declare Sub print_duals Lib "lpsolve55.dll" Alias "print_duals" (lp As Integer)
        Public Declare Sub print_lp Lib "lpsolve55.dll" Alias "print_lp" (lp As Integer)
        Public Declare Sub print_objective Lib "lpsolve55.dll" Alias "print_objective" (lp As Integer)
        Public Declare Sub print_scales Lib "lpsolve55.dll" Alias "print_scales" (lp As Integer)
        Public Declare Sub print_solution Lib "lpsolve55.dll" Alias "print_solution" (lp As Integer, columns As Integer)
        Public Declare Sub print_str Lib "lpsolve55.dll" Alias "print_str" (lp As Integer, str_Renamed As String)
        Public Declare Sub print_tableau Lib "lpsolve55.dll" Alias "print_tableau" (lp As Integer)
        Public Delegate Function abortfunc(lp As Integer, userhandle As Integer) As Integer
        Public Declare Sub put_abortfunc Lib "lpsolve55.dll" Alias "put_abortfunc" (lp As Integer, newctrlc As abortfunc, ctrlchandle As Integer)
        Public Delegate Sub logfunc(lp As Integer, userhandle As Integer, buf As String)
        Public Declare Sub put_logfunc Lib "lpsolve55.dll" Alias "put_logfunc" (lp As Integer, newlog As logfunc, loghandle As Integer)
        Public Delegate Sub msgfunc(lp As Integer, userhandle As Integer, message As lpsolve_msgmask)
        Public Declare Sub put_msgfunc Lib "lpsolve55.dll" Alias "put_msgfunc" (lp As Integer, newmsg As msgfunc, msghandle As Integer, mask As lpsolve_msgmask)
        Public Declare Function read_basis Lib "lpsolve55.dll" Alias "read_basis" (lp As Integer, filename As String, info As String) As Boolean
        Public Declare Function read_freeMPS Lib "lpsolve55.dll" Alias "read_freeMPS" (filename As String, options As Integer) As Integer
        Public Declare Function read_LP Lib "lpsolve55.dll" Alias "read_LP" (filename As String, verbose As Integer, lp_name As String) As Integer
        Public Declare Function read_MPS Lib "lpsolve55.dll" Alias "read_MPS" (filename As String, options As Integer) As Integer
        Public Declare Function read_XLI Lib "lpsolve55.dll" Alias "read_XLI" (xliname As String, modelname As String, dataname As String, options As String, verbose As Integer) As Integer
        Public Declare Function read_params Lib "lpsolve55.dll" Alias "read_params" (lp As Integer, filename As String, options As String) As Boolean
        Public Declare Sub reset_basis Lib "lpsolve55.dll" Alias "reset_basis" (lp As Integer)
        Public Declare Sub reset_params Lib "lpsolve55.dll" Alias "reset_params" (lp As Integer)
        Public Declare Function set_add_rowmode Lib "lpsolve55.dll" Alias "set_add_rowmode" (lp As Integer, turnon As Boolean) As Boolean
        Public Declare Sub set_anti_degen Lib "lpsolve55.dll" Alias "set_anti_degen" (lp As Integer, anti_degen As lpsolve_anti_degen)
        Public Declare Function set_basis Lib "lpsolve55.dll" Alias "set_basis" (lp As Integer, bascolumn() As Integer, nonbasic As Boolean) As Boolean
        Public Declare Sub set_basiscrash Lib "lpsolve55.dll" Alias "set_basiscrash" (lp As Integer, mode As lpsolve_basiscrash)
        Public Declare Sub set_basisvar Lib "lpsolve55.dll" Alias "set_basisvar" (lp As Integer, basisPos As Integer, enteringCol As Integer)
        Public Declare Sub set_bb_depthlimit Lib "lpsolve55.dll" Alias "set_bb_depthlimit" (lp As Integer, bb_maxlevel As Integer)
        Public Declare Sub set_bb_floorfirst Lib "lpsolve55.dll" Alias "set_bb_floorfirst" (lp As Integer, bb_floorfirst As lpsolve_branch)
        Public Declare Sub set_bb_rule Lib "lpsolve55.dll" Alias "set_bb_rule" (lp As Integer, bb_rule As lpsolve_BBstrategies)
        Public Declare Function set_BFP Lib "lpsolve55.dll" Alias "set_BFP" (lp As Integer, filename As String) As Boolean
        Public Declare Function set_binary Lib "lpsolve55.dll" Alias "set_binary" (lp As Integer, column As Integer, must_be_bin As Boolean) As Boolean
        Public Declare Function set_bounds Lib "lpsolve55.dll" Alias "set_bounds" (lp As Integer, column As Integer, lower As Double, upper As Double) As Boolean
        Public Declare Sub set_bounds_tighter Lib "lpsolve55.dll" Alias "set_bounds_tighter" (lp As Integer, tighten As Boolean)
        Public Declare Sub set_break_at_first Lib "lpsolve55.dll" Alias "set_break_at_first" (lp As Integer, break_at_first As Boolean)
        Public Declare Sub set_break_at_value Lib "lpsolve55.dll" Alias "set_break_at_value" (lp As Integer, break_at_value As Double)
        Public Declare Function set_col_name Lib "lpsolve55.dll" Alias "set_col_name" (lp As Integer, column As Integer, new_name As String) As Boolean
        Public Declare Function set_column Lib "lpsolve55.dll" Alias "set_column" (lp As Integer, col_no As Integer, column() As Double) As Boolean
        Public Declare Function set_columnex Lib "lpsolve55.dll" Alias "set_columnex" (lp As Integer, col_no As Integer, count As Integer, column() As Double, rowno() As Integer) As Boolean
        Public Declare Function set_constr_type Lib "lpsolve55.dll" Alias "set_constr_type" (lp As Integer, row As Integer, con_type As lpsolve_constr_types) As Boolean
        Public Declare Sub set_debug Lib "lpsolve55.dll" Alias "set_debug" (lp As Integer, debug_ As Boolean)
        Public Declare Sub set_epsb Lib "lpsolve55.dll" Alias "set_epsb" (lp As Integer, epsb As Double)
        Public Declare Sub set_epsd Lib "lpsolve55.dll" Alias "set_epsd" (lp As Integer, epsd As Double)
        Public Declare Sub set_epsel Lib "lpsolve55.dll" Alias "set_epsel" (lp As Integer, epsel As Double)
        Public Declare Sub set_epsint Lib "lpsolve55.dll" Alias "set_epsint" (lp As Integer, epsint As Double)
        Public Declare Function set_epslevel Lib "lpsolve55.dll" Alias "set_epslevel" (lp As Integer, epslevel As Integer) As Boolean
        Public Declare Sub set_epsperturb Lib "lpsolve55.dll" Alias "set_epsperturb" (lp As Integer, epsperturb As Double)
        Public Declare Sub set_epspivot Lib "lpsolve55.dll" Alias "set_epspivot" (lp As Integer, epspivot As Double)
        Public Declare Sub set_improve Lib "lpsolve55.dll" Alias "set_improve" (lp As Integer, improve As lpsolve_improves)
        Public Declare Sub set_infinite Lib "lpsolve55.dll" Alias "set_infinite" (lp As Integer, infinite As Double)
        Public Declare Function set_int Lib "lpsolve55.dll" Alias "set_int" (lp As Integer, column As Integer, must_be_int As Boolean) As Boolean
        Public Declare Sub set_lag_trace Lib "lpsolve55.dll" Alias "set_lag_trace" (lp As Integer, lag_trace As Boolean)
        Public Declare Function set_lowbo Lib "lpsolve55.dll" Alias "set_lowbo" (lp As Integer, column As Integer, value As Double) As Boolean
        Public Declare Function set_lp_name Lib "lpsolve55.dll" Alias "set_lp_name" (lp As Integer, lpname As String) As Boolean
        Public Declare Function set_mat Lib "lpsolve55.dll" Alias "set_mat" (lp As Integer, row As Integer, column As Integer, value As Double) As Boolean
        Public Declare Sub set_maxim Lib "lpsolve55.dll" Alias "set_maxim" (lp As Integer)
        Public Declare Sub set_maxpivot Lib "lpsolve55.dll" Alias "set_maxpivot" (lp As Integer, max_num_inv As Integer)
        Public Declare Sub set_minim Lib "lpsolve55.dll" Alias "set_minim" (lp As Integer)
        Public Declare Sub set_mip_gap Lib "lpsolve55.dll" Alias "set_mip_gap" (lp As Integer, absolute As Boolean, mip_gap As Double)
        Public Declare Sub set_negrange Lib "lpsolve55.dll" Alias "set_negrange" (lp As Integer, negrange As Double)
        Public Declare Function set_obj Lib "lpsolve55.dll" Alias "set_obj" (lp As Integer, column As Integer, value As Double) As Boolean
        Public Declare Sub set_obj_bound Lib "lpsolve55.dll" Alias "set_obj_bound" (lp As Integer, obj_bound As Double)
        Public Declare Function set_obj_fn Lib "lpsolve55.dll" Alias "set_obj_fn" (lp As Integer, row() As Double) As Boolean
        Public Declare Function set_obj_fnex Lib "lpsolve55.dll" Alias "set_obj_fnex" (lp As Integer, count As Integer, row() As Double, colno() As Integer) As Boolean
        Public Declare Function set_outputfile Lib "lpsolve55.dll" Alias "set_outputfile" (lp As Integer, filename As String) As Boolean
        Public Declare Sub set_pivoting Lib "lpsolve55.dll" Alias "set_pivoting" (lp As Integer, piv_rule As lpsolve_piv_rules)
        Public Declare Sub set_preferdual Lib "lpsolve55.dll" Alias "set_preferdual" (lp As Integer, dodual As Boolean)
        Public Declare Sub set_presolve Lib "lpsolve55.dll" Alias "set_presolve" (lp As Integer, do_presolve As lpsolve_presolve, maxloops As Integer)
        Public Declare Sub set_print_sol Lib "lpsolve55.dll" Alias "set_print_sol" (lp As Integer, print_sol As Integer)
        Public Declare Function set_PseudoCosts Lib "lpsolve55.dll" Alias "set_PseudoCosts" (lp As Integer, clower() As Double, cupper() As Double, updatelimit() As Integer) As Boolean
        Public Declare Function set_rh Lib "lpsolve55.dll" Alias "set_rh" (lp As Integer, row As Integer, value As Double) As Boolean
        Public Declare Function set_rh_range Lib "lpsolve55.dll" Alias "set_rh_range" (lp As Integer, row As Integer, deltavalue As Double) As Boolean
        Public Declare Sub set_rh_vec Lib "lpsolve55.dll" Alias "set_rh_vec" (lp As Integer, rh() As Double)
        Public Declare Function set_row Lib "lpsolve55.dll" Alias "set_row" (lp As Integer, row_no As Integer, row() As Double) As Boolean
        Public Declare Function set_row_name Lib "lpsolve55.dll" Alias "set_row_name" (lp As Integer, row As Integer, new_name As String) As Boolean
        Public Declare Function set_rowex Lib "lpsolve55.dll" Alias "set_rowex" (lp As Integer, row_no As Integer, count As Integer, row() As Double, colno() As Integer) As Boolean
        Public Declare Sub set_scalelimit Lib "lpsolve55.dll" Alias "set_scalelimit" (lp As Integer, scalelimit As Double)
        Public Declare Sub set_scaling Lib "lpsolve55.dll" Alias "set_scaling" (lp As Integer, scalemode As lpsolve_scales)
        Public Declare Function set_semicont Lib "lpsolve55.dll" Alias "set_semicont" (lp As Integer, column As Integer, must_be_sc As Boolean) As Boolean
        Public Declare Sub set_sense Lib "lpsolve55.dll" Alias "set_sense" (lp As Integer, maximize As Boolean)
        Public Declare Sub set_simplextype Lib "lpsolve55.dll" Alias "set_simplextype" (lp As Integer, simplextype As lpsolve_simplextypes)
        Public Declare Sub set_solutionlimit Lib "lpsolve55.dll" Alias "set_solutionlimit" (lp As Integer, limit As Integer)
        Public Declare Sub set_timeout Lib "lpsolve55.dll" Alias "set_timeout" (lp As Integer, sectimeout As Integer)
        Public Declare Sub set_trace Lib "lpsolve55.dll" Alias "set_trace" (lp As Integer, trace As Boolean)
        Public Declare Function set_unbounded Lib "lpsolve55.dll" Alias "set_unbounded" (lp As Integer, column As Integer) As Boolean
        Public Declare Function set_upbo Lib "lpsolve55.dll" Alias "set_upbo" (lp As Integer, column As Integer, value As Double) As Boolean
        Public Declare Sub set_use_names Lib "lpsolve55.dll" Alias "set_use_names" (lp As Integer, isrow As Boolean, use_names As Boolean)
        Public Declare Function set_var_branch Lib "lpsolve55.dll" Alias "set_var_branch" (lp As Integer, column As Integer, branch_mode As lpsolve_branch) As Boolean
        Public Declare Function set_var_weights Lib "lpsolve55.dll" Alias "set_var_weights" (lp As Integer, weights() As Double) As Boolean
        Public Declare Sub set_verbose Lib "lpsolve55.dll" Alias "set_verbose" (lp As Integer, verbose As Integer)
        Public Declare Function set_XLI Lib "lpsolve55.dll" Alias "set_XLI" (lp As Integer, filename As String) As Boolean
        Public Declare Function solve Lib "lpsolve55.dll" Alias "solve" (lp As Integer) As lpsolve_return
        Public Declare Function str_add_column Lib "lpsolve55.dll" Alias "str_add_column" (lp As Integer, col_string As String) As Boolean
        Public Declare Function str_add_constraint Lib "lpsolve55.dll" Alias "str_add_constraint" (lp As Integer, row_string As String, constr_type As lpsolve_constr_types, rh As Double) As Boolean
        Public Declare Function str_add_lag_con Lib "lpsolve55.dll" Alias "str_add_lag_con" (lp As Integer, row_string As String, con_type As lpsolve_constr_types, rhs As Double) As Boolean
        Public Declare Function str_set_obj_fn Lib "lpsolve55.dll" Alias "str_set_obj_fn" (lp As Integer, row_string As String) As Boolean
        Public Declare Function str_set_rh_vec Lib "lpsolve55.dll" Alias "str_set_rh_vec" (lp As Integer, rh_string As String) As Boolean
        Public Declare Function time_elapsed Lib "lpsolve55.dll" Alias "time_elapsed" (lp As Integer) As Double
        Public Declare Sub unscale Lib "lpsolve55.dll" Alias "unscale" (lp As Integer)
        Public Declare Function write_basis Lib "lpsolve55.dll" Alias "write_basis" (lp As Integer, filename As String) As Boolean
        Public Declare Function write_freemps Lib "lpsolve55.dll" Alias "write_freemps" (lp As Integer, filename As String) As Boolean
        Public Declare Function write_lp Lib "lpsolve55.dll" Alias "write_lp" (lp As Integer, filename As String) As Boolean
        Public Declare Function write_mps Lib "lpsolve55.dll" Alias "write_mps" (lp As Integer, filename As String) As Boolean
        Public Declare Function write_XLI Lib "lpsolve55.dll" Alias "write_XLI" (lp As Integer, filename As String, options As String, results As Boolean) As Boolean
        Public Declare Function write_params Lib "lpsolve55.dll" Alias "write_params" (lp As Integer, filename As String, options As String) As Boolean

        '-----------------------------------------------------------------------------------------------------------------------------

        'possible type of constraints
        Public Enum lpsolve_constr_types
            LE = 1
            EQ = 3
            GE = 2
            FR = 0
        End Enum

        'Possible Scalings
        Public Enum lpsolve_scales
            SCALE_EXTREME = 1
            SCALE_RANGE = 2
            SCALE_MEAN = 3
            SCALE_GEOMETRIC = 4
            SCALE_CURTISREID = 7
            SCALE_QUADRATIC = 8
            SCALE_LOGARITHMIC = 16
            SCALE_USERWEIGHT = 31
            SCALE_POWER2 = 32
            SCALE_EQUILIBRATE = 64
            SCALE_INTEGERS = 128
        End Enum

        'Possible Improvements
        Public Enum lpsolve_improves
            IMPROVE_NONE = 0
            IMPROVE_SOLUTION = 1
            IMPROVE_DUALFEAS = 2
            IMPROVE_THETAGAP = 4
            IMPROVE_BBSIMPLEX = 8
            IMPROVE_DEFAULT = (IMPROVE_DUALFEAS + IMPROVE_THETAGAP)
            IMPROVE_INVERSE = (IMPROVE_SOLUTION + IMPROVE_THETAGAP)
        End Enum

        Public Enum lpsolve_piv_rules
            PRICER_FIRSTINDEX = 0
            PRICER_DANTZIG = 1
            PRICER_DEVEX = 2
            PRICER_STEEPESTEDGE = 3
            PRICE_PRIMALFALLBACK = 4
            PRICE_MULTIPLE = 8
            PRICE_PARTIAL = 16
            PRICE_ADAPTIVE = 32
            PRICE_HYBRID = 64
            PRICE_RANDOMIZE = 128
            PRICE_AUTOPARTIALCOLS = 256
            PRICE_AUTOPARTIALROWS = 512
            PRICE_LOOPLEFT = 1024
            PRICE_LOOPALTERNATE = 2048
            PRICE_AUTOPARTIAL = lpsolve_piv_rules.PRICE_AUTOPARTIALCOLS + lpsolve_piv_rules.PRICE_AUTOPARTIALROWS
        End Enum

        Public Enum lpsolve_presolve
            PRESOLVE_NONE = 0
            PRESOLVE_ROWS = 1
            PRESOLVE_COLS = 2
            PRESOLVE_LINDEP = 4
            PRESOLVE_SOS = 32
            PRESOLVE_REDUCEMIP = 64
            PRESOLVE_KNAPSACK = 128
            PRESOLVE_ELIMEQ2 = 256
            PRESOLVE_IMPLIEDFREE = 512
            PRESOLVE_REDUCEGCD = 1024
            PRESOLVE_PROBEFIX = 2048
            PRESOLVE_PROBEREDUCE = 4096
            PRESOLVE_ROWDOMINATE = 8192
            PRESOLVE_COLDOMINATE = 16384
            PRESOLVE_MERGEROWS = 32768
            PRESOLVE_IMPLIEDSLK = 65536
            PRESOLVE_COLFIXDUAL = 131072
            PRESOLVE_BOUNDS = 262144
            PRESOLVE_DUALS = 524288
            PRESOLVE_SENSDUALS = 1048576
        End Enum

        Public Enum lpsolve_anti_degen
            ANTIDEGEN_NONE = 0
            ANTIDEGEN_FIXEDVARS = 1
            ANTIDEGEN_COLUMNCHECK = 2
            ANTIDEGEN_STALLING = 4
            ANTIDEGEN_NUMFAILURE = 8
            ANTIDEGEN_LOSTFEAS = 16
            ANTIDEGEN_INFEASIBLE = 32
            ANTIDEGEN_DYNAMIC = 64
            ANTIDEGEN_DURINGBB = 128
            ANTIDEGEN_RHSPERTURB = 256
            ANTIDEGEN_BOUNDFLIP = 512
        End Enum

        Public Enum lpsolve_basiscrash
            CRASH_NOTHING = 0
            CRASH_MOSTFEASIBLE = 2
        End Enum

        Public Enum lpsolve_simplextypes
            SIMPLEX_PRIMAL_PRIMAL = 5
            SIMPLEX_DUAL_PRIMAL = 6
            SIMPLEX_PRIMAL_DUAL = 9
            SIMPLEX_DUAL_DUAL = 10
        End Enum

        'B&B strategies
        Public Enum lpsolve_BBstrategies
            NODE_FIRSTSELECT = 0
            NODE_GAPSELECT = 1
            NODE_RANGESELECT = 2
            NODE_FRACTIONSELECT = 3
            NODE_PSEUDOCOSTSELECT = 4
            NODE_PSEUDONONINTSELECT = 5
            NODE_PSEUDORATIOSELECT = 6
            NODE_USERSELECT = 7
            NODE_WEIGHTREVERSEMODE = 8
            NODE_BRANCHREVERSEMODE = 16
            NODE_GREEDYMODE = 32
            NODE_PSEUDOCOSTMODE = 64
            NODE_DEPTHFIRSTMODE = 128
            NODE_RANDOMIZEMODE = 256
            NODE_GUBMODE = 512
            NODE_DYNAMICMODE = 1024
            NODE_RESTARTMODE = 2048
            NODE_BREADTHFIRSTMODE = 4096
            NODE_AUTOORDER = 8192
            NODE_RCOSTFIXING = 16384
            NODE_STRONGINIT = 32768
        End Enum

        'possible return values of lp solver
        Public Enum lpsolve_return
            NOMEMORY = -2
            OPTIMAL = 0
            SUBOPTIMAL = 1
            INFEASIBLE = 2
            UNBOUNDED = 3
            DEGENERATE = 4
            NUMFAILURE = 5
            USERABORT = 6
            TIMEOUT = 7
            PRESOLVED = 9
            PROCFAIL = 10
            PROCBREAK = 11
            FEASFOUND = 12
            NOFEASFOUND = 13
        End Enum

        'possible branch values
        Public Enum lpsolve_branch
            BRANCH_CEILING = 0
            BRANCH_FLOOR = 1
            BRANCH_AUTOMATIC = 2
            BRANCH_DEFAULT = 3
        End Enum

        'possible message values
        Public Enum lpsolve_msgmask
            MSG_PRESOLVE = 1
            MSG_LPFEASIBLE = 8
            MSG_LPOPTIMAL = 16
            MSG_MILPEQUAL = 32
            MSG_MILPFEASIBLE = 128
            MSG_MILPBETTER = 512
        End Enum

    End Class
    Private Class cDef
        Public m_key As Object
        Public m_ord As Integer
        Public m_dMin As Double
        Public m_dMax As Double
        Public m_dResult As Double
        Public m_DualValue As Double
        Public Sub New(key As Object, ord As Integer)
            Me.m_key = key
            Me.m_ord = ord
        End Sub
    End Class

    Private Class cVarDef
        Inherits cDef
        Public Sub New(key As Object, ord As Integer)
            MyBase.New(key, ord)
        End Sub
    End Class

    Private Class cRowDef
        Inherits cDef
        Public m_dVals As New Dictionary(Of Object, Double)
        Public Sub New(key As Object, ord As Integer)
            MyBase.New(key, ord)
        End Sub
    End Class

#End Region ' Private classes

#Region " Private vars "

    Private m_lDefs As New List(Of cDef)
    Private m_iGoal As Integer = -1
    Private m_bMinimize As Boolean = False

#End Region ' Private vars

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        Me.m_lDefs.Add(Nothing) ' One-based index
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.AddVariable"/>
    ''' -----------------------------------------------------------------------
    Public Function AddVariable(key As Object, ByRef iIndex As Integer) As Boolean _
        Implements ILPSolver.AddVariable
        iIndex = Me.m_lDefs.Count
        Me.m_lDefs.Add(New cVarDef(key, iIndex))
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.SetBounds"/>
    ''' -----------------------------------------------------------------------
    Public Sub SetBounds(iVar As Integer, dMin As Double, dMax As Double) _
          Implements ILPSolver.SetBounds
        Dim vd As cDef = Me.m_lDefs(iVar)
        vd.m_dMin = dMin
        vd.m_dMax = dMax
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.AddRow"/>
    ''' -----------------------------------------------------------------------
    Public Function AddRow(key As Object, ByRef iIndex As Integer) As Boolean _
          Implements ILPSolver.AddRow
        iIndex = Me.m_lDefs.Count
        Me.m_lDefs.Add(New cRowDef(key, iIndex))
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.AddVariable"/>
    ''' -----------------------------------------------------------------------
    Public Function AddGoal(iRow As Integer, ip As Integer, bMinimize As Boolean) As Boolean _
         Implements ILPSolver.AddGoal
        ' ip (priority) is ignored
        Me.m_iGoal = iRow
        Me.m_bMinimize = bMinimize
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.SetCoefficient"/>
    ''' -----------------------------------------------------------------------
    Public Sub SetCoefficient(iRow As Integer, iVar As Integer, dVal As Double) _
         Implements ILPSolver.SetCoefficient
        Dim rd As cRowDef = DirectCast(Me.m_lDefs(iRow), cRowDef)
        Dim vd As cVarDef = DirectCast(Me.m_lDefs(iVar), cVarDef)
        rd.m_dVals(vd.m_key) = dVal
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.Solve"/>
    ''' <remarks>
    ''' This method creates the unmanaged solver, populates and runs it, extracts
    ''' results and destroys the unmanaged solver.
    ''' </remarks>
    ''' <returns>
    ''' True if ran successful. Remember to check whether this particular solver 
    ''' <see cref="IsSupported">is supported by the operating system</see>.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function Solve(iTimeStepIndex As Integer) As EwEUtils.Core.eSolverReturnValues _
          Implements ILPSolver.Solve
        Dim rv As eSolverReturnValues

        Debug.Assert(Me.m_iGoal > 0, "Goal not defined")

        ' Safety check
        If Not Me.IsSupported Then
            Debug.Assert(False, "lpsolve55 did not initialize")
            Return eSolverReturnValues.ERROR
        End If

        Dim vars() As cVarDef = Me.Vars
        Dim rows() As cRowDef = Me.Rows
        Dim lp As Integer = 0

        Try
            lp = lpsolve55.make_lp(0, vars.Length)
        Catch ex As Exception
            cLog.Write(ex, "cLPSolver.Solve() Failed on make_lp(,)")
            Return eSolverReturnValues.ERROR
        End Try

        Try

            For v As Integer = 0 To vars.Length - 1
                Dim vd As cVarDef = vars(v)
                lpsolve55.set_bounds(lp, vd.m_ord, vd.m_dMin, vd.m_dMax)
                lpsolve55.set_col_name(lp, vd.m_ord, vd.m_key.ToString())
            Next

            For r As Integer = 0 To rows.Length - 1
                Dim dRow(vars.Length) As Double
                Dim rd As cRowDef = rows(r)
                For v As Integer = 0 To vars.Length - 1
                    Dim vd As cVarDef = vars(v)
                    If rd.m_dVals.ContainsKey(vd.m_key) Then
                        dRow(v + 1) = rd.m_dVals(vd.m_key)
                    End If
                Next v
                Dim bAdded As Boolean

                'only add a lower constraint if it is not equal to zero
                If rd.m_dMin <> 0 Then
                    bAdded = lpsolve55.add_constraint(lp, dRow, lpsolve55.lpsolve_constr_types.GE, rd.m_dMin)
                End If

                'Always add the upper constraint! I think LPSolve will ignore constraints that are zero! Maybe...
                lpsolve55.add_constraint(lp, dRow, lpsolve55.lpsolve_constr_types.LE, rd.m_dMax)

                lpsolve55.set_row_name(lp, rd.m_ord, rd.m_key.ToString())
            Next r

            If True Then
                Dim dRow(vars.Length) As Double
                Dim rd As cRowDef = Me.Goal()
                For v As Integer = 0 To vars.Length - 1
                    Dim vd As cVarDef = vars(v)
                    If rd.m_dVals.ContainsKey(vd.m_key) Then
                        dRow(v + 1) = rd.m_dVals(vd.m_key)
                    End If
                Next v
                lpsolve55.set_obj_fn(lp, dRow)
                If Me.m_bMinimize Then
                    lpsolve55.set_minim(lp)
                Else
                    lpsolve55.set_maxim(lp)
                End If
            End If


            Dim lpResult As lpsolve55.lpsolve_return
            lpResult = lpsolve55.solve(lp)

            'this works because there is a one to one mapping for lpsolve55.lpsolve_return and eSolverReturnValues
            rv = CType(lpResult, eSolverReturnValues)

            If rv <> eSolverReturnValues.OPTIMAL Then

#If DEBUG Then
                'Need to find a better way to do this
                Dim tmpPath As String = System.IO.Path.GetTempPath
                Dim solverFile As String = System.IO.Path.Combine(tmpPath, "EWE6_LPSolve_model_" & iTimeStepIndex.ToString & ".txt")
                System.Console.WriteLine("cLPSolver.Solve() Non Optimal Solution: " & lpResult.ToString & " Timestep " & iTimeStepIndex.ToString & " file saved to ")
                System.Console.WriteLine(solverFile)
                lpsolve55.write_lp(lp, solverFile)
#End If
            End If

            ' This looks incredibly fragile...
            Dim n As Integer = 1 + Me.Vars.Length + Me.Rows.Length
            Debug.Assert(n = 1 + lpsolve55.get_Ncolumns(lp) + lpsolve55.get_Nrows(lp), "cLPSolver number of variables and rows does not match.")

            Dim dualValues(n) As Double
            Dim dSol(n) As Double

            Dim iSol As Integer = 0
            lpsolve55.get_primal_solution(lp, dSol)
            lpsolve55.get_dual_solution(lp, dualValues)
            Me.Goal.m_dResult = dSol(iSol)
            iSol += 1

            For iRow As Integer = 0 To rows.Length - 1
                rows(iRow).m_dResult = dSol(iSol)
                rows(iRow).m_DualValue = dualValues(iSol)
                iSol += 1
            Next

            For iVar As Integer = 0 To vars.Length - 1
                vars(iVar).m_dResult = dSol(iSol)
                vars(iVar).m_DualValue = dualValues(iSol)
                iSol += 1
            Next

            '  lpsolve55.write_lp(lp, "cLPSolver.txt")

        Catch ex As Exception
            rv = eSolverReturnValues.ERROR
        End Try

        'lpsolve55.write_lp(lp, "cLPSolver.txt")

        lpsolve55.delete_lp(lp)

        Return rv

    End Function



    Public Sub SolveLPSolve()

        'SimplexSolver solver = new SimplexSolver();
        Dim lp As Integer = lpsolve55.make_lp(0, 2)

        ' - Vars already defined in constructor
        'int savid, vzvid;
        'solver.AddVariable("Saudi Arabia", out savid);
        'solver.SetBounds(savid, 0, 9000);
        lpsolve55.set_bounds(lp, 1, 0, 9000)
        'solver.AddVariable("Venezuela", out vzvid);
        'solver.SetBounds(vzvid, 0, 6000);
        lpsolve55.set_bounds(lp, 2, 0, 6000)

        'int gasoline, jetfuel, machinelubricant, cost;
        Dim drow As Double()

        'solver.AddRow("gasoline", out gasoline);
        'solver.SetCoefficient(gasoline, savid, 0.3);
        'solver.SetCoefficient(gasoline, vzvid, 0.4);
        'solver.SetBounds(gasoline, 2000, Rational.PositiveInfinity);
        drow = New Double() {0, 0.3, 0.4}
        lpsolve55.add_constraint(lp, drow, lpsolve55.lpsolve_constr_types.GE, 2000)

        'solver.AddRow("jetfuel", out jetfuel);
        'solver.SetCoefficient(jetfuel, savid, 0.4);
        'solver.SetCoefficient(jetfuel, vzvid, 0.2);
        'solver.SetBounds(jetfuel, 1500, Rational.PositiveInfinity);
        drow = New Double() {0, 0.4, 0.2}
        lpsolve55.add_constraint(lp, drow, lpsolve55.lpsolve_constr_types.GE, 1500)

        'solver.AddRow("machinelubricant", out machinelubricant);
        'solver.SetCoefficient(machinelubricant, savid, 0.2);
        'solver.SetCoefficient(machinelubricant, vzvid, 0.3);
        'solver.SetBounds(machinelubricant, 500, Rational.PositiveInfinity);
        drow = New Double() {0, 0.2, 0.3}
        lpsolve55.add_constraint(lp, drow, lpsolve55.lpsolve_constr_types.GE, 500)

        'solver.AddRow("cost", out cost);
        'solver.SetCoefficient(cost, savid, 20);
        'solver.SetCoefficient(cost, vzvid, 15);
        'solver.AddGoal(cost, 1, true);
        drow = New Double() {0, 20, 15}
        lpsolve55.set_obj_fn(lp, drow)

        'solver.Solve(new SimplexSolverParams());
        lpsolve55.set_minim(lp)

        'lpsolve55.print_lp(lp)
        lpsolve55.solve(lp)

        'Console.WriteLine("SA {0}, VZ {1}, Gasoline {2}, Jet Fuel {3}, Machine Lubricant {4}, Cost {5}",
        '    solver.GetValue(savid).ToDouble(),
        '    solver.GetValue(vzvid).ToDouble(),
        '    solver.GetValue(gasoline).ToDouble(),
        '    solver.GetValue(jetfuel).ToDouble(),
        '    solver.GetValue(machinelubricant).ToDouble(),
        '    solver.GetValue(cost).ToDouble());

        'lpsolve55.print_objective(lp)
        'lpsolve55.print_solution(lp, 1)
        'lpsolve55.print_constraints(lp, 1)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.GetValue"/>
    ''' -----------------------------------------------------------------------
    Public Function GetValue(iData As Integer) As Double _
          Implements ILPSolver.GetValue
        Return Me.m_lDefs(iData).m_dResult
    End Function


    Public Function GetDualValue(iData As Integer) As Double Implements EwEUtils.Core.ILPSolver.GetDualValue
        Return Me.m_lDefs(iData).m_DualValue
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.IsSupported"/>
    ''' -----------------------------------------------------------------------
    Public Function IsSupported() As Boolean Implements EwEUtils.Core.ILPSolver.IsSupported
        lpsolve55.Init()
        Return lpsolve55.IsUsable()
    End Function

#End Region ' Public access

#Region " Internals "

    Private Function Vars() As cVarDef()
        Dim lvars As New List(Of cVarDef)
        For Each def As cDef In Me.m_lDefs
            If TypeOf def Is cVarDef Then
                Dim vd As cVarDef = DirectCast(def, cVarDef)
                lvars.Add(vd)
            End If
        Next
        Return lvars.ToArray
    End Function

    Private Function Rows() As cRowDef()
        Dim lrows As New List(Of cRowDef)
        For Each def As cDef In Me.m_lDefs
            If TypeOf def Is cRowDef Then
                Dim rd As cRowDef = DirectCast(def, cRowDef)
                If rd.m_ord <> Me.m_iGoal Then
                    lrows.Add(rd)
                End If
            End If
        Next
        Return lrows.ToArray()
    End Function

    Private Function Goal() As cRowDef
        For Each def As cDef In Me.m_lDefs
            If TypeOf def Is cRowDef Then
                Dim rd As cRowDef = DirectCast(def, cRowDef)
                If rd.m_ord = Me.m_iGoal Then Return rd
            End If
        Next
        Return Nothing
    End Function

#End Region ' Internals

    
End Class
