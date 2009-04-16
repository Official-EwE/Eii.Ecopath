'==============================================================================
'
' $Log: ucShapeToolboxToolbar.vb,v $
' Revision 1.7  2009/04/16 17:49:00  jeroens
' m_!
'
' Revision 1.6  2009/04/12 22:14:18  jeroens
' Initial state correctly processed
'
' Revision 1.5  2009/03/24 14:08:23  jeroens
' Uses moved cToolstripUtils
'
' Revision 1.4  2009/03/20 17:55:42  jeroens
' Shape controls are multiple selection
'
' Revision 1.3  2009/03/02 20:08:02  jeroens
' Defaults -> ResetAll
'
' Revision 1.2  2009/03/02 02:04:15  jeroens
' Properly named handlers
'
' Revision 1.1  2008/12/15 15:36:41  jeroens
' Moved from ScInt
'
' Revision 1.2  2008/11/05 05:09:08  jeroens
' Apply -> Weight
'
' Revision 1.1  2008/09/26 07:31:43  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Imports EwEUtils.Commands

Namespace Controls

    <CLSCompliant(True)> _
    Public Class ucShapeToolboxToolbar

#Region " Private vars "

        Private m_handler As cShapeGUIHandler = Nothing

#End Region ' Private vars

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
            Set(ByVal handler As cShapeGUIHandler)
                Me.m_handler = handler
                Me.UpdateControls()
            End Set
        End Property

#End Region

#Region " Public interfaces "

        Public Overrides Sub Refresh()
            MyBase.Refresh()
            Me.UpdateControls()
        End Sub

#End Region ' Public interfaces

#Region " Helper methods "

        Protected Overrides Sub OnVisibleChanged(ByVal e As System.EventArgs)
            Me.UpdateControls()
        End Sub

        Private Sub UpdateControls()

            If (Me.m_handler Is Nothing) Then Return

            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Add, Me.m_tsbAdd)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Weight, Me.m_tsbWeight)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Duplicate, Me.m_tsbDuplicate)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Load, Me.m_tsbLoad)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Import, Me.m_tsbImport)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.Remove, Me.m_tsbRemove)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.ResetAll, Me.m_tsbResetAll)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SetToZero, Me.m_tsbSetTo0)
            Me.UpdateCommand(cShapeGUIHandler.eShapeCommandTypes.SetValue, Me.m_tsbSetToValue)

            cToolstripUtils.HideRepeatingSeparators(Me.m_ts)

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

#End Region ' Helper methods

#Region " Event handlers "

        Private Sub tsbAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbAdd.Click
            If (Me.m_handler Is Nothing) Then Return
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Add)
        End Sub

        Private Sub tsbDuplicate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbDuplicate.Click
            If (Me.m_handler Is Nothing) Then Return
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Duplicate)
        End Sub

        Private Sub tsbRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbRemove.Click
            If (Me.m_handler Is Nothing) Then Return
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Remove)
        End Sub

        Private Sub tsbWeight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbWeight.Click

            If (Me.m_handler Is Nothing) Then Return
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Weight)

        End Sub

        Private Sub tsbLoad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbLoad.Click

            If (Me.m_handler Is Nothing) Then Return
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Load)

        End Sub

        Private Sub tsbImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbImport.Click

            If (Me.m_handler Is Nothing) Then Return
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Import)

        End Sub

        Private Sub m_tsbResetAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tsbResetAll.Click

            If (Me.m_handler Is Nothing) Then Return
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.ResetAll)

        End Sub

        Private Sub tsbSetTo0_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tsbSetTo0.Click

            If Me.m_handler IsNot Nothing Then
                Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Reset, Me.m_handler.SelectedShapes, 0.0!)
            End If

        End Sub

        Private Sub tsbSetToValue_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tsbSetToValue.Click

            Dim strCaption As String = My.Resources.RUN_ECOSIM_F_VALUE_CAPTION
            Dim strMessage As String = My.Resources.RUN_ECOSIM_F_VALUE_MSG
            Dim strDefault As String = "1"
            Dim strValue As String = String.Empty
            Dim shape As EwECore.cShapeData = Nothing

            ' Sanity check
            If Me.m_handler Is Nothing Then Return
            If Me.m_handler.SelectedShapes Is Nothing Then Return

            strValue = Interaction.InputBox(strMessage, strCaption, strDefault)

            'User clicks OK
            If strValue.Length <> 0 Then

                Dim astrEntered As String() = strValue.Split(CChar(" "))

                ' One character entered?
                If astrEntered.Length = 1 Then
                    ' #Yes: duplicate this char over the entire shape
                    Try
                        Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Reset, _
                            Me.m_handler.SelectedShapes, CSng(Val(astrEntered(0))))
                    Catch ex As Exception
                    End Try

                ElseIf astrEntered.Length > 1 Then

                    ' Translate individual values
                    Dim asValues(shape.XMax) As Single
                    Dim sValue As Single = 0.0!

                    For i As Integer = 0 To shape.XMax
                        If (i < (astrEntered.Length - 1)) Then
                            Try
                                sValue = CSng(Val(astrEntered(i)))
                            Catch ex As Exception
                                sValue = -1
                            End Try
                        End If
                        asValues(i) = sValue
                    Next

                    shape.LockUpdates()
                    shape.ShapeData = asValues
                    shape.UnlockUpdates()

                End If
            End If
        End Sub

#End Region ' Event handlers

    End Class

End Namespace
