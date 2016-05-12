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
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls

    Public Class cEcoismEnviroResponseShapeGUIHandler
        Inherits cCapacityShapeGUIHandler

        Public Sub New(uic As cUIContext)
            MyBase.New(uic)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cShapeGUIHandler.ExecuteCommand"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub ExecuteCommand(ByVal cmd As ScientificInterfaceShared.Controls.cShapeGUIHandler.eShapeCommandTypes, _
                                          Optional ByVal ashapes() As EwECore.cShapeData = Nothing, _
                                          Optional ByVal data As Object = Nothing)

            Try
                Select Case cmd

                    Case eShapeCommandTypes.DefineMediation
                        Debug.Assert((TypeOf Me.SelectedShape Is EwECore.cEnviroResponseFunction), "OPPSSS...")
                        Dim dlgDefBP As New dlgDefineEcosimResponseAssignments(Me.UIContext, DirectCast(Me.SelectedShape, EwECore.cEnviroResponseFunction), UIContext.Core.EcosimEnviroResponseManager)
                        If dlgDefBP.ShowDialog() = Windows.Forms.DialogResult.OK Then
                            Me.MediationAssignments.RefreshContent()
                        End If

                    Case eShapeCommandTypes.Import
                        Dim dlg As New dlgImportShapes(Me.UIContext, Me.ShapeManager)
                        If dlg.ShowDialog() = DialogResult.OK Then
                            Me.MediationAssignments.RefreshContent()
                        End If

                    Case Else
                        MyBase.ExecuteCommand(cmd, ashapes, data)

                End Select
            Catch ex As Exception

            End Try

        End Sub

    End Class


End Namespace
