' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

Namespace Controls

    Public Class cEnvironmentalResponseShapeGUIHandler
        Inherits cCapacityShapeGUIHandler

        Public Sub New(uic As cUIContext)
            MyBase.New(uic)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cShapeGUIHandler.ExecuteCommand"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub ExecuteCommand(cmd As ScientificInterfaceShared.Controls.cShapeGUIHandler.eShapeCommandTypes,
                                          Optional ashapes() As EwECore.cShapeData = Nothing,
                                          Optional data As Object = Nothing)

            Try
                Select Case cmd

                    Case eShapeCommandTypes.DefineMediation
                        Debug.Assert((TypeOf Me.SelectedShape Is EwECore.cEnviroResponseFunction), "OPPSSS...")
                        Dim dlgDefBP As New dlgDefineEcosimFunctionalResponses(Me.UIContext, DirectCast(Me.SelectedShape, EwECore.cEnviroResponseFunction), Me.UIContext.Core.EcosimEnviroResponseManager)
                        If dlgDefBP.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                            Me.MediationAssignments.RefreshContent()
                        End If

                    Case eShapeCommandTypes.Import
                        Dim dlg As New dlgImportShapes(Me.UIContext, Me.ShapeManager)
                        If dlg.ShowDialog() = DialogResult.OK Then
                            If Me.MediationAssignments IsNot Nothing Then
                                Me.MediationAssignments.RefreshContent()
                            End If
                        End If

                    Case Else
                        MyBase.ExecuteCommand(cmd, ashapes, data)

                End Select
            Catch ex As Exception

            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cShapeGUIHandler.OnShapeSelected"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeSelected(shape() As EwECore.cShapeData)
            MyBase.OnShapeSelected(shape)
            If (Me.MediationAssignments IsNot Nothing) Then
                Dim strTitle As String = ""
                If shape IsNot Nothing Then
                    If shape.Length > 0 Then
                        Dim fmt As New cCoreInterfaceFormatter()
                        strTitle = cStringUtils.Localize(My.Resources.HEADER_ASSIGNED_ENV_FORCING, fmt.ToString(shape(0), eDescriptorTypes.Name))
                    End If
                End If
                Me.MediationAssignments.Title = strTitle
            End If
        End Sub

        Protected Overrides Function Datatypes() As eDataTypes()
            Return {eDataTypes.EcosimEnviroResponseFunctionManager}
        End Function

    End Class

End Namespace
