'==============================================================================
'
' $Log: dlgGraphDisplayOptions.vb,v $
' Revision 1.4  2009/03/19 16:02:25  jeroens
' Added FormatProvider.Release
'
' Revision 1.3  2009/03/02 17:44:20  jeroens
' Cleaned up
'
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
        Private m_sketchpad As ucSketchPad = Nothing
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

            Me.m_sketchpad = sketchPad
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
            Me.m_sketchpad.DisplayAxis = Me.m_cbShowScaleAndTitle.Checked

            ' Do we need auto scale? 
            If Me.cbAutoScale.Checked Then
                Me.m_sketchpad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto
            Else
                Me.m_sketchpad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Fixed
            End If

            '' Do we want mouse right click auto scale?
            'If Me.cbRightClickAutoScale.Checked Then
            '    Me.m_SketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto
            'Else
            '    Me.m_SketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Fixed
            'End If

            ' Set display mode
            If Me.m_rbFill.Checked Then Me.m_sketchpad.SketchDrawMode = eSketchDrawModeTypes.Fill
            If Me.m_rbLine.Checked Then Me.m_sketchpad.SketchDrawMode = eSketchDrawModeTypes.Line
            If Me.m_rbDots.Checked Then Me.m_sketchpad.SketchDrawMode = eSketchDrawModeTypes.Dots

            ' The Y scale
            Me.m_sketchpad.YAxisMaxValue = CSng(Me.m_fbYMax.Value)

        End Sub

#End Region ' Internal implementation

#Region " Events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; populates the dialog for use by plundering its 
        ''' settings from the attached <see cref="ucSketchPad">sketch pad</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SketchPadOption_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles MyBase.Load

            'Initialization of the interface controls
            Me.m_fbYMax = New cEwEFormatProvider(Me.nupYMax, GetType(Single))

            Me.m_cbShowScaleAndTitle.Checked = Me.m_sketchpad.DisplayAxis

            ' Rendering method
            Select Case Me.m_sketchpad.SketchDrawMode
                Case eSketchDrawModeTypes.Fill
                    Me.m_rbFill.Checked = True
                Case eSketchDrawModeTypes.Line
                    Me.m_rbLine.Checked = True
                Case eSketchDrawModeTypes.Dots
                    Me.m_rbDots.Checked = True
            End Select

            ' Is mediation sketch pad?
            If (Me.m_sketchpad.ShapeType = eShapeCategoryTypes.Mediation) Then
                ' #Yes: not allowed to rescale
                Me.cbAutoScale.Enabled = False
                'Me.cbRightClickAutoScale.Enabled = False
                Me.m_fbYMax.Enabled = False
            Else
                ' #No: scale ahead, Wanda!
                Me.cbAutoScale.Checked = (Me.m_sketchpad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto)
                'Me.cbRightClickAutoScale.Checked = (m_SketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto)
            End If

        End Sub

        Private Sub dlgGraphDisplayOptions_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) _
            Handles Me.FormClosing

            Me.m_fbYMax.Release()
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
