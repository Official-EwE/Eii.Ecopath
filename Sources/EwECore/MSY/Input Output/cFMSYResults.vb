
Namespace MSY

    Public Class cFMSYResults

        Public FMSY As Single()
        Public CMSY As Single()
        Public CMSYBase As Single()
        Public FBase As Single()
        Public Value As Single()
        Public ValueBase As Single()
        Public IsFopt As Boolean()

        Public CatchAtFMSY As Single()
        Public ValueAtFMSY As Single()

        Friend Sub New(ByVal nGroups As Integer)
            ReDim FMSY(nGroups)
            ReDim CMSY(nGroups)
            ReDim CMSYBase(nGroups)
            ReDim FBase(nGroups)
            ReDim Value(nGroups)
            ReDim ValueBase(nGroups)
            ReDim IsFopt(nGroups)

            ReDim CatchAtFMSY(nGroups)
            ReDim ValueAtFMSY(nGroups)
        End Sub

    End Class

    Public Class cMSYOptimum

        ''' <summary>FMSY per group.</summary>
        Public Fopt As Single()
        ''' <summary>Flag stating whether Fmsy was actually found.</summary>
        Public IsFopt As Boolean()

        Public Sub New(ByVal nGroups As Integer)
            ReDim Me.Fopt(nGroups)
            ReDim Me.IsFopt(nGroups)
        End Sub

    End Class

    ''' <summary>
    ''' MSY Results 
    ''' </summary>
    Public Class cMSYFResult

        Public curF As Single
        Public TotalValue As Single

        ''' <summary>
        ''' Biomass at the current F
        ''' </summary>
        Public B() As Single

        ''' <summary>
        ''' Catch by group.
        ''' </summary>
        Public [Catch]() As Single

        ''' <summary>
        ''' Fishing Mortality by group.
        ''' </summary>
        Public FishingMort() As Single

        Public Sub New(ByVal nGroups As Integer, ByVal F As Single, Value As Single)
            Me.curF = F
            TotalValue = Value
            Me.dimArrays(nGroups)
        End Sub

        Private Sub dimArrays(ByVal nGroups As Integer)

            ReDim Me.B(nGroups)
            ReDim Me.Catch(nGroups)
            ReDim Me.FishingMort(nGroups)

        End Sub

    End Class
End Namespace
