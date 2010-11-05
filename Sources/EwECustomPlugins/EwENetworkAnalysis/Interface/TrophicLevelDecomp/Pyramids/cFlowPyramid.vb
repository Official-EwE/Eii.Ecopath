#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports System.IO
Imports System.Globalization
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Content manager derived class to invoke the NA flow pyramid interface.
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class cFlowPyramid
    Inherits cPyramid

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="cPyramid.PyramidType"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function PyramidType() As modUtility.ePyramidTypes
        Return ePyramidTypes.Flow
    End Function

    Public Overrides Function PageTitle() As String
        Return "Flow pyramid (external)"
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="cPyramid.WritePyramidFile"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function WritePyramidFile(ByVal core As cCore, _
                                               ByVal sw As System.IO.StreamWriter, _
                                               ByVal ci As CultureInfo) As Boolean

        Dim mbr As MsgBoxResult = MsgBoxResult.Ignore
        Dim iMaxTL As Integer
        Dim iFlag As Integer
        Dim bShowItem As Boolean = True
        Dim bSucces As Boolean = True

        Try

            iFlag = 1
            sw.Write(Format(iFlag, "0"))

            iMaxTL = CInt(IIf(NetworkManager.nTrophicLevels > 9, 9, NetworkManager.nTrophicLevels))
            sw.WriteLine(Format(iMaxTL, "0"))

            'If Not (currUnitIndex = 6 Or currUnitIndex = 9) Then
            'Print #fnum, Trim(currUnitName);
            'Else
            'Print #fnum, Trim(currUnitName);
            'End If
            'Print #fnum, "/";
            'Print #fnum, Trim(TimeUnitName)
            sw.WriteLine("t/km²/year")

            sw.WriteLine(NetworkManager.TotalThroughput.ToString("00000000.000", ci))

            mbr = MsgBoxResult.Yes
            For i As Integer = 1 To NetworkManager.nGroups
                bShowItem = Me.StyleGuide.GroupVisible(i)
                If (bShowItem = False) Then 'There is at least one hidden
                    mbr = MsgBox(My.Resources.PROMPT_DISPLAY_ALL_HIDDEN_GROUPS, MsgBoxStyle.YesNo, My.Resources.CAPTION)
                    Exit For
                End If
            Next

            Select Case mbr
                Case MsgBoxResult.Yes 'all groups
                    For i As Integer = 1 To iMaxTL
                        Dim sngTemp As Single
                        sngTemp = CSng(IIf(Math.Abs(NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i)) > 0.001, _
                            NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i), 0))
                        sw.Write(sngTemp.ToString("00000000.000", ci))
                        'the values from transfer eff table
                        Dim Tr1 As Single
                        Tr1 = NetworkManager.PPConsByPred(i) + NetworkManager.DetConsByPred(i)
                        If Tr1 > 0 Then
                            If NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i) > 0 Then
                                'TrEm1(i) = Tr1 / (m_NetworkManager.PPThroughtput(i) + m_NetworkManager.DetThroughtput(i))
                                NetworkManager.TrEm1(i) = Tr1 / (NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i))
                            End If
                        End If
                        Dim TotTr As Single
                        TotTr = NetworkManager.PPConsByPred(i) + NetworkManager.DetConsByPred(i) + _
                            NetworkManager.CA(i) + NetworkManager.CatchDetritus(i)

                        If NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i) > 0 Then
                            TotTr = TotTr / (NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i))
                            sngTemp = CSng(IIf(Math.Abs(100 * TotTr) > 0.001, 100 * TotTr, 0))
                        Else
                            'TrEm1(i) = 0
                            NetworkManager.TrEm1(i) = 0
                            sngTemp = 0
                        End If
                        sw.WriteLine(sngTemp.ToString("00000000.000", ci))
                    Next

                Case MsgBoxResult.No 'hidden groups only
                    For i As Integer = 1 To iMaxTL
                        Dim sngTemp As Single
                        sngTemp = CSng(IIf(NetworkManager.ThroughtputShow(i) > 0.001, NetworkManager.ThroughtputShow(i), 0))
                        sw.Write(sngTemp.ToString("00000000.000", ci))
                        'the values from transfer eff table
                        Dim Tr1 As Single
                        Tr1 = NetworkManager.PPConsByPred(i) + NetworkManager.DetConsByPred(i)
                        If Tr1 > 0 Then
                            If NetworkManager.ThroughtputShow(i) > 0 Then
                                'TrEm1(i) = Tr1 / (m_NetworkManager.ThroughtputShow(i))
                                NetworkManager.TrEm1(i) = Tr1 / (NetworkManager.ThroughtputShow(i))
                            End If
                        End If
                        Dim TotTr As Single
                        TotTr = NetworkManager.PPConsByPred(i) + NetworkManager.DetConsByPred(i) + _
                            NetworkManager.CA(i) + NetworkManager.CatchDetritus(i)

                        If NetworkManager.ThroughtputShow(i) > 0 Then
                            TotTr = TotTr / (NetworkManager.ThroughtputShow(i))
                            sngTemp = CSng(IIf(Math.Abs(100 * TotTr) > 0.001, 100 * TotTr, 0))
                        Else
                            'TrEm1(i) = 0
                            NetworkManager.TrEm1(i) = 0
                            sngTemp = 0
                        End If
                        sw.WriteLine(sngTemp.ToString("00000000.000", ci))
                    Next
            End Select

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

End Class
