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

        Public Overrides Function SupportCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean

            Select Case cmd
                Case eShapeCommandTypes.ChangeShape, _
                     eShapeCommandTypes.Duplicate, _
                     eShapeCommandTypes.Modify, _
                     eShapeCommandTypes.Reset, _
                     eShapeCommandTypes.ResetAll, _
                     eShapeCommandTypes.SetToZero, _
                     eShapeCommandTypes.SetValue
                    Return False
            End Select
            Return MyBase.SupportCommand(cmd)

        End Function

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