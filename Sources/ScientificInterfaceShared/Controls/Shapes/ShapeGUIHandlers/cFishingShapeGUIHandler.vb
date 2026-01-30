' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Definitions



Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling fishing effort <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)>
    Public MustInherit Class cFishingBaseShapeGUIHandler
        Inherits cForcingShapeGUIHandler

        Public Sub New(uic As cUIContext)
            MyBase.New(uic)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to allow use of specific <see cref="eShapeCommandTypes">commands</see>.
        ''' </summary>
        ''' <param name="cmd">The command that is queried.</param>
        ''' <returns>True if the queried command is supported.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportCommand(cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean
            Select Case cmd
                Case eShapeCommandTypes.SetToZero,
                     eShapeCommandTypes.SetToValue,
                     eShapeCommandTypes.SetToEcopathBaseline
                    Return True

                Case eShapeCommandTypes.Reset,
                     eShapeCommandTypes.ResetAll
                    Return True

                Case eShapeCommandTypes.Modify
                    Return True

                Case eShapeCommandTypes.FilterName
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
        Public Overrides Function EnableCommand(cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean

            Dim bHasSelection As Boolean = (Me.SelectedShapes IsNot Nothing)
            Dim bHasSingleSelection As Boolean = (Me.SelectedShape IsNot Nothing)

            Select Case cmd

                Case eShapeCommandTypes.ResetAll
                    Return True

                Case eShapeCommandTypes.Modify
                    Return bHasSingleSelection

                Case eShapeCommandTypes.Reset,
                     eShapeCommandTypes.SetToZero,
                     eShapeCommandTypes.SetToValue,
                     eShapeCommandTypes.SetToEcopathBaseline
                    Return bHasSelection

                Case eShapeCommandTypes.FilterName
                    Return True

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
        Public Overrides Sub ExecuteCommand(cmd As cShapeGUIHandler.eShapeCommandTypes,
                    Optional ashapes As EwECore.cShapeData() = Nothing,
                    Optional data As Object = Nothing)

            If (ashapes Is Nothing) Then ashapes = Me.SelectedShapes
            Select Case cmd

                Case eShapeCommandTypes.SetToEcopathBaseline
                    Me.SetToBaseline(ashapes)

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

        Protected Overrides Sub ResetShapes(ashapes As cShapeData(),
                Optional sDefaultValue As Single = 1.0!)

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
                    For i As Integer = 0 To shape.nPoints ' - 1'jb why the minus one
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
            Set(value As ucSketchPad)
                MyBase.SketchPad = value
                If value IsNot Nothing Then
                    If (TypeOf value Is ucForcingSketchPad) Then
                        DirectCast(value, ucForcingSketchPad).AxisTickMarkDisplayMode = Me.ScaleMode()
                    End If
                End If
            End Set
        End Property

        Protected Sub SetToBaseline(ashapes As cShapeData())

            Dim man As cFishingBaseShapeManager = Nothing
            Dim sBaseValue As Single = 0.0

            For Each shape As cShapeData In ashapes
                ' Repeat values across shape
                man = DirectCast(Me.ShapeManager, cFishingBaseShapeManager)
                sBaseValue = man.EcopathBaseValue(shape.Index)

                shape.LockUpdates()
                For iTime As Integer = 0 To shape.nPoints
                    shape.ShapeData(iTime) = sBaseValue
                Next
                shape.UnlockUpdates()
            Next
            Me.SelectedShapes = Me.SelectedShapes

        End Sub

    End Class

End Namespace
