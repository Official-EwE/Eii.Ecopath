'==============================================================================
'
' $Log: cPSDDatastructures.vb,v $
' Revision 1.2  2009/03/18 13:25:58  jeroens
' Moved PSD data from EcopathDS to PSDDS
'
' Revision 1.1  2009/03/16 16:55:57  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEUtils.Core

''' <summary>
''' This class wraps the underlying particle size distribution data structures
''' </summary>
Public Class cPSDDatastructures

    ''' <summary>Total number of groups (living and detritus)</summary>
    Public NumGroups As Integer
    ''' <summary>Total number of living groups.</summary>
    Public NumLiving As Integer

    Public NAgeSteps As Integer = 101
    Public MortalityType As ePSDMortalityTypes = ePSDMortalityTypes.GroupZ
    Public LatNWCorner As Single = 45
    Public LatSECorner As Single = 45
    Public SelectedGroup() As Boolean
    Public NWeightClasses As Integer = 25
    Public FirstWeightClass As Single = 0.125

    Public Tcatch() As Single
    Public AinLW() As Single
    Public BinLW() As Single
    Public Loo() As Single
    Public Winf() As Single
    Public t0() As Single
    Public Tmax() As Single

    Public AinLWInput() As Single
    Public BinLWInput() As Single
    Public LooInput() As Single
    Public WinfInput() As Single
    Public t0Input() As Single
    Public TmaxInput() As Single

    Public EcopathWeight(,) As Single
    Public EcopathNumber(,) As Single
    Public EcopathBiomass(,) As Single
    Public LorenzenMortality(,) As Single
    Public PSD(,) As Single

    ''' <summary>
    ''' redimension array variables 
    ''' called when a new model is loaded
    ''' </summary>
    ''' <returns></returns>
    ''' True if no error
    ''' <remarks></remarks>
    Public Function redimGroupVariables() As Boolean

        ReDim Tcatch(NumGroups)
        ReDim AinLW(NumGroups)
        ReDim BinLW(NumGroups)
        ReDim Loo(NumGroups)
        ReDim Winf(NumGroups)
        ReDim t0(NumGroups)
        ReDim Tmax(NumGroups)

        ReDim AinLWInput(NumGroups)
        ReDim BinLWInput(NumGroups)
        ReDim LooInput(NumGroups)
        ReDim WinfInput(NumGroups)
        ReDim t0Input(NumGroups)
        ReDim TmaxInput(NumGroups)

        ReDim EcopathWeight(NumGroups, NAgeSteps)
        ReDim EcopathNumber(NumGroups, NAgeSteps)
        ReDim EcopathBiomass(NumGroups, NAgeSteps)
        ReDim LorenzenMortality(NumGroups, NAgeSteps)

        ReDim PSD(NumGroups, NWeightClasses)
        ReDim SelectedGroup(NumGroups)

        Return True

    End Function

    ''' <summary>
    ''' Copy the Input arrays into the arrays that are used for modeling and model output.
    ''' </summary>
    ''' <returns>True if all the values were copied successfully.</returns>
    ''' <remarks>This is call at the start of an Ecopath model run to copy the input data into the arrays that are used
    ''' for model computations and output. I.e. copies EEinput(NumGroups) into EE(NumGroups). In EwE5 this is called MakeUnknownUnknown </remarks>
    Public Function CopyInputToModelArrays() As Boolean

        'Warning EwE5 also included input variables for BA, Immig, and Emigration 
        'See modEcosSense.MakeUnknownUnknown
        Try
            AinLWInput.CopyTo(AinLW, 0)
            BinLWInput.CopyTo(BinLW, 0)
            LooInput.CopyTo(Loo, 0)
            WinfInput.CopyTo(Winf, 0)
            t0Input.CopyTo(t0, 0)
            TmaxInput.CopyTo(Tmax, 0)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try
        Return True

    End Function

    Friend Sub copyTo(ByRef dest As cPSDDatastructures)
        Try
            'Joeh
            Tcatch.CopyTo(dest.Tcatch, 0)

            AinLW.CopyTo(dest.AinLW, 0)
            BinLW.CopyTo(dest.BinLW, 0)
            Loo.CopyTo(dest.Loo, 0)
            Winf.CopyTo(dest.Winf, 0)
            t0.CopyTo(dest.t0, 0)
            Tmax.CopyTo(dest.Tmax, 0)

            AinLWInput.CopyTo(dest.AinLWInput, 0)
            BinLWInput.CopyTo(dest.BinLWInput, 0)
            LooInput.CopyTo(dest.LooInput, 0)
            WinfInput.CopyTo(dest.WinfInput, 0)
            t0Input.CopyTo(dest.t0Input, 0)
            TmaxInput.CopyTo(dest.TmaxInput, 0)

            dest.EcopathWeight = DirectCast(EcopathWeight.Clone, Single(,))
            dest.EcopathNumber = DirectCast(EcopathNumber.Clone, Single(,))
            dest.EcopathBiomass = DirectCast(EcopathBiomass.Clone, Single(,))
            dest.LorenzenMortality = DirectCast(LorenzenMortality.Clone, Single(,))

            dest.PSD = DirectCast(PSD.Clone, Single(,))
        Catch ex2 As Exception
            Debug.Assert(False, ex2.Message)
        End Try

    End Sub

End Class
