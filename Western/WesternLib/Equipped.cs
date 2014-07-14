using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{

    public class Equipped
    {
        Toggleable _nextWeaponFlag = new Toggleable(true); // false = left, true = right
        public Weapon Left { get; private set; }
        public Weapon Right { get; private set; }
        public bool EquippedLeft { get { return Left != null; } }
        public bool EquippedRight { get { return Right != null; } }
        public int WeaponNum { get { return (EquippedLeft ? 1 : 0) + (EquippedRight ? 1 : 0); } }
        public Pawn Owner { get; set; }
        public int BulletsLeft
        {
            get
            {
                int sum = 0;
                if (EquippedLeft)
                    sum += Left.BulletsLoaded;
                if (EquippedRight)
                    sum += Right.BulletsLoaded;
                return sum;
            }
        }

        public event PawnEvents.ReloadEventHandler Reloading;

        protected Weapon NextWeapon
        {
            get
            {
                Weapon next = null;
                if (EquippedLeft && !EquippedRight)
                    return Left;
                else if (!EquippedLeft && EquippedRight)
                    return Right;
                else if (!EquippedLeft && !EquippedRight)
                    return null;
                else if (_nextWeaponFlag)
                    next = Right;
                else
                    next = Left;
                _nextWeaponFlag.Toggle();
                return next;
            }
        }

        public Equipped(Pawn owner, Weapon right = null, Weapon left=null)
        {
            Owner = owner;
            EquipRight(right);
            EquipLeft(left);
        }

        public void Randomize(Random random, double fireIntervalModifyFactor = 0.0, double reloadSpeedModifyFactor =0.0)
        {
            if (EquippedRight)
                Right.Randomize(random, fireIntervalModifyFactor, reloadSpeedModifyFactor);
            if (EquippedLeft)
                Left.Randomize(random, fireIntervalModifyFactor, reloadSpeedModifyFactor);
        }

        public void Equip(Weapon weapon)
        {
            if (EquippedRight)
                EquipLeft(weapon);
            else
                EquipRight(weapon);
        }

        public bool Trigger()
        {
            Weapon w = NextWeapon;
            bool triggered = w.Trigger();
            if(w.BulletsLoaded == 0 && w.Reload())
                HandleReload();
            return triggered;
        }

        public void EquipLeft(Weapon weapon)
        {
            Left = weapon;
        }

        public void EquipRight(Weapon weapon)
        {
            Right = weapon;
        }

        public void Reload()
        {
            bool hasReloaded = false;
            if (EquippedLeft)
                hasReloaded = Left.Reload();
            if (EquippedRight)
                hasReloaded |= Right.Reload();
            if (hasReloaded)
                HandleReload();
        }

        private void HandleReload()
        {
            if (Reloading != null)
                Reloading(Owner);
        }

        public void Update(double ticksPassed)
        {
            if (EquippedRight)
                Right.Update(ticksPassed);
            if (EquippedLeft)
                Left.Update(ticksPassed);
        }
    }
}


