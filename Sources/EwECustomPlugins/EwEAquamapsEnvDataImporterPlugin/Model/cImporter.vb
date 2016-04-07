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
' Copyright 1991- Ecopath International Initiative, Barcelona, Spain and
'                 Joint Reseach Centre, Ispra, Italy.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' <summary>
''' Performs the actual import from <see cref="cImportData"/> to <see cref="cEnviroResponseFunction"/>.
''' </summary>
Public Class cImporter

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_core As cCore = Nothing
    Private m_data As cImportData = Nothing
    Private m_dictShapeEnvelopes As New Dictionary(Of Integer, cImportData.cEnvelopeData)

    ' ToDo: obtain from EwE
    Private Const NMedPoints As Integer = 1200

#End Region ' Private vars

#Region " Construction "

    Public Sub New(data As cImportData, uic As cUIContext)

        Me.m_data = data
        Me.m_uic = uic
        Me.m_core = uic.Core

    End Sub

#End Region ' Construction

#Region " Public bits "

    Public Function Import(species As String(), drivers As String()) As Boolean

        Dim bSuccess As Boolean = False
        Dim msg As New cMessage(My.Resources.PROMPT_IMPORT_GENERIC, eMessageType.DataImport, eCoreComponentType.EcoSpace, eMessageImportance.Information)
        Dim vs As cVariableStatus = Nothing

        If Not Me.m_core.SaveChanges(False) Then Return bSuccess

        cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
        Try

            Me.m_dictShapeEnvelopes.Clear()
            bSuccess = True

            cApplicationStatusNotifier.UpdateProgress(Me.m_uic.Core, My.Resources.STATUS_CREATING, -1)
            Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure)

            For Each file As cImportData.cFileData In Me.m_data.Files
                If (Array.IndexOf(species, file.Species) >= 0) Then
                    For Each env As cImportData.cEnvelopeData In file.Envelopes
                        If (Array.IndexOf(drivers, env.Name) >= 0) Then
                            Try
                                Dim strName As String = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, env.Name, file.Species)
                                cApplicationStatusNotifier.UpdateProgress(Me.m_uic.Core, _
                                                                          String.Format(My.Resources.STATUS_CREATING_DETAIL, strName), _
                                                                          -1)
                                If Me.CreateShape(strName, env) Then
                                    Me.m_dictShapeEnvelopes(env.DBID) = env
                                    vs = New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.PROMPT_IMPORT_DETAIL_SUCCESS, strName), _
                                                             eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
                                Else
                                    vs = New cVariableStatus(eStatusFlags.ErrorEncountered, String.Format(My.Resources.PROMPT_IMPORT_DETAIL_FAILED, strName), _
                                                             eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
                                End If
                                msg.AddVariable(vs)
                            Catch ex As Exception
                                bSuccess = False
                            End Try
                        End If
                    Next
                End If
            Next
            ' Cap shapes stored with SIM
            Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim)

            cApplicationStatusNotifier.UpdateProgress(Me.m_uic.Core, My.Resources.STATUS_CONFIGURING, -1)
            Me.m_core.SetBatchLock(cCore.eBatchLockType.Update)
            ' Set X axis range
            Me.UpdateShapes()
            ' Cap shapes stored with SIM
            Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim)

            cApplicationStatusNotifier.UpdateProgress(Me.m_uic.Core, My.Resources.STATUS_SAVING, -1)
            bSuccess = bSuccess And Me.m_core.SaveChanges(True)
        Catch ex As Exception

        End Try

        Me.m_core.Messages.SendMessage(msg)
        cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

        Return bSuccess

    End Function

#End Region ' Public bits

#Region " Internals "

    Private Function CreateShape(strName As String, env As cImportData.cEnvelopeData) As Boolean

        Dim man As cCapMapResponseManager = Me.m_core.CapacityShapeManager
        Dim shp As cForcingFunction = Nothing
        Dim data(NMedPoints) As Single
        Dim p0 As Integer = 0
        Dim p1 As Single = NMedPoints * env.MinPref / env.Max
        Dim p2 As Single = NMedPoints * env.MaxPref / env.Max
        Dim p3 = NMedPoints

        For i As Integer = 1 To NMedPoints

            Dim x0 As Single = 0
            Dim y0 As Single = 0
            Dim rc As Single = 1.0

            If (i < p1) Then
                x0 = p0
                y0 = 0
                rc = cSystemUtils.IIF(p1 = 0, 1.0!, CSng(1 / p1))
            ElseIf (i < p2) Then
                x0 = p1
                y0 = 1
                rc = 0
            ElseIf (i < p3) Then
                x0 = p2
                y0 = 1
                rc = cSystemUtils.IIF((p3 - p2) = 0, -1, -1 / (p3 - p2))
            Else
                ' Past max: use 0
                y0 = 0
                rc = 0
            End If

            data(i) = y0 + CSng((i - x0) * rc)
        Next

        shp = man.CreateNewShape(strName, data)
        env.DBID = shp.DBID

        Return True

    End Function

    Private Function UpdateShapes() As Boolean

        Dim man As cCapMapResponseManager = Me.m_core.CapacityShapeManager
        For i As Integer = 0 To man.Count - 1
            Dim shp As cEnviroResponseFunction = CType(man.Item(i), cEnviroResponseFunction)
            Dim env As cImportData.cEnvelopeData = Nothing

            If Me.m_dictShapeEnvelopes.ContainsKey(shp.DBID) Then
                env = Me.m_dictShapeEnvelopes(shp.DBID)
                shp.ResponseLeftLimit = env.Min
                shp.ResponseRightLimit = env.Max
            End If
        Next
        man.Update()

        Return True

    End Function

#End Region ' Internals

End Class
