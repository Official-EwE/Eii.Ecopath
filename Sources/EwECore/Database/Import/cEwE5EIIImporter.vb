#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore.Database
Imports EwEUtils.Core
Imports EwEUtils.Database

#End Region ' Imports 

Public Class cEwE5EIIImporter
    Implements IEwE5ModelImporter

    Private m_core As cCore = Nothing
    Private m_strFileName As String = ""
    Private m_iFNum As Integer = cCore.NULL_VALUE

    Public Sub New(ByVal core As cCore, ByVal strFileName As String)
        Me.m_core = core
        Me.m_strFileName = strFileName
    End Sub

#Region " Interface implementation "

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IEwE5ModelImporter.Close"/>
    ''' -----------------------------------------------------------------------
    Public Function Open() As Boolean _
        Implements Database.IEwE5ModelImporter.Open

        Debug.Assert(Not Me.IsOpen())

        Me.m_iFNum = FreeFile()
        Try
            FileOpen(Me.m_iFNum, Me.m_strFileName, OpenMode.Input)
        Catch ex As Exception
            cLog.Write(Me.ToString + ".LoadEcopath(...) Error opening eii file. " + vbCrLf + m_strFileName + vbCrLf + "Error:" + ex.Message())
            Me.m_iFNum = cCore.NULL_VALUE
            Return False
        End Try

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IEwE5ModelImporter.Close"/>
    ''' -----------------------------------------------------------------------
    Public Sub Close() _
        Implements Database.IEwE5ModelImporter.Close

        Debug.Assert(Me.IsOpen())

        FileClose(Me.m_iFNum)
        Me.m_iFNum = cCore.NULL_VALUE

    End Sub

    ''' -------------------------------------------------------------------
    ''' <inheritdoc cref="IEwE5ModelImporter.IsOpen"/>
    ''' -------------------------------------------------------------------
    Public Function IsOpen() As Boolean _
        Implements IEwE5ModelImporter.IsOpen

        Return (Me.m_iFNum <> cCore.NULL_VALUE)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IEwE5ModelImporter.GetModels"/>
    ''' -----------------------------------------------------------------------
    Public Function GetModels() As cEwE5ModelInfo() _
        Implements Database.IEwE5ModelImporter.GetModels

        Debug.Assert(Me.IsOpen())

        Dim info As New cEwE5ModelInfo("1", Path.GetFileNameWithoutExtension(Me.m_strFileName), "Ecopath 5 EII file", 0)
        Return New cEwE5ModelInfo() {info}

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="IEwE5ModelImporter.Import"/>
    ''' -----------------------------------------------------------------------
    Public Function Import(ByVal strModelName As String, _
                           ByVal db As cEwEDatabase, _
                           ByVal strLogfileName As String) As Boolean _
                           Implements Database.IEwE5ModelImporter.Import

        Return False

    End Function

#End Region ' Interface implementation

    Private Function LoadEII() As Boolean

        'read the contents of the eii file into an EcopathParameters object
        'this is written using vb file access instead of a filestream to keep it as close to the original vb code as possible
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim psdDS As cPSDDatastructures = Me.m_core.m_PSDData
        Dim pvar As Single
        Dim i As Integer
        Dim j As Integer
        Dim K As Integer
        Dim Dummy As Single
        Dim jnk As String
        Dim Import As Integer

        If m_strFileName = "" Then
            cLog.Write(Me.ToString + ".LoadEcopath(...) No file name specified.")
            Return False
        End If

        Try
            FileOpen(Me.m_iFNum, m_strFileName, OpenMode.Input)
        Catch ex As Exception
            cLog.Write(Me.ToString + ".LoadEcopath(...) Error opening eii file. " + vbCrLf + m_strFileName + vbCrLf + "Error:" + ex.Message())
            Return False
        End Try

        'fake model data
        m_core.m_EwEModelDBID = 1
        m_core.m_EwEModelName = Path.GetFileName(m_strFileName)
        m_core.m_EwEModelNumDigits = 3
        m_core.m_EwEModelDescription = "Simulated model read from EII file " & m_strFileName

        'read the file
        Try
            Input(Me.m_iFNum, ecopathDS.NumGroups)
            Input(Me.m_iFNum, ecopathDS.NumLiving)
            Input(Me.m_iFNum, Me.m_core.m_EwEModelUnitCurrencyCustom)
            Input(Me.m_iFNum, ecopathDS.currUnitIndex)

            If Not ecopathDS.redimGroupVariables() Or Not psdDS.redimGroupVariables() Then
                cLog.Write(Me.ToString + ".LoadModel(...) Failed to Re-Dimension group parameter arrays.")
                Return False
            End If

            'groups
            For K = 1 To ecopathDS.NumGroups
                Input(Me.m_iFNum, ecopathDS.GroupName(K)) : Input(Me.m_iFNum, pvar) : Input(Me.m_iFNum, ecopathDS.DtImp(K))
                Input(Me.m_iFNum, ecopathDS.Ex(K)) : Input(Me.m_iFNum, ecopathDS.fCatch(K)) : Input(Me.m_iFNum, ecopathDS.DC(K, 0))
                Input(Me.m_iFNum, ecopathDS.Binput(K)) : Input(Me.m_iFNum, ecopathDS.PBinput(K)) : Input(Me.m_iFNum, ecopathDS.EEinput(K))
                Input(Me.m_iFNum, ecopathDS.GEinput(K)) : Input(Me.m_iFNum, ecopathDS.QBinput(K))

                ecopathDS.BHinput(K) = ecopathDS.Binput(K) / ecopathDS.Area(K)

                ecopathDS.GroupDBID(K) = K

                'Input #me.m_iFNum, GroupName(K), Pvar, DtImp(K), Ex(K), Catch(K), parms.DC(K, 0), parms.B(K), parms.pb(K), parms.ee(K), parms.ge(K), parms.qb(K)
                'jb this does not make any sence
                'it uses the Primary Porduction as the version number ????
                'If pvar < -1.99 Then
                '    txt = "It is not possible to import your old version of the " _
                '        + "Ecopath data file. " _
                '        + "You may have to reenter your data.  " _
                '        + "Open the eii file in Notepad, and check it. " _
                '        + "A testversion of Ecopath with Ecosim had a bug where it would place, " _
                '        + "e.g., '-94-95' instead of '-94 -95' in the eii file. If this is the case then add spaces where needed. " _
                '        + "If not, please email v.christensen@cgiar.org " + vbNewLine _
                '        + "Please edit data.  Press any key to abort. "

                '    MsgBox(txt, vbCritical + vbOKOnly, "Problem importing old file type")

                '    FileClose(me.m_iFNum)
                '    ReadEii = False
                '    Exit Function
                'End If

                ecopathDS.PP(K) = pvar - 2
                If K > ecopathDS.NumLiving Then ecopathDS.PP(K) = 2
                If ecopathDS.GE(K) = 0 Then ecopathDS.GE(K) = -9

            Next K

            ' "Read DietComp"
            ReDim ecopathDS.DietChanged(1, 0)
            For K = 1 To ecopathDS.NumGroups
                For j = 1 To ecopathDS.NumGroups
                    Input(Me.m_iFNum, ecopathDS.DCInput(K, j))
                    If ecopathDS.DCInput(K, j) > 0 Then
                        ecopathDS.DietWasChanged(K, j)
                    End If
                Next j
            Next K

            If EOF(Me.m_iFNum) Then Return True

            'jb totp read in original routine using a string will read the entire line
            Input(Me.m_iFNum, jnk)
            'jb I have no idea what this is all about 
            If Import < 0 Then Import = 0

            'Unassimilated food
            For j = 1 To ecopathDS.NumGroups
                Input(Me.m_iFNum, Dummy) : Input(Me.m_iFNum, ecopathDS.GS(j))
                If Dummy < 0 Then Dummy = 0
                ecopathDS.GS(j) = Dummy + ecopathDS.GS(j)
                If ecopathDS.GS(j) > 1 Then ecopathDS.GS(j) = ecopathDS.GS(j) / 100
            Next j

            Input(Me.m_iFNum, jnk)

            'the time unit name
            If EOF(Me.m_iFNum) = False Then
                Dim tmpbuff As String
                Input(Me.m_iFNum, tmpbuff)
                ecopathDS.TimeUnitName = tmpbuff.Trim
                Select Case LCase(ecopathDS.TimeUnitName)
                    Case "year"
                        Me.m_core.m_EwEModelUnitTime = eUnitTimeType.Year
                    Case "day"
                        Me.m_core.m_EwEModelUnitTime = eUnitTimeType.Day
                    Case Else
                        Me.m_core.m_EwEModelUnitTime = eUnitTimeType.Custom
                        Me.m_core.m_EwEModelUnitTimeCustom = ecopathDS.TimeUnitName

                End Select
            End If

            'the ecosystem remarks.
            Input(Me.m_iFNum, jnk)

            For i = 1 To ecopathDS.NumGroups             ' parms.Bomass accumulation added March 95/VC
                Input(Me.m_iFNum, ecopathDS.BA(i))
            Next i

            'If EOF(me.m_iFNum) = False And NumGroups > NumLiving + 1 Then
            'More than 1 detritusbox Any reason for this??
            For i = 1 To ecopathDS.NumGroups
                For j = ecopathDS.NumLiving + 1 To ecopathDS.NumGroups
                    Input(Me.m_iFNum, ecopathDS.DF(i, j - ecopathDS.NumLiving))     ' Diet Fate array added July 1994/VC
                Next j
            Next i

            Input(Me.m_iFNum, jnk) ' 
            For i = 1 To ecopathDS.NumGroups             ' Emigration added Dec 98/VC
                Input(Me.m_iFNum, ecopathDS.Emigration(i))
            Next i

            Input(Me.m_iFNum, jnk)
            For i = 1 To ecopathDS.NumGroups                 ' immigration added Dec 98/VC
                Input(Me.m_iFNum, ecopathDS.Immig(i))
            Next i

            Input(Me.m_iFNum, jnk)  'NumGear
            Input(Me.m_iFNum, ecopathDS.NumFleet)

            ecopathDS.RedimFleetVariables(True)

            Input(Me.m_iFNum, jnk) 'Gearnames
            For i = 1 To ecopathDS.NumFleet             ' Added Dec 98/VC
                Input(Me.m_iFNum, ecopathDS.FleetName(i))
                ecopathDS.FleetDBID(i) = i
            Next i

            Input(Me.m_iFNum, jnk)  'cost
            For i = 1 To ecopathDS.NumFleet
                'First is fixed cost, second is cost per unit effort' Added Dec 98/VC
                Input(Me.m_iFNum, ecopathDS.CostPct(i, eCostIndex.Fixed))
                Input(Me.m_iFNum, ecopathDS.CostPct(i, eCostIndex.CUPE))
                Input(Me.m_iFNum, ecopathDS.CostPct(i, eCostIndex.Sail))
            Next i

            Input(Me.m_iFNum, jnk)  'landing
            For i = 1 To ecopathDS.NumFleet
                For j = 1 To ecopathDS.NumGroups
                    Input(Me.m_iFNum, ecopathDS.Landing(i, j))    ' Landing added Dec 98/VC
                Next j
            Next i

            Input(Me.m_iFNum, jnk)  'discard
            For i = 1 To ecopathDS.NumFleet
                For j = 1 To ecopathDS.NumGroups
                    Input(Me.m_iFNum, ecopathDS.Discard(i, j))    ' Added Dec 98/VC
                Next j
            Next i

            Input(Me.m_iFNum, jnk)  'discard
            For i = 1 To ecopathDS.NumFleet
                For j = 1 To ecopathDS.NumGroups - ecopathDS.NumLiving
                    Input(Me.m_iFNum, ecopathDS.DiscardFate(i, j))   ' Added Dec 98/VC
                Next j
            Next i

            Input(Me.m_iFNum, jnk)  'market
            For i = 1 To ecopathDS.NumFleet
                For j = 1 To ecopathDS.NumGroups
                    Input(Me.m_iFNum, ecopathDS.Market(i, j))    ' Added Dec 98/VC
                Next j
            Next i

            ecopathDS.NoGearData = False

            'shadow
            Input(Me.m_iFNum, jnk)
            For i = 1 To ecopathDS.NumGroups             ' Added Dec 98/VC
                Input(Me.m_iFNum, ecopathDS.Shadow(i))
            Next i

            'Habitatarea
            Input(Me.m_iFNum, jnk)  '
            For i = 1 To ecopathDS.NumGroups             ' Added Dec 98/VC
                Input(Me.m_iFNum, ecopathDS.Area(i))
                Input(Me.m_iFNum, ecopathDS.BH(i))
            Next i

        Catch ex As Exception 'catch any error during the reading of the data
            'some kind of a reading error better find out what happend
            cLog.Write(Me.ToString + ".LoadEcopath() Error reading eii file. Error: " + ex.Message())
            Debug.Assert(False)
            Return False
        End Try

        Return True

    End Function

End Class
