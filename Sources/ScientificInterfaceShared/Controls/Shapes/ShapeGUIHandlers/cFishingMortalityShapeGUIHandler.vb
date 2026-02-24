' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Definitions

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling fishing mortality <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)>
    Public Class cFishingMortalityShapeGUIHandler
        Inherits cFishingBaseShapeGUIHandler

        Public Sub New(uic As cUIContext)
            MyBase.New(uic)
        End Sub

        Public Overrides Function SupportCommand(cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean

            Select Case cmd

                Case eShapeCommandTypes.Duplicate
                    Return False

                    'Case eShapeCommandTypes.ChangeShape, _
                    '     eShapeCommandTypes.Duplicate, _
                    '     eShapeCommandTypes.Modify, _
                    '     eShapeCommandTypes.Reset, _
                    '     eShapeCommandTypes.ResetAll, _
                    '     eShapeCommandTypes.SetToZero, _
                    '     eShapeCommandTypes.SetValue
                    '    Return False

            End Select
            Return MyBase.SupportCommand(cmd)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering fishing mortality shapes.
        ''' </summary>
        ''' <returns>The color for rendering fishing mortality shapes.</returns>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Color() As System.Drawing.Color
            Debug.Assert(Me.UIContext IsNot Nothing)
            Return Me.UIContext.StyleGuide.ShapeColor(eDataTypes.FishMort)
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

        Protected Overrides Function Datatypes() As eDataTypes()
            Return {eDataTypes.FishMort, eDataTypes.FishingEffort}
        End Function

    End Class

End Namespace