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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering fishing effort shapes.
        ''' </summary>
        ''' <returns>The color for rendering fishing effort shapes.</returns>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Color() As System.Drawing.Color
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
