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
    ''' handling fishing effort <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public MustInherit Class cFishingBaseShapeGUIHandler
        : Inherits cForcingShapeGUIHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to allow use of specific <see cref="eShapeCommandTypes">commands</see>.
        ''' </summary>
        ''' <param name="cmd">The command that is queried.</param>
        ''' <returns>True if the queried command is supported.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean
            Select Case cmd
                Case eShapeCommandTypes.SetToZero
                    Return True
                Case eShapeCommandTypes.SetValue
                    Return True
                Case eShapeCommandTypes.Reset, _
                     eShapeCommandTypes.ResetAll
                    Return True
                Case eShapeCommandTypes.Modify
                    Return True
            End Select
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enable fishing effort shape specific commands.
        ''' </summary>
        ''' <param name="cmd">The command that is queried.</param>
        ''' <returns>True if the queried command may be enabled.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function EnableCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean

            Dim bHasSelection As Boolean = (Me.SelectedShapes IsNot Nothing)
            Dim bHasSingleSelection As Boolean = (Me.SelectedShape IsNot Nothing)

            Select Case cmd

                Case eShapeCommandTypes.ResetAll
                    Return True

                Case eShapeCommandTypes.Modify
                    Return bHasSingleSelection

                Case eShapeCommandTypes.Reset, _
                     eShapeCommandTypes.SetToZero, _
                     eShapeCommandTypes.SetValue
                    Return bHasSelection

            End Select
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to execute a given command by this handler. 
        ''' Overridden to implement fishing forcing function commands.
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <param name="ashapes">The <see cref="EwECore.cShapeData">shapes</see> to apply the command to.</param>
        ''' <param name="data">Optional data to accompany the command.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub ExecuteCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes, _
                    Optional ByVal ashapes As EwECore.cShapeData() = Nothing, _
                    Optional ByVal data As Object = Nothing)

            If (ashapes Is Nothing) Then ashapes = Me.SelectedShapes
            Select Case cmd

                Case eShapeCommandTypes.Reset

                    If (data IsNot Nothing) Then
                        MyBase.ResetShapes(ashapes, CSng(data))
                    Else
                        Me.ResetShapePrompted(ashapes)
                    End If

                Case Else
                    MyBase.ExecuteCommand(cmd, ashapes, data)

            End Select
        End Sub

        Protected Overrides Sub ResetShapes(ByVal ashapes As cShapeData(), _
                Optional ByVal sDefaultValue As Single = 1.0!)

            Dim sm As cBaseShapeManager = Nothing
            Dim shape As cShapeData = Nothing
            Dim lShapes As List(Of cShapeData) = Nothing

            If (ashapes Is Nothing) Then
                sm = Me.ShapeManager
                lShapes = New List(Of cShapeData)
                For Each shape In sm
                    lShapes.Add(shape)
                Next
                ashapes = lShapes.ToArray()
            End If

            For iShape As Integer = 0 To ashapes.Length - 1
                shape = ashapes(iShape)
                If shape IsNot Nothing Then
                    shape.LockUpdates()
                    For i As Integer = 0 To shape.XMax ' - 1'jb why the minus one
                        shape.ShapeData(i) = sDefaultValue
                    Next i
                    shape.UnlockUpdates(True)
                End If
            Next

            Me.SelectedShapes = Me.SelectedShapes
        End Sub

        Protected Overrides Sub ResetAllShapes()
            Me.Core.FishingEffortShapeManager.ResetToDefaults()
            Me.Core.FishMortShapeManager.ResetToDefaults()
        End Sub

        Protected MustOverride Function ScaleMode() As eAxisTickmarkDisplayModeTypes

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ucSketchPad">Sketch pad control</see> to manage
        ''' by this handler. Overridden to fix some behaviours of this control
        ''' particular to displaying fishing shapes.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property SketchPad() As ucSketchPad
            Get
                Return MyBase.SketchPad
            End Get
            Set(ByVal value As ucSketchPad)
                MyBase.SketchPad = value
                If value IsNot Nothing Then
                    If (TypeOf value Is ucForcingSketchPad) Then
                        DirectCast(value, ucForcingSketchPad).AxisTickMarkDisplayMode = Me.ScaleMode()
                    End If
                End If
            End Set
        End Property

#Region " Internals "

        Private Sub ResetShapePrompted(ByVal ashapes As cShapeData())

            Dim strCaption As String = My.Resources.RUN_ECOSIM_F_VALUE_CAPTION
            Dim strMessage As String = My.Resources.RUN_ECOSIM_F_VALUE_MSG
            Dim strDefault As String = "1"
            Dim strValue As String = String.Empty

            ' Sanity check
            If ashapes Is Nothing Then Return

            strValue = Interaction.InputBox(strMessage, strCaption, strDefault)

            'User clicks OK
            If strValue.Length <> 0 Then

                Dim astrEntered As String() = strValue.Split(CChar(" "))

                ' One character entered?
                If astrEntered.Length = 1 Then
                    ' #Yes: duplicate this char over the entire shape
                    Try
                        Me.ResetShapes(ashapes, CSng(Val(astrEntered(0))))
                    Catch ex As Exception
                        Me.Core.Messages.SendMessage(New cMessage(String.Format("Failed to set value {0}", astrEntered(0)), _
                                eMessageType.NotSet, eCoreComponentType.ShapesManager, eMessageImportance.Warning))
                    End Try

                ElseIf astrEntered.Length > 1 Then

                    For Each shape As cShapeData In ashapes

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

                    Next

                End If
            End If
        End Sub

#End Region ' Internals

    End Class

End Namespace
