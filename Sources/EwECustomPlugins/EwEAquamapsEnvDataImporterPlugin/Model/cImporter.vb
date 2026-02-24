' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Common
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

''' <summary>
''' Performs the actual import from <see cref="cImportData"/> to <see cref="cEnviroResponseFunction"/>.
''' </summary>
Public Class cImporter

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_core As cCore = Nothing
    Private m_data As cImportData = Nothing

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
        Dim msg As New cMessage(My.Resources.PROMPT_IMPORT_GENERIC, eMessageType.DataImport, eCoreComponentType.Ecospace, eMessageImportance.Information)
        Dim vs As cVariableStatus = Nothing

        If Not Me.m_core.SaveChanges(False) Then Return bSuccess

        cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
        Try

            bSuccess = True

            cApplicationStatusNotifier.UpdateProgress(Me.m_uic.Core, My.Resources.STATUS_CREATING, -1)
            Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure)

            For Each file As cImportData.cFileData In Me.m_data.Files
                If (Array.IndexOf(species, file.Species) >= 0) Then
                    For Each env As cImportData.cEnvelopeData In file.Envelopes
                        If (Array.IndexOf(drivers, env.Name) >= 0) Then
                            Try
                                Dim strName As String = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, env.Name, file.Species)
                                cApplicationStatusNotifier.UpdateProgress(Me.m_uic.Core,
                                                                          cStringUtils.Localize(My.Resources.STATUS_CREATING_DETAIL, strName),
                                                                          -1)
                                If Me.CreateShape(strName, env) Then
                                    vs = New cVariableStatus(eStatusFlags.OK, cStringUtils.Localize(My.Resources.PROMPT_IMPORT_DETAIL_SUCCESS, strName),
                                                             eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
                                Else
                                    vs = New cVariableStatus(eStatusFlags.ErrorEncountered, String.Format(My.Resources.PROMPT_IMPORT_DETAIL_FAILED, strName),
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
            For Each file As cImportData.cFileData In Me.m_data.Files
                If (Array.IndexOf(species, file.Species) >= 0) Then
                    For Each env As cImportData.cEnvelopeData In file.Envelopes
                        If (Array.IndexOf(drivers, env.Name) >= 0) Then
                            Try
                                Dim strName As String = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, env.Name, file.Species)
                                cApplicationStatusNotifier.UpdateProgress(Me.m_uic.Core,
                                                                          cStringUtils.Localize(My.Resources.STATUS_CONFIGURING, strName),
                                                                          -1)
                                If Me.ConfigureShape(env) Then
                                    vs = New cVariableStatus(eStatusFlags.OK, cStringUtils.Localize(My.Resources.PROMPT_IMPORT_DETAIL_SUCCESS, strName),
                                                             eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
                                Else
                                    vs = New cVariableStatus(eStatusFlags.ErrorEncountered, String.Format(My.Resources.PROMPT_IMPORT_DETAIL_FAILED, strName),
                                                             eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
                                    bSuccess = False
                                End If
                                msg.AddVariable(vs)
                            Catch ex As Exception
                                bSuccess = False
                            End Try
                        End If
                    Next
                End If
            Next
            Me.Commit()

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

        Dim man As cEnviroResponseShapeManager = Me.m_core.EnviroResponseShapeManager
        Dim shp As cEnviroResponseFunction = Nothing

        shp = DirectCast(man.CreateNewShape(strName, env.Shape(cMediationDataStructures.N_DEFAULT_MEDIATIONPOINTS), eShapeFunctionType.Trapezoid, env.Parameters), cEnviroResponseFunction)
        env.DBID = shp.DBID

        Return True

    End Function

    Private Function ConfigureShape(env As cImportData.cEnvelopeData) As Boolean

        Dim man As cEnviroResponseShapeManager = Me.m_core.EnviroResponseShapeManager
        Dim shp As cEnviroResponseFunction = Nothing

        For Each shp In man
            If shp.DBID = env.DBID Then
                shp.LockUpdates()
                shp.ResponseLeftLimit = env.LeftBottom
                shp.ResponseRightLimit = env.RightBottom
                shp.UnlockUpdates()
                Return True
            End If
        Next

        Return False

    End Function

    Private Function Commit() As Boolean

        Return Me.m_core.EnviroResponseShapeManager.Update() And Me.m_core.SaveChanges(True)

    End Function

#End Region ' Internals

End Class
