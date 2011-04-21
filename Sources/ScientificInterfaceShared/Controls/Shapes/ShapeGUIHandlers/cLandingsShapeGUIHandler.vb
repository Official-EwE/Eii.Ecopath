#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Definitions
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling fishing mortality <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cLandingsShapeGUIHandler
        : Inherits cMediationShapeGUIHandler

        Public Overrides Sub Attach(ByVal uic As cUIContext, ByVal stb As ucShapeToolbox, ByVal stbtb As ucShapeToolboxToolbar, ByVal sp As ucSketchPad, ByVal sptb As ucSketchPadToolbar, ByVal bp As ucMediationAssignments, ByVal bpt As ucMediationAssignmentsToolbar)
            MyBase.Attach(uic, stb, stbtb, sp, sptb, bp, bpt)

            If (Me.MediationAssignments IsNot Nothing) Then
                Me.MediationAssignments.XAxisLabel = My.Resources.HEADER_ASSIGNED_LANDINGS
            End If

            If (Me.MediationAssignmentsToolbar IsNot Nothing) Then
                Me.MediationAssignmentsToolbar.DefineMediationLabel = My.Resources.PROMPT_DEFINE_MEDIATING_LANDINGS
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As cBaseShapeManager
            Return Me.Core.LandingsShapeManager
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering price elasticity shapes.
        ''' </summary>
        ''' <returns>The color for rendering price elasticity shapes.</returns>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Color() As System.Drawing.Color
            Return Color.FromArgb(255, 41, 233, 41)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the name for a new landings mediation shape..
        ''' </summary>
        ''' <returns>The name for a new landings mediation shape.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function NewShapeNameMask() As String
            Return My.Resources.ECOSIM_DEFAULT_NEWLANDINGSSHAPE
        End Function

        Protected Overrides Function Datatypes() As EwEUtils.Core.eDataTypes()
            Return New eDataTypes() {eDataTypes.PriceMediation}
        End Function
    End Class

End Namespace