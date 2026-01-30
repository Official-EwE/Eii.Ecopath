' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style



Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling fishing mortality <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)>
    Public Class cLandingsShapeGUIHandler
        Inherits cMediationShapeGUIHandler

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
            Return Me.Core.LandingsShapeManager
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering price elasticity shapes.
        ''' </summary>
        ''' <returns>The color for rendering price elasticity shapes.</returns>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Color() As System.Drawing.Color
            Debug.Assert(Me.UIContext IsNot Nothing)
            Return Me.UIContext.StyleGuide.ShapeColor(eDataTypes.PriceMediation)
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

        Protected Overrides Function Datatypes() As eDataTypes()
            Return New eDataTypes() {eDataTypes.PriceMediation}
        End Function

        Public Overrides Sub OnShapeSelected(shape() As EwECore.cShapeData)
            MyBase.OnShapeSelected(shape)
            If (Me.MediationAssignments IsNot Nothing) Then
                Dim strTitle As String = ""
                If shape IsNot Nothing Then
                    If shape.Length > 0 Then
                        Dim fmt As New cCoreInterfaceFormatter()
                        strTitle = cStringUtils.Localize(My.Resources.HEADER_ASSIGNED_LANDINGS_SHAPE, fmt.ToString(shape(0), eDescriptorTypes.Name))
                    End If
                End If
                Me.MediationAssignments.Title = strTitle
            End If
        End Sub

    End Class

End Namespace