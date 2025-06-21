namespace Cocktails.Api.Domain.Aggregates.CocktailAggregate;

using System.ComponentModel.DataAnnotations;

public enum GlasswareType
{
    /// <summary>The none</summary>
    [Display(Name = "")]
    None = 1,

    /// <summary>The rocks</summary>
    [Display(Name = "Rocks")]
    Rocks = 2,

    /// <summary>The highball</summary>
    [Display(Name = "Highball")]
    Highball = 3,

    /// <summary>The shot glass</summary>
    [Display(Name = "Shot Glass")]
    ShotGlass = 4,

    /// <summary>The coupe</summary>
    [Display(Name = "Coupe")]
    Coupe = 5,

    /// <summary>The copper mug</summary>
    [Display(Name = "Copper Mug")]
    CopperMug = 6,

    /// <summary>The collins</summary>
    [Display(Name = "Collins")]
    Collins = 7,

    /// <summary>The cocktail glass</summary>
    [Display(Name = "Cocktail Glass")]
    CocktailGlass = 8,

    /// <summary>The wine glass</summary>
    [Display(Name = "Wine Glass")]
    WineGlass = 9,

    /// <summary>The flute</summary>
    [Display(Name = "Flute")]
    Flute = 10,

    /// <summary>The lowball</summary>
    [Display(Name = "Lowball")]
    Lowball = 11,

    /// <summary>The fizz</summary>
    [Display(Name = "Fizz")]
    Fizz = 12,

    /// <summary>The tiki mug</summary>
    [Display(Name = "Tiki Mug")]
    TikiMug = 13,

    /// <summary>The pint</summary>
    [Display(Name = "Pint Glass")]
    PintGlass = 14,

    /// <summary>The julep tin</summary>
    [Display(Name = "Julep Tin")]
    JulepTin = 15,

    /// <summary>The double rocks</summary>
    [Display(Name = "Double Rocks")]
    DoubleRocks = 16,

    /// <summary>The hurricane</summary>
    [Display(Name = "Hurricane")]
    Hurricane = 17,

    /// <summary>The hollowed pineapple</summary>
    [Display(Name = "Hollowed Pineapple")]
    HollowedPineapple = 18,

    /// <summary>The snifter</summary>
    [Display(Name = "Snifter")]
    Snifter = 19,

    /// <summary>The scorpion bowl</summary>
    [Display(Name = "Scorpion Bowl")]
    ScorpionBowl = 20
}
