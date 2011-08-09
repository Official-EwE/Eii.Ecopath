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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling <see cref="cEnviroResponseFunction">environmental response functions</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
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

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cMediationShapeGUIHandler.ExecuteCommand"/>
        ''' -----------------------------------------------------------------------
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="uic"><see cref="cUIContext">UI context</see> to connect to.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' <param name="ma"><see cref="ucMediationAssignments">Mediation assignments control</see> to handle, if any.</param>
        ''' <param name="mat"><see cref="ucMediationAssignmentsToolbar"/> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Overridable Shadows Sub Attach(ByVal uic As cUIContext, _
                                  ByVal stb As ucShapeToolbox, _
                                  ByVal stbtb As ucShapeToolboxToolbar, _
                                  ByVal sp As ucSketchPad, _
                                  ByVal sptb As ucSketchPadToolbar, _
                                  ByVal ma As ucMediationAssignments, _
                                  ByVal mat As ucMediationAssignmentsToolbar)

            MyBase.Attach(uic, stb, stbtb, sp, sptb, ma, mat)

            Me.SketchPad.ShowXMark = False
            ' Tooltip does not make much sense for mediation functions
            Me.SketchPad.ShowValueTooltip = False

            'Me.MediationAssignments = ma
            'If (Me.MediationAssignments IsNot Nothing) Then
            '    Me.MediationAssignments.Title = ""
            '    Me.MediationAssignments.XAxisLabel = My.Resources.HEADER_ASSIGNED_GROUPS_FLEETS
            'End If

            Me.MediationAssignmentsToolbar = mat
            If (Me.MediationAssignmentsToolbar IsNot Nothing) Then
                Me.MediationAssignmentsToolbar.DefineMediationLabel = "Set X axis values."
            End If

            '' Manually update selection
            'Me.MediationAssignments.Shape = DirectCast(Me.SelectedShape, cMediationBaseFunction)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the name for a new capacity shape.
        ''' </summary>
        ''' <returns>The name for a new capacity shape.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function NewShapeNameMask() As String
            Return My.Resources.ECOSIM_DEFAULT_NEWCAPACITYSHAPE
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cShapeGUIHandler.Datatypes"/>
        ''' <remarks>Overridden to enable handler for 
        ''' <see cref="cEnviroResponseFunction">environmental response functions</see>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function Datatypes() As EwEUtils.Core.eDataTypes()
            Return New eDataTypes() {eDataTypes.CapacityMediation}
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cShapeGUIHandler.OnShapeSelected"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeSelected(ByVal shape() As EwECore.cShapeData)
            MyBase.OnShapeSelected(shape)
            If (Me.MediationAssignments IsNot Nothing) Then
                Dim strTitle As String = ""
                If shape IsNot Nothing Then
                    If shape.Length > 0 Then
                        Dim fmt As New cCoreInterfaceFormatter()
                        '  strTitle = String.Format(My.Resources.HEADER_ASSIGNED_LANDINGS_SHAPE, fmt.GetDescriptor(shape(0), eDescriptorTypes.Name))
                        strTitle = String.Format("Response", fmt.GetDescriptor(shape(0), eDescriptorTypes.Name))
                    End If
                End If
                Me.MediationAssignments.Title = strTitle
            End If
        End Sub
    End Class

End Namespace
