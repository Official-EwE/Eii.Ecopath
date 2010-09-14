#Region " Imports "

Option Strict On
Imports EwECore.Ecosim
Imports EwEUtils.Core
Imports EwECore.Ecospace.Advection.cAdvectionManager

#End Region ' Imports

Namespace Ecospace.Advection

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class for calculating advection patterns.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Class cAdvection

#Region " Private vars "

        ''' <summary>Core to operate on.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>Ecospace model to work with.</summary>
        Private m_ecospace As cEcoSpace = Nothing
        ''' <summary>Ecospace data structures to operate on.</summary>
        Private m_data As cEcospaceDataStructures = Nothing

        ''' <summary>Delegate to notify that calculations have started.</summary>
        Private m_RunStartedDelegate As ComputationStartedDelegate
        ''' <summary>Delegate to notify that calculations have progressed through another iteration.</summary>
        Private m_RunProgressDelegate As ComputationProgressDelegate
        ''' <summary>Delegate to notify that calculations have ended.</summary>
        Private m_RunCompletedDelegate As ComputationCompletedDelegate

        ''' <summary>Iteration counter.</summary>
        Private m_iter As Integer = 0
        ''' <summary>Iteration interrupt flag.</summary>
        Private m_bInterrupted As Boolean = False
        ''' <summary>Iteration results quality flag.</summary>
        Private m_bBadFlow As Boolean = False

#End Region ' Private vars

#Region " Public access "

        Public Sub Init(ByVal core As cCore, ByVal ecospace As cEcoSpace)
            Me.m_core = core
            Me.m_data = core.m_EcoSpaceData
            Me.m_ecospace = ecospace
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the current iteration that has been computed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Iteration() As Integer
            Get
                Return Me.m_iter
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the calculations should be interrupted.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Interrupted() As Boolean
            Get
                Return Me.m_bInterrupted
            End Get
            Set(ByVal value As Boolean)
                Me.m_bInterrupted = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether the calculations produced bad flows.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property BadFlow() As Boolean
            Get
                Return Me.m_bBadFlow
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the delegate to call when a computations have started.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property RunStartedCallBack() As ComputationStartedDelegate
            Get
                Return Me.m_RunStartedDelegate
            End Get
            Set(ByVal value As ComputationStartedDelegate)
                Me.m_RunStartedDelegate = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the delegate to call when a new iteration has been calculated.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ProgressCallback() As ComputationProgressDelegate
            Get
                Return Me.m_RunProgressDelegate
            End Get
            Set(ByVal value As ComputationProgressDelegate)
                Me.m_RunProgressDelegate = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the delegate to call when a computations have completed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property RunCompletedCallback() As ComputationCompletedDelegate
            Get
                Return Me.m_RunCompletedDelegate
            End Get
            Set(ByVal value As ComputationCompletedDelegate)
                Me.m_RunCompletedDelegate = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Run advection computations.
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Run() As Boolean

            Dim Vel(Me.m_data.InRow + 1, Me.m_data.InCol + 1) As Single
            Dim VelNew(Me.m_data.InRow + 1, Me.m_data.InCol + 1) As Single
            Dim XvTot(Me.m_data.InRow + 1, Me.m_data.InCol + 1) As Single
            Dim YvTot(Me.m_data.InRow + 1, Me.m_data.InCol + 1) As Single

            'iterates to find Xvel,Yvel velocity field at cell right boundaries
            'xvel(i,j) is velocity of flow from cell i,j to cell i,j+1
            'yvel(i,j) is velocity of flow from cell i,j to cell i+1,j
            Dim i As Integer, j As Integer, Differ As Single
            Dim Th As Single, Tn As Single, jj As Integer
            '   ReDim XvLoc(Inrow + 1, Incol + 1) As Single, YvLoc(Inrow + 1, Incol + 1) As Single
            Dim XvelBase As Single, YvelBase As Single
            Dim xMax As Single = 0

            Dim Grav As Single = CSng(9.8 * 60 * 60 * 24 * 365 / (1000 * Me.m_data.CellLength))
            Dim Upwell As Single = CSng(36.5 * Me.m_data.CellLength / 3) 'value of 6000*celllength
            Dim Hstress As Single = Math.Min(0.2!, 1 / Me.m_data.CellLength)

            Try
                Me.m_RunStartedDelegate.Invoke()
            Catch ex As Exception

            End Try

            'set boundary flow depths and intial velocity field
            Me.m_ecospace.initSpatialEquilibrium()

            XvelBase = Me.m_data.XVelocity
            YvelBase = Me.m_data.YVelocity

            If Math.Abs(Me.m_data.XVelocity) > Math.Abs(Me.m_data.YVelocity) Then xMax = 2 * Math.Abs(Me.m_data.XVelocity) Else xMax = 2 * Math.Abs(Me.m_data.YVelocity)
            If xMax = 0 Then xMax = 1
            For i = 0 To Me.m_data.InRow + 1
                For j = 0 To Me.m_data.InCol + 1
                    Vel(i, j) = 0 '1 - i * yvel - j * Xvel
                    '      XvLoc(i, j) = Me.m_data.XVelocity
                    '      YvLoc(i, j) = Me.m_data.YVelocity
                Next
            Next

            ' Get ready for new run
            m_iter = 0
            m_bBadFlow = False
            m_bInterrupted = False

            Do While m_iter < 10000 And m_bInterrupted = False
                m_iter = m_iter + 1
                Differ = 0

                Try

                    SetVtot(XvTot, YvTot, Me.m_data.Coriolis, Hstress)

                    For i = 1 To Me.m_data.InRow
                        For jj = 1 To Me.m_data.InCol
                            j = Me.m_data.jord(jj)
                            If Me.m_data.Depth(i, j) > 0 Then
                                Th = 0 : Tn = 0
                                ' For ii = i - 1 To i + 1 Step 2
                                '     If Depth(ii, j) > 0 Then
                                '         th = th + Vel(ii, j) ' - (ii - i) * YvLoc(ii, j)
                                '         Tn = Tn + 1
                                '     End If
                                ' Next
                                ' For jj = j - 1 To j + 1 Step 2
                                '     If Depth(i, jj) > 0 Then
                                '         th = th + Vel(i, jj) ' - (jj - j) * XvLoc(i, jj)
                                '         Tn = Tn + 1
                                '     End If
                                ' Next
                                ' th = Grav * th

                                If Me.m_data.Depth(i, j - 1) > 0 Then
                                    Th = Th + Me.m_data.DepthX(i, j - 1) * (XvTot(i, j - 1) + Grav * Vel(i, j - 1))
                                    Tn = Tn + Me.m_data.DepthX(i, j - 1)
                                End If
                                If Me.m_data.Depth(i, j + 1) > 0 Then
                                    Th = Th + Me.m_data.DepthX(i, j) * (Grav * Vel(i, j + 1) - XvTot(i, j))
                                    Tn = Tn + Me.m_data.DepthX(i, j)
                                End If
                                If Me.m_data.Depth(i - 1, j) > 0 Then
                                    Th = Th + Me.m_data.DepthY(i - 1, j) * (YvTot(i - 1, j) + Grav * Vel(i - 1, j))
                                    Tn = Tn + Me.m_data.DepthY(i - 1, j)
                                End If
                                If Me.m_data.Depth(i + 1, j) > 0 Then
                                    Th = Th + Me.m_data.DepthY(i, j) * (Grav * Vel(i + 1, j) - YvTot(i, j))
                                    Tn = Tn + Me.m_data.DepthY(i, j)
                                End If
                                Tn = Tn * Grav + Upwell * Me.m_data.DepthA(i, j)
                                If Tn > 0 Then
                                    VelNew(i, j) = Th / Tn
                                Else
                                    VelNew(i, j) = 0
                                End If
                                Differ = Differ + Math.Abs(Vel(i, j) - VelNew(i, j)) ' * Depth(i, j)
                                Vel(i, j) = VelNew(i, j)
                            End If
                        Next
                    Next
                    '                GoTo skipiter
                    '                For i = 1 To Me.m_data.InRow
                    '                    For j = 1 To Me.m_data.InCol
                    '                        If Me.m_data.Depth(i, j) > 0 Then
                    '                            Vel(i, j) = (1 - Me.m_data.SorWv) * Vel(i, j) + Me.m_data.SorWv * VelNew(i, j)
                    '                        End If
                    '                    Next
                    '                Next
                    'skipiter:

                    ' Verify_JS 14Sep2010: Should SorWv not be Ecospace W (SOR)?
                    Me.SetVelocities(Vel, Me.m_data.SorWv, Grav, Upwell, XvTot, YvTot)

                Catch ex As Exception
                    ' Computation error
                    Return False
                End Try

                Try
                    Me.m_RunProgressDelegate.Invoke(m_iter)
                Catch ex As Exception
                    Return False
                End Try

                If Differ < 0.0000001 * xMax / Grav / Me.m_data.CellLength Then Exit Do

                Threading.Thread.Sleep(100)

            Loop

            'check velocity field
            For i = 1 To Me.m_data.InRow
                For j = 1 To Me.m_data.InCol
                    If Me.m_data.Depth(i, j) > 0 Then
                        Th = Me.m_data.Xvel(i, j - 1) - Me.m_data.Xvel(i, j) + Me.m_data.Yvel(i - 1, j) - Me.m_data.Yvel(i, j) - Upwell * Me.m_data.DepthA(i, j) * Vel(i, j)
                        If Math.Abs(Th) > 0.00001 * xMax Then
                            m_bBadFlow = True ':        Stop
                            'map.Line (j, i)-Step(1, 1), QBColor(4), BF
                        End If
                    End If
                Next
            Next

            Try
                Me.m_RunCompletedDelegate.Invoke(Me.m_iter, Me.m_bInterrupted, Me.m_bBadFlow)
            Catch ex As Exception
                Return False
            End Try

            Return True

            'If m_bBadAdvection = True Then MsgBox("Inflows and outflows do not balance at cells shown in red; recommend not using this velocity field for simulations if ecospace shows strange behavior for these cells")
        End Function

#End Region ' Public access

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="XvTot"></param>
        ''' <param name="YvTot"></param>
        ''' <param name="Corio"></param>
        ''' <param name="Hstress"></param>
        ''' -------------------------------------------------------------------
        Private Sub SetVtot(ByVal XvTot(,) As Single, ByVal YvTot(,) As Single, ByVal Corio As Single, ByVal Hstress As Single)
            'sets total pressure in x and y directions for all cells
            Dim i As Integer, j As Integer
            For i = 0 To Me.m_data.InRow + 1
                For j = 0 To Me.m_data.InCol + 1
                    If Me.m_data.Depth(i, j) > 0 Then
                        XvTot(i, j) = Me.m_data.Xvloc(i, j)
                        YvTot(i, j) = Me.m_data.Yvloc(i, j)
                    End If
                Next
            Next
            'add force components due to horizontal shear along box sides
            For i = 1 To Me.m_data.InRow
                For j = 1 To Me.m_data.InCol
                    If Me.m_data.Depth(i, j) > 0 Then
                        XvTot(i, j) = CSng(XvTot(i, j) - Corio * Me.m_data.Yvel(i, j) + Hstress * (Me.m_data.Xvel(i - 1, j) + Me.m_data.Xvel(i + 1, j) - 2.0# * Me.m_data.Xvel(i, j)))
                        YvTot(i, j) = CSng(YvTot(i, j) + Corio * Me.m_data.Xvel(i, j) + Hstress * (Me.m_data.Yvel(i, j - 1) + Me.m_data.Yvel(i, j + 1) - 2.0# * Me.m_data.Yvel(i, j)))
                    End If
                Next
            Next
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vel"></param>
        ''' <param name="SorWv"></param>
        ''' <param name="Grav"></param>
        ''' <param name="UpWell"></param>
        ''' <param name="XvToT"></param>
        ''' <param name="YvTot"></param>
        ''' -------------------------------------------------------------------
        Private Sub SetVelocities(ByRef vel(,) As Single, _
                                  ByVal SorWv As Single, ByVal Grav As Single, ByVal UpWell As Single, _
                                  ByVal XvToT(,) As Single, ByVal YvTot(,) As Single)
            Dim i As Integer
            Dim j As Integer
            For i = 0 To Me.m_data.InRow
                For j = 0 To Me.m_data.InCol
                    If Me.m_data.Depth(i, j) > 0 Then
                        If Me.m_data.Depth(i, j + 1) > 0 Then Me.m_data.Xvel(i, j) = (1 - SorWv) * Me.m_data.Xvel(i, j) + SorWv * Me.m_data.DepthX(i, j) * (XvToT(i, j) + Grav * (vel(i, j) - vel(i, j + 1))) Else Me.m_data.Xvel(i, j) = 0
                        If Me.m_data.Depth(i + 1, j) > 0 Then Me.m_data.Yvel(i, j) = (1 - SorWv) * Me.m_data.Yvel(i, j) + SorWv * Me.m_data.DepthY(i, j) * (YvTot(i, j) + Grav * (vel(i, j) - vel(i + 1, j))) Else Me.m_data.Yvel(i, j) = 0
                        Me.m_data.UpVel(i, j) = -UpWell * Me.m_data.DepthA(i, j) * vel(i, j)
                    Else
                        Me.m_data.Xvel(i, j) = 0
                        Me.m_data.Yvel(i, j) = 0
                    End If
                Next
            Next
        End Sub

#End Region ' Internals

    End Class

End Namespace
