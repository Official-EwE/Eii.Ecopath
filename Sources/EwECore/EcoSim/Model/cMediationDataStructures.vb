
Public Class cMediationDataStructures
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Mediation vars 


    Public Const MAXFUNCTIONS As Integer = 5

    ''' <summary>Number of functions</summary>
    Public MediationShapes As Integer
    ''' <summary>number of points per mediation function</summary>
    Public NMedPoints As Integer
    ''' <summary>mediation function points(iMedPt, iMedFn)</summary>
    Public Medpoints(,) As Single
    ''' <summary>defines biomass weights for med X (iMedGrp, iShp)</summary>
    ''' <remarks>Only used for non-landings mediations</remarks>
    Public MedWeights(,) As Single
    ''' <summary>number of biomasses (mediation weights) in an iMediation</summary>
    Public NMedXused() As Integer
    ''' <summary>groups used in med function X IMedUsed(nGroups + nGear, MediationShapes)</summary>
    Public IMedUsed(,) As Integer
    ''' <summary>ecopath base value of med function(iMedFn)</summary>
    Public MedXbase() As Single
    ''' <summary>value of med function at ecopath base(iMedFn)</summary>
    Public MedYbase() As Single
    ''' <summary>true if med function iMediation is used(iMedFn)</summary>
    Public MedIsUsed() As Boolean
    ''' <summary>current value of mediation function(iMedFn)</summary>
    Public MedVal() As Single

    ''' <summary>IMedBase() index of ecopath base biomass vertical line on mediation plot</summary>
    ''' <remarks>integer X positions for ecopath base X</remarks>
    Public IMedBase() As Integer
    ''' <summary>titles of mediation shapes</summary>
    Public MediationTitles() As String
    ''' <summary>Unique ID from the Database for each function(iMedFN)</summary>
    Public MediationDBIDs() As Integer
    ''' <summary>parameters that where used to create a curve from the Database Table and Fields i.e. EcoSimShapes.YZero</summary>
    Public MediationShapeParams() As cEcosimDatastructures.ShapeParameters
    ''' <summary>defines biomass weights for med XMedGFWeights(iMedGrp, iMedFlt, iShp)</summary>
    ''' <remarks>Only used for Landings mediations</remarks>
    Public MedPriceWeights(,,) As Single
    ''' <summary>?</summary>
    Public IMedFltUsed(,) As Integer

    Public PriceMedFuncNum(,,) As Integer


    Public FunctionNumber(,,) As Integer
    Public IsMedFunction(,,) As Boolean
    Public FunctionType(,,) As Integer


    Public Sub ReDimMediation(ByVal nGroups As Integer, ByVal nFleets As Integer)
        Dim i, j As Integer
        'following is for Mediation:
        NMedPoints = 1200
        ' JS18apr09: spawning 9 dummy mediation shapes without any valid database IDS screws up the database
        '            I tested Ecosim without mediation shapes and both core and GUI behave well
        'If MediationShapes <= 0 Then MediationShapes = 9
        ReDim Medpoints(NMedPoints, MediationShapes)
        ReDim MedWeights(nGroups + nFleets, MediationShapes)
        ReDim NMedXused(MediationShapes)
        ReDim IMedUsed(nGroups + nFleets, MediationShapes)
        ReDim MedXbase(MediationShapes)
        ReDim MedYbase(MediationShapes)
        ReDim MedIsUsed(MediationShapes)
        ReDim MedVal(MediationShapes)
        ReDim IMedBase(MediationShapes)

        ReDim MedPriceWeights(nGroups, nFleets, MediationShapes)
        ReDim IMedFltUsed(nGroups, MediationShapes)

        'jb added
        ReDim MediationTitles(MediationShapes)
        ReDim MediationShapeParams(MediationShapes)
        ReDim MediationDBIDs(MediationShapes)


        ReDim PriceMedFuncNum(nGroups, nFleets, MAXFUNCTIONS)

        ReDim FunctionNumber(nGroups, nGroups, cMediationDataStructures.MAXFUNCTIONS)
        ReDim IsMedFunction(nGroups, nGroups, cMediationDataStructures.MAXFUNCTIONS)
        ReDim FunctionType(nGroups, nGroups, cMediationDataStructures.MAXFUNCTIONS)


        'jb this is now handled by MedShapeParams() above
        'If ForcingShapes > MediationShapes Then
        '    ReDim Preserve Shapes(5, ForcingShapes)
        'Else
        '    ReDim Preserve Shapes(5, MediationShapes)
        'End If

        'ToDo: Sort out XBaseLine()what is it used for
        'ReDim XBaseLine(MediationShapes)
        For i = 0 To MediationShapes
            IMedBase(i) = NMedPoints \ 3
            For j = 0 To NMedPoints
                Medpoints(j, i) = 0.5
            Next
        Next

    End Sub


    Friend Sub SetMedFunctions(ByVal Biom() As Single, ByVal iTime As Integer, ByVal EcosimData As cEcosimDatastructures)
        'called from derivt, derivtred if MedIsUsed(0)=true to set
        'current Y value of each active trophic mediation function
        Dim iShp As Integer, iGrp As Integer, MedX As Single, ip As Long
        Try

            For iShp = 1 To Me.MediationShapes
                If Me.MedIsUsed(iShp) Then
                    MedX = 0.0000000001
                    For iGrp = 1 To Me.NMedXused(iShp)
                        If Me.IMedUsed(iGrp, iShp) <= EcosimData.nGroups Then
                            MedX = MedX + Biom(Me.IMedUsed(iGrp, iShp)) * Me.MedWeights(Me.IMedUsed(iGrp, iShp), iShp)
                        Else    'a fleet
                            MedX = MedX + EcosimData.FishRateGear(Me.IMedUsed(iGrp, iShp) - EcosimData.nGroups, iTime) * Me.MedWeights(Me.IMedUsed(iGrp, iShp), iShp)
                        End If
                    Next
                    '060328 CJW found that without the +0.01 below it could be unstable when slope
                    'was large around Ecopath base point in mediation function, causing instability.
                    'This solves it. VC.
                    ip = Int(Me.IMedBase(iShp) * MedX / Me.MedXbase(iShp) + 0.01)
                    If ip < 1 Then ip = 1
                    If ip > Me.NMedPoints Then ip = Me.NMedPoints
                    Me.MedVal(iShp) = Me.Medpoints(ip, iShp) / Me.MedYbase(iShp)
                End If
            Next

        Catch ex As Exception
            '  Debug.Assert(False)
        End Try

    End Sub

    ''' <summary>
    ''' Set MedVal() for the applied price elasticity function to the annual catch at the current time step
    ''' </summary>
    ''' <param name="LandingsGroupFleet">Catch by group, fleet</param>
    ''' <remarks>Price mediation function are initialized to Ecopath base values which are annual. 
    ''' This means that the catch must also be the Ecopath annual catch.
    '''  </remarks>
    Friend Sub SetPriceMedFunctions(ByVal LandingsGroupFleet(,) As Single)
        Dim iShp As Integer, iGrp As Integer, MedX As Single, ip As Long
        Dim iMedGrp As Integer
        Dim iMedFlt As Integer

        Try

            For iShp = 1 To Me.MediationShapes
                If Me.MedIsUsed(iShp) Then
                    MedX = 0.0000000001
                    For iGrp = 1 To Me.NMedXused(iShp)
                        If Me.IMedUsed(iGrp, iShp) Then
                            'Get the Group and Fleet index
                            iMedGrp = Me.IMedUsed(iGrp, iShp)
                            iMedFlt = Me.IMedFltUsed(iGrp, iShp)
                            MedX = MedX + LandingsGroupFleet(iMedGrp, iMedFlt) * Me.MedPriceWeights(iMedGrp, iMedFlt, iShp)
                        End If
                    Next
                    '060328 CJW found that without the +0.01 below it could be unstable when slope
                    'was large around Ecopath base point in mediation function, causing instability.
                    'This solves it. VC.
                    ip = Int(Me.IMedBase(iShp) * MedX / Me.MedXbase(iShp) + 0.01)
                    If ip < 1 Then ip = 1
                    If ip > Me.NMedPoints Then ip = Me.NMedPoints
                    Me.MedVal(iShp) = Me.Medpoints(ip, iShp) / Me.MedYbase(iShp)
                End If
            Next

        Catch ex As Exception
            '  Debug.Assert(False)
        End Try

    End Sub

    ''' <summary>
    ''' Return the Price Elasticity of Supply multiplier for a Group Fleet
    ''' </summary>
    ''' <param name="iGroup"></param>
    ''' <param name="iFleet"></param>
    ''' <returns>Value = cEcopathDataStructures.Market(Fleet,Group) * getPESMult(Group,Fleet)</returns>
    ''' <remarks></remarks>
    Public Function getPESMult(ByVal iGroup As Integer, ByVal iFleet As Integer) As Single
        Dim pMult As Single
        Dim bFoundMed As Boolean = False

        'now sum the multiplier for all applied price med functions for this Group Fleet
        For iFnt As Integer = 1 To cMediationDataStructures.MAXFUNCTIONS

            If Me.PriceMedFuncNum(iGroup, iFleet, iFnt) <= 0 Then
                Exit For
            End If

            pMult += Me.MedVal(Me.PriceMedFuncNum(iGroup, iFleet, iFnt))
            bFoundMed = True

        Next

        'If bFoundMed Then
        '    System.Console.WriteLine("Market value=" & Me.m_EPData.Market(iFlt, iGrp).ToString & ", mediation=" & pMult.ToString)
        'End If

        'No price elasticity function found set the multiplier to 1
        If Not bFoundMed Then pMult = 1
        'apply the price elasticity multiplier to market value for this Group/Fleet
        Return pMult

    End Function

    Public Sub New()

    End Sub
End Class
