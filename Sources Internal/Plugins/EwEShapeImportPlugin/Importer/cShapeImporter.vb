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
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls.Shapes

    Public Class cShapeImporter

        Private m_core As cCore
        Private m_data As cShapeImportData

        Public Sub New(core As cCore, data As cShapeImportData)
            Me.m_core = core
            Me.m_data = data
        End Sub

        Public Function Import() As Boolean

            Dim man As cBaseShapeManager = Me.GetManager()
            Dim defs As cShapeImportData.cFunctionDefinition() = Me.m_data.FunctionDefinitions
            Dim msgStatus As cMessage = Nothing
            Dim fmt As New cDataTypeFormatter()
            Dim bSuccess As Boolean = True

            If (man IsNot Nothing) Then
                Try
                    Dim strMessage As String = cStringUtils.Localize(My.Resources.STATUS_IMPORTED, fmt.GetDescriptor(Me.m_data.DataType))

                    msgStatus = New cMessage(cStringUtils.ToSentenceCase(strMessage), eMessageType.DataImport, eCoreComponentType.EcoSim, eMessageImportance.Information)
                    Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure)
                    For Each def As cShapeImportData.cFunctionDefinition In defs
                        Dim ff As cForcingFunction = man.CreateNewShape(def.Name, def.ShapeFunction.Shape(1200), def.Parms(1), def.Parms(2), def.Parms(3), def.Parms(4), def.ShapeFunction.ShapeFunctionType)
                        Dim vs As cVariableStatus = Nothing
                        If (ff IsNot Nothing) Then
                            vs = New cVariableStatus(eStatusFlags.OK, _
                                                     cStringUtils.Localize(My.Resources.STATUS_IMPORT_DETAIL_SUCCESS, def.Name), _
                                                     eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.External, 0)
                        Else
                            bSuccess = False
                            vs = New cVariableStatus(eStatusFlags.ErrorEncountered, _
                                                     cStringUtils.Localize(My.Resources.STATUS_IMPORT_DETAIL_FAILED, def.Name), _
                                                     eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.External, 0)
                        End If
                        msgStatus.AddVariable(vs)
                    Next
                Catch ex As Exception

                End Try
                Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim, bSuccess)
            Else
                msgStatus = New cMessage(cStringUtils.Localize(My.Resources.STATUS_UNKNOWN_TARGET, fmt.GetDescriptor(Me.m_data.DataType)), _
                                         eMessageType.DataImport, eCoreComponentType.EcoSim, eMessageImportance.Warning)
                bSuccess = False
            End If

            Me.m_core.Messages.SendMessage(msgStatus)

            Return bSuccess
        End Function

        Private Function GetManager() As cBaseShapeManager
            Select Case Me.m_data.DataType
                Case eDataTypes.CapacityMediation
                    Return Me.m_core.CapacityShapeManager
                Case eDataTypes.Forcing
                    Return Me.m_core.ForcingShapeManager
                Case Else
                    Debug.Assert(False, "Unsupported shape type " & Me.m_data.DataType)
            End Select
            Return Nothing
        End Function

    End Class

End Namespace
