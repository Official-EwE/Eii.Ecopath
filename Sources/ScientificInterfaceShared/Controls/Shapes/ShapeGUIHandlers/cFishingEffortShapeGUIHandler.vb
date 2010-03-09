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
    ''' handling fishing effort <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cFishingEffortShapeGUIHandler
        : Inherits cFishingBaseShapeGUIHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="uic"><see cref="cUIContext">UI contextual</see> information.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
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
        ''' Returns the colour for rendering fishing effort shapes.
        ''' </summary>
        ''' <returns>The color for rendering fishing effort shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function Color() As System.Drawing.Color
            Return Drawing.Color.Coral
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to refresh all shapes when the 'all fleet' shape was changed.
        ''' </summary>
        ''' <param name="shape"></param>
        ''' <param name="sketchpad"></param>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub OnShapeFinalized(ByVal shape As EwECore.cShapeData, ByVal sketchpad As ucSketchPad)
            MyBase.OnShapeFinalized(shape, sketchpad)
            Me.Refresh()
        End Sub
        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As EwECore.cBaseShapeManager
            Return Me.Core.FishingEffortShapeManager
        End Function

        Protected Overrides Function ScaleMode() As eAxisTickmarkDisplayModeTypes
            Return eAxisTickmarkDisplayModeTypes.Relative
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to make shape display controls auto-scale the Y axis.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function MinYScale() As Single
            Return cCore.NULL_VALUE
        End Function

    End Class

End Namespace
