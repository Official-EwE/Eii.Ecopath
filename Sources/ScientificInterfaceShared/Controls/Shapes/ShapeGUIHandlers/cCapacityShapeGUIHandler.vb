#Region " Imports "

Option Strict On

Imports EwECore
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Commands
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls



    Public Class cCapacityShapeGUIHandler
        Inherits cMediationShapeGUIHandler


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As cBaseShapeManager
            Return Me.Core.CapacityShapeManager
        End Function


        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering price elasticity shapes.
        ''' </summary>
        ''' <returns>The color for rendering price elasticity shapes.</returns>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Color() As System.Drawing.Color
            Return Drawing.Color.SandyBrown
        End Function


        Public Overrides Sub ExecuteCommand(ByVal cmd As ScientificInterfaceShared.Controls.cShapeGUIHandler.eShapeCommandTypes, _
                                          Optional ByVal ashapes() As EwECore.cShapeData = Nothing, _
                                          Optional ByVal data As Object = Nothing)

            Try
                Select Case cmd

                    Case eShapeCommandTypes.DefineMediation
                        Debug.Assert((TypeOf Me.SelectedShape Is EwECore.cEnviroResponseFunction), "OPPSSS...")
                        Dim dlgDefBP As New dlgDefineMapResponseAssignments(Me.UIContext, DirectCast(Me.SelectedShape, EwECore.cEnviroResponseFunction), UIContext.Core.CapacitMapInteractionManager)
                        '   Dim dlgDefBP As New ScientificInterfaceShared.d
                        If dlgDefBP.ShowDialog() = Windows.Forms.DialogResult.OK Then
                            Me.MediationAssignments.RefreshContent()
                        End If

                        'Case eShapeCommandTypes.ViewMode
                        '    Me.m_medass.ViewMode = DirectCast(data, ucMediationAssignments.eViewModeTypes)

                    Case Else
                        MyBase.ExecuteCommand(cmd, ashapes, data)

                End Select
            Catch ex As Exception

            End Try

        End Sub


    End Class

End Namespace
