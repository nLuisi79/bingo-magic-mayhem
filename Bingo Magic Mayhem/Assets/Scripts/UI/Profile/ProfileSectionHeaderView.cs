using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class ProfileSectionHeaderView : MonoBehaviour
    {
        [SerializeField] private Text titleText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
        }

        public void Initialize(Text title)
        {
            titleText = title;
        }

        public void Apply(ProfileSectionHeaderDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.TitleText;
            }
        }
    }
}
