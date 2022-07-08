//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Azzandra.Items
//{
//    public class YogaPants : Equipment
//    {
//        public override int Slot => 4;
//        public override string Name => "yoga pants";
//        public override string AssetID => "yoga_pants";
//        public override string Desc => "Leggings imbued with stretchy powers.";
//        public override string EquipAction => "wear";
//        public override int Evade => 4;

//        public YogaPants(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class LargeWoodenShield : Shield
//    {
//        public override string Name => "large wooden shield";
//        public override string AssetID => "wooden_round_shield";
//        public override string Desc => "A large circular wooden shield.";
//        public override string EquipAction => "wield";
//        public override int Block => 4;
//        public override int Armour => 4;

//        public LargeWoodenShield(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class IronBucketHelm : Equipment
//    {
//        public override int Slot => 2;
//        public override string Name => "iron bucket helm";
//        public override string AssetID => "iron_bucket_helm";
//        public override string Desc => "Who needs vision anyways?";
//        public override int Attack => -3;
//        public override int Armour => 6 ;

//        public IronBucketHelm(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class IronKettleHelm : Equipment
//    {
//        public override int Slot => 2;
//        public override string Name => "iron kettle helm";
//        public override string AssetID => "iron_kettle_helm";
//        public override string Desc => "A fashionable helm.";
//        public override int Armour => 4;

//        public IronKettleHelm(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class IronMailChest : Equipment
//    {
//        public override int Slot => 3;
//        public override string Name => "iron mail hauberk";
//        public override string AssetID => "iron_mail_body";
//        public override string Desc => "Armour made of interlocked metal rings.";
//        public override int Armour => 5;

//        public IronMailChest(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class IronChestplate : Equipment
//    {
//        public override int Slot => 3;
//        public override string Name => "iron chestplate";
//        public override string AssetID => "iron_chestplate";
//        public override string Desc => "Real armour.";
//        public override int Attack => -2;
//        public override int Evade => -2;
//        public override int Armour => 8;

//        public IronChestplate(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class IronPlatelegs : Equipment
//    {
//        public override int Slot => 4;
//        public override string Name => "iron platelegs";
//        public override string AssetID => "iron_platelegs";
//        public override string Desc => "Real armour.";
//        public override int Evade => -2;
//        public override int Armour => 5;

//        public IronPlatelegs(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class IronSpikedBoots : Equipment
//    {
//        public override int Slot => 5;
//        public override string Name => "iron spiked boots";
//        public override string AssetID => "iron_boots";
//        public override string Desc => "Pointy metal boots.";
//        public override int Attack => 1;
//        public override int Armour => 2;

//        public IronSpikedBoots(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class IronGauntlets : Equipment
//    {
//        public override int Slot => 6;
//        public override string Name => "gauntlets";
//        public override string AssetID => "iron_gauntlets";
//        public override string Desc => "These will protect your hands.";
//        public override int Armour => 2;

//        public IronGauntlets(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class ShadowCloak : Equipment
//    {
//        public override int Slot => 9;
//        public override string Name => "shadow cloak";
//        public override string AssetID => "cape";
//        public override string Desc => "This cloak makes you harder to spot.";
//        public override string EquipAction => "wear";
//        public override int Evade => 4;

//        public ShadowCloak(int quantity = 1)
//        {
//            Quantity = quantity;
//        }
//    }
//    public class EnchantedRing : Equipment
//    {
//        public override int Slot => 8;
//        public override string Name => "enchanted ring";
//        public override string AssetID => "golden_ring";
//        public override string EquipAction => "wear";

//        public EnchantedRing(int quantity = 1)
//        {
//            Quantity = quantity;
//        }


//    }
//    public class EnchantedAmulet : Equipment
//    {
//        public override int Slot => 7;
//        public override string Name => "enchanted amulet";
//        public override string AssetID => "golden_amulet";
//        public override string EquipAction => "wear";

//        public EnchantedAmulet(int quantity = 1)
//        {
//            Quantity = quantity;
//        }

//    }

//    public class WizardHat : Equipment
//    {
//        public override int Slot => 2;
//        public override string Name => "wizard hat";
//        public override string AssetID => "wizard_hat";
//        public override string EquipAction => "wear";
//        public override string Desc => "You'll feel like a fool wearing this.";

//        public override int Spellcast => 2;
//        public override int Resistance => 1;

//        public WizardHat(int quantity = 1)
//        {
//            Quantity = quantity;
//        }

//    }
//}
