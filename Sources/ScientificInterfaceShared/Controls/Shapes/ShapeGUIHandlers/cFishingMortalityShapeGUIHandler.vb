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
    Public Class cFishingMortalityShapeGUIHandler
        : Inherits cFishingBaseShapeGUIHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="uic"><see cref="cUIContext">UI contextual</see> information.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal stb As ucShapeToolbox, _
                       ByVal sp As ucSketchPad, _
                       Optional ByVal stbtb As ucShapeToolboxToolbar = Nothing, _
                       Optional ByVal sptb As ucSketchPadToolbar = Nothing)
            MyBase.New(uic, stb, stbtb, sp, sptb)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering fishing mortality shapes.
        ''' </summary>
        ''' <returns>The color for rendering fishing mortality shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function Color() As System.Drawing.Color
            Return Drawing.Color.DarkGray
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As EwECore.cBaseShapeManager
            Return Me.Core.FishMortShapeManager
        End Function

        Protected Overrides Function ScaleMode() As eAxisTickmarkDisplayModeTypes
            Return eAxisTickmarkDisplayModeTypes.Absolute
        End Function

        Protected Overrides Function MinYScale() As Single
            Return 0
        End Function

    End Class

End Namespace