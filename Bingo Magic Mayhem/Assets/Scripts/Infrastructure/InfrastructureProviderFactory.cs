using System;

namespace BingoMagicMayhem.Infrastructure
{
    public sealed class InfrastructureProviderSet
    {
        public InfrastructureCompositionSnapshot Composition = new InfrastructureCompositionSnapshot();
        public IIdentityFacade Identity;
        public IAnalyticsFacade Analytics;
        public IProfileSettingsCloudSync ProfileCloudSync;
    }

    /// <summary>
    /// Centralizes provider selection so the composition root can stay focused on
    /// storage/journal/diagnostics assembly while adapter swap points remain isolated.
    /// </summary>
    public static class InfrastructureProviderFactory
    {
        public static InfrastructureProviderSet Create(
            JsonFileDurableStateStore durableState,
            JsonLinesActionJournal actionJournal,
            InfrastructureCompositionOptions options = null)
        {
            if (durableState == null)
            {
                throw new ArgumentNullException(nameof(durableState));
            }

            if (actionJournal == null)
            {
                throw new ArgumentNullException(nameof(actionJournal));
            }

            options ??= new InfrastructureCompositionOptions();

            InfrastructureCompositionSnapshot snapshot = new InfrastructureCompositionSnapshot
            {
                DesiredIdentityProvider = Describe(options.Identity),
                DesiredAnalyticsProvider = Describe(options.Analytics),
                DesiredProfileCloudSyncProvider = Describe(options.ProfileCloudSync),
                ActiveIdentityProvider = "local_guest",
                ActiveAnalyticsProvider = "local_journal",
                ActiveProfileCloudSyncProvider = "disabled_local",
                UgsAdaptersCompiled = UgsAdapterBoundary.IsAdapterCompiled(),
                UsesLocalFallback = false,
                FallbackReason = ""
            };

            IIdentityFacade identity = new LocalIdentityFacade(durableState);
            IAnalyticsFacade analytics = new LocalAnalyticsFacade(actionJournal, identity);
            IProfileSettingsCloudSync profileCloudSync = new DisabledProfileSettingsCloudSync();

            UgsAdapterProviderFactory.ApplyRequestedProviders(
                options,
                snapshot,
                ref identity,
                ref analytics,
                ref profileCloudSync);

            snapshot.UsesLocalFallback =
                snapshot.ActiveIdentityProvider != snapshot.DesiredIdentityProvider ||
                snapshot.ActiveAnalyticsProvider != snapshot.DesiredAnalyticsProvider ||
                snapshot.ActiveProfileCloudSyncProvider != snapshot.DesiredProfileCloudSyncProvider;

            if (snapshot.UsesLocalFallback)
            {
                snapshot.FallbackReason = snapshot.UgsAdaptersCompiled
                    ? "UGS-backed providers were requested, but runtime approval gates keep local-first providers active."
                    : "UGS-backed providers were requested, but the compile gate is absent so local-first providers remain active.";
            }

            return new InfrastructureProviderSet
            {
                Composition = snapshot,
                Identity = identity,
                Analytics = analytics,
                ProfileCloudSync = profileCloudSync
            };
        }

        private static string Describe(IdentityProviderPreference preference)
        {
            return preference == IdentityProviderPreference.UgsAnonymous ? "ugs_anonymous" : "local_guest";
        }

        private static string Describe(AnalyticsProviderPreference preference)
        {
            return preference == AnalyticsProviderPreference.UgsAnalytics ? "ugs_analytics" : "local_journal";
        }

        private static string Describe(ProfileCloudSyncPreference preference)
        {
            return preference == ProfileCloudSyncPreference.UgsCloudSave ? "ugs_cloud_save" : "disabled_local";
        }
    }
}
