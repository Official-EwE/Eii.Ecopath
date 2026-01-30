' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Threading
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cIBMSolver

    Private Const CELL_BOUNDS As Single = 0.99F

    ''' <summary>
    ''' Signal mechanism used by the calling thread for thread Synchronization
    ''' </summary>
    ''' <remarks>
    ''' When the Solve() thread is running (SignalState in a non-signaled state SignalState.Reset()) 
    ''' calls to SignalState.WaitOne() will block until Solve() has completed (SignalState in a signaled state SignalState.Set())
    ''' </remarks>
    Public SignalState As New ManualResetEvent(True)

    ''' <summary>
    ''' Delegate for posting error messages.
    ''' </summary>
    ''' <remarks>
    ''' All error handling must be done on the same thread. Errors can not be thrown from one thread to another.
    ''' A delegate must be used to cross the thread boundary. EcospaceErrorHandler is a delegate to a sub on the main Ecospace thread.
    ''' </remarks>
    Public EcospaceErrorHandler As cEcoSpace.SolverErrorDelegate

    Public isOkToRun As Boolean
    Public ThreadID As Integer

    'references
    Public m_EcospaceModel As cEcoSpace
    Public m_Data As cEcospaceDataStructures
    Public m_ESData As cEcosimDatastructures
    Public m_Stanza As cStanzaDatastructures
    Public m_Ecosim As Ecosim.cEcosimModel

    Public Bcw(,,) As Single
    Public C(,,) As Single
    Public d(,,) As Single
    Public e(,,) As Single
    Public Cper(,,) As Single

    Public iFirstPacket As Integer
    Public iLastPacket As Integer
    Public BcellThread(,,) As Single
    Public PredCellThread(,,) As Single

    Public threadTime1 As Single
    Public threadTime2 As Single
    Public threadTimeMove As Single

    Private m_rand As Random
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cIBMSolver)()


    Public Sub Init()

    End Sub

#Region "Public 'Solve'"

    ''' <summary>
    ''' This is the method that the ThreadPool calls. 
    ''' It must have the object argument to match the Delegate signature required by ThreadPool.QueueUserWorkItem()
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub runMovePackets(obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)

        'if this is running on a thread this may not work
        'all flags need to be set outside the thread
        Me.isOkToRun = False
        Try
            'set signal state to 'non-signaled' SignalState.WaitOne() will block
            Me.SignalState.Reset()
            Dim iPacket As Integer

            'do the processing here
            For iPacket = Me.iFirstPacket To Me.iLastPacket
                'now do the computations
                'GrowSurvivePackets(iGrp) 'this is called outside now
                Me.MovePackets(iPacket)
                Me.UpDateBcellIBM(iPacket)
            Next iPacket

            'thread has finished it is ok to run this again
            Me.isOkToRun = True
            'set signal state to 'signaled' 
            'the processing has finished SignalState.WaitOne() will return immediately
            Me.SignalState.Set()

        Catch ex As Exception
            m_logger.LogError(ex, "runMovePackets. Ecospace IBM Solver Thread {ThreadID} encountered an error", Me.ThreadID)

            'prevent this thread from blocking forever if it throws an error
            Me.SignalState.Set()
            Me.isOkToRun = True

            'tell the main thread that this solver has had a problem
            'If EcospaceErrorHandler IsNot Nothing Then
            'Me.EcospaceErrorHandler(Me.ThreadID, ex.Message)
            'Else
            Debug.Assert(False, ex.Message)
            'End If

        End Try

    End Sub

    Public Sub runGrowSurvivePackets(obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)

        'if this is running on a thread this may not work
        'all flags need to be set outside the thread
        Me.isOkToRun = False
        Try
            'set signal state to 'non-signaled' SignalState.WaitOne() will block
            Me.SignalState.Reset()

            'do the processing here
            For i As Integer = 1 To Me.m_Stanza.Nsplit
                If Me.m_Data.nIBMGroupsPerThread(Me.ThreadID, i) = 0 Then Exit For
                'Console.WriteLine("Thread " & Me.ThreadID & " growing isp " & Me.m_Data.nIBMGroupsPerThread(Me.ThreadID, i))
                Me.GrowSurvivePackets(Me.m_Data.nIBMGroupsPerThread(Me.ThreadID, i))
            Next i

            'thread has finished it is ok to run this again
            Me.isOkToRun = True
            'set signal state to 'signaled' 
            'the processing has finished SignalState.WaitOne() will return immediately
            Me.SignalState.Set()

        Catch ex As Exception
            m_logger.LogError(ex, "runGrowSurvivePackets. Ecospace IBM Solver Thread {ThreadID} encountered an error", Me.ThreadID)

            'prevent this thread from blocking forever if it throws an error
            Me.SignalState.Set()
            Me.isOkToRun = True

            'tell the main thread that this solver has had a problem
            If Me.EcospaceErrorHandler IsNot Nothing Then
                Me.EcospaceErrorHandler(Me.ThreadID, ex.Message)
            Else
                Debug.Assert(False, ex.Message)
            End If

        End Try

    End Sub


#End Region

    Sub MovePackets(ip As Integer)
        'IBM model routine to move packets over spatial grid using orientation information from the ecospace instantaneous movement arrays
        'uses moves per month (IBMMovesPerMonth) and distance per move (IBMDistMove) calculated from ecospace stanza information in InitPackets
        Dim ist As Integer, ieco As Integer, ia As Integer, iaa As Integer, imm As Integer
        Dim i As Integer
        Dim j As Integer
        Dim Dmove As Single, Nmoves As Integer, isp As Integer
        Dim Mrat As Single, Ipos As Single, Jpos As Single
        Dim aa As Single, bb As Single, cc As Single, dd As Single
        Dim dAllow As Single
        'jb Just the conversion from cm/s to km/y
        'The cell length conversion comes later
        Dim AdScale = 315.36 ' / Me.m_Data.CellLength

        Try
            For isp = 1 To Me.m_Stanza.Nsplit
                For iaa = 0 To Me.m_Stanza.MaxAgeSpecies(isp)
                    ia = Me.m_Stanza.AgeIndex1(isp) + iaa
                    If ia > Me.m_Stanza.MaxAgeSpecies(isp) Then
                        ia = ia - Me.m_Stanza.MaxAgeSpecies(isp) - 1
                    End If

                    ist = Me.m_Stanza.StanzaNo(isp, ia)
                    ieco = Me.m_Stanza.EcopathCode(isp, ist)

                    If Me.m_Data.MovePacketsAtStanzaEntry Then
                        'Move packets into good habitat 
                        'as they enter the next stanza group
                        If Math.Abs(ia - Me.m_Stanza.Age1(isp, ist)) < 2 Then

                            i = Math.Truncate(Me.m_Stanza.iPacket(isp, iaa, ip))
                            j = Math.Truncate(Me.m_Stanza.jPacket(isp, iaa, ip))

                            If Me.HabIsOk(ieco, i, j) = False And Me.m_Data.ItoUse(isp, ist, i, j) <> 0 Then
                                'System.Console.WriteLine("Moving Stanza " & isp.ToString & " Group " & ist.ToString & " To " & Me.m_Data.ItoUse(isp, ist, i, j).ToString & "," & Me.m_Data.JtoUse(isp, ist, i, j).ToString)
                                Me.m_Stanza.iPacket(isp, iaa, ip) = Me.m_Data.ItoUse(isp, ist, i, j) + Me.m_rand.NextDouble
                                Me.m_Stanza.jPacket(isp, iaa, ip) = Me.m_Data.JtoUse(isp, ist, i, j) + Me.m_rand.NextDouble
                                'Bounds Checking
                                If Me.m_Stanza.iPacket(isp, iaa, ip) > Me.m_Data.InRow + CELL_BOUNDS Then Me.m_Stanza.iPacket(isp, ia, ip) = Me.m_Data.InRow + CELL_BOUNDS
                                If Me.m_Stanza.jPacket(isp, iaa, ip) > Me.m_Data.InCol + CELL_BOUNDS Then Me.m_Stanza.jPacket(isp, ia, ip) = Me.m_Data.InCol + CELL_BOUNDS

                            End If

                        End If 'Math.Abs(ia - m_Stanza.Age1(isp, ist)) < 2
                    End If 'Me.m_Data.MovePacketsAtStanzaEntry

                    Mrat = Me.m_Data.Mrate(ieco)
                    Dmove = Me.m_Stanza.IBMdistmove(isp, ia)
                    dAllow = Dmove + 0.0001
                    i = Math.Truncate(Me.m_Stanza.iPacket(isp, iaa, ip)) : j = Math.Truncate(Me.m_Stanza.jPacket(isp, iaa, ip))

                    If Me.m_Data.HabCap(ieco)(i, j) > 0.1 And Me.m_Data.Depth(i, j) > 0 Then
                        Nmoves = Me.m_Stanza.IBMMovesPerMonth(ieco) '* relmove
                    Else
                        Dmove = Me.m_Stanza.IBMdistmove(isp, ia)
                        Nmoves = Me.m_Stanza.IBMMovesPerMonth(ieco) * Me.m_Data.RelMoveBad(ieco)
                        'jb non linear movement speed
                        'Nmoves = m_Stanza.IBMMovesPerMonth(ieco) * Math.Log(m_Data.HabCap(ieco)(i, j), 0.1) ^ m_Data.RelMoveBad(ieco)
                    End If

                    'Debug.Assert(ieco <> 1)

                    ' JS 15Dec21: packet movement is ignoring advection speed
                    ' CW 16Dec21: You have to move the calculation of IBMDistMove and nmoves to inside the loop, recalculate IBMDistMove 
                    '             using the cell-specific vector velocity sqrt(vx^2+vy^2+basedist) where vx,vy are the cell-specific 
                    '             x and y advection velocities.  Note that nmove will go up a lot when these velocities are high.
                    '             Look at how IBMDistMove is calculated from the species base movement distance/year to see how to scale
                    '             the calculation when the vector velocity is added to the species base movement distance/year
                    If Me.m_Data.IsAdvected(ieco) Then
                        ' Increase local DMove (not IBMDistMove) with advection velocity vector
                        Dim AdvectDist As Single = Math.Sqrt((Me.m_Data.Xvel(i, j) ^ 2) + (Me.m_Data.Yvel(i, j) ^ 2)) * AdScale
                        'Add distance moved from advection (in km/y) to the base dispersal rate
                        'and calculate the new cell specific Nmoves from that
                        Nmoves = (Me.m_Data.Mvel(ieco) + AdvectDist) / (12 * Me.m_Data.CellLength) * 2.0
                        'this still needs to be modified to check DMove is not < 0.5
                        'if it is it needs to be set to the correct distance to move
                        'See InitPackets()

                    End If

                    If Me.m_Data.IsMigratory(ieco) Then
                        If Me.m_Data.MigMaps(ieco, Me.m_Data.MonthNow)(i, j) > cEcoSpace.MIN_MIG_PROB Then
                            'inside a preferred migration area
                            'use the base movement rate
                            'Nmoves was calculated on the base dispersal rate so it will be correct
                            Mrat = Me.m_Data.Mvel(ieco) / (3.14159 * Me.m_Data.CellLength)
                        Else
                            'outside preferred migration area
                            'increase the number of moves to match the movement rate set by dispersal rates in cEcospace.initSpatialEquilibrium()
                            Nmoves *= Me.m_Data.IBMMigMovRatio(ieco) ' Me.m_Data.Mrate(ieco) / (Me.m_Data.Mvel(ieco) / (3.14159 * Me.m_Data.CellLength))
                        End If
                    End If


                    For imm = 1 To Nmoves

                        ' Q: should nmoves be adapted in this loop if an advected packet traverses cells with a different advection velocity?
                        'Nope to complicated. Treat movement the same way we treat growth, relative to the cell at the start of the time step.
                        'We can't really do that kind of detail at a monthly time step.
                        Try

                            i = Math.Truncate(Me.m_Stanza.iPacket(isp, iaa, ip)) : j = Math.Truncate(Me.m_Stanza.jPacket(isp, iaa, ip))
                            If i <= Me.m_Data.InRow Then
                                aa = Me.Bcw(i + 1, j, ieco)  'south move
                            Else
                                aa = 0.0
                            End If
                            If i >= 1 Then
                                bb = Me.C(i - 1, j, ieco) 'north move
                            Else
                                bb = 0.0
                            End If

                            cc = Me.d(i, j, ieco) 'east move
                            dd = Me.e(i, j, ieco) 'west move

                            If Me.m_Data.HabCap(ieco)(i, j) > 0.1 And Me.m_Data.Depth(i, j) > 0 Then
                                'jb 22-Dec-2021 remove cap on number of moves because advection vectors set Nmoves to local values for this cell
                                'If imm > Me.m_Stanza.IBMMovesPerMonth(ieco) Then Exit For
                                If Me.m_Data.IsMigratory(ieco) = False Then
                                    'this changes movement if it's inside the box s.t. it can't get out in one move
                                    Ipos = Me.m_Stanza.iPacket(isp, iaa, ip) - i : Jpos = Me.m_Stanza.jPacket(isp, iaa, ip) - j
                                    If Ipos < 1.0 - dAllow Then
                                        aa = Mrat
                                    End If
                                    If Ipos > dAllow Then
                                        bb = Mrat
                                    End If
                                    If Jpos < 1.0 - dAllow Then
                                        cc = Mrat
                                    End If
                                    If Jpos > dAllow Then
                                        dd = Mrat
                                    End If
                                End If
                            Else 'Me.m_Data.HabCap(ieco)(i, j) > 0.1 And Me.m_Data.Depth(i, j) > 0
                                'In either low foraging capacity cell or on land 
                                Dmove = Me.m_Stanza.IBMdistmove(isp, ia)
                                Nmoves = Me.m_Stanza.IBMMovesPerMonth(ieco) * Me.m_Data.RelMoveBad(ieco)

                                If Me.m_Data.Depth(i, j) <= 0 Then
                                    'In a land cell
                                    'Increase the distance moved
                                    Dmove = Me.m_Stanza.IBMdistmove(isp, ia) * Me.m_Data.RelMoveBad(ieco)
                                    'Direction preference may not have been set
                                    'this will create a random walk to get out of the land cell
                                    aa = Mrat : bb = Mrat : cc = Mrat : dd = Mrat
                                End If


                            End If

                            Debug.Assert((aa + bb + cc + dd) > 0, "Opps!")
                            Me.MoveThePacket(isp, ieco, iaa, ip, Dmove, aa, bb, cc, dd)

                        Catch ex As Exception
                            Debug.Assert(False, ex.Message)
                        End Try


                        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                        ''for debugging 
                        ''track a single packet
                        'If isp = 1 And iaa = 1 And ip = 1 Then
                        '    System.Console.WriteLine("isp=" + isp.ToString + ", iage=" + iaa.ToString + ", " + m_Stanza.iPacket(isp, iaa, 1).ToString + ", " + m_Stanza.jPacket(isp, iaa, 1).ToString)
                        'End If
                        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                    Next imm
                Next iaa
            Next isp

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Sub MoveThePacket(isp As Integer, ieco As Integer, ia As Integer, ip As Integer, Dmove As Single, aa As Single, bb As Single, cc As Single, dd As Single)
        Dim bMove As Boolean = True
        Dim n As Integer

        bb = bb + aa '+ 0.0000000001
        cc = cc + bb '+ 0.0000000001
        dd = dd + cc '+ 0.0000000001

        Do While bMove
            Dim randMove As Single = Me.m_rand.NextDouble * dd
            'Tns = aa + bb + 0.0000000001 : Tew = cc + dd + 0.0000000001
            If randMove < aa Then 'move south
                Me.m_Stanza.iPacket(isp, ia, ip) = Me.m_Stanza.iPacket(isp, ia, ip) + Dmove
            ElseIf randMove < bb Then 'move north
                Me.m_Stanza.iPacket(isp, ia, ip) = Me.m_Stanza.iPacket(isp, ia, ip) - Dmove
            ElseIf randMove < cc Then 'move east
                Me.m_Stanza.jPacket(isp, ia, ip) = Me.m_Stanza.jPacket(isp, ia, ip) + Dmove
            Else 'move west
                Me.m_Stanza.jPacket(isp, ia, ip) = Me.m_Stanza.jPacket(isp, ia, ip) - Dmove
            End If

            'Bounds check the new position before it gets used
            If Me.m_Stanza.iPacket(isp, ia, ip) < 1 Then Me.m_Stanza.iPacket(isp, ia, ip) = 1
            If Me.m_Stanza.iPacket(isp, ia, ip) > Me.m_Data.InRow + CELL_BOUNDS Then Me.m_Stanza.iPacket(isp, ia, ip) = Me.m_Data.InRow + CELL_BOUNDS
            If Me.m_Stanza.jPacket(isp, ia, ip) < 1 Then Me.m_Stanza.jPacket(isp, ia, ip) = 1
            If Me.m_Stanza.jPacket(isp, ia, ip) > Me.m_Data.InCol + CELL_BOUNDS Then Me.m_Stanza.jPacket(isp, ia, ip) = Me.m_Data.InCol + CELL_BOUNDS

            'Debug.Assert(Me.m_Data.Depth(Me.m_Stanza.iPacket(isp, ia, ip), Me.m_Stanza.jPacket(isp, ia, ip)) > 0.0, "opps I'm on land!")

            'did we land on a water cell
            If ((Me.m_Data.Depth(Math.Truncate(Me.m_Stanza.iPacket(isp, ia, ip)), Math.Truncate(Me.m_Stanza.jPacket(isp, ia, ip))) > 0.0)) Or (n > 10) Then
                'Either in a water cell or 
                bMove = False
            Else
                bMove = True
                Dmove *= Me.m_Data.RelMoveBad(ieco)
                n += 1
                Debug.WriteLine(n.ToString + ", " + m_Stanza.iPacket(isp, ia, ip).ToString + ", " + m_Stanza.jPacket(isp, ia, ip).ToString + ", " + isp.ToString + ", " + ip.ToString)
            End If
        Loop


        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Code from EwE5
        'Dim i As Integer = 0
        'Dim j As Integer = 0
        'Dim Tns As Single = 0.0!
        'Dim Tew As Single = 0.0!
        'aa As Single, bb As Single, CC As Single, dd As Single, 

        'aa = aa + 0.0000000001

        'Tns = aa + bb + 0.0000000001 : Tew = cc + dd + 0.0000000001
        'If Rnd() < Tns / (Tns + Tew) Then 'choose north-south move
        '    If Rnd() < aa / Tns Then 'move south
        '        m_Stanza.iPacket(isp, ia, ip) = m_Stanza.iPacket(isp, ia, ip) + Dmove
        '    ElseIf bb > 0 Then 'move north
        '        m_Stanza.iPacket(isp, ia, ip) = m_Stanza.iPacket(isp, ia, ip) - Dmove
        '    End If
        'Else 'choose east-west move
        '    If Rnd() < cc / Tew Then 'move east
        '        m_Stanza.jPacket(isp, ia, ip) = m_Stanza.jPacket(isp, ia, ip) + Dmove
        '    ElseIf dd > 0 Then 'move west
        '        m_Stanza.jPacket(isp, ia, ip) = m_Stanza.jPacket(isp, ia, ip) - Dmove
        '    End If
        'End If
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


    End Sub

    Sub GrowSurvivePackets(isp As Integer)
        ' IBM model routine to update Npacket and Wpacket numbers and body sizes for multistanza spatial packets
        'this routine is same as SpaceSplitUpdate except for indices and disposition of new recruits
        Dim ist As Integer, ieco As Integer, ia As Integer, iaa As Integer, ip As Integer
        Dim Su As Single, Gf As Single
        Dim Nt As Single = 0.0!
        Dim Agemax As Integer = 0
        Dim AgeMin As Integer = 0
        Dim Be As Single
        Dim i As Integer, j As Integer
        Dim ia1 As Integer, TotRecruits As Single, iNurse As Integer
        Dim Egg As Single
        Dim Te(,) As Single, Xe As Single, XeT As Single
        Dim lstNursery As List(Of Integer)

        'update numbers and body weights
        ieco = Me.m_Stanza.EcopathCode(isp, Me.m_Stanza.Nstanza(isp))
        If Me.m_Ecosim.ResetPred(ieco) = False Then

            Be = 0 'initialize variable to accumulate total egg production by the species for this time step
            For iaa = 0 To Me.m_Stanza.MaxAgeSpecies(isp)
                'set age dependnt on age of fish in first index position for this time step
                ia = Me.m_Stanza.AgeIndex1(isp) + iaa
                If ia > Me.m_Stanza.MaxAgeSpecies(isp) Then
                    ia = ia - Me.m_Stanza.MaxAgeSpecies(isp) - 1
                End If
                If ia = Me.m_Stanza.MaxAgeSpecies(isp) Then
                    ia1 = iaa 'save array element to be overwritten with new recruits
                End If

                ist = Me.m_Stanza.StanzaNo(isp, ia)
                ieco = Me.m_Stanza.EcopathCode(isp, ist)
                'Debug.Assert(ieco <> 1)
                'loop over packets within this age and update numbers,wt dependent on current cell position
                For ip = 1 To Me.m_Stanza.Npackets
                    i = Math.Truncate(Me.m_Stanza.iPacket(isp, iaa, ip))
                    j = Math.Truncate(Me.m_Stanza.jPacket(isp, iaa, ip))
                    Su = Math.Exp(-Me.m_Stanza.Zcell(i, j, ieco) / 12.0#) 'mortality
                    Gf = Me.Cper(i, j, ieco) '(month factor here included in splitalpha scaling setup)

                    'calculate mortality and weight change for the packet
                    Me.m_Stanza.Npacket(isp, iaa, ip) = Me.m_Stanza.Npacket(isp, iaa, ip) * Su
                    Me.m_Stanza.Wpacket(isp, iaa, ip) = Me.m_Stanza.vBM(isp) * Me.m_Stanza.Wpacket(isp, iaa, ip) + Gf * Me.m_Stanza.SplitAlpha(isp, ia)
                    'accumulate contribution of this packet to total egg production by the species
                    Egg = 0
                    If Me.m_Stanza.FixedFecundity(isp) Then
                        Egg = Me.m_Stanza.Npacket(isp, iaa, ip) * Me.m_Stanza.EggsSplit(isp, ia) * Me.m_Stanza.SpawnProp(isp, ist)
                    Else
                        If Me.m_Stanza.Wpacket(isp, iaa, ip) > Me.m_Stanza.WmatWinf(isp) Then
                            Egg = Me.m_Stanza.Npacket(isp, iaa, ip) * (Me.m_Stanza.Wpacket(isp, iaa, ip) - Me.m_Stanza.WmatWinf(isp)) * Me.m_Stanza.SpawnProp(isp, ist)
                        End If
                    End If
                    Be = Be + Egg
                    Me.m_Stanza.EggCell(i, j, isp) = Me.m_Stanza.EggCell(i, j, isp) + Egg
                Next
            Next

            Me.m_Stanza.EggsStanza(isp) = Be

            'update age of fish for first iaa array element
            Me.m_Stanza.AgeIndex1(isp) = Me.m_Stanza.AgeIndex1(isp) + 1
            If Me.m_Stanza.AgeIndex1(isp) > Me.m_Stanza.MaxAgeSpecies(isp) Then
                Me.m_Stanza.AgeIndex1(isp) = 0
            End If


            'finally set abundance at youngest age to recruitment rate
            'WARNING Youngest age is stored in the ia1 index NOT Age 0 as it is in Ecosim
            If Me.m_Stanza.BaseEggsStanza(isp) > 0 Then
                TotRecruits = Me.m_Stanza.RscaleSplit(isp) * Me.m_ESData.tval(Me.m_Stanza.EggProdShapeSplit(isp)) * Me.m_Stanza.RzeroS(isp) * Me.m_ESData.tval(Me.m_Stanza.HatchCode(isp))
                'TotRecruits = m_ESData.tval(m_Stanza.HatchCode(isp))
            End If

            If Me.m_Stanza.HatchCode(isp) = 0 And Me.m_Stanza.BaseEggsStanza(isp) > 0 Then
                'NO hatchery code 
                'apply the RecPowerSplit() and scale up to the total habitat area 
                TotRecruits = TotRecruits * Me.m_Data.ThabArea * (Me.m_Stanza.EggsStanza(isp) / (Me.m_Data.ThabArea * Me.m_Stanza.BaseEggsStanza(isp))) ^ Me.m_Stanza.RecPowerSplit(isp)
            ElseIf Me.m_Stanza.HatchCode(isp) <> 0 And Me.m_Stanza.BaseEggsStanza(isp) > 0 Then
                'YES there is a hatchery code
                'so scale the recruits up to the total area without applying the RecPowerSplit()
                TotRecruits *= Me.m_Data.ThabArea
            End If

            'Are the recruits forced
            If Me.m_Stanza.isForcedIBMRecruits(isp) = True Then

                'Yes recruits are forced 
                'set the nursery cells to the forcing data
                Me.ForceNurseryCells(isp)

            End If 'm_Stanza.isForcedIBMRecruits(ist) = True 

            ' JS 30-Mar-2021: link recruitment
            If Me.m_Stanza.RecStanza(isp) > 0 Then
                Dim sFrom As Single = Me.m_Stanza.IBMTotRecruits(Me.m_Stanza.RecStanza(isp))
                Dim sTo As Single = TotRecruits
                If (Me.m_Stanza.RecStanzaScalar(isp) = 0) Then Me.m_Stanza.RecStanzaScalar(isp) = sTo / sFrom
                ' JS 05Dec21: Joe, how about applying the recruitment power to control linked stanza recruitment?
                TotRecruits = (sFrom * Me.m_Stanza.RecStanzaScalar(isp)) '^ Me.m_Stanza.RecPowerSplit(isp)
            Else
                Me.m_Stanza.IBMTotRecruits(isp) = TotRecruits
            End If

            'distribute the total recruits (totrecruits) over packets and suitable spatial cells for recruitment
            'and set initial body sizes for packets representing new recruits
            For ip = 1 To Me.m_Stanza.Npackets
                Me.m_Stanza.Npacket(isp, ia1, ip) = TotRecruits / Me.m_Stanza.Npackets
                Me.m_Stanza.Wpacket(isp, ia1, ip) = 0.0000000001
            Next

            If Me.m_Stanza.EggAtSpawn(isp) Then
                'distribute juvenile packets in proportion to eggcell distribution
                ReDim Te(Me.m_Data.InRow, Me.m_Data.InCol)
                XeT = 0
                For i = 1 To Me.m_Data.InRow : For j = 1 To Me.m_Data.InCol
                        XeT = XeT + Me.m_Stanza.EggCell(i, j, isp)
                        Te(i, j) = XeT 'cumulative probability distribution
                    Next : Next
                For ip = 1 To Me.m_Stanza.Npackets
                    Xe = Me.m_rand.NextDouble * XeT 'Be
                    For i = 1 To Me.m_Data.InRow
                        For j = 1 To Me.m_Data.InCol
                            If Xe < Te(i, j) Then
                                Me.m_Stanza.iPacket(isp, ia1, ip) = i + Me.m_rand.NextDouble
                                Me.m_Stanza.jPacket(isp, ia1, ip) = j + Me.m_rand.NextDouble
                                Exit For 'have found the packet position
                            End If
                        Next
                        If j < Me.m_Data.InCol + 1 Then Exit For 'have found the packet position, exit i loop as well
                    Next

                    If Me.m_Stanza.iPacket(isp, ia1, ip) < 1 Then Me.m_Stanza.iPacket(isp, ia1, ip) = 1
                    If Me.m_Stanza.iPacket(isp, ia1, ip) > Me.m_Data.InRow + CELL_BOUNDS Then Me.m_Stanza.iPacket(isp, ia1, ip) = Me.m_Data.InRow + CELL_BOUNDS

                    If Me.m_Stanza.jPacket(isp, ia1, ip) < 1 Then Me.m_Stanza.jPacket(isp, ia1, ip) = 1
                    If Me.m_Stanza.jPacket(isp, ia1, ip) > Me.m_Data.InCol + CELL_BOUNDS Then Me.m_Stanza.jPacket(isp, ia1, ip) = Me.m_Data.InCol + CELL_BOUNDS

                Next
            Else
                ''simple model for random distribution of packets over nursery cells for the species
                'For ip = 1 To m_Stanza.Npackets
                '    iNurse = 1 + Me.m_rand.NextDouble * (m_Stanza.Nnursery(isp) - 1)
                '    m_Stanza.iPacket(isp, ia1, ip) = m_Stanza.iNursery(isp, iNurse) + Me.m_rand.NextDouble
                '    m_Stanza.jPacket(isp, ia1, ip) = m_Stanza.jNursery(isp, iNurse) + Me.m_rand.NextDouble
                'Next

                lstNursery = New List(Of Integer)
                'simple model for random distribution of packets over nursery cells for the species'
                'this has been modified to make settlement probs for each nursery cell proportional
                'to the habitat capacities for the cells m_Data.HabCap(i, j, ieco) for approp ieco
                ieco = Me.m_Stanza.EcopathCode(isp, 1)
                For ip = 1 To Me.m_Stanza.Npackets
                    'randomly select the nursery cell
                    'iNurse = 1 + Me.m_rand.NextDouble() * (m_Stanza.Nnursery(isp) - 1)
                    iNurse = Me.m_rand.Next(1, Me.m_Stanza.Nnursery(isp))
                    'randomly select where in the cell to put the packet
                    Me.m_Stanza.iPacket(isp, ia1, ip) = Me.m_Stanza.iNursery(isp, iNurse) + Me.m_rand.NextDouble()
                    Me.m_Stanza.jPacket(isp, ia1, ip) = Me.m_Stanza.jNursery(isp, iNurse) + Me.m_rand.NextDouble()

                    'If the nursery cells are forced 
                    'Don't try to find better habitat. 
                    'Just assume the forcing pattern is correct!
                    If Me.m_Stanza.isForcedIBMRecruits(isp) = False Then

                        'Now randomly move some of the packets again if this is a low quality habitat
                        If Me.m_rand.NextDouble() > Me.m_Data.HabCap(ieco)(Me.m_Stanza.iNursery(isp, iNurse), Me.m_Stanza.jNursery(isp, iNurse)) Then
                            'If Me.m_rand.NextDouble() > Me.m_Data.HabCap(i, j, ieco) Then
                            'try up to 10 alternative locations
                            For icheck As Integer = 1 To 10
                                'iNurse = 1 + Me.m_rand.NextDouble() * (m_Stanza.Nnursery(isp) - 1)
                                iNurse = Me.m_rand.Next(1, Me.m_Stanza.Nnursery(isp)) '
                                If Me.m_rand.NextDouble() < Me.m_Data.HabCap(ieco)(Me.m_Stanza.iNursery(isp, iNurse), Me.m_Stanza.jNursery(isp, iNurse)) Then
                                    Me.m_Stanza.iPacket(isp, ia1, ip) = Me.m_Stanza.iNursery(isp, iNurse) + Me.m_rand.NextDouble()
                                    Me.m_Stanza.jPacket(isp, ia1, ip) = Me.m_Stanza.jNursery(isp, iNurse) + Me.m_rand.NextDouble()
                                    Exit For
                                End If
                            Next icheck
                        End If 'Me.m_rand.NextDouble() > Me.m_Data.HabCap(i, j, ieco)
                    Else
                        'IBM Forced Recruits
                        'remember the nursery cells
                        'so we can populate them with the forced values
                        lstNursery.Add(iNurse)
                    End If ' m_Stanza.isForcedIBMRecruits(isp)

                    'bounds check the nursery cells
                    If Me.m_Stanza.iPacket(isp, ia1, ip) < 1 Then Me.m_Stanza.iPacket(isp, ia1, ip) = 1
                    If Me.m_Stanza.iPacket(isp, ia1, ip) > Me.m_Data.InRow + CELL_BOUNDS Then Me.m_Stanza.iPacket(isp, ia1, ip) = Me.m_Data.InRow + CELL_BOUNDS

                    If Me.m_Stanza.jPacket(isp, ia1, ip) < 1 Then Me.m_Stanza.jPacket(isp, ia1, ip) = 1
                    If Me.m_Stanza.jPacket(isp, ia1, ip) > Me.m_Data.InCol + CELL_BOUNDS Then Me.m_Stanza.jPacket(isp, ia1, ip) = Me.m_Data.InCol + CELL_BOUNDS

                Next ip
            End If ' m_Stanza.EggAtSpawn(isp)


            If Me.m_Stanza.isForcedIBMRecruits(isp) = True Then
                'this stanza is forced
                'populate all the packets in each nursery cell 
                'with the forcing values in that cell

                Me.PopulateForcedNurseryCells(isp, ia1, lstNursery)

            End If 'm_Stanza.isForcedIBMRecruits(ist) = True 

        End If 'm_Ecosim.ResetPred(ieco) = False

    End Sub

    Private Sub PopulateForcedNurseryCells(isp As Integer, ia1 As Integer, lstNursery As List(Of Integer))
        'this stanza is forced
        'populate all the packets in each nursery cell 
        'with the forcing values in that cell
        Dim nAge0 As Single
        Dim irow As Integer, icol As Integer
        Dim npcks As Integer
        Dim sumAge0 As Single
        Dim age0NoScale As Single

        'Get the forcing cells
        'Any relative scaling has already been done
        Dim ForcedCells(,) As Single = Me.m_Stanza.IBMForcedCells(isp)
        Dim lstipkt As New List(Of Integer)

        'Loop over each nursery cell and populate the packets in that cell 
        'with the age 0 forcing numbers
        For Each inur As Integer In lstNursery
            irow = Me.m_Stanza.iNursery(isp, inur)
            icol = Me.m_Stanza.jNursery(isp, inur)
            nAge0 = ForcedCells(irow, icol)
            age0NoScale += nAge0

            lstipkt.Clear()

            npcks = 0
            'find all the packets of this age in this nursery cell
            For ip As Integer = 1 To Me.m_Stanza.Npackets
                If Math.Truncate(Me.m_Stanza.iPacket(isp, ia1, ip)) = irow And Math.Truncate(Me.m_Stanza.jPacket(isp, ia1, ip)) = icol Then
                    'm_Stanza.Npacket(isp, ia1, ip) = nAge0
                    'm_Stanza.Wpacket(isp, ia1, ip) = 0.0000000001
                    npcks += 1
                    lstipkt.Add(ip)
                End If
            Next ip

            For Each ipk As Integer In lstipkt
                Me.m_Stanza.Npacket(isp, ia1, ipk) = nAge0 / npcks
                Me.m_Stanza.Wpacket(isp, ia1, ipk) = 0.0000000001
                sumAge0 += nAge0 / npcks
            Next ipk

        Next inur

        Debug.Print("N Packets = " + Me.m_Stanza.Npackets.ToString + ", N Nursery Packets = " + npcks.ToString)

    End Sub


    Private Function ForceNurseryCells(isp As Integer) As Single
        'Set all the forced cells with age 0 forcing values to Nursery Cells 
        'These Nursery Cells will later be used to populate the location of the packets (iPacket() and jPacket())
        'and the age 0 forcing number in nPackets() 
        Dim ForcedCells(,) As Single = Me.m_Stanza.IBMForcedCells(isp)
        Dim Nused As Integer = 0
        Dim sumForced As Single
        For irow As Integer = 1 To Me.m_Data.InRow
            For icol As Integer = 1 To Me.m_Data.InCol

                If ForcedCells(irow, icol) > 0 And Me.m_Data.Depth(irow, icol) > 0 Then
                    'this cell is forced 
                    Nused += 1
                    Me.m_Stanza.iNursery(isp, Nused) = irow
                    Me.m_Stanza.jNursery(isp, Nused) = icol
                    sumForced += ForcedCells(irow, icol)

                End If
            Next icol
        Next irow

        Me.m_Stanza.Nnursery(isp) = Nused

        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        ''Debugging stuff
        Dim BaseRec As Single = Me.m_Stanza.RzeroS(isp) * Me.m_Data.ThabArea * 12 ' {Include the Ecosim Hatchery Forcing function} * m_ESData.tval(m_Stanza.HatchCode(isp))) 
        Debug.Print("Total Forced Recruits = " + sumForced.ToString + ", Ecopath Base Recruits = " + BaseRec.ToString)
        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Return sumForced

    End Function

    Sub UpDateBcellIBM(ipkt As Integer)
        'recalculates Bcell and predcell for multistanza groups when using IBM model
        'this goes through every packet and adds it's biomass to Bcell in its i,j position
        Dim ia As Integer, iaa As Integer, isp As Integer, ist As Integer
        Dim ieco As Integer, Tb(10) As Single, i As Integer, j As Integer
        'Dim isc As Integer

        Try
            For isp = 1 To Me.m_Stanza.Nsplit
                'accumulate bcell and predcell information over packet
                For iaa = 0 To Me.m_Stanza.MaxAgeSpecies(isp)
                    'get indices and locations
                    ia = Me.m_Stanza.AgeIndex1(isp) + iaa : If ia > Me.m_Stanza.MaxAgeSpecies(isp) Then ia = ia - Me.m_Stanza.MaxAgeSpecies(isp) - 1
                    ist = Me.m_Stanza.StanzaNo(isp, ia)
                    ieco = Me.m_Stanza.EcopathCode(isp, ist)

                    If Me.m_Stanza.iPacket(isp, iaa, ipkt) < 1 Then Me.m_Stanza.iPacket(isp, iaa, ipkt) = 1
                    If Me.m_Stanza.iPacket(isp, iaa, ipkt) > Me.m_Data.InRow + CELL_BOUNDS Then Me.m_Stanza.iPacket(isp, iaa, ipkt) = Me.m_Data.InRow + CELL_BOUNDS

                    If Me.m_Stanza.jPacket(isp, iaa, ipkt) < 1 Then Me.m_Stanza.jPacket(isp, iaa, ipkt) = 1
                    If Me.m_Stanza.jPacket(isp, iaa, ipkt) > Me.m_Data.InCol + CELL_BOUNDS Then Me.m_Stanza.jPacket(isp, iaa, ipkt) = Me.m_Data.InCol + CELL_BOUNDS

                    i = Math.Truncate(Me.m_Stanza.iPacket(isp, iaa, ipkt))
                    j = Math.Truncate(Me.m_Stanza.jPacket(isp, iaa, ipkt))

                    'do the updating
                    Me.BcellThread(i, j, ieco) = Me.BcellThread(i, j, ieco) + Me.m_Stanza.Npacket(isp, iaa, ipkt) * Me.m_Stanza.Wpacket(isp, iaa, ipkt)
                    Me.PredCellThread(i, j, ieco) = Me.PredCellThread(i, j, ieco) + Me.m_Stanza.Npacket(isp, iaa, ipkt) * Me.m_Stanza.WWa(isp, ia)
                Next

            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Function HabIsOk(ieco As Integer, i As Integer, j As Integer) As Boolean
        'If Depth(i, j) > 0 And (PrefHab(ieco, HabType(i, j)) = True Or PrefHab(ieco, 0) = True) Then
        If Me.m_Data.Depth(i, j) > 0 And Me.m_Data.HabCap(ieco)(i, j) > 0.5 Then
            HabIsOk = True
        Else
            HabIsOk = False
        End If
    End Function

    Public Sub New(ThreadNumber As Integer)
        Me.isOkToRun = True
        Me.ThreadID = ThreadNumber
        'Seed the random number generator 
        'So it will return a different sequence for each run of Ecospace
        Me.m_rand = New Random(CInt(Date.Now.Ticks And &HFFFF))

    End Sub
End Class
