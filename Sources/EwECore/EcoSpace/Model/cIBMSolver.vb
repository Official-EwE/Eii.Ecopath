Imports System
Imports System.Threading

Public Class cIBMSolver

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
    Public m_Ecosim As EcoSim.cEcoSimModel

    Public Bcw(,,) As Single
    Public C(,,) As Single
    Public d(,,) As Single
    Public e(,,) As Single
    Public Cper(,,) As Single

    'the isp groups to solve
    Private iFrstGrp As Integer
    Private iLastGrp As Integer

    Public iFirstPacket As Integer
    Public iLastPacket As Integer

    Public BcellThread(,,) As Single
    Public PredCellThread(,,) As Single

    Public threadTime1 As Single
    Public threadTime2 As Single
    Public threadTimeMove As Single


    Public Sub Init()


    End Sub

    ''' <summary>
    ''' Set the groups to iterate over.
    ''' </summary>
    ''' <param name="iFirstGroup"></param>
    ''' <param name="iLastGroup"></param>
    ''' <remarks>Call for each thread, before the thread is started, to set the groups to solve.</remarks>
    Public Sub FirstLastGroups(ByVal iFirstGroup As Integer, ByVal iLastGroup As Integer)
        iFrstGrp = iFirstGroup
        iLastGrp = iLastGroup
    End Sub

#Region "Public 'Solve'"

    ''' <summary>
    ''' This is the method that the ThreadPool calls. 
    ''' It must have the object argument to match the Delegate signature required by ThreadPool.QueueUserWorkItem()
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Solve(ByVal obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)

        'if this is running on a thread this may not work
        'all flags need to be set outside the thread
        isOkToRun = False
        Dim timeTemp As Single = Microsoft.VisualBasic.Timer
        Dim timeTemp2 As Single
        Try
            'set signal state to 'non-signaled' SignalState.WaitOne() will block
            SignalState.Reset()
            Dim iPacket As Integer

            'do the processing here
            For iPacket = iFirstPacket To iLastPacket
                'now do the computations
                'GrowSurvivePackets(iGrp) 'this is called outside now
                timeTemp2 = timeTemp2 - Microsoft.VisualBasic.Timer
                MovePackets(iPacket)
                timeTemp2 = timeTemp2 + Microsoft.VisualBasic.Timer
                UpDateBcellIBM(iPacket)
            Next iPacket

            'thread has finished it is ok to run this again
            isOkToRun = True
            threadTime2 = threadTime2 + Microsoft.VisualBasic.Timer - timeTemp
            threadTimeMove = threadTimeMove + timeTemp2
            'set signal state to 'signaled' 
            'the processing has finished SignalState.WaitOne() will return immediately
            SignalState.Set()

        Catch ex As Exception
            cLog.Write(ex) 'this is dangerous clog.Write is not thread safe

            'prevent this thread from blocking forever if it throws an error
            SignalState.Set()
            isOkToRun = True

            'tell the main thread that this solver has had a problem
            'If EcospaceErrorHandler IsNot Nothing Then
            'Me.EcospaceErrorHandler(Me.ThreadID, ex.Message)
            'Else
            Debug.Assert(False, ex.Message)
            'End If

        End Try

    End Sub

    Public Sub SolveFirst(ByVal obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)

        'if this is running on a thread this may not work
        'all flags need to be set outside the thread
        isOkToRun = False
        Dim timeTemp As Single = Microsoft.VisualBasic.Timer
        Try
            'set signal state to 'non-signaled' SignalState.WaitOne() will block
            SignalState.Reset()
            Dim iGrp As Integer

            'do the processing here
            For iGrp = iFrstGrp To iLastGrp
                'now do the computations
                GrowSurvivePackets(iGrp) 'this is called outside now
            Next iGrp

            'thread has finished it is ok to run this again
            isOkToRun = True
            threadTime1 = threadTime1 + Microsoft.VisualBasic.Timer - timeTemp
            'set signal state to 'signaled' 
            'the processing has finished SignalState.WaitOne() will return immediately
            SignalState.Set()

        Catch ex As Exception
            cLog.Write(ex) 'this is dangerous clog.Write is not thread safe

            'prevent this thread from blocking forever if it throws an error
            SignalState.Set()
            isOkToRun = True

            'tell the main thread that this solver has had a problem
            If EcospaceErrorHandler IsNot Nothing Then
                Me.EcospaceErrorHandler(Me.ThreadID, ex.Message)
            Else
                Debug.Assert(False, ex.Message)
            End If

        End Try

    End Sub


#End Region

    Sub MovePackets(ByVal ip As Integer)
        'IBM model routine to move packets over spatial grid using orientation information from the ecospace instantaneous movement arrays
        'uses moves per month (IBMMovesPerMonth) and distance per move (IBMDistMove) calculated from ecospace stanza information in InitPackets
        Dim ist As Integer, ieco As Integer, ia As Integer, iaa As Integer, imm As Integer
        Dim i As Integer, j As Integer, Dmove As Single, Nmoves As Integer, isp As Integer
        Dim Mrat As Single, Ipos As Single, Jpos As Single
        Dim aa As Single, bb As Single, cc As Single, dd As Single
        'DEBUG to stop movement
        'Exit Sub
        Dim dAllow As Single
        Try
            For isp = 1 To m_Stanza.Nsplit
                For iaa = 0 To m_Stanza.MaxAgeSpecies(isp)
                    ia = m_Stanza.AgeIndex1(isp) + iaa : If ia > m_Stanza.MaxAgeSpecies(isp) Then ia = ia - m_Stanza.MaxAgeSpecies(isp) - 1
                    ist = m_Stanza.StanzaNo(isp, ia)
                    ieco = m_Stanza.EcopathCode(isp, ist)

                    Mrat = m_Data.Mrate(ieco)
                    Dmove = m_Stanza.IBMdistmove(isp, ia)
                    dAllow = Dmove + 0.01
                    i = Int(m_Stanza.iPacket(isp, iaa, ip)) : j = Int(m_Stanza.jPacket(isp, iaa, ip))

                    If (m_Data.PrefHab(ieco, m_Data.HabType(i, j)) = True Or m_Data.PrefHab(ieco, 0) = True) And m_Data.Depth(i, j) > 0 Then
                        Nmoves = m_Stanza.IBMMovesPerMonth(ieco)
                    Else
                        Dmove = m_Stanza.IBMdistmove(isp, ia) '* RelMoveBad(ieco)
                        Nmoves = m_Stanza.IBMMovesPerMonth(ieco) * m_Data.RelMoveBad(ieco)
                    End If
                    For imm = 1 To Nmoves
                        'For ip = 1 To m_Stanza.Npackets
                        'use rapid movement if packet is initially in unfavorable habitat
                        i = Int(m_Stanza.iPacket(isp, iaa, ip)) : j = Int(m_Stanza.jPacket(isp, iaa, ip))
                        aa = Bcw(i + 1, j, ieco) 'south move
                        bb = C(i - 1, j, ieco) 'north move
                        CC = d(i, j, ieco) 'east move
                        dd = e(i, j, ieco) 'west move

                        If (m_Data.PrefHab(ieco, m_Data.HabType(i, j)) = True Or m_Data.PrefHab(ieco, 0) = True) And m_Data.Depth(i, j) > 0 Then
                            If imm > m_Stanza.IBMMovesPerMonth(ieco) Then Exit For
                            If m_Data.IsMigratory(ieco) = False Then
                                'this changes movement if it's inside the box s.t. it can't get out in one move
                                Ipos = m_Stanza.iPacket(isp, iaa, ip) - i : Jpos = m_Stanza.jPacket(isp, iaa, ip) - j
                                If Ipos < 1.0 - dAllow Then aa = Mrat
                                If Ipos > dAllow Then bb = Mrat
                                If Jpos < 1.0 - dAllow Then cc = Mrat
                                If Jpos > dAllow Then dd = Mrat
                            End If
                            'Nmoves = m_Stanza.IBMMovesPerMonth(ieco)
                        Else
                            Dmove = m_Stanza.IBMdistmove(isp, ia) '* RelMoveBad(ieco)
                            Nmoves = m_Stanza.IBMMovesPerMonth(ieco) * m_Data.RelMoveBad(ieco)
                        End If


                        MoveThePacket(isp, ieco, iaa, ip, Dmove, aa, bb, cc, dd)
                        'i = Int(m_Stanza.iPacket(isp, ia, ip)) : j = Int(m_Stanza.jPacket(isp, ia, ip))

                    Next
                    'Next
                Next
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Sub MoveThePacket(ByVal isp As Integer, ByVal ieco As Integer, ByVal ia As Integer, ByVal ip As Integer, ByVal Dmove As Single, ByVal aa As Single, ByVal bb As Single, ByVal cc As Single, ByVal dd As Single)
        'Dim i As Integer = 0
        'Dim j As Integer = 0
        'Dim Tns As Single = 0.0!
        'Dim Tew As Single = 0.0!
        'aa As Single, bb As Single, CC As Single, dd As Single, 

        'aa = aa + 0.0000000001
        bb = bb + aa '+ 0.0000000001
        cc = cc + bb '+ 0.0000000001
        dd = dd + cc '+ 0.0000000001
        Dim randMove As Single = Rnd() * dd
        'Tns = aa + bb + 0.0000000001 : Tew = cc + dd + 0.0000000001
        If randMove < aa Then 'move south
            m_Stanza.iPacket(isp, ia, ip) = m_Stanza.iPacket(isp, ia, ip) + Dmove
        ElseIf randMove < bb Then 'move north
            m_Stanza.iPacket(isp, ia, ip) = m_Stanza.iPacket(isp, ia, ip) - Dmove
        ElseIf randMove < cc Then 'move east
            m_Stanza.jPacket(isp, ia, ip) = m_Stanza.jPacket(isp, ia, ip) + Dmove
        Else 'move west
            m_Stanza.jPacket(isp, ia, ip) = m_Stanza.jPacket(isp, ia, ip) - Dmove
        End If

        'Tns = aa + bb + 0.0000000001 : Tew = CC + dd + 0.0000000001
        'If Rnd() < Tns / (Tns + Tew) Then 'choose north-south move
        '    If Rnd() < aa / Tns Then 'move south
        '        m_Stanza.iPacket(isp, ia, ip) = m_Stanza.iPacket(isp, ia, ip) + Dmove
        '    ElseIf bb > 0 Then 'move north
        '        m_Stanza.iPacket(isp, ia, ip) = m_Stanza.iPacket(isp, ia, ip) - Dmove
        '    End If
        'Else 'choose east-west move
        '    If Rnd() < CC / Tew Then 'move east
        '        m_Stanza.jPacket(isp, ia, ip) = m_Stanza.jPacket(isp, ia, ip) + Dmove
        '    ElseIf dd > 0 Then 'move west
        '        m_Stanza.jPacket(isp, ia, ip) = m_Stanza.jPacket(isp, ia, ip) - Dmove
        '    End If
        'End If
        'If iPacket(isp, ia, ip) < 1 Then iPacket(isp, ia, ip) = 1
        'If iPacket(isp, ia, ip) > Inrow + 0.9999 Then iPacket(isp, ia, ip) = Inrow + 0.9
        'If jPacket(isp, ia, ip) < 1 Then jPacket(isp, ia, ip) = 1
        'If jPacket(isp, ia, ip) > Incol + 0.9999 Then jPacket(isp, ia, ip) = Incol + 0.9

    End Sub
    Sub GrowSurvivePackets(ByVal isp As Integer)
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

        m_Stanza.EggAtSpawn = True

        'update numbers and body weights
        ieco = m_Stanza.EcopathCode(isp, m_Stanza.Nstanza(isp))
        If m_Ecosim.ResetPred(ieco) = False Then

            Be = 0 'initialize variable to accumulate total egg production by the species for this time step
            For iaa = 0 To m_Stanza.MaxAgeSpecies(isp)
                'set age dependnt on age of fish in first index position for this time step
                ia = m_Stanza.AgeIndex1(isp) + iaa : If ia > m_Stanza.MaxAgeSpecies(isp) Then ia = ia - m_Stanza.MaxAgeSpecies(isp) - 1
                If ia = m_Stanza.MaxAgeSpecies(isp) Then ia1 = iaa 'save array element to be overwritten with new recruits
                ist = m_Stanza.StanzaNo(isp, ia)
                ieco = m_Stanza.EcopathCode(isp, ist)
                'loop over packets within this age and update numbers,wt dependent on current cell position
                For ip = 1 To m_Stanza.Npackets
                    i = Int(m_Stanza.iPacket(isp, iaa, ip))
                    j = Int(m_Stanza.jPacket(isp, iaa, ip))
                    Su = Math.Exp(-m_Stanza.Zcell(i, j, ieco) / 12.0#) 'mortality
                    Gf = Cper(i, j, ieco) '(month factor here included in splitalpha scaling setup)

                    'calculate mortality and weight change for the packet
                    m_Stanza.Npacket(isp, iaa, ip) = m_Stanza.Npacket(isp, iaa, ip) * Su
                    m_Stanza.Wpacket(isp, iaa, ip) = m_Stanza.vBM(isp) * m_Stanza.Wpacket(isp, iaa, ip) + Gf * m_Stanza.SplitAlpha(isp, ia)
                    'accumulate contribution of this packet to total egg production by the species
                    Egg = 0
                    If m_Stanza.FixedFecundity(isp) Then
                        Egg = m_Stanza.Npacket(isp, iaa, ip) * m_Stanza.EggsSplit(isp, ia)
                    Else
                        If m_Stanza.Wpacket(isp, iaa, ip) > m_Stanza.WmatWinf(isp) Then
                            Egg = m_Stanza.Npacket(isp, iaa, ip) * (m_Stanza.Wpacket(isp, iaa, ip) - m_Stanza.WmatWinf(isp))
                        End If
                    End If
                    Be = Be + Egg
                    m_Stanza.EggCell(i, j, isp) = m_Stanza.EggCell(i, j, isp) + Egg
                Next
            Next

            m_Stanza.EggsStanza(isp) = Be
            'WageS(iSp, 0) = 0
            'update age of fish for first iaa array element

            m_Stanza.AgeIndex1(isp) = m_Stanza.AgeIndex1(isp) + 1 : If m_Stanza.AgeIndex1(isp) > m_Stanza.MaxAgeSpecies(isp) Then m_Stanza.AgeIndex1(isp) = 0

            'finally set abundance at youngest age to recruitment rate
            If m_Stanza.BaseEggsStanza(isp) > 0 Then TotRecruits = m_Stanza.RscaleSplit(isp) * m_ESData.tval(m_Stanza.EggProdShapeSplit(isp)) * m_Stanza.RzeroS(isp) * m_ESData.tval(m_Stanza.HatchCode(isp))
            If m_Stanza.HatchCode(isp) = 0 Then TotRecruits = TotRecruits * (m_Stanza.EggsStanza(isp) / (m_Stanza.BaseEggsStanza(isp))) ^ m_Stanza.RecPowerSplit(isp)

            'distribute the total recruits (totrecruits) over packets and suitable spatial cells for recruitment
            'and set initial body sizes for packets representing new recruits
            For ip = 1 To m_Stanza.Npackets
                m_Stanza.Npacket(isp, ia1, ip) = TotRecruits / m_Stanza.Npackets
                m_Stanza.Wpacket(isp, ia1, ip) = 0.0000000001
            Next

            If m_Stanza.EggAtSpawn Then
                'distribute juvenile packets in proportion to eggcell distribution
                ReDim Te(m_Data.Inrow, m_Data.InCol)
                XeT = 0
                For i = 1 To m_Data.Inrow : For j = 1 To m_Data.InCol
                        XeT = XeT + m_Stanza.EggCell(i, j, isp)
                        Te(i, j) = XeT 'cumulative probability distribution
                    Next : Next
                For ip = 1 To m_Stanza.Npackets
                    Xe = Rnd() * XeT 'Be
                    For i = 1 To m_Data.Inrow
                        For j = 1 To m_Data.InCol
                            If Xe < Te(i, j) Then
                                m_stanza.iPacket(isp, ia1, ip) = i + Rnd()
                                m_Stanza.jPacket(isp, ia1, ip) = j + Rnd()
                                Exit For 'have found the packet position
                            End If
                        Next
                        If j < m_Data.InCol + 1 Then Exit For 'have found the packet position, exit i loop as well
                    Next
                Next
            Else
                'simple model for random distribution of packets over nursery cells for the species
                For ip = 1 To m_Stanza.Npackets
                    iNurse = 1 + Rnd() * (m_Stanza.Nnursery(isp) - 1)
                    m_Stanza.iPacket(isp, ia1, ip) = m_Stanza.iNursery(isp, iNurse) + Rnd()
                    m_Stanza.jPacket(isp, ia1, ip) = m_Stanza.jNursery(isp, iNurse) + Rnd()
                Next
            End If
        End If

    End Sub

    Sub UpDateBcellIBM(ByVal ip As Integer)
        'recalculates Bcell and predcell for multistanza groups when using IBM model
        'this goes through every packet and adds it's biomass to Bcell in its i,j position
        Dim ia As Integer, iaa As Integer, isp As Integer, ist As Integer
        Dim ieco As Integer, Tb(10) As Single, i As Integer, j As Integer
        'Dim isc As Integer

        Try
            For isp = 1 To m_Stanza.Nsplit
                'accumulate bcell and predcell information over packet
                For iaa = 0 To m_Stanza.MaxAgeSpecies(isp)
                    'get indices and locations
                    ia = m_Stanza.AgeIndex1(isp) + iaa : If ia > m_Stanza.MaxAgeSpecies(isp) Then ia = ia - m_Stanza.MaxAgeSpecies(isp) - 1
                    ist = m_Stanza.StanzaNo(isp, ia)
                    ieco = m_Stanza.EcopathCode(isp, ist)
                    i = Int(m_Stanza.iPacket(isp, iaa, ip))
                    j = Int(m_Stanza.jPacket(isp, iaa, ip))

                    'do the updating
                    BcellThread(i, j, ieco) = BcellThread(i, j, ieco) + m_Stanza.Npacket(isp, iaa, ip) * m_Stanza.Wpacket(isp, iaa, ip)
                    PredCellThread(i, j, ieco) = PredCellThread(i, j, ieco) + m_Stanza.Npacket(isp, iaa, ip) * m_Stanza.WWa(isp, ia)
                Next

            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Public Sub New(ByVal ThreadNumber As Integer)
        isOkToRun = True
        ThreadID = ThreadNumber
    End Sub
End Class
