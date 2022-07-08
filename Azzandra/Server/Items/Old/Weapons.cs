//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Azzandra.Items
//{
//    public class IronBroadsword : Weapon
//    {
//        public override string Name => "iron broadsword";
//        public override string AssetID => "iron_broadsword";
//        public override string Desc => "A powerful one handed blade.";

//        public override int Attack => 6;
//        public override int Strength => 6;
//        public override int Parry => 4;

//        public IronBroadsword(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }

//    public class IronLongsword : Weapon
//    {
//        public override string Name => "iron longsword";
//        public override string AssetID => "iron_longsword";
//        public override string Desc => "A magnificient two handed weapon.";

//        public override int Attack => 10;
//        public override int Strength => 10;
//        public override int Parry => 6;
//        public override bool IsTwoHander => true;

//        public IronLongsword(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }

//    public class IronDagger : Weapon
//    {
//        public override string Name => "iron dagger";
//        public override string AssetID => "iron_dagger";
//        public override string Desc => "A simple weapon.";

//        public override bool CanOffHand => true;

//        public override int Attack => 4;
//        public override int Strength => 4;
//        public override int Parry => 3;

//        public IronDagger(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }

//    public class IronMace : Weapon
//    {
//        public override string Name => "iron mace";
//        public override string AssetID => "iron_mace";
//        public override string Desc => "Spiky.";

//        public override int Attack => 8;
//        public override int Strength => 8;
//        public override int Parry => 3;

//        public IronMace(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }

//    public class IronSpear : Weapon
//    {
//        public override string Name => "iron spear";
//        public override string AssetID => "iron_spear";
//        public override string Desc => "An ultra large skewer.";

//        public override int Attack => 8;
//        public override int Strength => 8;
//        public override int Parry => 3;
//        public override int Range => 2;

//        public IronSpear(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }

//    public class IronGreatsword : Weapon
//    {
//        public override string Name => "iron greatsword";
//        public override string AssetID => "iron_greatsword";
//        public override string Desc => "\"The mower\"";

//        public override int Attack => 14;
//        public override int Strength => 14;
//        public override int Parry => 5;
//        public override bool IsTwoHander => true;

//        public IronGreatsword(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }

//    public class EnchantedLongsword : Weapon
//    {
//        public override string Name => "enchanted longsword";
//        public override string AssetID => "iron_longsword";
//        public override string Desc => "A magical weapon.";
//        public override Color StringColor => Color.Fuchsia;

//        public override int Attack => 14;
//        public override int Strength => 14;
//        public override int Parry => 5;
//        public override bool IsTwoHander => true;

//        public EnchantedLongsword(int quantity = 1)
//        {
//            Quantity = quantity;
//        }

//        public override List<AttackProperty> AttackProperties => new List<AttackProperty> { new AttackProperty(AttackPropertyID.Enchanted, 1) };
//    }

//    public class FireDagger : Weapon
//    {
//        public override string Name => "fire dagger";
//        public override string AssetID => "iron_dagger";
//        public override string Desc => "It's odd how the dagger is cold to the touch.";
//        public override Color StringColor => Color.DarkOrange;

//        public override int Attack => 4;
//        public override int Strength => 4;
//        public override int Parry => 3;

//        public FireDagger(int quantity = 1)
//        {
//            Quantity = quantity;
//        }

//        public override List<AttackProperty> AttackProperties => new List<AttackProperty> { new AttackProperty(AttackPropertyID.Fire, 1) };
//    }
//    public class IronSpearPoisoned : Weapon
//    {
//        public override string Name => "iron spear (poisoned)";
//        public override string AssetID => "iron_spear";
//        public override string Desc => "An iron tipped skewer with added poison.";
//        public override Color StringColor => Color.Green;

//        public override int Attack => 4;
//        public override int Strength => 4;
//        public override int Parry => 3;

//        public IronSpearPoisoned(int quantity = 1)
//        {
//            Quantity = quantity;
//        }

//        public override List<AttackProperty> AttackProperties => new List<AttackProperty> { new AttackProperty(AttackPropertyID.Poison, 1) };
//    }

//    public class EnchantedIceBlade : Weapon
//    {
//        public override string Name => "enchanted ice blade";
//        public override string AssetID => "iron_broadsword";
//        public override string Desc => "A crude angular blade, cold to the touch.";
//        public override Color StringColor => Color.Fuchsia;

//        public override int Attack => 10;
//        public override int Strength => 14;
//        public override int Parry => 7;

//        public EnchantedIceBlade(int quantity = 1)
//        {
//            Quantity = quantity;
//        }

//        public override List<AttackProperty> AttackProperties => new List<AttackProperty> { new AttackProperty(AttackPropertyID.Frost, 1), new AttackProperty(AttackPropertyID.Enchanted, 1) };
//    }

//    public class OakenLongbow : RangedWeapon
//    {
//        public override AmmunitionType AmmunitionType => AmmunitionType.Arrow;

//        public override string Name => "oaken longbow";
//        public override string AssetID => "longbow";
//        public override string Desc => "A long curved stick with a string attached, or is it something more?";
//        public override Color StringColor => Color.ForestGreen;

//        public override int Range => 10;
//        public override int Attack => 7;
//        public override int Strength => 7;

//        public OakenLongbow(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//}
