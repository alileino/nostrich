using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{
    public class Weapon
    {
        private double _timeAfterFiring = 0;
        private int _bulletsLoaded = 0;
        private bool _reloading = false;
        private double _reloadTime = 26.0;
        public bool AutoReload { get; set; }
        public double FireInterval { get; protected set; }
        public int BulletsLoaded { get { return _bulletsLoaded; } }
        public int Capacity { get; protected set; }
        public double ReloadTime { get { return _reloadTime; } protected set { _reloadTime = value; } }
        public bool Reloading { get { return _reloading; } }

        public Weapon()
        {
            AutoReload = true;
            FireInterval = 15.0;
            Capacity = 6;
            _timeAfterFiring=0;
        }

        internal void Randomize(Random random, double fireIntervalModifyFactor=0.0, double reloadTimeModifyFactor=0.0)
        {
            _reloading = true;
            _timeAfterFiring = random.NextDouble() * ReloadTime;
            FireInterval = FireInterval + random.NextDouble() * fireIntervalModifyFactor - fireIntervalModifyFactor / 2.0;
            ReloadTime = ReloadTime + random.NextDouble() * reloadTimeModifyFactor - reloadTimeModifyFactor / 2.0;

        }

        public void Update(double ticksPassed)
        {
            _timeAfterFiring += ticksPassed;
            if (_reloading && _timeAfterFiring > ReloadTime)
                DoReload();
            else if (!_reloading && BulletsLoaded == 0 && AutoReload)
                Reload();
        }

        private void DoReload()
        {
            _reloading = false;
            _bulletsLoaded = Capacity;
        }

        public bool Trigger()
        {
            if (_timeAfterFiring > FireInterval && BulletsLoaded > 0)
            {
                Shoot();
                return true;
            }
            else
                return false;

        }

        public bool Reload()
        {
            bool returnValue = !_reloading;
            _reloading = true;
            return returnValue;
        }

        private void Shoot()
        {
            _timeAfterFiring = 0;
            _bulletsLoaded -= 1;
        }
    }

    public class PlayerRevolver : Weapon
    {
        public PlayerRevolver()
        {
            FireInterval = 4f;
        }
    }
}