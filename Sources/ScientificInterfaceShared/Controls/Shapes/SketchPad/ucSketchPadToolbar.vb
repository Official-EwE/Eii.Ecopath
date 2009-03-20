'==============================================================================
'
' $Log: ucSketchPadToolbar.vb,v $
' Revision 1.3  2009/03/20 17:55:42  jeroens
' Shape controls are multiple selection
'
' Revision 1.2  2009/03/02 02:05:37  jeroens
' Properly named handlers
'
' Revision 1.1  2008/12/15 15:36:42  jeroens
' Moved from ScInt
'
' Revision 1.1  2008/09/26 07:31:44  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports EwECore
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This Sketchpad control class is used to render 
    ''' <see cref="cShapeData">cShapeData</see> and support mouse interaction.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class ucSketchPadToolbar

#Region " Variables "

        Private m_handler As cShapeGUIHandler = Nothing

#End Region

#Region " Constructors "

        Public Sub New()
            InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Properties "

        Public Property Handler() As cShapeGUIHandler
            Get
                Return Me.m_handler
            End Get
            Set(ByVal value As cShapeGUIHandler)
                Me.m_handler = value
                Me.UpdateControls()
            End Set
        End Property

        Public WriteOnly Property IsMenuVisible() As Boolean
            Set(ByVal value As Boolean)
                tsMenus.Visible = value
            End Set
        End Property

#End Region ' Properties

#Region " Public interfaces "

        Public Overrides Sub Refresh()
            MyBase.Refresh()
            Me.UpdateControls()
        End Sub

#End Region ' Public interfaces

#Region " Event handlers "

        Private Sub SketchPadWithMenus_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.UpdateControls()
        End Sub

        Private Sub SketchPadWithMenus_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            ' Release event hooks
            Me.Handler = Nothing
        End Sub

        Private Sub ResetShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnReset.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Reset)
        End Sub

        Private Sub ShapeValue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnValue.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Modify)
        End Sub

        Private Sub LoadShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Load)
        End Sub

        Private Sub SaveShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnSave.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.SaveAsImage)
        End Sub

        Private Sub tsbChangeShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbChangeShape.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.ChangeShape)
        End Sub

        Private Sub ShapeOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles tsBtnOptions.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.DisplayOptions)
        End Sub

        Private Sub tscbbType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles tscbbShapeView.SelectedIndexChanged
            If Me.m_bInUpdate Then Return
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Seasonal, Nothing, (tscbbShapeView.SelectedIndex = 1))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; responds to an [ENTER] key press to apply entered text
        ''' to the grid selection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_tstbWeight_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles m_tstbWeight.KeyDown
            ' Is [ENTER]?
            If e.KeyCode = Keys.Enter Then
                If (Me.Handler IsNot Nothing) Then
                    Dim sWeight As Single = 1.0!
                    Try
                        sWeight = Single.Parse(m_tstbWeight.Text)
                    Catch ex As Exception
                        sWeight = 1.0!
                    End Try
                    Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.SetWeight, Nothing, sWeight)
                End If
            End If
        End Sub

#End Region ' Event handlers

#Region " Internal implementation "

        Private m_bInUpdate As Boolean = False

        Private Sub UpdateControls()

            If (Me.Handler Is Nothing) Then Return

            Dim shapeSelected As cShapeData = Me.Handler.SelectedShape

            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.DisplayOptions, Me.tsBtnOptions)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SaveAsImage, Me.tsBtnSave)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.ChangeShape, Me.tsbChangeShape)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Reset, Me.tsBtnReset)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Modify, Me.tsBtnValue)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Seasonal, Me.tscbbShapeView)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Seasonal, Me.tslbShapeView)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SetWeight, Me.m_tslWeight)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SetWeight, Me.m_tstbWeight)

            If ((shapeSelected IsNot Nothing) And (Me.tscbbShapeView.Visible = True)) Then

                Me.m_bInUpdate = True
                If shapeSelected.IsSeasonal Then
                    Me.tscbbShapeView.SelectedIndex = 1 'Seasonal
                Else
                    Me.tscbbShapeView.SelectedIndex = 0 'Long term
                End If
                Me.m_bInUpdate = False

            End If

            If ((shapeSelected IsNot Nothing) And (TypeOf shapeSelected Is cTimeSeries)) Then

                Me.m_bInUpdate = True
                Me.m_tstbWeight.Text = CStr(DirectCast(shapeSelected, cTimeSeries).WtType)
                Me.m_bInUpdate = False

            End If

            ToolstripUtils.HideRepeatingSeparators(Me.tsMenus)

        End Sub

        Private Sub UpdateCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes, ByVal tsi As ToolStripItem)
            If (Me.m_handler Is Nothing) Then Return
            If Me.m_handler.SupportCommand(cmd) Then
                tsi.Visible = True
                tsi.Enabled = (m_handler.EnableCommand(cmd))
            Else
                tsi.Visible = False
            End If
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace



