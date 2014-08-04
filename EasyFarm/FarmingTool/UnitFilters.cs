﻿using FFACETools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroLimits.FarmingTool;
using ZeroLimits.XITools;

namespace EasyFarm.FarmingTool
{
    public class UnitFilters
    {
        public static Func<Unit, bool> MobFilter(FFACE fface)
        {
            var ftools = FarmingTools.GetInstance(fface);

            // Function to use to filter surrounding mobs by.
            return new Func<Unit, bool>((Unit x) =>
            {
                // No fface? Bail. 
                if (fface == null) return false;

                //FIXED: Added null check to avoid certain states
                // secretly throwing null pointer exceptions. 
                // If the mob is null, bail.
                if (x == null) return false;

                // Mob not active
                if (!x.IsActive) return false;

                // Type is not mob 
                if (!x.NPCType.Equals(NPCType.Mob)) return false;

                // Mob is not visible
                if (!x.NPCBit.Equals(8)) return false;

                // Mob is out of range
                if (!(x.Distance < ftools.UserSettings.MiscSettings.DetectionDistance)) return false;

                // Mob too high out of reach. 
                if (x.YDifference > ftools.UserSettings.MiscSettings.HeightThreshold) return false;

                // The mob is claimed but it is not our claim.
                if (x.IsClaimed && x.ClaimedID != fface.Player.ID)
                {
                    // and the claim filter is off, invalid. if the filter is on
                    // the program will attack claimed mobs. 
                    if (!ftools.UserSettings.FilterInfo.ClaimedFilter) return false;
                }

                // Has aggroed and user doesn't want to kill aggro
                if (x.HasAggroed && !ftools.UserSettings.FilterInfo.AggroFilter) return false;

                // Party has claim but we don't want to kill party mobs. 
                if (x.PartyClaim && !ftools.UserSettings.FilterInfo.PartyFilter) return false;

                // Mob not claimed but we don't want to kill unclaimed mobs. 
                if (!x.IsClaimed && !ftools.UserSettings.FilterInfo.UnclaimedFilter) return false;                

                // If mob is on the ignored list ignore it. 
                if (ftools.UserSettings.FilterInfo.IgnoredMobs.Contains(x.Name)) return false;

                // Not on our targets list.
                if (!ftools.UserSettings.FilterInfo.TargetedMobs.Contains(x.Name) && ftools.UserSettings.FilterInfo.TargetedMobs.Count > 0) return false;

                // Mob is valid
                return true;
            });
        }
    }
}
