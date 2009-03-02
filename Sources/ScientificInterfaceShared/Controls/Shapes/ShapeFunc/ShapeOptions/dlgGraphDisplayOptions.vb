'==============================================================================
'
' $Log: dlgGraphDisplayOptions.vb,v $
' Revision 1.2  2009/03/02 01:49:20  jeroens
' Removed right-click scaling option
'
' Revision 1.1  2008/12/15 15:36:38  jeroens
' Moved from ScInt
'
' Revision 1.1  2008/09/26 07:31:42  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports ScientificInterfaceShared.Definitions
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Dialog that allows a user to modify how a <see cref="ucSketchPad">sketch pad</see>
    ''' displays a <see cref="EwECore.cShapeData">shape</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class dlgGraphDisplayOptions

#Region " Private vars "

        ''' <summary></summary>
        Private m_SketchPad As ucSketchPad = Nothing
        ''' <summary></summary>
        Private m_fbYMax As cEwEFormatProvider = Nothing

#End Region ' Private vars

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this dialog.
        ''' </summary>
        ''' <param name="sketchPad"><see cref="ucSketchPad">sketch pad</see> to modify.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByRef sketchPad As ucSketchPad)

            Me.InitializeComponent()

            Me.m_SketchPad = sketchPad
            Me.CenterToParent()

        End Sub

#End Region ' Constructor

#Region " Internal implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Apply the contents of the dialog to the connected <see cref="ucSketchPad">sketch pad</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Apply()

            ' Show marks or not
            If Me.m_cbShowScaleAndTitle.Checked Then
                Me.m_SketchPad.AxisDisplayMode = eAxisDisplayModeTypes.Show
            Else
                Me.m_SketchPad.AxisDisplayMode = eAxisDisplayModeTypes.Hide
            End If

            ' Do we need auto scale? 
            If Me.cbAutoScale.Checked Then
                Me.m_SketchPad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto
            Else
                Me.m_SketchPad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Fixed
            End If

            '' Do we want mouse right click auto scale?
            'If Me.cbRightClickAutoScale.Checked Then
            '    Me.m_SketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto
            'Else
            '    Me.m_SketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Fixed
            'End If

            ' Set display mode
            If Me.m_rbFill.Checked Then Me.m_SketchPad.SketchDrawMode = eSketchDrawModeTypes.Fill
            If Me.m_rbLine.Checked Then Me.m_SketchPad.SketchDrawMode = eSketchDrawModeTypes.Line
            If Me.m_rbDots.Checked Then Me.m_SketchPad.SketchDrawMode = eSketchDrawModeTypes.Dots

            ' The Y scale
            Me.m_SketchPad.YAxisMaxValue = CSng(Me.m_fbYMax.Value)

        End Sub

#End Region ' Internal implementation

#Region " Events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; populates the dialog for use by plundering its 
        ''' settings from the attached <see cref="ucSketchPad">sketch pad</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SketchPadOption_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Initialization of the interface controls
            Me.m_fbYMax = New cEwEFormatProvider(Me.nupYMax, GetType(Single))

            Me.m_cbShowScaleAndTitle.Checked = (Me.m_SketchPad.AxisDisplayMode = eAxisDisplayModeTypes.Show)

            ' Rendering method
            Select Case Me.m_SketchPad.SketchDrawMode
                Case eSketchDrawModeTypes.Fill
                    Me.m_rbFill.Checked = True
                Case eSketchDrawModeTypes.Line
                    Me.m_rbLine.Checked = True
                Case eSketchDrawModeTypes.Dots
                    Me.m_rbDots.Checked = True
            End Select

            ' Is mediation sketch pad?
            If (Me.m_SketchPad.ShapeType = eShapeCategoryTypes.Mediation) Then
                ' #Yes: not allowed to rescale
                Me.cbAutoScale.Enabled = False
                'Me.cbRightClickAutoScale.Enabled = False
                Me.m_fbYMax.Enabled = False
            Else
                ' #No: scale ahead, Wanda!
                Me.cbAutoScale.Checked = (Me.m_SketchPad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto)
                'Me.cbRightClickAutoScale.Checked = (m_SketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; okidokionizes the dialog.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
            Me.Apply()
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; cancels the dialog.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

#End Region ' Events

    End Class

End Namespace
