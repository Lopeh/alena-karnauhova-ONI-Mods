using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using UnityEngine;
using Klei.AI;
using Utils;
using static Meteor_Seasons.STRINGS.MISC.NOTIFICATIONS;
using System.Text;

namespace Meteor_Seasons
{
    public class Patches : KMod.UserMod2
    {
        public const string NoMeteorsId = "None";
        public static Dictionary<string, NotificationTexts> MeteorNotificationTexts;
        public static List<string> ActiveSeasons = new List<string>(4);
        public static Notification ActiveSeasonsNotification;
        protected static bool loaded = false;
        public static bool BreachedSurface => Game.Instance.savedInfo.discoveredSurface || Options.Instance.General.ShowBeforeSurfaceBreach;

        protected static Notifier Notifier => GameFlowManager.Instance.gameObject.AddOrGet<Notifier>();

        protected static void AddNotification(
            string seasonAnnouncement,
            string seasonId)
        {
            NotificationTexts notification;
            if (!MeteorNotificationTexts.TryGetValue(seasonId, out notification))
            {
                notification = new NotificationTexts(seasonId, UNKNOWNSEASON);
            }
            Notifier.Add(new Notification($"{seasonAnnouncement}{notification.Name}",
                NotificationType.Neutral,
                (n, o) => notification.Tooltip));
        }

        protected static void UpdateActiveSeasonsNotification(List<string> seasonIds)
        {
            if (ActiveSeasonsNotification != null)
            {
                Notifier.Remove(ActiveSeasonsNotification);
                ActiveSeasonsNotification = null;
            }
            if (seasonIds.Count != 0)
            {
                NotificationTexts notificationTexts;
                StringBuilder notificationTooltip = new StringBuilder();
                notificationTooltip.AppendLine(CURRENTSEASON);
                foreach (string seasonId in seasonIds)
                {
                    if (!MeteorNotificationTexts.TryGetValue(seasonId, out notificationTexts))
                        notificationTexts = new NotificationTexts(seasonId, UNKNOWNSEASON);
                    notificationTooltip.AppendLine(notificationTexts.Name);
                }
                ActiveSeasonsNotification = new Notification(METEORSEASONACTIVE,
                    NotificationType.Neutral,
                    (n, o) => notificationTooltip.ToString(),
                    null, false);
                Notifier.Add(ActiveSeasonsNotification);
            }
        }

        protected static List<string> FindActiveSeason(GameplaySeasonInstance season)
        {
            List<string> result = new List<string>(4);
            MeteorShowerEvent.StatesInstance smi;
            foreach (GameplayEvent ev in season.Season.events)
            {
                smi = GameplayEventManager.Instance.GetGameplayEventInstance(ev.IdHash)?.smi as MeteorShowerEvent.StatesInstance;
                if (smi == null) continue;
                //StateMachine.BaseState state = smi.GetCurrentState();
                //if (state == smi.sm.running || state.parent == smi.sm.running)
                if (smi.GetCurrentState().parent == smi.sm.running)
                {
                    result.Add(ev.Id);
                }
                /*if (GameplayEventManager.Instance.IsGameplayEventActive(ev))
                {
                    result.Add(ev.Id);
                }*/
            }
            return result;
        }
        protected static void NotifyActiveSeason()
        {
            //bool found = false;
            //List<string> seasonIds = new List<string>(4);
            ActiveSeasons.Clear();
            foreach (var worldContainer in ClusterManager.Instance.WorldContainers)
            {
                if (worldContainer.IsDiscovered)
                {
                    foreach (GameplaySeasonInstance season in worldContainer.GetSMI<GameplaySeasonManager.Instance>().activeSeasons)
                    {
                        ActiveSeasons.AddRange(FindActiveSeason(season));
                    }
                }
            }
            Debug.Log($"MeteorSeasons: {ActiveSeasons.Count}");
            UpdateActiveSeasonsNotification(ActiveSeasons);
            /*foreach (string seasonId in seasonIds)
            {
                //found = true;
                AddNotification(CURRENTSEASON, seasonId);
            }*/
            /*if (!found)
            {
                AddNotification(CURRENTSEASON, NoMeteorsId);
            }*/
        }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new POptions().RegisterOptions(this, typeof(Options));
        }

        [HarmonyPatch(typeof(GameplayEventManager), "RestoreEvents")]
        protected static class GameplayEventManager_LoadEvents
        {
            private static void Postfix()
            {
                //Debug.Log(BreachedSurface ? "MeteorSeasons: breached surface, notifications will be shown." : "MeteorSeasons: did not breach surface, notifications won't be shown until then.");
                if (BreachedSurface && Options.Instance.ShowNotifications.AlwaysShowActiveSeason)
                {
                    NotifyActiveSeason();
                }
                loaded = true;
            }
        }

        [HarmonyPatch(typeof(MeteorShowerEvent.States), nameof(MeteorShowerEvent.States.InitializeStates))]
        protected static class MeteorShowerEvent_InitializeStates
        {
            public static void NotifyNewSeason(string seasonId, bool state)
            {
                if (!BreachedSurface || !loaded)
                    return;
                if (Options.Instance.ShowNotifications.ShowSeasonChange)
                {
                    AddNotification(state ? NEWSEASON : SEASONEND, seasonId);
                }
                if (Options.Instance.ShowNotifications.AlwaysShowActiveSeason
                    && ActiveSeasonsNotification != null)
                {
                    if (state)
                        ActiveSeasons.Add(seasonId);
                    else
                        ActiveSeasons.Remove(seasonId);
                    UpdateActiveSeasonsNotification(ActiveSeasons);
                    //NotifyActiveSeason();
                }
            }
            private static void Postfix(MeteorShowerEvent.States __instance)
            {
                __instance.running.Enter(smi => NotifyNewSeason(smi.gameplayEvent.Id, true))
                    .Exit(smi => NotifyNewSeason(smi.gameplayEvent.Id, false));
            }
        }

        [HarmonyPatch(typeof(MeteorShowerEvent.StatesInstance), nameof(MeteorShowerEvent.StatesInstance.StartBackgroundEffects))]
        protected static class MeteorShowerEvent_StartBombardment
        {
            private static void Postfix(MeteorShowerEvent.StatesInstance __instance)
            {
                if (!BreachedSurface || !Options.Instance.ShowNotifications.ShowBombardmentStart)
                    return;
                AddNotification(METEORSHOWERS, __instance.gameplayEvent.Id);
            }
        }
        [HarmonyPatch(typeof(MeteorShowerEvent.StatesInstance), nameof(MeteorShowerEvent.StatesInstance.StopBackgroundEffects))]
        protected static class MeteorShowerEvent_StopBombardment
        {
            private static void Postfix(MeteorShowerEvent.StatesInstance __instance)
            {
                if (!BreachedSurface || !Options.Instance.ShowNotifications.ShowBombardmentStart)
                    return;
                AddNotification(SHOWERSENDING, __instance.gameplayEvent.Id);
            }
        }

        [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
        private static class Localization_Initialize_Patch
        {
            private static void Postfix()
            {
                LocalizationUtils.Translate(typeof(STRINGS));
                MeteorNotificationTexts = new Dictionary<string, NotificationTexts>()
                {
                    {
                        NoMeteorsId,
                        new NotificationTexts(METEORSHOWERNONE.NAME, METEORSHOWERNONE.TOOLTIP)
                    },
                    {
                        nameof(Db.GameplayEvents.MeteorShowerIronEvent),
                        new NotificationTexts(METEORSHOWERIRON.NAME, METEORSHOWERIRON.TOOLTIP)
                    },
                    {
                        nameof(Db.GameplayEvents.MeteorShowerCopperEvent),
                        new NotificationTexts(METEORSHOWERCOPPER.NAME, METEORSHOWERCOPPER.TOOLTIP)
                    },
                    {
                        nameof(Db.GameplayEvents.MeteorShowerGoldEvent),
                        new NotificationTexts(METEORSHOWERGOLD.NAME, METEORSHOWERGOLD.TOOLTIP)
                    },
                    {
                        nameof(Db.GameplayEvents.MeteorShowerDustEvent),
                        new NotificationTexts(METEORSHOWERDUST.NAME, METEORSHOWERDUST.TOOLTIP)
                    },
                    {
                        nameof(Db.GameplayEvents.MeteorShowerFullereneEvent),
                        new NotificationTexts(METEORSHOWERFULLERENE.NAME, METEORSHOWERFULLERENE.TOOLTIP)
                    },
                    {
                        nameof(Db.GameplayEvents.GassyMooteorEvent),
                        new NotificationTexts(METEORSHOWERGASSYMOO.NAME, METEORSHOWERGASSYMOO.TOOLTIP)
                    },
                };
            }
        }

        public readonly struct NotificationTexts
        {
            public readonly string Name;
            public readonly string Tooltip;

            public NotificationTexts(string name, string tooltip)
            {
                Name = name;
                Tooltip = tooltip;
            }
        }
    }
}