' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports ScientificInterfaceShared.Style

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This Sketchpad control class is used to render 
    ''' <see cref="cShapeData">cShapeData</see> and support mouse interaction.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)>
    Public Class ucSketchPadToolbar
        Implements IUIElement

#Region " Variables "

        Private m_handler As cShapeGUIHandler = Nothing
        Private m_uic As cUIContext = Nothing

#End Region ' Variables

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Properties "

        Public Property Handler() As cShapeGUIHandler
            Get
                Return Me.m_handler
            End Get
            Set(value As cShapeGUIHandler)
                Me.m_handler = value
                Me.UpdateControls()
            End Set
        End Property

        Public WriteOnly Property IsMenuVisible() As Boolean
            Set(value As Boolean)
                Me.m_tsMenus.Visible = value
            End Set
        End Property

        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As cUIContext)
                If (Me.m_uic IsNot Nothing) Then
                    RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
                End If
                Me.m_uic = value
                If (Me.m_uic IsNot Nothing) Then
                    AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
                End If
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

        Protected Overrides Sub OnVisibleChanged(e As System.EventArgs)
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            Me.UpdateControls()
        End Sub

        Private Sub SketchPadWithMenus_Disposed(sender As Object, e As System.EventArgs) Handles Me.Disposed
            ' Release event hooks
            Me.Handler = Nothing
        End Sub

        Private Sub ResetShape_Click(sender As System.Object, e As System.EventArgs) Handles m_tsbnReset.Click
            If (Me.Handler IsNot Nothing) Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Reset)
        End Sub

        Private Sub ShapeValue_Click(sender As System.Object, e As System.EventArgs) Handles m_tsbnValues.Click
            If (Me.Handler IsNot Nothing) Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Modify)
        End Sub

        Private Sub LoadShape_Click(sender As System.Object, e As System.EventArgs)
            If (Me.Handler IsNot Nothing) Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Load)
        End Sub

        Private Sub SaveShape_Click(sender As System.Object, e As System.EventArgs) Handles m_tsbnSaveAsImage.Click
            If (Me.Handler IsNot Nothing) Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.SaveAsImage)
        End Sub

        Private Sub tsbChangeShape_Click(sender As System.Object, e As System.EventArgs) Handles m_tsbnChangeShape.Click
            If (Me.Handler IsNot Nothing) Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.ChangeShape)
        End Sub

        Private Sub ShapeOptions_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnOptions.Click
            If (Me.Handler IsNot Nothing) Then Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.DisplayOptions)
        End Sub

        Private Sub OnConvertToLongTerm(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnLongTerm.Click
            If Me.m_bInUpdate Then Return
            If (Me.Handler IsNot Nothing) Then
                Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Seasonal, Nothing, False)
            End If
        End Sub

        Private Sub OnConvertToSeasonal(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnSeasonal.Click
            If Me.m_bInUpdate Then Return
            If (Me.Handler IsNot Nothing) Then
                Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Seasonal, Nothing, True)
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; responds to an [ENTER] key press to apply entered text
        ''' to the grid selection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_tstbWeight_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles m_tstbWeight.KeyDown
            ' Is [ENTER]?
            If e.KeyCode = Keys.Enter Then
                If (Me.Handler IsNot Nothing) Then
                    Dim sWeight As Single = 1.0!
                    Try
                        ' Parse value using UI number settings
                        sWeight = Single.Parse(Me.m_tstbWeight.Text)
                    Catch ex As Exception
                        sWeight = 1.0!
                    End Try
                    Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.SetWeight, Nothing, sWeight)
                End If
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; responds to an [ENTER] key press to apply entered text
        ''' to the grid selection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_tstbMaxValue_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles m_tstbMaxValue.KeyDown
            ' Is [ENTER]?
            If e.KeyCode = Keys.Enter Then
                If (Me.Handler IsNot Nothing) Then
                    Dim sValue As Single = 1.0!
                    Try
                        ' Parse value using UI number settings
                        sValue = Single.Parse(Me.m_tstbMaxValue.Text)
                    Catch ex As Exception
                        sValue = 1.0!
                    End Try
                    Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.SetMaxValue, Nothing, sValue)
                End If
            End If
        End Sub

#End Region ' Event handlers

#Region " Internal implementation "

        Private m_bInUpdate As Boolean = False

        Private Sub UpdateControls()

            If (Me.Handler Is Nothing) Then Return

            Dim shapeSelected As cShapeData = Me.Handler.SelectedShape

            Me.m_bInUpdate = True

            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.DisplayOptions, Me.m_tsbnOptions)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SaveAsImage, Me.m_tsbnSaveAsImage)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.ChangeShape, Me.m_tsbnChangeShape)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Reset, Me.m_tsbnReset)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Modify, Me.m_tsbnValues)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Seasonal, Me.m_tsbnLongTerm)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Seasonal, Me.m_tsbnSeasonal)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SetWeight, Me.m_tslWeight)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SetWeight, Me.m_tstbWeight)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SetMaxValue, Me.m_tslMaxValue)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SetMaxValue, Me.m_tstbMaxValue)

            If (shapeSelected IsNot Nothing) Then
                Me.m_tsbnSeasonal.Checked = shapeSelected.IsSeasonal
                Me.m_tsbnLongTerm.Checked = Not shapeSelected.IsSeasonal
            End If

            If ((shapeSelected IsNot Nothing) And (TypeOf shapeSelected Is cTimeSeries)) Then
                ' #1079
                Dim ts As cTimeSeries = DirectCast(shapeSelected, cTimeSeries)
                If (ts.IsReference) Then
                    Me.m_tstbWeight.Text = Me.m_uic.StyleGuide.FormatNumber(DirectCast(shapeSelected, cTimeSeries).WtType)
                Else
                    Me.m_tstbWeight.Text = ""
                End If
            End If

            If (shapeSelected IsNot Nothing) Then
                Dim sVal As Single = shapeSelected.YMax
                Me.m_tstbMaxValue.Text = CStr(sVal)
            End If

            Me.m_bInUpdate = False

        End Sub

        Private Sub UpdateCommand(cmd As cShapeGUIHandler.eShapeCommandTypes, tsi As ToolStripItem)

            If (Me.m_handler Is Nothing) Then Return
            If Me.m_handler.SupportCommand(cmd) Then
                tsi.Visible = True
                tsi.Enabled = (Me.m_handler.EnableCommand(cmd))
            Else
                tsi.Visible = False
            End If

        End Sub

        Private Sub OnStyleGuideChanged(ct As cStyleGuide.eChangeType)
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace

