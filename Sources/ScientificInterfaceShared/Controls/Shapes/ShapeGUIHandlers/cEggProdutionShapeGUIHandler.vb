' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling egg production <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)>
    Public Class cEggProductionShapeGUIHandler
        Inherits cForcingShapeGUIHandler

        Public Sub New(uic As cUIContext)
            MyBase.New(uic)
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
        Public Overrides Function Color() As System.Drawing.Color
            Debug.Assert(Me.UIContext IsNot Nothing)
            Return Me.UIContext.StyleGuide.ShapeColor(eDataTypes.EggProd)
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
