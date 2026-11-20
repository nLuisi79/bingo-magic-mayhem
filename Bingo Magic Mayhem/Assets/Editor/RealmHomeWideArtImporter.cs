using UnityEditor;

// Import settings for wide realm-home "bleed" backgrounds (Resources/prototype/ui/realm_home/<realm>/<realm>-realm-<state>-wide.png).
// They are 2640x1620 px, so the default 2048 max size and power-of-two rescaling would shrink or distort them.
// See Docs/REALM_HOME_WIDE_ART_SPEC.md.
public sealed class RealmHomeWideArtImporter : AssetPostprocessor
{
    private const string RealmHomeArtFolder = "Assets/Resources/prototype/ui/realm_home/";

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(RealmHomeArtFolder) || !assetPath.EndsWith("-wide.png"))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Default;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.maxTextureSize = 4096;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = false;
    }
}
