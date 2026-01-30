' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Definitions



Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling fishing effort <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)>
    Public Class cFishingEffortShapeGUIHandler
        Inherits cFishingBaseShapeGUIHandler

        Public Sub New(uic As cUIContext)
            MyBase.New(uic)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering fishing effort shapes.
        ''' </summary>
        ''' <returns>The color for rendering fishing effort shapes.</returns>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Color() As System.Drawing.Color
            Debug.Assert(Me.UIContext IsNot Nothing)
            Return Me.UIContext.StyleGuide.ShapeColor(eDataTypes.FishingEffort)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to refresh all shapes when the 'all fleet' shape was changed.
        ''' </summary>
        ''' <param name="shape"></param>
        ''' <param name="sketchpad"></param>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub OnShapeFinalized(shape As EwECore.cShapeData, sketchpad As ucSketchPad)
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

        Protected Overrides Function Datatypes() As eDataTypes()
            Return {eDataTypes.FishingEffort}
        End Function

    End Class

End Namespace
