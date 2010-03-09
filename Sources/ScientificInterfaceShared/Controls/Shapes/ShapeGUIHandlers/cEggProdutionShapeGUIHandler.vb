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
    ''' handling egg production <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cEggProductionShapeGUIHandler
        Inherits cForcingShapeGUIHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="uic"><see cref="cUIContext">UI contextual</see> information.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal stb As ucShapeToolbox, _
                       ByVal stbtb As ucShapeToolboxToolbar, _
                       ByVal sp As ucSketchPad, _
                       ByVal sptb As ucSketchPadToolbar)

            MyBase.New(uic, stb, stbtb, sp, sptb)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As cBaseShapeManager
            Return Me.Core.EggProdShapeManager
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering egg production shapes.
        ''' </summary>
        ''' <returns>The color for rendering egg production shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function Color() As System.Drawing.Color
            Return Color.Orange
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the name for a new egg production shape.
        ''' </summary>
        ''' <returns>The name for a new egg production shape.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function NewShapeNameMask() As String
            Return My.Resources.ECOSIM_DEFAULT_NEWEGGPRODSHAPE
        End Function

    End Class

End Namespace
