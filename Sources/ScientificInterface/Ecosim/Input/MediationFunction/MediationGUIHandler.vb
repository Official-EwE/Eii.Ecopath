'==============================================================================
'
' $Log: MediationGUIHandler.vb,v $
' Revision 1.8  2009/06/19 04:10:29  jeroens
' YMarkValue aligned to shape / XBase intersection, no longer to YBase
'
' Revision 1.7  2009/04/12 21:20:52  jeroens
' Custom -> DefineXAxis (issue 611)
'
' Revision 1.6  2009/03/24 20:28:35  jeroens
' Uses mediation tool bar
'
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
Imports ScientificInterfaceShared.Controls

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
        Private m_bp As ucBioPercent = Nothing
        ''' <summary>Biomass percent control toolbar to handle.</summary>
        Private m_bpt As ucBioPercentToolbar = Nothing

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
                 ByVal bp As ucBioPercent, ByVal bpt As ucBioPercentToolbar)

            MyBase.New(core, stb, stbtb, sp, sptb)

            Me.SketchPad.ShowXMark = True
            Me.BiomassPercent = bp
            Me.BiomassPercentToolbar = bpt
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Destructor; releases all controls.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub Finalize()
            Me.BiomassPercent = Nothing
            Me.BiomassPercentToolbar = Nothing
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

        Public Overridable Property BiomassPercent() As ucBioPercent
            Get
                Return Me.m_bp
            End Get
            Protected Set(ByVal value As ucBioPercent)

                If (Me.m_bp IsNot Nothing) Then
                    'Me.m_bp.Handler = Nothing
                End If

                Me.m_bp = value

                If (Me.m_bp IsNot Nothing) Then
                    'Me.m_bp.Handler = Me
                End If

            End Set
        End Property

        Public Overridable Property BiomassPercentToolbar() As ucBioPercentToolbar
            Get
                Return Me.m_bpt
            End Get
            Protected Set(ByVal value As ucBioPercentToolbar)

                If (Me.m_bpt IsNot Nothing) Then
                    Me.m_bpt.Handler = Nothing
                End If

                Me.m_bpt = value

                If (Me.m_bpt IsNot Nothing) Then
                    Me.m_bpt.Handler = Me
                End If

            End Set
        End Property

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
        ''' BioPercent control.
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

                If (Me.BiomassPercent IsNot Nothing) Then Me.BiomassPercent.Shape = shapeSelected

                If Me.SketchPad IsNot Nothing Then
                    If (shapeSelected Is Nothing) Then
                        Me.SketchPad.XMarkValue = cCore.NULL_VALUE
                        Me.SketchPad.YMarkValue = cCore.NULL_VALUE
                    Else
                        Dim mf As cMediationFunction = DirectCast(shapeSelected, cMediationFunction)
                        Me.SketchPad.XMarkValue = CSng(mf.XBaseIndex)
                        Me.SketchPad.YMarkValue = mf.ShapeData(Math.Max(0, Math.Min(mf.XBaseIndex, mf.ShapeData.Length - 1)))
                    End If
                End If

                If (Me.BiomassPercentToolbar IsNot Nothing) Then Me.BiomassPercentToolbar.Refresh()

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to suppress Seasonal command
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean
            Select Case cmd
                Case eShapeCommandTypes.Seasonal
                    Return False
                Case eShapeCommandTypes.DefineXAxis
                    Return True
            End Select
            Return MyBase.SupportCommand(cmd)
        End Function

        Public Overrides Function EnableCommand(ByVal cmd As ScientificInterfaceShared.Controls.cShapeGUIHandler.eShapeCommandTypes) As Boolean
            Select Case cmd
                Case eShapeCommandTypes.DefineXAxis
                    Return (Me.SelectedShape IsNot Nothing)
            End Select
            Return MyBase.EnableCommand(cmd)
        End Function

        Public Overrides Sub ExecuteCommand(ByVal cmd As ScientificInterfaceShared.Controls.cShapeGUIHandler.eShapeCommandTypes, _
                                            Optional ByVal ashapes() As EwECore.cShapeData = Nothing, _
                                            Optional ByVal data As Object = Nothing)

            Select Case cmd

                Case eShapeCommandTypes.DefineXAxis
                    Dim dlgDefBP As New defBioPercent(DirectCast(Me.SelectedShape, cMediationFunction))
                    If dlgDefBP.ShowDialog() = Windows.Forms.DialogResult.OK Then
                        Me.BiomassPercent.LoadGraphData()
                    End If

                Case Else
                    MyBase.ExecuteCommand(cmd, ashapes, data)

            End Select
        End Sub

        Public Overrides Sub OnShapeFinalized(ByVal shape As EwECore.cShapeData, ByVal sketchpad As ucSketchPad)
            DirectCast(shape, cMediationFunction).XBaseIndex = CInt(Math.Round(sketchpad.XMarkValue))
            MyBase.OnShapeFinalized(shape, sketchpad)
        End Sub

    End Class

#End Region ' Mediation

End Namespace ' Ecosim
