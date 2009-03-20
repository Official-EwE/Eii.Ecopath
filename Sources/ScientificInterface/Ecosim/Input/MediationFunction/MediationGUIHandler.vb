'==============================================================================
'
' $Log: MediationGUIHandler.vb,v $
' Revision 1.5  2009/03/20 17:55:41  jeroens
' Shape controls are multiple selection
'
' Revision 1.4  2009/03/19 16:13:42  jeroens
' X mark can be suppressed
'
' Revision 1.3  2009/03/02 01:58:07  jeroens
' Connected XMarkValue, YMarkValue
'
' Revision 1.2  2009/02/12 15:32:20  jeroens
' Can add labels to XMark, YMark lines
'
' Revision 1.1  2008/12/15 19:49:12  jeroens
' Split off
'
' Revision 1.2  2008/11/08 23:51:05  jeroens
' Renamed file commands
'
' Revision 1.1  2008/09/26 07:31:41  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On

Imports EwECore
Imports ScientificInterface.Other
Imports EwEUtils.Commands
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared

Namespace Ecosim

#Region " Mediation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling <see cref="cMediationFunction">mediation shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cMediationShapeGUIHandler
        Inherits cForcingShapeGUIHandler

        ''' <summary>Biomass percent control to handle.</summary>
        Private m_bioPercent As ucBioPercent = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="core">Core to connect to.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' <param name="bp"><see cref="ucBioPercent">Biomass percentage control</see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, _
                 ByVal stb As ucShapeToolbox, ByVal stbtb As ucShapeToolboxToolbar, _
                 ByVal sp As ucSketchPad, ByVal sptb As ucSketchPadToolbar, _
                 ByVal bp As ucBioPercent)

            MyBase.New(core, stb, stbtb, sp, sptb)

            Me.SketchPad.ShowXMark = True
            Me.BioPercent = bp
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Destructor; releases all controls.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub Finalize()
            Me.BioPercent = Nothing
            MyBase.Finalize()
        End Sub

        Public Overrides Sub SetSeasonal(ByVal shape As EwECore.cShapeData, ByVal bSeasonal As Boolean)
            ' Not allowed to do this; it makes absolutely no sense for Mediation shapes
            Debug.Assert(False)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specifies the shapes manager that delivers the data for this handler.
        ''' </summary>
        ''' <returns>The shapes manager that delivers the data for this handler.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ShapeManager() As cBaseShapeManager
            Return Me.m_core.MediationShapeManager
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for rendering mediation shapes.
        ''' </summary>
        ''' <returns>The color for rendering mediation shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function Color() As System.Drawing.Color
            Return Color.FromArgb(255, 81, 133, 255)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the lower limit for the sketch pad Y-axis when displaying 
        ''' mediation shapes.
        ''' </summary>
        ''' <returns>The lower limit for the sketch pad Y-axis when displaying 
        ''' mediation shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function MinYScale() As Single
            Return 1.0!
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the name for a new Mediation shape..
        ''' </summary>
        ''' <returns>The name for a new Mediation shape.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function NewShapeNameMask() As String
            Return My.Resources.ECOSIM_DEFAULT_NEWMEDIATIONSHAPE
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected shape in the GUI. Overridden to synchronize the
        ''' <see cref="m_bioPercent">BioPercent</see> control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property SelectedShapes() As EwECore.cShapeData()
            Get
                Return MyBase.SelectedShapes
            End Get
            Set(ByVal value As EwECore.cShapeData())

                MyBase.SelectedShapes = value

                ' Single selection
                Dim shapeSelected As cShapeData = Nothing
                If (value IsNot Nothing) Then
                    If (value.Length = 1) Then shapeSelected = value(0)
                End If

                If (Me.BioPercent IsNot Nothing) Then Me.BioPercent.Shape = shapeSelected

                If Me.SketchPad IsNot Nothing Then
                    If (shapeSelected Is Nothing) Then
                        Me.SketchPad.XMarkValue = cCore.NULL_VALUE
                        Me.SketchPad.YMarkValue = cCore.NULL_VALUE
                    Else
                        Me.SketchPad.XMarkValue = CSng(DirectCast(shapeSelected, cMediationFunction).XBaseIndex)
                        Me.SketchPad.YMarkValue = DirectCast(shapeSelected, cMediationFunction).YBase
                    End If
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the Biomass percent control to handle.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property BioPercent() As ucBioPercent
            Get
                Return Me.m_bioPercent
            End Get
            Set(ByVal value As ucBioPercent)
                Me.m_bioPercent = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to suppress Seasonal command
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean
            If cmd = eShapeCommandTypes.Seasonal Then Return False
            Return MyBase.SupportCommand(cmd)
        End Function

        Public Overrides Sub OnShapeFinalized(ByVal shape As EwECore.cShapeData, ByVal sketchpad As ucSketchPad)
            DirectCast(shape, cMediationFunction).XBaseIndex = CInt(Math.Round(sketchpad.XMarkValue))
            MyBase.OnShapeFinalized(shape, sketchpad)
        End Sub

    End Class

#End Region ' Mediation

End Namespace ' Ecosim
