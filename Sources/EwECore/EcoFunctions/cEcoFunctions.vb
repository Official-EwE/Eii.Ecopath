

''' <summary>
''' Class to provide access to function needed both internally to the core and externally by plugins or other things...(what that would be I don't know)
''' </summary>
''' <remarks></remarks>
Public Class cEcoFunctions

    Private m_core As cCore

    Friend Sub Init(ByVal theCore As cCore)
        m_core = theCore
    End Sub

    Public Function KemptonsQ(ByVal Bio() As Single, ByVal Quan As Single) As Single
        'VC programmed this function 23 October 2002 from Tony Pitcher's description
        Dim BLower As Single
        Dim BUpper As Single
        Dim i As Integer
        Dim j As Integer
        Dim minB As Single
        Dim Smallest As Integer
        Dim Rank() As Integer
        Dim Used() As Boolean
        Dim Lower As Single
        Dim upper As Single
        Dim NumGr As Integer

        Try

            Debug.Assert(m_core IsNot Nothing, Me.ToString & " not initialized properly!")
            Dim epdata As cEcopathDataStructures = m_core.m_EcoPathData

            'We now know the current biomasses for each group = bb(i) the biomass for each group at the end of the simulation
            'Find the min and max biomass, only look at theliving groups
            KemptonsQ = 0
            ReDim Rank(epdata.NumLiving)
            ReDim Used(epdata.NumLiving)
            NumGr = 0
            For i = 1 To epdata.NumLiving
                If epdata.TTLX(i) < 3 Then
                    Used(i) = True 'don't include low trophic level species in diversity index
                Else
                    NumGr = NumGr + 1
                End If
            Next

            'if there are very few groups we better include all
            'VC Nov 2008
            If NumGr < 10 Then
                NumGr = 0
                ReDim Used(epdata.NumLiving)
                For i = 1 To epdata.NumLiving
                    NumGr += 1
                Next
            End If
            For i = 1 To NumGr
                minB = 1000000
                Smallest = 0
                For j = 1 To epdata.NumLiving
                    If Used(j) = False And Bio(j) < minB Then
                        minB = Bio(j)
                        Smallest = j
                    End If
                Next
                'After each round we have the smallest remaining biomass
                If Smallest > 0 Then    'there will be some where it won't
                    Used(Smallest) = True
                    Rank(i) = Smallest
                End If
            Next
            'after i rounds we have sorted all groups after biomasses in Rank()
            'Now we can find the percentiles:
            Lower = Quan * NumGr    'm_epdata.NumLiving           'e.g., 0.25* m_epdata.NumLiving
            upper = (1 - Quan) * NumGr  'm_epdata.NumLiving
            BLower = (Lower - CInt(Lower - 0.5)) * Bio(Rank(CInt(Lower - 0.5))) + (1 - (Lower - CInt(Lower - 0.5))) * Bio(Rank(CInt(Lower - 0.5) + 1))
            BUpper = (1 - (upper - CInt(upper - 0.5))) * Bio(Rank(CInt(upper - 0.5))) + (upper - CInt(upper - 0.5)) * Bio(Rank(CInt(upper - 0.5) + 1))
            'We can now calculate Kemptons Q-index:
            Return CSng(NumGr / Math.Log(BUpper / BLower) / 2)
            'Using the equation from Kemptons Species diversity index:
            'Q= St / [ 2 log(Pi0.25ST/Pi0.75St)] wher Piq is the proportional abundance of the qth most abundant species
            'exitFunction:

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".FunctionKemptonsQ() Error: " & ex.Message)
            Dim msg As New cMessage("Error in FunctionKemptonsQ() " & ex.Message, eMessageType.ErrorEncountered, eMessageSource.Core, eMessageImportance.Critical, EwEUtils.Core.eDataTypes.NotSet)
            m_core.Messages.SendMessage(msg)
            'swallow all errors!!!!
            '     Throw New ApplicationException(Me.ToString & ".FunctionKemptonsQ() Error: " & ex.Message, ex)
        End Try


    End Function



End Class
