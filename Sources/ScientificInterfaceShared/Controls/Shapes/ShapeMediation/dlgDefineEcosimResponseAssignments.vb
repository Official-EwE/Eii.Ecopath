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

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports ZedGraph
Imports ScientificInterfaceShared.Commands
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' Main interface to define the functional responses of groups to environmental drivers.
''' </summary>
Public Class dlgDefineEcosimResponseAssignments
    Inherits dlgDefineMapResponseAssignments

    Sub New(ByVal uic As cUIContext, _
                   ByVal shape As EwECore.cEnviroResponseFunction, _
                   ByVal manager As EwECore.IEnvironmentalResponseManager)
        MyBase.New(uic, shape, manager)

    End Sub

#Region " Overrides "

    Protected Overrides Function GetGroupList() As Integer()
        Dim lstGroups As New List(Of Integer)
        For iGrp As Integer = 1 To Me.m_uic.Core.nLivingGroups
            lstGroups.Add(iGrp)
        Next
        Return lstGroups.ToArray()
    End Function

    Protected Overrides Sub LoadDrivers()

        Dim data As IEnviroInputData = Nothing
        Dim fmt As New cCoreInterfaceFormatter()

        Try
            Me.m_tvDrivers.Nodes.Clear()

            For iDriver As Integer = 1 To Me.m_manager.nEnviroData

                data = Me.m_manager.EnviroData(iDriver)
                'Dim ndApply As TreeNode = Me.m_tvMaps.Nodes.Add(fmt.GetDescriptor(DirectCast(map, cEnviroInputMap).Layer))
                Dim ndApply As TreeNode = Me.m_tvDrivers.Nodes.Add(data.Name)
                ndApply.Tag = data

                For igrp As Integer = 1 To Me.m_uic.Core.nGroups
                    'Is the current shape selected as the response function for any group
                    If Me.m_shape.Index = data.ResponseIndexForGroup(igrp) Then
                        'Yes this shape is set for this group
                        'add a group node
                        Dim grp As cEcoSimGroupInput = Me.m_uic.Core.EcoSimGroupInputs(igrp)

                        Dim ndgrp As TreeNode = ndApply.Nodes.Add(fmt.GetDescriptor(grp))
                        ndgrp.Tag = grp

                        If Not ndApply.IsExpanded Then
                            'if there are groups assigned to this Map/Node then expand it the tree to this point
                            ndApply.ExpandAll()
                        End If
                    End If

                Next
            Next

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".loadMaps() Exception: " & ex.Message)
        End Try

    End Sub

#End Region ' Overrides

End Class

